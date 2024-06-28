namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface ICustomerAPIRepository
    {
        string GetCustomerShipTo(string factoryCode, string token);

        string GetCustomerList(string factoryCode, string token);

        string GetCustomerByCusID(string factoryCode, string cusID, string token);

        string GetCustomersByCusID(string factoryCode, string cusID, string token);

        string GetCustomerByCusIDAndCustName(string factoryCode, string cusID, string custName, string token);

        void SaveCustomer(string jsonString, string token);

        void UpdateCustomer(string jsonString, string token);

        void DeleteCustomer(string jsonString, string token);

        string GetCustomerById(string factoryCode, int Id, string token);

        string GetCustomerShipToByCustname(string factoryCode, string cusName, string token);
        string GetCustomerShipToByCustCode(string factoryCode, string cusCode, string token);
        string GetCustomerShipToByCustId(string factoryCode, string cusID, string token);
        void DeleteCustomerByID(string Factorycode, int ID, string token);
        string GetCustomersByCustomerGroup(string factoryCode, string token);

    }
}
