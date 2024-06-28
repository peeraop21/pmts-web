using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PMTs.DataAccess;
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
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services
{
    [TraceAspect]
    public class ProductCustomerService : IProductCustomerService
    {

        IHttpContextAccessor _httpContextAccessor;

        private readonly INewProductService _newProductService;
        private readonly IProductGroupAPIRepository _productGroupAPIRepository;
        private readonly ICustomerAPIRepository _customerAPIRepository;
        private readonly ICustShipToAPIRepository _custShipToAPIRepository;
        private readonly IQaItemsAPIRepository _qaItemsAPIRepository;
        private readonly IQualitySpecAPIRepository _qualitySpecAPIRepository;
        private readonly IMasterDataAPIRepository _masterDataAPIRepository;
        private readonly IPlantViewAPIRepository _plantViewAPIRepository;
        private readonly ISalesViewAPIRepository _salesViewAPIRepository;
        private readonly IRoutingAPIRepository _routingAPIRepository;
        private readonly ITransactionsDetailAPIRepository _transactionsDetailAPIRepository;
        private readonly IAutoPackingSpecAPIRepository autoPackingSpecAPIRepository;
        private readonly ITagPrintSORepository _tagPrintSORepository;
        private readonly IFormulaAPIRepository _formulaAPIRepository;


        private readonly string _username;
        private readonly string _saleOrg;
        private readonly string _factoryCode;
        private readonly string _token;

        public ProductCustomerService(IHttpContextAccessor httpContextAccessor,
            ICustomerAPIRepository customerAPIRepository,
            ICustShipToAPIRepository custShipToAPIRepository,
            INewProductService newProductService,
            IProductGroupAPIRepository productGroupAPIRepository,
            IQaItemsAPIRepository qaItemsAPIRepository,
            IQualitySpecAPIRepository qualitySpecAPIRepository,
            IMasterDataAPIRepository masterDataAPIRepository,
            IPlantViewAPIRepository plantViewAPIRepository,
            ISalesViewAPIRepository salesViewAPIRepository,
            IRoutingAPIRepository routingAPIRepository,
            ITransactionsDetailAPIRepository transactionsDetailAPIRepository,
            IAutoPackingSpecAPIRepository autoPackingSpecAPIRepository,
            ITagPrintSORepository tagPrintSORepository,
            IFormulaAPIRepository formulaAPIRepository)
        {
            // Initialize HttpContext
            _httpContextAccessor = httpContextAccessor;

            _newProductService = newProductService;
            _productGroupAPIRepository = productGroupAPIRepository;
            // Initialize Repository
            _customerAPIRepository = customerAPIRepository;
            _custShipToAPIRepository = custShipToAPIRepository;
            _qaItemsAPIRepository = qaItemsAPIRepository;
            _qualitySpecAPIRepository = qualitySpecAPIRepository;
            _masterDataAPIRepository = masterDataAPIRepository;
            _plantViewAPIRepository = plantViewAPIRepository;
            _salesViewAPIRepository = salesViewAPIRepository;
            _routingAPIRepository = routingAPIRepository;
            _transactionsDetailAPIRepository = transactionsDetailAPIRepository;
            this.autoPackingSpecAPIRepository = autoPackingSpecAPIRepository;
            _tagPrintSORepository = tagPrintSORepository;
            _formulaAPIRepository = formulaAPIRepository;

            // Initialize SaleOrg and PlantCode from Session
            var userSessionModel = SessionExtentions.GetSession<UserSessionModel>(_httpContextAccessor.HttpContext.Session, "UserSessionModel");
            if (userSessionModel != null)
            {
                _username = userSessionModel.UserName;
                _saleOrg = userSessionModel.SaleOrg;
                _factoryCode = userSessionModel.FactoryCode;
                _token = userSessionModel.Token;
            }
        }

        public void GetCustomer(NewProductController newProductController, ref TransactionDataModel transactionDataModel)
        {
            var transactionDataModelSession = SessionExtentions.GetSession<TransactionDataModel>(_httpContextAccessor.HttpContext.Session, "TransactionDataModel");

            if (transactionDataModelSession.modelProductCustomer == null)
            {
                transactionDataModelSession.modelProductCustomer = new ProductCustomer();
            }

            transactionDataModelSession.modelProductCustomer.CustomerList = JsonConvert.DeserializeObject<List<Customer>>(_customerAPIRepository.GetCustomersByCustomerGroup(_factoryCode, _token));
            transactionDataModelSession.modelProductCustomer.CustShipToList = JsonConvert.DeserializeObject<List<CustShipTo>>(_custShipToAPIRepository.GetCustShipToList(_factoryCode, _token));
            transactionDataModelSession.modelProductCustomer.ProductGroupList = JsonConvert.DeserializeObject<List<ProductGroup>>(_productGroupAPIRepository.GetProductGroupList(_factoryCode, _token));

            transactionDataModelSession.modelProductCustomer.QaItems = new List<QaItems>();
            transactionDataModelSession.modelProductCustomer.QaItems = JsonConvert.DeserializeObject<List<QaItems>>(_qaItemsAPIRepository.GetQaItems(_token));

            //transactionDataModelSession.modelProductCustomer.TagPrintSO = new List<TagPrintSO>();
            //transactionDataModelSession.modelProductCustomer.TagPrintSO = JsonConvert.DeserializeObject<List<TagPrintSO>>(_tagPrintSORepository.GetTagPrintSO(_factoryCode,_token));
            transactionDataModelSession.modelProductCustomer.TagPrintSO = JsonConvert.DeserializeObject<List<TagPrintSo>>(_tagPrintSORepository.GetTagPrintSO(_factoryCode, _token)).OrderBy(q => q.Id).Select(q => q.DataText).Distinct().ToList(); ;


            transactionDataModelSession.CurrentTransaction = "Customer";

            if (!String.IsNullOrEmpty(transactionDataModelSession.MaterialNo) && transactionDataModelSession.modelProductCustomer.QualitySpecs == null)
            {
                transactionDataModelSession.modelProductCustomer.QualitySpecs = new List<QualitySpec>();
                transactionDataModelSession.modelProductCustomer.QualitySpecs = JsonConvert.DeserializeObject<List<QualitySpec>>(_qualitySpecAPIRepository.GetQualitySpecByMaterialNo(_factoryCode, transactionDataModelSession.MaterialNo, _token));
            }
            else if (String.IsNullOrEmpty(transactionDataModelSession.MaterialNo) && transactionDataModelSession.modelProductCustomer.QualitySpecs == null)
            {
                transactionDataModelSession.modelProductCustomer.QualitySpecs = new List<QualitySpec>();
            }

            _newProductService.SetTransactionStatus(ref transactionDataModelSession, "Customer");

            if (transactionDataModelSession.EventFlag == "Presale")
            {
                var customer = transactionDataModelSession.modelProductCustomer.CustomerList.FirstOrDefault(c => c.CusId == transactionDataModelSession.modelProductCustomer.CusId);
                if (customer != null)
                {
                    transactionDataModelSession.modelProductCustomer.NoTagBundle = customer.NoTagBundle;
                    transactionDataModelSession.modelProductCustomer.TagBundle = customer.TagBundle;
                    transactionDataModelSession.modelProductCustomer.TagPallet = customer.TagPallet;
                    transactionDataModelSession.modelProductCustomer.HeadTagBundle = customer.HeadTagBundle;
                    transactionDataModelSession.modelProductCustomer.FootTagBundle = customer.FootTagBundle;
                    transactionDataModelSession.modelProductCustomer.HeadTagPallet = customer.HeadTagPallet;
                    transactionDataModelSession.modelProductCustomer.FootTagPallet = customer.FootTagPallet;
                }
            }

            transactionDataModel = transactionDataModelSession;

            SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "TransactionDataModel", transactionDataModelSession);

            newProductController.ViewBag.PrintMaster = "Hide";
        }

        public void SaveCustomer(ref TransactionDataModel transactionDataModel, string QaSpecArr)
        {
            var transactionDataModelSession = SessionExtentions.GetSession<TransactionDataModel>(_httpContextAccessor.HttpContext.Session, "TransactionDataModel");
            var isOursource = false;
            transactionDataModelSession.modelProductCustomer = transactionDataModel.modelProductCustomer;

            //if (transactionDataModelSession.EventFlag != "Edit" || transactionDataModelSession.EventFlag != "EditOs")
            //{
            var qualitySpecs = JsonConvert.DeserializeObject<List<QualitySpec>>(QaSpecArr);
            qualitySpecs = qualitySpecs.Where(q => Convert.ToDecimal(q.Value) != 0.0m).ToList();
            transactionDataModelSession.modelProductCustomer.QualitySpecs = qualitySpecs != null ? qualitySpecs : new List<QualitySpec>();
            //}


            //// จัดลำดับ Tag
            ///
            string[] TagBundlewords;
            string[] TagPalletwords;
            var tagBundle = "";
            var tagPallet = "";
            if (!String.IsNullOrEmpty(transactionDataModel.modelProductCustomer.TagBundle))
            {
                TagBundlewords = transactionDataModel.modelProductCustomer.TagBundle.Split('$');

                //var TagBundlewordsorted = TagBundlewords.OrderBy(item => item[0]);
                var TagBundlewordsorted = TagBundlewords
                    .OrderBy(item =>
                        Convert.ToInt32(
                            !string.IsNullOrEmpty(item.Split(':')[0]) &&
                            !string.IsNullOrWhiteSpace(item.Split(':')[0])
                                ? item.Split(':')[0]
                                : "0"
                        )
                    );
                tagBundle = string.Join("$", TagBundlewordsorted);
            }

            if (!String.IsNullOrEmpty(transactionDataModel.modelProductCustomer.TagPallet))
            {
                TagPalletwords = transactionDataModel.modelProductCustomer.TagPallet.Split('$');

                //var TagPalletwordsorted = TagPalletwords.OrderBy(item => item[0]);
                var TagPalletwordsorted = TagPalletwords
                    .OrderBy(item =>
                        Convert.ToInt32(
                            !string.IsNullOrEmpty(item.Split(':')[0]) &&
                            !string.IsNullOrWhiteSpace(item.Split(':')[0])
                                ? item.Split(':')[0]
                                : "0"
                        )
                    );
                tagPallet = string.Join("$", TagPalletwordsorted);
            }

            transactionDataModelSession.modelProductCustomer.TagBundle = tagBundle;
            transactionDataModelSession.modelProductCustomer.TagPallet = tagPallet;

            if (transactionDataModelSession.MaterialNo != null && (transactionDataModelSession.EventFlag == "Create" || transactionDataModelSession.EventFlag == "Edit"))
            {
                #region Save Quality Spec
                var existQualitySpec = JsonConvert.DeserializeObject<List<QualitySpec>>(_qualitySpecAPIRepository.GetQualitySpecByMaterialNo(_factoryCode, transactionDataModelSession.MaterialNo, _token));
                if (existQualitySpec.Count() > 0)
                {
                    _qualitySpecAPIRepository.DeleteQualitySpec(_factoryCode, JsonConvert.SerializeObject(existQualitySpec), _token);
                }

                if (qualitySpecs != null && qualitySpecs.Count() > 0)
                {
                    var qualitySpecModels = new List<QualitySpec>();

                    if (qualitySpecs.Count > 0)
                    {

                        foreach (var qualitySpec in qualitySpecs)
                        {
                            decimal result;
                            if (Decimal.TryParse(qualitySpec.Value.ToString(), out result))
                            {
                                //transactionDataModel.MaterialNo
                                var qualitySpecModel = new QualitySpec
                                {
                                    FactoryCode = _factoryCode,
                                    MaterialNo = transactionDataModelSession.MaterialNo,
                                    Name = qualitySpec.Name,
                                    Unit = qualitySpec.Unit,
                                    Value = qualitySpec.Value
                                };

                                qualitySpecModels.Add(qualitySpecModel);
                            }
                        }
                        _qualitySpecAPIRepository.SaveQualitySpec(_factoryCode, JsonConvert.SerializeObject(qualitySpecModels), _token);
                    }
                }

                #endregion

                #region Update Master data and other Status

                // Get Material No From Transaction
                var materialNo = transactionDataModelSession.MaterialNo;

                var existMasterdata = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNo(_factoryCode, materialNo, _token));
                var existTransactionDetail = JsonConvert.DeserializeObject<TransactionsDetail>(_transactionsDetailAPIRepository.GetTransactionsDetailByMat(_factoryCode, materialNo, _token));
                isOursource = existTransactionDetail != null && existTransactionDetail.Outsource ? true : false;
                #region Update master data

                //update column CustRequirement, MatComment in masterdata
                existMasterdata.CustComment = transactionDataModel.modelProductCustomer.CustReq;
                existMasterdata.MaterialComment = transactionDataModel.modelProductCustomer.MaterialComment;
                existMasterdata.CusId = transactionDataModel.modelProductCustomer.CusId;
                existMasterdata.CustCode = transactionDataModel.modelProductCustomer.CustCode;
                existMasterdata.CustName = transactionDataModel.modelProductCustomer.CustName.Length > 50 ? transactionDataModel.modelProductCustomer.CustName.Substring(0, 50) : transactionDataModel.modelProductCustomer.CustName;
                existMasterdata.IndGrp = transactionDataModel.modelProductCustomer.IndDes == null ? null : transactionDataModel.modelProductCustomer.IndDes.Substring(0, 3);
                existMasterdata.IndDes = transactionDataModel.modelProductCustomer.IndDes;
                existMasterdata.NoTagBundle = transactionDataModel.modelProductCustomer.NoTagBundle;
                //existMasterdata.TagBundle = transactionDataModel.modelProductCustomer.TagBundle;
                // existMasterdata.TagPallet = transactionDataModel.modelProductCustomer.TagPallet;

                //// จัดลำดับ Tag

                //string[] TagBundlewords = transactionDataModel.modelProductCustomer.TagBundle.Split(',');

                //var TagBundlewordsorted = TagBundlewords.OrderBy(item => item[0]);
                //var tagBundle = string.Join(",", TagBundlewordsorted);

                //string[] TagPalletwords = transactionDataModel.modelProductCustomer.TagPallet.Split(',');

                //var TagPalletwordsorted = TagPalletwords.OrderBy(item => item[0]);
                ////customer.TagBundle = 
                //var tagPallet = string.Join(",", TagPalletwordsorted);


                existMasterdata.TagBundle = tagBundle;
                existMasterdata.TagPallet = tagPallet;
                existMasterdata.HeadTagBundle = transactionDataModel.modelProductCustomer.HeadTagBundle;
                existMasterdata.HeadTagPallet = transactionDataModel.modelProductCustomer.HeadTagPallet;
                existMasterdata.FootTagBundle = transactionDataModel.modelProductCustomer.FootTagBundle;
                existMasterdata.FootTagPallet = transactionDataModel.modelProductCustomer.FootTagPallet;

                existMasterdata.Freetext1TagBundle = transactionDataModel.modelProductCustomer.Freetext1TagBundle;
                existMasterdata.Freetext2TagBundle = transactionDataModel.modelProductCustomer.Freetext2TagBundle;
                existMasterdata.Freetext3TagBundle = transactionDataModel.modelProductCustomer.Freetext3TagBundle;

                existMasterdata.Freetext1TagPallet = transactionDataModel.modelProductCustomer.Freetext1TagPallet;
                existMasterdata.Freetext2TagPallet = transactionDataModel.modelProductCustomer.Freetext2TagPallet;
                existMasterdata.Freetext3TagPallet = transactionDataModel.modelProductCustomer.Freetext3TagPallet;
                existMasterdata.User = _username;

                if (!string.IsNullOrEmpty(existMasterdata?.BoxType))
                {
                    existMasterdata.SizeDimensions = JsonConvert.DeserializeObject<string>(_formulaAPIRepository.CalSizeDimensions(_factoryCode, transactionDataModelSession.MaterialNo, _token));
                }

                if (existMasterdata.PdisStatus != "N" && existMasterdata.SapStatus == true)
                {
                    existMasterdata.PdisStatus = "M";
                    existMasterdata.TranStatus = isOursource ? true : false;
                    transactionDataModelSession.PdisStatus = "M";
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
                #endregion

                #region Update status of PlantView
                var existPlantView = JsonConvert.DeserializeObject<PlantView>(_plantViewAPIRepository.GetPlantViewByMaterialNo(_factoryCode, materialNo, _token));
                if (existPlantView != null)
                {
                    if (!isOursource)
                    {
                        existPlantView.PdisStatus = existMasterdata.PdisStatus;
                        existPlantView.TranStatus = existMasterdata.TranStatus;
                        existPlantView.SapStatus = existMasterdata.SapStatus;
                    }

                    ParentModel parentModelPlantView = new ParentModel()
                    {
                        AppName = Globals.AppNameEncrypt,
                        SaleOrg = _saleOrg,
                        PlantCode = _factoryCode,
                        PlantView = existPlantView
                    };

                    string parentModelPlantViewJsonString = JsonConvert.SerializeObject(parentModelPlantView);
                    _plantViewAPIRepository.UpdatePlantView(parentModelPlantViewJsonString, _token);
                }


                #endregion

                #region Update status of SaleView
                var existSaleViews = JsonConvert.DeserializeObject<List<SalesView>>(_salesViewAPIRepository.GetSaleViewsByMaterialNoAndFactoryCode(_factoryCode, materialNo, _token));
                foreach (var existSaleView in existSaleViews)
                {
                    if (!isOursource)
                    {
                        existSaleView.PdisStatus = existMasterdata.PdisStatus;
                        existSaleView.TranStatus = existMasterdata.TranStatus;
                        existSaleView.SapStatus = existMasterdata.SapStatus;
                    }

                    existSaleView.CustCode = transactionDataModel.modelProductCustomer.CustCode;

                    ParentModel parentModelSaleView = new ParentModel()
                    {
                        AppName = Globals.AppNameEncrypt,
                        SaleOrg = _saleOrg,
                        PlantCode = _factoryCode,
                        SalesView = existSaleView
                    };

                    string parentModelSaleViewJsonString = JsonConvert.SerializeObject(parentModelSaleView);
                    _salesViewAPIRepository.UpdateSaleView(parentModelSaleViewJsonString, _token);
                }
                #endregion

                #region Update status of Routing
                //var existRoutings = JsonConvert.DeserializeObject<List<Routing>>(_routingAPIRepository.GetRoutingsByMaterialNo(_factoryCode, materialNo,_token));
                //var routings = new List<Routing>();
                //foreach (var existRouting in existRoutings)
                //{
                //    existRouting.Id = 0;
                //    existRouting.PdisStatus = existMasterdata.PdisStatus;
                //    existRouting.TranStatus = existMasterdata.TranStatus;
                //    existRouting.SapStatus = existMasterdata.SapStatus;
                //    existRouting.Plant = _factoryCode;

                //    routings.Add(existRouting);
                //}

                //if (routings.Count > 0)
                //{
                //    _routingAPIRepository.SaveRouting(_factoryCode, materialNo, JsonConvert.SerializeObject(routings),_token);


                //}
                if (!isOursource)
                {
                    _routingAPIRepository.UpdateRoutingPDISStatus(_factoryCode, materialNo, existMasterdata.PdisStatus, _token);
                }

                #endregion

                #region Save AutoPackingSpec
                autoPackingSpecAPIRepository.SaveAndUpdateAutoPackingSpecFromCusId(_factoryCode, transactionDataModelSession.modelProductCustomer.CusId, _username, materialNo, _token);
                #endregion

                #endregion
            }

            //update max step
            _newProductService.UpdateMaxProgress(_factoryCode, ref transactionDataModelSession);

            SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "TransactionDataModel", transactionDataModelSession);
        }

        public TransactionDataModel CustomerData(TransactionDataModel transactionDataModel)
        {
            TransactionDataModel model = new TransactionDataModel();

            try
            {
                model = SessionExtentions.GetSession<TransactionDataModel>(_httpContextAccessor.HttpContext.Session, "TransactionDataModel");
                model.modelProductCustomer = new ProductCustomer();
                model.modelProductCustomer = transactionDataModel.modelProductCustomer;
            }
            catch (Exception ex)
            {
                model.modelProductCustomer = new ProductCustomer();
            }

            return model;

        }
    }
}
