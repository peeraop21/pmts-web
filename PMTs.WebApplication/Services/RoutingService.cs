using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using PMTs.DataAccess;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.NewProduct;
using PMTs.DataAccess.Repository;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Tracing;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services
{
    [TraceAspect]
    public class RoutingService : IRoutingService
    {
        private UserSessionModel userSessionModel;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly INewProductService _newProductService;
        private readonly IMachineAPIRepository _machineAPIRepository;
        private readonly IColorAPIRepository _colorAPIRepository;
        private readonly ICorConfigAPIRepository _corConfigAPIRepository;
        private readonly IPaperWidthAPIRepository _paperWidthAPIRepository;
        private readonly IFluteAPIRepository _fluteAPIRepository;
        private readonly IBoardUseAPIRepository _boardUseAPIRepository;
        private readonly IPaperGradeAPIRepository _paperGradeAPIRepository;
        private readonly IBuildRemarkAPIRepository _buildRemarkAPIRepository;
        private readonly IScoreGapAPIRepository _scoreGapAPIRepository;
        private readonly IScoreTypeAPIRepository _scoreTypeAPIRepository;
        private readonly IMasterDataAPIRepository _masterDataAPIRepository;
        private readonly IPlantViewAPIRepository _plantViewAPIRepository;
        private readonly IMachineGroupAPIRepository _machineGroupAPIRepository;
        private readonly IFormulaAPIRepository _formulaAPIRepository;
        private readonly IQualitySpecAPIRepository _qualitySpecAPIRepository;

        private readonly IRoutingAPIRepository _routingAPIRepository;

        //private readonly IPaperWidthRepository _paperWidthRepository;
        //tassanai update 11012022
        private readonly IPMTsConfigAPIRepository _pmtsConfigAPIRepository;

        private readonly IProductTypeAPIRepository _productTypeAPIRepository;

        private readonly string _username;
        private readonly string _saleOrg;
        private readonly string _factoryCode;
        private readonly string _token;

        public RoutingService(IHttpContextAccessor httpContextAccessor,
            INewProductService newProductService,
            IMachineAPIRepository machineAPIRepository,
            IColorAPIRepository colorAPIRepository,
            ICorConfigAPIRepository corConfigAPIRepository,
            IPaperWidthAPIRepository paperWidthAPIRepository,
            IFluteAPIRepository fluteAPIRepository,
            IBoardUseAPIRepository boardUseAPIRepository,
            IPaperGradeAPIRepository paperGradeAPIRepository,
            IBuildRemarkAPIRepository buildRemarkAPIRepository,
            IRoutingAPIRepository routingAPIRepository,
            IScoreGapAPIRepository scoreGapAPIRepository,
            IScoreTypeAPIRepository scoreTypeAPIRepository,
            IMasterDataAPIRepository masterDataAPIRepository,
            IPlantViewAPIRepository plantViewAPIRepository,
            IMachineGroupAPIRepository machineGroupAPIRepository,
            IFormulaAPIRepository formulaAPIRepository,
            IQualitySpecAPIRepository qualitySpecAPIRepository,

            //tassanai update 11012022
            IPMTsConfigAPIRepository pmtsConfigAPIRepository,
            IProductTypeAPIRepository productTypeAPIRepository
            )
        {
            _httpContextAccessor = httpContextAccessor;
            _newProductService = newProductService;
            _machineAPIRepository = machineAPIRepository;
            _colorAPIRepository = colorAPIRepository;
            _corConfigAPIRepository = corConfigAPIRepository;
            _paperWidthAPIRepository = paperWidthAPIRepository;
            _fluteAPIRepository = fluteAPIRepository;
            _boardUseAPIRepository = boardUseAPIRepository;
            _paperGradeAPIRepository = paperGradeAPIRepository;
            _buildRemarkAPIRepository = buildRemarkAPIRepository;
            _routingAPIRepository = routingAPIRepository;
            _scoreGapAPIRepository = scoreGapAPIRepository;
            _scoreTypeAPIRepository = scoreTypeAPIRepository;
            _masterDataAPIRepository = masterDataAPIRepository;
            _plantViewAPIRepository = plantViewAPIRepository;
            _machineGroupAPIRepository = machineGroupAPIRepository;
            _formulaAPIRepository = formulaAPIRepository;
            _qualitySpecAPIRepository = qualitySpecAPIRepository;

            //tassanai update 11012022
            _pmtsConfigAPIRepository = pmtsConfigAPIRepository;
            _productTypeAPIRepository = productTypeAPIRepository;
            userSessionModel = SessionExtentions.GetSession<UserSessionModel>(_httpContextAccessor.HttpContext.Session, "UserSessionModel");
            if (userSessionModel != null)
            {
                _username = userSessionModel.UserName;
                _saleOrg = userSessionModel.SaleOrg;
                _factoryCode = userSessionModel.FactoryCode;
                _token = userSessionModel.Token;
            }
        }

        public List<Machine> GetMachineList(string keywordMachine, string machineGroup)
        {
            List<Machine> result = new List<Machine>();
            if (String.IsNullOrEmpty(machineGroup) && String.IsNullOrEmpty(keywordMachine))
            {
                result = JsonConvert.DeserializeObject<List<Machine>>(_machineAPIRepository.GetMachineList(_factoryCode, _token)).Where(x => x.MachineStatus == true).ToList();
            }
            else if (String.IsNullOrEmpty(machineGroup) && !String.IsNullOrEmpty(keywordMachine))
            {
                result = JsonConvert.DeserializeObject<List<Machine>>(_machineAPIRepository.GetMachineList(_factoryCode, _token)).Where(w => w.Machine1.ToUpper().Contains(keywordMachine.ToUpper()) && w.MachineStatus == true).ToList();
            }
            else if (!String.IsNullOrEmpty(machineGroup) && String.IsNullOrEmpty(keywordMachine))
            {
                result = JsonConvert.DeserializeObject<List<Machine>>(_machineAPIRepository.GetMachineList(_factoryCode, _token)).Where(w => w.MachineGroup == machineGroup && w.MachineStatus == true).ToList();
            }
            else if (!String.IsNullOrEmpty(machineGroup) && !String.IsNullOrEmpty(keywordMachine))
            {
                result = JsonConvert.DeserializeObject<List<Machine>>(_machineAPIRepository.GetMachineList(_factoryCode, _token)).Where(w => w.Machine1.ToUpper().Contains(keywordMachine.ToUpper()) && w.MachineGroup.ToUpper() == machineGroup.ToUpper() && w.MachineStatus == true).ToList();
            }
            return result;
        }

        public Machine GetMachine(string machine)
        {
            Machine _machine = new Machine();
            _machine = JsonConvert.DeserializeObject<Machine>(_machineAPIRepository.GetMachineGroupByMachine(_factoryCode, machine, _token));
            return _machine;
        }

        public List<Machine> GetMachineGroupList()
        {
            return JsonConvert.DeserializeObject<List<Machine>>(_machineAPIRepository.GetMachineList(_factoryCode, _token)).ToList();
        }

        public string GetMachineGroupByMachine(string machine)
        {
            Machine _machine = new Machine();
            _machine = JsonConvert.DeserializeObject<Machine>(_machineAPIRepository.GetMachineGroupByMachine(_factoryCode, machine, _token));
            return _machine == null ? "" : _machine.MachineGroup.ToString();
        }

        private string GetMachineCodeByMachine(string machine)
        {
            Machine _machine = new Machine();
            _machine = JsonConvert.DeserializeObject<Machine>(_machineAPIRepository.GetMachineGroupByMachine(_factoryCode, machine, _token));
            return _machine == null ? "" : _machine.Code.ToString();
        }

        public string GetMachineDataByMachine(string machine)
        {
            Machine _machine = new Machine();
            _machine = JsonConvert.DeserializeObject<Machine>(_machineAPIRepository.GetMachineGroupByMachine(_factoryCode, machine, _token));
            return _machine.MachineGroup.ToString();
        }

        public List<Machine> GetMachineDataByFactorycode(string factorycode)
        {
            List<Machine> _machine = new List<Machine>();
            _machine = JsonConvert.DeserializeObject<List<Machine>>(_machineAPIRepository.GetMachineList(factorycode, _token));
            return _machine;
        }

        public bool GetMachineDataPlatenAndRotalyByMachine(string machine)
        {
            Machine _machine = new Machine();
            _machine = JsonConvert.DeserializeObject<Machine>(_machineAPIRepository.GetMachineGroupByMachine(_factoryCode, machine, _token));
            //if (_machine.Platen.ToString() == "-1")
            //{ return true; }
            //else if (_machine.Rotary.ToString() == "-1")
            //{
            //    return true;
            //}
            //else { return false; }
            if (_machine.IsPropPrint == true)
            {
                if (_machine.Platen == true || _machine.Rotary == true)
                { return true; }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public List<BuildRemark> GetRemarkList(string keywordRemark)
        {
            List<BuildRemark> result = new List<BuildRemark>();
            if (String.IsNullOrEmpty(keywordRemark))
            {
                var resulttmp = JsonConvert.DeserializeObject<List<BuildRemark>>(_buildRemarkAPIRepository.GetBuildRemarkList(_factoryCode, _token)).ToList();
                result = resulttmp;
            }
            else
            {
                var resulttmp = JsonConvert.DeserializeObject<List<BuildRemark>>(_buildRemarkAPIRepository.GetBuildRemarkList(_factoryCode, _token)).ToList().Where(w => !string.IsNullOrEmpty(w.List) && w.List.ToString().Contains(keywordRemark));
                result = resulttmp.ToList();
            }
            return result;
        }

        public List<int> GetPlatenRotary(string machine)
        {
            List<int> resultList = new List<int>();
            var result = JsonConvert.DeserializeObject<List<Machine>>(_machineAPIRepository.GetMachineList(_factoryCode, _token)).ToList().Where(w => w.Machine1 == machine).Select(s => new { isPropCor = Convert.ToInt16(s.IsPropCor), isPropPrint = Convert.ToInt16(s.IsPropPrint), isPropDieCut = Convert.ToInt16(s.IsPropDieCut), isCalPaperwidth = Convert.ToInt16(s.IsCalPaperwidth), isRepeatLength = Convert.ToInt16(s.IsPropRepeatLenght), isMCMove = Convert.ToInt16(s.McMove) }).ToList();
            resultList.Add(result.First().isPropCor == null ? 0 : result.First().isPropCor);
            resultList.Add(result.First().isPropPrint == null ? 0 : result.First().isPropPrint);
            resultList.Add(result.First().isPropDieCut == null ? 0 : result.First().isPropDieCut);
            resultList.Add(result.First().isCalPaperwidth == null ? 0 : result.First().isCalPaperwidth);
            resultList.Add(result.First().isRepeatLength == null ? 0 : result.First().isRepeatLength);
            resultList.Add(result.First().isMCMove == null ? 0 : result.First().isMCMove);
            return resultList;
        }

        public List<Color> GetInkShadeList()
        {
            return JsonConvert.DeserializeObject<List<Color>>(_colorAPIRepository.GetColorList(_factoryCode, _token)).ToList();
        }

        public CorConfig GetCorConfigByName(string machine)
        {
            return JsonConvert.DeserializeObject<List<CorConfig>>(_corConfigAPIRepository.GetCorConfigList(_factoryCode, _token)).ToList().Where(w => w.Name == machine).FirstOrDefault();
        }

        public List<Color> GetShadeByInkList(string inkName)
        {
            if (inkName == null)
            {
                return JsonConvert.DeserializeObject<List<Color>>(_colorAPIRepository.GetColorList(_factoryCode, _token)).OrderBy(o => o.Shade).ToList();
            }
            else
            {
                return JsonConvert.DeserializeObject<List<Color>>(_colorAPIRepository.GetColorList(_factoryCode, _token)).ToList().Where(w => w.Ink == inkName).OrderBy(o => o.Shade).ToList();
            }
        }

        public List<ScoreGap> GetScoreGapList(string flut, string scoretypeid)
        {
            return JsonConvert.DeserializeObject<List<ScoreGap>>(_scoreGapAPIRepository.GetScoreGapList(_factoryCode, _token)).ToList().Where(w => w.Flute == flut && w.ScoreType == scoretypeid).ToList();
        }

        public List<Color> GetInkByShadeList(string shadeName)
        {
            return JsonConvert.DeserializeObject<List<Color>>(_colorAPIRepository.GetColorList(_factoryCode, _token)).ToList().Where(w => w.Shade == shadeName).OrderBy(o => o.Ink).ToList();
        }

        public List<Routing> GetRoutingDataByMaterialNo(string MaterialNo)
        {
            return JsonConvert.DeserializeObject<List<Routing>>(_routingAPIRepository.GetRoutingsByMaterialNo(_factoryCode, MaterialNo, _token)).ToList();//.Where(w => w.MaterialNo == MaterialNo).ToList();
        }

        public List<string> GetPaperItemByMaterialNo(string MaterialNo)
        {
            var BoardUse = JsonConvert.DeserializeObject<List<BoardUse>>(_boardUseAPIRepository.GetBoardUseList(_factoryCode, _token)).Where(w => w.MaterialNo == MaterialNo).FirstOrDefault();
            if (BoardUse == null)
            {
                return null;
            }

            List<string> result = new List<string>();
            if (BoardUse.Gl != null)
            {
                result.Add(BoardUse.Gl);
            }
            if (BoardUse.Bm != null)
            {
                result.Add(BoardUse.Bm);
            }
            if (BoardUse.Bl != null)
            {
                result.Add(BoardUse.Bl);
            }
            if (BoardUse.Cm != null)
            {
                result.Add(BoardUse.Cm);
            }
            if (BoardUse.Cl != null)
            {
                result.Add(BoardUse.Cl);
            }
            if (BoardUse.Dm != null)
            {
                result.Add(BoardUse.Dm);
            }
            if (BoardUse.Dl != null)
            {
                result.Add(BoardUse.Dl);
            }

            return result;
        }

        public List<CorConfig> GetCoreConfig()
        {
            return JsonConvert.DeserializeObject<List<CorConfig>>(_corConfigAPIRepository.GetCorConfigList(_factoryCode, _token)).ToList();
        }

        //---------- method in newcontroller old
        public void BindDataToModel(TransactionDataModel model)
        {
            TransactionDataModel modelSession = SessionExtentions.GetSession<TransactionDataModel>(_httpContextAccessor.HttpContext.Session, "TransactionDataModel");

            if (model.modelRouting != null)
            {
                model.modelRouting.RoutingDataList = modelSession.modelRouting.RoutingDataList;
            }
            else
            {
                model.modelRouting = new RoutingViewModel();
                model.modelRouting.RoutingDataList = new List<RoutingDataModel>();
                //model.modelRouting.RoutingDataList = AutoRoutingList(model).OrderBy(x => x.SeqNo).ToList();
            }

            model.MaterialNo = modelSession.MaterialNo;

            if (modelSession.modelProductSpec != null)
            {
                model.modelRouting.WeightSheetDefault = Math.Round(Convert.ToDecimal(modelSession.modelProductSpec.WeightSh), 3).ToString();

                if (modelSession.modelCategories.Id_SU == 6)
                {
                    var sum = modelSession.modelProductSpec.WeightBox * modelSession.modelProductProp.PiecePatch;
                    model.modelRouting.WeightSelectList = new SelectList(new List<SelectListItem> {
                new SelectListItem { Selected = false, Text = Math.Round(Convert.ToDecimal(modelSession.modelProductSpec.WeightSh), 3).ToString(), Value = Math.Round(Convert.ToDecimal(modelSession.modelProductSpec.WeightSh), 3).ToString() },
                new SelectListItem { Selected = false, Text = Math.Round(Convert.ToDecimal(modelSession.modelProductSpec.WeightBox), 3).ToString(), Value = Math.Round(Convert.ToDecimal(modelSession.modelProductSpec.WeightBox), 3).ToString() },
                new SelectListItem { Selected = false, Text = Math.Round(Convert.ToDecimal(sum), 3).ToString(), Value = Math.Round(Convert.ToDecimal(sum), 3).ToString() },
                }, "Value", "Text", 1);
                }
                else
                {
                    model.modelRouting.WeightSelectList = new SelectList(new List<SelectListItem> {
                new SelectListItem { Selected = false, Text = Math.Round(Convert.ToDecimal(modelSession.modelProductSpec.WeightSh), 3).ToString(), Value = Math.Round(Convert.ToDecimal(modelSession.modelProductSpec.WeightSh), 3).ToString() },
                new SelectListItem { Selected = false, Text = Math.Round(Convert.ToDecimal(modelSession.modelProductSpec.WeightBox), 3).ToString(), Value = Math.Round(Convert.ToDecimal(modelSession.modelProductSpec.WeightBox), 3).ToString() },
                }, "Value", "Text", 1);
                }

                model.modelRouting.SheetLengthIn = modelSession.modelProductSpec.CutSheetLeng.ToString();
                model.modelRouting.SheetLengthOut = modelSession.modelProductSpec.CutSheetLeng.ToString();
                // model.modelRouting.SheetLengthOut = modelSession.modelCategories.FormGroup == "STDRSC" ? (modelSession.modelProductSpec.CutSheetLeng - modelSession.modelProductSpec.Slit).ToString() :   modelSession.modelProductSpec.CutSheetLeng.ToString();
                model.modelRouting.SheetWidthIn = modelSession.modelProductSpec.CutSheetWid.ToString();
                model.modelRouting.SheetWidthOut = modelSession.modelProductSpec.CutSheetWid.ToString();
            }

            model.modelRouting.MachineGroupSelectList = JsonConvert.DeserializeObject<List<GroupMachineModels>>(_machineGroupAPIRepository.GetByMachineGroupJoinMachine(_factoryCode, _token)).Select(sli => new SelectListItem { Value = sli.Id, Text = sli.GroupMachine });
            model.modelRouting.InkSelectList = JsonConvert.DeserializeObject<List<Color>>(_colorAPIRepository.GetColorList(_factoryCode, _token)).OrderBy(o => o.Ink).Select(s => s.Ink).Distinct().Select(sli => new SelectListItem { Value = sli, Text = sli });
            model.modelRouting.ShadeSelectList = JsonConvert.DeserializeObject<List<Color>>(_colorAPIRepository.GetColorList(_factoryCode, _token)).OrderBy(o => o.Shade).Select(s => s.Shade).Distinct().Select(sli => new SelectListItem { Value = sli, Text = sli });

            model.modelRouting.ScoreTypelist = JsonConvert.DeserializeObject<List<ScoreType>>(_scoreTypeAPIRepository.GetScoreTypeList(_factoryCode, _token)).Select(s => new { s.ScoreTypeId, s.ScoreTypeName }).Distinct().Select(sli => new SelectListItem { Value = sli.ScoreTypeId.ToString(), Text = sli.ScoreTypeName });
            model.modelRouting.ScoreGapList = JsonConvert.DeserializeObject<List<ScoreGap>>(_scoreGapAPIRepository.GetScoreGapsByFactoryCode(_factoryCode, _token)); ;

            model.modelBuildRemark = JsonConvert.DeserializeObject<List<BuildRemark>>(_buildRemarkAPIRepository.GetBuildRemarkList(_factoryCode, _token));
            model.modelBuildRemark = model.modelBuildRemark.OrderBy(x => x.List).ToList();
            model.modelGroupMachineRemark = new List<string>();
            var listtemp = model.modelBuildRemark.GroupBy(x => new { x.Machine }).ToList();
            foreach (var item in listtemp)
            {
                model.modelGroupMachineRemark.Add(item.Key.Machine);
            }
            model.modelGroupMachineRemark = model.modelGroupMachineRemark.OrderBy(x => x).ToList();
            model.amountColor = (Int16)(string.IsNullOrEmpty(modelSession.modelProductProp.AmountColor.ToString()) ? 0 : modelSession.modelProductProp.AmountColor);

            _newProductService.SetTransactionStatus(ref model, "ProductRouting");
        }

        public List<string> GetWeight(TransactionDataModel modelSession)
        {
            List<string> weightList = new List<string>();
            if (modelSession.modelCategories.Id_SU == 6)
            {
                var sum = modelSession.modelProductSpec.WeightBox * modelSession.modelProductProp.PiecePatch;
                weightList.Add(Math.Round(Convert.ToDecimal(modelSession.modelProductSpec.WeightSh), 3).ToString());
                weightList.Add(Math.Round(Convert.ToDecimal(modelSession.modelProductSpec.WeightBox), 3).ToString());
                weightList.Add(Math.Round(Convert.ToDecimal(sum), 3).ToString());
            }
            else
            {
                weightList.Add(Math.Round(Convert.ToDecimal(modelSession.modelProductSpec.WeightSh), 3).ToString());
                weightList.Add(Math.Round(Convert.ToDecimal(modelSession.modelProductSpec.WeightBox), 3).ToString());
            }
            return weightList;
        }

        public RoutingDataModel CalculateCorProp()
        {
            return new RoutingDataModel();
        }

        public RoutingDataModel CalculateRouting(string machineName)
        {
            RoutingDataModel ret = new RoutingDataModel();

            try
            {
                CorConfig corConfig = JsonConvert.DeserializeObject<CorConfig>(_corConfigAPIRepository.GetCorConfigByFactoryCode(_factoryCode, machineName, _token));
                TransactionDataModel tran = SessionExtentions.GetSession<TransactionDataModel>(_httpContextAccessor.HttpContext.Session, "TransactionDataModel");
                var flute = JsonConvert.DeserializeObject<Flute>(_fluteAPIRepository.GetFluteByFlute(_factoryCode, tran.modelProductSpec.Flute, _token));
                var RollWidth = JsonConvert.DeserializeObject<List<PaperWidth>>(_paperWidthAPIRepository.GetPaperWidthList(_factoryCode, _token)).OrderBy(o => o.Group2).ToList();

                int sheetIn_W = (tran.modelProductSpec.CutSheetWid).GetValueOrDefault();
                int maxCut = corConfig.CutOff;
                int trimWaste = (int)(flute.Trim);
                int? sizeWidth = 0;
                int pageMin = corConfig.MinOut;
                int pageMax = corConfig.MaxOut;
                //Mintrim

                if (corConfig.Mintrim) //Min Trim
                {
                    var PaperItem = JsonConvert.DeserializeObject<List<string>>(_boardUseAPIRepository.GetPaperItemByMaterialNo(_factoryCode, tran.MaterialNo, _token));
                    PaperGrade PaperGrad;
                    int GroupItem = 10000;
                    double[,] RollSize = new double[6, 4];
                    int X, M;
                    // Comming Soon
                    foreach (var Item in PaperItem)
                    {
                        PaperGrad = JsonConvert.DeserializeObject<PaperGrade>(_paperGradeAPIRepository.GetPaperGradeByGradeAndActive(_factoryCode, Item, _token));

                        if (PaperGrad.Group < GroupItem)
                        {
                            GroupItem = PaperGrad.Group;
                        }
                    }
                    ret.Trim = "1000";//กำหนดค่าหลอกเพื่อไปเปรียบเทียบกับ % Trimน้อยสุด

                    var Roll = RollWidth.FirstOrDefault(w => w.Group1 == pageMin || w.Group2 == pageMin || w.Group3 == pageMin || w.Group4 == pageMin);

                    if (Roll != null)
                    {
                        pageMin = GroupItem == 1 ? ConvertInt16ToShort(Roll.Group1) : GroupItem == 2 ? ConvertInt16ToShort(Roll.Group2) : GroupItem == 3 ? ConvertInt16ToShort(Roll.Group3) : ConvertInt16ToShort(Roll.Group4);
                    }

                    //for (X = 0; X < RollWidth.Count; X++)
                    //{
                    //        if (ConvertInt16ToShort(RollWidth[X].Group1) > pageMin) break;
                    //        if (ConvertInt16ToShort(RollWidth[X].Group1) == pageMin)
                    //        {
                    //            pageMin = GroupItem == 1 ? ConvertInt16ToShort(RollWidth[X].Group1) : GroupItem == 2 ? ConvertInt16ToShort(RollWidth[X].Group2) : GroupItem == 3 ? ConvertInt16ToShort(RollWidth[X].Group3) : ConvertInt16ToShort(RollWidth[X].Group4);
                    //            break;
                    //        }

                    //}

                    for (X = 0; X < maxCut; X++) //คำนวนหน้ากว้าง + Standard Trim
                    {
                        RollSize[X, 1] = (sheetIn_W * (X + 1)) + trimWaste;
                        if (RollSize[X, 1] < pageMin) RollSize[X, 0] = pageMin;   //น้อยกว่าหน้าน้อยสุด
                        else if (RollSize[X, 1] > pageMax) RollSize[X, 0] = 0;     //มากกว่าหน้าสูงสุด
                        else
                        {
                            switch (GroupItem)
                            {
                                case 1:
                                    for (M = 0; M < RollWidth.Count; M++)
                                    {
                                        if (ConvertInt16ToShort(RollWidth[M].Group1) >= pageMin && ConvertInt16ToShort(RollWidth[M].Group1) <= pageMax)
                                        {
                                            if (RollSize[X, 1] <= ConvertInt16ToShort(RollWidth[M].Group1))
                                            {
                                                RollSize[X, 0] = ConvertInt16ToShort(RollWidth[M].Group1);
                                                break;
                                            }
                                        }
                                    }
                                    break;

                                case 2:
                                    for (M = 0; M < RollWidth.Count; M++)
                                    {
                                        if (ConvertInt16ToShort(RollWidth[M].Group2) >= pageMin && ConvertInt16ToShort(RollWidth[M].Group2) <= pageMax)
                                        {
                                            if (RollSize[X, 1] <= ConvertInt16ToShort(RollWidth[M].Group2))
                                            {
                                                RollSize[X, 0] = ConvertInt16ToShort(RollWidth[M].Group2);
                                                break;
                                            }
                                        }
                                    }
                                    break;

                                case 3:
                                    for (M = 0; M < RollWidth.Count; M++)
                                    {
                                        if (ConvertInt16ToShort(RollWidth[M].Group3) >= pageMin && ConvertInt16ToShort(RollWidth[M].Group3) <= pageMax)
                                        {
                                            if (RollSize[X, 1] <= ConvertInt16ToShort(RollWidth[M].Group3))
                                            {
                                                RollSize[X, 0] = ConvertInt16ToShort(RollWidth[M].Group3);
                                                break;
                                            }
                                        }
                                    }
                                    break;

                                case 4:
                                    for (M = 0; M < RollWidth.Count; M++)
                                    {
                                        if (ConvertInt16ToShort(RollWidth[M].Group4) >= pageMin && ConvertInt16ToShort(RollWidth[M].Group4) <= pageMax)
                                        {
                                            if (RollSize[X, 1] <= ConvertInt16ToShort(RollWidth[M].Group4))
                                            {
                                                RollSize[X, 0] = ConvertInt16ToShort(RollWidth[M].Group4);
                                                break;
                                            }
                                        }
                                    }
                                    break;
                            }

                            if (RollSize[X, 0] > 0)
                            {
                                RollSize[X, 2] = (RollSize[X, 0] - (RollSize[X, 1] - trimWaste)) / RollSize[X, 0] * 100; //คำนวน % Trim

                                if (ConvertStringToShort(ret.Trim) >= Math.Round(RollSize[X, 3], 2))//เลือก % Trim น้อยที่สุด
                                {
                                    ret.PaperRollWidth = RollSize[X, 0].ToString();
                                    ret.Cut = (X + 1).ToString();
                                    ret.Trim = (RollSize[X, 0] - RollSize[X, 1] + trimWaste).ToString(); //เศษ
                                    ret.PercentTrim = (Math.Round(RollSize[X, 2], 2)).ToString();
                                }
                            }
                        }
                    }
                }
                else //Max Out
                {
                    sizeWidth = (sheetIn_W * maxCut) + trimWaste;

                    //ตรวจหาหน้ากว้างสุดและแคบสุด
                    if (sizeWidth < pageMin)
                    {
                        ret.PaperRollWidth = pageMin.ToString();                                //Paper Width
                        ret.Cut = "1";                                                          //จำนวนตัด
                        ret.Trim = (pageMin - sheetIn_W).ToString();                            //เศษตัดริม
                        ret.PercentTrim = Math.Round((Double.Parse(ret.Trim) / Convert.ToDouble(pageMin) * 100), 2).ToString();   //% Waste
                        return ret;
                    }

                    /////////////////////////////////////////////////////////////

                    if (sheetIn_W + trimWaste > pageMax)
                    {
                        ret.PaperRollWidth = pageMax.ToString();                                //Paper Width
                        ret.Cut = maxCut.ToString();                                            //จำนวนตัด
                        ret.Trim = (pageMax - sheetIn_W * 1).ToString();                        //เศษตัดริม
                        ret.PercentTrim = Math.Round((Double.Parse(ret.Trim) / Convert.ToDouble(pageMax) * 100), 2).ToString();   //% Waste
                        return ret;
                    }

                    /////////////////////////////////////////////////////////////

                    int k = maxCut;
                    for (k = maxCut; k > 0; k--)
                    {
                        if (sizeWidth > pageMax)
                        {
                            sizeWidth = sheetIn_W * k + trimWaste;
                            if (sizeWidth <= pageMax)
                            {
                                break;
                            }
                        }
                        else break;
                    }

                    foreach (var rollWidth in RollWidth)
                    {
                        if (rollWidth.Group2 >= sizeWidth)
                        {
                            ret.PaperRollWidth = rollWidth.Group2.ToString();                                        //Paper Width
                            ret.Cut = k.ToString();                                                                  //จำนวนตัด
                            ret.Trim = (rollWidth.Group2 - sheetIn_W * k).ToString();                                //เศษตัดริม
                            ret.PercentTrim = Math.Round((Double.Parse(ret.Trim) / Convert.ToDouble(rollWidth.Group2) * 100), 2).ToString();           //% Waste
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return ret;
        }

        public RoutingDataModel CalculateRouting(string machineName, TransactionDataModel ModelTrans, int wid)
        {
            RoutingDataModel ret = new RoutingDataModel();
            try
            {
                var modelRouting = ModelTrans.modelRouting.RoutingDataList;
                string sheetInWidth = Convert.ToString(wid);
                if (sheetInWidth == "" || sheetInWidth == "0" || sheetInWidth == null)
                {
                    foreach (var i in modelRouting)
                    {
                        if (i.IsPropCor == true && i.Machine == machineName)
                        {
                            sheetInWidth = i.SheetWidthIn;
                        }
                    }
                }
                sheetInWidth = sheetInWidth == "" ? ModelTrans.modelProductSpec.CutSheetWid.ToString() : sheetInWidth;

                ret = JsonConvert.DeserializeObject<RoutingDataModel>(_formulaAPIRepository.CalculateRouting(_factoryCode, machineName, ModelTrans.modelProductSpec.Flute, sheetInWidth, ModelTrans.MaterialNo, _token));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return ret;
        }

        public RoutingDataModel CalculateNewCut(string Cut, string WidthIn, string Flut, string materialNo, string machine)
        {
            RoutingDataModel ret = new RoutingDataModel();
            try
            {
                ret = JsonConvert.DeserializeObject<RoutingDataModel>(_formulaAPIRepository.CalculateRoutingByCut(_factoryCode, Cut, WidthIn, Flut, materialNo, machine, _token));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return ret;
        }

        public RoutingDataModel CalculateNewPaperRoll(string PaperwWidth, string WidthIn, string Flut, string cut)
        {
            RoutingDataModel ret = new RoutingDataModel();
            try
            {
                ret = JsonConvert.DeserializeObject<RoutingDataModel>(_formulaAPIRepository.CalculateRoutingByPaperWidth(_factoryCode, PaperwWidth, WidthIn, Flut, cut, _token));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return ret;
        }

        private string GetMachinePlantCode(string machine)
        {
            Machine _machine = new Machine();
            _machine = JsonConvert.DeserializeObject<Machine>(_machineAPIRepository.GetMachineGroupByMachine(_factoryCode, machine, _token));
            return _machine == null ? "" : _machine.PlanCode.ToString();
        }

        //Insert กับ Add Routing
        public void SaveRouting(TransactionDataModel modelToSave)
        {
            TransactionDataModel transactionDataModel = SessionExtentions.GetSession<TransactionDataModel>(_httpContextAccessor.HttpContext.Session, "TransactionDataModel");

            try
            {
                List<Routing> Routing_List = new List<Routing>();
                string plantTemp = "";// string.Empty;
                int countindex = 0;
                string factoryOutsource = "";
                foreach (var i in modelToSave.modelRouting.RoutingDataList)
                {
                    countindex++;
                    Routing tmp = new Routing();
                    tmp.SeqNo = Convert.ToByte(i.SeqNo);
                    string plantcode = "";
                    //if (modelToSave.EventFlag == "Create")
                    //{
                    //    plantcode = _factoryCode;
                    //}
                    //else
                    //{
                    //    if (i.Plant == _factoryCode)
                    //    {
                    //        plantcode = _factoryCode;
                    //    }
                    //    else if (string.IsNullOrEmpty(i.Plant))
                    //    {
                    //        plantcode = _factoryCode;
                    //    }
                    //    else
                    //    {
                    //        plantcode = i.Plant;
                    //    }
                    //}
                    //if (!string.IsNullOrEmpty(modelToSave.PlantOs))
                    //{
                    //    plantcode = modelToSave.PlantOs;
                    //}
                    //else if (!string.IsNullOrEmpty(modelToSave.modelProductInfo.PLANTCODE))
                    //{
                    //    plantcode = modelToSave.modelProductInfo.PLANTCODE;
                    //}
                    //else
                    //{
                    //    plantcode = _factoryCode;// modelToSave.modelProductInfo.PLANTCODE;
                    //}
                    tmp.FactoryCode = string.IsNullOrEmpty(i.Plant) ? _factoryCode : i.Plant;
                    factoryOutsource = string.IsNullOrEmpty(i.Plant) ? _factoryCode : i.Plant;
                    if (transactionDataModel.RealEventFlag == "Edit" && string.IsNullOrEmpty(modelToSave.modelProductInfo.MatOursource))
                    {
                        tmp.FactoryCode = _factoryCode;
                    }

                    plantTemp = string.IsNullOrEmpty(plantTemp) ? i.Plant : plantTemp;
                    tmp.Plant = string.IsNullOrEmpty(i.Plant) ? _factoryCode : i.Plant; //    modelToSave.PlantCode;
                    if (transactionDataModel.RealEventFlag == "Edit" && string.IsNullOrEmpty(i.Plant))
                    {
                        //tmp.FactoryCode = _factoryCode;
                        tmp.Plant = plantTemp;
                        //if (string.IsNullOrEmpty(plantTemp) && countindex == 1)
                        //{
                        //    //get plant by database case insert
                        //    //transactionDataModel.modelRouting.RoutingDataList
                        //    tmp.Plant = _factoryCode;
                        //}

                        if (string.IsNullOrEmpty(plantTemp))
                        {
                            tmp.Plant = _factoryCode;
                        }
                    }

                    tmp.MaterialNo = modelToSave.MaterialNo; //string.IsNullOrEmpty(transactionDataModel.modelProductInfo.MatOursource) ? modelToSave.MaterialNo : transactionDataModel.modelProductInfo.MatOursource;
                    tmp.MatCode = GetMachineCodeByMachine(i.Machine);//modelToSave.modelCategories.MatCode;
                    tmp.PlanCode = GetMachinePlantCode(i.Machine);//modelToSave.PlantCode;
                    tmp.Machine = i.Machine;
                    tmp.Alternative1 = i.Alternative1;
                    tmp.Alternative2 = i.Alternative2;
                    tmp.Alternative3 = i.Alternative3;
                    tmp.Alternative4 = i.Alternative4;
                    tmp.Alternative5 = i.Alternative5;
                    tmp.Alternative6 = i.Alternative6;
                    tmp.Alternative7 = i.Alternative7;
                    tmp.Alternative8 = i.Alternative8;
                    //StdProcess =
                    //Speed =
                    //ColourCount =
                    tmp.McMove = i.MachineMove;
                    //HandHold =
                    tmp.BlockNo = factoryOutsource != _factoryCode ? null : i.PrintingPlateNo;
                    tmp.PlateNo = factoryOutsource != _factoryCode ? null : i.CuttingDieNo == null ? "" : i.CuttingDieNo;
                    tmp.MylaNo = factoryOutsource != _factoryCode ? null : i.MylaNo;
                    tmp.PaperWidth = ConvertStringToShort(i.PaperRollWidth);
                    tmp.CutNo = Convert.ToByte(i.Cut);
                    tmp.Trim = ConvertStringToShort(i.Trim);
                    tmp.PercenTrim = Convert.ToDouble(i.PercentTrim);
                    //WasteLeg =
                    //WasteWid =
                    tmp.WasteLeg = i.WasteLeg;
                    tmp.WasteWid = i.WasteWid;

                    tmp.SheetInLeg = ConvertStringToShort(i.SheetLengthIn);
                    tmp.SheetInWid = ConvertStringToShort(i.SheetWidthIn);
                    tmp.SheetOutLeg = ConvertStringToShort(i.SheetLengthOut);
                    tmp.SheetOutWid = ConvertStringToShort(i.SheetWidthOut);
                    tmp.WeightIn = Convert.ToDouble(i.WeightIn);
                    tmp.WeightOut = Convert.ToDouble(i.WeightOut);
                    tmp.NoOpenIn = ConvertStringToShort(i.NoOpenIn);
                    tmp.NoOpenOut = ConvertStringToShort(i.NoOpenOut);
                    tmp.Color1 = factoryOutsource != _factoryCode ? null : i.Ink1;
                    tmp.Color2 = factoryOutsource != _factoryCode ? null : i.Ink2;
                    tmp.Color3 = factoryOutsource != _factoryCode ? null : i.Ink3;
                    tmp.Color4 = factoryOutsource != _factoryCode ? null : i.Ink4;
                    tmp.Color5 = factoryOutsource != _factoryCode ? null : i.Ink5;
                    tmp.Color6 = factoryOutsource != _factoryCode ? null : i.Ink6;
                    tmp.Color7 = factoryOutsource != _factoryCode ? null : i.Ink7;
                    tmp.Color8 = factoryOutsource != _factoryCode ? null : i.Ink8;
                    tmp.Shade1 = factoryOutsource != _factoryCode ? null : i.Shade1;
                    tmp.Shade2 = factoryOutsource != _factoryCode ? null : i.Shade2;
                    tmp.Shade3 = factoryOutsource != _factoryCode ? null : i.Shade3;
                    tmp.Shade4 = factoryOutsource != _factoryCode ? null : i.Shade4;
                    tmp.Shade5 = factoryOutsource != _factoryCode ? null : i.Shade5;
                    tmp.Shade6 = factoryOutsource != _factoryCode ? null : i.Shade6;
                    tmp.Shade7 = factoryOutsource != _factoryCode ? null : i.Shade7;
                    tmp.Shade8 = factoryOutsource != _factoryCode ? null : i.Shade8;
                    tmp.ColorArea1 = ConvertStringToShort(i.Area1);
                    tmp.ColorArea2 = ConvertStringToShort(i.Area2);
                    tmp.ColorArea3 = ConvertStringToShort(i.Area3);
                    tmp.ColorArea4 = ConvertStringToShort(i.Area4);
                    tmp.ColorArea5 = ConvertStringToShort(i.Area5);
                    tmp.ColorArea6 = ConvertStringToShort(i.Area6);
                    tmp.ColorArea7 = ConvertStringToShort(i.Area7);
                    tmp.ColorArea8 = ConvertStringToShort(i.Area8);
                    //Platen =
                    //Rotary =
                    tmp.TearTape = i.TearTape;
                    tmp.NoneBlk = i.NoneBlk;
                    tmp.StanBlk = i.StanBlk;
                    tmp.SemiBlk = i.SemiBlk;
                    tmp.ShipBlk = i.ShipBlk;
                    //BlockNo =
                    tmp.JoinMatNo = i.JoinToMaterialNo;
                    tmp.SeparatMatNo = i.SperateToMaterialNo;
                    //RemarkInprocess =
                    //Hardship =
                    //PdisStatus =
                    //TranStatus =
                    //SapStatus =
                    //RotateIn =
                    //RotateOut =
                    //StackHeight =
                    //SetupTm =
                    //SetupWaste =
                    //PrepareTm =
                    //PostTm =
                    //RunWaste =
                    //Human =
                    //ColorCount =
                    //UnUpgradBoard =
                    //ScoreType =
                    //ScoreGap =

                    tmp.RemarkInprocess = i.Remark;
                    tmp.PdisStatus = "N";
                    tmp.BlockNo2 = i.BlockNo2;
                    tmp.BlockNoPlant2 = i.BlockNoPlant2;
                    tmp.BlockNo3 = i.BlockNo3;
                    tmp.BlockNoPlant3 = i.BlockNoPlant3;
                    tmp.BlockNo4 = i.BlockNo4;
                    tmp.BlockNoPlant4 = i.BlockNoPlant4;
                    tmp.BlockNo5 = i.BlockNo5;
                    tmp.BlockNoPlant5 = i.BlockNoPlant5;
                    tmp.PlateNo2 = i.PlateNo2;
                    tmp.PlateNoPlant2 = i.PlateNoPlant2;
                    tmp.MylaNo2 = i.MylaNo2;
                    tmp.MylaNoPlant2 = i.MylaNoPlant2;
                    tmp.PlateNo3 = i.PlateNo3;
                    tmp.PlateNoPlant3 = i.PlateNoPlant3;
                    tmp.MylaNo3 = i.MylaNo3;
                    tmp.MylaNoPlant3 = i.MylaNoPlant3;
                    tmp.PlateNo4 = i.PlateNo4;
                    tmp.PlateNoPlant4 = i.PlateNoPlant4;
                    tmp.MylaNo4 = i.MylaNo4;
                    tmp.MylaNoPlant4 = i.MylaNoPlant4;
                    tmp.PlateNo5 = i.PlateNo5;
                    tmp.PlateNoPlant5 = i.PlateNoPlant5;
                    tmp.MylaNo5 = i.MylaNo5;
                    tmp.MylaNoPlant5 = i.MylaNoPlant5;

                    //add save
                    //tmp.Plant = i.Plant;
                    //tmp.PlanCode = i.Plant_Code;
                    tmp.StdProcess = i.StdProcess;
                    tmp.HandHold = i.HandHold;
                    tmp.Platen = i.Platen;
                    tmp.Rotary = i.Rotary;
                    tmp.Hardship = i.Hardship;
                    tmp.UnUpgradBoard = i.UnUpgrad_Board;
                    tmp.ColourCount = i.Color_count;
                    tmp.Human = i.Human;

                    tmp.TearTapeQty = i.TearTapeQty;
                    tmp.TearTapeDistance = i.TearTapeDistance;

                    tmp.Speed = i.Speed;
                    tmp.SetupTm = i.SetupTm;
                    tmp.PrepareTm = i.PrepareTm;
                    tmp.PostTm = i.PostTm;
                    tmp.SetupWaste = i.SetupWaste;
                    tmp.RunWaste = i.RunWaste;

                    tmp.StackHeight = i.StackHeight;
                    tmp.RotateIn = i.RotateIn;
                    tmp.RotateOut = i.RotateOut;

                    tmp.ScoreGap = i.ScoreGap;
                    tmp.ScoreType = i.ScoreType;

                    tmp.MylaSize = i.MylaSize;
                    tmp.RepeatLength = i.RepeatLength;
                    tmp.CustBarcodeNo = i.CustBarcodeNo;

                    tmp.ColorCount = i.Totalcolor;
                    tmp.ColorCount = i.Totalcolor;
                    int sumcolor = 0;
                    if (!string.IsNullOrEmpty(i.Ink1))
                    {
                        sumcolor = sumcolor + 1;
                    }
                    if (!string.IsNullOrEmpty(i.Ink2))
                    {
                        sumcolor = sumcolor + 1;
                    }
                    if (!string.IsNullOrEmpty(i.Ink3))
                    {
                        sumcolor = sumcolor + 1;
                    }
                    if (!string.IsNullOrEmpty(i.Ink4))
                    {
                        sumcolor = sumcolor + 1;
                    }
                    if (!string.IsNullOrEmpty(i.Ink5))
                    {
                        sumcolor = sumcolor + 1;
                    }
                    if (!string.IsNullOrEmpty(i.Ink6))
                    {
                        sumcolor = sumcolor + 1;
                    }
                    if (!string.IsNullOrEmpty(i.Ink7))
                    {
                        sumcolor = sumcolor + 1;
                    }
                    tmp.ColourCount = sumcolor;

                    if (tmp.Machine.Contains("คลัง") && tmp.SeqNo == 1 && (modelToSave.EventFlag == "Create" || modelToSave.EventFlag == "Copy"))
                    {
                        tmp.WeightIn = modelToSave.modelProductSpec.WeightBox;
                        tmp.WeightOut = modelToSave.modelProductSpec.WeightBox;
                    }
                    tmp.AutoCal = i.AutoCal;
                    Routing_List.Add(tmp);
                }
                _routingAPIRepository.SaveRouting(string.IsNullOrEmpty(modelToSave.modelProductInfo.MatOursource) ? _factoryCode : factoryOutsource, modelToSave.MaterialNo, JsonConvert.SerializeObject(Routing_List), _token);

                //update max step
                _newProductService.UpdateMaxProgress(_factoryCode, ref transactionDataModel);
                SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "TransactionDataModel", transactionDataModel);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void PresaleSaveRouting(TransactionDataModel modelToSave)
        {
            try
            {
                List<Routing> Routing_List = new List<Routing>();
                foreach (var i in modelToSave.modelRouting.RoutingDataList)
                {
                    Routing tmp = new Routing();
                    tmp.SeqNo = Convert.ToByte(i.SeqNo);
                    string plantcode = "";
                    //if (modelToSave.EventFlag == "Create")
                    //{
                    //    plantcode = _factoryCode;
                    //}
                    //else
                    //{
                    //    if (i.Plant == _factoryCode)
                    //    {
                    //        plantcode = _factoryCode;
                    //    }
                    //    else if (string.IsNullOrEmpty(i.Plant))
                    //    {
                    //        plantcode = _factoryCode;
                    //    }
                    //    else
                    //    {
                    //        plantcode = i.Plant;
                    //    }
                    //}
                    //if (!string.IsNullOrEmpty(modelToSave.PlantOs))
                    //{
                    //    plantcode = modelToSave.PlantOs;
                    //}
                    //else if (!string.IsNullOrEmpty(modelToSave.modelProductInfo.PLANTCODE))
                    //{
                    //    plantcode = modelToSave.modelProductInfo.PLANTCODE;
                    //}
                    //else
                    //{
                    //    plantcode = _factoryCode;// modelToSave.modelProductInfo.PLANTCODE;
                    //}
                    tmp.FactoryCode = _factoryCode; //string.IsNullOrEmpty(i.Plant) ? _factoryCode : i.Plant;
                    tmp.Plant = string.IsNullOrEmpty(i.Plant) ? _factoryCode : i.Plant; //    modelToSave.PlantCode;
                    tmp.MaterialNo = modelToSave.MaterialNo;
                    tmp.MatCode = GetMachineCodeByMachine(i.Machine);//modelToSave.modelCategories.MatCode;
                    tmp.PlanCode = GetMachinePlantCode(i.Machine);//modelToSave.PlantCode;
                    tmp.Machine = i.Machine;
                    tmp.Alternative1 = i.Alternative1;
                    tmp.Alternative2 = i.Alternative2;
                    tmp.Alternative3 = i.Alternative3;
                    tmp.Alternative4 = i.Alternative4;
                    tmp.Alternative5 = i.Alternative5;
                    tmp.Alternative6 = i.Alternative6;
                    tmp.Alternative7 = i.Alternative7;
                    tmp.Alternative8 = i.Alternative8;
                    //StdProcess =
                    //Speed =
                    //ColourCount =
                    tmp.McMove = i.MachineMove;
                    //HandHold =
                    tmp.BlockNo = i.PrintingPlateNo;
                    tmp.PlateNo = i.CuttingDieNo == null ? "" : i.CuttingDieNo;
                    tmp.MylaNo = i.MylaNo;
                    tmp.PaperWidth = ConvertStringToShort(i.PaperRollWidth);
                    tmp.CutNo = Convert.ToByte(i.Cut);
                    tmp.Trim = ConvertStringToShort(i.Trim);
                    tmp.PercenTrim = Convert.ToDouble(i.PercentTrim);
                    //WasteLeg =
                    //WasteWid =

                    tmp.WasteLeg = i.WasteLeg;
                    tmp.WasteWid = i.WasteWid;

                    tmp.SheetInLeg = !string.IsNullOrEmpty(i.SheetLengthIn) ? ConvertStringToShort(i.SheetLengthIn) : modelToSave.modelProductSpec.CutSheetLeng;
                    tmp.SheetInWid = !string.IsNullOrEmpty(i.SheetWidthIn) ? ConvertStringToShort(i.SheetWidthIn) : modelToSave.modelProductSpec.CutSheetWid;
                    tmp.SheetOutLeg = !string.IsNullOrEmpty(i.SheetLengthOut) ? ConvertStringToShort(i.SheetLengthOut) : modelToSave.modelProductSpec.CutSheetLeng;
                    tmp.SheetOutWid = !string.IsNullOrEmpty(i.SheetWidthOut) ? ConvertStringToShort(i.SheetWidthOut) : modelToSave.modelProductSpec.CutSheetWid;
                    tmp.WeightIn = !string.IsNullOrEmpty(i.WeightIn) ? Convert.ToDouble(i.WeightIn) : modelToSave.modelProductSpec.WeightSh;
                    tmp.WeightOut = !string.IsNullOrEmpty(i.WeightOut) ? Convert.ToDouble(i.WeightOut) : modelToSave.modelProductSpec.WeightBox;

                    tmp.NoOpenIn = ConvertStringToShort(i.NoOpenIn);
                    tmp.NoOpenOut = ConvertStringToShort(i.NoOpenOut);
                    tmp.Color1 = i.Ink1;
                    tmp.Color2 = i.Ink2;
                    tmp.Color3 = i.Ink3;
                    tmp.Color4 = i.Ink4;
                    tmp.Color5 = i.Ink5;
                    tmp.Color6 = i.Ink6;
                    tmp.Color7 = i.Ink7;
                    tmp.Shade1 = i.Shade1;
                    tmp.Shade2 = i.Shade2;
                    tmp.Shade3 = i.Shade3;
                    tmp.Shade4 = i.Shade4;
                    tmp.Shade5 = i.Shade5;
                    tmp.Shade6 = i.Shade6;
                    tmp.Shade7 = i.Shade7;
                    tmp.ColorArea1 = ConvertStringToShort(i.Area1);
                    tmp.ColorArea2 = ConvertStringToShort(i.Area2);
                    tmp.ColorArea3 = ConvertStringToShort(i.Area3);
                    tmp.ColorArea4 = ConvertStringToShort(i.Area4);
                    tmp.ColorArea5 = ConvertStringToShort(i.Area5);
                    tmp.ColorArea6 = ConvertStringToShort(i.Area6);
                    tmp.ColorArea7 = ConvertStringToShort(i.Area7);
                    //Platen =
                    //Rotary =
                    tmp.TearTape = i.TearTape;
                    tmp.NoneBlk = i.NoneBlk;
                    tmp.StanBlk = i.StanBlk;
                    tmp.SemiBlk = i.SemiBlk;
                    tmp.ShipBlk = i.ShipBlk;
                    //BlockNo =
                    tmp.JoinMatNo = i.JoinToMaterialNo;
                    tmp.SeparatMatNo = i.SperateToMaterialNo;
                    //RemarkInprocess =
                    //Hardship =
                    tmp.TranStatus = false;
                    tmp.SapStatus = false;
                    //RotateIn =
                    //RotateOut =
                    //StackHeight =
                    //SetupTm =
                    //SetupWaste =
                    //PrepareTm =
                    //PostTm =
                    //RunWaste =
                    //Human =
                    //ColorCount =
                    //UnUpgradBoard =
                    //ScoreType =
                    //ScoreGap =

                    tmp.RemarkInprocess = i.Remark;
                    tmp.PdisStatus = "N";
                    tmp.BlockNo2 = i.BlockNo2;
                    tmp.BlockNoPlant2 = i.BlockNoPlant2;
                    tmp.BlockNo3 = i.BlockNo3;
                    tmp.BlockNoPlant3 = i.BlockNoPlant3;
                    tmp.BlockNo4 = i.BlockNo4;
                    tmp.BlockNoPlant4 = i.BlockNoPlant4;
                    tmp.BlockNo5 = i.BlockNo5;
                    tmp.BlockNoPlant5 = i.BlockNoPlant5;
                    tmp.PlateNo2 = i.PlateNo2;
                    tmp.PlateNoPlant2 = i.PlateNoPlant2;
                    tmp.MylaNo2 = i.MylaNo2;
                    tmp.MylaNoPlant2 = i.MylaNoPlant2;
                    tmp.PlateNo3 = i.PlateNo3;
                    tmp.PlateNoPlant3 = i.PlateNoPlant3;
                    tmp.MylaNo3 = i.MylaNo3;
                    tmp.MylaNoPlant3 = i.MylaNoPlant3;
                    tmp.PlateNo4 = i.PlateNo4;
                    tmp.PlateNoPlant4 = i.PlateNoPlant4;
                    tmp.MylaNo4 = i.MylaNo4;
                    tmp.MylaNoPlant4 = i.MylaNoPlant4;
                    tmp.PlateNo5 = i.PlateNo5;
                    tmp.PlateNoPlant5 = i.PlateNoPlant5;
                    tmp.MylaNo5 = i.MylaNo5;
                    tmp.MylaNoPlant5 = i.MylaNoPlant5;

                    //add save
                    //tmp.Plant = i.Plant;
                    //tmp.PlanCode = i.Plant_Code;
                    tmp.StdProcess = i.StdProcess;
                    tmp.HandHold = i.HandHold;
                    tmp.Platen = i.Platen;
                    tmp.Rotary = i.Rotary;
                    tmp.Hardship = i.Hardship;
                    tmp.UnUpgradBoard = i.UnUpgrad_Board;
                    tmp.ColourCount = i.Color_count;
                    tmp.Human = i.Human;

                    tmp.TearTapeQty = i.TearTapeQty;
                    tmp.TearTapeDistance = i.TearTapeDistance;

                    tmp.Speed = i.Speed;
                    tmp.SetupTm = i.SetupTm;
                    tmp.PrepareTm = i.PrepareTm;
                    tmp.PostTm = i.PostTm;
                    tmp.SetupWaste = i.SetupWaste;
                    tmp.RunWaste = i.RunWaste;

                    tmp.StackHeight = i.StackHeight;
                    tmp.RotateIn = i.RotateIn;
                    tmp.RotateOut = i.RotateOut;

                    tmp.ScoreGap = i.ScoreGap;
                    tmp.ScoreType = i.ScoreType;

                    tmp.MylaSize = i.MylaSize;
                    tmp.RepeatLength = i.RepeatLength;
                    tmp.CustBarcodeNo = i.CustBarcodeNo;

                    tmp.ColorCount = i.Totalcolor;
                    tmp.ColorCount = i.Totalcolor;
                    int sumcolor = 0;
                    if (!string.IsNullOrEmpty(i.Ink1))
                    {
                        sumcolor = sumcolor + 1;
                    }
                    if (!string.IsNullOrEmpty(i.Ink2))
                    {
                        sumcolor = sumcolor + 1;
                    }
                    if (!string.IsNullOrEmpty(i.Ink3))
                    {
                        sumcolor = sumcolor + 1;
                    }
                    if (!string.IsNullOrEmpty(i.Ink4))
                    {
                        sumcolor = sumcolor + 1;
                    }
                    if (!string.IsNullOrEmpty(i.Ink5))
                    {
                        sumcolor = sumcolor + 1;
                    }
                    if (!string.IsNullOrEmpty(i.Ink6))
                    {
                        sumcolor = sumcolor + 1;
                    }
                    if (!string.IsNullOrEmpty(i.Ink7))
                    {
                        sumcolor = sumcolor + 1;
                    }
                    tmp.ColourCount = sumcolor;

                    Routing_List.Add(tmp);
                }
                _routingAPIRepository.SaveRouting(_factoryCode, modelToSave.MaterialNo, JsonConvert.SerializeObject(Routing_List), _token);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //Update Routing
        public void UpdateRouting(TransactionDataModel modelToSave)
        {
            try
            {
                TransactionDataModel transactionDataModel = SessionExtentions.GetSession<TransactionDataModel>(_httpContextAccessor.HttpContext.Session, "TransactionDataModel");

                bool isOutsource = transactionDataModel?.TransactionDetail != null && transactionDataModel.TransactionDetail.IsOutSource ? true : false;

                List<Routing> Routing_List = new List<Routing>();
                string plantTemp = "";
                foreach (var i in modelToSave.modelRouting.RoutingDataList)
                {
                    Routing tmp = new Routing();
                    tmp.SeqNo = Convert.ToByte(i.SeqNo);

                    tmp.FactoryCode = _factoryCode;// string.IsNullOrEmpty(i.Plant) ? _factoryCode : i.Plant;

                    plantTemp = string.IsNullOrEmpty(plantTemp) ? i.Plant : plantTemp;
                    tmp.Plant = string.IsNullOrEmpty(i.Plant) ? _factoryCode : i.Plant; //    modelToSave.PlantCode;
                    if (transactionDataModel.RealEventFlag == "Edit" && string.IsNullOrEmpty(i.Plant))
                    {
                        //tmp.FactoryCode = _factoryCode;
                        tmp.Plant = plantTemp;
                        if (string.IsNullOrEmpty(plantTemp))
                        {
                            tmp.Plant = _factoryCode;
                        }
                    }
                    // tmp.Plant = string.IsNullOrEmpty(i.Plant)?_factoryCode:i.Plant;// _factoryCode;//GetRoutingList(modelToSave.MaterialNo).OrderBy(x => x.SeqNo).Select(x=>x.Plant).FirstOrDefault();

                    //tmp.FactoryCode = _factoryCode;
                    // tmp.Plant = _factoryCode;//modelToSave.PlantCode;
                    tmp.MaterialNo = modelToSave.MaterialNo;

                    tmp.MatCode = GetMachineCodeByMachine(i.Machine);
                    tmp.PlanCode = GetMachinePlantCode(i.Machine); // modelToSave.PlantCode;
                    tmp.Machine = i.Machine;
                    tmp.Alternative1 = i.Alternative1;
                    tmp.Alternative2 = i.Alternative2;
                    tmp.Alternative3 = i.Alternative3;
                    tmp.Alternative4 = i.Alternative4;
                    tmp.Alternative5 = i.Alternative5;
                    tmp.Alternative6 = i.Alternative6;
                    tmp.Alternative7 = i.Alternative7;
                    tmp.Alternative8 = i.Alternative8;
                    //StdProcess =
                    //Speed =
                    //ColourCount =
                    tmp.McMove = i.MachineMove;
                    //HandHold =
                    tmp.BlockNo = i.PrintingPlateNo;
                    tmp.PlateNo = i.CuttingDieNo == null ? "" : i.CuttingDieNo;
                    tmp.MylaNo = i.MylaNo;
                    tmp.PaperWidth = ConvertStringToShort(i.PaperRollWidth);
                    tmp.CutNo = Convert.ToByte(i.Cut);
                    tmp.Trim = ConvertStringToShort(i.Trim);
                    tmp.PercenTrim = Convert.ToDouble(i.PercentTrim);
                    //WasteLeg =
                    //WasteWid =

                    tmp.WasteLeg = i.WasteLeg;
                    tmp.WasteWid = i.WasteWid;

                    tmp.SheetInLeg = ConvertStringToShort(i.SheetLengthIn);
                    tmp.SheetInWid = ConvertStringToShort(i.SheetWidthIn);
                    tmp.SheetOutLeg = ConvertStringToShort(i.SheetLengthOut);
                    tmp.SheetOutWid = ConvertStringToShort(i.SheetWidthOut);
                    tmp.WeightIn = Convert.ToDouble(i.WeightIn);
                    tmp.WeightOut = Convert.ToDouble(i.WeightOut);
                    tmp.NoOpenIn = ConvertStringToShort(i.NoOpenIn);
                    tmp.NoOpenOut = ConvertStringToShort(i.NoOpenOut);
                    tmp.Color1 = i.Ink1;
                    tmp.Color2 = i.Ink2;
                    tmp.Color3 = i.Ink3;
                    tmp.Color4 = i.Ink4;
                    tmp.Color5 = i.Ink5;
                    tmp.Color6 = i.Ink6;
                    tmp.Color7 = i.Ink7;
                    tmp.Color8 = i.Ink8;
                    tmp.Shade1 = i.Shade1;
                    tmp.Shade2 = i.Shade2;
                    tmp.Shade3 = i.Shade3;
                    tmp.Shade4 = i.Shade4;
                    tmp.Shade5 = i.Shade5;
                    tmp.Shade6 = i.Shade6;
                    tmp.Shade7 = i.Shade7;
                    tmp.Shade8 = i.Shade8;
                    tmp.ColorArea1 = ConvertStringToShort(i.Area1);
                    tmp.ColorArea2 = ConvertStringToShort(i.Area2);
                    tmp.ColorArea3 = ConvertStringToShort(i.Area3);
                    tmp.ColorArea4 = ConvertStringToShort(i.Area4);
                    tmp.ColorArea5 = ConvertStringToShort(i.Area5);
                    tmp.ColorArea6 = ConvertStringToShort(i.Area6);
                    tmp.ColorArea7 = ConvertStringToShort(i.Area7);
                    tmp.ColorArea8 = ConvertStringToShort(i.Area8);
                    //Platen =
                    //Rotary =
                    tmp.TearTape = i.TearTape;
                    tmp.NoneBlk = i.NoneBlk;
                    tmp.StanBlk = i.StanBlk;
                    tmp.SemiBlk = i.SemiBlk;
                    tmp.ShipBlk = i.ShipBlk;
                    //BlockNo =
                    tmp.JoinMatNo = i.JoinToMaterialNo;
                    tmp.SeparatMatNo = i.SperateToMaterialNo;
                    //RemarkInprocess =
                    //Hardship =
                    //PdisStatus =
                    tmp.TranStatus = isOutsource;
                    //SapStatus =
                    //RotateIn =
                    //RotateOut =
                    //StackHeight =
                    //SetupTm =
                    //SetupWaste =
                    //PrepareTm =
                    //PostTm =
                    //RunWaste =
                    //Human =
                    //ColorCount =
                    //UnUpgradBoard =
                    //ScoreType =
                    //ScoreGap =

                    tmp.RemarkInprocess = i.Remark;
                    tmp.PdisStatus = "N";
                    tmp.BlockNo2 = i.BlockNo2;
                    tmp.BlockNoPlant2 = i.BlockNoPlant2;
                    tmp.BlockNo3 = i.BlockNo3;
                    tmp.BlockNoPlant3 = i.BlockNoPlant3;
                    tmp.BlockNo4 = i.BlockNo4;
                    tmp.BlockNoPlant4 = i.BlockNoPlant4;
                    tmp.BlockNo5 = i.BlockNo5;
                    tmp.BlockNoPlant5 = i.BlockNoPlant5;
                    tmp.PlateNo2 = i.PlateNo2;
                    tmp.PlateNoPlant2 = i.PlateNoPlant2;
                    tmp.MylaNo2 = i.MylaNo2;
                    tmp.MylaNoPlant2 = i.MylaNoPlant2;
                    tmp.PlateNo3 = i.PlateNo3;
                    tmp.PlateNoPlant3 = i.PlateNoPlant3;
                    tmp.MylaNo3 = i.MylaNo3;
                    tmp.MylaNoPlant3 = i.MylaNoPlant3;
                    tmp.PlateNo4 = i.PlateNo4;
                    tmp.PlateNoPlant4 = i.PlateNoPlant4;
                    tmp.MylaNo4 = i.MylaNo4;
                    tmp.MylaNoPlant4 = i.MylaNoPlant4;
                    tmp.PlateNo5 = i.PlateNo5;
                    tmp.PlateNoPlant5 = i.PlateNoPlant5;
                    tmp.MylaNo5 = i.MylaNo5;
                    tmp.MylaNoPlant5 = i.MylaNoPlant5;

                    //add save
                    //tmp.Plant = i.Plant;
                    //tmp.PlanCode = i.Plant_Code;
                    tmp.StdProcess = i.StdProcess;
                    tmp.HandHold = i.HandHold;
                    tmp.Platen = i.Platen;
                    tmp.Rotary = i.Rotary;
                    tmp.Hardship = i.Hardship;
                    tmp.UnUpgradBoard = i.UnUpgrad_Board;
                    tmp.ColourCount = i.Color_count;
                    tmp.Human = i.Human;

                    tmp.TearTapeQty = i.TearTapeQty;
                    tmp.TearTapeDistance = i.TearTapeDistance;

                    tmp.Speed = i.Speed;
                    tmp.SetupTm = i.SetupTm;
                    tmp.PrepareTm = i.PrepareTm;
                    tmp.PostTm = i.PostTm;
                    tmp.SetupWaste = i.SetupWaste;
                    tmp.RunWaste = i.RunWaste;

                    tmp.StackHeight = i.StackHeight;
                    tmp.RotateIn = i.RotateIn;
                    tmp.RotateOut = i.RotateOut;

                    tmp.ScoreGap = i.ScoreGap;
                    tmp.ScoreType = i.ScoreType;
                    tmp.MylaSize = i.MylaSize;
                    tmp.RepeatLength = i.RepeatLength;
                    tmp.CustBarcodeNo = i.CustBarcodeNo;

                    tmp.ColorCount = i.Totalcolor;
                    int sumcolor = 0;
                    if (!string.IsNullOrEmpty(i.Ink1))
                    {
                        sumcolor = sumcolor + 1;
                    }
                    if (!string.IsNullOrEmpty(i.Ink2))
                    {
                        sumcolor = sumcolor + 1;
                    }
                    if (!string.IsNullOrEmpty(i.Ink3))
                    {
                        sumcolor = sumcolor + 1;
                    }
                    if (!string.IsNullOrEmpty(i.Ink4))
                    {
                        sumcolor = sumcolor + 1;
                    }
                    if (!string.IsNullOrEmpty(i.Ink5))
                    {
                        sumcolor = sumcolor + 1;
                    }
                    if (!string.IsNullOrEmpty(i.Ink6))
                    {
                        sumcolor = sumcolor + 1;
                    }
                    if (!string.IsNullOrEmpty(i.Ink7))
                    {
                        sumcolor = sumcolor + 1;
                    }
                    tmp.ColourCount = sumcolor;
                    tmp.AutoCal = i.AutoCal;

                    Routing_List.Add(tmp);
                }
                _routingAPIRepository.SaveRouting(_factoryCode, modelToSave.MaterialNo, JsonConvert.SerializeObject(Routing_List), _token);

                //update max step
                _newProductService.UpdateMaxProgress(_factoryCode, ref transactionDataModel);
                SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "TransactionDataModel", transactionDataModel);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<RoutingDataModel> GetRoutingList(string MaterialNo)
        {
            var routingByApi = JsonConvert.DeserializeObject<List<Routing>>(_routingAPIRepository.GetRoutingsByMaterialNo(_factoryCode, MaterialNo, _token)); ;
            List<RoutingDataModel> routing = new List<RoutingDataModel>();
            foreach (var item in routingByApi)
            {
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
                tmp.NoOpenIn = item.NoOpenIn.ToString();
                tmp.NoOpenOut = item.NoOpenOut.ToString();
                tmp.WeightIn = (Convert.ToDouble(item.WeightIn)).ToString("N3");
                tmp.WeightOut = (Convert.ToDouble(item.WeightOut)).ToString("N3");
                tmp.SheetLengthIn = item.SheetInLeg.ToString();
                tmp.SheetLengthOut = item.SheetOutLeg.ToString();
                tmp.SheetWidthIn = item.SheetInWid.ToString();
                tmp.SheetWidthOut = item.SheetOutWid.ToString();
                tmp.Coat = item.Coating.ToString();
                tmp.PaperRollWidth = item.PaperWidth.ToString();
                tmp.Cut = item.CutNo.ToString();
                tmp.Trim = item.Trim.ToString();
                tmp.PercentTrim = item.PercenTrim.ToString();
                tmp.TearTape = item.TearTape;
                tmp.LineQtyPerBox = ConvertInt16ToShort(item.TearTapeQty);
                tmp.MarginForPaper = item.TearTapeDistance;
                tmp.Ink1 = item.Color1;
                tmp.Ink2 = item.Color2;
                tmp.Ink3 = item.Color3;
                tmp.Ink4 = item.Color4;
                tmp.Ink5 = item.Color5;
                tmp.Ink6 = item.Color6;
                tmp.Ink7 = item.Color7;
                tmp.Ink8 = item.Color8;
                tmp.Shade1 = item.Shade1;
                tmp.Shade2 = item.Shade2;
                tmp.Shade3 = item.Shade3;
                tmp.Shade4 = item.Shade4;
                tmp.Shade5 = item.Shade5;
                tmp.Shade6 = item.Shade6;
                tmp.Shade7 = item.Shade7;
                tmp.Shade8 = item.Shade8;
                tmp.Area1 = item.ColorArea1.ToString();
                tmp.Area2 = item.ColorArea2.ToString();
                tmp.Area3 = item.ColorArea3.ToString();
                tmp.Area4 = item.ColorArea4.ToString();
                tmp.Area5 = item.ColorArea5.ToString();
                tmp.Area6 = item.ColorArea6.ToString();
                tmp.Area7 = item.ColorArea7.ToString();
                tmp.Area8 = item.ColorArea8.ToString();
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

                tmp.NoneBlk = item.NoneBlk ?? false;
                tmp.StanBlk = item.StanBlk ?? false;
                tmp.SemiBlk = item.SemiBlk ?? false;
                tmp.ShipBlk = item.ShipBlk ?? false;

                //add save
                tmp.Plant = item.Plant;
                tmp.Plant_Code = item.PlanCode;
                tmp.StdProcess = item.StdProcess ?? false;
                tmp.HandHold = item.HandHold ?? false;
                tmp.Platen = item.Platen ?? false;
                tmp.Rotary = item.Rotary ?? false;
                tmp.Hardship = ConvertInt16ToShort(item.Hardship);
                tmp.UnUpgrad_Board = item.UnUpgradBoard ?? false;
                tmp.Color_count = ConvertInt16ToShort(item.ColourCount);
                tmp.Human = ConvertInt16ToShort(item.Human);

                tmp.TearTapeQty = ConvertInt16ToShort(item.TearTapeQty);
                tmp.TearTapeDistance = item.TearTapeDistance;

                tmp.Speed = ConvertInt16ToShort(item.Speed);
                tmp.SetupTm = ConvertInt16ToShort(item.SetupTm);
                tmp.PrepareTm = ConvertInt16ToShort(item.PrepareTm);
                tmp.PostTm = ConvertInt16ToShort(item.PostTm);
                tmp.SetupWaste = ConvertInt16ToShort(item.SetupWaste);
                tmp.RunWaste = ConvertInt16ToShort(item.RunWaste);

                tmp.StackHeight = ConvertInt16ToShort(item.StackHeight);
                if (item.RotateIn)
                {
                    tmp.Rotate = "rotateIn";
                }
                else if (item.RotateOut)
                {
                    tmp.Rotate = "rotateOut";
                }
                else
                {
                    tmp.Rotate = "noRotate";
                }
                tmp.RotateIn = item.RotateIn;
                tmp.RotateOut = item.RotateOut;

                tmp.ScoreGap = Convert.ToDouble(item.ScoreGap);
                tmp.ScoreType = item.ScoreType;

                tmp.MylaSize = item.MylaSize;
                tmp.RepeatLength = item.RepeatLength.HasValue ? item.RepeatLength.Value : 0;
                tmp.CustBarcodeNo = item.CustBarcodeNo;
                tmp.Totalcolor = ConvertInt16ToShort(item.ColorCount);

                tmp.WasteLeg = item.WasteLeg;
                tmp.WasteWid = item.WasteWid;
                tmp.AutoCal = item.AutoCal.HasValue ? item.AutoCal.Value : false;

                routing.Add(tmp);
            }
            // routing = JsonConvert.DeserializeObject<List<RoutingDataModel>>(_routingAPIRepository.GetRoutingByMaterialNo(_factoryCode, MaterialNo));
            return routing;
        }

        public void InitialPresaleRouting()
        {
            var modelSession = SessionExtentions.GetSession<TransactionDataModel>(_httpContextAccessor.HttpContext.Session, "TransactionDataModel");
            //set routing Form data to PMTs DB.
            if (modelSession.EventFlag == "Presale")
            {
                var routingDatas = SessionExtentions.GetSession<List<RoutingDataModel>>(_httpContextAccessor.HttpContext.Session, "RoutingDataModels");
                var routings = new List<Routing>();

                foreach (var routingData in routingDatas)
                {
                    Routing routing = new Routing();
                    routing.SeqNo = Convert.ToByte(routingData.SeqNo);
                    routing.FactoryCode = _factoryCode;
                    routing.Plant = _factoryCode;//modelSession.PlantCode;
                    routing.MaterialNo = modelSession.MaterialNo;
                    routing.MatCode = modelSession.modelCategories.MatCode;
                    routing.PlanCode = GetMachinePlantCode(routingData.Machine); // modelSession.PlantCode;
                    routing.Machine = routingData.Machine;
                    routing.Alternative1 = routingData.Alternative1;
                    routing.Alternative2 = routingData.Alternative2;
                    routing.Alternative3 = routingData.Alternative3;
                    routing.Alternative4 = routingData.Alternative4;
                    routing.Alternative5 = routingData.Alternative5;
                    routing.Alternative6 = routingData.Alternative6;
                    routing.Alternative7 = routingData.Alternative7;
                    routing.Alternative8 = routingData.Alternative8;
                    routing.McMove = routingData.MachineMove;
                    routing.BlockNo = routingData.PrintingPlateNo;
                    routing.PlateNo = routingData.CuttingDieNo == null ? "" : routingData.CuttingDieNo;
                    routing.MylaNo = routingData.MylaNo;
                    routing.PaperWidth = ConvertStringToShort(routingData.PaperRollWidth);
                    routing.CutNo = Convert.ToByte(routingData.Cut);
                    routing.Trim = ConvertStringToShort(routingData.Trim);
                    routing.PercenTrim = Convert.ToDouble(routingData.PercentTrim);
                    routing.SheetInLeg = ConvertStringToShort(routingData.SheetLengthIn);
                    routing.SheetInWid = ConvertStringToShort(routingData.SheetWidthIn);
                    routing.SheetOutLeg = ConvertStringToShort(routingData.SheetLengthOut);
                    routing.SheetOutWid = ConvertStringToShort(routingData.SheetWidthOut);
                    routing.WeightIn = Convert.ToDouble(routingData.WeightIn);
                    routing.WeightOut = Convert.ToDouble(routingData.WeightOut);
                    routing.NoOpenIn = ConvertStringToShort(routingData.NoOpenIn);
                    routing.NoOpenOut = ConvertStringToShort(routingData.NoOpenOut);
                    routing.Color1 = routingData.Ink1;
                    routing.Color2 = routingData.Ink2;
                    routing.Color3 = routingData.Ink3;
                    routing.Color4 = routingData.Ink4;
                    routing.Color5 = routingData.Ink5;
                    routing.Color6 = routingData.Ink6;
                    routing.Color7 = routingData.Ink7;
                    routing.Color8 = routingData.Ink8;
                    routing.Shade1 = routingData.Shade1;
                    routing.Shade2 = routingData.Shade2;
                    routing.Shade3 = routingData.Shade3;
                    routing.Shade4 = routingData.Shade4;
                    routing.Shade5 = routingData.Shade5;
                    routing.Shade6 = routingData.Shade6;
                    routing.Shade7 = routingData.Shade7;
                    routing.Shade8 = routingData.Shade8;
                    routing.ColorArea1 = ConvertStringToShort(routingData.Area1);
                    routing.ColorArea2 = ConvertStringToShort(routingData.Area2);
                    routing.ColorArea3 = ConvertStringToShort(routingData.Area3);
                    routing.ColorArea4 = ConvertStringToShort(routingData.Area4);
                    routing.ColorArea5 = ConvertStringToShort(routingData.Area5);
                    routing.ColorArea6 = ConvertStringToShort(routingData.Area6);
                    routing.ColorArea7 = ConvertStringToShort(routingData.Area7);
                    routing.ColorArea8 = ConvertStringToShort(routingData.Area8);
                    routing.TearTape = routingData.TearTape;
                    routing.NoneBlk = routingData.NoneBlk;
                    routing.StanBlk = routingData.StanBlk;
                    routing.SemiBlk = routingData.SemiBlk;
                    routing.ShipBlk = routingData.ShipBlk;
                    routing.JoinMatNo = routingData.JoinToMaterialNo;
                    routing.SeparatMatNo = routingData.SperateToMaterialNo;
                    routing.RemarkInprocess = routingData.Remark;
                    routing.PdisStatus = "N";
                    routing.BlockNo2 = routingData.BlockNo2;
                    routing.BlockNoPlant2 = routingData.BlockNoPlant2;
                    routing.BlockNo3 = routingData.BlockNo3;
                    routing.BlockNoPlant3 = routingData.BlockNoPlant3;
                    routing.BlockNo4 = routingData.BlockNo4;
                    routing.BlockNoPlant4 = routingData.BlockNoPlant4;
                    routing.BlockNo5 = routingData.BlockNo5;
                    routing.BlockNoPlant5 = routingData.BlockNoPlant5;
                    routing.PlateNo2 = routingData.PlateNo2;
                    routing.PlateNoPlant2 = routingData.PlateNoPlant2;
                    routing.MylaNo2 = routingData.MylaNo2;
                    routing.MylaNoPlant2 = routingData.MylaNoPlant2;
                    routing.PlateNo3 = routingData.PlateNo3;
                    routing.PlateNoPlant3 = routingData.PlateNoPlant3;
                    routing.MylaNo3 = routingData.MylaNo3;
                    routing.MylaNoPlant3 = routingData.MylaNoPlant3;
                    routing.PlateNo4 = routingData.PlateNo4;
                    routing.PlateNoPlant4 = routingData.PlateNoPlant4;
                    routing.MylaNo4 = routingData.MylaNo4;
                    routing.MylaNoPlant4 = routingData.MylaNoPlant4;
                    routing.PlateNo5 = routingData.PlateNo5;
                    routing.PlateNoPlant5 = routingData.PlateNoPlant5;
                    routing.MylaNo5 = routingData.MylaNo5;
                    routing.MylaNoPlant5 = routingData.MylaNoPlant5;

                    //add save
                    //routing.Plant = routingData.Plant;
                    //routing.PlanCode = routingData.Plant_Code;
                    routing.StdProcess = routingData.StdProcess;
                    routing.HandHold = routingData.HandHold;
                    routing.Platen = routingData.Platen;
                    routing.Rotary = routingData.Rotary;
                    routing.Hardship = routingData.Hardship;
                    routing.UnUpgradBoard = routingData.UnUpgrad_Board;
                    routing.ColourCount = routingData.Color_count;
                    routing.Human = routingData.Human;
                    routing.TearTapeQty = routingData.TearTapeQty;
                    routing.TearTapeDistance = routingData.TearTapeDistance;
                    routing.Speed = routingData.Speed;
                    routing.SetupTm = routingData.SetupTm;
                    routing.PrepareTm = routingData.PrepareTm;
                    routing.PostTm = routingData.PostTm;
                    routing.SetupWaste = routingData.SetupWaste;
                    routing.RunWaste = routingData.RunWaste;
                    routing.StackHeight = routingData.StackHeight;
                    routing.RotateIn = routingData.RotateIn;
                    routing.RotateOut = routingData.RotateOut;
                    routing.ScoreGap = routingData.ScoreGap;
                    routing.ScoreType = routingData.ScoreType;
                    routing.ColorCount = routingData.Totalcolor;

                    routing.WasteLeg = routingData.WasteLeg;
                    routing.WasteWid = routingData.WasteWid;

                    routing.MylaSize = routingData.MylaSize;

                    routings.Add(routing);
                }

                _routingAPIRepository.SaveRouting(_factoryCode, modelSession.MaterialNo, JsonConvert.SerializeObject(routings), _token);
            }
        }

        private int ConvertStringToShort(string Input)
        {
            return string.IsNullOrEmpty(Input) ? 0 : Convert.ToInt32(Input);
        }

        private Int16 ConvertInt16ToShort(int? Input)
        {
            return (Int16)(string.IsNullOrEmpty(Input.ToString()) ? 0 : Input);
        }

        public void MappingModelRouting(TransactionDataModel sessionModel, TransactionDataModel transactionDataModel, ref RoutingDataModel retuneTransactionModel, ref int seqNum)
        {
            string[] arrayDiecut = null;
            if (transactionDataModel.arrayDiecut != null)
            {
                arrayDiecut = JsonConvert.DeserializeObject<string[]>(transactionDataModel.arrayDiecut);
            }
            string[] arrayPrint = null;
            if (transactionDataModel.arrayPrint != null)
            {
                arrayPrint = JsonConvert.DeserializeObject<string[]>(transactionDataModel.arrayPrint);
            }

            seqNum = sessionModel.modelRouting.RoutingDataList.Count + 1;

            RoutingDataModel routingModel = new RoutingDataModel();
            routingModel.SeqNo = seqNum;
            routingModel.GroupMachine = transactionDataModel.modelRouting.GroupMachine;
            routingModel.Machine = transactionDataModel.modelRouting.Machine;
            routingModel.MachineMove = transactionDataModel.modelRouting.MachineMove;
            routingModel.Alternative1 = transactionDataModel.modelRouting.Alternative1;
            routingModel.Alternative2 = transactionDataModel.modelRouting.Alternative2;
            routingModel.Alternative3 = transactionDataModel.modelRouting.Alternative3;
            routingModel.Alternative4 = transactionDataModel.modelRouting.Alternative4;
            routingModel.Alternative5 = transactionDataModel.modelRouting.Alternative5;
            routingModel.Alternative6 = transactionDataModel.modelRouting.Alternative6;
            routingModel.Alternative7 = transactionDataModel.modelRouting.Alternative7;
            routingModel.Alternative8 = transactionDataModel.modelRouting.Alternative8;
            routingModel.NoOpenIn = transactionDataModel.modelRouting.NoOpenIn;
            routingModel.NoOpenOut = transactionDataModel.modelRouting.NoOpenOut;
            routingModel.WeightIn = transactionDataModel.modelRouting.WeightIn;
            routingModel.WeightOut = transactionDataModel.modelRouting.WeightOut;
            routingModel.SheetLengthIn = transactionDataModel.modelRouting.SheetLengthIn;
            routingModel.SheetLengthOut = transactionDataModel.modelRouting.SheetLengthOut;
            routingModel.SheetWidthIn = transactionDataModel.modelRouting.SheetWidthIn;
            routingModel.SheetWidthOut = transactionDataModel.modelRouting.SheetWidthOut;
            routingModel.Coat = transactionDataModel.modelRouting.Coat;
            routingModel.PaperRollWidth = transactionDataModel.modelRouting.PaperRollWidth;
            routingModel.Cut = transactionDataModel.modelRouting.Cut;
            routingModel.Trim = transactionDataModel.modelRouting.Trim;
            routingModel.PercentTrim = transactionDataModel.modelRouting.PercentTrim;
            routingModel.TearTape = transactionDataModel.modelRouting.TearTape;
            routingModel.LineQtyPerBox = transactionDataModel.modelRouting.LineQtyPerBox;
            routingModel.MarginForPaper = transactionDataModel.modelRouting.MarginForPaper;
            routingModel.Ink1 = transactionDataModel.modelRouting.Ink1 == null || transactionDataModel.modelRouting.Ink1.Trim() == "--- Choose here ---" ? null : transactionDataModel.modelRouting.Ink1;
            routingModel.Ink2 = transactionDataModel.modelRouting.Ink2 == null || transactionDataModel.modelRouting.Ink2.Trim() == "--- Choose here ---" ? null : transactionDataModel.modelRouting.Ink2;
            routingModel.Ink3 = transactionDataModel.modelRouting.Ink3 == null || transactionDataModel.modelRouting.Ink3.Trim() == "--- Choose here ---" ? null : transactionDataModel.modelRouting.Ink3;
            routingModel.Ink4 = transactionDataModel.modelRouting.Ink4 == null || transactionDataModel.modelRouting.Ink4.Trim() == "--- Choose here ---" ? null : transactionDataModel.modelRouting.Ink4;
            routingModel.Ink5 = transactionDataModel.modelRouting.Ink5 == null || transactionDataModel.modelRouting.Ink5.Trim() == "--- Choose here ---" ? null : transactionDataModel.modelRouting.Ink5;
            routingModel.Ink6 = transactionDataModel.modelRouting.Ink6 == null || transactionDataModel.modelRouting.Ink6.Trim() == "--- Choose here ---" ? null : transactionDataModel.modelRouting.Ink6;
            routingModel.Ink7 = transactionDataModel.modelRouting.Ink7 == null || transactionDataModel.modelRouting.Ink7.Trim() == "--- Choose here ---" ? null : transactionDataModel.modelRouting.Ink7;
            routingModel.Ink8 = transactionDataModel.modelRouting.Ink8 == null || transactionDataModel.modelRouting.Ink8.Trim() == "--- Choose here ---" ? null : transactionDataModel.modelRouting.Ink8;
            routingModel.Shade1 = transactionDataModel.modelRouting.Shade1 == null || transactionDataModel.modelRouting.Shade1.Trim() == "--- Choose here ---" ? null : transactionDataModel.modelRouting.Shade1;
            routingModel.Shade2 = transactionDataModel.modelRouting.Shade2 == null || transactionDataModel.modelRouting.Shade2.Trim() == "--- Choose here ---" ? null : transactionDataModel.modelRouting.Shade2;
            routingModel.Shade3 = transactionDataModel.modelRouting.Shade3 == null || transactionDataModel.modelRouting.Shade3.Trim() == "--- Choose here ---" ? null : transactionDataModel.modelRouting.Shade3;
            routingModel.Shade4 = transactionDataModel.modelRouting.Shade4 == null || transactionDataModel.modelRouting.Shade4.Trim() == "--- Choose here ---" ? null : transactionDataModel.modelRouting.Shade4;
            routingModel.Shade5 = transactionDataModel.modelRouting.Shade5 == null || transactionDataModel.modelRouting.Shade5.Trim() == "--- Choose here ---" ? null : transactionDataModel.modelRouting.Shade5;
            routingModel.Shade6 = transactionDataModel.modelRouting.Shade6 == null || transactionDataModel.modelRouting.Shade6.Trim() == "--- Choose here ---" ? null : transactionDataModel.modelRouting.Shade6;
            routingModel.Shade7 = transactionDataModel.modelRouting.Shade7 == null || transactionDataModel.modelRouting.Shade7.Trim() == "--- Choose here ---" ? null : transactionDataModel.modelRouting.Shade7;
            routingModel.Shade8 = transactionDataModel.modelRouting.Shade8 == null || transactionDataModel.modelRouting.Shade8.Trim() == "--- Choose here ---" ? null : transactionDataModel.modelRouting.Shade8;
            routingModel.Area1 = transactionDataModel.modelRouting.Area1;
            routingModel.Area2 = transactionDataModel.modelRouting.Area2;
            routingModel.Area3 = transactionDataModel.modelRouting.Area3;
            routingModel.Area4 = transactionDataModel.modelRouting.Area4;
            routingModel.Area5 = transactionDataModel.modelRouting.Area5;
            routingModel.Area6 = transactionDataModel.modelRouting.Area6;
            routingModel.Area7 = transactionDataModel.modelRouting.Area7;
            routingModel.Area8 = transactionDataModel.modelRouting.Area8;
            routingModel.PrintingPlateNo = transactionDataModel.modelRouting.PrintingPlateNo;
            routingModel.CuttingDieNo = transactionDataModel.modelRouting.CuttingDieNo;
            routingModel.RepeatLength = transactionDataModel.modelRouting.RepeatLength;
            routingModel.CustBarcodeNo = transactionDataModel.modelRouting.CustBarcodeNo;

            if (transactionDataModel.modelRouting.MylaNo != null)
            {
                routingModel.MylaNo = transactionDataModel.modelRouting.MylaNo;
            }
            else
            {
                routingModel.MylaNo = transactionDataModel.modelRouting.MylaNo_Copy;
            }

            routingModel.PrintingPlateType = transactionDataModel.modelRouting.PrintingPlateType;
            routingModel.JoinToMaterialNo = transactionDataModel.modelRouting.JoinToMaterialNo;
            routingModel.SperateToMaterialNo = transactionDataModel.modelRouting.SperateToMaterialNo;
            routingModel.Remark = transactionDataModel.modelRouting.Remark;
            routingModel.MachineGroupSelect = transactionDataModel.modelRouting.MachineGroupSelect;

            routingModel.Plant = transactionDataModel.FactoryCode;
            //routingModel.Plant_Code = "Machine";
            //routingModel.StdProcess = true;
            routingModel.HandHold = false;
            // routingModel.Platen =
            //routingModel.Rotary
            //routingModel.Hardship
            routingModel.UnUpgrad_Board = false;
            //routingModel.Color_count =
            //routingModel.Human =

            routingModel.Speed = transactionDataModel.modelRouting.Speed;
            routingModel.SetupTm = transactionDataModel.modelRouting.SetupTm;
            routingModel.PrepareTm = transactionDataModel.modelRouting.PrepareTm;
            routingModel.PostTm = transactionDataModel.modelRouting.PostTm;
            routingModel.SetupWaste = transactionDataModel.modelRouting.SetupWaste;
            routingModel.RunWaste = transactionDataModel.modelRouting.RunWaste;
            routingModel.StackHeight = transactionDataModel.modelRouting.StackHeight;
            routingModel.RotateIn = transactionDataModel.modelRouting.RotateIn;
            routingModel.RotateOut = transactionDataModel.modelRouting.RotateOut;

            routingModel.TearTapeQty = transactionDataModel.modelRouting.LineQtyPerBox; //  string.IsNullOrEmpty(transactionDataModel.modelRouting.LineQtyPerBox) ? 0 : Convert.ToInt16(transactionDataModel.modelRouting.LineQtyPerBox); ;
            routingModel.TearTapeDistance = transactionDataModel.modelRouting.MarginForPaper;

            routingModel.ScoreGap = transactionDataModel.modelRouting.ScoreGap;
            routingModel.ScoreType = transactionDataModel.modelRouting.ScoreType;

            routingModel.MylaSize = transactionDataModel.modelRouting.MylaSize;
            routingModel.AutoCal = transactionDataModel.modelRouting.AutoCal;

            switch (transactionDataModel.modelRouting.PrintingPlateType)
            {
                case "Non-Print":
                    routingModel.NoneBlk = true;
                    routingModel.StanBlk = false;
                    routingModel.SemiBlk = false;
                    routingModel.ShipBlk = false;
                    break;

                case "Standard":
                    routingModel.NoneBlk = false;
                    routingModel.StanBlk = true;
                    routingModel.SemiBlk = false;
                    routingModel.ShipBlk = false;
                    break;

                case "Semi":
                    routingModel.NoneBlk = false;
                    routingModel.StanBlk = false;
                    routingModel.SemiBlk = true;
                    routingModel.ShipBlk = false;
                    break;

                case "Shipping Mark":
                    routingModel.NoneBlk = false;
                    routingModel.StanBlk = false;
                    routingModel.SemiBlk = false;
                    routingModel.ShipBlk = true;
                    break;
            }

            switch (transactionDataModel.modelRouting.Rotate)
            {
                case "noRotate":
                    routingModel.RotateIn = false;
                    routingModel.RotateOut = false;
                    break;

                case "rotateIn":
                    routingModel.RotateIn = true;
                    routingModel.RotateOut = false;
                    break;

                case "rotateOut":
                    routingModel.RotateIn = false;
                    routingModel.RotateOut = true;
                    break;
            }

            if (arrayPrint != null)
            {
                switch (arrayPrint.Length)
                {
                    case 2:
                        routingModel.PlateNo2 = arrayPrint[0];
                        routingModel.PlateNoPlant2 = arrayPrint[1];
                        routingModel.PlateNo3 = null;
                        routingModel.PlateNoPlant3 = null;
                        routingModel.PlateNo4 = null;
                        routingModel.PlateNoPlant4 = null;
                        routingModel.PlateNo5 = null;
                        routingModel.PlateNoPlant5 = null;
                        break;

                    case 4:
                        routingModel.PlateNo2 = arrayPrint[0];
                        routingModel.PlateNoPlant2 = arrayPrint[1];
                        routingModel.PlateNo3 = arrayPrint[2];
                        routingModel.PlateNoPlant3 = arrayPrint[3];
                        routingModel.PlateNo4 = null;
                        routingModel.PlateNoPlant4 = null;
                        routingModel.PlateNo5 = null;
                        routingModel.PlateNoPlant5 = null;
                        break;

                    case 6:
                        routingModel.PlateNo2 = arrayPrint[0];
                        routingModel.PlateNoPlant2 = arrayPrint[1];
                        routingModel.PlateNo3 = arrayPrint[2];
                        routingModel.PlateNoPlant3 = arrayPrint[3];
                        routingModel.PlateNo4 = arrayPrint[4];
                        routingModel.PlateNoPlant4 = arrayPrint[5];
                        routingModel.PlateNo5 = null;
                        routingModel.PlateNoPlant5 = null;
                        break;

                    case 8:
                        routingModel.PlateNo2 = arrayPrint[0];
                        routingModel.PlateNoPlant2 = arrayPrint[1];
                        routingModel.PlateNo3 = arrayPrint[2];
                        routingModel.PlateNoPlant3 = arrayPrint[3];
                        routingModel.PlateNo4 = arrayPrint[4];
                        routingModel.PlateNoPlant4 = arrayPrint[5];
                        routingModel.PlateNo5 = arrayPrint[6];
                        routingModel.PlateNoPlant5 = arrayPrint[7];
                        break;

                    default:
                        break;
                }
            }

            if (arrayDiecut != null)
            {
                switch (arrayDiecut.Length)
                {
                    case 4:
                        routingModel.BlockNo2 = arrayDiecut[0];
                        routingModel.BlockNoPlant2 = arrayDiecut[1];
                        routingModel.MylaNo2 = arrayDiecut[2];
                        routingModel.MylaNoPlant2 = arrayDiecut[3];
                        routingModel.BlockNo3 = null;
                        routingModel.BlockNoPlant3 = null;
                        routingModel.MylaNo3 = null;
                        routingModel.MylaNoPlant3 = null;
                        routingModel.BlockNo4 = null;
                        routingModel.BlockNoPlant4 = null;
                        routingModel.MylaNo4 = null;
                        routingModel.MylaNoPlant4 = null;
                        routingModel.BlockNo5 = null;
                        routingModel.BlockNoPlant5 = null;
                        routingModel.MylaNo5 = null;
                        routingModel.MylaNoPlant5 = null;
                        break;

                    case 8:
                        routingModel.BlockNo2 = arrayDiecut[0];
                        routingModel.BlockNoPlant2 = arrayDiecut[1];
                        routingModel.MylaNo2 = arrayDiecut[2];
                        routingModel.MylaNoPlant2 = arrayDiecut[3];
                        routingModel.BlockNo3 = arrayDiecut[4];
                        routingModel.BlockNoPlant3 = arrayDiecut[5];
                        routingModel.MylaNo3 = arrayDiecut[6];
                        routingModel.MylaNoPlant3 = arrayDiecut[7];
                        routingModel.BlockNo4 = null;
                        routingModel.BlockNoPlant4 = null;
                        routingModel.MylaNo4 = null;
                        routingModel.MylaNoPlant4 = null;
                        routingModel.BlockNo5 = null;
                        routingModel.BlockNoPlant5 = null;
                        routingModel.MylaNo5 = null;
                        routingModel.MylaNoPlant5 = null;
                        break;

                    case 12:
                        routingModel.BlockNo2 = arrayDiecut[0];
                        routingModel.BlockNoPlant2 = arrayDiecut[1];
                        routingModel.MylaNo2 = arrayDiecut[2];
                        routingModel.MylaNoPlant2 = arrayDiecut[3];
                        routingModel.BlockNo3 = arrayDiecut[4];
                        routingModel.BlockNoPlant3 = arrayDiecut[5];
                        routingModel.MylaNo3 = arrayDiecut[6];
                        routingModel.MylaNoPlant3 = arrayDiecut[7];
                        routingModel.BlockNo4 = arrayDiecut[8];
                        routingModel.BlockNoPlant4 = arrayDiecut[9];
                        routingModel.MylaNo4 = arrayDiecut[10];
                        routingModel.MylaNoPlant4 = arrayDiecut[11];
                        routingModel.BlockNo5 = null;
                        routingModel.BlockNoPlant5 = null;
                        routingModel.MylaNo5 = null;
                        routingModel.MylaNoPlant5 = null;
                        break;

                    case 16:
                        routingModel.BlockNo2 = arrayDiecut[0];
                        routingModel.BlockNoPlant2 = arrayDiecut[1];
                        routingModel.MylaNo2 = arrayDiecut[2];
                        routingModel.MylaNoPlant2 = arrayDiecut[3];
                        routingModel.BlockNo3 = arrayDiecut[4];
                        routingModel.BlockNoPlant3 = arrayDiecut[5];
                        routingModel.MylaNo3 = arrayDiecut[6];
                        routingModel.MylaNoPlant3 = arrayDiecut[7];
                        routingModel.BlockNo4 = arrayDiecut[8];
                        routingModel.BlockNoPlant4 = arrayDiecut[9];
                        routingModel.MylaNo4 = arrayDiecut[10];
                        routingModel.MylaNoPlant4 = arrayDiecut[11];
                        routingModel.BlockNo5 = arrayDiecut[12];
                        routingModel.BlockNoPlant5 = arrayDiecut[13];
                        routingModel.MylaNo5 = arrayDiecut[14];
                        routingModel.MylaNoPlant5 = arrayDiecut[15];
                        break;

                    default:
                        break;
                }
            }

            routingModel.Totalcolor = transactionDataModel.modelRouting.Totalcolor;

            retuneTransactionModel = routingModel;
        }

        public RoutingDataModel MappingModelRoutingUpdateAndDelete(TransactionDataModel sessionModel, TransactionDataModel transactionDataModel)
        {
            string[] arrayDiecut = null;// = JsonConvert.DeserializeObject<string[]>(transactionDataModel.arrayDiecut);
            if (transactionDataModel.arrayDiecut != null)
            {
                arrayDiecut = JsonConvert.DeserializeObject<string[]>(transactionDataModel.arrayDiecut);
            }
            string[] arrayPrint = null;//= JsonConvert.DeserializeObject<string[]>(transactionDataModel.arrayPrint);
            if (transactionDataModel.arrayPrint != null)
            {
                arrayPrint = JsonConvert.DeserializeObject<string[]>(transactionDataModel.arrayPrint);
            }

            RoutingDataModel routingModel = new RoutingDataModel();
            routingModel.GroupMachine = transactionDataModel.modelRouting.GroupMachine;
            routingModel.Machine = transactionDataModel.modelRouting.Machine;
            routingModel.MachineMove = transactionDataModel.modelRouting.MachineMove;
            routingModel.Alternative1 = transactionDataModel.modelRouting.Alternative1;
            routingModel.Alternative2 = transactionDataModel.modelRouting.Alternative2;
            routingModel.Alternative3 = transactionDataModel.modelRouting.Alternative3;
            routingModel.Alternative4 = transactionDataModel.modelRouting.Alternative4;
            routingModel.Alternative5 = transactionDataModel.modelRouting.Alternative5;
            routingModel.Alternative6 = transactionDataModel.modelRouting.Alternative6;
            routingModel.Alternative7 = transactionDataModel.modelRouting.Alternative7;
            routingModel.Alternative8 = transactionDataModel.modelRouting.Alternative8;
            routingModel.NoOpenIn = transactionDataModel.modelRouting.NoOpenIn;
            routingModel.NoOpenOut = transactionDataModel.modelRouting.NoOpenOut;
            routingModel.WeightIn = transactionDataModel.modelRouting.WeightIn;
            routingModel.WeightOut = transactionDataModel.modelRouting.WeightOut;
            routingModel.SheetLengthIn = transactionDataModel.modelRouting.SheetLengthIn;
            routingModel.SheetLengthOut = transactionDataModel.modelRouting.SheetLengthOut;
            routingModel.SheetWidthIn = transactionDataModel.modelRouting.SheetWidthIn;
            routingModel.SheetWidthOut = transactionDataModel.modelRouting.SheetWidthOut;
            routingModel.Coat = transactionDataModel.modelRouting.Coat;
            routingModel.PaperRollWidth = transactionDataModel.modelRouting.PaperRollWidth;
            routingModel.Cut = transactionDataModel.modelRouting.Cut;
            routingModel.Trim = transactionDataModel.modelRouting.Trim;
            routingModel.PercentTrim = transactionDataModel.modelRouting.PercentTrim;
            routingModel.TearTape = transactionDataModel.modelRouting.TearTape;
            routingModel.LineQtyPerBox = transactionDataModel.modelRouting.LineQtyPerBox;
            routingModel.MarginForPaper = transactionDataModel.modelRouting.MarginForPaper;
            routingModel.Ink1 = transactionDataModel.modelRouting.Ink1 == null || transactionDataModel.modelRouting.Ink1.Trim() == "--- Choose here ---" ? null : transactionDataModel.modelRouting.Ink1;
            routingModel.Ink2 = transactionDataModel.modelRouting.Ink2 == null || transactionDataModel.modelRouting.Ink2.Trim() == "--- Choose here ---" ? null : transactionDataModel.modelRouting.Ink2;
            routingModel.Ink3 = transactionDataModel.modelRouting.Ink3 == null || transactionDataModel.modelRouting.Ink3.Trim() == "--- Choose here ---" ? null : transactionDataModel.modelRouting.Ink3;
            routingModel.Ink4 = transactionDataModel.modelRouting.Ink4 == null || transactionDataModel.modelRouting.Ink4.Trim() == "--- Choose here ---" ? null : transactionDataModel.modelRouting.Ink4;
            routingModel.Ink5 = transactionDataModel.modelRouting.Ink5 == null || transactionDataModel.modelRouting.Ink5.Trim() == "--- Choose here ---" ? null : transactionDataModel.modelRouting.Ink5;
            routingModel.Ink6 = transactionDataModel.modelRouting.Ink6 == null || transactionDataModel.modelRouting.Ink6.Trim() == "--- Choose here ---" ? null : transactionDataModel.modelRouting.Ink6;
            routingModel.Ink7 = transactionDataModel.modelRouting.Ink7 == null || transactionDataModel.modelRouting.Ink7.Trim() == "--- Choose here ---" ? null : transactionDataModel.modelRouting.Ink7;
            routingModel.Ink8 = transactionDataModel.modelRouting.Ink8 == null || transactionDataModel.modelRouting.Ink8.Trim() == "--- Choose here ---" ? null : transactionDataModel.modelRouting.Ink8;
            routingModel.Shade1 = transactionDataModel.modelRouting.Shade1 == null || transactionDataModel.modelRouting.Shade1.Trim() == "--- Choose here ---" ? null : transactionDataModel.modelRouting.Shade1;
            routingModel.Shade2 = transactionDataModel.modelRouting.Shade2 == null || transactionDataModel.modelRouting.Shade2.Trim() == "--- Choose here ---" ? null : transactionDataModel.modelRouting.Shade2;
            routingModel.Shade3 = transactionDataModel.modelRouting.Shade3 == null || transactionDataModel.modelRouting.Shade3.Trim() == "--- Choose here ---" ? null : transactionDataModel.modelRouting.Shade3;
            routingModel.Shade4 = transactionDataModel.modelRouting.Shade4 == null || transactionDataModel.modelRouting.Shade4.Trim() == "--- Choose here ---" ? null : transactionDataModel.modelRouting.Shade4;
            routingModel.Shade5 = transactionDataModel.modelRouting.Shade5 == null || transactionDataModel.modelRouting.Shade5.Trim() == "--- Choose here ---" ? null : transactionDataModel.modelRouting.Shade5;
            routingModel.Shade6 = transactionDataModel.modelRouting.Shade6 == null || transactionDataModel.modelRouting.Shade6.Trim() == "--- Choose here ---" ? null : transactionDataModel.modelRouting.Shade6;
            routingModel.Shade7 = transactionDataModel.modelRouting.Shade7 == null || transactionDataModel.modelRouting.Shade7.Trim() == "--- Choose here ---" ? null : transactionDataModel.modelRouting.Shade7;
            routingModel.Shade8 = transactionDataModel.modelRouting.Shade8 == null || transactionDataModel.modelRouting.Shade8.Trim() == "--- Choose here ---" ? null : transactionDataModel.modelRouting.Shade8;
            routingModel.Area1 = transactionDataModel.modelRouting.Area1;
            routingModel.Area2 = transactionDataModel.modelRouting.Area2;
            routingModel.Area3 = transactionDataModel.modelRouting.Area3;
            routingModel.Area4 = transactionDataModel.modelRouting.Area4;
            routingModel.Area5 = transactionDataModel.modelRouting.Area5;
            routingModel.Area6 = transactionDataModel.modelRouting.Area6;
            routingModel.Area7 = transactionDataModel.modelRouting.Area7;
            routingModel.Area8 = transactionDataModel.modelRouting.Area8;
            routingModel.PrintingPlateNo = transactionDataModel.modelRouting.PrintingPlateNo;
            routingModel.CuttingDieNo = transactionDataModel.modelRouting.CuttingDieNo;
            routingModel.RepeatLength = transactionDataModel.modelRouting.RepeatLength;
            routingModel.CustBarcodeNo = transactionDataModel.modelRouting.CustBarcodeNo;

            if (transactionDataModel.modelRouting.MylaNo != null)
            {
                routingModel.MylaNo = transactionDataModel.modelRouting.MylaNo;
            }
            else
            {
                routingModel.MylaNo = transactionDataModel.modelRouting.MylaNo_Copy;
            }

            routingModel.PrintingPlateType = transactionDataModel.modelRouting.PrintingPlateType;
            routingModel.JoinToMaterialNo = transactionDataModel.modelRouting.JoinToMaterialNo;
            routingModel.SperateToMaterialNo = transactionDataModel.modelRouting.SperateToMaterialNo;
            routingModel.Remark = transactionDataModel.modelRouting.Remark;
            routingModel.MachineGroupSelect = transactionDataModel.modelRouting.MachineGroupSelect;

            routingModel.Plant = transactionDataModel.FactoryCode;
            //routingModel.Plant_Code = "Machine";
            //routingModel.StdProcess = true;
            routingModel.HandHold = false;
            // routingModel.Platen =
            //routingModel.Rotary
            //routingModel.Hardship
            routingModel.UnUpgrad_Board = false;
            //routingModel.Color_count =
            //routingModel.Human =

            routingModel.Speed = transactionDataModel.modelRouting.Speed;
            routingModel.SetupTm = transactionDataModel.modelRouting.SetupTm;
            routingModel.PrepareTm = transactionDataModel.modelRouting.PrepareTm;
            routingModel.PostTm = transactionDataModel.modelRouting.PostTm;
            routingModel.SetupWaste = transactionDataModel.modelRouting.SetupWaste;
            routingModel.RunWaste = transactionDataModel.modelRouting.RunWaste;
            routingModel.StackHeight = transactionDataModel.modelRouting.StackHeight;
            routingModel.RotateIn = transactionDataModel.modelRouting.RotateIn;
            routingModel.RotateOut = transactionDataModel.modelRouting.RotateOut;

            routingModel.TearTapeQty = transactionDataModel.modelRouting.LineQtyPerBox; //  string.IsNullOrEmpty(transactionDataModel.modelRouting.LineQtyPerBox) ? 0 : Convert.ToInt16(transactionDataModel.modelRouting.LineQtyPerBox); ;
            routingModel.TearTapeDistance = transactionDataModel.modelRouting.MarginForPaper;

            routingModel.ScoreGap = transactionDataModel.modelRouting.ScoreGap;
            routingModel.ScoreType = transactionDataModel.modelRouting.ScoreType;

            routingModel.MylaSize = transactionDataModel.modelRouting.MylaSize;
            routingModel.AutoCal = transactionDataModel.modelRouting.AutoCal;

            switch (transactionDataModel.modelRouting.PrintingPlateType)
            {
                case "Non-Print":
                    routingModel.NoneBlk = true;
                    routingModel.StanBlk = false;
                    routingModel.SemiBlk = false;
                    routingModel.ShipBlk = false;
                    break;

                case "Standard":
                    routingModel.NoneBlk = false;
                    routingModel.StanBlk = true;
                    routingModel.SemiBlk = false;
                    routingModel.ShipBlk = false;
                    break;

                case "Semi":
                    routingModel.NoneBlk = false;
                    routingModel.StanBlk = false;
                    routingModel.SemiBlk = true;
                    routingModel.ShipBlk = false;
                    break;

                case "Shipping Mark":
                    routingModel.NoneBlk = false;
                    routingModel.StanBlk = false;
                    routingModel.SemiBlk = false;
                    routingModel.ShipBlk = true;
                    break;
            }

            switch (transactionDataModel.modelRouting.Rotate)
            {
                case "noRotate":
                    routingModel.RotateIn = false;
                    routingModel.RotateOut = false;
                    break;

                case "rotateIn":
                    routingModel.RotateIn = true;
                    routingModel.RotateOut = false;
                    break;

                case "rotateOut":
                    routingModel.RotateIn = false;
                    routingModel.RotateOut = true;
                    break;
            }

            if (arrayPrint != null)
            {
                switch (arrayPrint.Length)
                {
                    case 2:
                        routingModel.PlateNo2 = arrayPrint[0];
                        routingModel.PlateNoPlant2 = arrayPrint[1];
                        routingModel.PlateNo3 = null;
                        routingModel.PlateNoPlant3 = null;
                        routingModel.PlateNo4 = null;
                        routingModel.PlateNoPlant4 = null;
                        routingModel.PlateNo5 = null;
                        routingModel.PlateNoPlant5 = null;
                        break;

                    case 4:
                        routingModel.PlateNo2 = arrayPrint[0];
                        routingModel.PlateNoPlant2 = arrayPrint[1];
                        routingModel.PlateNo3 = arrayPrint[2];
                        routingModel.PlateNoPlant3 = arrayPrint[3];
                        routingModel.PlateNo4 = null;
                        routingModel.PlateNoPlant4 = null;
                        routingModel.PlateNo5 = null;
                        routingModel.PlateNoPlant5 = null;
                        break;

                    case 6:
                        routingModel.PlateNo2 = arrayPrint[0];
                        routingModel.PlateNoPlant2 = arrayPrint[1];
                        routingModel.PlateNo3 = arrayPrint[2];
                        routingModel.PlateNoPlant3 = arrayPrint[3];
                        routingModel.PlateNo4 = arrayPrint[4];
                        routingModel.PlateNoPlant4 = arrayPrint[5];
                        routingModel.PlateNo5 = null;
                        routingModel.PlateNoPlant5 = null;
                        break;

                    case 8:
                        routingModel.PlateNo2 = arrayPrint[0];
                        routingModel.PlateNoPlant2 = arrayPrint[1];
                        routingModel.PlateNo3 = arrayPrint[2];
                        routingModel.PlateNoPlant3 = arrayPrint[3];
                        routingModel.PlateNo4 = arrayPrint[4];
                        routingModel.PlateNoPlant4 = arrayPrint[5];
                        routingModel.PlateNo5 = arrayPrint[6];
                        routingModel.PlateNoPlant5 = arrayPrint[7];
                        break;

                    default:
                        break;
                }
            }

            if (arrayDiecut != null)
            {
                switch (arrayDiecut.Length)
                {
                    case 4:
                        routingModel.BlockNo2 = arrayDiecut[0];
                        routingModel.BlockNoPlant2 = arrayDiecut[1];
                        routingModel.MylaNo2 = arrayDiecut[2];
                        routingModel.MylaNoPlant2 = arrayDiecut[3];
                        routingModel.BlockNo3 = null;
                        routingModel.BlockNoPlant3 = null;
                        routingModel.MylaNo3 = null;
                        routingModel.MylaNoPlant3 = null;
                        routingModel.BlockNo4 = null;
                        routingModel.BlockNoPlant4 = null;
                        routingModel.MylaNo4 = null;
                        routingModel.MylaNoPlant4 = null;
                        routingModel.BlockNo5 = null;
                        routingModel.BlockNoPlant5 = null;
                        routingModel.MylaNo5 = null;
                        routingModel.MylaNoPlant5 = null;
                        break;

                    case 8:
                        routingModel.BlockNo2 = arrayDiecut[0];
                        routingModel.BlockNoPlant2 = arrayDiecut[1];
                        routingModel.MylaNo2 = arrayDiecut[2];
                        routingModel.MylaNoPlant2 = arrayDiecut[3];
                        routingModel.BlockNo3 = arrayDiecut[4];
                        routingModel.BlockNoPlant3 = arrayDiecut[5];
                        routingModel.MylaNo3 = arrayDiecut[6];
                        routingModel.MylaNoPlant3 = arrayDiecut[7];
                        routingModel.BlockNo4 = null;
                        routingModel.BlockNoPlant4 = null;
                        routingModel.MylaNo4 = null;
                        routingModel.MylaNoPlant4 = null;
                        routingModel.BlockNo5 = null;
                        routingModel.BlockNoPlant5 = null;
                        routingModel.MylaNo5 = null;
                        routingModel.MylaNoPlant5 = null;
                        break;

                    case 12:
                        routingModel.BlockNo2 = arrayDiecut[0];
                        routingModel.BlockNoPlant2 = arrayDiecut[1];
                        routingModel.MylaNo2 = arrayDiecut[2];
                        routingModel.MylaNoPlant2 = arrayDiecut[3];
                        routingModel.BlockNo3 = arrayDiecut[4];
                        routingModel.BlockNoPlant3 = arrayDiecut[5];
                        routingModel.MylaNo3 = arrayDiecut[6];
                        routingModel.MylaNoPlant3 = arrayDiecut[7];
                        routingModel.BlockNo4 = arrayDiecut[8];
                        routingModel.BlockNoPlant4 = arrayDiecut[9];
                        routingModel.MylaNo4 = arrayDiecut[10];
                        routingModel.MylaNoPlant4 = arrayDiecut[11];
                        routingModel.BlockNo5 = null;
                        routingModel.BlockNoPlant5 = null;
                        routingModel.MylaNo5 = null;
                        routingModel.MylaNoPlant5 = null;
                        break;

                    case 16:
                        routingModel.BlockNo2 = arrayDiecut[0];
                        routingModel.BlockNoPlant2 = arrayDiecut[1];
                        routingModel.MylaNo2 = arrayDiecut[2];
                        routingModel.MylaNoPlant2 = arrayDiecut[3];
                        routingModel.BlockNo3 = arrayDiecut[4];
                        routingModel.BlockNoPlant3 = arrayDiecut[5];
                        routingModel.MylaNo3 = arrayDiecut[6];
                        routingModel.MylaNoPlant3 = arrayDiecut[7];
                        routingModel.BlockNo4 = arrayDiecut[8];
                        routingModel.BlockNoPlant4 = arrayDiecut[9];
                        routingModel.MylaNo4 = arrayDiecut[10];
                        routingModel.MylaNoPlant4 = arrayDiecut[11];
                        routingModel.BlockNo5 = arrayDiecut[12];
                        routingModel.BlockNoPlant5 = arrayDiecut[13];
                        routingModel.MylaNo5 = arrayDiecut[14];
                        routingModel.MylaNoPlant5 = arrayDiecut[15];
                        break;

                    default:
                        break;
                }
            }
            routingModel.Totalcolor = transactionDataModel.modelRouting.Totalcolor;

            return routingModel;
        }

        public TransactionDataModel UpdateRouting(TransactionDataModel model, TransactionDataModel modelToUpdate, RoutingDataModel routingModel)
        {
            model.modelRouting.RoutingDataList.Where(w => w.SeqNo == modelToUpdate.modelRouting.SeqNo).ToList().ForEach(i =>
            {
                i.GroupMachine = routingModel.GroupMachine;
                i.Machine = routingModel.Machine;
                i.MachineMove = routingModel.MachineMove;
                i.Alternative1 = routingModel.Alternative1;
                i.Alternative2 = routingModel.Alternative2;
                i.Alternative3 = routingModel.Alternative3;
                i.Alternative4 = routingModel.Alternative4;
                i.Alternative5 = routingModel.Alternative5;
                i.Alternative6 = routingModel.Alternative6;
                i.Alternative7 = routingModel.Alternative7;
                i.Alternative8 = routingModel.Alternative8;
                i.NoOpenIn = routingModel.NoOpenIn;
                i.NoOpenOut = routingModel.NoOpenOut;
                i.WeightIn = routingModel.WeightIn;
                i.WeightOut = routingModel.WeightOut;
                i.SheetLengthIn = routingModel.SheetLengthIn;
                i.SheetLengthOut = routingModel.SheetLengthOut;
                i.SheetWidthIn = routingModel.SheetWidthIn;
                i.SheetWidthOut = routingModel.SheetWidthOut;
                i.Coat = routingModel.Coat;
                i.PaperRollWidth = routingModel.PaperRollWidth;
                i.Cut = routingModel.Cut;
                i.Trim = routingModel.Trim;
                i.PercentTrim = routingModel.PercentTrim;
                i.TearTape = routingModel.TearTape;
                i.LineQtyPerBox = routingModel.LineQtyPerBox;
                i.MarginForPaper = routingModel.MarginForPaper;
                i.Ink1 = routingModel.Ink1;
                i.Ink2 = routingModel.Ink2;
                i.Ink3 = routingModel.Ink3;
                i.Ink4 = routingModel.Ink4;
                i.Ink5 = routingModel.Ink5;
                i.Ink6 = routingModel.Ink6;
                i.Ink7 = routingModel.Ink7;
                i.Ink8 = routingModel.Ink8;
                i.Shade1 = routingModel.Shade1;
                i.Shade2 = routingModel.Shade2;
                i.Shade3 = routingModel.Shade3;
                i.Shade4 = routingModel.Shade4;
                i.Shade5 = routingModel.Shade5;
                i.Shade6 = routingModel.Shade6;
                i.Shade7 = routingModel.Shade7;
                i.Shade8 = routingModel.Shade8;
                i.Area1 = routingModel.Area1;
                i.Area2 = routingModel.Area2;
                i.Area3 = routingModel.Area3;
                i.Area4 = routingModel.Area4;
                i.Area5 = routingModel.Area5;
                i.Area6 = routingModel.Area6;
                i.Area7 = routingModel.Area7;
                i.Area8 = routingModel.Area8;
                i.PrintingPlateNo = routingModel.PrintingPlateNo;
                i.CuttingDieNo = routingModel.CuttingDieNo;
                i.MylaNo = routingModel.MylaNo;
                i.PrintingPlateType = routingModel.PrintingPlateType;
                i.JoinToMaterialNo = routingModel.JoinToMaterialNo;
                i.SperateToMaterialNo = routingModel.SperateToMaterialNo;
                i.Remark = routingModel.Remark;
                i.MachineGroupSelect = routingModel.MachineGroupSelect;

                i.Plant = string.IsNullOrEmpty(modelToUpdate.FactoryCode) ? i.Plant : modelToUpdate.FactoryCode;

                //routingModel.Plant_Code = "Machine";
                //routingModel.StdProcess = true;
                i.HandHold = false;
                // routingModel.Platen =
                //routingModel.Rotary
                //routingModel.Hardship
                i.UnUpgrad_Board = false;
                //routingModel.Color_count =
                //routingModel.Human =

                i.TearTapeQty = routingModel.LineQtyPerBox;
                i.TearTapeDistance = routingModel.MarginForPaper;

                i.NoneBlk = routingModel.NoneBlk;
                i.StanBlk = routingModel.StanBlk;
                i.SemiBlk = routingModel.SemiBlk;
                i.ShipBlk = routingModel.ShipBlk;

                i.PlateNo2 = routingModel.PlateNo2;
                i.PlateNoPlant2 = routingModel.PlateNoPlant2;
                i.PlateNo3 = routingModel.PlateNo3;
                i.PlateNoPlant3 = routingModel.PlateNoPlant3;
                i.PlateNo4 = routingModel.PlateNo4;
                i.PlateNoPlant4 = routingModel.PlateNoPlant4;
                i.PlateNo5 = routingModel.PlateNo5;
                i.PlateNoPlant5 = routingModel.PlateNoPlant5;

                i.BlockNo2 = routingModel.BlockNo2;
                i.BlockNoPlant2 = routingModel.BlockNoPlant2;
                i.MylaNo2 = routingModel.MylaNo2;
                i.MylaNoPlant2 = routingModel.MylaNoPlant2;
                i.BlockNo3 = routingModel.BlockNo3;
                i.BlockNoPlant3 = routingModel.BlockNoPlant3;
                i.MylaNo3 = routingModel.MylaNo3;
                i.MylaNoPlant3 = routingModel.MylaNoPlant3;
                i.BlockNo4 = routingModel.BlockNo4;
                i.BlockNoPlant4 = routingModel.BlockNoPlant4;
                i.MylaNo4 = routingModel.MylaNo4;
                i.MylaNoPlant4 = routingModel.MylaNoPlant4;
                i.BlockNo5 = routingModel.BlockNo5;
                i.BlockNoPlant5 = routingModel.BlockNoPlant5;
                i.MylaNo5 = routingModel.MylaNo5;
                i.MylaNoPlant5 = routingModel.MylaNoPlant5;

                i.Speed = routingModel.Speed;
                i.SetupTm = routingModel.SetupTm;
                i.PrepareTm = routingModel.PrepareTm;
                i.PostTm = routingModel.PostTm;
                i.SetupWaste = routingModel.SetupWaste;
                i.RunWaste = routingModel.RunWaste;
                i.StackHeight = routingModel.StackHeight;
                i.RotateIn = routingModel.RotateIn;
                i.RotateOut = routingModel.RotateOut;

                i.ScoreGap = routingModel.ScoreGap;
                i.ScoreType = routingModel.ScoreType;

                i.MylaSize = routingModel.MylaSize;
                i.RepeatLength = routingModel.RepeatLength;
                i.CustBarcodeNo = routingModel.CustBarcodeNo;
                i.Totalcolor = routingModel.Totalcolor;
                i.AutoCal = modelToUpdate.modelRouting.AutoCal;
            });

            UpdateRouting(model);
            model.modelRouting.RoutingDataList.Clear();
            model.modelRouting.RoutingDataList = GetRoutingList(model.MaterialNo).OrderBy(x => x.SeqNo).ToList();

            if (modelToUpdate.modelRouting.RemarkAttachFileStatus == 0)
            {
                model.modelRouting.RoutingDataList.First(i => i.SeqNo == modelToUpdate.modelRouting.SeqNo).RemarkImageFile = null;
                model.modelRouting.RoutingDataList.First(i => i.SeqNo == modelToUpdate.modelRouting.SeqNo).RemarkImageFileName = null;
            }
            return model;
        }

        public TransactionDataModel InsertRouting(TransactionDataModel model, TransactionDataModel transactionDataModel, RoutingDataModel routingModel)
        {
            string plantLast = model.modelRouting.RoutingDataList.Select(x => x.Plant).First();
            model.modelRouting.RoutingDataList.Insert(transactionDataModel.modelRouting.SeqNo - 1, new RoutingDataModel
            {
                GroupMachine = routingModel.GroupMachine,
                Machine = routingModel.Machine,
                MachineMove = routingModel.MachineMove,
                Alternative1 = routingModel.Alternative1,
                Alternative2 = routingModel.Alternative2,
                Alternative3 = routingModel.Alternative3,
                Alternative4 = routingModel.Alternative4,
                Alternative5 = routingModel.Alternative5,
                Alternative6 = routingModel.Alternative6,
                Alternative7 = routingModel.Alternative7,
                Alternative8 = routingModel.Alternative8,
                NoOpenIn = routingModel.NoOpenIn,
                NoOpenOut = routingModel.NoOpenOut,
                WeightIn = routingModel.WeightIn,
                WeightOut = routingModel.WeightOut,
                SheetLengthIn = routingModel.SheetLengthIn,
                SheetLengthOut = routingModel.SheetLengthOut,
                SheetWidthIn = routingModel.SheetWidthIn,
                SheetWidthOut = routingModel.SheetWidthOut,
                Coat = routingModel.Coat,
                PaperRollWidth = routingModel.PaperRollWidth,
                Cut = routingModel.Cut,
                Trim = routingModel.Trim,
                PercentTrim = routingModel.PercentTrim,
                TearTape = routingModel.TearTape,
                LineQtyPerBox = routingModel.LineQtyPerBox,
                MarginForPaper = routingModel.MarginForPaper,
                Ink1 = routingModel.Ink1,
                Ink2 = routingModel.Ink2,
                Ink3 = routingModel.Ink3,
                Ink4 = routingModel.Ink4,
                Ink5 = routingModel.Ink5,
                Ink6 = routingModel.Ink6,
                Ink7 = routingModel.Ink7,
                Ink8 = routingModel.Ink8,
                Shade1 = routingModel.Shade1,
                Shade2 = routingModel.Shade2,
                Shade3 = routingModel.Shade3,
                Shade4 = routingModel.Shade4,
                Shade5 = routingModel.Shade5,
                Shade6 = routingModel.Shade6,
                Shade7 = routingModel.Shade7,
                Shade8 = routingModel.Shade8,
                Area1 = routingModel.Area1,
                Area2 = routingModel.Area2,
                Area3 = routingModel.Area3,
                Area4 = routingModel.Area4,
                Area5 = routingModel.Area5,
                Area6 = routingModel.Area6,
                Area7 = routingModel.Area7,
                Area8 = routingModel.Area8,
                PrintingPlateNo = routingModel.PrintingPlateNo,
                CuttingDieNo = routingModel.CuttingDieNo,
                MylaNo = routingModel.MylaNo,
                PrintingPlateType = routingModel.PrintingPlateType,
                JoinToMaterialNo = routingModel.JoinToMaterialNo,
                SperateToMaterialNo = routingModel.SperateToMaterialNo,
                Remark = routingModel.Remark,
                MachineGroupSelect = routingModel.MachineGroupSelect,

                Plant = plantLast,//routingModel.Plant,//transactionDataModel.FactoryCode,
                HandHold = false,
                UnUpgrad_Board = false,
                TearTapeQty = routingModel.LineQtyPerBox,
                TearTapeDistance = routingModel.MarginForPaper,

                NoneBlk = routingModel.NoneBlk,
                StanBlk = routingModel.StanBlk,
                SemiBlk = routingModel.SemiBlk,
                ShipBlk = routingModel.ShipBlk,

                PlateNo2 = routingModel.PlateNo2,
                PlateNoPlant2 = routingModel.PlateNoPlant2,
                PlateNo3 = routingModel.PlateNo3,
                PlateNoPlant3 = routingModel.PlateNoPlant3,
                PlateNo4 = routingModel.PlateNo4,
                PlateNoPlant4 = routingModel.PlateNoPlant4,
                PlateNo5 = routingModel.PlateNo5,
                PlateNoPlant5 = routingModel.PlateNoPlant5,

                BlockNo2 = routingModel.BlockNo2,
                BlockNoPlant2 = routingModel.BlockNoPlant2,
                MylaNo2 = routingModel.MylaNo2,
                MylaNoPlant2 = routingModel.MylaNoPlant2,
                BlockNo3 = routingModel.BlockNo3,
                BlockNoPlant3 = routingModel.BlockNoPlant3,
                MylaNo3 = routingModel.MylaNo3,
                MylaNoPlant3 = routingModel.MylaNoPlant3,
                BlockNo4 = routingModel.BlockNo4,
                BlockNoPlant4 = routingModel.BlockNoPlant4,
                MylaNo4 = routingModel.MylaNo4,
                MylaNoPlant4 = routingModel.MylaNoPlant4,
                BlockNo5 = routingModel.BlockNo5,
                BlockNoPlant5 = routingModel.BlockNoPlant5,
                MylaNo5 = routingModel.MylaNo5,
                MylaNoPlant5 = routingModel.MylaNoPlant5,

                Speed = routingModel.Speed,
                SetupTm = routingModel.SetupTm,
                PrepareTm = routingModel.PrepareTm,
                PostTm = routingModel.PostTm,
                SetupWaste = routingModel.SetupWaste,
                RunWaste = routingModel.RunWaste,
                StackHeight = routingModel.StackHeight,
                RotateIn = routingModel.RotateIn,
                RotateOut = routingModel.RotateOut,

                ScoreGap = routingModel.ScoreGap,
                ScoreType = routingModel.ScoreType,

                MylaSize = routingModel.MylaSize,
                RepeatLength = routingModel.RepeatLength,
                CustBarcodeNo = routingModel.CustBarcodeNo,
                Totalcolor = routingModel.Totalcolor,
                AutoCal = routingModel.AutoCal,
            });

            int seqNumber = 1;

            model.modelRouting.RoutingDataList.ForEach(i => { i.SeqNo = seqNumber; seqNumber++; });

            SaveRouting(model);
            model.modelRouting.RoutingDataList.Clear();
            model.modelRouting.RoutingDataList = GetRoutingList(model.MaterialNo).OrderBy(x => x.SeqNo).ToList();
            return model;
        }

        public TransactionDataModel CopyRouting(TransactionDataModel model, int seqNo)
        {
            List<RoutingDataModel> copyData = new List<RoutingDataModel>();

            model.modelRouting.RoutingDataList.ForEach(i =>
            {
                if (i.SeqNo != seqNo)
                {
                    copyData.Add(new RoutingDataModel
                    {
                        GroupMachine = i.GroupMachine,
                        Machine = i.Machine,
                        MachineMove = i.MachineMove,
                        Alternative1 = i.Alternative1,
                        Alternative2 = i.Alternative2,
                        Alternative3 = i.Alternative3,
                        Alternative4 = i.Alternative4,
                        Alternative5 = i.Alternative5,
                        Alternative6 = i.Alternative6,
                        Alternative7 = i.Alternative7,
                        Alternative8 = i.Alternative8,
                        NoOpenIn = i.NoOpenIn,
                        NoOpenOut = i.NoOpenOut,
                        WeightIn = i.WeightIn,
                        WeightOut = i.WeightOut,
                        SheetLengthIn = i.SheetLengthIn,
                        SheetLengthOut = i.SheetLengthOut,
                        SheetWidthIn = i.SheetWidthIn,
                        SheetWidthOut = i.SheetWidthOut,
                        Coat = i.Coat,
                        PaperRollWidth = i.PaperRollWidth,
                        Cut = i.Cut,
                        Trim = i.Trim,
                        PercentTrim = i.PercentTrim,
                        TearTape = i.TearTape,
                        LineQtyPerBox = i.LineQtyPerBox,
                        MarginForPaper = i.MarginForPaper,
                        Ink1 = i.Ink1,
                        Ink2 = i.Ink2,
                        Ink3 = i.Ink3,
                        Shade1 = i.Shade1,
                        Shade2 = i.Shade2,
                        Shade3 = i.Shade3,
                        Area1 = i.Area1,
                        Area2 = i.Area2,
                        Area3 = i.Area3,
                        PrintingPlateNo = i.PrintingPlateNo,
                        CuttingDieNo = i.CuttingDieNo,
                        MylaNo = i.MylaNo,
                        PrintingPlateType = i.PrintingPlateType,
                        JoinToMaterialNo = i.JoinToMaterialNo,
                        SperateToMaterialNo = i.SperateToMaterialNo,
                        Remark = i.Remark,
                        MachineGroupSelect = i.MachineGroupSelect,
                    });
                }
                else
                {
                    copyData.Add(new RoutingDataModel
                    {
                        GroupMachine = i.GroupMachine,
                        Machine = i.Machine,
                        MachineMove = i.MachineMove,
                        Alternative1 = i.Alternative1,
                        Alternative2 = i.Alternative2,
                        Alternative3 = i.Alternative3,
                        Alternative4 = i.Alternative4,
                        Alternative5 = i.Alternative5,
                        Alternative6 = i.Alternative6,
                        Alternative7 = i.Alternative7,
                        Alternative8 = i.Alternative8,
                        NoOpenIn = i.NoOpenIn,
                        NoOpenOut = i.NoOpenOut,
                        WeightIn = i.WeightIn,
                        WeightOut = i.WeightOut,
                        SheetLengthIn = i.SheetLengthIn,
                        SheetLengthOut = i.SheetLengthOut,
                        SheetWidthIn = i.SheetWidthIn,
                        SheetWidthOut = i.SheetWidthOut,
                        Coat = i.Coat,
                        PaperRollWidth = i.PaperRollWidth,
                        Cut = i.Cut,
                        Trim = i.Trim,
                        PercentTrim = i.PercentTrim,
                        TearTape = i.TearTape,
                        LineQtyPerBox = i.LineQtyPerBox,
                        MarginForPaper = i.MarginForPaper,
                        Ink1 = i.Ink1,
                        Ink2 = i.Ink2,
                        Ink3 = i.Ink3,
                        Shade1 = i.Shade1,
                        Shade2 = i.Shade2,
                        Shade3 = i.Shade3,
                        Area1 = i.Area1,
                        Area2 = i.Area2,
                        Area3 = i.Area3,
                        PrintingPlateNo = i.PrintingPlateNo,
                        CuttingDieNo = i.CuttingDieNo,
                        MylaNo = i.MylaNo,
                        PrintingPlateType = i.PrintingPlateType,
                        JoinToMaterialNo = i.JoinToMaterialNo,
                        SperateToMaterialNo = i.SperateToMaterialNo,
                        Remark = i.Remark,
                        MachineGroupSelect = i.MachineGroupSelect,
                    });

                    copyData.Add(new RoutingDataModel
                    {
                        GroupMachine = i.GroupMachine,
                        Machine = i.Machine,
                        MachineMove = i.MachineMove,
                        Alternative1 = i.Alternative1,
                        Alternative2 = i.Alternative2,
                        Alternative3 = i.Alternative3,
                        Alternative4 = i.Alternative4,
                        Alternative5 = i.Alternative5,
                        Alternative6 = i.Alternative6,
                        Alternative7 = i.Alternative7,
                        Alternative8 = i.Alternative8,
                        NoOpenIn = i.NoOpenIn,
                        NoOpenOut = i.NoOpenOut,
                        WeightIn = i.WeightIn,
                        WeightOut = i.WeightOut,
                        SheetLengthIn = i.SheetLengthIn,
                        SheetLengthOut = i.SheetLengthOut,
                        SheetWidthIn = i.SheetWidthIn,
                        SheetWidthOut = i.SheetWidthOut,
                        Coat = i.Coat,
                        PaperRollWidth = i.PaperRollWidth,
                        Cut = i.Cut,
                        Trim = i.Trim,
                        PercentTrim = i.PercentTrim,
                        TearTape = i.TearTape,
                        LineQtyPerBox = i.LineQtyPerBox,
                        MarginForPaper = i.MarginForPaper,
                        Ink1 = i.Ink1,
                        Ink2 = i.Ink2,
                        Ink3 = i.Ink3,
                        Shade1 = i.Shade1,
                        Shade2 = i.Shade2,
                        Shade3 = i.Shade3,
                        Area1 = i.Area1,
                        Area2 = i.Area2,
                        Area3 = i.Area3,
                        PrintingPlateNo = i.PrintingPlateNo,
                        CuttingDieNo = i.CuttingDieNo,
                        MylaNo = i.MylaNo,
                        PrintingPlateType = i.PrintingPlateType,
                        JoinToMaterialNo = i.JoinToMaterialNo,
                        SperateToMaterialNo = i.SperateToMaterialNo,
                        Remark = i.Remark,
                        MachineGroupSelect = i.MachineGroupSelect,
                        CopyStatus = true
                    });
                }
            });

            int seqNumber = 1;

            model.modelRouting.RoutingDataList = copyData;

            model.modelRouting.RoutingDataList.ForEach(i => { i.SeqNo = seqNumber; seqNumber++; });

            SaveRouting(model);
            model.modelRouting.RoutingDataList.Clear();
            model.modelRouting.RoutingDataList = GetRoutingList(model.MaterialNo).OrderBy(x => x.SeqNo).ToList();
            return model;
        }

        public void UpdatePDISStatus(string Factorycode, string MaterialNo, string Status)
        {
            _masterDataAPIRepository.UpdateMasterDataPDISStatus(_factoryCode, MaterialNo, Status, _token);
        }

        public void UpdatePlantViewShipBlk(string MaterialNo, string Status)
        {
            _plantViewAPIRepository.UpdatePlantViewShipBlk(_factoryCode, MaterialNo, Status, _token);
        }

        public List<QualitySpec> GetQualitySpecsByMaterial(string Material)
        {
            return JsonConvert.DeserializeObject<List<QualitySpec>>(_qualitySpecAPIRepository.GetQualitySpecByMaterialNo(_factoryCode, Material, _token));
        }

        public void DeleteRoutingByMaterialNoAndFactoryAndSeq(string Material, string Seq)
        {
            _routingAPIRepository.DeleteRoutingByMaterialNoAndFactoryAndSeq(_factoryCode, Material, Seq, _token);
        }

        public void UpdateRoutingPDISStatusEmployment(string MaterialNo, string Status, string SaleOrg)
        {
            _masterDataAPIRepository.UpdateRoutingPDISStatusEmployment(_factoryCode, MaterialNo, Status, SaleOrg, _token);
        }

        //Auto Routing Tassanai

        public List<RoutingDataModel> AutoRoutingList(TransactionDataModel model)
        {
            // get LV2
            //1. Find Machine from Group to API
            var HieLV2 = model.modelCategories.HierarchyLV2.Trim();
            List<RoutingDataModel> routing = new List<RoutingDataModel>();
            List<Machine> MachineResult = new List<Machine>();
            RoutingDataModel tmp = new RoutingDataModel();

            var seqNum = 0;
            /// Check งานจ้างหรือไม่
            if (model.modelProductProp.StatusFlag == "จ้างผลิต")
            {
                //1. find plancode from PMTsConfig
                var plancode = JsonConvert.DeserializeObject<PmtsConfig>(_pmtsConfigAPIRepository.GetPMTsConfigByFactoryName(_factoryCode, "AutoRouting1st", _token)).FucValue;

                if (String.IsNullOrEmpty(plancode))
                {
                    MachineResult = JsonConvert.DeserializeObject<List<Machine>>(_machineAPIRepository.GetMachineByMachineGroup(_factoryCode, "8", _token)).ToList();
                    //var MachineResulttop = MachineResult.FirstOrDefault();
                    var MachineResulttop = MachineResult.FirstOrDefault();
                    InsertAutoRouting(MachineResulttop, tmp);

                    seqNum = seqNum + 1;
                    // tmp.Machine = MachineResulttop.Machine1;
                }
                else
                {
                    Machine MachineResult2 = new Machine();
                    MachineResult2 = JsonConvert.DeserializeObject<Machine>(_machineAPIRepository.GetMachineGroupByPlanCode(_factoryCode, plancode, _token));
                    //tmp.Machine = MachineResult2.Machine1;
                    InsertAutoRouting(MachineResult2, tmp);

                    seqNum = seqNum + 1;
                }
            }
            else
            {
                //1. find Corr
                MachineResult = JsonConvert.DeserializeObject<List<Machine>>(_machineAPIRepository.GetMachineByMachineGroup(_factoryCode, "1", _token)).ToList();
                var MachineResulttop = MachineResult.FirstOrDefault();
                //tmp.Machine = MachineResulttop.Machine1;
                RoutingDataModel CalculateRoutingData = new RoutingDataModel();
                CalculateRoutingData = CalculateRouting(MachineResulttop.Machine1, model, Convert.ToInt32(model.modelRouting.SheetWidthIn));

                // Corrugator Properties
                model.modelRouting.PaperRollWidth = CalculateRoutingData.PaperRollWidth;
                model.modelRouting.Cut = CalculateRoutingData.Cut;
                model.modelRouting.Trim = CalculateRoutingData.Trim;
                model.modelRouting.PercentTrim = CalculateRoutingData.PercentTrim;
                model.modelRouting.TearTape = CalculateRoutingData.TearTape ?? false;
                model.modelRouting.LineQtyPerBox = CalculateRoutingData.LineQtyPerBox;
                model.modelRouting.MarginForPaper = CalculateRoutingData.MarginForPaper;

                //if (MachineResult.Count == 2)
                //{
                //    model.modelRouting.Alternative1 = MachineResult[1].Machine1;
                //}
                //if (MachineResult.Count == 3)
                //{
                //    model.modelRouting.Alternative1 = MachineResult[1].Machine1;
                //    model.modelRouting.Alternative2 = MachineResult[2].Machine1;

                //}
                //if (MachineResult.Count == 4)
                //{
                //    model.modelRouting.Alternative1 = MachineResult[1].Machine1;
                //    model.modelRouting.Alternative2 = MachineResult[2].Machine1;
                //    model.modelRouting.Alternative3 = MachineResult[3].Machine1;
                //}

                //tmp.PaperRollWidth = CalculateRoutingData.PaperRollWidth;
                //tmp.Trim =
                InsertAutoRouting(MachineResulttop, tmp);

                seqNum = seqNum + 1;
            }

            //MappingModelAutoRouting(TransactionDataModel model,RoutingDataModel routing, int seqNum);

            MappingModelAutoRouting(model, tmp, seqNum);

            model.modelRouting.RoutingDataList.Add(tmp);

            model.modelRouting.PaperRollWidth = "0";
            model.modelRouting.Cut = "0";
            model.modelRouting.Trim = "0";
            model.modelRouting.PercentTrim = "0";
            model.modelRouting.TearTape = false;
            model.modelRouting.LineQtyPerBox = 0;
            model.modelRouting.MarginForPaper = "0";
            var JoinType = model.modelProductProp.JoinType;
            // routing.Add(tmp);

            //routing.Add(MachineResult.FirstOrDefault());
            //2. find Flexo
            //2.1 find ProductType

            var flexoData = JsonConvert.DeserializeObject<List<ProductType>>(_productTypeAPIRepository.GetFormGroupByHierarchyLv2List(_factoryCode, HieLV2, model.modelCategories.FormGroup, _token));
            //2.2 loop flexo
            model.modelRouting.Alternative1 = null;
            model.modelRouting.Alternative2 = null;
            model.modelRouting.Alternative3 = null;
            model.modelRouting.Alternative4 = null;
            model.modelRouting.Alternative5 = null;
            model.modelRouting.Alternative6 = null;
            model.modelRouting.Alternative7 = null;
            model.modelRouting.Alternative8 = null;

            List<Machine> mm = new List<Machine>();

            if (!string.IsNullOrEmpty(flexoData[0].FlexoOrderBy))
            {
                string[] words = flexoData[0].FlexoOrderBy.Split(',');
                foreach (var floxotype in words)
                {
                    MachineResult = JsonConvert.DeserializeObject<List<Machine>>(_machineAPIRepository.GetMachineHierarchy(_factoryCode, HieLV2, model.MaterialNo, floxotype, JoinType, _token)).ToList();
                    if (MachineResult.Count > 0)
                    {
                        RoutingDataModel tmp2 = new RoutingDataModel();
                        List<Machine> tmpM = new List<Machine>();
                        //เช็คสี
                        if (model.modelProductProp.AmountColor > 0)
                        {
                            MachineResult = MachineResult.Where(m => m.Colour >= model.modelProductProp.AmountColor).ToList();
                        }
                        //เช็ค กาว
                        if (JoinType.Contains("กาว"))
                        {
                            tmpM = MachineResult.Where(m => m.GlueType == JoinType).ToList();
                            if (tmpM.Count > 0)
                            {
                                tmp2.Machine = tmpM[0].Machine1;
                            }
                            else
                            {
                                tmp2.Machine = MachineResult[0].Machine1;
                            }

                            #region Altermachine auto

                            //if (tmpM.Count == 2)
                            //{
                            //    model.modelRouting.Alternative1 = MachineResult[1].Machine1;
                            //}
                            //if (tmpM.Count == 3)
                            //{
                            //    model.modelRouting.Alternative1 = MachineResult[1].Machine1;
                            //    model.modelRouting.Alternative2 = MachineResult[2].Machine1;

                            //}
                            //if (tmpM.Count == 4)
                            //{
                            //    model.modelRouting.Alternative1 = MachineResult[1].Machine1;
                            //    model.modelRouting.Alternative2 = MachineResult[2].Machine1;
                            //    model.modelRouting.Alternative3 = MachineResult[3].Machine1;
                            //}
                            //if (tmpM.Count == 5)
                            //{
                            //    model.modelRouting.Alternative1 = MachineResult[1].Machine1;
                            //    model.modelRouting.Alternative2 = MachineResult[2].Machine1;
                            //    model.modelRouting.Alternative3 = MachineResult[3].Machine1;
                            //    model.modelRouting.Alternative4 = MachineResult[4].Machine1;

                            //}
                            //if (tmpM.Count == 6)
                            //{
                            //    model.modelRouting.Alternative1 = MachineResult[1].Machine1;
                            //    model.modelRouting.Alternative2 = MachineResult[2].Machine1;
                            //    model.modelRouting.Alternative3 = MachineResult[3].Machine1;
                            //    model.modelRouting.Alternative4 = MachineResult[4].Machine1;
                            //    model.modelRouting.Alternative5 = MachineResult[5].Machine1;
                            //}
                            //if (tmpM.Count == 7)
                            //{
                            //    model.modelRouting.Alternative1 = MachineResult[1].Machine1;
                            //    model.modelRouting.Alternative2 = MachineResult[2].Machine1;
                            //    model.modelRouting.Alternative3 = MachineResult[3].Machine1;
                            //    model.modelRouting.Alternative4 = MachineResult[4].Machine1;
                            //    model.modelRouting.Alternative5 = MachineResult[5].Machine1;
                            //    model.modelRouting.Alternative6 = MachineResult[6].Machine1;

                            //}
                            //if (tmpM.Count == 8)
                            //{
                            //    model.modelRouting.Alternative1 = MachineResult[1].Machine1;
                            //    model.modelRouting.Alternative2 = MachineResult[2].Machine1;
                            //    model.modelRouting.Alternative3 = MachineResult[3].Machine1;
                            //    model.modelRouting.Alternative4 = MachineResult[4].Machine1;
                            //    model.modelRouting.Alternative5 = MachineResult[5].Machine1;
                            //    model.modelRouting.Alternative6 = MachineResult[6].Machine1;
                            //    model.modelRouting.Alternative7 = MachineResult[7].Machine1;

                            //}
                            //if (tmpM.Count == 9)
                            //{
                            //    model.modelRouting.Alternative1 = MachineResult[1].Machine1;
                            //    model.modelRouting.Alternative2 = MachineResult[2].Machine1;
                            //    model.modelRouting.Alternative3 = MachineResult[3].Machine1;
                            //    model.modelRouting.Alternative4 = MachineResult[4].Machine1;
                            //    model.modelRouting.Alternative5 = MachineResult[5].Machine1;
                            //    model.modelRouting.Alternative6 = MachineResult[6].Machine1;
                            //    model.modelRouting.Alternative7 = MachineResult[7].Machine1;
                            //    model.modelRouting.Alternative8 = MachineResult[8].Machine1;

                            //}

                            #endregion Altermachine auto
                        }
                        else
                        {
                            tmp2.Machine = MachineResult[0].Machine1;

                            #region Alternachine auto

                            //if (MachineResult.Count == 2)
                            //                        {
                            //                            model.modelRouting.Alternative1 = MachineResult[1].Machine1;
                            //                        }
                            //                        if (MachineResult.Count == 3)
                            //                        {
                            //                            model.modelRouting.Alternative1 = MachineResult[1].Machine1;
                            //                            model.modelRouting.Alternative2 = MachineResult[2].Machine1;

                            //                        }
                            //                        if (MachineResult.Count == 4)
                            //                        {
                            //                            model.modelRouting.Alternative1 = MachineResult[1].Machine1;
                            //                            model.modelRouting.Alternative2 = MachineResult[2].Machine1;
                            //                            model.modelRouting.Alternative3 = MachineResult[3].Machine1;
                            //                        }
                            //                        if (MachineResult.Count == 5)
                            //                        {
                            //                            model.modelRouting.Alternative1 = MachineResult[1].Machine1;
                            //                            model.modelRouting.Alternative2 = MachineResult[2].Machine1;
                            //                            model.modelRouting.Alternative3 = MachineResult[3].Machine1;
                            //                            model.modelRouting.Alternative4 = MachineResult[4].Machine1;

                            //                        }
                            //                        if (MachineResult.Count == 6)
                            //                        {
                            //                            model.modelRouting.Alternative1 = MachineResult[1].Machine1;
                            //                            model.modelRouting.Alternative2 = MachineResult[2].Machine1;
                            //                            model.modelRouting.Alternative3 = MachineResult[3].Machine1;
                            //                            model.modelRouting.Alternative4 = MachineResult[4].Machine1;
                            //                            model.modelRouting.Alternative5 = MachineResult[5].Machine1;
                            //                        }
                            //                        if (MachineResult.Count == 7)
                            //                        {
                            //                            model.modelRouting.Alternative1 = MachineResult[1].Machine1;
                            //                            model.modelRouting.Alternative2 = MachineResult[2].Machine1;
                            //                            model.modelRouting.Alternative3 = MachineResult[3].Machine1;
                            //                            model.modelRouting.Alternative4 = MachineResult[4].Machine1;
                            //                            model.modelRouting.Alternative5 = MachineResult[5].Machine1;
                            //                            model.modelRouting.Alternative6 = MachineResult[6].Machine1;

                            //                        }
                            //                        if (MachineResult.Count == 8)
                            //                        {
                            //                            model.modelRouting.Alternative1 = MachineResult[1].Machine1;
                            //                            model.modelRouting.Alternative2 = MachineResult[2].Machine1;
                            //                            model.modelRouting.Alternative3 = MachineResult[3].Machine1;
                            //                            model.modelRouting.Alternative4 = MachineResult[4].Machine1;
                            //                            model.modelRouting.Alternative5 = MachineResult[5].Machine1;
                            //                            model.modelRouting.Alternative6 = MachineResult[6].Machine1;
                            //                            model.modelRouting.Alternative7 = MachineResult[7].Machine1;

                            //                        }
                            //                        if (MachineResult.Count == 9)
                            //                        {
                            //                            model.modelRouting.Alternative1 = MachineResult[1].Machine1;
                            //                            model.modelRouting.Alternative2 = MachineResult[2].Machine1;
                            //                            model.modelRouting.Alternative3 = MachineResult[3].Machine1;
                            //                            model.modelRouting.Alternative4 = MachineResult[4].Machine1;
                            //                            model.modelRouting.Alternative5 = MachineResult[5].Machine1;
                            //                            model.modelRouting.Alternative6 = MachineResult[6].Machine1;
                            //                            model.modelRouting.Alternative7 = MachineResult[7].Machine1;
                            //                            model.modelRouting.Alternative8 = MachineResult[8].Machine1;

                            //                        }

                            #endregion Alternachine auto
                        }

                        // tmp2.Machine = MachineResult[0].Machine1;

                        //routing.Add(tmp2);
                        seqNum = seqNum + 1;
                        MappingModelAutoRouting(model, tmp2, seqNum);

                        model.modelRouting.RoutingDataList.Add(tmp2);
                        break; // get out of the loop
                    }
                }
            }
            //MachineResult = JsonConvert.DeserializeObject<List<Machine>>(_machineAPIRepository.GetMachineHierarchy(_factoryCode, HieLV2, model.MaterialNo, _token)).ToList();
            //var MachineResulttop2 = MachineResult.FirstOrDefault();
            //var MachineResulttop23 = MachineResult;

            //3. find DC

            if (!string.IsNullOrEmpty(flexoData[0].Dc))
            {
                model.modelRouting.Alternative1 = null;
                model.modelRouting.Alternative2 = null;
                model.modelRouting.Alternative3 = null;
                model.modelRouting.Alternative4 = null;
                model.modelRouting.Alternative5 = null;
                model.modelRouting.Alternative6 = null;
                model.modelRouting.Alternative7 = null;
                model.modelRouting.Alternative8 = null;
                MachineResult = JsonConvert.DeserializeObject<List<Machine>>(_machineAPIRepository.GetMachineHierarchy(_factoryCode, HieLV2, model.MaterialNo, flexoData[0].Dc, JoinType, _token)).ToList();
                if (MachineResult.Count > 0)
                {
                    RoutingDataModel tmp3 = new RoutingDataModel();
                    tmp3.Machine = MachineResult[0].Machine1;
                    seqNum = seqNum + 1;
                    //routing.Add(tmp2);

                    #region Altermachine auto

                    //if (MachineResult.Count == 2)
                    //                    {
                    //                        model.modelRouting.Alternative1 = MachineResult[1].Machine1;
                    //                    }
                    //                    if (MachineResult.Count == 3)
                    //                    {
                    //                        model.modelRouting.Alternative1 = MachineResult[1].Machine1;
                    //                        model.modelRouting.Alternative2 = MachineResult[2].Machine1;

                    //                    }
                    //                    if (MachineResult.Count == 4)
                    //                    {
                    //                        model.modelRouting.Alternative1 = MachineResult[1].Machine1;
                    //                        model.modelRouting.Alternative2 = MachineResult[2].Machine1;
                    //                        model.modelRouting.Alternative3 = MachineResult[3].Machine1;
                    //                    }
                    //                    if (MachineResult.Count == 5)
                    //                    {
                    //                        model.modelRouting.Alternative1 = MachineResult[1].Machine1;
                    //                        model.modelRouting.Alternative2 = MachineResult[2].Machine1;
                    //                        model.modelRouting.Alternative3 = MachineResult[3].Machine1;
                    //                        model.modelRouting.Alternative4 = MachineResult[4].Machine1;

                    //                    }
                    //                    if (MachineResult.Count == 6)
                    //                    {
                    //                        model.modelRouting.Alternative1 = MachineResult[1].Machine1;
                    //                        model.modelRouting.Alternative2 = MachineResult[2].Machine1;
                    //                        model.modelRouting.Alternative3 = MachineResult[3].Machine1;
                    //                        model.modelRouting.Alternative4 = MachineResult[4].Machine1;
                    //                        model.modelRouting.Alternative5 = MachineResult[5].Machine1;
                    //                    }
                    //                    if (MachineResult.Count == 7)
                    //                    {
                    //                        model.modelRouting.Alternative1 = MachineResult[1].Machine1;
                    //                        model.modelRouting.Alternative2 = MachineResult[2].Machine1;
                    //                        model.modelRouting.Alternative3 = MachineResult[3].Machine1;
                    //                        model.modelRouting.Alternative4 = MachineResult[4].Machine1;
                    //                        model.modelRouting.Alternative5 = MachineResult[5].Machine1;
                    //                        model.modelRouting.Alternative6 = MachineResult[6].Machine1;

                    //                    }
                    //                    if (MachineResult.Count == 8)
                    //                    {
                    //                        model.modelRouting.Alternative1 = MachineResult[1].Machine1;
                    //                        model.modelRouting.Alternative2 = MachineResult[2].Machine1;
                    //                        model.modelRouting.Alternative3 = MachineResult[3].Machine1;
                    //                        model.modelRouting.Alternative4 = MachineResult[4].Machine1;
                    //                        model.modelRouting.Alternative5 = MachineResult[5].Machine1;
                    //                        model.modelRouting.Alternative6 = MachineResult[6].Machine1;
                    //                        model.modelRouting.Alternative7 = MachineResult[7].Machine1;

                    //                    }
                    //                    if (MachineResult.Count == 9)
                    //                    {
                    //                        model.modelRouting.Alternative1 = MachineResult[1].Machine1;
                    //                        model.modelRouting.Alternative2 = MachineResult[2].Machine1;
                    //                        model.modelRouting.Alternative3 = MachineResult[3].Machine1;
                    //                        model.modelRouting.Alternative4 = MachineResult[4].Machine1;
                    //                        model.modelRouting.Alternative5 = MachineResult[5].Machine1;
                    //                        model.modelRouting.Alternative6 = MachineResult[6].Machine1;
                    //                        model.modelRouting.Alternative7 = MachineResult[7].Machine1;
                    //                        model.modelRouting.Alternative8 = MachineResult[8].Machine1;

                    //                    }

                    #endregion Altermachine auto

                    MappingModelAutoRouting(model, tmp3, seqNum);

                    model.modelRouting.RoutingDataList.Add(tmp3);
                }
            }
            //4. check stitch

            if (JoinType.Contains("ตอก"))
            {
                model.modelRouting.Alternative1 = null;
                model.modelRouting.Alternative2 = null;
                model.modelRouting.Alternative3 = null;
                model.modelRouting.Alternative4 = null;
                model.modelRouting.Alternative5 = null;
                model.modelRouting.Alternative6 = null;
                model.modelRouting.Alternative7 = null;
                model.modelRouting.Alternative8 = null;
                MachineResult = JsonConvert.DeserializeObject<List<Machine>>(_machineAPIRepository.GetMachineByMachineGroup(_factoryCode, JoinType, _token)).ToList();
                //var MachineResulttop = MachineResult.FirstOrDefault();
                //var MachineResulttop = MachineResult.FirstOrDefault();
                RoutingDataModel tmp4 = new RoutingDataModel();
                tmp4.Machine = MachineResult[0].Machine1;
                seqNum = seqNum + 1;
                //routing.Add(tmp2);

                #region Altermachine auto

                //if (MachineResult.Count == 2)
                //           {
                //               model.modelRouting.Alternative1 = MachineResult[1].Machine1;
                //           }
                //           if (MachineResult.Count == 3)
                //           {
                //               model.modelRouting.Alternative1 = MachineResult[1].Machine1;
                //               model.modelRouting.Alternative2 = MachineResult[2].Machine1;

                //           }
                //           if (MachineResult.Count == 4)
                //           {
                //               model.modelRouting.Alternative1 = MachineResult[1].Machine1;
                //               model.modelRouting.Alternative2 = MachineResult[2].Machine1;
                //               model.modelRouting.Alternative3 = MachineResult[3].Machine1;
                //           }
                //           if (MachineResult.Count == 5)
                //           {
                //               model.modelRouting.Alternative1 = MachineResult[1].Machine1;
                //               model.modelRouting.Alternative2 = MachineResult[2].Machine1;
                //               model.modelRouting.Alternative3 = MachineResult[3].Machine1;
                //               model.modelRouting.Alternative4 = MachineResult[4].Machine1;

                //           }
                //           if (MachineResult.Count == 6)
                //           {
                //               model.modelRouting.Alternative1 = MachineResult[1].Machine1;
                //               model.modelRouting.Alternative2 = MachineResult[2].Machine1;
                //               model.modelRouting.Alternative3 = MachineResult[3].Machine1;
                //               model.modelRouting.Alternative4 = MachineResult[4].Machine1;
                //               model.modelRouting.Alternative5 = MachineResult[5].Machine1;
                //           }
                //           if (MachineResult.Count == 7)
                //           {
                //               model.modelRouting.Alternative1 = MachineResult[1].Machine1;
                //               model.modelRouting.Alternative2 = MachineResult[2].Machine1;
                //               model.modelRouting.Alternative3 = MachineResult[3].Machine1;
                //               model.modelRouting.Alternative4 = MachineResult[4].Machine1;
                //               model.modelRouting.Alternative5 = MachineResult[5].Machine1;
                //               model.modelRouting.Alternative6 = MachineResult[6].Machine1;

                //           }
                //           if (MachineResult.Count == 8)
                //           {
                //               model.modelRouting.Alternative1 = MachineResult[1].Machine1;
                //               model.modelRouting.Alternative2 = MachineResult[2].Machine1;
                //               model.modelRouting.Alternative3 = MachineResult[3].Machine1;
                //               model.modelRouting.Alternative4 = MachineResult[4].Machine1;
                //               model.modelRouting.Alternative5 = MachineResult[5].Machine1;
                //               model.modelRouting.Alternative6 = MachineResult[6].Machine1;
                //               model.modelRouting.Alternative7 = MachineResult[7].Machine1;

                //           }
                //           if (MachineResult.Count == 9)
                //           {
                //               model.modelRouting.Alternative1 = MachineResult[1].Machine1;
                //               model.modelRouting.Alternative2 = MachineResult[2].Machine1;
                //               model.modelRouting.Alternative3 = MachineResult[3].Machine1;
                //               model.modelRouting.Alternative4 = MachineResult[4].Machine1;
                //               model.modelRouting.Alternative5 = MachineResult[5].Machine1;
                //               model.modelRouting.Alternative6 = MachineResult[6].Machine1;
                //               model.modelRouting.Alternative7 = MachineResult[7].Machine1;
                //               model.modelRouting.Alternative8 = MachineResult[8].Machine1;

                //           }

                #endregion Altermachine auto

                MappingModelAutoRouting(model, tmp4, seqNum);

                model.modelRouting.RoutingDataList.Add(tmp4);
            }
            //else
            if (JoinType.Contains("กาว") && JoinType != "กาวเครื่อง")
            {
                //5. Glue
                MachineResult = JsonConvert.DeserializeObject<List<Machine>>(_machineAPIRepository.GetMachineByMachineGroup(_factoryCode, JoinType, _token)).ToList();
                //var MachineResulttop = MachineResult.FirstOrDefault();
                //var MachineResulttop = MachineResult.FirstOrDefault();
                if (MachineResult.Count > 0)
                {
                    RoutingDataModel tmp4 = new RoutingDataModel();
                    tmp4.Machine = MachineResult[0].Machine1;
                    seqNum = seqNum + 1;
                    //routing.Add(tmp2);
                    MappingModelAutoRouting(model, tmp4, seqNum);

                    model.modelRouting.RoutingDataList.Add(tmp4);
                }
            }

            //9. coa
            if (model.modelProductCustomer.COA == true)
            {
                model.modelRouting.Alternative1 = null;
                model.modelRouting.Alternative2 = null;
                model.modelRouting.Alternative3 = null;
                model.modelRouting.Alternative4 = null;
                model.modelRouting.Alternative5 = null;
                model.modelRouting.Alternative6 = null;
                model.modelRouting.Alternative7 = null;
                model.modelRouting.Alternative8 = null;
                MachineResult = JsonConvert.DeserializeObject<List<Machine>>(_machineAPIRepository.GetMachineByMachineGroup(_factoryCode, "5", _token)).ToList();
                //var MachineResulttop = MachineResult.FirstOrDefault();
                //var MachineResulttop = MachineResult.FirstOrDefault();
                RoutingDataModel tmp5 = new RoutingDataModel();
                tmp5.Machine = MachineResult[0].Machine1;
                seqNum = seqNum + 1;
                //routing.Add(tmp2);

                #region Altermachine auto

                //if (MachineResult.Count == 2)
                //                {
                //                    model.modelRouting.Alternative1 = MachineResult[1].Machine1;
                //                }
                //                if (MachineResult.Count == 3)
                //                {
                //                    model.modelRouting.Alternative1 = MachineResult[1].Machine1;
                //                    model.modelRouting.Alternative2 = MachineResult[2].Machine1;

                //                }
                //                if (MachineResult.Count == 4)
                //                {
                //                    model.modelRouting.Alternative1 = MachineResult[1].Machine1;
                //                    model.modelRouting.Alternative2 = MachineResult[2].Machine1;
                //                    model.modelRouting.Alternative3 = MachineResult[3].Machine1;
                //                }
                //                if (MachineResult.Count == 5)
                //                {
                //                    model.modelRouting.Alternative1 = MachineResult[1].Machine1;
                //                    model.modelRouting.Alternative2 = MachineResult[2].Machine1;
                //                    model.modelRouting.Alternative3 = MachineResult[3].Machine1;
                //                    model.modelRouting.Alternative4 = MachineResult[4].Machine1;

                //                }
                //                if (MachineResult.Count == 6)
                //                {
                //                    model.modelRouting.Alternative1 = MachineResult[1].Machine1;
                //                    model.modelRouting.Alternative2 = MachineResult[2].Machine1;
                //                    model.modelRouting.Alternative3 = MachineResult[3].Machine1;
                //                    model.modelRouting.Alternative4 = MachineResult[4].Machine1;
                //                    model.modelRouting.Alternative5 = MachineResult[5].Machine1;
                //                }
                //                if (MachineResult.Count == 7)
                //                {
                //                    model.modelRouting.Alternative1 = MachineResult[1].Machine1;
                //                    model.modelRouting.Alternative2 = MachineResult[2].Machine1;
                //                    model.modelRouting.Alternative3 = MachineResult[3].Machine1;
                //                    model.modelRouting.Alternative4 = MachineResult[4].Machine1;
                //                    model.modelRouting.Alternative5 = MachineResult[5].Machine1;
                //                    model.modelRouting.Alternative6 = MachineResult[6].Machine1;

                //                }
                //                if (MachineResult.Count == 8)
                //                {
                //                    model.modelRouting.Alternative1 = MachineResult[1].Machine1;
                //                    model.modelRouting.Alternative2 = MachineResult[2].Machine1;
                //                    model.modelRouting.Alternative3 = MachineResult[3].Machine1;
                //                    model.modelRouting.Alternative4 = MachineResult[4].Machine1;
                //                    model.modelRouting.Alternative5 = MachineResult[5].Machine1;
                //                    model.modelRouting.Alternative6 = MachineResult[6].Machine1;
                //                    model.modelRouting.Alternative7 = MachineResult[7].Machine1;

                //                }
                //                if (MachineResult.Count == 9)
                //                {
                //                    model.modelRouting.Alternative1 = MachineResult[1].Machine1;
                //                    model.modelRouting.Alternative2 = MachineResult[2].Machine1;
                //                    model.modelRouting.Alternative3 = MachineResult[3].Machine1;
                //                    model.modelRouting.Alternative4 = MachineResult[4].Machine1;
                //                    model.modelRouting.Alternative5 = MachineResult[5].Machine1;
                //                    model.modelRouting.Alternative6 = MachineResult[6].Machine1;
                //                    model.modelRouting.Alternative7 = MachineResult[7].Machine1;
                //                    model.modelRouting.Alternative8 = MachineResult[8].Machine1;

                //                }

                #endregion Altermachine auto

                MappingModelAutoRouting(model, tmp5, seqNum);

                model.modelRouting.RoutingDataList.Add(tmp5);
                //tmp.Machine = MachineResulttop.Machine1;
                //routing.Add(tmp);
            }
            //9. ฟิล์ม
            if (model.modelProductCustomer.Film == true)
            {
                var film = "ฟิล์ม";
                MachineResult = JsonConvert.DeserializeObject<List<Machine>>(_machineAPIRepository.GetMachineByMachineGroup(_factoryCode, film, _token)).ToList();
                //var MachineResulttop = MachineResult.FirstOrDefault();
                var MachineResulttop = MachineResult.FirstOrDefault();
                if (MachineResult.Count != 0)
                {
                    seqNum = seqNum + 1;
                    tmp.Machine = MachineResulttop.Machine1;
                    routing.Add(tmp);
                }
            }

            // คลัง

            model.modelRouting.Alternative1 = null;
            model.modelRouting.Alternative2 = null;
            model.modelRouting.Alternative3 = null;
            model.modelRouting.Alternative4 = null;
            model.modelRouting.Alternative5 = null;
            model.modelRouting.Alternative6 = null;
            model.modelRouting.Alternative7 = null;
            model.modelRouting.Alternative8 = null;
            MachineResult = JsonConvert.DeserializeObject<List<Machine>>(_machineAPIRepository.GetMachineByMachineGroup(_factoryCode, "9", _token)).ToList();
            //var MachineResulttop = MachineResult.FirstOrDefault();
            //var MachineResulttop = MachineResult.FirstOrDefault();
            RoutingDataModel tmp6 = new RoutingDataModel();
            tmp6.Machine = MachineResult[0].Machine1;
            seqNum = seqNum + 1;
            //routing.Add(tmp2);

            #region Altermachine auto

            //if (MachineResult.Count == 2)
            //          {
            //              model.modelRouting.Alternative1 = MachineResult[1].Machine1;
            //          }
            //          if (MachineResult.Count == 3)
            //          {
            //              model.modelRouting.Alternative1 = MachineResult[1].Machine1;
            //              model.modelRouting.Alternative2 = MachineResult[2].Machine1;

            //          }
            //          if (MachineResult.Count == 4)
            //          {
            //              model.modelRouting.Alternative1 = MachineResult[1].Machine1;
            //              model.modelRouting.Alternative2 = MachineResult[2].Machine1;
            //              model.modelRouting.Alternative3 = MachineResult[3].Machine1;
            //          }
            //          if (MachineResult.Count == 5)
            //          {
            //              model.modelRouting.Alternative1 = MachineResult[1].Machine1;
            //              model.modelRouting.Alternative2 = MachineResult[2].Machine1;
            //              model.modelRouting.Alternative3 = MachineResult[3].Machine1;
            //              model.modelRouting.Alternative4 = MachineResult[4].Machine1;

            //          }
            //          if (MachineResult.Count == 6)
            //          {
            //              model.modelRouting.Alternative1 = MachineResult[1].Machine1;
            //              model.modelRouting.Alternative2 = MachineResult[2].Machine1;
            //              model.modelRouting.Alternative3 = MachineResult[3].Machine1;
            //              model.modelRouting.Alternative4 = MachineResult[4].Machine1;
            //              model.modelRouting.Alternative5 = MachineResult[5].Machine1;
            //          }
            //          if (MachineResult.Count == 7)
            //          {
            //              model.modelRouting.Alternative1 = MachineResult[1].Machine1;
            //              model.modelRouting.Alternative2 = MachineResult[2].Machine1;
            //              model.modelRouting.Alternative3 = MachineResult[3].Machine1;
            //              model.modelRouting.Alternative4 = MachineResult[4].Machine1;
            //              model.modelRouting.Alternative5 = MachineResult[5].Machine1;
            //              model.modelRouting.Alternative6 = MachineResult[6].Machine1;

            //          }
            //          if (MachineResult.Count == 8)
            //          {
            //              model.modelRouting.Alternative1 = MachineResult[1].Machine1;
            //              model.modelRouting.Alternative2 = MachineResult[2].Machine1;
            //              model.modelRouting.Alternative3 = MachineResult[3].Machine1;
            //              model.modelRouting.Alternative4 = MachineResult[4].Machine1;
            //              model.modelRouting.Alternative5 = MachineResult[5].Machine1;
            //              model.modelRouting.Alternative6 = MachineResult[6].Machine1;
            //              model.modelRouting.Alternative7 = MachineResult[7].Machine1;

            //          }
            //          if (MachineResult.Count == 9)
            //          {
            //              model.modelRouting.Alternative1 = MachineResult[1].Machine1;
            //              model.modelRouting.Alternative2 = MachineResult[2].Machine1;
            //              model.modelRouting.Alternative3 = MachineResult[3].Machine1;
            //              model.modelRouting.Alternative4 = MachineResult[4].Machine1;
            //              model.modelRouting.Alternative5 = MachineResult[5].Machine1;
            //              model.modelRouting.Alternative6 = MachineResult[6].Machine1;
            //              model.modelRouting.Alternative7 = MachineResult[7].Machine1;
            //              model.modelRouting.Alternative8 = MachineResult[8].Machine1;

            //          }

            #endregion Altermachine auto

            MappingModelAutoRouting(model, tmp6, seqNum);

            model.modelRouting.RoutingDataList.Add(tmp6);
            //tmp.Machine = MachineResultWH.Machine1;
            //routing.Add(tmp);

            SaveRouting(model);

            return routing;
        }

        public void InsertAutoRouting(Machine machine, RoutingDataModel tmp)
        {
            List<RoutingDataModel> routing = new List<RoutingDataModel>();

            //RoutingDataModel tmp = new RoutingDataModel();
            tmp.Machine = machine.Machine1;

            //routing.Add(tmp);
        }

        public void MappingModelAutoRouting(TransactionDataModel transactionDataModel, RoutingDataModel routingModel, int seqNum)
        {
            string[] arrayDiecut = null;
            if (transactionDataModel.arrayDiecut != null)
            {
                arrayDiecut = JsonConvert.DeserializeObject<string[]>(transactionDataModel.arrayDiecut);
            }
            string[] arrayPrint = null;
            if (transactionDataModel.arrayPrint != null)
            {
                arrayPrint = JsonConvert.DeserializeObject<string[]>(transactionDataModel.arrayPrint);
            }

            //seqNum = sessionModel.modelRouting.RoutingDataList.Count + 1;

            //RoutingDataModel routingModel = new RoutingDataModel();
            routingModel.SeqNo = seqNum;
            routingModel.GroupMachine = transactionDataModel.modelRouting.GroupMachine;
            //routingModel.Machine = transactionDataModel.modelRouting.Machine;
            routingModel.MachineMove = transactionDataModel.modelRouting.MachineMove;
            routingModel.Alternative1 = transactionDataModel.modelRouting.Alternative1;
            routingModel.Alternative2 = transactionDataModel.modelRouting.Alternative2;
            routingModel.Alternative3 = transactionDataModel.modelRouting.Alternative3;
            routingModel.Alternative4 = transactionDataModel.modelRouting.Alternative4;
            routingModel.Alternative5 = transactionDataModel.modelRouting.Alternative5;
            routingModel.Alternative6 = transactionDataModel.modelRouting.Alternative6;
            routingModel.Alternative7 = transactionDataModel.modelRouting.Alternative7;
            routingModel.Alternative8 = transactionDataModel.modelRouting.Alternative8;
            routingModel.NoOpenIn = transactionDataModel.modelRouting.NoOpenIn == null ? "1" : transactionDataModel.modelRouting.NoOpenIn;
            routingModel.NoOpenOut = transactionDataModel.modelRouting.NoOpenOut == null ? "1" : transactionDataModel.modelRouting.NoOpenOut;
            routingModel.WeightIn = transactionDataModel.modelRouting.WeightIn == null ? transactionDataModel.modelProductSpec.WeightSh.ToString() : transactionDataModel.modelRouting.WeightIn;
            routingModel.WeightOut = transactionDataModel.modelRouting.WeightOut == null ? transactionDataModel.modelProductSpec.WeightSh.ToString() : transactionDataModel.modelRouting.WeightOut;
            routingModel.SheetLengthIn = transactionDataModel.modelRouting.SheetLengthIn == null ? transactionDataModel.modelProductSpec.CutSheetLeng.ToString() : transactionDataModel.modelRouting.SheetLengthIn;
            routingModel.SheetLengthOut = transactionDataModel.modelRouting.SheetLengthOut == null ? transactionDataModel.modelProductSpec.CutSheetLeng.ToString() : transactionDataModel.modelRouting.SheetLengthOut;
            routingModel.SheetWidthIn = transactionDataModel.modelRouting.SheetWidthIn == null ? transactionDataModel.modelProductSpec.CutSheetWid.ToString() : transactionDataModel.modelRouting.SheetWidthIn;
            routingModel.SheetWidthOut = transactionDataModel.modelRouting.SheetWidthOut == null ? transactionDataModel.modelProductSpec.CutSheetWid.ToString() : transactionDataModel.modelRouting.SheetWidthOut;
            routingModel.Coat = transactionDataModel.modelRouting.Coat;
            routingModel.PaperRollWidth = transactionDataModel.modelRouting.PaperRollWidth;
            routingModel.Cut = transactionDataModel.modelRouting.Cut;
            routingModel.Trim = transactionDataModel.modelRouting.Trim;
            routingModel.PercentTrim = transactionDataModel.modelRouting.PercentTrim;
            routingModel.TearTape = transactionDataModel.modelRouting.TearTape;
            routingModel.LineQtyPerBox = transactionDataModel.modelRouting.LineQtyPerBox;
            routingModel.MarginForPaper = transactionDataModel.modelRouting.MarginForPaper;
            routingModel.Ink1 = transactionDataModel.modelRouting.Ink1 == null || transactionDataModel.modelRouting.Ink1.Trim() == "--- Choose here ---" ? null : transactionDataModel.modelRouting.Ink1;
            routingModel.Ink2 = transactionDataModel.modelRouting.Ink2 == null || transactionDataModel.modelRouting.Ink2.Trim() == "--- Choose here ---" ? null : transactionDataModel.modelRouting.Ink2;
            routingModel.Ink3 = transactionDataModel.modelRouting.Ink3 == null || transactionDataModel.modelRouting.Ink3.Trim() == "--- Choose here ---" ? null : transactionDataModel.modelRouting.Ink3;
            routingModel.Ink4 = transactionDataModel.modelRouting.Ink4 == null || transactionDataModel.modelRouting.Ink4.Trim() == "--- Choose here ---" ? null : transactionDataModel.modelRouting.Ink4;
            routingModel.Ink5 = transactionDataModel.modelRouting.Ink5 == null || transactionDataModel.modelRouting.Ink5.Trim() == "--- Choose here ---" ? null : transactionDataModel.modelRouting.Ink5;
            routingModel.Ink6 = transactionDataModel.modelRouting.Ink6 == null || transactionDataModel.modelRouting.Ink6.Trim() == "--- Choose here ---" ? null : transactionDataModel.modelRouting.Ink6;
            routingModel.Ink7 = transactionDataModel.modelRouting.Ink7 == null || transactionDataModel.modelRouting.Ink7.Trim() == "--- Choose here ---" ? null : transactionDataModel.modelRouting.Ink7;
            routingModel.Ink8 = transactionDataModel.modelRouting.Ink8 == null || transactionDataModel.modelRouting.Ink8.Trim() == "--- Choose here ---" ? null : transactionDataModel.modelRouting.Ink8;
            routingModel.Shade1 = transactionDataModel.modelRouting.Shade1 == null || transactionDataModel.modelRouting.Shade1.Trim() == "--- Choose here ---" ? null : transactionDataModel.modelRouting.Shade1;
            routingModel.Shade2 = transactionDataModel.modelRouting.Shade2 == null || transactionDataModel.modelRouting.Shade2.Trim() == "--- Choose here ---" ? null : transactionDataModel.modelRouting.Shade2;
            routingModel.Shade3 = transactionDataModel.modelRouting.Shade3 == null || transactionDataModel.modelRouting.Shade3.Trim() == "--- Choose here ---" ? null : transactionDataModel.modelRouting.Shade3;
            routingModel.Shade4 = transactionDataModel.modelRouting.Shade4 == null || transactionDataModel.modelRouting.Shade4.Trim() == "--- Choose here ---" ? null : transactionDataModel.modelRouting.Shade4;
            routingModel.Shade5 = transactionDataModel.modelRouting.Shade5 == null || transactionDataModel.modelRouting.Shade5.Trim() == "--- Choose here ---" ? null : transactionDataModel.modelRouting.Shade5;
            routingModel.Shade6 = transactionDataModel.modelRouting.Shade6 == null || transactionDataModel.modelRouting.Shade6.Trim() == "--- Choose here ---" ? null : transactionDataModel.modelRouting.Shade6;
            routingModel.Shade7 = transactionDataModel.modelRouting.Shade7 == null || transactionDataModel.modelRouting.Shade7.Trim() == "--- Choose here ---" ? null : transactionDataModel.modelRouting.Shade7;
            routingModel.Shade8 = transactionDataModel.modelRouting.Shade8 == null || transactionDataModel.modelRouting.Shade8.Trim() == "--- Choose here ---" ? null : transactionDataModel.modelRouting.Shade8;
            routingModel.Area1 = transactionDataModel.modelRouting.Area1;
            routingModel.Area2 = transactionDataModel.modelRouting.Area2;
            routingModel.Area3 = transactionDataModel.modelRouting.Area3;
            routingModel.Area4 = transactionDataModel.modelRouting.Area4;
            routingModel.Area5 = transactionDataModel.modelRouting.Area5;
            routingModel.Area6 = transactionDataModel.modelRouting.Area6;
            routingModel.Area7 = transactionDataModel.modelRouting.Area7;
            routingModel.Area8 = transactionDataModel.modelRouting.Area8;
            routingModel.PrintingPlateNo = transactionDataModel.modelRouting.PrintingPlateNo;
            routingModel.CuttingDieNo = transactionDataModel.modelRouting.CuttingDieNo;
            routingModel.RepeatLength = transactionDataModel.modelRouting.RepeatLength;
            routingModel.CustBarcodeNo = transactionDataModel.modelRouting.CustBarcodeNo;

            if (transactionDataModel.modelRouting.MylaNo != null)
            {
                routingModel.MylaNo = transactionDataModel.modelRouting.MylaNo;
            }
            else
            {
                routingModel.MylaNo = transactionDataModel.modelRouting.MylaNo_Copy;
            }

            routingModel.PrintingPlateType = transactionDataModel.modelRouting.PrintingPlateType;
            routingModel.JoinToMaterialNo = transactionDataModel.modelRouting.JoinToMaterialNo;
            routingModel.SperateToMaterialNo = transactionDataModel.modelRouting.SperateToMaterialNo;
            routingModel.Remark = transactionDataModel.modelRouting.Remark;
            routingModel.MachineGroupSelect = transactionDataModel.modelRouting.MachineGroupSelect;

            routingModel.Plant = transactionDataModel.FactoryCode;
            //routingModel.Plant_Code = "Machine";
            //routingModel.StdProcess = true;
            routingModel.HandHold = false;
            // routingModel.Platen =
            //routingModel.Rotary
            //routingModel.Hardship
            routingModel.UnUpgrad_Board = false;
            //routingModel.Color_count =
            //routingModel.Human =

            routingModel.Speed = transactionDataModel.modelRouting.Speed;
            routingModel.SetupTm = transactionDataModel.modelRouting.SetupTm;
            routingModel.PrepareTm = transactionDataModel.modelRouting.PrepareTm;
            routingModel.PostTm = transactionDataModel.modelRouting.PostTm;
            routingModel.SetupWaste = transactionDataModel.modelRouting.SetupWaste;
            routingModel.RunWaste = transactionDataModel.modelRouting.RunWaste;
            routingModel.StackHeight = transactionDataModel.modelRouting.StackHeight;
            routingModel.RotateIn = transactionDataModel.modelRouting.RotateIn;
            routingModel.RotateOut = transactionDataModel.modelRouting.RotateOut;

            routingModel.TearTapeQty = transactionDataModel.modelRouting.LineQtyPerBox; //  string.IsNullOrEmpty(transactionDataModel.modelRouting.LineQtyPerBox) ? 0 : Convert.ToInt16(transactionDataModel.modelRouting.LineQtyPerBox); ;
            routingModel.TearTapeDistance = transactionDataModel.modelRouting.MarginForPaper;

            routingModel.ScoreGap = transactionDataModel.modelRouting.ScoreGap;
            routingModel.ScoreType = transactionDataModel.modelRouting.ScoreType;

            routingModel.MylaSize = transactionDataModel.modelRouting.MylaSize;

            switch (transactionDataModel.modelRouting.PrintingPlateType)
            {
                case "Non-Print":
                    routingModel.NoneBlk = true;
                    routingModel.StanBlk = false;
                    routingModel.SemiBlk = false;
                    routingModel.ShipBlk = false;
                    break;

                case "Standard":
                    routingModel.NoneBlk = false;
                    routingModel.StanBlk = true;
                    routingModel.SemiBlk = false;
                    routingModel.ShipBlk = false;
                    break;

                case "Semi":
                    routingModel.NoneBlk = false;
                    routingModel.StanBlk = false;
                    routingModel.SemiBlk = true;
                    routingModel.ShipBlk = false;
                    break;

                case "Shipping Mark":
                    routingModel.NoneBlk = false;
                    routingModel.StanBlk = false;
                    routingModel.SemiBlk = false;
                    routingModel.ShipBlk = true;
                    break;
            }

            switch (transactionDataModel.modelRouting.Rotate)
            {
                case "noRotate":
                    routingModel.RotateIn = false;
                    routingModel.RotateOut = false;
                    break;

                case "rotateIn":
                    routingModel.RotateIn = true;
                    routingModel.RotateOut = false;
                    break;

                case "rotateOut":
                    routingModel.RotateIn = false;
                    routingModel.RotateOut = true;
                    break;
            }

            if (arrayPrint != null)
            {
                switch (arrayPrint.Length)
                {
                    case 2:
                        routingModel.PlateNo2 = arrayPrint[0];
                        routingModel.PlateNoPlant2 = arrayPrint[1];
                        routingModel.PlateNo3 = null;
                        routingModel.PlateNoPlant3 = null;
                        routingModel.PlateNo4 = null;
                        routingModel.PlateNoPlant4 = null;
                        routingModel.PlateNo5 = null;
                        routingModel.PlateNoPlant5 = null;
                        break;

                    case 4:
                        routingModel.PlateNo2 = arrayPrint[0];
                        routingModel.PlateNoPlant2 = arrayPrint[1];
                        routingModel.PlateNo3 = arrayPrint[2];
                        routingModel.PlateNoPlant3 = arrayPrint[3];
                        routingModel.PlateNo4 = null;
                        routingModel.PlateNoPlant4 = null;
                        routingModel.PlateNo5 = null;
                        routingModel.PlateNoPlant5 = null;
                        break;

                    case 6:
                        routingModel.PlateNo2 = arrayPrint[0];
                        routingModel.PlateNoPlant2 = arrayPrint[1];
                        routingModel.PlateNo3 = arrayPrint[2];
                        routingModel.PlateNoPlant3 = arrayPrint[3];
                        routingModel.PlateNo4 = arrayPrint[4];
                        routingModel.PlateNoPlant4 = arrayPrint[5];
                        routingModel.PlateNo5 = null;
                        routingModel.PlateNoPlant5 = null;
                        break;

                    case 8:
                        routingModel.PlateNo2 = arrayPrint[0];
                        routingModel.PlateNoPlant2 = arrayPrint[1];
                        routingModel.PlateNo3 = arrayPrint[2];
                        routingModel.PlateNoPlant3 = arrayPrint[3];
                        routingModel.PlateNo4 = arrayPrint[4];
                        routingModel.PlateNoPlant4 = arrayPrint[5];
                        routingModel.PlateNo5 = arrayPrint[6];
                        routingModel.PlateNoPlant5 = arrayPrint[7];
                        break;

                    default:
                        break;
                }
            }

            if (arrayDiecut != null)
            {
                switch (arrayDiecut.Length)
                {
                    case 4:
                        routingModel.BlockNo2 = arrayDiecut[0];
                        routingModel.BlockNoPlant2 = arrayDiecut[1];
                        routingModel.MylaNo2 = arrayDiecut[2];
                        routingModel.MylaNoPlant2 = arrayDiecut[3];
                        routingModel.BlockNo3 = null;
                        routingModel.BlockNoPlant3 = null;
                        routingModel.MylaNo3 = null;
                        routingModel.MylaNoPlant3 = null;
                        routingModel.BlockNo4 = null;
                        routingModel.BlockNoPlant4 = null;
                        routingModel.MylaNo4 = null;
                        routingModel.MylaNoPlant4 = null;
                        routingModel.BlockNo5 = null;
                        routingModel.BlockNoPlant5 = null;
                        routingModel.MylaNo5 = null;
                        routingModel.MylaNoPlant5 = null;
                        break;

                    case 8:
                        routingModel.BlockNo2 = arrayDiecut[0];
                        routingModel.BlockNoPlant2 = arrayDiecut[1];
                        routingModel.MylaNo2 = arrayDiecut[2];
                        routingModel.MylaNoPlant2 = arrayDiecut[3];
                        routingModel.BlockNo3 = arrayDiecut[4];
                        routingModel.BlockNoPlant3 = arrayDiecut[5];
                        routingModel.MylaNo3 = arrayDiecut[6];
                        routingModel.MylaNoPlant3 = arrayDiecut[7];
                        routingModel.BlockNo4 = null;
                        routingModel.BlockNoPlant4 = null;
                        routingModel.MylaNo4 = null;
                        routingModel.MylaNoPlant4 = null;
                        routingModel.BlockNo5 = null;
                        routingModel.BlockNoPlant5 = null;
                        routingModel.MylaNo5 = null;
                        routingModel.MylaNoPlant5 = null;
                        break;

                    case 12:
                        routingModel.BlockNo2 = arrayDiecut[0];
                        routingModel.BlockNoPlant2 = arrayDiecut[1];
                        routingModel.MylaNo2 = arrayDiecut[2];
                        routingModel.MylaNoPlant2 = arrayDiecut[3];
                        routingModel.BlockNo3 = arrayDiecut[4];
                        routingModel.BlockNoPlant3 = arrayDiecut[5];
                        routingModel.MylaNo3 = arrayDiecut[6];
                        routingModel.MylaNoPlant3 = arrayDiecut[7];
                        routingModel.BlockNo4 = arrayDiecut[8];
                        routingModel.BlockNoPlant4 = arrayDiecut[9];
                        routingModel.MylaNo4 = arrayDiecut[10];
                        routingModel.MylaNoPlant4 = arrayDiecut[11];
                        routingModel.BlockNo5 = null;
                        routingModel.BlockNoPlant5 = null;
                        routingModel.MylaNo5 = null;
                        routingModel.MylaNoPlant5 = null;
                        break;

                    case 16:
                        routingModel.BlockNo2 = arrayDiecut[0];
                        routingModel.BlockNoPlant2 = arrayDiecut[1];
                        routingModel.MylaNo2 = arrayDiecut[2];
                        routingModel.MylaNoPlant2 = arrayDiecut[3];
                        routingModel.BlockNo3 = arrayDiecut[4];
                        routingModel.BlockNoPlant3 = arrayDiecut[5];
                        routingModel.MylaNo3 = arrayDiecut[6];
                        routingModel.MylaNoPlant3 = arrayDiecut[7];
                        routingModel.BlockNo4 = arrayDiecut[8];
                        routingModel.BlockNoPlant4 = arrayDiecut[9];
                        routingModel.MylaNo4 = arrayDiecut[10];
                        routingModel.MylaNoPlant4 = arrayDiecut[11];
                        routingModel.BlockNo5 = arrayDiecut[12];
                        routingModel.BlockNoPlant5 = arrayDiecut[13];
                        routingModel.MylaNo5 = arrayDiecut[14];
                        routingModel.MylaNoPlant5 = arrayDiecut[15];
                        break;

                    default:
                        break;
                }
            }

            routingModel.Totalcolor = transactionDataModel.modelRouting.Totalcolor;

            // tmp = routingModel;
        }
    }
}