
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
using PMTs.DataAccess.ModelView.MaintenanceAllowance;
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
    public class MaintenanceAllowanceController : Controller
    {
        private readonly IMaintenanceAllowanceService _maintenanceAllowanceService;
        private readonly IExtensionService _extensionService;

        public MaintenanceAllowanceController(IMaintenanceAllowanceService maintenanceAllowanceService, IExtensionService extensionService)
        {
            // Initialize Service
            _maintenanceAllowanceService = maintenanceAllowanceService;
            _extensionService = extensionService;
        }

        #region AllowanceManagement
        [SessionTimeout]
        public IActionResult Index()
        {
            MaintenanceAllowanceViewModel maintenanceAllowanceViewModel = new MaintenanceAllowanceViewModel();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceAllowanceService.GetAllowance(maintenanceAllowanceViewModel);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }

            return View(maintenanceAllowanceViewModel);
        }
        [SessionTimeout]
        [HttpPost]
        public JsonResult SaveAllowance(MaintenanceAllowanceViewModel maintenanceAllowanceViewModel)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceAllowanceService.SaveAllowance(maintenanceAllowanceViewModel);
                _maintenanceAllowanceService.GetAllowance(maintenanceAllowanceViewModel);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                exceptionMessage = ex.Message;
                isSuccess = false;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_AllowanceTable", maintenanceAllowanceViewModel) });
        }
        [SessionTimeout]
        public PartialViewResult UpdateAllowanceTable()
        {
            MaintenanceAllowanceViewModel maintenanceAllowanceViewModel = new MaintenanceAllowanceViewModel();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceAllowanceService.GetAllowance(maintenanceAllowanceViewModel);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }

            return PartialView("_AllowanceTable", maintenanceAllowanceViewModel);
        }
        [SessionTimeout]
        [HttpPost]
        public JsonResult UpdateAllowance(string req)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;
            MaintenanceAllowanceViewModel maintenanceAllowanceViewModel = new MaintenanceAllowanceViewModel();
            try
            {
                AllowanceViewModel AllowanceViewModel = new AllowanceViewModel();
                AllowanceViewModel = JsonConvert.DeserializeObject<AllowanceViewModel>(req);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceAllowanceService.UpdateAllowance(AllowanceViewModel);
                _maintenanceAllowanceService.GetAllowance(maintenanceAllowanceViewModel);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                exceptionMessage = ex.Message;
                isSuccess = false;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_AllowanceTable", maintenanceAllowanceViewModel) });
        }

        [SessionTimeout]
        [HttpPut]
        public JsonResult DeleteAllowance(int Id)
        {

            bool isSuccess;
            string exceptionMessage = string.Empty;
            MaintenanceAllowanceViewModel maintenanceAllowanceViewModel = new MaintenanceAllowanceViewModel();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceAllowanceService.DeleteAllowance(Id);
                _maintenanceAllowanceService.GetAllowance(maintenanceAllowanceViewModel);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                exceptionMessage = ex.Message;
                isSuccess = false;
            }
            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_AllowanceTable", maintenanceAllowanceViewModel) });
        }
        #endregion
    }
}