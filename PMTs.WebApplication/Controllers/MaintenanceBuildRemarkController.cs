using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PMTs.DataAccess;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.MaintenanceBuildRemark;
using PMTs.DataAccess.Repository;
using PMTs.DataAccess.Shared;
using PMTs.Logs.Logger;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Controllers
{
    [SessionTimeout]
    public class MaintenanceBuildRemarkController : Controller
    {
        private readonly IMaintenanceBuildRemarkService _maintenanceBuildRemarkService;
        private readonly IExtensionService _extensionService;

        public MaintenanceBuildRemarkController(IMaintenanceBuildRemarkService maintenanceBuildRemarkService, IExtensionService extensionService)
        {
            // Initialize Service
            _maintenanceBuildRemarkService = maintenanceBuildRemarkService;
            _extensionService = extensionService;
        }

        #region BuildRemarkManagement
        [SessionTimeout]
        public IActionResult Index()
        {
            MaintenanceBuildRemarkViewModel maintenanceBuildRemarkViewModel = new MaintenanceBuildRemarkViewModel();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceBuildRemarkService.GetBuildRemark(maintenanceBuildRemarkViewModel);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }

            return View(maintenanceBuildRemarkViewModel);
        }
        [SessionTimeout]
        [HttpPost]
        public JsonResult SaveBuildRemark(MaintenanceBuildRemarkViewModel maintenanceBuildRemarkViewModel)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceBuildRemarkService.SaveBuildRemark(maintenanceBuildRemarkViewModel);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                exceptionMessage = ex.Message;
                isSuccess = false;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage });
        }
        [SessionTimeout]
        public PartialViewResult UpdateBuildRemarkTable()
        {
            MaintenanceBuildRemarkViewModel maintenanceBuildRemarkViewModel = new MaintenanceBuildRemarkViewModel();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceBuildRemarkService.GetBuildRemark(maintenanceBuildRemarkViewModel);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }

            return PartialView("_BuildRemarkTable", maintenanceBuildRemarkViewModel);
        }
        [SessionTimeout]
        [HttpPost]
        public JsonResult UpdateBuildRemark(string req)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;

            try
            {
                var buildRemarkViewModel = JsonConvert.DeserializeObject<BuildRemarkViewModel>(req, new IsoDateTimeConverter { DateTimeFormat = "yyyy-dd-MMTHH:mm:ss" });
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceBuildRemarkService.UpdateBuildRemark(buildRemarkViewModel);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                exceptionMessage = ex.Message;
                isSuccess = false;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage });
        }



        #endregion
    }
}