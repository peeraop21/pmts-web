namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IMoBoardUseAPIRepository
    {
        string GetMoBoardUseList(string factoryCode, string token);

        void SaveMoBoardUse(string jsonString, string token);

        void UpdateMoBoardUse(string jsonString, string token);

        void DeleteMoBoardUse(string jsonString, string token);
    }
}
