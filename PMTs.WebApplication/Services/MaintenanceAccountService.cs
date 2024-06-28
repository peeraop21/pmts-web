using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.MaintenanceAccount;
using PMTs.DataAccess.Repository;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Shared;
using PMTs.DataAccess.Tracing;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services
{
    [TraceAspect]
    public class MaintenanceAccountService : IMaintenanceAccountService
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper mapper;
        private readonly IAccountAPIRepository _accountAPIRepository;
        private readonly IMasterRoleAPIRepository _masterRoleAPIRepository;
        private readonly IMasterUserAPIRepository _masterUserAPIRepository;
        private readonly ICompanyProfileAPIRepository _companyProfileAPIRepository;
        private readonly IExtensionService _extensionService;

        private string _username;
        private string _saleOrg;
        private string _factoryCode;
        private string _token;
        private int _roldId;


        public MaintenanceAccountService(IHttpContextAccessor httpContextAccessor,
            IAccountAPIRepository accountAPIRepository,
            IMasterRoleAPIRepository masterRoleAPIRepository,
            IMasterUserAPIRepository masterUserAPIRepository,
            ICompanyProfileAPIRepository companyProfileAPIRepository,
            IExtensionService extensionService,
            IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;

            _accountAPIRepository = accountAPIRepository;
            _masterRoleAPIRepository = masterRoleAPIRepository;
            _masterUserAPIRepository = masterUserAPIRepository;
            _companyProfileAPIRepository = companyProfileAPIRepository;
            _extensionService = extensionService;

            var userSessionModel = SessionExtentions.GetSession<UserSessionModel>(_httpContextAccessor.HttpContext.Session, "UserSessionModel");
            if (userSessionModel != null)
            {
                _username = userSessionModel.UserName;
                _saleOrg = userSessionModel.SaleOrg;
                _factoryCode = userSessionModel.FactoryCode;
                _roldId = userSessionModel.DefaultRoleId;
                _token = userSessionModel.Token;
            }

            this.mapper = mapper;
        }

        public void ChangePassword(ChangePasswordViewModel changePasswordViewModel)
        {
            var loginViewModel = new LoginViewModel
            {
                UserName = changePasswordViewModel.Username,
                Password = EncryptionHelper.GenerateMD5(changePasswordViewModel.OldPassword),
                UserDomain = ""
            };

            string LoginJsonString = JsonConvert.SerializeObject(loginViewModel);

            var masterUser = JsonConvert.DeserializeObject<MasterUser>(_accountAPIRepository.CheckGetLogin(LoginJsonString, _token));
            if (masterUser == null)
            {
                throw new Exception("User doesn't exist.");
            }
            else
            {
                masterUser.UpdatedDate = DateTime.Now;
                masterUser.LastPasswordChangeDate = DateTime.Now;
                masterUser.Password = EncryptionHelper.GenerateMD5(changePasswordViewModel.NewPassword);
                masterUser.IsLockedOut = false;
                masterUser.NumberOfLogins = 0;
                masterUser.MustChangePassword = false;
                masterUser.AppName = "PMTs";
                masterUser.LastLoginDate = DateTime.Now;


                var masterUserParentModel = new ParentModel
                {
                    AppName = Globals.AppNameEncrypt,
                    SaleOrg = _saleOrg,
                    PlantCode = _factoryCode,
                    MasterUser = masterUser,
                };

                _masterUserAPIRepository.UpdateMasterUser(JsonConvert.SerializeObject(masterUserParentModel), _token);
            }
        }
        public void GetAccount(MaintenanceAccountViewModel maintenanceAccouuntViewModel)
        {
            var accountList = new List<MasterUser>();
            // Convert Json String to List Object
            //if (_roldId == 1)
            //{
            //    accountList = JsonConvert.DeserializeObject<List<MasterUser>>(_accountAPIRepository.GetAllAccountList(_token));
            //}
            //else
            //{
            accountList = JsonConvert.DeserializeObject<List<MasterUser>>(_accountAPIRepository.GetaccountList(_factoryCode, _token));
            //}


            // Convert List Object to List Object View Model
            var AccountModelViewList = mapper.Map<List<MasterUser>, List<AccountViewModel>>(accountList);

            maintenanceAccouuntViewModel.AccountDataList = AccountModelViewList;

            //maintenanceAccouuntViewModel.AccountViewModel.SaleOrg = "0258";
            //string imageDataURL = string.Format("data:image/png;base64,{0}", maintenanceAccountViewModel.AccountDataList.);

            var masterRoleList = JsonConvert.DeserializeObject<List<MasterRoleList>>(_masterRoleAPIRepository.GetMasterRoleList(_factoryCode, _token));
            maintenanceAccouuntViewModel.RoleList = masterRoleList;
            //maintenanceAccouuntViewModel.AccountViewModel.SaleOrg = "0252";
        }

        public void SaveAccount(MaintenanceAccountViewModel model, IFormFile PictureUser)
        {
            //check usename
            if (IsDuplicateUsername(model.AccountViewModel.UserName, model.AccountViewModel.FactoryCode))
            {
                throw new Exception("This username already exists or your factory already active user.");
            }

            if (string.IsNullOrEmpty(model.AccountViewModel.UserDomain))
            {
                // haspassword 
                var hashPassword = EncryptionHelper.GenerateMD5(model.AccountViewModel.Password);
                model.AccountViewModel.Password = hashPassword;
            }

            model.AccountViewModel.CreatedBy = _username;
            model.AccountViewModel.CreatedDate = DateTime.Now;
            model.AccountViewModel.IsFlagDelete = false;
            model.AccountViewModel.IsLockedOut = false;
            model.AccountViewModel.MustChangePassword = false;
            model.AccountViewModel.IsReceiveMail = false;
            model.AccountViewModel.LastPasswordChangeDate = DateTime.Now;
            model.AccountViewModel.UpdatedDate = DateTime.Now;
            model.AccountViewModel.UpdatedBy = _username;
            model.AccountViewModel.UserDomain = string.IsNullOrEmpty(model.AccountViewModel.UserDomain) ? "" : model.AccountViewModel.UserDomain;
            model.AccountViewModel.NumberOfLogins = 0;
            model.AccountViewModel.AppName = "PMTs";
            //model.AccountViewModel.PictureUser = ConvertPictureToBase64._ConvertPictureToBase64(PictureUser);

            //var PictureUserToBase64 = "";
            //using (var ms = new MemoryStream())
            //{
            //    PictureUser.CopyTo(ms);
            //    var fileBytes = ms.ToArray();
            //    PictureUserToBase64 = Convert.ToBase64String(fileBytes);
            //    // act on the Base64 data

            //}

            if (PictureUser != null)
            {
                string base64StringImage = _extensionService.ConvertImageToBase64(PictureUser);

                //var aa = !string.IsNullOrEmpty(PictureUser.ToString()) ? ConvertPictureToBase64._ConvertPictureToBase64(PictureUser.ToString()) : string.Empty;
                model.AccountViewModel.PictureUser = base64StringImage;
            }

            ParentModel accountModel = new ParentModel();

            accountModel.MasterUser = mapper.Map<AccountViewModel, MasterUser>(model.AccountViewModel);
            string accountListJsonString = JsonConvert.SerializeObject(accountModel);

            _accountAPIRepository.SaveAccount(accountListJsonString, _token);
        }

        public void UpdateAccount(AccountViewModel model, IFormFile PictureUser, string iUserPath)
        {
            //get MasteruseById
            //AccountViewModel accountViewModel;
            //accountViewModel = JsonConvert.DeserializeObject<AccountViewModel>(AccountAPIRepository.GetAccountById(_factoryCode, model.Id,_token));
            //accountViewModel.UserName = model.UserName;
            //accountViewModel.UserDomain = model.UserDomain;
            //accountViewModel.SaleOrg = model.SaleOrg;
            //accountViewModel.FactoryCode = model.FactoryCode;
            //accountViewModel.FirstNameTh = model.FirstNameTh;
            //accountViewModel.LastNameTh = model.LastNameTh;
            //accountViewModel.FirstNameEn = model.FirstNameEn;
            //accountViewModel.LastNameEn = model.LastNameEn;
            //accountViewModel.Email = model.Email;
            //accountViewModel.Telephone = model.Telephone;
            //accountViewModel.MobileNo = model.MobileNo;
            //accountViewModel.PasswordHint = model.PasswordHint;
            //accountViewModel.Comment = model.Comment;
            //accountViewModel.UpdatedBy = _username;
            //accountViewModel.IsFlagDelete = model.IsFlagDelete;
            //accountViewModel.UpdatedDate = DateTime.Now;

            model.UpdatedBy = _username;
            model.UpdatedDate = DateTime.Now;
            model.UserDomain = string.IsNullOrEmpty(model.UserDomain) ? "" : model.UserDomain;

            //check recieve new password
            if (model.IsChangePassword && !string.IsNullOrEmpty(model.NewPassword) && !string.IsNullOrEmpty(model.NewPassword))
            {
                //model.NewPassword = "kiwiplan_vci";
                var hashPassword = EncryptionHelper.GenerateMD5(model.NewPassword);
                if (IsOldPassword(hashPassword, model.FactoryCode))
                {
                    throw new Exception($"The password has already been used in {model.FactoryCode}.");
                }

                model.Password = hashPassword;
                model.LastPasswordChangeDate = DateTime.Now;

                model.MustChangePassword = false;
                model.IsFlagDelete = false;
                model.IsLockedOut = false;
                model.MustChangePassword = false;
                model.LastPasswordChangeDate = DateTime.Now;
                model.NumberOfLogins = 0;
            }

            if (!model.IsLockedOut)
            {
                model.NumberOfLogins = 0;
                model.IsLockedOut = false;
            }
            if (PictureUser != null)
            {
                string base64StringImage = _extensionService.ConvertImageToBase64(PictureUser);

                //var aa = !string.IsNullOrEmpty(PictureUser.ToString()) ? ConvertPictureToBase64._ConvertPictureToBase64(PictureUser.ToString()) : string.Empty;
                model.PictureUser = base64StringImage;
            }
            if (iUserPath == "Remove")
            {
                model.PictureUser = null;
            }
            ParentModel accountModel = new ParentModel();
            accountModel.AppName = Globals.AppNameEncrypt;
            accountModel.SaleOrg = _saleOrg;
            accountModel.FactoryCode = _factoryCode;
            accountModel.AppName = "PMTs";

            model.AppName = "PMTs";

            var masterUser = mapper.Map<AccountViewModel, MasterUser>(model);
            accountModel.MasterUser = masterUser;
            string jsonString = JsonConvert.SerializeObject(accountModel);

            _accountAPIRepository.UpdateAccount(jsonString, _token);

            //ParentModel accountModel = new ParentModel();

            //accountModel.Customer = mapper.Map<CustomerViewModel, Customer>(customerViewModel);

            //string jsonString = JsonConvert.SerializeObject(customerModel);

            //_customerAPIRepository.UpdateCustomer(jsonString);
        }

        private bool IsDuplicateUsername(string username, string factoryCode)
        {
            var masterUsers = JsonConvert.DeserializeObject<List<MasterUser>>(_accountAPIRepository.GetAllAccountList(_token));
            var existUsernames = masterUsers.FirstOrDefault(e => e.UserName == username);
            var factoryIsActive = masterUsers.Where(e => e.FactoryCode == factoryCode && e.IsFlagDelete == false);

            if (existUsernames != null || existUsernames != null)
            {
                return true;
            }

            return false;
        }

        private bool IsOldPassword(string password, string factoryCode)
        {
            var result = false;

            var existUsernames = new MasterUser();

            if (_roldId == 1)
            {
                existUsernames = JsonConvert.DeserializeObject<List<MasterUser>>(_accountAPIRepository.GetAllAccountList(_token)).FirstOrDefault(e => e.Password == password);
            }
            else
            {
                existUsernames = JsonConvert.DeserializeObject<List<MasterUser>>(_accountAPIRepository.GetaccountList(factoryCode, _token)).FirstOrDefault(e => e.Password == password);
            }

            if (existUsernames != null)
            {
                result = true;
            }

            return result;
        }

        public List<SelectListItem> GetListSaleOrg()
        {
            var tmp = JsonConvert.DeserializeObject<List<CompanyProfile>>(_companyProfileAPIRepository.GetCompanyProfileList(_factoryCode, _token));
            List<SelectListItem> lst = new List<SelectListItem>();
            var tmpselect = tmp.Select(x => new { x.SaleOrg, x.ShortName }).Distinct();
            foreach (var item in tmpselect)
            {
                SelectListItem op = new SelectListItem();
                op.Value = item.SaleOrg;
                op.Text = item.SaleOrg + " " + item.ShortName;
                lst.Add(op);
            }
            return lst;
        }

        public List<SelectListItem> GetListPlant()
        {
            var tmp = JsonConvert.DeserializeObject<List<CompanyProfile>>(_companyProfileAPIRepository.GetCompanyProfileList(_factoryCode, _token));
            List<SelectListItem> lst = new List<SelectListItem>();
            var tmpselect = tmp.Select(x => new { x.Plant, x.ShortName }).Distinct();
            foreach (var item in tmpselect)
            {
                SelectListItem op = new SelectListItem();
                op.Value = item.Plant;
                op.Text = item.Plant + " " + item.ShortName;
                lst.Add(op);
            }
            return lst;
        }

        //Tassanai Update 13/07/2020
        // public void GetAccountDetail(AccountViewModel accountViewModel)
        // {
        // accountViewModel = JsonConvert.DeserializeObject<AccountViewModel>(_accountAPIRepository.GetAccountById(_factoryCode, accountViewModel.Id, _token));

        //var masterRoleList = JsonConvert.DeserializeObject<List<MasterRoleList>>(_masterRoleAPIRepository.GetMasterRoleList(_factoryCode, _token));
        ///accountViewModel.RoleList = masterRoleList;
        //MaintenanceAccountViewModel m = new MaintenanceAccountViewModel();
        //m.AccountViewModel = accountViewModel;
        //  }
    }
}
