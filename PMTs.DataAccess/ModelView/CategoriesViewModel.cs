using PMTs.DataAccess.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PMTs.DataAccess.ModelView
{
    public enum NameOfRSC
    {
        Standard = 1,
        Full_Overlap = 2,
        Tele_Top_Lid = 3,
        Tele_Bottom_Lid = 4,
        Sleeve = 5,
        Rotary = 6,
    };

    public class KPG
    {
        public int Uid { get; set; }
        public string Name { get; set; }
    }
    public class KOP
    {
        public int Uid { get; set; }
        public string Name { get; set; }
    }
    public class PCC
    {
        public int Uid { get; set; }
        public string Name { get; set; }
    }
    public class PDT
    {
        public int Uid { get; set; }
        public string Name { get; set; }
    }
    public class ViewCategories
    {
        public int Id_kProdGrp { get; set; }
        public string KpgName { get; set; }
        [Required]
        [DisplayName("kind of product")]
        public int Id_kProd { get; set; }
        [Required]
        [DisplayName("process cost")]
        public int Id_ProcCost { get; set; }
        [Required]
        [DisplayName("product type")]
        public int Id_ProdType { get; set; }
        public string ProductTypeName { get; set; }
        [Required]
        public int Id_MatType { get; set; }
        public string MatCode { get; set; }
        [Required]
        public int Id_PU { get; set; }
        [Required]
        public int Id_SU { get; set; }
        public string fscCode { get; set; }
        public string fscFgCode { get; set; }
        public string RpacLob { get; set; }
        public string RpacProgram { get; set; }
        public string RpacBrand { get; set; }
        public string RpacPackagingType { get; set; }
        public string HierarchyLV2 { get; set; }
        [Required]
        [DisplayName("hierarchy level 3")]
        public int hierarchyLV3Id { get; set; }
        public string HierarchyLV3 { get; set; }

        public int hierarchyLV4Id { get; set; }
        public string HierarchyLV4 { get; set; }

        public string FormGroup { get; set; }
        public int RSCStyleId { get; set; }
        public string RSCStyle { get; set; }
        public bool? IsTwoPiece { get; set; }

        //tassanai 25/9/62
        public bool BoxHandle { get; set; }

        public List<KindOfProductGroup> KindOfProductGroupList { get; set; }
        public List<ProcessCost> ProcessCostList { get; set; }
        public List<KindOfProduct> KindOfProductList { get; set; }
        public List<ProductType> ProductTypeList { get; set; }
        public List<MaterialType> MaterialTypeList { get; set; }
        public List<UnitMaterial> UnitMaterialList { get; set; }
        public List<HierarchyLv2Matrix> HierarchyLV2List { get; set; }
        public List<MapCost> MapcostList { get; set; }
        public List<HierarchyLv3> HierarchyLv3s { get; set; }
        public List<HierarchyLv4> HierarchyLv4s { get; set; }
        public List<PpcFscCode> FSCCodes { get; set; }
        public List<PpcFscFgCode> FSCFGCodes { get; set; }
        public List<PpcMasterRpac> PPCMasterRpacs { get; set; }
    }
}
