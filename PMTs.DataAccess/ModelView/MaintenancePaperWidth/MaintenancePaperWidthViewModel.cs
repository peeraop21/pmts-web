using System;
using System.Collections.Generic;

namespace PMTs.DataAccess.ModelView.MaintenancePaperWidth
{
    public class MaintenancePaperWidthViewModel
    {
        public IEnumerable<PaperWidthViewModel> PaperWidthViewModelList { get; set; }
        public PaperWidthViewModel PaperWidthViewModel { get; set; }
    }

    public class PaperWidthViewModel
    {
        public int Id { get; set; }
        public string FactoryCode { get; set; }
        public int Group1 { get; set; }
        public int? Group2 { get; set; }
        public int? Group3 { get; set; }
        public int? Group4 { get; set; }
        public int Width { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }


}
