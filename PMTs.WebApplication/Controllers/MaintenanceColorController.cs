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
using PMTs.DataAccess.ModelView.MaintenanceColor;
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
    public class MaintenanceColorController : Controller
    {
        private readonly IMaintenanceColorService _maintenanceColorService;
        private readonly IExtensionService _extensionService;

        public MaintenanceColorController(IMaintenanceColorService maintenanceColorService, IExtensionService extensionService)
        {
            // Initialize Service
            _maintenanceColorService = maintenanceColorService;
            _extensionService = extensionService;
        }

        #region ColorManagement
        [SessionTimeout]
        public IActionResult Index()
        {
            MaintenanceColorViewModel maintenanceColorViewModel = new MaintenanceColorViewModel();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceColorService.GetColor(maintenanceColorViewModel);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }

            return View(maintenanceColorViewModel);
        }
        [SessionTimeout]
        [HttpPost]
        public JsonResult SaveColor(MaintenanceColorViewModel maintenanceColorViewModel)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;
            maintenanceColorViewModel.ColorViewModelList = new List<Color>();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceColorService.SaveColor(maintenanceColorViewModel);
                _maintenanceColorService.GetColor(maintenanceColorViewModel);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                exceptionMessage = ex.Message;
                isSuccess = false;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_ColorTable", maintenanceColorViewModel) });
        }
        [SessionTimeout]
        public PartialViewResult UpdateColorTable()
        {
            MaintenanceColorViewModel maintenanceColorViewModel = new MaintenanceColorViewModel();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceColorService.GetColor(maintenanceColorViewModel);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }

            return PartialView("_ColorTable", maintenanceColorViewModel);
        }
        [SessionTimeout]
        [HttpPost]
        public JsonResult UpdateColor(string req)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;
            var ColorViewModel = JsonConvert.DeserializeObject<Color>(req, new IsoDateTimeConverter { DateTimeFormat = "yyyy-dd-MMTHH:mm:ss" });
            MaintenanceColorViewModel maintenanceColorViewModel = new MaintenanceColorViewModel();
            maintenanceColorViewModel.ColorViewModelList = new List<Color>();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceColorService.UpdateColor(ColorViewModel);
                _maintenanceColorService.GetColor(maintenanceColorViewModel);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                exceptionMessage = ex.Message;
                isSuccess = false;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_ColorTable", maintenanceColorViewModel) });
        }

        #endregion
    }
}