using System;

namespace PMTs.DataAccess.Models
{
    public partial class HighDetail
    {
        public int Id { get; set; }
        public string HiGroup { get; set; }
        public string HiCode { get; set; }
        public string Description { get; set; }
        public DateTime? CreateDate { get; set; }
        public bool? Active { get; set; }
    }
}
