using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.ModelView.MaintenanceAllowanceHard
{
    public class MaintenanceAllowanceHardViewModel
    {
        public IEnumerable<AllowanceHard> allowanceHards { get; set; }
        public AllowanceHard allowance { get; set; }
    }
}
