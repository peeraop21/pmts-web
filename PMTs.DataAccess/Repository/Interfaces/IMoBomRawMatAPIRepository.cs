namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IMoBomRawMatAPIRepository
    {
        string GetMoBomRawMatsByFactoryCode(string factoryCode, string token);
        string GetMoBomRawMatsByFgMaterial(string factoryCode, string fgMaterial, string orderItem, string token);
        void SaveMoBomRawMatsList(string factoryCode, string jsonString, string token);
    }
}
