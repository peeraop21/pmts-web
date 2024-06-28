using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.ModelView
{
    public class ChangePalletSizeViewModel
    {
        public List<MasterData> MasterDatas { get; set; }
        public MasterData MasterData { get; set; }
        public List<StandardPatternName> StandardPatternNames { get; set; }

    }
}
