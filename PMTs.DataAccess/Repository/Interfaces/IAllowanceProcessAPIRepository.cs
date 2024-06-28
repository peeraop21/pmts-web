namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IAllowanceProcessAPIRepository
    {
        string GetAllowanceProcessList(string factoryCode, string token);

        void SaveAllowanceProcess(string factoryCode, string jsonString, string token);

        void UpdateAllowanceProcess(string factoryCode, string jsonString, string token);

        void DeleteAllowanceProcess(string jsonString, string token);

        string GetAllowanceById(int Id, string token);

        string GetAllowanceProcessByFactoryCode(string factoryCode, string token);
    }
}
