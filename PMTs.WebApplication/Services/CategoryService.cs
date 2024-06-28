using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.NewProduct;
using PMTs.DataAccess.Repository;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Shared;
using PMTs.DataAccess.Tracing;
using PMTs.WebApplication.Controllers;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Services;
using PMTs.WebApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMTs.WebApplication.Services
{
    [TraceAspect]
    public class CategoryService : ICategoryService
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;

        private readonly INewProductService _newProductService;
        private readonly IPresaleService _presaleService;

        private readonly IKindOfProductGroupAPIRepository _kindOfProductGroupAPIRepository;
        private readonly IKindOfProductAPIRepository _kindOfProductAPIRepository;
        private readonly IProcessCostAPIRepository _processCostAPIRepository;
        private readonly IProductTypeAPIRepository _productTypeAPIRepository;
        private readonly IMaterialTypeAPIRepository _materialTypeAPIRepository;
        private readonly IUnitMaterialAPIRepository _unitMaterialAPIRepository;
        private readonly IMapCostAPIRepository _mapCostAPIRepository;
        private readonly IHierarchyLv2APIRepository _hierarchyLv2APIRepository;
        private readonly IBuildRemarkAPIRepository _buildRemarkAPIRepository;
        private readonly ITransactionsDetailAPIRepository _transactionsDetailAPIRepository;
        private readonly IMasterDataAPIRepository _masterDataAPIRepository;
        private readonly ISetCategoriesOldMatAPIRepository _setCategoriesOldMatAPIRepository;
        private readonly IFSCCodeAPIRepository fSCCodeAPIRepository;
        private readonly IFSCFGCodeAPIRepository fSCFGCodeAPIRepository;
        private readonly IFormulaAPIRepository _formulaAPIRepository;

        private readonly string _username;
        private readonly string _saleOrg;
        private readonly string _factoryCode;
        private readonly string _token;

        public CategoryService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor,
            INewProductService newProductService,
            IPresaleService presaleService,
            IKindOfProductGroupAPIRepository kindOfProductGroupAPIRepository,
            IKindOfProductAPIRepository kindOfProductAPIRepository,
            IProcessCostAPIRepository processCostAPIRepository,
            IProductTypeAPIRepository productTypeAPIRepository,
            IMaterialTypeAPIRepository materialTypeAPIRepository,
            IUnitMaterialAPIRepository unitMaterialAPIRepository,
            IHierarchyLv2APIRepository hierarchyLv2APIRepository,
            IMapCostAPIRepository mapCostAPIRepository,
            IHostingEnvironment hostingEnvironment,
            IBuildRemarkAPIRepository buildRemarkAPIRepository,
            ITransactionsDetailAPIRepository transactionsDetailAPIRepository,
            IMasterDataAPIRepository masterDataAPIRepository,
            ISetCategoriesOldMatAPIRepository setCategoriesOldMatAPIRepository,
            IFSCCodeAPIRepository fSCCodeAPIRepository,
            IFSCFGCodeAPIRepository fSCFGCodeAPIRepository,
            IFormulaAPIRepository formulaAPIRepository
            )
        {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
            _httpContextAccessor = httpContextAccessor;
            // Initialize Services
            _newProductService = newProductService;
            _presaleService = presaleService;

            // Initialize API Repository
            _kindOfProductGroupAPIRepository = kindOfProductGroupAPIRepository;
            _kindOfProductAPIRepository = kindOfProductAPIRepository;
            _processCostAPIRepository = processCostAPIRepository;
            _productTypeAPIRepository = productTypeAPIRepository;
            _materialTypeAPIRepository = materialTypeAPIRepository;
            _unitMaterialAPIRepository = unitMaterialAPIRepository;
            _hierarchyLv2APIRepository = hierarchyLv2APIRepository;
            _mapCostAPIRepository = mapCostAPIRepository;
            _buildRemarkAPIRepository = buildRemarkAPIRepository;
            _transactionsDetailAPIRepository = transactionsDetailAPIRepository;
            _masterDataAPIRepository = masterDataAPIRepository;
            _setCategoriesOldMatAPIRepository = setCategoriesOldMatAPIRepository;
            this.fSCCodeAPIRepository = fSCCodeAPIRepository;
            this.fSCFGCodeAPIRepository = fSCFGCodeAPIRepository;

            _formulaAPIRepository = formulaAPIRepository;

            // Initialize UserData From Session
            var userSessionModel = SessionExtentions.GetSession<UserSessionModel>(_httpContextAccessor.HttpContext.Session, "UserSessionModel");
            if (userSessionModel != null)
            {
                _username = userSessionModel.UserName;
                _saleOrg = userSessionModel.SaleOrg;
                _factoryCode = userSessionModel.FactoryCode;
                _token = userSessionModel.Token;
            }
        }

        public void GetCategory(NewProductController newProductController, ref TransactionDataModel transactionDataModel, string actionTran, string materialNo, string psmId)
        {
            var realEventFlag = string.Empty;
            if (actionTran != "Backward")
            {
                realEventFlag = actionTran;
            }

            var existTransactionData = SessionExtentions.GetSession<TransactionDataModel>(_httpContextAccessor.HttpContext.Session, "TransactionDataModel");

            if (existTransactionData != null)
            {
                if (actionTran == "Create" && existTransactionData.EventFlag != "Create" && existTransactionData.MaterialNo != null)
                {
                    SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "TransactionDataModel", transactionDataModel);
                    existTransactionData = new TransactionDataModel();
                }

                transactionDataModel = existTransactionData;
                realEventFlag = existTransactionData.RealEventFlag;
            }

            transactionDataModel.EventFlag = actionTran;

            if (actionTran == "Create" || actionTran == "CreateOs")
            {
                //    transactionDataModel.EventFlag = "Create";
                transactionDataModel.TransactionDetail = existTransactionData != null ? existTransactionData.TransactionDetail : new TransactionDetail();

                GetCategoriesData(ref transactionDataModel);
                _newProductService.SetTransactionStatus(ref transactionDataModel, "Categories");
            }
            //else if (actionTran == "CreateOs")
            //{
            //    var plantOs = SessionExtentions.GetSession<TransactionDataModel>(_httpContextAccessor.HttpContext.Session, "TransactionDataModel").PlantOs;
            //    transactionDataModel.EventFlag = "CreateOs";
            //    transactionDataModel.TransactionDetail = new TransactionDetail();

            //    GetCategoriesData(ref transactionDataModel);
            //    _newProductService.SetTransactionStatus(ref transactionDataModel, "Categories");

            //    transactionDataModel.PlantOs = plantOs;
            //}
            else if (actionTran == "CopyOs")
            {
                BindDataToModelEditCopy(ref transactionDataModel, _hostingEnvironment, actionTran, materialNo, null);
                if (transactionDataModel.TransactionDetail != null)
                {
                    transactionDataModel.TransactionDetail.MaxStep = 0;
                }
            }
            else if (actionTran == "Edit")
            {
                BindDataToModelEditCopy(ref transactionDataModel, _hostingEnvironment, actionTran, materialNo, null);
            }
            else if (actionTran == "View")
            {
                BindDataToModelEditCopy(ref transactionDataModel, _hostingEnvironment, actionTran, materialNo, null);
            }
            else if (actionTran == "Copy" || actionTran == "CopyX" || actionTran == "CopyAndDelete")
            {
                BindDataToModelEditCopy(ref transactionDataModel, _hostingEnvironment, actionTran, materialNo, null);
                if (transactionDataModel.TransactionDetail != null)
                {
                    transactionDataModel.TransactionDetail.MaxStep = 0;
                }
            }
            else if (actionTran == "Presale" || actionTran == "PresaleChangeNewProduct")
            {
                var IsPresaleCreateNewMat = false;

                if (actionTran == "PresaleChangeNewProduct")
                {
                    actionTran = "Presale";
                    realEventFlag = "Presale";
                    IsPresaleCreateNewMat = true;
                }

                transactionDataModel.EventFlag = "Presale";

                PresaleViewModel presaleViewModelObject = new PresaleViewModel();
                presaleViewModelObject.Plant = _factoryCode;
                presaleViewModelObject.PSM_ID = psmId;

                MasterDataTransactionModel trans = _presaleService.ImportPresale(_configuration, presaleViewModelObject);
                BindDataToModelEditCopy(ref transactionDataModel, _hostingEnvironment, actionTran, "", trans);
                _presaleService.SentToMasterCardPresale("2", psmId);
                if (transactionDataModel.TransactionDetail != null)
                {
                    transactionDataModel.TransactionDetail.MaxStep = 0;
                }

                if (IsPresaleCreateNewMat)
                {
                    transactionDataModel.TransactionDetail.IsPresaleCreateNewMat = true;
                }
                SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "PresaleViewModel", presaleViewModelObject);
            }
            else if (actionTran == "Backward")
            {
                transactionDataModel = BindDataToModelCategoriesBackward(transactionDataModel);
                _newProductService.SetTransactionStatus(ref transactionDataModel, "Categories");
            }
            else if (actionTran == "Detail")
            {
                BindDataToModelEditCopy(ref transactionDataModel, _hostingEnvironment, actionTran, materialNo, null);
            }
            else
            {
                GetCategoriesData(ref transactionDataModel);
                _newProductService.SetTransactionStatus(ref transactionDataModel, "Categories");
            }

            transactionDataModel.CurrentTransaction = "Categories";
            transactionDataModel.RealEventFlag = realEventFlag;
            SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "TransactionDataModel", transactionDataModel);
            if (realEventFlag.Equals("CopyX") || realEventFlag.Equals("CopyAndDelete"))
            {
                SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "CopyTransactionDataModel", transactionDataModel);
            }

            newProductController.ViewBag.PrintMaster = "Hide";
        }

        private void GetCategoriesData(ref TransactionDataModel transactionDataModel)
        {
            transactionDataModel.modelCategories = new ViewCategories();

            transactionDataModel.modelCategories.Id_ProcCost = 60;
            transactionDataModel.modelCategories.Id_kProd = 0;
            transactionDataModel.modelCategories.Id_ProdType = 0;
            transactionDataModel.modelCategories.Id_MatType = 5;
            transactionDataModel.modelCategories.Id_PU = 1;
            transactionDataModel.modelCategories.Id_SU = 1;

            _newProductService.GetCategoriesListData(ref transactionDataModel);
        }

        public void SaveCategories(TransactionDataModel transactionDataModel)
        {
            var transactionDataModelSession = new TransactionDataModel();

            transactionDataModelSession = SessionExtentions.GetSession<TransactionDataModel>(_httpContextAccessor.HttpContext.Session, "TransactionDataModel");

            if (transactionDataModelSession.EventFlag != "Edit" && string.IsNullOrEmpty(transactionDataModelSession.MaterialNo)
                || transactionDataModelSession.EventFlag == "CreateOs" || transactionDataModelSession.EventFlag.Contains("Copy")
                || transactionDataModelSession.EventFlag == "Create" && !string.IsNullOrEmpty(transactionDataModelSession.MaterialNo))
            {
                transactionDataModelSession.modelCategories = transactionDataModel.modelCategories;
            }

            if (transactionDataModelSession.EventFlag == "Presale")
            {
                transactionDataModelSession.modelProductSpec.Hierarchy = "03" + transactionDataModelSession.modelCategories.HierarchyLV2.Trim() + transactionDataModelSession.modelCategories.HierarchyLV3 + "999" + transactionDataModelSession.modelProductSpec.Code;
                transactionDataModelSession.modelCategories.FormGroup = JsonConvert.DeserializeObject<ProductType>(_productTypeAPIRepository.GetProductTypeById(_factoryCode, transactionDataModelSession.modelCategories.Id_ProdType, _token)).FormGroup;
            }

            #region Save transaction detail when edit old material number
            var existTransactionDetailOldMat = JsonConvert.DeserializeObject<TransactionsDetail>(_transactionsDetailAPIRepository.GetTransactionsDetailByMat(_factoryCode, transactionDataModelSession.MaterialNo, _token));

            if (transactionDataModelSession.TransactionDetail.IsOldMaterial && transactionDataModelSession.EventFlag == "Edit" && existTransactionDetailOldMat == null)
            {
                transactionDataModelSession.modelCategories = transactionDataModel.modelCategories;

                var transactionDetailObject = new TransactionsDetail();
                transactionDetailObject.FactoryCode = _factoryCode;
                transactionDetailObject.MaterialNo = transactionDataModelSession.MaterialNo;

                //category data
                transactionDetailObject.IdKindOfProduct = transactionDataModel.modelCategories.Id_kProd;
                transactionDetailObject.IdKindOfProductGroup = transactionDataModel.modelCategories.Id_kProdGrp;
                transactionDetailObject.IdProcessCost = transactionDataModel.modelCategories.Id_ProcCost;
                transactionDetailObject.IdProductType = transactionDataModel.modelCategories.Id_ProdType;
                transactionDetailObject.IdMaterialType = transactionDataModel.modelCategories.Id_MatType;
                transactionDetailObject.IdProductUnit = transactionDataModel.modelCategories.Id_PU;
                transactionDetailObject.IdSaleUnit = transactionDataModel.modelCategories.Id_SU;
                transactionDetailObject.HierarchyLv4 = transactionDataModel.modelCategories.HierarchyLV4 == "whitespace" ? "" : transactionDataModel.modelCategories.HierarchyLV4;

                //info data
                if (transactionDataModel.modelProductInfo != null)
                {
                    transactionDetailObject.HvaGroup1 = transactionDataModel.modelProductInfo.HvaProductType;
                    transactionDetailObject.HvaGroup2 = transactionDataModel.modelProductInfo.HvaStructural;
                    transactionDetailObject.HvaGroup3 = transactionDataModel.modelProductInfo.HvaPrinting;
                    transactionDetailObject.HvaGroup4 = transactionDataModel.modelProductInfo.HvaFlute;
                    transactionDetailObject.HvaGroup5 = transactionDataModel.modelProductInfo.HvaCorrugating;
                    transactionDetailObject.HvaGroup6 = transactionDataModel.modelProductInfo.HvaCoating;
                    transactionDetailObject.HvaGroup7 = transactionDataModel.modelProductInfo.HvaFinishing;
                }

                transactionDetailObject.Outsource = transactionDataModelSession.SaleOrg == _saleOrg ? false : true;
                if (transactionDetailObject.Outsource)
                {
                    transactionDataModelSession.TransactionDetail.IsOutSource = true;
                    transactionDetailObject.HireOrderType = 1;
                }

                if (transactionDataModelSession.SapStatus)
                {
                    transactionDetailObject.MaxStep = 8;
                    transactionDataModelSession.TransactionDetail.MaxStep = 8;
                }

                var parentModelTransactionsDetail = new ParentModel()
                {
                    AppName = Globals.AppNameEncrypt,
                    SaleOrg = _saleOrg,
                    PlantCode = _factoryCode,
                    TransactionsDetail = transactionDetailObject
                };

                var jsonTransactionsDetail = JsonConvert.SerializeObject(parentModelTransactionsDetail);
                _transactionsDetailAPIRepository.SaveTransactionsDetail(jsonTransactionsDetail, _token);

                var existMasterdata = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNo(_factoryCode, transactionDataModelSession.MaterialNo, _token));
                existMasterdata.LastUpdate = DateTime.Now;
                existMasterdata.User = _username;
                existMasterdata.UpdatedBy = _username;
                existMasterdata.Hierarchy = "03" + transactionDataModel.modelCategories.HierarchyLV2.Trim() + transactionDataModel.modelCategories.HierarchyLV3.Trim() + transactionDetailObject.HierarchyLv4;
                existMasterdata.RscStyle = !string.IsNullOrEmpty(transactionDataModel.modelCategories.FormGroup) && transactionDataModel.modelCategories.FormGroup.ToUpper().Contains("RSC") ? transactionDataModel.modelCategories.RSCStyle : "";

                if (!String.IsNullOrEmpty(existMasterdata.Code) && existMasterdata.Hierarchy.Length == 10)
                {
                    existMasterdata.Hierarchy = "03" + transactionDataModel.modelCategories.HierarchyLV2.Trim() + transactionDataModel.modelCategories.HierarchyLV3.Trim() + transactionDetailObject.HierarchyLv4 + existMasterdata.Code;
                }

                if (!string.IsNullOrEmpty(existMasterdata?.BoxType))
                {
                    existMasterdata.SizeDimensions = JsonConvert.DeserializeObject<string>(_formulaAPIRepository.CalSizeDimensions(_factoryCode, transactionDataModelSession.MaterialNo, _token));
                }

                var parentModelMasterData = new ParentModel()
                {
                    AppName = Globals.AppNameEncrypt,
                    SaleOrg = _saleOrg,
                    PlantCode = _factoryCode,
                    MasterData = existMasterdata
                };

                var jsonExistMasterdata = JsonConvert.SerializeObject(parentModelMasterData);
                _masterDataAPIRepository.UpdateMasterData(jsonExistMasterdata, _token);

                transactionDataModelSession.TransactionDetail.HierarchyDetail = existMasterdata.Hierarchy;
            }
            else if (transactionDataModelSession.RealEventFlag.Equals("CopyOs"))
            {
                var existMasterdata = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNo(transactionDataModelSession.PlantOs, transactionDataModelSession.MaterialNo, _token));
                var hierarchyLv4 = transactionDataModel.modelCategories.HierarchyLV4 == "whitespace" ? "" : transactionDataModel.modelCategories.HierarchyLV4;
                existMasterdata.Hierarchy = "03" + transactionDataModel.modelCategories.HierarchyLV2.Trim() + transactionDataModel.modelCategories.HierarchyLV3.Trim() + hierarchyLv4;

                if (!String.IsNullOrEmpty(existMasterdata.Code) && existMasterdata.Hierarchy.Length == 10)
                {
                    existMasterdata.Hierarchy = "03" + transactionDataModel.modelCategories.HierarchyLV2.Trim() + transactionDataModel.modelCategories.HierarchyLV3.Trim() + hierarchyLv4;
                }
                transactionDataModelSession.TransactionDetail.HierarchyDetail = existMasterdata.Hierarchy;
            }
            else if (!string.IsNullOrEmpty(transactionDataModelSession.MaterialNo) && !transactionDataModelSession.RealEventFlag.Contains("CreateOs"))
            {
                var existMasterdata = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNo(_factoryCode, transactionDataModelSession.MaterialNo, _token));
                var hierarchyLv4 = transactionDataModel.modelCategories.HierarchyLV4 == "whitespace" ? "" : transactionDataModel.modelCategories.HierarchyLV4;
                existMasterdata.Hierarchy = "03" + transactionDataModel.modelCategories.HierarchyLV2.Trim() + transactionDataModel.modelCategories.HierarchyLV3.Trim() + hierarchyLv4;

                if (!String.IsNullOrEmpty(existMasterdata.Code) && existMasterdata.Hierarchy.Length == 10)
                {
                    existMasterdata.Hierarchy = "03" + transactionDataModel.modelCategories.HierarchyLV2.Trim() + transactionDataModel.modelCategories.HierarchyLV3.Trim() + hierarchyLv4;
                }
                transactionDataModelSession.TransactionDetail.HierarchyDetail = existMasterdata.Hierarchy;
            }

            #endregion

            //update max step
            _newProductService.UpdateMaxProgress(_factoryCode, ref transactionDataModelSession);

            SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "TransactionDataModel", transactionDataModelSession);
        }

        public TransactionDataModel BindDataToModelCategoriesBackward(TransactionDataModel transactionDataModel)
        {
            transactionDataModel = SessionExtentions.GetSession<TransactionDataModel>(_httpContextAccessor.HttpContext.Session, "TransactionDataModel");

            _newProductService.GetCategoriesListData(ref transactionDataModel);
            return transactionDataModel;
        }

        public TransactionDataModel BindDataToModelEditCopy(ref TransactionDataModel model, IHostingEnvironment hostEnvironment, string actionTran, string MaterialNo, MasterDataTransactionModel masterDataTran)
        {
            var factoryCode = _factoryCode;
            var orderTypeId = model.TransactionDetail != null ? model.TransactionDetail.OrderTypeId.ToString() : string.Empty;
            var isOldMaterial = false;
            // Clear TransactionData Session
            _httpContextAccessor.HttpContext.Session.Remove("TransactionDataModel");

            if (model == null)
            {
                model = new TransactionDataModel();
            }

            model.modelCategories = new ViewCategories();
            model.modelProductSpec = new ProductSpecViewModel();
            model.modelProductProp = new ProductPropViewModel();
            model.modelProductERP = new ProductERPPlantViewModel();
            model.modelProductERPSale = new ProductERPSaleViewModel();
            model.modelProductCustomer = new ProductCustomer();
            model.modelProductInfo = new ProductInfoView();
            model.modelRouting = new RoutingViewModel();
            model.modelProductPicture = new ProductPictureView();
            model.TransactionDetail = new TransactionDetail();

            MasterDataTransactionModel oMasterData = new MasterDataTransactionModel();
            if (!string.IsNullOrEmpty(MaterialNo))
            {
                model.MaterialNo = MaterialNo;
            }

            if (actionTran == "Presale")
            {
                oMasterData = _newProductService.GetMasterDataTransactionPresale(masterDataTran);
                oMasterData.TransactionAction = actionTran;
                model.PsmId = masterDataTran.MasterData.PsmId;
                if (oMasterData.MasterData != null)
                {
                    SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "MasterDataFromPreSale", oMasterData.MasterData);
                }

            }
            else
            {
                string plantOs = null;
                if (actionTran == "CopyOs")
                {
                    plantOs = model.PlantOs;
                    factoryCode = model.PlantOs;
                }

                //var TransDetailList = JsonConvert.DeserializeObject<List<TransactionsDetail>>(_transactionsDetailAPIRepository.GetTransactionsDetailsByMaterialNoOnly(_factoryCode, model.MaterialNo));
                //if (TransDetailList.Count > 1)
                //    plantOs = TransDetailList.Where(t => t.Outsource == true).OrderBy(t => t.Id).FirstOrDefault().FactoryCode;

                oMasterData = _newProductService.GetMasterDataTransaction(actionTran, MaterialNo, plantOs);
            }

            if (oMasterData.MasterData == null)
            {
                oMasterData.MasterData = new MasterData();
                oMasterData.MasterData.Hierarchy = null;
            }
            //set new transaction details
            if ((actionTran == "Edit" || actionTran == "Copy" || actionTran == "CopyX" || actionTran == "CopyAndDelete" || actionTran == "CreateOs" || actionTran == "CreateTrading" || actionTran == "View" || actionTran == "Detail")
                && oMasterData.TransactionsDetail == null
                && (!string.IsNullOrEmpty(oMasterData.MasterData.Hierarchy)
                && !string.IsNullOrWhiteSpace(oMasterData.MasterData.Hierarchy)))
            {

                //binding data to TransactionsDetail from hierarchy value
                var hierarchyLv2 = oMasterData.MasterData.Hierarchy.Substring(2, 2);

                //var hierarchyLv3 = oMasterData.MasterData.Hierarchy.Substring(4, 3);
                var hierarchyLv4 = string.Empty;
                if (oMasterData.TransactionsDetail == null)
                {
                    if (oMasterData.MasterData.Hierarchy.Length > 9)
                    {
                        hierarchyLv4 = oMasterData.MasterData.Hierarchy.Substring(7, 3);
                    }
                }
                else
                {
                    hierarchyLv4 = oMasterData.TransactionsDetail.HierarchyLv4;
                }


                if (oMasterData.TransactionsDetail != null && oMasterData.MasterData.Hierarchy.Length > 7)
                {
                    oMasterData.TransactionsDetail.HierarchyLv4 = hierarchyLv4;
                }
                else
                {
                    oMasterData.TransactionsDetail = new TransactionsDetail();
                    oMasterData.TransactionsDetail.HierarchyLv4 = null;
                }

                var setCategoryModel = new SetCategoriesOldMat();
                if (hierarchyLv2.ToLower().Equals("so"))
                {
                    var setCategories = JsonConvert.DeserializeObject<List<SetCategoriesOldMat>>(_setCategoriesOldMatAPIRepository.GetSetCategoriesOldMatByLV2(_factoryCode, hierarchyLv2, _token));
                    if (oMasterData.MasterData.BoxType.Contains("ไส้ฟัน"))
                    {
                        setCategoryModel = setCategories.FirstOrDefault(s => s.Lv2.ToLower().Equals("so") && s.PdtName.ToLower().Contains("ไส้ฟัน"));
                    }
                    else if (oMasterData.MasterData.BoxType.Contains("แผ่นรอง"))
                    {
                        setCategoryModel = setCategories.FirstOrDefault(s => s.Lv2.ToLower().Equals("so") && s.PdtName.ToLower().Contains("แผ่นรอง"));
                    }
                    else
                    {
                        setCategoryModel = setCategories.FirstOrDefault(s => s.Lv2.ToLower().Equals("so") && s.PdtName.ToLower().Contains("rsc"));
                    }
                }
                else
                {
                    setCategoryModel = JsonConvert.DeserializeObject<List<SetCategoriesOldMat>>(_setCategoriesOldMatAPIRepository.GetSetCategoriesOldMatByLV2(_factoryCode, hierarchyLv2, _token)).FirstOrDefault();
                    if (setCategoryModel == null)
                    {
                        setCategoryModel = JsonConvert.DeserializeObject<List<SetCategoriesOldMat>>(_setCategoriesOldMatAPIRepository.GetCategoriesMatrixByLV2(_factoryCode, hierarchyLv2, _token)).FirstOrDefault();
                    }
                }

                if (setCategoryModel != null)
                {
                    oMasterData.TransactionsDetail.IdKindOfProduct = setCategoryModel.IdKindOfProduct;
                    oMasterData.TransactionsDetail.IdProcessCost = setCategoryModel.IdProcessCost;
                    oMasterData.TransactionsDetail.IdProductType = setCategoryModel.IdProductType;
                    oMasterData.TransactionsDetail.IdKindOfProductGroup = setCategoryModel.IdKindOfProductGroup;
                }
                else
                {

                    oMasterData.TransactionsDetail.IdKindOfProduct = 0;
                    oMasterData.TransactionsDetail.IdProcessCost = 0;
                    oMasterData.TransactionsDetail.IdProductType = 0;
                    oMasterData.TransactionsDetail.IdKindOfProductGroup = 0;
                }

                //TransactionDataModel transactionDataModel = new TransactionDataModel();
                //transactionDataModel.modelCategories = new ViewCategories();

                ////set it to original material

                isOldMaterial = true;
                //_newProductService.GetCategoriesListData(ref transactionDataModel);

                //oMasterData.TransactionsDetail.IdKindOfProductGroup = null;

                //if (oMasterData.TransactionsDetail.IdProductType.HasValue && (oMasterData.TransactionsDetail.IdProductType.Value != 0))
                //{
                //    var hierarchy = transactionDataModel.modelCategories.HierarchyLV2List.FirstOrDefault(h => h.IdProductType == oMasterData.TransactionsDetail.IdProductType.Value);
                //    oMasterData.TransactionsDetail.IdKindOfProduct = hierarchy.IdKindOfProduct;
                //    oMasterData.TransactionsDetail.IdProcessCost = hierarchy.IdProcessCost;
                //    oMasterData.TransactionsDetail.IdKindOfProductGroup = hierarchy.IdKindOfProductGroup;
                //}
                //else
                //{
                //    oMasterData.TransactionsDetail.IdKindOfProduct = 0;
                //    oMasterData.TransactionsDetail.IdProcessCost = 0;
                //    oMasterData.TransactionsDetail.IdKindOfProductGroup = 0;
                //}

                //if (hierarchyLv2 == "SO")
                //{
                //    oMasterData.TransactionsDetail.IdProductType = 70;
                //}
                //else if (hierarchyLv2 == "DF")
                //{
                //    oMasterData.TransactionsDetail.IdProductType = 100;
                //}
                //else
                //{
                //    var productType = transactionDataModel.modelCategories.ProductTypeList.FirstOrDefault(p => p.HierarchyLv2.Trim() == hierarchyLv2.Trim());
                //    var productTypeName = productType != null ? productType.Name : null;
                //    oMasterData.TransactionsDetail.IdProductType = !string.IsNullOrEmpty(productTypeName) ? productType.Id : 0;
                //}

                //if (hierarchyLv2 == "RD" || hierarchyLv2 == "DF" || hierarchyLv2 == "DF" || hierarchyLv2 == "DC" || hierarchyLv2 == "PB" || hierarchyLv2 == "SO" || hierarchyLv2 == "SH" || hierarchyLv2 == "JR"
                //    || hierarchyLv2 == "JO" || hierarchyLv2 == "JH")
                //{
                //    oMasterData.TransactionsDetail.IdProcessCost = hierarchyLv4 == "OOO" || hierarchyLv4 == "999" ? 90 : 60;
                //}

                var materialType = new MaterialType();
                if (!string.IsNullOrEmpty(oMasterData.MasterData.MaterialType))
                {
                    materialType = JsonConvert.DeserializeObject<MaterialType>(_materialTypeAPIRepository.GetMaterialTypeByMaterialCode(oMasterData.MasterData.MaterialType, _token));
                }

                oMasterData.TransactionsDetail.IdMaterialType = materialType == null ? 5 : materialType.Id;
                oMasterData.TransactionsDetail.IdProductUnit = 1;
                oMasterData.TransactionsDetail.IdSaleUnit = 1;

            }

            model.EventFlag = actionTran;
            model.modelCategories = _newProductService.BindCategoriesData(oMasterData);
            var IdkProd = model.modelCategories.Id_kProd;
            model.KindOfProductName = model.modelCategories.KindOfProductList.Where(k => k.Id == IdkProd).Select(k => k.Name).FirstOrDefault();
            var IdProcCost = model.modelCategories.Id_ProcCost;
            model.ProcessCostName = model.modelCategories.ProcessCostList.Where(o => o.Id == IdProcCost).Select(x => x.Name).FirstOrDefault();
            //var mattype = model.modelCategories.Id_MatType;
            //model.MaterialType = model.modelCategories.MaterialTypeList.Where(m => m.Id == mattype).FirstOrDefault();
            var SaleUit = model.modelCategories.Id_SU;
            model.SaleUnit = model.modelCategories.UnitMaterialList.Where(m => m.Id == SaleUit).Select(x => x.Name).FirstOrDefault();

            //Tassanaithe Update 12/11/2019
            //Get Unitmaterial For Detail
            var MatUnit = model.modelCategories.Id_PU;
            model.UnitMaterial = model.modelCategories.UnitMaterialList.Where(m => m.Id == MatUnit).Select(x => x.Name).FirstOrDefault();
            var mattype = model.modelCategories.Id_MatType;
            model.MaterialType = model.modelCategories.MaterialTypeList.Where(m => m.Id == mattype).Select(x => x.Description).FirstOrDefault();

            model.modelProductCustomer = _newProductService.BindCustomerData(oMasterData);
            model.modelProductInfo = _newProductService.BindProductInfoData(oMasterData);
            model.modelProductSpec = _newProductService.BindProductSpecData(oMasterData, hostEnvironment);
            model.modelProductProp = _newProductService.BindProductProp(oMasterData);
            TransactionDataModel trans = new TransactionDataModel();
            trans.modelCategories = model.modelCategories;
            trans.modelProductProp = model.modelProductProp;
            trans.modelProductSpec = model.modelProductSpec;
            model.modelRouting = _newProductService.BindRoutingData(oMasterData, model.modelProductSpec, trans);
            model.modelProductERPPurchase = _newProductService.BindDataPurchase(oMasterData);
            model.modelProductPicture = _newProductService.BindProductPictureData(oMasterData, hostEnvironment);

            model.modelBuildRemark = JsonConvert.DeserializeObject<List<BuildRemark>>(_buildRemarkAPIRepository.GetBuildRemarkList(factoryCode, _token));
            model.modelBuildRemark = model.modelBuildRemark.OrderBy(x => x.List).ToList();
            model.modelGroupMachineRemark = new List<string>();
            var listtemp = model.modelBuildRemark.GroupBy(x => new { x.Machine }).ToList();
            foreach (var item in listtemp)
            {
                model.modelGroupMachineRemark.Add(item.Key.Machine);
            }
            model.modelGroupMachineRemark = model.modelGroupMachineRemark.OrderBy(x => x).ToList();
            model.amountColor = model.modelProductProp.AmountColor == null ? 0 : model.modelProductProp.AmountColor;
            // model.amountColor = (Int16)(string.IsNullOrEmpty(modelSession.modelProductProp.AmountColor.ToString()) ? 0 : modelSession.modelProductProp.AmountColor);

            model.TransactionDetail = _newProductService.BindDataTransactionDetail(model);
            model.TransactionDetail.IsOldMaterial = isOldMaterial;
            model.TransactionDetail.IsOutSource = oMasterData.TransactionsDetail.Outsource != null ? oMasterData.TransactionsDetail.Outsource : false;
            model.TransactionDetail.MaxStep = oMasterData.TransactionsDetail.MaxStep.HasValue ? oMasterData.TransactionsDetail.MaxStep.Value : 0;
            orderTypeId = oMasterData.TransactionsDetail.HireOrderType != null ? oMasterData.TransactionsDetail.HireOrderType.ToString() : orderTypeId;
            model.TransactionDetail.OrderTypeId = !string.IsNullOrEmpty(orderTypeId) ? Convert.ToInt32(orderTypeId) : 0;
            #region Set transaction detail of ppc from presale
            model.TransactionDetail.NewPrintPlate = oMasterData.TransactionsDetail.NewPrintPlate;
            model.TransactionDetail.OldPrintPlate = oMasterData.TransactionsDetail.OldPrintPlate;
            model.TransactionDetail.NewBlockDieCut = oMasterData.TransactionsDetail.NewBlockDieCut;
            model.TransactionDetail.OldBlockDieCut = oMasterData.TransactionsDetail.OldBlockDieCut;
            model.TransactionDetail.ExampleColor = oMasterData.TransactionsDetail.ExampleColor;
            model.TransactionDetail.CoatingType = oMasterData.TransactionsDetail.CoatingType;
            model.TransactionDetail.CoatingTypeDesc = oMasterData.TransactionsDetail.CoatingTypeDesc;
            model.TransactionDetail.PaperHorizontal = oMasterData.TransactionsDetail.PaperHorizontal;
            model.TransactionDetail.PaperVertical = oMasterData.TransactionsDetail.PaperVertical;
            model.TransactionDetail.FluteHorizontal = oMasterData.TransactionsDetail.FluteHorizontal;
            model.TransactionDetail.FluteVertical = oMasterData.TransactionsDetail.FluteVertical;
            #endregion
            model.MaterialNo = actionTran == "Presale" ? null : MaterialNo;
            _newProductService.SetTransactionStatus(ref model, "Categories");
            model.SapStatus = !string.IsNullOrEmpty(model.MaterialNo) ? JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNo(factoryCode, model.MaterialNo, _token)).SapStatus : false;
            model.SaleOrg = oMasterData.MasterData.SaleOrg;

            return model;
        }
    }
}
