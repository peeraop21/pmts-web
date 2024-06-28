using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.MaintenanceProductType;
using PMTs.DataAccess.ModelView.NewProduct;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Tracing;
using PMTs.WebApplication.Controllers;
using PMTs.WebApplication.Extentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMTs.WebApplication.Services.Interfaces
{
    [TraceAspect]
    public class MaintenanceCategoriesService : IMaintenanceCategoriesService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly ICategoryService _categoryService;
        private readonly INewProductService _newProductService;
        private readonly IKindOfProductAPIRepository _kindOfProductAPIRepository;
        private readonly IKindOfProductGroupAPIRepository _kindOfProductGroupAPIRepository;
        private readonly IProcessCostAPIRepository _processCostAPIRepository;
        private readonly IProductTypeAPIRepository _productTypeAPIRepository;
        private readonly IHierarchyLv2APIRepository _hierarchyLv2APIRepository;
        private readonly IMapper mapper;

        private readonly string _username;
        private readonly string _saleOrg;
        private readonly string _factoryCode;
        private readonly string _token;

        public MaintenanceCategoriesService(ICategoryService categoryService,
            IHttpContextAccessor httpContextAccessor,
            INewProductService newProductService,
            IKindOfProductAPIRepository kindOfProductAPIRepository,
            IKindOfProductGroupAPIRepository kindOfProductGroupAPIRepository,
            IProcessCostAPIRepository processCostAPIRepository,
            IProductTypeAPIRepository productTypeAPIRepository,
            IHierarchyLv2APIRepository hierarchyLv2APIRepository,
            IMapper mapper
            )
        {
            _categoryService = categoryService;
            _httpContextAccessor = httpContextAccessor;
            _newProductService = newProductService;

            _kindOfProductAPIRepository = kindOfProductAPIRepository;
            _kindOfProductGroupAPIRepository = kindOfProductGroupAPIRepository;
            _processCostAPIRepository = processCostAPIRepository;
            _productTypeAPIRepository = productTypeAPIRepository;
            _hierarchyLv2APIRepository = hierarchyLv2APIRepository;
            this.mapper = mapper;

            var userSessionModel = SessionExtentions.GetSession<UserSessionModel>(_httpContextAccessor.HttpContext.Session, "UserSessionModel");
            if (userSessionModel != null)
            {
                _username = userSessionModel.UserName;
                _saleOrg = userSessionModel.SaleOrg;
                _factoryCode = userSessionModel.FactoryCode;
                _token = userSessionModel.Token;
            }
        }

        #region Main Matching
        public void GetMaintenanceCategoriesData(MaintenanceCategoriesController maintenanceProductTypeController, ref MaintenanceKindOfProductGroupCreateModel maintenanceKindOfProductGroup)
        {
            var transactionModel = new TransactionDataModel();
            transactionModel.modelCategories = new DataAccess.ModelView.ViewCategories();
            _newProductService.GetCategoriesListData(ref transactionModel);

            maintenanceKindOfProductGroup.ProductTypes = new List<ProductTypeModel>();

            maintenanceKindOfProductGroup.KindOfProductGroups = transactionModel.modelCategories.KindOfProductGroupList;
            maintenanceKindOfProductGroup.KindOfProducts = transactionModel.modelCategories.KindOfProductList;
            maintenanceKindOfProductGroup.ProcessCosts = transactionModel.modelCategories.ProcessCostList;
            maintenanceKindOfProductGroup.HierarchyLv2Matrices = transactionModel.modelCategories.HierarchyLV2List;

            foreach (var productType in transactionModel.modelCategories.ProductTypeList)
            {
                maintenanceKindOfProductGroup.ProductTypes.Add(new ProductTypeModel
                {
                    Id = productType.Id,
                    Name = productType.Name,
                    Description = productType.Description,
                    Status = productType.Status,
                    SortIndex = productType.SortIndex,
                    HierarchyLv2 = productType.HierarchyLv2,
                    CreatedDate = productType.CreatedDate,
                    CreatedBy = productType.CreatedBy,
                    UpdatedDate = productType.UpdatedDate,
                    UpdatedBy = productType.UpdatedBy,
                    FormGroup = productType.FormGroup,
                    SelectStatus = false,
                    BoxHandle = productType.BoxHandle,
                    IsTwoPiece = productType.IsTwoPiece,
                    UnitDesc = productType.UnitDesc
                });
            }

            //get FormGroup and HierarchyLv2
            var maintenanceProductType = new MaintenanceProductTypeCreateModel();
            maintenanceProductType.SelectHierarchyLv2s = new List<SelectListItem>();
            maintenanceProductType.SelectFormGroups = new List<SelectListItem>();
            var formGroup = transactionModel.modelCategories.ProductTypeList.Select(x => x.FormGroup).Distinct().ToList();
            var hierarchyLv2s = transactionModel.modelCategories.ProductTypeList.Select(x => x.HierarchyLv2).Distinct().ToList();

            maintenanceProductType.SelectHierarchyLv2s = hierarchyLv2s.Select(x => new SelectListItem { Text = x.ToString(), Value = x.ToString() }).ToList();
            maintenanceProductType.HierarchyLv2s = hierarchyLv2s;
            maintenanceProductType.SelectFormGroups = formGroup.Select(x => new SelectListItem { Text = x.ToString(), Value = x.ToString() }).ToList();

            SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "MaintenanceProductTypeCreateModel", maintenanceProductType);
            SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "MaintenanceKindOfProductGroupCreateModel", maintenanceKindOfProductGroup);

            maintenanceProductTypeController.ViewBag.TitleName = "Kind of Product Group";
            maintenanceProductTypeController.ViewBag.ActionToSave = "SaveKindOfProductGroup";
        }

        public void SaveKindOfProductGroup(string KindOfProductID, string ProcessCostID, string KindOfProductGroupID, string ProductTypeArr)
        {
            var kindOfProductGroupCreateModel = SessionExtentions.GetSession<MaintenanceKindOfProductGroupCreateModel>(_httpContextAccessor.HttpContext.Session, "MaintenanceKindOfProductGroupCreateModel");
            var hierarchyLv2Matrix = new HierarchyLv2Matrix();
            hierarchyLv2Matrix.IdKindOfProduct = Convert.ToInt32(KindOfProductID);
            hierarchyLv2Matrix.IdProcessCost = Convert.ToInt32(ProcessCostID);
            hierarchyLv2Matrix.IdKindOfProductGroup = Convert.ToInt32(KindOfProductGroupID);
            hierarchyLv2Matrix.Status = true;

            var productTypes = new List<ProductType>();
            productTypes = JsonConvert.DeserializeObject<List<ProductType>>(Convert.ToString(_productTypeAPIRepository.GetProductTypeList(_factoryCode, _token)));

            var ProductTypeArrs = JsonConvert.DeserializeObject<List<ProductTypeModel>>(ProductTypeArr);

            ////exist match data in HierarchyLv2Matrix
            //var existMatchProduct = kindOfProductGroupCreateModel.HierarchyLv2Matrices.Where(h => h.IdKindOfProduct == hierarchyLv2Matrix.IdKindOfProduct &&
            //    h.IdProcessCost == hierarchyLv2Matrix.IdProcessCost && h.IdKindOfProductGroup == hierarchyLv2Matrix.IdKindOfProductGroup).ToArray();

            ////delete match data in HierarchyLv2Matrix
            //foreach (var existItem in existMatchProduct)
            //{
            //    if (ProductTypeArrs.FirstOrDefault(p => p.Id != existItem.IdProductType) == null)
            //    {
            //        _hierarchyLv2APIRepository.DeleteHierarchyLv2Matrix(_factoryCode, JsonConvert.SerializeObject(existItem));
            //    }
            //}

            //save HierarchyLv2Matrix from
            foreach (var productArr in ProductTypeArrs)
            {
                var sortIndex = GenerateSortIndex("HierarchyLv2Matrix");
                var productType = productTypes.FirstOrDefault(x => x.Id == productArr.Id);
                hierarchyLv2Matrix.IdProductType = productType.Id;
                hierarchyLv2Matrix.HierarchyLv2Code = productType.HierarchyLv2;
                hierarchyLv2Matrix.SortIndex = sortIndex;
                hierarchyLv2Matrix.CreatedBy = _username;
                hierarchyLv2Matrix.CreatedDate = DateTime.Now;

                var existItem = kindOfProductGroupCreateModel.HierarchyLv2Matrices.FirstOrDefault(h => h.IdKindOfProduct == hierarchyLv2Matrix.IdKindOfProduct &&
                        h.IdProcessCost == hierarchyLv2Matrix.IdProcessCost && h.IdKindOfProductGroup == hierarchyLv2Matrix.IdKindOfProductGroup && h.IdProductType == productArr.Id);

                if (productArr.Status.Value)
                {
                    //exist in db
                    if (existItem == null)
                    {
                        _hierarchyLv2APIRepository.CreateHierarchyLv2Matrix(_factoryCode, JsonConvert.SerializeObject(hierarchyLv2Matrix), _token);
                    }
                }
                else
                {
                    //delete unselect macth
                    _hierarchyLv2APIRepository.DeleteHierarchyLv2Matrix(_factoryCode, JsonConvert.SerializeObject(existItem), _token);
                }
            }
        }
        #endregion

        #region Kind of Product Group
        public void GetKindOfProductGroups(ref List<KindOfProductGroup> kindOfProductGroups)
        {
            var maintenanceKindOfProductGroupCreateModel = SessionExtentions.GetSession<MaintenanceKindOfProductGroupCreateModel>(_httpContextAccessor.HttpContext.Session, "MaintenanceKindOfProductGroupCreateModel");

            if (maintenanceKindOfProductGroupCreateModel != null)
            {
                kindOfProductGroups = maintenanceKindOfProductGroupCreateModel.KindOfProductGroups;
            }
            else
            {
                kindOfProductGroups = JsonConvert.DeserializeObject<List<KindOfProductGroup>>(_kindOfProductGroupAPIRepository.GetKindOfProductGroupList(_factoryCode, _token));
            }
        }

        public void CreateKindOfProductGroup(MaintenanceProductTypeCreateModel maintenanceProductTypeCreateModel)
        {
            var kindOfproductGroup = new KindOfProductGroup();
            var IsSuccess = false;

            kindOfproductGroup.Id = maintenanceProductTypeCreateModel.UID;
            kindOfproductGroup.Name = maintenanceProductTypeCreateModel.Name;
            kindOfproductGroup.Description = maintenanceProductTypeCreateModel.Description;
            kindOfproductGroup.Status = maintenanceProductTypeCreateModel.Status;
            kindOfproductGroup.CreatedBy = _username;
            kindOfproductGroup.CreatedDate = DateTime.Now;

            //create new KindOfProductGroup
            var sortIndex = GenerateSortIndex("KindOfProductGroup");
            if (sortIndex != null && sortIndex != 0)
            {
                kindOfproductGroup.SortIndex = sortIndex;

                var kindOfproductGroupData = JsonConvert.SerializeObject(kindOfproductGroup);

                _kindOfProductGroupAPIRepository.CreateKindOfProductGroup(_factoryCode, kindOfproductGroupData, _token);
                IsSuccess = true;
            }

            if (IsSuccess)
            {
                //save data to
                var sessionOfkindOfProductGroupCreateModel = SessionExtentions.GetSession<MaintenanceKindOfProductGroupCreateModel>(_httpContextAccessor.HttpContext.Session, "MaintenanceKindOfProductGroupCreateModel");
                if (sessionOfkindOfProductGroupCreateModel != null)
                {
                    sessionOfkindOfProductGroupCreateModel.KindOfProductGroups.Add(kindOfproductGroup);
                    SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "MaintenanceKindOfProductGroupCreateModel", sessionOfkindOfProductGroupCreateModel);
                }
            }
        }

        public void EditKindOfProductGroup(MaintenanceProductTypeCreateModel maintenanceProductTypeCreateModel)
        {
            var kindOfproductGroup = new KindOfProductGroup();

            //save data to
            var sessionOfkindOfProductGroupCreateModel = SessionExtentions.GetSession<MaintenanceKindOfProductGroupCreateModel>(_httpContextAccessor.HttpContext.Session, "MaintenanceKindOfProductGroupCreateModel");
            //save data to
            if (sessionOfkindOfProductGroupCreateModel != null)
            {

                var editModel = sessionOfkindOfProductGroupCreateModel.KindOfProductGroups.FirstOrDefault(k => k.Id == maintenanceProductTypeCreateModel.UID);
                kindOfproductGroup.Id = maintenanceProductTypeCreateModel.UID;
                kindOfproductGroup.Name = maintenanceProductTypeCreateModel.Name;
                kindOfproductGroup.Description = maintenanceProductTypeCreateModel.Description;
                kindOfproductGroup.Status = maintenanceProductTypeCreateModel.Status;

                kindOfproductGroup.SortIndex = editModel.SortIndex;
                kindOfproductGroup.CreatedBy = editModel.CreatedBy;
                kindOfproductGroup.CreatedDate = Convert.ToDateTime(editModel.CreatedDate);
                kindOfproductGroup.UpdatedBy = _username;
                kindOfproductGroup.UpdatedDate = DateTime.Now;

                //edit new KindOfProductGroup
                var kindOfproductGroupData = JsonConvert.SerializeObject(kindOfproductGroup);

                _kindOfProductGroupAPIRepository.UpdateKindOfProductGroup(_factoryCode, kindOfproductGroupData, _token);


                sessionOfkindOfProductGroupCreateModel.KindOfProductGroups.Remove(editModel);
                sessionOfkindOfProductGroupCreateModel.KindOfProductGroups.Add(kindOfproductGroup);
                SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "MaintenanceKindOfProductGroupCreateModel", sessionOfkindOfProductGroupCreateModel);
            }

        }

        public void DeleteKindOfProductGroup(int id)
        {
            //First get model data by id then send form to API.
            var kindOfProductGroupById = id != null && id != 0 ? JsonConvert.DeserializeObject<KindOfProductGroup>(_kindOfProductGroupAPIRepository.GetKindOfProductGroupById(_factoryCode, id, _token)) : null;

            //not find data from api server 
            if (kindOfProductGroupById == null)
            {
                throw new Exception();
            }

            _kindOfProductGroupAPIRepository.DeleteKindOfProductGroup(_factoryCode, JsonConvert.SerializeObject(kindOfProductGroupById), _token);

            //remove data in session
            var sessionOfkindOfProductGroupCreateModel = SessionExtentions.GetSession<MaintenanceKindOfProductGroupCreateModel>(_httpContextAccessor.HttpContext.Session, "MaintenanceKindOfProductGroupCreateModel");
            if (sessionOfkindOfProductGroupCreateModel != null)
            {
                var editModel = sessionOfkindOfProductGroupCreateModel.KindOfProductGroups.FirstOrDefault(k => k.Id == id);
                sessionOfkindOfProductGroupCreateModel.KindOfProductGroups.Remove(editModel);
                SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "MaintenanceKindOfProductGroupCreateModel", sessionOfkindOfProductGroupCreateModel);
            }
        }

        #endregion

        #region Kind of Product
        public void GetKindOfProducts(ref List<KindOfProduct> kindOfProducts)

        {
            var maintenanceKindOfProductGroupCreateModel = SessionExtentions.GetSession<MaintenanceKindOfProductGroupCreateModel>(_httpContextAccessor.HttpContext.Session, "MaintenanceKindOfProductGroupCreateModel");

            if (maintenanceKindOfProductGroupCreateModel != null)
            {
                kindOfProducts = maintenanceKindOfProductGroupCreateModel.KindOfProducts;
            }
            else
            {
                kindOfProducts = JsonConvert.DeserializeObject<List<KindOfProduct>>(_kindOfProductAPIRepository.GetKindOfProductList(_factoryCode, _token));
            }
        }

        public void CreateKindOfProduct(MaintenanceProductTypeCreateModel maintenanceProductTypeCreateModel)
        {
            var kindOfproduct = new KindOfProduct();
            var IsSuccess = false;

            kindOfproduct.Id = maintenanceProductTypeCreateModel.UID;
            kindOfproduct.Name = maintenanceProductTypeCreateModel.Name;
            kindOfproduct.Description = maintenanceProductTypeCreateModel.Description;
            kindOfproduct.Status = maintenanceProductTypeCreateModel.Status;
            kindOfproduct.CreatedBy = _username;
            kindOfproduct.CreatedDate = DateTime.Now;

            //create new KindOfProductGroup
            var sortIndex = GenerateSortIndex("KindOfProduct");
            if (sortIndex != null && sortIndex != 0)
            {
                kindOfproduct.SortIndex = sortIndex;

                var kindOfproductData = JsonConvert.SerializeObject(kindOfproduct);

                _kindOfProductAPIRepository.CreateKindOfProduct(_factoryCode, kindOfproductData, _token);
                IsSuccess = true;
            }

            if (IsSuccess)
            {
                //save data to
                var sessionOfkindOfProductCreateModel = SessionExtentions.GetSession<MaintenanceKindOfProductGroupCreateModel>(_httpContextAccessor.HttpContext.Session, "MaintenanceKindOfProductGroupCreateModel");
                if (sessionOfkindOfProductCreateModel != null)
                {
                    sessionOfkindOfProductCreateModel.KindOfProducts.Add(kindOfproduct);
                    SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "MaintenanceKindOfProductGroupCreateModel", sessionOfkindOfProductCreateModel);
                }
            }
        }

        public void EditKindOfProduct(MaintenanceProductTypeCreateModel maintenanceProductTypeCreateModel)
        {
            var kindOfproduct = new KindOfProduct();

            //save data to
            var sessionOfkindOfProductCreateModel = SessionExtentions.GetSession<MaintenanceKindOfProductGroupCreateModel>(_httpContextAccessor.HttpContext.Session, "MaintenanceKindOfProductGroupCreateModel");
            if (sessionOfkindOfProductCreateModel != null)
            {
                var editModel = sessionOfkindOfProductCreateModel.KindOfProducts.FirstOrDefault(k => k.Id == maintenanceProductTypeCreateModel.UID);
                kindOfproduct.Id = maintenanceProductTypeCreateModel.UID;
                kindOfproduct.Name = maintenanceProductTypeCreateModel.Name;
                kindOfproduct.Description = maintenanceProductTypeCreateModel.Description;
                kindOfproduct.Status = maintenanceProductTypeCreateModel.Status;
                kindOfproduct.SortIndex = editModel.SortIndex;
                kindOfproduct.CreatedBy = editModel.CreatedBy;
                kindOfproduct.CreatedDate = Convert.ToDateTime(editModel.CreatedDate);
                kindOfproduct.UpdatedBy = _username;
                kindOfproduct.UpdatedDate = DateTime.Now;

                //edit new KindOfProductGroup
                var kindOfproductData = JsonConvert.SerializeObject(kindOfproduct);

                _kindOfProductAPIRepository.UpdateKindOfProduct(_factoryCode, kindOfproductData, _token);

                sessionOfkindOfProductCreateModel.KindOfProducts.Remove(editModel);
                sessionOfkindOfProductCreateModel.KindOfProducts.Add(kindOfproduct);
                SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "MaintenanceKindOfProductGroupCreateModel", sessionOfkindOfProductCreateModel);
            }
        }

        public void DeleteKindOfProduct(int id)
        {
            //First get model data by id then send form to API.
            var kindOfProductById = id != null && id != 0 ? JsonConvert.DeserializeObject<KindOfProduct>(_kindOfProductAPIRepository.GetKindOfProductById(_factoryCode, id, _token)) : null;

            //not find data from api server 
            if (kindOfProductById == null)
            {
                throw new Exception();
            }

            _kindOfProductAPIRepository.DeleteKindOfProduct(_factoryCode, JsonConvert.SerializeObject(kindOfProductById), _token);

            //remove data in session
            var sessionOfkindOfProductCreateModel = SessionExtentions.GetSession<MaintenanceKindOfProductGroupCreateModel>(_httpContextAccessor.HttpContext.Session, "MaintenanceKindOfProductGroupCreateModel");
            if (sessionOfkindOfProductCreateModel != null)
            {
                var editModel = sessionOfkindOfProductCreateModel.KindOfProducts.FirstOrDefault(k => k.Id == id);
                sessionOfkindOfProductCreateModel.KindOfProducts.Remove(editModel);
                SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "MaintenanceKindOfProductGroupCreateModel", sessionOfkindOfProductCreateModel);
            }
        }
        #endregion

        #region Product Type
        public void GetProductTypes(ref MaintenanceKindOfProductGroupCreateModel kindOfProductGroupCreateModel)
        {
            kindOfProductGroupCreateModel = SessionExtentions.GetSession<MaintenanceKindOfProductGroupCreateModel>(_httpContextAccessor.HttpContext.Session, "MaintenanceKindOfProductGroupCreateModel");
            var maintenanceProductTypeCreate = SessionExtentions.GetSession<MaintenanceProductTypeCreateModel>(_httpContextAccessor.HttpContext.Session, "MaintenanceProductTypeCreateModel");
            kindOfProductGroupCreateModel.HierarchyLv2s = maintenanceProductTypeCreate.HierarchyLv2s;
            kindOfProductGroupCreateModel.SelectHierarchyLv2s = maintenanceProductTypeCreate.SelectHierarchyLv2s;
            kindOfProductGroupCreateModel.SelectFormGroups = maintenanceProductTypeCreate.SelectFormGroups;

            if (kindOfProductGroupCreateModel.ProductTypes == null)
            {
                var productTypes = JsonConvert.DeserializeObject<List<ProductType>>(_productTypeAPIRepository.GetProductTypeList(_factoryCode, _token));
                kindOfProductGroupCreateModel.ProductTypes = new List<ProductTypeModel>();
                foreach (var productType in productTypes)
                {
                    kindOfProductGroupCreateModel.ProductTypes.Add(new ProductTypeModel
                    {
                        Id = productType.Id,
                        Name = productType.Name,
                        Description = productType.Description,
                        Status = productType.Status,
                        SortIndex = productType.SortIndex,
                        HierarchyLv2 = productType.HierarchyLv2,
                        CreatedDate = productType.CreatedDate,
                        CreatedBy = productType.CreatedBy,
                        UpdatedDate = productType.UpdatedDate,
                        UpdatedBy = productType.UpdatedBy,
                        FormGroup = productType.FormGroup,
                        SelectStatus = false,
                        BoxHandle = productType.BoxHandle,
                        IsTwoPiece = productType.IsTwoPiece,
                        UnitDesc = productType.UnitDesc

                    });
                }
            }
        }

        public void CreateProductType(MaintenanceProductTypeCreateModel maintenanceProductTypeCreateModel)
        {
            var productType = new ProductType();
            var IsSuccess = false;

            productType.Id = maintenanceProductTypeCreateModel.UID;
            productType.Name = maintenanceProductTypeCreateModel.Name;
            productType.Description = maintenanceProductTypeCreateModel.Description;
            productType.Status = maintenanceProductTypeCreateModel.Status;
            productType.CreatedBy = _username;
            productType.CreatedDate = DateTime.Now;
            productType.HierarchyLv2 = maintenanceProductTypeCreateModel.HierarchyLv2;
            productType.FormGroup = maintenanceProductTypeCreateModel.FormGroup;
            productType.IsTwoPiece = maintenanceProductTypeCreateModel.IsTwoPiece;
            productType.BoxHandle = maintenanceProductTypeCreateModel.BoxHandle;
            productType.UnitDesc = maintenanceProductTypeCreateModel.UnitDesc;

            //create new KindOfProductGroup
            var sortIndex = GenerateSortIndex("ProductType");
            if (sortIndex != null && sortIndex != 0)
            {
                productType.SortIndex = sortIndex;

                var productTypeData = JsonConvert.SerializeObject(productType);

                _productTypeAPIRepository.CreateProductType(_factoryCode, productTypeData, _token);
                IsSuccess = true;
            }

            if (IsSuccess)
            {
                //save data to
                var sessionOfkindOfProductCreateModel = SessionExtentions.GetSession<MaintenanceKindOfProductGroupCreateModel>(_httpContextAccessor.HttpContext.Session, "MaintenanceKindOfProductGroupCreateModel");
                var maintenanceProductTypeCreate = SessionExtentions.GetSession<MaintenanceProductTypeCreateModel>(_httpContextAccessor.HttpContext.Session, "MaintenanceProductTypeCreateModel");

                if (sessionOfkindOfProductCreateModel != null && maintenanceProductTypeCreate != null)
                {
                    var productTypeModel = mapper.Map<ProductType, ProductTypeModel>(productType);
                    productTypeModel.SelectStatus = false;
                    sessionOfkindOfProductCreateModel.ProductTypes.Add(productTypeModel);
                    if (maintenanceProductTypeCreate.HierarchyLv2s.FirstOrDefault(x => x == productTypeModel.HierarchyLv2) == null)
                    {
                        maintenanceProductTypeCreate.HierarchyLv2s.Add(productTypeModel.HierarchyLv2);
                    }

                    SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "MaintenanceProductTypeCreateModel", maintenanceProductTypeCreate);
                    SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "MaintenanceKindOfProductGroupCreateModel", sessionOfkindOfProductCreateModel);
                }
            }
        }

        public void EditProductType(MaintenanceProductTypeCreateModel maintenanceProductTypeCreateModel)
        {
            var productType = new ProductType();
            var IsSuccess = false;
            //save data to
            var sessionOfkindOfProductCreateModel = SessionExtentions.GetSession<MaintenanceKindOfProductGroupCreateModel>(_httpContextAccessor.HttpContext.Session, "MaintenanceKindOfProductGroupCreateModel");
            var maintenanceProductTypeCreate = SessionExtentions.GetSession<MaintenanceProductTypeCreateModel>(_httpContextAccessor.HttpContext.Session, "MaintenanceProductTypeCreateModel");

            if (sessionOfkindOfProductCreateModel != null && maintenanceProductTypeCreate != null)
            {
                var editModel = sessionOfkindOfProductCreateModel.ProductTypes.FirstOrDefault(k => k.Id == maintenanceProductTypeCreateModel.UID);
                productType.Id = maintenanceProductTypeCreateModel.UID;
                productType.Name = maintenanceProductTypeCreateModel.Name;
                productType.Description = maintenanceProductTypeCreateModel.Description;
                productType.Status = maintenanceProductTypeCreateModel.Status;
                productType.SortIndex = maintenanceProductTypeCreateModel.SortIndex;
                productType.FormGroup = maintenanceProductTypeCreateModel.FormGroup;
                productType.HierarchyLv2 = maintenanceProductTypeCreateModel.HierarchyLv2;
                productType.IsTwoPiece = maintenanceProductTypeCreateModel.IsTwoPiece;
                productType.BoxHandle = maintenanceProductTypeCreateModel.BoxHandle;
                productType.UnitDesc = maintenanceProductTypeCreateModel.UnitDesc;

                productType.CreatedBy = editModel.CreatedBy;
                productType.CreatedDate = Convert.ToDateTime(editModel.CreatedDate);
                productType.UpdatedBy = _username;
                productType.UpdatedDate = DateTime.Now;

                //edit new KindOfProductGroup
                var productTypeData = JsonConvert.SerializeObject(productType);

                _productTypeAPIRepository.UpdateProductType(_factoryCode, productTypeData, _token);


                sessionOfkindOfProductCreateModel.ProductTypes.Remove(editModel);

                var productTypeModel = mapper.Map<ProductType, ProductTypeModel>(productType);
                productTypeModel.SelectStatus = false;
                sessionOfkindOfProductCreateModel.ProductTypes.Add(productTypeModel);
                if (maintenanceProductTypeCreate.HierarchyLv2s.FirstOrDefault(x => x == productTypeModel.HierarchyLv2) == null)
                {
                    maintenanceProductTypeCreate.HierarchyLv2s.Add(productTypeModel.HierarchyLv2);
                }

                SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "MaintenanceProductTypeCreateModel", maintenanceProductTypeCreate);
                SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "MaintenanceKindOfProductGroupCreateModel", sessionOfkindOfProductCreateModel);
            }
        }

        public void DeleteProductType(int id)
        {
            //First get model data by id then send form to API.
            var productTypeById = id != null && id != 0 ? JsonConvert.DeserializeObject<ProductType>(_productTypeAPIRepository.GetProductTypeById(_factoryCode, id, _token)) : null;

            //not find data from api server 
            if (productTypeById == null)
            {
                throw new Exception();
            }

            _productTypeAPIRepository.DeleteProductType(_factoryCode, JsonConvert.SerializeObject(productTypeById), _token);

            //remove data in session
            var sessionOfProductTypeCreateModel = SessionExtentions.GetSession<MaintenanceKindOfProductGroupCreateModel>(_httpContextAccessor.HttpContext.Session, "MaintenanceKindOfProductGroupCreateModel");
            if (sessionOfProductTypeCreateModel != null)
            {
                var editModel = sessionOfProductTypeCreateModel.ProductTypes.FirstOrDefault(k => k.Id == id);
                sessionOfProductTypeCreateModel.ProductTypes.Remove(editModel);

                SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "MaintenanceKindOfProductGroupCreateModel", sessionOfProductTypeCreateModel);
            }
        }

        #endregion

        #region Process Cost
        public void GetProcessCosts(ref List<ProcessCost> processCosts)
        {
            var maintenanceKindOfProductGroupCreateModel = SessionExtentions.GetSession<MaintenanceKindOfProductGroupCreateModel>(_httpContextAccessor.HttpContext.Session, "MaintenanceKindOfProductGroupCreateModel");

            if (maintenanceKindOfProductGroupCreateModel != null)
            {
                processCosts = maintenanceKindOfProductGroupCreateModel.ProcessCosts;
            }
            else
            {
                processCosts = JsonConvert.DeserializeObject<List<ProcessCost>>(_processCostAPIRepository.GetProcessCostList(_factoryCode, _token));
            }
        }

        public void CreateProcessCost(MaintenanceProductTypeCreateModel maintenanceProductTypeCreateModel)
        {
            var processCost = new ProcessCost();
            var IsSuccess = false;

            processCost.Id = maintenanceProductTypeCreateModel.UID;
            processCost.Name = maintenanceProductTypeCreateModel.Name;
            processCost.Description = maintenanceProductTypeCreateModel.Description;
            processCost.Status = maintenanceProductTypeCreateModel.Status;
            processCost.CreatedBy = _username;
            processCost.CreatedDate = DateTime.Now;

            //create new KindOfProductGroup
            var sortIndex = GenerateSortIndex("ProcessCost");
            if (sortIndex != null && sortIndex != 0)
            {
                processCost.SortIndex = sortIndex;

                var processCostData = JsonConvert.SerializeObject(processCost);

                _processCostAPIRepository.CreateProcessCost(_factoryCode, processCostData, _token);
                IsSuccess = true;
            }

            if (IsSuccess)
            {
                //save data to
                var sessionOfkindOfProductCreateModel = SessionExtentions.GetSession<MaintenanceKindOfProductGroupCreateModel>(_httpContextAccessor.HttpContext.Session, "MaintenanceKindOfProductGroupCreateModel");
                if (sessionOfkindOfProductCreateModel != null)
                {
                    sessionOfkindOfProductCreateModel.ProcessCosts.Add(processCost);
                    SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "MaintenanceKindOfProductGroupCreateModel", sessionOfkindOfProductCreateModel);
                }
            }
        }

        public void EditProcessCost(MaintenanceProductTypeCreateModel maintenanceProductTypeCreateModel)
        {
            var processCost = new ProcessCost();
            var IsSuccess = false;

            //save data to
            var sessionOfProcessCostCreateModel = SessionExtentions.GetSession<MaintenanceKindOfProductGroupCreateModel>(_httpContextAccessor.HttpContext.Session, "MaintenanceKindOfProductGroupCreateModel");
            if (sessionOfProcessCostCreateModel != null)
            {
                var editModel = sessionOfProcessCostCreateModel.ProcessCosts.FirstOrDefault(k => k.Id == maintenanceProductTypeCreateModel.UID);

                processCost.Id = maintenanceProductTypeCreateModel.UID;
                processCost.Name = maintenanceProductTypeCreateModel.Name;
                processCost.Description = maintenanceProductTypeCreateModel.Description;
                processCost.Status = maintenanceProductTypeCreateModel.Status;

                processCost.SortIndex = editModel.SortIndex;
                processCost.CreatedBy = editModel.CreatedBy;
                processCost.CreatedDate = Convert.ToDateTime(editModel.CreatedDate);
                processCost.UpdatedBy = _username;
                processCost.UpdatedDate = DateTime.Now;

                //edit new 
                var processCostData = JsonConvert.SerializeObject(processCost);

                _processCostAPIRepository.UpdateProcessCost(_factoryCode, processCostData, _token);

                sessionOfProcessCostCreateModel.ProcessCosts.Remove(editModel);
                sessionOfProcessCostCreateModel.ProcessCosts.Add(processCost);
                SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "MaintenanceKindOfProductGroupCreateModel", sessionOfProcessCostCreateModel);
            }
        }

        public void DeleteProcessCost(int id)
        {
            //First get model data by id then send form to API.
            var processCostById = id != null && id != 0 ? JsonConvert.DeserializeObject<ProcessCost>(_processCostAPIRepository.GetProcessCostById(_factoryCode, id, _token)) : null;

            //not find data from api server 
            if (processCostById == null)
            {
                throw new Exception();
            }

            _processCostAPIRepository.DeleteProcessCost(_factoryCode, JsonConvert.SerializeObject(processCostById), _token);

            //remove data in session
            var sessionOfprocessCostCreateModel = SessionExtentions.GetSession<MaintenanceKindOfProductGroupCreateModel>(_httpContextAccessor.HttpContext.Session, "MaintenanceKindOfProductGroupCreateModel");
            if (sessionOfprocessCostCreateModel != null)
            {
                var editModel = sessionOfprocessCostCreateModel.ProcessCosts.FirstOrDefault(k => k.Id == id);
                sessionOfprocessCostCreateModel.ProcessCosts.Remove(editModel);
                SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "MaintenanceKindOfProductGroupCreateModel", sessionOfprocessCostCreateModel);
            }
        }
        #endregion

        #region Function

        private int GenerateSortIndex(string type)
        {
            int sortIndex = 0;

            if (type == "KindOfProduct")
            {
                var lastIndexOfKindOfProduct = JsonConvert.DeserializeObject<List<KindOfProduct>>(_kindOfProductAPIRepository.GetKindOfProductList(_factoryCode, _token)).OrderByDescending(o => o.SortIndex).FirstOrDefault();
                sortIndex = lastIndexOfKindOfProduct.SortIndex.Value + 1;
            }
            else if (type == "ProcessCost")
            {
                var lastIndexOfProcessCost = JsonConvert.DeserializeObject<List<ProcessCost>>(_processCostAPIRepository.GetProcessCostList(_factoryCode, _token)).OrderByDescending(o => o.SortIndex).FirstOrDefault();
                sortIndex = lastIndexOfProcessCost.SortIndex.Value + 1;
            }
            else if (type == "ProductType")
            {
                var lastIndexOfProductType = JsonConvert.DeserializeObject<List<ProductType>>(_productTypeAPIRepository.GetProductTypeList(_factoryCode, _token)).OrderByDescending(o => o.SortIndex).FirstOrDefault();
                sortIndex = lastIndexOfProductType.SortIndex.Value + 1;
            }
            else if (type == "KindOfProductGroup")
            {
                var lastIndexOfKindOfProductGroup = JsonConvert.DeserializeObject<List<KindOfProductGroup>>(_kindOfProductGroupAPIRepository.GetKindOfProductGroupList(_factoryCode, _token)).OrderByDescending(o => o.SortIndex).FirstOrDefault();
                sortIndex = lastIndexOfKindOfProductGroup.SortIndex.Value + 1;
            }
            else if (type == "HierarchyLv2Matrix")
            {
                var lastIndexOfHierarchyLv2Matrix = JsonConvert.DeserializeObject<List<HierarchyLv2Matrix>>(_hierarchyLv2APIRepository.GetHierarchyLv2List(_factoryCode, _token)).OrderByDescending(o => o.SortIndex).FirstOrDefault();
                sortIndex = lastIndexOfHierarchyLv2Matrix.SortIndex.Value + 1;
            }

            return sortIndex;
        }

        #endregion
    }
}
