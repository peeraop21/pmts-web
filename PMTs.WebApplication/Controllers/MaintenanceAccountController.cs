using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PMTs.DataAccess;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.MaintenanceAccount;
using PMTs.DataAccess.Repository;
using PMTs.DataAccess.Shared;
using PMTs.Logs.Logger;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Services;
using PMTs.WebApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Controllers
{
    [SessionTimeout]
    public class MaintenanceAccountController : Controller
    {
        private readonly IMaintenanceAccountService _maintenanceAccountService;
        private readonly IExtensionService _extensionService;

        public MaintenanceAccountController(IMaintenanceAccountService maintenanceAccountService, IExtensionService extensionService)
        {
            // Initialize Service
            _maintenanceAccountService = maintenanceAccountService;
            _extensionService = extensionService;
        }

        [SessionTimeout]
        public IActionResult Index()
        {
            MaintenanceAccountViewModel maintenanceAccountViewModel = new MaintenanceAccountViewModel();
            try
            {

                var userSessionModel = SessionExtentions.GetSession<UserSessionModel>(HttpContext.Session, "UserSessionModel");

                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceAccountService.GetAccount(maintenanceAccountViewModel);
                maintenanceAccountViewModel.Lst_SaleOrg = _maintenanceAccountService.GetListSaleOrg();
                //maintenanceAccountViewModel.Lst_SaleOrg.Select = "0252";
                maintenanceAccountViewModel.Lst_Plant = _maintenanceAccountService.GetListPlant();
                //maintenanceAccountViewModel.AccountViewModel.SaleOrg = "0252";
                // maintenanceAccountViewModel.AccountViewModel.FactoryCode = userSessionModel.FactoryCode;

                ViewBag.SaleOrg = userSessionModel.SaleOrg;
                ViewBag.FactoryCode = userSessionModel.FactoryCode;

                //ViewBag.ImageData = imageDataURL;
                //ViewBag.Base64String = "data:image/png;base64," + Convert.ToBase64String(maintenanceAccountViewModel.AccountViewModel.PictureUser, 0, maintenanceAccountViewModel.AccountViewModel.PictureUser.Length);

                ViewBag.UserDomain = Globals.Domain();
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }
            return View(maintenanceAccountViewModel);
        }

        [SessionTimeout]
        public PartialViewResult UpdateAccountTable()
        {
            MaintenanceAccountViewModel maintenanceAccountViewModel = new MaintenanceAccountViewModel();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceAccountService.GetAccount(maintenanceAccountViewModel);
                maintenanceAccountViewModel.Lst_SaleOrg = _maintenanceAccountService.GetListSaleOrg();
                maintenanceAccountViewModel.Lst_Plant = _maintenanceAccountService.GetListPlant();
                ViewBag.UserDomain = Globals.Domain();
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }
            return PartialView("_AccountTable", maintenanceAccountViewModel);

        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult SaveAccount(MaintenanceAccountViewModel maintenanceAccountViewModel, IFormFile PictureUser)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceAccountService.SaveAccount(maintenanceAccountViewModel, PictureUser);

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
        [HttpPost]
        public JsonResult UpdateAccount(AccountViewModel AccountViewModel, IFormFile PictureUser, string iUserPath)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceAccountService.UpdateAccount(AccountViewModel, PictureUser, iUserPath);
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

    }

}
