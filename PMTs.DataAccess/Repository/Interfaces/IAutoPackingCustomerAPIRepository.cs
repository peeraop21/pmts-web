namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IAutoPackingCustomerAPIRepository
    {
        string GetAutoPackingCustomers(string factoryCode, string token);
        string GetAutoPackingCustomerByCusId(string factoryCode, string cusId, string token);
        void SaveAutoPackingCustomer(string factoryCode, string jsonString, string token);
        void UpdateAutoPackingCustomer(string factoryCode, string jsonString, string token);
        void DeleteAutoPackingCustomer(string factoryCode, string jsonString, string token);
        string GetAllAutoPackingCustomerAndCustomer(string FactoryCode, string KeySearch, string token);
        string GetAutoPackingCustomerAndCustomerByCustName(string FactoryCode, string CustName, string token);
        string GetAutoPackingCustomerAndCustomerByCustCode(string FactoryCode, string CustCode, string token);
        string GetAutoPackingCustomerAndCustomerByCusId(string FactoryCode, string CusId, string token);
    }
}
