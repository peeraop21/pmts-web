using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.ComplexModel
{
    public class CustomerShipToViewModel
    {
        public int Id { get; set; }
        public string SaleOrg { get; set; }
        public string Plant { get; set; }
        public string CustName { get; set; }
        public string CusCode { get; set; }
        public string Cust { get; set; }
        public string SoldToCode { get; set; }
        public string CustId { get; set; }
        public string Zone { get; set; }
        public string Route { get; set; }
        public string CustShipTo { get; set; }
        public string CustDeliveryTime { get; set; }
        public string CustClass { get; set; }
        public string CustReq { get; set; }
        public string CustAlert { get; set; }
        public bool? CustStatus { get; set; }
        public List<CustShipTo> ShipTo { get; set; }
    }
}
