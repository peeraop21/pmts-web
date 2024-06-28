using AutoMapper;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.MaintenanceJoint;
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
using JointViewModel = PMTs.DataAccess.ModelView.MaintenanceJoint.JointViewModel;

namespace PMTs.WebApplication.Services
{
    [TraceAspect]
    public class MaintenanceJointService : IMaintenanceJointService
    {
        IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper mapper;
        private readonly IJoinAPIRepository _JointAPIRepository;

        private readonly string _username;
        private readonly string _saleOrg;
        private readonly string _factoryCode;
        private readonly string _token;

        public MaintenanceJointService(IHttpContextAccessor httpContextAccessor,
            IJoinAPIRepository JointAPIRepository,
            IMapper mapper)
        {
            // Initialize HttpContext
            _httpContextAccessor = httpContextAccessor;

            // Initialize Repository
            _JointAPIRepository = JointAPIRepository;
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

        public void GetJoint(MaintenanceJointViewModel maintenanceJointViewModel)
        {
            // Convert Json String to List Object
            var JointList = JsonConvert.DeserializeObject<List<Joint>>(_JointAPIRepository.GetJoinList(_factoryCode, _token));

            var JointModelViewList = mapper.Map<List<Joint>, List<JointViewModel>>(JointList);

            maintenanceJointViewModel.JointViewModelList = JointModelViewList;
            ////////////////////////////////////////////////////////////////////////////////////////////////////

        }

        public void SaveJoint(MaintenanceJointViewModel maintenanceJointViewModel)
        {
            ParentModel JointModel = new ParentModel();
            JointModel.AppName = Globals.AppNameEncrypt;
            JointModel.FactoryCode = _factoryCode;
            JointModel.PlantCode = _factoryCode;

            //model.JointViewModel.JointStatus = true;
            //model.JointViewModel.FactoryCode = _factoryCode;

            JointModel.Joint = mapper.Map<JointViewModel, Joint>(maintenanceJointViewModel.JointViewModel);
            JointModel.Joint.JointDescription = JointModel.Joint.JointName;
            JointModel.Joint.CreatedBy = _username;
            JointModel.Joint.CreatedDate = DateTime.Now;

            string JointListJsonString = JsonConvert.SerializeObject(JointModel);

            _JointAPIRepository.SaveJoin(_factoryCode, JointListJsonString, _token);
        }


        public void UpdateJoint(JointViewModel maintenanceJointViewModel)
        {
            ParentModel JointModel = new ParentModel();
            JointModel.AppName = Globals.AppNameEncrypt;
            JointModel.FactoryCode = _factoryCode;
            JointModel.PlantCode = _factoryCode;

            //JointViewModel.JointStatus = true;
            //JointViewModel.FactoryCode = _factoryCode;

            JointModel.Joint = mapper.Map<JointViewModel, Joint>(maintenanceJointViewModel);
            JointModel.Joint.JointDescription = JointModel.Joint.JointName;
            JointModel.Joint.CreatedBy = maintenanceJointViewModel.CreatedBy;
            JointModel.Joint.CreatedDate = maintenanceJointViewModel.CreatedDate;
            JointModel.Joint.UpdatedBy = _username;
            JointModel.Joint.UpdatedDate = DateTime.Now;

            string jsonString = JsonConvert.SerializeObject(JointModel);

            _JointAPIRepository.UpdateJoin(_factoryCode, jsonString, _token);
        }


    }
}
