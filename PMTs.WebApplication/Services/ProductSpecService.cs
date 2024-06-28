using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
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
    public class ProductSpecService : IProductSpecService
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHostingEnvironment _environment;
        private readonly IMapper mapper;
        private readonly ITransactionsDetailAPIRepository _transactionsDetailAPIRepository;
        private readonly IPMTsConfigAPIRepository _pmtsConfigAPIRepository;
        private readonly IPaperGradeAPIRepository _paperGradeAPIRepository;
        private readonly IPlantViewAPIRepository _plantViewAPIRepository;
        private readonly ISalesViewAPIRepository _salesViewAPIRepository;
        private readonly IBoardCombineAPIRepository _boardCombineAPIRepository;
        private readonly IBoardCombineAccAPIRepository _boardCombineAccAPIRepository;
        private readonly IMapCostAPIRepository _mapCostAPIRepository;
        private readonly IBoardSpecAPIRepository _boardSpecAPIRepository;
        private readonly IHoneyPaperAPIRepository _honeyPaperAPIRepository;
        private readonly IFluteAPIRepository _fluteAPIRepository;
        private readonly IFluteTrAPIRepository _fluteTrAPIRepository;
        private readonly IFormulaAPIRepository _formulaAPIRepository;
        private readonly IBoardAlternativeAPIRepository _boardAlternativeAPIRepository;
        private readonly IBoardAltSpecAPIRepository _boardAltSpecAPIRepository;
        private readonly IBoardUseAPIRepository _boardUseAPIRepository;
        private readonly IMasterDataAPIRepository _masterDataAPIRepository;
        private readonly IExtensionService _extensionService;
        private readonly INewProductService _newProductService;
        private readonly IBoardCostAPIRepository _boardCostAPIRepository;
        private readonly ICoatingAPIRepository _coatingAPIRepository;
        private readonly IAdditiveAPIRepository _additiveAPIRepository;

        private readonly string _username;
        private readonly string _saleOrg;
        private readonly string _factoryCode;
        private readonly string _token;

        public ProductSpecService(IHttpContextAccessor httpContextAccessor,
            IExtensionService extensionService,
            ITransactionsDetailAPIRepository transactionsDetailAPIRepository,
            IPMTsConfigAPIRepository pmtsConfigAPIRepository,
            IPaperGradeAPIRepository paperGradeAPIRepository,
            IPlantViewAPIRepository plantViewAPIRepository,
            ISalesViewAPIRepository salesViewAPIRepository,
            IBoardCombineAPIRepository boardCombineAPIRepository,
            IBoardCombineAccAPIRepository boardCombineAccAPIRepository,
            IMapCostAPIRepository mapCostAPIRepository,
            IBoardSpecAPIRepository boardSpecAPIRepository,
            IHoneyPaperAPIRepository honeyPaperAPIRepository,
            IFluteAPIRepository fluteAPIRepository,
            IFluteTrAPIRepository fluteTrAPIRepository,
            IFormulaAPIRepository formulaAPIRepository,
            IBoardAlternativeAPIRepository boardAlternativeAPIRepository,
            IBoardAltSpecAPIRepository boardAltSpecAPIRepository,
            IBoardUseAPIRepository boardUseAPIRepository,
            IMasterDataAPIRepository masterDataAPIRepository,
            INewProductService newProductService,
            ICoatingAPIRepository coatingAPIRepository,
            IBoardCostAPIRepository boardCostAPIRepository,
            IAdditiveAPIRepository additiveAPIRepository,
            IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;

            _extensionService = extensionService;
            _newProductService = newProductService;

            _transactionsDetailAPIRepository = transactionsDetailAPIRepository;
            _pmtsConfigAPIRepository = pmtsConfigAPIRepository;
            _paperGradeAPIRepository = paperGradeAPIRepository;
            _plantViewAPIRepository = plantViewAPIRepository;
            _salesViewAPIRepository = salesViewAPIRepository;
            _boardCombineAPIRepository = boardCombineAPIRepository;
            _boardCombineAccAPIRepository = boardCombineAccAPIRepository;
            _mapCostAPIRepository = mapCostAPIRepository;
            _boardSpecAPIRepository = boardSpecAPIRepository;
            _honeyPaperAPIRepository = honeyPaperAPIRepository;
            _fluteAPIRepository = fluteAPIRepository;
            _fluteTrAPIRepository = fluteTrAPIRepository;
            _formulaAPIRepository = formulaAPIRepository;
            _boardAlternativeAPIRepository = boardAlternativeAPIRepository;
            _boardAltSpecAPIRepository = boardAltSpecAPIRepository;
            _boardUseAPIRepository = boardUseAPIRepository;
            _masterDataAPIRepository = masterDataAPIRepository;
            _boardCostAPIRepository = boardCostAPIRepository;
            _coatingAPIRepository = coatingAPIRepository;
            _additiveAPIRepository = additiveAPIRepository;

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

        private void GetProductSpecDataList(ref TransactionDataModel transactionDataModel)
        {
            transactionDataModel.modelProductSpec.BoardLists = new List<SearchBoardAlt>();
            transactionDataModel.modelProductSpec.SearchBoardAltLists = new List<SearchBoardAlt>();
            transactionDataModel.modelProductSpec.SearchBoardAltLists = JsonConvert.DeserializeObject<List<SearchBoardAlt>>(_boardCombineAPIRepository.GetBoardCombineListSearch(_factoryCode, _token));
            transactionDataModel.modelProductSpec.Coating = new List<Coating>();
            transactionDataModel.modelProductSpec.Coating = JsonConvert.DeserializeObject<List<Coating>>(_coatingAPIRepository.GetCoatingByMaterialNo(_factoryCode, transactionDataModel.MaterialNo, _token));
            transactionDataModel.modelProductSpec.Additive = new List<Additive>();
            transactionDataModel.modelProductSpec.Additive = JsonConvert.DeserializeObject<List<Additive>>(_additiveAPIRepository.GetAdditiveList(_factoryCode, _token));

            // Bind Data for ProductSpec
            transactionDataModel.modelProductSpec.BoardLists = transactionDataModel.modelProductSpec.SearchBoardAltLists;

            if (transactionDataModel.modelCategories.FormGroup == "CG")
            {
                transactionDataModel.modelProductSpec.BoardLists = transactionDataModel.modelProductSpec.BoardLists.Where(b => b.Flute == "CP").ToList();
                transactionDataModel.modelProductSpec.DLists = transactionDataModel.modelProductSpec.BoardLists.GroupBy(b => b.Thickness).Select(c => c.Key).OrderBy(c => c.Value).ToList();
            }
            else if (transactionDataModel.modelCategories.FormGroup == "HC")
            {
                transactionDataModel.modelProductSpec.BoardLists = transactionDataModel.modelProductSpec.BoardLists.Where(b => b.Flute.Contains("H") && !b.Board.Contains("GL3")).ToList();
            }
            else if (transactionDataModel.modelCategories.FormGroup == "HB")
            {
                transactionDataModel.modelProductSpec.BoardLists = transactionDataModel.modelProductSpec.BoardLists.Where(b => b.Flute.Contains("H") && b.Board.Contains("GL3")).ToList();
            }
            //transactionDataModel.modelProductSpec.DLists = transactionDataModel.modelProductSpec.BoardLists.GroupBy(b => b.Thickness).Select(c => c.Key).OrderBy(c => c.Value).ToList();
            if (transactionDataModel.modelProductSpec.DLists == null)
            {
                transactionDataModel.modelProductSpec.DLists = transactionDataModel.modelProductSpec.BoardLists.Select(x => x.Thickness).ToList();
            }
            transactionDataModel.modelProductSpec.HLists = transactionDataModel.modelProductSpec.BoardLists.GroupBy(b => b.Height).Select(c => c.Key).ToList();
            transactionDataModel.modelProductSpec.CellSizeLists = transactionDataModel.modelProductSpec.BoardLists.GroupBy(b => b.JoinSize).Select(c => c.Key).ToList();
        }


        public void GetProductSpec(ref TransactionDataModel transactionDataModel, string action)
        {
            var transactionDataModelSession = SessionExtentions.GetSession<TransactionDataModel>(_httpContextAccessor.HttpContext.Session, "TransactionDataModel");

            transactionDataModelSession.modelProductSpec.Size2Piece = Convert.ToInt32(JsonConvert.DeserializeObject<PmtsConfig>(_pmtsConfigAPIRepository.GetPMTsConfigByFactoryName(_factoryCode, "Size_2Piece", _token)).FucValue);

            if (transactionDataModelSession.modelProductSpec == null)
            {
                transactionDataModelSession.modelProductSpec = new ProductSpecViewModel();
                GetDataToProductSpectViewModel(ref transactionDataModelSession);
            }
            else
            {
                if (transactionDataModelSession.modelProductSpec.WeightBox == null)
                {
                    GetDataToProductSpectViewModel(ref transactionDataModelSession);
                }

                if (transactionDataModelSession.RealEventFlag == "Copy")
                {
                    if (transactionDataModelSession.modelProductSpec.Code != null)
                    {
                        var mapcost = JsonConvert.DeserializeObject<MapCost>(_mapCostAPIRepository.GetCostField(_factoryCode, transactionDataModelSession.modelCategories.HierarchyLV2.Trim(), transactionDataModelSession.modelCategories.HierarchyLV3.Trim(), transactionDataModelSession.modelCategories.HierarchyLV4.Trim(), _token));
                        transactionDataModelSession.modelProductSpec.costField = mapcost == null ? transactionDataModelSession.modelProductSpec.costField : mapcost.CostField;
                        var cost = JsonConvert.DeserializeObject<Cost>(_boardCombineAccAPIRepository.GetCost(_factoryCode, transactionDataModelSession.modelProductSpec.Code, transactionDataModelSession.modelProductSpec.costField, _token));
                        if (cost != null)
                        {
                            transactionDataModelSession.modelProductSpec.CostPerTon = cost.CostPerTon == null ? 0 : Convert.ToDecimal(cost.CostPerTon);
                        }
                    }
                    transactionDataModelSession.modelProductSpec.SAPStatus = false;
                    transactionDataModelSession.SapStatus = false;
                }

                if (transactionDataModelSession.modelProductSpec.CapImg == true)
                {
                    transactionDataModelSession.modelProductSpec.PrintMaster = "";
                    transactionDataModelSession.modelProductSpec.PrintMasterPath = "";
                }

                if (transactionDataModelSession.modelProductInfo.PLANTCODE != null && transactionDataModelSession.modelProductSpec.CostOEM == null)
                {
                    if (transactionDataModelSession.modelProductInfo.MatOursource == "")
                        transactionDataModelSession.modelProductInfo.MatOursource = transactionDataModelSession.MaterialNo;

                    var OEM = JsonConvert.DeserializeObject<PlantView>(_plantViewAPIRepository.GetPlantViewByMaterialNo(transactionDataModelSession.modelProductInfo.PLANTCODE, transactionDataModelSession.modelProductInfo.MatOursource, _token));
                    if (OEM != null)
                    {
                        if (OEM.StdTotalCost != 0)
                        {
                            transactionDataModelSession.modelProductSpec.CostOEM = Convert.ToDecimal(OEM.StdTotalCost);
                        }
                        else
                        {
                            transactionDataModelSession.modelProductSpec.CostOEM = Convert.ToDecimal(OEM.StdMovingCost);
                        }
                    }
                }
            }

            if (transactionDataModelSession.modelProductSpec.costField == null || transactionDataModelSession.modelProductSpec.costField == "")
            {
                // Set HierarchyLV3 for Categories
                transactionDataModelSession.modelCategories.HierarchyLV3 = transactionDataModelSession.modelCategories.HierarchyLV3 == null ? "OOO" : transactionDataModelSession.modelCategories.HierarchyLV3.Trim();
                var mapcost = JsonConvert.DeserializeObject<MapCost>(_mapCostAPIRepository.GetCostField(_factoryCode, transactionDataModelSession.modelCategories.HierarchyLV2.Trim(), transactionDataModelSession.modelCategories.HierarchyLV3.Trim(), transactionDataModelSession.modelCategories.HierarchyLV4.Trim(), _token));
                transactionDataModelSession.modelProductSpec.costField = mapcost != null ? mapcost.CostField : "";
            }
            else if (transactionDataModelSession.modelProductSpec.CostPerTon == null)
            {
                var cost = JsonConvert.DeserializeObject<Cost>(_boardCombineAccAPIRepository.GetCost(_factoryCode, transactionDataModelSession.modelProductSpec.Code, transactionDataModelSession.modelProductSpec.costField, _token));
                if (cost != null)
                {
                    transactionDataModelSession.modelProductSpec.CostPerTon = cost.CostPerTon == null ? 0 : Math.Round(Convert.ToDecimal(cost.CostPerTon), 2);
                }
            }
            transactionDataModelSession.modelProductSpec.Hierarchy =
                "03" +
                (transactionDataModelSession.modelCategories.HierarchyLV2 != null ?
                    transactionDataModelSession.modelCategories.HierarchyLV2.Trim() :
                    ""
                ) +
                (transactionDataModelSession.modelCategories.HierarchyLV3 != null ?
                    transactionDataModelSession.modelCategories.HierarchyLV3.Trim() :
                    "") +
                (transactionDataModelSession.modelCategories.HierarchyLV4 != null ?
                    transactionDataModelSession.modelCategories.HierarchyLV4.Trim() :
                    "") +
                transactionDataModelSession.modelProductSpec.Code;
            _newProductService.SetTransactionStatus(ref transactionDataModelSession, "ProductSpec");
            //transactionDataModel.modelProductSpec.Coating = transactionDataModelSession.modelProductSpec.Coating;
            transactionDataModel = transactionDataModelSession;
            GetProductSpecDataList(ref transactionDataModel);
            SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "TransactionDataModel", transactionDataModelSession);
        }

        private void GetDataToProductSpectViewModel(ref TransactionDataModel transactionDataModel)
        {
            var master = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNo(_factoryCode, transactionDataModel.MaterialNo, _token));
            transactionDataModel.Id = master.Id;

            //GetProductSpecDataList(ref transactionDataModel);
            transactionDataModel.modelProductSpec.MaterialNo = transactionDataModel.MaterialNo;
            transactionDataModel.modelProductSpec.SAPStatus = master.SapStatus;

            transactionDataModel.modelProductSpec.No_Slot = transactionDataModel.modelCategories.FormGroup == "AC" ? 1 : 0;
            transactionDataModel.modelProductSpec.Code = "";
            transactionDataModel.modelProductSpec.Flute = "";
            transactionDataModel.modelProductSpec.Board = "";
            transactionDataModel.modelProductSpec.Weight = 0;
            transactionDataModel.modelProductSpec.CostPerTon = 0;
            transactionDataModel.modelProductSpec.Hierarchy = "";

            transactionDataModel.modelProductSpec.Priority = 1;

            transactionDataModel.modelProductSpec.Leg = 0;
            transactionDataModel.modelProductSpec.Wid = 0;
            transactionDataModel.modelProductSpec.Hig = 0;

            transactionDataModel.modelProductSpec.ScoreL8 = 0;
            //transactionDataModel.modelProductSpec.ScoreL9 = transactionDataModel.modelProductSpec.Slit;
            transactionDataModel.modelProductSpec.ScoreL6 = 0;
            transactionDataModel.modelProductSpec.ScoreL7 = 0;

            transactionDataModel.modelProductSpec.JointLap = 0;
            transactionDataModel.modelProductSpec.ScoreL2 = 0;
            transactionDataModel.modelProductSpec.ScoreL3 = 0;
            transactionDataModel.modelProductSpec.ScoreL4 = 0;
            transactionDataModel.modelProductSpec.ScoreL5 = 0;

            transactionDataModel.modelProductSpec.ScoreW1 = 0;
            transactionDataModel.modelProductSpec.Scorew2 = 0;
            transactionDataModel.modelProductSpec.Scorew3 = 0;
            transactionDataModel.modelProductSpec.Scorew4 = 0;
            transactionDataModel.modelProductSpec.Scorew5 = 0;
            transactionDataModel.modelProductSpec.Scorew6 = 0;
            transactionDataModel.modelProductSpec.Scorew7 = 0;
            transactionDataModel.modelProductSpec.Scorew8 = 0;
            transactionDataModel.modelProductSpec.Scorew9 = 0;
            transactionDataModel.modelProductSpec.Scorew10 = 0;
            transactionDataModel.modelProductSpec.Scorew11 = 0;
            transactionDataModel.modelProductSpec.Scorew12 = 0;
            transactionDataModel.modelProductSpec.Scorew13 = 0;
            transactionDataModel.modelProductSpec.Scorew14 = 0;
            transactionDataModel.modelProductSpec.Scorew15 = 0;
            transactionDataModel.modelProductSpec.Scorew16 = 0;
            transactionDataModel.modelProductSpec.CutSheetWid = 0;
            transactionDataModel.modelProductSpec.CutSheetLeng = 0;

            transactionDataModel.modelProductSpec.SheetArea = 0;
            transactionDataModel.modelProductSpec.BoxArea = 0;
            transactionDataModel.modelProductSpec.WeightSh = 0;
            transactionDataModel.modelProductSpec.WeightBox = 0;
            transactionDataModel.modelProductSpec.WeightBoxInit = 0;
            transactionDataModel.modelProductSpec.FlagRouting = 0;
            transactionDataModel.modelProductSpec.Flag = 0;
            transactionDataModel.modelProductSpec.GLWid = false;
            transactionDataModel.modelProductSpec.GLTail = false;

            //Error
            transactionDataModel.modelProductSpec.Slit = transactionDataModel.modelCategories.FormGroup == "AC" ? 0 : Convert.ToInt32(JsonConvert.DeserializeObject<PmtsConfig>(_pmtsConfigAPIRepository.GetSlit(_factoryCode, _token)).FucValue);
            transactionDataModel.modelProductSpec.ScoreL9 = transactionDataModel.modelProductSpec.Slit;
        }

        public int chkPriority(ProductSpecViewModel model)
        {
            TransactionDataModel modelx = new TransactionDataModel();
            modelx = SessionExtentions.GetSession<TransactionDataModel>(_httpContextAccessor.HttpContext.Session, "TransactionDataModel");
            var dup = 0;
            var bName = GetBoardName(model);
            if (bName == model.Board.Trim())
            {
                dup = 100;
            }
            else
            {

                var List = modelx.modelProductSpec.BoardAlt.Where(b => (b.MaterialNo == modelx.MaterialNo && b.Priority == model.Priority) || (b.MaterialNo == modelx.MaterialNo && b.BoardName == bName)).ToList();

                if (List.Count == 0)
                    dup = modelx.modelProductSpec.BoardAlt.Count + 2;
                else
                    dup = 99;
            }
            return dup;
        }

        public string GetBoardName(ProductSpecViewModel board)
        {
            var bName = "";
            if (board.GL != "" && board.GL != null && board.GL != "undefined")
                bName = board.GL;
            else
                board.GL = "";
            //////////////////////////////////////////////
            if (board.BM != "" && board.BM != null && board.BM != "undefined")
                bName = bName + "/" + board.BM;
            else
                board.BM = "";
            //////////////////////////////////////////////
            if (board.BL != "" && board.BL != null && board.BL != "undefined")
                bName = bName + "/" + board.BL;
            else
                board.BL = "";
            //////////////////////////////////////////////
            if (board.CM != "" && board.CM != null && board.CM != "undefined")
                bName = bName + "/" + board.CM;
            else
                board.CM = "";
            //////////////////////////////////////////////
            if (board.CL != "" && board.CL != null && board.CL != "undefined")
                bName = bName + "/" + board.CL;
            else
                board.CL = "";
            //////////////////////////////////////////////
            if (board.DM != "" && board.DM != null && board.DM != "undefined")
                bName = bName + "/" + board.DM;
            else
                board.DM = "";
            //////////////////////////////////////////////
            if (board.DL != "" && board.DL != null && board.DL != "undefined")
                bName = bName + "/" + board.DL;
            else
                board.DL = "";

            return bName;
        }

        //public string GetBoardKIWI(string boardCombine)
        //{
        //    var bKiwi = "";
        //    var flu = "";

        //    string[] ArrarBoard = boardCombine.Split("/");

        //    var paperGrades = JsonConvert.DeserializeObject<List<PaperGrade>>(_paperGradeAPIRepository.GetPaperGradeList(_factoryCode));

        //    if (ArrarBoard.Count() > 0)
        //    {
        //        if (ArrarBoard[0].Length == 1)
        //            flu = ArrarBoard[0] + " ";
        //        else
        //            flu = ArrarBoard[0];
        //    }

        //    bKiwi = flu;

        //    if (ArrarBoard.Count() > 1)
        //    {
        //        var board1 = paperGrades.Where(p => p.Grade == ArrarBoard[1].Trim()).FirstOrDefault();
        //        if (board1 != null)
        //            bKiwi = bKiwi + "/" + board1.Kiwi + board1.BasicWeight;
        //        else
        //            bKiwi = bKiwi + "/" + ArrarBoard[1].Trim();
        //    }
        //    if (ArrarBoard.Count() > 2)
        //    {
        //        var board2 = paperGrades.Where(p => p.Grade == ArrarBoard[2].Trim()).FirstOrDefault();
        //        if (board2 != null)
        //            bKiwi = bKiwi + "/" + board2.Kiwi + board2.BasicWeight;
        //        else
        //            bKiwi = bKiwi + "/" + ArrarBoard[2].Trim();
        //    }
        //    if (ArrarBoard.Count() > 3)
        //    {
        //        var board3 = paperGrades.Where(p => p.Grade == ArrarBoard[3].Trim()).FirstOrDefault();
        //        if (board3 != null)
        //            bKiwi = bKiwi + "/" + board3.Kiwi + board3.BasicWeight;
        //        else
        //            bKiwi = bKiwi + "/" + ArrarBoard[3].Trim();
        //    }
        //    if (ArrarBoard.Count() > 4)
        //    {
        //        var board4 = paperGrades.Where(p => p.Grade == ArrarBoard[4].Trim()).FirstOrDefault();
        //        if (board4 != null)
        //            bKiwi = bKiwi + "/" + board4.Kiwi + board4.BasicWeight;
        //        else
        //            bKiwi = bKiwi + "/" + ArrarBoard[4].Trim();
        //    }
        //    if (ArrarBoard.Count() > 5)
        //    {
        //        var board5 = paperGrades.Where(p => p.Grade == ArrarBoard[5].Trim()).FirstOrDefault();
        //        if (board5 != null)
        //            bKiwi = bKiwi + "/" + board5.Kiwi + board5.BasicWeight;
        //        else
        //            bKiwi = bKiwi + "/" + ArrarBoard[5].Trim();
        //    }
        //    if (ArrarBoard.Count() > 6)
        //    {
        //        var board6 = paperGrades.Where(p => p.Grade == ArrarBoard[6].Trim()).FirstOrDefault();
        //        if (board6 != null)
        //            bKiwi = bKiwi + "/" + board6.Kiwi + board6.BasicWeight;
        //        else
        //            bKiwi = bKiwi + "/" + ArrarBoard[6].Trim();
        //    }
        //    if (ArrarBoard.Count() > 7)
        //    {
        //        var board7 = paperGrades.Where(p => p.Grade == ArrarBoard[7].Trim()).FirstOrDefault();
        //        if (board7 != null)
        //            bKiwi = bKiwi + "/" + board7.Kiwi + board7.BasicWeight;
        //        else
        //            bKiwi = bKiwi + "/" + ArrarBoard[7].Trim();
        //    }

        //    return bKiwi;
        //}

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

        public List<BoardViewModel> GetBoardAlt(string prefix, string flute)
        {
            var boardList = JsonConvert.DeserializeObject<List<BoardCombine>>(_boardCombineAPIRepository.GetBoardByFlute(_factoryCode, flute, _token));
            var boardViewList = mapper.Map<List<BoardCombine>, List<BoardViewModel>>(boardList);

            return boardViewList;
        }

        public List<SearchBoardAlt> SearchBoardAlt(string flute)
        {
            var boardList = JsonConvert.DeserializeObject<List<SearchBoardAlt>>(_boardCombineAPIRepository.GetBoardByFlute(_factoryCode, flute, _token));

            return boardList;
        }

        public List<BoardSpecWeight> GetBoardSpec(string code, string flute, string PlantCode)
        {
            //var boardSpec = JsonConvert.DeserializeObject<List<BoardSpecStation>>(_boardSpecAPIRepository.GetBoardSpecStationByBoardId(_factoryCode, code));
            var boardSpec = JsonConvert.DeserializeObject<List<BoardSpecWeight>>(_boardCombineAPIRepository.GetBoardSpecWeightByCode(_factoryCode, code, _token));
            var factoryCode = PlantCode == null ? _factoryCode : PlantCode;
            var flu = JsonConvert.DeserializeObject<List<FluteTr>>(_fluteTrAPIRepository.GetFluteTrByFlute(_factoryCode, flute, _token));
            int i = 0, j = 0;

            if (boardSpec.Count > flu.Count)
                j = flu.Count;
            else if (flu.Count > boardSpec.Count)
                j = boardSpec.Count;
            else
                j = boardSpec.Count;

            foreach (var item in boardSpec)
            {
                item.Station = flu[i].Station;
                i++;
                if (i == j)
                    break;
            }
            return boardSpec;
        }

        public BoardCombine GetBoardByCode(string code)
        {
            var boardList = JsonConvert.DeserializeObject<BoardCombine>(_boardCombineAPIRepository.GetBoardByCode(_factoryCode, code, _token));

            return boardList;
        }

        public ProductSpecViewModel AddBoardAlt(ProductSpecViewModel model)
        {
            TransactionDataModel modelx;
            modelx = SessionExtentions.GetSession<TransactionDataModel>(_httpContextAccessor.HttpContext.Session, "TransactionDataModel");
            model.MaterialNo = modelx.MaterialNo;
            model.BoardName = GetBoardName(model);

            if (modelx.modelProductSpec.BoardAlt == null)
                modelx.modelProductSpec.BoardAlt = new List<BoardAltViewModel>();

            var flu = "";
            if (model.Flute.Length == 1)
                flu = model.Flute + " /";
            else
                flu = model.Flute + "/";

            modelx.modelProductSpec.BoardAlt.Add(new BoardAltViewModel
            {
                MaterialNo = model.MaterialNo,
                BoardName = model.BoardName,
                BoardKiwi = _newProductService.GetBoardKIWI(flu + model.BoardName),
                Priority = model.Priority,
                Active = model.Active,
                Flute = model.Flute,
                GL = model.GL,
                BM = model.BM,
                BL = model.BL,
                CM = model.CM,
                CL = model.CL,
                DM = model.DM,
                DL = model.DL
            });
            modelx.modelProductSpec.chkPrior = modelx.modelProductSpec.BoardAlt.Count + 1;
            SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "TransactionDataModel", modelx);

            return modelx.modelProductSpec;
        }

        public ProductSpecViewModel RemoveBoardAlt(int prior)
        {
            TransactionDataModel modelx;
            modelx = SessionExtentions.GetSession<TransactionDataModel>(_httpContextAccessor.HttpContext.Session, "TransactionDataModel");

            int seqNumber = 1;
            modelx.modelProductSpec.BoardAlt.RemoveAt(prior - 1);
            modelx.modelProductSpec.BoardAlt.ForEach(i => { i.Priority = seqNumber; seqNumber++; });
            modelx.modelProductSpec.chkPrior = modelx.modelProductSpec.BoardAlt.Count + 1;

            SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "TransactionDataModel", modelx);

            return modelx.modelProductSpec;
        }

        public ProductSpecViewModel ShowBoardAlt()
        {
            TransactionDataModel modelx;
            modelx = SessionExtentions.GetSession<TransactionDataModel>(_httpContextAccessor.HttpContext.Session, "TransactionDataModel");

            if (modelx.modelProductSpec.BoardAlt == null)
                modelx.modelProductSpec.BoardAlt = new List<BoardAltViewModel>();

            modelx.modelProductSpec.chkPrior = modelx.modelProductSpec.BoardAlt.Count + 1;

            SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "TransactionDataModel", modelx);

            return modelx.modelProductSpec;
        }

        public ProductSpecViewModel SortBoardAlt(TransactionDataModel model, int seqNo, string action)
        {
            int seqNumber = 1;

            var itemToMove = model.modelProductSpec.BoardAlt[seqNo - 1];

            if (action == "Up" && (seqNo - 1) != 0)
            {
                model.modelProductSpec.BoardAlt.RemoveAt(seqNo - 1);
                model.modelProductSpec.BoardAlt.Insert(seqNo - 2, itemToMove);
            }
            else if (action == "Down" && (seqNo - 1) != model.modelProductSpec.BoardAlt.Count - 1)
            {
                model.modelProductSpec.BoardAlt.RemoveAt(seqNo - 1);
                model.modelProductSpec.BoardAlt.Insert(seqNo, itemToMove);
            }

            model.modelProductSpec.BoardAlt.ForEach(i => { i.Priority = seqNumber; seqNumber++; });

            SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "TransactionDataModel", model);

            return model.modelProductSpec;
        }

        public TransactionDataModel SaveDataToModel_ProductSpec(TransactionDataModel temp, IFormFile printMaster)
        {
            TransactionDataModel model;

            model = SessionExtentions.GetSession<TransactionDataModel>(_httpContextAccessor.HttpContext.Session, "TransactionDataModel");
            model.CountMat = JsonConvert.DeserializeObject<List<MasterData>>(_masterDataAPIRepository.GetMasterDatasByMaterialNo(_factoryCode, model.MaterialNo, _token)).Count;
            model.modelProductSpec.No_Slot = temp.modelProductSpec.No_Slot == null ? 0 : temp.modelProductSpec.No_Slot;
            model.modelProductSpec.Code = temp.modelProductSpec.Code;
            model.modelProductSpec.Flute = temp.modelProductSpec.Flute;
            model.modelProductSpec.Board = temp.modelProductSpec.Board;
            model.modelProductSpec.BoardKIWI = temp.modelProductSpec.BoardKIWI;
            model.modelProductSpec.Weight = temp.modelProductSpec.Weight;
            model.modelProductSpec.CostPerTon = temp.modelProductSpec.CostPerTon;
            model.modelProductSpec.CostOEM = temp.modelProductSpec.CostOEM;
            model.modelProductSpec.Hierarchy = temp.modelProductSpec.Hierarchy == null ? model.TransactionDetail.HierarchyDetail : temp.modelProductSpec.Hierarchy;
            model.modelProductSpec.CoatingTable = temp.modelProductSpec.CoatingTable;
            if (model.modelProductSpec.Flute != null)
            {
                var flute = JsonConvert.DeserializeObject<Flute>(_fluteAPIRepository.GetFluteByFlute(_factoryCode, model.modelProductSpec.Flute, _token));
                model.modelProductSpec.A = flute == null ? null : flute.A;
                model.modelProductSpec.B = flute == null ? null : flute.B;
                model.modelProductSpec.C = flute == null ? null : flute.C;
                model.modelProductSpec.D1 = flute == null ? null : flute.D1;
                model.modelProductSpec.D2 = flute == null ? null : flute.D2;
                model.modelProductSpec.JoinSize = flute == null ? null : flute.JoinSize;
                model.modelProductSpec.LayerPallet = flute == null ? null : flute.LayerPallet;
            }

            model.modelProductSpec.SideA = temp.modelProductSpec.SideA == null ? 0 : temp.modelProductSpec.SideA;
            model.modelProductSpec.SideB = temp.modelProductSpec.SideB == null ? 0 : temp.modelProductSpec.SideB;
            model.modelProductSpec.SideC = temp.modelProductSpec.SideC == null ? 0 : temp.modelProductSpec.SideC;
            model.modelProductSpec.SideD = temp.modelProductSpec.SideD == null ? 0 : temp.modelProductSpec.SideD;

            model.modelProductSpec.Leg = temp.modelProductSpec.Leg == null ? 0 : temp.modelProductSpec.Leg;
            model.modelProductSpec.Wid = temp.modelProductSpec.Wid == null ? 0 : temp.modelProductSpec.Wid;
            if (model.modelCategories.FormGroup == "CG")
            {
                model.modelProductSpec.Wid = model.modelProductSpec.SideA + model.modelProductSpec.SideB + model.modelProductSpec.SideC;
                model.modelProductSpec.Hig = Convert.ToInt32(model.modelProductSpec.SideD);
            }
            else if (model.modelCategories.FormGroup == "HC" || model.modelCategories.FormGroup == "HB")
            {
                model.modelProductSpec.Hig = temp.modelProductSpec.Height;
                model.modelProductSpec.Height = temp.modelProductSpec.Height;
            }
            else if (model.modelCategories.FormGroup == "PALLET")
            {
                model.modelProductSpec.Leg = temp.modelProductSpec.Leg == null ? temp.modelProductSpec.CutSheetLeng : temp.modelProductSpec.Leg;
                model.modelProductSpec.Wid = temp.modelProductSpec.Wid == null ? temp.modelProductSpec.CutSheetWid : temp.modelProductSpec.Wid;
                model.modelProductSpec.Hig = temp.modelProductSpec.Hig;
            }
            else
            {
                model.modelProductSpec.Hig = temp.modelProductSpec.Hig == null ? 0 : temp.modelProductSpec.Hig;
            }
            model.modelProductSpec.ScoreL8 = temp.modelProductSpec.ScoreL8 == null ? 0 : temp.modelProductSpec.ScoreL8;
            model.modelProductSpec.ScoreL9 = temp.modelProductSpec.ScoreL9 == null ? 0 : temp.modelProductSpec.ScoreL9;
            model.modelProductSpec.ScoreL6 = temp.modelProductSpec.ScoreL6 == null ? 0 : temp.modelProductSpec.ScoreL6;
            model.modelProductSpec.ScoreL7 = temp.modelProductSpec.ScoreL7 == null ? 0 : temp.modelProductSpec.ScoreL7;

            model.modelProductSpec.JointLap = model.modelCategories.FormGroup == "HC" ? temp.modelProductSpec.JoinSize : temp.modelProductSpec.JointLap;
            model.modelProductSpec.ScoreL2 = temp.modelProductSpec.ScoreL2 == null ? 0 : temp.modelProductSpec.ScoreL2;
            model.modelProductSpec.ScoreL3 = temp.modelProductSpec.ScoreL3 == null ? 0 : temp.modelProductSpec.ScoreL3;
            model.modelProductSpec.ScoreL4 = temp.modelProductSpec.ScoreL4 == null ? 0 : temp.modelProductSpec.ScoreL4;
            model.modelProductSpec.ScoreL5 = temp.modelProductSpec.ScoreL5 == null ? 0 : temp.modelProductSpec.ScoreL5;
            model.modelProductSpec.Slit = temp.modelProductSpec.Slit == null ? 0 : temp.modelProductSpec.Slit;

            model.modelProductSpec.ScoreW1 = temp.modelProductSpec.ScoreW1 == null ? 0 : temp.modelProductSpec.ScoreW1;
            model.modelProductSpec.Scorew2 = temp.modelProductSpec.Scorew2 == null ? 0 : temp.modelProductSpec.Scorew2;
            model.modelProductSpec.Scorew3 = temp.modelProductSpec.Scorew3 == null ? 0 : temp.modelProductSpec.Scorew3;
            model.modelProductSpec.Scorew4 = temp.modelProductSpec.Scorew4 == null ? 0 : temp.modelProductSpec.Scorew4;
            model.modelProductSpec.Scorew5 = temp.modelProductSpec.Scorew5 == null ? 0 : temp.modelProductSpec.Scorew5;
            model.modelProductSpec.Scorew6 = temp.modelProductSpec.Scorew6 == null ? 0 : temp.modelProductSpec.Scorew6;
            model.modelProductSpec.Scorew7 = temp.modelProductSpec.Scorew7 == null ? 0 : temp.modelProductSpec.Scorew7;
            model.modelProductSpec.Scorew8 = temp.modelProductSpec.Scorew8 == null ? 0 : temp.modelProductSpec.Scorew8;
            model.modelProductSpec.Scorew9 = temp.modelProductSpec.Scorew9 == null ? 0 : temp.modelProductSpec.Scorew9;
            model.modelProductSpec.Scorew10 = temp.modelProductSpec.Scorew10 == null ? 0 : temp.modelProductSpec.Scorew10;
            model.modelProductSpec.Scorew11 = temp.modelProductSpec.Scorew11 == null ? 0 : temp.modelProductSpec.Scorew11;
            model.modelProductSpec.Scorew12 = temp.modelProductSpec.Scorew12 == null ? 0 : temp.modelProductSpec.Scorew12;
            model.modelProductSpec.Scorew13 = temp.modelProductSpec.Scorew13 == null ? 0 : temp.modelProductSpec.Scorew13;
            model.modelProductSpec.Scorew14 = temp.modelProductSpec.Scorew14 == null ? 0 : temp.modelProductSpec.Scorew14;
            model.modelProductSpec.Scorew15 = temp.modelProductSpec.Scorew15 == null ? 0 : temp.modelProductSpec.Scorew15;
            model.modelProductSpec.Scorew16 = temp.modelProductSpec.Scorew16 == null ? 0 : temp.modelProductSpec.Scorew16;

            model.modelProductSpec.CGType = temp.modelProductSpec.CGType;
            model.modelProductSpec.CutSheetWid = temp.modelProductSpec.CutSheetWid == null && model.modelCategories.FormGroup == "CG" ? (model.modelProductSpec.SideA + model.modelProductSpec.SideB + model.modelProductSpec.SideC) : temp.modelProductSpec.CutSheetWid;
            model.modelProductSpec.CutSheetLeng = temp.modelProductSpec.CutSheetLeng;
            model.modelProductSpec.CutSheetWidInch = model.modelCategories.FormGroup == "OFFSETTS" ? _newProductService.ConvertmmToInch(model.modelProductSpec.CutSheetWid) : _newProductService.ConvertmmToInch(model.modelProductSpec.CutSheetWid);
            model.modelProductSpec.CutSheetLengInch = model.modelCategories.FormGroup == "OFFSETTS" ? _newProductService.ConvertmmToInch(model.modelProductSpec.CutSheetLeng) : _newProductService.ConvertmmToInch(model.modelProductSpec.CutSheetLeng);
            model.modelProductSpec.TwoPiece = model.modelCategories.IsTwoPiece == false ? false : temp.modelProductSpec.TwoPiece;
            model.modelProductSpec.GLWid = temp.modelProductSpec.GLWid == null ? false : temp.modelProductSpec.GLWid;
            model.modelProductSpec.GLTail = temp.modelProductSpec.GLTail == null ? false : temp.modelProductSpec.GLTail;
            model.modelProductSpec.MaterialNo = model.MaterialNo;
            model.modelProductSpec.Flag = temp.modelProductSpec.Flag;
            model.modelProductSpec.IsWrap = temp.modelProductSpec.IsWrap;
            model.modelProductSpec.IsNotch = temp.modelProductSpec.IsNotch;
            model.modelProductSpec.NotchDegree = temp.modelProductSpec.NotchDegree == 0 ? temp.modelProductSpec.NotchDegreex : temp.modelProductSpec.NotchDegree;
            model.modelProductSpec.NotchArea = temp.modelProductSpec.NotchArea;
            model.modelProductSpec.NotchSide = temp.modelProductSpec.NotchSide;
            model.modelProductSpec.widHC = temp.modelProductSpec.widHC;
            model.modelProductSpec.lenHC = temp.modelProductSpec.lenHC;

            model.modelProductSpec.Perforate1 = temp.modelProductSpec.Perforate1 == null ? 0 : temp.modelProductSpec.Perforate1;
            model.modelProductSpec.Perforate2 = temp.modelProductSpec.Perforate2 == null ? 0 : temp.modelProductSpec.Perforate2;
            model.modelProductSpec.Perforate3 = temp.modelProductSpec.Perforate3 == null ? 0 : temp.modelProductSpec.Perforate3;
            model.modelProductSpec.Perforate4 = temp.modelProductSpec.Perforate4 == null ? 0 : temp.modelProductSpec.Perforate4;
            model.modelProductSpec.Perforate5 = temp.modelProductSpec.Perforate5 == null ? 0 : temp.modelProductSpec.Perforate5;
            model.modelProductSpec.Perforate6 = temp.modelProductSpec.Perforate6 == null ? 0 : temp.modelProductSpec.Perforate6;
            model.modelProductSpec.Perforate7 = temp.modelProductSpec.Perforate7 == null ? 0 : temp.modelProductSpec.Perforate7;
            model.modelProductSpec.Perforate8 = temp.modelProductSpec.Perforate8 == null ? 0 : temp.modelProductSpec.Perforate8;
            model.modelProductSpec.Perforate9 = temp.modelProductSpec.Perforate9 == null ? 0 : temp.modelProductSpec.Perforate9;
            model.modelProductSpec.Perforate10 = temp.modelProductSpec.Perforate10 == null ? 0 : temp.modelProductSpec.Perforate10;
            model.modelProductSpec.Perforate11 = temp.modelProductSpec.Perforate11 == null ? 0 : temp.modelProductSpec.Perforate11;
            model.modelProductSpec.Perforate12 = temp.modelProductSpec.Perforate12 == null ? 0 : temp.modelProductSpec.Perforate12;
            model.modelProductSpec.Perforate13 = temp.modelProductSpec.Perforate13 == null ? 0 : temp.modelProductSpec.Perforate13;
            model.modelProductSpec.Perforate14 = temp.modelProductSpec.Perforate14 == null ? 0 : temp.modelProductSpec.Perforate14;
            model.modelProductSpec.Perforate15 = temp.modelProductSpec.Perforate15 == null ? 0 : temp.modelProductSpec.Perforate15;
            model.modelProductSpec.Perforate16 = temp.modelProductSpec.Perforate16 == null ? 0 : temp.modelProductSpec.Perforate16;
            model.modelProductSpec.PerforateGap = temp.modelProductSpec.PerforateGap == null ? 0 : temp.modelProductSpec.PerforateGap;

            var WeightAndArea = new List<ProductSpecViewModel>();

            if (WeightAndArea.Count == 0)
            {
                model.modelProductSpec.SheetArea = temp.modelProductSpec.SheetArea == null ? 0 : temp.modelProductSpec.SheetArea;
                model.modelProductSpec.BoxArea = temp.modelProductSpec.BoxArea == null ? 0 : temp.modelProductSpec.BoxArea;
                model.modelProductSpec.WeightSh = temp.modelProductSpec.WeightSh;
                model.modelProductSpec.WeightBox = temp.modelProductSpec.WeightBox;
            }
            else
            {
                model.modelProductSpec.SheetArea = WeightAndArea.First().SheetArea == null ? 0 : WeightAndArea.First().SheetArea;
                model.modelProductSpec.BoxArea = WeightAndArea.First().BoxArea == null ? 0 : WeightAndArea.First().BoxArea;
                model.modelProductSpec.WeightSh = WeightAndArea.First().WeightSh;
                model.modelProductSpec.WeightBox = WeightAndArea.First().WeightBox;
            }
            model.modelProductSpec.UnUpgradBoard = temp.modelProductSpec.UnUpgradBoard;

            //string[] fileName = _extensionService.UploadPicture(printMaster);
            string dataUrl = _extensionService.ResizeImageRatio(printMaster, 1080, 1080);

            if (printMaster != null)
            {
                temp.modelProductSpec.PrintMasterPath = dataUrl;
                model.modelProductSpec.PrintMaster = printMaster.FileName;
            }

            if (temp.modelProductSpec.PrintMasterPath != null)
            {
                model.modelProductSpec.DimensionPropertiesImageBase64String = temp.modelProductSpec.PrintMasterPath;
                model.modelProductSpec.CapImg = false;
                if (model.modelProductSpec.PrintMaster != null)
                {
                    var indexPath = model.modelProductSpec.PrintMaster.LastIndexOf('\\');
                    model.modelProductSpec.PrintMaster = model.modelProductSpec.PrintMaster.Substring(indexPath + 1, model.modelProductSpec.PrintMaster.Length - indexPath - 1);
                }
            }
            else
            {
                if (model.modelCategories.FormGroup != "TCG" && model.modelCategories.FormGroup != "DIGITAL")
                {
                    model.modelProductSpec.DimensionPropertiesImageBase64String = temp.modelProductSpec.DimensionPropertiesImageBase64String;
                    model.modelProductSpec.CapImg = true;
                }
                else
                {
                    model.modelProductSpec.CapImg = false;
                }
            }
            model.modelProductSpec.PrintMasterPath = model.modelProductSpec.DimensionPropertiesImageBase64String;

            if (model.modelProductSpec.Board != null)
                model.TransactionDetail.BoardDetail = model.modelProductSpec.Flute + ":" + model.modelProductSpec.Board;

            model.TransactionDetail.CostDetail = Convert.ToString(model.modelProductSpec.CostPerTon);
            model.TransactionDetail.HierarchyDetail = Convert.ToString(model.modelProductSpec.Hierarchy);
            model.modelProductSpec.Coating.Clear();

            model.modelProductSpec.PrintMaster = SaveProductSpec(model.modelProductSpec, model.EventFlag, model.CountMat, model.modelProductInfo, model.modelCategories.Id_ProdType);

            UpdateTransactionsDetail(model);

            UpdateCostIntoPlantView(model);
            if (model.modelProductSpec.BoardAlt != null)
            {
                RemoveBoardAlt(model.MaterialNo);
                AddBoardAltToDatabase(model.modelProductSpec);
            }

            if (model.EventFlag == "Edit")
            {
                model.modelRouting.SheetLengthIn = model.modelProductSpec.CutSheetLeng.ToString();
                model.modelRouting.SheetLengthOut = model.modelProductSpec.CutSheetLeng.ToString();
                model.modelRouting.SheetWidthIn = model.modelProductSpec.CutSheetWid.ToString();
                model.modelRouting.SheetWidthOut = model.modelProductSpec.CutSheetWid.ToString();

                model.modelRouting.WeightIn = Math.Round(Convert.ToDecimal(model.modelProductSpec.WeightSh), 3).ToString();
                model.modelRouting.WeightOut = Math.Round(Convert.ToDecimal(model.modelProductSpec.WeightSh), 3).ToString();
            }

            //update max step
            _newProductService.UpdateMaxProgress(_factoryCode, ref model);

            SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "TransactionDataModel", model);

            return model;
        }

        public void RemoveCoating(string mat)
        {
            var del = JsonConvert.DeserializeObject<List<Coating>>(_coatingAPIRepository.GetCoatingByMaterialNo(mat, _factoryCode, _token));
            if (del.Count != 0)
            {
                foreach (var item in del)
                {
                    ParentModel Parent = new ParentModel();
                    Parent.AppName = Globals.AppNameEncrypt;
                    Parent.SaleOrg = _saleOrg;
                    Parent.PlantCode = _factoryCode;


                    dynamic itemCoatingToDelete = _coatingAPIRepository.GetCoatingById(_factoryCode, item.Id, _token);

                    Parent.Coating = JsonConvert.DeserializeObject<Coating>(itemCoatingToDelete);
                    string CoatingJsonString = JsonConvert.SerializeObject(Parent);
                    _coatingAPIRepository.DeleteCoating(_factoryCode, CoatingJsonString, _token);
                }
            }
        }

        public List<Coating> Coatinglst(TransactionDataModel model)
        {

            return JsonConvert.DeserializeObject<List<Coating>>(_coatingAPIRepository.GetCoatingByMaterialNo(_factoryCode, model.MaterialNo, _token));

        }

        public void Coating(string CoatingArray, TransactionDataModel model)
        {
            _coatingAPIRepository.DeleteCoating(_factoryCode, model.MaterialNo, _token);
            List<Tempcoating> tmp = new List<Tempcoating>();
            if (CoatingArray != null)
            {

                tmp = JsonConvert.DeserializeObject<List<Tempcoating>>(CoatingArray);
            }

            var Parent = new ParentModel();
            Parent.AppName = Globals.AppNameEncrypt;
            Parent.FactoryCode = _factoryCode;
            Parent.PlantCode = _factoryCode;
            Parent.CoatingList = new List<Coating>();
            string CoatingJsonString = "";


            foreach (var item in tmp)
            {

                Coating tmpCoating = new Coating();
                tmpCoating.FactoryCode = _factoryCode;
                tmpCoating.MaterialNo = model.MaterialNo;
                tmpCoating.Station = item.Station;
                string[] arraylayer = item.Layer.Split(" ");
                tmpCoating.Slide = arraylayer[0] == "Top" ? "T" : "B";
                tmpCoating.Layer = Convert.ToInt32(arraylayer[1] == null ? "0" : arraylayer[1]);
                tmpCoating.Type = item.Type;
                tmpCoating.Name = item.Name;
                Parent.CoatingList.Add(tmpCoating);


            }
            CoatingJsonString = JsonConvert.SerializeObject(Parent);
            _coatingAPIRepository.SaveCoating(_factoryCode, CoatingJsonString, _token);

        }

        public string SaveProductSpec(ProductSpecViewModel board, string eventFlag, int countMat, ProductInfoView info, int idPDT)
        {
            var Parent = new ParentModel();
            Parent.AppName = Globals.AppNameEncrypt;
            Parent.SaleOrg = _saleOrg;
            Parent.PlantCode = _factoryCode;

            Parent.MasterData = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNo(_factoryCode, board.MaterialNo, _token));
            var transactionDetailObject = JsonConvert.DeserializeObject<TransactionsDetail>(_transactionsDetailAPIRepository.GetTransactionsDetailByMat(_factoryCode, Parent.MasterData.MaterialNo, _token));
            var isOursource = transactionDetailObject != null && transactionDetailObject.Outsource ? true : false;

            if (Parent.MasterData.PdisStatus != "N" && Parent.MasterData.SapStatus == true)
            {
                Parent.MasterData.PdisStatus = "M";
                if (_saleOrg != Parent.MasterData.SaleOrg)
                    Parent.MasterData.TranStatus = true;
                else
                    Parent.MasterData.TranStatus = isOursource ? true : false;
            }
            else if (Parent.MasterData.SapStatus == false)
            {
                if (_saleOrg != Parent.MasterData.SaleOrg)
                    Parent.MasterData.TranStatus = true;
                else
                    Parent.MasterData.TranStatus = isOursource ? true : false;
            }

            Parent.MasterData.Hierarchy = board.Hierarchy;
            Parent.MasterData.TwoPiece = board.TwoPiece;
            Parent.MasterData.Flute = board.Flute;
            Parent.MasterData.FluteDesc = board.Flute;
            Parent.MasterData.Code = board.Code;
            Parent.MasterData.Board = board.Board;

            if (board.Board != null)
            {
                List<PaperGrade> ppg;
                List<FluteTr> fluteTr;
                string[] st = { };
                int i = 0;

                st = board.Board.Split("/");
                ppg = JsonConvert.DeserializeObject<List<PaperGrade>>(_paperGradeAPIRepository.GetPaperGradeList(_factoryCode, _token));
                fluteTr = JsonConvert.DeserializeObject<List<FluteTr>>(_fluteTrAPIRepository.GetFluteTrByFlute(_factoryCode, board.Flute, _token));
                Parent.MasterData.Gl = null;
                Parent.MasterData.Glweigth = null;
                Parent.MasterData.Bm = null;
                Parent.MasterData.Bmweigth = null;
                Parent.MasterData.Bl = null;
                Parent.MasterData.Blweigth = null;
                Parent.MasterData.Cm = null;
                Parent.MasterData.Cmweigth = null;
                Parent.MasterData.Cl = null;
                Parent.MasterData.Clweigth = null;
                Parent.MasterData.Dm = null;
                Parent.MasterData.Dmweigth = null;
                Parent.MasterData.Dl = null;
                Parent.MasterData.Dlweigth = null;

                if (fluteTr.Count > i && st.Count() > i)
                {
                    if (fluteTr[i].Item == 1)
                    {
                        var paper = ppg.Where(p => p.Grade == st[i]).FirstOrDefault();
                        if (paper != null)
                        {
                            Parent.MasterData.Gl = paper.Paper;
                            Parent.MasterData.Glweigth = Convert.ToInt32(paper.BasicWeight);
                            i++;
                        }
                        else
                        {
                            if (st[i] != "")
                            {
                                if (st[i].Length >= 5)
                                {
                                    Parent.MasterData.Gl = st[i].Substring(0, st[i].Length - 3);
                                    Parent.MasterData.Glweigth = Convert.ToInt32(st[i].Substring(st[i].Length - 3, 3));
                                    i++;
                                }
                                else if (st[i].Length == 4)
                                {
                                    Parent.MasterData.Gl = st[i].Substring(0, st[i].Length - 2);
                                    Parent.MasterData.Glweigth = Convert.ToInt32(st[i].Substring(st[i].Length - 2, 2));
                                    i++;
                                }
                                else
                                {
                                    Parent.MasterData.Gl = st[i].Substring(0, st[i].Length);
                                    i++;
                                }
                            }
                        }
                    }
                }
                else if (st.Count() > i)
                {
                    if (st[i].Length >= 5)
                    {
                        Parent.MasterData.Gl = st[i].Substring(0, st[i].Length - 3);
                        Parent.MasterData.Glweigth = Convert.ToInt32(st[i].Substring(st[i].Length - 3, 3));
                        i++;
                    }
                }

                if (fluteTr.Count > i && st.Count() > i)
                {
                    if (fluteTr[i].Item == 2)
                    {
                        var paper = ppg.Where(p => p.Grade == st[i]).FirstOrDefault();
                        if (paper != null)
                        {
                            Parent.MasterData.Bm = paper.Paper;
                            Parent.MasterData.Bmweigth = Convert.ToInt32(paper.BasicWeight);
                            i++;
                        }
                        else
                        {
                            if (st[i] != "")
                            {
                                if (st[i].Length >= 5)
                                {
                                    Parent.MasterData.Bm = st[i].Substring(0, st[i].Length - 3);
                                    Parent.MasterData.Bmweigth = Convert.ToInt32(st[i].Substring(st[i].Length - 3, 3));
                                    i++;
                                }
                                else if (st[i].Length == 4)
                                {
                                    Parent.MasterData.Bm = st[i].Substring(0, st[i].Length - 2);
                                    Parent.MasterData.Bmweigth = Convert.ToInt32(st[i].Substring(st[i].Length - 2, 2));
                                    i++;
                                }
                                else
                                {
                                    Parent.MasterData.Bm = st[i].Substring(0, st[i].Length);
                                    i++;
                                }
                            }
                        }
                    }
                }
                else if (st.Count() > i)
                {
                    if (st[i].Length >= 5)
                    {
                        Parent.MasterData.Bm = st[i].Substring(0, st[i].Length - 3);
                        Parent.MasterData.Bmweigth = Convert.ToInt32(st[i].Substring(st[i].Length - 3, 3));
                        i++;
                    }
                }

                if (fluteTr.Count > i && st.Count() > i)
                {
                    if (fluteTr[i].Item == 3)
                    {
                        var paper = ppg.Where(p => p.Grade == st[i]).FirstOrDefault();
                        if (paper != null)
                        {
                            Parent.MasterData.Bl = paper.Paper;
                            Parent.MasterData.Blweigth = Convert.ToInt32(paper.BasicWeight);
                            i++;
                        }
                        else
                        {
                            if (st[i] != "")
                            {
                                if (st[i].Length >= 5)
                                {
                                    Parent.MasterData.Bl = st[i].Substring(0, st[i].Length - 3);
                                    Parent.MasterData.Blweigth = Convert.ToInt32(st[i].Substring(st[i].Length - 3, 3));
                                    i++;
                                }
                                else if (st[i].Length == 4)
                                {
                                    Parent.MasterData.Bl = st[i].Substring(0, st[i].Length - 2);
                                    Parent.MasterData.Blweigth = Convert.ToInt32(st[i].Substring(st[i].Length - 2, 2));
                                    i++;
                                }
                                else
                                {
                                    Parent.MasterData.Bl = st[i].Substring(0, st[i].Length);
                                    i++;
                                }
                            }
                        }
                    }
                }
                else if (st.Count() > i)
                {
                    if (st[i].Length >= 5)
                    {
                        Parent.MasterData.Bl = st[i].Substring(0, st[i].Length - 3);
                        Parent.MasterData.Blweigth = Convert.ToInt32(st[i].Substring(st[i].Length - 3, 3));
                        i++;
                    }
                }

                if (fluteTr.Count > i && st.Count() > i)
                {
                    if (fluteTr[i].Item == 4)
                    {
                        var paper = ppg.Where(p => p.Grade == st[i]).FirstOrDefault();
                        if (paper != null)
                        {
                            Parent.MasterData.Cm = paper.Paper;
                            Parent.MasterData.Cmweigth = Convert.ToInt32(paper.BasicWeight);
                            i++;
                        }
                        else
                        {
                            if (st[i] != "")
                            {
                                if (st[i].Length >= 5)
                                {
                                    Parent.MasterData.Cm = st[i].Substring(0, st[i].Length - 3);
                                    Parent.MasterData.Cmweigth = Convert.ToInt32(st[i].Substring(st[i].Length - 3, 3));
                                    i++;
                                }
                                else if (st[i].Length == 4)
                                {
                                    Parent.MasterData.Cm = st[i].Substring(0, st[i].Length - 2);
                                    Parent.MasterData.Cmweigth = Convert.ToInt32(st[i].Substring(st[i].Length - 2, 2));
                                    i++;
                                }
                                else
                                {
                                    Parent.MasterData.Cm = st[i].Substring(0, st[i].Length);
                                    i++;
                                }
                            }
                        }
                    }
                }
                else if (st.Count() > i)
                {
                    if (st[i].Length >= 5)
                    {
                        Parent.MasterData.Cm = st[i].Substring(0, st[i].Length - 3);
                        Parent.MasterData.Cmweigth = Convert.ToInt32(st[i].Substring(st[i].Length - 3, 3));
                        i++;
                    }
                }

                if (fluteTr.Count > i && st.Count() > i)
                {
                    if (fluteTr[i].Item == 5)
                    {
                        var paper = ppg.Where(p => p.Grade == st[i]).FirstOrDefault();
                        if (paper != null)
                        {
                            Parent.MasterData.Cl = paper.Paper;
                            Parent.MasterData.Clweigth = Convert.ToInt32(paper.BasicWeight);
                            i++;
                        }
                        else
                        {
                            if (st[i] != "")
                            {
                                if (st[i].Length >= 5)
                                {
                                    Parent.MasterData.Cl = st[i].Substring(0, st[i].Length - 3);
                                    Parent.MasterData.Clweigth = Convert.ToInt32(st[i].Substring(st[i].Length - 3, 3));
                                    i++;
                                }
                                else if (st[i].Length == 4)
                                {
                                    Parent.MasterData.Cl = st[i].Substring(0, st[i].Length - 2);
                                    Parent.MasterData.Clweigth = Convert.ToInt32(st[i].Substring(st[i].Length - 2, 2));
                                    i++;
                                }
                                else
                                {
                                    Parent.MasterData.Cl = st[i].Substring(0, st[i].Length);
                                    i++;
                                }
                            }
                        }
                    }
                }
                else if (st.Count() > i)
                {
                    if (st[i].Length >= 5)
                    {
                        Parent.MasterData.Cl = st[i].Substring(0, st[i].Length - 3);
                        Parent.MasterData.Clweigth = Convert.ToInt32(st[i].Substring(st[i].Length - 3, 3));
                        i++;
                    }
                }

                if (fluteTr.Count > i && st.Count() > i)
                {
                    if (fluteTr[i].Item == 6)
                    {
                        var paper = ppg.Where(p => p.Grade == st[i]).FirstOrDefault();
                        if (paper != null)
                        {
                            Parent.MasterData.Dm = paper.Paper;
                            Parent.MasterData.Dmweigth = Convert.ToInt32(paper.BasicWeight);
                            i++;
                        }
                        else
                        {
                            if (st[i] != "")
                            {
                                if (st[i].Length >= 5)
                                {
                                    Parent.MasterData.Dm = st[i].Substring(0, st[i].Length - 3);
                                    Parent.MasterData.Dmweigth = Convert.ToInt32(st[i].Substring(st[i].Length - 3, 3));
                                    i++;
                                }
                                else if (st[i].Length == 4)
                                {
                                    Parent.MasterData.Dm = st[i].Substring(0, st[i].Length - 2);
                                    Parent.MasterData.Dmweigth = Convert.ToInt32(st[i].Substring(st[i].Length - 2, 2));
                                    i++;
                                }
                                else
                                {
                                    Parent.MasterData.Dm = st[i].Substring(0, st[i].Length);
                                    i++;
                                }
                            }
                        }
                    }
                }
                else if (st.Count() > i)
                {
                    if (st[i].Length >= 5)
                    {
                        Parent.MasterData.Dm = st[i].Substring(0, st[i].Length - 3);
                        Parent.MasterData.Dmweigth = Convert.ToInt32(st[i].Substring(st[i].Length - 3, 3));
                        i++;
                    }
                }

                if (fluteTr.Count > i && st.Count() > i)
                {
                    if (fluteTr[i].Item == 7)
                    {
                        var paper = ppg.Where(p => p.Grade == st[i]).FirstOrDefault();
                        if (paper != null)
                        {
                            Parent.MasterData.Dl = paper.Paper;
                            Parent.MasterData.Dlweigth = Convert.ToInt32(paper.BasicWeight);
                            i++;
                        }
                        else
                        {
                            if (st[i] != "")
                            {
                                if (st[i].Length >= 5)
                                {
                                    Parent.MasterData.Dl = st[i].Substring(0, st[i].Length - 3);
                                    Parent.MasterData.Dlweigth = Convert.ToInt32(st[i].Substring(st[i].Length - 3, 3));
                                    i++;
                                }
                                else if (st[i].Length == 4)
                                {
                                    Parent.MasterData.Dl = st[i].Substring(0, st[i].Length - 2);
                                    Parent.MasterData.Dlweigth = Convert.ToInt32(st[i].Substring(st[i].Length - 2, 2));
                                    i++;
                                }
                                else
                                {
                                    Parent.MasterData.Dl = st[i].Substring(0, st[i].Length);
                                    i++;
                                }
                            }
                        }
                    }
                }
                else if (st.Count() > i)
                {
                    if (st[i].Length >= 5)
                    {
                        Parent.MasterData.Dl = st[i].Substring(0, st[i].Length - 3);
                        Parent.MasterData.Dlweigth = Convert.ToInt32(st[i].Substring(st[i].Length - 3, 3));
                        i++;
                    }
                }
            }

            var DCpath = "";
            var DCpathx = "";
            if (board.DimensionPropertiesImageBase64String != null)
            {
                var cursor = board.DimensionPropertiesImageBase64String.IndexOf(',') + 1;
                var base64 = board.DimensionPropertiesImageBase64String.Substring(cursor);

                //get path form pmtcontext
                var path = JsonConvert.DeserializeObject<PmtsConfig>(_pmtsConfigAPIRepository.GetPMTsConfigByFactoryName(_factoryCode, "Drawing_Path", _token)).FucValue;
                var pathFTP = JsonConvert.DeserializeObject<PmtsConfig>(_pmtsConfigAPIRepository.GetPMTsConfigByFactoryName(_factoryCode, "DrawingFTP", _token)).FucValue;

                if (board.CapImg == true)
                {
                    DCpath = path + "\\" + _factoryCode + "-" + board.MaterialNo + ".png";
                    DCpathx = pathFTP + "\\" + _factoryCode + "-" + board.MaterialNo + ".png";
                }
                else
                {
                    DCpath = path + "\\" + board.PrintMaster;
                    DCpathx = pathFTP + "\\" + board.PrintMaster;
                }

                if (path != null)
                {
                    //byte[] bytes = Convert.FromBase64String(base64);

                    //using (MemoryStream ms = new MemoryStream(bytes))
                    //{
                    //    using (Bitmap bm2 = new Bitmap(ms))
                    //    {
                    //        bm2.Save(DCpath);
                    //    }
                    //}

                    _extensionService.UploadImage(base64, DCpath);
                }

                if (pathFTP != null)  //if (pathFTP != null && idPDT != 70)
                {
                    //byte[] bytes = Convert.FromBase64String(base64);

                    //using (MemoryStream ms = new MemoryStream(bytes))
                    //{
                    //    using (Bitmap bm2 = new Bitmap(ms))
                    //    {
                    //        bm2.Save(DCpathx);
                    //    }
                    //}

                    _extensionService.UploadImage(base64, DCpathx);
                }

            }

            Parent.MasterData.Wid = board.Wid;
            Parent.MasterData.Leg = board.Leg;
            Parent.MasterData.Hig = board.Hig;
            Parent.MasterData.CutSheetLeng = board.CutSheetLeng;
            Parent.MasterData.CutSheetWid = board.CutSheetWid;
            Parent.MasterData.CutSheetLengInch = board.CutSheetLengInch;
            Parent.MasterData.CutSheetWidInch = board.CutSheetWidInch;
            Parent.MasterData.SheetArea = board.SheetArea;
            Parent.MasterData.BoxArea = board.BoxArea;
            Parent.MasterData.ScoreW1 = Convert.ToInt16(board.ScoreW1);
            //Parent.MasterData.Scorew2 = Parent.MasterData.RscStyle == "Sleeve"? 0 : Convert.ToInt16(board.Scorew2);
            Parent.MasterData.Scorew2 = Convert.ToInt16(board.Scorew2);
            Parent.MasterData.Scorew3 = Convert.ToInt16(board.Scorew3);
            Parent.MasterData.Scorew4 = Convert.ToInt16(board.Scorew4);
            Parent.MasterData.Scorew5 = Convert.ToInt16(board.Scorew5);
            Parent.MasterData.Scorew6 = Convert.ToInt16(board.Scorew6);
            Parent.MasterData.Scorew7 = Convert.ToInt16(board.Scorew7);
            Parent.MasterData.Scorew8 = Convert.ToInt16(board.Scorew8);
            Parent.MasterData.Scorew9 = Convert.ToInt16(board.Scorew9);
            Parent.MasterData.Scorew10 = Convert.ToInt16(board.Scorew10);
            Parent.MasterData.Scorew11 = Convert.ToInt16(board.Scorew11);
            Parent.MasterData.Scorew12 = Convert.ToInt16(board.Scorew12);
            Parent.MasterData.Scorew13 = Convert.ToInt16(board.Scorew13);
            Parent.MasterData.Scorew14 = Convert.ToInt16(board.Scorew14);
            Parent.MasterData.Scorew15 = Convert.ToInt16(board.Scorew15);
            Parent.MasterData.Scorew16 = Convert.ToInt16(board.Scorew16);
            Parent.MasterData.JointLap = Convert.ToByte(board.JointLap);
            Parent.MasterData.ScoreL2 = Convert.ToInt16(board.ScoreL2);
            Parent.MasterData.ScoreL3 = Convert.ToInt16(board.ScoreL3);
            Parent.MasterData.ScoreL4 = Convert.ToInt16(board.ScoreL4);
            Parent.MasterData.ScoreL5 = Convert.ToInt16(board.ScoreL5);
            Parent.MasterData.ScoreL6 = Convert.ToInt16(board.ScoreL6);
            Parent.MasterData.ScoreL7 = Convert.ToInt16(board.ScoreL7);
            Parent.MasterData.ScoreL8 = Convert.ToInt16(board.ScoreL8);
            Parent.MasterData.ScoreL9 = Convert.ToInt16(board.ScoreL9);
            Parent.MasterData.Slit = board.Slit;
            Parent.MasterData.NoSlot = Convert.ToInt16(board.No_Slot);
            Parent.MasterData.WeightSh = board.WeightSh;
            Parent.MasterData.WeightBox = board.WeightBox;
            Parent.MasterData.WeightMat = board.WeightBox;
            Parent.MasterData.DiecutPictPath = DCpath == "" && board.PrintMaster != null ? board.PrintMaster : DCpath;
            Parent.MasterData.UnUpgradBoard = board.UnUpgradBoard;
            Parent.MasterData.LastUpdate = DateTime.Now;
            Parent.MasterData.UpdatedBy = _username;
            Parent.MasterData.FactoryCode = _factoryCode;

            Parent.MasterData.Perforate1 = Convert.ToInt16(board.Perforate1);
            Parent.MasterData.Perforate2 = Convert.ToInt16(board.Perforate2);
            Parent.MasterData.Perforate3 = Convert.ToInt16(board.Perforate3);
            Parent.MasterData.Perforate4 = Convert.ToInt16(board.Perforate4);
            Parent.MasterData.Perforate5 = Convert.ToInt16(board.Perforate5);
            Parent.MasterData.Perforate6 = Convert.ToInt16(board.Perforate6);
            Parent.MasterData.Perforate7 = Convert.ToInt16(board.Perforate7);
            Parent.MasterData.Perforate8 = Convert.ToInt16(board.Perforate8);
            Parent.MasterData.Perforate9 = Convert.ToInt16(board.Perforate9);
            Parent.MasterData.Perforate10 = Convert.ToInt16(board.Perforate10);
            Parent.MasterData.Perforate11 = Convert.ToInt16(board.Perforate11);
            Parent.MasterData.Perforate12 = Convert.ToInt16(board.Perforate12);
            Parent.MasterData.Perforate13 = Convert.ToInt16(board.Perforate13);
            Parent.MasterData.Perforate14 = Convert.ToInt16(board.Perforate14);
            Parent.MasterData.Perforate15 = Convert.ToInt16(board.Perforate15);
            Parent.MasterData.Perforate16 = Convert.ToInt16(board.Perforate16);
            Parent.MasterData.PerforateGap = Convert.ToInt16(board.PerforateGap);
            Parent.MasterData.User = _username;
            if (!string.IsNullOrEmpty(Parent?.MasterData?.BoxType))
            {
                //Parent.MasterData.SizeDimensions = _newProductService.CalSizeDimensions(Parent.MasterData, null);
                Parent.MasterData.SizeDimensions = JsonConvert.DeserializeObject<string>(_formulaAPIRepository.CalSizeDimensions(_factoryCode, Parent.MasterData.MaterialNo, _token));
            }

            /*f ((Parent.MasterData.PdisStatus == "N" || Parent.MasterData.PdisStatus == "C") && board.Board != null)*/
            if (board.Board != null)
                SaveBoardUse(board);

            string MasterDataJsonString = JsonConvert.SerializeObject(Parent);

            _masterDataAPIRepository.UpdateMasterData(MasterDataJsonString, _token);

            if (info.PLANTCODE != null && Parent.MasterData.SapStatus == false)
            {
                if (info.MatOursource != "")
                    Parent.MasterData.MaterialNo = info.MatOursource;
                else
                    info.MatOursource = Parent.MasterData.MaterialNo;

                var MasterDataOS = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNo(info.PLANTCODE, info.MatOursource, _token));
                //Parent.MasterData = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNo(info.PLANTCODE, info.MatOursource, _token));
                Parent.MasterData.Id = MasterDataOS != null ? MasterDataOS.Id : 0;
                Parent.MasterData.FactoryCode = info.PLANTCODE;
                Parent.MasterData.SaleOrg = MasterDataOS.SaleOrg;
                Parent.MasterData.Plant = info.PLANTCODE;
                Parent.MasterData.MaterialType = MasterDataOS.MaterialType;
                Parent.MasterData.PurTxt1 = MasterDataOS.PurTxt1;
                Parent.MasterData.PurTxt2 = MasterDataOS.PurTxt2;
                Parent.MasterData.PurTxt3 = MasterDataOS.PurTxt3;
                Parent.MasterData.PurTxt4 = MasterDataOS.PurTxt4;
                Parent.MasterData.MatCopy = null;
                Parent.MasterData.CreateDate = MasterDataOS.CreateDate;
                Parent.MasterData.CreatedBy = MasterDataOS.CreatedBy;
                Parent.MasterData.TranStatus = MasterDataOS.TranStatus;
                Parent.MasterData.PdisStatus = MasterDataOS.PdisStatus;
                Parent.MasterData.User = _username;
                Parent.FactoryCode = info.PLANTCODE;
                Parent.PlantCode = info.PLANTCODE;

                if (!string.IsNullOrEmpty(Parent?.MasterData?.BoxType))
                {
                    //Parent.MasterData.SizeDimensions = _newProductService.CalSizeDimensions(Parent.MasterData, null);
                    Parent.MasterData.SizeDimensions = JsonConvert.DeserializeObject<string>(_formulaAPIRepository.CalSizeDimensions(_factoryCode, Parent.MasterData.MaterialNo, _token));
                }


                //Parent.MasterData.TranStatus = true;
                string OutSource1JsonString = JsonConvert.SerializeObject(Parent);
                if (Parent.MasterData.Id == 0)
                    _masterDataAPIRepository.SaveMasterData(OutSource1JsonString, _token);
                else
                    _masterDataAPIRepository.UpdateMasterData(OutSource1JsonString, _token);
            }

            return DCpath;
        }

        public void SaveBoardUse(ProductSpecViewModel board)
        {
            ParentModel Parent = new ParentModel();
            Parent.AppName = Globals.AppNameEncrypt;
            Parent.SaleOrg = _saleOrg;
            Parent.PlantCode = _factoryCode;
            Parent.BoardUse = JsonConvert.DeserializeObject<BoardUse>(_boardUseAPIRepository.GetBoardUseByMaterialNo(_factoryCode, board.MaterialNo, _token));
            //check create or update board use
            var IsCreated = Parent.BoardUse == null ? true : false;
            Parent.BoardUse = Parent.BoardUse == null ? new BoardUse() : Parent.BoardUse;

            Parent.BoardUse.FactoryCode = _factoryCode;
            Parent.BoardUse.MaterialNo = board.MaterialNo;
            Parent.BoardUse.BoardId = board.Code;
            Parent.BoardUse.BoardName = board.Board;
            //board.BoardAlt = board.BoardAlt == null ? new List<BoardAltViewModel>() : board.BoardAlt;
            //Parent.BoardUse.BoardName = board.BoardAlt.Count() == 0 ? board.Board : board.BoardAlt.Where(alt => alt.Priority == 1).FirstOrDefault().BoardName;
            Parent.BoardUse.Kiwi = board.BoardKIWI;
            Parent.BoardUse.Priority = 1;
            Parent.BoardUse.Active = true;
            Parent.BoardUse.Flute = board.Flute;
            Parent.BoardUse.Gl = null;
            Parent.BoardUse.Bm = null;
            Parent.BoardUse.Bl = null;
            Parent.BoardUse.Cm = null;
            Parent.BoardUse.Cl = null;
            Parent.BoardUse.Dm = null;
            Parent.BoardUse.Dl = null;
            string[] ArrBoard = Parent.BoardUse.BoardName.Split("/");

            if (ArrBoard.Count() > 0)
                Parent.BoardUse.Gl = ArrBoard[0];
            if (ArrBoard.Count() > 1)
            {
                if (Parent.BoardUse.Flute == "C")
                {
                    Parent.BoardUse.Bm = null;
                    Parent.BoardUse.Cm = ArrBoard[1];
                }
                else
                {
                    Parent.BoardUse.Bm = ArrBoard[1];
                }
            }
            if (ArrBoard.Count() > 2)
            {
                if (Parent.BoardUse.Flute == "C")
                {
                    Parent.BoardUse.Bl = null;
                    Parent.BoardUse.Cl = ArrBoard[2];
                }
                else
                {
                    Parent.BoardUse.Bl = ArrBoard[2];
                }
            }
            if (ArrBoard.Count() > 3)
                Parent.BoardUse.Cm = ArrBoard[3];
            if (ArrBoard.Count() > 4)
                Parent.BoardUse.Cl = ArrBoard[4];
            if (ArrBoard.Count() > 5)
                Parent.BoardUse.Dm = ArrBoard[5];
            if (ArrBoard.Count() > 6)
                Parent.BoardUse.Dl = ArrBoard[6];

            Parent.BoardUse.Weight = Convert.ToDouble(board.Weight);
            Parent.BoardUse.CreatedBy = _username;
            Parent.BoardUse.CreatedDate = DateTime.Now;

            string BoardUseJsonString = JsonConvert.SerializeObject(Parent);

            //if (board.action == "Create" || board.action == "Presale" || board.action == "CreateOs")
            //{
            if (IsCreated)
            {
                _boardUseAPIRepository.SaveBoardUse(BoardUseJsonString, _token);
            }
            else
            {
                _boardUseAPIRepository.UpdateBoardUse(BoardUseJsonString, _token);
            }
            //}
        }

        public void UpdateCostIntoPlantView(TransactionDataModel model)
        {
            ParentModel Parent = new ParentModel();
            Parent.AppName = Globals.AppNameEncrypt;
            Parent.SaleOrg = _saleOrg;
            Parent.PlantCode = _factoryCode;
            Parent.PlantView = JsonConvert.DeserializeObject<PlantView>(_plantViewAPIRepository.GetPlantViewByMaterialNo(_factoryCode, model.MaterialNo, _token));


            double costPerTon = Convert.ToDouble(model.modelProductSpec.CostPerTon);
            double costOEM = Convert.ToDouble(model.modelProductSpec.CostOEM);

            if (Parent.PlantView != null)
            {

                if (model.modelCategories.MatCode == "82")
                {
                    Parent.PlantView.StdTotalCost = 0;
                    Parent.PlantView.StdMovingCost = Math.Round(costPerTon, 2);
                }
                else
                {
                    Parent.PlantView.StdTotalCost = Math.Round(costPerTon, 2);
                    Parent.PlantView.StdMovingCost = 0;
                }

                if (Parent.PlantView.PdisStatus != "M" && costPerTon != 0)
                {
                    if (Parent.PlantView.PdisStatus == "N")
                    {
                        Parent.PlantView.PdisStatus = "N";
                    }
                    else
                    {
                        Parent.PlantView.PdisStatus = "C";
                    }


                    Parent.SalesView = JsonConvert.DeserializeObject<SalesView>(_salesViewAPIRepository.GetSaleViewByMaterialNoAndChannelAndDevPlant(_factoryCode, model.MaterialNo, 10, _factoryCode, _token));
                    if (Parent.SalesView != null)
                    {
                        if (Parent.SalesView.PdisStatus == "N")
                        {
                            Parent.SalesView.PdisStatus = "N";
                        }
                        else
                        {
                            Parent.SalesView.PdisStatus = "C";
                        }

                        string osSalesViewJsonString = JsonConvert.SerializeObject(Parent);
                        _salesViewAPIRepository.UpdateSaleView(osSalesViewJsonString, _token);
                    }

                    Parent.SalesView = JsonConvert.DeserializeObject<SalesView>(_salesViewAPIRepository.GetSaleViewByMaterialNoAndChannelAndDevPlant(_factoryCode, model.MaterialNo, 60, _factoryCode, _token));
                    if (Parent.SalesView != null)
                    {
                        if (Parent.SalesView.PdisStatus == "N")
                        {
                            Parent.SalesView.PdisStatus = "N";
                        }
                        else
                        {
                            Parent.SalesView.PdisStatus = "C";
                        }
                        string osSalesViewJsonString = JsonConvert.SerializeObject(Parent);
                        _salesViewAPIRepository.UpdateSaleView(osSalesViewJsonString, _token);
                    }
                }

                string PlantViewJsonString = JsonConvert.SerializeObject(Parent);

                _plantViewAPIRepository.UpdatePlantView(PlantViewJsonString, _token);

                if (model.modelProductInfo.PLANTCODE != null)
                {

                    Parent.PlantView = JsonConvert.DeserializeObject<PlantView>(_plantViewAPIRepository.GetPlantViewByMaterialNo(model.modelProductInfo.PLANTCODE, model.modelProductInfo.MatOursource, _token));
                    if (Parent.PlantView != null)
                    {
                        if (model.modelProductInfo.MatTypeOursource == "82")
                        {
                            Parent.PlantView.StdTotalCost = 0;
                            Parent.PlantView.StdMovingCost = Math.Round(costOEM, 2);
                        }
                        else
                        {
                            Parent.PlantView.StdTotalCost = Math.Round(costOEM, 2);
                            Parent.PlantView.StdMovingCost = 0;
                        }

                        if (costOEM != 0)
                        {
                            if (Parent.PlantView.PdisStatus == "N")
                            {
                                Parent.PlantView.PdisStatus = "N";
                            }
                            else
                            {
                                Parent.PlantView.PdisStatus = "C";

                            }

                            Parent.SalesView = JsonConvert.DeserializeObject<SalesView>(_salesViewAPIRepository.GetSaleViewByMaterialNoAndChannelAndDevPlant(model.modelProductInfo.PLANTCODE, model.modelProductInfo.MatOursource, 10, model.modelProductInfo.PLANTCODE, _token));
                            if (Parent.SalesView != null)
                            {
                                //Parent.SalesView.PdisStatus = "C";
                                if (Parent.SalesView.PdisStatus == "N")
                                {
                                    Parent.SalesView.PdisStatus = "N";
                                }
                                else
                                {
                                    Parent.SalesView.PdisStatus = "C";

                                }
                                string osSalesViewJsonString = JsonConvert.SerializeObject(Parent);
                                _salesViewAPIRepository.UpdateSaleView(osSalesViewJsonString, _token);
                            }

                            Parent.SalesView = JsonConvert.DeserializeObject<SalesView>(_salesViewAPIRepository.GetSaleViewByMaterialNoAndChannelAndDevPlant(model.modelProductInfo.PLANTCODE, model.modelProductInfo.MatOursource, 60, model.modelProductInfo.PLANTCODE, _token));
                            if (Parent.SalesView != null)
                            {
                                if (Parent.SalesView.PdisStatus == "N")
                                {
                                    Parent.SalesView.PdisStatus = "N";
                                }
                                else
                                {
                                    Parent.SalesView.PdisStatus = "C";

                                }
                                string osSalesViewJsonString = JsonConvert.SerializeObject(Parent);
                                _salesViewAPIRepository.UpdateSaleView(osSalesViewJsonString, _token);
                            }
                        }
                        else
                        {
                            Parent.PlantView.PdisStatus = "N";
                        }

                        string osPlantViewJsonString = JsonConvert.SerializeObject(Parent);
                        _plantViewAPIRepository.UpdatePlantView(osPlantViewJsonString, _token);
                    }
                }
            }
        }

        public void UpdateTransactionsDetail(TransactionDataModel model)
        {
            ParentModel Parent = new ParentModel();
            Parent.AppName = Globals.AppNameEncrypt;
            Parent.SaleOrg = _saleOrg;
            Parent.PlantCode = _factoryCode;
            Parent.TransactionsDetail = JsonConvert.DeserializeObject<TransactionsDetail>(_transactionsDetailAPIRepository.GetTransactionsDetailByMat(_factoryCode, model.MaterialNo, _token));
            Parent.TransactionsDetail.Glwid = model.modelProductSpec.GLWid;
            Parent.TransactionsDetail.Gltail = model.modelProductSpec.GLTail;
            Parent.TransactionsDetail.CapImg = model.modelProductSpec.CapImg == null ? false : model.modelProductSpec.CapImg;
            Parent.TransactionsDetail.IsWrap = model.modelProductSpec.IsWrap;
            Parent.TransactionsDetail.IsNotch = model.modelProductSpec.IsNotch;
            Parent.TransactionsDetail.NotchDegree = model.modelProductSpec.NotchDegree;
            Parent.TransactionsDetail.NotchArea = model.modelProductSpec.NotchArea;
            Parent.TransactionsDetail.NotchSide = model.modelProductSpec.NotchSide;
            Parent.TransactionsDetail.SideA = model.modelProductSpec.SideA;
            Parent.TransactionsDetail.SideB = model.modelProductSpec.SideB;
            Parent.TransactionsDetail.SideC = model.modelProductSpec.SideC;
            Parent.TransactionsDetail.SideD = model.modelProductSpec.SideD;
            Parent.TransactionsDetail.Cgtype = model.modelProductSpec.CGType;
            Parent.TransactionsDetail.HoneyCoreSize = model.modelProductSpec.widHC + " x " + model.modelProductSpec.lenHC;
            string TransactionsDetailJsonString = JsonConvert.SerializeObject(Parent);

            _transactionsDetailAPIRepository.UpdateTransactionsDetail(TransactionsDetailJsonString, _token);

            if (model.modelProductInfo.PLANTCODE != null)
            {
                Parent.TransactionsDetail = JsonConvert.DeserializeObject<TransactionsDetail>(_transactionsDetailAPIRepository.GetTransactionsDetailByMat(model.modelProductInfo.PLANTCODE, model.modelProductInfo.MatOursource, _token));
                Parent.TransactionsDetail.Glwid = model.modelProductSpec.GLWid;
                Parent.TransactionsDetail.Gltail = model.modelProductSpec.GLTail;
                Parent.TransactionsDetail.CapImg = model.modelProductSpec.CapImg == null ? false : model.modelProductSpec.CapImg;
                Parent.TransactionsDetail.IsWrap = model.modelProductSpec.IsWrap;
                Parent.TransactionsDetail.IsNotch = model.modelProductSpec.IsNotch;
                Parent.TransactionsDetail.NotchDegree = model.modelProductSpec.NotchDegree;
                Parent.TransactionsDetail.NotchArea = model.modelProductSpec.NotchArea;
                Parent.TransactionsDetail.NotchSide = model.modelProductSpec.NotchSide;
                Parent.TransactionsDetail.SideA = model.modelProductSpec.SideA;
                Parent.TransactionsDetail.SideB = model.modelProductSpec.SideB;
                Parent.TransactionsDetail.SideC = model.modelProductSpec.SideC;
                Parent.TransactionsDetail.SideD = model.modelProductSpec.SideD;
                Parent.TransactionsDetail.Cgtype = model.modelProductSpec.CGType;
                Parent.TransactionsDetail.HoneyCoreSize = model.modelProductSpec.widHC + " x " + model.modelProductSpec.lenHC;
                string TransactionsDetailJsonString1 = JsonConvert.SerializeObject(Parent);

                _transactionsDetailAPIRepository.UpdateTransactionsDetail(TransactionsDetailJsonString1, _token);
            }
        }

        public void RemoveBoardAlt(string mat)
        {
            var del = JsonConvert.DeserializeObject<List<BoardAlternative>>(_boardAlternativeAPIRepository.GetBoardAlternativeByMat(_factoryCode, mat, _token));

            foreach (var item in del)
            {
                ParentModel Parent = new ParentModel();
                Parent.AppName = Globals.AppNameEncrypt;
                Parent.SaleOrg = _saleOrg;
                Parent.PlantCode = _factoryCode;

                dynamic itemBoardAltToDelete = _boardAlternativeAPIRepository.GetBoardAlternativeById(_factoryCode, item.Id, _token);

                Parent.BoardAlternative = JsonConvert.DeserializeObject<BoardAlternative>(itemBoardAltToDelete);
                string BoardAlternativeJsonString = JsonConvert.SerializeObject(Parent);
                _boardAlternativeAPIRepository.DeleteBoardAlternative(BoardAlternativeJsonString, _token);
            }
        }

        public void AddBoardAltToDatabase(ProductSpecViewModel board)
        {
            List<BoardAlternative> boardAlternativesList = new List<BoardAlternative>();

            board.BoardAlt.ForEach(i =>
            {
                boardAlternativesList.Add(new BoardAlternative
                {
                    FactoryCode = _factoryCode,
                    MaterialNo = board.MaterialNo,
                    BoardName = i.BoardName,
                    Priority = i.Priority,
                    Active = i.Active,
                    Flute = i.Flute,
                    CreatedDate = DateTime.Now,
                    CreatedBy = _username
                });
            });

            ParentModel Parent = new ParentModel();
            Parent.AppName = Globals.AppNameEncrypt;
            Parent.SaleOrg = _saleOrg;
            Parent.PlantCode = _factoryCode;

            Parent.BoardAlternativeList = boardAlternativesList;

            string BoardAlternativeJsonString = JsonConvert.SerializeObject(Parent);
            _boardAlternativeAPIRepository.SaveBoardAlternative(BoardAlternativeJsonString, _token);
        }

        public List<ProductSpecViewModel> ComputeRSC(ProductSpecViewModel model)
        {
            List<ProductSpecViewModel> modelx = new List<ProductSpecViewModel>();
            RSCModel rscmodel = new RSCModel();
            rscmodel = mapper.Map<ProductSpecViewModel, RSCModel>(model);

            var rsc = JsonConvert.DeserializeObject<RSCResultModel>(_formulaAPIRepository.CalculateRSC(_factoryCode, JsonConvert.SerializeObject(rscmodel), _token));

            modelx.Add(new ProductSpecViewModel()
            {
                ScoreW1 = rsc.ScoreW1,
                Scorew2 = rsc.Scorew2,
                Scorew3 = rsc.Scorew3,
                JointLap = rsc.JointLap,
                ScoreL2 = rsc.ScoreL2,
                ScoreL3 = rsc.ScoreL3,
                ScoreL4 = rsc.ScoreL4,
                ScoreL5 = rsc.ScoreL5,
                ScoreL6 = rsc.ScoreL6,
                ScoreL7 = rsc.ScoreL7,
                ScoreL8 = rsc.ScoreL8,
                ScoreL9 = rsc.ScoreL9,
                Slit = rsc.Slit,
                CutSheetLeng = rsc.CutSheetLeng,
                CutSheetWid = rsc.CutSheetWid,
                SheetArea = rsc.SheetArea,
                BoxArea = rsc.BoxArea,
                WeightSh = rsc.WeightSh,
                WeightBox = rsc.WeightBox
            });

            return modelx;
        }

        public List<ProductSpecViewModel> ComputeOneP(ProductSpecViewModel model)
        {
            List<ProductSpecViewModel> modelx = new List<ProductSpecViewModel>();
            RSCModel rscmodel = new RSCModel();
            rscmodel = mapper.Map<ProductSpecViewModel, RSCModel>(model);

            var rsc = JsonConvert.DeserializeObject<RSCResultModel>(_formulaAPIRepository.CalculateRSC1Piece(_factoryCode, JsonConvert.SerializeObject(rscmodel), _token));

            modelx.Add(new ProductSpecViewModel()
            {
                ScoreL2 = rsc.ScoreL2,
                ScoreL3 = rsc.ScoreL3,
                ScoreL4 = rsc.ScoreL4,
                ScoreL5 = rsc.ScoreL5,
                ScoreL6 = rsc.ScoreL6,
                ScoreL7 = rsc.ScoreL7,
                ScoreL8 = rsc.ScoreL8,
                ScoreL9 = rsc.ScoreL9,
                Slit = rsc.Slit,
                CutSheetLeng = rsc.CutSheetLeng,
                SheetArea = rsc.SheetArea,
                BoxArea = rsc.BoxArea,
                WeightSh = rsc.WeightSh,
                WeightBox = rsc.WeightBox
            });

            return modelx;
        }

        public List<ProductSpecViewModel> ComputeTwoP(ProductSpecViewModel model)
        {
            List<ProductSpecViewModel> modelx = new List<ProductSpecViewModel>();

            RSCModel rscmodel = new RSCModel();
            rscmodel = mapper.Map<ProductSpecViewModel, RSCModel>(model);

            var rsc = JsonConvert.DeserializeObject<RSCResultModel>(_formulaAPIRepository.CalculateRSC2Piece(_factoryCode, JsonConvert.SerializeObject(rscmodel), _token));

            //int? w = model.Wid;
            //int? l = model.Leg;
            //int? h = model.Hig;

            //var factoryCode = model.PlantCode == null ? _factoryCode : model.PlantCode;
            //var flute = JsonConvert.DeserializeObject<Flute>(_fluteAPIRepository.GetFluteByFlute(factoryCode, model.Flute, _token));

            //int? a = flute.A;
            //int? b = flute.B;
            //int? c = flute.C;
            //int? d1 = flute.D1;
            //int? d2 = flute.D2;
            //int? join = flute.JoinSize;
            //int? slit = model.Slit;

            //int? hbl = 0;
            //int? hbr = 0;
            //int? hblx = 0;
            //int? hbrx = 0;
            //int? ll = 0;
            //int? wl = 0;
            //int? lr = 0;
            //int? wr = 0;
            //int? lid1 = model.ScoreW1 == null ? 0 : model.ScoreW1;
            //int? lid2 = model.Scorew3 == null ? 0 : model.Scorew3;
            //int? h1 = model.Scorew2 == null ? 0 : model.Scorew2;
            //int? shWid = 0;
            //int? shLen = 0;

            //int? sheet = 0;
            //int? box = 0;
            //int slot = 0;
            //double? sheetw = 0;
            //double? boxw = 0;
            //double basicw = model.Weight;

            //if (model.JoinSize != 0)
            //    join = model.JoinSize;

            //if (lid1 == 0 && h1 == 0 && lid2 == 0)
            //{
            //    if (model.rscStyle == "Full Overlap")
            //        lid1 = (int)Math.Floor(Convert.ToDecimal(Convert.ToDouble(w) + c));
            //    else
            //        lid1 = (int)Math.Floor(Convert.ToDecimal(Convert.ToDouble(w / 2) + c));

            //    lid2 = lid1;
            //}

            //if (model.rscStyle.Contains("Tele"))
            //    h1 = h + d2;
            //else
            //    h1 = h1 == 0 ? h + d1 : h1;

            //if (model.TwoPiece == true)
            //{
            //    if (model.GLWid == true)
            //    {
            //        ll = w + a;
            //        wl = l + b;
            //    }
            //    else
            //    {
            //        ll = model.ScoreL2 != 0 && model.ScoreL2 == model.ScoreL4 ? model.ScoreL2 : l + a;
            //        wl = model.ScoreL3 != 0 && model.ScoreL3 == model.ScoreL5 ? model.ScoreL3 : w + b;
            //        //ll = l + a;
            //        //wl = w + b;
            //    }

            //    lr = 0;
            //    wr = 0;

            //hblx = ll + wl;
            //hbrx = 0;

            //hbl = ll + join;
            //hbr = wl + slit;

            //shWid = lid1 + lid2 + h1;
            //shLen = ll + wl + join + slit;

            ////    hblx = l + a + w + b;
            ////    hbrx = 0;

            ////    hbl = l + a + join;
            ////    hbr = w + b + slit;

            ////    shWid = lid1 + lid2 + h1;
            ////    shLen = l + w + a + b + join + slit;
            //}

            //sheet = shWid * shLen;
            //slot = Convert.ToInt32((shWid - h1) * (21.5 + join) + (slit * shWid));
            //box = sheet - slot;

            //sheetw = (basicw * sheet / 1000000000);
            //boxw = (basicw * box / 1000000000);

            //modelx.Add(new ProductSpecViewModel()
            //{
            //    ScoreW1 = lid1,
            //    Scorew2 = h1,
            //    Scorew3 = lid2,
            //    JointLap = join,
            //    ScoreL2 = ll,
            //    ScoreL3 = wl,
            //    ScoreL4 = lr,
            //    ScoreL5 = wr,
            //    ScoreL6 = hblx,
            //    ScoreL7 = hbrx,
            //    ScoreL8 = hbl,
            //    ScoreL9 = hbr,
            //    CutSheetWid = shWid,
            //    CutSheetLeng = shLen,
            //    SheetArea = sheet,
            //    BoxArea = box,
            //    WeightSh = sheetw,
            //    WeightBox = boxw
            //});

            modelx.Add(new ProductSpecViewModel()
            {
                ScoreW1 = rsc.ScoreW1,
                Scorew2 = rsc.Scorew2,
                Scorew3 = rsc.Scorew3,
                JointLap = rsc.JointLap,
                ScoreL2 = rsc.ScoreL2,
                ScoreL3 = rsc.ScoreL3,
                ScoreL4 = rsc.ScoreL4,
                ScoreL5 = rsc.ScoreL5,
                ScoreL6 = rsc.ScoreL6,
                ScoreL7 = rsc.ScoreL7,
                ScoreL8 = rsc.ScoreL8,
                ScoreL9 = rsc.ScoreL9,
                CutSheetWid = rsc.CutSheetWid,
                CutSheetLeng = rsc.CutSheetLeng,
                SheetArea = rsc.SheetArea,
                BoxArea = rsc.BoxArea,
                WeightSh = rsc.WeightSh,
                WeightBox = rsc.WeightBox
            });

            return modelx;
        }

        public List<ProductSpecViewModel> ComputeDC(ProductSpecViewModel model)
        {
            List<ProductSpecViewModel> modelx = new List<ProductSpecViewModel>();

            RSCModel rscmodel = new RSCModel();
            rscmodel = mapper.Map<ProductSpecViewModel, RSCModel>(model);

            var rsc = JsonConvert.DeserializeObject<RSCResultModel>(_formulaAPIRepository.CalculateDC(_factoryCode, JsonConvert.SerializeObject(rscmodel), _token));
            //int? slit = 0;
            //int join = 0;

            //int? h1 = 0;
            //int? shWid = model.CutSheetWid;
            //int? shLen = model.CutSheetLeng;

            //int? sheet = 0;
            //int? sheetx = model.SheetArea;
            //int? box = model.BoxArea;
            //int? slot = 0;
            //double? sheetw = 0;
            //double? boxw = 0;
            //double basicw = model.Weight;

            //slot = Convert.ToInt32((shWid - h1) * (21.5 + join) + (slit * shWid));
            //sheet = shWid * shLen;
            //box = box == 0 || box == sheetx ? sheet : box;

            //sheetw = (basicw * sheet / 1000000000);
            //boxw = (basicw * box / 1000000000);

            //modelx.Add(new ProductSpecViewModel()
            //{
            //    SheetArea = sheet,
            //    BoxArea = box,
            //    WeightSh = sheetw,
            //    WeightBox = boxw
            //});

            modelx.Add(new ProductSpecViewModel()
            {
                SheetArea = rsc.SheetArea,
                BoxArea = rsc.BoxArea,
                WeightSh = rsc.WeightSh,
                WeightBox = rsc.WeightBox
            });

            return modelx;
        }

        public List<ProductSpecViewModel> ComputeSF(ProductSpecViewModel model)
        {
            List<ProductSpecViewModel> modelx = new List<ProductSpecViewModel>();
            RSCModel rscmodel = new RSCModel();
            rscmodel = mapper.Map<ProductSpecViewModel, RSCModel>(model);

            var rsc = JsonConvert.DeserializeObject<RSCResultModel>(_formulaAPIRepository.CalculateSF(_factoryCode, JsonConvert.SerializeObject(rscmodel), _token));
            //int? slit = 0;
            //int join = 0;

            //int? h1 = 0;
            //int? shWid = model.CutSheetWid;
            //int? shLen = model.CutSheetLeng;

            //int? sheet = 0;
            //int? sheetx = model.SheetArea;
            //int? box = model.BoxArea;
            //int? slot = 0;
            //double? sheetw = 0;
            //double? boxw = 0;
            //double basicw = model.Weight;

            //var kgLen = model.Flag == 1 ? shLen : Convert.ToInt32(1000000000 / (basicw * shWid));

            //sheet = shWid * kgLen;
            ////box = box == 0 || box == sheetx ? sheet : box;
            //box = sheet;

            //sheetw = (basicw * sheet / 1000000000);
            //boxw = (basicw * box / 1000000000);

            //modelx.Add(new ProductSpecViewModel()
            //{
            //    CutSheetLeng = kgLen,
            //    SheetArea = sheet,
            //    BoxArea = box,
            //    WeightSh = sheetw,
            //    WeightBox = boxw
            //});
            modelx.Add(new ProductSpecViewModel()
            {
                CutSheetLeng = rsc.CutSheetLeng,
                SheetArea = rsc.SheetArea,
                BoxArea = rsc.BoxArea,
                WeightSh = rsc.WeightSh,
                WeightBox = rsc.WeightBox
            });

            return modelx;
        }

        public List<ProductSpecViewModel> ComputeHC(ProductSpecViewModel model)
        {
            int? sheet = 0;
            int? box = 0;
            double? sheetw = 0;
            double? boxw = 0;

            List<ProductSpecViewModel> modelx = new List<ProductSpecViewModel>();
            RSCModel rscmodel = new RSCModel();
            rscmodel = mapper.Map<ProductSpecViewModel, RSCModel>(model);

            var rsc = JsonConvert.DeserializeObject<RSCResultModel>(_formulaAPIRepository.CalculateHC(_factoryCode, JsonConvert.SerializeObject(rscmodel), _token));

            //var factoryCode = model.PlantCode == null ? _factoryCode : model.PlantCode;
            //var flute = JsonConvert.DeserializeObject<Flute>(_fluteAPIRepository.GetFluteByFlute(factoryCode, model.Flute,_token));

            //int? join = flute.JoinSize;
            //int? height = flute.Height;
            //int? rangeGL = flute.B;
            //double? wheelGL = flute.C == 3 ? 3.5 : Convert.ToDouble(flute.C);

            //int? shWid = model.CutSheetWid;
            //int? shLen = model.CutSheetLeng;

            //var board = JsonConvert.DeserializeObject<List<BoardSpecWeight>>(_boardCombineAPIRepository.GetBoardSpecWeightByCode(_factoryCode, model.Code,_token));

            //var grade = board[0].PaperDes;

            //if (grade == "--000")
            //    grade = board[2].PaperDes;

            //var paper = JsonConvert.DeserializeObject<HoneyPaper>(_honeyPaperAPIRepository.GetHoneyPaperByGrade(_factoryCode, grade,_token));

            //var E = 73.63;
            //var So = 0.65;
            //var A = Convert.ToInt32(Math.Floor(Convert.ToDecimal(Convert.ToDouble(shWid) / (rangeGL + wheelGL))) - 2);   //จำนวนจุดทากาวแต่ละหน้ากว้าง เศษปัดลง

            //var kgPPm = (Convert.ToDouble(paper.Weight) / 1000) * (Convert.ToDouble(shWid) / 1000) * (Convert.ToDouble(height) / 1000) * paper.PaperAmt;
            //var Weight_DryGL2 = Convert.ToDouble(E * wheelGL * height * A * paper.PaperAmt / 1000000000) / So;

            //sheetw = kgPPm + Weight_DryGL2;
            //boxw = kgPPm + (Weight_DryGL2 * So);

            //shLen = Convert.ToInt32(Convert.ToDouble(1000) / boxw);
            //boxw = 1;
            //sheetw = 1;

            //sheet = shWid * shLen;
            //box = sheet;

            //modelx.Add(new ProductSpecViewModel()
            //{
            //    CutSheetLeng = shLen,
            //    SheetArea = sheet,
            //    BoxArea = box,
            //    WeightSh = sheetw,
            //    WeightBox = boxw
            //});

            modelx.Add(new ProductSpecViewModel()
            {
                CutSheetLeng = rsc.CutSheetLeng,
                SheetArea = rsc.SheetArea,
                BoxArea = rsc.BoxArea,
                WeightSh = rsc.WeightSh,
                WeightBox = rsc.WeightBox
            });

            return modelx;
        }

        public List<ProductSpecViewModel> ComputeHB(ProductSpecViewModel model)
        {
            int? sheet = 0;
            int? box = 0;
            double? sheetw = 0;
            double? boxw = 0;

            List<ProductSpecViewModel> modelx = new List<ProductSpecViewModel>();

            RSCModel rscmodel = new RSCModel();
            rscmodel = mapper.Map<ProductSpecViewModel, RSCModel>(model);

            var rsc = JsonConvert.DeserializeObject<RSCResultModel>(_formulaAPIRepository.CalculateHB(_factoryCode, JsonConvert.SerializeObject(rscmodel), _token));

            //var factoryCode = model.PlantCode == null ? _factoryCode : model.PlantCode;
            //var flute = JsonConvert.DeserializeObject<Flute>(_fluteAPIRepository.GetFluteByFlute(factoryCode, model.Flute,_token));

            //int? join = flute.JoinSize;
            //int? height = flute.Height;
            //int? rangeGL = flute.B;
            //double? wheelGL = flute.C == 3 ? 3.5 : Convert.ToDouble(flute.C);

            //int? shWid = model.CutSheetWid;
            //int? shLen = model.CutSheetLeng;

            //var board = JsonConvert.DeserializeObject<List<BoardSpecWeight>>(_boardCombineAPIRepository.GetBoardSpecWeightByCode(_factoryCode, model.Code,_token));

            //var grade = board[0].PaperDes;

            //if (grade == "--000")
            //    grade = board[2].PaperDes;

            //var paper = JsonConvert.DeserializeObject<HoneyPaper>(_honeyPaperAPIRepository.GetHoneyPaperByGrade(_factoryCode, grade,_token));

            //var E = 73.63;
            //var So = 0.65;
            //var percentGL3 = 0.1173;
            ////var A = Convert.ToInt32(Math.Floor(Convert.ToDecimal(Convert.ToDouble(shWid) / (rangeGL + wheelGL))) - 2);   //จำนวนจุดทากาวแต่ละหน้ากว้าง เศษปัดลง

            //var Wx = Convert.ToInt32(shWid / paper.Shrink);
            //var Lx = Convert.ToInt32(Convert.ToDouble(shLen) / paper.Stretch);
            //var Ax = Convert.ToInt32(Math.Floor(Convert.ToDecimal(Convert.ToDouble(Wx) / (rangeGL + wheelGL))) - 2);

            //if (board[4].PaperDes == "--000")
            //    board[4].PaperDes = board[3].PaperDes;

            //var B1 = board[1].PaperDes.Substring(board[1].PaperDes.Length - 3, 3);
            //var B4 = board[4].PaperDes.Substring(board[4].PaperDes.Length - 3, 3);
            //var PPLx = paper.PaperAmt * (Convert.ToDouble(Lx) / 1000);

            //var Weight_HCore = (paper.Weight * Wx * height * PPLx) / 1000000;
            //var Weight_DryGL2 = (E * (Convert.ToDouble(wheelGL) / 1000) * (Convert.ToDouble(height) / 1000) * Ax * PPLx);
            //var Weight_WetGL2 = Weight_DryGL2 / So;

            //var TopSheet = (Convert.ToInt32(B1) + Convert.ToInt32(B4)) * (Convert.ToDouble(shWid * shLen) / 1000000);
            //var ppWeight = Weight_HCore + Weight_WetGL2 + TopSheet;
            //var DryGL3 = Convert.ToDouble(percentGL3 * ppWeight) / 1000;
            //var Weight_DryGL3 = DryGL3 * Wx * Lx / 1000;
            //var Weight_WetGL3 = Weight_DryGL3 / So;

            //sheetw = (Weight_HCore + TopSheet + Weight_WetGL2 + Weight_WetGL3) / 1000;
            //boxw = (Weight_HCore + TopSheet + Weight_DryGL2 + Weight_DryGL3) / 1000;

            //sheet = shWid * shLen;
            //box = sheet;

            //modelx.Add(new ProductSpecViewModel()
            //{
            //    shrink = paper.Shrink,
            //    stretch = paper.Stretch,
            //    widHC = Wx,
            //    lenHC = Lx,
            //    SheetArea = sheet,
            //    BoxArea = box,
            //    WeightSh = sheetw,
            //    WeightBox = boxw
            //});

            modelx.Add(new ProductSpecViewModel()
            {
                shrink = rsc.shrink,
                stretch = rsc.stretch,
                widHC = rsc.widHC,
                lenHC = rsc.lenHC,
                SheetArea = rsc.SheetArea,
                BoxArea = rsc.BoxArea,
                WeightSh = rsc.WeightSh,
                WeightBox = rsc.WeightBox
            });

            return modelx;
        }

        public List<ProductSpecViewModel> ComputeCG(ProductSpecViewModel model)
        {
            List<ProductSpecViewModel> modelx = new List<ProductSpecViewModel>();

            RSCModel rscmodel = new RSCModel();
            rscmodel = mapper.Map<ProductSpecViewModel, RSCModel>(model);

            var rsc = JsonConvert.DeserializeObject<RSCResultModel>(_formulaAPIRepository.CalculateCG(_factoryCode, JsonConvert.SerializeObject(rscmodel), _token));

            //model.SideA = model.SideA == null ? 0 : model.SideA;
            //model.SideC = model.SideC == null ? 0 : model.SideC;
            //int? shWid = model.SideA + model.SideB + model.SideC;       //A+B+C
            //int? shLen = model.CutSheetLeng;                            //L

            //int? sheet = 0;
            //int? box = 0;
            //int? slot = 0;
            //double? sheetw = 0;
            //double? boxw = 0;
            //double basicw = model.Weight;
            //int? paperGram4Layer = 0;
            //int? paperLayer = 0;
            //int wrapLayer = 1;

            //var board = JsonConvert.DeserializeObject<List<BoardSpecWeight>>(_boardCombineAPIRepository.GetBoardSpecWeightByCode(_factoryCode, model.Code,_token));

            //foreach (var item in board)
            //{
            //    if (item.BasicWeight != 1)
            //    {
            //        paperGram4Layer = paperGram4Layer + item.BasicWeight;
            //        paperLayer = paperLayer + item.Layer;
            //    }
            //    else
            //    {
            //        break;
            //    }
            //}

            //var patchArea = shWid - (model.SideD * 2);            //(A+B+C) - (D*2)
            //var wrapGram = board.Last().BasicWeight * board.Last().Layer;   //E*e
            //var z = (Convert.ToDouble(paperGram4Layer + wrapGram) / (paperLayer + board.Last().Layer * 2 - 1) * (11.38 / 100)) / 1000;
            //sheet = shWid * shLen;
            //box = sheet;

            //double? paperWeight = 0;
            //double? glDryWeight = 0;

            //if (model.IsWrap == "Wrap")
            //{
            //    paperWeight = (Convert.ToDouble(paperGram4Layer) / 1000 * patchArea * shLen / 1000000) + (Convert.ToDouble(wrapGram) / 1000 * ((patchArea + model.SideD / 0.707) * 2 + 10) * shLen / 1000000);      //L-U หุ้มรอบ
            //    glDryWeight = ((Convert.ToDouble((paperLayer - 1) * patchArea * shLen) / 1000000) + (board.Last().Layer * ((patchArea + model.SideD / 0.707) * 2 + 10) * shLen / 1000000)) * z;   //L-U หุ้มรอบ
            //}
            //else
            //{
            //    paperWeight = (Convert.ToDouble(paperGram4Layer) / 1000 * patchArea * shLen / 1000000) + (Convert.ToDouble(wrapGram) / 1000 * (patchArea * 2) * shLen / 1000000);                        //L-U หุ้ม 2 ด้าน
            //    glDryWeight = ((Convert.ToDouble((paperLayer - 1) * patchArea * shLen) / 1000000) + (board.Last().Layer * (patchArea * 2) * shLen / 1000000)) * z;                    //L-U หุ้ม 2 ด้าน
            //}

            //var glWetWeight = glDryWeight / 0.65;
            //double degree = Convert.ToDouble(model.NotchDegree * 3.1416) / 360;

            //int? mmNotchHig = 0;
            //if (model.NotchSide == "A")
            //    mmNotchHig = model.SideA;
            //else if (model.NotchSide == "B")
            //    mmNotchHig = model.SideB;
            //else
            //    mmNotchHig = model.SideB;

            //int areaA = 0, areaC = 0;

            //if (model.IsNotch == true)
            //{
            //    if (model.CGType == "L")
            //    {
            //        //model.NotchArea = Convert.ToInt32(0.5 * Math.Pow(Convert.ToDouble(mmNotchHig - model.SideD) / Math.Sin(degreex), 2) * Math.Sin(degree));
            //        model.NotchArea = Convert.ToInt32(Math.Pow(Convert.ToDouble(mmNotchHig - model.SideD - 2), 2) * Math.Tan(degree));
            //    }
            //    else if (model.CGType == "U")
            //    {
            //        //areaA = Convert.ToInt32(0.5 * Math.Pow(Convert.ToDouble(model.SideA - model.SideD) / Math.Sin(degreex), 2) * Math.Sin(degree));
            //        //areaC = Convert.ToInt32(0.5 * Math.Pow(Convert.ToDouble(model.SideC - model.SideD) / Math.Sin(degreex), 2) * Math.Sin(degree));
            //        areaA = Convert.ToInt32(Math.Pow(Convert.ToDouble(model.SideA - model.SideD - 2), 2) * Math.Tan(degree));
            //        areaC = Convert.ToInt32(Math.Pow(Convert.ToDouble(model.SideC - model.SideD - 2), 2) * Math.Tan(degree));
            //        model.NotchArea = Convert.ToInt32(areaA + areaC);
            //    }
            //}

            //var ratio = Convert.ToDouble(model.NotchArea * model.No_Slot) / sheet;

            //sheetw = glWetWeight + paperWeight;
            //boxw = glDryWeight + paperWeight;

            //if (model.IsNotch == true)
            //{
            //    //sheet = sheet - (model.NotchArea * model.No_Slot);
            //    box = box - (model.NotchArea * model.No_Slot);      //TypeU No_Slot = 1 แปลว่า 1 คู่
            //    //sheetw = sheetw - (sheetw * ratio);
            //    boxw = boxw - (boxw * ratio);
            //}

            //modelx.Add(new ProductSpecViewModel()
            //{
            //    NotchArea = model.NotchArea,
            //    SheetArea = sheet,
            //    BoxArea = box,
            //    WeightSh = sheetw,
            //    WeightBox = boxw
            //});

            modelx.Add(new ProductSpecViewModel()
            {
                NotchArea = rsc.NotchArea,
                SheetArea = rsc.SheetArea,
                BoxArea = rsc.BoxArea,
                WeightSh = rsc.WeightSh,
                WeightBox = rsc.WeightBox
            });

            return modelx;
        }

        public List<ProductSpecViewModel> ComputeTeeth(ProductSpecViewModel model)
        {
            List<ProductSpecViewModel> modelx = new List<ProductSpecViewModel>();

            RSCModel rscmodel = new RSCModel();
            rscmodel = mapper.Map<ProductSpecViewModel, RSCModel>(model);

            var rsc = JsonConvert.DeserializeObject<RSCResultModel>(_formulaAPIRepository.CalculateAC(_factoryCode, JsonConvert.SerializeObject(rscmodel), _token));

            //int? slit = model.Slit;
            //int? n = model.No_Slot;

            //int? hbl = 0;
            //int? hbr = 0;
            //int? h1 = 0;
            //int? shWid = model.CutSheetWid;
            //int? shLen = model.CutSheetLeng;

            //int? sheet = 0;
            //int? box = 0;
            //int? slot = 0;
            //double? sheetw = 0;
            //double? boxw = 0;
            //double basicw = model.Weight;


            //h1 = model.Scorew2 != null? model.Scorew2 : shWid / 2;
            //hbl = model.ScoreL8 != null ? model.ScoreL8 : shLen / (n+1);
            //hbr = model.ScoreL9 != null ? model.ScoreL9 : hbl;
            //sheet = shWid * shLen;
            //slot = h1 * slit * n;
            //box = sheet - slot;

            //sheetw = (basicw * sheet / 1000000000);
            //boxw = (basicw * box / 1000000000);

            //modelx.Add(new ProductSpecViewModel()
            //{
            //    Scorew2 = h1,
            //    ScoreL8 = hbl,
            //    ScoreL9 = hbr,
            //    SheetArea = sheet,
            //    BoxArea = box,
            //    WeightSh = sheetw,
            //    WeightBox = boxw
            //});

            modelx.Add(new ProductSpecViewModel()
            {
                Scorew16 = rsc.Scorew16,
                ScoreL8 = rsc.ScoreL8,
                ScoreL9 = rsc.ScoreL9,
                SheetArea = rsc.SheetArea,
                BoxArea = rsc.BoxArea,
                WeightSh = rsc.WeightSh,
                WeightBox = rsc.WeightBox
            });

            return modelx;
        }

        public List<ProductSpecViewModel> ComputeWeightAndArea(ProductSpecViewModel model)
        {
            List<ProductSpecViewModel> modelx = new List<ProductSpecViewModel>();
            int? shWid = model.CutSheetWid;
            int? shLen = model.CutSheetLeng;
            int? slit = model.Slit;
            int? join = model.JointLap;
            int? n = model.No_Slot;
            int? h1 = model.Scorew2 == null || model.Scorew2 == 0 ? model.Scorew16 : model.Scorew2;
            double basicw = model.Weight;

            int? sheet = 0;
            int? box = model.BoxArea;
            int? slot = 0;
            double? sheetw = 0;
            double? boxw = 0;

            if (join == null)
            {
                slot = h1 * slit * n;
            }
            else
            {
                slot = Convert.ToInt32((shWid - h1) * (21.5 + join) + (slit * shWid));
            }

            sheet = shWid * shLen;

            if ((model.Flag != 1 || model.BoxArea == null || model.BoxArea == 0) && model.No_Slot == null)
                box = sheet - slot;
            else if (model.Wid != null && model.Leg != null && (model.BoxArea == null || model.BoxArea == 0))
                box = model.Wid * model.Leg - slot;


            sheetw = (basicw * sheet / 1000000000);
            boxw = (basicw * box / 1000000000);

            modelx.Add(new ProductSpecViewModel()
            {
                SheetArea = sheet,
                BoxArea = box,
                WeightSh = sheetw,
                WeightBox = boxw
            });

            return modelx;
        }

        public void SetPicData(string[] Base64)
        {
            TransactionDataModel model = new TransactionDataModel();

            try
            {
                model = SessionExtentions.GetSession<TransactionDataModel>(_httpContextAccessor.HttpContext.Session, "TransactionDataModel");
                if (model.modelProductSpec == null) model.modelProductSpec = new ProductSpecViewModel();
                model.modelProductSpec.PrintMasterPath = Base64[0];

                SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "TransactionDataModel", model);
            }
            catch (Exception ex)
            {
                model.modelProductPicture = new ProductPictureView();
            }
        }

        public List<ProductSpecViewModel> GetBoard(ProductSpecViewModel model)
        {
            List<ProductSpecViewModel> modelx = new List<ProductSpecViewModel>();
            var boardKiwi = model.BoardKIWI;
            var factoryCode = model.PlantCode == null ? _factoryCode : model.PlantCode;
            var weight = Convert.ToDouble(GetBasisWeight(model.Code, model.Flute, factoryCode));
            var mapcost = JsonConvert.DeserializeObject<MapCost>(_mapCostAPIRepository.GetCostField(_factoryCode, model.lv2, model.lv3, model.lv4, _token));
            model.costField = mapcost == null ? model.costField : mapcost.CostField;
            var flute = JsonConvert.DeserializeObject<Flute>(_fluteAPIRepository.GetFluteByFlute(factoryCode, model.Flute, _token));
            if (flute == null)
                flute = JsonConvert.DeserializeObject<Flute>(_fluteAPIRepository.GetFluteByFlute(_factoryCode, model.Flute, _token));

            var StationList = JsonConvert.DeserializeObject<List<FluteTr>>(_fluteTrAPIRepository.GetFluteTrList(factoryCode, _token)).Where(m => m.FluteCode == model.Flute).Distinct().ToList();

            decimal? costPerTon = 0;
            decimal? costOEM = model.CostOEM == null ? 0 : model.CostOEM;
            int? jointLap = model.JointLap;

            if (model.Code == model.Code1 && model.CostPerTon != 0 && model.CostPerTon != null)
            {
                costPerTon = model.CostPerTon;
            }
            else
            {
                if (model.Code != model.Code1)
                {
                    jointLap = flute == null ? 0 : flute.JoinSize;
                }

                var cost = JsonConvert.DeserializeObject<Cost>(_boardCombineAccAPIRepository.GetCost(_factoryCode, model.Code, model.costField, _token));
                if (cost != null)
                {
                    costPerTon = cost.CostPerTon == null ? 0 : Convert.ToDecimal(cost.CostPerTon);
                }
            }


            if (factoryCode != _factoryCode)
            {
                if (costOEM == 0)
                {
                    var cost = JsonConvert.DeserializeObject<Cost>(_boardCombineAccAPIRepository.GetCost(factoryCode, model.Code, model.costField, _token));
                    if (cost != null)
                    {
                        costOEM = cost.CostPerTon == null ? 0 : Convert.ToDecimal(cost.CostPerTon);
                    }
                }
            }


            if (boardKiwi == null)
                boardKiwi = _newProductService.GetBoardKIWI(model.BoardCombine);

            if (flute == null)
            {
                modelx.Add(new ProductSpecViewModel()
                {
                    Hierarchy = "03" + model.lv2.Trim() + model.lv3 + model.lv4 + model.Code
                });
            }
            else
            {
                modelx.Add(new ProductSpecViewModel()
                {
                    BoardKIWI = boardKiwi,
                    Weight = weight,
                    StationList = StationList,
                    A = flute.A,
                    B = flute.B,
                    C = flute.C,
                    D1 = flute.D1,
                    D2 = flute.D2,
                    JoinSize = flute.JoinSize,
                    JointLap = jointLap,
                    Height = flute.Height,
                    CostPerTon = costPerTon,
                    CostOEM = costOEM,
                    Hierarchy = "03" + model.lv2.Trim() + model.lv3 + model.lv4 + model.Code
                });
            }

            return modelx;
        }

        public ProductSpecViewModel CriteriaBoardFromThickness(ProductSpecViewModel model)
        {
            var modelx = SessionExtentions.GetSession<TransactionDataModel>(_httpContextAccessor.HttpContext.Session, "TransactionDataModel");

            if (model.IsWrap == null)
            {
                model.BoardLists = modelx.modelProductSpec.BoardLists.Where(b => b.Thickness == model.SideD).ToList();
            }
            else
            {
                if (model.IsWrap == "Wrap")
                    model.BoardLists = modelx.modelProductSpec.BoardLists.Where(b => b.Thickness == model.SideD && b.Board.Contains("O1")).ToList();
                else
                    model.BoardLists = modelx.modelProductSpec.BoardLists.Where(b => b.Thickness == model.SideD && !b.Board.Contains("O1")).ToList();
            }
            return model;
        }

        public ProductSpecViewModel CriteriaBoardHoney(ProductSpecViewModel model)
        {
            var modelx = SessionExtentions.GetSession<TransactionDataModel>(_httpContextAccessor.HttpContext.Session, "TransactionDataModel");

            if (model.Hig != null && model.JointLap != null && model.JointLap != 0)
                model.BoardLists = modelx.modelProductSpec.BoardLists.Where(b => b.Height == model.Hig && b.JoinSize == model.JointLap).ToList();
            else if (model.Hig != null && (model.JointLap == null || model.JointLap == 0))
                model.BoardLists = modelx.modelProductSpec.BoardLists.Where(b => b.Height == model.Hig).ToList();
            else if (model.Hig == null && model.JointLap != null && model.JointLap != 0)
                model.BoardLists = modelx.modelProductSpec.BoardLists.Where(b => b.JoinSize == model.JointLap).ToList();
            else
                model.BoardLists = modelx.modelProductSpec.BoardLists;

            return model;
        }

    }
}
