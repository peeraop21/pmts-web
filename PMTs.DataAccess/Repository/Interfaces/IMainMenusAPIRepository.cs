namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IMainMenusAPIRepository
    {
        string GetMainMenusList(string factoryCode);

        string GetMainMenuByRoleId(string factoryCode, int roleId);

        void SaveMainMenus(string jsonString);

        void UpdateMainMenus(string jsonString);

        void DeleteMainMenus(string jsonString);

        //Tassanai Update 03/04/2020
        string GetMainMenuAllByRoleId(string factoryCode, int roleId);
    }
}
