using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.MaintenanceFluteTrim;
using PMTs.DataAccess.Repository;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Services.Interfaces;
using System.Collections.Generic;

namespace PMTs.WebApplication.Services
{
    public class MaintenanceFluteTrimService : IMaintenanceFluteTrimService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMachineFluteTrimAPIRepository _machineFluteTrimAPIRepository;



        UserSessionModel userSessionModel;

        private readonly string _username;
        private readonly string _saleOrg;
        private readonly string _factoryCode;
        private readonly string _token;
        public MaintenanceFluteTrimService(IMachineFluteTrimAPIRepository machineFluteTrimAPIRepository, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;

            _machineFluteTrimAPIRepository = machineFluteTrimAPIRepository;

            userSessionModel = SessionExtentions.GetSession<UserSessionModel>(_httpContextAccessor.HttpContext.Session, "UserSessionModel");
            if (userSessionModel != null)
            {
                _username = userSessionModel.UserName;
                _saleOrg = userSessionModel.SaleOrg;
                _factoryCode = userSessionModel.FactoryCode;
                _token = userSessionModel.Token;
            }
        }



        public MaintenanceFluteTrimModel InitalPage()
        {
            MaintenanceFluteTrimModel maintenanceFluteTrimModel = new MaintenanceFluteTrimModel();
            maintenanceFluteTrimModel = JsonConvert.DeserializeObject<MaintenanceFluteTrimModel>(_machineFluteTrimAPIRepository.GetDataForInitMachineFluteTrimPage(_factoryCode, _token));
            return maintenanceFluteTrimModel;
        }

        public bool SaveMachineFluteTrim(MachineFluteTrim machineFluteTrim)
        {
            machineFluteTrim.CreatedBy = _username;
            machineFluteTrim.FactoryCode = _factoryCode;
            bool result = false;
            try 
            {
                result = JsonConvert.DeserializeObject<bool>(_machineFluteTrimAPIRepository.AddMachineFluteTrim(JsonConvert.SerializeObject(machineFluteTrim), _factoryCode, _token));
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public bool UpdateMachineFluteTrim(MachineFluteTrim machineFluteTrim)
        {
            machineFluteTrim.UpdatedBy = _username;
            machineFluteTrim.FactoryCode = _factoryCode;
            bool result = false;
            try
            {
                result = JsonConvert.DeserializeObject<bool>(_machineFluteTrimAPIRepository.UpdateMachineFluteTrim(JsonConvert.SerializeObject(machineFluteTrim), _factoryCode, _token));
            }
            catch
            {
                result = false;
            }
            return result;
        }



    }
}
