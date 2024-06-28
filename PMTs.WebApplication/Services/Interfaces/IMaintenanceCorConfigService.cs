using PMTs.DataAccess.ModelView.MaintenanceCorConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services.Interfaces
{
    public interface IMaintenanceCorConfigService
    {
        void GetCorConfig(MaintenanceCorConfigViewModel maintenanceCorConfigViewModel);
        void SaveCorConfig(MaintenanceCorConfigViewModel maintenanceCorConfigViewModel);
        void UpdateCorConfig(CorConfigViewModel CorConfigViewModel);
    }
}
