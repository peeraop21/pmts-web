using System;

namespace PMTs.DataAccess.ModelView.MaintenanceProductType
{
    public class ProductTypeModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool? Status { get; set; }
        public int? SortIndex { get; set; }
        public string HierarchyLv2 { get; set; }
        public string UnitDesc { get; set; }
        public string FormGroup { get; set; }
        public int? BoardMoDisplay { get; set; }
        public bool? IsTwoPiece { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public bool BoxHandle { get; set; }

        //Check Box
        public bool SelectStatus { get; set; }
        public bool SelectStatusValue { get; set; }
    }
}
