namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IInterfaceSystemAPIAPIRepository
    {
        string GetInterfaceSystemAPIsByFactoryCode(string factoryCode, string token);

        void SaveInterfaceSystemAPI(string factoryCode, string jsonString, string token);

        void UpdateInterfaceSystemAPI(string factoryCode, string jsonString, string token);
    }
}
