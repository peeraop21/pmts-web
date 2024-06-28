using Microsoft.AspNetCore.Mvc;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.Shared;
using PMTs.Logs.Logger;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Controllers
{
    public class ChangePasswordController : Controller
    {
        private readonly IMaintenanceAccountService maintenanceAccountService;

        public ChangePasswordController(IMaintenanceAccountService maintenanceAccountService)
        {
            this.maintenanceAccountService = maintenanceAccountService;
        }

        public IActionResult Index()
        {
            ChangePasswordViewModel changePasswordViewModel = new ChangePasswordViewModel();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                ViewBag.UserDomain = Globals.Domain();
                changePasswordViewModel.Username = TempData["Username"].ToString();
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }
            return View(changePasswordViewModel);
        }

        [HttpPost]
        public JsonResult UpdatePassword(ChangePasswordViewModel changePasswordViewModel)
        {
            var isSuccess = false;
            var exceptionMessage = string.Empty;
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                maintenanceAccountService.ChangePassword(changePasswordViewModel);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                isSuccess = true;
            }
            catch (Exception ex)
            {
                exceptionMessage = ex.Message;
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage });
        }
    }
}