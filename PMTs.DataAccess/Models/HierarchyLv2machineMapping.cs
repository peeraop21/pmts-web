using System;

namespace PMTs.DataAccess.Models
{
    public partial class HierarchyLv2machineMapping
    {
        public int Id { get; set; }
        public string HierarchyLv2 { get; set; }
        public string MachineGroup { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdateBy { get; set; }
    }
}
