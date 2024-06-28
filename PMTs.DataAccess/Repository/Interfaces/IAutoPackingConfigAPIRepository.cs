namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IAutoPackingConfigAPIRepository
    {
        string GetAutoPackingConfigs(string factoryCode, string token);
        string GetAutoPackingConfigsBySubject(string factoryCode, string subject, string token);
        void SaveAutoPackingConfig(string factoryCode, string jsonString, string token);
        void UpdateAutoPackingConfig(string factoryCode, string jsonString, string token);
        void DeleteAutoPackingConfig(string factoryCode, string jsonString, string token);
    }
}
