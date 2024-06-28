using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView.AutoPackingCustomer;
using System.Collections.Generic;

namespace PMTs.DataAccess.ModelView.AutoPackingSpec
{
    public class AutoPackingSpecMainModel
    {
        public List<AutoPackingSpecViewModel> AutoPackingSpecs { get; set; }
        public Models.AutoPackingSpec AutoPackingSpec { get; set; }
        public Models.AutoPackingCustomer AutoPackingCustomer { get; set; }
        public List<AutoPackingConfig> AutoPackingConfigs { get; set; }
        public List<AutoPackingCustomerData> CustomerViewModelList { get; set; }
    }
}
