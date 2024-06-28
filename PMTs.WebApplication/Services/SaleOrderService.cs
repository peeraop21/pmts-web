using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.NewProduct;
using PMTs.DataAccess.ModelView.Report;
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
    public class SaleOrderService : ISaleOrderService
    {
        UserSessionModel userSessionModel;

        private readonly string _username;
        private readonly string _saleOrg;
        private readonly string _factoryCode;
        private readonly string _token;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMoSpecAPIRepository _moSpecAPIRepository;
        private readonly IBuildRemarkAPIRepository _buildRemarkAPIRepository;
        private readonly IColorAPIRepository _colorAPIRepository;
        private readonly IMachineGroupAPIRepository _machineGroupAPIRepository;
        private readonly IScoreTypeAPIRepository _scoreTypeAPIRepository;
        private readonly IMoRoutingAPIRepository _moRoutingAPIRepository;
        private readonly ITransactionsDetailAPIRepository _transactionsDetailAPIRepository;
        private readonly IProductTypeAPIRepository _productTypeAPIRepository;
        private readonly IFormulaAPIRepository _formulaAPIRepository;
        private readonly IQualitySpecAPIRepository _qualitySpecAPIRepository;
        private readonly IMachineAPIRepository _machineAPIRepository;
        private readonly IMoDataAPIRepository _moDataAPIRepository;

        public SaleOrderService(IHttpContextAccessor httpContextAccessor
            , IMoSpecAPIRepository moSpecAPIRepository
            , IBuildRemarkAPIRepository buildRemarkAPIRepository
            , IColorAPIRepository colorAPIRepository
            , IMachineGroupAPIRepository machineGroupAPIRepository
            , IScoreTypeAPIRepository scoreTypeAPIRepository
            , IMoRoutingAPIRepository moRoutingAPIRepository
            , ITransactionsDetailAPIRepository transactionsDetailAPIRepository
            , IProductTypeAPIRepository productTypeAPIRepository
            , IFormulaAPIRepository formulaAPIRepository
            , IQualitySpecAPIRepository qualitySpecAPIRepository
            , IMachineAPIRepository machineAPIRepository
            , IMoDataAPIRepository moDataAPIRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _moSpecAPIRepository = moSpecAPIRepository;
            _buildRemarkAPIRepository = buildRemarkAPIRepository;
            _colorAPIRepository = colorAPIRepository;
            _machineGroupAPIRepository = machineGroupAPIRepository;
            _scoreTypeAPIRepository = scoreTypeAPIRepository;
            _moRoutingAPIRepository = moRoutingAPIRepository;
            _transactionsDetailAPIRepository = transactionsDetailAPIRepository;
            _productTypeAPIRepository = productTypeAPIRepository;
            _formulaAPIRepository = formulaAPIRepository;
            _qualitySpecAPIRepository = qualitySpecAPIRepository;
            _machineAPIRepository = machineAPIRepository;
            _moDataAPIRepository = moDataAPIRepository;

            userSessionModel = SessionExtentions.GetSession<UserSessionModel>(_httpContextAccessor.HttpContext.Session, "UserSessionModel");
            if (userSessionModel != null)
            {
                _username = userSessionModel.UserName;
                _saleOrg = userSessionModel.SaleOrg;
                _factoryCode = userSessionModel.FactoryCode;
                _token = userSessionModel.Token;
            }
        }


        public SaleOrderModel GetMoSpecByOrderItem(string orderitem)
        {
            SaleOrderModel model = new SaleOrderModel();
            model.moSpec = JsonConvert.DeserializeObject<MoSpec>(_moSpecAPIRepository.GetMoSpecBySaleOrder(_factoryCode, orderitem, _token));
            return model;
        }

        public SaleOrderModel BindMoRouting(string orderitem)
        {
            SaleOrderModel model = new SaleOrderModel();
            model.moSpec = JsonConvert.DeserializeObject<MoSpec>(_moSpecAPIRepository.GetMoSpecBySaleOrder(_factoryCode, orderitem, _token));

            if (model.moSpec != null)
            {
                model.modelRouting.WeightSheetDefault = Math.Round(Convert.ToDecimal(model.moSpec.WeightSh), 3).ToString();
                model.modelRouting.SheetLengthIn = model.moSpec.CutSheetLeng.ToString();
                model.modelRouting.SheetLengthOut = model.moSpec.CutSheetLeng.ToString();
                model.modelRouting.SheetWidthIn = model.moSpec.CutSheetWid.ToString();
                model.modelRouting.SheetWidthOut = model.moSpec.CutSheetWid.ToString();

                var trans = JsonConvert.DeserializeObject<TransactionsDetail>(_transactionsDetailAPIRepository.GetTransactionsDetailByMat(_factoryCode, model.moSpec.MaterialNo, _token));
                var prodType = JsonConvert.DeserializeObject<ProductType>(_productTypeAPIRepository.GetProductTypeByBoxType(model.moSpec.BoxType, _token));
                if (trans == null)
                {
                    model.modelRouting.WeightSelectList = new SelectList(new List<SelectListItem> {
                    new SelectListItem { Selected = false, Text = Math.Round(Convert.ToDecimal(model.moSpec.WeightSh), 3).ToString(), Value = Math.Round(Convert.ToDecimal(model.moSpec.WeightSh), 3).ToString() },
                    new SelectListItem { Selected = false, Text = Math.Round(Convert.ToDecimal(model.moSpec.WeightBox), 3).ToString(), Value = Math.Round(Convert.ToDecimal(model.moSpec.WeightBox), 3).ToString() },
                    }, "Value", "Text", 1);

                    model.GLTail = "";
                    model.MaterialNo = model.moSpec.MaterialNo;
                    model.Slit = model.moSpec.Slit.ToString();
                    model.Flute = model.moSpec.Flute;
                    model.amountColor = 0;
                    model.MatCode = model.moSpec.MaterialType;
                    model.RealEventFlag = "";
                    model.EventFlag = "Create";
                    model.BoxHandle = model.moSpec.BoxType.Contains("มี") ? "True" : "False";
                    model.PLANTCODE = "";
                    model.PlantOs = "";
                    if (prodType == null)
                    {
                        model.FormGroup = "";
                    }
                    else
                    {
                        model.FormGroup = prodType.FormGroup;
                    }
                }
                else
                {
                    if (trans.IdSaleUnit == 6)
                    {
                        var sum = model.moSpec.WeightBox * model.moSpec.PieceSet;
                        model.modelRouting.WeightSelectList = new SelectList(new List<SelectListItem> {
                    new SelectListItem { Selected = false, Text = Math.Round(Convert.ToDecimal(model.moSpec.WeightSh), 3).ToString(), Value = Math.Round(Convert.ToDecimal(model.moSpec.WeightSh), 3).ToString() },
                    new SelectListItem { Selected = false, Text = Math.Round(Convert.ToDecimal(model.moSpec.WeightBox), 3).ToString(), Value = Math.Round(Convert.ToDecimal(model.moSpec.WeightBox), 3).ToString() },
                    new SelectListItem { Selected = false, Text = Math.Round(Convert.ToDecimal(sum), 3).ToString(), Value = Math.Round(Convert.ToDecimal(sum), 3).ToString() },
                    }, "Value", "Text", 1);
                    }
                    else
                    {
                        model.modelRouting.WeightSelectList = new SelectList(new List<SelectListItem> {
                    new SelectListItem { Selected = false, Text = Math.Round(Convert.ToDecimal(model.moSpec.WeightSh), 3).ToString(), Value = Math.Round(Convert.ToDecimal(model.moSpec.WeightSh), 3).ToString() },
                    new SelectListItem { Selected = false, Text = Math.Round(Convert.ToDecimal(model.moSpec.WeightBox), 3).ToString(), Value = Math.Round(Convert.ToDecimal(model.moSpec.WeightBox), 3).ToString() },
                    }, "Value", "Text", 1);
                    }


                    model.GLTail = trans.Gltail.ToString();
                    model.MaterialNo = model.moSpec.MaterialNo;
                    model.Slit = model.moSpec.Slit.ToString();
                    model.Flute = model.moSpec.Flute;
                    model.amountColor = 0;
                    model.MatCode = model.moSpec.MaterialType;
                    model.RealEventFlag = "";
                    model.EventFlag = "Create";
                    model.BoxHandle = model.moSpec.BoxType.Contains("มี") ? "True" : "False";
                    model.PLANTCODE = "";
                    model.PlantOs = "";
                    if (prodType == null)
                    {
                        model.FormGroup = "";
                    }
                    else
                    {
                        model.FormGroup = prodType.FormGroup;
                    }
                }
            }

            model.modelRouting.RoutingDataList = new List<RoutingDataModel>();
            model.moRoutings = JsonConvert.DeserializeObject<List<MoRouting>>(_moRoutingAPIRepository.GetMORoutingsBySaleOrder(_factoryCode, orderitem, _token));
            var ModelByMapper = MapperProductSpectToRoutingDatalist(model.moRoutings);
            model.modelRouting.RoutingDataList = ModelByMapper.modelRouting.RoutingDataList;
            if (model.modelRouting.RoutingDataList != null)
            {
                model.amountColor = 0;
                foreach (var it in model.modelRouting.RoutingDataList)
                {
                    if (model.amountColor < it.Color_count)
                    {
                        model.amountColor = it.Color_count;
                    }
                }
            }


            model.modelRouting.MachineGroupSelectList = JsonConvert.DeserializeObject<List<GroupMachineModels>>(_machineGroupAPIRepository.GetByMachineGroupJoinMachine(_factoryCode, _token)).Select(sli => new SelectListItem { Value = sli.Id, Text = sli.GroupMachine });
            model.modelRouting.InkSelectList = JsonConvert.DeserializeObject<List<Color>>(_colorAPIRepository.GetColorList(_factoryCode, _token)).OrderBy(o => o.Ink).Select(s => s.Ink).Distinct().Select(sli => new SelectListItem { Value = sli, Text = sli });
            model.modelRouting.ShadeSelectList = JsonConvert.DeserializeObject<List<Color>>(_colorAPIRepository.GetColorList(_factoryCode, _token)).OrderBy(o => o.Shade).Select(s => s.Shade).Distinct().Select(sli => new SelectListItem { Value = sli, Text = sli });

            model.modelRouting.ScoreTypelist = JsonConvert.DeserializeObject<List<ScoreType>>(_scoreTypeAPIRepository.GetScoreTypeList(_factoryCode, _token)).Select(s => new { s.ScoreTypeId, s.ScoreTypeName }).Distinct().Select(sli => new SelectListItem { Value = sli.ScoreTypeId.ToString(), Text = sli.ScoreTypeName });
            model.modelRouting.ScoreGapList = null; // = JsonConvert.DeserializeObject<List<ScoreGap>>(_scoreGapAPIRepository.GetScoreGapList(_factoryCode)).Select(s => s.ScoreGap1).Distinct().Select(sli => new SelectListItem { Value = sli.ToString(), Text = sli.ToString() });

            model.modelBuildRemark = JsonConvert.DeserializeObject<List<BuildRemark>>(_buildRemarkAPIRepository.GetBuildRemarkList(_factoryCode, _token));
            model.modelBuildRemark = model.modelBuildRemark.OrderBy(x => x.List).ToList();
            model.modelGroupMachineRemark = new List<string>();
            var listtemp = model.modelBuildRemark.GroupBy(x => new { x.Machine }).ToList();
            foreach (var item in listtemp)
            {
                model.modelGroupMachineRemark.Add(item.Key.Machine);
            }
            model.modelGroupMachineRemark = model.modelGroupMachineRemark.OrderBy(x => x).ToList();
            // model.amountColor = (Int16)(string.IsNullOrEmpty(modelSession.modelProductProp.AmountColor.ToString()) ? 0 : modelSession.modelProductProp.AmountColor);


            return model;
        }

        public SaleOrderModel GetMoRoutingByOrderItem(string OrderItem)
        {
            SaleOrderModel model = new SaleOrderModel();
            model.modelRouting.RoutingDataList = new List<RoutingDataModel>();
            model.moRoutings = JsonConvert.DeserializeObject<List<MoRouting>>(_moRoutingAPIRepository.GetMORoutingsBySaleOrder(_factoryCode, OrderItem, _token));
            var ModelByMapper = MapperProductSpectToRoutingDatalist(model.moRoutings);
            model.modelRouting.RoutingDataList = ModelByMapper.modelRouting.RoutingDataList;
            return model;
        }

        public SaleOrderModel GetBuildRemark()
        {
            SaleOrderModel model = new SaleOrderModel();
            model.modelBuildRemark = JsonConvert.DeserializeObject<List<BuildRemark>>(_buildRemarkAPIRepository.GetBuildRemarkList(_factoryCode, _token));
            model.modelBuildRemark = model.modelBuildRemark.OrderBy(x => x.List).ToList();
            return model;
        }

        public List<string> GetWeight(string orderitem)
        {
            SaleOrderModel model = new SaleOrderModel();
            List<string> weightList = new List<string>();
            model.moSpec = JsonConvert.DeserializeObject<MoSpec>(_moSpecAPIRepository.GetMoSpecBySaleOrder(_factoryCode, orderitem, _token));
            if (model.moSpec != null)
            {
                var trans = JsonConvert.DeserializeObject<TransactionsDetail>(_transactionsDetailAPIRepository.GetTransactionsDetailByMat(_factoryCode, model.moSpec.MaterialNo, _token));
                if (trans == null)
                {
                    weightList.Add(Math.Round(Convert.ToDecimal(model.moSpec.WeightSh), 3).ToString());
                    weightList.Add(Math.Round(Convert.ToDecimal(model.moSpec.WeightBox), 3).ToString());
                }
                else
                {
                    if (trans.IdSaleUnit == 6)
                    {
                        var sum = model.moSpec.WeightBox * model.moSpec.PieceSet;
                        weightList.Add(Math.Round(Convert.ToDecimal(model.moSpec.WeightSh), 3).ToString());
                        weightList.Add(Math.Round(Convert.ToDecimal(model.moSpec.WeightBox), 3).ToString());
                        weightList.Add(Math.Round(Convert.ToDecimal(sum), 3).ToString());
                    }
                    else
                    {
                        weightList.Add(Math.Round(Convert.ToDecimal(model.moSpec.WeightSh), 3).ToString());
                        weightList.Add(Math.Round(Convert.ToDecimal(model.moSpec.WeightBox), 3).ToString());
                    }
                }
            }
            else
            {
                var trans = JsonConvert.DeserializeObject<TransactionsDetail>(_transactionsDetailAPIRepository.GetTransactionsDetailByMat(_factoryCode, model.moSpec.MaterialNo, _token));
                if (trans == null)
                {
                    weightList.Add(Math.Round(Convert.ToDecimal(model.moSpec.WeightSh), 3).ToString());
                    weightList.Add(Math.Round(Convert.ToDecimal(model.moSpec.WeightBox), 3).ToString());
                }
                else
                {
                    if (trans.IdSaleUnit == 6)
                    {
                        var sum = model.moSpec.WeightBox * model.moSpec.PieceSet;
                        weightList.Add(Math.Round(Convert.ToDecimal(model.moSpec.WeightSh), 3).ToString());
                        weightList.Add(Math.Round(Convert.ToDecimal(model.moSpec.WeightBox), 3).ToString());
                        weightList.Add(Math.Round(Convert.ToDecimal(sum), 3).ToString());
                    }
                    else
                    {
                        weightList.Add(Math.Round(Convert.ToDecimal(model.moSpec.WeightSh), 3).ToString());
                        weightList.Add(Math.Round(Convert.ToDecimal(model.moSpec.WeightBox), 3).ToString());
                    }
                }
            }
            return weightList;
        }

        public RoutingDataModel CalculateRouting(string machineName, string OrderItem)
        {
            SaleOrderModel model = new SaleOrderModel();
            model.moSpec = JsonConvert.DeserializeObject<MoSpec>(_moSpecAPIRepository.GetMoSpecBySaleOrder(_factoryCode, OrderItem, _token));
            RoutingDataModel ret = new RoutingDataModel();
            try
            {
                ret = JsonConvert.DeserializeObject<RoutingDataModel>(_formulaAPIRepository.CalculateRouting(_factoryCode, machineName, model.moSpec.Flute, model.moSpec.CutSheetWid.ToString(), model.moSpec.MaterialNo, _token));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return ret;
        }

        public List<QualitySpec> GetQualitySpecsByMaterial(string Material)
        {
            return JsonConvert.DeserializeObject<List<QualitySpec>>(_qualitySpecAPIRepository.GetQualitySpecByMaterialNo(_factoryCode, Material, _token));
        }

        public string GetMachineGroupByMachine(string machine)
        {
            Machine _machine = new Machine();
            _machine = JsonConvert.DeserializeObject<Machine>(_machineAPIRepository.GetMachineGroupByMachine(_factoryCode, machine, _token));
            return _machine == null ? "" : _machine.MachineGroup.ToString();
        }
        public List<Machine> GetMachineDataByFactorycode(string factorycode)
        {
            List<Machine> _machine = new List<Machine>();
            _machine = JsonConvert.DeserializeObject<List<Machine>>(_machineAPIRepository.GetMachineList(factorycode, _token));
            return _machine;
        }


        #region [Function Help]
        private int ConvertStringToShort(string Input)
        {
            return string.IsNullOrEmpty(Input) ? 0 : Convert.ToInt32(Input);
        }

        private Int16 ConvertInt16ToShort(int? Input)
        {
            return (Int16)(string.IsNullOrEmpty(Input.ToString()) ? 0 : Input);
        }

        private string GetMachinePlantCode(string machine)
        {
            Machine _machine = new Machine();
            _machine = JsonConvert.DeserializeObject<Machine>(_machineAPIRepository.GetMachineGroupByMachine(_factoryCode, machine, _token));
            return _machine == null ? "" : _machine.PlanCode.ToString();
        }

        private string GetMachineCodeByMachine(string machine)
        {
            Machine _machine = new Machine();
            _machine = JsonConvert.DeserializeObject<Machine>(_machineAPIRepository.GetMachineGroupByMachine(_factoryCode, machine, _token));
            return _machine == null ? "" : _machine.Code.ToString();
        }

        private SaleOrderModel MapperProductSpectToRoutingDatalist(List<MoRouting> modelMoRouting)
        {
            SaleOrderModel model = new SaleOrderModel();
            List<RoutingDataModel> routinngModel = new List<RoutingDataModel>();
            foreach (var item in modelMoRouting)
            {
                RoutingDataModel tmp = new RoutingDataModel();
                tmp.id = item.Id;
                tmp.SeqNo = item.SeqNo;
                tmp.GroupMachine = item.MatCode;
                tmp.Machine = item.Machine;
                tmp.MachineMove = item.McMove ?? false;
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
                tmp.WeightIn = item.WeightIn.ToString();
                tmp.WeightOut = item.WeightOut.ToString();
                tmp.SheetLengthIn = item.SheetInLeg.ToString();
                tmp.SheetWidthIn = item.SheetInWid.ToString(); ;
                tmp.SheetLengthOut = item.SheetOutLeg.ToString();
                tmp.SheetWidthOut = item.SheetOutWid.ToString();
                // tmp.Coat = item.;
                tmp.PaperRollWidth = item.PaperWidth.ToString();
                tmp.Cut = item.CutNo.ToString();
                tmp.Trim = item.Trim.ToString();
                tmp.PercentTrim = item.PercenTrim.ToString();
                // tmp.LineQtyPerBox = item.;
                //  tmp.MarginForPaper = item.m;
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
                //tmp.c
                tmp.CuttingDieNo = item.PlateNo;
                tmp.MylaNo = item.MylaNo;
                // tmp.MylaNo_Copy = item.my;
                //tmp.PrintingPlateType = item.pla;
                tmp.JoinToMaterialNo = item.JoinMatNo;
                tmp.SperateToMaterialNo = item.SeparatMatNo;
                tmp.Remark = item.RemarkInprocess;
                //tmp.RemarkImageFile = item.;
                // tmp.RemarkImageFileName = item.;
                // tmp.RemarkAttachFileStatus = item.;
                // tmp.MachineGroupSelect = item.;
                // tmp.CopyStatus = item.cop;
                // tmp.IsPropCor = item.isp;
                //tmp.IsPropPrint = item.is;
                //tmp.IsPropDieCut = item.is;
                tmp.NoneBlk = item.NoneBlk ?? false;
                tmp.StanBlk = item.StanBlk ?? false;
                tmp.SemiBlk = item.SemiBlk ?? false;
                tmp.ShipBlk = item.ShipBlk ?? false;

                if (tmp.NoneBlk)
                {
                    tmp.PrintingPlateType = "Non-Print";
                }
                else if (tmp.StanBlk)
                {
                    tmp.PrintingPlateType = "Standard";
                }
                else if (tmp.SemiBlk)
                {
                    tmp.PrintingPlateType = "Semi";
                }
                else if (tmp.ShipBlk)
                {
                    tmp.PrintingPlateType = "Shipping Mark";
                }
                else
                {
                    tmp.PrintingPlateType = "";
                }

                //tmp.BlockNo2 = item.;
                //tmp.BlockNo3 = item.;
                //tmp.BlockNo4 = item.;
                //tmp.BlockNo5 = item.;
                //tmp.BlockNoPlant2 = item.;
                //tmp.BlockNoPlant3 = item.;
                //tmp.BlockNoPlant4 = item.;
                //tmp.BlockNoPlant5 = item.;
                //tmp.MylaNo2 = item.;
                //tmp.MylaNo3 = item.;
                //tmp.MylaNo4 = item.;
                //tmp.MylaNo5 = item.;
                //tmp.MylaNo5 = item.;
                //tmp.MylaNoPlant2 = item.;
                //tmp.MylaNoPlant3 = item.;
                //tmp.MylaNoPlant4 = item.;
                //tmp.MylaNoPlant5 = item.;
                //tmp.PlateNo2 = item.;
                //tmp.PlateNo3 = item.;
                //tmp.PlateNo4 = item.;
                //tmp.PlateNo5 = item.;
                //tmp.PlateNoPlant2 = item.;
                //tmp.PlateNoPlant3 = item.;
                //tmp.PlateNoPlant4 = item.;
                //tmp.PlateNoPlant5 = item.;
                tmp.Plant = item.Plant;
                tmp.Plant_Code = item.PlanCode;
                tmp.HandHold = item.HandHold ?? false;
                tmp.Hardship = item.Hardship ?? 0;
                tmp.Platen = item.Platen ?? false;
                tmp.Rotary = item.Rotary ?? false;
                tmp.UnUpgrad_Board = item.UnUpgradBoard ?? false;
                tmp.Color_count = item.ColourCount ?? 0;
                tmp.StdProcess = item.StdProcess ?? false;
                tmp.Human = item.Human ?? 0;
                tmp.TearTape = item.TearTape;
                //tmp.TearTapeDistance = item.t;
                tmp.Speed = item.Speed ?? 0;
                tmp.SetupTm = item.SetupTm ?? 0;
                tmp.PrepareTm = item.PrepareTm ?? 0;
                tmp.PostTm = item.PostTm ?? 0;
                tmp.SetupWaste = item.SetupWaste ?? 0;
                tmp.RunWaste = item.RunWaste ?? 0;
                tmp.StackHeight = item.StackHeight ?? 0;
                tmp.RotateIn = item.RotateIn;
                tmp.RotateOut = item.RotateOut;
                tmp.ScoreType = item.ScoreType;
                tmp.ScoreGap = item.ScoreGap ?? 0;
                //tmp.Rotate = item.r;
                //tmp.MylaSize = item.myla;
                tmp.RepeatLength = item.RepeatLength ?? 0;
                tmp.CustBarcodeNo = item.CustBarcodeNo;
                //tmp.Totalcolor = item.;
                tmp.WasteLeg = item.WasteLeg;
                tmp.WasteWid = item.WasteWid;
                routinngModel.Add(tmp);
            }
            model.modelRouting.RoutingDataList = routinngModel;
            return model;
        }

        public void MapperAddData(SaleOrderModel sessionModel, SaleOrderModel transactionDataModel, ref RoutingDataModel retuneTransactionModel, ref int seqNum)
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

            routingModel.Plant = _factoryCode;
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


            retuneTransactionModel = routingModel;
        }

        public RoutingDataModel MappingModelRoutingUpdateAndDelete(SaleOrderModel sessionModel, SaleOrderModel transactionDataModel)
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

            routingModel.Plant = _factoryCode;
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

            return routingModel;

        }

        public SaleOrderModel UpdateRouting(SaleOrderModel model, SaleOrderModel modelToUpdate, RoutingDataModel routingModel)
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

                i.Plant = _factoryCode;
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


            });

            UpdateRouting(model);
            model.modelRouting.RoutingDataList.Clear();
            var tmp = GetMoRoutingByOrderItem(model.OrderSelect);
            model.modelRouting.RoutingDataList = tmp.modelRouting.RoutingDataList;

            //if (modelToUpdate.modelRouting.RemarkAttachFileStatus == 0)
            //{
            //    model.modelRouting.RoutingDataList.First(i => i.SeqNo == modelToUpdate.modelRouting.SeqNo).RemarkImageFile = null;
            //    model.modelRouting.RoutingDataList.First(i => i.SeqNo == modelToUpdate.modelRouting.SeqNo).RemarkImageFileName = null;
            //}
            return model;
        }

        public SaleOrderModel InsertRouting(SaleOrderModel model, SaleOrderModel transactionDataModel, RoutingDataModel routingModel)
        {
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

                Plant = _factoryCode,
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
                Totalcolor = routingModel.Totalcolor


            });

            int seqNumber = 1;

            model.modelRouting.RoutingDataList.ForEach(i => { i.SeqNo = seqNumber; seqNumber++; });

            SaveMoRouting(model);
            model.modelRouting.RoutingDataList.Clear();
            var tmp = GetMoRoutingByOrderItem(model.OrderSelect);
            model.modelRouting.RoutingDataList = tmp.modelRouting.RoutingDataList;
            var moData = JsonConvert.DeserializeObject<MoData>(_moDataAPIRepository.GetMoDataBySaleOrderNonX(_factoryCode, model.OrderSelect, _token));
            if (moData != null)
            {
                moData.InterfaceTips = false;
                _moDataAPIRepository.UpdateMoData(JsonConvert.SerializeObject(moData), _token);
            }
            return model;
        }

        public SaleOrderModel CopyRouting(SaleOrderModel model, int seqNo)
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

            SaveMoRouting(model);
            model.modelRouting.RoutingDataList.Clear();
            var tmp = GetMoRoutingByOrderItem(model.OrderSelect);
            model.modelRouting.RoutingDataList = tmp.modelRouting.RoutingDataList;
            return model;
        }
        #endregion


        #region[Function ManageData]
        public void SaveMoRouting(SaleOrderModel modelToSave)
        {
            try
            {
                List<MoRouting> Routing_List = new List<MoRouting>();
                foreach (var i in modelToSave.modelRouting.RoutingDataList)
                {
                    MoRouting tmp = new MoRouting();
                    tmp.SeqNo = Convert.ToByte(i.SeqNo);
                    tmp.OrderItem = modelToSave.OrderSelect;
                    string plantcode = "";
                    tmp.FactoryCode = string.IsNullOrEmpty(i.Plant) ? _factoryCode : i.Plant;
                    tmp.Plant = string.IsNullOrEmpty(i.Plant) ? _factoryCode : i.Plant;
                    tmp.MaterialNo = modelToSave.MaterialNo;
                    tmp.MatCode = GetMachineCodeByMachine(i.Machine);
                    tmp.PlanCode = GetMachinePlantCode(i.Machine);
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
                    tmp.Color8 = i.Area8;
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
                    //tmp.PdisStatus = "N";
                    //tmp.BlockNo2 = i.BlockNo2;
                    //tmp.BlockNoPlant2 = i.BlockNoPlant2;
                    //tmp.BlockNo3 = i.BlockNo3;
                    //tmp.BlockNoPlant3 = i.BlockNoPlant3;
                    //tmp.BlockNo4 = i.BlockNo4;
                    //tmp.BlockNoPlant4 = i.BlockNoPlant4;
                    //tmp.BlockNo5 = i.BlockNo5;
                    //tmp.BlockNoPlant5 = i.BlockNoPlant5;
                    //tmp.PlateNo2 = i.PlateNo2;
                    //tmp.PlateNoPlant2 = i.PlateNoPlant2;
                    //tmp.MylaNo2 = i.MylaNo2;
                    //tmp.MylaNoPlant2 = i.MylaNoPlant2;
                    //tmp.PlateNo3 = i.PlateNo3;
                    //tmp.PlateNoPlant3 = i.PlateNoPlant3;
                    //tmp.MylaNo3 = i.MylaNo3;
                    //tmp.MylaNoPlant3 = i.MylaNoPlant3;
                    //tmp.PlateNo4 = i.PlateNo4;
                    //tmp.PlateNoPlant4 = i.PlateNoPlant4;
                    //tmp.MylaNo4 = i.MylaNo4;
                    //tmp.MylaNoPlant4 = i.MylaNoPlant4;
                    //tmp.PlateNo5 = i.PlateNo5;
                    //tmp.PlateNoPlant5 = i.PlateNoPlant5;
                    //tmp.MylaNo5 = i.MylaNo5;
                    //tmp.MylaNoPlant5 = i.MylaNoPlant5;


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

                    tmp.TearTape = i.TearTapeQty == 1 ? true : false;
                    //tmp.TearTapeDistance = i.TearTapeDistance;

                    tmp.Speed = i.Speed;
                    tmp.SetupTm = i.SetupTm;
                    tmp.PrepareTm = i.PrepareTm;
                    tmp.PostTm = i.PostTm;
                    tmp.SetupWaste = i.SetupWaste;
                    tmp.RunWaste = i.RunWaste;

                    tmp.StackHeight = i.StackHeight;
                    tmp.RotateIn = i.RotateIn;
                    tmp.RotateOut = i.RotateOut;

                    tmp.ScoreGap = Convert.ToInt16(i.ScoreGap);
                    tmp.ScoreType = i.ScoreType;

                    // tmp.MylaSize = i.MylaSize;
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
                    if (!string.IsNullOrEmpty(i.Ink8))
                    {
                        sumcolor = sumcolor + 1;
                    }
                    tmp.ColourCount = sumcolor;

                    Routing_List.Add(tmp);
                }
                _moRoutingAPIRepository.SaveMORoutingsBySaleOrder(_factoryCode, modelToSave.OrderSelect, JsonConvert.SerializeObject(Routing_List), _token);
                _moDataAPIRepository.UpdateMoDataSentKIWI(_factoryCode, modelToSave.OrderSelect, _username, _token);
            }
            catch (Exception ex)
            {
                throw;
            }


        }
        public void UpdateRouting(SaleOrderModel modelToSave)
        {
            try
            {
                List<MoRouting> Routing_List = new List<MoRouting>();
                foreach (var i in modelToSave.modelRouting.RoutingDataList)
                {
                    MoRouting tmp = new MoRouting();
                    tmp.SeqNo = Convert.ToByte(i.SeqNo);
                    tmp.OrderItem = modelToSave.OrderSelect;
                    string plantcode = "";
                    tmp.FactoryCode = string.IsNullOrEmpty(i.Plant) ? _factoryCode : i.Plant;
                    tmp.Plant = string.IsNullOrEmpty(i.Plant) ? _factoryCode : i.Plant;
                    tmp.MaterialNo = modelToSave.MaterialNo;
                    tmp.MatCode = GetMachineCodeByMachine(i.Machine);
                    tmp.PlanCode = GetMachinePlantCode(i.Machine);
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
                    //tmp.PdisStatus = "N";
                    //tmp.BlockNo2 = i.BlockNo2;
                    //tmp.BlockNoPlant2 = i.BlockNoPlant2;
                    //tmp.BlockNo3 = i.BlockNo3;
                    //tmp.BlockNoPlant3 = i.BlockNoPlant3;
                    //tmp.BlockNo4 = i.BlockNo4;
                    //tmp.BlockNoPlant4 = i.BlockNoPlant4;
                    //tmp.BlockNo5 = i.BlockNo5;
                    //tmp.BlockNoPlant5 = i.BlockNoPlant5;
                    //tmp.PlateNo2 = i.PlateNo2;
                    //tmp.PlateNoPlant2 = i.PlateNoPlant2;
                    //tmp.MylaNo2 = i.MylaNo2;
                    //tmp.MylaNoPlant2 = i.MylaNoPlant2;
                    //tmp.PlateNo3 = i.PlateNo3;
                    //tmp.PlateNoPlant3 = i.PlateNoPlant3;
                    //tmp.MylaNo3 = i.MylaNo3;
                    //tmp.MylaNoPlant3 = i.MylaNoPlant3;
                    //tmp.PlateNo4 = i.PlateNo4;
                    //tmp.PlateNoPlant4 = i.PlateNoPlant4;
                    //tmp.MylaNo4 = i.MylaNo4;
                    //tmp.MylaNoPlant4 = i.MylaNoPlant4;
                    //tmp.PlateNo5 = i.PlateNo5;
                    //tmp.PlateNoPlant5 = i.PlateNoPlant5;
                    //tmp.MylaNo5 = i.MylaNo5;
                    //tmp.MylaNoPlant5 = i.MylaNoPlant5;


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

                    tmp.TearTape = i.TearTapeQty == 1 ? true : false;
                    //tmp.TearTapeDistance = i.TearTapeDistance;

                    tmp.Speed = i.Speed;
                    tmp.SetupTm = i.SetupTm;
                    tmp.PrepareTm = i.PrepareTm;
                    tmp.PostTm = i.PostTm;
                    tmp.SetupWaste = i.SetupWaste;
                    tmp.RunWaste = i.RunWaste;

                    tmp.StackHeight = i.StackHeight;
                    tmp.RotateIn = i.RotateIn;
                    tmp.RotateOut = i.RotateOut;

                    tmp.ScoreGap = Convert.ToInt16(i.ScoreGap);
                    tmp.ScoreType = i.ScoreType;

                    // tmp.MylaSize = i.MylaSize;
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
                    if (!string.IsNullOrEmpty(i.Ink8))
                    {
                        sumcolor = sumcolor + 1;
                    }
                    tmp.ColourCount = sumcolor;

                    Routing_List.Add(tmp);
                }
                _moRoutingAPIRepository.SaveMORoutingsBySaleOrder(_factoryCode, modelToSave.OrderSelect, JsonConvert.SerializeObject(Routing_List), _token);
                _moDataAPIRepository.UpdateMoDataSentKIWI(_factoryCode, modelToSave.OrderSelect, _username, _token);
                var moData = JsonConvert.DeserializeObject<MoData>(_moDataAPIRepository.GetMoDataBySaleOrderNonX(_factoryCode, modelToSave.OrderSelect, _token));
                if (moData != null)
                {
                    moData.InterfaceTips = false;
                    _moDataAPIRepository.UpdateMoData(JsonConvert.SerializeObject(moData), _token);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        #endregion



    }
}
