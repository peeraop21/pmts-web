using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView.MaintenanceMapCost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services.Interfaces
{
    public interface IMaintenanceMapCostService
    {
        void GetMapCost(ref MaintenanceMapCostViewModel maintenanceMapCostViewModel);

        void CreateMapCost(MaintenanceMapCostViewModel maintenanceMapCostViewModel);

        void UpdateMapCost(MaintenanceMapCostViewModel maintenanceMapCostViewModel);

        void DeleteMapCost(string id);

        void GetHierarchyLv3(ref MaintenanceMapCostViewModel maintenanceMapCostViewModel);
        void SaveHierarchyLv3(ref MaintenanceMapCostViewModel maintenanceMapCostViewModel, HierarchyCreateModel hierarchyCreateModel);
        void UpdateHierarchyLv3(ref MaintenanceMapCostViewModel maintenanceMapCostViewModel, HierarchyCreateModel hierarchyCreateModel);

        void GetHierarchyLv4(ref MaintenanceMapCostViewModel maintenanceMapCostViewModel);
        void SaveHierarchyLv4(ref MaintenanceMapCostViewModel maintenanceMapCostViewModel, HierarchyCreateModel hierarchyCreateModel);
        void UpdateHierarchyLv4(ref MaintenanceMapCostViewModel maintenanceMapCostViewModel, HierarchyCreateModel hierarchyCreateModel);
    }
}
