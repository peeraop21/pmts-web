using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView.MaintenanceCustomer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services.Interfaces
{
    public interface IMaintenanceCustomerService
    {
        void GetCustomer(ref MaintenanceCustomerViewModel CustomerModelList, string typeSearch, string keySearch);
        //void GetCustomer(MaintenanceCustomerViewModel maintenanceCustomerViewModel);
        void SaveCustomer(MaintenanceCustomerViewModel maintenanceCustomerViewModel);
        void UpdateCustomer(CustomerViewModel customerViewModel);
        void SetCustomerStatus(CustomerViewModel customerViewModel);
        void DeleteCustomer(int Id);
        // bool CheckCustomerDouplicate(int Id);
        bool CheckCustomerDouplicate(string cusId, string CusCode);
        bool CheckCustomerDouplicateUpdate(string cusId, string CusCode, string IdCus);
    }
}
