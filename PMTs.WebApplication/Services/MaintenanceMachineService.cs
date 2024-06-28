using AutoMapper;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.MaintenanceMachine;
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
    public class MaintenanceMachineService : IMaintenanceMachineService
    {
        IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper mapper;
        private readonly IMachineAPIRepository _machineAPIRepository;
        private readonly IMachineGroupAPIRepository _machineGroupAPIRepository;
        private readonly IJoinAPIRepository _joinAPIRepository;

        private readonly string _username;
        private readonly string _saleOrg;
        private readonly string _factoryCode;
        private readonly string _token;

        public MaintenanceMachineService(IHttpContextAccessor httpContextAccessor,
            IMachineAPIRepository machineAPIRepository,
            IMachineGroupAPIRepository machineGroupAPIRepository,
            IJoinAPIRepository joinAPIRepository,
            IMapper mapper)
        {
            // Initialize HttpContext
            _httpContextAccessor = httpContextAccessor;

            // Initialize Repository
            _machineAPIRepository = machineAPIRepository;
            _machineGroupAPIRepository = machineGroupAPIRepository;
            _joinAPIRepository = joinAPIRepository;
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

        public void GetMachine(MaintenanceMachineViewModel maintenanceMachineViewModel)
        {
            // Convert Json String to List Object
            var MachineList = JsonConvert.DeserializeObject<List<Machine>>(_machineAPIRepository.GetMachineList(_factoryCode, _token));

            var MachineModelViewList = mapper.Map<List<Machine>, List<MachineViewModel>>(MachineList);

            var MachineGroupL = JsonConvert.DeserializeObject<List<MachineGroup>>(_machineGroupAPIRepository.GetMachineGroupList(_factoryCode, _token)).ToList();

            maintenanceMachineViewModel.MachineGroupList = new List<MachineGroup>();
            maintenanceMachineViewModel.MachineGroupList = MachineGroupL;
            maintenanceMachineViewModel.MachineViewModelList = MachineModelViewList;

            maintenanceMachineViewModel.JoinList = new List<JointViewModel>();
            //maintenanceMachineViewModel.JoinList = JsonConvert.DeserializeObject<List<JointViewModel>>(_joinAPIRepository.GetJoinList(_factoryCode, _token)).Where(x=>x.JointName.Contains("กาว")).ToList();

            maintenanceMachineViewModel.JoinList = JsonConvert.DeserializeObject<List<JointViewModel>>(_joinAPIRepository.GetJoinList(_factoryCode, _token)).ToList();
            //matchingvalues  = maintenanceMachineViewModel.JoinList.Where(m => m.JointName.Contains("กาว")).ToList();


        }

        public void SaveMachine(MaintenanceMachineViewModel model)
        {
            ParentModel MachineModel = new ParentModel();
            MachineModel.AppName = Globals.AppNameEncrypt;
            MachineModel.FactoryCode = _factoryCode;
            MachineModel.PlantCode = _factoryCode;

            model.MachineViewModel.MachineStatus = true;
            model.MachineViewModel.FactoryCode = _factoryCode;
            model.MachineViewModel.Plant = _saleOrg;
            model.MachineViewModel.Id = 0;
            model.MachineViewModel.MachineGroup = model.MachineViewModel.MachineGroup == "--- Please Select Item ---" ? null : model.MachineViewModel.MachineGroup;


            MachineModel.Machine = mapper.Map<MachineViewModel, Machine>(model.MachineViewModel);
            MachineModel.Machine.CreatedDate = DateTime.Now;
            MachineModel.Machine.CreatedBy = _username;

            string MachineListJsonString = JsonConvert.SerializeObject(MachineModel);

            _machineAPIRepository.SaveMachine(MachineListJsonString, _token);
        }

        public void UpdateMachine(MachineViewModel machineViewModel)
        {
            ParentModel machineModel = new ParentModel();
            machineModel.AppName = Globals.AppNameEncrypt;
            machineModel.FactoryCode = _factoryCode;

            machineViewModel.MachineStatus = true;
            machineViewModel.FactoryCode = _factoryCode;
            machineViewModel.Plant = _saleOrg;
            machineViewModel.MachineGroup = machineViewModel.MachineGroup == "--- Please Select Item ---" ? null : machineViewModel.MachineGroup;

            machineModel.Machine = mapper.Map<MachineViewModel, Machine>(machineViewModel);
            machineModel.Machine.CreatedDate = machineViewModel.CreatedDate;
            machineModel.Machine.CreatedBy = machineViewModel.CreatedBy;
            machineModel.Machine.UpdatedDate = DateTime.Now;
            machineModel.Machine.UpdatedBy = _username;

            string jsonString = JsonConvert.SerializeObject(machineModel);

            _machineAPIRepository.UpdateMachine(jsonString, _token);
        }

        public void SetMachineStatus(MachineViewModel machineViewModel)
        {
            ParentModel machineModel = new ParentModel();
            machineModel.AppName = Globals.AppNameEncrypt;
            machineModel.FactoryCode = _factoryCode;
            machineModel.PlantCode = _factoryCode;

            if (machineViewModel.MachineStatus != false)
            {
                machineViewModel.MachineStatus = true;
            }
            machineViewModel.FactoryCode = _factoryCode;
            machineViewModel.Plant = _factoryCode;

            machineModel.Machine = mapper.Map<MachineViewModel, Machine>(machineViewModel);

            string jsonString = JsonConvert.SerializeObject(machineModel);

            _machineAPIRepository.UpdateMachine(jsonString, _token);

        }

        public void DeleteMachine(int Id)
        {
            var MachineData = JsonConvert.DeserializeObject<Machine>(_machineAPIRepository.GetMachineById(Id, _token));

            MachineData.MachineStatus = false;

            ParentModel MachineParent = new ParentModel();
            MachineParent.AppName = Globals.AppNameEncrypt;
            MachineParent.SaleOrg = _saleOrg;
            MachineParent.PlantCode = _factoryCode;
            MachineParent.Machine = MachineData;
            string jsonString = JsonConvert.SerializeObject(MachineParent);

            _machineAPIRepository.UpdateMachine(jsonString, _token);

            //return MachineData.MaterialNo;
        }

    }
}
