namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface ISubMenusAPIRepository
    {
        string GetSubMenusList(string factoryCode, string token);

        void SaveSubMenus(string jsonString, string token);

        void UpdateSubMenus(string jsonString, string token);

        void DeleteSubMenus(string jsonString, string token);

        string GetSubMenusListBYRole(string factoryCode, int roleId, string token);
        string GetSubMenusAllListBYRole(string factoryCode, int roleId, string token);

    }
}
