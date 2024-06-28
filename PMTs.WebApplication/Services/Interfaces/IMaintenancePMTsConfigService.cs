

using PMTs.DataAccess.ModelView.MaintenancePMTsConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services.Interfaces
{
    public interface IMaintenancePMTsConfigService
    {
        void GetPMTsConfig(ref MaintenancePMTsConfigViewModel maintenancePMTsConfigViewModel);
        void SavePMTsConfig(MaintenancePMTsConfigViewModel maintenancePMTsConfigViewModel);
        void UpdatePMTsConfig(PMTsConfigViewModel PMTsConfigViewModel);
    }
}
