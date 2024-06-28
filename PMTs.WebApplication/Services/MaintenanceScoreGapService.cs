using AutoMapper;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.MaintenanceScoreGap;
using PMTs.DataAccess.Repository;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Shared;
using PMTs.DataAccess.Tracing;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMTs.WebApplication.Services
{
    [TraceAspect]
    public class MaintenanceScoreGapService : IMaintenanceScoreGapService
    {
        IHttpContextAccessor _httpContextAccessor;

        private readonly IScoreGapAPIRepository _ScoreGapAPIRepository;
        private readonly IScoreTypeAPIRepository _ScoreTypeAPIRepository;
        private readonly IMapper mapper;
        private readonly string _username;
        private readonly string _saleOrg;
        private readonly string _factoryCode;
        private readonly string _token;

        public MaintenanceScoreGapService(IHttpContextAccessor httpContextAccessor,
            IScoreGapAPIRepository ScoreGapAPIRepository,
            IScoreTypeAPIRepository ScoreTypeAPIRepository,
            IMapper mapper)


        {
            // Initialize HttpContext
            _httpContextAccessor = httpContextAccessor;

            // Initialize Repository
            _ScoreGapAPIRepository = ScoreGapAPIRepository;
            _ScoreTypeAPIRepository = ScoreTypeAPIRepository;
            // Initialize SaleOrg and PlantCode from Session
            var userSessionModel = SessionExtentions.GetSession<UserSessionModel>(_httpContextAccessor.HttpContext.Session, "UserSessionModel");
            if (userSessionModel != null)
            {
                _username = userSessionModel.UserName;
                _saleOrg = userSessionModel.SaleOrg;
                _factoryCode = userSessionModel.FactoryCode;
                _token = userSessionModel.Token;
            }

            this.mapper = mapper;
        }

        public void GetScoreGap(MaintenanceScoreGapViewModel maintenanceScoreGapViewModel)
        {
            // Convert Json String to List Object
            var ScoreGapList = JsonConvert.DeserializeObject<List<ScoreGap>>(_ScoreGapAPIRepository.GetScoreGapList(_factoryCode, _token));

            var ScoreGapModelViewList = mapper.Map<List<ScoreGap>, List<ScoreGapViewModel>>(ScoreGapList);


            var ScoreTypeList = JsonConvert.DeserializeObject<List<ScoreType>>(_ScoreTypeAPIRepository.GetScoreTypeList(_factoryCode, _token));

            //for (int i = 0; i < ScoreGapModelViewList.Count; i++)
            //{
            //    for (int l = 0; l < ScoreTypeList.Count; l++)
            //    {
            //        if (ScoreGapList[i].ScoreType == ScoreTypeList[l].ScoreTypeId)
            //        {
            //            ScoreGapModelViewList[i].ScoreType = ScoreTypeList[l].ScoreTypeName;
            //        }
            //    }
            //}
            maintenanceScoreGapViewModel.ScoreTypeList = new List<ScoreType>();
            maintenanceScoreGapViewModel.ScoreTypeList = ScoreTypeList;
            maintenanceScoreGapViewModel.ScoreGapViewModelList = ScoreGapModelViewList;
            ////////////////////////////////////////////////////////////////////////////////////////////////////

        }

        public void SaveScoreGap(MaintenanceScoreGapViewModel model)
        {
            ParentModel ScoreGapModel = new ParentModel();
            ScoreGapModel.AppName = Globals.AppNameEncrypt;
            ScoreGapModel.FactoryCode = _factoryCode;
            ScoreGapModel.PlantCode = _factoryCode;

            //model.ScoreGapViewModel.ScoreGapStatus = true;
            model.ScoreGapViewModel.FactoryCode = _factoryCode;

            ScoreGapModel.ScoreGap = mapper.Map<ScoreGapViewModel, ScoreGap>(model.ScoreGapViewModel);
            ScoreGapModel.ScoreGap.CreatedBy = _username;
            ScoreGapModel.ScoreGap.CreatedDate = DateTime.Now;
            ScoreGapModel.ScoreGap.Id = 0;

            string ScoreGapListJsonString = JsonConvert.SerializeObject(ScoreGapModel);

            _ScoreGapAPIRepository.SaveScoreGap(_factoryCode, ScoreGapListJsonString, _token);
        }


        public void UpdateScoreGap(ScoreGapViewModel ScoreGapViewModel)
        {
            ParentModel ScoreGapModel = new ParentModel();
            ScoreGapModel.AppName = Globals.AppNameEncrypt;
            ScoreGapModel.FactoryCode = _factoryCode;
            ScoreGapModel.PlantCode = _factoryCode;

            //ScoreGapViewModel.ScoreGapStatus = true;
            ScoreGapViewModel.FactoryCode = _factoryCode;

            ScoreGapModel.ScoreGap = mapper.Map<ScoreGapViewModel, ScoreGap>(ScoreGapViewModel);
            ScoreGapModel.ScoreGap.CreatedBy = ScoreGapViewModel.CreatedBy;
            ScoreGapModel.ScoreGap.CreatedDate = ScoreGapViewModel.CreatedDate;
            ScoreGapModel.ScoreGap.UpdatedBy = _username;
            ScoreGapModel.ScoreGap.UpdatedDate = DateTime.Now;

            string jsonString = JsonConvert.SerializeObject(ScoreGapModel);

            _ScoreGapAPIRepository.UpdateScoreGap(_factoryCode, jsonString, _token);
        }


    }
}
