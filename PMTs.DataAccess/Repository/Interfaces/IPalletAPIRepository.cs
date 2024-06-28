namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IPalletAPIRepository
    {
        string GetPalletList(string factoryCode, string token);

        void SavePallet(string jsonString, string token);

        void UpdatePallet(string jsonString, string token);

        void DeletePallet(string jsonString, string token);
    }
}
