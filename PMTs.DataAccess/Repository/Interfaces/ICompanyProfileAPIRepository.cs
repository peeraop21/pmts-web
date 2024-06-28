namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface ICompanyProfileAPIRepository
    {

        string GetCompanyProfileList(string factoryCode, string token);

        string GetCompanyProfilesByStatusPMTs(string factoryCode, string token);

        void SaveCompanyProfile(string jsonString, string token);

        void UpdateCompanyProfile(string jsonString, string token);

        void DeleteCompanyProfile(string jsonString, string token);

        string GetCompanyProfileByPlant(string factoryCode, string token);

        string GetCompanyProfileBySaleOrg(string factoryCode, string saleOrg, string token);

        string GetFirstCompanyProfileBySaleOrg(string factoryCode, string saleOrg, string token);

    }
}
