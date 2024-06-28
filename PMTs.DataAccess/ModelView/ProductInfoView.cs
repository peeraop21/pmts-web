using Microsoft.AspNetCore.Mvc.Rendering;
using PMTs.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PMTs.DataAccess.ModelView
{
    public class ProductInfoView
    {
        [Key]
        [Required]
        public string MaterialNo { get; set; }
        [Required]
        public string SALEORG { get; set; }
        [Required]
        public string PLANTCODE { get; set; }
        //[Required]
        public string PartNo { get; set; }
        public string PC { get; set; }
        //public string Hierarchy { get; set; }
        [Required]
        public string CustCode { get; set; }
        [Required]
        public string CusID { get; set; }
        [Required]
        public string CustName { get; set; }
        public string Description { get; set; }
        public string SaleText1 { get; set; }
        public string SaleText2 { get; set; }
        public string SaleText3 { get; set; }
        public string SaleText4 { get; set; }
        //[Required]
        //public string Change { get; set; }
        [Required]
        public string Language { get; set; }
        //public string IndGrp { get; set; }
        //public string IndDes { get; set; }
        ////public string Material_Type { get; set; }
        //public string PrintMethod { get; set; }
        //public bool TwoPiece { get; set; }
        ////public string Flute { get; set; }

        //public string JoinType { get; set; }


        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }

        public string HighValue { get; set; }
        public string HighGroup { get; set; }
        public string HighValueDisplay { get; set; }

        //public virtual HVL_ProdTypeModel HVL_ProdType { get; set; }
        //public virtual HVA_MIX_MASTERModel HVA_MIX_MASTER { get; set; }
        //public virtual HVA_MIXModel HVA_MIX { get; set; }

        public string HvaProductType { get; set; }
        public string HvaStructural { get; set; }
        public string HvaPrinting { get; set; }
        public string HvaFlute { get; set; }
        public string HvaCorrugating { get; set; }
        public string HvaCoating { get; set; }
        public string HvaFinishing { get; set; }

        //public string hvacode { get; set; }

        public IEnumerable<HvaMaster> HvaMasters { get; set; }

        public IEnumerable<SelectListItem> HVL_ProdTypeList1 { get; set; }
        public IEnumerable<SelectListItem> HVL_ProdTypeList2 { get; set; }
        public IEnumerable<SelectListItem> HVL_ProdTypeList3 { get; set; }
        public IEnumerable<SelectListItem> HVL_ProdTypeList4 { get; set; }
        public IEnumerable<SelectListItem> HVL_ProdTypeList5 { get; set; }
        public IEnumerable<SelectListItem> HVL_ProdTypeList6 { get; set; }
        public IEnumerable<SelectListItem> HVL_ProdTypeList7 { get; set; }

        public List<Outsource> Outsources { get; set; }
        public List<CompanyProfile> CompanyProfiles { get; set; }
        public List<HireOrder> HireOrders { get; set; }
        public int? OrderTypeId { get; set; }
        public string MatOursource { get; set; }
        public string MatTypeOursource { get; set; }

        public string HvaCorrugatingDescription { get; set; }
        public string MaterialSaleOrg { get; set; }
    }

    public class Outsource
    {
        public string PlantOs { get; set; }
        public int? HireOrderType { get; set; }
        public string SaleOrg { get; set; }
        public string ShortName { get; set; }
        public bool IsOutsource { get; set; }
    }
}
