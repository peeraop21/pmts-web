using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.ModelView.MaintenanceColor
{
    public class MaintenanceColorViewModel
    {
        public IEnumerable<Color> ColorViewModelList { get; set; }
        public Color ColorViewModel { get; set; }
    }
}
