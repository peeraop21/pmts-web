using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.ModelView
{
    public class ProductSpecViewModel
    {
        public List<BoardAltViewModel> BoardAlt { get; set; }
        public List<SearchBoardAlt> BoardLists { get; set; }
        public List<double?> DLists { get; set; }
        public List<int?> HLists { get; set; }
        public List<int?> CellSizeLists { get; set; }
        public List<SearchBoardAlt> SearchBoardAltLists { get; set; }
        public List<FluteTr> StationList { get; set; }
        public List<Coating> Coating { get; set; }
        public List<Additive> Additive { get; set; }
        public List<CoatingViewModel> CoatingTable { get; set; }
        public IEnumerable<SelectListItem> ListCoatingType { get; set; }
        public IEnumerable<SelectListItem> ListCoatingName { get; set; }

        public int Id { get; set; }
        public string SaleOrg { get; set; }
        public string PlantCode { get; set; }
        public string Code { get; set; }
        public string Code1 { get; set; }
        public string Flute { get; set; }
        public string Board { get; set; }
        public string BoardCombine { get; set; }
        public string BoardKIWI { get; set; }
        public double Weight { get; set; }

        public int? A { get; set; }
        public int? B { get; set; }
        public int? C { get; set; }
        public int? D1 { get; set; }
        public int? D2 { get; set; }
        public int? JoinSize { get; set; }
        public int? Height { get; set; }
        public decimal? CostPerTon { get; set; }
        public string Hierarchy { get; set; }
        public string PaperDes { get; set; }
        public string Station { get; set; }
        public int Priority { get; set; }
        public int chkPrior { get; set; }
        public bool Active { get; set; }
        public string GL { get; set; }
        public string BM { get; set; }
        public string BL { get; set; }
        public string CM { get; set; }
        public string CL { get; set; }
        public string DM { get; set; }
        public string DL { get; set; }

        public string stt1 { get; set; }
        public string stt2 { get; set; }
        public string stt3 { get; set; }
        public string stt4 { get; set; }
        public string stt5 { get; set; }
        public string stt6 { get; set; }
        public string stt7 { get; set; }

        public string MaterialNo { get; set; }
        public string BoardName { get; set; }

        public int Size2Piece { get; set; }
        public bool? TwoPiece { get; set; }
        public bool? GLWid { get; set; }
        public bool? GLTail { get; set; }
        public bool? StdPic { get; set; }
        public bool? CapImg { get; set; }
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
        public double? WeightSh { get; set; }
        public double? WeightBox { get; set; }
        public double? WeightBoxInit { get; set; }
        public int? CutSheetWidInit { get; set; }
        public int FlagRouting { get; set; }
        public bool? UnUpgradBoard { get; set; }
        public decimal? CutSheetWidInch { get; set; }
        public decimal? CutSheetLengInch { get; set; }
        public int spcLen { get; set; }
        public int? widHC { get; set; }
        public int? lenHC { get; set; }
        public int? stretch { get; set; }
        public double? shrink { get; set; }

        public string PrintMaster { get; set; }
        public string PrintMasterPath { get; set; }

        public string action { get; set; }
        public string rscStyle { get; set; }
        public int? LayerPallet { get; set; }
        public int? Flag { get; set; }
        public bool? SAPStatus { get; set; }

        public int kgLength { get; set; }
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
        public string lv2 { get; set; }
        public string lv3 { get; set; }
        public string lv4 { get; set; }
        public string costField { get; set; }

        public decimal? CostOEM { get; set; }

        [HiddenInput]
        public string DimensionPropertiesImageBase64String { get; set; }

        //Nattha
        public int? Perforate1 { get; set; }
        public int? Perforate2 { get; set; }
        public int? Perforate3 { get; set; }
        public int? Perforate4 { get; set; }
        public int? Perforate5 { get; set; }
        public int? Perforate6 { get; set; }
        public int? Perforate7 { get; set; }
        public int? Perforate8 { get; set; }
        public int? Perforate9 { get; set; }
        public int? Perforate10 { get; set; }
        public int? Perforate11 { get; set; }
        public int? Perforate12 { get; set; }
        public int? Perforate13 { get; set; }
        public int? Perforate14 { get; set; }
        public int? Perforate15 { get; set; }
        public int? Perforate16 { get; set; }
        public int? PerforateGap { get; set; }
    }

    public class Tempcoating
    {
        public string Station { get; set; }
        public string Layer { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
    }
}
