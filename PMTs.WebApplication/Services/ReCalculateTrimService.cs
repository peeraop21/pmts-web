using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using OfficeOpenXml;
using PMTs.DataAccess.ComplexModel;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Tracing;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services
{
    [TraceAspect]
    public class ReCalculateTrimService : IReCalculateTrimService
    {
        IHttpContextAccessor httpContextAccessor;
        private readonly IFluteAPIRepository fluteAPIRepository;
        private readonly IPaperWidthAPIRepository paperWidthAPIRepository;
        private readonly IFormulaAPIRepository formulaAPIRepository;
        private readonly IRoutingAPIRepository routingAPIRepository;
        private readonly IMachineAPIRepository machineAPIRepository;

        private readonly string _username;
        private readonly string _saleOrg;
        private readonly string _factoryCode;
        private readonly string _token;

        public ReCalculateTrimService(IHttpContextAccessor httpContextAccessor,
            IFluteAPIRepository fluteAPIRepository,
            IFormulaAPIRepository formulaAPIRepository,
            IPaperWidthAPIRepository paperWidthAPIRepository,
            IRoutingAPIRepository routingAPIRepository,
            IMachineAPIRepository machineAPIRepository)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.fluteAPIRepository = fluteAPIRepository;
            this.formulaAPIRepository = formulaAPIRepository;
            this.paperWidthAPIRepository = paperWidthAPIRepository;
            this.routingAPIRepository = routingAPIRepository;
            this.machineAPIRepository = machineAPIRepository;

            var userSessionModel = SessionExtentions.GetSession<UserSessionModel>(this.httpContextAccessor.HttpContext.Session, "UserSessionModel");
            if (userSessionModel != null)
            {
                _username = userSessionModel.UserName;
                _saleOrg = userSessionModel.SaleOrg;
                _factoryCode = userSessionModel.FactoryCode;
                _token = userSessionModel.Token;
            }

        }

        public List<Flute> GetFlutesByFactoryCode()
        {
            var model = new List<Flute>();
            model = JsonConvert.DeserializeObject<List<Flute>>(fluteAPIRepository.GetFluteList(_factoryCode, _token));
            return model;
        }

        public ReCalculateTrimViewModel GetFlutesAndMachinesByFactoryCode()
        {
            ReCalculateTrimViewModel result = new ReCalculateTrimViewModel();
            result.FluteAndMachineModels = new List<FluteAndMachineModel>();
            result.Machines = new List<Machine>();
            var data = JsonConvert.DeserializeObject<List<FluteAndMachineModel>>(fluteAPIRepository.GetFlutesAndMachinesByFactoryCode(_factoryCode, _token));
            for (int i = 0; i < data.Count(); i++)
            {
                result.Machines.Add(new Machine() { Machine1 = data[i].FluteAndMachine.Split(",")[1].Trim() });
            }
            result.FluteAndMachineModels = data.DistinctBy(d => d.Flute).ToList();
            result.Machines = result.Machines.DistinctBy(d => d.Machine1).ToList();
            return result;
        }

        public ChangeReCalculateTrimModel GetReCalculateTrim(string flute, string machine, string boxType, string printMethod, string proType)
        {
            SessionExtentions.SetSession(httpContextAccessor.HttpContext.Session, "ReCalculateTrimFailedResult", new ChangeReCalculateTrimModel());
            SessionExtentions.SetSession(httpContextAccessor.HttpContext.Session, "ReCalculateTrimData", new ChangeReCalculateTrimModel());

            var changeReCalculateTrimModel = JsonConvert.DeserializeObject<ChangeReCalculateTrimModel>(formulaAPIRepository.GetReCalculateTrim(_factoryCode, flute, machine, boxType, printMethod, proType, _username, _token));
            changeReCalculateTrimModel.Flute = flute;
            SessionExtentions.SetSession(httpContextAccessor.HttpContext.Session, "ReCalculateTrimData", changeReCalculateTrimModel);
            return changeReCalculateTrimModel;
        }

        public void ReCalculateTrim(string flute, int numberOfProgress, int processLimit, ref ChangeReCalculateTrimModel model, ref List<ReCalculateTrimModel> reCalculateTrims, ref List<Routing> routings)
        {
            if (model.DataTable.Rows.Count <= 0)
            {
                model.DataTable = new DataTable();
                model.DataTable.Columns.AddRange(new DataColumn[] {
                new DataColumn("MaterialNo"),
                new DataColumn("Machine"),
                new DataColumn("SheetInWid"),
                new DataColumn("Flute"),
                new DataColumn("PaperWidthOld"),
                new DataColumn("CutNoOld"),
                new DataColumn("TrimOld"),
                new DataColumn("PercenTrimOld"),
                new DataColumn("PaperWidth"),
                new DataColumn("CutNo"),
                new DataColumn("Trim"),
                new DataColumn("PercenTrim"),
            });
            }
            //check if no record for recalculate
            if (model.ReCalculateTrimModels == null || model.ReCalculateTrimModels.Count == 0)
            {
                throw new Exception("Can't find routing for ReCalculateTrim.");
            }
            routings = new List<Routing>();
            var skipItems = processLimit * (numberOfProgress - 1);
            reCalculateTrims = model.ReCalculateTrimModels.Skip(skipItems).Take(processLimit).ToList();
            var ret = JsonConvert.DeserializeObject<List<ReturnCalPaperWidth>>(formulaAPIRepository.CalculateListRouting(_factoryCode, JsonConvert.SerializeObject(reCalculateTrims), _token));

            foreach (var i in ret)
            {
                model.DataTable.Rows.Add(
                    //reCalculateTrimModel.FactoryCode,
                    string.IsNullOrEmpty(i.MaterialNo) ? " " : i.MaterialNo,
                    string.IsNullOrEmpty(i.MachineName) ? " " : i.MachineName,
                    i.SheetInWid,
                    string.IsNullOrEmpty(i.Flute) ? " " : i.Flute,
                    string.IsNullOrEmpty(i.PaperWidthOld) ? " " : i.PaperWidthOld,
                    string.IsNullOrEmpty(i.CutOld) ? " " : i.CutOld,
                    string.IsNullOrEmpty(i.TrimOld) ? " " : i.TrimOld,
                    string.IsNullOrEmpty(i.PercentTrimOld) ? " " : i.PercentTrimOld,
                    //reCalculateTrimModel.TrimOfFlute,
                    string.IsNullOrEmpty(i.PaperWidth) ? " " : i.PaperWidth,
                    string.IsNullOrEmpty(i.Cut) ? " " : i.Cut,
                    string.IsNullOrEmpty(i.Trim) ? " " : i.Trim,
                    string.IsNullOrEmpty(i.PercentTrim) ? " " : i.PercentTrim
               );
            }

            #region old code

            //var RollWidth = JsonConvert.DeserializeObject<List<PaperWidth>>(paperWidthAPIRepository.GetPaperWidthList(_factoryCode, _token)).OrderBy(o => o.Group2).ToList();
            //foreach (var reCalculateTrimModel in reCalculateTrims)
            //{
            //    var routing = new Routing();
            //    int? sizeWidth = 0;                                                                                                 //   TransactionDataModel tran = SessionExtentions.GetSession<TransactionDataModel>(_httpContextAccessor.HttpContext.Session, "TransactionDataModel");
            //    var pageMin = reCalculateTrimModel.MinTrim ? reCalculateTrimModel.PageMinTrim : reCalculateTrimModel.PageMin;
            //    var pageMax = reCalculateTrimModel.PageMax;

            //    #region Set Routing Model
            //    routing.Id = reCalculateTrimModel.Id;
            //    routing.FactoryCode = reCalculateTrimModel.FactoryCode;
            //    routing.SeqNo = reCalculateTrimModel.SeqNo;
            //    routing.Plant = reCalculateTrimModel.Plant;
            //    routing.MaterialNo = reCalculateTrimModel.MaterialNo;
            //    routing.MatCode = reCalculateTrimModel.MatCode;
            //    routing.PlanCode = reCalculateTrimModel.PlanCode;
            //    routing.Machine = reCalculateTrimModel.Machine;
            //    routing.Alternative1 = reCalculateTrimModel.Alternative1;
            //    routing.Alternative2 = reCalculateTrimModel.Alternative2;
            //    routing.StdProcess = reCalculateTrimModel.StdProcess;
            //    routing.Speed = reCalculateTrimModel.Speed;
            //    routing.ColourCount = reCalculateTrimModel.ColourCount;
            //    routing.McMove = reCalculateTrimModel.McMove;
            //    routing.HandHold = reCalculateTrimModel.HandHold;
            //    routing.PlateNo = reCalculateTrimModel.PlateNo;
            //    routing.MylaNo = reCalculateTrimModel.MylaNo;
            //    routing.PaperWidth = reCalculateTrimModel.PaperWidth;
            //    routing.CutNo = reCalculateTrimModel.CutNo;
            //    routing.Trim = reCalculateTrimModel.Trim;
            //    routing.PercenTrim = reCalculateTrimModel.PercenTrim;
            //    routing.WasteLeg = reCalculateTrimModel.WasteLeg;
            //    routing.WasteWid = reCalculateTrimModel.WasteWid;
            //    routing.SheetInLeg = reCalculateTrimModel.SheetInLeg;
            //    routing.SheetInWid = reCalculateTrimModel.SheetInWid;
            //    routing.SheetOutLeg = reCalculateTrimModel.SheetOutLeg;
            //    routing.SheetOutWid = reCalculateTrimModel.SheetOutWid;
            //    routing.WeightIn = reCalculateTrimModel.WeightIn;
            //    routing.WeightOut = reCalculateTrimModel.WeightOut;
            //    routing.NoOpenIn = reCalculateTrimModel.NoOpenIn;
            //    routing.NoOpenOut = reCalculateTrimModel.NoOpenOut;
            //    routing.Color1 = reCalculateTrimModel.Color1;
            //    routing.Shade1 = reCalculateTrimModel.Shade1;
            //    routing.Color2 = reCalculateTrimModel.Color2;
            //    routing.Shade2 = reCalculateTrimModel.Shade2;
            //    routing.Color3 = reCalculateTrimModel.Color3;
            //    routing.Shade3 = reCalculateTrimModel.Shade3;
            //    routing.Color4 = reCalculateTrimModel.Color4;
            //    routing.Shade4 = reCalculateTrimModel.Shade4;
            //    routing.Color5 = reCalculateTrimModel.Color5;
            //    routing.Shade5 = reCalculateTrimModel.Shade5;
            //    routing.Color6 = reCalculateTrimModel.Color6;
            //    routing.Shade6 = reCalculateTrimModel.Shade6;
            //    routing.Color7 = reCalculateTrimModel.Color7;
            //    routing.Shade7 = reCalculateTrimModel.Shade7;
            //    routing.ColorArea1 = reCalculateTrimModel.ColorArea1;
            //    routing.ColorArea2 = reCalculateTrimModel.ColorArea2;
            //    routing.ColorArea3 = reCalculateTrimModel.ColorArea3;
            //    routing.ColorArea4 = reCalculateTrimModel.ColorArea4;
            //    routing.ColorArea5 = reCalculateTrimModel.ColorArea5;
            //    routing.ColorArea6 = reCalculateTrimModel.ColorArea6;
            //    routing.ColorArea7 = reCalculateTrimModel.ColorArea7;
            //    routing.Platen = reCalculateTrimModel.Platen;
            //    routing.Rotary = reCalculateTrimModel.Rotary;
            //    routing.TearTape = reCalculateTrimModel.TearTape;
            //    routing.NoneBlk = reCalculateTrimModel.NoneBlk;
            //    routing.StanBlk = reCalculateTrimModel.StanBlk;
            //    routing.SemiBlk = reCalculateTrimModel.SemiBlk;
            //    routing.ShipBlk = reCalculateTrimModel.ShipBlk;
            //    routing.BlockNo = reCalculateTrimModel.BlockNo;
            //    routing.JoinMatNo = reCalculateTrimModel.JoinMatNo;
            //    routing.SeparatMatNo = reCalculateTrimModel.SeparatMatNo;
            //    routing.RemarkInprocess = reCalculateTrimModel.RemarkInprocess;
            //    routing.Hardship = reCalculateTrimModel.Hardship;
            //    routing.PdisStatus = reCalculateTrimModel.PdisStatus;
            //    routing.TranStatus = reCalculateTrimModel.TranStatus;
            //    routing.SapStatus = reCalculateTrimModel.SapStatus;
            //    routing.Alternative3 = reCalculateTrimModel.Alternative3;
            //    routing.Alternative4 = reCalculateTrimModel.Alternative4;
            //    routing.Alternative5 = reCalculateTrimModel.Alternative5;
            //    routing.Alternative6 = reCalculateTrimModel.Alternative6;
            //    routing.Alternative7 = reCalculateTrimModel.Alternative7;
            //    routing.Alternative8 = reCalculateTrimModel.Alternative8;
            //    routing.RotateIn = reCalculateTrimModel.RotateIn;
            //    routing.RotateOut = reCalculateTrimModel.RotateOut;
            //    routing.StackHeight = reCalculateTrimModel.StackHeight;
            //    routing.SetupTm = reCalculateTrimModel.SetupTm;
            //    routing.SetupWaste = reCalculateTrimModel.SetupWaste;
            //    routing.PrepareTm = reCalculateTrimModel.PrepareTm;
            //    routing.PostTm = reCalculateTrimModel.PostTm;
            //    routing.RunWaste = reCalculateTrimModel.RunWaste;
            //    routing.Human = reCalculateTrimModel.Human;
            //    routing.ColorCount = reCalculateTrimModel.ColorCount;
            //    routing.UnUpgradBoard = reCalculateTrimModel.UnUpgradBoard;
            //    routing.ScoreType = reCalculateTrimModel.ScoreType;
            //    routing.ScoreGap = reCalculateTrimModel.ScoreGap;
            //    routing.Coating = reCalculateTrimModel.Coating;
            //    routing.BlockNo2 = reCalculateTrimModel.BlockNo2;
            //    routing.BlockNoPlant2 = reCalculateTrimModel.BlockNoPlant2;
            //    routing.BlockNo3 = reCalculateTrimModel.BlockNo3;
            //    routing.BlockNoPlant3 = reCalculateTrimModel.BlockNoPlant3;
            //    routing.BlockNo4 = reCalculateTrimModel.BlockNo4;
            //    routing.BlockNoPlant4 = reCalculateTrimModel.BlockNoPlant4;
            //    routing.BlockNo5 = reCalculateTrimModel.BlockNo5;
            //    routing.BlockNoPlant5 = reCalculateTrimModel.BlockNoPlant5;
            //    routing.PlateNo2 = reCalculateTrimModel.PlateNo2;
            //    routing.PlateNoPlant2 = reCalculateTrimModel.PlateNoPlant2;
            //    routing.MylaNo2 = reCalculateTrimModel.MylaNo2;
            //    routing.MylaNoPlant2 = reCalculateTrimModel.MylaNoPlant2;
            //    routing.PlateNo3 = reCalculateTrimModel.PlateNo3;
            //    routing.PlateNoPlant3 = reCalculateTrimModel.PlateNoPlant3;
            //    routing.MylaNo3 = reCalculateTrimModel.MylaNo3;
            //    routing.MylaNoPlant3 = reCalculateTrimModel.MylaNoPlant3;
            //    routing.PlateNo4 = reCalculateTrimModel.PlateNo4;
            //    routing.PlateNoPlant4 = reCalculateTrimModel.PlateNoPlant4;
            //    routing.MylaNo4 = reCalculateTrimModel.MylaNo4;
            //    routing.MylaNoPlant4 = reCalculateTrimModel.MylaNoPlant4;
            //    routing.PlateNo5 = reCalculateTrimModel.PlateNo5;
            //    routing.PlateNoPlant5 = reCalculateTrimModel.PlateNoPlant5;
            //    routing.MylaNo5 = reCalculateTrimModel.MylaNo5;
            //    routing.MylaNoPlant5 = reCalculateTrimModel.MylaNoPlant5;
            //    routing.TearTapeQty = reCalculateTrimModel.TearTapeQty;
            //    routing.TearTapeDistance = reCalculateTrimModel.TearTapeDistance;
            //    routing.MylaSize = reCalculateTrimModel.MylaSize;
            //    routing.RepeatLength = reCalculateTrimModel.RepeatLength;
            //    routing.CustBarcodeNo = reCalculateTrimModel.CustBarcodeNo;
            //    routing.ControllerCode = reCalculateTrimModel.ControllerCode;
            //    routing.PlanProgramCode = reCalculateTrimModel.PlanProgramCode;
            //    routing.CreatedDate = reCalculateTrimModel.CreatedDate;
            //    routing.CreatedBy = reCalculateTrimModel.CreatedBy;
            // #endregion
            //if (reCalculateTrimModel.MinTrim) //Min Trim
            //{

            //    double[,] RollSize = new double[6, 4];
            //    int X, M;

            //    var Roll = RollWidth.FirstOrDefault(w => w.Group1 == pageMin || w.Group2 == pageMin || w.Group3 == pageMin || w.Group4 == pageMin);

            //    if (Roll != null)
            //    {
            //        pageMin = reCalculateTrimModel.GroupPaperWidth == 1 ? ConvertInt16ToShort(Roll.Group1) : reCalculateTrimModel.GroupPaperWidth == 2 ? ConvertInt16ToShort(Roll.Group2) : reCalculateTrimModel.GroupPaperWidth == 3 ? ConvertInt16ToShort(Roll.Group3) : ConvertInt16ToShort(Roll.Group4);

            //    }

            //    for (X = 0; X < reCalculateTrimModel.CutOff; X++) //คำนวนหน้ากว้าง + Standard Trim
            //    {
            //        RollSize[X, 1] = (reCalculateTrimModel.CutSheetWid.Value * (X + 1)) + reCalculateTrimModel.TrimOfFlute.Value;

            //        if (RollSize[X, 1] < pageMin)
            //        {
            //            RollSize[X, 0] = pageMin;   //น้อยกว่าหน้าน้อยสุด
            //        }
            //        else if (RollSize[X, 1] > pageMax)
            //        {
            //            X = X - 1;
            //            //RollSize[X, 0] = RollSize[X - 1, 0];     //มากกว่าหน้าสูงสุด
            //            break;
            //        }
            //        else
            //        {
            //            switch (reCalculateTrimModel.GroupPaperWidth)
            //            {
            //                case 1:
            //                    for (M = 0; M < RollWidth.Count; M++)
            //                    {
            //                        if (ConvertInt16ToShort(RollWidth[M].Group1) >= pageMin && ConvertInt16ToShort(RollWidth[M].Group1) <= pageMax)
            //                        {
            //                            if (RollSize[X, 1] <= ConvertInt16ToShort(RollWidth[M].Group1))
            //                            {
            //                                RollSize[X, 0] = ConvertInt16ToShort(RollWidth[M].Group1);
            //                                break;
            //                            }
            //                        }
            //                    }
            //                    break;
            //                case 2:
            //                    for (M = 0; M < RollWidth.Count; M++)
            //                    {
            //                        if (ConvertInt16ToShort(RollWidth[M].Group2) >= pageMin && ConvertInt16ToShort(RollWidth[M].Group2) <= pageMax)
            //                        {
            //                            if (RollSize[X, 1] <= ConvertInt16ToShort(RollWidth[M].Group2))
            //                            {
            //                                RollSize[X, 0] = ConvertInt16ToShort(RollWidth[M].Group2);
            //                                break;
            //                            }
            //                        }
            //                    }
            //                    break;
            //                case 3:
            //                    for (M = 0; M < RollWidth.Count; M++)
            //                    {
            //                        if (ConvertInt16ToShort(RollWidth[M].Group3) >= pageMin && ConvertInt16ToShort(RollWidth[M].Group3) <= pageMax)
            //                        {
            //                            if (RollSize[X, 1] <= ConvertInt16ToShort(RollWidth[M].Group3))
            //                            {
            //                                RollSize[X, 0] = ConvertInt16ToShort(RollWidth[M].Group3);
            //                                break;
            //                            }
            //                        }
            //                    }
            //                    break;
            //                case 4:
            //                    for (M = 0; M < RollWidth.Count; M++)
            //                    {
            //                        if (ConvertInt16ToShort(RollWidth[M].Group4) >= pageMin && ConvertInt16ToShort(RollWidth[M].Group4) <= pageMax)
            //                        {
            //                            if (RollSize[X, 1] <= ConvertInt16ToShort(RollWidth[M].Group4))
            //                            {
            //                                RollSize[X, 0] = ConvertInt16ToShort(RollWidth[M].Group4);
            //                                break;
            //                            }
            //                        }
            //                    }
            //                    break;

            //            }
            //        } // อันเก่า


            //    }

            //    if (X >= reCalculateTrimModel.CutOff) X = reCalculateTrimModel.CutOff - 1;

            //    if (RollSize[X, 0] > 0)
            //    {
            //        for (X = X; X >= 0; X--)
            //        {
            //            RollSize[X, 2] = (RollSize[X, 0] - (RollSize[X, 1] - reCalculateTrimModel.TrimOfFlute.Value)) / RollSize[X, 0] * 100; //คำนวน % Trim

            //            if (Convert.ToDouble(routing.PercenTrim) >= Math.Round(RollSize[X, 2], 2))//เลือก % Trim น้อยที่สุด
            //            {
            //                routing.PaperWidth = Convert.ToInt32(RollSize[X, 0]);
            //                routing.CutNo = Convert.ToInt32((X + 1).ToString());
            //                routing.Trim = Convert.ToInt32((RollSize[X, 0] - RollSize[X, 1] + reCalculateTrimModel.TrimOfFlute.Value).ToString()); //เศษ
            //                routing.PercenTrim = Convert.ToDouble((Math.Round(RollSize[X, 2], 2)).ToString());
            //            }
            //        }
            //    }
            //}
            //else //Max Out
            //{
            //    sizeWidth = (reCalculateTrimModel.CutSheetWid * reCalculateTrimModel.CutOff) + reCalculateTrimModel.TrimOfFlute;

            //    //ตรวจหาหน้ากว้างสุดและแคบสุด
            //    if (sizeWidth < pageMin)
            //    {
            //        routing.PaperWidth = Convert.ToInt32(reCalculateTrimModel.PageMin.ToString());                                //Paper Width
            //        routing.CutNo = 1;                                                          //จำนวนตัด
            //        routing.Trim = Convert.ToInt32((reCalculateTrimModel.PageMin - reCalculateTrimModel.CutSheetWid).ToString());                            //เศษตัดริม
            //        routing.PercenTrim = Math.Round((Convert.ToDouble(routing.Trim.Value) / Convert.ToDouble(pageMin) * 100), 2);   //% Waste
            //    }
            //    /////////////////////////////////////////////////////////////

            //    if (reCalculateTrimModel.CutSheetWid + reCalculateTrimModel.TrimOfFlute > pageMax)
            //    {
            //        routing.PaperWidth = pageMax;                                //Paper Width
            //        routing.CutNo = reCalculateTrimModel.CutOff;                                            //จำนวนตัด
            //        routing.Trim = (pageMax - reCalculateTrimModel.CutSheetWid * 1);                        //เศษตัดริม
            //        routing.PercenTrim = Math.Round((Convert.ToDouble(routing.Trim.Value) / Convert.ToDouble(pageMax) * 100), 2);   //% Waste
            //    }

            //    /////////////////////////////////////////////////////////////

            //    int k = reCalculateTrimModel.CutOff;
            //    for (k = reCalculateTrimModel.CutOff; k > 0; k--)
            //    {
            //        if (sizeWidth > pageMin)
            //        {
            //            sizeWidth = reCalculateTrimModel.CutSheetWid * k + reCalculateTrimModel.TrimOfFlute;
            //            if (sizeWidth <= pageMax)
            //            {
            //                break;
            //            }
            //        }
            //        else break;
            //    }

            //    switch (reCalculateTrimModel.GroupPaperWidth)
            //    {
            //        case 1:
            //            foreach (var rollWidth in RollWidth)
            //            {
            //                if (rollWidth.Group1 >= sizeWidth)
            //                {
            //                    routing.PaperWidth = rollWidth.Group1;                                        //Paper Width
            //                    routing.CutNo = k;                                                            //จำนวนตัด
            //                    routing.Trim = (rollWidth.Group1 - reCalculateTrimModel.CutSheetWid.Value * k);               //เศษตัดริม
            //                    routing.PercenTrim = Math.Round((Convert.ToDouble(routing.Trim) / Convert.ToDouble(rollWidth.Group1) * 100), 2);           //% Waste
            //                    break;
            //                }
            //            }
            //            break;
            //        case 2:
            //            foreach (var rollWidth in RollWidth)
            //            {
            //                if (rollWidth.Group2 >= sizeWidth)
            //                {
            //                    routing.PaperWidth = rollWidth.Group2;                                        //Paper Width
            //                    routing.CutNo = k;                                                            //จำนวนตัด
            //                    routing.Trim = (rollWidth.Group2 - reCalculateTrimModel.CutSheetWid.Value * k);     //เศษตัดริม
            //                    routing.PercenTrim = Math.Round((Convert.ToDouble(routing.Trim) / Convert.ToDouble(rollWidth.Group2) * 100), 2);     //% Waste
            //                    break;
            //                }
            //            }
            //            break;
            //    }

            //}

            //model.DataTable.Rows.Add(
            //        //reCalculateTrimModel.FactoryCode,
            //        reCalculateTrimModel.MaterialNo,
            //        reCalculateTrimModel.CutSheetWid,
            //        reCalculateTrimModel.Flute,
            //        reCalculateTrimModel.PaperWidth,
            //        reCalculateTrimModel.CutNo,
            //        reCalculateTrimModel.Trim,
            //        reCalculateTrimModel.PercenTrim,
            //        reCalculateTrimModel.TrimOfFlute,
            //        routing.PaperWidth,
            //        routing.CutNo,
            //        routing.Trim,
            //        routing.PercenTrim
            //        );

            //routing update collection(waiting for update)
            //if (!double.IsInfinity(routing.PercenTrim.Value))
            //{
            //    model.Routings.Add(routing);
            //    routings.Add(routing);
            //}
            //}
            #endregion

            model.Flute = flute;
            SessionExtentions.SetSession(httpContextAccessor.HttpContext.Session, "ReCalculateTrimData", model);
        }

        private int ConvertStringToShort(string Input)
        {
            return string.IsNullOrEmpty(Input) ? 0 : Convert.ToInt16(Input);
        }

        private Int16 ConvertInt16ToShort(int? Input)
        {
            return (Int16)(string.IsNullOrEmpty(Input.ToString()) ? 0 : Input);
        }

        public void SaveReCalculateTrim(ChangeReCalculateTrimModel changeReCalculateTrimModel)
        {
            if (changeReCalculateTrimModel != null && changeReCalculateTrimModel.Routings != null && changeReCalculateTrimModel.ReCalculateTrimModels != null && changeReCalculateTrimModel.Routings.Count > 0 && changeReCalculateTrimModel.ReCalculateTrimModels.Count > 0)
            {
                changeReCalculateTrimModel.ReCalculateTrimModels.ForEach(r => r.UpdatedBy = "ReCalTrim_" + _username);
                changeReCalculateTrimModel.ReCalculateTrimModels.ForEach(r => r.UpdatedDate = DateTime.Now);
                changeReCalculateTrimModel.Routings.ForEach(r => r.UpdatedBy = "ReCalTrim_" + _username);
                changeReCalculateTrimModel.Routings.ForEach(r => r.UpdatedDate = DateTime.Now);
                formulaAPIRepository.SaveReCalculateTrim(_factoryCode, JsonConvert.SerializeObject(changeReCalculateTrimModel), _token);
            }
            else
            {
                throw new Exception("invalid data ReCalculateTrim.");
            }
        }

        public void ImportReCalculateTrimFromFile(IFormFile file, ref string exceptionMessage)
        {
            if (file != null)
            {
                long size = file.Length;

                // full path to file in temp location
                var filePath = Path.GetTempFileName();

                using (var ms = new MemoryStream())
                {
                    DataTable table = new DataTable();

                    file.CopyToAsync(ms);
                    var fileBytes = ms.ToArray();
                    string s = Convert.ToBase64String(fileBytes);
                    var invalidData = false;

                    using (ExcelPackage excelPackage = new ExcelPackage(ms))
                    {
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        //ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["BoardCombineAcc"];

                        List<ExcelWorksheet> worksheets = excelPackage.Workbook.Worksheets.ToList();
                        foreach (var worksheet in worksheets)
                        {
                            //loop all rows
                            for (int i = worksheet.Dimension.Start.Row; i <= worksheet.Dimension.End.Row; i++)
                            {
                                invalidData = false;
                                var rowsObj = new object[12];
                                //loop all columns in a row
                                for (int j = worksheet.Dimension.Start.Column; j <= 12; j++)
                                {
                                    var columnValue = worksheet.Cells[i, j].Value;
                                    if (i == 1)
                                    {
                                        if (j == 12)
                                        {
                                            if (columnValue != null)
                                            {
                                                table.Columns.Add(columnValue.ToString(), typeof(float));
                                            }
                                        }
                                        else
                                        {
                                            if (columnValue != null)
                                            {
                                                table.Columns.Add(columnValue.ToString(), typeof(string));
                                            }
                                        }
                                    }
                                    else if (i > 1)
                                    {
                                        if (j == 1 || j == 2)
                                        {
                                            rowsObj[j - 1] = columnValue == null ? columnValue : columnValue.ToString();
                                        }
                                        else
                                        {
                                            rowsObj[j - 1] = columnValue != null ? columnValue : null;
                                        }
                                    }

                                    if (j == 12 && i != 1)
                                    {
                                        if (worksheet.Cells[i, 1].Value != null || worksheet.Cells[i, 2].Value != null
                                            || worksheet.Cells[i, 9].Value != null || worksheet.Cells[i, 10].Value != null
                                            || worksheet.Cells[i, 11].Value != null || worksheet.Cells[i, 12].Value != null)
                                        {
                                            table.Rows.Add(rowsObj);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    var reCalculateTrimModels = new List<ReCalculateTrimModel>();
                    var result = new List<ReCalculateTrimModel>();
                    var failedResult = new List<ReCalculateTrimModel>();
                    var allFaileds = new List<ReCalculateTrimModel>();
                    int paperWidth = -999;
                    int cutNo = -999;
                    int trim = -999;
                    double percenTrim = -999;
                    #region Convert Datatable To ReCalculateTrimModel Model
                    reCalculateTrimModels = (from rw in table.AsEnumerable()
                                             select new ReCalculateTrimModel()
                                             {
                                                 Id = 0,
                                                 MaterialNo = rw["MaterialNo"].ToString(),
                                                 Machine = rw["Machine"].ToString(),
                                                 PaperWidth = Int32.TryParse(rw["PaperWidth"].ToString(), out paperWidth) ? paperWidth : -999,
                                                 CutNo = Int32.TryParse(rw["CutNo"].ToString(), out cutNo) ? cutNo : -999,
                                                 Trim = Int32.TryParse(rw["Trim"].ToString(), out trim) ? trim : -999,
                                                 PercenTrim = double.TryParse(rw["PercenTrim"].ToString(), out percenTrim) ? percenTrim : -999,
                                                 FactoryCode = _factoryCode,
                                                 UpdatedDate = DateTime.Now,
                                                 UpdatedBy = "ReCalTrim_" + _username,
                                             }).ToList();
                    #endregion

                    var validResult = reCalculateTrimModels.Where(r => !string.IsNullOrEmpty(r.Machine)
                        && !string.IsNullOrEmpty(r.MaterialNo)
                        && r.PaperWidth != -999
                        && r.CutNo != -999
                        && r.Trim != -999
                        && r.PercenTrim != -999).ToList();

                    var invalidResult = reCalculateTrimModels.Where(r => string.IsNullOrEmpty(r.Machine)
                         || string.IsNullOrEmpty(r.MaterialNo)
                         || r.PaperWidth == -999
                         || r.CutNo == -999
                         || r.Trim == -999
                         || r.PercenTrim == -999).ToList();
                    invalidResult.ForEach(i => i.UpdateStatus = false);
                    invalidResult.ForEach(i => i.ErrorMessase = string.IsNullOrEmpty(i.MaterialNo) ?
                    string.IsNullOrEmpty(i.ErrorMessase) ? "invalid MaterialNo." : string.Empty : string.Empty);

                    invalidResult.ForEach(i => i.ErrorMessase = string.IsNullOrEmpty(i.Machine) ?
                    string.IsNullOrEmpty(i.ErrorMessase) ? "invalid Machine" : i.ErrorMessase + ", Machine" : i.ErrorMessase);
                    invalidResult.ForEach(i => i.ErrorMessase = i.PaperWidth == -999 ?
                    string.IsNullOrEmpty(i.ErrorMessase) ? "invalid PaperWidth" : i.ErrorMessase + ", PaperWidth" : i.ErrorMessase);
                    invalidResult.ForEach(i => i.ErrorMessase = i.CutNo == -999 ?
                    string.IsNullOrEmpty(i.ErrorMessase) ? "invalid CutNo" : i.ErrorMessase + ", CutNo" : i.ErrorMessase);
                    invalidResult.ForEach(i => i.ErrorMessase = i.Trim == -999 ?
                    string.IsNullOrEmpty(i.ErrorMessase) ? "invalid Trim" : i.ErrorMessase + ", Trim" : i.ErrorMessase);
                    invalidResult.ForEach(i => i.ErrorMessase = i.PercenTrim == -999 ?
                     string.IsNullOrEmpty(i.ErrorMessase) ? "invalid PercenTrim" : i.ErrorMessase + ", PercenTrim" : i.ErrorMessase);

                    if (validResult.Count > 0)
                    {
                        result = JsonConvert.DeserializeObject<List<ReCalculateTrimModel>>(routingAPIRepository.UpdateReCalculateTrimFromFile(_factoryCode, JsonConvert.SerializeObject(validResult), _token));
                        if (result != null && result.Count > 0)
                        {
                            failedResult = result.Where(r => !r.UpdateStatus).ToList();
                        }
                    }

                    allFaileds.Clear();
                    allFaileds.AddRange(invalidResult);
                    allFaileds.AddRange(failedResult);
                    SessionExtentions.SetSession(httpContextAccessor.HttpContext.Session, "ReCalculateTrimFailedResult", allFaileds);

                    exceptionMessage = allFaileds != null && allFaileds.Count > 0 ? $"found invalid data to recalculate {allFaileds.Count} item." : string.Empty;
                }
            }
        }
    }
}
