namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IHierarchyLv2APIRepository
    {
        string GetHierarchyLv2List(string factoryCode, string token);

        void CreateHierarchyLv2Matrix(string factoryCode, string jsonString, string token);

        void DeleteHierarchyLv2Matrix(string factoryCode, string jsonString, string token);
    }
}
