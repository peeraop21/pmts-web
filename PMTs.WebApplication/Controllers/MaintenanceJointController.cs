using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PMTs.DataAccess;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.MaintenanceJoint;
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
using JointViewModel = PMTs.DataAccess.ModelView.MaintenanceJoint.JointViewModel;

namespace PMTs.WebApplication.Controllers
{
    [SessionTimeout]
    public class MaintenanceJointController : Controller
    {
        private readonly IMaintenanceJointService _maintenanceJointService;
        private readonly IExtensionService _extensionService;

        public MaintenanceJointController(IMaintenanceJointService maintenanceJointService, IExtensionService extensionService)
        {
            // Initialize Service
            _maintenanceJointService = maintenanceJointService;
            _extensionService = extensionService;
        }

        #region JointManagement
        [SessionTimeout]
        public IActionResult Index()
        {
            MaintenanceJointViewModel maintenanceJointViewModel = new MaintenanceJointViewModel();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceJointService.GetJoint(maintenanceJointViewModel);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }

            return View(maintenanceJointViewModel);
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult SaveJoint(MaintenanceJointViewModel maintenanceJointViewModel)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceJointService.SaveJoint(maintenanceJointViewModel);
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
        public PartialViewResult UpdateJointTable()
        {
            MaintenanceJointViewModel maintenanceJointViewModel = new MaintenanceJointViewModel();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceJointService.GetJoint(maintenanceJointViewModel);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }

            return PartialView("_JointTable", maintenanceJointViewModel);
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult UpdateJoint(string req)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;
            var jointModel = new JointViewModel();
            jointModel = JsonConvert.DeserializeObject<JointViewModel>(req);
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceJointService.UpdateJoint(jointModel);
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