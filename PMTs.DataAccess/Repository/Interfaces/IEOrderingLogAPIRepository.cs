namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IEOrderingLogAPIRepository
    {
        string GetEOrderingLogList(string factoryCode, string token);

        string GetLastEOrderingLog(string factoryCode, string token);

        void SaveEOrderingLog(string factoryCode, string jsonString, string token);

        void UpdateEOrderingLog(string factoryCode, string jsonString, string token);

        void DeleteEOrderingLog(string factoryCode, string jsonString, string token);
    }
}
