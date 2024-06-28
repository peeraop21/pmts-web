using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using PMTs.DataAccess;
using PMTs.DataAccess.ComplexModel;
using PMTs.DataAccess.ComplexModel;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.Repository;
using PMTs.DataAccess.Shared;
using PMTs.Logs.Logger;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Services;
using PMTs.WebApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Controllers
{
    public class LoginController : Controller
    {

        private readonly ILoginService _loginService;
        private readonly IExtensionService _extensionService;
        private readonly IStringLocalizer<LoginController> _localizer;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthenticationService _ldapAuthenticationService; //tassanai
        public LoginController(ILoginService loginService, IExtensionService extensionService, IStringLocalizer<LoginController> localizer, IHttpContextAccessor httpContextAccessor,
             IAuthenticationService ldapAuthenticationService)
        {
            _extensionService = extensionService;
            _loginService = loginService;
            _localizer = localizer;
            _httpContextAccessor = httpContextAccessor;
            _ldapAuthenticationService = ldapAuthenticationService;

        }

        public IActionResult Index()
        {
            try
            {


                _httpContextAccessor.HttpContext.Session.Clear();
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                ViewBag.UserDomain = Globals.Domain();
                ViewBag.titleMessage = "";
                ViewBag.errorMassage = "";
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }
            return View();
        }

        [HttpPost]
        public IActionResult Index(LoginViewModel LoginViewModel)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _loginService.GetLogin(ref LoginViewModel);
                //test Ldap START

                //UserLdap model = new UserLdap();

                //var checkloginldap = _ldapAuthenticationService.Login(LoginViewModel.UserName, LoginViewModel.Password);
                ////_loginService.getalluserbyad(login, _configuration.getsection("webadscg").value);
                //if (checkloginldap == null)
                //{
                //    throw;
                //}





                //test Ldap END

                if (LoginViewModel.Id == 0)
                {
                    //ViewBag.errorMassage = "Username Or Password Incorrect!!";// UsernameOrPasswordIncorrect
                    ViewBag.errorMassage = _localizer["UsernameOrPasswordIncorrect"];

                    ViewBag.UserDomain = Globals.Domain();
                }
                else
                {
                    if (LoginViewModel.DefaultRoleId != 1 && LoginViewModel.DefaultRoleId != 2)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return RedirectToAction("ViewListProduct", "MaintenanceProductInfo");
                    }
                }

                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                ViewBag.titleMessage = "Login Failed";
                ViewBag.errorMassage = "Username Or Password Incorrect!!";

                if (ex.Message.Equals("Locked User"))
                {
                    ViewBag.titleMessage = "Locked Out";
                    ViewBag.errorMassage = "Too many failed login attemps! Please contact admin to unlock.";
                }
                else if (ex.Message.Equals("Change Password"))
                {
                    ViewBag.titleMessage = "Change Password";
                    ViewBag.errorMassage = "Account has been locked! Please contact admin to unlock or change your password.";
                }
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                ViewBag.UserDomain = Globals.Domain();
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }

            TempData["Username"] = LoginViewModel.UserName;

            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            _httpContextAccessor.HttpContext.Session.Clear();
            return RedirectToAction(nameof(Index), "Login");
        }
    }
}