using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PMTs.DataAccess;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.MaintenancePMTsConfig;
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
    public class MaintenancePMTsConfigController : Controller
    {
        private readonly IMaintenancePMTsConfigService _maintenancePMTsConfigService;
        private readonly IExtensionService _extensionService;

        public MaintenancePMTsConfigController(IMaintenancePMTsConfigService maintenancePMTsConfigService, IExtensionService extensionService)
        {
            // Initialize Service
            _maintenancePMTsConfigService = maintenancePMTsConfigService;
            _extensionService = extensionService;
        }

        #region PMTsConfigManagement
        [SessionTimeout]
        public IActionResult Index()
        {
            MaintenancePMTsConfigViewModel maintenancePMTsConfigViewModel = new MaintenancePMTsConfigViewModel();
            maintenancePMTsConfigViewModel.PMTsConfigViewModelList = new List<PMTsConfigViewModel>();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenancePMTsConfigService.GetPMTsConfig(ref maintenancePMTsConfigViewModel);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }

            return View(maintenancePMTsConfigViewModel);
        }

        [SessionTimeout]
        public PartialViewResult UpdatePMTsConfigTable()
        {
            MaintenancePMTsConfigViewModel maintenancePMTsConfigViewModel = new MaintenancePMTsConfigViewModel();
            maintenancePMTsConfigViewModel.PMTsConfigViewModelList = new List<PMTsConfigViewModel>();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenancePMTsConfigService.GetPMTsConfig(ref maintenancePMTsConfigViewModel);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }

            return PartialView("_PMTsConfigTable", maintenancePMTsConfigViewModel);
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult SavePMTsConfig(MaintenancePMTsConfigViewModel maintenancePMTsConfigViewModel)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenancePMTsConfigService.SavePMTsConfig(maintenancePMTsConfigViewModel);
                maintenancePMTsConfigViewModel = new MaintenancePMTsConfigViewModel();
                maintenancePMTsConfigViewModel.PMTsConfigViewModelList = new List<PMTsConfigViewModel>();
                _maintenancePMTsConfigService.GetPMTsConfig(ref maintenancePMTsConfigViewModel);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                exceptionMessage = ex.Message;
                isSuccess = false;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_PMTsConfigTable", maintenancePMTsConfigViewModel) });
        }

        // [SessionTimeout]
        // [HttpPost]
        //public JsonResult UpdatePMTsConfig(string  req)
        //{
        //    bool isSuccess;
        //    string exceptionMessage = string.Empty;

        //    MaintenancePMTsConfigViewModel maintenancePMTsConfigViewModel = new MaintenancePMTsConfigViewModel();
        //    try
        //    {
        //        PMTsConfigViewModel PMTsConfigViewModel = new PMTsConfigViewModel();

        //        PMTsConfigViewModel = JsonConvert.DeserializeObject<PMTsConfigViewModel>(req);
        //        Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
        //        _maintenancePMTsConfigService.UpdatePMTsConfig(PMTsConfigViewModel);

        //        maintenancePMTsConfigViewModel = new MaintenancePMTsConfigViewModel();
        //        maintenancePMTsConfigViewModel.PMTsConfigViewModelList = new List<PMTsConfigViewModel>();
        //        _maintenancePMTsConfigService.GetPMTsConfig(ref maintenancePMTsConfigViewModel);
        //        isSuccess = true;
        //        Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
        //        exceptionMessage = ex.Message;
        //        isSuccess = false;
        //    }

        //    return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage , View =RenderView.RenderRazorViewToString(this, "_PMTsConfigTable",maintenancePMTsConfigViewModel) });
        //}
        [SessionTimeout]
        [HttpPost]
        public JsonResult UpdatePMTsConfig(PMTsConfigViewModel req)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;

            MaintenancePMTsConfigViewModel maintenancePMTsConfigViewModel = new MaintenancePMTsConfigViewModel();
            try
            {
                //PMTsConfigViewModel PMTsConfigViewModel = new PMTsConfigViewModel();

                //PMTsConfigViewModel = JsonConvert.DeserializeObject<PMTsConfigViewModel>(req);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenancePMTsConfigService.UpdatePMTsConfig(req);

                maintenancePMTsConfigViewModel = new MaintenancePMTsConfigViewModel();
                maintenancePMTsConfigViewModel.PMTsConfigViewModelList = new List<PMTsConfigViewModel>();
                _maintenancePMTsConfigService.GetPMTsConfig(ref maintenancePMTsConfigViewModel);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                exceptionMessage = ex.Message;
                isSuccess = false;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_PMTsConfigTable", maintenancePMTsConfigViewModel) });
        }

        #endregion
    }
}