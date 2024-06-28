using PMTs.DataAccess.ComplexModel;
using System.Collections.Generic;

namespace PMTs.DataAccess.ModelView.Report
{
    public class PrintMastercardViewModel
    {
        public List<MasterDataRoutingModel> MasterDataRoutingModels { get; set; }
        public string ErrorSearchOrderItems { get; set; }
    }
}
