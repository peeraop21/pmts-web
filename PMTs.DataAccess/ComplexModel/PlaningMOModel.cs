using System;
using System.Collections.Generic;

namespace PMTs.DataAccess.ComplexModel
{
    public class PlaningMOModel
    {
        public Data Data { get; set; }
        public string Description { get; set; }
    }
    public class Data
    {
        public List<PlanningMODataAndMoSpec> PlanningSpecList { get; set; }
        public List<PlanningRouting> PlanningRoutingList { get; set; }
    }

    public class PlanningRouting
    {
        public int Id { get; set; }
        public string FactoryCode { get; set; }
        public string OrderItem { get; set; }
        public string RunningNo { get; set; }
        public int Chop { get; set; }
        public string QueueStatus { get; set; }
        public int? FeedbackQty { get; set; }
        public int? Paper_Width { get; set; }
        public int? Sheet_in_Leg { get; set; }
        public int? Sheet_in_Wid { get; set; }
        public string Score_type { get; set; }
        //public int? Score_Gap { get; set; }
        public int? Cut_No { get; set; }
        public int? Trim { get; set; }
        public string NextProcess { get; set; }
        public string Plan_Code { get; set; }
        public string Machine { get; set; }
        public string Mat_Code { get; set; }
        public int Seq_No { get; set; }
        public int? Duration { get; set; }
        public string Remark_Inprocess { get; set; }
        public int? No_Open_in { get; set; }
        public int? No_Open_out { get; set; }
        public int? Speed { get; set; }
        public string Block_No { get; set; }
        public string Myla_No { get; set; }
        public string Plate_No { get; set; }
        public int? Colour_Count { get; set; }
        public string Color1 { get; set; }
        public string Shade1 { get; set; }
        public string Color2 { get; set; }
        public string Shade2 { get; set; }
        public string Color3 { get; set; }
        public string Shade3 { get; set; }
        public string Color4 { get; set; }
        public string Shade4 { get; set; }
        public string Color5 { get; set; }
        public string Shade5 { get; set; }
        public string Color6 { get; set; }
        public string Shade6 { get; set; }
        public string Color7 { get; set; }
        public string Shade7 { get; set; }
        public string Machine_Group { get; set; }
        public DateTime ProductionDate { get; set; }
        public int? ProduceAmount { get; set; }
        public int? MachineSpeed { get; set; }
        public int? ProduceAmountMeter { get; set; }
        public int? ProduceAmountSheet { get; set; }
        public bool? Semi_Blk { get; set; }
        public bool? Ship_Blk { get; set; }
    }

    public class PlanningMODataAndMoSpec
    {
        public int Id { get; set; }
        public string FactoryCode { get; set; }
        public string RunningNo { get; set; }
        //public int PlanSeqNo { get; set; }
        public string OrderItem { get; set; }
        public string Material_No { get; set; }
        public string Name { get; set; }
        public int Order_Quant { get; set; }
        public double? Tolerance_Over { get; set; }
        public double? Tolerance_Under { get; set; }
        public DateTime Due_Date { get; set; }
        public int? Target_Quant { get; set; }
        public string Item_Note { get; set; }
        public string PO_No { get; set; }
        public string Batch { get; set; }
        public string Part_No { get; set; }
        public string PC { get; set; }
        public string Sale_Org { get; set; }
        public string Plant { get; set; }
        public string Cust_Code { get; set; }
        public string Cus_ID { get; set; }
        public string Cust_Name { get; set; }
        public string Description { get; set; }
        public string Sale_Text1 { get; set; }
        public string Sale_Text2 { get; set; }
        public string Sale_Text3 { get; set; }
        public string Sale_Text4 { get; set; }
        public string Change { get; set; }
        public string Print_Method { get; set; }
        public bool? TwoPiece { get; set; }
        public string Flute { get; set; }
        public string Code { get; set; }
        public string Board { get; set; }
        public string GL { get; set; }
        public int? GLWeigth { get; set; }
        public string BM { get; set; }
        public int? BMWeigth { get; set; }
        public string BL { get; set; }
        public int? BLWeigth { get; set; }
        public string CM { get; set; }
        public int? CMWeigth { get; set; }
        public string CL { get; set; }
        public int? CLWeigth { get; set; }
        public string DM { get; set; }
        public int? DMWeigth { get; set; }
        public string DL { get; set; }
        public int? DLWeigth { get; set; }
        public int? Wid { get; set; }
        public int? Leg { get; set; }
        public int? Hig { get; set; }
        public string Box_Type { get; set; }
        public string RSC_Style { get; set; }
        public string Pro_Type { get; set; }
        public string JoinType { get; set; }
        public string Status_Flag { get; set; }
        public string Priority_Flag { get; set; }
        public int? Wire { get; set; }
        public bool? Outer_Join { get; set; }
        public int? CutSheetLeng { get; set; }
        public int? CutSheetWid { get; set; }
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
        public int? JointLap { get; set; }
        public int? ScoreL2 { get; set; }
        public int? ScoreL3 { get; set; }
        public int? ScoreL4 { get; set; }
        public int? ScoreL5 { get; set; }
        public int? ScoreL6 { get; set; }
        public int? ScoreL7 { get; set; }
        public int? ScoreL8 { get; set; }
        public int? ScoreL9 { get; set; }
        public int? Slit { get; set; }
        public int? No_Slot { get; set; }
        public int? Bun { get; set; }
        public int? BunLayer { get; set; }
        public int? LayerPalet { get; set; }
        public int? BoxPalet { get; set; }
        public double? Weight_Sh { get; set; }
        public double? Weight_Box { get; set; }
        public int SparePercen { get; set; }
        public int SpareMax { get; set; }
        public int SpareMin { get; set; }
        public int LeadTime { get; set; }
        public int Piece_Set { get; set; }
        public string Sale_UOM { get; set; }
        public string BOM_UOM { get; set; }
        public int Hardship { get; set; }
        public string PalletSize { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string User { get; set; }
        public string PicPallet { get; set; }
        public string ChangeHistory { get; set; }
        public string CustComment { get; set; }
        public string MaterialComment { get; set; }
        public decimal? CutSheetWid_Inch { get; set; }
        public decimal? CutSheetLeng_Inch { get; set; }
        public string Joint_ID { get; set; }
        public string FG_Material { get; set; }
        public string Unit_Desc { get; set; }
        public string STACK { get; set; }
        public int? PERFORATOR_1 { get; set; }
        public int? PERFORATOR_2 { get; set; }
        public int? PERFORATOR_3 { get; set; }
        public int? PERFORATOR_4 { get; set; }
        public int? PERFORATOR_5 { get; set; }
        public int? PERFORATOR_6 { get; set; }
        public int? PERFORATOR_7 { get; set; }
        public int? PERFORATOR_8 { get; set; }
        public int? PERFORATOR_9 { get; set; }
        public int? PERFORATOR_10 { get; set; }
        public int? PERFORATOR_11 { get; set; }
        public int? PERFORATOR_12 { get; set; }
        public int? PERFORATOR_13 { get; set; }
        public int? PERFORATOR_14 { get; set; }
        public int? PERFORATOR_15 { get; set; }
        public int? PERFORATOR_16 { get; set; }
        public int? PerforateGap { get; set; }
        public int? Box_Area { get; set; }
        public int? Sheet_Area { get; set; }
        public string MO_WETEND_COMMENT { get; set; }
        public string MO_DRYEND_COMMENT { get; set; }
        public string SCOREOFFSET { get; set; }
        public string TRIMTYPE { get; set; }
        public string TRIMCODE { get; set; }
    }

}
