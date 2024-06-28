using System;

namespace PMTs.DataAccess.ModelView.Report
{
    public class CheckRepeatOrder
    {
        public string FactoryCode { get; set; }
        public string OrderItem { get; set; }
        public string MaterialNo { get; set; }
        public string PC { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public int OrderQuant { get; set; }
        public DateTime OriginalDueDate { get; set; }
        public DateTime DueDate { get; set; }
        public int? TargetQuant { get; set; }
        public string PoNo { get; set; }
        public int repeatCount { get; set; }
        public string ItemNote { get; set; }
        public string Batch { get; set; }
        public string MOStatus { get; set; }
        public string DateTimeStamp { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
