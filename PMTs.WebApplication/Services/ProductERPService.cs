using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Cmp;
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
    public class ProductERPService : IProductERPService
    {
        IHttpContextAccessor _httpContextAccessor;
        UserSessionModel userSessionModel;

        private readonly ICompanyProfileAPIRepository _companyProfileAPIRepository;
        private readonly IPlantViewAPIRepository _plantViewAPIRepository;
        private readonly ISalesViewAPIRepository _salesViewAPIRepository;
        private readonly INewProductService _newProductService;
        private readonly IRoutingAPIRepository _routingAPIRepository;
        private readonly IMasterDataAPIRepository _masterDataAPIRepository;
        private readonly ITransactionsDetailAPIRepository _transactionsDetailAPIRepository;

        private readonly string _username;
        private readonly string _saleOrg;
        private readonly string _factoryCode;
        private readonly string _token;

        public ProductERPService(
            IHttpContextAccessor httpContextAccessor,
            ICompanyProfileAPIRepository companyProfileAPIRepository,
            IPlantViewAPIRepository plantViewAPIRepository,
            ISalesViewAPIRepository salesViewAPIRepository,
            INewProductService newProductService,
            IRoutingAPIRepository routingAPIRepository,
            IMasterDataAPIRepository masterDataAPIRepository,
            ITransactionsDetailAPIRepository transactionsDetailAPIRepository
            )
        {
            // Initialize HttpContext
            _httpContextAccessor = httpContextAccessor;
            _companyProfileAPIRepository = companyProfileAPIRepository;
            _plantViewAPIRepository = plantViewAPIRepository;
            _salesViewAPIRepository = salesViewAPIRepository;
            _newProductService = newProductService;
            _routingAPIRepository = routingAPIRepository;
            _masterDataAPIRepository = masterDataAPIRepository;
            _transactionsDetailAPIRepository = transactionsDetailAPIRepository;

            // Initialize User Data From Session
            this.userSessionModel = SessionExtentions.GetSession<UserSessionModel>(_httpContextAccessor.HttpContext.Session, "UserSessionModel");
            if (userSessionModel != null)
            {
                _username = userSessionModel.UserName;
                _saleOrg = userSessionModel.SaleOrg;
                _factoryCode = userSessionModel.FactoryCode;
                _token = userSessionModel.Token;
            }
        }

        #region Controller calling

        public void GetProductERP(NewProductController newProductController, ref TransactionDataModel transactionDataModel)
        {

            transactionDataModel = SessionExtentions.GetSession<TransactionDataModel>(_httpContextAccessor.HttpContext.Session, "TransactionDataModel");

            _newProductService.SetTransactionStatus(ref transactionDataModel, "ERPInterface");

            if (transactionDataModel.modelCategories.MatCode == "82")
            {
                newProductController.ViewBag.cboPrdType = "BANC";
            }
            else if (transactionDataModel.modelCategories.MatCode == "84" || transactionDataModel.modelCategories.MatCode == "14" || transactionDataModel.modelCategories.MatCode == "24")
            {
                newProductController.ViewBag.cboPrdType = "LUMF";
            }
            else
            {
                newProductController.ViewBag.cboPrdType = "ZMTO";
            }
            GetProductERPData(ref transactionDataModel);

            SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "TransactionDataModel", transactionDataModel);
            newProductController.ViewBag.Saleorg = _saleOrg;
        }

        //plant view
        public void SavePlantViewERP(NewProductController newProductController, ref TransactionDataModel transactionDataModel)
        {
            string btnSave = "";
            TransactionDataModel model = new TransactionDataModel();

            GetPlantViewData(ref transactionDataModel);

            model = transactionDataModel;
            //check ค่า ซ้ำ
            bool chkplantDuplicate;
            chkplantDuplicate = ChkPlantMat(model, btnSave);
            if (chkplantDuplicate)//mook
            {
                newProductController.ViewBag.errorMassege = "Duplicated PlantView!!";
                //ViewBag.Alert = "Plant Data exist!!";
            }
            else
            {
                model = SavePlantViewData(model, btnSave);
                newProductController.ViewBag.Alert = null;
            }

            GetProductERPData(ref model);
            model.modelProductERP.PlantList = JsonConvert.DeserializeObject<List<CompanyProfile>>(_companyProfileAPIRepository.GetCompanyProfileList(_factoryCode, _token)).Select(s => new SelectListItem() { Value = s.Plant.ToString(), Text = s.Plant.ToString() });
            SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "TransactionDataModel", model);

            newProductController.ViewBag.SelectedTab = 0;
            if (model.modelCategories.MatCode == "82")
            {
                newProductController.ViewBag.cboPrdType = "BANC";
            }
            else
            {
                newProductController.ViewBag.cboPrdType = "ZMTO";
            }

            transactionDataModel = model;
        }

        //public void UpdatePlantViewERP(NewProductController newProductController, ref TransactionDataModel transactionDataModel, string btnUpdateERP)
        public void UpdatePlantViewERP(NewProductController newProductController, ref TransactionDataModel transactionDataModel)
        {
            GetPlantViewData(ref transactionDataModel);

            // UpdatePlantViewData(ref transactionDataModel, btnUpdateERP);
            UpdatePlantViewData(ref transactionDataModel);

            GetProductERPData(ref transactionDataModel);

            SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "TransactionDataModel", transactionDataModel);

            newProductController.ViewBag.SelectedTab = 0;
        }

        public void DeletePlantViewERP(NewProductController newProductController, ref TransactionDataModel transactionDataModel, int selectedTab, string btnDeleteERP)
        {
            GetPlantViewData(ref transactionDataModel);

            transactionDataModel = DeletePlantViewData(transactionDataModel, btnDeleteERP);

            GetProductERPData(ref transactionDataModel);
            SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "TransactionDataModel", transactionDataModel);

            newProductController.ViewBag.SelectedTab = selectedTab;
        }

        //sale view
        public void SaveSaleViewERP(NewProductController newProductController, ref TransactionDataModel transactionDataModel)
        {
            string btnSave = "";

            //     try
            //     {
            GetSaleViewData(ref transactionDataModel);

            //check ค่า ซ้ำ
            bool chkplantDuplicate;
            //string btnSave = "";
            chkplantDuplicate = ChkSaleMat(transactionDataModel, btnSave);
            if (chkplantDuplicate == true)
            {
                newProductController.ViewBag.Alert = "Duplicated SaleView!!";
            }
            else
            {
                transactionDataModel = SaveSaleViewData(transactionDataModel, btnSave);
                newProductController.ViewBag.Alert = null;
            }

            GetProductERPData(ref transactionDataModel);

            SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "TransactionDataModel", transactionDataModel);

            newProductController.ViewBag.SelectedTab = 1;

            if (transactionDataModel.modelCategories.MatCode == "82")
            {
                newProductController.ViewBag.cboPrdType = "BANC";
            }
            else
            {
                newProductController.ViewBag.cboPrdType = "ZMTO";
            }
        }

        public void UpdateSaleViewERP(NewProductController newProductController, ref TransactionDataModel transactionDataModel)
        {
            string btnSave = "";

            GetSaleViewData(ref transactionDataModel);

            //check ค่า ซ้ำ
            bool chkplantDuplicate;

            UpdateSaleViewData(transactionDataModel, btnSave);

            newProductController.ViewBag.Alert = null;

            GetProductERPData(ref transactionDataModel);
            SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "TransactionDataModel", transactionDataModel);

            newProductController.ViewBag.SelectedTab = 1;

            if (transactionDataModel.modelCategories.MatCode == "82")
            {
                newProductController.ViewBag.cboPrdType = "BANC";
            }
            else
            {
                newProductController.ViewBag.cboPrdType = "ZMTO";
            }
        }

        public void DeleteSaleViewERP(NewProductController newProductController, ref TransactionDataModel transactionDataModel, int selectedTab)
        {
            string btnSave = "";

            GetSaleViewData(ref transactionDataModel);

            //check ค่า ซ้ำ
            bool chkplantDuplicate;

            transactionDataModel = DeleteSaleViewData(transactionDataModel, btnSave);

            newProductController.ViewBag.Alert = null;

            GetProductERPData(ref transactionDataModel);

            SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "TransactionDataModel", transactionDataModel);

            newProductController.ViewBag.SelectedTab = 1;

            if (transactionDataModel.modelCategories.MatCode == "82")
            {
                newProductController.ViewBag.cboPrdType = "BANC";
            }
            else
            {
                newProductController.ViewBag.cboPrdType = "ZMTO";
            }
        }

        //Save PurchaseView
        public void SavePurchaseViewERP(NewProductController newProductController, ref TransactionDataModel transactionDataModel)
        {
            GetPurchaseViewData(ref transactionDataModel);

            transactionDataModel = SavePurchaseViewData(transactionDataModel);

            newProductController.ViewBag.Massege = " Save Success ";

            GetProductERPData(ref transactionDataModel);

            TransactionDataModel transactionDataModelSession = SessionExtentions.GetSession<TransactionDataModel>(_httpContextAccessor.HttpContext.Session, "TransactionDataModel");

            //update max step
            _newProductService.UpdateMaxProgress(_factoryCode, ref transactionDataModelSession);
            transactionDataModel.TransactionDetail.MaxStep = transactionDataModelSession.TransactionDetail.MaxStep;
            SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "TransactionDataModel", transactionDataModel);



            newProductController.ViewBag.SelectedTab = 3;
        }

        #endregion

        #region service
        //get data
        public void GetProductERPData(ref TransactionDataModel transactionDataModel)
        {
            transactionDataModel.modelProductERP = new ProductERPPlantViewModel();

            //GetPlant
            List<CompanyProfile> CompanyList = JsonConvert.DeserializeObject<List<CompanyProfile>>(_companyProfileAPIRepository.GetCompanyProfileList(_factoryCode, _token));
            CompanyList.Insert(0, new CompanyProfile { Id = 0, Plant = "" });
            transactionDataModel.modelProductERP.PlantList = CompanyList.Select(s => new SelectListItem() { Value = s.Plant.ToString(), Text = s.Plant.ToString() + " " + s.ShortName });

            //model.modelProductERP.PlantList = ProductERPRepository.GetListCompany(context).Select(s => new SelectListItem() { Value = s.Plant.ToString(), Text = s.Plant.ToString() });
            transactionDataModel.modelProductERP.PurchCode = JsonConvert.DeserializeObject<CompanyProfile>(_companyProfileAPIRepository.GetCompanyProfileByPlant(_factoryCode, _token)).PurchasGrp;
            //model.modelProductERP.PurchCode = ProductERPRepository.GetPuchCode(context, sessions.PlantCode);

            transactionDataModel.modelProductERP.ModelList = new List<PlantView>();

            var materialNo = transactionDataModel.MaterialNo;

            transactionDataModel.modelProductERP.ModelList = JsonConvert.DeserializeObject<List<PlantView>>(_plantViewAPIRepository.GetPlantViewsByMaterialNo(_factoryCode, materialNo, _token));

            //Sale View 
            transactionDataModel.modelProductERPSale = new ProductERPSaleViewModel();

            List<CompanyProfile> SaleOrgList = JsonConvert.DeserializeObject<List<CompanyProfile>>(_companyProfileAPIRepository.GetCompanyProfileList(_factoryCode, _token));

            transactionDataModel.modelProductERPSale.SaleOrgList = SaleOrgList.Select(s => new SelectListItem() { Value = s.SaleOrg.ToString(), Text = s.SaleOrg.ToString() + " " + s.ShortName });

            //transactionDataModel.modelProductERPSale.ModelListSale = JsonConvert.DeserializeObject<List<SalesView>>(_salesViewAPIRepository.GetSaleViewsByMaterialNoAndFactoryCode(_factoryCode, materialNo));
            transactionDataModel.modelProductERPSale.ModelListSale = JsonConvert.DeserializeObject<List<SalesView>>(_salesViewAPIRepository.GetSaleViewsByMaterialNo(_factoryCode, materialNo, _token));

            transactionDataModel.modelProductERPPurchase = new ProductERPPurchaseViewModel();

            transactionDataModel.modelProductERPPurchase = _newProductService.BindDataPurchaseByMatNo(materialNo);

            ///===== Tassanai Update 03092020 ==Start
            //if (transactionDataModel.EventFlag=="Create" && transactionDataModel.modelCategories.MatCode == "82")
            //{
            //    transactionDataModel.modelProductERPPurchase.PurTxt1 = transactionDataModel.modelProductInfo.PC;
            //    transactionDataModel.modelProductERPPurchase.PurTxt2 = transactionDataModel.modelProductInfo.SaleText1;
            //}

            if (transactionDataModel.modelCategories.MatCode == "82")
            {
                transactionDataModel.modelProductERPPurchase.PurTxt1 = transactionDataModel.modelProductInfo.PC;
                transactionDataModel.modelProductERPPurchase.PurTxt2 = transactionDataModel.modelProductInfo.SaleText1;
                transactionDataModel.modelProductERPPurchase.PurTxt3 = transactionDataModel.modelProductInfo.SaleText2;
                transactionDataModel.modelProductERPPurchase.PurTxt4 = transactionDataModel.modelProductInfo.SaleText3;
            }
            ///===== Tassanai Update 03092020 ==End
        }

        public void GetPlantViewData(ref TransactionDataModel transactionDataModel)
        {
            TransactionDataModel model = new TransactionDataModel();

            try
            {
                model = SessionExtentions.GetSession<TransactionDataModel>(_httpContextAccessor.HttpContext.Session, "TransactionDataModel");
                model.modelProductERP = new ProductERPPlantViewModel();
                model.modelProductERP = transactionDataModel.modelProductERP;
            }
            catch (Exception ex)
            {
                model = SessionExtentions.GetSession<TransactionDataModel>(_httpContextAccessor.HttpContext.Session, "TransactionDataModel");
                model.modelProductERP = new ProductERPPlantViewModel();
            }

            transactionDataModel = model;
        }

        public void GetSaleViewData(ref TransactionDataModel transactionDataModel)
        {
            TransactionDataModel model = new TransactionDataModel();

            try
            {

                model = SessionExtentions.GetSession<TransactionDataModel>(_httpContextAccessor.HttpContext.Session, "TransactionDataModel");
                model.modelProductERPSale = new ProductERPSaleViewModel();
                model.modelProductERPSale = transactionDataModel.modelProductERPSale;

            }
            catch (Exception ex)
            {
                model.modelProductERPSale = new ProductERPSaleViewModel();
            }
            transactionDataModel = model;
        }

        public void GetPurchaseViewData(ref TransactionDataModel transactionDataModel)
        {
            TransactionDataModel model = new TransactionDataModel();
            try
            {

                model = SessionExtentions.GetSession<TransactionDataModel>(_httpContextAccessor.HttpContext.Session, "TransactionDataModel");
                model.modelProductERPPurchase = new ProductERPPurchaseViewModel();
                model.modelProductERPPurchase = transactionDataModel.modelProductERPPurchase;
            }
            catch (Exception ex)
            {
                model.modelProductERPPurchase = new ProductERPPurchaseViewModel();
            }
            transactionDataModel = model;
        }

        //other funtional
        public bool ChkPlantMat(TransactionDataModel transactionData, string btnSave)
        {
            bool statusDupicate = true;
            var materialNo = transactionData.MaterialNo;
            var plantView = JsonConvert.DeserializeObject<PlantView>(_plantViewAPIRepository.GetPlantViewByPlant(_factoryCode, transactionData.MaterialNo, transactionData.modelProductERP.Plant, _token));

            if (plantView == null)
            {
                statusDupicate = false;
            }
            return statusDupicate;
        }

        public string GenPurch(string Plant)
        {
            return JsonConvert.DeserializeObject<CompanyProfile>(_companyProfileAPIRepository.GetCompanyProfileByPlant(_factoryCode, _token)).PurchasGrp.Trim();
        }

        public bool ChkSaleMat(TransactionDataModel SaleData, string btnSave)
        {
            bool statusDupicate = true;
            var Material_no = SaleData.MaterialNo;
            var saleOrg = SaleData.modelProductERPSale.SaleOrg;
            var Chanel = SaleData.modelProductERPSale.Channel;


            //var result2 = JsonConvert.DeserializeObject<SalesView>(_salesViewAPIRepository.GetSaleViewBySaleOrg(_factoryCode, Material_no, saleOrg,_token));

            var result = JsonConvert.DeserializeObject<SalesView>(_salesViewAPIRepository.GetSaleViewBySaleOrgChannel(_factoryCode, Material_no, saleOrg, Chanel, _token));


            //context.SalesView.Where(x => x.MaterialNo == Material_no && x.SaleOrg == SaleData.modelProductERPSale.SaleOrg && x.PdisStatus != "X").FirstOrDefault();
            if (result == null)
            {
                statusDupicate = false;
            }
            return statusDupicate;
        }

        //Plant view service call api
        public TransactionDataModel SavePlantViewData(TransactionDataModel transactionDataModel, string btnSave)
        {
            //Check EOF PlantView ซ้ำหรือไม่
            // string alerttxt;
            TransactionDataModel model = new TransactionDataModel();
            PlantView plantView = new PlantView();

            var materialNo = transactionDataModel.MaterialNo;
            plantView = JsonConvert.DeserializeObject<PlantView>(_plantViewAPIRepository.GetPlantViewByPlant(_factoryCode, materialNo, transactionDataModel.modelProductERP.Plant, _token));

            var transactionDetailObject = JsonConvert.DeserializeObject<TransactionsDetail>(_transactionsDetailAPIRepository.GetTransactionsDetailByMat(_factoryCode, materialNo, _token));
            var isOursource = transactionDetailObject != null && transactionDetailObject.Outsource ? true : false;

            if (plantView == null)
            {
                //check routing Ship
                var chkship = JsonConvert.DeserializeObject<int>(_routingAPIRepository.GetNumberOfRoutingByShipBlk(_factoryCode, materialNo, true, _token));

                string Ship_Blk = "X";
                string Ship = "X";
                string PDIS_Status = "M";
                bool Tran_Status = false;
                bool SAP_Status = true;
                string PDIST = "M";

                var masterData = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNo(_factoryCode, materialNo, _token));

                if (masterData != null)
                {
                    if (masterData.TranStatus == false && masterData.SapStatus == false)
                    {
                        PDIS_Status = "C";
                        Tran_Status = isOursource ? true : false;
                        SAP_Status = false;
                        PDIST = "C";
                    }
                    else if ((masterData.TranStatus == null || masterData.SapStatus == null))
                    {
                        PDIS_Status = "C";
                        Tran_Status = isOursource ? true : false;
                        SAP_Status = false;
                        PDIST = "C";
                    }

                    if (chkship == 0)
                    {
                        Ship_Blk = "";
                        Ship = "";
                    }

                    // add new record
                    plantView = new PlantView
                    {
                        MaterialNo = materialNo, // ค่าตั้งต้นก่อน
                        FactoryCode = _factoryCode,
                        Plant = transactionDataModel.modelProductERP.Plant,
                        PurchCode = transactionDataModel.modelProductERP.PurchCode,
                        StdTotalCost = transactionDataModel.modelProductERP.StdtotalCost.Value,
                        StdMovingCost = 0,
                        StdFc = 0,
                        StdVc = 0,
                        ShipBlk = Ship_Blk,
                        PdisStatus = PDIS_Status,
                        TranStatus = Tran_Status,
                        SapStatus = SAP_Status
                    };

                    var plantViewParent = new ParentModel
                    {
                        AppName = Globals.AppNameEncrypt,
                        SaleOrg = _saleOrg,
                        PlantCode = _factoryCode,
                        PlantView = plantView
                    };

                    _plantViewAPIRepository.SavePlantView(JsonConvert.SerializeObject(plantViewParent), _token);


                    //Update masterdata
                    if (Tran_Status == true)
                    {
                        switch (PDIS_Status)
                        {
                            case "C":
                                if (SAP_Status = true)
                                {
                                    PDIS_Status = "D";
                                    Tran_Status = false;
                                }
                                else
                                {
                                    Tran_Status = false;
                                }
                                break;
                            case "D":
                                Tran_Status = false;
                                break;
                            case "M":
                                Tran_Status = false;
                                break;
                            case "X":
                                if (SAP_Status = false)
                                {
                                    Tran_Status = true;
                                    SAP_Status = true;
                                }
                                else
                                {
                                    Tran_Status = false;
                                }
                                break;
                            case null:
                                SAP_Status = false;
                                PDIS_Status = "C";
                                Tran_Status = false;

                                break;
                        }

                        //update master data
                        var masterDataTemp = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNo(_factoryCode, materialNo, _token));
                        masterDataTemp.SapStatus = isOursource ? false : SAP_Status;
                        masterDataTemp.PdisStatus = PDIS_Status;
                        masterDataTemp.TranStatus = isOursource ? true : Tran_Status;
                        masterDataTemp.User = _username;

                        var masterDataParent = new ParentModel
                        {
                            AppName = Globals.AppNameEncrypt,
                            SaleOrg = _saleOrg,
                            PlantCode = _factoryCode,
                            MasterData = masterDataTemp
                        };

                        _masterDataAPIRepository.UpdateMasterData(JsonConvert.SerializeObject(masterDataParent), _token);
                    }
                }
            }
            else
            {
                //ค่าซ้ำ    
                //UpdatePlantViewData(ref transactionDataModel, btnSave);
                UpdatePlantViewData(ref transactionDataModel);

            }

            // TransactionDataModel model = new TransactionDataModel();
            model = SessionExtentions.GetSession<TransactionDataModel>(_httpContextAccessor.HttpContext.Session, "TransactionDataModel");
            model.modelProductERP = new ProductERPPlantViewModel();
            //   model.modelProductProp = Propdata.modelProductProp;
            // model.modelProductProp = new ProductPropViewModel();
            return model;
        }

        //public void UpdatePlantViewData(ref TransactionDataModel transactionDataModel, string UpdateBtn)
        public void UpdatePlantViewData(ref TransactionDataModel transactionDataModel)
        {
            //Check EOF PlantView ซ้ำหรือไม่
            // string alerttxt;
            TransactionDataModel model = new TransactionDataModel();
            PlantView plantView = new PlantView();

            var materialNo = transactionDataModel.MaterialNo;
            plantView = JsonConvert.DeserializeObject<PlantView>(_plantViewAPIRepository.GetPlantViewByMaterialNoAndPlant(_factoryCode, materialNo, transactionDataModel.modelProductERP.Plant, _token));
            var chkship = JsonConvert.DeserializeObject<int>(_routingAPIRepository.GetNumberOfRoutingByShipBlk(_factoryCode, materialNo, true, _token));
            var transactionDetailObject = JsonConvert.DeserializeObject<TransactionsDetail>(_transactionsDetailAPIRepository.GetTransactionsDetailByMat(_factoryCode, materialNo, _token));
            var isOursource = transactionDetailObject != null && transactionDetailObject.Outsource ? true : false;

            string Ship_Blk = "X";
            string Ship = "X";

            var masterData = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNo(_factoryCode, materialNo, _token));

            string PDIS_Status = "M";
            bool Tran_Status = false;
            bool SAP_Status = true;
            string PDIST = "M";

            if (masterData.TranStatus == false && masterData.SapStatus == false)
            {
                PDIS_Status = "C";
                Tran_Status = false;
                SAP_Status = false;
                PDIST = "C";
            }
            else if ((masterData.TranStatus == null || masterData.SapStatus == null))
            {
                PDIS_Status = "C";
                Tran_Status = false;
                SAP_Status = false;
                PDIST = "C";
            }

            if (chkship == 0)
            {
                Ship_Blk = "";
                Ship = "";
            }

            //Update masterdata
            if (Tran_Status == true)
            {
                switch (PDIS_Status)
                {
                    case "C":
                        if (SAP_Status = true)
                        {
                            PDIS_Status = "D";
                            Tran_Status = false;
                        }
                        else
                        {
                            Tran_Status = false;
                        }
                        break;
                    case "D":
                        Tran_Status = false;
                        break;
                    case "M":
                        Tran_Status = false;
                        break;
                    case "X":
                        if (SAP_Status = false)
                        {
                            Tran_Status = true;
                            SAP_Status = true;
                        }
                        else
                        {
                            Tran_Status = false;
                        }
                        break;
                    case null:
                        SAP_Status = false;
                        PDIS_Status = "C";
                        Tran_Status = false;

                        break;
                }
            }

            // add new record
            if (plantView != null || masterData != null)
            {
                var plantViewId = plantView.Id;
                plantView = new PlantView
                {
                    Id = plantViewId,
                    MaterialNo = materialNo, // ค่าตั้งต้นก่อน
                    FactoryCode = _factoryCode,
                    Plant = transactionDataModel.modelProductERP.Plant,
                    PurchCode = transactionDataModel.modelProductERP.PurchCode,
                    StdTotalCost = transactionDataModel.modelProductERP.StdtotalCost.Value,
                    StdMovingCost = 0,
                    StdFc = 0,
                    StdVc = 0,
                    ShipBlk = Ship_Blk,
                    PdisStatus = PDIS_Status,
                    TranStatus = isOursource ? true : Tran_Status,
                    SapStatus = SAP_Status
                };



                var plantViewParent = new ParentModel
                {
                    AppName = Globals.AppNameEncrypt,
                    SaleOrg = _saleOrg,
                    PlantCode = _factoryCode,
                    PlantView = plantView
                };

                masterData.SapStatus = isOursource ? false : SAP_Status;
                masterData.PdisStatus = PDIS_Status;
                masterData.TranStatus = isOursource ? true : Tran_Status;
                masterData.User = _username;
                var masterDataParent = new ParentModel
                {
                    AppName = Globals.AppNameEncrypt,
                    SaleOrg = _saleOrg,
                    PlantCode = _factoryCode,
                    MasterData = masterData
                };

                //update platview and master data
                _plantViewAPIRepository.UpdatePlantView(JsonConvert.SerializeObject(plantViewParent), _token);
                _masterDataAPIRepository.UpdateMasterData(JsonConvert.SerializeObject(masterDataParent), _token);
            }

            model = SessionExtentions.GetSession<TransactionDataModel>(_httpContextAccessor.HttpContext.Session, "TransactionDataModel");
            model.modelProductERP = new ProductERPPlantViewModel();

            transactionDataModel = model;
        }

        public TransactionDataModel DeletePlantViewData(TransactionDataModel transactionDataModel, string btnDeleteERP)
        {
            //   TransactionDataModel model = new TransactionDataModel();
            var materialNo = transactionDataModel.MaterialNo;

            var plantView = JsonConvert.DeserializeObject<PlantView>(_plantViewAPIRepository.GetPlantViewByMaterialNoAndPlant(_factoryCode, materialNo, transactionDataModel.modelProductERP.Plant, _token));
            var masterData = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNo(_factoryCode, materialNo, _token));

            if (plantView != null && masterData != null)
            {
                plantView.PdisStatus = "X";
                plantView.TranStatus = false;

                var plantViewParent = new ParentModel
                {
                    AppName = Globals.AppNameEncrypt,
                    SaleOrg = _saleOrg,
                    PlantCode = _factoryCode,
                    PlantView = plantView
                };

                masterData.TranStatus = false;
                masterData.User = _username;
                var masterDataParent = new ParentModel
                {
                    AppName = Globals.AppNameEncrypt,
                    SaleOrg = _saleOrg,
                    PlantCode = _factoryCode,
                    MasterData = masterData
                };

                //update platview and master data
                _plantViewAPIRepository.UpdatePlantView(JsonConvert.SerializeObject(plantViewParent), _token);
                _masterDataAPIRepository.UpdateMasterData(JsonConvert.SerializeObject(masterDataParent), _token);
            }



            transactionDataModel = SessionExtentions.GetSession<TransactionDataModel>(_httpContextAccessor.HttpContext.Session, "TransactionDataModel");

            return transactionDataModel;
        }

        //Sale view service call api
        public TransactionDataModel SaveSaleViewData(TransactionDataModel transactionDataModel, string btnSave)
        {
            TransactionDataModel model = new TransactionDataModel();
            //Check EOF PlantView ซ้ำหรือไม่
            // string alerttxt;
            var materialNo = transactionDataModel.MaterialNo;
            //var salesView = JsonConvert.DeserializeObject<SalesView>(_salesViewAPIRepository.GetSaleViewBySaleOrg(_factoryCode, materialNo, transactionDataModel.modelProductERPSale.SaleOrg,_token));

            var salesView = JsonConvert.DeserializeObject<SalesView>(_salesViewAPIRepository.GetSaleViewBySaleOrgChannel(_factoryCode, materialNo, transactionDataModel.modelProductERPSale.SaleOrg, transactionDataModel.modelProductERPSale.Channel, _token));
            //get devplant 
            var companyProfiles = new List<CompanyProfile>();
            companyProfiles = JsonConvert.DeserializeObject<List<CompanyProfile>>(_companyProfileAPIRepository.GetCompanyProfileBySaleOrg(_factoryCode, transactionDataModel.modelProductERPSale.SaleOrg, _token));
            var transactionDetailObject = JsonConvert.DeserializeObject<TransactionsDetail>(_transactionsDetailAPIRepository.GetTransactionsDetailByMat(_factoryCode, materialNo, _token));
            var isOursource = transactionDetailObject != null && transactionDetailObject.Outsource ? true : false;

            if (salesView == null)

            {
                //check routing Ship
                var chkship = JsonConvert.DeserializeObject<int>(_routingAPIRepository.GetNumberOfRoutingByShipBlk(_factoryCode, materialNo, true, _token));

                string Ship_Blk = "X";
                string Ship = "X";

                var masterData = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNo(_factoryCode, materialNo, _token));
                MasterData master = new MasterData
                {
                    TranStatus = masterData.TranStatus,
                    SapStatus = masterData.SapStatus
                };
                string PDIS_Status = "M";
                bool Tran_Status = false;
                bool SAP_Status = true;
                string PDIST = "M";


                if (master.TranStatus == false && master.SapStatus == false)
                {
                    PDIS_Status = "C";
                    Tran_Status = false;
                    SAP_Status = false;
                    PDIST = "C";
                }
                else if ((master.TranStatus == null || master.SapStatus == null))
                {
                    PDIS_Status = "C";
                    Tran_Status = false;
                    SAP_Status = false;
                    PDIST = "C";
                }

                if (chkship == 0)
                {
                    Ship_Blk = "";
                    Ship = "";
                }
                string saleOrg = transactionDataModel.modelProductERPSale.SaleOrg;

                var saleOrg4Digit = saleOrg.Split(' ');
                // add new record
                SalesView saleV = new SalesView
                {
                    Id = 0,
                    MaterialNo = materialNo, // ค่าตั้งต้นก่อน
                    //SaleOrg = transactionDataModel.modelProductERPSale.SaleOrg,
                    SaleOrg = saleOrg4Digit[0],
                    FactoryCode = _factoryCode,
                    Channel = transactionDataModel.modelProductERPSale.Channel,
                    //DevPlant = _factoryCode,
                    DevPlant = companyProfiles[0].Plant,
                    CustCode = transactionDataModel.modelProductCustomer.CustCode,
                    MinQty = transactionDataModel.modelProductERPSale.MinQty,
                    OrderType = transactionDataModel.modelProductERPSale.OrderType,
                    PdisStatus = PDIS_Status,
                    TranStatus = isOursource ? true : Tran_Status,
                    SapStatus = SAP_Status,
                    PriceAdj = 0,
                    NewPrice = 0,
                    OldPrice = 0,
                    SaleUnitPrice = 0


                };

                //save saleview
                var saleViewParent = new ParentModel
                {
                    AppName = Globals.AppNameEncrypt,
                    SaleOrg = _saleOrg,
                    PlantCode = _factoryCode,
                    SalesView = saleV
                };

                _salesViewAPIRepository.SaveSaleView(JsonConvert.SerializeObject(saleViewParent), _token);


                //Update masterdata
                if (Tran_Status == true)
                {
                    switch (PDIS_Status)
                    {
                        case "C":
                            if (SAP_Status = true)
                            {
                                PDIS_Status = "D";
                                Tran_Status = false;
                            }
                            else
                            {
                                Tran_Status = false;
                            }
                            break;
                        case "D":
                            Tran_Status = false;
                            break;
                        case "M":
                            Tran_Status = false;
                            break;
                        case "X":
                            if (SAP_Status = false)
                            {
                                Tran_Status = true;
                                SAP_Status = true;
                            }
                            else
                            {
                                Tran_Status = false;
                            }
                            break;
                        case null:
                            SAP_Status = false;
                            PDIS_Status = "C";
                            Tran_Status = false;
                            break;
                    }

                    //update master data
                    var masterDataTemp = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNo(_factoryCode, materialNo, _token));

                    masterData.SapStatus = isOursource ? false : SAP_Status;
                    masterData.PdisStatus = PDIS_Status;
                    masterData.TranStatus = isOursource ? true : Tran_Status;
                    masterData.User = _username;
                    var masterDataParent = new ParentModel
                    {
                        AppName = Globals.AppNameEncrypt,
                        SaleOrg = _saleOrg,
                        PlantCode = _factoryCode,
                        MasterData = masterDataTemp
                    };

                    _masterDataAPIRepository.UpdateMasterData(JsonConvert.SerializeObject(masterDataParent), _token);
                }
            }
            else
            {
                //ค่าซ้ำ    
                UpdateSaleViewData(transactionDataModel, btnSave);
            }

            // TransactionDataModel model = new TransactionDataModel();
            model = SessionExtentions.GetSession<TransactionDataModel>(_httpContextAccessor.HttpContext.Session, "TransactionDataModel");

            model.modelProductERP = new ProductERPPlantViewModel();
            //   model.modelProductProp = Propdata.modelProductProp;
            // model.modelProductProp = new ProductPropViewModel();
            return model;
        }

        public TransactionDataModel UpdateSaleViewData(TransactionDataModel transactionDataModel, string UpdateBtn)
        {
            TransactionDataModel model = new TransactionDataModel();
            var saleView = new SalesView();
            //Check EOF PlantView ซ้ำหรือไม่
            // string alerttxt;
            var materialNo = transactionDataModel.MaterialNo;
            //saleView = JsonConvert.DeserializeObject<SalesView>(_salesViewAPIRepository.GetSaleViewBySaleOrg(_factoryCode, materialNo, transactionDataModel.modelProductERPSale.SaleOrg,_token));
            saleView = JsonConvert.DeserializeObject<SalesView>(_salesViewAPIRepository.GetSaleViewBySaleOrgChannel(_factoryCode, materialNo, transactionDataModel.modelProductERPSale.SaleOrg, transactionDataModel.modelProductERPSale.Channel, _token));

            var masterData = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNo(_factoryCode, materialNo, _token));
            var transactionDetailObject = JsonConvert.DeserializeObject<TransactionsDetail>(_transactionsDetailAPIRepository.GetTransactionsDetailByMat(_factoryCode, materialNo, _token));
            var isOursource = transactionDetailObject != null && transactionDetailObject.Outsource ? true : false;

            if (saleView != null || masterData != null)
            {
                //check routing Ship
                var chkship = JsonConvert.DeserializeObject<int>(_routingAPIRepository.GetNumberOfRoutingByShipBlk(_factoryCode, materialNo, true, _token));

                string Ship_Blk = "X";
                string Ship = "X";
                string PDIS_Status = "M";
                bool Tran_Status = false;
                bool SAP_Status = true;
                string PDIST = "M";


                if (masterData.TranStatus == false && masterData.SapStatus == false)
                {
                    PDIS_Status = "C";
                    Tran_Status = false;
                    SAP_Status = false;
                    PDIST = "C";
                }
                else if ((masterData.TranStatus == null || masterData.SapStatus == null))
                {
                    PDIS_Status = "C";
                    Tran_Status = false;
                    SAP_Status = false;
                    PDIST = "C";
                }

                if (chkship == 0)
                {
                    Ship_Blk = "";
                    Ship = "";
                }

                //Update masterdata
                if (Tran_Status == true)
                {
                    switch (PDIS_Status)
                    {
                        case "C":
                            if (SAP_Status = true)
                            {
                                PDIS_Status = "D";
                                Tran_Status = false;
                            }
                            else
                            {
                                Tran_Status = false;
                            }
                            break;
                        case "D":
                            Tran_Status = false;
                            break;
                        case "M":
                            Tran_Status = false;
                            break;
                        case "X":
                            if (SAP_Status = false)
                            {
                                Tran_Status = true;
                                SAP_Status = true;
                            }
                            else
                            {
                                Tran_Status = false;
                            }
                            break;
                        case null:
                            SAP_Status = false;
                            PDIS_Status = "C";
                            Tran_Status = false;
                            break;
                    }
                }

                // add new record
                var salesViewId = saleView.Id;
                saleView = new SalesView
                {
                    Id = salesViewId,
                    FactoryCode = _factoryCode,
                    MaterialNo = materialNo, // ค่าตั้งต้นก่อน
                    SaleOrg = transactionDataModel.modelProductERPSale.SaleOrg,
                    Channel = transactionDataModel.modelProductERPSale.Channel,
                    DevPlant = _factoryCode,
                    CustCode = transactionDataModel.modelProductCustomer.CustCode,
                    MinQty = transactionDataModel.modelProductERPSale.MinQty,
                    OrderType = transactionDataModel.modelProductERPSale.OrderType,
                    PdisStatus = PDIS_Status,
                    TranStatus = isOursource ? true : Tran_Status,
                    SapStatus = SAP_Status,
                    PriceAdj = 0,
                    NewPrice = 0,
                    OldPrice = 0,
                    SaleUnitPrice = 0
                };


                var saleViewParent = new ParentModel
                {
                    AppName = Globals.AppNameEncrypt,
                    SaleOrg = _saleOrg,
                    PlantCode = _factoryCode,
                    SalesView = saleView
                };

                masterData.SapStatus = isOursource ? false : SAP_Status;
                masterData.PdisStatus = PDIS_Status;
                masterData.TranStatus = isOursource ? true : Tran_Status;
                masterData.User = _username;
                var masterDataParent = new ParentModel
                {
                    AppName = Globals.AppNameEncrypt,
                    SaleOrg = _saleOrg,
                    PlantCode = _factoryCode,
                    MasterData = masterData
                };

                //update saleview and master data
                _salesViewAPIRepository.UpdateSaleView(JsonConvert.SerializeObject(saleViewParent), _token);
                _masterDataAPIRepository.UpdateMasterData(JsonConvert.SerializeObject(masterDataParent), _token);
            }
            else
            {
                //ค่าซ้ำ                

            }

            // TransactionDataModel model = new TransactionDataModel();
            model = SessionExtentions.GetSession<TransactionDataModel>(_httpContextAccessor.HttpContext.Session, "TransactionDataModel");
            model.modelProductERP = new ProductERPPlantViewModel();
            //   model.modelProductProp = Propdata.modelProductProp;
            // model.modelProductProp = new ProductPropViewModel();
            return model;
        }

        public TransactionDataModel DeleteSaleViewData(TransactionDataModel transactionDataModel, string UpdateBtn)
        {
            TransactionDataModel model = new TransactionDataModel();

            model = SessionExtentions.GetSession<TransactionDataModel>(_httpContextAccessor.HttpContext.Session, "TransactionDataModel");

            var materialNo = transactionDataModel.MaterialNo;

            var saleView = JsonConvert.DeserializeObject<SalesView>(_salesViewAPIRepository.GetSaleViewBySaleOrgChannel(_factoryCode, materialNo, transactionDataModel.modelProductERPSale.SaleOrg, transactionDataModel.modelProductERPSale.Channel, _token));
            var masterData = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNo(_factoryCode, materialNo, _token));

            if (saleView != null || masterData != null)
            {
                //var saleViewId = saleView.Id;

                //saleView = new SalesView
                //{
                //    Id = saleViewId,
                //    FactoryCode = saleView.FactoryCode,
                //    MaterialNo = materialNo, // ค่าตั้งต้นก่อน
                //    SaleOrg = transactionDataModel.modelProductERPSale.SaleOrg,
                //    Channel = transactionDataModel.modelProductERPSale.Channel,
                //    DevPlant = transactionDataModel.modelProductERPSale.DevPlant,
                //    CustCode = transactionDataModel.modelProductERPSale.CustCode,
                //    MinQty = transactionDataModel.modelProductERPSale.MinQty,
                //    OrderType = transactionDataModel.modelProductERPSale.OrderType,
                //    PdisStatus = "X",
                //    TranStatus = false
                //};
                saleView.PdisStatus = "X";
                saleView.TranStatus = false;
            }

            //update saleview by materialNo,Channel and update master data
            var saleViewParent = new ParentModel
            {
                AppName = Globals.AppNameEncrypt,
                SaleOrg = _saleOrg,
                PlantCode = _factoryCode,
                SalesView = saleView
            };

            masterData.TranStatus = false;
            masterData.User = _username;

            var masterDataParent = new ParentModel
            {
                AppName = Globals.AppNameEncrypt,
                SaleOrg = _saleOrg,
                PlantCode = _factoryCode,
                MasterData = masterData
            };

            //update saleview and master data
            _salesViewAPIRepository.UpdateSaleView(JsonConvert.SerializeObject(saleViewParent), _token);
            _masterDataAPIRepository.UpdateMasterData(JsonConvert.SerializeObject(masterDataParent), _token);

            return model;
        }
        //Purchase view service call api
        public TransactionDataModel SavePurchaseViewData(TransactionDataModel transactionDataModel)
        {
            TransactionDataModel model = new TransactionDataModel();

            //Check EOF PlantView ซ้ำหรือไม่
            var materialNo = transactionDataModel.MaterialNo;
            var masterData = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNo(_factoryCode, materialNo, _token));

            if (masterData.PdisStatus != "N" && masterData.SapStatus == true)
            {
                masterData.PdisStatus = "M";
                masterData.TranStatus = false;
            }

            if (masterData != null)
            {
                masterData.PurTxt1 = transactionDataModel.modelProductERPPurchase.PurTxt1;
                masterData.PurTxt2 = transactionDataModel.modelProductERPPurchase.PurTxt2;
                masterData.PurTxt3 = transactionDataModel.modelProductERPPurchase.PurTxt3;
                masterData.PurTxt4 = transactionDataModel.modelProductERPPurchase.PurTxt4;
                masterData.User = _username;
                //masterData.TranStatus = false;

                var masterDataParent = new ParentModel
                {
                    AppName = Globals.AppNameEncrypt,
                    SaleOrg = _saleOrg,
                    PlantCode = _factoryCode,
                    MasterData = masterData,
                    //FactoryCode = _factoryCode

                };

                //update master data
                _masterDataAPIRepository.UpdateMasterData(JsonConvert.SerializeObject(masterDataParent), _token);
            }

            // TransactionDataModel model = new TransactionDataModel();
            model = SessionExtentions.GetSession<TransactionDataModel>(_httpContextAccessor.HttpContext.Session, "TransactionDataModel");
            model.modelProductERP = new ProductERPPlantViewModel();
            //   model.modelProductProp = Propdata.modelProductProp;
            // model.modelProductProp = new ProductPropViewModel();
            return model;
        }
        #endregion
    }
}
