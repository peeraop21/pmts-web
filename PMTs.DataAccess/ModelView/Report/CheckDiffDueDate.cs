using System;

namespace PMTs.DataAccess.ModelView.Report
{
    public class CheckDiffDueDate
    {
        public string FactoryCode { get; set; }
        public string OrderItem { get; set; }
        public string MaterialNo { get; set; }
        public string PC { get; set; }
        public string Name { get; set; }
        public string BoxType { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime MaxDue { get; set; }
        public DateTime CreatedDate { get; set; }
        public int diff { get; set; }
    }
}
