using AutoMapper;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.MaintenancePaperWidth;
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
    public class MaintenancePaperWidthService : IMaintenancePaperWidthService
    {
        IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper mapper;
        private readonly IPaperWidthAPIRepository _PaperWidthAPIRepository;

        private readonly string _username;
        private readonly string _saleOrg;
        private readonly string _factoryCode;
        private readonly string _token;

        public MaintenancePaperWidthService(IHttpContextAccessor httpContextAccessor,
            IPaperWidthAPIRepository PaperWidthAPIRepository,
            IMapper mapper)
        {
            // Initialize HttpContext
            _httpContextAccessor = httpContextAccessor;

            // Initialize Repository
            _PaperWidthAPIRepository = PaperWidthAPIRepository;
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

        public void GetPaperWidth(MaintenancePaperWidthViewModel maintenancePaperWidthViewModel)
        {
            // Convert Json String to List Object
            var PaperWidthList = JsonConvert.DeserializeObject<List<PaperWidth>>(_PaperWidthAPIRepository.GetPaperWidthList(_factoryCode, _token));

            var PaperWidthModelViewList = mapper.Map<List<PaperWidth>, List<PaperWidthViewModel>>(PaperWidthList);

            maintenancePaperWidthViewModel.PaperWidthViewModelList = PaperWidthModelViewList;
            ////////////////////////////////////////////////////////////////////////////////////////////////////

        }

        public void SavePaperWidth(MaintenancePaperWidthViewModel model)
        {
            ParentModel PaperWidthModel = new ParentModel();
            PaperWidthModel.AppName = Globals.AppNameEncrypt;
            PaperWidthModel.FactoryCode = _factoryCode;
            PaperWidthModel.PlantCode = _factoryCode;

            //model.PaperWidthViewModel.PaperWidthStatus = true;
            model.PaperWidthViewModel.FactoryCode = _factoryCode;

            PaperWidthModel.PaperWidth = mapper.Map<PaperWidthViewModel, PaperWidth>(model.PaperWidthViewModel);
            PaperWidthModel.PaperWidth.CreatedBy = _username;
            PaperWidthModel.PaperWidth.CreatedDate = DateTime.Now;

            string PaperWidthListJsonString = JsonConvert.SerializeObject(PaperWidthModel);

            _PaperWidthAPIRepository.SavePaperWidth(_factoryCode, PaperWidthListJsonString, _token);
        }


        public void UpdatePaperWidth(PaperWidthViewModel PaperWidthViewModel)
        {
            ParentModel PaperWidthModel = new ParentModel();
            PaperWidthModel.AppName = Globals.AppNameEncrypt;
            PaperWidthModel.FactoryCode = _factoryCode;
            PaperWidthModel.PlantCode = _factoryCode;

            //PaperWidthViewModel.PaperWidthStatus = true;
            PaperWidthViewModel.FactoryCode = _factoryCode;

            PaperWidthModel.PaperWidth = mapper.Map<PaperWidthViewModel, PaperWidth>(PaperWidthViewModel);
            PaperWidthModel.PaperWidth.CreatedBy = PaperWidthViewModel.CreatedBy;
            PaperWidthModel.PaperWidth.CreatedDate = PaperWidthViewModel.CreatedDate;
            PaperWidthModel.PaperWidth.UpdatedBy = _username;
            PaperWidthModel.PaperWidth.UpdatedDate = DateTime.Now;

            string jsonString = JsonConvert.SerializeObject(PaperWidthModel);

            _PaperWidthAPIRepository.UpdatePaperWidth(_factoryCode, jsonString, _token);
        }


    }
}
