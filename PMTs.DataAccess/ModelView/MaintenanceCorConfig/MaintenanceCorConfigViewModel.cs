using System;
using System.Collections.Generic;

namespace PMTs.DataAccess.ModelView.MaintenanceCorConfig
{
    public class MaintenanceCorConfigViewModel
    {
        public IEnumerable<CorConfigViewModel> CorConfigViewModelList { get; set; }
        public CorConfigViewModel CorConfigViewModel { get; set; }
    }

    public class CorConfigViewModel
    {
        public int Id { get; set; }
        public string FactoryCode { get; set; }
        public string Name { get; set; }
        public int MinOut { get; set; }
        public int MaxOut { get; set; }
        public int CutOff { get; set; }
        public bool Mintrim { get; set; }
        public int TearTape { get; set; }
        public int TearTapeMax { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }


}
