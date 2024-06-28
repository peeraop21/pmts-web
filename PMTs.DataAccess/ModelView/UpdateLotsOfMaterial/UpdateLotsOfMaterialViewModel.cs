using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace PMTs.DataAccess.ModelView.UpdateLotsOfMaterial
{
    public class UpdateLotsOfMaterialViewModel
    {
        public List<SelectListItem> CompanyProfiles { get; set; }
        public MasterDataViewModel MasterData { get; set; }
        public List<MasterDataViewModel> MasterDatas { get; set; }
        public string factoryCode { get; set; }
    }
}
