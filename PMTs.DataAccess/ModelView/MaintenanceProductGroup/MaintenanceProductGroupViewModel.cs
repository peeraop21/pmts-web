using System;
using System.Collections.Generic;

namespace PMTs.DataAccess.ModelView.MaintenanceProductGroup
{
    public class MaintenanceProductGroupViewModel
    {
        public IEnumerable<ProductGroupViewModel> ProductGroupViewModelList { get; set; }
        public ProductGroupViewModel ProductGroupViewModel { get; set; }
    }

    public class ProductGroupViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }


}
