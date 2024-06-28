using PMTs.DataAccess.ModelView.MaintenancePrintMethod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services.Interfaces
{
    public interface IMaintenancePrintMethodService
    {
        void GetPrintMethod(MaintenancePrintMethodViewModel maintenancePrintMethodViewModel);
        void SavePrintMethod(MaintenancePrintMethodViewModel maintenancePrintMethodViewModel);
        void UpdatePrintMethod(PrintMethodViewModel PrintMethodViewModel);
    }
}

