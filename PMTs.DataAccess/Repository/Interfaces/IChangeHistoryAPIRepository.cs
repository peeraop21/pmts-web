namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IChangeHistoryAPIRepository
    {
        string GetChangeHistoryList(string factoryCode, string token);

        string GetChangeHistoryByMaterial(string factoryCode, string Materialno, string token);

        void SaveChangeHistory(string jsonString, string token);

        void UpdateChangeHistory(string jsonString, string token);

        void DeleteChangeHistory(string jsonString, string token);
    }
}
