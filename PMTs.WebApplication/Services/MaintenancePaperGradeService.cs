using AutoMapper;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.MaintenancePaperGrade;
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
    public class MaintenancePaperGradeService : IMaintenancePaperGradeService
    {
        IHttpContextAccessor _httpContextAccessor;

        private readonly IPaperGradeAPIRepository _PaperGradeAPIRepository;

        private readonly string _username;
        private readonly string _saleOrg;
        private readonly string _factoryCode;
        private readonly string _token;

        public MaintenancePaperGradeService(IHttpContextAccessor httpContextAccessor, IPaperGradeAPIRepository PaperGradeAPIRepository)
        {
            // Initialize HttpContext
            _httpContextAccessor = httpContextAccessor;

            // Initialize Repository
            _PaperGradeAPIRepository = PaperGradeAPIRepository;
            // Initialize SaleOrg and PlantCode from Session
            var userSessionModel = SessionExtentions.GetSession<UserSessionModel>(_httpContextAccessor.HttpContext.Session, "UserSessionModel");
            if (userSessionModel != null)
            {
                _username = userSessionModel.UserName;
                _saleOrg = userSessionModel.SaleOrg;
                _factoryCode = userSessionModel.FactoryCode;
                _token = userSessionModel.Token;
            }
        }

        public void GetPaperGrade(MaintenancePaperGradeViewModel maintenancePaperGradeViewModel)
        {
            // Convert Json String to List Object
            maintenancePaperGradeViewModel.PaperGradeViewModelList = JsonConvert.DeserializeObject<List<PaperGradeViewModel>>(_PaperGradeAPIRepository.GetPaperGradesWithGradeCodeMachine(_factoryCode, _token));

            //var PaperGradeModelViewList = Mapper.Map<List<PaperGrade>, List<PaperGradeViewModel>>(PaperGradeList);

            //maintenancePaperGradeViewModel.PaperGradeViewModelList = PaperGradeModelViewList;

        }

        public void SavePaperGrade(MaintenancePaperGradeViewModel model)
        {
            var PaperGradeList = JsonConvert.DeserializeObject<List<PaperGrade>>(_PaperGradeAPIRepository.GetPaperGradeList(_factoryCode, _token));

            var paperId = PaperGradeList.Max(x => x.Id);
            //ParentModel PaperGradeModel = new ParentModel();
            //PaperGradeModel.AppName = Globals.AppNameEncrypt;
            //PaperGradeModel.FactoryCode = _factoryCode;
            //PaperGradeModel.PlantCode = _factoryCode;
            //PaperGradeModel.PaperGrade = Mapper.Map<PaperGradeViewModel, PaperGrade>(model.PaperGradeViewModel);
            //PaperGradeModel.PaperGrade.CreatedDate = DateTime.Now;
            //PaperGradeModel.PaperGrade.CreatedBy = _username;
            //PaperGradeModel.PaperGrade.PaperId = paperId + 1;
            model.PaperGradeViewModel.CreatedDate = DateTime.Now;
            model.PaperGradeViewModel.CreatedBy = _username;
            model.PaperGradeViewModel.PaperId = paperId + 1;
            string jsonString = JsonConvert.SerializeObject(model.PaperGradeViewModel);

            var existPaperGrade = JsonConvert.DeserializeObject<PaperGrade>(_PaperGradeAPIRepository.GetPaperGradeByGrade(_factoryCode, model.PaperGradeViewModel.Grade, _token)) == null ? false : true;
            if (!existPaperGrade)
            {
                //_PaperGradeAPIRepository.SavePaperGrade(PaperGradeListJsonString, _token);
                _PaperGradeAPIRepository.SavePaperGradeWithGradeCodeMachine(_factoryCode, jsonString, _token);
            }
            else
            {
                throw new Exception($"Can't save exist grade ({model.PaperGradeViewModel.Grade}).");
            }
        }


        public void UpdatePaperGrade(PaperGradeViewModel PaperGradeViewModel)
        {
            //ParentModel PaperGradeModel = new ParentModel();
            //PaperGradeModel.AppName = Globals.AppNameEncrypt;
            //PaperGradeModel.FactoryCode = _factoryCode;
            //PaperGradeModel.PlantCode = _factoryCode;

            //PaperGradeModel.PaperGrade = Mapper.Map<PaperGradeViewModel, PaperGrade>(PaperGradeViewModel);
            PaperGradeViewModel.UpdatedDate = DateTime.Now;
            PaperGradeViewModel.UpdatedBy = _username;
            PaperGradeViewModel.CreatedDate = PaperGradeViewModel.CreatedDate;
            PaperGradeViewModel.CreatedBy = PaperGradeViewModel.CreatedBy;

            string jsonString = JsonConvert.SerializeObject(PaperGradeViewModel);

            _PaperGradeAPIRepository.UpdatePaperGradeWithGradeCodeMachine(_factoryCode, jsonString, _token);
        }


    }
}
