namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IConfigWordingReportAPIRepository
    {
        string GetConfigWordingReportsByFactoryCode(string factoryCode, string token);
        void CreateConfigWordingReport(string factorycode, string jsonString, string token);
        void UpdateConfigWordingReport(string factorycode, string jsonString, string token);
        string UpdateConfigWordingReportByFactoryCode(string factorycode, string username, string ConfigWording, string token);
    }
}
