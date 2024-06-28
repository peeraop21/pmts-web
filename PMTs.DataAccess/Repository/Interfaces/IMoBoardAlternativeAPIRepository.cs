namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IMoBoardAlternativeAPIRepository
    {
        string GetMoBoardAlternativeList(string factoryCode, string token);
        void SaveMoBoardAlternative(string jsonString, string token);
        void UpdateMoBoardAlternative(string jsonString, string token);
        void DeleteMoBoardAlternative(string jsonString, string token);
    }
}
