using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.ModelView.ManageMO
{
    public class ManageMOViewModel
    {
        public MoDataViewModel MoData { get; set; }
        public List<AttachFileMo> AttachFileMOs { get; set; }
        public List<MoDataViewModel> MoDatas { get; set; }
        public bool IsAttachFilePage { get; set; }
        public List<SearchTypeModel> SearchTypes { get; set; }
    }
}
