using PMTs.DataAccess.Models;

namespace PMTs.DataAccess.ComplexModel
{
    public class FluteAndMachineModel : Machine
    {
        public string Flute { get; set; }
        public string DescriptionDisplay { get; set; }
        public string FluteAndMachine { get; set; }
        public int? Trim { get; set; }
    }
}
