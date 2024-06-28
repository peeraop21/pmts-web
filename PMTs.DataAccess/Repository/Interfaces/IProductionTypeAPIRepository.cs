namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IProductionTypeAPIRepository
    {
        string GetProductionTypeList(string factoryCode, string token);

        void SaveProductionType(string jsonString, string token);

        void UpdateProductionType(string jsonString, string token);

        void DeleteProductionType(string jsonString, string token);
    }
}
