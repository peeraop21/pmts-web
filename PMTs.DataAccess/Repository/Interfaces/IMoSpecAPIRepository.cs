namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IMoSpecAPIRepository
    {
        string GetMoSpecList(string factoryCode, string token);

        void SaveMoSpec(string jsonString, string token);

        void UpdateMoSpec(string jsonString, string token);

        void DeleteMoSpec(string jsonString, string token);

        string GetMoSpecBySaleOrder(string factoryCode, string orderItem, string token);
        string GetMoSpecByOrderItem(string factoryCode, string orderItem, string token);

        string GetMOSpecsBySaleOrders(string factoryCode, string saleOrders, string token);

        void UpdateMoSpecChangestring(string factoryCode, string orderItem, string changeInfo, string token);
    }
}
