namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IMoDatalogAPIRepository
    {
        string GetMoDatalogList(string factoryCode, string token);

        void SaveMoDatalog(string jsonString, string token);

        void UpdateMoDatalog(string jsonString, string token);

        void DeleteMoDatalog(string jsonString, string token);

    }
}
