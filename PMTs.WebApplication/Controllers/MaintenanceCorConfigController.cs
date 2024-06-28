using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PMTs.DataAccess;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.MaintenanceCorConfig;
using PMTs.DataAccess.Repository;
using PMTs.DataAccess.Shared;
using PMTs.Logs.Logger;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Controllers
{
    [SessionTimeout]
    public class MaintenanceCorConfigController : Controller
    {
        private readonly IMaintenanceCorConfigService _maintenanceCorConfigService;
        private readonly IExtensionService _extensionService;

        public MaintenanceCorConfigController(IMaintenanceCorConfigService maintenanceCorConfigService, IExtensionService extensionService)
        {
            // Initialize Service
            _maintenanceCorConfigService = maintenanceCorConfigService;
            _extensionService = extensionService;
        }

        #region CorConfigManagement

        [SessionTimeout]
        public IActionResult Index()
        {
            MaintenanceCorConfigViewModel maintenanceCorConfigViewModel = new MaintenanceCorConfigViewModel();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceCorConfigService.GetCorConfig(maintenanceCorConfigViewModel);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }

            return View(maintenanceCorConfigViewModel);
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult SaveCorConfig(MaintenanceCorConfigViewModel maintenanceCorConfigViewModel)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceCorConfigService.SaveCorConfig(maintenanceCorConfigViewModel);
                _maintenanceCorConfigService.GetCorConfig(maintenanceCorConfigViewModel);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                exceptionMessage = ex.Message;
                isSuccess = false;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_CorConfigTable", maintenanceCorConfigViewModel) });
        }

        [SessionTimeout]
        public PartialViewResult UpdateCorConfigTable()
        {
            MaintenanceCorConfigViewModel maintenanceCorConfigViewModel = new MaintenanceCorConfigViewModel();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceCorConfigService.GetCorConfig(maintenanceCorConfigViewModel);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }

            return PartialView("_CorConfigTable", maintenanceCorConfigViewModel);
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult UpdateCorConfig(string req)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;
            MaintenanceCorConfigViewModel maintenanceCorConfigViewModel = new MaintenanceCorConfigViewModel();
            try
            {
                CorConfigViewModel CorConfigViewModel = new CorConfigViewModel();
                CorConfigViewModel = JsonConvert.DeserializeObject<CorConfigViewModel>(req, new IsoDateTimeConverter { DateTimeFormat = "yyyy-dd-MMTHH:mm:ss" });
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceCorConfigService.UpdateCorConfig(CorConfigViewModel);
                _maintenanceCorConfigService.GetCorConfig(maintenanceCorConfigViewModel);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                exceptionMessage = ex.Message;
                isSuccess = false;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_CorConfigTable", maintenanceCorConfigViewModel) });
        }
        #endregion
    }
}