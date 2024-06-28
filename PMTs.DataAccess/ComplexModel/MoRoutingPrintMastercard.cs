using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.ComplexModel
{
    public class MoRoutingPrintMastercard : MoRouting
    {
        public List<Coating> Coatings { get; set; }
        public string Color9 { get; set; }
        public string Shade9 { get; set; }
        public string Color10 { get; set; }
        public string Shade10 { get; set; }
        public string QualitySpecs { get; set; }
        public string MachineDetail { get; set; }
        public string MachineColorDetail { get; set; }
    }
}
