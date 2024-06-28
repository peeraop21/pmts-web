using AutoMapper;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.MaintenanceAllowance;
using PMTs.DataAccess.Repository;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Shared;
using PMTs.DataAccess.Tracing;
using PMTs.WebApplication.AutoMapper;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMTs.WebApplication.Services
{
    [TraceAspect]
    public class MaintenanceAllowanceService : IMaintenanceAllowanceService
    {
        IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper mapper;
        private readonly IAllowanceProcessAPIRepository _allowanceProcessAPIRepository;

        private readonly string _username;
        private readonly string _saleOrg;
        private readonly string _factoryCode;
        private string _token;


        public MaintenanceAllowanceService(IHttpContextAccessor httpContextAccessor,
            IAllowanceProcessAPIRepository allowanceProcessAPIRepository,
            IMapper mapper)
        {
            // Initialize HttpContext
            _httpContextAccessor = httpContextAccessor;

            // Initialize Repository
            _allowanceProcessAPIRepository = allowanceProcessAPIRepository;
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

        public void GetAllowance(MaintenanceAllowanceViewModel maintenanceAllowanceViewModel)
        {
            // Convert Json String to List Object
            var AllowanceList = JsonConvert.DeserializeObject<List<AllowanceProcess>>(_allowanceProcessAPIRepository.GetAllowanceProcessByFactoryCode(_factoryCode, _token));

            var AllowanceModelViewList = mapper.Map<List<AllowanceProcess>, List<AllowanceViewModel>>(AllowanceList);

            maintenanceAllowanceViewModel.AllowanceViewModelList = AllowanceModelViewList;
            ////////////////////////////////////////////////////////////////////////////////////////////////////

        }

        public void SaveAllowance(MaintenanceAllowanceViewModel model)
        {

            ParentModel AllowanceModel = new ParentModel();
            AllowanceModel.AppName = Globals.AppNameEncrypt;
            AllowanceModel.FactoryCode = _factoryCode;
            AllowanceModel.PlantCode = _factoryCode;

            //model.AllowanceViewModel.AllowanceStatus = true;
            model.AllowanceViewModel.FactoryCode = _factoryCode;

            AllowanceModel.AllowanceProcess = mapper.Map<AllowanceViewModel, AllowanceProcess>(model.AllowanceViewModel);
            AllowanceModel.AllowanceProcess.CreatedDate = DateTime.Now;
            AllowanceModel.AllowanceProcess.CreatedBy = _username;

            string AllowanceListJsonString = JsonConvert.SerializeObject(AllowanceModel);

            _allowanceProcessAPIRepository.SaveAllowanceProcess(_factoryCode, AllowanceListJsonString, _token);
        }


        public void UpdateAllowance(AllowanceViewModel AllowanceViewModel)
        {
            ParentModel AllowanceModel = new ParentModel();
            AllowanceModel.AppName = Globals.AppNameEncrypt;
            AllowanceModel.FactoryCode = _factoryCode;
            AllowanceModel.PlantCode = _factoryCode;

            //AllowanceViewModel.AllowanceStatus = true;
            AllowanceViewModel.FactoryCode = _factoryCode;

            AllowanceModel.AllowanceProcess = mapper.Map<AllowanceViewModel, AllowanceProcess>(AllowanceViewModel);
            AllowanceModel.AllowanceProcess.UpdatedDate = DateTime.Now;
            AllowanceModel.AllowanceProcess.UpdatedBy = _username;
            AllowanceModel.AllowanceProcess.CreatedBy = AllowanceViewModel.CreatedBy;
            AllowanceModel.AllowanceProcess.CreatedDate = AllowanceViewModel.CreatedDate;

            string jsonString = JsonConvert.SerializeObject(AllowanceModel);

            _allowanceProcessAPIRepository.UpdateAllowanceProcess(_factoryCode, jsonString, _token);
        }


        public void DeleteAllowance(int Id)
        {
            var AllowanceData = JsonConvert.DeserializeObject<AllowanceProcess>(_allowanceProcessAPIRepository.GetAllowanceById(Id, _token));

            //AllowanceData.AllowanceStatus = false;

            ParentModel AllowanceParent = new ParentModel();
            AllowanceParent.AppName = Globals.AppNameEncrypt;
            AllowanceParent.SaleOrg = _saleOrg;
            AllowanceParent.PlantCode = _factoryCode;
            AllowanceParent.AllowanceProcess = AllowanceData;
            string jsonString = JsonConvert.SerializeObject(AllowanceParent);

            _allowanceProcessAPIRepository.UpdateAllowanceProcess(_factoryCode, jsonString, _token);

            //return AllowanceData.MaterialNo;
        }

    }
}
