namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IHierarchyLv4APIRepository
    {
        string GetAllHierarchyLv4s(string factorycode, string token);
        void SaveHierarchy4(string factoryCode, string hierarchy4Json, string token);
        void UpdateHierarchy4(string factoryCode, string hierarchy4Json, string token);
    }
}
