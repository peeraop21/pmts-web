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
using PMTs.DataAccess.ModelView.MaintenancePrintMethod;
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
using PrintMethodViewModel = PMTs.DataAccess.ModelView.MaintenancePrintMethod.PrintMethodViewModel;

namespace PMTs.WebApplication.Controllers
{
    [SessionTimeout]
    public class MaintenancePrintMethodController : Controller
    {
        private readonly IMaintenancePrintMethodService _maintenancePrintMethodService;
        private readonly IExtensionService _extensionService;

        public MaintenancePrintMethodController(IMaintenancePrintMethodService maintenancePrintMethodService, IExtensionService extensionService)
        {
            // Initialize Service
            _maintenancePrintMethodService = maintenancePrintMethodService;
            _extensionService = extensionService;
        }

        #region PrintMethodManagement
        [SessionTimeout]
        public IActionResult Index()
        {
            MaintenancePrintMethodViewModel maintenancePrintMethodViewModel = new MaintenancePrintMethodViewModel();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenancePrintMethodService.GetPrintMethod(maintenancePrintMethodViewModel);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }

            return View(maintenancePrintMethodViewModel);
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult SavePrintMethod(MaintenancePrintMethodViewModel maintenancePrintMethodViewModel)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenancePrintMethodService.SavePrintMethod(maintenancePrintMethodViewModel);
                _maintenancePrintMethodService.GetPrintMethod(maintenancePrintMethodViewModel);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                exceptionMessage = ex.Message;
                isSuccess = false;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_PrintMethodTable", maintenancePrintMethodViewModel) });
        }

        //[SessionTimeout]
        //public PartialViewResult UpdatePrintMethodTable()
        //{
        //    MaintenancePrintMethodViewModel maintenancePrintMethodViewModel = new MaintenancePrintMethodViewModel();

        //    try
        //    {
        //        Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
        //        _maintenancePrintMethodService.GetPrintMethod(maintenancePrintMethodViewModel);
        //        Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
        //        throw ex;
        //    }

        //    return PartialView("_PrintMethodTable", maintenancePrintMethodViewModel);
        //}

        [SessionTimeout]
        [HttpPost]
        public JsonResult UpdatePrintMethod(string req)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;
            PrintMethodViewModel model = new PrintMethodViewModel();
            model = JsonConvert.DeserializeObject<PrintMethodViewModel>(req, new IsoDateTimeConverter { DateTimeFormat = "yyyy-dd-MMTHH:mm:ss" });
            MaintenancePrintMethodViewModel maintenancePrintMethodViewModel = new MaintenancePrintMethodViewModel();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenancePrintMethodService.UpdatePrintMethod(model);
                _maintenancePrintMethodService.GetPrintMethod(maintenancePrintMethodViewModel);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                exceptionMessage = ex.Message;
                isSuccess = false;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_PrintMethodTable", maintenancePrintMethodViewModel) });
        }

        #endregion
    }
}