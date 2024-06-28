using PMTs.DataAccess.ComplexModel;
using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.ModelView.Report
{
    public class SaleOrderModel
    {
        public SaleOrderModel()
        {
            modelRouting = new RoutingViewModel();
            MasterRoutings = new List<MasterDataRoutingModel>();
        }
        public string OrderItem { get; set; }

        public MoSpec moSpec { get; set; }
        public List<MoSpec> moSpecs { get; set; }
        public MoRouting moRouting { get; set; }
        public List<MoRouting> moRoutings { get; set; }
        public string OrderSelect { get; set; }

        public List<MasterDataRoutingModel> MasterRoutings { get; set; }

        #region [JS Requi]
        //Find in MoRouting
        public int? amountColor { get; set; }
        //MoSpect  BoxType = Name  Productype
        public string FormGroup { get; set; }
        //Material_Type in MoSpec
        public string MatCode { get; set; }
        //Set = ""
        public string RealEventFlag { get; set; }
        public string MaterialNo { get; set; }
        //UnUpgrade_Board in MoRouting
        public string UnUpgradBoard { get; set; }
        //Transaction_Detail
        public string GLTail { get; set; }
        //Slit in MoSpec
        public string Slit { get; set; }
        public string Flute { get; set; }
        public string EventFlag { get; set; }
        public string PLANTCODE { get; set; }
        public string PlantOs { get; set; }
        public string BoxHandle { get; set; }
        #endregion

        #region [Routing]  
        public RoutingViewModel modelRouting { get; set; }
        public List<string> modelGroupMachineRemark { get; set; }
        public IEnumerable<BuildRemark> modelBuildRemark { get; set; }
        public string arrayPrint { get; set; }
        public string arrayDiecut { get; set; }
        #endregion

    }
}
