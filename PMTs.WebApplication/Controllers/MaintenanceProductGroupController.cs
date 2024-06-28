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
using PMTs.DataAccess.ModelView.MaintenanceProductGroup;
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
    public class MaintenanceProductGroupController : Controller
    {
        private readonly IMaintenanceProductGroupService _maintenanceProductGroupService;
        private readonly IExtensionService _extensionService;

        public MaintenanceProductGroupController(IMaintenanceProductGroupService maintenanceProductGroupService, IExtensionService extensionService)
        {
            // Initialize Service
            _maintenanceProductGroupService = maintenanceProductGroupService;
            _extensionService = extensionService;
        }

        #region ProductGroupManagement
        [SessionTimeout]
        public IActionResult Index()
        {
            MaintenanceProductGroupViewModel maintenanceProductGroupViewModel = new MaintenanceProductGroupViewModel();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceProductGroupService.GetProductGroup(maintenanceProductGroupViewModel);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }

            return View(maintenanceProductGroupViewModel);
        }

        //[SessionTimeout]
        //public PartialViewResult UpdateProductGroupTable()
        //{
        //    MaintenanceProductGroupViewModel maintenanceProductGroupViewModel = new MaintenanceProductGroupViewModel();

        //    try
        //    {
        //        Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
        //        _maintenanceProductGroupService.GetProductGroup(maintenanceProductGroupViewModel);
        //        Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
        //        throw ex;
        //    }

        //    return PartialView("_ProductGroupTable", maintenanceProductGroupViewModel);
        //}

        [SessionTimeout]
        [HttpPost]
        public JsonResult SaveProductGroup(MaintenanceProductGroupViewModel maintenanceProductGroupViewModel)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceProductGroupService.SaveProductGroup(maintenanceProductGroupViewModel);
                _maintenanceProductGroupService.GetProductGroup(maintenanceProductGroupViewModel);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                exceptionMessage = ex.Message;
                isSuccess = false;
            }

            return Json(new { IsSuccess = isSuccess, View = RenderView.RenderRazorViewToString(this, "_ProductGroupTable", maintenanceProductGroupViewModel) });//Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage });
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult UpdateProductGroup(string req)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;
            MaintenanceProductGroupViewModel maintenanceProductGroupViewModel = new MaintenanceProductGroupViewModel();
            try
            {
                ProductGroupViewModel ProductGroupViewModel = new ProductGroupViewModel();
                ProductGroupViewModel = JsonConvert.DeserializeObject<ProductGroupViewModel>(req, new IsoDateTimeConverter { DateTimeFormat = "yyyy-dd-MMTHH:mm:ss" });
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceProductGroupService.UpdateProductGroup(ProductGroupViewModel);
                _maintenanceProductGroupService.GetProductGroup(maintenanceProductGroupViewModel);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                exceptionMessage = ex.Message;
                isSuccess = false;
            }

            return Json(new { IsSuccess = isSuccess, View = RenderView.RenderRazorViewToString(this, "_ProductGroupTable", maintenanceProductGroupViewModel) });// Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage });
        }
        #endregion

    }
}