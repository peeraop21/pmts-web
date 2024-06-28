namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IJoinAPIRepository
    {
        string GetJoinList(string factoryCode, string token);

        void SaveJoin(string factoryCode, string jsonString, string token);

        void UpdateJoin(string factoryCode, string jsonString, string token);

        void DeleteJoin(string jsonString, string token);
    }
}
