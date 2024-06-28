namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IAttachFileMOAPIRepository
    {
        string GetAttachFileMOsByOrderItem(string factoryCode, string orderItem, string token);

        void SaveAttachFileMO(string jsonString, string token);

        void UpdateAttachFileMO(string jsonString, string token);

        void DeleteAttachFileMO(string jsonString, string token);

        string GetAttachFileMOByFileName(string factoryCode, string orderItem, string fullPath, string token);

        string GetAttachFileMOsByOrderItems(string factoryCode, string saleOrders, string token);
        string GetAttachFileMOsByOrderItemsAndFactoryCode(string factoryCode, string saleOrders, string token);
    }
}
