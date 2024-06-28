namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IHierarchyLv3APIRepository
    {
        string GetAllHierarchyLv3s(string factorycode, string token);
        void SaveHierarchy3(string factoryCode, string hierarchyLv3Json, string token);
        void UpdateHierarchy3(string factoryCode, string hierarchyLv3Json, string token);
    }
}
