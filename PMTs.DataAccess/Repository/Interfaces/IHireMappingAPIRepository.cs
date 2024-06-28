namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IHireMappingAPIRepository
    {
        string GetAllHireMapping(string factoryCode, string token);
    }
}
