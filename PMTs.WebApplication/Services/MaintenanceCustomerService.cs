using AutoMapper;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.MaintenanceCustomer;
using PMTs.DataAccess.Repository;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Shared;
using PMTs.DataAccess.Tracing;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMTs.WebApplication.Services
{
    [TraceAspect]
    public class MaintenanceCustomerService : IMaintenanceCustomerService
    {

        IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper mapper;
        private readonly ICustomerAPIRepository _customerAPIRepository;
        private readonly ICustShipToAPIRepository _custShipToAPIRepository;
        private readonly IProductGroupAPIRepository _productGroupAPIRepository;
        private readonly IQaItemsAPIRepository _qaItemsAPIRepository;
        private readonly ICompanyProfileAPIRepository _companyProfileAPIRepository;
        //tassanai
        private readonly ITagPrintSORepository _tagPrintSORepository;

        private readonly string _username;
        private readonly string _saleOrg;
        private readonly string _factoryCode;
        private readonly string _token;

        public MaintenanceCustomerService(IHttpContextAccessor httpContextAccessor,
            ICustomerAPIRepository customerAPIRepository,
            ICustShipToAPIRepository custShipToAPIRepository,
            IProductGroupAPIRepository productGroupAPIRepository,
            IQaItemsAPIRepository qaItemsAPIRepository,
            ICompanyProfileAPIRepository companyProfileAPIRepository,
            ITagPrintSORepository tagPrintSORepository,
            IMapper mapper)
        {
            // Initialize HttpContext
            _httpContextAccessor = httpContextAccessor;

            // Initialize Repository
            _customerAPIRepository = customerAPIRepository;
            _custShipToAPIRepository = custShipToAPIRepository;
            _productGroupAPIRepository = productGroupAPIRepository;
            _qaItemsAPIRepository = qaItemsAPIRepository;
            _companyProfileAPIRepository = companyProfileAPIRepository;
            _tagPrintSORepository = tagPrintSORepository;

            // Initialize SaleOrg and PlantCode from Session
            var userSessionModel = SessionExtentions.GetSession<UserSessionModel>(_httpContextAccessor.HttpContext.Session, "UserSessionModel");
            if (userSessionModel != null)
            {
                _username = userSessionModel.UserName;
                _saleOrg = userSessionModel.SaleOrg;
                _factoryCode = userSessionModel.FactoryCode;
                _token = userSessionModel.Token;
            }

            this.mapper = mapper;
        }
        public void GetCustomer(ref MaintenanceCustomerViewModel CustomerModelList, string typeSearch, string keySearch)
        {
            var CustomerShipToList = JsonConvert.DeserializeObject<List<CustomerViewModel>>(_customerAPIRepository.GetCustomerShipTo(_factoryCode, _token));


            if (String.IsNullOrEmpty(keySearch) || string.IsNullOrWhiteSpace(keySearch))
            {
                CustomerModelList.CustomerViewModelList = CustomerShipToList;
            }
            else
            {
                if (typeSearch == "Customer_Name")
                {
                    CustomerModelList.CustomerViewModelList = JsonConvert.DeserializeObject<List<CustomerViewModel>>(_customerAPIRepository.GetCustomerShipToByCustname(_factoryCode, keySearch, _token));
                }
                if (typeSearch == "Customer_Code")
                {
                    CustomerModelList.CustomerViewModelList = JsonConvert.DeserializeObject<List<CustomerViewModel>>(_customerAPIRepository.GetCustomerShipToByCustCode(_factoryCode, keySearch, _token));
                }
                if (typeSearch == "Customer_Id")
                {
                    CustomerModelList.CustomerViewModelList = JsonConvert.DeserializeObject<List<CustomerViewModel>>(_customerAPIRepository.GetCustomerShipToByCustId(_factoryCode, keySearch, _token));
                }
            }
            CustomerModelList.CustShipToList = JsonConvert.DeserializeObject<List<CustShipTo>>(_custShipToAPIRepository.GetCustShipToList(_factoryCode, _token));
            CustomerModelList.ProductGroupList = JsonConvert.DeserializeObject<List<ProductGroup>>(_productGroupAPIRepository.GetProductGroupList(_factoryCode, _token));
            CustomerModelList.QaItems = JsonConvert.DeserializeObject<List<QaItems>>(_qaItemsAPIRepository.GetQaItems(_token)).OrderBy(q => q.TestName).Select(q => q.TestName).Distinct().ToList();
            // CustomerModelList.TagPrintSO = JsonConvert.DeserializeObject<List<TagPrintSO>>(_tagPrintSORepository.GetTagPrintSO(_factoryCode,_token).Select(q => q.DataText));
            //Tassanai
            CustomerModelList.TagPrintSO = JsonConvert.DeserializeObject<List<TagPrintSo>>(_tagPrintSORepository.GetTagPrintSO(_factoryCode, _token)).OrderBy(q => q.Id).Select(q => q.DataText).Distinct().ToList(); ;
        }

        //public void GetCustomer(MaintenanceCustomerViewModel maintenanceCustomerViewModel)
        //{
        //    var CustomerShipToList = JsonConvert.DeserializeObject<List<CustomerViewModel>>(_customerAPIRepository.GetCustomerShipTo(_factoryCode));


        //    // Convert Json String to List Object
        //    //var customerList = JsonConvert.DeserializeObject<List<Customer>>(_customerAPIRepository.GetCustomerList(_factoryCode));
        //    var custShipToList = JsonConvert.DeserializeObject<List<CustShipTo>>(_custShipToAPIRepository.GetCustShipToList(_factoryCode));

        //    //for (int i = 0; i < customerList.Count; i++)
        //    //{
        //    //    string ShipTo = custShipToList.Where(w => w.CustCode == customerList[i].CustCode).Select(a => a.ShipTo).FirstOrDefault();
        //    //    customerModelViewList[i].CustShipTo = ShipTo;
        //    //}
        //    // Convert List Object to List Object View Model

        //    maintenanceCustomerViewModel.CustShipToList = custShipToList;
        //    maintenanceCustomerViewModel.CustomerViewModelList = CustomerShipToList;
        //    maintenanceCustomerViewModel.QaItems = JsonConvert.DeserializeObject<List<QaItems>>(_qaItemsAPIRepository.GetQaItems()).OrderBy(q => q.TestName).Select(q => q.TestName).Distinct().ToList();


        //    var ProductGroupList = JsonConvert.DeserializeObject<List<ProductGroup>>(_productGroupAPIRepository.GetProductGroupList(_factoryCode));

        //    maintenanceCustomerViewModel.ProductGroupList = ProductGroupList;
        //}

        public void SaveCustomer(MaintenanceCustomerViewModel model)
        {
            List<CustShipTo> custShipToList = new List<CustShipTo>();
            var companyProfile = JsonConvert.DeserializeObject<CompanyProfile>(_companyProfileAPIRepository.GetCompanyProfileByPlant(_factoryCode, _token));
            if (companyProfile == null)
            {
                throw new Exception("Can't find your customer group.");
            }
            if (model.CustShipToArrayList != null)
            {
                List<string> custShipToArrayList = model.CustShipToArrayList.Split(',').ToList<string>();

                custShipToArrayList.ForEach((Action<string>)(i =>
                {
                    custShipToList.Add(new CustShipTo
                    {
                        ShipTo = i,
                        CustCode = model.CustomerViewModel.CustCode,
                    });
                }));
            }

            model.CustomerViewModel.CustStatus = model.CustomerViewModel.CustStatus;

            ParentModel customerModel = new ParentModel();
            customerModel.AppName = Globals.AppNameEncrypt;
            customerModel.SaleOrg = _saleOrg;
            customerModel.PlantCode = _factoryCode;
            customerModel.FactoryCode = _factoryCode;

            ParentModel custShipToModel = new ParentModel();
            custShipToModel.AppName = Globals.AppNameEncrypt;
            custShipToModel.SaleOrg = _saleOrg;
            custShipToModel.PlantCode = _factoryCode;
            custShipToModel.FactoryCode = _factoryCode;

            model.CustomerViewModel.SaleOrg = _saleOrg;
            model.CustomerViewModel.PlantCode = _factoryCode;

            customerModel.Customer = mapper.Map<CustomerViewModel, Customer>(model.CustomerViewModel);
            customerModel.Customer.PriorityFlag = 10;
            customerModel.Customer.CreatedBy = _username;
            customerModel.Customer.CreatedDate = DateTime.Now;
            customerModel.Customer.CustomerGroup = companyProfile.CustomerGroup;
            customerModel.Customer.DataFrom = "PMTs";

            custShipToModel.CustShipToList = custShipToList;

            //// จัดลำดับ Tag

            if (!String.IsNullOrEmpty(model.CustomerViewModel.TagBundle) || !String.IsNullOrWhiteSpace(model.CustomerViewModel.TagBundle))

            {
                string[] TagBundlewords = model.CustomerViewModel.TagBundle.Split('$');

                var TagBundlewordsorted = TagBundlewords.OrderBy(item => item[0]);
                var tagBundle = string.Join("$", TagBundlewordsorted);
                customerModel.Customer.TagBundle = tagBundle;
            }


            if (!String.IsNullOrEmpty(model.CustomerViewModel.TagPallet) || !String.IsNullOrWhiteSpace(model.CustomerViewModel.TagPallet))

            {
                string[] TagPalletwords = model.CustomerViewModel.TagPallet.Split('$');

                var TagPalletwordsorted = TagPalletwords.OrderBy(item => item[0]);
                //customer.TagBundle = 
                var tagPallet = string.Join("$", TagPalletwordsorted);
                customerModel.Customer.TagPallet = tagPallet;
            }








            string customerListJsonString = JsonConvert.SerializeObject(customerModel);
            string custShipToListJsonString = JsonConvert.SerializeObject(custShipToModel);

            _customerAPIRepository.SaveCustomer(customerListJsonString, _token);
            _custShipToAPIRepository.SaveCustShipToList(custShipToListJsonString, _token);
        }

        public void UpdateCustomer(CustomerViewModel customerViewModel)
        {
            var companyProfile = JsonConvert.DeserializeObject<CompanyProfile>(_companyProfileAPIRepository.GetCompanyProfileByPlant(_factoryCode, _token));
            if (companyProfile == null)
            {
                throw new Exception("Can't find your customer group.");
            }
            customerViewModel.UpdatedBy = _username;
            customerViewModel.UpdatedDate = DateTime.Now;
            customerViewModel.CustStatus = true;
            customerViewModel.CreatedBy = string.IsNullOrEmpty(customerViewModel.CreatedBy) ? null : customerViewModel.CreatedBy;
            customerViewModel.UpdatedBy = _username;
            customerViewModel.UpdatedDate = DateTime.Now;

            ////// จัดลำดับ Tag

            //string[] TagBundlewords = customerViewModel.TagBundle.Split(',');

            //var TagBundlewordsorted = TagBundlewords.OrderBy(item => item[0]);
            //var tagBundle = string.Join(",", TagBundlewordsorted);

            //string[] TagPalletwords = customerViewModel.TagPallet.Split(',');

            //var TagPalletwordsorted = TagPalletwords.OrderBy(item => item[0]);
            ////customer.TagBundle = 
            //var tagPallet = string.Join(",", TagPalletwordsorted);


            //customerViewModel.TagBundle = tagBundle;
            //customerViewModel.TagPallet = tagPallet;

            //// จัดลำดับ Tag

            if (!String.IsNullOrEmpty(customerViewModel.TagBundle) || !String.IsNullOrWhiteSpace(customerViewModel.TagBundle))
            {
                string[] TagBundlewords = customerViewModel.TagBundle.Split('$');

                var TagBundlewordsorted = TagBundlewords.OrderBy(item => item[0]);
                var tagBundle = string.Join("$", TagBundlewordsorted);
                customerViewModel.TagBundle = tagBundle;
            }


            if (!String.IsNullOrEmpty(customerViewModel.TagPallet) || !String.IsNullOrWhiteSpace(customerViewModel.TagPallet))
            {
                string[] TagPalletwords = customerViewModel.TagPallet.Split('$');

                var TagPalletwordsorted = TagPalletwords.OrderBy(item => item[0]);
                //customer.TagBundle = 
                var tagPallet = string.Join("$", TagPalletwordsorted);
                customerViewModel.TagPallet = tagPallet;
            }


            ParentModel customerModel = new ParentModel();
            customerModel.AppName = Globals.AppNameEncrypt;
            customerModel.SaleOrg = _saleOrg;
            customerModel.PlantCode = _factoryCode;
            var customer = mapper.Map<CustomerViewModel, Customer>(customerViewModel);
            customer.CustomerGroup = companyProfile.CustomerGroup;
            customer.DataFrom = "PMTs";


            customerModel.Customer = customer;

            string jsonString = JsonConvert.SerializeObject(customerModel);

            _customerAPIRepository.UpdateCustomer(jsonString, _token);
        }

        public void SetCustomerStatus(CustomerViewModel customerViewModel)
        {
            customerViewModel.CustStatus = false;

            string jsonString = JsonConvert.SerializeObject(customerViewModel);

            _customerAPIRepository.UpdateCustomer(jsonString, _token);
        }

        public void DeleteCustomer(int Id)
        {
            //var CustomerData = JsonConvert.DeserializeObject<Customer>(_customerAPIRepository.GetCustomerById(_factoryCode, Id));

            //CustomerData.CustStatus = false;

            //ParentModel CustomerParent = new ParentModel();
            //CustomerParent.AppName = Globals.AppNameEncrypt;
            //CustomerParent.SaleOrg = _saleOrg;
            //CustomerParent.PlantCode = _factoryCode;
            //CustomerParent.Customer = CustomerData;
            //string jsonString = JsonConvert.SerializeObject(CustomerParent);

            //_customerAPIRepository.UpdateCustomer(jsonString);
            _customerAPIRepository.DeleteCustomerByID(_factoryCode, Id, _token);

        }

        public bool CheckCustomerDouplicate(string Id, string CusCode)
        {
            var CustomerData = JsonConvert.DeserializeObject<Customer>(_customerAPIRepository.GetCustomerByCusID(_factoryCode, Id, _token));
            bool resalt = false;
            if (CustomerData != null)
            {
                resalt = true;
            }
            return resalt;
        }

        public bool CheckCustomerDouplicateUpdate(string cusId, string CusCode, string IdCus)
        {
            //var CustomerData = JsonConvert.DeserializeObject<List<Customer>>(_customerAPIRepository.GetCustomerById(_factoryCode, cusId, _token));
            var CustomerData = JsonConvert.DeserializeObject<List<Customer>>(_customerAPIRepository.GetCustomersByCusID(_factoryCode, cusId, _token));
            bool resalt = false;
            if (CustomerData != null)
            {
                var custo = CustomerData.Where(x => x.Id.ToString() != IdCus).ToList();
                foreach (var item in custo)
                {
                    if (item.CustCode == CusCode)
                    {
                        resalt = true;
                    }
                }
            }
            return resalt;
        }

    }
}
