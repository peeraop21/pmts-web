namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IPresaleChangeRoutingAPIRepository
    {
        string GetAllPresaleChangeRoutings(string factoryCode, string token);
        string GetPresaleChangeRoutingsByMaterialNo(string factoryCode, string materialNo, string token);
        void UpdatePresaleRoutings(string factoryCode, string jsonString, string token);
    }
}
