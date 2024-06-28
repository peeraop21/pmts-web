namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IPaperWidthAPIRepository
    {
        string GetPaperWidthList(string factoryCode, string token);

        void SavePaperWidth(string factoryCode, string jsonString, string token);

        void UpdatePaperWidth(string factoryCode, string jsonString, string token);

        void DeletePaperWidth(string jsonString, string token);
    }
}
