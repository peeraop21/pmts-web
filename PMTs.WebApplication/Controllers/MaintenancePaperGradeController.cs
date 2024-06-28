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
using PMTs.DataAccess.ModelView.MaintenancePaperGrade;
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
    public class MaintenancePaperGradeController : Controller
    {
        private readonly IMaintenancePaperGradeService _maintenancePaperGradeService;
        private readonly IExtensionService _extensionService;

        public MaintenancePaperGradeController(IMaintenancePaperGradeService maintenancePaperGradeService, IExtensionService extensionService)
        {
            // Initialize Service
            _maintenancePaperGradeService = maintenancePaperGradeService;
            _extensionService = extensionService;
        }

        #region PaperGradeManagement

        [SessionTimeout]
        public IActionResult Index()
        {
            MaintenancePaperGradeViewModel maintenancePaperGradeViewModel = new MaintenancePaperGradeViewModel();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenancePaperGradeService.GetPaperGrade(maintenancePaperGradeViewModel);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }

            return View(maintenancePaperGradeViewModel);
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult SavePaperGrade(MaintenancePaperGradeViewModel maintenancePaperGradeViewModel)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenancePaperGradeService.SavePaperGrade(maintenancePaperGradeViewModel);
                _maintenancePaperGradeService.GetPaperGrade(maintenancePaperGradeViewModel);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                exceptionMessage = ex.Message;
                isSuccess = false;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_PaperGradeTable", maintenancePaperGradeViewModel) });
        }

        [SessionTimeout]
        public PartialViewResult UpdatePaperGradeTable()
        {
            MaintenancePaperGradeViewModel maintenancePaperGradeViewModel = new MaintenancePaperGradeViewModel();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenancePaperGradeService.GetPaperGrade(maintenancePaperGradeViewModel);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }

            return PartialView("_PaperGradeTable", maintenancePaperGradeViewModel);
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult UpdatePaperGrade(string req)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;
            MaintenancePaperGradeViewModel maintenancePaperGradeViewModel = new MaintenancePaperGradeViewModel();
            try
            {
                PaperGradeViewModel PaperGradeViewModel = new PaperGradeViewModel();
                PaperGradeViewModel = JsonConvert.DeserializeObject<PaperGradeViewModel>(req, new IsoDateTimeConverter { DateTimeFormat = "yyyy-dd-MMTHH:mm:ss" });
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenancePaperGradeService.UpdatePaperGrade(PaperGradeViewModel);
                _maintenancePaperGradeService.GetPaperGrade(maintenancePaperGradeViewModel);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                exceptionMessage = ex.Message;
                isSuccess = false;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_PaperGradeTable", maintenancePaperGradeViewModel) });
        }

        #endregion
    }
}