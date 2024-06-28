using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.ModelView
{
    public class BoardViewModel
    {
        public List<BoardAlternative> BoardAlt { get; set; }
        public int Id { get; set; }
        public string SaleOrg { get; set; }
        public string PlantCode { get; set; }
        public string Code { get; set; }
        public string Flute { get; set; }
        public string Board { get; set; }
        public string SearchBoard { get; set; }
        public string BoardKiwi { get; set; }
        public decimal? Weight { get; set; }

        public int? A { get; set; }
        public int? B { get; set; }
        public int? C { get; set; }
        public int? D1 { get; set; }
        public int? D2 { get; set; }
        public int? JoinSize { get; set; }
        public decimal? CostPerTon { get; set; }
        public string Hierarchy { get; set; }
        public string PaperDes { get; set; }
        public string Station { get; set; }
        public int Priority { get; set; }
        public bool Active { get; set; }
        public string GL { get; set; }
        public string BM { get; set; }
        public string BL { get; set; }
        public string CM { get; set; }
        public string CL { get; set; }
        public string DM { get; set; }
        public string DL { get; set; }

        public string MaterialNo { get; set; }
        public string BoardName { get; set; }

        public bool? TwoPiece { get; set; }
        public int? Wid { get; set; }
        public int? Leg { get; set; }
        public int? Hig { get; set; }
        public int? CutSheetLeng { get; set; }
        public int? CutSheetWid { get; set; }
        public int? SheetArea { get; set; }
        public int? BoxArea { get; set; }
        public int? ScoreW1 { get; set; }
        public int? Scorew2 { get; set; }
        public int? Scorew3 { get; set; }
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
        public decimal? WeightSh { get; set; }
        public decimal? WeightBox { get; set; }
    }

    public class SaveProductSpecViewModel
    {
        public string Code { get; set; }
        public string Flute { get; set; }
        public string Board { get; set; }
        public string Hierarchy { get; set; }

        public bool? TwoPiece { get; set; }
        public int? Wid { get; set; }
        public int? Leg { get; set; }
        public int? Hig { get; set; }
        public int? CutSheetLeng { get; set; }
        public int? CutSheetWid { get; set; }
        public int? SheetArea { get; set; }
        public int? BoxArea { get; set; }
        public int? ScoreW1 { get; set; }
        public int? Scorew2 { get; set; }
        public int? Scorew3 { get; set; }
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
        public decimal? WeightSh { get; set; }
        public decimal? WeightBox { get; set; }
    }
}
