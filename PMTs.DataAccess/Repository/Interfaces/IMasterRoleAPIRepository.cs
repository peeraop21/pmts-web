namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IMasterRoleAPIRepository
    {
        string GetMasterRoleList(string factoryCode, string token);

        void SaveMasterRole(string jsonString, string token);

        void UpdateMasterRole(string jsonString, string token);

        void DeleteMasterRole(string jsonString, string token);

        void SaveMenuByRoles(string jsonString, string token);

        void DeleteMenuByRoles(int idmenurole, string token);

        void SaveSubMenuByRoles(string jsonString, string token);

        void DeleteSubMenuByRoles(int subMenuroleID, string token);



    }
}
