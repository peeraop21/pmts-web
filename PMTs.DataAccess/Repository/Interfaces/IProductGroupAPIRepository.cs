namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IProductGroupAPIRepository
    {
        string GetProductGroupList(string factoryCode, string token);

        void SaveProductGroup(string factoryCode, string jsonString, string token);

        void UpdateProductGroup(string factoryCode, string jsonString, string token);

        void DeleteProductGroup(string jsonString, string token);

        string GetProductGroupByCode(string factoryCode, string indGrp, string token);
    }
}
