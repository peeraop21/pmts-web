using PMTs.DataAccess.ModelView.MaintenanceProductGroup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services.Interfaces
{
    public interface IMaintenanceProductGroupService
    {
        void GetProductGroup(MaintenanceProductGroupViewModel maintenanceProductGroupViewModel);
        void SaveProductGroup(MaintenanceProductGroupViewModel maintenanceProductGroupViewModel);
        void UpdateProductGroup(ProductGroupViewModel ProductGroupViewModel);
    }
}
