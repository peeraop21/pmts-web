using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PMTs.DataAccess;
using PMTs.DataAccess.ComplexModel;
using PMTs.DataAccess.ModelPresale;
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
using PMTs.WebApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services
{
    [TraceAspect]
    public class ProductInfoService : IProductInfoService
    {
        private IHttpContextAccessor _httpContextAccessor;

        private readonly INewProductService _newProductService;

        private readonly IMaterialTypeAPIRepository _materialTypeAPIRepository;
        private readonly IRunningNoAPIRepository _runningNoAPIRepository;
        private readonly IMasterDataAPIRepository _masterDataAPIRepository;
        private readonly IUnitMaterialAPIRepository _unitMaterialAPIRepository;
        private readonly IKindOfProductAPIRepository _kindOfProductAPIRepository;
        private readonly ITransactionsDetailAPIRepository _transactionsDetailAPIRepository;
        private readonly ISalesViewAPIRepository _salesViewAPIRepository;
        private readonly IPMTsConfigAPIRepository _pmtsConfigAPIRepository;
        private readonly ICompanyProfileAPIRepository _companyProfileAPIRepository;
        private readonly IPlantViewAPIRepository _plantViewAPIRepository;
        private readonly IPresaleService _presaleService;
        private readonly IHvaMasterAPIRepository _hvaMasterAPIRepository;
        private readonly IQualitySpecAPIRepository _qualitySpecAPIRepository;
        private readonly IRoutingAPIRepository _routingAPIRepository;
        private readonly IHireOrderAPIRepository _hireOrderAPIRepository;
        private readonly IBoardUseAPIRepository _boardUseAPIRepository;
        private readonly IBoardCombineAccAPIRepository _boardCombineAccAPIRepository;
        private readonly IAutoPackingSpecAPIRepository autoPackingSpecAPIRepository;
        private readonly IRoutingService _routingService;
        private static IConfiguration _configuration;
        private readonly IFSCCodeAPIRepository fscCodeAPIRepository;
        private readonly IFSCFGCodeAPIRepository fscFGCodeAPIRepository;
        private readonly IPPCRawMaterialProductionBomAPIRepository pPCRawMaterialProductionBomAPIRepository;
        private readonly IFormulaAPIRepository _formulaAPIRepository;
        private readonly IAutoPackingCustomerAPIRepository autoPackingCustomerAPIRepository;

        private readonly string _username;
        private readonly string _saleOrg;
        private readonly string _factoryCode;
        private readonly string _token;

        public ProductInfoService(IHttpContextAccessor httpContextAccessor,
            INewProductService newProductService,
            IMaterialTypeAPIRepository materialTypeAPIRepository,
            IRunningNoAPIRepository runningNoAPIRepository,
            IMasterDataAPIRepository masterDataAPIRepository,
            IUnitMaterialAPIRepository unitMaterialAPIRepository,
            IKindOfProductAPIRepository kindOfProductAPIRepository,
            ITransactionsDetailAPIRepository transactionsDetailAPIRepository,
            ISalesViewAPIRepository salesViewAPIRepository,
            IPMTsConfigAPIRepository pmtsConfigAPIRepository,
            ICompanyProfileAPIRepository companyProfileAPIRepository,
            IPlantViewAPIRepository plantViewAPIRepository,
            IPresaleService presaleService,
            IHvaMasterAPIRepository hvaMasterAPIRepository,
            IRoutingAPIRepository routingAPIRepository,
            IQualitySpecAPIRepository qualitySpecAPIRepository,
            IHireOrderAPIRepository hireOrderAPIRepository,
            IConfiguration configuration,
            IRoutingService routingService,
            IBoardUseAPIRepository boardUseAPIRepository,
            IBoardCombineAccAPIRepository boardCombineAccAPIRepository,
            IAutoPackingSpecAPIRepository autoPackingSpecAPIRepository,
            IFSCCodeAPIRepository fSCCodeAPIRepository,
            IFSCFGCodeAPIRepository fSCFGCodeAPIRepository,
            IPPCRawMaterialProductionBomAPIRepository pPCRawMaterialProductionBomAPIRepository,
            IFormulaAPIRepository formulaAPIRepository,
            IAutoPackingCustomerAPIRepository autoPackingCustomerRepository
            )
        {
            _httpContextAccessor = httpContextAccessor;

            _newProductService = newProductService;

            _materialTypeAPIRepository = materialTypeAPIRepository;
            _runningNoAPIRepository = runningNoAPIRepository;
            _masterDataAPIRepository = masterDataAPIRepository;
            _unitMaterialAPIRepository = unitMaterialAPIRepository;
            _kindOfProductAPIRepository = kindOfProductAPIRepository;
            _transactionsDetailAPIRepository = transactionsDetailAPIRepository;
            _salesViewAPIRepository = salesViewAPIRepository;
            _pmtsConfigAPIRepository = pmtsConfigAPIRepository;
            _companyProfileAPIRepository = companyProfileAPIRepository;
            _plantViewAPIRepository = plantViewAPIRepository;
            _hvaMasterAPIRepository = hvaMasterAPIRepository;
            _qualitySpecAPIRepository = qualitySpecAPIRepository;
            _routingAPIRepository = routingAPIRepository;
            _hireOrderAPIRepository = hireOrderAPIRepository;
            _configuration = configuration;
            _routingService = routingService;
            _boardUseAPIRepository = boardUseAPIRepository;
            _boardCombineAccAPIRepository = boardCombineAccAPIRepository;
            this.autoPackingSpecAPIRepository = autoPackingSpecAPIRepository;
            this.fscCodeAPIRepository = fSCCodeAPIRepository;
            this.fscFGCodeAPIRepository = fSCFGCodeAPIRepository;
            this.pPCRawMaterialProductionBomAPIRepository = pPCRawMaterialProductionBomAPIRepository;
            _presaleService = presaleService;
            _formulaAPIRepository = formulaAPIRepository;
            this.autoPackingCustomerAPIRepository = autoPackingCustomerRepository;

            var userSessionModel = SessionExtentions.GetSession<UserSessionModel>(_httpContextAccessor.HttpContext.Session, "UserSessionModel");
            if (userSessionModel != null)
            {
                _username = userSessionModel.UserName;
                _saleOrg = userSessionModel.SaleOrg;
                _factoryCode = userSessionModel.FactoryCode;
                _token = userSessionModel.Token;
            }
        }

        public void GetProductInfo(NewProductController newProductController, ref TransactionDataModel transactionDataModel)
        {
            var transactionDataModelSession = SessionExtentions.GetSession<TransactionDataModel>(_httpContextAccessor.HttpContext.Session, "TransactionDataModel");

            if (transactionDataModelSession.modelProductInfo == null)
                transactionDataModelSession.modelProductInfo = new ProductInfoView();

            transactionDataModelSession.SaleOrg = string.IsNullOrEmpty(transactionDataModelSession.SaleOrg) ? _saleOrg : transactionDataModelSession.SaleOrg;
            //get hva select
            GetHVLProductTypeSelectList(ref transactionDataModelSession);

            //get Outsourcing list
            GetOutsources(ref transactionDataModelSession);

            if (string.IsNullOrWhiteSpace(transactionDataModelSession.modelProductInfo.Description) || string.IsNullOrEmpty(transactionDataModelSession.modelProductInfo.Description))
            {
                if (!string.IsNullOrEmpty(transactionDataModelSession.modelProductCustomer.CustCode) || !string.IsNullOrWhiteSpace(transactionDataModelSession.modelProductCustomer.CustCode))
                {
                    transactionDataModelSession.modelProductInfo.Description = transactionDataModelSession.modelProductCustomer.CustCode + " ";
                }
                else
                {
                    transactionDataModelSession.modelProductInfo.Description = "";
                }
            }

            if (transactionDataModelSession.TransactionDetail == null)
            {
                transactionDataModelSession.TransactionDetail = new TransactionDetail();
            }
            transactionDataModelSession.TransactionDetail.ProductTypDetail = transactionDataModelSession.modelCategories.ProductTypeName;
            transactionDataModelSession.TransactionDetail.HierarchyLV2Detail = transactionDataModelSession.modelCategories.HierarchyLV2;
            transactionDataModelSession.TransactionDetail.CustNameDetail = transactionDataModelSession.modelProductCustomer.CustName;
            transactionDataModelSession.TransactionDetail.PCDetail = transactionDataModelSession.modelProductInfo.PC;
            transactionDataModelSession.TransactionDetail.MaterialDescriptionDetail = transactionDataModelSession.modelProductInfo.Description;
            if (transactionDataModelSession.TransactionDetail.RoutingDetail == null)
            {
                transactionDataModelSession.TransactionDetail.RoutingDetail = new List<RoutingDataModel>();
            }

            transactionDataModel.CurrentTransaction = "ProductInformation";

            _newProductService.SetTransactionStatus(ref transactionDataModelSession, "ProductInformation");

            //if (transactionDataModelSession.TransactionDetail.IsCreateBOM)
            //{
            //    transactionDataModelSession.modelCategories.Id_MatType = 7;
            //    transactionDataModelSession.EventFlag = "Copy";
            //    transactionDataModelSession.modelProductSpec = new ProductSpecViewModel();
            //}
            var transactionDetail = JsonConvert.DeserializeObject<TransactionsDetail>(_transactionsDetailAPIRepository.GetTransactionsDetailByMat(_factoryCode, transactionDataModelSession.MaterialNo, _token));
            if (transactionDetail != null && !string.IsNullOrEmpty(transactionDetail.MatSaleOrg))
            {
                transactionDataModelSession.TransactionDetail.MaterialSaleOrg = transactionDetail.MatSaleOrg;
                transactionDataModelSession.modelProductInfo.MaterialSaleOrg = transactionDetail.MatSaleOrg;
            }

            //set FSC code and fg code
            if (!string.IsNullOrEmpty(transactionDataModelSession.modelCategories.fscCode))
            {
                var newSaleText4 = transactionDataModelSession.modelProductInfo.SaleText4 + transactionDataModelSession.modelCategories.fscCode;
                transactionDataModelSession.modelProductInfo.SaleText4 = !string.IsNullOrEmpty(newSaleText4) && newSaleText4.Length > 40 ? newSaleText4.Substring(0, 40) : newSaleText4;
            }

            transactionDataModel = transactionDataModelSession;
            transactionDataModel.modelProductInfo.CompanyProfiles = JsonConvert.DeserializeObject<List<CompanyProfile>>(_companyProfileAPIRepository.GetCompanyProfilesByStatusPMTs(_factoryCode, _token));
            transactionDataModel.modelProductInfo.HireOrders = JsonConvert.DeserializeObject<List<HireOrder>>(_hireOrderAPIRepository.GetAllHireOrder(_factoryCode, _token));
            transactionDataModel.modelProductCustomer.CustCode = string.IsNullOrEmpty(transactionDataModel.modelProductCustomer.CustCode) ? "" : transactionDataModel.modelProductCustomer.CustCode.Trim();
            SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "TransactionDataModel", transactionDataModelSession);

            newProductController.ViewBag.PrintMaster = "Hide";
        }

        public void SaveProductInfo(ref TransactionDataModel transactionDataModel, ref bool isOverBackward, ref bool isExistCost)
        {
            TransactionDataModel transactionDataModelSession = new TransactionDataModel();
            transactionDataModelSession = SessionExtentions.GetSession<TransactionDataModel>(_httpContextAccessor.HttpContext.Session, "TransactionDataModel");
            var originalMaterialNo = string.IsNullOrEmpty(transactionDataModelSession.MaterialNo) ? string.Empty : transactionDataModelSession.MaterialNo.ToString();
            transactionDataModel.EventFlag = transactionDataModelSession.EventFlag.ToString();
            string materialNo = string.Empty;
            string matTypeOfSession = transactionDataModelSession.modelCategories.MatCode;
            string matTypeOs = string.Empty;

            if (transactionDataModelSession.EventFlag == "Edit" || (transactionDataModelSession.MaterialNo != null && transactionDataModelSession.EventFlag == "Create"))
            {
                var isOursource = false;
                var materialNoOsEdit = string.Empty;
                // Get Material No From Transaction
                materialNo = transactionDataModelSession.MaterialNo;

                #region Update Transaction detail

                //get exist TransactionDetail
                var transactionDetailObject = JsonConvert.DeserializeObject<TransactionsDetail>(_transactionsDetailAPIRepository.GetTransactionsDetailByMat(_factoryCode, transactionDataModelSession.MaterialNo, _token));
                isOursource = transactionDetailObject != null && transactionDetailObject.Outsource ? true : false;
                transactionDetailObject.HvaGroup1 = transactionDataModel.modelProductInfo.HvaProductType;
                transactionDetailObject.HvaGroup2 = transactionDataModel.modelProductInfo.HvaStructural;
                transactionDetailObject.HvaGroup3 = transactionDataModel.modelProductInfo.HvaPrinting;
                transactionDetailObject.HvaGroup4 = transactionDataModel.modelProductInfo.HvaFlute;
                transactionDetailObject.HvaGroup5 = transactionDataModel.modelProductInfo.HvaCorrugating;
                transactionDetailObject.HvaGroup6 = transactionDataModel.modelProductInfo.HvaCoating;
                transactionDetailObject.HvaGroup7 = transactionDataModel.modelProductInfo.HvaFinishing;
                transactionDetailObject.MatSaleOrg = transactionDataModel.modelProductInfo.MaterialSaleOrg;
                //if (!string.IsNullOrEmpty(transactionDataModel.modelProductInfo.PLANTCODE))
                //{
                //    transactionDetailObject.Outsource = false;

                //}

                var parentModelTransactionsDetail = new ParentModel()
                {
                    AppName = Globals.AppNameEncrypt,
                    SaleOrg = _saleOrg,
                    PlantCode = _factoryCode,
                    TransactionsDetail = transactionDetailObject
                };

                var jsonTransactionsDetail = JsonConvert.SerializeObject(parentModelTransactionsDetail);
                _transactionsDetailAPIRepository.UpdateTransactionsDetail(jsonTransactionsDetail, _token);

                #endregion Update Transaction detail

                #region update master data

                #region Set Master data Model

                //update column CustRequirement, MatComment in masterdata
                var existMasterdata = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNo(_factoryCode, materialNo, _token));
                //existMasterdata.CusId = transactionDataModelSession.modelProductCustomer.CusId;
                //existMasterdata.CustCode = transactionDataModelSession.modelProductCustomer.CustCode;
                //if (!String.IsNullOrEmpty(existMasterdata.CustName) && !String.IsNullOrEmpty(transactionDataModelSession.modelProductCustomer.CustName))
                //{
                //    existMasterdata.CustName = transactionDataModelSession.modelProductCustomer.CustName.Length > 50 ? transactionDataModelSession.modelProductCustomer.CustName.Substring(0, 50) : transactionDataModelSession.modelProductCustomer.CustName;
                //}
                //existMasterdata.CustComment = transactionDataModelSession.modelProductCustomer.CustReq;
                //existMasterdata.IndGrp = transactionDataModelSession.modelProductCustomer.IndDes == null ? null : transactionDataModelSession.modelProductCustomer.IndDes.Substring(0, 3);
                //existMasterdata.IndDes = transactionDataModelSession.modelProductCustomer.IndDes;
                //existMasterdata.MaterialComment = transactionDataModelSession.modelProductCustomer.MaterialComment;
                existMasterdata.HighValue = transactionDataModel.modelProductInfo.HighValue;

                if (existMasterdata.PdisStatus != "N" && existMasterdata.SapStatus == true)
                {
                    existMasterdata.PdisStatus = "M";
                    existMasterdata.TranStatus = isOursource && transactionDataModelSession.TransactionDetail.OrderTypeId != 4 ? true : false;

                    transactionDataModelSession.PdisStatus = "M";

                    #region Update Routing Status

                    if (!isOursource)
                    {
                        UpdateRoutingStatus(existMasterdata);
                    }

                    #endregion Update Routing Status
                }

                var isNotDuplicate = false;

                #region Check duplicate PC, Description

                if (!transactionDataModelSession.modelCategories.FormGroup.Equals("AC") && matTypeOfSession != "82")
                {
                    if ((transactionDataModel.modelProductInfo.Description != transactionDataModelSession.modelProductInfo.Description))
                    {
                        if (!DescriptionCheck(transactionDataModel.modelProductInfo.Description))
                        {
                            throw new Exception("Duplicated description!");
                        }
                        else
                        {
                            existMasterdata.Description =
                                !string.IsNullOrEmpty(transactionDataModel.modelProductInfo.Description) ?
                                transactionDataModel.modelProductInfo.Description.Replace("'", "").Replace(";", "").TrimEnd(new Char[] { ' ' }) :
                                transactionDataModel.modelProductInfo.Description.TrimEnd(new Char[] { ' ' });
                        }
                    }
                    else
                    {
                        existMasterdata.Description =
                                !string.IsNullOrEmpty(transactionDataModel.modelProductInfo.Description) ?
                                transactionDataModel.modelProductInfo.Description.Replace("'", "").Replace(";", "").TrimEnd(new Char[] { ' ' }) :
                                transactionDataModel.modelProductInfo.Description.TrimEnd(new Char[] { ' ' });
                    }

                    if (transactionDataModel.modelProductInfo.PC != transactionDataModelSession.modelProductInfo.PC)
                    {
                        if (!ProdCodeCheck(transactionDataModel.modelProductInfo.PC))
                        {
                            throw new Exception("Duplicated product code!");
                        }
                        else
                        {
                            existMasterdata.Pc = transactionDataModel.modelProductInfo.PC;
                        }
                    }
                    else
                    {
                        existMasterdata.Pc = transactionDataModel.modelProductInfo.PC;
                    }
                }
                else
                {
                    existMasterdata.Description =
                                !string.IsNullOrEmpty(transactionDataModel.modelProductInfo.Description) ?
                                transactionDataModel.modelProductInfo.Description.Replace("'", "").Replace(";", "").TrimEnd(new Char[] { ' ' }) :
                                transactionDataModel.modelProductInfo.Description.TrimEnd(new Char[] { ' ' });

                    if (transactionDataModel.modelProductInfo.PC != transactionDataModelSession.modelProductInfo.PC)
                    {
                        if (!ProdCodeCheck(transactionDataModel.modelProductInfo.PC))
                        {
                            throw new Exception("Duplicated product code!");
                        }
                        else
                        {
                            existMasterdata.Pc = transactionDataModel.modelProductInfo.PC;
                        }
                    }
                    else
                    {
                        existMasterdata.Pc = transactionDataModel.modelProductInfo.PC;
                    }
                }

                #endregion Check duplicate PC, Description

                existMasterdata.PartNo = transactionDataModel.modelProductInfo.PartNo;
                existMasterdata.SaleText1 = transactionDataModel.modelProductInfo.SaleText1;
                existMasterdata.SaleText2 = transactionDataModel.modelProductInfo.SaleText2;
                existMasterdata.SaleText3 = transactionDataModel.modelProductInfo.SaleText3;
                existMasterdata.SaleText4 = transactionDataModel.modelProductInfo.SaleText4;
                existMasterdata.UpdatedBy = _username;
                existMasterdata.LastUpdate = DateTime.Now;
                existMasterdata.FscCode = transactionDataModelSession.modelCategories.fscCode;
                existMasterdata.FscFgCode = transactionDataModelSession.modelCategories.fscFgCode;
                existMasterdata.RpacLob = transactionDataModelSession.modelCategories.RpacLob;
                existMasterdata.RpacProgram = transactionDataModelSession.modelCategories.RpacProgram;
                existMasterdata.RpacBrand = transactionDataModelSession.modelCategories.RpacBrand;
                existMasterdata.RpacPackagingType = transactionDataModelSession.modelCategories.RpacPackagingType;

                if (transactionDataModelSession.TransactionDetail.IsOutSource && !transactionDataModelSession.RealEventFlag.Contains("Copy"))
                {
                    existMasterdata.TranStatus = transactionDataModelSession.TransactionDetail.OrderTypeId != 4 ? true : false;
                    existMasterdata.User = _username;
                }

                if (existMasterdata.MaterialType == "82")
                {
                    existMasterdata.PurTxt1 = existMasterdata.Pc;
                    existMasterdata.PurTxt2 = existMasterdata.SaleText1;
                    existMasterdata.PurTxt3 = existMasterdata.SaleText2;
                    var matOutsources = _transactionsDetailAPIRepository.GetAllMatOutsourceByMaterialNo(_factoryCode, existMasterdata.MaterialNo, _token);
                    existMasterdata.PurTxt4 = string.IsNullOrEmpty(matOutsources) ? existMasterdata.SaleText3 : matOutsources;
                }

                if (!string.IsNullOrEmpty(existMasterdata.BoxType))
                {
                    existMasterdata.SizeDimensions = JsonConvert.DeserializeObject<string>(_formulaAPIRepository.CalSizeDimensions(_factoryCode, materialNo, _token));
                }

                #endregion Set Master data Model

                var parentModelMasterData = new ParentModel()
                {
                    AppName = Globals.AppNameEncrypt,
                    SaleOrg = _saleOrg,
                    PlantCode = _factoryCode,
                    MasterData = existMasterdata
                };

                var jsonExistMasterdata = JsonConvert.SerializeObject(parentModelMasterData);
                _masterDataAPIRepository.UpdateMasterData(jsonExistMasterdata, _token);

                #endregion update master data

                #region Update SaleView and PlantView

                if (!isOursource)
                {
                    //Update SaleView
                    UpdateSalesView(materialNo, existMasterdata);
                    //Update PlantView
                    UpdatePlantView(materialNo, existMasterdata);
                }

                #endregion Update SaleView and PlantView

                #region Save New MasterData for Outsource

                if (!string.IsNullOrEmpty(transactionDataModel.modelProductInfo.PLANTCODE))
                {
                    var plantOS = transactionDataModel.modelProductInfo.PLANTCODE;
                    var companyProfiles = JsonConvert.DeserializeObject<List<CompanyProfile>>(_companyProfileAPIRepository.GetCompanyProfileList(_factoryCode, _token));
                    var saleOrgOS = companyProfiles.FirstOrDefault(c => c.Plant == plantOS).SaleOrg;
                    //exist master data
                    var existOriginalMasterdata = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNo(_factoryCode, transactionDataModelSession.MaterialNo, _token));
                    var jsonExistOriginalMasterdata = JsonConvert.SerializeObject(existOriginalMasterdata);

                    //exist routing data
                    var orginalRoutings = JsonConvert.DeserializeObject<List<Routing>>(_routingAPIRepository.GetRoutingsByMaterialNo(_factoryCode, transactionDataModelSession.MaterialNo, _token));
                    var routingOsList = new List<Routing>();
                    var existMasterDataPm2OS = false;
                    var materialNoPm2OS = string.Empty;

                    #region Set Material Type

                    var plantOutsource = transactionDataModel.modelProductInfo.PLANTCODE;
                    var companyProfile = JsonConvert.DeserializeObject<List<CompanyProfile>>(_companyProfileAPIRepository.GetCompanyProfileList(_factoryCode, _token));
                    var groupOfFactory = companyProfile.FirstOrDefault(c => c.Plant == _factoryCode).Group;
                    var groupOfOutsource = companyProfile.FirstOrDefault(c => c.Plant == plantOutsource).Group;
                    string matTypeNewOS = existOriginalMasterdata.MaterialType;

                    //set new matType
                    //if (((groupOfFactory == 1 || groupOfFactory == 2 || groupOfFactory == 3) && groupOfOutsource == 4) || transactionDataModel.modelProductInfo.OrderTypeId == 4)
                    //{
                    //    matTypeNewOS = "81";
                    //}

                    #endregion Set Material Type

                    #region Generate Material No

                    var orderTypeId = transactionDataModel.modelProductInfo.OrderTypeId.Value;
                    var hireOrderItem = JsonConvert.DeserializeObject<HireOrder>(_hireOrderAPIRepository.GetHireOrderById(_factoryCode, orderTypeId, _token));
                    if (hireOrderItem.SyncMat.HasValue && hireOrderItem.SyncMat.Value)
                    {
                        materialNoOsEdit = existOriginalMasterdata.MaterialNo;
                    }
                    else
                    {
                        //var existOsTransactionDetail = JsonConvert.DeserializeObject<TransactionsDetail>(_transactionsDetailAPIRepository.GetMatOutsourceByMatSaleOrg(_factoryCode, existOriginalMasterdata.MaterialNo, _token));
                        var existOsTransactionDetail = JsonConvert.DeserializeObject<TransactionsDetail>(_transactionsDetailAPIRepository.GetMatOutsourceByMatSaleOrg(_factoryCode, existOriginalMasterdata.MaterialNo, _token));

                        if (existOsTransactionDetail != null)
                        {
                            //var originalMatType = JsonConvert.DeserializeObject<MaterialType>(_materialTypeAPIRepository.GetMaterialCode(existOsTransactionDetail.IdMaterialType.Value, _token)).MatCode;
                            //if (originalMatType == matType)
                            //    materialNoOsEdit = existOsTransactionDetail.MaterialNo;
                            //else
                            if (existOsTransactionDetail.FactoryCode == transactionDataModel.modelProductInfo.PLANTCODE)
                                materialNoOsEdit = existOsTransactionDetail.MaterialNo;
                            else
                                materialNoOsEdit = GenMatNo(matTypeNewOS, transactionDataModel.modelProductInfo.PLANTCODE);
                        }
                        else
                        {
                            materialNoOsEdit = GenMatNo(matTypeNewOS, transactionDataModel.modelProductInfo.PLANTCODE);
                        }

                        var factoryProfile = JsonConvert.DeserializeObject<CompanyProfile>(_companyProfileAPIRepository.GetCompanyProfileByPlant(transactionDataModel.modelProductInfo.PLANTCODE, _token));
                        // Update RunningNo
                        UpdateRunningNo(matTypeNewOS, factoryProfile.Plant, factoryProfile.SaleOrg);
                    }

                    #endregion Generate Material No

                    if (transactionDataModelSession.SapStatus == false)
                    {
                        var existOsTransactionDetail = JsonConvert.DeserializeObject<TransactionsDetail>(_transactionsDetailAPIRepository.GetTransactionsDetailFirstOutsource(_factoryCode, existOriginalMasterdata.MaterialNo, _token));

                        if (existOsTransactionDetail != null)
                        {
                            var originalOsMatNo = existOsTransactionDetail.MaterialNo.ToString();
                            var originalFactoryCode = existOsTransactionDetail.FactoryCode.ToString();

                            #region Update TransactionDetail

                            var updateTransactionDetailOs = new TransactionsDetail();
                            updateTransactionDetailOs = existOsTransactionDetail;
                            updateTransactionDetailOs.FactoryCode = transactionDataModel.modelProductInfo.PLANTCODE;
                            updateTransactionDetailOs.HireOrderType = transactionDataModel.modelProductInfo.OrderTypeId;
                            updateTransactionDetailOs.MatSaleOrg = originalMaterialNo;
                            updateTransactionDetailOs.MaxStep = 0;
                            updateTransactionDetailOs.MaterialNo = materialNoOsEdit;
                            updateTransactionDetailOs.IdMaterialType = transactionDataModel.modelProductInfo.OrderTypeId == 4 ? 5 : updateTransactionDetailOs.IdMaterialType;

                            var parentModelexistOsTransactionDetail = new ParentModel()
                            {
                                AppName = Globals.AppNameEncrypt,
                                SaleOrg = _saleOrg,
                                PlantCode = _factoryCode,
                                TransactionsDetail = updateTransactionDetailOs
                            };

                            var jsonExistOsTransactionDetail = JsonConvert.SerializeObject(parentModelexistOsTransactionDetail);
                            _transactionsDetailAPIRepository.UpdateTransactionsDetail(jsonExistOsTransactionDetail, _token);

                            #endregion Update TransactionDetail

                            var existOsMasterData = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNo(originalFactoryCode, originalOsMatNo, _token));

                            #region Update MasterData

                            if (existOsMasterData != null)
                            {
                                existOsMasterData.FactoryCode = transactionDataModel.modelProductInfo.PLANTCODE;
                                existOsMasterData.Plant = transactionDataModel.modelProductInfo.PLANTCODE;
                                existOsMasterData.MaterialNo = materialNoOsEdit;
                                existOsMasterData.User = _username;
                                existOsMasterData.SapStatus = false;
                                existOsMasterData.HireFactory = _factoryCode;
                                existOsMasterData.SaleOrg = transactionDataModel.modelProductInfo.OrderTypeId == 4 ? saleOrgOS : existMasterdata.SaleOrg;
                                existOsMasterData.TranStatus = isOursource ? true : transactionDataModel.modelProductInfo.OrderTypeId == 4 ? false : true;
                                existOsMasterData.MaterialType = transactionDataModel.modelProductInfo.OrderTypeId == 4 ? "81" : existOsMasterData.MaterialType;
                                existOsMasterData.MatCopy = null;
                                existOsMasterData.FscCode = transactionDataModelSession.modelCategories.fscCode;
                                existOsMasterData.FscFgCode = transactionDataModelSession.modelCategories.fscFgCode;
                                existOsMasterData.RpacLob = transactionDataModelSession.modelCategories.RpacLob;
                                existOsMasterData.RpacProgram = transactionDataModelSession.modelCategories.RpacProgram;
                                existOsMasterData.RpacBrand = transactionDataModelSession.modelCategories.RpacBrand;
                                existOsMasterData.RpacPackagingType = transactionDataModelSession.modelCategories.RpacPackagingType;
                                existOsMasterData.User = _username;
                                existOsMasterData.MaterialType = matTypeNewOS;
                                existOsMasterData.AttachFileMoPath = existMasterdata.AttachFileMoPath;
                                existOsMasterData.SemiFilePdfPath = existMasterdata.SemiFilePdfPath;

                                //check mattype to set new SaleText4
                                if (existOriginalMasterdata.MaterialType == "82" && matTypeNewOS == "81")
                                {
                                    existOsMasterData.SaleText4 = existOsMasterData.SaleText4 + " " + existOriginalMasterdata.MaterialNo + " " + existOriginalMasterdata.Pc;
                                }
                                existOsMasterData.SaleText4 = existOsMasterData.SaleText4 != null && existOsMasterData.SaleText4.Length >= 40 ? existOsMasterData.SaleText4.Substring(0, 40) : existOsMasterData.SaleText4;

                                if (!string.IsNullOrEmpty(existMasterdata.BoxType))
                                {
                                    existMasterdata.SizeDimensions = JsonConvert.DeserializeObject<string>(_formulaAPIRepository.CalSizeDimensions(_factoryCode, materialNo, _token));
                                }

                                var parentModelexistOsMasterData = new ParentModel()
                                {
                                    AppName = Globals.AppNameEncrypt,
                                    SaleOrg = _saleOrg,
                                    PlantCode = _factoryCode,
                                    MasterData = existOsMasterData
                                };

                                var jsonExistOsMasterData = JsonConvert.SerializeObject(parentModelexistOsMasterData);
                                _masterDataAPIRepository.UpdateMasterData(jsonExistOsMasterData, _token);

                                //Create boarduse when doesn't exist.
                                if (!string.IsNullOrEmpty(existOriginalMasterdata.Code))
                                {
                                    CheckCreateBoardUse(originalMaterialNo, materialNoOsEdit, transactionDataModel.modelProductInfo.PLANTCODE);
                                }
                            }

                            #endregion Update MasterData

                            var existOsSaleViews = JsonConvert.DeserializeObject<List<SalesView>>(_salesViewAPIRepository.GetSaleViewsByMaterialNo(originalFactoryCode, originalOsMatNo, _token));

                            #region Update SaleViews

                            //delete all saleview OS
                            if (existOsSaleViews.Count > 0)
                            {
                                existOsSaleViews = existOsSaleViews.Where(e => e.FactoryCode != _factoryCode).ToList();
                                var jsonExistOsSaleViews = JsonConvert.SerializeObject(existOsSaleViews);
                                _salesViewAPIRepository.DeleteSaleViews(jsonExistOsSaleViews, _token);

                                var infoModel = new ProductInfoView();
                                infoModel = transactionDataModel.modelProductInfo;

                                UpdateSalesViewOs(existOriginalMasterdata.MaterialNo, infoModel, materialNoOsEdit);
                            }
                            else
                            {
                                if (transactionDataModel.modelProductInfo.OrderTypeId != 4)
                                {
                                    SaveSalesView(existOriginalMasterdata.MaterialNo, matTypeNewOS, transactionDataModelSession.modelProductCustomer.CustCode, transactionDataModel.modelProductInfo, materialNoOsEdit);
                                }
                            }

                            #endregion Update SaleViews

                            var existOsPlantView = JsonConvert.DeserializeObject<PlantView>(_plantViewAPIRepository.GetPlantViewByMaterialNoAndPlant(originalFactoryCode, originalOsMatNo, originalFactoryCode, _token));

                            #region Update PlantView

                            if (existOsPlantView != null)
                            {
                                #region Update PlantView

                                existOsPlantView.FactoryCode = transactionDataModel.modelProductInfo.PLANTCODE;
                                existOsPlantView.Plant = transactionDataModel.modelProductInfo.PLANTCODE;
                                existOsPlantView.MaterialNo = materialNoOsEdit;
                                existOsPlantView.PurchCode = JsonConvert.DeserializeObject<CompanyProfile>(_companyProfileAPIRepository.GetCompanyProfileByPlant(transactionDataModel.modelProductInfo.PLANTCODE, _token)).PurchasGrp;

                                double costPerTon = 0;
                                if (transactionDataModelSession.modelCategories.MatCode == "82")
                                {
                                    costPerTon = existOsPlantView.StdMovingCost;
                                }
                                else
                                {
                                    costPerTon = existOsPlantView.StdTotalCost;
                                }

                                if (costPerTon == 0)
                                {
                                    existOsPlantView.PdisStatus = "N";
                                    isExistCost = false;
                                }

                                var parentModelexistOsPlantView = new ParentModel()
                                {
                                    AppName = Globals.AppNameEncrypt,
                                    SaleOrg = _saleOrg,
                                    PlantCode = _factoryCode,
                                    PlantView = existOsPlantView
                                };

                                var jsonExistOsPlantView = JsonConvert.SerializeObject(parentModelexistOsPlantView);
                                _plantViewAPIRepository.UpdatePlantView(jsonExistOsPlantView, _token);

                                #endregion Update PlantView
                            }

                            #endregion Update PlantView

                            #region Check and update routing with OS

                            var existRoutingOs = JsonConvert.DeserializeObject<List<Routing>>(_routingAPIRepository.GetRoutingsByMaterialNo(originalFactoryCode, originalOsMatNo, _token));
                            var OsRoutings = new List<Routing>();

                            if (existRoutingOs.Count == 0)
                            {
                                OsRoutings = new List<Routing>();

                                #region Save routings OS from Original routings

                                foreach (var orginalRouting in orginalRoutings)
                                {
                                    orginalRouting.FactoryCode = transactionDataModel.modelProductInfo.PLANTCODE;
                                    orginalRouting.Plant = transactionDataModel.modelProductInfo.PLANTCODE;
                                    orginalRouting.Id = 0;
                                    /// ณัตเพิ่ม clear ค่า Block No, PlateNo, Color จากผู้จ้าง เพราะผู้ผลิตไม่ได้ใช้ แล้วเค้าไม่เห็น ลบเองไม่ได้
                                    orginalRouting.BlockNo = null;
                                    orginalRouting.PlateNo = null;
                                    orginalRouting.MylaNo = null;
                                    orginalRouting.MylaSize = null;
                                    //orginalRouting.Color1 = null;
                                    //orginalRouting.Color2 = null;
                                    //orginalRouting.Color3 = null;
                                    //orginalRouting.Color4 = null;
                                    //orginalRouting.Color5 = null;
                                    //orginalRouting.Color6 = null;
                                    //orginalRouting.Color7 = null;
                                    //orginalRouting.Color8 = null;
                                    //orginalRouting.Shade1 = null;
                                    //orginalRouting.Shade2 = null;
                                    //orginalRouting.Shade3 = null;
                                    //orginalRouting.Shade4 = null;
                                    //orginalRouting.Shade5 = null;
                                    //orginalRouting.Shade6 = null;
                                    //orginalRouting.Shade7 = null;
                                    //orginalRouting.Shade8 = null;
                                    orginalRouting.ScoreType = null;
                                    OsRoutings.Add(orginalRouting);
                                }

                                if (OsRoutings.Count > 0)
                                {
                                    _routingAPIRepository.SaveRouting(transactionDataModel.modelProductInfo.PLANTCODE, materialNoOsEdit, JsonConvert.SerializeObject(OsRoutings), _token);
                                }

                                #endregion Save routings OS from Original routings
                            }
                            else if (existRoutingOs.Count > 0 && transactionDataModel.modelProductInfo.PLANTCODE != originalFactoryCode)
                            {
                                //delete exist exist routings
                                if (existRoutingOs.Count > 0)
                                {
                                    var plantRouting = existRoutingOs.First();
                                    if (plantRouting.Plant != transactionDataModel.modelProductInfo.PLANTCODE)
                                    {
                                        _routingAPIRepository.DeleteRoutingByMaterialNoAndFactory(plantRouting.Plant, plantRouting.MaterialNo, _token);
                                    }
                                }

                                OsRoutings = new List<Routing>();
                                foreach (var orginalRouting in existRoutingOs)
                                {
                                    orginalRouting.FactoryCode = transactionDataModel.modelProductInfo.PLANTCODE;
                                    orginalRouting.Plant = transactionDataModel.modelProductInfo.PLANTCODE;
                                    orginalRouting.MaterialNo = materialNoOsEdit; ;
                                    orginalRouting.Id = 0;
                                    /// ณัตเพิ่ม clear ค่า Block No, PlateNo, Color จากผู้จ้าง เพราะผู้ผลิตไม่ได้ใช้ แล้วเค้าไม่เห็น ลบเองไม่ได้
                                    orginalRouting.BlockNo = null;
                                    orginalRouting.PlateNo = null;
                                    orginalRouting.MylaNo = null;
                                    orginalRouting.MylaSize = null;
                                    //orginalRouting.Color1 = null;
                                    //orginalRouting.Color2 = null;
                                    //orginalRouting.Color3 = null;
                                    //orginalRouting.Color4 = null;
                                    //orginalRouting.Color5 = null;
                                    //orginalRouting.Color6 = null;
                                    //orginalRouting.Color7 = null;
                                    //orginalRouting.Color8 = null;
                                    //orginalRouting.Shade1 = null;
                                    //orginalRouting.Shade2 = null;
                                    //orginalRouting.Shade3 = null;
                                    //orginalRouting.Shade4 = null;
                                    //orginalRouting.Shade5 = null;
                                    //orginalRouting.Shade6 = null;
                                    //orginalRouting.Shade7 = null;
                                    //orginalRouting.Shade8 = null;
                                    orginalRouting.ScoreType = null;
                                    OsRoutings.Add(orginalRouting);
                                }

                                if (OsRoutings.Count > 0)
                                {
                                    _routingAPIRepository.SaveRouting(originalFactoryCode, materialNoOsEdit, JsonConvert.SerializeObject(OsRoutings), _token);
                                }
                            }

                            #endregion Check and update routing with OS
                        }
                        else
                        {
                            //find outsource from pm2
                            var masterDataPm2OS = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetOutsourceFromMaterialNoAndSaleOrg(_factoryCode, existOriginalMasterdata.MaterialNo, _saleOrg, transactionDataModel.modelProductInfo.PLANTCODE, _token));
                            existMasterDataPm2OS = masterDataPm2OS != null ? true : false;
                            materialNoPm2OS = masterDataPm2OS != null ? masterDataPm2OS.MaterialNo : string.Empty;

                            if (!existMasterDataPm2OS)
                            {
                                //save new master data and transaction detail for outsource

                                #region Save new master data with OS

                                var newOsMasterData = new MasterData();
                                newOsMasterData = JsonConvert.DeserializeObject<MasterData>(jsonExistOriginalMasterdata);
                                newOsMasterData.FactoryCode = transactionDataModel.modelProductInfo.PLANTCODE;
                                newOsMasterData.Plant = transactionDataModel.modelProductInfo.PLANTCODE;
                                newOsMasterData.HireFactory = _factoryCode;
                                newOsMasterData.MaterialNo = materialNoOsEdit;
                                newOsMasterData.Id = 0;
                                newOsMasterData.CreateDate = DateTime.Now;
                                newOsMasterData.CreatedBy = _username;
                                newOsMasterData.LastUpdate = DateTime.Now;
                                newOsMasterData.UpdatedBy = _username;
                                newOsMasterData.SapStatus = false;
                                newOsMasterData.PdisStatus = "N";
                                newOsMasterData.SaleOrg = transactionDataModel.modelProductInfo.OrderTypeId == 4 ? saleOrgOS : _saleOrg;
                                newOsMasterData.TranStatus = isOursource ? true : transactionDataModel.modelProductInfo.OrderTypeId == 4 ? false : true;
                                newOsMasterData.MaterialType = transactionDataModel.modelProductInfo.OrderTypeId == 4 ? "81" : newOsMasterData.MaterialType;
                                newOsMasterData.MatCopy = null;
                                newOsMasterData.FscCode = transactionDataModelSession.modelCategories.fscCode;
                                newOsMasterData.FscFgCode = transactionDataModelSession.modelCategories.fscFgCode;
                                newOsMasterData.RpacLob = transactionDataModelSession.modelCategories.RpacLob;
                                newOsMasterData.RpacProgram = transactionDataModelSession.modelCategories.RpacProgram;
                                newOsMasterData.RpacBrand = transactionDataModelSession.modelCategories.RpacBrand;
                                newOsMasterData.RpacPackagingType = transactionDataModelSession.modelCategories.RpacPackagingType;
                                newOsMasterData.MaterialType = matTypeNewOS;

                                //check mattype to set new SaleText4
                                if (existOriginalMasterdata.MaterialType == "82" && matTypeNewOS == "81")
                                {
                                    newOsMasterData.SaleText4 = newOsMasterData.SaleText4 + " " + existOriginalMasterdata.MaterialNo + " " + existOriginalMasterdata.Pc;
                                }
                                newOsMasterData.SaleText4 = newOsMasterData.SaleText4 != null && newOsMasterData.SaleText4.Length >= 40 ? newOsMasterData.SaleText4.Substring(0, 40) : newOsMasterData.SaleText4;
                                if (!string.IsNullOrEmpty(newOsMasterData?.BoxType))
                                {
                                    //newOsMasterData.SizeDimensions = _newProductService.CalSizeDimensions(newOsMasterData, null);
                                    newOsMasterData.SizeDimensions = JsonConvert.DeserializeObject<string>(_formulaAPIRepository.CalSizeDimensions(_factoryCode, newOsMasterData.MaterialNo, _token));
                                }

                                var parentModelNewOsMasterData = new ParentModel()
                                {
                                    AppName = Globals.AppNameEncrypt,
                                    SaleOrg = _saleOrg,
                                    PlantCode = _factoryCode,
                                    MasterData = newOsMasterData
                                };

                                if (!IsExistMasterData(transactionDataModel.modelProductInfo.PLANTCODE, materialNoOsEdit))
                                {
                                    var jsonNewOsMasterData = JsonConvert.SerializeObject(parentModelNewOsMasterData);
                                    _masterDataAPIRepository.SaveMasterData(jsonNewOsMasterData, _token);

                                    //Create boarduse when doesn't exist.
                                    if (!string.IsNullOrEmpty(existOriginalMasterdata.Code))
                                    {
                                        CheckCreateBoardUse(originalMaterialNo, materialNoOsEdit, transactionDataModel.modelProductInfo.PLANTCODE);
                                    }
                                }

                                #endregion Save new master data with OS

                                //Create new PlantView
                                SavePlantView(existOriginalMasterdata.MaterialNo, transactionDataModel.modelProductInfo, matTypeNewOS, materialNoOsEdit, transactionDataModelSession.modelProductSpec, ref isExistCost);

                                //Create new SaleView
                                SaveSalesView(existOriginalMasterdata.MaterialNo, matTypeNewOS, transactionDataModelSession.modelProductCustomer.CustCode, transactionDataModel.modelProductInfo, materialNoOsEdit);

                                #region Save routing with OS

                                var OsRoutings = new List<Routing>();
                                foreach (var orginalRouting in orginalRoutings)
                                {
                                    orginalRouting.FactoryCode = transactionDataModel.modelProductInfo.PLANTCODE;
                                    orginalRouting.Plant = transactionDataModel.modelProductInfo.PLANTCODE;
                                    orginalRouting.MaterialNo = materialNoOsEdit;
                                    orginalRouting.PdisStatus = "N";
                                    orginalRouting.Id = 0;
                                    /// ณัตเพิ่ม clear ค่า Block No, PlateNo, Color จากผู้จ้าง เพราะผู้ผลิตไม่ได้ใช้ แล้วเค้าไม่เห็น ลบเองไม่ได้
                                    orginalRouting.BlockNo = null;
                                    orginalRouting.PlateNo = null;
                                    orginalRouting.MylaNo = null;
                                    orginalRouting.MylaSize = null;
                                    //orginalRouting.Color1 = null;
                                    //orginalRouting.Color2 = null;
                                    //orginalRouting.Color3 = null;
                                    //orginalRouting.Color4 = null;
                                    //orginalRouting.Color5 = null;
                                    //orginalRouting.Color6 = null;
                                    //orginalRouting.Color7 = null;
                                    //orginalRouting.Color8 = null;
                                    //orginalRouting.Shade1 = null;
                                    //orginalRouting.Shade2 = null;
                                    //orginalRouting.Shade3 = null;
                                    //orginalRouting.Shade4 = null;
                                    //orginalRouting.Shade5 = null;
                                    //orginalRouting.Shade6 = null;
                                    //orginalRouting.Shade7 = null;
                                    //orginalRouting.Shade8 = null;
                                    orginalRouting.ScoreType = null;
                                    OsRoutings.Add(orginalRouting);
                                }

                                if (OsRoutings.Count > 0)
                                {
                                    _routingAPIRepository.SaveRouting(transactionDataModel.modelProductInfo.PLANTCODE, materialNoOsEdit, JsonConvert.SerializeObject(OsRoutings), _token);
                                }

                                #endregion Save routing with OS
                            }

                            #region Get exist transacionDetail and create new for OS

                            var orginalTransactionDetail = transactionDetailObject;
                            var newOsTransactionDetail = new TransactionsDetail();
                            newOsTransactionDetail = transactionDetailObject;
                            newOsTransactionDetail.FactoryCode = transactionDataModel.modelProductInfo.PLANTCODE;
                            newOsTransactionDetail.Outsource = true;
                            newOsTransactionDetail.MaterialNo = string.IsNullOrEmpty(materialNoPm2OS) ? materialNoOsEdit : materialNoPm2OS;
                            newOsTransactionDetail.HireOrderType = transactionDataModel.modelProductInfo.OrderTypeId;
                            newOsTransactionDetail.MatSaleOrg = originalMaterialNo;
                            newOsTransactionDetail.MaxStep = 0;
                            newOsTransactionDetail.Id = 0;
                            newOsTransactionDetail.IdMaterialType = transactionDataModel.modelProductInfo.OrderTypeId == 4 ? 5 : newOsTransactionDetail.IdMaterialType;

                            var parentModelNewOsTransactionsDetail = new ParentModel()
                            {
                                AppName = Globals.AppNameEncrypt,
                                SaleOrg = _saleOrg,
                                PlantCode = _factoryCode,
                                TransactionsDetail = newOsTransactionDetail
                            };

                            if (!IsExistTransactionDetail(transactionDataModel.modelProductInfo.PLANTCODE, newOsTransactionDetail.MaterialNo))
                            {
                                var jsonNewOsTransactionsDetail = JsonConvert.SerializeObject(parentModelNewOsTransactionsDetail);
                                _transactionsDetailAPIRepository.SaveTransactionsDetail(jsonNewOsTransactionsDetail, _token);
                            }

                            #endregion Get exist transacionDetail and create new for OS
                        }
                    }
                    else
                    {
                        //find outsource from pm2
                        var masterDataPm2OS = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetOutsourceFromMaterialNoAndSaleOrg(_factoryCode, existOriginalMasterdata.MaterialNo, _saleOrg, transactionDataModel.modelProductInfo.PLANTCODE, _token));
                        existMasterDataPm2OS = masterDataPm2OS != null ? true : false;
                        materialNoPm2OS = masterDataPm2OS != null ? masterDataPm2OS.MaterialNo : string.Empty;

                        if (!existMasterDataPm2OS)
                        {
                            #region create master data for outsourcing

                            var masterDataObjectForOutsource = new MasterData();
                            masterDataObjectForOutsource = JsonConvert.DeserializeObject<MasterData>(jsonExistOriginalMasterdata);
                            masterDataObjectForOutsource.Id = 0;
                            masterDataObjectForOutsource.MaterialNo = materialNoOsEdit;
                            masterDataObjectForOutsource.Plant = transactionDataModel.modelProductInfo.PLANTCODE;
                            masterDataObjectForOutsource.FactoryCode = transactionDataModel.modelProductInfo.PLANTCODE;
                            masterDataObjectForOutsource.HireFactory = _factoryCode;
                            masterDataObjectForOutsource.SaleOrg = transactionDataModel.modelProductInfo.OrderTypeId == 4 ? transactionDataModel.modelProductInfo.SALEORG : _saleOrg;
                            masterDataObjectForOutsource.MaterialType = transactionDataModel.modelProductInfo.OrderTypeId == 4 ? "81" : matTypeOfSession;
                            masterDataObjectForOutsource.TranStatus = isOursource ? true : transactionDataModel.modelProductInfo.OrderTypeId == 4 ? false : true;
                            masterDataObjectForOutsource.SapStatus = false;
                            masterDataObjectForOutsource.PdisStatus = "N";
                            //JoinType = product.JoinType;
                            masterDataObjectForOutsource.CreateDate = DateTime.Now;
                            masterDataObjectForOutsource.CreatedBy = _username;
                            masterDataObjectForOutsource.LastUpdate = DateTime.Now;
                            masterDataObjectForOutsource.UpdatedBy = _username;
                            masterDataObjectForOutsource.PriorityFlag = transactionDataModelSession.modelProductCustomer.PriorityFlag != null ? Convert.ToString(transactionDataModelSession.modelProductCustomer.PriorityFlag.Value) : null;
                            masterDataObjectForOutsource.MaterialType = matTypeNewOS;

                            //check mattype to set new SaleText4
                            if (existOriginalMasterdata.MaterialType == "82" && matTypeNewOS == "81")
                            {
                                masterDataObjectForOutsource.SaleText4 = masterDataObjectForOutsource.SaleText4 + " " + existOriginalMasterdata.MaterialNo + " " + existOriginalMasterdata.Pc;
                            }
                            masterDataObjectForOutsource.SaleText4 = masterDataObjectForOutsource.SaleText4 != null && masterDataObjectForOutsource.SaleText4.Length >= 40 ? masterDataObjectForOutsource.SaleText4.Substring(0, 40) : masterDataObjectForOutsource.SaleText4;

                            if (transactionDataModel.EventFlag == "Presale")
                            {
                                masterDataObjectForOutsource.SapStatus = false;
                            }

                            if (transactionDataModelSession.TransactionDetail.IsCreateBOM && !transactionDataModelSession.RealEventFlag.Contains("Copy") && !transactionDataModelSession.RealEventFlag.Contains("Presale"))
                            {
                                var tempProduct = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNo(_factoryCode, originalMaterialNo, _token));
                                masterDataObjectForOutsource.Change = tempProduct.Change;
                                masterDataObjectForOutsource.Language = tempProduct.Language;
                                masterDataObjectForOutsource.StatusFlag = tempProduct.StatusFlag;
                                masterDataObjectForOutsource.Bun = tempProduct.Bun;
                                masterDataObjectForOutsource.BunLayer = tempProduct.BunLayer;
                                masterDataObjectForOutsource.LayerPalet = tempProduct.LayerPalet;
                                masterDataObjectForOutsource.BoxPalet = tempProduct.BoxPalet;
                                masterDataObjectForOutsource.SparePercen = tempProduct.SparePercen;
                                masterDataObjectForOutsource.SpareMax = tempProduct.SpareMax;
                                masterDataObjectForOutsource.SpareMin = tempProduct.SpareMin;
                                masterDataObjectForOutsource.LeadTime = tempProduct.LeadTime;
                                masterDataObjectForOutsource.PieceSet = tempProduct.PieceSet;
                                masterDataObjectForOutsource.SaleUom = tempProduct.SaleUom;
                                masterDataObjectForOutsource.BomUom = tempProduct.BomUom;
                                masterDataObjectForOutsource.Hardship = tempProduct.Hardship;
                                masterDataObjectForOutsource.PalletSize = tempProduct.PalletSize;
                                masterDataObjectForOutsource.PalletizationPath = tempProduct.PalletizationPath;
                                masterDataObjectForOutsource.DiecutPictPath = tempProduct.DiecutPictPath;
                                masterDataObjectForOutsource.FgpicPath = tempProduct.FgpicPath;
                                masterDataObjectForOutsource.PrintMasterPath = tempProduct.PrintMasterPath;
                                masterDataObjectForOutsource.PltAxleHeight = tempProduct.PltAxleHeight;
                                masterDataObjectForOutsource.PltBeam = tempProduct.PltBeam;
                                masterDataObjectForOutsource.PltDoubleAxle = tempProduct.PltDoubleAxle;
                                masterDataObjectForOutsource.PltFloorAbove = tempProduct.PltFloorAbove;
                                masterDataObjectForOutsource.PltFloorUnder = tempProduct.PltFloorUnder;
                                masterDataObjectForOutsource.PltLegDouble = tempProduct.PltLegDouble;
                                masterDataObjectForOutsource.PltLegSingle = tempProduct.PltLegSingle;
                                masterDataObjectForOutsource.PltSingleAxle = tempProduct.PltSingleAxle;
                                masterDataObjectForOutsource.EanCode = GetEanCode(masterDataObjectForOutsource.MaterialNo);
                                masterDataObjectForOutsource.NewH = tempProduct.NewH;
                                masterDataObjectForOutsource.PurTxt1 = tempProduct.PurTxt1;
                                masterDataObjectForOutsource.PurTxt2 = tempProduct.PurTxt2;
                                masterDataObjectForOutsource.PurTxt3 = tempProduct.PurTxt3;
                                masterDataObjectForOutsource.PurTxt4 = tempProduct.PurTxt4;
                                masterDataObjectForOutsource.UnUpgradBoard = tempProduct.UnUpgradBoard;
                                masterDataObjectForOutsource.ChangeInfo = tempProduct.ChangeInfo;
                                masterDataObjectForOutsource.PiecePatch = tempProduct.PiecePatch;
                                masterDataObjectForOutsource.BoxHandle = tempProduct.BoxHandle;
                                masterDataObjectForOutsource.PicPallet = tempProduct.PicPallet;
                                masterDataObjectForOutsource.ChangeHistory = tempProduct.ChangeHistory;
                                masterDataObjectForOutsource.Hierarchy = tempProduct.Hierarchy;
                                masterDataObjectForOutsource.WeightBox = null;
                                masterDataObjectForOutsource.PrintMethod = null;
                                masterDataObjectForOutsource.JoinType = null;
                                if (!string.IsNullOrEmpty(masterDataObjectForOutsource?.BoxType))
                                {
                                    //masterDataObjectForOutsource.SizeDimensions = _newProductService.CalSizeDimensions(masterDataObjectForOutsource, null);
                                    masterDataObjectForOutsource.SizeDimensions = JsonConvert.DeserializeObject<string>(_formulaAPIRepository.CalSizeDimensions(_factoryCode, masterDataObjectForOutsource.MaterialNo, _token));
                                }
                            }

                            var parentModelMasterDataForOutsource = new ParentModel()
                            {
                                AppName = Globals.AppNameEncrypt,
                                SaleOrg = _saleOrg,
                                PlantCode = _factoryCode,
                                MasterData = masterDataObjectForOutsource
                            };

                            if (!IsExistMasterData(transactionDataModel.modelProductInfo.PLANTCODE, materialNoOsEdit))
                            {
                                var jsonMasterDataForOutsource = JsonConvert.SerializeObject(parentModelMasterDataForOutsource);
                                _masterDataAPIRepository.SaveMasterData(jsonMasterDataForOutsource, _token);

                                //Create boarduse when doesn't exist.
                                if (!string.IsNullOrEmpty(existOriginalMasterdata.Code))
                                {
                                    CheckCreateBoardUse(originalMaterialNo, materialNoOsEdit, transactionDataModel.modelProductInfo.PLANTCODE);
                                }
                            }

                            #endregion create master data for outsourcing

                            #region Create new SaleView and PlantView

                            SavePlantView(existOriginalMasterdata.MaterialNo, transactionDataModel.modelProductInfo, matTypeNewOS, materialNoOsEdit, transactionDataModelSession.modelProductSpec, ref isExistCost);

                            SaveSalesView(existOriginalMasterdata.MaterialNo, matTypeNewOS, transactionDataModelSession.modelProductCustomer.CustCode, transactionDataModel.modelProductInfo, materialNoOsEdit);

                            #endregion Create new SaleView and PlantView

                            #region Save routing with OS

                            var OsRoutings = new List<Routing>();
                            foreach (var orginalRouting in orginalRoutings)
                            {
                                orginalRouting.FactoryCode = transactionDataModel.modelProductInfo.PLANTCODE;
                                orginalRouting.Plant = transactionDataModel.modelProductInfo.PLANTCODE;
                                orginalRouting.MaterialNo = materialNoOsEdit;
                                orginalRouting.PdisStatus = "N";
                                orginalRouting.Id = 0;
                                /// ณัตเพิ่ม clear ค่า Block No, PlateNo, Color จากผู้จ้าง เพราะผู้ผลิตไม่ได้ใช้ แล้วเค้าไม่เห็น ลบเองไม่ได้
                                orginalRouting.BlockNo = null;
                                orginalRouting.PlateNo = null;
                                orginalRouting.MylaNo = null;
                                orginalRouting.MylaSize = null;
                                //orginalRouting.Color1 = null;
                                //orginalRouting.Color2 = null;
                                //orginalRouting.Color3 = null;
                                //orginalRouting.Color4 = null;
                                //orginalRouting.Color5 = null;
                                //orginalRouting.Color6 = null;
                                //orginalRouting.Color7 = null;
                                //orginalRouting.Color8 = null;
                                //orginalRouting.Shade1 = null;
                                //orginalRouting.Shade2 = null;
                                //orginalRouting.Shade3 = null;
                                //orginalRouting.Shade4 = null;
                                //orginalRouting.Shade5 = null;
                                //orginalRouting.Shade6 = null;
                                //orginalRouting.Shade7 = null;
                                //orginalRouting.Shade8 = null;
                                orginalRouting.ScoreType = null;
                                OsRoutings.Add(orginalRouting);
                            }

                            if (OsRoutings.Count > 0)
                            {
                                _routingAPIRepository.SaveRouting(transactionDataModel.modelProductInfo.PLANTCODE, materialNoOsEdit, JsonConvert.SerializeObject(OsRoutings), _token);
                            }

                            #endregion Save routing with OS
                        }

                        #region create transaction detail for outsourcing

                        var existTranssactionDetail = JsonConvert.DeserializeObject<TransactionsDetail>(_transactionsDetailAPIRepository.GetTransactionsDetailByMat(_factoryCode, transactionDataModelSession.MaterialNo, _token));

                        var transactionDetailModel = new TransactionsDetail
                        {
                            AmountColor = existTranssactionDetail.AmountColor,
                            FactoryCode = transactionDataModel.modelProductInfo.PLANTCODE,
                            Outsource = true,
                            CapImg = existTranssactionDetail.CapImg,
                            Cgtype = existTranssactionDetail.Cgtype,
                            Gltail = existTranssactionDetail.Gltail,
                            Glwid = existTranssactionDetail.Glwid,
                            HierarchyLv4 = existTranssactionDetail.HierarchyLv4,
                            HvaGroup1 = existTranssactionDetail.HvaGroup1,
                            HvaGroup2 = existTranssactionDetail.HvaGroup2,
                            HvaGroup3 = existTranssactionDetail.HvaGroup3,
                            HvaGroup4 = existTranssactionDetail.HvaGroup4,
                            HvaGroup5 = existTranssactionDetail.HvaGroup5,
                            HvaGroup6 = existTranssactionDetail.HvaGroup6,
                            HvaGroup7 = existTranssactionDetail.HvaGroup7,
                            IdKindOfProduct = existTranssactionDetail.IdKindOfProduct,
                            IdKindOfProductGroup = existTranssactionDetail.IdKindOfProductGroup,
                            IdMaterialType = transactionDataModel.modelProductInfo.OrderTypeId == 4 ? 5 : existTranssactionDetail.IdMaterialType,
                            IdProcessCost = existTranssactionDetail.IdProcessCost,
                            IdProductType = existTranssactionDetail.IdProductType,
                            IdProductUnit = existTranssactionDetail.IdProductUnit,
                            IdSaleUnit = existTranssactionDetail.IdSaleUnit,
                            IsNotch = existTranssactionDetail.IsNotch,
                            IsWrap = existTranssactionDetail.IsWrap,
                            MaterialNo = string.IsNullOrEmpty(materialNoPm2OS) ? materialNoOsEdit : materialNoPm2OS,
                            NotchArea = existTranssactionDetail.NotchArea,
                            NotchDegree = existTranssactionDetail.NotchDegree,
                            NotchSide = existTranssactionDetail.NotchSide,
                            PalletOverhang = existTranssactionDetail.PalletOverhang,
                            SideA = existTranssactionDetail.SideA,
                            SideB = existTranssactionDetail.SideB,
                            SideC = existTranssactionDetail.SideC,
                            SideD = existTranssactionDetail.SideD,
                            MatSaleOrg = originalMaterialNo,
                            HireOrderType = transactionDataModel.modelProductInfo.OrderTypeId,
                            MaxStep = 0,
                        };

                        var parentModelTransactionsDetailForOutsource = new ParentModel()
                        {
                            AppName = Globals.AppNameEncrypt,
                            SaleOrg = _saleOrg,
                            PlantCode = _factoryCode,
                            TransactionsDetail = transactionDetailModel
                        };

                        if (!IsExistTransactionDetail(transactionDataModel.modelProductInfo.PLANTCODE, materialNoOsEdit))
                        {
                            var jsonTransactionsDetailForOutsource = JsonConvert.SerializeObject(parentModelTransactionsDetailForOutsource);
                            _transactionsDetailAPIRepository.SaveTransactionsDetail(jsonTransactionsDetailForOutsource, _token);
                        }
                        else
                        {
                            var existPM2TransactionDetail = JsonConvert.DeserializeObject<TransactionsDetail>(_transactionsDetailAPIRepository.GetTransactionsDetailByMat(transactionDataModel.modelProductInfo.PLANTCODE, materialNoOsEdit, _token));

                            existPM2TransactionDetail.AmountColor = existTranssactionDetail.AmountColor;
                            existPM2TransactionDetail.FactoryCode = transactionDataModel.modelProductInfo.PLANTCODE;
                            existPM2TransactionDetail.Outsource = true;
                            existPM2TransactionDetail.CapImg = existTranssactionDetail.CapImg;
                            existPM2TransactionDetail.Cgtype = existTranssactionDetail.Cgtype;
                            existPM2TransactionDetail.Gltail = existTranssactionDetail.Gltail;
                            existPM2TransactionDetail.Glwid = existTranssactionDetail.Glwid;
                            existPM2TransactionDetail.HierarchyLv4 = existTranssactionDetail.HierarchyLv4;
                            existPM2TransactionDetail.HvaGroup1 = existTranssactionDetail.HvaGroup1;
                            existPM2TransactionDetail.HvaGroup2 = existTranssactionDetail.HvaGroup2;
                            existPM2TransactionDetail.HvaGroup3 = existTranssactionDetail.HvaGroup3;
                            existPM2TransactionDetail.HvaGroup4 = existTranssactionDetail.HvaGroup4;
                            existPM2TransactionDetail.HvaGroup5 = existTranssactionDetail.HvaGroup5;
                            existPM2TransactionDetail.HvaGroup6 = existTranssactionDetail.HvaGroup6;
                            existPM2TransactionDetail.HvaGroup7 = existTranssactionDetail.HvaGroup7;
                            existPM2TransactionDetail.IdKindOfProduct = existTranssactionDetail.IdKindOfProduct;
                            existPM2TransactionDetail.IdKindOfProductGroup = existTranssactionDetail.IdKindOfProductGroup;
                            existPM2TransactionDetail.IdMaterialType = transactionDataModel.modelProductInfo.OrderTypeId == 4 ? 5 : existTranssactionDetail.IdMaterialType;
                            existPM2TransactionDetail.IdProcessCost = existTranssactionDetail.IdProcessCost;
                            existPM2TransactionDetail.IdProductType = existTranssactionDetail.IdProductType;
                            existPM2TransactionDetail.IdProductUnit = existTranssactionDetail.IdProductUnit;
                            existPM2TransactionDetail.IdSaleUnit = existTranssactionDetail.IdSaleUnit;
                            existPM2TransactionDetail.IsNotch = existTranssactionDetail.IsNotch;
                            existPM2TransactionDetail.IsWrap = existTranssactionDetail.IsWrap;
                            existPM2TransactionDetail.MaterialNo = string.IsNullOrEmpty(materialNoPm2OS) ? materialNoOsEdit : materialNoPm2OS;
                            existPM2TransactionDetail.NotchArea = existTranssactionDetail.NotchArea;
                            existPM2TransactionDetail.NotchDegree = existTranssactionDetail.NotchDegree;
                            existPM2TransactionDetail.NotchSide = existTranssactionDetail.NotchSide;
                            existPM2TransactionDetail.PalletOverhang = existTranssactionDetail.PalletOverhang;
                            existPM2TransactionDetail.SideA = existTranssactionDetail.SideA;
                            existPM2TransactionDetail.SideB = existTranssactionDetail.SideB;
                            existPM2TransactionDetail.SideC = existTranssactionDetail.SideC;
                            existPM2TransactionDetail.SideD = existTranssactionDetail.SideD;
                            existPM2TransactionDetail.MatSaleOrg = originalMaterialNo;
                            existPM2TransactionDetail.HireOrderType = transactionDataModel.modelProductInfo.OrderTypeId;

                            var parentModel = new ParentModel()
                            {
                                AppName = Globals.AppNameEncrypt,
                                SaleOrg = _saleOrg,
                                PlantCode = _factoryCode,
                                TransactionsDetail = existPM2TransactionDetail
                            };

                            var jsonTransactionsDetailForOutsource = JsonConvert.SerializeObject(parentModel);
                            _transactionsDetailAPIRepository.UpdateTransactionsDetail(jsonTransactionsDetailForOutsource, _token);
                        }

                        #endregion create transaction detail for outsourcing
                    }

                    #region Save New RawMatBom for OS

                    var existRawMatBom = new List<PpcRawMaterialProductionBom>();
                    if (transactionDataModelSession.RealEventFlag.Contains("Copy"))
                    {
                        existRawMatBom = JsonConvert.DeserializeObject<List<PpcRawMaterialProductionBom>>(pPCRawMaterialProductionBomAPIRepository.GetPPCRawMaterialProductionBOMsByFgMaterial(_factoryCode, originalMaterialNo, _token));
                        var copyRawMatBom = new List<PpcRawMaterialProductionBom>();
                        if (existRawMatBom != null)
                        {
                            foreach (var item in existRawMatBom)
                            {
                                var bomModel = new PpcRawMaterialProductionBom
                                {
                                    FgMaterial = materialNoOsEdit,
                                    Plant = transactionDataModel.modelProductInfo.PLANTCODE,
                                    MaterialDescription = !string.IsNullOrEmpty(item.MaterialDescription) ? item.MaterialDescription.Replace("'", "").Replace(";", "") : item.MaterialDescription,
                                    MaterialGroup = item.MaterialGroup,
                                    MaterialNumber = item.MaterialNumber,
                                    MaterialType = item.MaterialType,
                                    NetWeight = item.NetWeight,
                                    BomAmount = item.BomAmount,
                                    CutSize = item.CutSize,
                                    Lay = item.Lay,
                                    Length = item.Length,
                                    Uom = item.Uom,
                                    Width = item.Width,
                                    OldMaterialNumber = item.OldMaterialNumber,
                                    SizeUom = item.SizeUom,
                                    CreateBy = _username,
                                    CreateDate = DateTime.Now,
                                    UpdateBy = _username,
                                    UpdateDate = DateTime.Now,
                                };

                                copyRawMatBom.Add(bomModel);
                            }

                            //save new Raw Bom Mat for new product
                            pPCRawMaterialProductionBomAPIRepository.SaveRawMaterialProductionBoms(_factoryCode, JsonConvert.SerializeObject(copyRawMatBom), _token);
                        }
                    }

                    #endregion Save New RawMatBom for OS

                    transactionDataModelSession.modelProductInfo.MatOursource = orderTypeId == 4 ? materialNoOsEdit : string.Empty;
                    transactionDataModelSession.modelProductInfo.MatTypeOursource = matTypeNewOS;
                }

                #endregion Save New MasterData for Outsource

                #region Save AutoPackingSpec

                autoPackingSpecAPIRepository.SaveAndUpdateAutoPackingSpecFromCusId(_factoryCode, transactionDataModelSession.modelProductCustomer.CusId, _username, materialNo, _token);

                #endregion Save AutoPackingSpec

                #region set new data to session

                transactionDataModelSession.modelProductInfo.SaleText1 = transactionDataModel.modelProductInfo.SaleText1;
                transactionDataModelSession.modelProductInfo.SaleText2 = transactionDataModel.modelProductInfo.SaleText2;
                transactionDataModelSession.modelProductInfo.SaleText3 = transactionDataModel.modelProductInfo.SaleText3;
                transactionDataModelSession.modelProductInfo.SaleText4 = transactionDataModel.modelProductInfo.SaleText4;
                transactionDataModelSession.modelProductInfo.SaleText4 = transactionDataModelSession.modelProductInfo.SaleText4 != null && transactionDataModelSession.modelProductInfo.SaleText4.Length >= 40 ?
                    transactionDataModelSession.modelProductInfo.SaleText4.Substring(0, 40) : transactionDataModelSession.modelProductInfo.SaleText4;
                transactionDataModelSession.modelProductInfo.HighValue = transactionDataModel.modelProductInfo.HighValue;
                transactionDataModelSession.modelProductInfo.HighValueDisplay = transactionDataModel.modelProductInfo.HighValueDisplay;
                transactionDataModelSession.modelProductInfo.Description =
                    !string.IsNullOrEmpty(transactionDataModel.modelProductInfo.Description) ?
                                transactionDataModel.modelProductInfo.Description.Replace("'", "").Replace(";", "").TrimEnd(new Char[] { ' ' }) :
                                transactionDataModel.modelProductInfo.Description.TrimEnd(new Char[] { ' ' });
                transactionDataModelSession.modelProductInfo.PC = transactionDataModel.modelProductInfo.PC;
                transactionDataModelSession.modelProductInfo.PartNo = transactionDataModel.modelProductInfo.PartNo;

                transactionDataModelSession.modelProductInfo.HvaCoating = transactionDataModel.modelProductInfo.HvaCoating;
                transactionDataModelSession.modelProductInfo.HvaCorrugating = transactionDataModel.modelProductInfo.HvaCorrugating;
                if (!string.IsNullOrEmpty(transactionDataModel.modelProductInfo.HvaCorrugating))
                {
                    var hvaMaster = JsonConvert.DeserializeObject<HvaMaster>(_hvaMasterAPIRepository.GetHvaMasterByHighValue(_factoryCode, transactionDataModel.modelProductInfo.HvaCorrugating, _token));
                    transactionDataModelSession.modelProductInfo.HvaCorrugatingDescription = hvaMaster != null ? hvaMaster.HighDescription : null;
                    UpdateRemarkCOROfRouting(transactionDataModelSession.MaterialNo, _factoryCode, ref transactionDataModelSession, transactionDataModelSession.modelProductInfo.HvaCorrugatingDescription);
                }
                transactionDataModelSession.modelProductInfo.HvaFinishing = transactionDataModel.modelProductInfo.HvaFinishing;
                transactionDataModelSession.modelProductInfo.HvaFlute = transactionDataModel.modelProductInfo.HvaFlute;
                transactionDataModelSession.modelProductInfo.HvaStructural = transactionDataModel.modelProductInfo.HvaStructural;
                transactionDataModelSession.modelProductInfo.HvaPrinting = transactionDataModel.modelProductInfo.HvaPrinting;
                transactionDataModelSession.modelProductInfo.HvaProductType = transactionDataModel.modelProductInfo.HvaProductType;

                transactionDataModelSession.modelProductInfo.PLANTCODE = !string.IsNullOrEmpty(transactionDataModel.modelProductInfo.PLANTCODE) ? transactionDataModel.modelProductInfo.PLANTCODE : null;
                transactionDataModelSession.modelProductInfo.OrderTypeId = transactionDataModel.modelProductInfo.OrderTypeId != null ? transactionDataModel.modelProductInfo.OrderTypeId : null;
                transactionDataModelSession.PdisStatus = transactionDataModel.PdisStatus;

                if (transactionDataModel.modelProductERPPurchase == null)
                    transactionDataModel.modelProductERPPurchase = new ProductERPPurchaseViewModel();
                transactionDataModel.modelProductERPPurchase.PurTxt1 = existMasterdata.Pc;
                transactionDataModel.modelProductERPPurchase.PurTxt2 = existMasterdata.SaleText1;
                transactionDataModel.modelProductERPPurchase.PurTxt3 = existMasterdata.SaleText2;
                transactionDataModel.modelProductERPPurchase.PurTxt4 = existMasterdata.SaleText3;

                transactionDataModel = transactionDataModelSession;

                //update max step
                _newProductService.UpdateMaxProgress(_factoryCode, ref transactionDataModelSession);

                SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "TransactionDataModel", transactionDataModelSession);

                #endregion set new data to session

                //#region Update Transaction Session
                //transactionDataModelSession.modelProductInfo.MatTypeOursource = matTypeOs;
                //transactionDataModelSession.TransactionDetail.PCDetail = transactionDataModel.modelProductInfo.PC;
                //transactionDataModelSession.TransactionDetail.MaterialDescriptionDetail = transactionDataModel.modelProductInfo.Description;

                //_newProductService.SetTransactionStatus(ref transactionDataModelSession, "ProductInformation");

                //transactionDataModel = transactionDataModelSession;
            }
            else
            {
                string materialNoOs = string.Empty;
                var transactionDetailObject = JsonConvert.DeserializeObject<TransactionsDetail>(_transactionsDetailAPIRepository.GetTransactionsDetailByMat(_factoryCode, transactionDataModelSession.MaterialNo, _token));
                var isOursource = transactionDetailObject != null && transactionDetailObject.Outsource ? true : false;

                #region Set Material Type

                //if (transactionDataModelSession.TransactionDetail.IsCreateBOM)
                //{
                //    matTypeOfSession = "84";
                //}

                #endregion Set Material Type

                #region Update By Action Copy (CopyX, Copy & Delete)

                if (transactionDataModelSession.RealEventFlag.Equals("CopyX") || transactionDataModelSession.RealEventFlag.Equals("CopyAndDelete"))
                {
                    TransactionDataModel copyTransactionDataModelSession = new TransactionDataModel();
                    copyTransactionDataModelSession = SessionExtentions.GetSession<TransactionDataModel>(_httpContextAccessor.HttpContext.Session, "CopyTransactionDataModel");
                    var changeExistMasterData = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNumberNonX(_factoryCode, copyTransactionDataModelSession.MaterialNo, _token));
                    if (changeExistMasterData != null)
                    {
                        changeExistMasterData.Pc = copyTransactionDataModelSession.modelProductInfo.PC.Length >= 20 ? "x" + copyTransactionDataModelSession.modelProductInfo.PC.Substring(0, 19) : "x" + copyTransactionDataModelSession.modelProductInfo.PC;
                        changeExistMasterData.Description = copyTransactionDataModelSession.modelProductInfo.Description.Length >= 40 ? "x" + copyTransactionDataModelSession.modelProductInfo.Description.Substring(0, 39) : "x" + copyTransactionDataModelSession.modelProductInfo.Description;
                        changeExistMasterData.Description =
                            !string.IsNullOrEmpty(changeExistMasterData.Description) ?
                            changeExistMasterData.Description.Replace("'", "").Replace(";", "").TrimEnd(new Char[] { ' ' }) :
                            changeExistMasterData.Description.TrimEnd(new Char[] { ' ' });
                        changeExistMasterData.UpdatedBy = _username;
                        changeExistMasterData.LastUpdate = DateTime.Now;
                        changeExistMasterData.TranStatus = isOursource ? true : false;
                        changeExistMasterData.User = _username;

                        if (transactionDataModelSession.RealEventFlag.Equals("CopyAndDelete"))
                        {
                            changeExistMasterData.PdisStatus = "X";
                        }
                        if (!string.IsNullOrEmpty(changeExistMasterData?.BoxType))
                        {
                            changeExistMasterData.SizeDimensions = JsonConvert.DeserializeObject<string>(_formulaAPIRepository.CalSizeDimensions(_factoryCode, copyTransactionDataModelSession.MaterialNo, _token));
                        }
                        var changeExistMasterDataParentModel = new ParentModel
                        {
                            PlantCode = _factoryCode,
                            FactoryCode = _factoryCode,
                            SaleOrg = _saleOrg,
                            AppName = Globals.AppNameEncrypt,
                            MasterData = changeExistMasterData
                        };

                        _masterDataAPIRepository.UpdateMasterData(JsonConvert.SerializeObject(changeExistMasterDataParentModel), _token);
                    }
                }

                #endregion Update By Action Copy (CopyX, Copy & Delete)

                #region Check Duplicate Product Code And Description

                if (transactionDataModelSession.TransactionDetail.IsPresaleCreateNewMat)
                {
                    var allowDuplicateDesc = matTypeOfSession == "82" ? true : false;
                    var pcInfo = transactionDataModel.modelProductInfo.PC;
                    var descriptionInfo = transactionDataModel.modelProductInfo.Description;
                    if (allowDuplicateDesc)
                    {
                        descriptionInfo = string.Empty;
                    }

                    var presaleMasterData = new PresaleMasterData();
                    presaleMasterData = SessionExtentions.GetSession<PresaleMasterData>(_httpContextAccessor.HttpContext.Session, "ImportPresaleChangeNewMat");

                    if (string.IsNullOrEmpty(presaleMasterData.MaterialNo))
                    {
                        throw new Exception("Can't create new mat from invalid material number in presale");
                    }

                    //get masterdata
                    var masterData = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNoAndFactory(_factoryCode, presaleMasterData.MaterialNo, _token));

                    if (masterData != null && pcInfo == masterData.Pc)
                    {
                        #region Old Mat change PC and Desc

                        //update pc in masterdata
                        if (masterData.Pc.Length >= 20)
                        {
                            masterData.Pc = masterData.Pc.Substring(0, 19);
                        }
                        masterData.Pc = masterData.Pc.PadLeft(masterData.Pc.Length + 1, 'x');

                        if (masterData.Description.Length >= 40)
                        {
                            masterData.Description = masterData.Description.Substring(0, 39);
                        }
                        masterData.Description = masterData.Description.PadLeft(masterData.Description.Length + 1, 'x');
                        masterData.User = _username;
                        masterData.TranStatus = isOursource ? true : false;
                        if (!string.IsNullOrEmpty(masterData?.BoxType))
                        {
                            masterData.SizeDimensions = JsonConvert.DeserializeObject<string>(_formulaAPIRepository.CalSizeDimensions(_factoryCode, masterData.MaterialNo, _token));
                        }
                        ParentModel masterDataParent = new ParentModel
                        {
                            AppName = Globals.AppNameEncrypt,
                            SaleOrg = _saleOrg,
                            PlantCode = _factoryCode,
                            MasterData = masterData
                        };
                        _masterDataAPIRepository.UpdateMasterData(JsonConvert.SerializeObject(masterDataParent), _token);

                        //update all pc with mark x
                        _masterDataAPIRepository.UpdateProductCodeAndDescriptionFromPresaleNewMat(_factoryCode, pcInfo, descriptionInfo, masterData.MaterialNo, _token);

                        #endregion Old Mat change PC and Desc
                    }
                    else if (masterData != null && pcInfo != masterData.Pc)
                    {
                        #region check duplicate pc and desc

                        if (!transactionDataModelSession.modelCategories.FormGroup.Equals("AC") && matTypeOfSession != "82")
                        {
                            if (!DescriptionCheck(descriptionInfo))
                            {
                                throw new Exception("Duplicated description!");
                            }
                            else if (!ProdCodeCheck(pcInfo))
                            {
                                throw new Exception("Duplicated product code!");
                            }
                        }
                        else
                        {
                            if (!ProdCodeCheck(pcInfo))
                            {
                                throw new Exception("Duplicated product code!");
                            }
                        }

                        #endregion check duplicate pc and desc

                        #region Old Mat change PC and Desc

                        //update pc in masterdata
                        if (masterData.Pc.Length >= 20)
                        {
                            masterData.Pc = masterData.Pc.Substring(0, 19);
                        }
                        masterData.Pc = masterData.Pc.PadLeft(masterData.Pc.Length + 1, 'x');

                        if (masterData.Description.Length >= 40)
                        {
                            masterData.Description = masterData.Description.Substring(0, 39);
                        }
                        masterData.Description = masterData.Description.PadLeft(masterData.Description.Length + 1, 'x');
                        masterData.User = _username;
                        masterData.TranStatus = isOursource ? true : false;
                        if (!string.IsNullOrEmpty(masterData?.BoxType))
                        {
                            masterData.SizeDimensions = JsonConvert.DeserializeObject<string>(_formulaAPIRepository.CalSizeDimensions(_factoryCode, masterData.MaterialNo, _token));
                        }
                        ParentModel masterDataParent = new ParentModel
                        {
                            AppName = Globals.AppNameEncrypt,
                            SaleOrg = _saleOrg,
                            PlantCode = _factoryCode,
                            MasterData = masterData
                        };
                        _masterDataAPIRepository.UpdateMasterData(JsonConvert.SerializeObject(masterDataParent), _token);

                        //update pc mat original
                        _masterDataAPIRepository.UpdateProductCodeAndDescriptionFromPresaleNewMat(_factoryCode, masterData.Pc, masterData.Description, masterData.MaterialNo, _token);

                        #endregion Old Mat change PC and Desc

                        ////update pc mat new with mark x
                        //_masterDataAPIRepository.UpdateProductCodeAndDescriptionFromPresaleNewMat(_factoryCode, pcInfo, descriptionInfo, string.Empty, _token);
                    }
                }
                else
                {
                    if (!transactionDataModelSession.modelCategories.FormGroup.Equals("AC") && matTypeOfSession != "82")
                    {
                        if (!DescriptionCheck(transactionDataModel.modelProductInfo.Description))
                        {
                            throw new Exception("Duplicated description!");
                        }
                        else if (!ProdCodeCheck(transactionDataModel.modelProductInfo.PC))
                        {
                            throw new Exception("Duplicated product code!");
                        }
                    }
                    else
                    {
                        if (!ProdCodeCheck(transactionDataModel.modelProductInfo.PC))
                        {
                            throw new Exception("Duplicated product code!");
                        }
                    }
                }

                #endregion Check Duplicate Product Code And Description

                #region Generate Material No

                if (transactionDataModelSession.EventFlag == "CreateOs")
                {
                    materialNo = transactionDataModelSession.MaterialNo;
                    transactionDataModelSession.PlantCode = _factoryCode;
                    transactionDataModelSession.FactoryCode = _factoryCode;
                    transactionDataModelSession.MaterialNo = materialNo;
                }
                else if (transactionDataModelSession.EventFlag == "CopyOs")
                {
                    //check for genarate new material no
                    var existHireOrder = JsonConvert.DeserializeObject<HireOrder>(_hireOrderAPIRepository.GetHireOrderById(_factoryCode, transactionDataModelSession.TransactionDetail.OrderTypeId.Value, _token));

                    materialNo = transactionDataModelSession.MaterialNo;
                    if (existHireOrder != null)
                    {
                        // Generate Material No
                        if (existHireOrder.SyncMat.HasValue && existHireOrder.SyncMat.Value)
                        {
                            materialNo = transactionDataModelSession.MaterialNo;
                        }
                        else
                        {
                            materialNo = GenMatNo(matTypeOfSession, _factoryCode);
                            // Update RunningNo
                            UpdateRunningNo(matTypeOfSession, _factoryCode, _saleOrg);
                        }
                    }
                    else
                    {
                        materialNo = transactionDataModelSession.MaterialNo;
                    }

                    transactionDataModelSession.PlantCode = transactionDataModelSession.PlantOs;
                    transactionDataModelSession.FactoryCode = _factoryCode;
                    transactionDataModelSession.SaleOrg = transactionDataModelSession.SaleOrg;
                    transactionDataModelSession.MaterialNo = materialNo;
                }
                else
                {
                    //get materialNo for FSC FG Type Code
                    if (!string.IsNullOrEmpty(transactionDataModelSession.modelCategories.fscFgCode))
                    {
                        materialNo = GenFSCCodeMatNo(transactionDataModelSession.modelCategories.fscFgCode);
                        transactionDataModelSession.MaterialNo = materialNo;
                    }
                    else
                    {
                        materialNo = GenMatNo(matTypeOfSession, _factoryCode);
                        transactionDataModelSession.MaterialNo = materialNo;
                        // Update RunningNo
                        UpdateRunningNo(matTypeOfSession, _factoryCode, _saleOrg);
                    }

                    if (!string.IsNullOrEmpty(transactionDataModel.modelProductInfo.PLANTCODE))
                    {
                        var orderTypeId = transactionDataModel.modelProductInfo.OrderTypeId.Value;
                        var hireOrderItem = JsonConvert.DeserializeObject<HireOrder>(_hireOrderAPIRepository.GetHireOrderById(_factoryCode, orderTypeId, _token));
                        // Generate Material No
                        if (hireOrderItem.SyncMat.HasValue && hireOrderItem.SyncMat.Value)
                        {
                            materialNoOs = materialNo;
                            matTypeOs = matTypeOfSession;
                        }
                        else
                        {
                            matTypeOs = orderTypeId == 4 ? "81" : matTypeOfSession;
                            materialNoOs = GenMatNo(matTypeOs, transactionDataModel.modelProductInfo.PLANTCODE);

                            var factoryProfile = JsonConvert.DeserializeObject<CompanyProfile>(_companyProfileAPIRepository.GetCompanyProfileByPlant(transactionDataModel.modelProductInfo.PLANTCODE, _token));
                            // Update RunningNo
                            UpdateRunningNo(matTypeOs, factoryProfile.Plant, factoryProfile.SaleOrg);
                        }
                    }

                    transactionDataModelSession.PlantCode = _factoryCode;
                    transactionDataModelSession.FactoryCode = _factoryCode;
                    transactionDataModelSession.SaleOrg = _saleOrg;
                }

                if (string.IsNullOrEmpty(originalMaterialNo))
                {
                    originalMaterialNo = materialNo;
                }

                #endregion Generate Material No

                //Check for over backward
                isOverBackward = false;

                #region Check Duplicate Material NO

                if (string.IsNullOrEmpty(materialNo))
                {
                    throw new Exception("Can't genarate new material number!");
                }

                //check exist material number
                var existMat = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNumberNonX(_factoryCode, materialNo, _token));
                if (existMat != null)
                {
                    throw new Exception("Your factory has been created material!");
                }

                #endregion Check Duplicate Material NO

                // If Generate Material No Complete

                #region Update and save (MasterData, SaleView, PlantView, TransactionDetail)

                if (!String.IsNullOrEmpty(materialNo)
                    || (transactionDataModelSession.EventFlag == "CreateOs" || transactionDataModelSession.EventFlag == "CopyOs")
                    || !string.IsNullOrEmpty(transactionDataModel.modelProductInfo.PLANTCODE))
                {
                    transactionDataModelSession.modelProductInfo = transactionDataModel.modelProductInfo;
                    //transactionDataModelSession.MaterialNo = materialNo;

                    if ((transactionDataModelSession.EventFlag == "CreateOs" || transactionDataModelSession.EventFlag == "CopyOs"))
                    {
                        transactionDataModel.modelProductInfo.SALEORG = transactionDataModelSession.SaleOrg;
                        if (transactionDataModel.EventFlag == "CopyOs")
                        {
                            SaveMasterData(ref transactionDataModelSession, transactionDataModel.modelProductInfo, originalMaterialNo, "", transactionDataModelSession.PlantOs);
                        }
                        else
                        {
                            SaveMasterData(ref transactionDataModelSession, transactionDataModel.modelProductInfo, originalMaterialNo, "", _factoryCode);
                        }
                    }
                    else if (!string.IsNullOrEmpty(transactionDataModel.modelProductInfo.PLANTCODE))
                    {
                        SaveMasterData(ref transactionDataModelSession, transactionDataModel.modelProductInfo, originalMaterialNo, materialNoOs, _factoryCode);
                    }
                    else
                    {
                        SaveMasterData(ref transactionDataModelSession, transactionDataModel.modelProductInfo, originalMaterialNo, "", _factoryCode);
                    }

                    var hireOrder = new HireOrder();
                    int orderId = 0;

                    #region Save SaleView

                    if (!string.IsNullOrEmpty(transactionDataModel.modelProductInfo.PLANTCODE) && (transactionDataModelSession.EventFlag != "CreateOs" && transactionDataModelSession.EventFlag != "CopyOs"))
                    {
                        orderId = transactionDataModel.modelProductInfo.OrderTypeId.HasValue ? transactionDataModel.modelProductInfo.OrderTypeId.Value : 1;
                        hireOrder = JsonConvert.DeserializeObject<HireOrder>(_hireOrderAPIRepository.GetHireOrderById(_factoryCode, orderId, _token));
                        SaveSalesView(materialNo, matTypeOfSession, transactionDataModelSession.modelProductCustomer.CustCode, transactionDataModel.modelProductInfo, materialNoOs);
                    }
                    else if ((transactionDataModelSession.EventFlag != "CreateOs" && transactionDataModelSession.EventFlag != "CopyOs"))
                    {
                        //normal save saleview
                        SaveSalesView(materialNo, matTypeOfSession, transactionDataModelSession.modelProductCustomer.CustCode, new ProductInfoView(), materialNoOs);
                    }

                    #endregion Save SaleView

                    //Save PlantView
                    SavePlantView(materialNo, transactionDataModel.modelProductInfo, matTypeOfSession, materialNoOs, null, ref isExistCost);

                    #region Add routing and set new session Create BOM

                    if (transactionDataModelSession.TransactionDetail.IsCreateBOM && !transactionDataModelSession.RealEventFlag.Contains("Copy") && !transactionDataModelSession.RealEventFlag.Contains("Presale"))
                    {
                        //get temp routing
                        var routings = JsonConvert.DeserializeObject<List<Routing>>(_routingAPIRepository.GetRoutingsByMaterialNo(_factoryCode, originalMaterialNo, _token));
                        if (routings != null)
                        {
                            var dispatchRoutings = routings.Where(r => r.Machine.Contains("คลัง")).ToList();
                            var bomRoutings = new List<Routing>();

                            foreach (var dispatchRouting in dispatchRoutings)
                            {
                                dispatchRouting.Id = 0;
                                dispatchRouting.Plant = _factoryCode;
                                dispatchRouting.PdisStatus = "C";
                                dispatchRouting.TranStatus = isOursource ? true : false;
                                dispatchRouting.MaterialNo = materialNo;
                                bomRoutings.Add(dispatchRouting);
                            }

                            if (bomRoutings.Count > 0)
                            {
                                _routingAPIRepository.SaveRouting(_factoryCode, materialNo, JsonConvert.SerializeObject(bomRoutings), _token);
                            }
                        }

                        transactionDataModelSession.modelProductSpec = new ProductSpecViewModel();
                    }

                    #endregion Add routing and set new session Create BOM

                    #region Save Routings with Copy

                    //if (transactionDataModelSession.EventFlag.Contains("Copy"))
                    //{
                    //    var routings = JsonConvert.DeserializeObject<List<Routing>>(_routingAPIRepository.GetRoutingsByMaterialNo(_factoryCode, originalMaterialNo));
                    //    var saveRoutings = new List<Routing>();
                    //    foreach (var routing in routings)
                    //    {
                    //        routing.MaterialNo = materialNo;
                    //        routing.Plant = _factoryCode;
                    //        routing.FactoryCode = _factoryCode;
                    //        routing.Id = 0;
                    //        routing.PdisStatus = "C";
                    //        saveRoutings.Add(routing);
                    //    }
                    //    if (saveRoutings.Count > 0)
                    //    {
                    //        _routingAPIRepository.SaveRouting(_factoryCode, materialNo, JsonConvert.SerializeObject(saveRoutings));
                    //    }
                    //}

                    #endregion Save Routings with Copy
                }

                #endregion Update and save (MasterData, SaleView, PlantView, TransactionDetail)

                #region Save AutoPackingSpec

                autoPackingSpecAPIRepository.SaveAndUpdateAutoPackingSpecFromCusId(_factoryCode, transactionDataModelSession.modelProductCustomer.CusId, _username, materialNo, _token);

                #endregion Save AutoPackingSpec

                #region Update Transaction Session

                transactionDataModelSession.modelProductInfo.MatTypeOursource = matTypeOs;
                transactionDataModelSession.TransactionDetail.PCDetail = transactionDataModel.modelProductInfo.PC;
                transactionDataModelSession.TransactionDetail.MaterialDescriptionDetail =
                    !string.IsNullOrEmpty(transactionDataModel.modelProductInfo.Description) ?
                    transactionDataModel.modelProductInfo.Description.Replace("'", "").Replace(";", "") :
                    transactionDataModel.modelProductInfo.Description;
                transactionDataModelSession.MaterialNo = materialNo;

                _newProductService.SetTransactionStatus(ref transactionDataModelSession, "ProductInformation");

                transactionDataModel = transactionDataModelSession;

                transactionDataModelSession.modelCategories.HierarchyLV4 = transactionDataModelSession.modelCategories.HierarchyLV4 == "whitespace" ? "" : transactionDataModelSession.modelCategories.HierarchyLV4;
                transactionDataModelSession.modelCategories.HierarchyLV2 = transactionDataModelSession.modelCategories.HierarchyLV2.Trim();

                transactionDataModel.TransactionDetail.HierarchyDetail = !string.IsNullOrEmpty(transactionDataModel.TransactionDetail.HierarchyDetail) ? transactionDataModel.TransactionDetail.HierarchyDetail : "03" + transactionDataModelSession.modelCategories.HierarchyLV2 + transactionDataModelSession.modelCategories.HierarchyLV3 + transactionDataModelSession.modelCategories.HierarchyLV4;

                if (transactionDataModelSession.EventFlag == "Presale")
                {
                    transactionDataModelSession.RealEventFlag = "Presale";
                    transactionDataModelSession.modelProductSpec.SAPStatus = false;
                }

                transactionDataModelSession.EventFlag = "Create";
                //transactionDataModelSession.TransactionDetail.IsCreateBOM = transactionDataModelSession.TransactionDetail.IsCreateBOM?true:false;

                transactionDataModelSession.modelProductInfo.PLANTCODE = !string.IsNullOrEmpty(transactionDataModel.modelProductInfo.PLANTCODE) ? transactionDataModel.modelProductInfo.PLANTCODE : null;
                transactionDataModelSession.modelProductInfo.OrderTypeId = transactionDataModel.modelProductInfo.OrderTypeId != null ? transactionDataModel.modelProductInfo.OrderTypeId : null;

                transactionDataModel = transactionDataModelSession;

                if (transactionDataModelSession.RealEventFlag.Equals("CopyX") || transactionDataModelSession.RealEventFlag.Equals("CopyAndDelete"))
                {
                    transactionDataModelSession.RealEventFlag = "Copy";
                }

                var routingSession = _routingService.GetRoutingList(materialNo);
                if (routingSession.Count > 0 && transactionDataModelSession.modelRouting != null)
                {
                    transactionDataModelSession.modelRouting.RoutingDataList.Clear();
                    transactionDataModelSession.modelRouting.RoutingDataList = routingSession;
                }

                if (!string.IsNullOrEmpty(transactionDataModel.modelProductInfo.HvaCorrugating))
                {
                    var hvaMaster = JsonConvert.DeserializeObject<HvaMaster>(_hvaMasterAPIRepository.GetHvaMasterByHighValue(_factoryCode, transactionDataModel.modelProductInfo.HvaCorrugating, _token));
                    transactionDataModelSession.modelProductInfo.HvaCorrugatingDescription = hvaMaster != null ? hvaMaster.HighDescription : null;
                    UpdateRemarkCOROfRouting(transactionDataModelSession.MaterialNo, _factoryCode, ref transactionDataModelSession, transactionDataModelSession.modelProductInfo.HvaCorrugatingDescription);
                }

                //update max step
                _newProductService.UpdateMaxProgress(_factoryCode, ref transactionDataModelSession);

                SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "TransactionDataModel", transactionDataModelSession);

                #endregion Update Transaction Session
            }
        }

        public void SaveMasterData(ref TransactionDataModel transactionDataModel, ProductInfoView formModel, string originalMaterialNo, string materialNoOs, string factoryCode)
        {
            TransactionDataModel transactionDataModelSession = new TransactionDataModel();
            transactionDataModelSession = SessionExtentions.GetSession<TransactionDataModel>(_httpContextAccessor.HttpContext.Session, "TransactionDataModel");
            var transactionDetailObject = JsonConvert.DeserializeObject<TransactionsDetail>(_transactionsDetailAPIRepository.GetTransactionsDetailByMat(_factoryCode, transactionDataModelSession.MaterialNo, _token));
            var isOursource = transactionDetailObject != null && transactionDetailObject.Outsource ? true : false;
            var saleUnit = JsonConvert.DeserializeObject<UnitMaterial>(_unitMaterialAPIRepository.GetUnitMaterialById(_factoryCode, transactionDataModel.modelCategories.Id_SU, _token));
            string nameOfSaleUnit = saleUnit == null ? "" : saleUnit.Name;
            string kindOfProductName = JsonConvert.DeserializeObject<KindOfProduct>(_kindOfProductAPIRepository.GetKindOfProductById(_factoryCode, transactionDataModel.modelCategories.Id_kProd, _token)).Name;

            var hierarchyLV3 = string.IsNullOrWhiteSpace(transactionDataModel.modelCategories.HierarchyLV3) ? "" : transactionDataModel.modelCategories.HierarchyLV3;
            var hierarchyLV4 = string.IsNullOrWhiteSpace(transactionDataModel.modelCategories.HierarchyLV4) ? "" : transactionDataModel.modelCategories.HierarchyLV4;
            hierarchyLV4 = transactionDataModel.modelCategories.HierarchyLV4 == "whitespace" ? "" : hierarchyLV4;

            // Save MasterData

            #region Set Master Data Model to Save

            var masterDataObject = new MasterData();

            if (transactionDataModel.RealEventFlag.Contains("Copy"))
            {
                var plantOs = string.Empty;
                if (transactionDataModel.RealEventFlag.Equals("CopyAndDelete"))
                {
                    masterDataObject = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNumberX(factoryCode, originalMaterialNo, _token));
                    masterDataObject.Id = 0;
                    if (masterDataObject == null)
                    {
                        masterDataObject = new MasterData();
                    }
                }
                else
                {
                    masterDataObject = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNumberNonX(factoryCode, originalMaterialNo, _token));
                    masterDataObject.Id = 0;
                    if (masterDataObject == null)
                    {
                        masterDataObject = new MasterData();
                    }
                }

                if (transactionDataModel.RealEventFlag.Equals("CopyOs"))
                {
                    plantOs = _factoryCode;
                }
                else
                {
                    plantOs = formModel.PLANTCODE;
                }

                CopyRouting(transactionDataModel.RealEventFlag, masterDataObject, transactionDataModel.MaterialNo, plantOs);

                masterDataObject.User = _username;
                masterDataObject.MatCopy = originalMaterialNo;
                //save routing
            }
            else if (transactionDataModel.RealEventFlag.Equals("Presale"))
            {
                CopyRoutingFromPresale(transactionDataModelSession, transactionDataModel.MaterialNo, formModel.PLANTCODE);
                masterDataObject = SessionExtentions.GetSession<MasterData>(_httpContextAccessor.HttpContext.Session, "MasterDataFromPreSale");
            }

            masterDataObject.MaterialNo = transactionDataModel.MaterialNo;
            var originalMatNoBom = masterDataObject.MaterialNo.ToString();
            masterDataObject.SaleOrg = _saleOrg;
            masterDataObject.Plant = transactionDataModel.PlantCode;

            if (transactionDataModel.RealEventFlag.Equals("CopyOs") || transactionDataModel.RealEventFlag.Equals("CreateOs"))
            {
                masterDataObject.SaleOrg = transactionDataModel.TransactionDetail.OrderTypeId == 4 && transactionDataModel.RealEventFlag.Equals("CopyOs") ? _saleOrg : transactionDataModel.SaleOrg;
                masterDataObject.Plant = _factoryCode;
            }

            masterDataObject.FactoryCode = _factoryCode;
            masterDataObject.PartNo = transactionDataModel.modelProductInfo.PartNo;
            masterDataObject.Pc = transactionDataModel.modelProductInfo.PC;

            // edit & old mat || plantCode != null (is Os)
            if (transactionDataModelSession.RealEventFlag.Contains("Create") || transactionDataModelSession.RealEventFlag.Equals("Presale") || transactionDataModelSession.RealEventFlag.Contains("Copy") || (transactionDataModelSession.RealEventFlag.Equals("Edit") && transactionDataModelSession.TransactionDetail.IsOldMaterial))
            {
                masterDataObject.Hierarchy = "03" + transactionDataModel.modelCategories.HierarchyLV2.Trim() + hierarchyLV3 + hierarchyLV4;

                if (masterDataObject.Hierarchy.Length == 10 && !String.IsNullOrEmpty(masterDataObject.Code) && transactionDataModelSession.modelCategories.FormGroup != "TCG")
                {
                    masterDataObject.Hierarchy = "03" + transactionDataModel.modelCategories.HierarchyLV2.Trim() + hierarchyLV3 + hierarchyLV4 + masterDataObject.Code.ToString().Trim();
                }
            }
            else
            {
                var originalHierarchy = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNo(_factoryCode, originalMaterialNo, _token)).Hierarchy;
                masterDataObject.Hierarchy = originalHierarchy;
            }

            masterDataObject.CustCode = transactionDataModel.modelProductCustomer.CustCode;
            masterDataObject.CusId = transactionDataModel.modelProductCustomer.CusId;
            masterDataObject.CustName = transactionDataModel.modelProductCustomer.CustName == null ? "" : transactionDataModel.modelProductCustomer.CustName.Length > 50 ? transactionDataModel.modelProductCustomer.CustName.Substring(0, 50) : transactionDataModel.modelProductCustomer.CustName;
            masterDataObject.Description =
                !string.IsNullOrEmpty(transactionDataModel.modelProductInfo.Description) ?
                transactionDataModel.modelProductInfo.Description.Replace("'", "").Replace(";", "").TrimEnd(new Char[] { ' ' }) :
                transactionDataModel.modelProductInfo.Description.TrimEnd(new Char[] { ' ' });
            masterDataObject.SaleText1 = transactionDataModel.modelProductInfo.SaleText1;
            masterDataObject.SaleText2 = transactionDataModel.modelProductInfo.SaleText2;
            masterDataObject.SaleText3 = transactionDataModel.modelProductInfo.SaleText3;
            masterDataObject.SaleText4 = transactionDataModel.modelProductInfo.SaleText4;
            masterDataObject.SaleText4 = masterDataObject.SaleText4 != null && masterDataObject.SaleText4.Length >= 40 ? masterDataObject.SaleText4.Substring(0, 40) : masterDataObject.SaleText4;
            masterDataObject.IndGrp = transactionDataModel.modelProductCustomer.IndDes == null ? null :
                transactionDataModel.modelProductCustomer.IndDes.Length >= 3 ? transactionDataModel.modelProductCustomer.IndDes.Substring(0, 3) : transactionDataModel.modelProductCustomer.IndDes;
            masterDataObject.IndDes = transactionDataModel.modelProductCustomer.IndDes;
            masterDataObject.Language = "TH";
            masterDataObject.MaterialType = transactionDataModel.modelCategories.MatCode;
            masterDataObject.TwoPiece = false;
            masterDataObject.PdisStatus = "N";
            masterDataObject.TranStatus = isOursource ? true : false; ;
            masterDataObject.SapStatus = false;
            masterDataObject.CreateDate = DateTime.Now;
            masterDataObject.CreatedBy = _username;
            masterDataObject.LastUpdate = DateTime.Now;
            masterDataObject.UpdatedBy = _username;
            masterDataObject.HighValue = transactionDataModel.modelProductInfo.HighValue;
            masterDataObject.SaleUom = nameOfSaleUnit;
            masterDataObject.ProType = kindOfProductName;
            masterDataObject.BoxType = transactionDataModel.modelCategories.ProductTypeName;
            masterDataObject.PsmId = transactionDataModel.PsmId;
            masterDataObject.RscStyle = transactionDataModel.modelCategories.FormGroup.ToUpper().Contains("RSC") ? transactionDataModel.modelCategories.RSCStyle : "";//transactionDataModel.modelCategories.RSCStyle;
            masterDataObject.CustComment = transactionDataModel.modelProductCustomer.CustReq;
            masterDataObject.MaterialComment = transactionDataModel.modelProductCustomer.MaterialComment;
            masterDataObject.PriorityFlag = transactionDataModel.modelProductCustomer.PriorityFlag != null ? Convert.ToString(transactionDataModel.modelProductCustomer.PriorityFlag.Value) : null;
            masterDataObject.EanCode = GetEanCode(masterDataObject.MaterialNo);

            // update 15072021
            masterDataObject.TagBundle = transactionDataModel.modelProductCustomer.TagBundle;
            masterDataObject.TagPallet = transactionDataModel.modelProductCustomer.TagPallet;
            masterDataObject.NoTagBundle = transactionDataModel.modelProductCustomer.NoTagBundle;
            masterDataObject.HeadTagBundle = transactionDataModel.modelProductCustomer.HeadTagBundle;
            masterDataObject.FootTagBundle = transactionDataModel.modelProductCustomer.FootTagBundle;
            masterDataObject.HeadTagPallet = transactionDataModel.modelProductCustomer.HeadTagPallet;
            masterDataObject.FootTagPallet = transactionDataModel.modelProductCustomer.FootTagPallet;

            masterDataObject.FscCode = transactionDataModelSession.modelCategories.fscCode;
            masterDataObject.FscFgCode = transactionDataModelSession.modelCategories.fscFgCode;
            masterDataObject.RpacLob = transactionDataModelSession.modelCategories.RpacLob;
            masterDataObject.RpacProgram = transactionDataModelSession.modelCategories.RpacProgram;
            masterDataObject.RpacBrand = transactionDataModelSession.modelCategories.RpacBrand;
            masterDataObject.RpacPackagingType = transactionDataModelSession.modelCategories.RpacPackagingType;

            masterDataObject.Freetext1TagBundle = transactionDataModel.modelProductCustomer.Freetext1TagBundle;
            masterDataObject.Freetext2TagBundle = transactionDataModel.modelProductCustomer.Freetext2TagBundle;
            masterDataObject.Freetext3TagBundle = transactionDataModel.modelProductCustomer.Freetext3TagBundle;

            masterDataObject.Freetext1TagPallet = transactionDataModel.modelProductCustomer.Freetext1TagPallet;
            masterDataObject.Freetext2TagPallet = transactionDataModel.modelProductCustomer.Freetext2TagPallet;
            masterDataObject.Freetext3TagPallet = transactionDataModel.modelProductCustomer.Freetext3TagPallet;
            masterDataObject.HireFactory = _factoryCode;

            if (transactionDataModel.RealEventFlag == "Presale")
            {
                masterDataObject.SapStatus = false;
            }

            if (transactionDataModel.TransactionDetail.IsCreateBOM && !transactionDataModelSession.RealEventFlag.Contains("Copy") && !transactionDataModelSession.RealEventFlag.Contains("Presale"))
            {
                var tempMasterData = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNo(_factoryCode, originalMaterialNo, _token));
                masterDataObject.Change = tempMasterData.Change;
                masterDataObject.Language = tempMasterData.Language;
                masterDataObject.StatusFlag = tempMasterData.StatusFlag;
                masterDataObject.Bun = tempMasterData.Bun;
                masterDataObject.BunLayer = tempMasterData.BunLayer;
                masterDataObject.LayerPalet = tempMasterData.LayerPalet;
                masterDataObject.BoxPalet = tempMasterData.BoxPalet;
                masterDataObject.SparePercen = tempMasterData.SparePercen;
                masterDataObject.SpareMax = tempMasterData.SpareMax;
                masterDataObject.SpareMin = tempMasterData.SpareMin;
                masterDataObject.LeadTime = tempMasterData.LeadTime;
                masterDataObject.PieceSet = tempMasterData.PieceSet;
                masterDataObject.SaleUom = tempMasterData.SaleUom;
                masterDataObject.BomUom = tempMasterData.BomUom;
                masterDataObject.Hardship = tempMasterData.Hardship;
                masterDataObject.PalletSize = tempMasterData.PalletSize;
                masterDataObject.PalletizationPath = tempMasterData.PalletizationPath;
                masterDataObject.DiecutPictPath = tempMasterData.DiecutPictPath;
                masterDataObject.FgpicPath = tempMasterData.FgpicPath;
                masterDataObject.PrintMasterPath = tempMasterData.PrintMasterPath;
                masterDataObject.PltAxleHeight = tempMasterData.PltAxleHeight;
                masterDataObject.PltBeam = tempMasterData.PltBeam;
                masterDataObject.PltDoubleAxle = tempMasterData.PltDoubleAxle;
                masterDataObject.PltFloorAbove = tempMasterData.PltFloorAbove;
                masterDataObject.PltFloorUnder = tempMasterData.PltFloorUnder;
                masterDataObject.PltLegDouble = tempMasterData.PltLegDouble;
                masterDataObject.PltLegSingle = tempMasterData.PltLegSingle;
                masterDataObject.PltSingleAxle = tempMasterData.PltSingleAxle;
                masterDataObject.EanCode = GetEanCode(masterDataObject.MaterialNo);
                masterDataObject.NewH = tempMasterData.NewH;
                masterDataObject.PurTxt1 = tempMasterData.PurTxt1;
                masterDataObject.PurTxt2 = tempMasterData.PurTxt2;
                masterDataObject.PurTxt3 = tempMasterData.PurTxt3;
                masterDataObject.PurTxt4 = tempMasterData.PurTxt4;

                if (masterDataObject.MaterialType == "84")
                {
                    masterDataObject.PurTxt1 = masterDataObject.Pc;
                    masterDataObject.PurTxt2 = masterDataObject.SaleText1;
                    masterDataObject.PurTxt3 = masterDataObject.SaleText2;
                    masterDataObject.PurTxt4 = masterDataObject.SaleText3;
                }

                masterDataObject.UnUpgradBoard = tempMasterData.UnUpgradBoard;
                masterDataObject.ChangeInfo = tempMasterData.ChangeInfo;
                masterDataObject.PiecePatch = tempMasterData.PiecePatch;
                masterDataObject.BoxHandle = tempMasterData.BoxHandle;
                masterDataObject.PicPallet = tempMasterData.PicPallet;
                masterDataObject.ChangeHistory = tempMasterData.ChangeHistory;
                masterDataObject.Hierarchy = tempMasterData.Hierarchy;
                masterDataObject.WeightBox = tempMasterData.WeightBox != null && tempMasterData.WeightBox.HasValue ? tempMasterData.WeightBox.Value : tempMasterData.WeightBox;
                masterDataObject.PrintMethod = transactionDataModelSession.modelProductProp != null ? transactionDataModelSession.modelProductProp.PrintMethod : null;
                masterDataObject.JoinType = transactionDataModelSession.modelProductProp != null ? transactionDataModelSession.modelProductProp.JoinType : null;
            }

            #endregion Set Master Data Model to Save

            #region Save New Master Data

            var parentModelMasterData = new ParentModel()
            {
                AppName = Globals.AppNameEncrypt,
                SaleOrg = _saleOrg,
                PlantCode = _factoryCode,
                MasterData = masterDataObject
            };

            if (!IsExistMasterData(_factoryCode, transactionDataModel.MaterialNo))
            {
                var jsonMasterData = JsonConvert.SerializeObject(parentModelMasterData);
                _masterDataAPIRepository.SaveMasterData(jsonMasterData, _token);

                //set new pdis status to session
                transactionDataModel.PdisStatus = masterDataObject.PdisStatus;
                transactionDataModel.TransactionDetail.HierarchyDetail = masterDataObject.Hierarchy;
                transactionDataModel.SapStatus = masterDataObject.SapStatus;
                transactionDataModel.modelProductInfo.Description =
                    !string.IsNullOrEmpty(masterDataObject.Description) ?
                    masterDataObject.Description.Replace("'", "").Replace(";", "") :
                    masterDataObject.Description;

                if (transactionDataModel.modelProductERPPurchase == null)
                    transactionDataModel.modelProductERPPurchase = new ProductERPPurchaseViewModel();

                transactionDataModel.modelProductERPPurchase.PurTxt1 = masterDataObject.Pc;
                transactionDataModel.modelProductERPPurchase.PurTxt2 = masterDataObject.SaleText1;
                transactionDataModel.modelProductERPPurchase.PurTxt3 = masterDataObject.SaleText2;
                transactionDataModel.modelProductERPPurchase.PurTxt4 = masterDataObject.SaleText3;
            }

            #endregion Save New Master Data

            #region check new product to save autoPackingspec

            if (_factoryCode == "254B")
            {
                var autoPackingCustomer = JsonConvert.DeserializeObject<AutoPackingCustomer>(autoPackingCustomerAPIRepository.GetAutoPackingCustomerByCusId(_factoryCode, masterDataObject.CusId, _token));
                var autoPackingSpec = new AutoPackingSpec();
                if (autoPackingCustomer != null)
                {
                    autoPackingSpec = new AutoPackingSpec
                    {
                        Id = 0,
                        FactoryCode = _factoryCode,
                        MaterialNo = masterDataObject.MaterialNo,
                        CornerGuard = autoPackingCustomer.CornerGuard,
                        CPalletArrange = autoPackingCustomer.CPalletArrange,
                        CPalletStackPos = autoPackingCustomer.CPalletStackPos,
                        CStrapperBottomProtection = autoPackingCustomer.CStrapperBottomProtection,
                        CStrapperTopProtection = autoPackingCustomer.CStrapperTopProtection,
                        CStrapType = autoPackingCustomer.CStrapType,
                        NBottomBoardType = autoPackingCustomer.NBottomBoardType,
                        NPalletType = autoPackingCustomer.NPalletType,
                        NStrapCompression = autoPackingCustomer.NStrapCompression,
                        NTopBoardType = autoPackingCustomer.NTopBoardType,
                        NWrapType = autoPackingCustomer.NWrapType,
                        CreatedBy = _username,
                        CreatedDate = DateTime.Now,
                    };
                }
                else
                {
                    autoPackingSpec = new AutoPackingSpec
                    {
                        Id = 0,
                        FactoryCode = _factoryCode,
                        MaterialNo = masterDataObject.MaterialNo,
                        CornerGuard = "0",
                        CPalletArrange = "11",
                        CPalletStackPos = "05",
                        CStrapperBottomProtection = "00",
                        CStrapperTopProtection = "00",
                        CStrapType = "0",
                        NBottomBoardType = "0",
                        NPalletType = "0",
                        NStrapCompression = null,
                        NTopBoardType = "0",  
                        NWrapType = "6",
                        CreatedBy = _username,
                        CreatedDate = DateTime.Now,
                    };
                }

                autoPackingSpecAPIRepository.SaveAutoPackingSpec(_factoryCode, JsonConvert.SerializeObject(autoPackingSpec), _token);
            }

            #endregion check new product to save autoPackingspec

            #region Save New RawMatBom

            var existRawMatBom = new List<PpcRawMaterialProductionBom>();
            if (transactionDataModelSession.RealEventFlag.Contains("Copy"))
            {
                existRawMatBom = JsonConvert.DeserializeObject<List<PpcRawMaterialProductionBom>>(pPCRawMaterialProductionBomAPIRepository.GetPPCRawMaterialProductionBOMsByFgMaterial(_factoryCode, originalMaterialNo, _token));
                var copyRawMatBom = new List<PpcRawMaterialProductionBom>();
                if (existRawMatBom != null)
                {
                    foreach (var item in existRawMatBom)
                    {
                        var bomModel = new PpcRawMaterialProductionBom
                        {
                            FgMaterial = transactionDataModel.MaterialNo,
                            Plant = _factoryCode,
                            MaterialDescription = !string.IsNullOrEmpty(item.MaterialDescription) ? item.MaterialDescription.Replace("'", "").Replace(";", "") : item.MaterialDescription,
                            MaterialGroup = item.MaterialGroup,
                            MaterialNumber = item.MaterialNumber,
                            MaterialType = item.MaterialType,
                            NetWeight = item.NetWeight,
                            BomAmount = item.BomAmount,
                            CutSize = item.CutSize,
                            Lay = item.Lay,
                            Length = item.Length,
                            Uom = item.Uom,
                            Width = item.Width,
                            OldMaterialNumber = item.OldMaterialNumber,
                            SizeUom = item.SizeUom,
                            CreateBy = _username,
                            CreateDate = DateTime.Now,
                            UpdateBy = _username,
                            UpdateDate = DateTime.Now,
                        };

                        copyRawMatBom.Add(bomModel);
                    }

                    //save new Raw Bom Mat for new product
                    pPCRawMaterialProductionBomAPIRepository.SaveRawMaterialProductionBoms(_factoryCode, JsonConvert.SerializeObject(copyRawMatBom), _token);
                }
            }

            #endregion Save New RawMatBom

            #region Save Quality Spec

            var qualitySpecs = transactionDataModelSession.modelProductCustomer.QualitySpecs;
            if (qualitySpecs != null)
            {
                if (qualitySpecs.Count > 0)
                {
                    var qualitySpecModels = new List<QualitySpec>();
                    var qualitySpecModelsForOutsource = new List<QualitySpec>();

                    foreach (var qualitySpec in qualitySpecs)
                    {
                        decimal result;
                        if (Decimal.TryParse(qualitySpec.Value.ToString(), out result))
                        {
                            //transactionDataModel.MaterialNo
                            var qualitySpecModel = new QualitySpec
                            {
                                FactoryCode = _factoryCode,
                                MaterialNo = transactionDataModel.MaterialNo,
                                Name = qualitySpec.Name,
                                Unit = qualitySpec.Unit,
                                Value = qualitySpec.Value
                            };

                            qualitySpecModels.Add(qualitySpecModel);

                            if (!string.IsNullOrEmpty(formModel.PLANTCODE))
                            {
                                qualitySpecModel.FactoryCode = formModel.PLANTCODE;
                                qualitySpecModelsForOutsource.Add(qualitySpecModel);
                            }
                        }
                    }

                    _qualitySpecAPIRepository.SaveQualitySpec(_factoryCode, JsonConvert.SerializeObject(qualitySpecModels), _token);

                    if (!string.IsNullOrEmpty(formModel.PLANTCODE))
                    {
                        _qualitySpecAPIRepository.SaveQualitySpec(_factoryCode, JsonConvert.SerializeObject(qualitySpecModelsForOutsource), _token);
                    }
                }
            }

            #endregion Save Quality Spec

            // Save TransactionDetail

            #region Set Transaction Detail Model to Save

            var TransactionsDetailObject = new TransactionsDetail
            {
                FactoryCode = _factoryCode,
                MaterialNo = transactionDataModel.MaterialNo,
                Outsource = transactionDataModel.RealEventFlag.Equals("CopyOs") ? true : false,
                IdKindOfProductGroup = transactionDataModel.modelCategories.Id_kProdGrp,
                IdProcessCost = transactionDataModel.modelCategories.Id_ProcCost,
                IdKindOfProduct = transactionDataModel.modelCategories.Id_kProd,
                IdProductType = transactionDataModel.modelCategories.Id_ProdType,
                IdMaterialType = transactionDataModel.modelCategories.Id_MatType,
                IdProductUnit = transactionDataModel.modelCategories.Id_PU,
                IdSaleUnit = transactionDataModel.modelCategories.Id_SU,
                HierarchyLv4 = transactionDataModel.modelCategories.HierarchyLV4 == "whitespace" ? "" : transactionDataModel.modelCategories.HierarchyLV4,
                HvaGroup1 = transactionDataModel.modelProductInfo.HvaProductType,
                HvaGroup2 = transactionDataModel.modelProductInfo.HvaStructural,
                HvaGroup3 = transactionDataModel.modelProductInfo.HvaPrinting,
                HvaGroup4 = transactionDataModel.modelProductInfo.HvaFlute,
                HvaGroup5 = transactionDataModel.modelProductInfo.HvaCorrugating,
                HvaGroup6 = transactionDataModel.modelProductInfo.HvaCoating,
                HvaGroup7 = transactionDataModel.modelProductInfo.HvaFinishing,
                HireOrderType = transactionDataModel.RealEventFlag.Equals("CopyOs") ? transactionDataModel.TransactionDetail.OrderTypeId : null,
                MatSaleOrg = transactionDataModel.RealEventFlag.Equals("CopyOs") ? originalMaterialNo : null,
                NewPrintPlate = transactionDataModel.TransactionDetail.NewPrintPlate,
                OldPrintPlate = transactionDataModel.TransactionDetail.OldPrintPlate,
                NewBlockDieCut = transactionDataModel.TransactionDetail.NewBlockDieCut,
                OldBlockDieCut = transactionDataModel.TransactionDetail.OldBlockDieCut,
                ExampleColor = transactionDataModel.TransactionDetail.ExampleColor,
                CoatingType = transactionDataModel.TransactionDetail.CoatingType,
                CoatingTypeDesc = transactionDataModel.TransactionDetail.CoatingTypeDesc,
                PaperHorizontal = transactionDataModel.TransactionDetail.PaperHorizontal,
                PaperVertical = transactionDataModel.TransactionDetail.PaperVertical,
                FluteHorizontal = transactionDataModel.TransactionDetail.FluteHorizontal,
                FluteVertical = transactionDataModel.TransactionDetail.FluteVertical,
            };

            #endregion Set Transaction Detail Model to Save

            #region Save New Transaction Detail

            var parentModelTransactionsDetail = new ParentModel()
            {
                AppName = Globals.AppNameEncrypt,
                SaleOrg = _saleOrg,
                PlantCode = _factoryCode,
                TransactionsDetail = TransactionsDetailObject
            };

            if (!IsExistTransactionDetail(_factoryCode, transactionDataModel.MaterialNo))
            {
                var jsonTransactionsDetail = JsonConvert.SerializeObject(parentModelTransactionsDetail);
                _transactionsDetailAPIRepository.SaveTransactionsDetail(jsonTransactionsDetail, _token);
            }

            #endregion Save New Transaction Detail

            // Update Presale
            if (transactionDataModel.RealEventFlag == "Presale")
            {
                var psmId = SessionExtentions.GetSession<PresaleViewModel>(_httpContextAccessor.HttpContext.Session, "PresaleViewModel").PSM_ID;
                _presaleService.UpdatePresale(psmId, transactionDataModel.MaterialNo);
            }

            #region Save For Outsource

            if (!string.IsNullOrEmpty(formModel.PLANTCODE))
            {
                var plantOutsource = transactionDataModel.modelProductInfo.PLANTCODE;
                var companyProfile = JsonConvert.DeserializeObject<List<CompanyProfile>>(_companyProfileAPIRepository.GetCompanyProfileList(_factoryCode, _token));
                var groupOfFactory = companyProfile.FirstOrDefault(c => c.Plant == _factoryCode).Group;
                var groupOfOutsource = companyProfile.FirstOrDefault(c => c.Plant == plantOutsource).Group;
                var saleOrgOS = companyProfile.FirstOrDefault(c => c.Plant == plantOutsource).SaleOrg;
                var matType = masterDataObject.MaterialType;
                //if (((groupOfFactory == 1 || groupOfFactory == 2 || groupOfFactory == 3) && groupOfOutsource == 4) || transactionDataModel.modelProductInfo.OrderTypeId == 4)
                //{
                //    masterDataObject.MaterialType = "81";
                //}

                originalMaterialNo = masterDataObject.MaterialNo.ToString();

                #region Save Master Data OutSource

                //save master data for outsource
                masterDataObject.Plant = formModel.PLANTCODE;
                masterDataObject.FactoryCode = formModel.PLANTCODE;
                masterDataObject.TranStatus = isOursource ? true : formModel.OrderTypeId == 4 ? false : true;
                masterDataObject.MaterialNo = materialNoOs;
                masterDataObject.SaleOrg = formModel.OrderTypeId == 4 ? saleOrgOS : masterDataObject.SaleOrg;
                masterDataObject.HireFactory = _factoryCode;
                masterDataObject.MaterialType = formModel.OrderTypeId == 4 ? "81" : masterDataObject.MaterialType;
                masterDataObject.SapStatus = false;
                masterDataObject.User = _username;
                masterDataObject.FscCode = transactionDataModelSession.modelCategories.fscCode;
                masterDataObject.FscFgCode = transactionDataModelSession.modelCategories.fscFgCode;
                masterDataObject.RpacLob = transactionDataModelSession.modelCategories.RpacLob;
                masterDataObject.RpacProgram = transactionDataModelSession.modelCategories.RpacProgram;
                masterDataObject.RpacBrand = transactionDataModelSession.modelCategories.RpacBrand;
                masterDataObject.RpacPackagingType = transactionDataModelSession.modelCategories.RpacPackagingType;

                //check mattype to set new SaleText4
                if (matType == "82" && masterDataObject.MaterialType == "81")
                {
                    masterDataObject.SaleText4 = masterDataObject.SaleText4 + " " + originalMaterialNo + " " + masterDataObject.Pc;
                }

                masterDataObject.SaleText4 = masterDataObject.SaleText4 != null && masterDataObject.SaleText4.Length >= 40 ? masterDataObject.SaleText4.Substring(0, 40) : masterDataObject.SaleText4;

                var parentModelOutsourceMasterData = new ParentModel()
                {
                    AppName = Globals.AppNameEncrypt,
                    SaleOrg = _saleOrg,
                    PlantCode = formModel.PLANTCODE,
                    MasterData = masterDataObject
                };

                if (!IsExistMasterData(formModel.PLANTCODE, materialNoOs))
                {
                    var jsonOutsourceMasterData = JsonConvert.SerializeObject(parentModelMasterData);
                    _masterDataAPIRepository.SaveMasterData(jsonOutsourceMasterData, _token);

                    //Create boarduse when doesn't exist.
                    if (!string.IsNullOrEmpty(masterDataObject.Code))
                    {
                        CheckCreateBoardUse(originalMaterialNo, materialNoOs, transactionDataModel.modelProductInfo.PLANTCODE);
                    }
                }

                #endregion Save Master Data OutSource

                #region Save Transaction Detail Outsource

                //save transaction detail for outsource
                TransactionsDetailObject.Outsource = true;
                TransactionsDetailObject.FactoryCode = formModel.PLANTCODE;
                TransactionsDetailObject.MaterialNo = materialNoOs;
                TransactionsDetailObject.HireOrderType = formModel.OrderTypeId;
                TransactionsDetailObject.MaxStep = 0;
                TransactionsDetailObject.IdMaterialType = transactionDataModel.modelProductInfo.OrderTypeId == 4 ? 5 : TransactionsDetailObject.IdMaterialType;
                if (transactionDataModel.TransactionDetail.IsCreateBOM && !transactionDataModelSession.RealEventFlag.Contains("Copy") && !transactionDataModelSession.RealEventFlag.Contains("Presale"))
                {
                    TransactionsDetailObject.MatSaleOrg = originalMatNoBom;
                }
                else
                {
                    TransactionsDetailObject.MatSaleOrg = originalMaterialNo;
                }

                var parentModelTransactionsDetailForOutsource = new ParentModel()
                {
                    AppName = Globals.AppNameEncrypt,
                    SaleOrg = _saleOrg,
                    PlantCode = formModel.PLANTCODE,
                    TransactionsDetail = TransactionsDetailObject
                };

                if (!IsExistTransactionDetail(formModel.PLANTCODE, materialNoOs))
                {
                    var jsonTransactionsDetailForOutsource = JsonConvert.SerializeObject(parentModelTransactionsDetailForOutsource);
                    _transactionsDetailAPIRepository.SaveTransactionsDetail(jsonTransactionsDetailForOutsource, _token);
                }

                #endregion Save Transaction Detail Outsource

                #region Create Qualiry Spec for OS

                var qaSpecsOs = JsonConvert.DeserializeObject<List<QualitySpec>>(_qualitySpecAPIRepository.GetQualitySpecByMaterialNo(_factoryCode, transactionDataModel.MaterialNo, _token));
                var qaSpecList = new List<QualitySpec>();
                foreach (var qaSpecOs in qaSpecsOs)
                {
                    qaSpecOs.MaterialNo = materialNoOs;
                    qaSpecOs.FactoryCode = formModel.PLANTCODE;
                    qaSpecList.Add(qaSpecOs);
                }

                if (qaSpecList.Count > 0)
                {
                    var jsonQaSpecOs = JsonConvert.SerializeObject(qaSpecList);
                    _qualitySpecAPIRepository.SaveQualitySpec(formModel.PLANTCODE, jsonQaSpecOs, _token);
                }

                #endregion Create Qualiry Spec for OS

                #region Save New RawMatBom

                if (transactionDataModelSession.RealEventFlag.Contains("Copy"))
                {
                    var copyRawMatBom = new List<PpcRawMaterialProductionBom>();
                    if (existRawMatBom != null)
                    {
                        foreach (var item in existRawMatBom)
                        {
                            var bomModel = new PpcRawMaterialProductionBom
                            {
                                FgMaterial = materialNoOs,
                                Plant = formModel.PLANTCODE,
                                MaterialDescription = !string.IsNullOrEmpty(item.MaterialDescription) ? item.MaterialDescription.Replace("'", "").Replace(";", "") : item.MaterialDescription,
                                MaterialGroup = item.MaterialGroup,
                                MaterialNumber = item.MaterialNumber,
                                MaterialType = item.MaterialType,
                                NetWeight = item.NetWeight,
                                BomAmount = item.BomAmount,
                                CutSize = item.CutSize,
                                Lay = item.Lay,
                                Length = item.Length,
                                Uom = item.Uom,
                                Width = item.Width,
                                OldMaterialNumber = item.OldMaterialNumber,
                                SizeUom = item.SizeUom,
                                CreateBy = _username,
                                CreateDate = DateTime.Now,
                                UpdateBy = _username,
                                UpdateDate = DateTime.Now,
                            };

                            copyRawMatBom.Add(bomModel);
                        }

                        //save new Raw Bom Mat for new product
                        pPCRawMaterialProductionBomAPIRepository.SaveRawMaterialProductionBoms(_factoryCode, JsonConvert.SerializeObject(copyRawMatBom), _token);
                    }
                }

                #endregion Save New RawMatBom

                transactionDataModel.modelProductInfo.MatOursource = formModel.OrderTypeId.Value == 4 ? materialNoOs : string.Empty;
                transactionDataModel.modelProductInfo.PLANTCODE = formModel.PLANTCODE;
                transactionDataModel.modelProductInfo.OrderTypeId = formModel.OrderTypeId.Value != null ? formModel.OrderTypeId : null;
            }

            #endregion Save For Outsource
        }

        public string GenMatNo(string matCode, string factoryCode)
        {
            string materialTypeGroupId = JsonConvert.DeserializeObject<MaterialType>(_materialTypeAPIRepository.GetMaterialTypeByMaterialCode(matCode, _token)).GroupId;

            var Running = JsonConvert.DeserializeObject<RunningNo>(_runningNoAPIRepository.GetRunningNoByGroupId(factoryCode, materialTypeGroupId, _token));

            try
            {
                if (Running == null)
                {
                    var ex = new ArgumentNullException($"Running No does not exist.");
                    throw ex;
                }
                if (Running.Running >= Running.EndNo)
                {
                    var ex = new ArgumentOutOfRangeException(nameof(Running), $"Limited Running No. ,Please contact admin to correct.");
                    throw ex;
                }

                int mat_no;
                string mat_str, Material_No;

                mat_no = Running.Running + 1;
                mat_str = Convert.ToString(mat_no);
                mat_str = mat_str.PadLeft(Running.Length, '0');
                Material_No = Running.Fix + mat_str;

                return Material_No;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public string GenFSCCodeMatNo(string fscFgCode)
        {
            var fscFgCodeData = JsonConvert.DeserializeObject<PpcFscFgCode>(fscFGCodeAPIRepository.GetFSCFGCodeByFSCFGCode(_factoryCode, fscFgCode, _token));

            try
            {
                if (fscFgCodeData == null)
                {
                    var ex = new ArgumentNullException($"FSC FG Type Code Running No does not exist.");
                    throw ex;
                }

                //if (fscFgCodeData.Running >= fscFgCodeData.EndNo)
                //{
                //    var ex = new ArgumentOutOfRangeException(nameof(fscFgCodeData), $"Limited Running No. ,Please contact admin to correct.");
                //    throw ex;
                //}

                int mat_no;
                string mat_str, Material_No;

                mat_no = fscFgCodeData.Running.Value + 1;
                mat_str = Convert.ToString(mat_no);
                mat_str = mat_str.PadLeft(fscFgCodeData.Length.Value, '0');
                Material_No = fscFgCodeData.FscFgPrefix + mat_str;

                // Update RunningNo FSC FG Type Code
                var Running = 0;

                if (fscFgCodeData != null)
                {
                    fscFgCodeData.Running = fscFgCodeData.Running + 1;

                    // Update RunningNo
                    fscFGCodeAPIRepository.UpdateFSCFGCode(_factoryCode, JsonConvert.SerializeObject(fscFgCodeData), _token);
                }

                return Material_No;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public bool DescriptionCheck(string description)
        {
            var masterData = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByDescription(_factoryCode, description, _token));

            if (masterData == null)
            {
                return true; // Not Duplicate
            }
            else
            {
                return false; // Duplicate
            }
        }

        public bool ProdCodeCheck(string prodCode)
        {
            var masterData = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByProdCode(_factoryCode, prodCode, _token));

            if (masterData == null)
            {
                return true; // Not Duplicate
            }
            else
            {
                return false; // Duplicate
            }
        }

        private void UpdateRunningNo(string matType, string fatoryCode, string saleOrg)
        {
            var Running = 0;

            //var companyProfiles = new List<CompanyProfile>();
            //companyProfiles = JsonConvert.DeserializeObject<List<CompanyProfile>>(_companyProfileAPIRepository.GetCompanyProfileBySaleOrg(fatoryCode, saleOrg, _token));
            var companyProfile = new CompanyProfile();
            companyProfile = JsonConvert.DeserializeObject<CompanyProfile>(_companyProfileAPIRepository.GetCompanyProfileByPlant(fatoryCode, _token));

            // Get MaterialType GroupId
            string materialTypeGroupId = JsonConvert.DeserializeObject<MaterialType>(_materialTypeAPIRepository.GetMaterialTypeByMaterialCode(matType, _token)).GroupId;
            // Get Original RunningNo
            var runningNoOriginal = JsonConvert.DeserializeObject<RunningNo>(_runningNoAPIRepository.GetRunningNoByGroupId(fatoryCode, materialTypeGroupId, _token));

            if (runningNoOriginal != null)
            {
                Running = runningNoOriginal.Running + 1;
            }

            //foreach (var companyProfile in companyProfiles)
            //{
            // Get RunningNo
            var runningNoObject = JsonConvert.DeserializeObject<RunningNo>(_runningNoAPIRepository.GetRunningNoByGroupId(companyProfile.Plant, materialTypeGroupId, _token));

            // Running Number
            runningNoObject.Running = Running;
            runningNoObject.UpdatedBy = _username;
            runningNoObject.UpdatedDate = DateTime.Now;

            // Update RunningNo
            ParentModel parentModelRunningNo = new ParentModel
            {
                AppName = Globals.AppNameEncrypt,
                SaleOrg = saleOrg,
                PlantCode = companyProfile.Plant,
                RunningNo = runningNoObject
            };

            // Update RunningNo
            _runningNoAPIRepository.UpdateRunningNo(JsonConvert.SerializeObject(parentModelRunningNo), _token);
            //}
        }

        private void SaveSalesView(string materialNo, string matCode, string custCode, ProductInfoView formModel, string materialNoOS)
        {
            var orderType = "";
            var orderTypeId = formModel.OrderTypeId;
            int[] channels = new int[] { };
            bool isExistSaleView = false;
            var pdisstatus = string.Empty;
            var transactionDetailObject = JsonConvert.DeserializeObject<TransactionsDetail>(_transactionsDetailAPIRepository.GetTransactionsDetailByMat(_factoryCode, materialNoOS, _token));
            var isOursource = transactionDetailObject != null && transactionDetailObject.Outsource ? true : false;

            if (formModel.OrderTypeId != null)
            {
                switch (formModel.OrderTypeId)
                {
                    case 1:
                        channels = new int[] { 10, 60 };
                        break;

                    case 2:
                        channels = new int[] { 10, 60 };
                        break;

                    case 3:
                        //channels = new int[] {60 };
                        channels = new int[] { 10, 60 };
                        break;

                    case 4:
                        channels = new int[] { 10 };
                        break;

                    default:
                        break;
                }
            }

            string pattern = @"[\d][4]";
            Regex rgx = new Regex(pattern);
            var match = rgx.Match(matCode);
            if (match.Success)
            {
                orderType = "LUMF";
            }
            else
            {
                orderType = matCode == "82" ? "BANC" : "ZMTO";
            }

            SalesView salesViewObject;
            salesViewObject = new SalesView()
            {
                MaterialNo = materialNo,
                FactoryCode = _factoryCode,
                SaleOrg = _saleOrg,
                Channel = 10,
                DevPlant = _factoryCode,
                CustCode = custCode,
                MinQty = 0,
                OrderType = orderType,
                PdisStatus = "N",
                TranStatus = isOursource ? true : false,
                SapStatus = false,
            };

            ParentModel parentModelSalesView = new ParentModel()
            {
                AppName = Globals.AppNameEncrypt,
                SaleOrg = _saleOrg,
                PlantCode = _factoryCode,
                SalesView = salesViewObject
            };

            if (!IsExistSaleView(_factoryCode, materialNo, _saleOrg, 10))
            {
                string parentModelSalesViewJsonString = JsonConvert.SerializeObject(parentModelSalesView);
                _salesViewAPIRepository.SaveSaleView(parentModelSalesViewJsonString, _token);
                isExistSaleView = true;
                pdisstatus = salesViewObject.PdisStatus;
            }
            else
            {
                isExistSaleView = true;
                pdisstatus = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNumberNonX(_factoryCode, materialNo, _token)).PdisStatus;
            }

            foreach (var channel in channels)
            {
                var devPlant = formModel.PLANTCODE;
                var companyProfileOs = JsonConvert.DeserializeObject<CompanyProfile>(_companyProfileAPIRepository.GetCompanyProfileByPlant(devPlant, _token));
                salesViewObject = new SalesView()
                {
                    MaterialNo = materialNoOS,
                    FactoryCode = devPlant,
                    SaleOrg = formModel.SALEORG,
                    Channel = channel,
                    DevPlant = devPlant,
                    CustCode = custCode,
                    MinQty = 0,
                    OrderType = orderTypeId == 4 ? "ZMTO" : orderType,
                    TranStatus = isOursource ? true : orderTypeId == 4 ? false : true,
                    SapStatus = false,
                };

                salesViewObject.PdisStatus = "N";

                ParentModel parentModelSalesViewOs = new ParentModel()
                {
                    AppName = Globals.AppNameEncrypt,
                    SaleOrg = _saleOrg,
                    PlantCode = _factoryCode,
                    SalesView = salesViewObject
                };

                if (!IsExistSaleView(devPlant, materialNoOS, _saleOrg, channel))
                {
                    string parentModelSalesViewOsJsonString = JsonConvert.SerializeObject(parentModelSalesViewOs);
                    _salesViewAPIRepository.SaveSaleView(parentModelSalesViewOsJsonString, _token);
                }
            }
        }

        private void UpdateSalesView(string materialNo, MasterData masterData)
        {
            var saleviews = JsonConvert.DeserializeObject<List<SalesView>>(_salesViewAPIRepository.GetSaleViewsByMaterialNoAndFactoryCode(_factoryCode, materialNo, _token));

            string ComCode = string.Empty, Factory = string.Empty;
            string WHStatus = string.Empty;
            string PDIS_Status = string.Empty;
            bool Tran_Status = false, SAP_Status = false;

            ComCode = JsonConvert.DeserializeObject<PmtsConfig>(_pmtsConfigAPIRepository.GetPMTsConfigByFactoryName(_factoryCode, "ComCode", _token)).FucValue;
            Factory = JsonConvert.DeserializeObject<PmtsConfig>(_pmtsConfigAPIRepository.GetPMTsConfigByFactoryName(_factoryCode, "Factory", _token)).FucValue;

            foreach (var saleview in saleviews)
            {
                #region Update Orginal SaleView

                if (ComCode != Factory)
                {
                    saleview.PdisStatus = "C";
                    saleview.TranStatus = true;
                    saleview.SapStatus = true;
                }
                else
                {
                    //if (saleview.PdisStatus != "X" && saleview.SapStatus == false)
                    //{
                    //    saleview.PdisStatus = WHStatus;
                    //    saleview.TranStatus = true;
                    //    saleview.SapStatus = false;
                    //}
                    //else if (saleview.TranStatus == null || saleview.SapStatus == null)
                    //{
                    //    saleview.PdisStatus = "C";
                    //    saleview.TranStatus = false;
                    //    saleview.SapStatus = false;
                    //}
                    //else
                    //{
                    //    saleview.PdisStatus = "M";
                    //    saleview.TranStatus = false;
                    //    saleview.SapStatus = true;
                    //}

                    saleview.PdisStatus = masterData.PdisStatus;
                    saleview.TranStatus = masterData.TranStatus;
                    saleview.SapStatus = masterData.SapStatus;
                }

                ParentModel parentModelSalesView = new ParentModel()
                {
                    AppName = Globals.AppNameEncrypt,
                    SaleOrg = _saleOrg,
                    PlantCode = _factoryCode,
                    SalesView = saleview
                };

                string parentModelSalesViewJsonString = JsonConvert.SerializeObject(parentModelSalesView);
                _salesViewAPIRepository.UpdateSaleView(parentModelSalesViewJsonString, _token);

                #endregion Update Orginal SaleView
            }
        }

        private void UpdateSalesViewOs(string oldMaterialNo, ProductInfoView formModel, string newMaterialNo)
        {
            var saleviews = JsonConvert.DeserializeObject<List<SalesView>>(_salesViewAPIRepository.GetSaleViewsByMaterialNo(_factoryCode, oldMaterialNo, _token));
            var transactionDetailObject = JsonConvert.DeserializeObject<TransactionsDetail>(_transactionsDetailAPIRepository.GetTransactionsDetailByMat(_factoryCode, newMaterialNo, _token));
            var isOursource = transactionDetailObject != null && transactionDetailObject.Outsource ? true : false;
            var orderTypeId = formModel.OrderTypeId;
            int[] channels = new int[] { };

            if (formModel.OrderTypeId != null)
            {
                switch (formModel.OrderTypeId)
                {
                    case 1:
                        channels = new int[] { 10, 60 };
                        break;

                    case 2:
                        channels = new int[] { 10, 60 };
                        break;

                    case 3:
                        channels = new int[] { 10, 60 };
                        break;

                    case 4:
                        channels = new int[] { 10 };
                        break;

                    default:
                        break;
                }
            }

            foreach (var saleview in saleviews)
            {
                #region Update Outsource SaleView

                foreach (var channel in channels)
                {
                    var devPlant = formModel.PLANTCODE;
                    var saleViewModel = new SalesView
                    {
                        FactoryCode = devPlant,
                        MaterialNo = newMaterialNo,
                        SaleOrg = formModel.SALEORG,
                        ChangeDate = saleview.ChangeDate,
                        Channel = channel,
                        CustCode = saleview.CustCode,
                        DevPlant = devPlant,
                        Effective = saleview.Effective,
                        MinQty = saleview.MinQty,
                        NewPrice = saleview.NewPrice,
                        OldPrice = saleview.OldPrice,
                        OrderType = orderTypeId == 4 ? "ZMTO" : saleview.OrderType,
                        PdisStatus = "N",
                        PriceAdj = saleview.PriceAdj,
                        SaleUnitPrice = saleview.SaleUnitPrice,
                        SapStatus = saleview.SapStatus,
                        TranStatus = isOursource ? true : orderTypeId == 4 ? false : saleview.TranStatus
                    };

                    ParentModel parentCreateSalesView = new ParentModel()
                    {
                        AppName = Globals.AppNameEncrypt,
                        SaleOrg = _saleOrg,
                        PlantCode = _factoryCode,
                        SalesView = saleViewModel
                    };

                    if (!IsExistSaleView(devPlant, oldMaterialNo, saleViewModel.SaleOrg, channel))
                    {
                        string parentCreateSalesViewJsonString = JsonConvert.SerializeObject(parentCreateSalesView);
                        _salesViewAPIRepository.SaveSaleView(parentCreateSalesViewJsonString, _token);
                    }
                }

                #endregion Update Outsource SaleView
            }
        }

        private void SavePlantView(string materialNo, ProductInfoView productInfoView, string matCode, string materialNoOs, ProductSpecViewModel modelProductSpec, ref bool isExistCost)
        {
            var facoryCode = string.Empty;
            var transactionDetailObject = JsonConvert.DeserializeObject<TransactionsDetail>(_transactionsDetailAPIRepository.GetTransactionsDetailByMat(_factoryCode, materialNo, _token));
            var isOursource = transactionDetailObject != null && transactionDetailObject.Outsource ? true : false;
            var existPlantView = new PlantView();
            var plantViewObject = new PlantView()
            {
                MaterialNo = materialNo,
                FactoryCode = _factoryCode,
                Plant = _factoryCode,
                PurchCode = JsonConvert.DeserializeObject<CompanyProfile>(_companyProfileAPIRepository.GetCompanyProfileByPlant(_factoryCode, _token)).PurchasGrp,
                StdTotalCost = 0,
                StdMovingCost = 0,
                StdFc = 0,
                StdVc = 0,
                PdisStatus = "N",
                TranStatus = isOursource ? true : false,
                SapStatus = false
            };

            ParentModel parentModelPlantView = new ParentModel()
            {
                AppName = Globals.AppNameEncrypt,
                SaleOrg = _saleOrg,
                PlantCode = _factoryCode,
                PlantView = plantViewObject
            };

            if (!IsExistPlantView(_factoryCode, materialNo))
            {
                existPlantView = parentModelPlantView.PlantView;
                string parentModelPlantViewJsonString = JsonConvert.SerializeObject(parentModelPlantView);
                _plantViewAPIRepository.SavePlantView(parentModelPlantViewJsonString, _token);
            }
            else
            {
                existPlantView = JsonConvert.DeserializeObject<PlantView>(_plantViewAPIRepository.GetPlantViewByMaterialNoAndPlant(_factoryCode, materialNo, _factoryCode, _token));
            }

            if (!string.IsNullOrEmpty(productInfoView.PLANTCODE))
            {
                plantViewObject.FactoryCode = productInfoView.PLANTCODE;
                plantViewObject.Plant = productInfoView.PLANTCODE;
                plantViewObject.MaterialNo = materialNoOs;
                plantViewObject.PdisStatus = "N";
                plantViewObject.TranStatus = true;
                plantViewObject.SapStatus = false;
                plantViewObject.PurchCode = JsonConvert.DeserializeObject<CompanyProfile>(_companyProfileAPIRepository.GetCompanyProfileByPlant(productInfoView.PLANTCODE, _token)).PurchasGrp;
                plantViewObject.EffectiveDate = existPlantView.EffectiveDate;
                plantViewObject.ShipBlk = existPlantView.ShipBlk;
                plantViewObject.StdFc = existPlantView.StdFc;
                plantViewObject.StdVc = existPlantView.StdVc;

                if (modelProductSpec != null && !string.IsNullOrEmpty(modelProductSpec.Code) && modelProductSpec.CostPerTon != null && modelProductSpec.CostPerTon != 0)
                {
                    var cost = JsonConvert.DeserializeObject<Cost>(_boardCombineAccAPIRepository.GetCost(productInfoView.PLANTCODE, modelProductSpec.Code, modelProductSpec.costField, _token));
                    if (cost == null || cost.CostPerTon == 0)
                    {
                        plantViewObject.PdisStatus = "N";
                        plantViewObject.StdTotalCost = 0;
                        isExistCost = false;
                    }
                    else
                    {
                        if (matCode == "82")
                        {
                            plantViewObject.StdTotalCost = 0;
                            plantViewObject.StdMovingCost = cost.CostPerTon.HasValue ? Math.Round(cost.CostPerTon.Value, 2) : 0;
                        }
                        else
                        {
                            plantViewObject.StdTotalCost = cost.CostPerTon.HasValue ? Math.Round(cost.CostPerTon.Value, 2) : 0;
                            plantViewObject.StdMovingCost = 0;
                        }

                        plantViewObject.PdisStatus = "C";
                    }
                }
                else
                {
                    plantViewObject.PdisStatus = "N";
                    plantViewObject.StdTotalCost = 0;
                    if (modelProductSpec != null)
                    {
                        isExistCost = false;
                    }
                }

                ParentModel parentModelPlantViewForOutsource = new ParentModel()
                {
                    AppName = Globals.AppNameEncrypt,
                    SaleOrg = _saleOrg,
                    PlantCode = productInfoView.PLANTCODE,
                    PlantView = plantViewObject
                };

                if (!IsExistPlantView(productInfoView.PLANTCODE, materialNoOs))
                {
                    string parentModelPlantViewJsonStringForOutsource = JsonConvert.SerializeObject(parentModelPlantViewForOutsource);
                    _plantViewAPIRepository.SavePlantView(parentModelPlantViewJsonStringForOutsource, _token);
                }
            }
        }

        private void UpdatePlantView(string materialNo, MasterData masterData)
        {
            var plantViewObject = JsonConvert.DeserializeObject<PlantView>(_plantViewAPIRepository.GetPlantViewByMaterialNo(_factoryCode, materialNo, _token));

            string PDIS_Status = string.Empty;
            bool Tran_Status = false, SAP_Status = false;

            if (plantViewObject == null)
            {
                plantViewObject = new PlantView()
                {
                    MaterialNo = materialNo,
                    FactoryCode = _factoryCode,
                    Plant = _factoryCode,
                    PurchCode = JsonConvert.DeserializeObject<CompanyProfile>(_companyProfileAPIRepository.GetCompanyProfileByPlant(_factoryCode, _token)).PurchasGrp,
                    StdTotalCost = 0,
                    StdMovingCost = 0,
                    StdFc = 0,
                    StdVc = 0,
                    PdisStatus = "N",
                    TranStatus = true,
                    SapStatus = false
                };

                ParentModel parentModelPlantView = new ParentModel()
                {
                    AppName = Globals.AppNameEncrypt,
                    SaleOrg = _saleOrg,
                    PlantCode = _factoryCode,
                    PlantView = plantViewObject
                };

                string parentModelPlantViewJsonString = JsonConvert.SerializeObject(parentModelPlantView);
                _plantViewAPIRepository.SavePlantView(parentModelPlantViewJsonString, _token);
            }
            else
            {
                //if (plantViewObject.TranStatus == false && plantViewObject.SapStatus == false)
                //{
                //    plantViewObject.PdisStatus = "C";
                //    plantViewObject.TranStatus = false;
                //    plantViewObject.SapStatus = false;

                //}
                //else if (plantViewObject.TranStatus == null || plantViewObject.SapStatus == null)
                //{
                //    plantViewObject.PdisStatus = "C";
                //    plantViewObject.TranStatus = false;
                //    plantViewObject.SapStatus = false;
                //}
                //else if (plantViewObject.PdisStatus == "N")
                //{
                //    plantViewObject.PdisStatus = "C";
                //    plantViewObject.TranStatus = false;
                //    plantViewObject.SapStatus = false;
                //}
                //else
                //{
                //    plantViewObject.PdisStatus = "M";
                //    plantViewObject.TranStatus = false;
                //    plantViewObject.SapStatus = plantViewObject.SapStatus;
                //}

                plantViewObject.PdisStatus = masterData.PdisStatus;
                plantViewObject.TranStatus = masterData.TranStatus;
                plantViewObject.SapStatus = masterData.SapStatus;

                ParentModel parentModelPlantView = new ParentModel()
                {
                    AppName = Globals.AppNameEncrypt,
                    SaleOrg = _saleOrg,
                    PlantCode = _factoryCode,
                    PlantView = plantViewObject
                };
                string parentModelPlantViewJsonString = JsonConvert.SerializeObject(parentModelPlantView);
                _plantViewAPIRepository.UpdatePlantView(parentModelPlantViewJsonString, _token);
            }
        }

        private void UpdatePlantViewOs(string materialNo, ProductInfoView productInfoView, string materialNoOs)
        {
            var plantViewObject = JsonConvert.DeserializeObject<PlantView>(_plantViewAPIRepository.GetPlantViewByMaterialNo(_factoryCode, materialNo, _token));

            var plantViewOs = new PlantView
            {
                EffectiveDate = plantViewObject.EffectiveDate,
                MaterialNo = materialNoOs,
                FactoryCode = productInfoView.PLANTCODE,
                PdisStatus = "N",
                Plant = productInfoView.PLANTCODE,
                PurchCode = JsonConvert.DeserializeObject<CompanyProfile>(_companyProfileAPIRepository.GetCompanyProfileByPlant(productInfoView.PLANTCODE, _token)).PurchasGrp,
                SapStatus = plantViewObject.SapStatus,
                ShipBlk = plantViewObject.ShipBlk,
                StdFc = plantViewObject.StdFc,
                StdMovingCost = plantViewObject.StdMovingCost,
                StdTotalCost = plantViewObject.StdTotalCost,
                StdVc = plantViewObject.StdVc,
                TranStatus = plantViewObject.TranStatus,
            };

            ParentModel parentModelPlantViewForOutsource = new ParentModel()
            {
                AppName = Globals.AppNameEncrypt,
                SaleOrg = _saleOrg,
                PlantCode = productInfoView.PLANTCODE,
                PlantView = plantViewOs
            };

            string parentModelPlantViewJsonStringForOutsource = JsonConvert.SerializeObject(parentModelPlantViewForOutsource);
            _plantViewAPIRepository.SavePlantView(parentModelPlantViewJsonStringForOutsource, _token);
        }

        void GetHVLProductTypeSelectList(ref TransactionDataModel transactionDataModel)
        {
            var hvaMasters = JsonConvert.DeserializeObject<List<HvaMaster>>(_hvaMasterAPIRepository.GetHvaMasters(_factoryCode, _token));

            transactionDataModel.modelProductInfo.HvaMasters = hvaMasters;

            transactionDataModel.modelProductInfo.HVL_ProdTypeList1 = hvaMasters.Where(h => h.Seq == 1).Select(s => new SelectListItem() { Value = s.HighValue.ToString(), Text = s.HighValue.ToString() + " " + s.HighDescription });
            transactionDataModel.modelProductInfo.HVL_ProdTypeList2 = hvaMasters.Where(h => h.Seq == 2).Select(s => new SelectListItem() { Value = s.HighValue.ToString(), Text = s.HighValue.ToString() + " " + s.HighDescription });
            transactionDataModel.modelProductInfo.HVL_ProdTypeList3 = hvaMasters.Where(h => h.Seq == 3).Select(s => new SelectListItem() { Value = s.HighValue.ToString(), Text = s.HighValue.ToString() + " " + s.HighDescription });
            transactionDataModel.modelProductInfo.HVL_ProdTypeList4 = hvaMasters.Where(h => h.Seq == 4).Select(s => new SelectListItem() { Value = s.HighValue.ToString(), Text = s.HighValue.ToString() + " " + s.HighDescription });
            transactionDataModel.modelProductInfo.HVL_ProdTypeList5 = hvaMasters.Where(h => h.Seq == 5).Select(s => new SelectListItem() { Value = s.HighValue.ToString(), Text = s.HighValue.ToString() + " " + s.HighDescription });
            transactionDataModel.modelProductInfo.HVL_ProdTypeList6 = hvaMasters.Where(h => h.Seq == 6).Select(s => new SelectListItem() { Value = s.HighValue.ToString(), Text = s.HighValue.ToString() + " " + s.HighDescription });
            transactionDataModel.modelProductInfo.HVL_ProdTypeList7 = hvaMasters.Where(h => h.Seq == 7).Select(s => new SelectListItem() { Value = s.HighValue.ToString(), Text = s.HighValue.ToString() + " " + s.HighDescription });
        }

        private void GetOutsources(ref TransactionDataModel transactionDataModelSession)
        {
            var masterdatas = new List<MasterData>();
            if (!string.IsNullOrEmpty(transactionDataModelSession.MaterialNo))
            {
                masterdatas = JsonConvert.DeserializeObject<List<MasterData>>(_masterDataAPIRepository.GetMasterDatasByMatSaleOrgNonX(_factoryCode, transactionDataModelSession.MaterialNo, _token));
            }

            transactionDataModelSession.modelProductInfo.Outsources = new List<Outsource>();

            //get all plantCode
            var companyprofiles = JsonConvert.DeserializeObject<List<CompanyProfile>>(_companyProfileAPIRepository.GetCompanyProfileList(_factoryCode, _token)).Where(p => p.Plant != _factoryCode).Select(c => new { Plant = c.Plant, SaleOrg = c.SaleOrg, ShortName = c.ShortName });
            var isOutsource = false;
            int? orderTypeId = null;
            var realFlag = transactionDataModelSession.RealEventFlag;
            var plantOs = "";

            var outsourceStrs = new List<string>();

            foreach (var companyprofile in companyprofiles)
            {
                orderTypeId = null;
                isOutsource = false;
                var masterdata = masterdatas.FirstOrDefault(m => m.FactoryCode == companyprofile.Plant);
                //var selectFirstOursource = JsonConvert.DeserializeObject<TransactionsDetail>(_transactionsDetailAPIRepository.GetSelectedFirstOutsourceByMaterialNo(_factoryCode, transactionDataModelSession.MaterialNo, _token));

                if (masterdata != null
                    && ((realFlag != "Copy" && realFlag != "CopyAndDelete" && realFlag != "CopyX")
                    || (transactionDataModelSession.EventFlag == "Create" && !String.IsNullOrEmpty(transactionDataModelSession.MaterialNo))))
                {
                    var transactionDetail = JsonConvert.DeserializeObject<TransactionsDetail>(_transactionsDetailAPIRepository.GetTransactionsDetailByMat(masterdata.FactoryCode, masterdata.MaterialNo, _token));

                    if (transactionDetail != null)
                    {
                        isOutsource = transactionDetail.Outsource;
                        orderTypeId = transactionDetail.HireOrderType;
                    }

                    plantOs = masterdata.Plant;
                }
                else
                {
                    plantOs = companyprofile.Plant;
                }

                transactionDataModelSession.modelProductInfo.Outsources.Add(new Outsource
                {
                    IsOutsource = isOutsource,
                    HireOrderType = orderTypeId,
                    SaleOrg = companyprofile.SaleOrg,
                    PlantOs = plantOs,
                    ShortName = companyprofile.ShortName
                });
            }
        }

        private bool IsExistTransactionDetail(string factoryCode, string materialNo)
        {
            var existTransactionDetail = JsonConvert.DeserializeObject<TransactionsDetail>(_transactionsDetailAPIRepository.GetTransactionsDetailByMat(factoryCode, materialNo, _token));

            return existTransactionDetail != null ? true : false;
        }

        private bool IsExistPlantView(string factoryCode, string materialNo)
        {
            var existPlantView = JsonConvert.DeserializeObject<PlantView>(_plantViewAPIRepository.GetPlantViewByMaterialNoAndPlant(_factoryCode, materialNo, factoryCode, _token));

            return existPlantView != null ? true : false;
        }

        private bool IsExistSaleView(string factoryCode, string materialNo, string saleOrg, int channel)
        {
            var existSaleView = JsonConvert.DeserializeObject<SalesView>(_salesViewAPIRepository.GetSaleViewByMaterialNoAndChannelAndDevPlant(_factoryCode, materialNo, channel, factoryCode, _token));

            return existSaleView != null ? true : false;
        }

        private bool IsExistMasterData(string factoryCode, string materialNo)
        {
            var existMasterData = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNumberNonX(factoryCode, materialNo, _token));

            return existMasterData != null ? true : false;
        }

        private void UpdateRoutingStatus(MasterData masterData)
        {
            var existRoutings = JsonConvert.DeserializeObject<List<Routing>>(_routingAPIRepository.GetRoutingsByMaterialNo(masterData.FactoryCode, masterData.MaterialNo, _token));
            var routings = new List<Routing>();
            foreach (var routing in existRoutings)
            {
                routing.PdisStatus = masterData.PdisStatus;
                routing.TranStatus = masterData.TranStatus;
                routing.SapStatus = masterData.SapStatus;
                routing.Id = 0;

                routings.Add(routing);
            }

            if (routings.Count > 0)
            {
                _routingAPIRepository.SaveRouting(_factoryCode, masterData.MaterialNo, JsonConvert.SerializeObject(routings), _token);
            }
        }

        private void UpdateRemarkCOROfRouting(string materialNo, string factoryCode, ref TransactionDataModel transactionDataModel, string hvaCorr)
        {
            var existRoutings = JsonConvert.DeserializeObject<List<Routing>>(_routingAPIRepository.GetRoutingsByMaterialNo(factoryCode, materialNo, _token));
            var routingCorr = existRoutings.FirstOrDefault(r => r.MatCode.ToUpper().Contains("COR"));
            if (routingCorr != null && !string.IsNullOrEmpty(hvaCorr))
            {
                var hvaMasters = JsonConvert.DeserializeObject<List<HvaMaster>>(_hvaMasterAPIRepository.GetHvaMasters(_factoryCode, _token));

                transactionDataModel.modelProductInfo.HVL_ProdTypeList5 = hvaMasters.Where(h => h.Seq == 5).Select(s => new SelectListItem() { Value = s.HighValue.ToString(), Text = s.HighValue.ToString() + " " + s.HighDescription });
                var hvasCor = new SelectListItem();
                if (!string.IsNullOrEmpty(routingCorr.RemarkInprocess))
                {
                    hvasCor = transactionDataModel.modelProductInfo.HVL_ProdTypeList5.FirstOrDefault(h => routingCorr.RemarkInprocess.Contains(h.Text.Substring(5)));
                }
                else
                {
                    hvasCor = null;
                }

                if (hvasCor == null)
                {
                    transactionDataModel.modelRouting.RoutingDataList.ForEach(r => r.Remark = r.Machine == routingCorr.Machine ? $"{hvaCorr} {routingCorr.RemarkInprocess}" : r.Remark);
                    routingCorr.RemarkInprocess = $"{hvaCorr} {routingCorr.RemarkInprocess}";
                    _routingAPIRepository.Update1RowOfRouting(_factoryCode, JsonConvert.SerializeObject(routingCorr), _token);
                }
                else
                {
                    var hvaCorrOld = hvasCor.Text.Substring(5);
                    var oldLenght = hvaCorrOld.Length;

                    if (hvaCorr != hvaCorrOld)
                    {
                        if (routingCorr.RemarkInprocess.Substring(0, oldLenght) != hvaCorrOld)
                        {
                            //update remark
                            if (transactionDataModel.modelRouting.RoutingDataList.FirstOrDefault(r => r.Machine == routingCorr.Machine) != null)
                            {
                                transactionDataModel.modelRouting.RoutingDataList.ForEach(r => r.Remark = r.Machine == routingCorr.Machine ? hvaCorr + routingCorr.RemarkInprocess : r.Remark);
                                routingCorr.RemarkInprocess = hvaCorr + routingCorr.RemarkInprocess;
                                _routingAPIRepository.Update1RowOfRouting(_factoryCode, JsonConvert.SerializeObject(routingCorr), _token);
                            }
                        }
                        else
                        {
                            transactionDataModel.modelRouting.RoutingDataList.ForEach(r => r.Remark = r.Machine == routingCorr.Machine ? hvaCorr + routingCorr.RemarkInprocess.Substring(oldLenght) : r.Remark);
                            routingCorr.RemarkInprocess = hvaCorr + routingCorr.RemarkInprocess.Substring(oldLenght);
                            _routingAPIRepository.Update1RowOfRouting(_factoryCode, JsonConvert.SerializeObject(routingCorr), _token);
                        }
                    }
                }
            }
        }

        private void CopyRouting(string realFlag, MasterData masterData, string newMaterialNo, string plantOs)
        {
            var existRoutings = JsonConvert.DeserializeObject<List<Routing>>(_routingAPIRepository.GetRoutingsByMaterialNo(masterData.FactoryCode, masterData.MaterialNo, _token));
            var routings = new List<Routing>();
            foreach (var routing in existRoutings)
            {
                routing.MaterialNo = newMaterialNo;
                routing.PdisStatus = "N";
                routing.TranStatus = false;
                routing.SapStatus = false;
                routing.Id = 0;
                routing.ScoreType = null;
                routings.Add(routing);
            }

            if (routings.Count > 0)
            {
                if (!realFlag.Equals("CopyOs"))
                {
                    _routingAPIRepository.SaveRouting(_factoryCode, newMaterialNo, JsonConvert.SerializeObject(routings), _token);
                }

                if (!string.IsNullOrEmpty(plantOs))
                {
                    routings.ForEach(r => r.FactoryCode = plantOs);
                    routings.ForEach(r => r.Plant = plantOs);
                    routings.ForEach(r => r.TranStatus = true);
                    _routingAPIRepository.SaveRouting(plantOs, newMaterialNo, JsonConvert.SerializeObject(routings), _token);
                }
            }
        }

        private void CopyRoutingFromPresale(TransactionDataModel transactionDataModel, string materialNo, string plantOs)
        {
            transactionDataModel.MaterialNo = materialNo;
            _routingService.PresaleSaveRouting(transactionDataModel);

            if (!string.IsNullOrEmpty(plantOs))
            {
                var existRoutings = JsonConvert.DeserializeObject<List<Routing>>(_routingAPIRepository.GetRoutingsByMaterialNo(_factoryCode, materialNo, _token));
                var routings = new List<Routing>();
                foreach (var routing in existRoutings)
                {
                    routing.MaterialNo = materialNo;
                    routing.PdisStatus = "N";
                    routing.TranStatus = true;
                    routing.SapStatus = false;
                    routing.FactoryCode = plantOs;
                    routing.Plant = plantOs;
                    routing.Id = 0;
                    routing.ScoreType = null;

                    routings.Add(routing);
                }

                if (routings.Count > 0)
                {
                    _routingAPIRepository.SaveRouting(_factoryCode, materialNo, JsonConvert.SerializeObject(routings), _token);
                }
            }
        }

        private void CheckCreateBoardUse(string materialNoMainOEM, string materialNoSubOEM, string factoryCode)
        {
            var boardUse = JsonConvert.DeserializeObject<BoardUse>(_boardUseAPIRepository.GetBoardUseByMaterialNo(_factoryCode, materialNoMainOEM, _token));
            var boardUseOEM = JsonConvert.DeserializeObject<BoardUse>(_boardUseAPIRepository.GetBoardUseByMaterialNo(factoryCode, materialNoSubOEM, _token));
            if (boardUse != null && boardUseOEM == null)
            {
                boardUse.FactoryCode = factoryCode;
                boardUse.MaterialNo = materialNoSubOEM;
                boardUse.Id = 0;
                boardUse.CreatedBy = _username;
                boardUse.CreatedDate = DateTime.Now;
                boardUse.UpdatedBy = _username;
                boardUse.UpdatedDate = DateTime.Now;

                ParentModel Parent = new ParentModel();
                Parent.AppName = Globals.AppNameEncrypt;
                Parent.SaleOrg = _saleOrg;
                Parent.PlantCode = _factoryCode;
                Parent.BoardUse = boardUse;
                _boardUseAPIRepository.SaveBoardUse(JsonConvert.SerializeObject(Parent), _token);
            }
        }

        #region EANBarcode

        private string GetEanCode(string materialNo)
        {
            if (string.IsNullOrEmpty(materialNo))
            {
                return string.Empty;
            }

            string MatString = materialNo.Substring(materialNo.Length - 5);

            string[] Ean = new string[14];
            Ean[0] = "885" + _saleOrg + MatString;
            Ean[1] = Ean[0].Substring(0, 1);
            Ean[2] = Ean[0].Substring(1, 1);
            Ean[3] = Ean[0].Substring(2, 1);
            Ean[4] = Ean[0].Substring(3, 1);
            Ean[5] = Ean[0].Substring(4, 1);
            Ean[6] = Ean[0].Substring(5, 1);
            Ean[7] = Ean[0].Substring(6, 1);

            Ean[8] = Ean[0].Substring(7, 1);
            Ean[9] = Ean[0].Substring(8, 1);
            Ean[10] = Ean[0].Substring(9, 1);
            Ean[11] = Ean[0].Substring(10, 1);
            Ean[12] = Ean[0].Substring(11, 1);
            // Ean[12] = "";

            int[] EanLevel = new int[5];
            EanLevel[0] = 0;
            EanLevel[1] = 0;
            EanLevel[2] = 0;
            //long result;
            for (int i = 1; i < 13; i++)
            {
                if ((i % 2) == 0)
                {
                    // EanLevel[1] = EanLevel[1] + Int32.Parse(Ean[i]);
                    try
                    {
                        EanLevel[1] = EanLevel[1] + Convert.ToInt32(Ean[i], 16);
                    }
                    catch (Exception)
                    {
                        EanLevel[1] = EanLevel[1] + 0; ///Ean[8] ==I //โรงงาน PPC
                    }
                }
                else
                {
                    //result = Convert.ToInt64(Ean[i]);
                    try
                    {
                        EanLevel[0] = EanLevel[0] + Convert.ToInt32(Ean[i], 16);
                    }
                    catch (Exception)
                    {
                        EanLevel[0] = EanLevel[0] + 0; ///Ean[7] ==G //โรงงาน Din
                        //throw;
                    }
                }
            }

            EanLevel[2] = EanLevel[1] * 3;
            EanLevel[3] = EanLevel[0] + EanLevel[2];
            EanLevel[4] = EanLevel[3] % 10;
            if (EanLevel[4] == 0)
            {
                Ean[13] = "0";
            }
            else
            {
                Ean[13] = (10 - EanLevel[4]).ToString();
            }
            var EANCODE = Ean[0] + Ean[13];
            return EANCODE;
        }

        #endregion EANBarcode
    }
}