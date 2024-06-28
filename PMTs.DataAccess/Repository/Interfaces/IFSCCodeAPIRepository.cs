namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IFSCCodeAPIRepository
    {
        string GetFSCCodes(string factoryCode, string token);
        string GetFSCCodeByFSCCode(string factoryCode, string fscCode, string token);

        void SaveFSCCode(string factoryCode, string jsonString, string token);

        void UpdateFSCCode(string factoryCode, string jsonString, string token);

        void DeleteFSCCode(string jsonString, string token);
    }
}
