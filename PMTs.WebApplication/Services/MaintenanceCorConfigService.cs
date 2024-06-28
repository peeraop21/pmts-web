using AutoMapper;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.MaintenanceCorConfig;
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
    public class MaintenanceCorConfigService : IMaintenanceCorConfigService
    {
        IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper mapper;
        private readonly ICorConfigAPIRepository _CorConfigAPIRepository;

        private readonly string _username;
        private readonly string _saleOrg;
        private readonly string _factoryCode;
        private readonly string _token;

        public MaintenanceCorConfigService(IHttpContextAccessor httpContextAccessor,
            ICorConfigAPIRepository CorConfigAPIRepository,
            IMapper mapper)
        {
            // Initialize HttpContext
            _httpContextAccessor = httpContextAccessor;

            // Initialize Repository
            _CorConfigAPIRepository = CorConfigAPIRepository;
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

        public void GetCorConfig(MaintenanceCorConfigViewModel maintenanceCorConfigViewModel)
        {
            // Convert Json String to List Object
            var CorConfigList = JsonConvert.DeserializeObject<List<CorConfig>>(_CorConfigAPIRepository.GetCorConfigList(_factoryCode, _token));

            var CorConfigModelViewList = mapper.Map<List<CorConfig>, List<CorConfigViewModel>>(CorConfigList);

            maintenanceCorConfigViewModel.CorConfigViewModelList = CorConfigModelViewList;
            ////////////////////////////////////////////////////////////////////////////////////////////////////

        }

        public void SaveCorConfig(MaintenanceCorConfigViewModel model)
        {
            ParentModel CorConfigModel = new ParentModel();
            CorConfigModel.AppName = Globals.AppNameEncrypt;
            CorConfigModel.FactoryCode = _factoryCode;
            CorConfigModel.PlantCode = _factoryCode;

            //model.CorConfigViewModel.CorConfigStatus = true;
            model.CorConfigViewModel.FactoryCode = _factoryCode;

            CorConfigModel.CorConfig = mapper.Map<CorConfigViewModel, CorConfig>(model.CorConfigViewModel);
            CorConfigModel.CorConfig.CreatedBy = _username;
            CorConfigModel.CorConfig.CreatedDate = DateTime.Now;

            string CorConfigListJsonString = JsonConvert.SerializeObject(CorConfigModel);

            _CorConfigAPIRepository.SaveCorConfig(_factoryCode, CorConfigListJsonString, _token);
        }


        public void UpdateCorConfig(CorConfigViewModel CorConfigViewModel)
        {
            ParentModel CorConfigModel = new ParentModel();
            CorConfigModel.AppName = Globals.AppNameEncrypt;
            CorConfigModel.FactoryCode = _factoryCode;
            CorConfigModel.PlantCode = _factoryCode;

            //CorConfigViewModel.CorConfigStatus = true;
            CorConfigViewModel.FactoryCode = _factoryCode;

            CorConfigModel.CorConfig = mapper.Map<CorConfigViewModel, CorConfig>(CorConfigViewModel);
            CorConfigModel.CorConfig.UpdatedBy = _username;
            CorConfigModel.CorConfig.UpdatedDate = DateTime.Now;

            string jsonString = JsonConvert.SerializeObject(CorConfigModel);

            _CorConfigAPIRepository.UpdateCorConfig(_factoryCode, jsonString, _token);
        }


    }
}
