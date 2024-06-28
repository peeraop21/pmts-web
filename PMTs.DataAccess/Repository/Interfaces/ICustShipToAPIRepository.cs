namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface ICustShipToAPIRepository
    {
        string GetCustShipToList(string factoryCode, string token);

        string GetCustShipToListByCustCode(string factoryCode, string custCode, string token);

        void SaveCustShipToList(string jsonString, string token);
    }
}
