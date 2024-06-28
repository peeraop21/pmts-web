using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView.MaintenanceAllowanceHard;
using PMTs.Logs.Logger;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Controllers
{
    [SessionTimeout]
    public class MaintenanceAllowanceHardController : Controller
    {
        private readonly IMaintenanceAllowanceHardService _maintenanceAllowanceHardService;
        private readonly IExtensionService _extensionService;

        public MaintenanceAllowanceHardController(IMaintenanceAllowanceHardService maintenanceAllowanceHardService, IExtensionService extensionService)
        {
            // Initialize Service
            _maintenanceAllowanceHardService = maintenanceAllowanceHardService;
            _extensionService = extensionService;
        }

        [SessionTimeout]
        public IActionResult Index()
        {
            MaintenanceAllowanceHardViewModel model = new MaintenanceAllowanceHardViewModel();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                model = _maintenanceAllowanceHardService.GetAllowancehard();
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return View(model);
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult ManageData(string req, string flag)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;
            Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
            MaintenanceAllowanceHardViewModel model = new MaintenanceAllowanceHardViewModel();
            try
            {
                model.allowance = JsonConvert.DeserializeObject<AllowanceHard>(req, new IsoDateTimeConverter { DateTimeFormat = "yyyy-dd-MMTHH:mm:ss" });

                if (flag == "Add")
                {
                    _maintenanceAllowanceHardService.AddAllowanceHard(model.allowance);
                }
                else
                {
                    _maintenanceAllowanceHardService.UpdateAllowanceHard(model.allowance);
                }
                model = _maintenanceAllowanceHardService.GetAllowancehard();
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_AllowanceHardTable", model) });

        }
    }
}