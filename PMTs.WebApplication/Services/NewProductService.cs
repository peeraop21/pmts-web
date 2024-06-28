using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySqlX.XDevAPI.Common;
using Newtonsoft.Json;
using PMTs.DataAccess;
using PMTs.DataAccess.ComplexModel;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.MaintenanceCustomer;
using PMTs.DataAccess.ModelView.NewProduct;
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
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static PMTs.DataAccess.ModelView.NewProduct.TransactionDataModel;

namespace PMTs.WebApplication.Services
{
    [TraceAspect]
    public class NewProductService : INewProductService
    {
        private IHttpContextAccessor _httpContextAccessor;
        private IHostingEnvironment _hostingEnvironment;
        private readonly IMapper mapper;
        private readonly ICustomerAPIRepository _customerAPIRepository;
        private readonly IBoardCombineAPIRepository _boardCombineAPIRepository;
        private readonly IBoardCombineAccAPIRepository _boardCombineAccAPIRepository;
        private readonly IFluteAPIRepository _fluteAPIRepository;
        private readonly IFluteTrAPIRepository _fluteTrAPIRepository;
        private readonly IHoneyPaperAPIRepository _honeyPaperAPIRepository;
        private readonly IProductTypeAPIRepository _productTypeAPIRepository;
        private readonly IPaperGradeAPIRepository _paperGradeAPIRepository;
        private readonly IMapCostAPIRepository _mapCostAPIRepository;
        private readonly IMachineAPIRepository _machineAPIRepository;
        private readonly IColorAPIRepository _colorAPIRepository;
        private readonly IBoardAlternativeAPIRepository _boardAlternativeAPIRepository;
        private readonly IMasterDataAPIRepository _masterDataAPIRepository;
        private readonly IMaterialTypeAPIRepository _materialTypeAPIRepository;
        private readonly IPlantViewAPIRepository _plantViewAPIRepository;
        private readonly IUnitMaterialAPIRepository _unitMaterialAPIRepository;
        private readonly IKindOfProductGroupAPIRepository _kindOfProductGroupAPIRepository;
        private readonly ITransactionsDetailAPIRepository _transactionsDetailAPIRepository;
        private readonly IRoutingAPIRepository _routingAPIRepository;
        private readonly IJoinAPIRepository _joinAPIRepository;
        private readonly IPrintMethodAPIRepository _printMethodAPIRepository;
        private readonly IPalletAPIRepository _palletAPIRepository;
        private readonly ICustShipToAPIRepository _custShipToAPIRepository;
        private readonly ICompanyProfileAPIRepository _companyProfileAPIRepository;
        private readonly IRunningNoAPIRepository _runningNoAPIRepository;
        private readonly ISalesViewAPIRepository _salesViewAPIRepository;
        private readonly IPMTsConfigAPIRepository _pMTsConfigAPIRepository;
        private readonly IKindOfProductAPIRepository _kindOfProductAPIRepository;
        private readonly IProcessCostAPIRepository _processCostAPIRepository;
        private readonly IHierarchyLv2APIRepository _hierarchyLv2APIRepository;
        private readonly IHierarchyLv3APIRepository _hierarchyLv3APIRepository;
        private readonly IHierarchyLv4APIRepository _hierarchyLv4APIRepository;
        private readonly IScoreGapAPIRepository _scoreGapAPIRepository;
        private readonly IScoreTypeAPIRepository _scoreTypeAPIRepository;
        private readonly ICoatingAPIRepository _coatingAPIRepository;
        private readonly IProductSpecService _productSpecService;
        private readonly IMachineGroupAPIRepository _machineGroupAPIRepository;
        private readonly IHvaMasterAPIRepository _hvaMasterAPIRepository;
        private readonly IQualitySpecAPIRepository _qualitySpecAPIRepository;
        private readonly IHireMappingAPIRepository _hireMappingAPIRepository;
        private readonly IHireOrderAPIRepository _hireOrderAPIRepository;
        private readonly IProductGroupAPIRepository _productGroupAPIRepository;
        private readonly IFSCCodeAPIRepository fscCodeAPIRepository;
        private readonly IFSCFGCodeAPIRepository fscFGCodeAPIRepository;
        private readonly IPPCMasterRpacAPIRepository pPCMasterRpacAPIRepository;

        private readonly IExtensionService _extensionService;

        private readonly string _username;
        private readonly string _saleOrg;
        private readonly string _factoryCode;
        private readonly string _token;
        private readonly bool _businessGroup;

        public NewProductService(
            IHttpContextAccessor httpContextAccessor,
            IHostingEnvironment hostingEnvironment,
            ICustomerAPIRepository customerAPIRepository,
            IBoardCombineAPIRepository boardCombineAPIRepository,
            IBoardCombineAccAPIRepository boardCombineAccAPIRepository,
            IFluteAPIRepository fluteAPIRepository,
            IFluteTrAPIRepository fluteTrAPIRepository,
            IHoneyPaperAPIRepository honeyPaperAPIRepository,
            IProductTypeAPIRepository productTypeAPIRepository,
            IPaperGradeAPIRepository paperGradeAPIRepository,
            IMapCostAPIRepository mapCostAPIRepository,
            IMachineAPIRepository machineAPIRepository,
            IColorAPIRepository colorAPIRepository,
            IBoardAlternativeAPIRepository boardAlternativeAPIRepository,
            IMasterDataAPIRepository masterDataAPIRepository,
            IMaterialTypeAPIRepository materialTypeAPIRepository,
            IPlantViewAPIRepository plantViewAPIRepository,
            IUnitMaterialAPIRepository unitMaterialAPIRepository,
            IKindOfProductGroupAPIRepository kindOfProductGroupAPIRepository,
            ITransactionsDetailAPIRepository transactionsDetailAPIRepository,
            IRoutingAPIRepository routingAPIRepository,
            IJoinAPIRepository joinAPIRepository,
            IPrintMethodAPIRepository printMethodAPIRepository,
            IPalletAPIRepository palletAPIRepository,
            ICustShipToAPIRepository custShipToAPIRepository,
            ICompanyProfileAPIRepository companyProfileAPIRepository,
            IRunningNoAPIRepository runningNoAPIRepository,
            ISalesViewAPIRepository salesViewAPIRepository,
            IPMTsConfigAPIRepository pMTsConfigAPIRepository,
            IProcessCostAPIRepository processCostAPIRepository,
            IKindOfProductAPIRepository kindOfProductAPIRepository,
            IHierarchyLv2APIRepository hierarchyLv2APIRepository,
            IHierarchyLv3APIRepository hierarchyLv3APIRepository,
            IHierarchyLv4APIRepository hierarchyLv4APIRepository,
            IExtensionService extensionService,
            IScoreTypeAPIRepository scoreTypeAPIRepository,
            IScoreGapAPIRepository scoreGapAPIRepository,
            ICoatingAPIRepository coatingAPIRepository,
            IMachineGroupAPIRepository machineGroupAPIRepository,
            IHvaMasterAPIRepository hvaMasterAPIRepository,
            IQualitySpecAPIRepository qualitySpecAPIRepository,
            IHireMappingAPIRepository hireMappingAPIRepository,
            IHireOrderAPIRepository hireOrderAPIRepository,
            IProductGroupAPIRepository productGroupAPIRepository,
            IFSCCodeAPIRepository fSCCodeAPIRepository,
            IFSCFGCodeAPIRepository fSCFGCodeAPIRepository,
            IPPCMasterRpacAPIRepository pPCMasterRpacAPIRepository,
            IMapper mapper
            )

        {
            // Initialize HttpContext
            _httpContextAccessor = httpContextAccessor;
            _hostingEnvironment = hostingEnvironment;

            // Initialize Repository
            _customerAPIRepository = customerAPIRepository;
            _boardCombineAPIRepository = boardCombineAPIRepository;
            _boardCombineAccAPIRepository = boardCombineAccAPIRepository;
            _fluteAPIRepository = fluteAPIRepository;
            _fluteTrAPIRepository = fluteTrAPIRepository;
            _honeyPaperAPIRepository = honeyPaperAPIRepository;
            _productTypeAPIRepository = productTypeAPIRepository;
            _paperGradeAPIRepository = paperGradeAPIRepository;
            _mapCostAPIRepository = mapCostAPIRepository;
            _machineAPIRepository = machineAPIRepository;
            _colorAPIRepository = colorAPIRepository;
            _boardAlternativeAPIRepository = boardAlternativeAPIRepository;
            _masterDataAPIRepository = masterDataAPIRepository;
            _materialTypeAPIRepository = materialTypeAPIRepository;
            _plantViewAPIRepository = plantViewAPIRepository;
            _unitMaterialAPIRepository = unitMaterialAPIRepository;
            _kindOfProductGroupAPIRepository = kindOfProductGroupAPIRepository;
            _transactionsDetailAPIRepository = transactionsDetailAPIRepository;
            _routingAPIRepository = routingAPIRepository;
            _joinAPIRepository = joinAPIRepository;
            _printMethodAPIRepository = printMethodAPIRepository;
            _palletAPIRepository = palletAPIRepository;
            _custShipToAPIRepository = custShipToAPIRepository;
            _companyProfileAPIRepository = companyProfileAPIRepository;
            _runningNoAPIRepository = runningNoAPIRepository;
            _salesViewAPIRepository = salesViewAPIRepository;
            _pMTsConfigAPIRepository = pMTsConfigAPIRepository;
            _processCostAPIRepository = processCostAPIRepository;
            _hierarchyLv2APIRepository = hierarchyLv2APIRepository;
            _hierarchyLv3APIRepository = hierarchyLv3APIRepository;
            _hierarchyLv4APIRepository = hierarchyLv4APIRepository;
            _kindOfProductAPIRepository = kindOfProductAPIRepository;
            _extensionService = extensionService;
            _scoreTypeAPIRepository = scoreTypeAPIRepository;
            _scoreGapAPIRepository = scoreGapAPIRepository;
            _coatingAPIRepository = coatingAPIRepository;
            _machineGroupAPIRepository = machineGroupAPIRepository;
            _hvaMasterAPIRepository = hvaMasterAPIRepository;
            _qualitySpecAPIRepository = qualitySpecAPIRepository;
            _hireMappingAPIRepository = hireMappingAPIRepository;
            _hireOrderAPIRepository = hireOrderAPIRepository;
            _productGroupAPIRepository = productGroupAPIRepository;
            this.fscCodeAPIRepository = fSCCodeAPIRepository;
            this.fscFGCodeAPIRepository = fSCFGCodeAPIRepository;
            this.pPCMasterRpacAPIRepository = pPCMasterRpacAPIRepository;
            this.mapper = mapper;

            // Initialize User Data From Session
            var userSessionModel = SessionExtentions.GetSession<UserSessionModel>(_httpContextAccessor.HttpContext.Session, "UserSessionModel");
            if (userSessionModel != null)
            {
                _username = userSessionModel.UserName;
                _saleOrg = userSessionModel.SaleOrg;
                _factoryCode = userSessionModel.FactoryCode;
                _token = userSessionModel.Token;
                _businessGroup = userSessionModel.BusinessGroup == "Offset";
            }
        }

        public MasterDataTransactionModel GetMasterDataTransaction(string actionTran, string materialNo, string plantCode)
        {
            var factoryCode = _factoryCode;
            if (!string.IsNullOrEmpty(plantCode))
            {
                factoryCode = plantCode;
            }
            MasterDataTransactionModel oMasterData = new MasterDataTransactionModel();
            oMasterData.TransactionAction = actionTran;

            oMasterData.MasterData = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNo(factoryCode, materialNo, _token));
            oMasterData.TransactionsDetail = JsonConvert.DeserializeObject<TransactionsDetail>(_transactionsDetailAPIRepository.GetTransactionsDetailByMat(factoryCode, materialNo, _token));

            if (actionTran == "CopyOs" && oMasterData.TransactionsDetail == null)
            {
                throw new Exception("กรุณาให้ผู้จ้าง เข้าไป edit mat นี้ให้เรียบร้อยก่อน");
            }

            if (!string.IsNullOrEmpty(oMasterData.MasterData.HighValue))
            {
                var hvaCodeFromHighValue = JsonConvert.DeserializeObject<HvaMaster>(_hvaMasterAPIRepository.GetHvaMasterByHighValue(_factoryCode, oMasterData.MasterData.HighValue, _token));
                if (hvaCodeFromHighValue != null && oMasterData.TransactionsDetail != null)
                {
                    switch (hvaCodeFromHighValue.Seq)
                    {
                        case 1:
                            oMasterData.TransactionsDetail.HvaGroup1 = hvaCodeFromHighValue.HighValue;
                            break;

                        case 2:
                            oMasterData.TransactionsDetail.HvaGroup2 = hvaCodeFromHighValue.HighValue;
                            break;

                        case 3:
                            oMasterData.TransactionsDetail.HvaGroup3 = hvaCodeFromHighValue.HighValue;
                            break;

                        case 4:
                            oMasterData.TransactionsDetail.HvaGroup4 = hvaCodeFromHighValue.HighValue;
                            break;

                        case 5:
                            oMasterData.TransactionsDetail.HvaGroup5 = hvaCodeFromHighValue.HighValue;
                            break;

                        case 6:
                            oMasterData.TransactionsDetail.HvaGroup6 = hvaCodeFromHighValue.HighValue;
                            break;

                        case 7:
                            oMasterData.TransactionsDetail.HvaGroup7 = hvaCodeFromHighValue.HighValue;
                            break;
                    }
                }
            }

            //oMasterData.PlantView = JsonConvert.DeserializeObject<PlantView>(_plantViewAPIRepository.GetPlantViewByMaterialNo(plantCode, materialNo));
            //if (oMasterData.PlantView == null)
            oMasterData.PlantView = JsonConvert.DeserializeObject<PlantView>(_plantViewAPIRepository.GetPlantViewByMaterialNo(factoryCode, materialNo, _token));

            oMasterData.BoardAltViewModels = mapper.Map<List<BoardAlternative>, List<BoardAltViewModel>>(JsonConvert.DeserializeObject<List<BoardAlternative>>(_boardAlternativeAPIRepository.GetBoardAlternativeByMat(factoryCode, materialNo, _token)));
            oMasterData.Routings = JsonConvert.DeserializeObject<List<Routing>>(_routingAPIRepository.GetRoutingsByMaterialNo(factoryCode, materialNo, _token));

