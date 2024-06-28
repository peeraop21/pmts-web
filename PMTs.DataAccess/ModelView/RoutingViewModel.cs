using Microsoft.AspNetCore.Mvc.Rendering;
using PMTs.DataAccess.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PMTs.DataAccess.ModelView
{
    public class RoutingViewModel
    {
        public int id { get; set; }
        public int SeqNo { get; set; }
        public string GroupMachine { get; set; }

        [Required]
        public string Machine { get; set; }

        public bool MachineMove { get; set; }
        public string Alternative1 { get; set; }
        public string Alternative2 { get; set; }
        public string Alternative3 { get; set; }
        public string Alternative4 { get; set; }
        public string Alternative5 { get; set; }
        public string Alternative6 { get; set; }
        public string Alternative7 { get; set; }
        public string Alternative8 { get; set; }

        // In/Out properties
        [Required]
        public string NoOpenIn { get; set; }

        [Required]
        public string NoOpenOut { get; set; }

        [Required]
        public string WeightIn { get; set; }

        [Required]
        public string WeightOut { get; set; }

        [Required]
        public string SheetLengthIn { get; set; }

        [Required]
        public string SheetLengthOut { get; set; }

        [Required]
        public string SheetWidthIn { get; set; }

        [Required]
        public string SheetWidthOut { get; set; }

        public string Coat { get; set; }

        // Corrugator Properties
        public string PaperRollWidth { get; set; }

        public string Cut { get; set; }
        public string Trim { get; set; }
        public string PercentTrim { get; set; }
        public bool TearTape { get; set; }
        public int LineQtyPerBox { get; set; }
        public string MarginForPaper { get; set; }

        // Printing properties
        public string Ink1 { get; set; }

        public string Ink2 { get; set; }
        public string Ink3 { get; set; }
        public string Ink4 { get; set; }
        public string Ink5 { get; set; }
        public string Ink6 { get; set; }
        public string Ink7 { get; set; }
        public string Ink8 { get; set; }

        public string Shade1 { get; set; }
        public string Shade2 { get; set; }
        public string Shade3 { get; set; }
        public string Shade4 { get; set; }
        public string Shade5 { get; set; }
        public string Shade6 { get; set; }
        public string Shade7 { get; set; }
        public string Shade8 { get; set; }

        public string Area1 { get; set; }
        public string Area2 { get; set; }
        public string Area3 { get; set; }
        public string Area4 { get; set; }
        public string Area5 { get; set; }
        public string Area6 { get; set; }
        public string Area7 { get; set; }
        public string Area8 { get; set; }

        public string PrintingPlateNo { get; set; }
        public string CuttingDieNo { get; set; }
        public string MylaNo { get; set; }
        public string MylaNo_Copy { get; set; }
        public string PrintingPlateType { get; set; }

        // Join to/ Sperate to
        public string JoinToMaterialNo { get; set; }

        public string SperateToMaterialNo { get; set; }

        public int TearTapeQty { get; set; }
        public string TearTapeDistance { get; set; }

        public string Remark { get; set; }
        public string RemarkImageFile { get; set; }
        public string RemarkImageFileName { get; set; }
        public int RemarkAttachFileStatus { get; set; }

        public string MachineGroupSelect { get; set; }

        public string WeightSheetDefault { get; set; }

        public string ScoreType { get; set; }
        public double ScoreGap { get; set; }

        public List<RoutingDataModel> RoutingDataList { get; set; }
        public IEnumerable<SelectListItem> MachineGroupSelectList { get; set; }
        public IEnumerable<SelectListItem> MachineSelectList { get; set; }
        public IEnumerable<SelectListItem> WeightSelectList { get; set; }
        public IEnumerable<SelectListItem> InkSelectList { get; set; }
        public IEnumerable<SelectListItem> ShadeSelectList { get; set; }

        public IEnumerable<SelectListItem> ScoreTypelist { get; set; }
        public List<ScoreGap> ScoreGapList { get; set; }

        //============ add special prop in routing
        public int Speed { get; set; }

        public int SetupTm { get; set; }
        public int PrepareTm { get; set; }
        public int PostTm { get; set; }
        public int SetupWaste { get; set; }
        public int RunWaste { get; set; }

        public int StackHeight { get; set; }
        public bool RotateIn { get; set; }
        public bool RotateOut { get; set; }

        public string Rotate { get; set; }

        public string MylaSize { get; set; }

        public int RepeatLength { get; set; }

        public string CustBarcodeNo { get; set; }

        public int Totalcolor { get; set; }

        public int? WasteLeg { get; set; }
        public int? WasteWid { get; set; }
        public bool AutoCal { get; set; }
    }

    public class RoutingDataModel
    {
        public int id { get; set; }
        public int SeqNo { get; set; }
        public string GroupMachine { get; set; }
        public string Machine { get; set; }
        public bool MachineMove { get; set; }
        public string Alternative1 { get; set; }
        public string Alternative2 { get; set; }
        public string Alternative3 { get; set; }
        public string Alternative4 { get; set; }
        public string Alternative5 { get; set; }
        public string Alternative6 { get; set; }
        public string Alternative7 { get; set; }
        public string Alternative8 { get; set; }

        // In/Out properties
        public string NoOpenIn { get; set; }

        public string NoOpenOut { get; set; }
        public string WeightIn { get; set; }
        public string WeightOut { get; set; }
        public string SheetLengthIn { get; set; }
        public string SheetLengthOut { get; set; }
        public string SheetWidthIn { get; set; }
        public string SheetWidthOut { get; set; }
        public string Coat { get; set; }

        // Corrugator Properties
        public string PaperRollWidth { get; set; }

        public string Cut { get; set; }
        public string Trim { get; set; }
        public string PercentTrim { get; set; }
        public bool? TearTape { get; set; }
        public int LineQtyPerBox { get; set; }
        public string MarginForPaper { get; set; }

        // Printing properties
        public string Ink1 { get; set; }

        public string Ink2 { get; set; }
        public string Ink3 { get; set; }
        public string Ink4 { get; set; }
        public string Ink5 { get; set; }
        public string Ink6 { get; set; }
        public string Ink7 { get; set; }
        public string Ink8 { get; set; }

        public string Shade1 { get; set; }
        public string Shade2 { get; set; }
        public string Shade3 { get; set; }
        public string Shade4 { get; set; }
        public string Shade5 { get; set; }
        public string Shade6 { get; set; }
        public string Shade7 { get; set; }
        public string Shade8 { get; set; }

        public string Area1 { get; set; }
        public string Area2 { get; set; }
        public string Area3 { get; set; }
        public string Area4 { get; set; }
        public string Area5 { get; set; }
        public string Area6 { get; set; }
        public string Area7 { get; set; }
        public string Area8 { get; set; }
        public string PrintingPlateNo { get; set; }
        public string CuttingDieNo { get; set; }
        public string MylaNo { get; set; }
        public string MylaNo_Copy { get; set; }
        public string PrintingPlateType { get; set; }

        // Join to/ Sperate to
        public string JoinToMaterialNo { get; set; }

        public string SperateToMaterialNo { get; set; }

        public string Remark { get; set; }
        public string RemarkImageFile { get; set; }
        public string RemarkImageFileName { get; set; }
        public int RemarkAttachFileStatus { get; set; }

        public string MachineGroupSelect { get; set; }

        public bool CopyStatus { get; set; }

        public bool? IsPropCor { get; set; }
        public bool? IsPropPrint { get; set; }
        public bool? IsPropDieCut { get; set; }

        //========== add fields  check box palatetype
        public bool NoneBlk { get; set; }

        public bool StanBlk { get; set; }
        public bool SemiBlk { get; set; }
        public bool ShipBlk { get; set; }

        //================ add fields diecut
        public string BlockNo2 { get; set; }

        public string BlockNoPlant2 { get; set; }
        public string BlockNo3 { get; set; }
        public string BlockNoPlant3 { get; set; }
        public string BlockNo4 { get; set; }
        public string BlockNoPlant4 { get; set; }
        public string BlockNo5 { get; set; }
        public string BlockNoPlant5 { get; set; }
        public string PlateNo2 { get; set; }
        public string PlateNoPlant2 { get; set; }
        public string MylaNo2 { get; set; }
        public string MylaNoPlant2 { get; set; }
        public string PlateNo3 { get; set; }
        public string PlateNoPlant3 { get; set; }
        public string MylaNo3 { get; set; }
        public string MylaNoPlant3 { get; set; }
        public string PlateNo4 { get; set; }
        public string PlateNoPlant4 { get; set; }
        public string MylaNo4 { get; set; }
        public string MylaNoPlant4 { get; set; }
        public string PlateNo5 { get; set; }
        public string PlateNoPlant5 { get; set; }
        public string MylaNo5 { get; set; }
        public string MylaNoPlant5 { get; set; }

        //========= add field other
        public string Plant { get; set; }

        public string Plant_Code { get; set; }
        public bool HandHold { get; set; }
        public bool Platen { get; set; }
        public bool Rotary { get; set; }
        public int Hardship { get; set; }
        public bool UnUpgrad_Board { get; set; }
        public int Color_count { get; set; }
        public bool StdProcess { get; set; }
        public int Human { get; set; }

        public int TearTapeQty { get; set; }
        public string TearTapeDistance { get; set; }

        //==========add special prop to routing
        public int Speed { get; set; }

        public int SetupTm { get; set; }
        public int PrepareTm { get; set; }
        public int PostTm { get; set; }
        public int SetupWaste { get; set; }
        public int RunWaste { get; set; }

        public int StackHeight { get; set; }
        public bool RotateIn { get; set; }
        public bool RotateOut { get; set; }

        public string ScoreType { get; set; }
        public double ScoreGap { get; set; }

        public string Rotate { get; set; }

        public string MylaSize { get; set; }

        public int RepeatLength { get; set; }

        public string CustBarcodeNo { get; set; }
        public int Totalcolor { get; set; }

        //add pmt2
        public int? WasteLeg { get; set; }

        public int? WasteWid { get; set; }
        public bool AutoCal { get; set; }
    }

    public class GroupMachineModels
    {
        public string Id { get; set; }
        public string GroupMachine { get; set; }
    }
}