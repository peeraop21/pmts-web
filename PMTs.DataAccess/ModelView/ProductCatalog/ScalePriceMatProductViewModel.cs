using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.ModelView.ProductCatalog
{
    public class ScalePriceMatProductViewModel
    {
        public List<ScalePriceMatProductModel> scalePriceMatProductModels { get; set; }
        public List<MaterialType> MaterialTypeList { get; set; }
        public List<PlantModel> Plants { get; set; }
    }
}