            return oMasterData;
        }

        public MasterDataTransactionModel GetMasterDataTransactionPresale(MasterDataTransactionModel oMasterData)
        {
            TransactionsDetail transactionsDetail = new TransactionsDetail();
            String matType = oMasterData.MasterData != null ? oMasterData.MasterData.MaterialType : string.Empty;

            var productType = JsonConvert.DeserializeObject<ProductType>(_productTypeAPIRepository.GetProductTypeByBoxType(oMasterData.MasterData != null ? oMasterData.MasterData.BoxType : string.Empty, _token));
            var materialType = JsonConvert.DeserializeObject<MaterialType>(_materialTypeAPIRepository.GetMaterialTypeByMaterialCode(oMasterData.MasterData != null ? oMasterData.MasterData.MaterialType : string.Empty, _token));

            transactionsDetail.IdMaterialType = oMasterData.MasterData != null && materialType != null ? materialType.Id : 0;
            transactionsDetail.IdProductType = oMasterData.MasterData != null && productType != null ? productType.Id : 0;
            transactionsDetail.IdProcessCost = 0;
            transactionsDetail.IdKindOfProductGroup = 0;
            transactionsDetail.IdKindOfProduct = 0;

            if (oMasterData.TransactionsDetail != null)
            {
                transactionsDetail.IsWrap = oMasterData.TransactionsDetail.IsWrap;
                transactionsDetail.IsNotch = oMasterData.TransactionsDetail.IsNotch;
                transactionsDetail.NotchDegree = oMasterData.TransactionsDetail.NotchDegree;
                transactionsDetail.NotchArea = oMasterData.TransactionsDetail.NotchArea;
                transactionsDetail.NotchSide = oMasterData.TransactionsDetail.NotchSide;
                transactionsDetail.SideA = oMasterData.TransactionsDetail.SideA;
                transactionsDetail.SideB = oMasterData.TransactionsDetail.SideB;
                transactionsDetail.SideC = oMasterData.TransactionsDetail.SideC;
                transactionsDetail.SideD = oMasterData.TransactionsDetail.SideD;
                transactionsDetail.Cgtype = oMasterData.TransactionsDetail.Cgtype;
                transactionsDetail.HvaGroup1 = oMasterData.TransactionsDetail.HvaGroup1;
                transactionsDetail.HvaGroup2 = oMasterData.TransactionsDetail.HvaGroup2;
                transactionsDetail.HvaGroup3 = oMasterData.TransactionsDetail.HvaGroup3;
                transactionsDetail.HvaGroup4 = oMasterData.TransactionsDetail.HvaGroup4;
                transactionsDetail.HvaGroup5 = oMasterData.TransactionsDetail.HvaGroup5;
                transactionsDetail.HvaGroup6 = oMasterData.TransactionsDetail.HvaGroup6;
                transactionsDetail.HvaGroup7 = oMasterData.TransactionsDetail.HvaGroup7;
                transactionsDetail.IdProcessCost = oMasterData.TransactionsDetail.IdProcessCost;
                transactionsDetail.IdKindOfProduct = oMasterData.TransactionsDetail.IdKindOfProduct;
                transactionsDetail.IdMaterialType = oMasterData.TransactionsDetail.IdMaterialType;
                transactionsDetail.IdKindOfProduct = oMasterData.TransactionsDetail.IdKindOfProduct == null ? 0 : oMasterData.TransactionsDetail.IdKindOfProduct;
                transactionsDetail.IdMaterialType = transactionsDetail.IdMaterialType != null ? transactionsDetail.IdMaterialType : 0;
                transactionsDetail.IdProductType = transactionsDetail.IdProductType != null ? transactionsDetail.IdProductType : 0;
                transactionsDetail.IdProcessCost = transactionsDetail.IdProcessCost != null ? transactionsDetail.IdProcessCost : 0;
                transactionsDetail.IdKindOfProductGroup = transactionsDetail.IdKindOfProductGroup != null ? transactionsDetail.IdKindOfProductGroup : 0;
                transactionsDetail.IdKindOfProduct = transactionsDetail.IdKindOfProduct != null ? transactionsDetail.IdKindOfProduct : 0;

                transactionsDetail.NewPrintPlate = oMasterData.TransactionsDetail.NewPrintPlate;
                transactionsDetail.OldPrintPlate = oMasterData.TransactionsDetail.OldPrintPlate;
                transactionsDetail.NewBlockDieCut = oMasterData.TransactionsDetail.NewBlockDieCut;
                transactionsDetail.OldBlockDieCut = oMasterData.TransactionsDetail.OldBlockDieCut;
                transactionsDetail.ExampleColor = oMasterData.TransactionsDetail.ExampleColor;
                transactionsDetail.CoatingType = oMasterData.TransactionsDetail.CoatingType;
                transactionsDetail.CoatingTypeDesc = oMasterData.TransactionsDetail.CoatingTypeDesc;
                transactionsDetail.PaperHorizontal = oMasterData.TransactionsDetail.PaperHorizontal;
                transactionsDetail.PaperVertical = oMasterData.TransactionsDetail.PaperVertical;
                transactionsDetail.FluteHorizontal = oMasterData.TransactionsDetail.FluteHorizontal;
                transactionsDetail.FluteVertical = oMasterData.TransactionsDetail.FluteVertical;
                //transactionsDetail.IdMaterialType = oMasterData.TransactionsDetail.IdMaterialType == null ?
                //    oMasterData.MasterData != null && materialType != null ?
                //    materialType.Id : 0
                //    : oMasterData.TransactionsDetail.IdMaterialType;
            }
            //else
            //{
            //    transactionsDetail.IdMaterialType = oMasterData.MasterData != null && materialType != null ? materialType.Id : 0;
            //}

            if (matType == "84" || matType == "14" || matType == "24") // งาน BOM
            {
                transactionsDetail.IdProductUnit = 3;
                transactionsDetail.IdSaleUnit = 3;
            }
            else
            {
                int unit = 0;
                if (oMasterData.MasterData != null)
                {
                    unit = JsonConvert.DeserializeObject<UnitMaterial>(_unitMaterialAPIRepository.GetUnitMaterialByName(oMasterData.MasterData.SaleUom, _token)).Id;
                }

                transactionsDetail.IdProductUnit = unit;
                transactionsDetail.IdSaleUnit = unit;
            }

            oMasterData.TransactionsDetail = new TransactionsDetail
            {
                IdKindOfProduct = transactionsDetail.IdKindOfProduct,
                IdKindOfProductGroup = transactionsDetail.IdKindOfProductGroup,
                IdProcessCost = transactionsDetail.IdProcessCost,
                IdMaterialType = transactionsDetail.IdMaterialType,
                IdProductType = transactionsDetail.IdProductType,
                IdSaleUnit = transactionsDetail.IdSaleUnit,
                IdProductUnit = transactionsDetail.IdProductUnit,
                HvaGroup1 = transactionsDetail.HvaGroup1,
                HvaGroup2 = transactionsDetail.HvaGroup2,
                HvaGroup3 = transactionsDetail.HvaGroup3,
                HvaGroup4 = transactionsDetail.HvaGroup4,
                HvaGroup5 = transactionsDetail.HvaGroup5,
                HvaGroup6 = transactionsDetail.HvaGroup6,
                HvaGroup7 = transactionsDetail.HvaGroup7,
                NewPrintPlate = transactionsDetail.NewPrintPlate,
                OldPrintPlate = transactionsDetail.OldPrintPlate,
                NewBlockDieCut = transactionsDetail.NewBlockDieCut,
                OldBlockDieCut = transactionsDetail.OldBlockDieCut,
                ExampleColor = transactionsDetail.ExampleColor,
                CoatingType = transactionsDetail.CoatingType,
                CoatingTypeDesc = transactionsDetail.CoatingTypeDesc,
                PaperHorizontal = transactionsDetail.PaperHorizontal,
                PaperVertical = transactionsDetail.PaperVertical,
                FluteHorizontal = transactionsDetail.FluteHorizontal,
                FluteVertical = transactionsDetail.FluteVertical,
            };

            if (oMasterData.MasterData != null && !string.IsNullOrEmpty(oMasterData.MasterData.HighValue))
            {
                var hvaCodeFromHighValue = JsonConvert.DeserializeObject<List<HvaMaster>>(_hvaMasterAPIRepository.GetHvaMasters(_factoryCode, _token)).FirstOrDefault(h => h.HighValue == oMasterData.MasterData.HighValue);
                if (hvaCodeFromHighValue != null)
                {
                    switch (hvaCodeFromHighValue.Seq)
                    {
                        case 1:
                            oMasterData.TransactionsDetail.HvaGroup1 = hvaCodeFromHighValue.HighValue;
                            break;

                        case 2:
                            oMasterData.TransactionsDetail.HvaGroup2 = hvaCodeFromHighValue.HighValue;
                            break;

                        case 3:
                            oMasterData.TransactionsDetail.HvaGroup3 = hvaCodeFromHighValue.HighValue;
                            break;

                        case 4:
                            oMasterData.TransactionsDetail.HvaGroup4 = hvaCodeFromHighValue.HighValue;
                            break;

                        case 5:
                            oMasterData.TransactionsDetail.HvaGroup5 = hvaCodeFromHighValue.HighValue;
                            break;

                        case 6:
                            oMasterData.TransactionsDetail.HvaGroup6 = hvaCodeFromHighValue.HighValue;
                            break;

                        case 7:
                            oMasterData.TransactionsDetail.HvaGroup7 = hvaCodeFromHighValue.HighValue;
                            break;
                    }
                }
            }

            oMasterData.MasterData = oMasterData.MasterData == null ? new MasterData() : oMasterData.MasterData;
            oMasterData.PlantView = new PlantView();
            oMasterData.BoardAltViewModels = new List<BoardAltViewModel>();
            oMasterData.MasterData.FactoryCode = _factoryCode;

            return oMasterData;
        }

        #region BindData

        public ProductERPPurchaseViewModel BindDataPurchase(MasterDataTransactionModel obj)
        {
            ProductERPPurchaseViewModel model = new ProductERPPurchaseViewModel();
            model.PurTxt1 = obj.MasterData.PurTxt1;
            model.PurTxt2 = obj.MasterData.PurTxt2;
            model.PurTxt3 = obj.MasterData.PurTxt3;
            model.PurTxt4 = obj.MasterData.PurTxt4;

            //model.modelProductERPPurchase = ProductERPRepository

            return model;
        }

        public ProductERPPurchaseViewModel BindDataPurchaseByMatNo(string materialNo)
        {
            var master = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNo(_factoryCode, materialNo, _token));
            ProductERPPurchaseViewModel model = new ProductERPPurchaseViewModel();
            model.PurTxt1 = master.PurTxt1;
            model.PurTxt2 = master.PurTxt2;
            model.PurTxt3 = master.PurTxt3;
            model.PurTxt4 = master.PurTxt4;

