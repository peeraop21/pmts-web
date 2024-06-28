namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IRunningNoAPIRepository
    {
        string GetRunningNoList(string factoryCode, string token);

        string GetRunningNoByGroupId(string factoryCode, string groupId, string token);

        void SaveRunningNo(string jsonString, string token);

        void UpdateRunningNo(string jsonString, string token);

        void DeleteRunningNo(string jsonString, string token);
    }
}
