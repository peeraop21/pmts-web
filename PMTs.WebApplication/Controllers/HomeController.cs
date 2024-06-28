using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PMTs.DataAccess;
using PMTs.DataAccess.ModelPresale;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.MaintenanceAccount;
using PMTs.DataAccess.Repository;
using PMTs.DataAccess.Shared;
using PMTs.Logs.Logger;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Models;
using PMTs.WebApplication.Services;
using PMTs.WebApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Controllers
{
    [SessionTimeout]
    public class HomeController : Controller
    {
        private readonly IMaintenanceAccountService _maintenanceAccountService;
        private static PresaleContext _presaleContext;


        public HomeController(PresaleContext presaleContext, IMaintenanceAccountService maintenanceAccountService)
        {
            _maintenanceAccountService = maintenanceAccountService;
            _presaleContext = presaleContext;
        }
        [SessionTimeout]
        public IActionResult Index(int Id)

        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var userSessionModel = SessionExtentions.GetSession<UserSessionModel>(HttpContext.Session, "UserSessionModel");
                var userId = userSessionModel.Id;
                SessionExtentions.SetSession(HttpContext.Session, "TransactionDataModel", null);
                AccountViewModel accountViewModel = new AccountViewModel();
                //MaintenanceAccountViewModel maintenanceAccountViewModel = new MaintenanceAccountViewModel();
                accountViewModel.Id = userId;
                //_maintenanceAccountService.GetAccountDetail(accountViewModel);


                accountViewModel = JsonConvert.DeserializeObject<AccountViewModel>(AccountAPIRepository.GetAccountById(userSessionModel.FactoryCode, userId, userSessionModel.Token));

                var masterRoleList = JsonConvert.DeserializeObject<List<MasterRoleList>>(MasterRoleAPIRepository.GetMasterRoleListdata(userSessionModel.FactoryCode, userSessionModel.Token));
                accountViewModel.RoleList = masterRoleList;
                //accountViewModel.RoleList = masterRoleList;
                ViewBag.UserDomain = Globals.Domain();
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return View(accountViewModel);
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return RedirectToAction("index", "Login");
            }
        }
    }
}
