using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.MaintenanceAccount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services.Interfaces
{
    public interface IMaintenanceAccountService
    {
        void GetAccount(MaintenanceAccountViewModel maintenanceAccouuntViewModel);
        void SaveAccount(MaintenanceAccountViewModel maintenanceCustomerViewModel, IFormFile PictureUser);
        void UpdateAccount(AccountViewModel maintenanceCustomerViewModel, IFormFile PictureUser, string iUserPath);
        void ChangePassword(ChangePasswordViewModel changePasswordViewModel);
        List<SelectListItem> GetListSaleOrg();
        List<SelectListItem> GetListPlant();

        //Tassanai Update 13//07/2020
        //void GetAccountDetail(AccountViewModel accountViewModel);
    }
}
