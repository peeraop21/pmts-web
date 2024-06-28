using Microsoft.AspNetCore.Http;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView.AutoPackingCustomer;
using PMTs.DataAccess.ModelView.AutoPackingSpec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services.Interfaces
{
    public interface IAutoPackingCustomerService
    {
        void SaveAndUpdateAutoPackingCustomer(AutoPackingCustomer autoPackingCustomer, string action);
        void ImportAutoPackingCustomerFromFile(IFormFile file, ref AutoPackingSpecMainModel result, ref string exceptionMessage);
        void GetAutoPackingCustomerData(ref List<AutoPackingCustomerData> maintenanceCustomerViewModel, string typeSearch, string keySearch);
    }
}
