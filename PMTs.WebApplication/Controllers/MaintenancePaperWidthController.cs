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
using PMTs.DataAccess.ModelView.MaintenancePaperWidth;
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
    public class MaintenancePaperWidthController : Controller
    {
        private readonly IMaintenancePaperWidthService _maintenancePaperWidthService;
        private readonly IExtensionService _extensionService;

        public MaintenancePaperWidthController(IMaintenancePaperWidthService maintenancePaperWidthService, IExtensionService extensionService)
        {
            // Initialize Service
            _maintenancePaperWidthService = maintenancePaperWidthService;
            _extensionService = extensionService;
        }

        #region PaperWidthManagement
        [SessionTimeout]
        public IActionResult Index()
        {
            MaintenancePaperWidthViewModel maintenancePaperWidthViewModel = new MaintenancePaperWidthViewModel();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenancePaperWidthService.GetPaperWidth(maintenancePaperWidthViewModel);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }

            return View(maintenancePaperWidthViewModel);
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult SavePaperWidth(MaintenancePaperWidthViewModel maintenancePaperWidthViewModel)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenancePaperWidthService.SavePaperWidth(maintenancePaperWidthViewModel);
                _maintenancePaperWidthService.GetPaperWidth(maintenancePaperWidthViewModel);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                exceptionMessage = ex.Message;
                isSuccess = false;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_PaperWidthTable", maintenancePaperWidthViewModel) });
        }

        //[SessionTimeout]
        //public PartialViewResult UpdatePaperWidthTable()
        //{
        //    MaintenancePaperWidthViewModel maintenancePaperWidthViewModel = new MaintenancePaperWidthViewModel();

        //    try
        //    {
        //        Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
        //        _maintenancePaperWidthService.GetPaperWidth(maintenancePaperWidthViewModel);
        //        Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
        //        throw ex;
        //    }

        //    return PartialView("_PaperWidthTable", maintenancePaperWidthViewModel);
        //}

        [SessionTimeout]
        [HttpPost]
        public JsonResult UpdatePaperWidth(string req)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;
            MaintenancePaperWidthViewModel maintenancePaperWidthViewModel = new MaintenancePaperWidthViewModel();
            try
            {
                PaperWidthViewModel PaperWidthViewModel = new PaperWidthViewModel();
                PaperWidthViewModel = JsonConvert.DeserializeObject<PaperWidthViewModel>(req, new IsoDateTimeConverter { DateTimeFormat = "yyyy-dd-MMTHH:mm:ss" });
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenancePaperWidthService.UpdatePaperWidth(PaperWidthViewModel);
                _maintenancePaperWidthService.GetPaperWidth(maintenancePaperWidthViewModel);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                exceptionMessage = ex.Message;
                isSuccess = false;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_PaperWidthTable", maintenancePaperWidthViewModel) });
        }
        #endregion
    }
}