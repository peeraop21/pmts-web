using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using PMTs.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PMTs.DataAccess.ModelView
{

    public class ProductPropViewModel
    {
        public int Id { get; set; }
        public string MaterialNo { get; set; }
        public string TypeProp { get; set; }
        public string TabProp { get; set; }
        public int? CutSheetWid { get; set; }
        public int? CutSheetLeng { get; set; }




        // public IEnumerable<SelectListItem> JoinList { get; set; }
        public string JoinId { get; set; }
        public string JoinType { get; set; }


        public string IdNameJoinCharacter { get; set; }
        public string NameJoinCharacter { get; set; }


        //public IEnumerable<SelectListItem> PrintMethodList { get; set; }
        public List<PrintMethodViewModel> PrintMethodList { get; set; }


        //public List<PrintMethod> PrintMethodList { get; set; }
        [Required]
        public string PrintMethod { get; set; }
        public int? AmountColor { get; set; }
        public string PrintMethoddata { get; set; }
        public int? Wire { get; set; }

        public bool OuterJoin { get; set; }

        public IEnumerable<SelectListItem> PalletList { get; set; }

        public string PalletSize { get; set; }

        public int? Bun { get; set; }
        //[Range(0, 999.99)]
        //public string Bun { get; set; }
        public int? BunLayer { get; set; }
        public int? LayerPallet { get; set; }
        [Display(Name = "BoxPallet")]
        public int? BoxPalet { get; set; }
        public string PicPallet { get; set; }
        public int? PieceSet { get; set; }
        public int? PiecePatch { get; set; }
        public bool BoxHandle { get; set; }
        public int? SparePercen { get; set; }
        public int? SpareMax { get; set; }
        public int? SpareMin { get; set; }
        public int? LeadTime { get; set; }
        public int? Hardship { get; set; }
        public int? RunSpeed { get; set; }
        public int? SetupTime { get; set; }
        public int? PrepareTime { get; set; }
        public int? PostTime { get; set; }
        public int? SetupWaste { get; set; }
        public int? RunWaste { get; set; }
        public int? SheetStack { get; set; }
        public int? RotationIn { get; set; }
        public int? Rotationout { get; set; }
        public int? ScoreType { get; set; }
        public int? ScoreGap { get; set; }
        public string ChangeInfo { get; set; }
        public string Change { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public bool Status { get; set; }
        public string EANCODE { get; set; }

        public int? BoxPerBundleNoJoint { get; set; }
        public int? LayerPerPalletNoJoint { get; set; }

        public string picgetPallet { get; set; }

        public IFormFile MyImage { get; set; }

        //Tassanai update
        public double Thickness { get; set; }
        public int PalletOverhang { get; set; }

        //Tassanai Update 29/8/62
        public string PathpalletSuggess { get; set; }
        public int Wid { get; set; }
        public int Leg { get; set; }
        public string PathPallet { get; set; }

        //Tassanai 24/09/82
        public string StatusFlag { get; set; }

        //Tassanai update 11/03/2020
        public List<JointViewModel> JoinList { get; set; }
        public List<ProductPropStandardPatternName> StandardPatternNameList { get; set; }
        public List<PpcBoiStatus> PpcBoiStatusList { get; set; }
        public string BOIStatus { get; set; }

        public List<PpcWorkType> PpcWorkTypeList { get; set; }
        public string WorkType { get; set; }

        //vmi
        public string CustInvType { get; set; }
        public string CIPInvType { get; set; }

        ////Tassanai update 30/07/2020
        //public string FGMaterial { get; set; }

        //Tassanai update 23/11/2020
        public string TopSheetMaterial { get; set; }

        //Tassanai Update 11/01/2020
        public List<JoinCharacterViewModel> JoinCharacterList { get; set; }

        //Tassanai Update 23/02/2020
        public bool NoneStandardPaper { get; set; }

        //Tassanai Update 13/05/2022
        public int? BoxPacking { get; set; }

    }



    public class ProductPropChangeHisViewModel
    {
        public int ID { get; set; }
        public string Material_No { get; set; }
        public string ChangeInfo { get; set; }
        public string ChangeHistoryText { get; set; }
        public DateTime CREATEDDATE { get; set; }
        public string CREATEDBY { get; set; }
        public DateTime UPDATEDDATE { get; set; }
        public string UPDATEDBY { get; set; }
        public bool? STATUS { get; set; }

        public List<ChangeHistory> ModelChangeList { get; set; }
    }

    public class ColumnPalletModel
    {
        public string Type { get; set; }
        public string LxW { get; set; }
        public int BundlePerLayyer { get; set; }
        public int L { get; set; }
        public int W { get; set; }
        public int L1 { get; set; }
        public int W1 { get; set; }
        public int L2 { get; set; }
        public int W2 { get; set; }
        public int L1_L1 { get; set; }
        public int W1_W1 { get; set; }
        public int L1_W1 { get; set; }
        public int W1_L1 { get; set; }
        public int L2_L2 { get; set; }
        public int W2_W2 { get; set; }
        public int L2_W2 { get; set; }
        public int W2_L2 { get; set; }
        public int CartonPerLayer { get; set; }







    }

    public class PalletresultModel
    {
        public string formatPalletName { get; set; }
        public string typePalletName { get; set; }
        public int qtycartonPerLayer { get; set; }
    }


}



