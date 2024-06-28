using System;

namespace PMTs.DataAccess.ModelView.ProductCatalog
{
    public class ScalePriceMatProductModel
    {
        public string MaterialNo { get; set; }
        public string Pc { get; set; }
        public string PartNo { get; set; }
        public string CustCode { get; set; }
        public string CusId { get; set; }
        public string CustName { get; set; }
        public string SaleText1 { get; set; }
        public string Flute { get; set; }
        public string Board { get; set; }
        public int? Wid { get; set; }
        public int? Leg { get; set; }
        public int? Hig { get; set; }
        public int? ScaleQty { get; set; }
        public double? Rate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? NetPrice { get; set; }
    }
}
