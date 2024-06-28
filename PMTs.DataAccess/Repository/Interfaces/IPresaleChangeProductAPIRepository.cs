namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IPresaleChangeProductAPIRepository
    {
        string GetAllPresaleChangeProducts(string factoryCode, string token);
        string GetPresaleChangeProductByMaterialNo(string factoryCode, string materialNo, string psmId, string token);
        string GetPresaleChangeProductsByMaterialNo(string factoryCode, string materialNo, string token);
        void UpdatePresaleChangeProduct(string factoryCode, string changePresaleJson, string token);
        string GetPresaleChangeProductsByKeySearch(string factoryCode, string typeSearch, string keySearch, string token);
        void UpdatePresaleChangeProductStatusById(string factoryCode, int id, string status, string token);
        string GetPresaleChangeProductsByActiveStatus(string factoryCode, string token);
    }
}
