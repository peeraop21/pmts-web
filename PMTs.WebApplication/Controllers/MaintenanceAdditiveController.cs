using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView.MaintenanceAdditive;
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
    public class MaintenanceAdditiveController : Controller
    {
        private readonly IMaintenanceAdditiveService _maintenanceAdditiveService;
        private readonly IExtensionService _extensionService;

        public MaintenanceAdditiveController(IMaintenanceAdditiveService maintenanceAdditiveService, IExtensionService extensionService)
        {
            // Initialize Service
            _maintenanceAdditiveService = maintenanceAdditiveService;
            _extensionService = extensionService;
        }

        [SessionTimeout]
        public IActionResult Index()
        {
            MaintenanceAdditiveViewModel model = new MaintenanceAdditiveViewModel();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                model = _maintenanceAdditiveService.GetAdditive();
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
            MaintenanceAdditiveViewModel model = new MaintenanceAdditiveViewModel();
            try
            {
                model.Additive = JsonConvert.DeserializeObject<Additive>(req);

                if (flag == "Add")
                {
                    _maintenanceAdditiveService.AddAdditive(model.Additive);
                }
                else
                {
                    _maintenanceAdditiveService.UpdateAdditive(model.Additive);
                }
                model = _maintenanceAdditiveService.GetAdditive();
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_AdditiveTable", model) });

        }
    }
}