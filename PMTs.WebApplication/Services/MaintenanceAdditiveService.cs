using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.MaintenanceAdditive;
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
    public class MaintenanceAdditiveService : IMaintenanceAdditiveService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAdditiveAPIRepository _additiveAPIRepository;


        private string _username;
        private string _saleOrg;
        private string _factoryCode;
        private int _roldId;
        private string _token;


        public MaintenanceAdditiveService(IHttpContextAccessor httpContextAccessor
            , IAdditiveAPIRepository additiveAPIRepository
           )
        {
            _httpContextAccessor = httpContextAccessor;

            _additiveAPIRepository = additiveAPIRepository;


            var userSessionModel = SessionExtentions.GetSession<UserSessionModel>(_httpContextAccessor.HttpContext.Session, "UserSessionModel");
            if (userSessionModel != null)
            {
                _username = userSessionModel.UserName;
                _saleOrg = userSessionModel.SaleOrg;
                _factoryCode = userSessionModel.FactoryCode;
                _roldId = userSessionModel.DefaultRoleId;
                _token = userSessionModel.Token;

            }

        }

        public MaintenanceAdditiveViewModel GetAdditive()
        {
            MaintenanceAdditiveViewModel maintenanceAllowanceHardView = new MaintenanceAdditiveViewModel();
            maintenanceAllowanceHardView.additives = JsonConvert.DeserializeObject<List<Additive>>(_additiveAPIRepository.GetAdditiveList(_factoryCode, _token));
            maintenanceAllowanceHardView.Additive = new Additive();
            return maintenanceAllowanceHardView;
        }

        public void AddAdditive(Additive additive)
        {
            additive.CreatedBy = _username;
            _additiveAPIRepository.SaveAdditiveManual(JsonConvert.SerializeObject(additive), _token);
        }

        public void UpdateAdditive(Additive additive)
        {
            additive.UpdatedBy = _username;
            _additiveAPIRepository.UpdateAdditiveManual(JsonConvert.SerializeObject(additive), _token);
        }
    }
}
