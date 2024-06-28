namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IProcessCostAPIRepository
    {
        string GetProcessCostList(string factoryCode, string token);

        void CreateProcessCost(string factoryCode, string jsonString, string token);

        void UpdateProcessCost(string factoryCode, string jsonString, string token);

        void DeleteProcessCost(string factoryCode, string jsonString, string token);

        string GetProcessCostById(string factoryCode, int id, string token);
    }
}