            return model;
        }

        public ProductPropViewModel BindProductProp(MasterDataTransactionModel obj)
        {
            ProductPropViewModel model = new ProductPropViewModel();
            model.MaterialNo = obj.MasterData.MaterialNo;
            model.CutSheetWid = obj.MasterData.CutSheetWid;
            model.CutSheetLeng = obj.MasterData.CutSheetLeng;
            model.JoinId = obj.MasterData.JointId;
            model.JoinType = obj.MasterData.JoinType;
            model.Wire = obj.MasterData.Wire;
            model.OuterJoin = obj.MasterData.OuterJoin == true ? true : false;
            model.EANCODE = obj.MasterData.EanCode;
            model.AmountColor = obj.TransactionsDetail.AmountColor;
            model.PalletOverhang = obj.TransactionsDetail.PalletOverhang != null ? obj.TransactionsDetail.PalletOverhang.Value : 100;
            //model.PalletSize = obj.MasterData.PalletSize;
            model.NameJoinCharacter = obj.MasterData.JoinCharacter;

            //set pallet size to model

            if (!string.IsNullOrEmpty(obj.MasterData.PalletSize) && !string.IsNullOrWhiteSpace(obj.MasterData.PalletSize))
            {
                var indexOfSymbol = obj.MasterData.PalletSize.IndexOf('x');
                indexOfSymbol = indexOfSymbol == -1 ? obj.MasterData.PalletSize.IndexOf('X') : indexOfSymbol;
                if (indexOfSymbol != -1)
                {
                    var palletSizeWidth = obj.MasterData.PalletSize.Substring(0, indexOfSymbol);
                    var palletSizeLeg = obj.MasterData.PalletSize.Substring(indexOfSymbol + 1, obj.MasterData.PalletSize.Length - (indexOfSymbol + 1));
                    model.PalletSize = ($"{Convert.ToDouble(palletSizeWidth):0.00}" + "x" + $"{Convert.ToDouble(palletSizeLeg): 0.00}").Replace(" ", "");
                }
            }

            if (obj.MasterData.Flute == null)
            {
                model.BoxPerBundleNoJoint = 0;
                model.LayerPerPalletNoJoint = 0;
            }
            else
            {
                obj.MasterData.Flute = !string.IsNullOrEmpty(obj.MasterData.Flute) ? obj.MasterData.Flute.Trim() : obj.MasterData.Flute;

                var flu = new Flute();
                if (!string.IsNullOrEmpty(obj.MasterData.Flute))
                {
                    flu = JsonConvert.DeserializeObject<Flute>(_fluteAPIRepository.GetFluteByFlute(_factoryCode, obj.MasterData.Flute, _token));
                }

                model.BoxPerBundleNoJoint = flu != null && flu.BoxPerBundleNoJoint != null ? flu.BoxPerBundleNoJoint : 0;
                model.LayerPerPalletNoJoint = flu != null && flu.LayerPerPalletNoJoint != null ? flu.LayerPerPalletNoJoint : 0;
            }

            model.Bun = obj.MasterData.Bun;
            model.BunLayer = obj.MasterData.BunLayer;
            model.LayerPallet = obj.MasterData.LayerPalet;
            model.BoxPalet = obj.MasterData.BoxPalet;
            model.PieceSet = obj.MasterData.PieceSet == null ? 1 : obj.MasterData.PieceSet;
            model.PiecePatch = obj.MasterData.PiecePatch == null ? 1 : obj.MasterData.PiecePatch;
            model.BoxHandle = obj.MasterData.BoxHandle == true ? true : false;
            model.SparePercen = obj.MasterData.SparePercen;
            model.SpareMin = obj.MasterData.SpareMin;
            model.SpareMax = obj.MasterData.SpareMax;
            model.Hardship = obj.MasterData.Hardship == null ? 5 : obj.MasterData.Hardship;
            model.LeadTime = obj.MasterData.LeadTime;
            model.ChangeInfo = obj.MasterData.ChangeInfo;
            model.Change = obj.MasterData.Change;
            model.PrintMethod = obj.MasterData.PrintMethod;
            model.PathpalletSuggess = obj.MasterData.PalletizationPath;
            model.PathPallet = obj.MasterData.PalletizationPath;
            model.StatusFlag = obj.MasterData.StatusFlag;
            //model.ChangeHistory = MasterProp.ChangeHistory;
            //Get Joint
            //List<Joint> joinList = JsonConvert.DeserializeObject<List<Joint>>(_joinAPIRepository.GetJoinList(_factoryCode));
            //joinList.Insert(0, new Joint { JointId = "", JointName = "" });
            //model.JoinList = joinList.Select(s => new SelectListItem() { Value = s.JointName.ToString(), Text = s.JointName.ToString() }); ;

            List<Joint> joinList = JsonConvert.DeserializeObject<List<Joint>>(_joinAPIRepository.GetJoinList(_factoryCode, _token));
            joinList.Insert(0, new Joint { JointId = "", JointName = "" });

            // GetPrintMethod
            List<PrintMethod> PrintList = JsonConvert.DeserializeObject<List<PrintMethod>>(_printMethodAPIRepository.GetPrintMethodList(_factoryCode, _token));
            PrintList.Insert(0, new PrintMethod { Code = "0", Method = "" });

            //GetPallet
            //model.PalletSize = "1.00x1.20";

            model.PalletList = JsonConvert.DeserializeObject<List<Pallet>>(_palletAPIRepository.GetPalletList(_factoryCode, _token)).Select(s => new SelectListItem() { Value = s.PalletWidth.ToString() + "x" + s.PalletLength.ToString(), Text = s.PalletWidth.ToString() + "x" + s.PalletLength.ToString() });

            //Tassanai update 30072020
            model.TopSheetMaterial = obj.MasterData.TopSheetMaterial;
            model.NoneStandardPaper = obj.MasterData.NoneStandardPaper == null ? true : obj.MasterData.NoneStandardPaper.Value;

            //Tassanai update 13052022
            model.BoxPacking = obj.MasterData.BoxPacking;
            model.BOIStatus = obj.MasterData.Boistatus;
            model.WorkType = obj.MasterData.WorkType;

            return model;
        }

        public ProductCustomer BindCustomerData(MasterDataTransactionModel masterDataTransactionModel)
        {
            ProductCustomer productCustomer = new ProductCustomer();
            Customer customer = new Customer();
            if (!string.IsNullOrEmpty(masterDataTransactionModel.MasterData.CusId) && !string.IsNullOrEmpty(masterDataTransactionModel.MasterData.CustName))
            {
                customer = JsonConvert.DeserializeObject<Customer>(_customerAPIRepository.GetCustomerByCusID(_factoryCode, masterDataTransactionModel.MasterData.CusId, _token));
            }

            List<CustShipTo> custShipToList = !string.IsNullOrEmpty(masterDataTransactionModel.MasterData.CustCode) ? JsonConvert.DeserializeObject<List<CustShipTo>>(_custShipToAPIRepository.GetCustShipToListByCustCode(_factoryCode, masterDataTransactionModel.MasterData.CustCode, _token)) : null;

            if (customer != null)
            {
                productCustomer.CustCode = customer.CustCode;
                productCustomer.CusId = customer.CusId;
                productCustomer.CustName = customer.CustName;
                productCustomer.SoldToCode = customer.SoldToCode;
                productCustomer.CustAlert = customer.CustAlert;
                //check Industial Group in bind data.
                var productGroup = JsonConvert.DeserializeObject<ProductGroup>(_productGroupAPIRepository.GetProductGroupByCode(_factoryCode, masterDataTransactionModel.MasterData.IndGrp, _token));
                if (productGroup != null)
                {
                    productCustomer.IndGrp = masterDataTransactionModel.MasterData.IndGrp;
                    productCustomer.IndDes = masterDataTransactionModel.MasterData.IndDes;
                }
                else
                {
                    productCustomer.IndGrp = string.Empty;
                    productCustomer.IndDes = string.Empty;
                }

                //productCustomer.CustDeliveryTime = customer.CustDeliveryTime;
                productCustomer.Zone = customer.Zone;
                productCustomer.Route = customer.Route;
                productCustomer.CustReq = masterDataTransactionModel.MasterData.CustComment;
                productCustomer.MaterialComment = masterDataTransactionModel.MasterData.MaterialComment;
                productCustomer.PalletOverhang = Convert.ToInt32(customer.PalletOverhang);
                int? priorityFlag = null;
                if (customer.PriorityFlag != null)
                {
                    priorityFlag = Convert.ToInt32(customer.PriorityFlag.Value);
                }

                productCustomer.PriorityFlag = priorityFlag;

                if ((masterDataTransactionModel.TransactionAction == "Copy" || masterDataTransactionModel.TransactionAction == "CopyOs" || masterDataTransactionModel.TransactionAction == "Detail") && !String.IsNullOrEmpty(masterDataTransactionModel.MasterData.MaterialNo))
                {
                    productCustomer.QualitySpecs = new List<QualitySpec>();
                    var existQualitySpecs = JsonConvert.DeserializeObject<List<QualitySpec>>(_qualitySpecAPIRepository.GetQualitySpecByMaterialNo(_factoryCode, masterDataTransactionModel.MasterData.MaterialNo, _token));
                    if (existQualitySpecs != null)
                    {
                        foreach (var existQualitySpec in existQualitySpecs)
                        {
                            var matNo = string.Empty;

                            if (masterDataTransactionModel.TransactionAction == "Detail")
                            {
                                matNo = masterDataTransactionModel.MasterData.MaterialNo;
                            }

                            productCustomer.QualitySpecs.Add(new QualitySpec
                            {
                                FactoryCode = _factoryCode,
                                MaterialNo = !string.IsNullOrEmpty(matNo) ? matNo : null,
                                Name = existQualitySpec.Name,
                                Unit = existQualitySpec.Unit,
                                Value = existQualitySpec.Value
                            });
                        }
                    }
                }

                if (custShipToList != null && custShipToList.Count > 0)
                {
                    productCustomer.CustShipTo = String.Join(",", custShipToList.Select(s => s.ShipTo)); ;
                }

                productCustomer.NoTagBundle = masterDataTransactionModel.MasterData.NoTagBundle;
                productCustomer.TagBundle = masterDataTransactionModel.MasterData.TagBundle;
                productCustomer.TagPallet = masterDataTransactionModel.MasterData.TagPallet;
                productCustomer.HeadTagBundle = masterDataTransactionModel.MasterData.HeadTagBundle;
                productCustomer.FootTagBundle = masterDataTransactionModel.MasterData.FootTagBundle;
                productCustomer.HeadTagPallet = masterDataTransactionModel.MasterData.HeadTagPallet;
                productCustomer.FootTagPallet = masterDataTransactionModel.MasterData.FootTagPallet;

                productCustomer.Freetext1TagBundle = masterDataTransactionModel.MasterData.Freetext1TagBundle;
                productCustomer.Freetext2TagBundle = masterDataTransactionModel.MasterData.Freetext2TagBundle;
                productCustomer.Freetext3TagBundle = masterDataTransactionModel.MasterData.Freetext3TagBundle;

                productCustomer.Freetext1TagPallet = masterDataTransactionModel.MasterData.Freetext1TagPallet;
                productCustomer.Freetext2TagPallet = masterDataTransactionModel.MasterData.Freetext2TagPallet;
                productCustomer.Freetext3TagPallet = masterDataTransactionModel.MasterData.Freetext3TagPallet;

                //tassanai 19012022
                productCustomer.COA = customer.Coa;
                productCustomer.Film = customer.Film;
            }

            return productCustomer;
        }

        public RoutingViewModel BindRoutingData(MasterDataTransactionModel obj, ProductSpecViewModel modelProductSpec, TransactionDataModel trans)
        {
            RoutingViewModel model = new RoutingViewModel();
            model.RoutingDataList = new List<RoutingDataModel>();

            if (modelProductSpec != null)
            {
                model.WeightSheetDefault = Math.Round(Convert.ToDecimal(modelProductSpec.WeightSh), 3).ToString();

                model.WeightSelectList = new SelectList(new List<SelectListItem> {
                new SelectListItem { Selected = false, Text = Math.Round(Convert.ToDecimal(modelProductSpec.WeightSh), 3).ToString(), Value = Math.Round(Convert.ToDecimal(modelProductSpec.WeightSh), 2).ToString() },
                new SelectListItem { Selected = false, Text = Math.Round(Convert.ToDecimal(modelProductSpec.WeightBox), 3).ToString(), Value = Math.Round(Convert.ToDecimal(modelProductSpec.WeightBox), 2).ToString() },
            }, "Value", "Text", 1);

                if (trans.modelCategories.Id_SU == 6)
                {
                    var sum = trans.modelProductSpec.WeightBox * trans.modelProductProp.PiecePatch;
                    model.WeightSelectList = new SelectList(new List<SelectListItem> {
                new SelectListItem { Selected = false, Text = Math.Round(Convert.ToDecimal(trans.modelProductSpec.WeightSh), 3).ToString(), Value = Math.Round(Convert.ToDecimal(trans.modelProductSpec.WeightSh), 3).ToString() },
                new SelectListItem { Selected = false, Text = Math.Round(Convert.ToDecimal(trans.modelProductSpec.WeightBox), 3).ToString(), Value = Math.Round(Convert.ToDecimal(trans.modelProductSpec.WeightBox), 3).ToString() },
                new SelectListItem { Selected = false, Text = Math.Round(Convert.ToDecimal(sum), 3).ToString(), Value = Math.Round(Convert.ToDecimal(sum), 3).ToString() },
                }, "Value", "Text", 1);
                }
                else
                {
                    model.WeightSelectList = new SelectList(new List<SelectListItem> {
                new SelectListItem { Selected = false, Text = Math.Round(Convert.ToDecimal(trans.modelProductSpec.WeightSh), 3).ToString(), Value = Math.Round(Convert.ToDecimal(trans.modelProductSpec.WeightSh), 3).ToString() },
                new SelectListItem { Selected = false, Text = Math.Round(Convert.ToDecimal(trans.modelProductSpec.WeightBox), 3).ToString(), Value = Math.Round(Convert.ToDecimal(trans.modelProductSpec.WeightBox), 3).ToString() },
                }, "Value", "Text", 1);
                }

                model.SheetLengthIn = modelProductSpec.CutSheetLeng.ToString();
                model.SheetLengthOut = modelProductSpec.CutSheetLeng.ToString();
                model.SheetWidthIn = modelProductSpec.CutSheetWid.ToString();
                model.SheetWidthOut = modelProductSpec.CutSheetWid.ToString();
            }

            model.NoOpenIn = "1";//item.NoOpenIn.ToString();
            model.NoOpenOut = "1"; //item.NoOpenOut.ToString();
            model.WeightIn = model.WeightSheetDefault;
            model.WeightOut = model.WeightSheetDefault;
            model.MachineGroupSelectList = JsonConvert.DeserializeObject<List<GroupMachineModels>>(_machineGroupAPIRepository.GetByMachineGroupJoinMachine(_factoryCode, _token)).Select(sli => new SelectListItem { Value = sli.Id, Text = sli.GroupMachine });
            //  model.MachineGroupSelectList = JsonConvert.DeserializeObject<List<Machine>>(_machineAPIRepository.GetMachineList(_factoryCode)).OrderBy(o => o.MachineGroup).Select(s => s.MachineGroup).Distinct().Select(sli => new SelectListItem { Value = sli, Text = sli });
            //model.InkSelectList = JsonConvert.DeserializeObject<List<Color>>(_colorAPIRepository.GetColorList(_factoryCode)).OrderBy(o => o.Ink).Select(s => s.Ink).Distinct().Select(sli => new SelectListItem { Value = sli, Text = sli });
            //model.InkSelectList = JsonConvert.DeserializeObject<List<Color>>(_colorAPIRepository.GetColorList(_factoryCode)).OrderBy(o => o.Shade).Select(s => s.Shade).Distinct().Select(sli => new SelectListItem { Value = sli, Text = sli });
            model.InkSelectList = JsonConvert.DeserializeObject<List<Color>>(_colorAPIRepository.GetColorList(_factoryCode, _token)).OrderBy(o => o.Ink).Select(s => s.Ink).Distinct().Select(sli => new SelectListItem { Value = sli, Text = sli });
            model.ShadeSelectList = JsonConvert.DeserializeObject<List<Color>>(_colorAPIRepository.GetColorList(_factoryCode, _token)).OrderBy(o => o.Shade).Select(s => s.Shade).Distinct().Select(sli => new SelectListItem { Value = sli, Text = sli });

            model.ScoreTypelist = JsonConvert.DeserializeObject<List<ScoreType>>(_scoreTypeAPIRepository.GetScoreTypeList(_factoryCode, _token)).Select(s => new { s.ScoreTypeId, s.ScoreTypeName }).Distinct().Select(sli => new SelectListItem { Value = sli.ScoreTypeId.ToString(), Text = sli.ScoreTypeName });
            model.ScoreGapList = JsonConvert.DeserializeObject<List<ScoreGap>>(_scoreGapAPIRepository.GetScoreGapsByFactoryCode(_factoryCode, _token));

            int i = 0;
            List<RoutingDataModel> RoutingDataModel = new List<RoutingDataModel>();
            foreach (var item in obj.Routings.OrderBy(x => x.SeqNo).ToList())
            {
                i++;
                if (i == obj.Routings.Count())
                {
                    model.NoOpenIn = item.NoOpenIn.ToString();
                    model.NoOpenOut = item.NoOpenOut.ToString();
                    model.SheetLengthIn = item.SheetInLeg.ToString();//modelProductSpec.CutSheetLeng.ToString();
                    model.SheetLengthOut = item.SheetOutLeg.ToString();//modelProductSpec.CutSheetLeng.ToString();
                    model.SheetWidthIn = item.SheetInWid.ToString();//modelProductSpec.CutSheetWid.ToString();
                    model.SheetWidthOut = item.SheetOutWid.ToString();// modelProductSpec.CutSheetWid.ToString();
                    model.WeightIn = (Convert.ToDouble(item.WeightIn)).ToString("N3");
                    model.WeightOut = (Convert.ToDouble(item.WeightOut)).ToString("N3");
                    model.CustBarcodeNo = item.CustBarcodeNo;
                }

                if (!string.IsNullOrEmpty(item.Machine) && !string.IsNullOrWhiteSpace(item.Machine))
                {
                    //var machineGroup = JsonConvert.DeserializeObject<Machine>(_machineAPIRepository.GetMachineGroupByMachine(_factoryCode, item.Machine)).MachineGroup;
                    var isPropCor = JsonConvert.DeserializeObject<List<Machine>>(_machineAPIRepository.GetMachineList(_factoryCode, _token)).Where(w => w.Machine1 == item.Machine).Select(s => s.IsPropCor).FirstOrDefault();
                    var isPropPrint = JsonConvert.DeserializeObject<List<Machine>>(_machineAPIRepository.GetMachineList(_factoryCode, _token)).Where(w => w.Machine1 == item.Machine).Select(s => s.IsPropPrint).FirstOrDefault();
                    var isPropDieCut = JsonConvert.DeserializeObject<List<Machine>>(_machineAPIRepository.GetMachineList(_factoryCode, _token)).Where(w => w.Machine1 == item.Machine).Select(s => s.IsPropDieCut).FirstOrDefault();

                    RoutingDataModel tmp = new RoutingDataModel();
                    tmp.id = item.Id;
                    tmp.SeqNo = item.SeqNo;
                    tmp.GroupMachine = null;
                    tmp.Machine = item.Machine;
                    tmp.MachineMove = item.McMove.Value;
                    tmp.Alternative1 = item.Alternative1;
                    tmp.Alternative2 = item.Alternative2;
                    tmp.Alternative3 = item.Alternative3;
                    tmp.Alternative4 = item.Alternative4;
                    tmp.Alternative5 = item.Alternative5;
                    tmp.Alternative6 = item.Alternative6;
                    tmp.Alternative7 = item.Alternative7;
                    tmp.Alternative8 = item.Alternative8;
                    tmp.NoOpenIn = item.NoOpenIn.ToString() == "0" || string.IsNullOrEmpty(item.NoOpenIn.ToString()) ? "1" : item.NoOpenIn.ToString();
                    tmp.NoOpenOut = item.NoOpenOut.ToString() == "0" || string.IsNullOrEmpty(item.NoOpenOut.ToString()) ? "1" : item.NoOpenOut.ToString();
                    tmp.WeightIn = item.WeightIn.HasValue ? (Convert.ToDouble(item.WeightIn)).ToString("N3") : null;
                    tmp.WeightOut = item.WeightOut.HasValue ? (Convert.ToDouble(item.WeightOut)).ToString("N3") : null;
                    tmp.SheetLengthIn = item.SheetInLeg.HasValue ? item.SheetInLeg.ToString() : null;
                    tmp.SheetLengthOut = item.SheetOutLeg.HasValue ? item.SheetOutLeg.ToString() : null;
                    tmp.SheetWidthIn = item.SheetInWid.HasValue ? item.SheetInWid.ToString() : null;
                    tmp.SheetWidthOut = item.SheetOutWid.HasValue ? item.SheetOutWid.ToString() : null;
                    tmp.Coat = item.Coating.ToString();
                    tmp.PaperRollWidth = item.PaperWidth.ToString();
                    tmp.Cut = item.CutNo.ToString();
                    tmp.Trim = item.Trim.ToString();
                    tmp.PercentTrim = item.PercenTrim.ToString();
                    tmp.TearTape = item.TearTape;
                    tmp.LineQtyPerBox = Convert.ToInt16(item.TearTapeQty);
                    tmp.MarginForPaper = item.TearTapeDistance;

                    // Tassanai 31032020
                    item.CustBarcodeNo = item.CustBarcodeNo;
                    tmp.Ink1 = item.Color1;
                    tmp.Ink2 = item.Color2;
                    tmp.Ink3 = item.Color3;
                    tmp.Ink4 = item.Color4;
                    tmp.Ink5 = item.Color5;
                    tmp.Ink6 = item.Color6;
                    tmp.Ink7 = item.Color7;
                    tmp.Shade1 = item.Shade1;
                    tmp.Shade2 = item.Shade2;
                    tmp.Shade3 = item.Shade3;
                    tmp.Shade4 = item.Shade4;
                    tmp.Shade5 = item.Shade5;
                    tmp.Shade6 = item.Shade6;
                    tmp.Shade7 = item.Shade7;
                    tmp.Area1 = item.ColorArea1.ToString();
                    tmp.Area2 = item.ColorArea2.ToString();
                    tmp.Area3 = item.ColorArea3.ToString();
                    tmp.Area4 = item.ColorArea4.ToString();
                    tmp.Area5 = item.ColorArea5.ToString();
                    tmp.Area6 = item.ColorArea6.ToString();
                    tmp.Area7 = item.ColorArea7.ToString();
                    tmp.PrintingPlateNo = item.BlockNo;
                    tmp.CuttingDieNo = item.PlateNo;
                    tmp.MylaNo = item.MylaNo;
                    string plate_type = null;
                    if (item.NoneBlk == true)
                    {
                        plate_type = "Non-Print";
                    }
                    else if (item.StanBlk == true)
                    {
                        plate_type = "Standard";
                    }
                    else if (item.SemiBlk == true)
                    {
                        plate_type = "Semi";
                    }
                    else if (item.ShipBlk == true)
                    {
                        plate_type = "Shipping Mark";
                    }
                    tmp.PrintingPlateType = plate_type;
                    tmp.JoinToMaterialNo = item.JoinMatNo;
                    tmp.SperateToMaterialNo = item.SeparatMatNo;
                    tmp.Remark = item.RemarkInprocess;
                    //tmp.RemarkImageFile = null;
                    //tmp.RemarkImageFileName = null;
                    //tmp.RemarkAttachFileStatus = null;
                    //tmp.MachineGroupSelect = null;
                    //tmp.CopyStatus
                    //tmp.IsPropCor
                    //tmp.IsPropPrint
                    //tmp.IsPropDieCut
                    tmp.BlockNo2 = item.BlockNo2;
                    tmp.BlockNoPlant2 = item.BlockNoPlant2;
                    tmp.BlockNo3 = item.BlockNo3;
                    tmp.BlockNoPlant3 = item.BlockNoPlant3;
                    tmp.BlockNo4 = item.BlockNo4;
                    tmp.BlockNoPlant4 = item.BlockNoPlant4;
                    tmp.BlockNo5 = item.BlockNo5;
                    tmp.BlockNoPlant5 = item.BlockNoPlant5;
                    tmp.PlateNo2 = item.PlateNo2;
                    tmp.PlateNoPlant2 = item.PlateNoPlant2;
                    tmp.MylaNo2 = item.MylaNo2;
                    tmp.MylaNoPlant2 = item.MylaNoPlant2;
                    tmp.PlateNo3 = item.PlateNo3;
                    tmp.PlateNoPlant3 = item.PlateNoPlant3;
                    tmp.MylaNo3 = item.MylaNo3;
                    tmp.MylaNoPlant3 = item.MylaNoPlant3;
                    tmp.PlateNo4 = item.PlateNo4;
                    tmp.PlateNoPlant4 = item.PlateNoPlant4;
                    tmp.MylaNo4 = item.MylaNo4;
                    tmp.MylaNoPlant4 = item.MylaNoPlant4;
                    tmp.PlateNo5 = item.PlateNo5;
                    tmp.PlateNoPlant5 = item.PlateNoPlant5;
                    tmp.MylaNo5 = item.MylaNo5;
                    tmp.MylaNoPlant5 = item.MylaNoPlant5;

                    if (isPropPrint != null && !isPropPrint.Value)
                    {
                        tmp.NoneBlk = false;
                        tmp.StanBlk = false;
                        tmp.SemiBlk = false;
                        tmp.ShipBlk = false;
                    }
                    else
                    {
                        tmp.NoneBlk = item.NoneBlk ?? false;
                        tmp.StanBlk = item.StanBlk ?? false;
                        tmp.SemiBlk = item.SemiBlk ?? false;
                        tmp.ShipBlk = item.ShipBlk ?? false;
                    }

                    //add save
                    tmp.Plant = item.Plant;
                    tmp.Plant_Code = item.PlanCode;
                    tmp.StdProcess = item.StdProcess ?? false;
                    tmp.HandHold = item.HandHold ?? false;
                    tmp.Platen = item.Platen ?? false;
                    tmp.Rotary = item.Rotary ?? false;
                    tmp.Hardship = Convert.ToInt16(item.Hardship);
                    tmp.UnUpgrad_Board = item.UnUpgradBoard ?? false;
                    tmp.Color_count = Convert.ToInt16(item.ColourCount);
                    tmp.Human = Convert.ToInt16(item.Human);

                    tmp.TearTapeQty = Convert.ToInt16(item.TearTapeQty);
                    tmp.TearTapeDistance = item.TearTapeDistance;

                    tmp.Speed = Convert.ToInt16(item.Speed);
                    tmp.SetupTm = Convert.ToInt16(item.SetupTm);
                    tmp.PrepareTm = Convert.ToInt16(item.PrepareTm);
                    tmp.PostTm = Convert.ToInt16(item.PostTm);
                    tmp.SetupWaste = Convert.ToInt16(item.SetupWaste);
                    tmp.RunWaste = Convert.ToInt16(item.RunWaste);

                    tmp.StackHeight = Convert.ToInt16(item.StackHeight);
                    tmp.RotateIn = item.RotateIn;
                    tmp.RotateOut = item.RotateOut;

                    tmp.IsPropCor = isPropCor;
                    tmp.IsPropPrint = isPropPrint;
                    tmp.IsPropDieCut = isPropDieCut;

                    tmp.MylaSize = item.MylaSize;
                    tmp.RepeatLength = Convert.ToInt16(item.RepeatLength);
                    tmp.CustBarcodeNo = item.CustBarcodeNo;

                    tmp.ScoreGap = item.ScoreGap == null ? 0 : Convert.ToDouble(item.ScoreGap);
                    tmp.ScoreType = item.ScoreType;

                    RoutingDataModel.Add(tmp);
                }
            }

            var seqNo = 1;
            //Sort Seq. number of routing
            foreach (var routing in RoutingDataModel)
            {
                routing.SeqNo = seqNo;
                seqNo++;
            }

            SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "RoutingDataModels", RoutingDataModel);

            model.RoutingDataList = RoutingDataModel.OrderBy(x => x.SeqNo).ToList();

            return model;
        }

        public ViewCategories BindCategoriesData(MasterDataTransactionModel obj)
        {
            TransactionDataModel transactionDataModel = new TransactionDataModel();
            transactionDataModel.modelCategories = new ViewCategories();
            GetCategoriesListData(ref transactionDataModel);

            if (obj.TransactionAction.Contains("Presale"))
            {
                //var materialType = transactionDataModel.modelCategories.MaterialTypeList.FirstOrDefault(m => m.Id == transactionDataModel.modelCategories.Id_MatType);
                transactionDataModel.modelCategories.Id_kProdGrp = obj.TransactionsDetail.IdKindOfProductGroup.Value;
                transactionDataModel.modelCategories.Id_ProcCost = obj.TransactionsDetail.IdProcessCost.Value;
                if (!string.IsNullOrEmpty(obj?.PresaleMasterData?.ProcessCost))
                {
                    var ProcessCost = transactionDataModel.modelCategories.ProcessCostList.FirstOrDefault(p => p.Name.Trim().ToUpper() == (obj.PresaleMasterData.ProcessCost.Trim().ToUpper()));
                    transactionDataModel.modelCategories.Id_ProcCost = ProcessCost?.Id != null ? ProcessCost.Id : 0;
                }
                transactionDataModel.modelCategories.Id_kProd = obj.TransactionsDetail.IdKindOfProduct.Value;
                transactionDataModel.modelCategories.Id_ProdType = obj.TransactionsDetail.IdProductType.Value;
                transactionDataModel.modelCategories.Id_MatType = obj.TransactionsDetail.IdMaterialType.Value == 0 && !string.IsNullOrEmpty(obj.MasterData.MaterialType) ? transactionDataModel.modelCategories.MaterialTypeList.Where(m => m.MatCode == obj.MasterData.MaterialType).FirstOrDefault().Id : obj.TransactionsDetail.IdMaterialType.Value;
                transactionDataModel.modelCategories.Id_PU = obj.TransactionsDetail.IdProductUnit.Value;
                transactionDataModel.modelCategories.Id_SU = obj.TransactionsDetail.IdSaleUnit.Value;

                transactionDataModel.modelCategories.KpgName = transactionDataModel.modelCategories.Id_kProdGrp == 0 ? null : transactionDataModel.modelCategories.KindOfProductGroupList.FirstOrDefault(k => k.Id == transactionDataModel.modelCategories.Id_kProdGrp).Name;
                transactionDataModel.modelCategories.MatCode = transactionDataModel.modelCategories.Id_MatType == 0 || obj.MasterData.MaterialType == null ? "81" : obj.MasterData.MaterialType;
                transactionDataModel.modelCategories.ProductTypeName = transactionDataModel.modelCategories.Id_ProdType == 0 ? null : transactionDataModel.modelCategories.ProductTypeList.FirstOrDefault(p => p.Id == transactionDataModel.modelCategories.Id_ProdType).Name;
                transactionDataModel.modelCategories.HierarchyLV2 = transactionDataModel.modelCategories.Id_ProdType == 0 ? null : transactionDataModel.modelCategories.HierarchyLV2List.FirstOrDefault(h => h.IdProductType == transactionDataModel.modelCategories.Id_ProdType).HierarchyLv2Code;
                transactionDataModel.modelCategories.FormGroup = transactionDataModel.modelCategories.Id_ProdType == 0 ? null : transactionDataModel.modelCategories.ProductTypeList.FirstOrDefault(p => p.Id == transactionDataModel.modelCategories.Id_ProdType).FormGroup;
                transactionDataModel.modelCategories.IsTwoPiece = transactionDataModel.modelCategories.Id_ProdType == 0 ? null : transactionDataModel.modelCategories.ProductTypeList.FirstOrDefault(p => p.Id == transactionDataModel.modelCategories.Id_ProdType).IsTwoPiece;
                transactionDataModel.modelCategories.BoxHandle = transactionDataModel.modelCategories.Id_ProdType == 0 ? false : transactionDataModel.modelCategories.ProductTypeList.FirstOrDefault(p => p.Id == transactionDataModel.modelCategories.Id_ProdType).BoxHandle;
            }
            else
            {
                transactionDataModel.modelCategories.Id_kProdGrp = obj.TransactionsDetail.IdKindOfProductGroup.Value;
                transactionDataModel.modelCategories.Id_ProcCost = obj.TransactionsDetail.IdProcessCost.Value;
                transactionDataModel.modelCategories.Id_kProd = obj.TransactionsDetail.IdKindOfProduct.Value;
                transactionDataModel.modelCategories.Id_ProdType = obj.TransactionsDetail.IdProductType.Value;
                transactionDataModel.modelCategories.Id_MatType = obj.TransactionsDetail.IdMaterialType.Value;
                transactionDataModel.modelCategories.Id_PU = obj.TransactionsDetail.IdProductUnit.Value;
                transactionDataModel.modelCategories.Id_SU = obj.TransactionsDetail.IdSaleUnit.Value;

                transactionDataModel.modelCategories.KpgName = transactionDataModel.modelCategories.Id_kProdGrp == 0 ? null : transactionDataModel.modelCategories.KindOfProductGroupList.FirstOrDefault(k => k.Id == transactionDataModel.modelCategories.Id_kProdGrp).Name;
                transactionDataModel.modelCategories.MatCode = transactionDataModel.modelCategories.Id_MatType == 0 ? "81" : transactionDataModel.modelCategories.MaterialTypeList.FirstOrDefault(m => m.Id == transactionDataModel.modelCategories.Id_MatType).MatCode;
                transactionDataModel.modelCategories.ProductTypeName = transactionDataModel.modelCategories.Id_ProdType == 0 ? null : transactionDataModel.modelCategories.ProductTypeList.FirstOrDefault(p => p.Id == transactionDataModel.modelCategories.Id_ProdType).Name;
                transactionDataModel.modelCategories.HierarchyLV2 = transactionDataModel.modelCategories.Id_ProdType == 0 ? null : transactionDataModel.modelCategories.HierarchyLV2List.FirstOrDefault(h => h.IdProductType == transactionDataModel.modelCategories.Id_ProdType).HierarchyLv2Code;
                transactionDataModel.modelCategories.FormGroup = transactionDataModel.modelCategories.Id_ProdType == 0 ? null : transactionDataModel.modelCategories.ProductTypeList.FirstOrDefault(p => p.Id == transactionDataModel.modelCategories.Id_ProdType).FormGroup;
                transactionDataModel.modelCategories.IsTwoPiece = transactionDataModel.modelCategories.Id_ProdType == 0 ? null : transactionDataModel.modelCategories.ProductTypeList.FirstOrDefault(p => p.Id == transactionDataModel.modelCategories.Id_ProdType).IsTwoPiece;
                transactionDataModel.modelCategories.BoxHandle = transactionDataModel.modelCategories.Id_ProdType == 0 ? false : transactionDataModel.modelCategories.ProductTypeList.FirstOrDefault(p => p.Id == transactionDataModel.modelCategories.Id_ProdType).BoxHandle;
            }

            if (!string.IsNullOrEmpty(obj.MasterData.Hierarchy) && obj.MasterData.Hierarchy.Length > 4)
            {
                transactionDataModel.modelCategories.HierarchyLV3 = obj.MasterData.Hierarchy.Substring(4, 3);
            }
            else
            {
                transactionDataModel.modelCategories.HierarchyLV3 = "";
            }

            var hierarchyLv4 = string.IsNullOrEmpty(obj.TransactionsDetail.HierarchyLv4) && !string.IsNullOrEmpty(obj.MasterData.Hierarchy) && obj.MasterData.Hierarchy.Length >= 10 ? obj.MasterData.Hierarchy.Substring(7, 3) : obj.TransactionsDetail.HierarchyLv4;
            transactionDataModel.modelCategories.HierarchyLV4 = hierarchyLv4;

            if (!string.IsNullOrEmpty(obj.MasterData.RscStyle) || !string.IsNullOrWhiteSpace(obj.MasterData.RscStyle))
            {
                transactionDataModel.modelCategories.RSCStyle = obj.MasterData.RscStyle;
                transactionDataModel.modelCategories.RSCStyleId = SetRSCId(obj.MasterData.RscStyle);
            }

            if (_businessGroup)
            {
                transactionDataModel.modelCategories.fscCode = obj.MasterData.FscCode;
                transactionDataModel.modelCategories.fscFgCode = obj.MasterData.FscFgCode;
                transactionDataModel.modelCategories.RpacLob = obj.MasterData.RpacLob;
                transactionDataModel.modelCategories.RpacProgram = obj.MasterData.RpacProgram;
                transactionDataModel.modelCategories.RpacBrand = obj.MasterData.RpacBrand;
                transactionDataModel.modelCategories.RpacPackagingType = obj.MasterData.RpacPackagingType;
            }
            //transactionDataModel.modelCategories.KpgName = JsonConvert.DeserializeObject<KindOfProductGroup>(_kindOfProductGroupAPIRepository.GetKindOfProductGroupById(_factoryCode, transactionDataModel.modelCategories.Id_kProdGrp)).Name;
            //transactionDataModel.modelCategories.MatCode = JsonConvert.DeserializeObject<MaterialType>(_materialTypeAPIRepository.GetMaterialCode(_saleOrg, transactionDataModel.modelCategories.Id_MatType)).MatCode;
            //transactionDataModel.modelCategories.ProductTypeName = JsonConvert.DeserializeObject<ProductType>(_productTypeAPIRepository.GetProductTypeById(_factoryCode, transactionDataModel.modelCategories.Id_ProdType)).Name;
            //transactionDataModel.modelCategories.HierarchyLV2 = JsonConvert.DeserializeObject<ProductType>(_productTypeAPIRepository.GenLV2(_factoryCode, transactionDataModel.modelCategories.Id_ProdType)).HierarchyLv2;

            return transactionDataModel.modelCategories;
        }

        //public ViewCategories BindCategoriesPresaleData(string MaterialNo)
        //{
        //    ViewCategories model = new ViewCategories();
        //    var cat = CategoriesRepository.GetCategories(dbContext, MaterialNo);

        //    //model.Id_kProdGrp = cat.IDkpg;
        //    //model.Id_ProcCost = cat.IDpcc;
        //    //model.Id_kProd = cat.IDkop;
        //    //model.Id_ProdType = cat.IDpdt;
        //    //model.Id_MatType = cat.IDmtt;
        //    //model.Id_PU = cat.IDpu;
        //    //model.Id_SU = cat.IDsu;

        //    model.Id_kProdGrp = 0;
        //    model.Id_ProcCost = 0;
        //    model.Id_kProd = 0;
        //    model.Id_ProdType = 70;
        //    model.Id_MatType = 10;
        //    model.Id_PU = 1;
        //    model.Id_SU = 1;
        //    model.PCCList = CategoriesRepository.GenPCCWherePDT(model.Id_ProdType).Select(s => new SelectListItem() { Value = s.Uid.ToString(), Text = s.Name });
        //    model.KOPList = CategoriesRepository.GenKOPWherePDT(model.Id_ProdType).Select(s => new SelectListItem() { Value = s.Uid.ToString(), Text = s.Name });
        //    model.PDTList = CategoriesRepository.GenPDTNoWhere().Select(s => new SelectListItem() { Value = s.Uid.ToString(), Text = s.Name });

        //    List<MaterialType> materialTypeList = JsonConvert.DeserializeObject<List<MaterialType>>(_materialTypeAPIRepository.GetMaterialTypeList(_factoryCode));
        //    model.MATList = materialTypeList.Select(s => new SelectListItem() { Value = s.MatCode, Text = s.Description });
        //    List<UnitMaterial> UnitMaterialList = JsonConvert.DeserializeObject<List<UnitMaterial>>(_unitMaterialAPIRepository.GetUnitMaterialList(_factoryCode));
        //    model.PUList = UnitMaterialList.Select(s => new SelectListItem() { Value = s.Id.ToString(), Text = s.Name });
        //    model.SUList = UnitMaterialList.Select(s => new SelectListItem() { Value = s.Id.ToString(), Text = s.Name });

        //    model.KpgName = JsonConvert.DeserializeObject<KindOfProductGroup>(_kindOfProductGroupAPIRepository.GetKindOfProductGroupById(_factoryCode, model.Id_kProdGrp)).Name;
        //    model.MatCode = "81";
        //    model.ProductTypeName = JsonConvert.DeserializeObject<ProductType>(_productTypeAPIRepository.GetProductTypeById(_factoryCode, model.Id_ProdType)).Name;
        //    model.HierarchyLV2 = JsonConvert.DeserializeObject<ProductType>(_productTypeAPIRepository.GenLV2(_factoryCode, model.Id_ProdType)).HierarchyLv2;
        //    return model;
        //}

        public ProductSpecViewModel BindProductSpecData(MasterDataTransactionModel obj, IHostingEnvironment hostEnvironment)
        {
            ProductSpecViewModel model = new ProductSpecViewModel();
            model.SAPStatus = obj.MasterData.SapStatus;

            //var LV2 = obj.TransactionsDetail.IdProductType.Value == 0 ? "" : JsonConvert.DeserializeObject<ProductType>(_productTypeAPIRepository.GetProductTypeById(_factoryCode, obj.TransactionsDetail.IdProductType.Value, _token)).HierarchyLv2;
            var LV2 = obj.MasterData.Hierarchy.Length > 2 ? obj.MasterData.Hierarchy.Substring(2, 2) : JsonConvert.DeserializeObject<ProductType>(_productTypeAPIRepository.GetProductTypeById(_factoryCode, obj.TransactionsDetail.IdProductType.Value, _token)).HierarchyLv2;
            var LV3 = obj.MasterData.Hierarchy.Length > 4 ? obj.MasterData.Hierarchy.Substring(4, 3) : "";
            var LV4 = obj.MasterData.Hierarchy.Length > 7 ? obj.MasterData.Hierarchy.Substring(7, 3) : obj.TransactionsDetail.HierarchyLv4;

            if (!string.IsNullOrEmpty(obj.MasterData.Hierarchy))
            {
                var mapcost = JsonConvert.DeserializeObject<MapCost>(_mapCostAPIRepository.GetCostField(_factoryCode, LV2, LV3, LV4, _token));
                model.costField = mapcost != null ? mapcost.CostField : "";
            }
            else
            {
                model.costField = "";
            }

            model.Code = obj.MasterData.Code;
            model.Board = obj.MasterData.Board;
            model.Flute = obj.MasterData.Flute;
            model.Hierarchy = obj.MasterData.Hierarchy;
            if (!string.IsNullOrEmpty(obj.MasterData.FactoryCode) && !string.IsNullOrEmpty(obj.MasterData.MaterialNo))
            {
                model.Coating = JsonConvert.DeserializeObject<List<Coating>>(_coatingAPIRepository.GetCoatingByMaterialNo(obj.MasterData.FactoryCode, obj.MasterData.MaterialNo, _token));
            }

            if (!string.IsNullOrEmpty(obj.MasterData.Code))
            {
                var Board = JsonConvert.DeserializeObject<BoardCombine>(_boardCombineAPIRepository.GetBoardByCode(obj.MasterData.FactoryCode, obj.MasterData.Code, _token));
                if (Board != null)
                {
                    model.BoardKIWI = Board.Kiwi == null || Board.Kiwi == "" ? GetBoardKIWI(Board.BoardCombine1) : Board.Kiwi;
                    model.Weight = Convert.ToDouble(GetBasisWeight(obj.MasterData.Code, Board.Flute, _factoryCode));
                }
                else
                {
                    model.Weight = 0;
                }
            }

            if (obj.PlantView != null)
            {
                if (obj.PlantView.Id != 0 && (obj.PlantView.StdMovingCost != 0 || obj.PlantView.StdTotalCost != 0))
                {
                    if (obj.MasterData.MaterialType == "82")
                    {
                        model.CostPerTon = obj.PlantView.StdMovingCost == null ? 0 : Convert.ToDecimal(obj.PlantView.StdMovingCost);
                    }
                    else
                    {
                        model.CostPerTon = obj.PlantView.StdTotalCost == null ? 0 : Convert.ToDecimal(obj.PlantView.StdTotalCost);
                    }
                }
                else if (model.costField != "" && model.Code != null)
                {
                    var cost = JsonConvert.DeserializeObject<Cost>(_boardCombineAccAPIRepository.GetCost(_factoryCode, model.Code, model.costField, _token));
                    if (cost != null)
                    {
                        model.CostPerTon = cost.CostPerTon == null ? 0 : Math.Round(Convert.ToDecimal(cost.CostPerTon), 2);
                    }
                }
            }

            model.BoardAlt = obj.BoardAltViewModels;
            string[] ArrBoard;
            foreach (var item in model.BoardAlt)
            {
                item.BoardKiwi = GetBoardKIWI(item.Flute + "/" + item.BoardName);
                ArrBoard = item.BoardName.Split("/");
                if (ArrBoard.Count() > 0)
                    item.GL = ArrBoard[0];
                if (ArrBoard.Count() > 1)
                    item.BM = ArrBoard[1];
                if (ArrBoard.Count() > 2)
                    item.BL = ArrBoard[2];
                if (ArrBoard.Count() > 3)
                    item.CM = ArrBoard[3];
                if (ArrBoard.Count() > 4)
                    item.CL = ArrBoard[4];
                if (ArrBoard.Count() > 5)
                    item.DM = ArrBoard[5];
                if (ArrBoard.Count() > 6)
                    item.DL = ArrBoard[6];
            }
            model.Priority = model.BoardAlt.Count + 1;

            model.Leg = obj.MasterData.Leg;
            model.Wid = obj.MasterData.Wid;
            model.Hig = obj.MasterData.Hig;
            model.Height = obj.MasterData.Hig;

            model.IsWrap = obj.TransactionsDetail.IsWrap;
            model.IsNotch = obj.TransactionsDetail.IsNotch;
            model.NotchArea = obj.TransactionsDetail.NotchArea;
            model.NotchSide = obj.TransactionsDetail.NotchSide;

            if (obj.TransactionsDetail.NotchDegree == 30 || obj.TransactionsDetail.NotchDegree == 60 || obj.TransactionsDetail.NotchDegree == 90)
            {
                model.NotchDegree = obj.TransactionsDetail.NotchDegree;
            }
            else
            {
                model.NotchDegree = 0;
                model.NotchDegreex = obj.TransactionsDetail.NotchDegree;
            }
            model.CGType = obj.TransactionsDetail.Cgtype;
            model.SideA = obj.TransactionsDetail.SideA;
            model.SideB = obj.TransactionsDetail.SideB;
            model.SideC = obj.TransactionsDetail.SideC;
            model.SideD = obj.TransactionsDetail.SideD == null ? 0 : obj.TransactionsDetail.SideD;

            model.GLWid = obj.TransactionsDetail.Glwid;
            model.GLTail = obj.TransactionsDetail.Gltail;
            model.TwoPiece = obj.MasterData.TwoPiece;
            model.UnUpgradBoard = obj.MasterData.UnUpgradBoard;
            model.JointLap = obj.MasterData.JointLap == null ? 0 : obj.MasterData.JointLap;
            model.JoinSize = obj.MasterData.JointLap == null ? 0 : obj.MasterData.JointLap;
            model.ScoreL2 = obj.MasterData.ScoreL2 == null ? 0 : obj.MasterData.ScoreL2;
            model.ScoreL3 = obj.MasterData.ScoreL3 == null ? 0 : obj.MasterData.ScoreL3;
            model.ScoreL4 = obj.MasterData.ScoreL4 == null ? 0 : obj.MasterData.ScoreL4;
            model.ScoreL5 = obj.MasterData.ScoreL5 == null ? 0 : obj.MasterData.ScoreL5;
            model.Slit = obj.MasterData.Slit == null ? 6 : obj.MasterData.Slit;
            model.No_Slot = obj.MasterData.NoSlot == null ? 0 : obj.MasterData.NoSlot;

            model.ScoreL8 = obj.MasterData.ScoreL8 == null ? model.ScoreL2 + model.ScoreL3 + model.JointLap : obj.MasterData.ScoreL8;
            model.ScoreL9 = obj.MasterData.ScoreL9 == null ? model.ScoreL4 + model.ScoreL5 + model.Slit : obj.MasterData.ScoreL9;
            model.ScoreL6 = obj.MasterData.ScoreL6 == null ? model.ScoreL2 + model.ScoreL3 : obj.MasterData.ScoreL6;
            model.ScoreL7 = obj.MasterData.ScoreL7 == null ? model.ScoreL4 + model.ScoreL5 : obj.MasterData.ScoreL7;

            model.ScoreW1 = obj.MasterData.ScoreW1 == null ? 0 : obj.MasterData.ScoreW1;
            model.Scorew2 = obj.MasterData.Scorew2 == null ? 0 : obj.MasterData.Scorew2;
            model.Scorew3 = obj.MasterData.Scorew3 == null ? 0 : obj.MasterData.Scorew3;
            model.Scorew4 = obj.MasterData.Scorew4 == null ? 0 : obj.MasterData.Scorew4;
            model.Scorew5 = obj.MasterData.Scorew5 == null ? 0 : obj.MasterData.Scorew5;
            model.Scorew6 = obj.MasterData.Scorew6 == null ? 0 : obj.MasterData.Scorew6;
            model.Scorew7 = obj.MasterData.Scorew7 == null ? 0 : obj.MasterData.Scorew7;
            model.Scorew8 = obj.MasterData.Scorew8 == null ? 0 : obj.MasterData.Scorew8;
            model.Scorew9 = obj.MasterData.Scorew9 == null ? 0 : obj.MasterData.Scorew9;
            model.Scorew10 = obj.MasterData.Scorew10 == null ? 0 : obj.MasterData.Scorew10;
            model.Scorew11 = obj.MasterData.Scorew11 == null ? 0 : obj.MasterData.Scorew11;
            model.Scorew12 = obj.MasterData.Scorew12 == null ? 0 : obj.MasterData.Scorew12;
            model.Scorew13 = obj.MasterData.Scorew13 == null ? 0 : obj.MasterData.Scorew13;
            model.Scorew14 = obj.MasterData.Scorew14 == null ? 0 : obj.MasterData.Scorew14;
            model.Scorew15 = obj.MasterData.Scorew15 == null ? 0 : obj.MasterData.Scorew15;
            model.Scorew16 = obj.MasterData.Scorew16 == null ? 0 : obj.MasterData.Scorew16;
            model.CutSheetWid = obj.MasterData.CutSheetWid == null ? model.ScoreW1 + model.Scorew2 + model.Scorew3 : obj.MasterData.CutSheetWid;
            model.CutSheetLeng = obj.MasterData.CutSheetLeng == null ? model.ScoreW1 + model.Scorew2 + model.Scorew3 : obj.MasterData.CutSheetLeng;
            //model.Scorew2 = obj.MasterData.RscStyle == "Sleeve" ? model.CutSheetWid : model.Scorew2;

            model.Perforate1 = obj.MasterData.Perforate1 == null ? 0 : obj.MasterData.Perforate1;
            model.Perforate2 = obj.MasterData.Perforate2 == null ? 0 : obj.MasterData.Perforate2;
            model.Perforate3 = obj.MasterData.Perforate3 == null ? 0 : obj.MasterData.Perforate3;
            model.Perforate4 = obj.MasterData.Perforate4 == null ? 0 : obj.MasterData.Perforate4;
            model.Perforate5 = obj.MasterData.Perforate5 == null ? 0 : obj.MasterData.Perforate5;
            model.Perforate6 = obj.MasterData.Perforate6 == null ? 0 : obj.MasterData.Perforate6;
            model.Perforate7 = obj.MasterData.Perforate7 == null ? 0 : obj.MasterData.Perforate7;
            model.Perforate8 = obj.MasterData.Perforate8 == null ? 0 : obj.MasterData.Perforate8;
            model.Perforate9 = obj.MasterData.Perforate9 == null ? 0 : obj.MasterData.Perforate9;
            model.Perforate10 = obj.MasterData.Perforate10 == null ? 0 : obj.MasterData.Perforate10;
            model.Perforate11 = obj.MasterData.Perforate11 == null ? 0 : obj.MasterData.Perforate11;
            model.Perforate12 = obj.MasterData.Perforate12 == null ? 0 : obj.MasterData.Perforate12;
            model.Perforate13 = obj.MasterData.Perforate13 == null ? 0 : obj.MasterData.Perforate13;
            model.Perforate14 = obj.MasterData.Perforate14 == null ? 0 : obj.MasterData.Perforate14;
            model.Perforate15 = obj.MasterData.Perforate15 == null ? 0 : obj.MasterData.Perforate15;
            model.Perforate16 = obj.MasterData.Perforate16 == null ? 0 : obj.MasterData.Perforate16;

            //////Honey////////
            if (!string.IsNullOrEmpty(model.Code))
            {
                var boardSplit = JsonConvert.DeserializeObject<List<BoardSpecWeight>>(_boardCombineAPIRepository.GetBoardSpecWeightByCode(_factoryCode, model.Code, _token));
                if (boardSplit.Count != 0)
                {
                    var grade = boardSplit[0].PaperDes;

                    if (grade == "--000" && boardSplit.Count > 2)
                        grade = boardSplit[2].PaperDes;

                    var paper = JsonConvert.DeserializeObject<HoneyPaper>(_honeyPaperAPIRepository.GetHoneyPaperByGrade(_factoryCode, grade, _token));

                    if (paper != null)
                    {
                        model.shrink = paper.Shrink;
                        model.stretch = paper.Stretch;
                        model.widHC = Convert.ToInt32(model.CutSheetWid / paper.Shrink);
                        model.lenHC = Convert.ToInt32(Convert.ToDouble(model.CutSheetLeng) / paper.Stretch);
                    }
                }
            }
            //////Honey////////

            model.CutSheetWidInch = obj.MasterData.CutSheetWidInch == null ? 0 : obj.MasterData.CutSheetWidInch;
            model.CutSheetLengInch = obj.MasterData.CutSheetLengInch == null ? 0 : obj.MasterData.CutSheetLengInch;

            model.SheetArea = obj.MasterData.SheetArea == null || obj.MasterData.SheetArea == 0 ? model.CutSheetWid * model.CutSheetLeng : obj.MasterData.SheetArea;
            int? slot = 0;
            if (model.JointLap == 0)
            {
                slot = model.Scorew2 * model.Slit * model.No_Slot;
            }
            else
            {
                slot = Convert.ToInt32((model.CutSheetWid - model.Scorew2) * (21.5 + model.JointLap) + (model.Slit * model.CutSheetWid));
            }
            model.BoxArea = obj.MasterData.BoxArea == null || obj.MasterData.BoxArea == 0 ? model.SheetArea - slot : obj.MasterData.BoxArea;
            model.WeightSh = obj.MasterData.WeightSh == null || obj.MasterData.WeightSh == 0 ? model.Weight * model.SheetArea / 1000000000 : obj.MasterData.WeightSh;
            model.WeightBox = obj.MasterData.WeightBox == null || obj.MasterData.WeightBox == 0 ? model.Weight * model.BoxArea / 1000000000 : obj.MasterData.WeightBox;
            model.WeightBoxInit = Convert.ToDouble(decimal.Round(Convert.ToDecimal(model.WeightBox), 3));
            model.CutSheetWidInit = model.CutSheetWid;

            if (obj.Routings.Count > 0 && obj.TransactionAction == "Edit")
            {
                model.FlagRouting = 1;
            }

            //model.PrintMaster = "";
            model.PrintMasterPath = "";
            if (obj.TransactionsDetail.CapImg == false)
            {
                if (obj.MasterData.DiecutPictPath != "")
                {
                    model.PrintMaster = obj.MasterData.DiecutPictPath;
                    //model.PrintMasterPath = _extensionService.Base64String(hostEnvironment, obj.MasterData.DiecutPictPath);
                    model.PrintMasterPath = ConvertPictureToBase64._ConvertPictureToBase64(obj.MasterData.DiecutPictPath);
                }
            }
            else if (obj.TransactionsDetail.CapImg == null)
            {
                model.PrintMaster = string.IsNullOrEmpty(obj.MasterData.DiecutPictPath) ? null : obj.MasterData.DiecutPictPath;
                if (!string.IsNullOrEmpty(model.PrintMaster))
                {
                    model.PrintMasterPath = ConvertPictureToBase64._ConvertPictureToBase64(obj.MasterData.DiecutPictPath);
                    if (model.PrintMasterPath == "")
                    {
                        model.PrintMaster = null;
                    }
                }
            }

            return model;
        }

        public string GetBoardKIWI(string boardCombine)
        {
            var bKiwi = "";
            var flu = "";

            string[] ArrarBoard = boardCombine.Split("/");

            var paperGrades = JsonConvert.DeserializeObject<List<PaperGrade>>(_paperGradeAPIRepository.GetPaperGradeList(_factoryCode, _token));

            if (ArrarBoard.Count() > 0)
            {
                if (ArrarBoard[0].Length == 1)
                    flu = ArrarBoard[0] + " ";
                else
                    flu = ArrarBoard[0];
            }

            bKiwi = flu + "(";

            if (ArrarBoard.Count() > 1)
            {
                //var board1 = ProductSpecRepository.GetKIWI(context, ArrarBoard[1].Trim());
                var board1 = paperGrades.Where(p => p.Grade == ArrarBoard[1].Trim()).FirstOrDefault();
                if (board1 != null)
                    bKiwi = bKiwi + board1.Kiwi;
                else
                    bKiwi = bKiwi + ArrarBoard[1].Trim();
            }
            if (ArrarBoard.Count() > 2)
            {
                //var board2 = ProductSpecRepository.GetKIWI(context, ArrarBoard[2].Trim());
                var board2 = paperGrades.Where(p => p.Grade == ArrarBoard[2].Trim()).FirstOrDefault();
                if (board2 != null)
                    bKiwi = bKiwi + "," + board2.Kiwi;
                else
                    bKiwi = bKiwi + "," + ArrarBoard[2].Trim();
            }
            if (ArrarBoard.Count() > 3)
            {
                //var board3 = ProductSpecRepository.GetKIWI(context, ArrarBoard[3].Trim());
                var board3 = paperGrades.Where(p => p.Grade == ArrarBoard[3].Trim()).FirstOrDefault();
                if (board3 != null)
                    bKiwi = bKiwi + "," + board3.Kiwi;
                else
                    bKiwi = bKiwi + "," + ArrarBoard[3].Trim();
            }
            if (ArrarBoard.Count() > 4)
            {
                //var board4 = ProductSpecRepository.GetKIWI(context, ArrarBoard[4].Trim());
                var board4 = paperGrades.Where(p => p.Grade == ArrarBoard[4].Trim()).FirstOrDefault();
                if (board4 != null)
                    bKiwi = bKiwi + "," + board4.Kiwi;
                else
                    bKiwi = bKiwi + "," + ArrarBoard[4].Trim();
            }
            if (ArrarBoard.Count() > 5)
            {
                //var board5 = ProductSpecRepository.GetKIWI(context, ArrarBoard[5].Trim());
                var board5 = paperGrades.Where(p => p.Grade == ArrarBoard[5].Trim()).FirstOrDefault();
                if (board5 != null)
                    bKiwi = bKiwi + "," + board5.Kiwi;
                else
                    bKiwi = bKiwi + "," + ArrarBoard[5].Trim();
            }
            if (ArrarBoard.Count() > 6)
            {
                //var board6 = ProductSpecRepository.GetKIWI(context, ArrarBoard[6].Trim());
                var board6 = paperGrades.Where(p => p.Grade == ArrarBoard[6].Trim()).FirstOrDefault();
                if (board6 != null)
                    bKiwi = bKiwi + "," + board6.Kiwi;
                else
                    bKiwi = bKiwi + "," + ArrarBoard[6].Trim();
            }
            if (ArrarBoard.Count() > 7)
            {
                //var board7 = ProductSpecRepository.GetKIWI(context, ArrarBoard[7].Trim());
                var board7 = paperGrades.Where(p => p.Grade == ArrarBoard[7].Trim()).FirstOrDefault();
                if (board7 != null)
                    bKiwi = bKiwi + "," + board7.Kiwi;
                else
                    bKiwi = bKiwi + "," + ArrarBoard[7].Trim();
            }

            return bKiwi + ")";
        }

        public double? GetBasisWeight(string code, string flute, string factoryCode)
        {
            double? bWeight = 0;

            if (flute.Contains("H"))
            {
                var board = JsonConvert.DeserializeObject<BoardCombine>(_boardCombineAPIRepository.GetBoardByCode(_factoryCode, code, _token));
                bWeight = board.Weight;
            }
            else
            {
                var board = JsonConvert.DeserializeObject<List<BoardSpecWeight>>(_boardCombineAPIRepository.GetBoardSpecWeightByCode(_factoryCode, code, _token));
                var flu = JsonConvert.DeserializeObject<List<FluteTr>>(_fluteTrAPIRepository.GetFluteTrByFlute(factoryCode, flute, _token));

                int i = 0, j = 0;

                if (board.Count > flu.Count)
                    j = flu.Count;
                else if (flu.Count > board.Count)
                    j = board.Count;
                else
                    j = board.Count;

                if (flu.Count != 0)
                {
                    foreach (var item in board)
                    {
                        if (i < j)
                        {
                            bWeight = bWeight + (item.BasicWeight * flu[i].Tr);
                            i++;
                        }
                        else
                        {
                            bWeight = bWeight + (item.BasicWeight);
                            i++;
                        }
                    }
                }
            }
            return bWeight;
        }

        public decimal ConvertmmToInch(int? mm)
        {
            decimal inch = Convert.ToDecimal(mm / 25.4);
            var dec = Math.Floor((inch - Math.Floor(inch)) * 100);
            if (dec >= 0 && dec <= 12)
                dec = 0;
            else if (dec >= 13 && dec <= 37)
                dec = Convert.ToDecimal(0.25);
            else if (dec >= 38 && dec <= 62)
                dec = Convert.ToDecimal(0.50);
            else if (dec >= 63 && dec <= 87)
                dec = Convert.ToDecimal(0.75);
            else
                dec = 1;

            inch = Math.Floor(inch) + dec;

            return inch;
        }

        public ProductInfoView BindProductInfoData(MasterDataTransactionModel obj)
        {
            ProductInfoView result = new ProductInfoView();

            result.PartNo = obj.MasterData.PartNo;
            result.Description = obj.MasterData.Description;
            result.PC = obj.MasterData.Pc;
            result.SaleText1 = obj.MasterData.SaleText1;
            result.SaleText2 = obj.MasterData.SaleText2;
            result.SaleText3 = obj.MasterData.SaleText3;
            result.SaleText4 = obj.MasterData.SaleText4;
            result.HighValue = obj.MasterData.HighValue;

            var hvaMasters = JsonConvert.DeserializeObject<List<HvaMaster>>(_hvaMasterAPIRepository.GetHvaMasters(_factoryCode, _token));

            result.HvaMasters = hvaMasters;
            if (!string.IsNullOrEmpty(obj.MasterData.HighValue))
            {
                var desc = hvaMasters.Where(h => h.HighValue == result.HighValue).FirstOrDefault();
                if (desc == null)
                    result.HighValueDisplay = result.HighValue;
                else
                    result.HighValueDisplay = result.HighValue + " " + desc.HighDescription;
                result.HVL_ProdTypeList1 = hvaMasters.Where(h => h.Seq == 1).Select(s => new SelectListItem() { Value = s.HighValue.ToString(), Text = s.HighValue.ToString() + " " + s.HighDescription });
                result.HVL_ProdTypeList2 = hvaMasters.Where(h => h.Seq == 2).Select(s => new SelectListItem() { Value = s.HighValue.ToString(), Text = s.HighValue.ToString() + " " + s.HighDescription });
                result.HVL_ProdTypeList3 = hvaMasters.Where(h => h.Seq == 3).Select(s => new SelectListItem() { Value = s.HighValue.ToString(), Text = s.HighValue.ToString() + " " + s.HighDescription });
                result.HVL_ProdTypeList4 = hvaMasters.Where(h => h.Seq == 4).Select(s => new SelectListItem() { Value = s.HighValue.ToString(), Text = s.HighValue.ToString() + " " + s.HighDescription });
                result.HVL_ProdTypeList5 = hvaMasters.Where(h => h.Seq == 5).Select(s => new SelectListItem() { Value = s.HighValue.ToString(), Text = s.HighValue.ToString() + " " + s.HighDescription });
                result.HVL_ProdTypeList6 = hvaMasters.Where(h => h.Seq == 6).Select(s => new SelectListItem() { Value = s.HighValue.ToString(), Text = s.HighValue.ToString() + " " + s.HighDescription });
                result.HVL_ProdTypeList7 = hvaMasters.Where(h => h.Seq == 7).Select(s => new SelectListItem() { Value = s.HighValue.ToString(), Text = s.HighValue.ToString() + " " + s.HighDescription });
            }

            if (obj.TransactionsDetail != null)
            {
                var hvaCodeFromHighValue = JsonConvert.DeserializeObject<HvaMaster>(_hvaMasterAPIRepository.GetHvaMasterByHighValue(_factoryCode, obj.MasterData.HighValue, _token));
                if (hvaCodeFromHighValue != null)
                {
                    switch (hvaCodeFromHighValue.Seq)
                    {
                        case 1:
                            obj.TransactionsDetail.HvaGroup1 = hvaCodeFromHighValue.HighValue;
                            break;

                        case 2:
                            obj.TransactionsDetail.HvaGroup2 = hvaCodeFromHighValue.HighValue;
                            break;

                        case 3:
                            obj.TransactionsDetail.HvaGroup3 = hvaCodeFromHighValue.HighValue;
                            break;

                        case 4:
                            obj.TransactionsDetail.HvaGroup4 = hvaCodeFromHighValue.HighValue;
                            break;

                        case 5:
                            obj.TransactionsDetail.HvaGroup5 = hvaCodeFromHighValue.HighValue;
                            break;

                        case 6:
                            obj.TransactionsDetail.HvaGroup6 = hvaCodeFromHighValue.HighValue;
                            break;

                        case 7:
                            obj.TransactionsDetail.HvaGroup7 = hvaCodeFromHighValue.HighValue;
                            break;
                    }
                }

                result.HvaProductType = obj.TransactionsDetail.HvaGroup1;
                result.HvaStructural = obj.TransactionsDetail.HvaGroup2;
                result.HvaPrinting = obj.TransactionsDetail.HvaGroup3;
                result.HvaFlute = obj.TransactionsDetail.HvaGroup4;
                result.HvaCorrugating = obj.TransactionsDetail.HvaGroup5;
                result.HvaCoating = obj.TransactionsDetail.HvaGroup6;
                result.HvaFinishing = obj.TransactionsDetail.HvaGroup7;
            }
            return result;
        }

        public ProductPictureView BindProductPictureData(MasterDataTransactionModel obj, IHostingEnvironment hostEnvironment)
        {
            ProductPictureView model = new ProductPictureView();

            model.Pic_DrawingName = obj.MasterData.DiecutPictPath;
            model.Pic_DrawingPath = obj.MasterData.DiecutPictPath;

            model.Pic_PrintName = obj.MasterData.PrintMasterPath;
            model.Pic_PrintPath = obj.MasterData.PrintMasterPath;

            model.Pic_PalletName = obj.MasterData.PalletizationPath;
            model.Pic_PalletPath = obj.MasterData.PalletizationPath;

            model.Pic_FGName = obj.MasterData.FgpicPath;
            model.Pic_FGPath = obj.MasterData.FgpicPath;

            return model;
        }

        public TransactionDetail BindDataTransactionDetail(TransactionDataModel model)
        {
            TransactionDetail Detail = new TransactionDetail();
            if (model == null)
            {
                return null;
            }
            Detail.ProductTypDetail = model.modelCategories.ProductTypeName;
            Detail.HierarchyLV2Detail = model.modelCategories.HierarchyLV2;
            Detail.CustNameDetail = model.modelProductCustomer.CustName;
            Detail.MaterialDescriptionDetail = model.modelProductInfo.Description;
            Detail.PCDetail = model.modelProductInfo.PC;
            if (!string.IsNullOrEmpty(model.modelProductSpec.Board))
                Detail.BoardDetail = model.modelProductSpec.Flute + ":" + model.modelProductSpec.Board;
            Detail.CostDetail = Convert.ToString(model.modelProductSpec.CostPerTon);
            Detail.HierarchyDetail = model.modelProductSpec.Hierarchy;
            Detail.RoutingDetail = new List<RoutingDataModel>();
            Detail.RoutingDetail = model.modelRouting.RoutingDataList;
            Detail.IsCreateBOM = model.modelCategories != null && (model.modelCategories.MatCode == "14" || model.modelCategories.MatCode == "24" || model.modelCategories.MatCode == "84") ? true : false;

            //RoutingDataList
            return Detail;
        }

        #endregion BindData

        #region Services

        public void GetCategoriesListData(ref TransactionDataModel transactionDataModel)
        {
            transactionDataModel.modelCategories.KindOfProductList = JsonConvert.DeserializeObject<List<KindOfProduct>>(_kindOfProductAPIRepository.GetKindOfProductList(_factoryCode, _token));
            transactionDataModel.modelCategories.KindOfProductGroupList = JsonConvert.DeserializeObject<List<KindOfProductGroup>>(Convert.ToString(_kindOfProductGroupAPIRepository.GetKindOfProductGroupList(_factoryCode, _token)));
            transactionDataModel.modelCategories.ProcessCostList = JsonConvert.DeserializeObject<List<ProcessCost>>(Convert.ToString(_processCostAPIRepository.GetProcessCostList(_factoryCode, _token)));
            transactionDataModel.modelCategories.ProductTypeList = JsonConvert.DeserializeObject<List<ProductType>>(Convert.ToString(_productTypeAPIRepository.GetProductTypeList(_factoryCode, _token)));
            transactionDataModel.modelCategories.MaterialTypeList = JsonConvert.DeserializeObject<List<MaterialType>>(Convert.ToString(_materialTypeAPIRepository.GetMaterialTypeList(_token)));
            transactionDataModel.modelCategories.UnitMaterialList = JsonConvert.DeserializeObject<List<UnitMaterial>>(Convert.ToString(_unitMaterialAPIRepository.GetUnitMaterialList(_factoryCode, _token)));
            transactionDataModel.modelCategories.HierarchyLV2List = JsonConvert.DeserializeObject<List<HierarchyLv2Matrix>>(Convert.ToString(_hierarchyLv2APIRepository.GetHierarchyLv2List(_factoryCode, _token)));
            transactionDataModel.modelCategories.MapcostList = JsonConvert.DeserializeObject<List<MapCost>>(Convert.ToString(_mapCostAPIRepository.GetMapCostList(_factoryCode, _token)));
            transactionDataModel.modelCategories.HierarchyLv3s = JsonConvert.DeserializeObject<List<HierarchyLv3>>(_hierarchyLv3APIRepository.GetAllHierarchyLv3s(_factoryCode, _token));
            transactionDataModel.modelCategories.HierarchyLv4s = JsonConvert.DeserializeObject<List<HierarchyLv4>>(_hierarchyLv4APIRepository.GetAllHierarchyLv4s(_factoryCode, _token));

            //get FSC Code and FSC FG Code
            transactionDataModel.modelCategories.FSCCodes = JsonConvert.DeserializeObject<List<PpcFscCode>>(fscCodeAPIRepository.GetFSCCodes(_factoryCode, _token));
            transactionDataModel.modelCategories.FSCFGCodes = JsonConvert.DeserializeObject<List<PpcFscFgCode>>(fscFGCodeAPIRepository.GetFSCFGCodes(_factoryCode, _token));

            //get PPCMasterRpacs for select RpacLob, RpacProgram, RpacBrand, RpacPackagingType
            transactionDataModel.modelCategories.PPCMasterRpacs = JsonConvert.DeserializeObject<List<PpcMasterRpac>>(pPCMasterRpacAPIRepository.GetPPCMasterRpacsByFactoryCode(_factoryCode, _token));
        }

        private int SetRSCId(string nameOfRSC)
        {
            var RSCId = 1;
            nameOfRSC = nameOfRSC.Replace(" ", "_");
            if (nameOfRSC == NameOfRSC.Standard.ToString())
                RSCId = (int)NameOfRSC.Standard;
            else if (nameOfRSC == NameOfRSC.Full_Overlap.ToString())
                RSCId = (int)NameOfRSC.Full_Overlap;
            else if (nameOfRSC == NameOfRSC.Tele_Top_Lid.ToString())
                RSCId = (int)NameOfRSC.Tele_Top_Lid;
            else if (nameOfRSC == NameOfRSC.Tele_Bottom_Lid.ToString())
                RSCId = (int)NameOfRSC.Tele_Bottom_Lid;
            else if (nameOfRSC == NameOfRSC.Sleeve.ToString())
                RSCId = (int)NameOfRSC.Sleeve;
            else if (nameOfRSC == NameOfRSC.Rotary.ToString())
                RSCId = (int)NameOfRSC.Rotary;

            return RSCId;
        }

        public void SetTransactionStatus(ref TransactionDataModel model, string transactionName)
        {
            var transactionStatusStyle = new TransactionStatusStyle();

            if (transactionName == "Categories")
            {
                transactionStatusStyle.Categories = "active";
                transactionStatusStyle.Customer = "";
                transactionStatusStyle.ProductInformation = "";
                transactionStatusStyle.ProductSpec = "";
                transactionStatusStyle.ProductProperties = "";
                transactionStatusStyle.ProductRouting = "";
                transactionStatusStyle.ProductERPInterface = "";
                transactionStatusStyle.ProductPicture = "";
            }
            else if (transactionName == "Customer")
            {
                transactionStatusStyle.Categories = "done";
                transactionStatusStyle.Customer = "active";
                transactionStatusStyle.ProductInformation = "";
                transactionStatusStyle.ProductSpec = "";
                transactionStatusStyle.ProductProperties = "";
                transactionStatusStyle.ProductRouting = "";
                transactionStatusStyle.ProductERPInterface = "";
                transactionStatusStyle.ProductPicture = "";
            }
            else if (transactionName == "ProductInformation")
            {
                transactionStatusStyle.Categories = "done";
                transactionStatusStyle.Customer = "done";
                transactionStatusStyle.ProductInformation = "active";
                transactionStatusStyle.ProductSpec = "";
                transactionStatusStyle.ProductProperties = "";
                transactionStatusStyle.ProductRouting = "";
                transactionStatusStyle.ProductERPInterface = "";
                transactionStatusStyle.ProductPicture = "";
            }
            else if (transactionName == "ProductSpec")
            {
                transactionStatusStyle.Categories = "done";
                transactionStatusStyle.Customer = "done";
                transactionStatusStyle.ProductInformation = "done";
                transactionStatusStyle.ProductSpec = "active";
                transactionStatusStyle.ProductProperties = "";
                transactionStatusStyle.ProductRouting = "";
                transactionStatusStyle.ProductERPInterface = "";
                transactionStatusStyle.ProductPicture = "";
            }
            else if (transactionName == "ProductProperties")
            {
                transactionStatusStyle.Categories = "done";
                transactionStatusStyle.Customer = "done";
                transactionStatusStyle.ProductInformation = "done";
                if (model.modelCategories.MatCode == "14" || model.modelCategories.MatCode == "24" || model.modelCategories.MatCode == "84")
                    transactionStatusStyle.ProductSpec = "";
                else
                    transactionStatusStyle.ProductSpec = "done";
                transactionStatusStyle.ProductProperties = "active";
                transactionStatusStyle.ProductRouting = "";
                transactionStatusStyle.ProductERPInterface = "";
                transactionStatusStyle.ProductPicture = "";
            }
            else if (transactionName == "ProductRouting")
            {
                transactionStatusStyle.Categories = "done";
                transactionStatusStyle.Customer = "done";
                transactionStatusStyle.ProductInformation = "done";
                if (model.modelCategories.MatCode == "14" || model.modelCategories.MatCode == "24" || model.modelCategories.MatCode == "84")
                    transactionStatusStyle.ProductSpec = "";
                else
                    transactionStatusStyle.ProductSpec = "done";
                transactionStatusStyle.ProductProperties = "done";
                transactionStatusStyle.ProductRouting = "active";
                transactionStatusStyle.ProductERPInterface = "";
                transactionStatusStyle.ProductPicture = "";
            }
            else if (transactionName == "ERPInterface")
            {
                transactionStatusStyle.Categories = "done";
                transactionStatusStyle.Customer = "done";
                transactionStatusStyle.ProductInformation = "done";
                if (model.modelCategories.MatCode == "14" || model.modelCategories.MatCode == "24" || model.modelCategories.MatCode == "84")
                    transactionStatusStyle.ProductSpec = "";
                else
                    transactionStatusStyle.ProductSpec = "done";
                transactionStatusStyle.ProductProperties = "done";
                transactionStatusStyle.ProductRouting = "done";
                transactionStatusStyle.ProductERPInterface = "active";
                transactionStatusStyle.ProductPicture = "";
            }
            else if (transactionName == "Picture")
            {
                transactionStatusStyle.Categories = "done";
                transactionStatusStyle.Customer = "done";
                transactionStatusStyle.ProductInformation = "done";
                if (model.modelCategories.MatCode == "14" || model.modelCategories.MatCode == "24" || model.modelCategories.MatCode == "84")
                    transactionStatusStyle.ProductSpec = "";
                else
                    transactionStatusStyle.ProductSpec = "done";
                transactionStatusStyle.ProductProperties = "done";
                transactionStatusStyle.ProductRouting = "done";
                transactionStatusStyle.ProductERPInterface = "done";
                transactionStatusStyle.ProductPicture = "active";
            }
            else if (transactionName == "End")
            {
                transactionStatusStyle.Categories = "done";
                transactionStatusStyle.Customer = "done";
                transactionStatusStyle.ProductInformation = "done";
                if (model.modelCategories.MatCode == "14" || model.modelCategories.MatCode == "24" || model.modelCategories.MatCode == "84")
                    transactionStatusStyle.ProductSpec = "";
                else
                    transactionStatusStyle.ProductSpec = "done";
                transactionStatusStyle.ProductProperties = "done";
                transactionStatusStyle.ProductRouting = "done";
                transactionStatusStyle.ProductERPInterface = "done";
                transactionStatusStyle.ProductPicture = "done";
            }

            model.CurrentTransaction = transactionName;
            model.TransactionStatus = transactionStatusStyle;
        }

        public void UpdateMaxProgress(string factoryCode, ref TransactionDataModel transactionDataModel)
        {
            var numberOfProgress = 0;
            var activeString = "active";
            if (transactionDataModel.TransactionStatus.Categories == activeString)
            {
                numberOfProgress = (int)NumberOfProgress.Categories;
            }
            else if (transactionDataModel.TransactionStatus.Customer == activeString)
            {
                numberOfProgress = (int)NumberOfProgress.Customer;
            }
            else if (transactionDataModel.TransactionStatus.ProductInformation == activeString)
            {
                numberOfProgress = (int)NumberOfProgress.ProductInformation;
            }
            else if (transactionDataModel.TransactionStatus.ProductSpec == activeString)
            {
                numberOfProgress = (int)NumberOfProgress.ProductSpec;
            }
            else if (transactionDataModel.TransactionStatus.ProductProperties == activeString)
            {
                numberOfProgress = (int)NumberOfProgress.ProductProperties;
            }
            else if (transactionDataModel.TransactionStatus.ProductRouting == activeString)
            {
                numberOfProgress = (int)NumberOfProgress.ProductRouting;
            }
            else if (transactionDataModel.TransactionStatus.ProductERPInterface == activeString)
            {
                numberOfProgress = (int)NumberOfProgress.ERPInterface;
            }
            else if (transactionDataModel.TransactionStatus.ProductPicture == activeString)
            {
                numberOfProgress = (int)NumberOfProgress.Picture;
            }

            var existTransactionDetail = JsonConvert.DeserializeObject<TransactionsDetail>(_transactionsDetailAPIRepository.GetTransactionsDetailByMat(factoryCode, transactionDataModel.MaterialNo, _token));
            if (existTransactionDetail != null)
            {
                existTransactionDetail.MaxStep = existTransactionDetail.MaxStep == null ? 0 : existTransactionDetail.MaxStep;
                if (existTransactionDetail.MaxStep <= numberOfProgress)
                {
                    existTransactionDetail.MaxStep = numberOfProgress;

                    ParentModel parentModel = new ParentModel
                    {
                        AppName = Globals.AppNameEncrypt,
                        FactoryCode = _factoryCode,
                        PlantCode = _factoryCode,
                        TransactionsDetail = existTransactionDetail
                    };

                    _transactionsDetailAPIRepository.UpdateTransactionsDetail(JsonConvert.SerializeObject(parentModel), _token);
                    transactionDataModel.TransactionDetail.MaxStep = numberOfProgress;
                }
                else if (transactionDataModel.TransactionDetail.MaxStep <= numberOfProgress && transactionDataModel.TransactionDetail.MaxStep <= existTransactionDetail.MaxStep)
                {
                    transactionDataModel.TransactionDetail.MaxStep = numberOfProgress;
                }
            }
        }

        #region Outsourcing

        public void GetCompanyProfiles(ref OutsourcingViewModel outsourcingViewModel)
        {
            outsourcingViewModel.CompanyProfiles = new List<CompanyProfile>();
            outsourcingViewModel.CompanyProfiles = JsonConvert.DeserializeObject<List<CompanyProfile>>(_companyProfileAPIRepository.GetCompanyProfileList(_factoryCode, _token)).ToList();
            outsourcingViewModel.FactoryLogon = _factoryCode;
        }

        public void SaveFactoryCodeToSession(OutsourcingViewModel outsourcingViewModel)
        {
            TransactionDataModel transactionDataModel = new TransactionDataModel();
            transactionDataModel.TransactionDetail = new TransactionDetail();
            transactionDataModel.PlantOs = outsourcingViewModel.Plant;
            transactionDataModel.SaleOrg = outsourcingViewModel.SaleOrg;
            transactionDataModel.EventFlag = outsourcingViewModel.Action == "Create" ? "CreateOs" : "CopyOs";
            transactionDataModel.RealEventFlag = outsourcingViewModel.Action == "Create" ? "CreateOs" : "CopyOs";
            transactionDataModel.TransactionDetail.IsOutSource = true;
            transactionDataModel.TransactionDetail.OrderTypeId = outsourcingViewModel.OrderTypeId;
            transactionDataModel.TransactionDetail.MaxStep = 0;
            transactionDataModel.MaterialNo = outsourcingViewModel.MaterialNo.ToUpper();

            SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "TransactionDataModel", transactionDataModel);
        }

        public void GetHireOrders(ref OutsourcingViewModel outsourcingViewModel)
        {
            outsourcingViewModel.HireOrders = new List<HireOrder>();
            outsourcingViewModel.HireOrders = JsonConvert.DeserializeObject<List<HireOrder>>(_hireOrderAPIRepository.GetAllHireOrder(_factoryCode, _token));
        }

        public void GetHireMappings(ref OutsourcingViewModel outsourcingViewModel)
        {
            outsourcingViewModel.HireMappings = new List<HireMapping>();
            outsourcingViewModel.HireMappings = JsonConvert.DeserializeObject<List<HireMapping>>(_hireMappingAPIRepository.GetAllHireMapping(_factoryCode, _token));
        }

        public void CheckDuplicateMaterial(string plantOs, string materialNo, string action)
        {
            var existMaterialNo = new MasterData();

            if (action.Equals("Copy"))
            {
                //check material of plantOs selected.
                var existMaterialNoOfPlantOs = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNo(plantOs, materialNo, _token));

                if (existMaterialNoOfPlantOs == null)
                {
                    var fromSaleOrg = JsonConvert.DeserializeObject<CompanyProfile>(_companyProfileAPIRepository.GetCompanyProfileByPlant(plantOs, _token)).SaleOrg.Split(" ");
                    var exitMasterData = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNoOnly(_factoryCode, materialNo, _token));
                    var saleOrg = exitMasterData != null ? exitMasterData.SaleOrg : string.Empty;
                    if (saleOrg != string.Empty)
                    {
                        throw new Exception($"Can't find material No. {materialNo} for sale org '{fromSaleOrg[0]}'.But you can find this material No from sale org '{saleOrg}'");
                    }
                    else
                    {
                        throw new Exception($"Can't find material No. {materialNo} for sale org '{fromSaleOrg[0]}'.");
                    }
                }
            }

            //check duplicate material no in your factory.
            existMaterialNo = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNoAndFactory(_factoryCode, materialNo, _token));
            if (existMaterialNo != null)
            {
                throw new Exception("Your factory has been created material!");
            }
        }

        #endregion Outsourcing

        #endregion Services



        #region Function Cal
        //public string CalSizeDimensions(MasterData masterData,List<Routing> routings)
        //{
        //    var result = string.Empty;
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(masterData?.Flute))
        //        {
        //            var flute = JsonConvert.DeserializeObject<Flute>(_fluteAPIRepository.GetFluteByFlute(_factoryCode, masterData.Flute, _token));
        //            double truckStack = 0;
        //            if (flute != null) { truckStack = flute.TruckStack ?? 0; }
        //            double sizeDimensionConst = 1905;
        //            var pmtsConfig = JsonConvert.DeserializeObject<PmtsConfig>(_pMTsConfigAPIRepository.GetPMTsConfigByFactoryName(_factoryCode, "Size_Dimession_Const", _token));
        //            double.TryParse(pmtsConfig?.FucValue, out sizeDimensionConst);
        //            sizeDimensionConst = sizeDimensionConst == 0 ? 1905 : sizeDimensionConst;
        //            if (!string.IsNullOrEmpty(masterData.BoxType))
        //            {
        //                if (!masterData.BoxType.ToUpper().Contains("DIECUT"))
        //                {
        //                    double cutSheetWid = 0, cutSheetLeng = 0;
        //                    cutSheetWid = Convert.ToDouble(masterData.CutSheetWid ?? 0);
        //                    cutSheetLeng = Convert.ToDouble(masterData.CutSheetLeng ?? 0);

        //                    double res = 0;
        //                    if (sizeDimensionConst > 0 && truckStack > 0)
        //                    {
        //                        res = (cutSheetWid / sizeDimensionConst) * (cutSheetLeng / truckStack);
        //                        result = res.ToString();
        //                        if (result.Length > 32)
        //                        {
        //                            result = result.Substring(0, 32);
        //                        }
        //                    }
        //                }
        //                else if (masterData.BoxType.ToUpper().Contains("DIECUT") || masterData.Hierarchy.Contains("03PA"))
        //                {
        //                    var routing = routings?
        //                        .FirstOrDefault(p =>
        //                            ("คลัง".Contains(p.Machine) || "WH".Contains(p.MatCode)));

        //                    double sheetInWid = 0, sheetInLeg = 0;
        //                    if (routing != null)
        //                    {
        //                        sheetInWid = Convert.ToDouble(routing.SheetInWid ?? 0);
        //                        sheetInLeg = Convert.ToDouble(routing.SheetInLeg ?? 0);
        //                        double res = 0;
        //                        if (sizeDimensionConst > 0 && truckStack > 0)
        //                        {
        //                            res = (sheetInWid / sizeDimensionConst) * (sheetInLeg / truckStack);
        //                            result = res.ToString();
        //                            if (result.Length > 32)
        //                            {
        //                                result = result.Substring(0, 32);
        //                            }
        //                        }
        //                    }

        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //    return result;
        //}

        #endregion Function Cal
    }
}