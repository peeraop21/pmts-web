using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.ModelView
{
    public class BOMViewModel
    {
        public string ParentMaterialNo { get; set; }
        public string MaterialNo { get; set; }
        public string Follower { get; set; }
        public string AbbservName { get; set; }
        public string ProductCode { get; set; }
        public string Pieces { get; set; }
        public string Weight { get; set; }

        public string GroupCompany { get; set; }


        public List<MasterData> lstMasterData { get; set; }
        public List<BomStruct> lstBomStructs { get; set; }

        public MasterData masterData { get; set; }
        public BomStruct bomStruct { get; set; }

        public List<Plants> plants { get; set; }
    }

    public class Plants
    {
        public string Plant { get; set; }
        public string Desc { get; set; }
    }
}
