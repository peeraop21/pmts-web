namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IHoneyPaperAPIRepository
    {
        string GetHoneyPaperByGrade(string factoryCode, string grade, string token);
        string GetAllHoneyPaper(string factoryCode, string token);
        void CreateHoneyPaper(string factoryCode, string jsonHoneyPaper, string token);
        void UpdateHoneyPaper(string factoryCode, string jsonHoneyPaper, string token);
    }
}
