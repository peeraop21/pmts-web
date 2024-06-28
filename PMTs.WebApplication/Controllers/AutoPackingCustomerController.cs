using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView.AutoPackingCustomer;
using PMTs.DataAccess.ModelView.AutoPackingSpec;
using PMTs.DataAccess.ModelView.MaintenanceCustomer;
using PMTs.Logs.Logger;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Controllers
{
    public class AutoPackingCustomerController : Controller
    {
        private readonly IAutoPackingCustomerService autoPackingCustomerService;
        private readonly IAutoPackingSpecService autoPackingSpecService;
        private readonly IMaintenanceCustomerService maintenanceCustomerService;

        public AutoPackingCustomerController(IAutoPackingCustomerService autoPackingCustomerService,
            IAutoPackingSpecService autoPackingSpecService,
            IMaintenanceCustomerService maintenanceCustomerService
            )
        {
            this.autoPackingCustomerService = autoPackingCustomerService;
            this.autoPackingSpecService = autoPackingSpecService;
            this.maintenanceCustomerService = maintenanceCustomerService;
        }

        [SessionTimeout]
        public IActionResult Index(string typeSearch, string keySearch)
        {
            var result = new AutoPackingSpecMainModel();
            result.AutoPackingSpecs = new List<AutoPackingSpecViewModel>();
            result.AutoPackingSpec = new AutoPackingSpec();
            result.AutoPackingConfigs = new List<AutoPackingConfig>();
            result.CustomerViewModelList = new List<AutoPackingCustomerData>();
            var maintenanceCustomerViewModel = new List<AutoPackingCustomerData>();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                autoPackingCustomerService.GetAutoPackingCustomerData(ref maintenanceCustomerViewModel, typeSearch, keySearch);
                result.CustomerViewModelList = maintenanceCustomerViewModel;
                autoPackingSpecService.GetAutoPackingConfigs(ref result);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }

            return View(result);
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult SaveAndEditAutoPackingCustomer(AutoPackingCustomer autoPackingCustomer, string action)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;
            var result = new AutoPackingSpecMainModel();
            result.AutoPackingSpecs = new List<AutoPackingSpecViewModel>();
            result.AutoPackingSpec = new AutoPackingSpec();
            result.AutoPackingConfigs = new List<AutoPackingConfig>();
            result.CustomerViewModelList = new List<AutoPackingCustomerData>();
            var maintenanceCustomerViewModel = new List<AutoPackingCustomerData>();

            try
            {
                result.CustomerViewModelList.Clear();
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                autoPackingCustomerService.SaveAndUpdateAutoPackingCustomer(autoPackingCustomer, action);
                autoPackingCustomerService.GetAutoPackingCustomerData(ref maintenanceCustomerViewModel, "Customer_Id", autoPackingCustomer.CusId);
                autoPackingSpecService.GetAutoPackingConfigs(ref result);
                result.CustomerViewModelList = maintenanceCustomerViewModel;
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                exceptionMessage = ex.Message;
                isSuccess = false;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_AutoPackingCustomerTable", result) });
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult SearchAutoPackingCustomerByCusId(string cusId)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;
            var result = new AutoPackingSpecMainModel();
            result.AutoPackingSpecs = new List<AutoPackingSpecViewModel>();
            result.AutoPackingSpec = new AutoPackingSpec();
            result.AutoPackingConfigs = new List<AutoPackingConfig>();
            result.CustomerViewModelList = new List<AutoPackingCustomerData>();
            var maintenanceCustomerViewModel = new List<AutoPackingCustomerData>();

            try
            {
                result.CustomerViewModelList.Clear();
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                autoPackingCustomerService.GetAutoPackingCustomerData(ref maintenanceCustomerViewModel, "Customer_Id", cusId);
                autoPackingSpecService.GetAutoPackingConfigs(ref result);
                result.CustomerViewModelList = maintenanceCustomerViewModel;
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                exceptionMessage = ex.Message;
                isSuccess = false;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_AutoPackingCustomerTable", result) });
        }

    }
}