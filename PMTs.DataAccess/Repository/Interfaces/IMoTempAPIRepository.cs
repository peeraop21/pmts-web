namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IMoTempAPIRepository
    {
        string GetMoTempList(string factoryCode, string token);

        void SaveMoTemp(string jsonString, string token);

        void UpdateMoTemp(string jsonString, string token);

        void DeleteMoTemp(string jsonString, string token);
    }
}
