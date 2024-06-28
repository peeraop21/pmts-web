using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView.ProductCatalog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services.Interfaces
{
    public interface IProductCatalogService
    {
        ProductCatalogModel GetProductCatalogFrist();
        // ProductCatalogModel GetProductCatalog();
        ProductCatalogModel GetProductCatalog(ProductCatalogsSearch model);
        void SaveProductCatalogConfig(string[] arrayColumn);
        void SaveProductCatalogRemark(ProductCatalogRemark Model);
        ProductCatalogRemark GetProductCatalogRemark(string pc);
        DataTable CreateDynamicDataTable(ProductCatalogsSearch dataSearch);
        ProductCatalogModel GetHoldMaterial(string Material);
        void SaveHoldMaterial(HoldMaterial model);
        void UpdateHoldMaterial(HoldMaterial model);
        void SaveHoldMaterialHistory(HoldMaterialHistory model);
        ProductCatalogModel GetAllPlant();
        ProductCatalogModel GetBom(string factorycode, string material);
        string GetOrderItemByMoData(string factoryCode, string Material);
        List<CompanyProfile> GetAllCompanyProfile();
        CompanyProfile GetAllCompanyProfileByLogin();
        void SearchScalePriceMatProduct(ref ScalePriceMatProductViewModel model, string custId, string custName, string custCode, string pc1, string pc2, string pc3, string materialType, string salePlants, string plantPdts, string materialNo);
        void SearchBOMMaterialProduct(ref List<BOMMaterialProductModel> bomMaterialProductModels, string custId, string custName, string custCode, string pc1, string pc2, string pc3);
        ProductCatalogModel GetAllPlantProduction();
        void GetMaterialTypesAndPlants(ref ScalePriceMatProductViewModel model);
        void CallApiE_Ordering(HoldMaterial holdMaterial);
    }
}
