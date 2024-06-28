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
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.MaintenanceCustomer;
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
    public class MaintenanceCustomerController : Controller
    {

        private readonly IMaintenanceCustomerService _maintenanceCustomerService;
        private readonly IExtensionService _extensionService;

        public MaintenanceCustomerController(IMaintenanceCustomerService maintenanceCustomerService, IExtensionService extensionService)
        {
            // Initialize Service
            _maintenanceCustomerService = maintenanceCustomerService;
            _extensionService = extensionService;
        }
        #region CustomerManagement
        //public IActionResult ViewListCustomer(string TxtSearch, string ddlSearch)
        //{
        //    MaintenanceCustomerViewModel maintenanceCustomerViewModel = new MaintenanceCustomerViewModel();


        //    try
        //    {
        //        _maintenanceCustomerService.GetCustomer(ref maintenanceCustomerViewModel, ddlSearch, TxtSearch);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    //return Redirect()
        //    return Json(RanderView.RenderRazorViewToString(this, "_CustomerTable", maintenanceCustomerViewModel));

        //}

        [SessionTimeout]
        public IActionResult Index(string TxtSearch, string ddlSearch)
        {
            MaintenanceCustomerViewModel maintenanceCustomerViewModel = new MaintenanceCustomerViewModel();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceCustomerService.GetCustomer(ref maintenanceCustomerViewModel, ddlSearch, TxtSearch);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }

            return View(maintenanceCustomerViewModel);
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult SaveCustomer(MaintenanceCustomerViewModel maintenanceCustomerViewModel)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceCustomerService.SaveCustomer(maintenanceCustomerViewModel);
                // maintenanceCustomerViewModel = //_maintenanceCustomerService.GetCustomer(maintenanceCustomerViewModel);
                _maintenanceCustomerService.GetCustomer(ref maintenanceCustomerViewModel, "", "");
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                exceptionMessage = ex.Message;
                isSuccess = false;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_CustomerTable", maintenanceCustomerViewModel) });
            //return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage });
        }

        [SessionTimeout]
        public PartialViewResult UpdateCustomerTable()
        {
            MaintenanceCustomerViewModel maintenanceCustomerViewModel = new MaintenanceCustomerViewModel();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceCustomerService.GetCustomer(ref maintenanceCustomerViewModel, null, null);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }

            return PartialView("_CustomerTable", maintenanceCustomerViewModel);
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult UpdateCustomer(string req)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;
            CustomerViewModel customerViewModel = new CustomerViewModel();

            MaintenanceCustomerViewModel maintenanceCustomerViewModel = new MaintenanceCustomerViewModel();

            try
            {
                customerViewModel = JsonConvert.DeserializeObject<CustomerViewModel>(req, new IsoDateTimeConverter { DateTimeFormat = "yyyy-dd-MMTHH:mm:ss" });
                //ColorViewModel ColorViewModel = new ColorViewModel();
                //ColorViewModel = JsonConvert.DeserializeObject<ColorViewModel>(req);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceCustomerService.UpdateCustomer(customerViewModel);
                // _maintenanceCustomerService.GetCustomer(maintenanceCustomerViewModel);
                _maintenanceCustomerService.GetCustomer(ref maintenanceCustomerViewModel, "", "");
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                exceptionMessage = ex.Message;
                isSuccess = false;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_CustomerTable", maintenanceCustomerViewModel) });
            // return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage });

        }


        [SessionTimeout]
        [HttpPost]
        public JsonResult SetCustomerStatus(CustomerViewModel customerViewModel)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;
            MaintenanceCustomerViewModel maintenanceCustomerViewModel = new MaintenanceCustomerViewModel();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceCustomerService.SetCustomerStatus(customerViewModel);
                _maintenanceCustomerService.GetCustomer(ref maintenanceCustomerViewModel, null, null);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                exceptionMessage = ex.Message;
                isSuccess = false;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_CustomerTable", maintenanceCustomerViewModel) });
        }
        [SessionTimeout]
        [HttpPut]
        public JsonResult DeleteCustomer(int Id)
        {

            bool isSuccess;
            string exceptionMessage = string.Empty;
            MaintenanceCustomerViewModel maintenanceCustomerViewModel = new MaintenanceCustomerViewModel();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceCustomerService.DeleteCustomer(Id);
                _maintenanceCustomerService.GetCustomer(ref maintenanceCustomerViewModel, "", "");
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                exceptionMessage = ex.Message;
                isSuccess = false;
            }
            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_CustomerTable", maintenanceCustomerViewModel) });
        }

        public PartialViewResult _CustomerTable()
        {
            MaintenanceCustomerViewModel maintenanceCustomerViewModel = new MaintenanceCustomerViewModel();
            return PartialView(maintenanceCustomerViewModel);
        }

        public JsonResult CheckDuplicateCusID(string cusId, string CusCode)
        {
            string result = "";
            if (_maintenanceCustomerService.CheckCustomerDouplicate(cusId, CusCode))
            {
                result = "1";
            }
            return Json(result);
        }

        public JsonResult CheckDuplicateCusIDUpdate(string cusId, string CusCode, string ID)
        {
            string result = "";
            if (_maintenanceCustomerService.CheckCustomerDouplicateUpdate(cusId, CusCode, ID))
            {
                result = "1";
            }
            return Json(result);
        }

        #endregion
    }
}