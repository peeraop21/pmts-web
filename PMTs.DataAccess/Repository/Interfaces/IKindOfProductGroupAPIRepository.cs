namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IKindOfProductGroupAPIRepository
    {
        string GetKindOfProductGroupList(string factoryCode, string token);

        string GetKindOfProductGroupById(string factoryCode, int Id, string token);

        void CreateKindOfProductGroup(string factoryCode, string jsonString, string token);

        void UpdateKindOfProductGroup(string factoryCode, string jsonString, string token);

        void DeleteKindOfProductGroup(string factoryCode, string jsonString, string token);

        string GetKindOfProductGroupsByIds(string factoryCode, string idKindOfProductGroups, string token);
    }
}
