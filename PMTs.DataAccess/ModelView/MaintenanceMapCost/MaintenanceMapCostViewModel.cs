using Microsoft.AspNetCore.Mvc.Rendering;
using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.ModelView.MaintenanceMapCost
{
    public class MaintenanceMapCostViewModel
    {
        public string Id { get; set; }
        public string Hierarchy2 { get; set; }
        public string Hierarchy3 { get; set; }
        public string Hierarchy4 { get; set; }

        public string CostField { get; set; }
        public string Description { get; set; }

        public List<MapCost> MapCosts { get; set; }
        public IEnumerable<SelectListItem> Hierarchy2SelectList { get; set; }
        public IEnumerable<SelectListItem> Hierarchy3SelectList { get; set; }
        public IEnumerable<SelectListItem> Hierarchy4SelectList { get; set; }

        public IEnumerable<HierarchyLv3> Hierarchy3s { get; set; }
        public IEnumerable<HierarchyLv4> Hierarchy4s { get; set; }
    }
}
