using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView.MaintenanceProductType;
using PMTs.WebApplication.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services.Interfaces
{
    public interface IMaintenanceCategoriesService
    {
        void GetMaintenanceCategoriesData(MaintenanceCategoriesController maintenanceProductTypeController, ref MaintenanceKindOfProductGroupCreateModel maintenanceKindOfProductGroup);
        void SaveKindOfProductGroup(string KindOfProductID, string ProcessCostID, string KindOfProductGroupID, string ProductTypeArr);

        void GetKindOfProducts(ref List<KindOfProduct> kindOfProducts);
        void CreateKindOfProduct(MaintenanceProductTypeCreateModel maintenanceProductTypeCreateModel);
        void EditKindOfProduct(MaintenanceProductTypeCreateModel maintenanceProductTypeCreateModel);
        void DeleteKindOfProduct(int id);

        void GetKindOfProductGroups(ref List<KindOfProductGroup> kindOfProductGroups);
        void CreateKindOfProductGroup(MaintenanceProductTypeCreateModel maintenanceProductTypeCreateModel);
        void EditKindOfProductGroup(MaintenanceProductTypeCreateModel maintenanceProductTypeCreateModel);
        void DeleteKindOfProductGroup(int id);

        void GetProcessCosts(ref List<ProcessCost> processCosts);
        void CreateProcessCost(MaintenanceProductTypeCreateModel maintenanceProductTypeCreateModel);
        void EditProcessCost(MaintenanceProductTypeCreateModel maintenanceProductTypeCreateModel);
        void DeleteProcessCost(int id);

        void GetProductTypes(ref MaintenanceKindOfProductGroupCreateModel kindOfProductGroupCreateModel);
        void CreateProductType(MaintenanceProductTypeCreateModel maintenanceProductTypeCreateModel);
        void EditProductType(MaintenanceProductTypeCreateModel maintenanceProductTypeCreateModel);
        void DeleteProductType(int id);
    }
}
