using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.MaintenanceFlute;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Tracing;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services
{
    [TraceAspect]
    public class MaintenanceFluteService : IMaintenanceFluteService
    {
        IHttpContextAccessor _httpContextAccessor;
        private readonly IFluteAPIRepository _fluteAPIRepository;

        private readonly string _username;
        private readonly string _saleOrg;
        private readonly string _factoryCode;
        private readonly string _token;

        public MaintenanceFluteService(IHttpContextAccessor httpContextAccessor, IFluteAPIRepository fluteAPIRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _fluteAPIRepository = fluteAPIRepository;

            var userSessionModel = SessionExtentions.GetSession<UserSessionModel>(_httpContextAccessor.HttpContext.Session, "UserSessionModel");
            if (userSessionModel != null)
            {
                _username = userSessionModel.UserName;
                _saleOrg = userSessionModel.SaleOrg;
                _factoryCode = userSessionModel.FactoryCode;
                _token = userSessionModel.Token;
            }
        }

        public MaintenanceFluteModel GetFlute()
        {
            MaintenanceFluteModel model = new MaintenanceFluteModel();
            model = JsonConvert.DeserializeObject<MaintenanceFluteModel>(_fluteAPIRepository.GetFluteMaintain(_factoryCode, _token));
            return model;
        }

        public void AddFlute(MaintenanceFluteModel model)
        {
            model.Flute.FactoryCode = _factoryCode;
            model.Flute.CreatedBy = _username;
            _fluteAPIRepository.AddFluteMaintain(_factoryCode, JsonConvert.SerializeObject(model), _token);
        }
        public void UpdateFlute(MaintenanceFluteModel model)
        {
            model.Flute.FactoryCode = _factoryCode;
            model.Flute.UpdatedBy = _username;
            _fluteAPIRepository.UpdateFluteMaintain(_factoryCode, JsonConvert.SerializeObject(model), _token);
        }
    }
}
