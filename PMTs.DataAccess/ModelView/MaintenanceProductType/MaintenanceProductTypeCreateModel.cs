using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PMTs.DataAccess.ModelView.MaintenanceProductType
{
    public class MaintenanceProductTypeCreateModel
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "The field UID must be more than zero.")]
        public int UID { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required]
        [MaxLength(100)]
        public string Description { get; set; }
        public bool Status { get; set; }
        public int? SortIndex { get; set; }
        public string HierarchyLv2 { get; set; }
        public string FormGroup { get; set; }
        public string CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public bool BoxHandle { get; set; }
        public bool IsTwoPiece { get; set; }
        public string UnitDesc { get; set; }

        public List<SelectListItem> SelectFormGroups { get; set; }
        public List<SelectListItem> SelectHierarchyLv2s { get; set; }
        public List<string> HierarchyLv2s { get; set; }
    }
}
