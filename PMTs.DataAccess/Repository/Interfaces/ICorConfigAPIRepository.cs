namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface ICorConfigAPIRepository
    {
        string GetCorConfigList(string factoryCode, string token);

        void SaveCorConfig(string factoryCode, string jsonString, string token);

        void UpdateCorConfig(string factoryCode, string jsonString, string token);

        void DeleteCorConfig(string jsonString, string token);

        string GetCorConfigByFactoryCode(string factoryCode, string machineName, string token);
    }
}
