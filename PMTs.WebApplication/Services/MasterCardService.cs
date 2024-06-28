using AlanJuden.MvcReportViewer.ReportService;
using AutoMapper;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using PMTs.DataAccess.ComplexModel;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.Report;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Shared;
using PMTs.DataAccess.Tracing;
using PMTs.WebApplication.Controllers;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Services.Interfaces;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PMTs.WebApplication.Services
{
    [TraceAspect]
    public class MasterCardService : IMasterCardService
    {
        private IHttpContextAccessor _httpContextAccessor;
        private IHostingEnvironment _hostingEnvironment;
        private MapperConfiguration MapperConfiguration;
        private readonly IMapper mapper;
        private readonly IMoDataAPIRepository _moDataAPIRepository;
        private readonly IMoSpecAPIRepository _moSpecAPIRepository;
        private readonly IMoRoutingAPIRepository _moRoutingAPIRepository;
        private readonly IProductSpecService _productSpecService;
        private readonly IMasterDataAPIRepository _masterDataAPIRepository;
        private readonly IRoutingAPIRepository _routingAPIRepository;
        private readonly IPMTsConfigAPIRepository _pMTsConfigAPIRepository;
        private readonly IFluteTrAPIRepository _fluteTrAPIRepository;
        private readonly IFluteAPIRepository _fluteAPIRepository;
        private readonly IProductTypeAPIRepository _productTypeAPIRepository;
        private readonly IMachineAPIRepository _machineAPIRepository;
        private readonly ITransactionsDetailAPIRepository _transactionsDetailAPIRepository;
        private readonly IBoardUseAPIRepository _boardUseAPIRepository;
        private readonly IBoardAlternativeAPIRepository _boardAlternativeAPIRepository;
        private readonly IExtensionService _extensionService;
        private readonly IScoreTypeAPIRepository _scoreTypeAPIRepository;
        private readonly ICoatingAPIRepository _coatingAPIRepository;
        private readonly IBoardCombineAPIRepository _boardCombineAPIRepository;
        private readonly IBoardSpecAPIRepository _boardSpecAPIRepository;
        private readonly IQualitySpecAPIRepository _qualitySpecAPIRepository;
        private readonly IMasterUserAPIRepository _masterUserAPIRepository;
        private readonly IAttachFileMOAPIRepository _attachFileMOAPIRepository;
        private readonly IPPCRawMaterialProductionBomAPIRepository pPCRawMaterialProductionBomAPIRepository;
        private readonly IMoBomRawMatAPIRepository moBomRawMatAPIRepository;

        //tassanai
        private readonly IKindOfProductGroupAPIRepository _kindOfProductGroupAPIRepository;

        private readonly IKindOfProductAPIRepository _kindOfProductAPIRepository;
        private readonly IProcessCostAPIRepository _processCostAPIRepository;

        private readonly string _username;
        private readonly string _saleOrg;
        private readonly string _factoryCode;
        private readonly string _firstName;
        private readonly string _telephone;
        private readonly bool _businessGroup;
        private readonly string _token;

        public MasterCardService(IMoDataAPIRepository moDataAPIRepository,
            IMoSpecAPIRepository moSpecAPIRepository,
            IMoRoutingAPIRepository moRoutingAPIRepository,
            IProductSpecService productSpecService,
            IMasterDataAPIRepository masterDataAPIRepository,
            IRoutingAPIRepository routingAPIRepository,
            IPMTsConfigAPIRepository pMTsConfigAPIRepository,
            IHttpContextAccessor httpContextAccessor,
            IHostingEnvironment hostingEnvironment,
            IFluteTrAPIRepository fluteTrAPIRepository,
            IFluteAPIRepository fluteAPIRepository,
            IProductTypeAPIRepository productTypeAPIRepository,
            ITransactionsDetailAPIRepository transactionsDetailAPIRepository,
            IMachineAPIRepository machineAPIRepository,
            IBoardUseAPIRepository boardUseAPIRepository,
            IExtensionService extensionService,
            IScoreTypeAPIRepository scoreTypeAPIRepository,
            IBoardAlternativeAPIRepository boardAlternativeAPIRepository,
            ICoatingAPIRepository coatingAPIRepository,
            IKindOfProductGroupAPIRepository kindOfProductGroupAPIRepository,  //tassanai
            IKindOfProductAPIRepository kindOfProductAPIRepository, //tassanai
            IProcessCostAPIRepository processCostAPIRepository,
            IBoardCombineAPIRepository boardCombineAPIRepository,
            IBoardSpecAPIRepository boardSpecAPIRepository,
            IQualitySpecAPIRepository qualitySpecAPIRepository,
            IMasterUserAPIRepository masterUserAPIRepository,
            IAttachFileMOAPIRepository attachFileMOAPIRepository,
            IPPCRawMaterialProductionBomAPIRepository pPCRawMaterialProductionBomAPIRepository,
            IMoBomRawMatAPIRepository moBomRawMatAPIRepository,
            IMapper mapper
            )
        {
            _moDataAPIRepository = moDataAPIRepository;
            _moSpecAPIRepository = moSpecAPIRepository;
            _moRoutingAPIRepository = moRoutingAPIRepository;
            _productSpecService = productSpecService;
            _masterDataAPIRepository = masterDataAPIRepository;
            _routingAPIRepository = routingAPIRepository;
            _pMTsConfigAPIRepository = pMTsConfigAPIRepository;
            _fluteTrAPIRepository = fluteTrAPIRepository;
            _fluteAPIRepository = fluteAPIRepository;
            _httpContextAccessor = httpContextAccessor;
            hostingEnvironment = hostingEnvironment;
            _productTypeAPIRepository = productTypeAPIRepository;
            _transactionsDetailAPIRepository = transactionsDetailAPIRepository;
            _machineAPIRepository = machineAPIRepository;
            _boardUseAPIRepository = boardUseAPIRepository;
            _extensionService = extensionService;
            _scoreTypeAPIRepository = scoreTypeAPIRepository;
            _boardAlternativeAPIRepository = boardAlternativeAPIRepository;
            _coatingAPIRepository = coatingAPIRepository;
            _kindOfProductGroupAPIRepository = kindOfProductGroupAPIRepository;// tassanai
            _kindOfProductAPIRepository = kindOfProductAPIRepository;
            _processCostAPIRepository = processCostAPIRepository;
            _boardCombineAPIRepository = boardCombineAPIRepository;
            _boardSpecAPIRepository = boardSpecAPIRepository;
            _qualitySpecAPIRepository = qualitySpecAPIRepository;
            _masterUserAPIRepository = masterUserAPIRepository;
            _attachFileMOAPIRepository = attachFileMOAPIRepository;
            this.pPCRawMaterialProductionBomAPIRepository = pPCRawMaterialProductionBomAPIRepository;
            this.moBomRawMatAPIRepository = moBomRawMatAPIRepository;
            this.mapper = mapper;

            // Initialize User Data From Session
            var userSessionModel = SessionExtentions.GetSession<UserSessionModel>(_httpContextAccessor.HttpContext.Session, "UserSessionModel");
            if (userSessionModel != null)
            {
                _username = userSessionModel.UserName;
                _saleOrg = userSessionModel.SaleOrg;
                _factoryCode = userSessionModel.FactoryCode;
                _firstName = userSessionModel.FirstNameTh;
                _telephone = userSessionModel.Telephone;
                _token = userSessionModel.Token;
                _businessGroup = userSessionModel.BusinessGroup == "Offset";
            }
        }

        public MasterCardMO GetMasterCardMO(string Orderitem, bool isPreview, BasePrintMastercardData basePrintMastercard)
        {
            Orderitem = Orderitem.Trim();
            //MO_DATA
            var masterMOData = new MoDataPrintMastercard();
            masterMOData = basePrintMastercard.MoDatas.FirstOrDefault(m => m.OrderItem.Trim() == Orderitem);

            #region Update moRoutings From TIPs

            var tipMoData = new PlanningMODataAndMoSpec();
            var tipMoSpec = new PlanningMODataAndMoSpec();
            if (basePrintMastercard.PlanningMODataAndMoSpecs != null)
            {
                tipMoData = basePrintMastercard.PlanningMODataAndMoSpecs.FirstOrDefault(m => m.OrderItem.Trim() == Orderitem);

                if (tipMoData != null)
                {
                    UpdateTipMOData(ref masterMOData, tipMoData);
                }
            }

            #endregion Update moRoutings From TIPs

            //MasterData
            MasterData master = new MasterData();
            if (masterMOData is not null)
            {
                master = basePrintMastercard.MasterDatas.FirstOrDefault(m => m.MaterialNo.ToUpper().Trim() == masterMOData.MaterialNo.ToUpper().Trim() && m.PdisStatus != "X");
            }

            //MO_SPEC
            MoSpec moSpec = new MoSpec();
            moSpec = basePrintMastercard.MoSpecs.FirstOrDefault(m => m.OrderItem.Trim() == Orderitem);

            #region Update moRoutings From TIPs

            if (basePrintMastercard.PlanningMODataAndMoSpecs != null)
            {
                tipMoSpec = basePrintMastercard.PlanningMODataAndMoSpecs.FirstOrDefault(m => m.OrderItem.Trim() == Orderitem);
                if (tipMoSpec != null)
                {
                    UpdateTipMOSpec(ref moSpec, tipMoSpec);
                }
            }

            #endregion Update moRoutings From TIPs

            var isSheetBoard = moSpec.MaterialNo.ToLower().Contains("s/b") || moSpec.Hierarchy.Substring(2, 2).ToLower().Contains("sb") || master == null;
            //MO_ROUTING >> First
            MoRouting moRouting = new MoRouting();
            var moRoutings = new List<MoRoutingPrintMastercard>();
            var mainMORoutings = new List<MasterCardMoRouting>();
            var partOfMORoutings = new List<MasterCardMoRouting>();
            var hierarchyLv2 = string.Empty;
            var formGroup = string.Empty;
            string myFactory = string.Empty, ISO_DocDate = string.Empty, ISO_DocName = string.Empty;
            double tolerance_Over = 0, tolerance_Under = 0;
            var tempStations = new List<Station>();
            var stations = new List<Station>();
            string Setcut = "", SetLeng = "";
            MasterCardMO result = new MasterCardMO();
            var firstMachine = 0;
            var countRoutings = 0;
            var lineCount = 0;
            double? weightOut = null;
            var conditionPPCPrint = false;

            #region Find myFactory PmtsConfig

            List<PmtsConfig> pmts = new List<PmtsConfig>();
            pmts = basePrintMastercard.PmtsConfigs.Where(p => p.FactoryCode.Trim() == _factoryCode).ToList();
            //pmts = JsonConvert.DeserializeObject<List<PmtsConfig>>(_pMTsConfigAPIRepository.GetPMTsConfigList(_factoryCode));
            foreach (var x in pmts)
            {
                switch (x.FucName)
                {
                    case "Company":
                        myFactory = x.FucValue;
                        break;

                    case "ISO_DocDate":
                        ISO_DocDate = x.FucValue;
                        break;

                    case "ISO_DocName":
                        ISO_DocName = x.FucValue;
                        break;
                }
            }

            #endregion Find myFactory PmtsConfig

            if (isSheetBoard)
            {
                #region Set MO Routing

                moRoutings = basePrintMastercard.MoRoutings.Where(r => r.OrderItem.Trim() == Orderitem).OrderBy(r => r.SeqNo).ToList();

                #region Update moRoutings From TIPs

                if (basePrintMastercard.PlanningRoutings != null)
                {
                    var tipmoRoutings = basePrintMastercard.PlanningRoutings.Where(r => r.OrderItem.Trim() == Orderitem).OrderBy(r => r.Seq_No).ToList();
                    if (tipmoRoutings != null && tipmoRoutings.Count > 0)
                    {
                        UpdateTipMORouing(ref moRoutings, tipmoRoutings);
                    }
                }

                #endregion Update moRoutings From TIPs

                if (moRoutings.Count > 0)
                {
                    var hasCORR = moRoutings.Where(mr => mr.Machine.ToLower().Contains("cor") || mr.MatCode.ToLower().Contains("cor")).Count() > 0 ? true : false;
                    //has machine "COR" in MO routing
                    if (hasCORR)
                    {
                        moRouting = moRoutings.FirstOrDefault(mr => mr.Machine.ToLower().Contains("cor") || mr.MatCode.ToLower().Contains("cor"));
                        if (moRouting.CutNo != null && moRouting.CutNo.Value > 0)
                        {
                            if (masterMOData is not null)
                            {
                                //set จน.ตัด & ยาวเมตร
                                if (masterMOData.TargetQuant % moRouting.CutNo > 0)
                                {
                                    Setcut = String.Format("{0:N0}", Convert.ToDouble(masterMOData.TargetQuant / moRouting.CutNo) + 1);
                                }
                                else
                                {
                                    Setcut = String.Format("{0:N0}", Convert.ToDouble(masterMOData.TargetQuant / moRouting.CutNo));
                                }

                                int? t = 0;
                                if (!string.IsNullOrEmpty(moSpec.Flute) && moSpec.Flute.ToUpper().Equals("CP"))
                                {
                                    t = masterMOData.OrderQuant * moSpec.CutSheetLeng / 1000;
                                    SetLeng = String.Format("{0:N0}", t);
                                }
                                else
                                {
                                    if ((moSpec.CutSheetLeng * masterMOData.TargetQuant) % (moRouting.CutNo * 1000) > 0)
                                    {
                                        //SetLeng = String.Format("{0:n0}", (((T2.CutSheetLeng * T1.TargetQuant) % (T3.CutNo * 1000)) + 1));
                                        t = (((moSpec.CutSheetLeng * masterMOData.TargetQuant) / (moRouting.CutNo * 1000)) + 1);
                                        SetLeng = String.Format("{0:N0}", t);
                                    }
                                    else
                                    {
                                        SetLeng = String.Format("{0:N0}", ((moSpec.CutSheetLeng * masterMOData.TargetQuant) / (moRouting.CutNo * 1000)));
                                    }
                                }
                            }
                        }
                    }
                }

                foreach (var routing in moRoutings)
                {
                    countRoutings++;
                    var machineName = "";
                    var machineGroup = "";
                    bool showProcess = true;
                    var machine = basePrintMastercard.Machines.FirstOrDefault(m => m.PlanCode.ToUpper().Trim() == routing.PlanCode.ToUpper().Trim());
                    //var machine = JsonConvert.DeserializeObject<Machine>(_machineAPIRepository.GetMachineGroupByPlanCode(_factoryCode, routing.PlanCode));
                    machineGroup = machine != null ? machine.MachineGroup : null;
                    var machineIsCalPaperwidth = routing.Machine != null && machine != null && machine.IsCalPaperwidth.HasValue && machine.IsCalPaperwidth.Value;
                    showProcess = machine != null ? machine.ShowProcess : true;
                    if (showProcess)
                    {
                        if (routing.McMove.HasValue && routing.McMove.Value)
                        {
                            machineName = ReMachineName(null, routing);
                        }
                        else
                        {
                            machineName = routing.Machine + " (ห้ามย้ายเครื่อง)";
                        }

                        routing.Machine = machineName;
                        if (!string.IsNullOrEmpty(routing.ScoreType) && !string.IsNullOrWhiteSpace(routing.ScoreType))
                        {
                            var scoreType = JsonConvert.DeserializeObject<ScoreType>(_scoreTypeAPIRepository.GetScoreTypeByScoreTypeId(_factoryCode, routing.ScoreType, _token));
                            routing.ScoreType = scoreType != null ? scoreType.ScoreTypeName : routing.ScoreType;
                        }
                        else
                        {
                            routing.ScoreType = routing.ScoreType;
                        }
                        routing.Coatings = new List<Coating>();
                        lineCount = lineCount + MORoutingLineCount(routing);
                        conditionPPCPrint = _businessGroup ? lineCount > 110 : lineCount > 43;

                        if (conditionPPCPrint)
                        {
                            if (firstMachine == 0)
                            {
                                partOfMORoutings.Add(new MasterCardMoRouting
                                {
                                    MachineGroup = machineGroup,
                                    MachineIsCalPaperWidth = machineIsCalPaperwidth,
                                    Routing = routing,
                                    IsProp_Cor = machine.IsPropCor.Value || machine.IsCalPaperwidth.Value
                                });
                            }
                            else
                            {
                                partOfMORoutings.Add(new MasterCardMoRouting
                                {
                                    MachineGroup = machineGroup,
                                    MachineIsCalPaperWidth = machineIsCalPaperwidth,
                                    Routing = routing,
                                    IsProp_Cor = false
                                });
                            }

                            firstMachine++;
                        }
                        else
                        {
                            if (firstMachine == 0)
                            {
                                mainMORoutings.Add(new MasterCardMoRouting
                                {
                                    MachineGroup = machineGroup,
                                    MachineIsCalPaperWidth = machineIsCalPaperwidth,
                                    Routing = routing,
                                    IsProp_Cor = machine == null && routing.Machine.Contains("COR") ? true : machine.IsPropCor.Value || machine.IsCalPaperwidth.Value
                                });
                            }
                            else
                            {
                                mainMORoutings.Add(new MasterCardMoRouting
                                {
                                    MachineGroup = machineGroup,
                                    MachineIsCalPaperWidth = machineIsCalPaperwidth,
                                    Routing = routing,
                                    IsProp_Cor = false
                                });
                            }
                            firstMachine++;
                        }
                    }

                    if (countRoutings == moRoutings.Count)
                    {
                        weightOut = routing.WeightOut.HasValue ? routing.WeightOut.Value : new double();
                    }
                }

                #endregion Set MO Routing

                #region Set Stations

                var boardName = moSpec == null ? "" : moSpec.Board;
                // replace
                boardName = clean(boardName);
                var boardSplit = boardName == "" ? new List<string> { } : boardName.Split('/').ToList();

                var fluteTrs = basePrintMastercard.FluteTrs.Where(f => f.FluteCode.Trim() == moSpec.Flute.Trim()).OrderBy(f => f.Item).ToList();
                //var fluteTrs = JsonConvert.DeserializeObject<List<FluteTr>>(_fluteTrAPIRepository.GetFluteTrByFlute(_factoryCode, moSpec.Flute));

                stations.Clear();
                var boardNo = 0;
                foreach (var fluteTr in fluteTrs)
                {
                    var item = fluteTr.Item.Value;
                    var paperGrade = "";
                    if (fluteTr.FluteCode[0].ToString().ToLower() == "o" && fluteTr.Station.ToString().ToLower().Equals("top"))
                    {
                        item = 999;
                    }

                    if (boardNo >= boardSplit.Count)
                    {
                        paperGrade = "";
                    }
                    else
                    {
                        paperGrade = boardSplit[boardNo];
                    }

                    stations.Add(new Station
                    {
                        item = item,
                        TypeOfStation = fluteTr.Station,
                        PaperGrade = paperGrade,
                        Flute = fluteTr.FluteCode
                    });
                    boardNo++;
                }

                #endregion Set Stations

                #region Set Mastercard

                if (masterMOData is not null)
                {
                    var ymd = masterMOData.DateTimeStamp;
                    result.ProductType = moSpec.ProType;
                    result.CreateDate = moSpec.CreateDate.HasValue ? moSpec.CreateDate : null;
                    result.LastUpdate = moSpec.LastUpdate.HasValue ? moSpec.LastUpdate : null;
                    result.FactoryCode = _factoryCode;
                    result.Factory = myFactory;
                    result.DocName = ISO_DocName;
                    result.DocDate = ISO_DocDate;
                    result.OrderItem = masterMOData.OrderItem;
                    result.Material_No = masterMOData.MaterialNo;
                    result.Part_No = moSpec.PartNo;
                    result.PC = moSpec.Pc;
                    result.Cust_Name = masterMOData.Name.Length > 40 ? masterMOData.Name.Substring(0, 40) + $" ({masterMOData.SoldTo})" : masterMOData.Name + $" ({masterMOData.SoldTo})";
                    result.CustNameNOSoldto = masterMOData.Name.Length > 40 ? masterMOData.Name.Substring(0, 40) : masterMOData.Name;
                    result.CustNameNOSoldto = masterMOData.Name.Length > 40 ? masterMOData.Name.Substring(0, 40) : masterMOData.Name;
                    result.CustomerContact = $"{_firstName} {_telephone}";
                    result.Description = moSpec.Description;
                    result.Sale_Text1 = moSpec.SaleText1 + moSpec.SaleText2 + moSpec.SaleText3 + moSpec.SaleText4;
                    result.EanCode = moSpec.EanCode;
                    result.Box_Type = moSpec.BoxType;
                    result.RSC_Style = moSpec.RscStyle;
                    result.JoinType = moSpec.JoinType;
                    result.Print_Method = moSpec.PrintMethod;
                    result.PalletSize = moSpec.PalletSize;
                    result.Bun = moSpec.Bun == null ? 0 : moSpec.Bun;
                    result.BunLayer = moSpec.BunLayer == null ? null : moSpec.BunLayer;
                    result.LayerPalet = moSpec.LayerPalet == null ? 0 : moSpec.LayerPalet;
                    result.BoxPalet = moSpec.BoxPalet == null ? 0 : moSpec.BoxPalet;
                    result.Piece_Set = moSpec.PieceSet == null ? 0 : moSpec.PieceSet;
                    result.Material_Type = moSpec.MaterialType;
                    result.Status_Flag = moSpec.StatusFlag;
                    result.Wire = moSpec.Wire;
                    result.Wid = moSpec.Wid;
                    result.Leg = moSpec.Leg;
                    result.Hig = moSpec.Hig;
                    result.CutSheetWid = moSpec.CutSheetWid;
                    result.CutSheetLeng = moSpec.CutSheetLeng;
                    result.Flute = moSpec.Flute;
                    result.Batch = masterMOData.Batch;
                    result.ItemNote = masterMOData.ItemNote;
                    result.Due_Text = masterMOData.DueText;
                    result.Tolerance_Over = tolerance_Over;
                    result.Tolerance_Under = tolerance_Under;
                    result.Order_Quant = masterMOData.OrderQuant != null ? masterMOData.OrderQuant : 0;
                    result.ScoreW1 = moSpec.ScoreW1;
                    result.Scorew2 = moSpec.Scorew2;
                    result.Scorew3 = moSpec.Scorew3;
                    result.Scorew4 = moSpec.Scorew4;
                    result.Scorew5 = moSpec.Scorew5;
                    result.Scorew6 = moSpec.Scorew6;
                    result.Scorew7 = moSpec.Scorew7;
                    result.Scorew8 = moSpec.Scorew8;
                    result.Scorew9 = moSpec.Scorew9;
                    result.Scorew10 = moSpec.Scorew10;
                    result.Scorew11 = moSpec.Scorew11;
                    result.Scorew12 = moSpec.Scorew12;
                    result.Scorew13 = moSpec.Scorew13;
                    result.Scorew14 = moSpec.Scorew14;
                    result.Scorew15 = moSpec.Scorew15;
                    result.Scorew16 = moSpec.Scorew16;
                    result.ScoreL2 = moSpec.ScoreL2;
                    result.ScoreL3 = moSpec.ScoreL3;
                    result.ScoreL4 = moSpec.ScoreL4;
                    result.ScoreL5 = moSpec.ScoreL5;
                    result.ScoreL6 = moSpec.ScoreL6;
                    result.ScoreL7 = moSpec.ScoreL7;
                    result.ScoreL8 = moSpec.ScoreL8;
                    result.ScoreL9 = moSpec.ScoreL9;
                    result.FormGroup = "SB";
                    result.Palletization_Path = moSpec.PalletizationPath;
                    result.PalletPath_Base64 = string.IsNullOrEmpty(moSpec.PalletizationPath) || string.IsNullOrWhiteSpace(moSpec.PalletizationPath) ? "" : ConvertPictureToBase64._ConvertPictureToBase64(moSpec.PalletizationPath);
                    result.DiecutPict_Path = moSpec.DiecutPictPath;
                    result.DiecutPath_Base64 = string.IsNullOrEmpty(moSpec.DiecutPictPath) || string.IsNullOrWhiteSpace(moSpec.DiecutPictPath) ? "" : ConvertPictureToBase64._ConvertPictureToBase64(moSpec.DiecutPictPath);
                    result.Change = moSpec.Change;
                    result.Printed = masterMOData.Printed == null ? 0 : masterMOData.Printed.Value;
                    result.JointLap = moSpec.JointLap != null ? moSpec.JointLap.Value : 0;
                    result.StockQty = masterMOData.StockQty;
                    result.Distinct = string.IsNullOrEmpty(masterMOData.District) ? masterMOData.District : masterMOData.District.Trim();
                    result.TwoPiece = moSpec.TwoPiece != null ? moSpec.TwoPiece.Value : false;
                    result.Slit = ConvertInt16ToShort(moSpec.Slit);
                    result.Target_Quant = Convert.ToString(masterMOData.TargetQuant) == null ? "0" : Convert.ToString(masterMOData.TargetQuant);
                    result.CutNo = Setcut != null ? Setcut : "0";
                    result.Leng = SetLeng;
                    result.High_Value = moSpec.HighValue;
                    result.Hierarchy = moSpec.Hierarchy;
                    result.PoNo = masterMOData.PoNo;
                    result.SquareINCH = masterMOData.SquareInch;

                    //not sure
                    result.Stations = stations;
                    result.CutSheetLengInch = moSpec.CutSheetLeng.HasValue ? moSpec.CutSheetLeng.Value : 0;
                    result.CutSheetWidInch = moSpec.CutSheetWid.HasValue ? moSpec.CutSheetWid.Value : 0;

                    result.GlWid = false;
                    result.Piece_Patch = moSpec.PiecePatch.HasValue ? moSpec.PiecePatch.Value : 1;

                    var boardAlt = basePrintMastercard.BoardAlternatives.Where(b => b.MaterialNo.Trim() == masterMOData.MaterialNo.Trim()).ToList();
                    result.BoardAlternative = boardAlt.Count > 0 ? boardAlt.Where(b => b.Priority == 1).FirstOrDefault().BoardName : "";
                    result.TopSheetMaterial = !string.IsNullOrEmpty(moSpec.TopSheetMaterial) ? moSpec.TopSheetMaterial : null;
                    result.CustInvType = moSpec.CustInvType;
                    result.GrossWeight = weightOut.HasValue ? ((weightOut.Value * result.Order_Quant) / 1000).ToString() : null;
                    result.MoRout = new List<MasterCardMoRouting>();
                    result.PartOfMoRout = new List<MasterCardMoRouting>();
                    result.Rout = new List<MasterCardRouting>();
                    result.PartOfRout = new List<MasterCardRouting>();
                    var transDetail = master != null ? JsonConvert.DeserializeObject<TransactionsDetail>(_transactionsDetailAPIRepository.GetTransactionsDetailByMat(_factoryCode, master.MaterialNo, _token)) : null;

                    result.CGType = transDetail != null ?
                        transDetail.Cgtype == "B" ? "Base" :
                        transDetail.Cgtype == "L" ? "L Shape" :
                        transDetail.Cgtype == "U" ? "U Shape" : string.Empty
                        : string.Empty;
                    if (transDetail != null)
                    {
                        result.NewPrintPlate = transDetail.NewPrintPlate;
                        result.OldPrintPlate = transDetail.OldPrintPlate;
                        result.NewBlockDieCut = transDetail.NewBlockDieCut;
                        result.OldBlockDieCut = transDetail.OldBlockDieCut;
                        result.ExampleColor = transDetail.ExampleColor;
                        result.CoatingType = transDetail.CoatingType;
                        result.CoatingTypeDesc = transDetail.CoatingTypeDesc;
                        result.PaperHorizontal = transDetail.PaperHorizontal.HasValue ? transDetail.PaperHorizontal.Value : false;
                        result.PaperVertical = transDetail.PaperVertical.HasValue ? transDetail.PaperVertical.Value : false;
                        result.FluteHorizontal = transDetail.FluteHorizontal.HasValue ? transDetail.FluteHorizontal.Value : false;
                        result.FluteVertical = transDetail.FluteVertical.HasValue ? transDetail.FluteVertical.Value : false;
                    }
                    MoRoutingSetDetailOfMachineAndColor(ref mainMORoutings);
                    MoRoutingSetDetailOfMachineAndColor(ref partOfMORoutings);

                    result.PpcRawMaterialProductionBoms = new List<PpcRawMaterialProductionBom>();
                    result.MoBomRawmats = new List<MoBomRawMat>();
                    var moBomRawmats = JsonConvert.DeserializeObject<List<MoBomRawMat>>(moBomRawMatAPIRepository.GetMoBomRawMatsByFgMaterial(_factoryCode, masterMOData.MaterialNo, masterMOData.OrderItem, _token));
                    var ppcRawMaterialProductionBoms = JsonConvert.DeserializeObject<List<PpcRawMaterialProductionBom>>(pPCRawMaterialProductionBomAPIRepository.GetPPCRawMaterialProductionBOMsByFgMaterial(_factoryCode, masterMOData.MaterialNo, _token));
                    result.PpcRawMaterialProductionBoms.AddRange(ppcRawMaterialProductionBoms);
                    result.MoBomRawmats.AddRange(moBomRawmats);

                    result.MoRout = mainMORoutings;
                    result.PartOfMoRout = partOfMORoutings;

                    result.AllowancePrintNo = masterMOData.AllowancePrintNo.HasValue ? masterMOData.AllowancePrintNo.Value : 0;
                    result.PrintRoundNo = masterMOData.PrintRoundNo.HasValue ? masterMOData.PrintRoundNo.Value : 0;
                    result.AfterPrintNo = masterMOData.AfterPrintNo.HasValue ? masterMOData.AfterPrintNo.Value : 0;
                    result.DrawAmountNo = masterMOData.DrawAmountNo.HasValue ? masterMOData.DrawAmountNo.Value : 0;

                    //get attach file from sale order
                    result.AttchFilesBase64 = string.Empty;
                    //var attachfiles = basePrintMastercard.AttachFileMOs.Where(a => a.Status == true && a.FactoryCode == _factoryCode && a.OrderItem == Orderitem.Trim()).ToList();
                    var attachfiles = basePrintMastercard.AttachFileMOs.Where(a => a.Status == true && a.OrderItem == Orderitem.Trim()).ToList();
                    if (attachfiles.Count > 0)
                    {
                        result.AttchFilesBase64 = JsonConvert.SerializeObject(attachfiles);
                    }
                }

                #endregion Set Mastercard
            }
            else
            {
                hierarchyLv2 = master != null && master.Hierarchy is not null ? master.Hierarchy.Substring(2, 2) : string.Empty;
                var productTypeByHieLv2 = !string.IsNullOrEmpty(hierarchyLv2) ? JsonConvert.DeserializeObject<ProductType>(_productTypeAPIRepository.GetFormGroupByHierarchyLv2(_factoryCode, hierarchyLv2, _token)) : null;
                formGroup = productTypeByHieLv2 == null ? "RSC" : productTypeByHieLv2.FormGroup;

                moRoutings = basePrintMastercard.MoRoutings.Where(r => r.OrderItem.Trim() == Orderitem).OrderBy(r => r.SeqNo).ToList();

                #region Update moRoutings From TIPs

                if (basePrintMastercard.PlanningRoutings != null)
                {
                    var tipmoRoutings = basePrintMastercard.PlanningRoutings.Where(r => r.OrderItem.Trim() == Orderitem).OrderBy(r => r.Seq_No).ToList();
                    if (tipmoRoutings != null && tipmoRoutings.Count > 0)
                    {
                        UpdateTipMORouing(ref moRoutings, tipmoRoutings);
                    }
                }

                #endregion Update moRoutings From TIPs

                var hasCORR = false;
                if (moRoutings != null && moRoutings.Count > 0)
                {
                    moRouting = moRoutings.FirstOrDefault();
                    hasCORR = moRoutings.Where(mr => mr.Machine.ToLower().Contains("cor") || mr.MatCode.ToLower().Contains("cor")).Count() > 0 ? true : false;
                }
                var coating = new List<Coating>();
                if (masterMOData is not null) coating = GetCoatings(masterMOData.MaterialNo);

                foreach (var routing in moRoutings)
                {
                    countRoutings++;
                    var machineName = "";
                    var machineGroup = "";
                    var machine = basePrintMastercard.Machines.FirstOrDefault(m => m.PlanCode.ToUpper().Trim() == routing.PlanCode.ToUpper().Trim());
                    //var machine = JsonConvert.DeserializeObject<Machine>(_machineAPIRepository.GetMachineGroupByPlanCode(_factoryCode, routing.PlanCode));
                    machineGroup = routing.Machine != null && machine != null ? machine.MachineGroup : null;
                    var machineIsCalPaperwidth = routing.Machine != null && machine != null && machine.IsCalPaperwidth.HasValue && machine.IsCalPaperwidth.Value;

                    if (machine != null && machine.ShowProcess)
                    {
                        if (routing.McMove.HasValue && routing.McMove.Value)
                        {
                            machineName = ReMachineName(null, routing);
                        }
                        else
                        {
                            machineName = routing.Machine + " (ห้ามย้ายเครื่อง)";
                        }

                        routing.Machine = machineName;
                        if (!string.IsNullOrEmpty(routing.ScoreType) && !string.IsNullOrWhiteSpace(routing.ScoreType))
                        {
                            var scoreType = JsonConvert.DeserializeObject<ScoreType>(_scoreTypeAPIRepository.GetScoreTypeByScoreTypeId(_factoryCode, routing.ScoreType, _token));
                            routing.ScoreType = scoreType != null ? scoreType.ScoreTypeName : routing.ScoreType;
                        }
                        else
                        {
                            routing.ScoreType = routing.ScoreType;
                        }

                        lineCount = lineCount + MORoutingLineCount(routing);
                        if (!_businessGroup)
                        {
                            if (routing.Machine.Contains("COR"))
                            {
                                lineCount = coating.Count > 0 ? lineCount + coating.Count + 2 : lineCount;
                                routing.Coatings = coating.Count() == 0 ? new List<Coating>() : coating;
                            }

                            if (routing.Machine.Contains("COA"))
                            {
                                var qualitySpecs = master != null ? JsonConvert.DeserializeObject<List<QualitySpec>>(_qualitySpecAPIRepository.GetQualitySpecByMaterialNo(_factoryCode, master.MaterialNo, _token)) : null;
                                if (qualitySpecs != null && qualitySpecs.Count > 0)
                                {
                                    lineCount = lineCount + 1 + qualitySpecs.Count / 3;
                                    routing.QualitySpecs = QualitySpecsFromModel(qualitySpecs);
                                }
                            }
                            if (routing.Coatings == null)
                            {
                                routing.Coatings = new List<Coating>();
                            }
                        }

                        conditionPPCPrint = _businessGroup ? lineCount > 110 : lineCount > 43;
                        if (conditionPPCPrint)
                        {
                            if (firstMachine == 0)
                            {
                                partOfMORoutings.Add(new MasterCardMoRouting
                                {
                                    MachineGroup = machineGroup,
                                    MachineIsCalPaperWidth = machineIsCalPaperwidth,
                                    Routing = routing,
                                    IsProp_Cor = machine.IsPropCor.Value || machine.IsCalPaperwidth.Value
                                });
                            }
                            else
                            {
                                partOfMORoutings.Add(new MasterCardMoRouting
                                {
                                    MachineGroup = machineGroup,
                                    MachineIsCalPaperWidth = machineIsCalPaperwidth,
                                    Routing = routing,
                                    IsProp_Cor = false
                                });
                            }

                            firstMachine++;
                        }
                        else
                        {
                            if (firstMachine == 0)
                            {
                                mainMORoutings.Add(new MasterCardMoRouting
                                {
                                    MachineGroup = machineGroup,
                                    MachineIsCalPaperWidth = machineIsCalPaperwidth,
                                    Routing = routing,
                                    IsProp_Cor = machine.IsPropCor.Value || machine.IsCalPaperwidth.Value
                                });
                            }
                            else
                            {
                                mainMORoutings.Add(new MasterCardMoRouting
                                {
                                    MachineGroup = machineGroup,
                                    MachineIsCalPaperWidth = machineIsCalPaperwidth,
                                    Routing = routing,
                                    IsProp_Cor = false
                                });
                            }
                            firstMachine++;
                        }

                        if (countRoutings == moRoutings.Count)
                        {
                            weightOut = routing.WeightOut.HasValue ? routing.WeightOut.Value : new double();
                        }
                    }
                }

                if (tipMoSpec != null && !string.IsNullOrEmpty(tipMoSpec.Board))
                {
                    //set board from TIPs
                    var boardSplit = tipMoSpec.Board.Split('/').ToList();
                    var fluteTrs = basePrintMastercard.FluteTrs.Where(f => f.FluteCode.Trim() == moSpec.Flute.Trim()).OrderBy(f => f.Item).ToList();

                    stations.Clear();
                    var boardNo = 0;
                    foreach (var fluteTr in fluteTrs)
                    {
                        var item = fluteTr.Item.Value;
                        var paperGrade = "";
                        if (fluteTr.FluteCode[0].ToString().ToLower() == "o" && fluteTr.Station.ToString().ToLower().Equals("top"))
                        {
                            item = 999;
                        }

                        if (boardNo >= boardSplit.Count)
                        {
                            paperGrade = "";
                        }
                        else
                        {
                            paperGrade = boardSplit[boardNo];
                        }

                        stations.Add(new Station
                        {
                            item = item,
                            TypeOfStation = fluteTr.Station,
                            PaperGrade = paperGrade,
                            Flute = fluteTr.FluteCode
                        });
                        boardNo++;
                    }
                }
                else
                {
                    var boardCombine = master != null && master.Code != null ? JsonConvert.DeserializeObject<BoardCombine>(_boardCombineAPIRepository.GetBoardByCode(_factoryCode, master.Code, _token)) : null;

                    if (boardCombine != null)
                    {
                        if (boardCombine.StandardBoard.Value)
                        {
                            var boardUse = master != null ? basePrintMastercard.BoardUses.FirstOrDefault(b => b.FactoryCode == _factoryCode && b.MaterialNo == master.MaterialNo) : null; //JsonConvert.DeserializeObject<BoardUse>(_boardUseAPIRepository.GetBoardUseByMaterialNo(_factoryCode, master.MaterialNo, _token));
                            var boardName = boardUse == null ? "" : boardUse.BoardName;
                            // replace
                            boardName = clean(boardName);
                            var boardSplit = boardName == "" ? new List<string> { } : boardName.Split('/').ToList();

                            var fluteTrs = basePrintMastercard.FluteTrs.Where(f => f.FluteCode.Trim() == moSpec.Flute.Trim()).OrderBy(f => f.Item).ToList();

                            stations.Clear();
                            var boardNo = 0;
                            foreach (var fluteTr in fluteTrs)
                            {
                                var item = fluteTr.Item.Value;
                                var paperGrade = "";
                                if (fluteTr.FluteCode[0].ToString().ToLower() == "o" && fluteTr.Station.ToString().ToLower().Equals("top"))
                                {
                                    item = 999;
                                }

                                if (boardNo >= boardSplit.Count)
                                {
                                    paperGrade = "";
                                }
                                else
                                {
                                    paperGrade = boardSplit[boardNo];
                                }

                                stations.Add(new Station
                                {
                                    item = item,
                                    TypeOfStation = fluteTr.Station,
                                    PaperGrade = paperGrade,
                                    Flute = fluteTr.FluteCode
                                });
                                boardNo++;
                            }
                        }
                        else
                        {
                            stations.Clear();
                            var boardSpecs = JsonConvert.DeserializeObject<List<BoardSpec>>(_boardSpecAPIRepository.GetBoardSpecByCode(_factoryCode, boardCombine.Code, _token));
                            foreach (var boardSpec in boardSpecs)
                            {
                                stations.Add(new Station
                                {
                                    item = boardSpec.Item.Value,
                                    TypeOfStation = boardSpec.Station,
                                    PaperGrade = boardSpec.Grade,
                                    Flute = null
                                });
                            }
                        }
                    }
                }

                //has machine "COR" in MO routing
                if (hasCORR)
                {
                    if (moRouting.CutNo.Value > 0)
                    {
                        if (masterMOData is not null)
                        {
                            if (masterMOData.TargetQuant % moRouting.CutNo > 0)
                            {
                                Setcut = String.Format("{0:N0}", Convert.ToDouble(masterMOData.TargetQuant / moRouting.CutNo) + 1);
                            }
                            else
                            {
                                Setcut = String.Format("{0:N0}", Convert.ToDouble(masterMOData.TargetQuant / moRouting.CutNo));
                            }

                            int? t = 0;
                            if (!string.IsNullOrEmpty(moSpec.Flute) && moSpec.Flute.ToUpper().Equals("CP"))
                            {
                                t = masterMOData.OrderQuant * moSpec.CutSheetLeng / 1000;
                                SetLeng = String.Format("{0:N0}", t);
                            }
                            else
                            {
                                if ((moSpec.CutSheetLeng * masterMOData.TargetQuant) % (moRouting.CutNo * 1000) > 0)
                                {
                                    //SetLeng = String.Format("{0:n0}", (((T2.CutSheetLeng * T1.TargetQuant) % (T3.CutNo * 1000)) + 1));
                                    t = (((moSpec.CutSheetLeng * masterMOData.TargetQuant) / (moRouting.CutNo * 1000)) + 1);
                                    SetLeng = String.Format("{0:N0}", t);
                                }
                                else
                                {
                                    SetLeng = String.Format("{0:N0}", ((moSpec.CutSheetLeng * masterMOData.TargetQuant) / (moRouting.CutNo * 1000)));
                                }
                            }
                        }
                        //set จน.ตัด & ยาวเมตร
                    }
                }
                else
                {
                }
                if (masterMOData is not null)
                {
                    tolerance_Over = masterMOData.ToleranceOver.HasValue && masterMOData.ToleranceOver.Value > 0 ? (masterMOData.OrderQuant * Math.Truncate(masterMOData.ToleranceOver.Value)) / 100 : 0;
                    tolerance_Under = masterMOData.ToleranceUnder.HasValue && masterMOData.ToleranceUnder.Value > 0 ? (masterMOData.OrderQuant * Math.Truncate(masterMOData.ToleranceUnder.Value)) / 100 : 0;

                    tolerance_Over = CellingFloat(tolerance_Over.ToString());
                    tolerance_Under = CellingFloat(tolerance_Under.ToString());

                    #region Set masterCard

                    result.ProductType = moSpec.ProType;
                    result.CreateDate = moSpec.CreateDate.HasValue ? moSpec.CreateDate : null;
                    result.LastUpdate = moSpec.LastUpdate.HasValue ? moSpec.LastUpdate : null;
                    result.FactoryCode = _factoryCode;
                    result.Factory = myFactory;
                    result.DocName = ISO_DocName;
                    result.DocDate = ISO_DocDate;
                    result.OrderItem = masterMOData.OrderItem;
                    result.Material_No = masterMOData.MaterialNo;
                    result.Part_No = moSpec.PartNo;
                    result.PC = moSpec.Pc;
                    result.Cust_Name = masterMOData.Name.Length > 40 ? masterMOData.Name.Substring(0, 40) + $" ({masterMOData.SoldTo})" : masterMOData.Name + $" ({masterMOData.SoldTo})";
                    result.CustNameNOSoldto = masterMOData.Name.Length > 40 ? masterMOData.Name.Substring(0, 40) : masterMOData.Name;
                    result.CustomerContact = $"{_firstName} {_telephone}";
                    result.Description = moSpec.Description;
                    result.Sale_Text1 = moSpec.SaleText1 + moSpec.SaleText2 + moSpec.SaleText3 + moSpec.SaleText4;
                    result.EanCode = moSpec.EanCode;
                    result.Box_Type = moSpec.BoxType;
                    result.RSC_Style = moSpec.RscStyle;
                    result.JoinType = moSpec.JoinType;
                    result.Print_Method = moSpec.PrintMethod;
                    result.PalletSize = moSpec.PalletSize;
                    result.Bun = moSpec.Bun == null ? 0 : moSpec.Bun;
                    result.BunLayer = moSpec.BunLayer == null ? 0 : moSpec.BunLayer;
                    result.Material_Type = moSpec.MaterialType;
                    result.Status_Flag = moSpec.StatusFlag;
                    result.LayerPalet = moSpec.LayerPalet == null ? 0 : moSpec.LayerPalet;
                    result.BoxPalet = moSpec.BoxPalet == null ? 0 : moSpec.BoxPalet;
                    result.Piece_Set = moSpec.PieceSet == null ? 0 : moSpec.PieceSet;
                    result.Wire = moSpec.Wire;
                    result.Wid = moSpec.Wid;
                    result.Leg = moSpec.Leg;
                    result.Hig = moSpec.Hig;
                    result.CutSheetWid = moSpec.CutSheetWid;
                    result.CutSheetLeng = moSpec.CutSheetLeng;
                    result.CutSheetLengInch = moSpec.CutSheetLengInch.HasValue ? (moSpec.CutSheetLengInch.Value) : 0;
                    result.CutSheetWidInch = moSpec.CutSheetWidInch.HasValue ? (moSpec.CutSheetWidInch.Value) : 0;
                    result.Flute = moSpec.Flute;
                    result.Batch = masterMOData.Batch;
                    result.ItemNote = masterMOData.ItemNote;
                    result.Due_Text = masterMOData.DueText;
                    result.Tolerance_Over = tolerance_Over;
                    result.Tolerance_Under = tolerance_Under;
                    result.Order_Quant = masterMOData.OrderQuant != null ? masterMOData.OrderQuant : 0;
                    result.ScoreW1 = moSpec.ScoreW1;
                    result.Scorew2 = moSpec.Scorew2;
                    result.Scorew3 = moSpec.Scorew3;
                    result.Scorew4 = moSpec.Scorew4;
                    result.Scorew5 = moSpec.Scorew5;
                    result.Scorew6 = moSpec.Scorew6;
                    result.Scorew7 = moSpec.Scorew7;
                    result.Scorew8 = moSpec.Scorew8;
                    result.Scorew9 = moSpec.Scorew9;
                    result.Scorew10 = moSpec.Scorew10;
                    result.Scorew11 = moSpec.Scorew11;
                    result.Scorew12 = moSpec.Scorew12;
                    result.Scorew13 = moSpec.Scorew13;
                    result.Scorew14 = moSpec.Scorew14;
                    result.Scorew15 = moSpec.Scorew15;
                    result.Scorew16 = moSpec.Scorew16;
                    result.ScoreL2 = moSpec.ScoreL2;
                    result.ScoreL3 = moSpec.ScoreL3;
                    result.ScoreL4 = moSpec.ScoreL4;
                    result.ScoreL5 = moSpec.ScoreL5;
                    result.ScoreL6 = moSpec.ScoreL6;
                    result.ScoreL7 = moSpec.ScoreL7;
                    result.ScoreL8 = moSpec.ScoreL8;
                    result.ScoreL9 = moSpec.ScoreL9;
                    result.Stations = stations;
                    result.Palletization_Path = moSpec.PalletizationPath;
                    result.PalletPath_Base64 = string.IsNullOrEmpty(moSpec.PalletizationPath) || string.IsNullOrWhiteSpace(moSpec.PalletizationPath) ? "" : ConvertPictureToBase64._ConvertPictureToBase64(moSpec.PalletizationPath);
                    result.DiecutPict_Path = moSpec.DiecutPictPath;
                    result.DiecutPath_Base64 = string.IsNullOrEmpty(moSpec.DiecutPictPath) || string.IsNullOrWhiteSpace(moSpec.DiecutPictPath) ? "" : ConvertPictureToBase64._ConvertPictureToBase64(moSpec.DiecutPictPath);
                    result.Change = moSpec.Change;

                    result.MoRout = new List<MasterCardMoRouting>();
                    result.PartOfMoRout = new List<MasterCardMoRouting>();
                    result.PpcRawMaterialProductionBoms = new List<PpcRawMaterialProductionBom>();
                    result.MoBomRawmats = new List<MoBomRawMat>();
                    var moBomRawmats = JsonConvert.DeserializeObject<List<MoBomRawMat>>(moBomRawMatAPIRepository.GetMoBomRawMatsByFgMaterial(_factoryCode, masterMOData.MaterialNo, masterMOData.OrderItem, _token));
                    var ppcRawMaterialProductionBoms = JsonConvert.DeserializeObject<List<PpcRawMaterialProductionBom>>(pPCRawMaterialProductionBomAPIRepository.GetPPCRawMaterialProductionBOMsByFgMaterial(_factoryCode, masterMOData.MaterialNo, _token));
                    result.PpcRawMaterialProductionBoms.AddRange(ppcRawMaterialProductionBoms);
                    result.MoBomRawmats.AddRange(moBomRawmats);

                    MoRoutingSetDetailOfMachineAndColor(ref mainMORoutings);
                    MoRoutingSetDetailOfMachineAndColor(ref partOfMORoutings);

                    result.MoRout = mainMORoutings;
                    result.PartOfMoRout = partOfMORoutings;
                    result.Rout = new List<MasterCardRouting>();
                    result.PartOfRout = new List<MasterCardRouting>();
                    result.Target_Quant = Convert.ToString(masterMOData.TargetQuant) == null ? "0" : Convert.ToString(masterMOData.TargetQuant);
                    result.CutNo = Setcut != null ? Setcut : "0";
                    result.Leng = SetLeng;
                    result.TwoPiece = moSpec.TwoPiece != null ? moSpec.TwoPiece.Value : false;
                    result.Slit = moSpec.Slit;
                    var piecePatch = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNo(_factoryCode, masterMOData.MaterialNo, _token));
                    result.Piece_Patch = piecePatch != null ? piecePatch.PiecePatch : null;
                    result.StockQty = masterMOData.StockQty;
                    var transDetail = master != null ? JsonConvert.DeserializeObject<TransactionsDetail>(_transactionsDetailAPIRepository.GetTransactionsDetailByMat(_factoryCode, master.MaterialNo, _token)) : null;
                    if (transDetail != null)
                    {
                        result.NewPrintPlate = transDetail.NewPrintPlate;
                        result.OldPrintPlate = transDetail.OldPrintPlate;
                        result.NewBlockDieCut = transDetail.NewBlockDieCut;
                        result.OldBlockDieCut = transDetail.OldBlockDieCut;
                        result.ExampleColor = transDetail.ExampleColor;
                        result.CoatingType = transDetail.CoatingType;
                        result.CoatingTypeDesc = transDetail.CoatingTypeDesc;
                        result.PaperHorizontal = transDetail.PaperHorizontal.HasValue ? transDetail.PaperHorizontal.Value : false;
                        result.PaperVertical = transDetail.PaperVertical.HasValue ? transDetail.PaperVertical.Value : false;
                        result.FluteHorizontal = transDetail.FluteHorizontal.HasValue ? transDetail.FluteHorizontal.Value : false;
                        result.FluteVertical = transDetail.FluteVertical.HasValue ? transDetail.FluteVertical.Value : false;
                    }
                    result.GlWid = transDetail == null || transDetail.Glwid == null ? false : transDetail.Glwid.Value;
                    result.Distinct = string.IsNullOrEmpty(masterMOData.District) ? masterMOData.District : masterMOData.District.Trim();
                    var boardAlt = basePrintMastercard.BoardAlternatives.Where(b => b.MaterialNo.Trim() == masterMOData.MaterialNo.Trim()).ToList();
                    result.BoardAlternative = boardAlt.Count > 0 ? boardAlt.OrderBy(p => p.Priority).FirstOrDefault().BoardName : "";
                    result.FormGroup = formGroup.ToString().Trim();
                    result.High_Value = master != null ? master.HighValue : string.Empty;
                    result.Hierarchy = master != null ? master.Hierarchy : string.Empty;
                    result.Printed = masterMOData.Printed == null ? 0 : masterMOData.Printed.Value;
                    result.JointLap = moSpec.JointLap != null ? moSpec.JointLap.Value : 0;
                    result.IsXStatus = masterMOData.MoStatus.ToLower().Trim().Contains('x') == true ? true : false;
                    result.NoSlot = formGroup.ToString().Trim() == "AC" && master != null ? master.NoSlot.Value : 0;
                    result.PoNo = masterMOData.PoNo;
                    result.SquareINCH = masterMOData.SquareInch;
                    result.TopSheetMaterial = !string.IsNullOrEmpty(moSpec.TopSheetMaterial) ? moSpec.TopSheetMaterial : null;
                    result.CustInvType = moSpec.CustInvType;
                    result.GrossWeight = weightOut.HasValue ? ((weightOut.Value * result.Order_Quant) / 1000).ToString() : null;
                    //get attach file from sale order
                    result.AttchFilesBase64 = string.Empty;
                    //var attachfiles = basePrintMastercard.AttachFileMOs.Where(a => a.Status == true && a.FactoryCode == _factoryCode && a.OrderItem == Orderitem.Trim()).ToList();
                    var attachfiles = basePrintMastercard.AttachFileMOs.Where(a => a.Status == true && a.OrderItem == Orderitem.Trim()).ToList();
                    result.NoTagBundle = moSpec.NoTagBundle;
                    result.TagBundle = moSpec.TagBundle;
                    result.TagPallet = moSpec.TagPallet;
                    result.CGType = transDetail != null ?
                        transDetail.Cgtype == "B" ? "Base" :
                        transDetail.Cgtype == "L" ? "L Shape" :
                        transDetail.Cgtype == "U" ? "U Shape" : string.Empty
                        : string.Empty;
                    if (attachfiles.Count > 0)
                    {
                        result.AttchFilesBase64 = JsonConvert.SerializeObject(attachfiles);
                    }

                    #endregion Set masterCard

                    //tassanai 22020222
                    result.CustCode = moSpec.CustCode;

                    result.AllowancePrintNo = masterMOData.AllowancePrintNo.HasValue ? masterMOData.AllowancePrintNo.Value : 0;
                    result.PrintRoundNo = masterMOData.PrintRoundNo.HasValue ? masterMOData.PrintRoundNo.Value : 0;
                    result.AfterPrintNo = masterMOData.AfterPrintNo.HasValue ? masterMOData.AfterPrintNo.Value : 0;
                    result.DrawAmountNo = masterMOData.DrawAmountNo.HasValue ? masterMOData.DrawAmountNo.Value : 0;
                }
            }
            result.IsPreview = isPreview;
            //tassanai 22020222
            result.CustCode = moSpec.CustCode;
            return result;
        }

        public MasterCardMO GetMasterCard(string MaterialNo, BasePrintMastercardData basePrintMastercard)
        {
            MaterialNo = MaterialNo.Trim();
            var factoryCode = _factoryCode;
            //MasterData
            MasterData master = new MasterData();

            master = basePrintMastercard.MasterDatas.FirstOrDefault(m => m.MaterialNo.ToUpper().Trim() == MaterialNo.ToUpper() && m.PdisStatus != "X");
            //master = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNo(factoryCode, MaterialNo));

            //MO_ROUTING >> First
            // var xc = JsonConvert.DeserializeObject<Routing>(_routingAPIRepository.GetRoutingByMaterialNo(_factoryCode, MaterialNo));
            //Routing T3 = new Routing();
            //T3 = JsonConvert.DeserializeObject<List<Routing>>(_routingAPIRepository.GetRoutingsByMaterialNo(factoryCode, MaterialNo)).FirstOrDefault(x => x.SeqNo == 1);

            var hierarchyLv2 = master.Hierarchy.Substring(2, 2);
            var productTypeByHieLv2 = basePrintMastercard.ProductTypes.FirstOrDefault(p => p.HierarchyLv2.Trim() == hierarchyLv2.Trim());
            //var productTypeByHieLv2 = JsonConvert.DeserializeObject<ProductType>(_productTypeAPIRepository.GetFormGroupByHierarchyLv2(factoryCode, hierarchyLv2));
            var formGroup = productTypeByHieLv2.FormGroup;

            var boardCombine = master.Code != null ? basePrintMastercard.BoardCombines.FirstOrDefault(b => b.Code.Trim() == master.Code.Trim()) : null;
            //var boardCombine = master.Code != null ? JsonConvert.DeserializeObject<BoardCombine>(_boardCombineAPIRepository.GetBoardByCode(factoryCode, master.Code)) : null;
            var tempStations = new List<Station>();
            var stations = new List<Station>();
            int firstMachine = 0;
            var grossWeight = string.Empty;

            if (boardCombine != null)
            {
                if (boardCombine.StandardBoard.Value)
                {
                    var boardUse = basePrintMastercard.BoardUses.FirstOrDefault(b => b.MaterialNo == master.MaterialNo && b.FactoryCode == _factoryCode);
                    //var boardUse = JsonConvert.DeserializeObject<BoardUse>(_boardUseAPIRepository.GetBoardUseByMaterialNo(factoryCode, master.MaterialNo));
                    var boardName = boardUse == null ? "" : boardUse.BoardName;
                    // replace
                    boardName = clean(boardName);
                    var boardSplit = boardName == "" ? new List<string> { } : boardName.Split('/').ToList();

                    var fluteTrs = basePrintMastercard.FluteTrs.Where(f => f.FactoryCode == _factoryCode && f.FluteCode == boardCombine.Flute.ToUpper()).OrderBy(f => f.Item).ToList();
                    //var fluteTrs = JsonConvert.DeserializeObject<List<FluteTr>>(_fluteTrAPIRepository.GetFluteTrByFlute(factoryCode, boardCombine.Flute)).OrderBy(f => f.Item).ToList();

                    stations.Clear();
                    var boardNo = 0;
                    foreach (var fluteTr in fluteTrs)
                    {
                        var item = fluteTr.Item.Value;
                        var paperGrade = "";
                        if (fluteTr.FluteCode[0].ToString().ToLower() == "o" && fluteTr.Station.ToString().ToLower().Equals("top"))
                        {
                            item = 999;
                        }

                        if (boardNo >= boardSplit.Count)
                        {
                            paperGrade = "";
                        }
                        else
                        {
                            paperGrade = boardSplit[boardNo];
                        }

                        stations.Add(new Station
                        {
                            item = item,
                            TypeOfStation = fluteTr.Station,
                            PaperGrade = paperGrade,
                            Flute = fluteTr.FluteCode
                        });
                        boardNo++;
                    }
                }
                else
                {
                    stations.Clear();
                    var boardSpecs = basePrintMastercard.BoardSpecs.Where(b => b.Code.Trim() == boardCombine.Code.Trim());
                    //var boardSpecs = JsonConvert.DeserializeObject<List<BoardSpec>>(_boardSpecAPIRepository.GetBoardSpecByCode(factoryCode, boardCombine.Code));
                    foreach (var boardSpec in boardSpecs)
                    {
                        stations.Add(new Station
                        {
                            item = boardSpec.Item.Value,
                            TypeOfStation = boardSpec.Station,
                            PaperGrade = boardSpec.Grade,
                            Flute = null
                        });
                    }
                }
            }

            //TblPmtsConfig
            List<PmtsConfig> pmts = new List<PmtsConfig>();
            pmts = basePrintMastercard.PmtsConfigs; ;
            //pmts = JsonConvert.DeserializeObject<List<PmtsConfig>>(_pMTsConfigAPIRepository.GetPMTsConfigList(factoryCode));
            string myFactory = "", ISO_DocDate = "", ISO_DocName = "";
            foreach (var x in pmts)
            {
                switch (x.FucName)
                {
                    case "Import_PrefixName":
                        myFactory = x.FucValue;
                        //myFactory = "aaaa";
                        break;

                    case "ISO_DocDate":
                        ISO_DocDate = x.FucValue;
                        break;

                    case "ISO_DocName":
                        ISO_DocName = x.FucValue;
                        break;
                }
            }

            var lineCount = 0;
            var conditionPPCPrint = false;
            var routings = basePrintMastercard.Routings.Where(r => r.MaterialNo == MaterialNo && r.FactoryCode == _factoryCode && r.PdisStatus != "X").OrderBy(r => r.SeqNo).ToList(); //GetRouting(MaterialNo);
            var masterCardRouting = new List<MasterCardRouting>();
            var partOfMasterCardRouting = new List<MasterCardRouting>();
            var coatings = new List<Coating>();
            coatings = GetCoatings(MaterialNo);

            foreach (var routing in routings)
            {
                var machineName = "";
                var machine = basePrintMastercard.Machines.FirstOrDefault(m => m.FactoryCode == _factoryCode && m.PlanCode.ToUpper().Trim() == routing.PlanCode.ToUpper().Trim());
                //var machine = JsonConvert.DeserializeObject<Machine>(_machineAPIRepository.GetMachineGroupByPlanCode(factoryCode, routing.PlanCode));
                var machineGroup = routing.Machine != null && machine != null ? machine.MachineGroup : null;
                var machineIsCalPaperwidth = routing.Machine != null && machine != null && machine.IsCalPaperwidth.HasValue && machine.IsCalPaperwidth.Value;
                if (routing.McMove.Value)
                {
                    machineName = ReMachineName(routing, null);
                }
                else
                {
                    machineName = routing.Machine + " (ห้ามย้ายเครื่อง)";
                }

                routing.Machine = machineName;
                routing.ScoreType = !string.IsNullOrEmpty(routing.ScoreType) && !string.IsNullOrWhiteSpace(routing.ScoreType) ? basePrintMastercard.ScoreTypes.FirstOrDefault(s => s.FactoryCode == factoryCode && s.ScoreTypeId.Trim() == routing.ScoreType.Trim()).ScoreTypeName : routing.ScoreType;
                //routing.ScoreType = !string.IsNullOrEmpty(routing.ScoreType) && !string.IsNullOrWhiteSpace(routing.ScoreType) ? JsonConvert.DeserializeObject<ScoreType>(_scoreTypeAPIRepository.GetScoreTypeByScoreTypeId(factoryCode, routing.ScoreType)).ScoreTypeName : routing.ScoreType;

                lineCount = lineCount + RoutingLineCount(routing);

                var cloneRouting = mapper.Map<CloneRouting>(routing);
                cloneRouting.Coatings = new List<Coating>();

                if (!_businessGroup)
                {
                    if (routing.Machine.Contains("COA"))
                    {
                        var qualitySpecs = basePrintMastercard.QualitySpecs.Where(q => q.FactoryCode == _factoryCode && q.MaterialNo.Trim() == master.MaterialNo).ToList();
                        if (qualitySpecs != null && qualitySpecs.Count > 0)
                        {
                            lineCount = lineCount + 1 + qualitySpecs.Count / 3;
                            cloneRouting.QualitySpecs = QualitySpecsFromModel(qualitySpecs);
                        }
                        cloneRouting.CustBarcodeNo = routing.CustBarcodeNo;
                    }

                    if (machineGroup == "1")
                    {
                        lineCount = coatings.Count() > 0 ? lineCount + coatings.Count() + 2 : lineCount;
                        cloneRouting.Coatings = coatings;
                    }
                }

                conditionPPCPrint = _businessGroup ? lineCount > 110 : lineCount > 43;
                if (conditionPPCPrint)
                {
                    partOfMasterCardRouting.Add(new MasterCardRouting
                    {
                        Routing = cloneRouting,
                        MachineGroup = machineGroup,
                        MachineIsCalPaperWidth = machineIsCalPaperwidth,
                        IsProp_Cor = false
                    });
                }
                else
                {
                    masterCardRouting.Add(new MasterCardRouting
                    {
                        Routing = cloneRouting,
                        MachineGroup = machineGroup,
                        MachineIsCalPaperWidth = machineIsCalPaperwidth,
                        IsProp_Cor = false
                    });
                }
            }

            #region Set masterCard

            MasterCardMO result = new MasterCardMO();
            result.ProductType = master.ProType;
            result.CreateDate = master.CreateDate.HasValue ? master.CreateDate : null;
            result.LastUpdate = master.LastUpdate.HasValue ? master.LastUpdate : null;
            var transactionsDetail = basePrintMastercard.TransactionsDetails.FirstOrDefault(t => t.FactoryCode == _factoryCode && t.MaterialNo.Trim() == master.MaterialNo.Trim());
            result.GlWid = transactionsDetail == null || transactionsDetail.Glwid == null ? false : transactionsDetail.Glwid.Value;
            result.FactoryCode = factoryCode;
            result.Factory = myFactory;
            result.CustInvType = master.CustInvType;
            result.GrossWeight = grossWeight;
            result.DocName = ISO_DocName;
            result.DocDate = ISO_DocDate;
            result.OrderItem = null;
            //OrderItem = T1.OrderItem,
            result.Material_No = master.MaterialNo;
            result.Part_No = master.PartNo;
            result.PC = master.Pc;
            result.Cust_Name = master.CustName.Length > 40 ? master.CustName.Substring(0, 40) + $" ({master.CusId})" : master.CustName + $" ({master.CusId})";
            result.CustNameNOSoldto = master.CustName.Length > 40 ? master.CustName.Substring(0, 40) : master.CustName;
            result.CustomerContact = $"{_firstName} {_telephone}";
            result.Description = master.Description;
            result.Sale_Text1 = master.SaleText1 + master.SaleText2 + master.SaleText3 + master.SaleText4;
            result.EanCode = master.EanCode;
            result.Box_Type = master.BoxType;
            result.RSC_Style = master.RscStyle;
            result.JoinType = master.JoinType;
            result.Print_Method = master.PrintMethod;
            result.PalletSize = master.PalletSize;
            result.Bun = master.Bun == null ? 0 : master.Bun;
            result.BunLayer = master.BunLayer == null ? null : master.BunLayer;
            result.LayerPalet = master.LayerPalet == null ? 0 : master.LayerPalet;
            result.BoxPalet = master.BoxPalet == null ? 0 : master.BoxPalet;
            result.Material_Type = master.MaterialType;
            result.Status_Flag = master.StatusFlag;
            result.Piece_Set = master.PieceSet == null ? 0 : master.PieceSet;
            result.Piece_Patch = master.PiecePatch.HasValue ? master.PiecePatch.Value : 1;
            result.Wire = master.Wire;
            result.Wid = master.Wid;
            result.Leg = master.Leg;
            result.Hig = master.Hig;
            result.CutSheetWid = master.CutSheetWid;
            result.CutSheetLeng = master.CutSheetLeng;
            result.CutSheetLengInch = master.CutSheetLengInch.HasValue ? (master.CutSheetLengInch.Value) : 0;
            result.CutSheetWidInch = master.CutSheetWidInch.HasValue ? (master.CutSheetWidInch.Value) : 0;
            result.Flute = master.Flute;
            result.Batch = "";
            result.Due_Text = "";
            result.Tolerance_Over = null;
            result.Tolerance_Under = null;
            result.Order_Quant = 0;
            result.ScoreW1 = master.ScoreW1;
            result.Scorew2 = master.Scorew2;
            result.Scorew3 = master.Scorew3;
            result.Scorew4 = master.Scorew4;
            result.Scorew5 = master.Scorew5;
            result.Scorew6 = master.Scorew6;
            result.Scorew7 = master.Scorew7;
            result.Scorew8 = master.Scorew8;
            result.Scorew9 = master.Scorew9;
            result.Scorew10 = master.Scorew10;
            result.Scorew11 = master.Scorew11;
            result.Scorew12 = master.Scorew12;
            result.Scorew13 = master.Scorew13;
            result.Scorew14 = master.Scorew14;
            result.Scorew15 = master.Scorew15;
            result.Scorew16 = master.Scorew16;
            result.ScoreL2 = master.ScoreL2;
            result.ScoreL3 = master.ScoreL3;
            result.ScoreL4 = master.ScoreL4;
            result.ScoreL5 = master.ScoreL5;
            result.ScoreL6 = master.ScoreL6;
            result.ScoreL7 = master.ScoreL7;
            result.ScoreL8 = master.ScoreL8;
            result.ScoreL9 = master.ScoreL9;
            result.Stations = stations;
            result.Palletization_Path = master.PalletizationPath;
            result.PalletPath_Base64 = string.IsNullOrEmpty(master.PalletizationPath) || string.IsNullOrWhiteSpace(master.PalletizationPath) ? "" : ConvertPictureToBase64._ConvertPictureToBase64(master.PalletizationPath);
            result.DiecutPict_Path = master.DiecutPictPath;
            result.DiecutPath_Base64 = string.IsNullOrEmpty(master.DiecutPictPath) || string.IsNullOrWhiteSpace(master.DiecutPictPath) ? "" : ConvertPictureToBase64._ConvertPictureToBase64(master.DiecutPictPath);
            result.Change = master.Change;
            result.Rout = new List<MasterCardRouting>();
            result.PartOfRout = new List<MasterCardRouting>();

            RoutingSetDetailOfMachineAndColor(ref masterCardRouting);
            RoutingSetDetailOfMachineAndColor(ref partOfMasterCardRouting);

            result.PpcRawMaterialProductionBoms = new List<PpcRawMaterialProductionBom>();
            var ppcRawMaterialProductionBoms = JsonConvert.DeserializeObject<List<PpcRawMaterialProductionBom>>(pPCRawMaterialProductionBomAPIRepository.GetPPCRawMaterialProductionBOMsByFgMaterial(_factoryCode, master.MaterialNo, _token));
            result.PpcRawMaterialProductionBoms.AddRange(ppcRawMaterialProductionBoms);

            result.Rout = masterCardRouting;
            result.PartOfRout = partOfMasterCardRouting;
            result.MoRout = new List<MasterCardMoRouting>();
            result.PartOfMoRout = new List<MasterCardMoRouting>();
            result.Target_Quant = null;
            result.CutNo = null;
            result.Leng = null;
            result.TwoPiece = master.TwoPiece.HasValue ? master.TwoPiece.Value : false;
            result.Slit = master.Slit == null ? 0 : master.Slit;
            result.StockQty = null;
            var boardAlt = basePrintMastercard.BoardAlternatives.Where(b => b.FactoryCode == _factoryCode && b.MaterialNo.Trim() == master.MaterialNo.Trim()).ToList();
            //var boardAlt = JsonConvert.DeserializeObject<List<BoardAlternative>>(_boardAlternativeAPIRepository.GetBoardAlternativeByMat(factoryCode, master.MaterialNo));
            result.BoardAlternative = boardAlt.Count > 0 ? boardAlt.Where(b => b.Priority == 1).FirstOrDefault().BoardName : "";
            result.FormGroup = formGroup.ToString().Trim();
            result.Hierarchy = master.Hierarchy;
            result.JointLap = master.JointLap == null ? 0 : master.JointLap;

            result.NoTagBundle = master.NoTagBundle;
            result.TagBundle = master.TagBundle;
            result.TagPallet = master.TagPallet;

            //tassanai
            if (transactionsDetail != null)
            {
                result.CGType = transactionsDetail.Cgtype == "B" ? "Base" :
                    transactionsDetail.Cgtype == "L" ? "L Shape" :
                    transactionsDetail.Cgtype == "U" ? "U Shape" : string.Empty;
                result.KindofProductGroup = basePrintMastercard.KindOfProductGroups.FirstOrDefault(k => k.Id == transactionsDetail.IdKindOfProductGroup.Value).Name;
                //result.KindofProductGroup = JsonConvert.DeserializeObject<KindOfProductGroup>(_kindOfProductGroupAPIRepository.GetKindOfProductGroupById(factoryCode, transactionsDetail.IdKindOfProductGroup.Value)).Name;
                result.KindofProduct = basePrintMastercard.KindOfProducts.FirstOrDefault(k => k.Id == transactionsDetail.IdKindOfProduct.Value).Name;
                //result.KindofProduct = JsonConvert.DeserializeObject<KindOfProduct>(_kindOfProductAPIRepository.GetKindOfProductById(factoryCode, transactionsDetail.IdKindOfProduct.Value)).Name;
                result.ProcessCostList = basePrintMastercard.ProcessCosts;
                //result.ProcessCostList = JsonConvert.DeserializeObject<List<ProcessCost>>(Convert.ToString(_processCostAPIRepository.GetProcessCostList(_factoryCode)));
                var ProcessCostList = result.ProcessCostList.Where(p => p.Id == transactionsDetail.IdProcessCost).FirstOrDefault();
                result.ProcessCost = ProcessCostList.Name;

                result.NewPrintPlate = transactionsDetail.NewPrintPlate;
                result.OldPrintPlate = transactionsDetail.OldPrintPlate;
                result.NewBlockDieCut = transactionsDetail.NewBlockDieCut;
                result.OldBlockDieCut = transactionsDetail.OldBlockDieCut;
                result.ExampleColor = transactionsDetail.ExampleColor;
                result.CoatingType = transactionsDetail.CoatingType;
                result.CoatingTypeDesc = transactionsDetail.CoatingTypeDesc;
                result.PaperHorizontal = transactionsDetail.PaperHorizontal.HasValue ? transactionsDetail.PaperHorizontal.Value : false;
                result.PaperVertical = transactionsDetail.PaperVertical.HasValue ? transactionsDetail.PaperVertical.Value : false;
                result.FluteHorizontal = transactionsDetail.FluteHorizontal.HasValue ? transactionsDetail.FluteHorizontal.Value : false;
                result.FluteVertical = transactionsDetail.FluteVertical.HasValue ? transactionsDetail.FluteVertical.Value : false;
            }

            //tassanai
            result.High_Value = master.HighValue;

            result.NoSlot = formGroup.ToString().Trim() == "AC" && master.NoSlot.HasValue ? master.NoSlot.Value : 0;

            #endregion Set masterCard

            return result;
        }

        public List<MoRouting> GetMoRouting(string _OrderItem)
        {
            List<MoRouting> result = new List<MoRouting>();
            result = (from N in JsonConvert.DeserializeObject<List<MoRouting>>(_moRoutingAPIRepository.GetMoRoutingList(_factoryCode, _token))
                      where N.OrderItem.Equals(_OrderItem)
                      select N).OrderBy(o => o.SeqNo).ToList();
            return result;
        }

        private string QualitySpecsFromModel(List<QualitySpec> qualitySpecs)
        {
            var qualitySpecStr = string.Empty;
            foreach (var qualitySpec in qualitySpecs)
            {
                qualitySpecStr = qualitySpecStr + $"{qualitySpec.Name} : {qualitySpec.Value} {qualitySpec.Unit}, ";
            }

            qualitySpecStr = !String.IsNullOrEmpty(qualitySpecStr) ? qualitySpecStr.Substring(0, qualitySpecStr.Length - 2) : null;

            return qualitySpecStr;
        }

        public List<Routing> GetRouting(string MaterialNo)//2
        {
            //List<Routing> result = new List<Routing>();
            //result = (from N in JsonConvert.DeserializeObject<List<Routing>>(_routingAPIRepository.GetRoutingList(_factoryCode))
            //          where N.MaterialNo.Equals(MaterialNo)
            //          select N).ToList();
            //return result;
            return JsonConvert.DeserializeObject<List<Routing>>(_routingAPIRepository.GetRoutingsByMaterialNo(_factoryCode, MaterialNo, _token)).OrderBy(o => o.SeqNo).ToList();
        }

        private Int16 ConvertInt16ToShort(int? Input)
        {
            return (Int16)(string.IsNullOrEmpty(Input.ToString()) ? 0 : Input);
        }

        private string ReMachineName(Routing routing, MoRouting moRouting)
        {
            var machineName = "";
            var partOfMachineName = "";

            var alternatives = new List<string>();

            if (routing != null)
            {
                alternatives = new List<string>{
                routing.Alternative1,
                routing.Alternative2,
                routing.Alternative3,
                routing.Alternative4,
                routing.Alternative5,
                routing.Alternative6,
                routing.Alternative7,
                routing.Alternative8};
            }
            else
            {
                alternatives = new List<string>{
                moRouting.Alternative1,
                moRouting.Alternative2,
                moRouting.Alternative3,
                moRouting.Alternative4,
                moRouting.Alternative5,
                moRouting.Alternative6,
                moRouting.Alternative7,
                moRouting.Alternative8};
            }

            foreach (var alternative in alternatives)
            {
                if (!string.IsNullOrEmpty(alternative) && !string.IsNullOrWhiteSpace(alternative))
                {
                    partOfMachineName = partOfMachineName + " /" + alternative;
                }
            }

            machineName = routing != null ? routing.Machine + partOfMachineName : moRouting.Machine + partOfMachineName;

            return machineName;
        }

        private string RemoveNumberFromString(string str)
        {
            return System.Text.RegularExpressions.Regex.Replace(str, @"[\d-]", string.Empty);
        }

        private string SplitCharsAndNums(string text)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < text.Length - 1; i++)
            {
                if ((char.IsLetter(text[i]) && char.IsDigit(text[i + 1])) ||
                    (char.IsDigit(text[i]) && char.IsLetter(text[i + 1])))
                {
                    sb.Append(text[i]);
                    sb.Append(" ");
                }
                else
                {
                    sb.Append(text[i]);
                }
            }

            sb.Append(text[text.Length - 1]);

            return sb.ToString();
        }

        public List<MasterDataRoutingModel> SearchMasterCardMOByKeySearch(string startSO, string endSO)
        {
            var masterCardMoPrint = JsonConvert.DeserializeObject<List<MasterDataRoutingModel>>(_moDataAPIRepository.SearchMoDataListBySaleOrderNonXAndH(_factoryCode, startSO, endSO, _token)).ToList();
            return masterCardMoPrint;
        }

        public void SearchMasterCardMOByKeySearch(ref List<MasterDataRoutingModel> masterDataRoutings, string startSO, string endSO)
        {
            if (string.IsNullOrEmpty(endSO))
            {
                endSO = startSO;
            }

            if (!string.IsNullOrEmpty(startSO))
            {
                var moDatas = JsonConvert.DeserializeObject<List<MoData>>(_moDataAPIRepository.GetMoDataListBySaleOrderNonXAndH(_factoryCode, startSO, endSO, _token)).OrderBy(m => m.OrderItem).ToList();

                foreach (var moData in moDatas)
                {
                    var moSpec = JsonConvert.DeserializeObject<MoSpec>(_moSpecAPIRepository.GetMoSpecBySaleOrder(_factoryCode, moData.OrderItem, _token));
                    if (moSpec != null)
                    {
                        var moRoutings = JsonConvert.DeserializeObject<List<MoRouting>>(_moRoutingAPIRepository.GetMORoutingsBySaleOrder(_factoryCode, moData.OrderItem, _token));
                        var isSheetBoard = moSpec.MaterialNo.ToLower().Contains("s/b");
                        var masterData = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNumberNonX(_factoryCode, moData.MaterialNo, _token));
                        var isAvailableMasterData = MasterDataStatus.NotAvailable;
                        if (masterData != null && masterData.PdisStatus != "X")
                        {
                            isAvailableMasterData = MasterDataStatus.Available;
                        }
                        else
                        {
                            if (moSpec.Hierarchy.Substring(2, 2).ToUpper().Contains("SB"))
                            {
                                isAvailableMasterData = MasterDataStatus.AvailableSB;
                            }
                        }

                        var machineRouting = "";

                        foreach (var moRouting in moRoutings)
                        {
                            machineRouting = machineRouting + moRouting.Machine + ", ";
                        }

                        if (!string.IsNullOrEmpty(machineRouting))
                        {
                            machineRouting = machineRouting.Substring(0, machineRouting.Length - 2);
                        }
                        bool TagBunbleStatus = false;
                        bool TagPalletStatus = false;
                        if (!String.IsNullOrEmpty(moSpec.TagBundle))
                        {
                            TagBunbleStatus = true;
                        }
                        if (!String.IsNullOrEmpty(moSpec.TagPallet))
                        {
                            TagPalletStatus = true;
                        }
                        var boardCombine = JsonConvert.DeserializeObject<BoardCombine>(_boardCombineAPIRepository.GetBoardByCode(_factoryCode, moSpec.Code, _token));
                        masterDataRoutings.Add(new MasterDataRoutingModel
                        {
                            SaleOrder = moData.OrderItem,
                            Board = moSpec.Board,
                            BoxType = moSpec.BoxType,
                            CustCode = moSpec.CustCode,
                            CustID = moSpec.CusId,
                            CustName = moSpec.CustName,
                            Description = moSpec.Description,
                            LastUpdate = moSpec.LastUpdate,
                            Machine = machineRouting,
                            MaterialNo = moData.MaterialNo,
                            PC = moSpec.Pc,
                            OrderQuant = moData.OrderQuant,
                            Batch = moData.Batch,
                            DueDate = moData.DueText,
                            MasterDataStatus = isAvailableMasterData,
                            Printed = moData.Printed.HasValue ? moData.Printed.Value : 0,
                            TagBunbleStatus = TagBunbleStatus,
                            TagPalletStatus = TagPalletStatus,
                            MoStatus = moData.MoStatus,
                            Code = boardCombine != null ? boardCombine.Code : null
                        });
                    }
                }
            }
        }

        private int MORoutingLineCount(MoRouting moRouting)
        {
            int lineCount = _businessGroup ? 6 : 4;

            if (!_businessGroup)
            {
                lineCount = (!String.IsNullOrEmpty(moRouting.JoinMatNo) || !String.IsNullOrEmpty(moRouting.SeparatMatNo)) ? lineCount + 1 : lineCount;
                lineCount = (!String.IsNullOrEmpty(moRouting.ScoreType)) ? lineCount + 1 : lineCount;
                lineCount = (!String.IsNullOrEmpty(moRouting.MylaNo)) ? lineCount + 1 : lineCount;
                lineCount = moRouting.RepeatLength.HasValue && moRouting.RepeatLength.Value != 0 ? lineCount + 1 : lineCount;
            }
            lineCount = moRouting.Machine.Contains("COR") ? lineCount + 1 : lineCount;
            lineCount = (!String.IsNullOrEmpty(moRouting.PlateNo) || !String.IsNullOrEmpty(moRouting.BlockNo) || !String.IsNullOrEmpty(moRouting.MylaNo)) ? lineCount + 1 : lineCount;
            lineCount = (!String.IsNullOrEmpty(moRouting.Color1) || !String.IsNullOrEmpty(moRouting.Color2) || !String.IsNullOrEmpty(moRouting.Color3)) ? lineCount + 1 : lineCount;
            lineCount = (!String.IsNullOrEmpty(moRouting.Color4) || !String.IsNullOrEmpty(moRouting.Color5) || !String.IsNullOrEmpty(moRouting.Color6)) ? lineCount + 1 : lineCount;
            lineCount = (!String.IsNullOrEmpty(moRouting.Color7)) ? lineCount + 1 : lineCount;

            var lineOfRemarkInprocess = 0;
            if (!String.IsNullOrEmpty(moRouting.RemarkInprocess))
            {
                lineOfRemarkInprocess = 1;
                if (Regex.Matches(moRouting.RemarkInprocess, @"\r\n").Count > 0)
                {
                    lineOfRemarkInprocess = Regex.Matches(moRouting.RemarkInprocess, @"\r\n").Count + 1;

                    var remarkArr = moRouting.RemarkInprocess.Split("\r\n");
                    foreach (var remark in remarkArr)
                    {
                        if (remark.Length > 55)
                        {
                            lineOfRemarkInprocess += remark.Length / 55 > 0 ? (remark.Length / 55) + 1 : 1;
                        }
                    }
                }
                else
                {
                    if (moRouting.RemarkInprocess.Length > 35)
                    {
                        lineOfRemarkInprocess = moRouting.RemarkInprocess.Length / 35 > 0 ?
                            moRouting.RemarkInprocess.Length % 35 > 0 ? (moRouting.RemarkInprocess.Length / 35) + 1 : (moRouting.RemarkInprocess.Length / 35)
                            : 1;
                    }
                }
            }

            lineCount = lineCount + lineOfRemarkInprocess;

            return lineCount;
        }

        private int RoutingLineCount(Routing routing)
        {
            int lineCount = _businessGroup ? 6 : 4;

            if (!_businessGroup)
            {
                lineCount = (!String.IsNullOrEmpty(routing.JoinMatNo) || !String.IsNullOrEmpty(routing.SeparatMatNo)) ? lineCount + 1 : lineCount;
                lineCount = (!String.IsNullOrEmpty(routing.ScoreType)) ? lineCount + 1 : lineCount;
                lineCount = (!String.IsNullOrEmpty(routing.MylaNo)) ? lineCount + 1 : lineCount;
                lineCount = routing.RepeatLength.HasValue && routing.RepeatLength.Value != 0 ? lineCount + 1 : lineCount;
                lineCount = (!String.IsNullOrEmpty(routing.MylaSize)) ? lineCount + 1 : lineCount;
            }

            lineCount = routing.Machine.Contains("COR") ? lineCount + 1 : lineCount;
            lineCount = routing.Machine.Length > 50 ? lineCount + 1 : lineCount;
            lineCount = (!String.IsNullOrEmpty(routing.PlateNo) || !String.IsNullOrEmpty(routing.BlockNo) || !String.IsNullOrEmpty(routing.MylaNo)) ? lineCount + 1 : lineCount;
            lineCount = (!String.IsNullOrEmpty(routing.Color1) || !String.IsNullOrEmpty(routing.Color2) || !String.IsNullOrEmpty(routing.Color3)) ? lineCount + 1 : lineCount;
            lineCount = (!String.IsNullOrEmpty(routing.Color4) || !String.IsNullOrEmpty(routing.Color5) || !String.IsNullOrEmpty(routing.Color6)) ? lineCount + 1 : lineCount;
            lineCount = (!String.IsNullOrEmpty(routing.Color7)) ? lineCount + 1 : lineCount;

            var lineOfRemarkInprocess = 0;
            if (!String.IsNullOrEmpty(routing.RemarkInprocess))
            {
                lineOfRemarkInprocess = 1;
                if (Regex.Matches(routing.RemarkInprocess, @"\r\n").Count > 0)
                {
                    lineOfRemarkInprocess = Regex.Matches(routing.RemarkInprocess, @"\r\n").Count + 1;

                    var remarkArr = routing.RemarkInprocess.Split("\r\n");
                    foreach (var remark in remarkArr)
                    {
                        if (remark.Length > 55)
                        {
                            lineOfRemarkInprocess += remark.Length / 55 > 0 ? (remark.Length / 55) + 1 : 1;
                        }
                    }
                }
                else
                {
                    if (routing.RemarkInprocess.Length > 35)
                    {
                        lineOfRemarkInprocess = routing.RemarkInprocess.Length / 35 > 0 ?
                            routing.RemarkInprocess.Length % 35 > 0 ? (routing.RemarkInprocess.Length / 35) + 1 : (routing.RemarkInprocess.Length / 35)
                            : 1;
                    }
                }
            }

            lineCount = lineCount + lineOfRemarkInprocess;

            return lineCount;
        }

        private List<Coating> GetCoatings(string materialNo)
        {
            var coatings = new List<Coating>();
            var coatings_api = JsonConvert.DeserializeObject<List<Coating>>(_coatingAPIRepository.GetCoatingByMaterialNo(_factoryCode, materialNo, _token)).OrderBy(c => c.Id).ToList();

            foreach (var coating in coatings_api)
            {
                var itemCoating = new Coating
                {
                    Station = coating.Station,
                    Type = coating.Type,
                    Name = coating.Name,
                    Id = coating.Id
                };

                if (!String.IsNullOrEmpty(itemCoating.Name) && !String.IsNullOrWhiteSpace(itemCoating.Name))
                {
                    coatings.Add(itemCoating);
                }
            }

            return coatings.OrderBy(c => c.Id).ToList();
        }

        public bool UpdatePublicPrinted(string orderItem)
        {
            var moDatas = new List<MoData>();
            var i = 0;

            moDatas.Clear();
            moDatas = JsonConvert.DeserializeObject<List<MoData>>(_moDataAPIRepository.GetMoDataListBySaleOrderNonXAndH(_factoryCode, orderItem, orderItem, _token));

            foreach (var moData in moDatas)
            {
                i++;
                var printed = moData.Printed == null ? 1 : moData.Printed + 1;
                moData.Printed = printed;

                _moDataAPIRepository.UpdateMoData(JsonConvert.SerializeObject(moData), _token);
                var logPrintMO = new LogPrintMo
                {
                    Id = 0,
                    FactoryCode = _factoryCode,
                    OrderItem = orderItem,
                    Printed = printed,
                    PrintedBy = _username,
                    PrintedDate = DateTime.Now,
                };

                _moDataAPIRepository.UpdateLogPrintMO(JsonConvert.SerializeObject(logPrintMO), _token);

                if (i == moDatas.Count)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<PrintMastercardViewModel> SetMasterCardMOFromFile(List<IFormFile> fileUpload)
        {
            var PrintMastercardViewModel = new PrintMastercardViewModel();
            PrintMastercardViewModel.MasterDataRoutingModels = new List<MasterDataRoutingModel>();
            PrintMastercardViewModel.ErrorSearchOrderItems = string.Empty;
            var masterDataRoutings = new List<MasterDataRoutingModel>();

            List<MasterCardMO> list = new List<MasterCardMO>();
            MasterCardMO Mo = new MasterCardMO();
            var saleOrders = new List<string>();
            var result = new StringBuilder();

            foreach (var file in fileUpload)
            {
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    while (reader.Peek() >= 0)
                    {
                        var data = await reader.ReadLineAsync();
                        result.Append(data);
                        saleOrders.Add(result.ToString().Trim());

                        result.Clear();
                    }
                }
            }

            masterDataRoutings = JsonConvert.DeserializeObject<List<MasterDataRoutingModel>>(_moDataAPIRepository.GetMasterCardMOsBySaleOrders(_factoryCode, JsonConvert.SerializeObject(saleOrders), _token)).ToList();
            PrintMastercardViewModel.MasterDataRoutingModels = masterDataRoutings.OrderBy(m => saleOrders.FindIndex(s => s == m.SaleOrder)).ToList();
            return PrintMastercardViewModel;

            #region Old get mastercard

            //var moDatas = JsonConvert.DeserializeObject<List<MoData>>(_moDataAPIRepository.GetMoDataListBySaleOrders(_factoryCode, JsonConvert.SerializeObject(saleOrders), _token)).ToList();

            //foreach (var moData in moDatas)
            //{
            //    var orderItemTemp = moData.OrderItem;
            //    try
            //    {
            //        var moSpec = JsonConvert.DeserializeObject<MoSpec>(_moSpecAPIRepository.GetMoSpecBySaleOrder(_factoryCode, moData.OrderItem, _token));
            //        var moRoutings = JsonConvert.DeserializeObject<List<MoRouting>>(_moRoutingAPIRepository.GetMORoutingsBySaleOrder(_factoryCode, moData.OrderItem, _token));
            //        var isSheetBoard = moSpec.MaterialNo.ToLower().Contains("s/b");
            //        var machineRouting = "";
            //        var masterData = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNumberNonX(_factoryCode, moData.MaterialNo, _token));
            //        var isAvailableMasterData = MasterDataStatus.NotAvailable;
            //        if (masterData != null)
            //        {
            //            isAvailableMasterData = MasterDataStatus.Available;
            //        }
            //        else
            //        {
            //            if (moSpec.Hierarchy.Substring(2, 2).ToUpper().Contains("SB"))
            //            {
            //                isAvailableMasterData = MasterDataStatus.AvailableSB;
            //            }
            //        }

            //        foreach (var moRouting in moRoutings)
            //        {
            //            machineRouting = machineRouting + moRouting.Machine + ", ";
            //        }

            //        if (!string.IsNullOrEmpty(machineRouting))
            //        {
            //            machineRouting = machineRouting.Substring(0, machineRouting.Length - 2);
            //        }

            //        if (masterDataRoutings.FirstOrDefault(m => m.SaleOrder == moData.OrderItem) == null)
            //        {
            //            bool TagBunbleStatus = false;
            //            bool TagPalletStatus = false;
            //            if (!String.IsNullOrEmpty(moSpec.TagBundle))
            //            {
            //                TagBunbleStatus = true;
            //            }
            //            if (!String.IsNullOrEmpty(moSpec.TagPallet))
            //            {
            //                TagPalletStatus = true;
            //            }

            //            masterDataRoutings.Add(new MasterDataRoutingModel
            //            {
            //                SaleOrder = moData.OrderItem,
            //                Board = moSpec.Board,
            //                BoxType = moSpec.BoxType,
            //                CustCode = moSpec.CustCode,
            //                CustID = moSpec.CusId,
            //                CustName = moSpec.CustName,
            //                Description = moSpec.Description,
            //                LastUpdate = moSpec.LastUpdate,
            //                Machine = machineRouting,
            //                MaterialNo = moData.MaterialNo,
            //                PC = moSpec.Pc,
            //                OrderQuant = moData.OrderQuant,
            //                Batch = moData.Batch,
            //                DueDate = moData.DueText,
            //                MasterDataStatus = isAvailableMasterData,
            //                Printed = moData.Printed.HasValue ? moData.Printed.Value : 0,
            //                TagBunbleStatus = TagBunbleStatus,
            //                TagPalletStatus = TagPalletStatus
            //            });
            //        }

            //    }
            //    catch (Exception ex)
            //    {
            //        PrintMastercardViewModel.ErrorSearchOrderItems = PrintMastercardViewModel.ErrorSearchOrderItems + orderItemTemp + ", ";
            //        continue;
            //    }
            //}

            //PrintMastercardViewModel.MasterDataRoutingModels = masterDataRoutings;

            //return PrintMastercardViewModel;

            #endregion Old get mastercard
        }

        public int CellingFloat(string value)
        {
            int tolerance = 0;
            double doubleValue = Double.Parse(value);
            tolerance = (Int32)Math.Ceiling(doubleValue);

            return tolerance;
        }

        public void MoRoutingSetDetailOfMachineAndColor(ref List<MasterCardMoRouting> masterCardMoRoutings)
        {
            var moRoutings = new List<MoRouting>();
            for (int i = 0; i < masterCardMoRoutings.Count; i++)
            {
                var masterCardRouting = masterCardMoRoutings[i];
                var routingMachine = string.Empty;
                var routingColor = string.Empty;
                var machine = JsonConvert.DeserializeObject<DataAccess.Models.Machine>(_machineAPIRepository.GetMachineGroupByPlanCode(_factoryCode, masterCardRouting.Routing.PlanCode, _token));

                //machine
                if (masterCardRouting.Routing.PlateNo != null && masterCardRouting.Routing.PlateNo != "")
                {
                    routingMachine = routingMachine + "Plate No. " + masterCardRouting.Routing.PlateNo + ", ";
                }

                var blockBlk = "";
                if (!string.IsNullOrEmpty(masterCardRouting.Routing.BlockNo))
                {
                    if (masterCardRouting.Routing.NoneBlk.HasValue && masterCardRouting.Routing.NoneBlk.Value)
                    {
                        blockBlk = "Non print" + ", ";
                    }
                    else if (masterCardRouting.Routing.StanBlk.HasValue && masterCardRouting.Routing.StanBlk.Value)
                    {
                        blockBlk = "Block Std. No. " + masterCardRouting.Routing.BlockNo + ", ";
                    }
                    else if (masterCardRouting.Routing.SemiBlk.HasValue && masterCardRouting.Routing.SemiBlk.Value)
                    {
                        blockBlk = "Block Semi. No. " + masterCardRouting.Routing.BlockNo + ", ";
                    }
                    else if (masterCardRouting.Routing.ShipBlk.HasValue && masterCardRouting.Routing.ShipBlk.Value)
                    {
                        blockBlk = "Block Ship. No. " + masterCardRouting.Routing.BlockNo + ", ";
                    }
                    routingMachine = routingMachine + blockBlk;
                    //routingMachine = routingMachine + "Block Ship. No. " + masterCardRouting.Routing.BlockNo + ", ";
                }
                else
                {
                    if (masterCardRouting.Routing.NoneBlk.HasValue && masterCardRouting.Routing.NoneBlk.Value)
                    {
                        blockBlk = "Non print , ";
                    }
                    else if (masterCardRouting.Routing.StanBlk.HasValue && masterCardRouting.Routing.StanBlk.Value)
                    {
                        blockBlk = "Block Std. No. , ";
                    }
                    else if (masterCardRouting.Routing.SemiBlk.HasValue && masterCardRouting.Routing.SemiBlk.Value)
                    {
                        blockBlk = "Block Semi. No. , ";
                    }
                    else if (masterCardRouting.Routing.ShipBlk.HasValue && masterCardRouting.Routing.ShipBlk.Value)
                    {
                        blockBlk = "Block Ship. No. , ";
                    }
                    routingMachine = routingMachine + blockBlk;
                }

                if (machine is not null && !string.IsNullOrEmpty(masterCardRouting.Routing.MylaNo) && machine.IsPropPrint.Value)
                {
                    routingMachine = routingMachine + "Myla No. " + masterCardRouting.Routing.MylaNo + ", ";
                }

                if (!string.IsNullOrEmpty(masterCardRouting.Routing.MylaSize))
                {
                    routingMachine = routingMachine + "Myla Size " + masterCardRouting.Routing.MylaSize + ", ";
                }

                var lengthOfMachine = routingMachine.Length;
                if (lengthOfMachine != 0)
                {
                    routingMachine = routingMachine.Substring(0, lengthOfMachine - 2);
                }

                //color
                if (!string.IsNullOrEmpty(masterCardRouting.Routing.Color1) && !string.IsNullOrEmpty(masterCardRouting.Routing.Shade1))
                {
                    routingColor = routingColor + masterCardRouting.Routing.Color1 + " " + masterCardRouting.Routing.Shade1 + ", ";
                }
                if (!string.IsNullOrEmpty(masterCardRouting.Routing.Color2) && !string.IsNullOrEmpty(masterCardRouting.Routing.Shade2))
                {
                    routingColor = routingColor + masterCardRouting.Routing.Color2 + " " + masterCardRouting.Routing.Shade2 + ", ";
                }
                if (!string.IsNullOrEmpty(masterCardRouting.Routing.Color3) && !string.IsNullOrEmpty(masterCardRouting.Routing.Shade3))
                {
                    routingColor = routingColor + masterCardRouting.Routing.Color3 + " " + masterCardRouting.Routing.Shade3 + ", ";
                }
                if (!string.IsNullOrEmpty(masterCardRouting.Routing.Color4) && !string.IsNullOrEmpty(masterCardRouting.Routing.Shade4))
                {
                    routingColor = routingColor + masterCardRouting.Routing.Color4 + " " + masterCardRouting.Routing.Shade4 + ", ";
                }
                if (!string.IsNullOrEmpty(masterCardRouting.Routing.Color5) && !string.IsNullOrEmpty(masterCardRouting.Routing.Shade5))
                {
                    routingColor = routingColor + masterCardRouting.Routing.Color5 + " " + masterCardRouting.Routing.Shade5 + ", ";
                }
                if (!string.IsNullOrEmpty(masterCardRouting.Routing.Color6) && !string.IsNullOrEmpty(masterCardRouting.Routing.Shade6))
                {
                    routingColor = routingColor + masterCardRouting.Routing.Color6 + " " + masterCardRouting.Routing.Shade6 + ", ";
                }
                if (!string.IsNullOrEmpty(masterCardRouting.Routing.Color7) && !string.IsNullOrEmpty(masterCardRouting.Routing.Shade7))
                {
                    routingColor = routingColor + masterCardRouting.Routing.Color7 + " " + masterCardRouting.Routing.Shade7 + ", ";
                }
                if (!string.IsNullOrEmpty(masterCardRouting.Routing.Color8) && !string.IsNullOrEmpty(masterCardRouting.Routing.Shade8))
                {
                    routingColor = routingColor + masterCardRouting.Routing.Color8 + " " + masterCardRouting.Routing.Shade8 + ", ";
                }
                if (!string.IsNullOrEmpty(masterCardRouting.Routing.Color9) && !string.IsNullOrEmpty(masterCardRouting.Routing.Shade9))
                {
                    routingColor = routingColor + masterCardRouting.Routing.Color9 + " " + masterCardRouting.Routing.Shade9 + ", ";
                }
                if (!string.IsNullOrEmpty(masterCardRouting.Routing.Color10) && !string.IsNullOrEmpty(masterCardRouting.Routing.Shade10))
                {
                    routingColor = routingColor + masterCardRouting.Routing.Color10 + " " + masterCardRouting.Routing.Shade10 + ", ";
                }

                var lengthOfColor = routingColor.Length;
                if (lengthOfColor != 0)
                {
                    routingColor = routingColor.Substring(0, lengthOfColor - 2);
                }

                if (!string.IsNullOrEmpty(routingMachine))
                {
                    masterCardRouting.Routing.MachineDetail = routingMachine;
                }

                if (!string.IsNullOrEmpty(routingColor))
                {
                    masterCardRouting.Routing.MachineColorDetail = "Color : " + routingColor;
                }

                masterCardMoRoutings[i].Routing = masterCardRouting.Routing;
            }
        }

        public void RoutingSetDetailOfMachineAndColor(ref List<MasterCardRouting> masterCardRoutings)
        {
            var moRoutings = new List<MoRouting>();
            for (int i = 0; i < masterCardRoutings.Count; i++)
            {
                var masterCardRouting = masterCardRoutings[i];
                var routingMachine = string.Empty;
                var routingColor = string.Empty;
                var machine = JsonConvert.DeserializeObject<DataAccess.Models.Machine>(_machineAPIRepository.GetMachineGroupByPlanCode(_factoryCode, masterCardRouting.Routing.PlanCode, _token));

                //machine
                if (masterCardRouting.Routing.PlateNo != null && masterCardRouting.Routing.PlateNo != "")
                {
                    routingMachine = routingMachine + "Plate No. " + masterCardRouting.Routing.PlateNo + ", ";
                }

                var blockBlk = "";
                if (!string.IsNullOrEmpty(masterCardRouting.Routing.BlockNo))
                {
                    if (masterCardRouting.Routing.NoneBlk.HasValue && masterCardRouting.Routing.NoneBlk.Value)
                    {
                        blockBlk = "Non print" + ", ";
                    }
                    else if (masterCardRouting.Routing.StanBlk.HasValue && masterCardRouting.Routing.StanBlk.Value)
                    {
                        blockBlk = "Block Std. No. " + masterCardRouting.Routing.BlockNo + ", ";
                    }
                    else if (masterCardRouting.Routing.SemiBlk.HasValue && masterCardRouting.Routing.SemiBlk.Value)
                    {
                        blockBlk = "Block Semi. No. " + masterCardRouting.Routing.BlockNo + ", ";
                    }
                    else if (masterCardRouting.Routing.ShipBlk.HasValue && masterCardRouting.Routing.ShipBlk.Value)
                    {
                        blockBlk = "Block Ship. No. " + masterCardRouting.Routing.BlockNo + ", ";
                    }
                    routingMachine = routingMachine + blockBlk;
                    //routingMachine = routingMachine + "Block Ship. No. " + masterCardRouting.Routing.BlockNo + ", ";
                }
                else
                {
                    if (masterCardRouting.Routing.NoneBlk.HasValue && masterCardRouting.Routing.NoneBlk.Value)
                    {
                        blockBlk = "Non print , ";
                    }
                    else if (masterCardRouting.Routing.StanBlk.HasValue && masterCardRouting.Routing.StanBlk.Value)
                    {
                        blockBlk = "Block Std. No. , ";
                    }
                    else if (masterCardRouting.Routing.SemiBlk.HasValue && masterCardRouting.Routing.SemiBlk.Value)
                    {
                        blockBlk = "Block Semi. No. , ";
                    }
                    else if (masterCardRouting.Routing.ShipBlk.HasValue && masterCardRouting.Routing.ShipBlk.Value)
                    {
                        blockBlk = "Block Ship. No. , ";
                    }
                    routingMachine = routingMachine + blockBlk;
                }

                if (!string.IsNullOrEmpty(masterCardRouting.Routing.MylaNo) && machine.IsPropPrint.Value)
                {
                    routingMachine = routingMachine + "Myla No. " + masterCardRouting.Routing.MylaNo + ", ";
                }

                if (!string.IsNullOrEmpty(masterCardRouting.Routing.MylaSize))
                {
                    routingMachine = routingMachine + "Myla Size " + masterCardRouting.Routing.MylaSize + ", ";
                }

                var lengthOfMachine = routingMachine.Length;
                if (lengthOfMachine != 0)
                {
                    routingMachine = routingMachine.Substring(0, lengthOfMachine - 2);
                }

                //color
                if (!string.IsNullOrEmpty(masterCardRouting.Routing.Color1) && !string.IsNullOrEmpty(masterCardRouting.Routing.Shade1))
                {
                    routingColor = routingColor + masterCardRouting.Routing.Color1 + " " + masterCardRouting.Routing.Shade1 + ", ";
                }
                if (!string.IsNullOrEmpty(masterCardRouting.Routing.Color2) && !string.IsNullOrEmpty(masterCardRouting.Routing.Shade2))
                {
                    routingColor = routingColor + masterCardRouting.Routing.Color2 + " " + masterCardRouting.Routing.Shade2 + ", ";
                }
                if (!string.IsNullOrEmpty(masterCardRouting.Routing.Color3) && !string.IsNullOrEmpty(masterCardRouting.Routing.Shade3))
                {
                    routingColor = routingColor + masterCardRouting.Routing.Color3 + " " + masterCardRouting.Routing.Shade3 + ", ";
                }
                if (!string.IsNullOrEmpty(masterCardRouting.Routing.Color4) && !string.IsNullOrEmpty(masterCardRouting.Routing.Shade4))
                {
                    routingColor = routingColor + masterCardRouting.Routing.Color4 + " " + masterCardRouting.Routing.Shade4 + ", ";
                }
                if (!string.IsNullOrEmpty(masterCardRouting.Routing.Color5) && !string.IsNullOrEmpty(masterCardRouting.Routing.Shade5))
                {
                    routingColor = routingColor + masterCardRouting.Routing.Color5 + " " + masterCardRouting.Routing.Shade5 + ", ";
                }
                if (!string.IsNullOrEmpty(masterCardRouting.Routing.Color6) && !string.IsNullOrEmpty(masterCardRouting.Routing.Shade6))
                {
                    routingColor = routingColor + masterCardRouting.Routing.Color6 + " " + masterCardRouting.Routing.Shade6 + ", ";
                }
                if (!string.IsNullOrEmpty(masterCardRouting.Routing.Color7) && !string.IsNullOrEmpty(masterCardRouting.Routing.Shade7))
                {
                    routingColor = routingColor + masterCardRouting.Routing.Color7 + " " + masterCardRouting.Routing.Shade7 + ", ";
                }
                if (!string.IsNullOrEmpty(masterCardRouting.Routing.Color8) && !string.IsNullOrEmpty(masterCardRouting.Routing.Shade8))
                {
                    routingColor = routingColor + masterCardRouting.Routing.Color8 + " " + masterCardRouting.Routing.Shade8 + ", ";
                }
                if (!string.IsNullOrEmpty(masterCardRouting.Routing.Color9) && !string.IsNullOrEmpty(masterCardRouting.Routing.Shade9))
                {
                    routingColor = routingColor + masterCardRouting.Routing.Color9 + " " + masterCardRouting.Routing.Shade9 + ", ";
                }
                if (!string.IsNullOrEmpty(masterCardRouting.Routing.Color10) && !string.IsNullOrEmpty(masterCardRouting.Routing.Shade10))
                {
                    routingColor = routingColor + masterCardRouting.Routing.Color10 + " " + masterCardRouting.Routing.Shade10 + ", ";
                }

                var lengthOfColor = routingColor.Length;
                if (lengthOfColor != 0)
                {
                    routingColor = routingColor.Substring(0, lengthOfColor - 2);
                }

                if (!string.IsNullOrEmpty(routingMachine))
                {
                    masterCardRouting.Routing.MachineDetail = routingMachine;
                }

                if (!string.IsNullOrEmpty(routingColor))
                {
                    masterCardRouting.Routing.MachineColorDetail = "Color : " + routingColor;
                }

                masterCardRoutings[i].Routing = masterCardRouting.Routing;
            }
        }

        public MasterCardMO GetMasterCardProductCatalog(string MaterialNo, string Factorycode)
        {
            string _factoryCode = Factorycode;
            //MasterData
            MasterData master = new MasterData();
            master = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNo(_factoryCode, MaterialNo, _token));

            //MO_ROUTING >> First
            // var xc = JsonConvert.DeserializeObject<Routing>(_routingAPIRepository.GetRoutingByMaterialNo(_factoryCode, MaterialNo));
            //Routing T3 = new Routing();
            //T3 = JsonConvert.DeserializeObject<List<Routing>>(_routingAPIRepository.GetRoutingsByMaterialNo(_factoryCode, MaterialNo)).FirstOrDefault(x => x.SeqNo == 1);

            var hierarchyLv2 = master.Hierarchy.Substring(2, 2);
            var productTypeByHieLv2 = JsonConvert.DeserializeObject<ProductType>(_productTypeAPIRepository.GetFormGroupByHierarchyLv2(_factoryCode, hierarchyLv2, _token));
            var formGroup = productTypeByHieLv2.FormGroup;

            var boardCombine = master.Code != null ? JsonConvert.DeserializeObject<BoardCombine>(_boardCombineAPIRepository.GetBoardByCode(_factoryCode, master.Code, _token)) : null;
            var tempStations = new List<Station>();
            var stations = new List<Station>();
            int firstMachine = 0;

            if (boardCombine != null)
            {
                if (boardCombine.StandardBoard.Value)
                {
                    var boardUse = JsonConvert.DeserializeObject<BoardUse>(_boardUseAPIRepository.GetBoardUseByMaterialNo(_factoryCode, master.MaterialNo, _token));
                    var boardName = boardUse == null ? "" : boardUse.BoardName;
                    var boardSplit = boardName == "" ? new List<string> { } : boardName.Split('/').ToList();

                    var fluteTrs = JsonConvert.DeserializeObject<List<FluteTr>>(_fluteTrAPIRepository.GetFluteTrByFlute(_factoryCode, boardCombine.Flute, _token)).OrderBy(f => f.Item).ToList();

                    stations.Clear();
                    var boardNo = 0;
                    foreach (var fluteTr in fluteTrs)
                    {
                        var item = fluteTr.Item.Value;
                        var paperGrade = "";
                        if (fluteTr.FluteCode[0].ToString().ToLower() == "o" && fluteTr.Station.ToString().ToLower().Equals("top"))
                        {
                            item = 999;
                        }

                        if (boardNo >= boardSplit.Count)
                        {
                            paperGrade = "";
                        }
                        else
                        {
                            paperGrade = boardSplit[boardNo];
                        }

                        stations.Add(new Station
                        {
                            item = item,
                            TypeOfStation = fluteTr.Station,
                            PaperGrade = paperGrade,
                            Flute = fluteTr.FluteCode
                        });
                        boardNo++;
                    }
                }
                else
                {
                    stations.Clear();
                    var boardSpecs = JsonConvert.DeserializeObject<List<BoardSpec>>(_boardSpecAPIRepository.GetBoardSpecByCode(_factoryCode, boardCombine.Code, _token));
                    foreach (var boardSpec in boardSpecs)
                    {
                        stations.Add(new Station
                        {
                            item = boardSpec.Item.Value,
                            TypeOfStation = boardSpec.Station,
                            PaperGrade = boardSpec.Grade,
                            Flute = null
                        });
                    }
                }
            }

            //TblPmtsConfig
            List<PmtsConfig> pmts = new List<PmtsConfig>();
            pmts = JsonConvert.DeserializeObject<List<PmtsConfig>>(_pMTsConfigAPIRepository.GetPMTsConfigList(_factoryCode, _token));
            string myFactory = "", ISO_DocDate = "", ISO_DocName = "";
            foreach (var x in pmts)
            {
                switch (x.FucName)
                {
                    case "Import_PrefixName":
                        myFactory = x.FucValue;
                        //myFactory = "aaaa";
                        break;

                    case "ISO_DocDate":
                        ISO_DocDate = x.FucValue;
                        break;

                    case "ISO_DocName":
                        ISO_DocName = x.FucValue;
                        break;
                }
            }

            var lineCount = 0;
            var routings = GetRoutingProductCatalogs(MaterialNo, _factoryCode);
            var masterCardRouting = new List<MasterCardRouting>();
            var partOfMasterCardRouting = new List<MasterCardRouting>();
            var coatings = new List<Coating>();
            coatings = GetCoatingsProductCatalogs(MaterialNo, _factoryCode);

            foreach (var routing in routings)
            {
                var machineName = "";
                var machine = JsonConvert.DeserializeObject<DataAccess.Models.Machine>(_machineAPIRepository.GetMachineGroupByPlanCode(_factoryCode, routing.PlanCode, _token));
                var machineGroup = routing.Machine != null && machine != null ? machine.MachineGroup : null;
                var machineIsCalPaperwidth = routing.Machine != null && machine != null && machine.IsCalPaperwidth.HasValue && machine.IsCalPaperwidth.Value;
                if (routing.McMove.Value)
                {
                    machineName = ReMachineName(routing, null);
                }
                else
                {
                    machineName = routing.Machine + " (ห้ามย้ายเครื่อง)";
                }

                routing.Machine = machineName;
                routing.ScoreType = !string.IsNullOrEmpty(routing.ScoreType) && !string.IsNullOrWhiteSpace(routing.ScoreType) ? JsonConvert.DeserializeObject<ScoreType>(_scoreTypeAPIRepository.GetScoreTypeByScoreTypeId(_factoryCode, routing.ScoreType, _token)).ScoreTypeName : routing.ScoreType;

                lineCount = lineCount + RoutingLineCount(routing);

                var cloneRouting = mapper.Map<CloneRouting>(routing);
                cloneRouting.Coatings = new List<Coating>();

                if (routing.Machine.Contains("COA"))
                {
                    var qualitySpecs = JsonConvert.DeserializeObject<List<QualitySpec>>(_qualitySpecAPIRepository.GetQualitySpecByMaterialNo(_factoryCode, master.MaterialNo, _token));
                    if (qualitySpecs != null && qualitySpecs.Count > 0)
                    {
                        lineCount = lineCount + 1 + qualitySpecs.Count / 3;
                        cloneRouting.QualitySpecs = QualitySpecsFromModel(qualitySpecs);
                    }
                    cloneRouting.CustBarcodeNo = routing.CustBarcodeNo;
                }

                if (machineGroup == "1")
                {
                    lineCount = coatings.Count() > 0 ? lineCount + coatings.Count() + 2 : lineCount;
                    cloneRouting.Coatings = coatings;
                }

                if (lineCount > 42)
                {
                    partOfMasterCardRouting.Add(new MasterCardRouting
                    {
                        Routing = cloneRouting,
                        MachineGroup = machineGroup,
                        MachineIsCalPaperWidth = machineIsCalPaperwidth,
                        IsProp_Cor = false
                    });
                }
                else
                {
                    masterCardRouting.Add(new MasterCardRouting
                    {
                        Routing = cloneRouting,
                        MachineGroup = machineGroup,
                        MachineIsCalPaperWidth = machineIsCalPaperwidth,
                        IsProp_Cor = false
                    });
                }
            }

            #region Set masterCard

            MasterCardMO result = new MasterCardMO();
            result.CreateDate = master.CreateDate.HasValue ? master.CreateDate : null;
            result.LastUpdate = master.LastUpdate.HasValue ? master.LastUpdate : null;
            var transDetail = JsonConvert.DeserializeObject<TransactionsDetail>(_transactionsDetailAPIRepository.GetTransactionsDetailByMat(_factoryCode, master.MaterialNo, _token));
            result.GlWid = transDetail == null || transDetail.Glwid == null ? false : transDetail.Glwid.Value;
            result.FactoryCode = _factoryCode;
            result.Factory = myFactory;
            result.DocName = ISO_DocName;
            result.DocDate = ISO_DocDate;
            result.OrderItem = null;
            //OrderItem = T1.OrderItem,
            result.Material_No = master.MaterialNo;
            result.Part_No = master.PartNo;
            result.PC = master.Pc;
            result.Cust_Name = master.CustName.Length > 40 ? master.CustName.Substring(0, 40) + $" ({master.CusId})" : master.CustName + $" ({master.CusId})";
            result.CustNameNOSoldto = master.CustName.Length > 40 ? master.CustName.Substring(0, 40) : master.CustName;
            result.CustomerContact = $"{_firstName} {_telephone}";
            result.Description = master.Description;
            result.Sale_Text1 = master.SaleText1 + master.SaleText2 + master.SaleText3 + master.SaleText4;
            result.EanCode = master.EanCode;
            result.Box_Type = master.BoxType;
            result.RSC_Style = master.RscStyle;
            result.JoinType = master.JoinType;
            result.Print_Method = master.PrintMethod;
            result.PalletSize = master.PalletSize;
            result.Bun = master.Bun == null ? 0 : master.Bun;
            result.BunLayer = master.BunLayer == null ? null : master.BunLayer;
            result.LayerPalet = master.LayerPalet == null ? 0 : master.LayerPalet;
            result.BoxPalet = master.BoxPalet == null ? 0 : master.BoxPalet;
            result.Material_Type = master.MaterialType;
            result.Status_Flag = master.StatusFlag;
            result.Piece_Set = master.PieceSet == null ? 0 : master.PieceSet;
            result.Piece_Patch = master.PiecePatch.HasValue ? master.PiecePatch.Value : 1;
            result.Wire = master.Wire;
            result.Wid = master.Wid;
            result.Leg = master.Leg;
            result.Hig = master.Hig;
            result.CutSheetWid = master.CutSheetWid;
            result.CutSheetLeng = master.CutSheetLeng;
            result.CutSheetLengInch = master.CutSheetLengInch.HasValue ? (master.CutSheetLengInch.Value) : 0;
            result.CutSheetWidInch = master.CutSheetWidInch.HasValue ? (master.CutSheetWidInch.Value) : 0;
            result.Flute = master.Flute;
            result.Batch = "";
            result.Due_Text = "";
            result.Tolerance_Over = null;
            result.Tolerance_Under = null;
            result.Order_Quant = 0;
            result.ScoreW1 = master.ScoreW1;
            result.Scorew2 = master.Scorew2;
            result.Scorew3 = master.Scorew3;
            result.Scorew4 = master.Scorew4;
            result.Scorew5 = master.Scorew5;
            result.Scorew6 = master.Scorew6;
            result.Scorew7 = master.Scorew7;
            result.Scorew8 = master.Scorew8;
            result.Scorew9 = master.Scorew9;
            result.Scorew10 = master.Scorew10;
            result.Scorew11 = master.Scorew11;
            result.Scorew12 = master.Scorew12;
            result.Scorew13 = master.Scorew13;
            result.Scorew14 = master.Scorew14;
            result.Scorew15 = master.Scorew15;
            result.Scorew16 = master.Scorew16;
            result.ScoreL2 = master.ScoreL2;
            result.ScoreL3 = master.ScoreL3;
            result.ScoreL4 = master.ScoreL4;
            result.ScoreL5 = master.ScoreL5;
            result.ScoreL6 = master.ScoreL6;
            result.ScoreL7 = master.ScoreL7;
            result.ScoreL8 = master.ScoreL8;
            result.ScoreL9 = master.ScoreL9;
            result.Stations = stations;
            result.Palletization_Path = master.PalletizationPath;
            result.PalletPath_Base64 = string.IsNullOrEmpty(master.PalletizationPath) || string.IsNullOrWhiteSpace(master.PalletizationPath) ? "" : ConvertPictureToBase64._ConvertPictureToBase64(master.PalletizationPath);
            result.DiecutPict_Path = master.DiecutPictPath;
            result.DiecutPath_Base64 = string.IsNullOrEmpty(master.DiecutPictPath) || string.IsNullOrWhiteSpace(master.DiecutPictPath) ? "" : ConvertPictureToBase64._ConvertPictureToBase64(master.DiecutPictPath);
            result.Change = master.Change;
            result.Rout = new List<MasterCardRouting>();
            result.PartOfRout = new List<MasterCardRouting>();

            RoutingSetDetailOfMachineAndColorProductCatalogs(ref masterCardRouting, _factoryCode);
            RoutingSetDetailOfMachineAndColorProductCatalogs(ref partOfMasterCardRouting, _factoryCode);

            result.Rout = masterCardRouting;
            result.PartOfRout = partOfMasterCardRouting;
            result.MoRout = new List<MasterCardMoRouting>();
            result.PartOfMoRout = new List<MasterCardMoRouting>();
            result.Target_Quant = null;
            result.CutNo = null;
            result.Leng = null;
            result.TwoPiece = master.TwoPiece.HasValue ? master.TwoPiece.Value : false;
            result.Slit = master.Slit == null ? 0 : master.Slit;
            result.StockQty = null;
            var boardAlt = JsonConvert.DeserializeObject<List<BoardAlternative>>(_boardAlternativeAPIRepository.GetBoardAlternativeByMat(_factoryCode, master.MaterialNo, _token));
            result.BoardAlternative = boardAlt.Count > 0 ? boardAlt.Where(b => b.Priority == 1).FirstOrDefault().BoardName : "";
            result.FormGroup = formGroup.ToString().Trim();
            result.Hierarchy = master.Hierarchy;
            result.JointLap = master.JointLap == null ? 0 : master.JointLap;
            //tassanai
            TransactionsDetail transactionsDetail = new TransactionsDetail();
            transactionsDetail = JsonConvert.DeserializeObject<TransactionsDetail>(_transactionsDetailAPIRepository.GetTransactionsDetailByMat(_factoryCode, master.MaterialNo, _token));
            //tassanai
            if (transactionsDetail != null)
            {
                result.KindofProductGroup = JsonConvert.DeserializeObject<KindOfProductGroup>(_kindOfProductGroupAPIRepository.GetKindOfProductGroupById(_factoryCode, transactionsDetail.IdKindOfProductGroup.Value, _token)).Name;
                result.KindofProduct = JsonConvert.DeserializeObject<KindOfProduct>(_kindOfProductAPIRepository.GetKindOfProductById(_factoryCode, transactionsDetail.IdKindOfProduct.Value, _token)).Name;
                //result.ProcessCost = JsonConvert.DeserializeObject<ProcessCost>(_processCostAPIRepository.GetKindOfProductById(_factoryCode, transactionsDetail.IdKindOfProduct.Value)).Name;
                result.ProcessCostList = JsonConvert.DeserializeObject<List<ProcessCost>>(Convert.ToString(_processCostAPIRepository.GetProcessCostList(_factoryCode, _token)));
                var ProcessCostList = result.ProcessCostList.Where(p => p.Id == transactionsDetail.IdProcessCost).FirstOrDefault();
                result.ProcessCost = ProcessCostList.Name;

                result.NewPrintPlate = transactionsDetail.NewPrintPlate;
                result.OldPrintPlate = transactionsDetail.OldPrintPlate;
                result.NewBlockDieCut = transactionsDetail.NewBlockDieCut;
                result.OldBlockDieCut = transactionsDetail.OldBlockDieCut;
                result.ExampleColor = transactionsDetail.ExampleColor;
                result.CoatingType = transactionsDetail.CoatingType;
                result.CoatingTypeDesc = transactionsDetail.CoatingTypeDesc;
                result.PaperHorizontal = transactionsDetail.PaperHorizontal.HasValue ? transactionsDetail.PaperHorizontal.Value : false;
                result.PaperVertical = transactionsDetail.PaperVertical.HasValue ? transactionsDetail.PaperVertical.Value : false;
                result.FluteHorizontal = transactionsDetail.FluteHorizontal.HasValue ? transactionsDetail.FluteHorizontal.Value : false;
                result.FluteVertical = transactionsDetail.FluteVertical.HasValue ? transactionsDetail.FluteVertical.Value : false;
            }

            //tassanai
            result.High_Value = master.HighValue;

            result.NoSlot = formGroup.ToString().Trim() == "AC" ? master.NoSlot.Value : 0;

            #endregion Set masterCard

            //var cat = JsonConvert.DeserializeObject<List<BoardAlternative>>(_boardAlternativeAPIRepository.GetBoardAlternativeByMat(_factoryCode, master.MaterialNo));

            return result;
        }

        public List<Routing> GetRoutingProductCatalogs(string MaterialNo, string FactoryCode)//2
        {
            return JsonConvert.DeserializeObject<List<Routing>>(_routingAPIRepository.GetRoutingsByMaterialNo(FactoryCode, MaterialNo, _token)).OrderBy(o => o.SeqNo).ToList();
        }

        private List<Coating> GetCoatingsProductCatalogs(string materialNo, string FactoryCode)
        {
            var coatings = new List<Coating>();
            var coatings_api = JsonConvert.DeserializeObject<List<Coating>>(_coatingAPIRepository.GetCoatingByMaterialNo(FactoryCode, materialNo, _token)).OrderBy(c => c.Id).ToList();

            foreach (var coating in coatings_api)
            {
                var itemCoating = new Coating
                {
                    Station = coating.Station,
                    Type = coating.Type,
                    Name = coating.Name,
                    Id = coating.Id
                };

                if (!String.IsNullOrEmpty(itemCoating.Name) && !String.IsNullOrWhiteSpace(itemCoating.Name))
                {
                    coatings.Add(itemCoating);
                }
            }

            return coatings.OrderBy(c => c.Id).ToList();
        }

        public void RoutingSetDetailOfMachineAndColorProductCatalogs(ref List<MasterCardRouting> masterCardRoutings, string FactoryCode)
        {
            var moRoutings = new List<MoRouting>();
            for (int i = 0; i < masterCardRoutings.Count; i++)
            {
                var masterCardRouting = masterCardRoutings[i];
                var routingMachine = string.Empty;
                var routingColor = string.Empty;
                var machine = JsonConvert.DeserializeObject<DataAccess.Models.Machine>(_machineAPIRepository.GetMachineGroupByPlanCode(FactoryCode, masterCardRouting.Routing.PlanCode, _token));

                //machine
                if (masterCardRouting.Routing.PlateNo != null && masterCardRouting.Routing.PlateNo != "")
                {
                    routingMachine = routingMachine + "Plate No. " + masterCardRouting.Routing.PlateNo + ", ";
                }

                var blockBlk = "";
                if (!string.IsNullOrEmpty(masterCardRouting.Routing.BlockNo))
                {
                    if (masterCardRouting.Routing.NoneBlk.HasValue && masterCardRouting.Routing.NoneBlk.Value)
                    {
                        blockBlk = "Non print" + ", ";
                    }
                    else if (masterCardRouting.Routing.StanBlk.HasValue && masterCardRouting.Routing.StanBlk.Value)
                    {
                        blockBlk = "Block Std. No. " + masterCardRouting.Routing.BlockNo + ", ";
                    }
                    else if (masterCardRouting.Routing.SemiBlk.HasValue && masterCardRouting.Routing.SemiBlk.Value)
                    {
                        blockBlk = "Block Semi. No. " + masterCardRouting.Routing.BlockNo + ", ";
                    }
                    else if (masterCardRouting.Routing.ShipBlk.HasValue && masterCardRouting.Routing.ShipBlk.Value)
                    {
                        blockBlk = "Block Ship. No. " + masterCardRouting.Routing.BlockNo + ", ";
                    }
                    routingMachine = routingMachine + blockBlk;
                    //routingMachine = routingMachine + "Block Ship. No. " + masterCardRouting.Routing.BlockNo + ", ";
                }
                else
                {
                    if (masterCardRouting.Routing.NoneBlk.HasValue && masterCardRouting.Routing.NoneBlk.Value)
                    {
                        blockBlk = "Non print , ";
                    }
                    else if (masterCardRouting.Routing.StanBlk.HasValue && masterCardRouting.Routing.StanBlk.Value)
                    {
                        blockBlk = "Block Std. No. , ";
                    }
                    else if (masterCardRouting.Routing.SemiBlk.HasValue && masterCardRouting.Routing.SemiBlk.Value)
                    {
                        blockBlk = "Block Semi. No. , ";
                    }
                    else if (masterCardRouting.Routing.ShipBlk.HasValue && masterCardRouting.Routing.ShipBlk.Value)
                    {
                        blockBlk = "Block Ship. No. , ";
                    }
                    routingMachine = routingMachine + blockBlk;
                }

                if (!string.IsNullOrEmpty(masterCardRouting.Routing.MylaNo) && machine.IsPropPrint.Value)
                {
                    routingMachine = routingMachine + "Myla No. " + masterCardRouting.Routing.MylaNo + ", ";
                }

                var lengthOfMachine = routingMachine.Length;
                if (lengthOfMachine != 0)
                {
                    routingMachine = routingMachine.Substring(0, lengthOfMachine - 2);
                }

                //color
                if (!string.IsNullOrEmpty(masterCardRouting.Routing.Color1) && !string.IsNullOrEmpty(masterCardRouting.Routing.Shade1))
                {
                    routingColor = routingColor + masterCardRouting.Routing.Color1 + " " + masterCardRouting.Routing.Shade1 + ", ";
                }
                if (!string.IsNullOrEmpty(masterCardRouting.Routing.Color2) && !string.IsNullOrEmpty(masterCardRouting.Routing.Shade2))
                {
                    routingColor = routingColor + masterCardRouting.Routing.Color2 + " " + masterCardRouting.Routing.Shade2 + ", ";
                }
                if (!string.IsNullOrEmpty(masterCardRouting.Routing.Color3) && !string.IsNullOrEmpty(masterCardRouting.Routing.Shade3))
                {
                    routingColor = routingColor + masterCardRouting.Routing.Color3 + " " + masterCardRouting.Routing.Shade3 + ", ";
                }
                if (!string.IsNullOrEmpty(masterCardRouting.Routing.Color4) && !string.IsNullOrEmpty(masterCardRouting.Routing.Shade4))
                {
                    routingColor = routingColor + masterCardRouting.Routing.Color4 + " " + masterCardRouting.Routing.Shade4 + ", ";
                }
                if (!string.IsNullOrEmpty(masterCardRouting.Routing.Color5) && !string.IsNullOrEmpty(masterCardRouting.Routing.Shade5))
                {
                    routingColor = routingColor + masterCardRouting.Routing.Color5 + " " + masterCardRouting.Routing.Shade5 + ", ";
                }
                if (!string.IsNullOrEmpty(masterCardRouting.Routing.Color6) && !string.IsNullOrEmpty(masterCardRouting.Routing.Shade6))
                {
                    routingColor = routingColor + masterCardRouting.Routing.Color6 + " " + masterCardRouting.Routing.Shade6 + ", ";
                }
                if (!string.IsNullOrEmpty(masterCardRouting.Routing.Color7) && !string.IsNullOrEmpty(masterCardRouting.Routing.Shade7))
                {
                    routingColor = routingColor + masterCardRouting.Routing.Color7 + " " + masterCardRouting.Routing.Shade7 + ", ";
                }
                if (!string.IsNullOrEmpty(masterCardRouting.Routing.Color8) && !string.IsNullOrEmpty(masterCardRouting.Routing.Shade8))
                {
                    routingColor = routingColor + masterCardRouting.Routing.Color8 + " " + masterCardRouting.Routing.Shade8 + ", ";
                }
                if (!string.IsNullOrEmpty(masterCardRouting.Routing.Color9) && !string.IsNullOrEmpty(masterCardRouting.Routing.Shade9))
                {
                    routingColor = routingColor + masterCardRouting.Routing.Color9 + " " + masterCardRouting.Routing.Shade9 + ", ";
                }
                if (!string.IsNullOrEmpty(masterCardRouting.Routing.Color10) && !string.IsNullOrEmpty(masterCardRouting.Routing.Shade10))
                {
                    routingColor = routingColor + masterCardRouting.Routing.Color10 + " " + masterCardRouting.Routing.Shade10 + ", ";
                }

                var lengthOfColor = routingColor.Length;
                if (lengthOfColor != 0)
                {
                    routingColor = routingColor.Substring(0, lengthOfColor - 2);
                }

                if (!string.IsNullOrEmpty(routingMachine))
                {
                    masterCardRouting.Routing.MachineDetail = routingMachine;
                }

                if (!string.IsNullOrEmpty(routingColor))
                {
                    masterCardRouting.Routing.MachineColorDetail = "Color : " + routingColor;
                }

                masterCardRoutings[i].Routing = masterCardRouting.Routing;
            }
        }

        private string CombineMultiplePdFs(List<string> fileNames)
        {
            var isMergeSuccess = false;
            var pmtsConfig = JsonConvert.DeserializeObject<PmtsConfig>(_pMTsConfigAPIRepository.GetPMTsConfigByFactoryName(_factoryCode, "ManageMO_Path", _token));
            var pdfFileLocation = pmtsConfig != null ? pmtsConfig.FucValue : throw new Exception("Can't get file path.");
            //var fileMerge = $"{orderItem}_{DateTime.Now.ToString("ddMMyyyy_HH_mm_ss")}.pdf";
            //var outFile = Path.Combine(pdfFileLocation, fileMerge);
            var result = string.Empty;

            try
            {
                // step 1: creation of a document-object
                var document = new Document();

                //outFile = @"D:\ManageMO\TCNK\nanana.pdf";
                // step 2: we create a writer that listens to the document
                var baos = new MemoryStream();
                PdfCopy writer = new PdfCopy(document, baos);

                //PdfCopy writer = new PdfCopy(document, new FileStream(outFile, FileMode.Create));

                // step 3: we open the document
                document.Open();

                foreach (string fileName in fileNames)
                {
                    try
                    {
                        // we create a reader for a certain document
                        PdfReader reader = new PdfReader(fileName);
                        reader.ConsolidateNamedDestinations();

                        // step 4: we add content
                        for (int i = 1; i <= reader.NumberOfPages; i++)
                        {
                            PdfImportedPage page = writer.GetImportedPage(reader, i);
                            writer.AddPage(page);
                        }

                        PrAcroForm form = reader.AcroForm;
                        if (form != null)
                        {
                            //writer.CopyDocumentFields(reader);
                            writer.CopyAcroForm(reader);
                        }

                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }

                // step 5: we close the document and writer
                writer.Close();
                document.Close();
                byte[] documentBytes = baos.ToArray();
                result = JsonConvert.SerializeObject(documentBytes);
                isMergeSuccess = true;
            }
            catch (Exception ex)
            {
                result = string.Empty;
                isMergeSuccess = false;
            }

            if (isMergeSuccess)
            {
                return result;
            }
            else
            {
                return result;
            }
        }

        public async Task<BasePrintMastercardData> GetBaseOfPrintMastercardMO(BasePrintMastercardData basePrintMastercard, List<string> orderItem, UserTIP userTIP)
        {
            //mo from tip
            var planningMODataAndMoSpec = new List<PlanningMODataAndMoSpec>();
            var planningRouting = new List<PlanningRouting>();
            var isUserTIPs = userTIP != null && !string.IsNullOrEmpty(userTIP.Token) && !string.IsNullOrEmpty(userTIP.UrlApi);

            basePrintMastercard = JsonConvert.DeserializeObject<BasePrintMastercardData>(_moDataAPIRepository.GetBaseOfMasterCardMOsBySaleOrders(_factoryCode, isUserTIPs, JsonConvert.SerializeObject(orderItem), _token));
            if (basePrintMastercard.MoDatas.Count > 0 && isUserTIPs)
            {
                var planningModel = new PlaningMOModel();
                planningModel = await _moDataAPIRepository.GetDataOfMOFromTIPs(planningModel, userTIP, JsonConvert.SerializeObject(orderItem));
                planningRouting = planningModel.Data.PlanningRoutingList;
                planningMODataAndMoSpec = planningModel.Data.PlanningSpecList;
            }

            if (isUserTIPs && planningRouting.Count > 0)
            {
                basePrintMastercard.Machines = JsonConvert.DeserializeObject<List<DataAccess.Models.Machine>>(_machineAPIRepository.GetMachinesByPlanCodes(_factoryCode, JsonConvert.SerializeObject(planningRouting.Select(m => m.Plan_Code).ToList()), _token));
            }

            #region old get base of mastercard mo

            //var materialNosOfMODatas = new List<string>();
            //var materialNosOfMasterDatas = new List<string>();
            //var codesOfMasterDatas = new List<string>();
            //var lv2sOfMasterDatas = new List<string>();
            //var planCode = new List<string>();
            //var flutes = new List<string>();
            //var codesOfBoardCombines = new List<string>();
            //var machines = new List<Machine>();
            //var masterDatas = new List<MasterData>();
            //var qualitySpecs = new List<QualitySpec>();
            //var boardCombines = new List<BoardCombine>();
            //var boardUses = new List<BoardUse>();
            //var boardSpecs = new List<BoardSpec>();
            //var fluteTrs = new List<FluteTr>();
            //var productTypes = new List<ProductType>();
            //var moSpecs = new List<MoSpec>();
            //var attachFileMOs = new List<AttachFileMo>();
            //var moRoutings = new List<MoRoutingPrintMastercard>();
            //var pmtsConfig = new List<PmtsConfig>();
            //var boardAlternatives = new List<BoardAlternative>();
            //var isUserTIPs = userTIP != null && !string.IsNullOrEmpty(userTIP.Token) && !string.IsNullOrEmpty(userTIP.UrlApi);

            ////mo from tip
            //var planningMODataAndMoSpec = new List<PlanningMODataAndMoSpec>();
            //var planningRouting = new List<PlanningRouting>();

            //var moDatas = JsonConvert.DeserializeObject<List<MoDataPrintMastercard>>(_moDataAPIRepository.GetMoDataListBySaleOrders(_factoryCode, JsonConvert.SerializeObject(orderItem), _token));

            //if (moDatas.Count > 0)
            //{
            //    materialNosOfMODatas = moDatas.Select(m => m.MaterialNo).ToList();
            //    if (materialNosOfMODatas.Count > 0)
            //    {
            //        masterDatas = JsonConvert.DeserializeObject<List<MasterData>>(_masterDataAPIRepository.GetMasterDatasByMaterialNos(_factoryCode, JsonConvert.SerializeObject(materialNosOfMODatas), _token));
            //        boardAlternatives = JsonConvert.DeserializeObject<List<BoardAlternative>>(_boardAlternativeAPIRepository.GetBoardAlternativesByMaterialNos(_factoryCode, JsonConvert.SerializeObject(materialNosOfMODatas), _token));
            //    }
            //    materialNosOfMasterDatas = masterDatas.Select(m => m.MaterialNo).ToList();
            //    codesOfMasterDatas = masterDatas.Select(m => m.Code).ToList();
            //    lv2sOfMasterDatas = masterDatas.Select(m => m.Hierarchy.Substring(2, 2)).ToList();
            //    if (materialNosOfMasterDatas.Count > 0)
            //    {
            //        qualitySpecs = JsonConvert.DeserializeObject<List<QualitySpec>>(_qualitySpecAPIRepository.GetQualitySpecsByMaterialNos(_factoryCode, JsonConvert.SerializeObject(materialNosOfMasterDatas), _token));
            //        boardUses = JsonConvert.DeserializeObject<List<BoardUse>>(_boardUseAPIRepository.GetBoardUsesByMaterialNos(_factoryCode, JsonConvert.SerializeObject(materialNosOfMasterDatas), _token));
            //    }

            //    if (codesOfMasterDatas.Count > 0)
            //    {
            //        boardCombines = JsonConvert.DeserializeObject<List<BoardCombine>>(_boardCombineAPIRepository.GetBoardsByCodes(_factoryCode, JsonConvert.SerializeObject(codesOfMasterDatas), _token));
            //    }

            //    if (lv2sOfMasterDatas.Count > 0)
            //    {
            //        productTypes = JsonConvert.DeserializeObject<List<ProductType>>(_productTypeAPIRepository.GetProductTypesByHierarchyLv2s(_factoryCode, JsonConvert.SerializeObject(lv2sOfMasterDatas), _token));
            //    }

            //    if (isUserTIPs)
            //    {
            //        var planningModel = new PlaningMOModel();
            //        planningModel = await _moDataAPIRepository.GetDataOfMOFromTIPs(planningModel, userTIP, JsonConvert.SerializeObject(orderItem));
            //        planningRouting = planningModel.Data.PlanningRoutingList;
            //        planningMODataAndMoSpec = planningModel.Data.PlanningSpecList;
            //    }
            //}

            //moSpecs = JsonConvert.DeserializeObject<List<MoSpec>>(_moSpecAPIRepository.GetMOSpecsBySaleOrders(_factoryCode, JsonConvert.SerializeObject(orderItem), _token));
            //attachFileMOs = JsonConvert.DeserializeObject<List<AttachFileMo>>(_attachFileMOAPIRepository.GetAttachFileMOsByOrderItems(_factoryCode, JsonConvert.SerializeObject(orderItem), _token));
            //moRoutings = JsonConvert.DeserializeObject<List<MoRoutingPrintMastercard>>(_moRoutingAPIRepository.GetMORoutingsBySaleOrders(_factoryCode, JsonConvert.SerializeObject(orderItem), _token));
            //pmtsConfig = JsonConvert.DeserializeObject<List<PmtsConfig>>(_pMTsConfigAPIRepository.GetPMTsConfigList(_factoryCode, _token));

            //if (moRoutings.Count > 0)
            //{
            //    if (isUserTIPs && planningRouting.Count > 0)
            //    {
            //        planCode = planningRouting.Select(m => m.Plan_Code).ToList();
            //    }
            //    else
            //    {
            //        planCode = moRoutings.Select(m => m.PlanCode).ToList();
            //    }
            //    machines = JsonConvert.DeserializeObject<List<Machine>>(_machineAPIRepository.GetMachinesByPlanCodes(_factoryCode, JsonConvert.SerializeObject(planCode), _token));
            //}

            //if (moSpecs.Count > 0)
            //{
            //    flutes = moSpecs.Select(m => m.Flute).ToList();
            //    if (boardCombines.Count > 0)
            //    {
            //        flutes.AddRange(boardCombines.Select(b => b.Flute).ToList());
            //    }

            //    flutes = flutes.GroupBy(x => x).Select(g => g.First()).ToList();
            //    fluteTrs = JsonConvert.DeserializeObject<List<FluteTr>>(_fluteTrAPIRepository.GetFluteTrsByFlutes(_factoryCode, JsonConvert.SerializeObject(flutes), _token));
            //}

            //if (boardCombines.Count > 0)
            //{
            //    codesOfBoardCombines = boardCombines.Select(b => b.Code).ToList();
            //    boardSpecs = JsonConvert.DeserializeObject<List<BoardSpec>>(_boardSpecAPIRepository.GetBoardSpecsByCodes(_factoryCode, JsonConvert.SerializeObject(codesOfBoardCombines), _token));
            //}

            //basePrintMastercard = new BasePrintMastercardData
            //{
            //    AttachFileMOs = attachFileMOs,
            //    BoardCombines = boardCombines,
            //    BoardSpecs = boardSpecs,
            //    BoardUses = boardUses,
            //    FluteTrs = fluteTrs,
            //    Machines = machines,
            //    MasterDatas = masterDatas,
            //    MoDatas = moDatas,
            //    MoRoutings = moRoutings,
            //    MoSpecs = moSpecs,
            //    PmtsConfigs = pmtsConfig,
            //    ProductTypes = productTypes,
            //    QualitySpecs = qualitySpecs,
            //    BoardAlternatives = boardAlternatives,
            //    PlanningMODataAndMoSpecs = planningMODataAndMoSpec,
            //    PlanningRoutings = planningRouting

            //};

            #endregion old get base of mastercard mo

            return basePrintMastercard;
        }

        public void GetBaseOfPrintMastercard(ref BasePrintMastercardData basePrintMastercard, List<string> materialNOs)
        {
            var materialNosOfMasterDatas = new List<string>();
            var codesOfMasterDatas = new List<string>();
            var lv2sOfMasterDatas = new List<string>();
            var planCode = new List<string>();
            var scoreTypeOfRouting = new List<string>();
            var fluteTr = new List<string>();
            var codesOfBoardCombines = new List<string>();
            var idKindOfProducts = new List<string>();
            var idKindOfProductGroups = new List<string>();

            var machines = new List<DataAccess.Models.Machine>();
            var masterDatas = new List<MasterData>();
            var qualitySpecs = new List<QualitySpec>();
            var boardCombines = new List<BoardCombine>();
            var boardUses = new List<BoardUse>();
            var boardSpecs = new List<BoardSpec>();
            var fluteTrs = new List<FluteTr>();
            var productTypes = new List<ProductType>();
            var pmtsConfig = new List<PmtsConfig>();
            var scoreTypes = new List<ScoreType>();
            var boardAlternatives = new List<BoardAlternative>();
            var transactionsDetails = new List<TransactionsDetail>();
            var kindOfProductGroups = new List<KindOfProductGroup>();
            var kindOfProducts = new List<KindOfProduct>();
            var processCosts = new List<ProcessCost>();
            var routings = new List<Routing>();

            masterDatas = JsonConvert.DeserializeObject<List<MasterData>>(_masterDataAPIRepository.GetMasterDatasByMaterialNos(_factoryCode, JsonConvert.SerializeObject(materialNOs), _token));
            pmtsConfig = JsonConvert.DeserializeObject<List<PmtsConfig>>(_pMTsConfigAPIRepository.GetPMTsConfigList(_factoryCode, _token));

            if (masterDatas.Count > 0)
            {
                var lv2sOfMasterDatasTemp = masterDatas.Select(m => m.Hierarchy.Substring(2, 2)).ToList();
                lv2sOfMasterDatas = lv2sOfMasterDatasTemp.GroupBy(x => x).Select(g => g.First()).ToList();

                var codesOfMasterDatasTemp = masterDatas.Select(m => m.Code).ToList();
                codesOfMasterDatas = codesOfMasterDatasTemp.GroupBy(x => x).Select(g => g.First()).ToList();

                var materialNosOfMasterDatasTemp = masterDatas.Select(m => m.MaterialNo).ToList();
                materialNosOfMasterDatas = materialNosOfMasterDatasTemp.GroupBy(x => x).Select(g => g.First()).ToList();

                if (lv2sOfMasterDatas.Count > 0)
                {
                    productTypes = JsonConvert.DeserializeObject<List<ProductType>>(_productTypeAPIRepository.GetProductTypesByHierarchyLv2s(_factoryCode, JsonConvert.SerializeObject(lv2sOfMasterDatas), _token));
                }

                if (materialNosOfMasterDatas.Count > 0)
                {
                    qualitySpecs = JsonConvert.DeserializeObject<List<QualitySpec>>(_qualitySpecAPIRepository.GetQualitySpecsByMaterialNos(_factoryCode, JsonConvert.SerializeObject(materialNosOfMasterDatas), _token));
                    boardUses = JsonConvert.DeserializeObject<List<BoardUse>>(_boardUseAPIRepository.GetBoardUsesByMaterialNos(_factoryCode, JsonConvert.SerializeObject(materialNosOfMasterDatas), _token));
                    boardAlternatives = JsonConvert.DeserializeObject<List<BoardAlternative>>(_boardAlternativeAPIRepository.GetBoardAlternativesByMaterialNos(_factoryCode, JsonConvert.SerializeObject(materialNosOfMasterDatas), _token));
                    transactionsDetails = JsonConvert.DeserializeObject<List<TransactionsDetail>>(_transactionsDetailAPIRepository.GetTransactionsDetailsByMaterialNOs(_factoryCode, JsonConvert.SerializeObject(materialNosOfMasterDatas), _token));
                    routings = JsonConvert.DeserializeObject<List<Routing>>(_routingAPIRepository.GetRoutingsByMaterialNos(_factoryCode, JsonConvert.SerializeObject(materialNosOfMasterDatas), _token));
                }

                if (codesOfMasterDatas.Count > 0)
                {
                    boardCombines = JsonConvert.DeserializeObject<List<BoardCombine>>(_boardCombineAPIRepository.GetBoardsByCodes(_factoryCode, JsonConvert.SerializeObject(codesOfMasterDatas), _token));

                    fluteTr = boardCombines.Select(b => b.Flute).ToList();
                    codesOfBoardCombines = boardCombines.Select(b => b.Code).ToList();

                    if (fluteTr.Count > 0)
                    {
                        fluteTr = fluteTr.GroupBy(x => x).Select(g => g.First()).ToList();
                        fluteTrs = JsonConvert.DeserializeObject<List<FluteTr>>(_fluteTrAPIRepository.GetFluteTrsByFlutes(_factoryCode, JsonConvert.SerializeObject(fluteTr), _token));
                    }

                    if (codesOfBoardCombines.Count > 0)
                    {
                        boardSpecs = JsonConvert.DeserializeObject<List<BoardSpec>>(_boardSpecAPIRepository.GetBoardSpecsByCodes(_factoryCode, JsonConvert.SerializeObject(codesOfBoardCombines), _token));
                    }
                }

                if (routings.Count > 0)
                {
                    planCode = routings.Select(b => b.PlanCode).ToList();
                    scoreTypeOfRouting = routings.Select(b => b.ScoreType).ToList();
                    machines = JsonConvert.DeserializeObject<List<DataAccess.Models.Machine>>(_machineAPIRepository.GetMachinesByPlanCodes(_factoryCode, JsonConvert.SerializeObject(planCode), _token));
                    scoreTypes = JsonConvert.DeserializeObject<List<ScoreType>>(_scoreTypeAPIRepository.GetScoreTypesByScoreTypeIds(_factoryCode, JsonConvert.SerializeObject(scoreTypeOfRouting), _token));
                }

                if (transactionsDetails.Count > 0)
                {
                    idKindOfProductGroups = transactionsDetails.Select(k => k.IdKindOfProductGroup.Value.ToString()).ToList();
                    idKindOfProducts = transactionsDetails.Select(k => k.IdKindOfProduct.Value.ToString()).ToList();

                    kindOfProductGroups = JsonConvert.DeserializeObject<List<KindOfProductGroup>>(_kindOfProductGroupAPIRepository.GetKindOfProductGroupsByIds(_factoryCode, JsonConvert.SerializeObject(idKindOfProductGroups), _token));
                    kindOfProducts = JsonConvert.DeserializeObject<List<KindOfProduct>>(_kindOfProductAPIRepository.GetKindOfProductsByIds(_factoryCode, JsonConvert.SerializeObject(idKindOfProducts), _token));
                    processCosts = JsonConvert.DeserializeObject<List<ProcessCost>>(_processCostAPIRepository.GetProcessCostList(_factoryCode, _token));
                }
            }

            basePrintMastercard = new BasePrintMastercardData
            {
                AttachFileMOs = new List<AttachFileMo>(),
                BoardCombines = boardCombines,
                BoardSpecs = boardSpecs,
                BoardUses = boardUses,
                FluteTrs = fluteTrs,
                Machines = machines,
                MasterDatas = masterDatas,
                MoDatas = new List<MoDataPrintMastercard>(),
                MoRoutings = new List<MoRoutingPrintMastercard>(),
                MoSpecs = new List<MoSpec>(),
                PmtsConfigs = pmtsConfig,
                ProductTypes = productTypes,
                QualitySpecs = qualitySpecs,
                BoardAlternatives = boardAlternatives,
                KindOfProductGroups = kindOfProductGroups,
                KindOfProducts = kindOfProducts,
                ProcessCosts = processCosts,
                Routings = routings,
                ScoreTypes = scoreTypes,
                TransactionsDetails = transactionsDetails
            };
        }

        public async Task<byte[]> SavePDFWithOutAttachFile(ReportController reportController, PrintMasterCardMOModel printMasterCardMOModel)
        {
            var fileNames = new List<string>();
            var fileTemps = new List<string>();
            var MOsWithoutAttachfiles = new List<MasterCardMO>();
            var MOsSerialize = JsonConvert.SerializeObject(printMasterCardMOModel);
            var originalMOs = JsonConvert.DeserializeObject<PrintMasterCardMOModel>(MOsSerialize);
            var fileNo = 0;
            var MOsNo = 0;
            //var MOsWithoutAttachfiles = printMasterCardMOModel.MasterCardMOs.Where(m => string.IsNullOrEmpty(m.AttchFilesBase64)).ToList();
            var MOsWithAttachfiles = printMasterCardMOModel.MasterCardMOs.Where(m => !string.IsNullOrEmpty(m.AttchFilesBase64)).ToList();
            var pmtsConfig = JsonConvert.DeserializeObject<PmtsConfig>(_pMTsConfigAPIRepository.GetPMTsConfigByFactoryName(_factoryCode, "ManageMO_Path", _token));
            var filePath = pmtsConfig != null ? pmtsConfig.FucValue : throw new Exception("Can't get file path.");
            var datetimeNow = DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss");
            //fileTemps.Add(Path.Combine(filePath, $"{datetimeNow}_FileMain.pdf"));
            var pageSize = _businessGroup ? Rotativa.AspNetCore.Options.Size.A3 : Rotativa.AspNetCore.Options.Size.A4;
            var viewMasterCardMOName = _businessGroup ? "PPCMasterCardMO" : "MasterCardMO";

            //create folder if doesn't exist
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            #region Create PDF with Razor View

            //if (MOsWithoutAttachfiles.Count > 0)
            //{
            //    printMasterCardMOModel.MasterCardMOs = new List<MasterCardMO>();
            //    printMasterCardMOModel.MasterCardMOs.AddRange(MOsWithoutAttachfiles);
            //    var actionResultWithoutAttachfiles = new ViewAsPdf("MasterCardMO", printMasterCardMOModel)
            //    {
            //        FileName = Path.Combine(filePath, $"{datetimeNow}_FileMain.pdf"),
            //        PageMargins = new Margins(0, 2, 0, 2),
            //        PageSize = Rotativa.AspNetCore.Options.Size.A4,
            //        ContentType = "application/pdf",
            //        PageOrientation = Orientation.Portrait,
            //    };

            //    var byteArrayWithoutAttachfiles = actionResultWithoutAttachfiles.BuildFile(reportController.ControllerContext);

            //    //create folder if doesn't exist
            //    if (!Directory.Exists(filePath))
            //    {
            //        Directory.CreateDirectory(filePath);
            //    }

            //    var fileStreamWithoutAttachfiles = new FileStream(Path.Combine(filePath, $"{datetimeNow}_FileMain.pdf"), FileMode.Create, FileAccess.Write);
            //    fileStreamWithoutAttachfiles.Write(byteArrayWithoutAttachfiles.Result, 0, byteArrayWithoutAttachfiles.Result.Length);
            //    fileStreamWithoutAttachfiles.Close();
            //}

            //foreach (var mo in MOsWithAttachfiles)
            //{
            //    var GenarateFileName = $"{datetimeNow}_{mo.OrderItem}_FileMain.pdf";
            //    var fullPath = Path.Combine(filePath, GenarateFileName);
            //    fileTemps.Add(fullPath);
            //    printMasterCardMOModel.MasterCardMOs = new List<MasterCardMO>();
            //    printMasterCardMOModel.MasterCardMOs.Add(mo);
            //    var actionResult = new ViewAsPdf("MasterCardMO", printMasterCardMOModel)
            //    {
            //        FileName = fullPath,
            //        PageMargins = new Margins(0, 2, 0, 2),
            //        PageSize = Rotativa.AspNetCore.Options.Size.A4,
            //        ContentType = "application/pdf",
            //        PageOrientation = Orientation.Portrait,
            //    };

            //    var byteArray = actionResult.BuildFile(reportController.ControllerContext);

            //    var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
            //    fileStream.Write(byteArray.Result, 0, byteArray.Result.Length);
            //    fileStream.Close();
            //}

            #endregion Create PDF with Razor View

            var activity = ActivitySourceProvider.Source!.StartActivity($"{nameof(MasterCardService)} :: {nameof(SavePDFWithOutAttachFile)}");

            #region Combine all print mastercard file

            foreach (var masterCardMO in originalMOs.MasterCardMOs)
            {
                MOsNo++;
                var activity1 = ActivitySourceProvider.Source!.StartActivity($"{nameof(MasterCardService)} :: {nameof(SavePDFWithOutAttachFile)} :: Loop {MOsNo}");
                if (string.IsNullOrEmpty(masterCardMO.AttchFilesBase64))
                {
                    if (masterCardMO.Material_No is not null) MOsWithoutAttachfiles.Add(masterCardMO);
                    if (originalMOs.MasterCardMOs.Count == MOsNo)
                    {
                        if (MOsWithoutAttachfiles != null && MOsWithoutAttachfiles.Count > 0)
                        {
                            var activity2 = ActivitySourceProvider.Source!.StartActivity($"{nameof(MasterCardService)} :: {nameof(SavePDFWithOutAttachFile)} :: if (string.IsNullOrEmpty(masterCardMO.AttchFilesBase64))");
                            fileNo++;
                            var GenarateFileName = $"{datetimeNow}_FileMain{fileNo}.pdf";
                            var fullPath = Path.Combine(filePath, GenarateFileName);
                            printMasterCardMOModel.MasterCardMOs = new List<MasterCardMO>();
                            printMasterCardMOModel.MasterCardMOs.AddRange(MOsWithoutAttachfiles);

                            var actionResultWithoutAttachfiles = new ViewAsPdf(viewMasterCardMOName, printMasterCardMOModel)
                            {
                                FileName = Path.Combine(filePath, $"{datetimeNow}_FileMain{fileNo}.pdf"),
                                PageMargins = new Margins(0, 2, 0, 2),
                                PageSize = pageSize,
                                ContentType = "application/pdf",
                                PageOrientation = Orientation.Portrait,
                            };

                            var byteArrayWithoutAttachfiles = await actionResultWithoutAttachfiles.BuildFile(reportController.ControllerContext);

                            var fileStreamWithoutAttachfiles = new FileStream(Path.Combine(filePath, $"{datetimeNow}_FileMain{fileNo}.pdf"), FileMode.Create, FileAccess.Write);
                            fileStreamWithoutAttachfiles.Write(byteArrayWithoutAttachfiles, 0, byteArrayWithoutAttachfiles.Length);
                            fileStreamWithoutAttachfiles.Close();

                            fileTemps.Add(fullPath);
                            fileNames.Add(fullPath);

                            MOsWithoutAttachfiles.Clear();
                            activity2.Stop();
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(masterCardMO.AttchFilesBase64) || originalMOs.MasterCardMOs.Count == MOsNo)
                {
                    if (MOsWithoutAttachfiles != null && MOsWithoutAttachfiles.Count > 0)
                    {
                        var activity2 = ActivitySourceProvider.Source!.StartActivity($"(!string.IsNullOrEmpty(masterCardMO.AttchFilesBase64) || originalMOs.MasterCardMOs.Count == MOsNo) :: if (MOsWithoutAttachfiles != null && MOsWithoutAttachfiles.Count > 0)");
                        fileNo++;
                        var GenarateFileName = $"{datetimeNow}_FileMain{fileNo}.pdf";
                        var fullPath = Path.Combine(filePath, GenarateFileName);
                        printMasterCardMOModel.MasterCardMOs = new List<MasterCardMO>();
                        printMasterCardMOModel.MasterCardMOs.AddRange(MOsWithoutAttachfiles);

                        var actionResultWithoutAttachfiles = new ViewAsPdf(viewMasterCardMOName, printMasterCardMOModel)
                        {
                            FileName = Path.Combine(filePath, $"{datetimeNow}_FileMain{fileNo}.pdf"),
                            PageMargins = new Margins(0, 2, 0, 2),
                            PageSize = pageSize,
                            ContentType = "application/pdf",
                            PageOrientation = Orientation.Portrait,
                        };

                        var byteArrayWithoutAttachfiles = await actionResultWithoutAttachfiles.BuildFile(reportController.ControllerContext);

                        var fileStreamWithoutAttachfiles = new FileStream(Path.Combine(filePath, $"{datetimeNow}_FileMain{fileNo}.pdf"), FileMode.Create, FileAccess.Write);
                        fileStreamWithoutAttachfiles.Write(byteArrayWithoutAttachfiles, 0, byteArrayWithoutAttachfiles.Length);
                        fileStreamWithoutAttachfiles.Close();

                        fileTemps.Add(fullPath);
                        fileNames.Add(fullPath);

                        MOsWithoutAttachfiles.Clear();
                        activity2.Stop();
                    }

                    var attachfiles = JsonConvert.DeserializeObject<List<AttachFileMo>>(masterCardMO.AttchFilesBase64);
                    if (attachfiles != null && attachfiles.Count > 0)
                    {
                        var activity2 = ActivitySourceProvider.Source!.StartActivity($"(!string.IsNullOrEmpty(masterCardMO.AttchFilesBase64) || originalMOs.MasterCardMOs.Count == MOsNo) :: if (attachfiles != null && attachfiles.Count > 0)");
                        var GenarateFileName = $"{datetimeNow}_{masterCardMO.OrderItem}_FileMain.pdf";
                        var fullPath = Path.Combine(filePath, GenarateFileName);
                        printMasterCardMOModel.MasterCardMOs = new List<MasterCardMO>();
                        printMasterCardMOModel.MasterCardMOs.Add(masterCardMO);
                        var actionResult = new ViewAsPdf(viewMasterCardMOName, printMasterCardMOModel)
                        {
                            FileName = fullPath,
                            PageMargins = new Margins(0, 2, 0, 2),
                            PageSize = pageSize,
                            ContentType = "application/pdf",
                            PageOrientation = Orientation.Portrait,
                        };

                        var byteArray = await actionResult.BuildFile(reportController.ControllerContext);

                        var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
                        fileStream.Write(byteArray, 0, byteArray.Length);
                        fileStream.Close();

                        fileTemps.Add(fullPath);
                        fileNames.Add(fullPath);

                        foreach (var attachfile in attachfiles)
                        {
                            fileNames.Add(attachfile.PathNew);
                        }
                        activity2.Stop();
                    }
                }
                activity1.Stop();
            }
            //fileNames.Add(Path.Combine(filePath, $"{datetimeNow}_FileMain.pdf"));

            var bytesOfAllFile = CombineMultiplePdFs(fileNames);

            if (!string.IsNullOrEmpty(bytesOfAllFile))
            {
                byte[] pdfbytearray = JsonConvert.DeserializeObject<byte[]>(bytesOfAllFile);
                if (pdfbytearray.Length > 0)
                {
                    //delete temp pdf file
                    foreach (var fileTemp in fileTemps)
                    {
                        File.Delete(fileTemp);
                    }
                }
                return pdfbytearray;
            }
            else
            {
                throw new Exception("Print mastercard Failed");
            }
            activity.Stop();

            #endregion Combine all print mastercard file
        }

        public List<MoRouting> CheckMORoutingAttachFile(List<string> orderItem)
        {
            var moRoutings = new List<MoRouting>();
            var attachFileMOs = new List<AttachFileMo>();
            try
            {
                //Find MO Rounting
                moRoutings = JsonConvert.DeserializeObject<List<MoRouting>>(_moRoutingAPIRepository.GetMORoutingsBySaleOrders(_factoryCode, JsonConvert.SerializeObject(orderItem), _token));
                if (moRoutings.Count > 0)
                {
                    //Find MO Rounting Where SemiBlk = 1 Or ShipBlk = 1
                    moRoutings = moRoutings.Where(x => ((x.SemiBlk ?? false) || (x.ShipBlk ?? false))).ToList();
                    if (moRoutings.Count > 0)
                    {
                        //Find AttachFile MO
                        attachFileMOs = JsonConvert.DeserializeObject<List<AttachFileMo>>(_attachFileMOAPIRepository.GetAttachFileMOsByOrderItemsAndFactoryCode(_factoryCode, JsonConvert.SerializeObject(moRoutings.Select(x => x.OrderItem)), _token));
                        if (attachFileMOs.Count > 0)
                        {
                            //Find MO Rounting Not In AttachFile MO
                            moRoutings = moRoutings.Where(x => !x.OrderItem.Equals(attachFileMOs.Select(p => p.OrderItem).ToList())).ToList();
                            if (moRoutings.Count > 0)
                            {
                                moRoutings = new List<MoRouting>();
                            }
                        }
                    }
                    else
                    {
                        moRoutings = new List<MoRouting>();
                    }
                }
                else
                {
                    moRoutings = new List<MoRouting>();
                }
                return moRoutings;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public byte[] SaveTextFileWithOutAttachFile(ReportController reportController, List<MoRouting> moRoutings)
        {
            var pmtsConfig = JsonConvert.DeserializeObject<PmtsConfig>(_pMTsConfigAPIRepository.GetPMTsConfigByFactoryName(_factoryCode, "ManageMO_Path", _token));
            var filePath = pmtsConfig != null ? pmtsConfig.FucValue : throw new Exception("Can't get file path.");
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            MemoryStream memoryStream = new MemoryStream();
            TextWriter tw = new StreamWriter(memoryStream);

            //tw.WriteLine("Hello World");
            if (moRoutings.Count > 0)
            {
                moRoutings.ForEach(o => tw.WriteLine(o.OrderItem));
            }
            tw.Flush();
            tw.Close();

            return memoryStream.GetBuffer();
        }

        public async Task<byte[]> SavePDFTagFile(ReportController reportController, PrintMasterCardMOModel printMasterCardMOModel)
        {
            var fileNames = new List<string>();
            var fileTemps = new List<string>();
            var MOsWithoutAttachfiles = new List<MasterCardMO>();
            var MOsSerialize = JsonConvert.SerializeObject(printMasterCardMOModel);
            var originalMOs = JsonConvert.DeserializeObject<PrintMasterCardMOModel>(MOsSerialize);
            var fileNo = 0;
            var MOsNo = 0;
            //var MOsWithoutAttachfiles = printMasterCardMOModel.MasterCardMOs.Where(m => string.IsNullOrEmpty(m.AttchFilesBase64)).ToList();
            var MOsWithAttachfiles = printMasterCardMOModel.MasterCardMOs.Where(m => !string.IsNullOrEmpty(m.AttchFilesBase64)).ToList();
            var pmtsConfig = JsonConvert.DeserializeObject<PmtsConfig>(_pMTsConfigAPIRepository.GetPMTsConfigByFactoryName(_factoryCode, "ManageMO_Path", _token));
            var filePath = pmtsConfig != null ? pmtsConfig.FucValue : throw new Exception("Can't get file path.");
            var datetimeNow = DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss");
            //fileTemps.Add(Path.Combine(filePath, $"{datetimeNow}_FileMain.pdf"));
            var pageSize = _businessGroup ? Rotativa.AspNetCore.Options.Size.A3 : Rotativa.AspNetCore.Options.Size.A4;
            var viewMasterCardMOName = _businessGroup ? "PPCMasterCardTag" : "MasterCardTag";

            //create folder if doesn't exist
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            #region Combine all print mastercard file

            foreach (var masterCardMO in originalMOs.MasterCardMOs)
            {
                MOsNo++;
                if (string.IsNullOrEmpty(masterCardMO.AttchFilesBase64))
                {
                    MOsWithoutAttachfiles.Add(masterCardMO);
                    if (originalMOs.MasterCardMOs.Count == MOsNo)
                    {
                        if (MOsWithoutAttachfiles != null && MOsWithoutAttachfiles.Count > 0)
                        {
                            fileNo++;
                            var GenarateFileName = $"{datetimeNow}_FileMain{fileNo}.pdf";
                            var fullPath = Path.Combine(filePath, GenarateFileName);
                            printMasterCardMOModel.MasterCardMOs = new List<MasterCardMO>();
                            printMasterCardMOModel.MasterCardMOs.AddRange(MOsWithoutAttachfiles);

                            var actionResultWithoutAttachfiles = new ViewAsPdf(viewMasterCardMOName, printMasterCardMOModel)
                            {
                                FileName = Path.Combine(filePath, $"{datetimeNow}_FileMain{fileNo}.pdf"),
                                PageMargins = new Margins(0, 2, 0, 2),
                                PageSize = pageSize,
                                ContentType = "application/pdf",
                                PageOrientation = Orientation.Portrait,
                            };

                            var byteArrayWithoutAttachfiles = await actionResultWithoutAttachfiles.BuildFile(reportController.ControllerContext);

                            var fileStreamWithoutAttachfiles = new FileStream(Path.Combine(filePath, $"{datetimeNow}_FileMain{fileNo}.pdf"), FileMode.Create, FileAccess.Write);
                            fileStreamWithoutAttachfiles.Write(byteArrayWithoutAttachfiles, 0, byteArrayWithoutAttachfiles.Length);
                            fileStreamWithoutAttachfiles.Close();

                            fileTemps.Add(fullPath);
                            fileNames.Add(fullPath);

                            MOsWithoutAttachfiles.Clear();
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(masterCardMO.AttchFilesBase64) || originalMOs.MasterCardMOs.Count == MOsNo)
                {
                    if (MOsWithoutAttachfiles != null && MOsWithoutAttachfiles.Count > 0)
                    {
                        fileNo++;
                        var GenarateFileName = $"{datetimeNow}_FileMain{fileNo}.pdf";
                        var fullPath = Path.Combine(filePath, GenarateFileName);
                        printMasterCardMOModel.MasterCardMOs = new List<MasterCardMO>();
                        printMasterCardMOModel.MasterCardMOs.AddRange(MOsWithoutAttachfiles);

                        var actionResultWithoutAttachfiles = new ViewAsPdf(viewMasterCardMOName, printMasterCardMOModel)
                        {
                            FileName = Path.Combine(filePath, $"{datetimeNow}_FileMain{fileNo}.pdf"),
                            PageMargins = new Margins(0, 2, 0, 2),
                            PageSize = pageSize,
                            ContentType = "application/pdf",
                            PageOrientation = Orientation.Portrait,
                        };

                        var byteArrayWithoutAttachfiles = await actionResultWithoutAttachfiles.BuildFile(reportController.ControllerContext);

                        var fileStreamWithoutAttachfiles = new FileStream(Path.Combine(filePath, $"{datetimeNow}_FileMain{fileNo}.pdf"), FileMode.Create, FileAccess.Write);
                        fileStreamWithoutAttachfiles.Write(byteArrayWithoutAttachfiles, 0, byteArrayWithoutAttachfiles.Length);
                        fileStreamWithoutAttachfiles.Close();

                        fileTemps.Add(fullPath);
                        fileNames.Add(fullPath);

                        MOsWithoutAttachfiles.Clear();
                    }

                    var attachfiles = JsonConvert.DeserializeObject<List<AttachFileMo>>(masterCardMO.AttchFilesBase64);
                    if (attachfiles != null && attachfiles.Count > 0)
                    {
                        var GenarateFileName = $"{datetimeNow}_{masterCardMO.OrderItem}_FileMain.pdf";
                        var fullPath = Path.Combine(filePath, GenarateFileName);
                        printMasterCardMOModel.MasterCardMOs = new List<MasterCardMO>();
                        printMasterCardMOModel.MasterCardMOs.Add(masterCardMO);
                        var actionResult = new ViewAsPdf(viewMasterCardMOName, printMasterCardMOModel)
                        {
                            FileName = fullPath,
                            PageMargins = new Margins(0, 2, 0, 2),
                            PageSize = pageSize,
                            ContentType = "application/pdf",
                            PageOrientation = Orientation.Portrait,
                        };

                        var byteArray = await actionResult.BuildFile(reportController.ControllerContext);

                        var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
                        fileStream.Write(byteArray, 0, byteArray.Length);
                        fileStream.Close();

                        fileTemps.Add(fullPath);
                        fileNames.Add(fullPath);

                        foreach (var attachfile in attachfiles)
                        {
                            fileNames.Add(attachfile.PathNew);
                        }
                    }
                }
            }

            //fileNames.Add(Path.Combine(filePath, $"{datetimeNow}_FileMain.pdf"));

            var bytesOfAllFile = CombineMultiplePdFs(fileNames);

            if (!string.IsNullOrEmpty(bytesOfAllFile))
            {
                byte[] pdfbytearray = JsonConvert.DeserializeObject<byte[]>(bytesOfAllFile);
                if (pdfbytearray.Length > 0)
                {
                    //delete temp pdf file
                    foreach (var fileTemp in fileTemps)
                    {
                        File.Delete(fileTemp);
                    }
                }
                return pdfbytearray;
            }
            else
            {
                throw new Exception("Print mastercard Failed");
            }

            #endregion Combine all print mastercard file
        }

        #region Update TIP data to print mastercardmo

        public void UpdateTipMOData(ref MoDataPrintMastercard moData, PlanningMODataAndMoSpec planningMOData)
        {
            moData.MaterialNo = string.IsNullOrEmpty(planningMOData.Material_No) ? moData.MaterialNo : planningMOData.Material_No;
            moData.Name = string.IsNullOrEmpty(planningMOData.Name) ? moData.Name : planningMOData.Name;
            moData.ItemNote = string.IsNullOrEmpty(planningMOData.Item_Note) ? moData.ItemNote : planningMOData.Item_Note;
            moData.PoNo = string.IsNullOrEmpty(planningMOData.PO_No) ? moData.PoNo : planningMOData.PO_No;
            moData.Batch = string.IsNullOrEmpty(planningMOData.Batch) ? moData.Batch : planningMOData.Batch;
            moData.PC = string.IsNullOrEmpty(planningMOData.PC) ? moData.PC : planningMOData.PC;
            moData.OrderQuant = planningMOData.Order_Quant == null ? moData.OrderQuant : planningMOData.Order_Quant;
            moData.ToleranceOver = planningMOData.Tolerance_Over == null ? moData.ToleranceOver : planningMOData.Tolerance_Over;
            moData.ToleranceUnder = planningMOData.Tolerance_Under == null ? moData.ToleranceUnder : planningMOData.Tolerance_Under;
            moData.TargetQuant = planningMOData.Target_Quant == null ? moData.TargetQuant : planningMOData.Target_Quant;
            moData.DueDate = planningMOData.Due_Date == null ? moData.DueDate : planningMOData.Due_Date;
        }

        public void UpdateTipMOSpec(ref MoSpec moSpec, PlanningMODataAndMoSpec planningMOData)
        {
            moSpec.MaterialNo = string.IsNullOrEmpty(planningMOData.Material_No) ? moSpec.MaterialNo : planningMOData.Material_No;
            moSpec.PartNo = string.IsNullOrEmpty(planningMOData.Part_No) ? moSpec.PartNo : planningMOData.Part_No;
            moSpec.Pc = string.IsNullOrEmpty(planningMOData.PC) ? moSpec.Pc : planningMOData.PC;
            moSpec.SaleOrg = string.IsNullOrEmpty(planningMOData.Sale_Org) ? moSpec.SaleOrg : planningMOData.Sale_Org;
            moSpec.Plant = string.IsNullOrEmpty(planningMOData.Plant) ? moSpec.Plant : planningMOData.Plant;
            moSpec.CustCode = string.IsNullOrEmpty(planningMOData.Cust_Code) ? moSpec.CustCode : planningMOData.Cust_Code;
            moSpec.CusId = string.IsNullOrEmpty(planningMOData.Cus_ID) ? moSpec.CusId : planningMOData.Cus_ID;
            moSpec.CustName = string.IsNullOrEmpty(planningMOData.Cust_Name) ? moSpec.CustName : planningMOData.Cust_Name;
            moSpec.Description = string.IsNullOrEmpty(planningMOData.Description) ? moSpec.Description : planningMOData.Description;
            moSpec.SaleText1 = string.IsNullOrEmpty(planningMOData.Sale_Text1) ? moSpec.SaleText1 : planningMOData.Sale_Text1;
            moSpec.SaleText2 = string.IsNullOrEmpty(planningMOData.Sale_Text2) ? moSpec.SaleText2 : planningMOData.Sale_Text2;
            moSpec.SaleText3 = string.IsNullOrEmpty(planningMOData.Sale_Text3) ? moSpec.SaleText3 : planningMOData.Sale_Text3;
            moSpec.SaleText4 = string.IsNullOrEmpty(planningMOData.Sale_Text4) ? moSpec.SaleText4 : planningMOData.Sale_Text4;
            moSpec.Change = string.IsNullOrEmpty(planningMOData.Change) ? moSpec.Change : planningMOData.Change;
            moSpec.PrintMethod = string.IsNullOrEmpty(planningMOData.Print_Method) ? moSpec.PrintMethod : planningMOData.Print_Method;
            moSpec.TwoPiece = planningMOData.TwoPiece == null ? moSpec.TwoPiece : planningMOData.TwoPiece;
            moSpec.Flute = string.IsNullOrEmpty(planningMOData.Flute) ? moSpec.Flute : planningMOData.Flute;
            moSpec.Code = string.IsNullOrEmpty(planningMOData.Code) ? moSpec.Code : planningMOData.Code;
            moSpec.Board = string.IsNullOrEmpty(planningMOData.Board) ? moSpec.Board : planningMOData.Board;
            moSpec.Gl = string.IsNullOrEmpty(planningMOData.GL) ? moSpec.Gl : planningMOData.GL;
            moSpec.Glweigth = planningMOData.GLWeigth == null ? moSpec.Glweigth : planningMOData.GLWeigth;
            moSpec.Bm = string.IsNullOrEmpty(planningMOData.BM) ? moSpec.Bm : planningMOData.BM;
            moSpec.Bmweigth = planningMOData.BMWeigth == null ? moSpec.Bmweigth : planningMOData.BMWeigth;
            moSpec.Bl = string.IsNullOrEmpty(planningMOData.BL) ? moSpec.Bl : planningMOData.BL;
            moSpec.Blweigth = planningMOData.BLWeigth == null ? moSpec.Blweigth : planningMOData.BLWeigth;
            moSpec.Cm = string.IsNullOrEmpty(planningMOData.CM) ? moSpec.Cm : planningMOData.CM;
            moSpec.Cmweigth = planningMOData.CMWeigth == null ? moSpec.Cmweigth : planningMOData.CMWeigth;
            moSpec.Cl = string.IsNullOrEmpty(planningMOData.CL) ? moSpec.Cl : planningMOData.CL;
            moSpec.Clweigth = planningMOData.CLWeigth == null ? moSpec.Clweigth : planningMOData.CLWeigth;
            moSpec.Dm = string.IsNullOrEmpty(planningMOData.DM) ? moSpec.Dm : planningMOData.DM;
            moSpec.Dmweigth = planningMOData.DMWeigth == null ? moSpec.Dmweigth : planningMOData.DMWeigth;
            moSpec.Dl = string.IsNullOrEmpty(planningMOData.DL) ? moSpec.Dl : planningMOData.DL;
            moSpec.Dlweigth = planningMOData.DLWeigth == null ? moSpec.Dlweigth : planningMOData.DLWeigth;
            moSpec.Wid = planningMOData.Wid == null ? moSpec.Wid : planningMOData.Wid;
            moSpec.Leg = planningMOData.Leg == null ? moSpec.Leg : planningMOData.Leg;
            moSpec.Hig = planningMOData.Hig == null ? moSpec.Hig : planningMOData.Hig;
            moSpec.BoxType = string.IsNullOrEmpty(planningMOData.Box_Type) ? moSpec.BoxType : planningMOData.Box_Type;
            moSpec.RscStyle = string.IsNullOrEmpty(planningMOData.RSC_Style) ? moSpec.RscStyle : planningMOData.RSC_Style;
            moSpec.ProType = string.IsNullOrEmpty(planningMOData.Pro_Type) ? moSpec.ProType : planningMOData.Pro_Type;
            moSpec.JoinType = string.IsNullOrEmpty(planningMOData.JoinType) ? moSpec.JoinType : planningMOData.JoinType;
            moSpec.StatusFlag = string.IsNullOrEmpty(planningMOData.Status_Flag) ? moSpec.StatusFlag : planningMOData.Status_Flag;
            moSpec.PriorityFlag = string.IsNullOrEmpty(planningMOData.Priority_Flag) ? moSpec.PriorityFlag : planningMOData.Priority_Flag;
            moSpec.Wire = planningMOData.Wire == null ? moSpec.Wire : planningMOData.Wire;
            moSpec.OuterJoin = planningMOData.Outer_Join == null ? moSpec.OuterJoin : planningMOData.Outer_Join;
            moSpec.CutSheetLeng = planningMOData.CutSheetLeng == null ? moSpec.CutSheetLeng : planningMOData.CutSheetLeng;
            moSpec.CutSheetWid = planningMOData.CutSheetWid == null ? moSpec.CutSheetWid : planningMOData.CutSheetWid;
            moSpec.SheetArea = planningMOData.Sheet_Area == null ? moSpec.SheetArea : planningMOData.Sheet_Area;
            moSpec.BoxArea = planningMOData.Box_Area == null ? moSpec.BoxArea : planningMOData.Box_Area;
            moSpec.ScoreW1 = planningMOData.ScoreW1 == null ? moSpec.ScoreW1 : planningMOData.ScoreW1;
            moSpec.Scorew2 = planningMOData.Scorew2 == null ? moSpec.Scorew2 : planningMOData.Scorew2;
            moSpec.Scorew3 = planningMOData.Scorew3 == null ? moSpec.Scorew3 : planningMOData.Scorew3;
            moSpec.Scorew4 = planningMOData.Scorew4 == null ? moSpec.Scorew4 : planningMOData.Scorew4;
            moSpec.Scorew5 = planningMOData.Scorew5 == null ? moSpec.Scorew5 : planningMOData.Scorew5;
            moSpec.Scorew6 = planningMOData.Scorew6 == null ? moSpec.Scorew6 : planningMOData.Scorew6;
            moSpec.Scorew7 = planningMOData.Scorew7 == null ? moSpec.Scorew7 : planningMOData.Scorew7;
            moSpec.Scorew8 = planningMOData.Scorew8 == null ? moSpec.Scorew8 : planningMOData.Scorew8;
            moSpec.Scorew9 = planningMOData.Scorew9 == null ? moSpec.Scorew9 : planningMOData.Scorew9;
            moSpec.Scorew10 = planningMOData.Scorew10 == null ? moSpec.Scorew10 : planningMOData.Scorew10;
            moSpec.Scorew11 = planningMOData.Scorew11 == null ? moSpec.Scorew11 : planningMOData.Scorew11;
            moSpec.Scorew12 = planningMOData.Scorew12 == null ? moSpec.Scorew12 : planningMOData.Scorew12;
            moSpec.Scorew13 = planningMOData.Scorew13 == null ? moSpec.Scorew13 : planningMOData.Scorew13;
            moSpec.Scorew14 = planningMOData.Scorew14 == null ? moSpec.Scorew14 : planningMOData.Scorew14;
            moSpec.Scorew15 = planningMOData.Scorew15 == null ? moSpec.Scorew15 : planningMOData.Scorew15;
            moSpec.Scorew16 = planningMOData.Scorew16 == null ? moSpec.Scorew16 : planningMOData.Scorew16;
            moSpec.JointLap = planningMOData.JointLap == null ? moSpec.JointLap : planningMOData.JointLap;
            moSpec.ScoreL2 = planningMOData.ScoreL2 == null ? moSpec.ScoreL2 : planningMOData.ScoreL2;
            moSpec.ScoreL3 = planningMOData.ScoreL3 == null ? moSpec.ScoreL3 : planningMOData.ScoreL3;
            moSpec.ScoreL4 = planningMOData.ScoreL4 == null ? moSpec.ScoreL4 : planningMOData.ScoreL4;
            moSpec.ScoreL5 = planningMOData.ScoreL5 == null ? moSpec.ScoreL5 : planningMOData.ScoreL5;
            moSpec.ScoreL6 = planningMOData.ScoreL6 == null ? moSpec.ScoreL6 : planningMOData.ScoreL6;
            moSpec.ScoreL7 = planningMOData.ScoreL7 == null ? moSpec.ScoreL7 : planningMOData.ScoreL7;
            moSpec.ScoreL8 = planningMOData.ScoreL8 == null ? moSpec.ScoreL8 : planningMOData.ScoreL8;
            moSpec.ScoreL9 = planningMOData.ScoreL9 == null ? moSpec.ScoreL9 : planningMOData.ScoreL9;
            moSpec.Slit = planningMOData.ScoreL9 == null ? moSpec.Slit : planningMOData.Slit;
            moSpec.NoSlot = planningMOData.No_Slot == null ? moSpec.NoSlot : planningMOData.No_Slot;
            moSpec.Bun = planningMOData.Bun == null ? moSpec.Bun : planningMOData.Bun;
            moSpec.BunLayer = planningMOData.BunLayer == null ? moSpec.BunLayer : planningMOData.BunLayer;
            moSpec.LayerPalet = planningMOData.LayerPalet == null ? moSpec.LayerPalet : planningMOData.LayerPalet;
            moSpec.BoxPalet = planningMOData.BoxPalet == null ? moSpec.BoxPalet : planningMOData.BoxPalet;
            moSpec.WeightSh = planningMOData.Weight_Sh == null ? moSpec.WeightSh : planningMOData.Weight_Sh;
            moSpec.WeightBox = planningMOData.Weight_Box == null ? moSpec.WeightBox : planningMOData.Weight_Box;
            moSpec.SparePercen = planningMOData.SparePercen == null ? moSpec.SparePercen : planningMOData.SparePercen;
            moSpec.SpareMax = planningMOData.SpareMax == null ? moSpec.SpareMax : planningMOData.SpareMax;
            moSpec.SpareMin = planningMOData.SpareMin == null ? moSpec.SpareMin : planningMOData.SpareMin;
            moSpec.LeadTime = planningMOData.LeadTime == null ? moSpec.LeadTime : planningMOData.LeadTime;
            moSpec.PieceSet = planningMOData.Piece_Set == null ? moSpec.PieceSet : planningMOData.Piece_Set;
            moSpec.CutSheetWidInch = planningMOData.CutSheetWid_Inch == null ? moSpec.CutSheetWidInch : planningMOData.CutSheetWid_Inch;
            moSpec.CutSheetLengInch = planningMOData.CutSheetLeng_Inch == null ? moSpec.CutSheetLengInch : planningMOData.CutSheetLeng_Inch;
            moSpec.Hardship = planningMOData.Hardship == null ? moSpec.Hardship : planningMOData.Hardship;
            moSpec.Perforate1 = planningMOData.PERFORATOR_1 == null ? moSpec.Perforate1 : planningMOData.PERFORATOR_1;
            moSpec.Perforate2 = planningMOData.PERFORATOR_2 == null ? moSpec.Perforate2 : planningMOData.PERFORATOR_2;
            moSpec.Perforate3 = planningMOData.PERFORATOR_3 == null ? moSpec.Perforate3 : planningMOData.PERFORATOR_3;
            moSpec.Perforate4 = planningMOData.PERFORATOR_4 == null ? moSpec.Perforate4 : planningMOData.PERFORATOR_4;
            moSpec.Perforate5 = planningMOData.PERFORATOR_5 == null ? moSpec.Perforate5 : planningMOData.PERFORATOR_5;
            moSpec.Perforate6 = planningMOData.PERFORATOR_6 == null ? moSpec.Perforate6 : planningMOData.PERFORATOR_6;
            moSpec.Perforate7 = planningMOData.PERFORATOR_7 == null ? moSpec.Perforate7 : planningMOData.PERFORATOR_7;
            moSpec.Perforate8 = planningMOData.PERFORATOR_8 == null ? moSpec.Perforate8 : planningMOData.PERFORATOR_8;
            moSpec.Perforate9 = planningMOData.PERFORATOR_9 == null ? moSpec.Perforate9 : planningMOData.PERFORATOR_9;
            moSpec.Perforate10 = planningMOData.PERFORATOR_10 == null ? moSpec.Perforate10 : planningMOData.PERFORATOR_10;
            moSpec.Perforate11 = planningMOData.PERFORATOR_11 == null ? moSpec.Perforate11 : planningMOData.PERFORATOR_11;
            moSpec.Perforate12 = planningMOData.PERFORATOR_12 == null ? moSpec.Perforate12 : planningMOData.PERFORATOR_12;
            moSpec.Perforate13 = planningMOData.PERFORATOR_13 == null ? moSpec.Perforate13 : planningMOData.PERFORATOR_13;
            moSpec.Perforate14 = planningMOData.PERFORATOR_14 == null ? moSpec.Perforate14 : planningMOData.PERFORATOR_14;
            moSpec.Perforate15 = planningMOData.PERFORATOR_15 == null ? moSpec.Perforate15 : planningMOData.PERFORATOR_15;
            moSpec.Perforate16 = planningMOData.PERFORATOR_16 == null ? moSpec.Perforate16 : planningMOData.PERFORATOR_16;
            moSpec.SaleUom = string.IsNullOrEmpty(planningMOData.Sale_UOM) ? moSpec.SaleUom : planningMOData.Sale_UOM;
            moSpec.BomUom = string.IsNullOrEmpty(planningMOData.BOM_UOM) ? moSpec.BomUom : planningMOData.BOM_UOM;
            moSpec.PalletSize = string.IsNullOrEmpty(planningMOData.PalletSize) ? moSpec.PalletSize : planningMOData.PalletSize;
            moSpec.User = string.IsNullOrEmpty(planningMOData.User) ? moSpec.User : planningMOData.User;
            moSpec.PicPallet = string.IsNullOrEmpty(planningMOData.PicPallet) ? moSpec.PicPallet : planningMOData.PicPallet;
            moSpec.ChangeHistory = string.IsNullOrEmpty(planningMOData.ChangeHistory) ? moSpec.ChangeHistory : planningMOData.ChangeHistory;
            moSpec.CustComment = string.IsNullOrEmpty(planningMOData.CustComment) ? moSpec.CustComment : planningMOData.CustComment;
            moSpec.MaterialComment = string.IsNullOrEmpty(planningMOData.MaterialComment) ? moSpec.MaterialComment : planningMOData.MaterialComment;
            moSpec.JointId = string.IsNullOrEmpty(planningMOData.Joint_ID) ? moSpec.JointId : planningMOData.Joint_ID;
        }

        public void UpdateTipMORouing(ref List<MoRoutingPrintMastercard> moRoutings, List<PlanningRouting> planningRoutings)
        {
            if (planningRoutings != null && planningRoutings.Count > 0)
            {
                var planCodes = planningRoutings.Select(b => b.Plan_Code).ToList();
                moRoutings = moRoutings.Where(mr => planCodes.Contains(mr.PlanCode)).ToList();
                foreach (var planningRouting in planningRoutings)
                {
                    var moRouting = moRoutings.FirstOrDefault(m => m.SeqNo == planningRouting.Seq_No);
                    if (moRouting != null)
                    {//Update moRouting from TIPS
                        moRouting.FactoryCode = string.IsNullOrEmpty(planningRouting.FactoryCode) ? moRouting.FactoryCode : planningRouting.FactoryCode;
                        moRouting.OrderItem = string.IsNullOrEmpty(planningRouting.OrderItem) ? moRouting.OrderItem : planningRouting.OrderItem;
                        moRouting.MatCode = string.IsNullOrEmpty(planningRouting.Mat_Code) ? moRouting.MatCode : planningRouting.Mat_Code;
                        moRouting.PlanCode = string.IsNullOrEmpty(planningRouting.Plan_Code) ? moRouting.PlanCode : planningRouting.Plan_Code;
                        moRouting.Machine = string.IsNullOrEmpty(planningRouting.Machine) ? moRouting.Machine : planningRouting.Machine;
                        moRouting.Speed = planningRouting.Speed == null ? moRouting.Speed : planningRouting.Speed;
                        moRouting.ColourCount = planningRouting.Colour_Count == null ? moRouting.ColourCount : planningRouting.Colour_Count;
                        moRouting.PlateNo = string.IsNullOrEmpty(planningRouting.Plate_No) ? moRouting.PlateNo : planningRouting.Plate_No;
                        moRouting.MylaNo = string.IsNullOrEmpty(planningRouting.Myla_No) ? moRouting.MylaNo : planningRouting.Myla_No;
                        moRouting.PaperWidth = planningRouting.Paper_Width != null ? moRouting.PaperWidth : planningRouting.Paper_Width;
                        moRouting.CutNo = planningRouting.Cut_No != null ? moRouting.CutNo : planningRouting.Cut_No;
                        moRouting.Trim = planningRouting.Trim != null ? moRouting.Trim : planningRouting.Trim;
                        moRouting.SheetInLeg = planningRouting.Sheet_in_Leg != null ? moRouting.SheetInLeg : planningRouting.Sheet_in_Leg;
                        moRouting.SheetInWid = planningRouting.Sheet_in_Wid != null ? moRouting.SheetInWid : planningRouting.Sheet_in_Wid;
                        //moRouting.WeightOut = planningRouting.WeightOut;
                        moRouting.NoOpenIn = planningRouting.No_Open_in != null ? moRouting.NoOpenIn : planningRouting.No_Open_in;
                        moRouting.NoOpenOut = planningRouting.No_Open_out != null ? moRouting.NoOpenOut : planningRouting.No_Open_out;
                        moRouting.Color1 = string.IsNullOrEmpty(planningRouting.Color1) ? moRouting.Color1 : planningRouting.Color1;
                        moRouting.Shade1 = string.IsNullOrEmpty(planningRouting.Shade1) ? moRouting.Shade1 : planningRouting.Shade1;
                        moRouting.Color2 = string.IsNullOrEmpty(planningRouting.Color2) ? moRouting.Color2 : planningRouting.Color2;
                        moRouting.Shade2 = string.IsNullOrEmpty(planningRouting.Shade2) ? moRouting.Shade2 : planningRouting.Shade2;
                        moRouting.Color3 = string.IsNullOrEmpty(planningRouting.Color3) ? moRouting.Color3 : planningRouting.Color3;
                        moRouting.Shade3 = string.IsNullOrEmpty(planningRouting.Shade3) ? moRouting.Shade3 : planningRouting.Shade3;
                        moRouting.Color4 = string.IsNullOrEmpty(planningRouting.Color4) ? moRouting.Color4 : planningRouting.Color4;
                        moRouting.Shade4 = string.IsNullOrEmpty(planningRouting.Shade4) ? moRouting.Shade4 : planningRouting.Shade4;
                        moRouting.Color5 = string.IsNullOrEmpty(planningRouting.Color5) ? moRouting.Color5 : planningRouting.Color5;
                        moRouting.Shade5 = string.IsNullOrEmpty(planningRouting.Shade5) ? moRouting.Shade5 : planningRouting.Shade5;
                        moRouting.Color6 = string.IsNullOrEmpty(planningRouting.Color6) ? moRouting.Color6 : planningRouting.Color6;
                        moRouting.Shade6 = string.IsNullOrEmpty(planningRouting.Shade6) ? moRouting.Shade6 : planningRouting.Shade6;
                        moRouting.Color7 = string.IsNullOrEmpty(planningRouting.Color7) ? moRouting.Color7 : planningRouting.Color7;
                        moRouting.Shade7 = string.IsNullOrEmpty(planningRouting.Shade7) ? moRouting.Shade7 : planningRouting.Shade7;
                        moRouting.SemiBlk = planningRouting.Semi_Blk != null ? moRouting.SemiBlk : planningRouting.Semi_Blk;
                        moRouting.ShipBlk = planningRouting.Ship_Blk != null ? moRouting.ShipBlk : planningRouting.Ship_Blk;
                        moRouting.BlockNo = string.IsNullOrEmpty(planningRouting.Block_No) ? moRouting.BlockNo : planningRouting.Block_No;
                        moRouting.RemarkInprocess = string.IsNullOrEmpty(planningRouting.Remark_Inprocess) ? moRouting.RemarkInprocess : planningRouting.Remark_Inprocess;
                        moRouting.ScoreType = string.IsNullOrEmpty(planningRouting.Score_type) ? moRouting.ScoreType : planningRouting.Score_type;
                        //moRouting.ScoreGap = planningRouting.Score_Gap != null ? moRouting.ScoreGap : planningRouting.Score_Gap;
                    }
                    else
                    {//Add Routing Tips
                        moRoutings.Add(new MoRoutingPrintMastercard
                        {
                            FactoryCode = planningRouting.FactoryCode,
                            OrderItem = planningRouting.OrderItem,
                            SeqNo = planningRouting.Seq_No,
                            MatCode = planningRouting.Mat_Code,
                            PlanCode = planningRouting.Plan_Code,
                            Machine = planningRouting.Machine,
                            Speed = planningRouting.Speed,
                            ColourCount = planningRouting.Colour_Count,
                            PlateNo = planningRouting.Plate_No,
                            MylaNo = planningRouting.Myla_No,
                            PaperWidth = planningRouting.Paper_Width,
                            CutNo = planningRouting.Cut_No,
                            Trim = planningRouting.Trim,
                            SheetInLeg = planningRouting.Sheet_in_Leg,
                            SheetInWid = planningRouting.Sheet_in_Wid,
                            //WeightOut = planningRouting.WeightOut,
                            NoOpenIn = planningRouting.No_Open_in,
                            NoOpenOut = planningRouting.No_Open_out,
                            Color1 = planningRouting.Color1,
                            Shade1 = planningRouting.Shade1,
                            Color2 = planningRouting.Color2,
                            Shade2 = planningRouting.Shade2,
                            Color3 = planningRouting.Color3,
                            Shade3 = planningRouting.Shade3,
                            Color4 = planningRouting.Color4,
                            Shade4 = planningRouting.Shade4,
                            Color5 = planningRouting.Color5,
                            Shade5 = planningRouting.Shade5,
                            Color6 = planningRouting.Color6,
                            Shade6 = planningRouting.Shade6,
                            Color7 = planningRouting.Color7,
                            Shade7 = planningRouting.Shade7,
                            SemiBlk = planningRouting.Semi_Blk,
                            ShipBlk = planningRouting.Ship_Blk,
                            BlockNo = planningRouting.Block_No,
                            RemarkInprocess = planningRouting.Remark_Inprocess,
                            ScoreType = planningRouting.Score_type,
                            //ScoreGap = planningRouting.Score_Gap,
                        });
                    }
                }

                moRoutings = moRoutings.OrderBy(x => x.SeqNo).ToList();
                var seqNo = 1;
                foreach (var mo in moRoutings)
                {
                    mo.SeqNo = seqNo;
                    seqNo++;
                }
            }
            else
            {
                moRoutings = new List<MoRoutingPrintMastercard>();
            }
        }

        #endregion Update TIP data to print mastercardmo

        // replace “FSC”, “H01”, “H02”, “H03”
        public static string clean(string s)
        {
            StringBuilder sb = new StringBuilder(s);
            sb.Replace("FSC/", "");
            sb.Replace("H01/", "");
            sb.Replace("H02/", "");
            sb.Replace("H03/", "");
            sb.Replace("FSC,", "");
            sb.Replace("H01,", "");
            sb.Replace("H02,", "");
            sb.Replace("H03,", "");
            return sb.ToString();
        }

        public void UpdateMaterCardPrintedByOrderItems(List<string> orderItem)
        {
            _moDataAPIRepository.UpdatePrintedMODataByOrderItems(_factoryCode, _username, JsonConvert.SerializeObject(orderItem), _token);
        }
    }
}