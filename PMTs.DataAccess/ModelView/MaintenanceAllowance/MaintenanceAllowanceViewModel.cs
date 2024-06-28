using System;
using System.Collections.Generic;

namespace PMTs.DataAccess.ModelView.MaintenanceAllowance
{
    public class MaintenanceAllowanceViewModel
    {
        public IEnumerable<AllowanceViewModel> AllowanceViewModelList { get; set; }
        public AllowanceViewModel AllowanceViewModel { get; set; }
    }

    public class AllowanceViewModel
    {
        public int Id { get; set; }
        public string FactoryCode { get; set; }
        public string Machine { get; set; }
        public int Range { get; set; }
        public double Percen { get; set; }
        public int SheetMin { get; set; }
        public int SheetMax { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }

}
