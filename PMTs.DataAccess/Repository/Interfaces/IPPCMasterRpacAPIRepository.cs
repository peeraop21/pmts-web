namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IPPCMasterRpacAPIRepository
    {
        string GetPPCMasterRpacList(string factoryCode, string token);
        string GetPPCMasterRpacsByFactoryCode(string factoryCode, string token);
        string GetPPCMasterRpacsByDimensionCode(string factoryCode, string dimensionCode, string token);

        void SavePPCMasterRpac(string factoryCode, string jsonString, string token);

        void UpdatePPCMasterRpac(string factoryCode, string jsonString, string token);

        //void DeletePPCMasterRpac(string jsonString, string token);
    }
}
