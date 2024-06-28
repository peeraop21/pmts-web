using System;
using System.Collections.Generic;

namespace PMTs.DataAccess.ModelView.MaintenanceBuildRemark
{
    public class MaintenanceBuildRemarkViewModel
    {
        public IEnumerable<BuildRemarkViewModel> BuildRemarkViewModelList { get; set; }
        public BuildRemarkViewModel BuildRemarkViewModel { get; set; }
    }

    public class BuildRemarkViewModel
    {
        public int Id { get; set; }
        public string FactoryCode { get; set; }
        public string Machine { get; set; }
        public string List { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }


}
