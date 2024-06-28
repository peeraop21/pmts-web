
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.MaintenancePMTsConfig;
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
    public class MaintenancePMTsConfigService : IMaintenancePMTsConfigService
    {
        IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper mapper;
        private readonly IPMTsConfigAPIRepository _PMTsConfigAPIRepository;

        private readonly string _username;
        private readonly string _saleOrg;
        private readonly string _factoryCode;
        private readonly string _token;

        public MaintenancePMTsConfigService(IHttpContextAccessor httpContextAccessor,
            IPMTsConfigAPIRepository PMTsConfigAPIRepository,
            IMapper mapper)
        {
            // Initialize HttpContext
            _httpContextAccessor = httpContextAccessor;

            // Initialize Repository
            _PMTsConfigAPIRepository = PMTsConfigAPIRepository;
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

        public void GetPMTsConfig(ref MaintenancePMTsConfigViewModel maintenancePMTsConfigViewModel)
        {
            // Convert Json String to List Object
            var PMTsConfigList = JsonConvert.DeserializeObject<List<PmtsConfig>>(_PMTsConfigAPIRepository.GetPMTsConfigList(_factoryCode, _token));

            var PMTsConfigModelViewList = mapper.Map<List<PmtsConfig>, List<PMTsConfigViewModel>>(PMTsConfigList);

            maintenancePMTsConfigViewModel.PMTsConfigViewModelList = PMTsConfigModelViewList;
            ////////////////////////////////////////////////////////////////////////////////////////////////////

        }

        public void SavePMTsConfig(MaintenancePMTsConfigViewModel model)
        {
            ParentModel PMTsConfigModel = new ParentModel();
            PMTsConfigModel.AppName = Globals.AppNameEncrypt;
            PMTsConfigModel.FactoryCode = _factoryCode;
            PMTsConfigModel.PlantCode = _factoryCode;

            //model.PMTsConfigViewModel.PMTsConfigStatus = true;
            model.PMTsConfigViewModel.FactoryCode = _factoryCode;

            PMTsConfigModel.PmtsConfig = mapper.Map<PMTsConfigViewModel, PmtsConfig>(model.PMTsConfigViewModel);
            PMTsConfigModel.PmtsConfig.CreatedBy = _username;
            PMTsConfigModel.PmtsConfig.CreatedDate = DateTime.Now;

            string PMTsConfigListJsonString = JsonConvert.SerializeObject(PMTsConfigModel);

            _PMTsConfigAPIRepository.SavePMTsConfig(_factoryCode, PMTsConfigListJsonString, _token);
        }


        public void UpdatePMTsConfig(PMTsConfigViewModel PMTsConfigViewModel)
        {
            ParentModel PMTsConfigModel = new ParentModel();
            PMTsConfigModel.AppName = Globals.AppNameEncrypt;
            PMTsConfigModel.FactoryCode = _factoryCode;
            PMTsConfigModel.PlantCode = _factoryCode;

            //PMTsConfigViewModel.PMTsConfigStatus = true;
            PMTsConfigViewModel.FactoryCode = _factoryCode;

            PMTsConfigModel.PmtsConfig = mapper.Map<PMTsConfigViewModel, PmtsConfig>(PMTsConfigViewModel);
            PMTsConfigModel.PmtsConfig.UpdatedBy = _username;
            PMTsConfigModel.PmtsConfig.UpdatedDate = DateTime.Now;

            string jsonString = JsonConvert.SerializeObject(PMTsConfigModel);

            _PMTsConfigAPIRepository.UpdatePMTsConfig(_factoryCode, jsonString, _token);
        }


    }
}
