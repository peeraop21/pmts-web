using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.ModelView.Report
{
    public class ReportCheckMOAndTextfileSAPViewModel
    {
        public List<MoData> moDatas { get; set; }
        public ConfigWordingReport ConfigWordingReport { get; set; }
    }
}
