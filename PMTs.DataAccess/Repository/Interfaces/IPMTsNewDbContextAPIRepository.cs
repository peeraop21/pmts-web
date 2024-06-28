namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IPMTsNewDbContextAPIRepository
    {
        string GetPMTsNewDbContextList(string factoryCode, string token);

        void SavePMTsNewDbContext(string jsonString, string token);

        void UpdatePMTsNewDbContext(string jsonString, string token);

        void DeletePMTsNewDbContext(string jsonString, string token);
    }
}
