using PMTs.DataAccess.ModelView.MaintenanceFlute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services.Interfaces
{
    public interface IMaintenanceFluteService
    {
        MaintenanceFluteModel GetFlute();
        void AddFlute(MaintenanceFluteModel model);
        void UpdateFlute(MaintenanceFluteModel model);
    }
}
