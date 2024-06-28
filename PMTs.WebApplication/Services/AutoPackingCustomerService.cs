using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView.AutoPackingCustomer;
using PMTs.DataAccess.ModelView.AutoPackingSpec;
using PMTs.DataAccess.ModelView.Login;
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
    public class AutoPackingCustomerService : IAutoPackingCustomerService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IAutoPackingCustomerAPIRepository autoPackingCustomerAPIRepository;

        private readonly string _username;
        private readonly string _saleOrg;
        private readonly string _factoryCode;
        private readonly string _token;

        public AutoPackingCustomerService(IHttpContextAccessor httpContextAccessor,
            IAutoPackingCustomerAPIRepository autoPackingCustomerAPIRepository)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.autoPackingCustomerAPIRepository = autoPackingCustomerAPIRepository;

            // Initialize UserData From Session
            var userSessionModel = SessionExtentions.GetSession<UserSessionModel>(httpContextAccessor.HttpContext.Session, "UserSessionModel");
            if (userSessionModel != null)
            {
                _username = userSessionModel.UserName;
                _saleOrg = userSessionModel.SaleOrg;
                _factoryCode = userSessionModel.FactoryCode;
                _token = userSessionModel.Token;
            }
        }

        public void GetAutoPackingCustomerData(ref List<AutoPackingCustomerData> maintenanceCustomerViewModel, string typeSearch, string keySearch)
        {
            maintenanceCustomerViewModel = new List<AutoPackingCustomerData>();
            maintenanceCustomerViewModel.Clear();
            if (String.IsNullOrEmpty(typeSearch) || String.IsNullOrEmpty(keySearch))
            {
                maintenanceCustomerViewModel.AddRange(JsonConvert.DeserializeObject<List<AutoPackingCustomerData>>(autoPackingCustomerAPIRepository.GetAllAutoPackingCustomerAndCustomer(_factoryCode, string.Empty, _token)));
            }
            else
            {
                if (typeSearch == "Customer_Name")
                {
                    maintenanceCustomerViewModel.AddRange(JsonConvert.DeserializeObject<List<AutoPackingCustomerData>>(autoPackingCustomerAPIRepository.GetAutoPackingCustomerAndCustomerByCustName(_factoryCode, keySearch, _token)));
                }
                if (typeSearch == "Customer_Code")
                {
                    maintenanceCustomerViewModel.AddRange(JsonConvert.DeserializeObject<List<AutoPackingCustomerData>>(autoPackingCustomerAPIRepository.GetAutoPackingCustomerAndCustomerByCustCode(_factoryCode, keySearch, _token)));
                }
                if (typeSearch == "Customer_Id")
                {
                    maintenanceCustomerViewModel.AddRange(JsonConvert.DeserializeObject<List<AutoPackingCustomerData>>(autoPackingCustomerAPIRepository.GetAutoPackingCustomerAndCustomerByCusId(_factoryCode, keySearch, _token)));
                }
            }
        }


        public void SaveAndUpdateAutoPackingCustomer(AutoPackingCustomer autoPackingCustomer, string action)
        {
            if (!string.IsNullOrEmpty(autoPackingCustomer.CusId))
            {
                if (action == "Save")
                {
                    autoPackingCustomer.CusId = autoPackingCustomer.CusId;
                    autoPackingCustomer.CusName = autoPackingCustomer.CusName;
                    autoPackingCustomer.CreatedBy = _username;
                    autoPackingCustomer.CreatedDate = DateTime.Now;
                    autoPackingCustomerAPIRepository.SaveAutoPackingCustomer(_factoryCode, JsonConvert.SerializeObject(autoPackingCustomer), _token);

                }
                else if (action == "Edit")
                {
                    autoPackingCustomer.UpdatedBy = _username;
                    autoPackingCustomer.UpdatedDate = DateTime.Now;

                    autoPackingCustomerAPIRepository.UpdateAutoPackingCustomer(_factoryCode, JsonConvert.SerializeObject(autoPackingCustomer), _token);
                }

            }
        }

        public void ImportAutoPackingCustomerFromFile(IFormFile file, ref AutoPackingSpecMainModel result, ref string exceptionMessage)
        {
            throw new NotImplementedException();
        }
    }
}
