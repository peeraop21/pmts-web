using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.MaintenanceAllowanceHard;
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
    public class MaintenanceAllowanceHardService : IMaintenanceAllowanceHardService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IAllowanceHardAPIRepository _allowanceHardAPIRepository;


        private string _username;
        private string _saleOrg;
        private string _factoryCode;
        private int _roldId;
        private string _token;


        public MaintenanceAllowanceHardService(IHttpContextAccessor httpContextAccessor
            , IAllowanceHardAPIRepository allowanceHardAPIRepository
            )
        {
            _httpContextAccessor = httpContextAccessor;

            _allowanceHardAPIRepository = allowanceHardAPIRepository;

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

        public MaintenanceAllowanceHardViewModel GetAllowancehard()
        {
            MaintenanceAllowanceHardViewModel maintenanceAllowanceHardView = new MaintenanceAllowanceHardViewModel();
            maintenanceAllowanceHardView.allowanceHards = JsonConvert.DeserializeObject<List<AllowanceHard>>(_allowanceHardAPIRepository.GetAllowanceHardList(_factoryCode, _token));
            return maintenanceAllowanceHardView;
        }

        public void AddAllowanceHard(AllowanceHard allowance)
        {
            allowance.FactoryCode = _factoryCode;
            allowance.CreatedBy = _username;
            allowance.CreatedDate = DateTime.Now;
            _allowanceHardAPIRepository.SaveAllowanceHard(JsonConvert.SerializeObject(allowance), _token);
        }

        public void UpdateAllowanceHard(AllowanceHard allowance)
        {
            allowance.FactoryCode = _factoryCode;
            allowance.UpdatedBy = _username;
            allowance.UpdatedDate = DateTime.Now;
            _allowanceHardAPIRepository.UpdateAllowanceHard(JsonConvert.SerializeObject(allowance), _token);
        }
    }
}
