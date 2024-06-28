using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView.MaintenanceColor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services.Interfaces
{
    public interface IMaintenanceColorService
    {
        void GetColor(MaintenanceColorViewModel maintenanceColorViewModel);
        void SaveColor(MaintenanceColorViewModel maintenanceColorViewModel);
        void UpdateColor(Color ColorViewModel);
    }
}
