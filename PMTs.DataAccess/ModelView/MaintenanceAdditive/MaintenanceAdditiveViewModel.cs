using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.ModelView.MaintenanceAdditive
{
    public class MaintenanceAdditiveViewModel
    {
        public IEnumerable<Additive> additives { get; set; }
        public Additive Additive { get; set; }
    }
}
