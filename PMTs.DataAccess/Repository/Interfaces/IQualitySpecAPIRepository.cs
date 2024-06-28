namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IQualitySpecAPIRepository
    {
        string GetQualitySpecByMaterialNo(string factoryCode, string materialNo, string token);

        string GetQualitySpecs(string factoryCode, string token);

        void SaveQualitySpec(string factoryCode, string jsonString, string token);

        void UpdateQualitySpec(string factoryCode, string jsonString, string token);

        void DeleteQualitySpec(string factoryCode, string jsonString, string token);

        string GetQualitySpecsByMaterialNos(string factoryCode, string materialNos, string token);
    }
}