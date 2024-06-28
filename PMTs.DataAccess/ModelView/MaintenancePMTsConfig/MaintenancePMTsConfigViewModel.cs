using System;
using System.Collections.Generic;

namespace PMTs.DataAccess.ModelView.MaintenancePMTsConfig
{
    public class MaintenancePMTsConfigViewModel
    {
        public IEnumerable<PMTsConfigViewModel> PMTsConfigViewModelList { get; set; }
        public PMTsConfigViewModel PMTsConfigViewModel { get; set; }
    }

    public class PMTsConfigViewModel
    {
        public int Id { get; set; }
        public string FactoryCode { get; set; }
        public string FucName { get; set; }
        public string FucValue { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }


}
