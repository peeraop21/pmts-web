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
using PMTs.DataAccess.ModelView.MaintenanceMachine;
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
    public class MaintenanceMachineController : Controller
    {
        private readonly IMaintenanceMachineService _maintenanceMachineService;
        private readonly IExtensionService _extensionService;

        public MaintenanceMachineController(IMaintenanceMachineService maintenanceMachineService, IExtensionService extensionService)
        {
            // Initialize Service
            _maintenanceMachineService = maintenanceMachineService;
            _extensionService = extensionService;
        }

        #region MachineManagement
        [SessionTimeout]
        public IActionResult Index()
        {
            MaintenanceMachineViewModel maintenanceMachineViewModel = new MaintenanceMachineViewModel();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceMachineService.GetMachine(maintenanceMachineViewModel);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }

            return View(maintenanceMachineViewModel);
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult SaveMachine(MaintenanceMachineViewModel maintenanceMachineViewModel)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceMachineService.SaveMachine(maintenanceMachineViewModel);
                _maintenanceMachineService.GetMachine(maintenanceMachineViewModel);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                _maintenanceMachineService.GetMachine(maintenanceMachineViewModel);
                exceptionMessage = ex.Message;
                isSuccess = false;
            }
            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, ViewResponse = RenderView.RenderRazorViewToString(this, "_MachineTable", maintenanceMachineViewModel) });
        }

        [SessionTimeout]
        public PartialViewResult UpdateMachineTable()
        {
            MaintenanceMachineViewModel maintenanceMachineViewModel = new MaintenanceMachineViewModel();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceMachineService.GetMachine(maintenanceMachineViewModel);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }

            return PartialView("_MachineTable", maintenanceMachineViewModel);
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult UpdateMachine(string req)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;
            var machineViewModel = new MachineViewModel();
            var maintenanceMachineViewModel = new MaintenanceMachineViewModel();

            try
            {
                machineViewModel = JsonConvert.DeserializeObject<MachineViewModel>(req, new IsoDateTimeConverter { DateTimeFormat = "yyyy-dd-MMTHH:mm:ss" });
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceMachineService.UpdateMachine(machineViewModel);
                _maintenanceMachineService.GetMachine(maintenanceMachineViewModel);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                _maintenanceMachineService.GetMachine(maintenanceMachineViewModel);
                exceptionMessage = ex.Message;
                isSuccess = false;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, ViewResponse = RenderView.RenderRazorViewToString(this, "_MachineTable", maintenanceMachineViewModel) });
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult SetMachineStatus(MachineViewModel MachineViewModel)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceMachineService.SetMachineStatus(MachineViewModel);
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
        [HttpPut]
        public JsonResult DeleteMachine(int Id)
        {

            bool isSuccess;
            string exceptionMessage = string.Empty;
            var maintenanceMachineViewModel = new MaintenanceMachineViewModel();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceMachineService.DeleteMachine(Id);
                _maintenanceMachineService.GetMachine(maintenanceMachineViewModel);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                exceptionMessage = ex.Message;
                isSuccess = false;
            }
            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, ViewResponse = RenderView.RenderRazorViewToString(this, "_MachineTable", maintenanceMachineViewModel) });
        }

        #endregion
    }
}