namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IFSCFGCodeAPIRepository
    {
        string GetFSCFGCodes(string factoryCode, string token);
        string GetFSCFGCodeByFSCFGCode(string factoryCode, string fscFgCode, string token);

        void SaveFSCFGCode(string factoryCode, string jsonString, string token);

        void UpdateFSCFGCode(string factoryCode, string jsonString, string token);

        void DeleteFSCFGCode(string jsonString, string token);
    }
}
