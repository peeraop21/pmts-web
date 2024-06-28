using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PMTs.DataAccess;
using PMTs.DataAccess.ComplexModel;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.Repository;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Tracing;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services
{
    [TraceAspect]
    public class LoginService : ILoginService
    {
        IHttpContextAccessor _httpContextAccessor;

        private readonly IAccountAPIRepository _accountAPIRepository;
        private readonly IMainMenusAPIRepository _mainMenusAPIRepository;
        private readonly ISubMenusAPIRepository _subMenusAPIRepository;
        private readonly IAuthenticationService _ldapAuthenticationService; //tassanai
        private readonly ICompanyProfileAPIRepository _companyProfileAPIRepository;

        private readonly string _username;
        private readonly string _saleOrg;
        private readonly string _factoryCode;
        private readonly string _token;

        public LoginService(IHttpContextAccessor httpContextAccessor,
            IAccountAPIRepository accountAPIRepository,
            IMainMenusAPIRepository mainMenusAPIRepository,
            ISubMenusAPIRepository subMenusAPIRepository,
              IAuthenticationService ldapAuthenticationService,
              ICompanyProfileAPIRepository companyProfileAPIRepository)

        {
            _httpContextAccessor = httpContextAccessor;
            _accountAPIRepository = accountAPIRepository;
            _mainMenusAPIRepository = mainMenusAPIRepository;
            _subMenusAPIRepository = subMenusAPIRepository;
            _ldapAuthenticationService = ldapAuthenticationService;
            _companyProfileAPIRepository = companyProfileAPIRepository;

        }
        public void GetLogin(ref LoginViewModel loginViewModel)
        {
            //ห้ามเเก้ ไม่เกี่ยกับ jwt


            var hashPassword = EncryptionHelper.GenerateMD5(loginViewModel.Password);
            var passwordAd = loginViewModel.Password;
            loginViewModel.Password = hashPassword;

            // get เพื่อ 
            GetUser(ref loginViewModel);

            if (loginViewModel.UserDomain != "")
            {



                loginViewModel.Password = passwordAd;
                var userAdSCG = new UserADSCG();
                GetUserADSCG(loginViewModel);
                //GetUser(ref loginViewModel);
            }
            else
            {
                loginViewModel.Password = hashPassword;
                loginViewModel.UserDomain = "";
                //  GetUser(ref loginViewModel);
            }
            // }
        }

        public void GetUser(ref LoginViewModel loginViewModel)
        {
            //ห้ามเเก้ ไม่เกี่ยกับ jwt
            string LoginJsonString = JsonConvert.SerializeObject(loginViewModel);
            UserSessionModel userSessionModel = new UserSessionModel();
            userSessionModel = JsonConvert.DeserializeObject<UserSessionModel>(_accountAPIRepository.GetLogin(LoginJsonString, _token));
            loginViewModel.Id = userSessionModel.Id;
            loginViewModel.DefaultRoleId = userSessionModel.DefaultRoleId;
            loginViewModel.UserDomain = userSessionModel.UserDomain;
            loginViewModel.PictureUser = userSessionModel.PictureUser;

            var companyProfile = JsonConvert.DeserializeObject<CompanyProfile>(_companyProfileAPIRepository.GetFirstCompanyProfileBySaleOrg(userSessionModel.FactoryCode, userSessionModel.SaleOrg, _token));
            userSessionModel.BusinessGroup = companyProfile.BusinessGroup;
            userSessionModel.PlanningProgram = companyProfile.PlanningProgram;


            SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "UserSessionModel", userSessionModel);
            SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "UserName", userSessionModel.UserName);
            // SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "NameSurname", userSessionModel.FirstNameTh +"-"+ userSessionModel.;
            List<SubMenus> subMenuList;
            List<MainMenus> menuList;
            menuList = JsonConvert.DeserializeObject<List<MainMenus>>(_mainMenusAPIRepository.GetMainMenuByRoleId(_factoryCode, userSessionModel.DefaultRoleId));
            subMenuList = JsonConvert.DeserializeObject<List<SubMenus>>(_subMenusAPIRepository.GetSubMenusList(_factoryCode, _token));


            List<SubMenus> subMenuListByRole;
            subMenuListByRole = JsonConvert.DeserializeObject<List<SubMenus>>(_subMenusAPIRepository.GetSubMenusListBYRole(_factoryCode, userSessionModel.DefaultRoleId, _token));
            List<MenuViewModel> menuViewModelList = new List<MenuViewModel>();
            menuList.ForEach(i =>
            {
                menuViewModelList.Add(new MenuViewModel
                {
                    Id = i.Id,
                    MenuNameEn = i.MenuNameEn,
                    MenuNameTh = i.MenuNameTh,
                    Controller = i.Controller,
                    Action = i.Action,
                    IconName = i.IconName,
                    SubMenuList = subMenuListByRole.Where(w => w.MainMenuId == i.Id).ToList()
                });
            });
            SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "menuList", menuViewModelList);


        }
        public void GetUserADSCG(LoginViewModel loginViewModel)
        {

            UserLdap model = new UserLdap();

            ////var checkloginldap = _ldapAuthenticationService.Login(loginViewModel.UserName, loginViewModel.Password);
            ////model.DisplayName = checkloginldap.DisplayName;
            ////ห้ามเเก้ ไม่เกี่ยกับ jwt
            //string username = loginViewModel.UserName;
            //string pass = loginViewModel.Password;
            //string userpass = username + ":" + pass;
            //string jsonstring = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(userpass));
            //string LoginJsonString = JsonConvert.SerializeObject(jsonstring);
            UserADSCG userADSCG = new UserADSCG();
            try
            {
                //userADSCG = JsonConvert.DeserializeObject<UserADSCG>(_accountAPIRepository.GetLoginAD(userpass, _token));
                //userADSCG.sAMAcountName = userADSCG.sAMAcountName;
                var checkloginldap = _ldapAuthenticationService.Login(loginViewModel.UserName, loginViewModel.Password);
                userADSCG.sAMAcountName = checkloginldap.UserName;
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public void GetUserTIP(ref UserTIP userTIP)
        {
            var userTipLogin = _accountAPIRepository.GetLoginTip(userTIP.UrlApi, JsonConvert.SerializeObject(userTIP));
            if (userTipLogin != null)
            {
                userTIP.Token = userTipLogin.Token;
            }
        }
    }
}
