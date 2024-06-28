using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView.MaintenanceMapCost;
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
    [SessionTimeout]
    public class MaintenanceMapCostController : Controller
    {
        public readonly IMaintenanceMapCostService _maintenanceMapCostService;

        public MaintenanceMapCostController(IMaintenanceMapCostService maintenanceMapCostService)
        {
            this._maintenanceMapCostService = maintenanceMapCostService;
        }

        #region MapCost
        [SessionTimeout]
        public IActionResult Index()
        {
            MaintenanceMapCostViewModel maintenanceMapCostViewModel = new MaintenanceMapCostViewModel();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceMapCostService.GetMapCost(ref maintenanceMapCostViewModel);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                //error
            }

            return View(maintenanceMapCostViewModel);
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult CreateMaintenanceMapCost(MaintenanceMapCostViewModel maintenanceMapCostViewModel)
        {
            bool isSuccess = false;
            string message = "";

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                //do something
                _maintenanceMapCostService.CreateMapCost(maintenanceMapCostViewModel);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                //message = ex.Message;
                message = ex.Message.ToString().Contains("ExecuteReader") ? "Can't add column to board combine acc!" : "Can't add new cost field!";
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = message, ActionType = "create" });
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult EditMaintenanceMapCost(MaintenanceMapCostViewModel maintenanceMapCostViewModel)
        {
            bool isSuccess = false;
            string message = "";

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                //do something
                _maintenanceMapCostService.UpdateMapCost(maintenanceMapCostViewModel);

                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                message = ex.Message;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = message, ActionType = "edit" });
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult DeleteMaintenanceMapCost(string id)
        {
            bool isSuccess = false;
            string message = "";

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                //do something
                _maintenanceMapCostService.DeleteMapCost(id);

                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                message = ex.Message;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = message, ActionType = "delete" });
        }

        #endregion

        #region HierarchyLv3
        [SessionTimeout]
        public IActionResult MaintenanceHierarchyLv3()
        {
            MaintenanceMapCostViewModel maintenanceMapCostViewModel = new MaintenanceMapCostViewModel();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceMapCostService.GetHierarchyLv3(ref maintenanceMapCostViewModel);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                //error
            }

            return View(maintenanceMapCostViewModel);
        }

        [SessionTimeout]
        [HttpPost]
        public IActionResult SaveHierarchyLv3(HierarchyCreateModel hierarchyCreateModel)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;
            MaintenanceMapCostViewModel maintenanceMapCostViewModel = new MaintenanceMapCostViewModel();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceMapCostService.SaveHierarchyLv3(ref maintenanceMapCostViewModel, hierarchyCreateModel);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                isSuccess = true;
            }
            catch (Exception ex)
            {
                maintenanceMapCostViewModel = new MaintenanceMapCostViewModel();
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                isSuccess = false;
                //error
                return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_HierarchyLv3Table", maintenanceMapCostViewModel) });
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_HierarchyLv3Table", maintenanceMapCostViewModel) });
        }

        [SessionTimeout]
        [HttpPost]
        public IActionResult UpdateHierarchyLv3(HierarchyCreateModel hierarchyCreateModel)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;
            MaintenanceMapCostViewModel maintenanceMapCostViewModel = new MaintenanceMapCostViewModel();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceMapCostService.UpdateHierarchyLv3(ref maintenanceMapCostViewModel, hierarchyCreateModel);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                isSuccess = true;
            }
            catch (Exception ex)
            {
                maintenanceMapCostViewModel = new MaintenanceMapCostViewModel();
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                isSuccess = false;
                //error
                return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_HierarchyLv3Table", maintenanceMapCostViewModel) });
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_HierarchyLv3Table", maintenanceMapCostViewModel) });
        }

        #endregion

        #region HierarchyLv4
        [SessionTimeout]
        public IActionResult MaintenanceHierarchyLv4()
        {
            MaintenanceMapCostViewModel maintenanceMapCostViewModel = new MaintenanceMapCostViewModel();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceMapCostService.GetHierarchyLv4(ref maintenanceMapCostViewModel);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                //error
            }

            return View(maintenanceMapCostViewModel);
        }

        [SessionTimeout]
        [HttpPost]
        public IActionResult SaveHierarchyLv4(HierarchyCreateModel hierarchyCreateModel)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;
            MaintenanceMapCostViewModel maintenanceMapCostViewModel = new MaintenanceMapCostViewModel();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceMapCostService.SaveHierarchyLv4(ref maintenanceMapCostViewModel, hierarchyCreateModel);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                isSuccess = true;
            }
            catch (Exception ex)
            {
                maintenanceMapCostViewModel = new MaintenanceMapCostViewModel();
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                isSuccess = false;
                //error
                return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_HierarchyLv4Table", maintenanceMapCostViewModel) });
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_HierarchyLv4Table", maintenanceMapCostViewModel) });
        }

        [SessionTimeout]
        [HttpPost]
        public IActionResult UpdateHierarchyLv4(HierarchyCreateModel hierarchyCreateModel)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;
            MaintenanceMapCostViewModel maintenanceMapCostViewModel = new MaintenanceMapCostViewModel();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceMapCostService.UpdateHierarchyLv4(ref maintenanceMapCostViewModel, hierarchyCreateModel);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                isSuccess = true;
            }
            catch (Exception ex)
            {
                maintenanceMapCostViewModel = new MaintenanceMapCostViewModel();
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                isSuccess = false;
                //error
                return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_HierarchyLv4Table", maintenanceMapCostViewModel) });
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_HierarchyLv4Table", maintenanceMapCostViewModel) });
        }
        #endregion
    }
}