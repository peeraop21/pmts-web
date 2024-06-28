namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IKindOfProductAPIRepository
    {
        string GetKindOfProductList(string factoryCode, string token);

        string GetKindOfProductById(string factoryCode, int id, string token);

        void CreateKindOfProduct(string factoryCode, string jsonString, string token);

        void UpdateKindOfProduct(string factoryCode, string jsonString, string token);

        void DeleteKindOfProduct(string factoryCode, string jsonString, string token);

        string GetKindOfProductsByIds(string factoryCode, string idKindOfProducts, string token);
    }
}
