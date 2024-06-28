using AutoMapper;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.MaintenanceProductGroup;
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
    public class MaintenanceProductGroupService : IMaintenanceProductGroupService
    {
        IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper mapper;
        private readonly IProductGroupAPIRepository _ProductGroupAPIRepository;

        private readonly string _username;
        private readonly string _saleOrg;
        private readonly string _factoryCode;
        private readonly string _token;

        public MaintenanceProductGroupService(IHttpContextAccessor httpContextAccessor,
            IProductGroupAPIRepository ProductGroupAPIRepository,
            IMapper mapper)
        {
            // Initialize HttpContext
            _httpContextAccessor = httpContextAccessor;

            // Initialize Repository
            _ProductGroupAPIRepository = ProductGroupAPIRepository;
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

        public void GetProductGroup(MaintenanceProductGroupViewModel maintenanceProductGroupViewModel)
        {
            // Convert Json String to List Object
            var ProductGroupList = JsonConvert.DeserializeObject<List<ProductGroup>>(_ProductGroupAPIRepository.GetProductGroupList(_factoryCode, _token));

            var ProductGroupModelViewList = mapper.Map<List<ProductGroup>, List<ProductGroupViewModel>>(ProductGroupList);

            maintenanceProductGroupViewModel.ProductGroupViewModelList = ProductGroupModelViewList;
            ////////////////////////////////////////////////////////////////////////////////////////////////////

        }

        public void SaveProductGroup(MaintenanceProductGroupViewModel model)
        {

            ParentModel ProductGroupModel = new ParentModel();
            ProductGroupModel.AppName = Globals.AppNameEncrypt;
            ProductGroupModel.FactoryCode = _factoryCode;
            ProductGroupModel.PlantCode = _factoryCode;

            //model.ProductGroupViewModel.ProductGroupStatus = true;
            //model.ProductGroupViewModel.FactoryCode = _factoryCode;

            ProductGroupModel.ProductGroup = mapper.Map<ProductGroupViewModel, ProductGroup>(model.ProductGroupViewModel);
            ProductGroupModel.ProductGroup.CreatedDate = DateTime.Now;
            ProductGroupModel.ProductGroup.CreatedBy = _username;

            string ProductGroupListJsonString = JsonConvert.SerializeObject(ProductGroupModel);

            _ProductGroupAPIRepository.SaveProductGroup(_factoryCode, ProductGroupListJsonString, _token);
        }


        public void UpdateProductGroup(ProductGroupViewModel ProductGroupViewModel)
        {
            ParentModel ProductGroupModel = new ParentModel();
            ProductGroupModel.AppName = Globals.AppNameEncrypt;
            ProductGroupModel.FactoryCode = _factoryCode;
            ProductGroupModel.PlantCode = _factoryCode;

            //ProductGroupViewModel.ProductGroupStatus = true;
            //ProductGroupViewModel.FactoryCode = _factoryCode;

            ProductGroupModel.ProductGroup = mapper.Map<ProductGroupViewModel, ProductGroup>(ProductGroupViewModel);
            ProductGroupModel.ProductGroup.CreatedDate = ProductGroupViewModel.CreatedDate;
            ProductGroupModel.ProductGroup.CreatedBy = ProductGroupViewModel.CreatedBy;
            ProductGroupModel.ProductGroup.UpdatedDate = DateTime.Now;
            ProductGroupModel.ProductGroup.UpdatedBy = _username;

            string jsonString = JsonConvert.SerializeObject(ProductGroupModel);

            _ProductGroupAPIRepository.UpdateProductGroup(_factoryCode, jsonString, _token);
        }


    }
}
