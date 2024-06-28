namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IPrintMethodAPIRepository
    {
        string GetPrintMethodList(string factoryCode, string token);

        void SavePrintMethod(string factoryCode, string jsonString, string token);

        void UpdatePrintMethod(string factoryCode, string jsonString, string token);

        void DeletePrintMethod(string jsonString, string token);
    }
}
