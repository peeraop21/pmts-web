namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IAllowanceHardAPIRepository
    {
        string GetAllowanceHardList(string factoryCode, string token);

        void SaveAllowanceHard(string jsonString, string token);

        void UpdateAllowanceHard(string jsonString, string token);

        void DeleteAllowanceHard(string jsonString, string token);
    }
}
