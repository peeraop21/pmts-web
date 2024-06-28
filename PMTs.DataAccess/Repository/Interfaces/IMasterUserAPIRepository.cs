namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IMasterUserAPIRepository
    {
        string GetMasterUserList(string factoryCode, string token);

        void SaveMasterUser(string jsonString, string token);

        void UpdateMasterUser(string jsonString, string token);

        void DeleteMasterUser(string jsonString, string token);
    }
}
