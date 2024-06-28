namespace PMTs.DataAccess.ComplexModel
{
    public class RSCModel
    {
        public string Code { get; set; }
        public string Flute { get; set; }
        public int? Wid { get; set; }
        public int? Leg { get; set; }
        public int? Hig { get; set; }
        public bool? TwoPiece { get; set; }
        public bool? GLWid { get; set; }

        public int? ScoreW1 { get; set; }
        public int? Scorew2 { get; set; }
        public int? Scorew3 { get; set; }
        public int? Scorew16 { get; set; }
        public int? ScoreL2 { get; set; }
        public int? ScoreL3 { get; set; }
        public int? ScoreL4 { get; set; }
        public int? ScoreL5 { get; set; }
        public int? ScoreL8 { get; set; }
        public int? ScoreL9 { get; set; }

        public int? Slit { get; set; }
        public int? JoinSize { get; set; }
        public string PlantCode { get; set; }
        public int spcLen { get; set; }
        public int? SheetArea { get; set; }
        public int? BoxArea { get; set; }
        public double Weight { get; set; }
        public string rscStyle { get; set; }
        public int? Flag { get; set; }

        public int? CutSheetWid { get; set; }
        public int? CutSheetLeng { get; set; }

        public string CGType { get; set; }
        public string IsWrap { get; set; }
        public bool? IsNotch { get; set; }
        public int? NotchDegree { get; set; }
        public int? NotchDegreex { get; set; }
        public int? NotchArea { get; set; }
        public string NotchSide { get; set; }
        public int? SideA { get; set; }
        public int? SideB { get; set; }
        public int? SideC { get; set; }
        public double? SideD { get; set; }
        public int? No_Slot { get; set; }
    }

    public class RSCResultModel
    {
        public int? ScoreW1 { get; set; }
        public int? Scorew2 { get; set; }
        public int? Scorew3 { get; set; }
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

        public int? CutSheetLeng { get; set; }
        public int? CutSheetWid { get; set; }
        public int? SheetArea { get; set; }
        public int? BoxArea { get; set; }
        public double? WeightSh { get; set; }
        public double? WeightBox { get; set; }

        public int? widHC { get; set; }
        public int? lenHC { get; set; }
        public int? stretch { get; set; }
        public double? shrink { get; set; }

        public int? NotchArea { get; set; }
    }
}
