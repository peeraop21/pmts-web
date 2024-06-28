using PMTs.DataAccess.ModelView.MaintenancePaperWidth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services.Interfaces
{
    public interface IMaintenancePaperWidthService
    {
        void GetPaperWidth(MaintenancePaperWidthViewModel maintenancePaperWidthViewModel);
        void SavePaperWidth(MaintenancePaperWidthViewModel maintenancePaperWidthViewModel);
        void UpdatePaperWidth(PaperWidthViewModel PaperWidthViewModel);
    }
}
