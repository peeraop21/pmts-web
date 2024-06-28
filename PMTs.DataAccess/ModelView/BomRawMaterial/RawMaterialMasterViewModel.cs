using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.ModelView.BomRawMaterial
{
    public class RawMaterialMasterViewModel
    {
        public PpcRawMaterialMaster PpcRawMaterialMaster { get; set; }
        public List<PpcRawMaterialMaster> PpcRawMaterialMasters { get; set; }
    }
}
