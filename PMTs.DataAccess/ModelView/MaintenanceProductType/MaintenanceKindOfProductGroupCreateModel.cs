using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.ModelView.MaintenanceProductType
{
    public class MaintenanceKindOfProductGroupCreateModel
    {
        public List<KindOfProductGroup> KindOfProductGroups { get; set; }
        public List<KindOfProduct> KindOfProducts { get; set; }
        public List<ProcessCost> ProcessCosts { get; set; }
        public List<ProductTypeModel> ProductTypes { get; set; }
        public List<HierarchyLv2Matrix> HierarchyLv2Matrices { get; set; }

        public List<SelectListItem> SelectFormGroups { get; set; }
        public List<SelectListItem> SelectHierarchyLv2s { get; set; }
        public List<string> HierarchyLv2s { get; set; }

        public int KindOfProductGroupID { get; set; }
        public int KindOfProductID { get; set; }
        public int ProcessCostID { get; set; }
        public string HierarchyLv2 { get; set; }
    }

    //ProductTypeArr Model
    public class ProductTypeArr
    {
        [JsonProperty("Id")]
        public string Id { get; set; }
        [JsonProperty("Status")]
        public string Status { get; set; }
    }
}
