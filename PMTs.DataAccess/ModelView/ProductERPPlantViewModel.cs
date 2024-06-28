using Microsoft.AspNetCore.Mvc.Rendering;
using PMTs.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PMTs.DataAccess.ModelView
{

    public class ProductERPPlantViewModel
    {
        public string Material_No { get; set; }
        public IEnumerable<SelectListItem> PlantList { get; set; }
        [Required]
        public string Plant { get; set; }
        [Required]
        public string PurchCode { get; set; }
        public string ShipBlk { get; set; }
        [Required]
        public double? StdtotalCost { get; set; }
        public double? StdVC { get; set; }
        public double? StdFC { get; set; }
        public double? StdMovingCost { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string PDISStatus { get; set; }
        public bool TranStatus { get; set; }
        public string SAPStatus { get; set; }

        public List<PlantView> ModelList { get; set; }
    }

    public class ProductERPSaleViewModel
    {
        public string Material_No { get; set; }
        [Required]
        public string SaleOrg { get; set; }
        [Required]
        public byte Channel { get; set; }
        public string CustCode { get; set; }
        [Required]
        public IEnumerable<SelectListItem> SaleOrgList { get; set; }

        public string DevPlant { get; set; }
        [Required]
        [RegularExpression("([0-9]+)", ErrorMessage = "Please enter valid Number")]
        public int? MinQty { get; set; }
        public double? SaleUnitPrice { get; set; }
        [Required]
        public string OrderType { get; set; }
        public double? PriceAdj { get; set; }
        public double? OldPrice { get; set; }
        public double? NewPrice { get; set; }
        public DateTime? Effective { get; set; }
        public DateTime? ChangeDate { get; set; }
        public string PdisStatus { get; set; }
        public bool TranStatus { get; set; }
        public bool SapStatus { get; set; }
        public List<SalesView> ModelListSale { get; set; }
    }

    public class ProductERPPurchaseViewModel
    {
        public string Material_No { get; set; }
        public string PurTxt1 { get; set; }
        public string PurTxt2 { get; set; }
        public string PurTxt3 { get; set; }
        public string PurTxt4 { get; set; }
    }
}



