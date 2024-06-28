namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IMoRoutingAPIRepository
    {
        string GetMoRoutingList(string factoryCode, string token);
        void SaveMoRouting(string jsonString, string token);

        void UpdateMoRouting(string jsonString, string token);

        void DeleteMoRouting(string jsonString, string token);

        string GetMORoutingsBySaleOrder(string factoryCode, string orderItem, string token);
        string SaveMORoutingsBySaleOrder(string factoryCode, string orderItem, string jsonString, string token);
        string GetMORoutingsBySaleOrders(string factoryCode, string saleOrders, string token);
    }
}
