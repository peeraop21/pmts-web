using System.Collections.Generic;

namespace PMTs.DataAccess.ComplexModel
{
    public class E_Ordering
    {
        public string requestId { get; set; }
        public List<Material> materialList { get; set; }
    }

    public class Material
    {
        public string materialNo { get; set; }
        public bool? isHold { get; set; }
        public string HoldRemark { get; set; }
    }
}