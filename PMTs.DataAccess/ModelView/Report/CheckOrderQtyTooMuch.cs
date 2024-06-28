using System;

namespace PMTs.DataAccess.ModelView.Report
{
    public class CheckOrderQtyTooMuch
    {
        public string FactoryCode { get; set; }
        public string OrderItem { get; set; }
        public string MaterialNo { get; set; }
        public string PC { get; set; }
        public string Name { get; set; }
        public DateTime DueDate { get; set; }
        public int OrderQuant { get; set; }
        public int SumQty { get; set; }
        public int CountTime { get; set; }
        public int AvgQty { get; set; }
        public string Description { get; set; }
    }
}
