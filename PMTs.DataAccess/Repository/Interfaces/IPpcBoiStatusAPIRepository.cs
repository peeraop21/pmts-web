namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IPpcBoiStatusAPIRepository
    {
        string GetPpcBoiStatusList(string factoryCode, string token);

        void SavePpcBoiStatus(string factoryCode, string jsonString, string token);

        void UpdatePpcBoiStatus(string factoryCode, string jsonString, string token);

        void DeletePpcBoiStatus(string jsonString, string token);
    }
}
