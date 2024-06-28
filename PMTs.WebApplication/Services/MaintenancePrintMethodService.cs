using AutoMapper;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.MaintenancePrintMethod;
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
using PrintMethodViewModel = PMTs.DataAccess.ModelView.MaintenancePrintMethod.PrintMethodViewModel;

namespace PMTs.WebApplication.Services
{
    [TraceAspect]
    public class MaintenancePrintMethodService : IMaintenancePrintMethodService
    {
        IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper mapper;
        private readonly IPrintMethodAPIRepository _PrintMethodAPIRepository;

        private readonly string _username;
        private readonly string _saleOrg;
        private readonly string _factoryCode;
        private readonly string _token;

        public MaintenancePrintMethodService(IHttpContextAccessor httpContextAccessor,
            IPrintMethodAPIRepository PrintMethodAPIRepository,
            IMapper mapper)
        {
            // Initialize HttpContext
            _httpContextAccessor = httpContextAccessor;

            // Initialize Repository
            _PrintMethodAPIRepository = PrintMethodAPIRepository;
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

        public void GetPrintMethod(MaintenancePrintMethodViewModel maintenancePrintMethodViewModel)
        {
            // Convert Json String to List Object
            var PrintMethodList = JsonConvert.DeserializeObject<List<PrintMethod>>(_PrintMethodAPIRepository.GetPrintMethodList(_factoryCode, _token));

            var PrintMethodModelViewList = mapper.Map<List<PrintMethod>, List<PrintMethodViewModel>>(PrintMethodList);

            maintenancePrintMethodViewModel.PrintMethodViewModelList = PrintMethodModelViewList;
            ////////////////////////////////////////////////////////////////////////////////////////////////////

        }

        public void SavePrintMethod(MaintenancePrintMethodViewModel maintenancePrintMethodViewModel)
        {

            ParentModel PrintMethodModel = new ParentModel();
            PrintMethodModel.AppName = Globals.AppNameEncrypt;
            PrintMethodModel.FactoryCode = _factoryCode;
            PrintMethodModel.PlantCode = _factoryCode;

            //model.PrintMethodViewModel.PrintMethodStatus = true;
            //model.PrintMethodViewModel.FactoryCode = _factoryCode;

            PrintMethodModel.PrintMethod = mapper.Map<PrintMethodViewModel, PrintMethod>(maintenancePrintMethodViewModel.PrintMethodViewModel);
            PrintMethodModel.PrintMethod.CreatedBy = _username;
            PrintMethodModel.PrintMethod.CreatedDate = DateTime.Now;

            string PrintMethodListJsonString = JsonConvert.SerializeObject(PrintMethodModel);

            _PrintMethodAPIRepository.SavePrintMethod(_factoryCode, PrintMethodListJsonString, _token);
        }


        public void UpdatePrintMethod(PrintMethodViewModel maintenancePrintMethodViewModel)
        {
            ParentModel PrintMethodModel = new ParentModel();
            PrintMethodModel.AppName = Globals.AppNameEncrypt;
            PrintMethodModel.FactoryCode = _factoryCode;
            PrintMethodModel.PlantCode = _factoryCode;

            //PrintMethodViewModel.PrintMethodStatus = true;
            //PrintMethodViewModel.FactoryCode = _factoryCode;

            PrintMethodModel.PrintMethod = mapper.Map<PrintMethodViewModel, PrintMethod>(maintenancePrintMethodViewModel);
            PrintMethodModel.PrintMethod.CreatedBy = maintenancePrintMethodViewModel.CreatedBy;
            PrintMethodModel.PrintMethod.CreatedDate = maintenancePrintMethodViewModel.CreatedDate;
            PrintMethodModel.PrintMethod.UpdatedBy = _username;
            PrintMethodModel.PrintMethod.UpdatedDate = DateTime.Now;

            string jsonString = JsonConvert.SerializeObject(PrintMethodModel);

            _PrintMethodAPIRepository.UpdatePrintMethod(_factoryCode, jsonString, _token);
        }


    }
}
