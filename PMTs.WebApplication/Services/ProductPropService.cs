using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using PMTs.DataAccess;
using PMTs.DataAccess.ComplexModel;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.NewProduct;
using PMTs.DataAccess.Repository;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Shared;
using PMTs.DataAccess.Tracing;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services
{
    [TraceAspect]
    public class ProductPropService : IProductPropService
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IJoinAPIRepository _joinAPIRepository;
        private readonly IPrintMethodAPIRepository _printMethodAPIRepository;
        private readonly IPalletAPIRepository _palletAPIRepository;
        private readonly IChangeHistoryAPIRepository _changeHistoryAPIRepository;
        private readonly IMasterDataAPIRepository _masterDataAPIRepository;
        private readonly ITransactionsDetailAPIRepository _transactionsDetailAPIRepository;
        private readonly INewProductService _newProductService;
        private readonly IFluteAPIRepository _fluteAPIRepository;
        private readonly IPMTsConfigAPIRepository _pmtsConfigAPIRepository;
        private readonly IExtensionService _extensionService;
        private readonly IStandardPatternNameAPIRepository _standardPatternNameAPIRepository;
        //Tassanai Update 11/01/2021
        private readonly IJoinCharacterRepository _joinCharacterRepository;
        //Tassanai Update 12/05/2022
        private readonly IPpcBoiStatusAPIRepository _ppcBoiStatusAPIRepository;
        private readonly IPpcWorkTypeAPIRepository _ppcWorkTypeAPIRepository;

        private readonly IFormulaAPIRepository _formulaAPIRepository;


        private readonly string _username;
        private readonly string _saleOrg;
        private readonly string _factoryCode;
        private readonly string _token;
        private readonly string _businessGroup;

        public ProductPropService(
            IHttpContextAccessor httpContextAccessor,
            IJoinAPIRepository joinAPIRepository,
            IPrintMethodAPIRepository printMethodAPIRepository,
            IPalletAPIRepository palletAPIRepository,
            IChangeHistoryAPIRepository changeHistoryAPIRepository,
            IMasterDataAPIRepository masterDataAPIRepository,
            INewProductService newProductService,
            IFluteAPIRepository fluteAPIRepository,
            IPMTsConfigAPIRepository pmtsConfigAPIRepository,
            IExtensionService extensionService,
            ITransactionsDetailAPIRepository transactionsDetailAPIRepository,
            IStandardPatternNameAPIRepository standardPatternNameAPIRepository,
              IJoinCharacterRepository joinCharacterRepository,
              IPpcBoiStatusAPIRepository ppcBoiStatusAPIRepository,//Tassanai Update 11/01/2021
              IPpcWorkTypeAPIRepository ppcWorkTypeAPIRepository,//Tassanai Update 11/01/2021
              IFormulaAPIRepository formulaAPIRepository

            )
        {
            _httpContextAccessor = httpContextAccessor;
            _joinAPIRepository = joinAPIRepository;
            _printMethodAPIRepository = printMethodAPIRepository;
            _palletAPIRepository = palletAPIRepository;
            _changeHistoryAPIRepository = changeHistoryAPIRepository;
            _masterDataAPIRepository = masterDataAPIRepository;
            _newProductService = newProductService;
            _fluteAPIRepository = fluteAPIRepository;
            _pmtsConfigAPIRepository = pmtsConfigAPIRepository;
            _extensionService = extensionService;
            _transactionsDetailAPIRepository = transactionsDetailAPIRepository;
            _standardPatternNameAPIRepository = standardPatternNameAPIRepository;
            _joinCharacterRepository = joinCharacterRepository;  //Tassanai Update 11/01/2021
            _ppcBoiStatusAPIRepository = ppcBoiStatusAPIRepository;
            _ppcWorkTypeAPIRepository = ppcWorkTypeAPIRepository;
            _formulaAPIRepository = formulaAPIRepository;


            // Initialize User Data From Session
            var userSessionModel = SessionExtentions.GetSession<UserSessionModel>(_httpContextAccessor.HttpContext.Session, "UserSessionModel");
            if (userSessionModel != null)
            {
                _username = userSessionModel.UserName;
                _saleOrg = userSessionModel.SaleOrg;
                _factoryCode = userSessionModel.FactoryCode;
                _token = userSessionModel.Token;
                _businessGroup = userSessionModel.BusinessGroup;
            }
        }

        public void ProductPropData(ref TransactionDataModel transactionDataModel)
        {

            var transactionDataModelSession = SessionExtentions.GetSession<TransactionDataModel>(_httpContextAccessor.HttpContext.Session, "TransactionDataModel");
            if (transactionDataModelSession.modelProductProp == null)
            {
                transactionDataModelSession.modelProductProp = new ProductPropViewModel();

            }
            transactionDataModelSession.modelProductPropChangeHis = new ProductPropChangeHisViewModel();
            //  List<Joint> joinList = JsonConvert.DeserializeObject<List<Joint>>(_joinAPIRepository.GetJoinList(_factoryCode));
            //  transactionDataModelSession.modelProductProp.JoinList = joinList.Select(s => new SelectListItem() { Value = s.JointName.ToString(), Text = s.JointName.ToString() });
            transactionDataModelSession.modelProductProp.JoinList = new List<JointViewModel>();
            transactionDataModelSession.modelProductProp.JoinList = JsonConvert.DeserializeObject<List<JointViewModel>>(_joinAPIRepository.GetJoinList(_factoryCode, _token));


            transactionDataModelSession.modelProductProp.PrintMethodList = new List<PrintMethodViewModel>();
            transactionDataModelSession.modelProductProp.PrintMethodList = JsonConvert.DeserializeObject<List<PrintMethodViewModel>>(_printMethodAPIRepository.GetPrintMethodList(_factoryCode, _token));
            //Tassanai Update 11/01/2021
            transactionDataModelSession.modelProductProp.JoinCharacterList = new List<JoinCharacterViewModel>();
            transactionDataModelSession.modelProductProp.JoinCharacterList = JsonConvert.DeserializeObject<List<JoinCharacterViewModel>>(_joinCharacterRepository.GetJoinCharacterList(_factoryCode, _token));
            //Tassanai Update 12/05/2022
            transactionDataModelSession.modelProductProp.PpcBoiStatusList = new List<PpcBoiStatus>();
            //var tmp = JsonConvert.DeserializeObject<List<CompanyProfile>>(_companyProfileAPIRepository.GetCompanyProfileList(_factoryCode, _token));
            //transactionDataModelSession.modelProductProp.BoiStatus = "NONE BOI";
            transactionDataModelSession.modelProductProp.PpcWorkTypeList = new List<PpcWorkType>();
            //transactionDataModelSession.modelProductProp.WorkType = "NEW";


            #region Offset

            if (_businessGroup == "Offset")
            {
                transactionDataModelSession.modelProductProp.PpcBoiStatusList = JsonConvert.DeserializeObject<List<PpcBoiStatus>>(_ppcBoiStatusAPIRepository.GetPpcBoiStatusList(_factoryCode, _token));
                transactionDataModelSession.modelProductProp.PpcWorkTypeList = JsonConvert.DeserializeObject<List<PpcWorkType>>(_ppcWorkTypeAPIRepository.GetPpcWorkTypeList(_factoryCode, _token));

            }
            #endregion
            var list = transactionDataModelSession.modelProductProp.PrintMethodList.Where(p => p.Method == transactionDataModelSession.modelProductProp.PrintMethod);

            int? AmountColor = 0;

            foreach (var item in list)
            {
                AmountColor = item.AmountColor;
            }
            transactionDataModelSession.modelProductProp.AmountColor = AmountColor;

            if (transactionDataModelSession.modelProductSpec != null && transactionDataModelSession.modelProductSpec.Flute != null)
            {
                var flu = JsonConvert.DeserializeObject<Flute>(_fluteAPIRepository.GetFluteByFlute(_factoryCode, transactionDataModelSession.modelProductSpec.Flute, _token));

                if (flu != null)
                {
                    transactionDataModelSession.modelProductProp.Bun = transactionDataModelSession.modelProductProp.Bun.HasValue ? transactionDataModelSession.modelProductProp.Bun : flu.BundlePiece;
                    transactionDataModelSession.modelProductProp.BoxPerBundleNoJoint = flu.BoxPerBundleNoJoint == null ? 0 : flu.BoxPerBundleNoJoint;
                    transactionDataModelSession.modelProductProp.LayerPerPalletNoJoint = flu.LayerPerPalletNoJoint == null ? 0 : flu.LayerPerPalletNoJoint;
                    transactionDataModelSession.modelProductProp.LayerPallet = transactionDataModelSession.modelProductProp.LayerPallet.HasValue ? transactionDataModelSession.modelProductProp.LayerPallet : flu.LayerPallet;
                }
            }

            transactionDataModelSession.modelProductProp.PalletSize = string.IsNullOrEmpty(transactionDataModelSession.modelProductProp.PalletSize) || string.IsNullOrWhiteSpace(transactionDataModelSession.modelProductProp.PalletSize) ? "1.00x1.20" : transactionDataModelSession.modelProductProp.PalletSize.Replace(" ", "");
            transactionDataModelSession.modelProductProp.PalletList = JsonConvert.DeserializeObject<List<Pallet>>(_palletAPIRepository.GetPalletList(_factoryCode, _token)).Select(s => new SelectListItem() { Value = $"{s.PalletWidth:0.00}" + "x" + $"{s.PalletLength:0.00}", Text = $"{s.PalletWidth:0.00}" + "x" + $"{s.PalletLength:0.00}" });
            //model.modelProductProp.PalletList = ProductPropRepository.GetPalletSize(_pMTsDbContext).Select(s => new SelectListItem(){ Value = s.PalletWidth.ToString() + "x" + s.PalletLength.ToString(), Text = s.PalletWidth.ToString() + "x" + s.PalletLength.ToString() });

            if (transactionDataModelSession.modelProductProp.Wire != (transactionDataModelSession.modelProductSpec.Scorew2 / 50) + 1)
            {
                transactionDataModelSession.modelProductProp.Wire = transactionDataModelSession.modelProductProp.Wire;
            }
            else
            {
                transactionDataModelSession.modelProductProp.Wire = (transactionDataModelSession.modelProductSpec.Scorew2 / 50) + 1;
            }


            //transactionDataModelSession.modelProductProp.Wire = transactionDataModelSession.modelProductSpec != null ? (transactionDataModelSession.modelProductSpec.Scorew2 / 50) + 1 : null;
            transactionDataModelSession.modelProductProp.CutSheetWid = transactionDataModelSession.modelProductSpec != null ? transactionDataModelSession.modelProductSpec.CutSheetWid : null;
            transactionDataModelSession.modelProductProp.CutSheetLeng = transactionDataModelSession.modelProductSpec != null ? transactionDataModelSession.modelProductSpec.CutSheetLeng : null;


            if (transactionDataModelSession.modelProductProp.PalletOverhang == null || transactionDataModelSession.modelProductProp.PalletOverhang == 0)
            {
                transactionDataModelSession.modelProductProp.PalletOverhang = transactionDataModelSession.modelProductCustomer.PalletOverhang;
                //  transactionDataModelSession.modelProductProp.PalletOverhang = transactionDataModelSession.modelProductProp.PalletOverhang;
            }
            //else
            //{
            //    transactionDataModelSession.modelProductProp.PalletOverhang = transactionDataModelSession.modelProductCustomer.PalletOverhang;
            //}

            #region EANBarcode
            string MatString = transactionDataModelSession.MaterialNo.Substring(transactionDataModelSession.MaterialNo.Length - 5);

            string[] Ean = new string[14];
            Ean[0] = "885" + _saleOrg + MatString;
            //    Ean[0] = "885" + "0258" + MatString;

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
                    //// EanLevel[1] = EanLevel[1] + Int32.Parse(Ean[i]);
                    //EanLevel[1] = EanLevel[1] + Convert.ToInt32(Ean[i], 16);
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
            transactionDataModelSession.modelProductProp.EANCODE = EANCODE;
            #endregion

            //set default value 
            transactionDataModelSession.modelProductProp.Hardship = transactionDataModelSession.modelProductProp.Hardship != null && transactionDataModelSession.modelProductProp.Hardship.HasValue ? transactionDataModelSession.modelProductProp.Hardship : 5;
            transactionDataModelSession.modelProductProp.PieceSet = transactionDataModelSession.modelProductProp.PieceSet != null && transactionDataModelSession.modelProductProp.PieceSet.HasValue ? transactionDataModelSession.modelProductProp.PieceSet : 1;
            transactionDataModelSession.modelProductProp.PiecePatch = transactionDataModelSession.modelProductProp.PiecePatch != null && transactionDataModelSession.modelProductProp.PiecePatch.HasValue ? transactionDataModelSession.modelProductProp.PiecePatch : 1;
            transactionDataModelSession.modelProductProp.Change = transactionDataModelSession.modelProductProp.Change;

            transactionDataModelSession.modelProductPropChangeHis.ModelChangeList = JsonConvert.DeserializeObject<List<ChangeHistory>>(_changeHistoryAPIRepository.GetChangeHistoryByMaterial(_factoryCode, transactionDataModelSession.MaterialNo, _token));
            if (transactionDataModelSession.modelProductPicture == null)
            {
                transactionDataModelSession.modelProductPicture = new ProductPictureView();
            }
            transactionDataModelSession.modelProductProp.StandardPatternNameList = JsonConvert.DeserializeObject<List<ProductPropStandardPatternName>>(_standardPatternNameAPIRepository.GetAllByFactory(_factoryCode, _token));

            //    string str;
            string[] picture = new string[4];

            var temp = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNo(_factoryCode, transactionDataModelSession.MaterialNo, _token));
            var Id = temp.Id;

            transactionDataModelSession.modelProductProp.StatusFlag = temp.StatusFlag;
            transactionDataModel = transactionDataModelSession;
            _newProductService.SetTransactionStatus(ref transactionDataModel, "ProductProperties");
            transactionDataModel.CurrentTransaction = "ProductProp";
            //tassanai 
            transactionDataModel.modelProductProp.CIPInvType = temp.CipinvType;
            transactionDataModel.modelProductProp.CustInvType = temp.CustInvType;



            var sessionModel = transactionDataModel;

            SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "TransactionDataModel", sessionModel);

            //transactionDataModelSession.modelProductProp.PathPallet = temp.PalletizationPath;


            if (!string.IsNullOrEmpty(transactionDataModelSession.modelProductProp.picgetPallet))
            {
                transactionDataModelSession.modelProductPicture.Pic_PalletPath = transactionDataModelSession.modelProductProp.picgetPallet;

            }
            else if (string.IsNullOrEmpty(transactionDataModelSession.modelProductProp.picgetPallet) ||
               !string.IsNullOrEmpty(transactionDataModelSession.modelProductPicture.Pic_PalletPath))
            //  if (!string.IsNullOrEmpty(transactionDataModelSession.modelProductPicture.Pic_PalletPath))
            {
                picture[0] = transactionDataModelSession.modelProductPicture.Pic_PalletPath;
                transactionDataModelSession.modelProductPicture.Pic_PalletPath = ConvertPictureToBase64._ConvertPictureToBase64(picture[0]);
            }
            if (!string.IsNullOrEmpty(transactionDataModelSession.modelProductPicture.Pic_FGPath))
            {
                picture[1] = transactionDataModelSession.modelProductPicture.Pic_FGPath;
                transactionDataModelSession.modelProductPicture.Pic_FGPath = ConvertPictureToBase64._ConvertPictureToBase64(picture[1]);
            }



            //Tassanai get pallet

        }

        public void SaveProductProp(ref TransactionDataModel transactionDataModel)
        {
            var transactionDataModelSession = SessionExtentions.GetSession<TransactionDataModel>(_httpContextAccessor.HttpContext.Session, "TransactionDataModel");

            transactionDataModelSession.modelProductProp = transactionDataModel.modelProductProp;

            //set picture
            //if (transactionDataModelSessionPic_PalletPath)

            transactionDataModelSession.modelProductPicture = transactionDataModel.modelProductPicture != null ? transactionDataModel.modelProductPicture : new ProductPictureView();
            transactionDataModelSession.modelProductPropChangeHis = transactionDataModel.modelProductPropChangeHis;
            SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "TransactionDataModel", transactionDataModelSession);
            transactionDataModel.modelProductProp.MaterialNo = transactionDataModelSession.MaterialNo;
            transactionDataModelSession.modelProductProp.PrintMethod = transactionDataModel.modelProductProp.PrintMethod;
            transactionDataModelSession.amountColor = transactionDataModel.modelProductProp.AmountColor;

            transactionDataModelSession.modelProductProp.PalletOverhang = transactionDataModel.modelProductProp.PalletOverhang;
            transactionDataModelSession.modelProductProp.StatusFlag = transactionDataModel.modelProductProp.StatusFlag;

            // UPDATE Masterdata
            UpdateMasterdata(transactionDataModelSession);

            UpdateChangeHisData(transactionDataModelSession);
            UpdateTransactionsDetail(transactionDataModelSession.modelProductProp);

            //if(transactionDataModelSession.modelProductInfo.MatOursource != "" || transactionDataModelSession.modelProductInfo.MatOursource != null)
            //{
            //    transactionDataModelSession.MaterialNo = transactionDataModelSession.
            //    UpdateMasterdata(transactionDataModelSession);
            //}

            //update max step
            _newProductService.UpdateMaxProgress(_factoryCode, ref transactionDataModelSession);

            SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "TransactionDataModel", transactionDataModelSession);
            //}
        }

        public void GetPallet(ref TransactionDataModel transactionDataModel, string JoinTypeFilter, string palletSizeFilter, int WidDC, int LegDC, int Overhang)
        {


            var transactionDataModelSession = SessionExtentions.GetSession<TransactionDataModel>(_httpContextAccessor.HttpContext.Session, "TransactionDataModel");
            if (transactionDataModelSession.modelProductProp == null)
            {
                transactionDataModelSession.modelProductProp = new ProductPropViewModel();

            }

            // GetPallet from API

            PalletCalculateParam palletCalculateParam = new PalletCalculateParam();

            palletCalculateParam.FormGroup = transactionDataModelSession.modelCategories.FormGroup;
            //palletCalculateParam.RSCStyle =
            palletCalculateParam.Flute = transactionDataModelSession.modelProductSpec.Flute;
            palletCalculateParam.WidDC = WidDC;
            palletCalculateParam.LegDC = LegDC;
            palletCalculateParam.Hig = transactionDataModelSession.modelProductSpec.Hig;
            palletCalculateParam.palletSizeFilter = palletSizeFilter;
            palletCalculateParam.Overhang = Overhang;
            palletCalculateParam.CutSheetWid = transactionDataModelSession.modelProductSpec.CutSheetWid;
            palletCalculateParam.CutSheetLeng = transactionDataModelSession.modelProductSpec.ScoreL6;
            palletCalculateParam.JoinTypeFilter = JoinTypeFilter;
            palletCalculateParam.ScoreL6 = transactionDataModelSession.modelProductSpec.ScoreL6;

            var PalletResultAPI = JsonConvert.DeserializeObject<PalletCalulate>(_standardPatternNameAPIRepository.GetCalculatePallet(_factoryCode,
                   JsonConvert.SerializeObject(palletCalculateParam), _token));


            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



            //       var flu = JsonConvert.DeserializeObject<Flute>(_fluteAPIRepository.GetFluteByFlute(_factoryCode, transactionDataModelSession.modelProductSpec.Flute,_token));
            //       transactionDataModelSession.modelProductProp.Thickness = flu.Thickness.Value;




            //       var formgroup = transactionDataModelSession.modelCategories.FormGroup;
            //       var Wid = 0;
            //       var Leg = 0;
            //       var Hig = 0;
            //       var ScoreL6 = transactionDataModelSession.modelProductSpec.ScoreL6;

            //       if (formgroup.Contains("RSC"))
            //       {
            //           Wid = Convert.ToInt32(transactionDataModelSession.modelProductSpec.CutSheetWid);
            //           Leg = Convert.ToInt32(transactionDataModelSession.modelProductSpec.ScoreL6);
            //           Hig = Convert.ToInt32(transactionDataModelSession.modelProductSpec.Hig);
            //       }
            //       else if ((formgroup.Contains("DC") || formgroup.Contains("HB") || formgroup.Contains("AC") || formgroup.Contains("SB") || formgroup.Contains("SS")) && (JoinTypeFilter != "OOO"))
            //       {
            //           Wid = WidDC;
            //           Leg = LegDC / 2;
            //           Hig = 1;
            //       }
            //       else if ((formgroup.Contains("DC") || formgroup.Contains("HB") || formgroup.Contains("AC") || formgroup.Contains("SB") || formgroup.Contains("SS")) && (JoinTypeFilter == "OOO"))
            //       {
            //           Wid = WidDC;
            //           Leg = LegDC;
            //           Hig = 1;
            //       }
            //       // if dc

            //       //var PalletResult = func_pallet(transactionDataModelSession.modelCategories.FormGroup, transactionDataModelSession.modelCategories.RSCStyle, flu.Flute1, flu.A, flu.B, flu.C, flu.D1, flu.Thickness,
            //       //Wid, Leg, Hig, palletSizeFilter, transactionDataModelSession.modelProductProp.PalletOverhang,
            //       // transactionDataModelSession.modelProductProp.CutSheetWid, transactionDataModelSession.modelProductProp.CutSheetLeng, JoinTypeFilter);
            //       var PalletResult = func_pallet(transactionDataModelSession.modelCategories.FormGroup, transactionDataModelSession.modelCategories.RSCStyle, flu.Flute1, flu.A, flu.B, flu.C, flu.D1, flu.Thickness.Value,
            //Wid, Leg, Hig, palletSizeFilter, Overhang, transactionDataModelSession.modelProductProp.CutSheetWid, transactionDataModelSession.modelProductProp.CutSheetLeng, JoinTypeFilter);


            //transactionDataModelSession.modelProductProp.PicPallet = PalletResult.Item1;
            //transactionDataModelSession.modelProductProp.BunLayer = PalletResult.Item2;
            transactionDataModelSession.modelProductProp.PicPallet = PalletResultAPI.PicPallet;
            transactionDataModelSession.modelProductProp.BunLayer = PalletResultAPI.BunLayer;

            SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "TransactionDataModel", transactionDataModelSession);
        }

        public void UpdateMasterdata(TransactionDataModel transactionDataModel)
        {

            ParentModel MasterDataModel = new ParentModel();
            MasterDataModel.MasterData = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNo(_factoryCode, transactionDataModel.MaterialNo, _token));
            var transactionDetailObject = JsonConvert.DeserializeObject<TransactionsDetail>(_transactionsDetailAPIRepository.GetTransactionsDetailByMat(_factoryCode, transactionDataModel.MaterialNo, _token));
            var isOursource = transactionDetailObject != null && transactionDetailObject.Outsource ? true : false;

            if (MasterDataModel.MasterData.PdisStatus != "N" && MasterDataModel.MasterData.SapStatus == true)
            {
                MasterDataModel.MasterData.PdisStatus = "M";
                if (_saleOrg != MasterDataModel.MasterData.SaleOrg)
                    MasterDataModel.MasterData.TranStatus = true;
                else
                    MasterDataModel.MasterData.TranStatus = isOursource ? true : false;

                //MasterDataModel.MasterData.TranStatus = isOursource ? true : false;
            }
            else if (MasterDataModel.MasterData.PdisStatus == "C" && MasterDataModel.MasterData.SapStatus != true)
            {
                if (_saleOrg != MasterDataModel.MasterData.SaleOrg)
                    MasterDataModel.MasterData.TranStatus = true;
                else
                    MasterDataModel.MasterData.TranStatus = isOursource ? true : false;
                //MasterDataModel.MasterData.TranStatus =  false;
            }

            MasterDataModel.MasterData.CutSheetWid = transactionDataModel.modelProductProp.CutSheetWid;
            MasterDataModel.MasterData.CutSheetLeng = transactionDataModel.modelProductProp.CutSheetLeng;
            MasterDataModel.MasterData.JointId = transactionDataModel.modelProductProp.JoinId;

            MasterDataModel.MasterData.JoinType = transactionDataModel.modelProductProp.JoinType;
            MasterDataModel.MasterData.PrintMethod = transactionDataModel.modelProductProp.PrintMethod;
            MasterDataModel.MasterData.Wire = transactionDataModel.modelProductProp.Wire;
            MasterDataModel.MasterData.OuterJoin = transactionDataModel.modelProductProp.OuterJoin;
            MasterDataModel.MasterData.PalletSize = transactionDataModel.modelProductProp.PalletSize;
            MasterDataModel.MasterData.Bun = transactionDataModel.modelProductProp.Bun;
            MasterDataModel.MasterData.BunLayer = transactionDataModel.modelProductProp.BunLayer;
            MasterDataModel.MasterData.LayerPalet = transactionDataModel.modelProductProp.LayerPallet;
            MasterDataModel.MasterData.BoxPalet = transactionDataModel.modelProductProp.BoxPalet;
            //  MasterDataModel.MasterData.PicPallet = transactionDataModel.modelProductPicture.PicPallet;
            MasterDataModel.MasterData.PieceSet = transactionDataModel.modelProductProp.PieceSet;
            MasterDataModel.MasterData.PiecePatch = transactionDataModel.modelProductProp.PiecePatch;
            MasterDataModel.MasterData.BoxHandle = transactionDataModel.modelProductProp.BoxHandle;
            MasterDataModel.MasterData.SparePercen = transactionDataModel.modelProductProp.SparePercen;
            MasterDataModel.MasterData.SpareMax = transactionDataModel.modelProductProp.SpareMax;
            MasterDataModel.MasterData.SpareMin = transactionDataModel.modelProductProp.SpareMin;
            MasterDataModel.MasterData.LeadTime = transactionDataModel.modelProductProp.LeadTime;
            MasterDataModel.MasterData.Hardship = transactionDataModel.modelProductProp.Hardship;

            //MasterDataModel.MasterData.ChangeInfo = transactionDataModel.modelProductProp.ChangeInfo;
            MasterDataModel.MasterData.Change = transactionDataModel.modelProductProp.Change;

            MasterDataModel.MasterData.LastUpdate = DateTime.Now;
            MasterDataModel.MasterData.UpdatedBy = _username;
            MasterDataModel.MasterData.EanCode = transactionDataModel.modelProductProp.EANCODE;
            MasterDataModel.MasterData.StatusFlag = transactionDataModel.modelProductProp.StatusFlag;

            //tassanai 
            MasterDataModel.MasterData.CipinvType = transactionDataModel.modelProductProp.CIPInvType;
            MasterDataModel.MasterData.CustInvType = transactionDataModel.modelProductProp.CustInvType;
            //tassanai update 23/11/2020
            //MasterDataModel.MasterData.FGMaterial = transactionDataModel.modelProductProp.FGMaterial; ;
            MasterDataModel.MasterData.TopSheetMaterial = transactionDataModel.modelProductProp.TopSheetMaterial; ;

            MasterDataModel.MasterData.JoinCharacter = transactionDataModel.modelProductProp.NameJoinCharacter;

            //tassanai update 23/02/2021
            MasterDataModel.MasterData.NoneStandardPaper = transactionDataModel.modelProductProp.NoneStandardPaper;

            // tassanai update 13052022
            MasterDataModel.MasterData.WorkType = transactionDataModel.modelProductProp.WorkType;
            MasterDataModel.MasterData.Boistatus = transactionDataModel.modelProductProp.BOIStatus;
            MasterDataModel.MasterData.BoxPacking = transactionDataModel.modelProductProp.BoxPacking;

            //// ==== TAssanai Update 06/02/2020 ====

            var palletPath = "";

            if (transactionDataModel.modelProductProp.PathpalletSuggess != null)
            {
                //Tassanai update 06/03/2020 ======
                //transactionDataModel.modelProductPicture.Pic_PalletPath = transactionDataModel.modelProductProp.PathpalletSuggess;
                transactionDataModel.modelProductPicture.Pic_PalletPath = null;
            }
            if (transactionDataModel.modelProductProp.picgetPallet != null)
            {
                transactionDataModel.modelProductPicture.Pic_PalletPath = transactionDataModel.modelProductProp.picgetPallet;
                palletPath = transactionDataModel.modelProductProp.picgetPallet;
            }
            else if (transactionDataModel.modelProductProp.picgetPallet == null && transactionDataModel.modelProductPicture.Pic_PalletPath != null)
            {
                var cursor = transactionDataModel.modelProductPicture.Pic_PalletPath.IndexOf(',') + 1;
                var base64 = transactionDataModel.modelProductPicture.Pic_PalletPath.Substring(cursor);
                //get path form pmtcontext
                var path = JsonConvert.DeserializeObject<PmtsConfig>(_pmtsConfigAPIRepository.GetPMTsConfigByFactoryName(_factoryCode, "Pallet_Path", _token)).FucValue;
                palletPath = path + "\\" + _factoryCode + "-" + transactionDataModel.MaterialNo + ".png";
                if (path != null)
                {
                    //byte[] bytes = Convert.FromBase64String(base64);

                    //using (MemoryStream ms = new MemoryStream(bytes))
                    //{
                    //    using (Bitmap bm2 = new Bitmap(ms))
                    //    {
                    //        bm2.Save(palletPath);
                    //    }
                    //}

                    _extensionService.UploadImage(base64, palletPath);

                    transactionDataModel.modelProductPicture.Pic_PalletPath = palletPath;
                }
            }
            else if (transactionDataModel.modelProductProp.PathpalletSuggess != "")
            {
                transactionDataModel.modelProductPicture.Pic_PalletPath = transactionDataModel.modelProductProp.PathpalletSuggess;
                palletPath = transactionDataModel.modelProductProp.PathpalletSuggess;
            }

            var fgPath = "";


            if (transactionDataModel.modelProductPicture.Pic_FGPath != null)
            {
                var cursor = transactionDataModel.modelProductPicture.Pic_FGPath.IndexOf(',') + 1;
                var base64 = transactionDataModel.modelProductPicture.Pic_FGPath.Substring(cursor);

                //get path form pmtcontext
                var path = JsonConvert.DeserializeObject<PmtsConfig>(_pmtsConfigAPIRepository.GetPMTsConfigByFactoryName(_factoryCode, "FG_Path", _token)).FucValue;
                fgPath = path + "\\" + _factoryCode + "-" + transactionDataModel.MaterialNo + ".png"; ;
                if (path != null)
                {
                    //byte[] bytes = Convert.FromBase64String(base64);

                    //using (MemoryStream ms = new MemoryStream(bytes))
                    //{
                    //    using (Bitmap bm2 = new Bitmap(ms))
                    //    {
                    //        bm2.Save(fgPath);
                    //    }
                    //}

                    _extensionService.UploadImage(base64, fgPath);

                    transactionDataModel.modelProductPicture.Pic_FGPath = fgPath;
                }
            }

            MasterDataModel.MasterData.PalletizationPath = palletPath;

            string picPallet;
            string pathPallet = @fgPath;
            picPallet = Path.GetFileName(palletPath);


            MasterDataModel.MasterData.FgpicPath = fgPath;
            MasterDataModel.MasterData.FactoryCode = _factoryCode;
            MasterDataModel.MasterData.PicPallet = picPallet;
            MasterDataModel.MasterData.User = _username;
            if (!string.IsNullOrEmpty(MasterDataModel?.MasterData?.BoxType))
            {
                MasterDataModel.MasterData.SizeDimensions = JsonConvert.DeserializeObject<string>(_formulaAPIRepository.CalSizeDimensions(_factoryCode, transactionDataModel.MaterialNo, _token));
            }

            // };


            var parentModelMasterData = new ParentModel()
            {
                AppName = Globals.AppNameEncrypt,
                SaleOrg = _saleOrg,
                PlantCode = _factoryCode,
                MasterData = MasterDataModel.MasterData
            };
            string MasterDataJsonString = JsonConvert.SerializeObject(parentModelMasterData);


            _masterDataAPIRepository.UpdateMasterData(MasterDataJsonString, _token);



            if (transactionDataModel.modelProductInfo.PLANTCODE != null && MasterDataModel.MasterData.SapStatus == false)
            {
                //Tassanai Update 15/11/2019
                // Get Masterdata by id ที่ 2

                //var idHire = JsonConvert.DeserializeObject<Machine>(_masterDataAPIRepository.GetMasterDataByMaterialNo(transactionDataModel.modelProductInfo.PLANTCODE, transactionDataModel.MaterialNo, _token)).Id;
                //MasterDataModel.MasterData = _masterDataAPIRepository.GetMasterDataByMaterialNo(transactionDataModel.modelProductInfo.PLANTCODE, transactionDataModel.MaterialNo);
                var MasterDataOS = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNo(transactionDataModel.modelProductInfo.PLANTCODE, transactionDataModel.modelProductInfo.MatOursource, _token));

                MasterDataModel.MasterData.FactoryCode = transactionDataModel.modelProductInfo.PLANTCODE;
                MasterDataModel.MasterData.Plant = transactionDataModel.modelProductInfo.PLANTCODE;
                MasterDataModel.MasterData.Id = MasterDataOS != null ? MasterDataOS.Id : 0;
                if (MasterDataOS != null && MasterDataOS.SaleOrg != _saleOrg && MasterDataModel.MasterData.MaterialNo != transactionDataModel.modelProductInfo.MatOursource)
                {
                    MasterDataModel.MasterData.SaleOrg = MasterDataOS.SaleOrg;
                    MasterDataModel.MasterData.TranStatus = isOursource ? true : false;
                    MasterDataModel.MasterData.PdisStatus = MasterDataOS.PdisStatus;
                    MasterDataModel.MasterData.MaterialType = MasterDataOS.MaterialType;
                }
                else
                {
                    MasterDataModel.SaleOrg = _saleOrg;
                    MasterDataModel.MasterData.TranStatus = true;
                }
                MasterDataModel.MasterData.MaterialNo = transactionDataModel.modelProductInfo.MatOursource;
                MasterDataModel.MasterData.User = _username;

                var parentModelMasterDataHire = new ParentModel()
                {
                    AppName = Globals.AppNameEncrypt,
                    PlantCode = transactionDataModel.modelProductInfo.PLANTCODE,
                    MasterData = MasterDataModel.MasterData,
                };
                string MasterDataJsonStringHire = JsonConvert.SerializeObject(parentModelMasterDataHire);
                _masterDataAPIRepository.UpdateMasterData(MasterDataJsonStringHire, _token);
            }
        }

        public void UpdateChangeHisData(TransactionDataModel transactionDataModel)
        {
            bool IsDuplicatedInfo = true;
            var ChangeHistorys = JsonConvert.DeserializeObject<List<ChangeHistory>>(_changeHistoryAPIRepository.GetChangeHistoryByMaterial(_factoryCode, transactionDataModel.MaterialNo, _token));
            var changeInfo = "";
            if (ChangeHistorys != null)
            {
                //  changeInfo = ChangeHistorys.Count == 0 ? "" : ChangeHistorys.OrderByDescending(c => c.CreatedDate).FirstOrDefault().ChangeInfo;
                changeInfo = ChangeHistorys.Count == 0 ? "" : ChangeHistorys.OrderByDescending(c => c.CreatedDate).FirstOrDefault().ChangeInfo;

            }

            //IsDuplicatedInfo = changeInfo == transactionDataModel.modelProductProp.ChangeInfo;
            IsDuplicatedInfo = changeInfo == transactionDataModel.modelProductProp.Change;

            //if (!IsDuplicatedInfo && !string.IsNullOrEmpty(transactionDataModel.modelProductProp.ChangeInfo) && !string.IsNullOrWhiteSpace(transactionDataModel.modelProductProp.ChangeInfo))
            if (!IsDuplicatedInfo && !string.IsNullOrEmpty(transactionDataModel.modelProductProp.Change) && !string.IsNullOrWhiteSpace(transactionDataModel.modelProductProp.Change))

            {
                ChangeHistory changeHistoryModel = new ChangeHistory
                {
                    MaterialNo = transactionDataModel.MaterialNo,
                    FactoryCode = _factoryCode,
                    //ChangeHistoryText = transactionDataModel.modelProductProp.ChangeInfo,
                    //ChangeInfo = transactionDataModel.modelProductProp.ChangeInfo,
                    ChangeHistoryText = transactionDataModel.modelProductProp.Change,
                    ChangeInfo = transactionDataModel.modelProductProp.Change,
                    CreatedDate = DateTime.Now,
                    CreatedBy = _username,
                    UpdatedDate = DateTime.Now,
                    UpdatedBy = _username,
                    Status = true

                };

                var PropHistory = new ParentModel();
                PropHistory.AppName = Globals.AppNameEncrypt;
                PropHistory.SaleOrg = _saleOrg;
                PropHistory.PlantCode = _factoryCode;
                PropHistory.ChangeHistory = changeHistoryModel;


                string changeHistoryListJsonString = JsonConvert.SerializeObject(PropHistory);

                // if (transactionDataModel.modelProductProp.ChangeInfo != null)
                if (transactionDataModel.modelProductProp.Change != null)
                {
                    _changeHistoryAPIRepository.SaveChangeHistory(changeHistoryListJsonString, _token);
                }
            }
        }

        public void UpdateTransactionsDetail(ProductPropViewModel model)
        {
            ParentModel Parent = new ParentModel();
            Parent.AppName = Globals.AppNameEncrypt;
            Parent.SaleOrg = _saleOrg;
            Parent.PlantCode = _factoryCode;

            Parent.TransactionsDetail = JsonConvert.DeserializeObject<TransactionsDetail>(_transactionsDetailAPIRepository.GetTransactionsDetailByMat(_factoryCode, model.MaterialNo, _token));
            Parent.TransactionsDetail.AmountColor = model.AmountColor;
            Parent.TransactionsDetail.PalletOverhang = model.PalletOverhang;

            string TransactionsDetailJsonString = JsonConvert.SerializeObject(Parent);

            _transactionsDetailAPIRepository.UpdateTransactionsDetail(TransactionsDetailJsonString, _token);
        }

        #region Palet

        public (string, int) func_pallet(string FormGroup, string RSCStyle, string Flute1, int? A, int? B, int? C, int? D1, double Thickness,
            int? wid, int? leg, int? hig, string palletsize, int palletOverhang, int? CutSheetWid, int? CutSheetLeng, string JoinTypeFilter)
        {

            int ForBoxtype_l, ForBoxtype_W, ForBoxtype_D, ForBoxtype_c;
            double ForBoxtype_T, FlatBoxtype_H = 0.0;
            int FlatBoxtype_L = 0;
            int FlatBoxtype_W = 0;
            int ForWL;

            // make ค่า Die Cut	เผื่อกว้างยาว กับ ??
            int DCValue = 0;
            int SpareWL = 0;
            int Unknow = 3; //flute B


            ForWL = SpareWL;

            //== Goto PalletAPi==

            //var palletResultapi = _palletAPIRepository.GetcalcPallet(FormGroup, RSCStyle, Flute1, A, B, C, D1, Thickness,
            //wid, leg, hig, palletsize, palletOverhang, CutSheetWid, CutSheetLeng, JoinTypeFilter);


            if (FormGroup == "STDRSC")
            {
                FlatBoxtype_L = Convert.ToInt32(Convert.ToInt32(leg));
                FlatBoxtype_W = Convert.ToInt32(Convert.ToInt32(wid));
                FlatBoxtype_H = Thickness * 2;
            }

            else if (FormGroup == "STDDC")
            {
                //ForBoxtype_l = dataflute.FluteDC;
                //ForBoxtype_W = dataflute.FluteDC;
                //ForBoxtype_D = dataflute.FluteDC;
                //ForBoxtype_T = dataflute.Thickness;

                FlatBoxtype_L = Convert.ToInt32(Convert.ToInt32(leg));
                FlatBoxtype_W = Convert.ToInt32(Convert.ToInt32(wid));
                FlatBoxtype_H = Thickness;

            }
            else if ((FormGroup.Contains("DC") || FormGroup.Contains("HB") || FormGroup.Contains("AC") || FormGroup.Contains("SB") || FormGroup.Contains("SS")))
            //  Pallet Size
            {
                FlatBoxtype_L = Convert.ToInt32(Convert.ToInt32(leg));
                FlatBoxtype_W = Convert.ToInt32(Convert.ToInt32(wid));
                FlatBoxtype_H = Thickness;
            }
            var PalletSize = palletsize.Split('x');
            // decimal PalletSize0 = Convert.ToDecimal(PalletSize[0]);
            var PalletNormal_W = Convert.ToInt32(Convert.ToDecimal(PalletSize[0]) * 1000);
            var PalletNormal_L = Convert.ToInt32(Convert.ToDecimal(PalletSize[1]) * 1000);
            var PalletOVERHANG_L = PalletNormal_L + palletOverhang;
            var PalletOVERHANG_W = PalletNormal_W + palletOverhang;



            //           ยอมให้วางเลย pallet เมื่อ มีด้านที่ยาวกว่า 1200
            int Pallet_L1 = 0, Pallet_W1 = 0;
            int Pallet_L2 = 0, Pallet_W2 = 0;
            Pallet_L1 = (FlatBoxtype_L <= 300) ? PalletNormal_L : PalletOVERHANG_L;
            Pallet_W1 = (FlatBoxtype_W <= 300) ? PalletNormal_W : PalletOVERHANG_W;
            Pallet_L2 = (FlatBoxtype_W <= 300) ? PalletNormal_L : PalletOVERHANG_L;
            Pallet_W2 = (FlatBoxtype_W <= 300) ? PalletNormal_W : PalletOVERHANG_W;





            List<PalletresultModel> palletresultModel = new List<PalletresultModel>();

            List<ColumnPalletModel> columnPalletModel = new List<ColumnPalletModel>();

            // =====Update getpallet from database

            var standardPatternName = JsonConvert.DeserializeObject<List<StandardPatternName>>(_standardPatternNameAPIRepository.GetAllByFactory(_factoryCode, _token));
            foreach (var values in standardPatternName.OrderBy(x => x.Id).ThenBy(x => x.Type))
            {
                var calcL1 = Convert.ToInt32(values.Col) * FlatBoxtype_L;
                var calcW1 = Convert.ToInt32(values.Row) * FlatBoxtype_W;
                var calcL2 = Convert.ToInt32(values.Col) * FlatBoxtype_W;
                var calcW2 = Convert.ToInt32(values.Row) * FlatBoxtype_L;


                var L1_L1 = calcL1 <= Pallet_L1 ? 1 : 0;
                //  var L1_L1 = calcL1, Pallet_L1);
                var W1_W1 = calcW1 <= Pallet_W1 ? 1 : 0;
                var L1_W1 = calcL1 <= Pallet_W1 ? 1 : 0;
                var W1_L1 = calcW1 <= Pallet_L1 ? 1 : 0;
                var L2_L2 = calcL2 <= Pallet_L2 ? 1 : 0;
                var W2_W2 = calcW2 <= Pallet_W2 ? 1 : 0;
                var L2_W2 = calcW2 <= Pallet_L2 ? 1 : 0;
                var W2_L2 = calcL2 <= Pallet_W2 ? 1 : 0;
                var CalcLLWW1 = L1_L1 * W1_W1; //A
                var CalcLWWL1 = L1_W1 * W1_L1; //B
                var CalcLLWW2 = L2_L2 * W2_W2; //C
                var CalcLWWL2 = L2_W2 * W2_L2; //D
                                               // var ChkL1 = chkValue2(CalcLL1WW1, Convert.ToInt32(values.BundlePerLayyer));
                var ChkLLWW1 = CalcLLWW1 == 1 ? Convert.ToInt32(values.Amount) : 0; //E
                var ChkLLWWL1 = CalcLWWL1 == 1 ? Convert.ToInt32(values.Amount) : 0; //F
                var ChkcLLWW2 = CalcLLWW2 == 1 ? Convert.ToInt32(values.Amount) : 0; //G
                var ChkcLWWL2 = CalcLWWL2 == 1 ? Convert.ToInt32(values.Amount) : 0;//H

                var maxChk = new[] { ChkLLWW1, ChkLLWWL1, ChkcLLWW2, ChkcLWWL2 };
                var qtyMax = maxChk.Max(); //ใบ/ชั้น

                columnPalletModel.Add(new ColumnPalletModel
                {
                    Type = values.Type,
                    LxW = values.Col + "x" + values.Row,
                    BundlePerLayyer = Convert.ToInt32(values.Amount),
                    L = FlatBoxtype_L,
                    W = FlatBoxtype_W,
                    L1 = calcL1,
                    W1 = calcW1,
                    L2 = calcL2,
                    W2 = calcW2,
                    L1_L1 = L1_L1,
                    W1_W1 = W1_W1,
                    L1_W1 = L1_W1,
                    W1_L1 = W1_L1,
                    L2_L2 = L2_L2,
                    W2_W2 = W2_W2,
                    L2_W2 = L2_W2,
                    W2_L2 = W2_L2,
                    CartonPerLayer = qtyMax


                });
                if (values.Type == "Column")
                {
                    palletresultModel.Add(new PalletresultModel
                    {
                        // formatPalletName = "3.ColumnType",
                        formatPalletName = values.Type,
                        typePalletName = values.PictureNamePallet,
                        qtycartonPerLayer = qtyMax
                    });
                }
                // Interlock  ====

                if (values.Type != "Column")
                {
                    palletresultModel.Add(new PalletresultModel
                    {
                        formatPalletName = values.Type,
                        typePalletName = values.PictureNamePallet,
                        qtycartonPerLayer = Qtycartontable(values.Type, FlatBoxtype_L, FlatBoxtype_W, PalletNormal_L, PalletOVERHANG_L,
                        PalletNormal_W, PalletOVERHANG_W, values.Col.Value, values.Row.Value, values.Amount.Value, values.PictureNamePallet)
                    });
                }




            }


            // ------- Fix 


            // var result = "";
            string lblMaxCarton = "";
            string lbltypePallet = "";
            var pathPic = "";
            string result = "";
            int bunlayer = 0;



            var dataresult = palletresultModel.Where(x => x.qtycartonPerLayer == palletresultModel.Where(xx => xx.qtycartonPerLayer >= 0).Max(y => y.qtycartonPerLayer)).OrderBy(x => x.formatPalletName).FirstOrDefault();



            lblMaxCarton = dataresult.qtycartonPerLayer.ToString();


            // find หา new name

            var PatternName = JsonConvert.DeserializeObject<StandardPatternName>(_standardPatternNameAPIRepository.GetStandardPatternName(_factoryCode, dataresult.typePalletName, _token));


            if (lblMaxCarton == "0")
            {
                pathPic = "C0111.png";
                bunlayer = 0;
                result = pathPic;
            }
            else
            {
                pathPic = PatternName.PatternName + ".png";
                bunlayer = Convert.ToInt32(lblMaxCarton);
                result = pathPic;
            }

            return (result, bunlayer);
        }




        public int Qtycartontable(string typename, int FlatBoxtype_L, int FlatBoxtype_W, int PalletNormal_L, int PalletOVERHANG_L, int PalletNormal_W,
            int PalletOVERHANG_W, int Col, int Row, int Amount, string PictureNamePallet)
        {
            var result = 0;

            try
            {


                // string typename = "";
                var L_W_ = 0; //L'+W'
                var L_ = 0; //L'
                var W2 = 0; //2W'
                var W2_2 = 0; //2W' check


                var PalletL_L1 = 0; //L
                var PalletL_W1 = 0; // W
                var PalletL_L2 = 0;// 2W'|L
                var PalletL_W2 = 0;// 2W'|W
                int L_L_W = 0;//L/(L'+W')
                int W_2W_L = 0;//W/ 2W' OR L'
                int L_2W_L = 0;//L/2W' OR L'
                var W_L_W = 0; //W/(L'+W') 
                var LPallet_L = 0;   //L_L_W     //หันด้านยาวไว้ด้าน L --L (Pallet)
                var WPallet_L = 0; // W_2W_L       //หันด้านยาวไว้ด้าน L --W (Pallet)
                var LPallet_W = 0;// L_2W_L       //หันด้านยาวไว้ด้าน L --L (Pallet)
                var WPallet_W = 0;// W_L_W       //หันด้านยาวไว้ด้าน L --W (Pallet)
                var chkLPallet_L = 0;
                var chkLPallet_W = 0;
                var cartonperlayer1 = 0;
                var cartonperlayer2 = 0;
                var maxcartonperLayer = 0; // ===================== Max

                var qtyCartonPLay = 0;

                //สลับด้าน กว้าง ยาว ของกล่อง สำหรับ Interlock กับ Spiral
                #region Interlock
                //
                // typename = "Type 4";   
                //var f_l = FlatBoxtype_W;
                //var f_w = FlatBoxtype_L;

                var f_l = FlatBoxtype_L > FlatBoxtype_W ? FlatBoxtype_L : FlatBoxtype_W;
                var f_w = FlatBoxtype_L < FlatBoxtype_W ? FlatBoxtype_L : FlatBoxtype_W;




                //  FlatBoxtype_L = f_l;
                //  f_l = f_w;
                if (typename == "Interlock")
                {

                    if (PictureNamePallet != "Type 15")
                    {
                        L_ = f_l * Row; //L'
                        W2 = f_w * Col; //2W'
                        W2_2 = W2 > L_ ? W2 : L_; //2W' check
                        L_W_ = f_l + f_w; //L'+W'
                        qtyCartonPLay = Amount;

                    }

                    else if (PictureNamePallet == "Type 15")
                    {

                        L_ = f_l * Row; //L'
                        W2 = f_w * Col; //2W'
                        var W_2W_L15 = (f_l * Row) + f_w;
                        // W2_2 = W2 > L_ ? W2 : L_ ; //2W' check
                        W2_2 = Math.Max(W2, Math.Max(L_, W_2W_L15));
                        L_W_ = f_w + f_l; //L'+W'

                        qtyCartonPLay = Amount;
                    }

                }
                #endregion
                #region Spiral
                if (typename == "Spiral")
                {
                    if (typename != "Type 24")
                    {
                        L_W_ = f_l + (f_w * (Col / 2)); //L'+W'

                        // // L_W_ = (f_l * 2) ; //L'+W'
                        //L_ = L_W_; //L'+W'
                        //W2 = L_W_; //2W'
                        //W2 = L_W_; //2W'
                        //W2_2 = L_W_; //2W' check
                        //qtyCartonPLay = Amount;
                    }

                    else if (typename == "Type 24")
                    {
                        L_W_ = (f_l * 2) + (f_w * 4); //L'+W'

                    }

                    L_ = L_W_; //L'+W'
                    W2 = L_W_; //2W'
                    W2 = L_W_; //2W'
                    W2_2 = L_W_; //2W' check
                    qtyCartonPLay = Amount;

                }
                // if (typename == "Type 5")
                //{
                //    //L_ = f_w * 2; //L'

                //    L_W_ = f_l + f_w; //L'+W'
                //    L_ = L_W_; //L'+W'
                //    W2 = L_W_; //2W'
                //    W2 = L_W_; //2W'
                //    W2_2 = L_W_; //2W' check
                //    qtyCartonPLay = 4;
                //}
                //else if (typename == "Type 10")
                //{
                //    L_W_ = f_l + (f_w * 2); //L'+W'
                //    L_ = L_W_; //L'+W'
                //    W2 = L_W_; //2W'
                //    W2 = L_W_; //2W'
                //    W2_2 = L_W_; //2W' check
                //    qtyCartonPLay = 8;
                //}
                //else if (typename == "Type 22")
                //{
                //    L_W_ = f_l + (f_w * 3); //L'+W'
                //    L_ = L_W_; //L'+W'
                //    W2 = L_W_; //2W'
                //    W2 = L_W_; //2W'
                //    W2_2 = L_W_; //2W' check
                //    qtyCartonPLay = 12;
                //}
                //else if (typename == "Type 23")
                //{
                //    L_W_ = f_l + (f_w * 4); //L'+W'
                //    L_ = L_W_; //L'+W'
                //    W2 = L_W_; //2W'
                //    W2 = L_W_; //2W'
                //    W2_2 = L_W_; //2W' check
                //    qtyCartonPLay = 16;
                //}
                //else if (typename == "Type 24")
                //{
                //    L_W_ = (f_l * 2) + (f_w * 4); //L'+W'
                //                                  // L_W_ = (f_l * 2) ; //L'+W'
                //    L_ = L_W_; //L'+W'
                //    W2 = L_W_; //2W'
                //    W2 = L_W_; //2W'
                //    W2_2 = L_W_; //2W' check
                //    qtyCartonPLay = 32;
                //}
                #endregion


                PalletL_L1 = L_W_ <= 300 ? PalletNormal_L : PalletOVERHANG_L; //L
                PalletL_W1 = PalletNormal_W; // W
                PalletL_L2 = W2_2 <= 300 ? PalletNormal_L : PalletOVERHANG_L;// 2W'|L
                PalletL_W2 = PalletOVERHANG_W;// 2W'|W
                //L_L_W = (int)Math.Floor(Convert.ToDouble(PalletL_L1 / L_W_));//L/(L'+W')
                L_L_W = (int)Math.Floor(Convert.ToDouble(PalletL_L1 / L_W_));//L/(L'+W')
                W_2W_L = (int)Math.Floor(Convert.ToDouble(PalletL_W1 / W2_2));//W/ 2W' OR L'
                L_2W_L = (int)Math.Floor(Convert.ToDouble(PalletOVERHANG_L / W2_2));//L/2W' OR L'
                W_L_W = (int)Math.Floor(Convert.ToDouble(PalletOVERHANG_W / L_W_));//W/(L'+W') 
                LPallet_L = L_L_W;    //หันด้านยาวไว้ด้าน L --L (Pallet)
                WPallet_L = W_2W_L;      //หันด้านยาวไว้ด้าน L --W (Pallet)
                LPallet_W = L_2W_L;     //หันด้านยาวไว้ด้าน L --L (Pallet)
                WPallet_W = W_L_W;    //หันด้านยาวไว้ด้าน L --W (Pallet)
                chkLPallet_L = (LPallet_L & WPallet_L) == 1 ? 1 : 0;
                chkLPallet_W = (LPallet_W & WPallet_W) == 1 ? 1 : 0;
                cartonperlayer1 = chkLPallet_L * qtyCartonPLay;
                cartonperlayer2 = chkLPallet_W * qtyCartonPLay;
                maxcartonperLayer = cartonperlayer1 >= cartonperlayer2 ? cartonperlayer1 : cartonperlayer2; // ===================== Max


                result = maxcartonperLayer;
            }
            catch
            {
                result = 0;
            }


            return result;
        }
        #endregion
    }
}
