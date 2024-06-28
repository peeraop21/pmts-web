using System;
using System.Collections.Generic;

namespace PMTs.DataAccess.ModelView.MaintenanceJoint
{
    public class MaintenanceJointViewModel
    {
        public IEnumerable<JointViewModel> JointViewModelList { get; set; }
        public JointViewModel JointViewModel { get; set; }
    }

    public class JointViewModel
    {
        public int Id { get; set; }
        public string JointId { get; set; }
        public string JointName { get; set; }
        public string JointDescription { get; set; }
        public string UnitDesc { get; set; }
        public string KiwiJoinCodeInner { get; set; }
        public string KiwiJoinCodeOuter { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }


}
