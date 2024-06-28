using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.ModelView
{
    public class UpdateMaterialViewModel
    {
        public List<MasterData> MasterDatas { get; set; }
        public List<Routing> Routings { get; set; }
    }
}
