using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.ModelView.AutoPackingCustomer
{
    public class AutoPackingCustomerData : Models.AutoPackingCustomer
    {
        public int Id { get; set; }
        public string SaleOrg { get; set; }
        public string PlantCode { get; set; }
        public string CustName { get; set; }
        public string SoldToCode { get; set; }
        public string CustCode { get; set; }
        public string IndGrp { get; set; }
        public string Zone { get; set; }
        public string Route { get; set; }
        public string CustShipTo { get; set; }
        public string CustDeliveryTime { get; set; }
        public string CustClass { get; set; }
        public string CustReq { get; set; }
        public string CustAlert { get; set; }
        public bool? CustStatus { get; set; }
        public int? PalletOverhang { get; set; }
        public List<ProductGroup> ProductGroupList { get; set; }
        public List<CustShipTo> CustShipToList { get; set; }
        public string CustShipToJsonList { get; set; }
        public string QASpec { get; set; }
        public string Accgroup { get; set; }
        public string Cust { get; set; }
        public int? PriorityFlag { get; set; }
        public bool HasAutoPackingCustomer { get; set; }
    }
}
