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
using PMTs.DataAccess.ModelView.MaintenanceScoreGap;
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
    public class MaintenanceScoreGapController : Controller
    {
        private readonly IMaintenanceScoreGapService _maintenanceScoreGapService;
        private readonly IExtensionService _extensionService;

        public MaintenanceScoreGapController(IMaintenanceScoreGapService maintenanceScoreGapService, IExtensionService extensionService)
        {
            // Initialize Service
            _maintenanceScoreGapService = maintenanceScoreGapService;
            _extensionService = extensionService;
        }

        #region ScoreGapManagement
        [SessionTimeout]
        public IActionResult Index()
        {
            MaintenanceScoreGapViewModel maintenanceScoreGapViewModel = new MaintenanceScoreGapViewModel();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceScoreGapService.GetScoreGap(maintenanceScoreGapViewModel);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }

            return View(maintenanceScoreGapViewModel);
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult SaveScoreGap(MaintenanceScoreGapViewModel maintenanceScoreGapViewModel)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceScoreGapService.SaveScoreGap(maintenanceScoreGapViewModel);
                _maintenanceScoreGapService.GetScoreGap(maintenanceScoreGapViewModel);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                exceptionMessage = ex.Message;
                isSuccess = false;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_ScoreGapTable", maintenanceScoreGapViewModel) });
        }

        //[SessionTimeout]
        //public PartialViewResult UpdateScoreGapTable()
        //{
        //    MaintenanceScoreGapViewModel maintenanceScoreGapViewModel = new MaintenanceScoreGapViewModel();

        //    try
        //    {
        //        Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
        //        _maintenanceScoreGapService.GetScoreGap(maintenanceScoreGapViewModel);
        //        Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
        //        throw ex;
        //    }

        //    return PartialView("_ScoreGapTable", maintenanceScoreGapViewModel);
        //}

        [SessionTimeout]
        [HttpPost]
        public JsonResult UpdateScoreGap(ScoreGapViewModel ScoreGapViewModel)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;
            MaintenanceScoreGapViewModel maintenanceScoreGapViewModel = new MaintenanceScoreGapViewModel();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceScoreGapService.UpdateScoreGap(ScoreGapViewModel);
                _maintenanceScoreGapService.GetScoreGap(maintenanceScoreGapViewModel);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                exceptionMessage = ex.Message;
                isSuccess = false;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_ScoreGapTable", maintenanceScoreGapViewModel) });
        }
        #endregion
    }
}