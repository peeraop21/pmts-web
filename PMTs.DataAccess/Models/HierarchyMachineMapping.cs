using System;

namespace PMTs.DataAccess.Models
{
    public partial class HierarchyMachineMapping
    {
        public int Id { get; set; }
        public string HierarchyLv2 { get; set; }
        public string MachineGroup { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public bool? Status { get; set; }
    }
}
