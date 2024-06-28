using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView.Report;
using System;
using System.Collections.Generic;

namespace PMTs.DataAccess.ModelView
{
    public class MasterCardMO
    {
        public List<MasterCardMoRouting> MoRout { get; set; }
        public List<MasterCardMoRouting> PartOfMoRout { get; set; }
        public List<MasterCardRouting> Rout { get; set; }
        public List<MasterCardRouting> PartOfRout { get; set; }

        public string Factory { get; set; }
        public string DocName { get; set; }
        public string DocDate { get; set; }
        public string FactoryCode { get; set; }
        public string CutNo { get; set; }
        public string Leng { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string FormGroup { get; set; }

        // *** MO DATA
        public string OrderItem { get; set; }
        public string Material_No { get; set; }
        public int? Order_Quant { get; set; }
        public double? Tolerance_Over { get; set; }
        public double? Tolerance_Under { get; set; }
        public int Printed { get; set; }
        public string Target_Quant { get; set; }
        public string Batch { get; set; }
        public string Due_Text { get; set; }
        public string BoardAlternative { get; set; }
        public int? StockQty { get; set; }
        public string High_Value { get; set; }
        public string ItemNote { get; set; }
        public double? SquareINCH { get; set; }
        public int PrintRoundNo { get; set; }
        public int AllowancePrintNo { get; set; }
        public int AfterPrintNo { get; set; }
        public int DrawAmountNo { get; set; }

        // *** MO_Routing
        public string Distinct { get; set; }

        // *** MO_Spec
        public string Part_No { get; set; }
        public string PC { get; set; }
        public string Cust_Name { get; set; }
        public string Description { get; set; }
        public string Sale_Text1 { get; set; }
        public string Sale_Text2 { get; set; }
        public string Sale_Text3 { get; set; }
        public string Sale_Text4 { get; set; }
        public string Change { get; set; }
        public string Material_Type { get; set; }
        public string Print_Method { get; set; }
        public bool? TwoPiece { get; set; }
        public string Flute { get; set; }
        public List<Station> Stations { get; set; }
        public int? Wid { get; set; }
        public int? Leg { get; set; }
        public int? Hig { get; set; }
        public string Box_Type { get; set; }
        public string RSC_Style { get; set; }
        public string JoinType { get; set; }
        public int? JointLap { get; set; }
        public string Status_Flag { get; set; }
        public int? Wire { get; set; }
        public int? CutSheetLeng { get; set; }
        public int? CutSheetWid { get; set; }
        public decimal? CutSheetLengInch { get; set; }
        public decimal? CutSheetWidInch { get; set; }
        public int? ScoreW1 { get; set; }
        public int? Scorew2 { get; set; }
        public int? Scorew3 { get; set; }
        public int? Scorew4 { get; set; }
        public int? Scorew5 { get; set; }
        public int? Scorew6 { get; set; }
        public int? Scorew7 { get; set; }
        public int? Scorew8 { get; set; }
        public int? Scorew9 { get; set; }
        public int? Scorew10 { get; set; }
        public int? Scorew11 { get; set; }
        public int? Scorew12 { get; set; }
        public int? Scorew13 { get; set; }
        public int? Scorew14 { get; set; }
        public int? Scorew15 { get; set; }
        public int? Scorew16 { get; set; }
        public int? ScoreL2 { get; set; }
        public int? ScoreL3 { get; set; }
        public int? ScoreL4 { get; set; }
        public int? ScoreL5 { get; set; }
        public int? ScoreL6 { get; set; }
        public int? ScoreL7 { get; set; }
        public int? ScoreL8 { get; set; }
        public int? ScoreL9 { get; set; }
        public int? Slit { get; set; }
        public int? Bun { get; set; }
        public int? BunLayer { get; set; }
        public int? LayerPalet { get; set; }
        public int? BoxPalet { get; set; }
        public int? Piece_Set { get; set; }
        public int? Piece_Patch { get; set; }
        public string PalletSize { get; set; }
        public string EanCode { get; set; }
        public bool? GlWid { get; set; }
        public string Hierarchy { get; set; }
        public string TopSheetMaterial { get; set; }
        public string CustInvType { get; set; }
        public string GrossWeight { get; set; }
        //tassanai 22022022
        public string CustCode { get; set; }
        //tassanai 
        public double? WeightBox { get; set; }

        //picture
        public string PalletPath_Base64 { get; set; }
        public string DiecutPath_Base64 { get; set; }
        public string Palletization_Path { get; set; }
        public string DiecutPict_Path { get; set; }


        //tassanai
        public string KindofProductGroup { get; set; }
        public string KindofProduct { get; set; }
        public string ProcessCost { get; set; }
        public string ProductType { get; set; }
        public List<ProcessCost> ProcessCostList { get; set; }
        public TransactionsDetail transactionsDetail { get; set; }

        public string PoNo { get; set; }

        //customer detail
        public string CustomerContact { get; set; }

        //is X status
        public bool IsXStatus { get; set; }

        public bool IsPreview { get; set; }

        public int NoSlot { get; set; }

        public string AttchFilesBase64 { get; set; }

        //Tassanai Update 26/03/2021
        public string TagBundle { get; set; }
        public string TagPallet { get; set; }
        public string NoTagBundle { get; set; }
        public string HeadTagBundle { get; set; }
        public string FootTagBundle { get; set; }
        public string HeadTagPallet { get; set; }
        public string FootTagPallet { get; set; }
        public string CGType { get; set; }
        public bool IsBundle { get; set; }
        public bool IsPallet { get; set; }
        public List<PpcRawMaterialProductionBom> PpcRawMaterialProductionBoms { get; set; }
        public List<MoBomRawMat> MoBomRawmats { get; set; }

        public string CustNameNOSoldto { get; set; }


        #region TransactionDetail wty 4/8/22
        public string NewPrintPlate { get; set; }
        public string OldPrintPlate { get; set; }
        public string NewBlockDieCut { get; set; }
        public string OldBlockDieCut { get; set; }
        public string ExampleColor { get; set; }
        public string CoatingType { get; set; }
        public string CoatingTypeDesc { get; set; }

        public bool PaperHorizontal { get; set; }
        public bool PaperVertical { get; set; }
        public bool FluteHorizontal { get; set; }
        public bool FluteVertical { get; set; }
        #endregion
    }

    public class PrintMasterCardData
    {
        public List<string> OrderItem { get; set; }
        public List<string> MaterialNo { get; set; }
        public int SizeOfPage { get; set; }
        public string ProductType { get; set; }
        public string FileName { get; set; }
        public bool IsPreview { get; set; }
        public bool IsBundle { get; set; }
        public bool IsTip { get; set; }
        public bool IsPallet { get; set; }
    }

    public class PrintMasterCardMOModel
    {
        public List<MasterCardMO> MasterCardMOs { get; set; }
        public int SizeOfPage { get; set; }
        public bool IsPrintedFromFile { get; set; }
        public string FileName { get; set; }
    }

    public class Station
    {
        public int item { get; set; }
        public string TypeOfStation { get; set; }
        public string PaperGrade { get; set; }
        public string Flute { get; set; }
    }
}
