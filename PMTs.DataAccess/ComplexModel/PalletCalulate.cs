namespace PMTs.DataAccess.ComplexModel
{
    public class PalletCalulate
    {
        public string PicPallet { get; set; }
        public int? BunLayer { get; set; }

    }
    public class PalletCalculateParam
    {
        public string FormGroup { get; set; }
        public string RSCStyle { get; set; }
        public string Flute { get; set; }
        //public int? A { get; set; }
        //public int? B { get; set; }
        //public int? C { get; set; }
        //public int? D1 { get; set; }
        //public double? Thickness { get; set; }
        public int WidDC { get; set; }
        public int LegDC { get; set; }
        public int? Hig { get; set; }
        public string palletSizeFilter { get; set; }
        public int? Overhang { get; set; }
        public int? CutSheetWid { get; set; }
        public int? CutSheetLeng { get; set; }
        public string JoinTypeFilter { get; set; }
        public int? ScoreL6 { get; set; }



    }
}
