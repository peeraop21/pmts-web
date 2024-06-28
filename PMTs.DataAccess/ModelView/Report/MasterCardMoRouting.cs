using PMTs.DataAccess.ComplexModel;

namespace PMTs.DataAccess.ModelView.Report
{
    public class MasterCardMoRouting
    {
        public MoRoutingPrintMastercard Routing { get; set; }
        public string MachineGroup { get; set; }
        public bool MachineIsCalPaperWidth { get; set; }
        public bool IsProp_Cor { get; set; }
    }
}