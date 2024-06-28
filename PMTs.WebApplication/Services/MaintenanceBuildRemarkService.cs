using AutoMapper;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.MaintenanceBuildRemark;
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
    public class MaintenanceBuildRemarkService : IMaintenanceBuildRemarkService
    {
        IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper mapper;
        private readonly IBuildRemarkAPIRepository _buildRemarkAPIRepository;

        private readonly string _username;
        private readonly string _saleOrg;
        private readonly string _factoryCode;
        private readonly string _token;

        public MaintenanceBuildRemarkService(IHttpContextAccessor httpContextAccessor,
            IBuildRemarkAPIRepository buildRemarkAPIRepository,
            IMapper mapper)
        {
            // Initialize HttpContext
            _httpContextAccessor = httpContextAccessor;

            // Initialize Repository
            _buildRemarkAPIRepository = buildRemarkAPIRepository;
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

        public void GetBuildRemark(MaintenanceBuildRemarkViewModel maintenanceBuildRemarkViewModel)
        {
            // Convert Json String to List Object
            var BuildRemarkList = JsonConvert.DeserializeObject<List<BuildRemark>>(_buildRemarkAPIRepository.GetBuildRemarkList(_factoryCode, _token));

            var BuildRemarkModelViewList = mapper.Map<List<BuildRemark>, List<BuildRemarkViewModel>>(BuildRemarkList);

            maintenanceBuildRemarkViewModel.BuildRemarkViewModelList = BuildRemarkModelViewList;
            ////////////////////////////////////////////////////////////////////////////////////////////////////

        }

        public void SaveBuildRemark(MaintenanceBuildRemarkViewModel model)
        {
            ParentModel BuildRemarkModel = new ParentModel();
            BuildRemarkModel.AppName = Globals.AppNameEncrypt;
            BuildRemarkModel.FactoryCode = _factoryCode;
            BuildRemarkModel.PlantCode = _factoryCode;

            //model.BuildRemarkViewModel.BuildRemarkStatus = true;
            model.BuildRemarkViewModel.FactoryCode = _factoryCode;

            BuildRemarkModel.BuildRemark = mapper.Map<BuildRemarkViewModel, BuildRemark>(model.BuildRemarkViewModel);
            BuildRemarkModel.BuildRemark.CreatedBy = _username;
            BuildRemarkModel.BuildRemark.CreatedDate = DateTime.Now;


            string BuildRemarkListJsonString = JsonConvert.SerializeObject(BuildRemarkModel);

            _buildRemarkAPIRepository.SaveBuildRemark(_factoryCode, BuildRemarkListJsonString, _token);
        }


        public void UpdateBuildRemark(BuildRemarkViewModel BuildRemarkViewModel)
        {
            ParentModel BuildRemarkModel = new ParentModel();
            BuildRemarkModel.AppName = Globals.AppNameEncrypt;
            BuildRemarkModel.FactoryCode = _factoryCode;
            BuildRemarkModel.PlantCode = _factoryCode;

            //BuildRemarkViewModel.BuildRemarkStatus = true;
            BuildRemarkViewModel.FactoryCode = _factoryCode;

            BuildRemarkModel.BuildRemark = mapper.Map<BuildRemarkViewModel, BuildRemark>(BuildRemarkViewModel);
            BuildRemarkModel.BuildRemark.UpdatedBy = _username;
            BuildRemarkModel.BuildRemark.UpdatedDate = DateTime.Now;

            string jsonString = JsonConvert.SerializeObject(BuildRemarkModel);

            _buildRemarkAPIRepository.UpdateBuildRemark(_factoryCode, jsonString, _token);
        }


    }
}
