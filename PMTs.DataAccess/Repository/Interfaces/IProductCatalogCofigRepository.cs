namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IProductCatalogCofigRepository
    {
        string GetProductGroupList(string factoryCode, string Username, string Model, string token);
        string UpdateroductGroupList(string factoryCode, string Username, string Model, string token);
        string UpdateProductCatalogRemark(string factoryCode, string Model, string token);
        string GetProductCatalogRemark(string factoryCode, string PC, string token);

        string GetHoldMaterialByMaterial(string factoryCode, string Material, string token);
        string SaveHoldMaterial(string factoryCode, string Model, string token);
        string UpdateHoldMaterial(string factoryCode, string Model, string token);
        string GetHoldMaterialHistoryByMaterial(string factoryCode, string Material, string token);
        string SaveHoldMaterialHistory(string factoryCode, string Model, string token);
        string GetOrderItemByMoData(string factoryCode, string Material, string token);
        string PresaleHoldMaterial(string config, string model);
        string GetScalePriceMatProduct(string factoryCode, string custId, string custName, string custCode, string pc1, string pc2, string pc3, string materialType, string salePlants, string plantPdts, string materialNo, string token);
        string GetBOMMaterialProduct(string factoryCode, string custId, string custName, string custCode, string pc1, string pc2, string pc3, string token);
    }
}
