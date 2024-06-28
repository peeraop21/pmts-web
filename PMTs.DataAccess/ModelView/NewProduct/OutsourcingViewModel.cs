using PMTs.DataAccess.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PMTs.DataAccess.ModelView.NewProduct
{
    public class OutsourcingViewModel
    {
        public List<CompanyProfile> CompanyProfiles { get; set; }
        public string Plant { get; set; }
        //public List<MasterDataRoutingModel> MasterDataRoutingModels { get; set; }
        public List<HireMapping> HireMappings { get; set; }
        public List<HireOrder> HireOrders { get; set; }
        public string FactoryLogon { get; set; }

        #region Form model for
        [Required]
        [StringLength(10, ErrorMessage = "The Material No ", MinimumLength = 10)]
        public string MaterialNo { get; set; }
        public string Action { get; set; }
        public string SaleOrg { get; set; }
        public int? OrderTypeId { get; set; }
        #endregion
    }
}
