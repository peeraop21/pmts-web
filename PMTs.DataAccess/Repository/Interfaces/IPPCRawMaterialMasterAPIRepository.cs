namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IPPCRawMaterialMasterAPIRepository
    {
        string SearchPPCRawMaterialMasterByMaterialNo(string factoryCode, string materialNo, string materialDesc, string token);
        string GetPPCRawMaterialMasterById(string factoryCode, int Id, string token);
        void SavePPCRawMaterialMaster(string jsonString, string token);
        void UpdatePPCRawMaterialMaster(string jsonString, string token);
        void DeletePPCRawMaterialMaster(string factoryCode, string jsonString, string token);
        string GetPPCRawMaterialMasterByFactoryAndMaterialNoAndDescription(string factoryCode, string materialNo, string materialDesc, string token);
        string GetPPCRawMaterialMastersByFactoryCode(string factoryCode, string token);
        string GetPPCRawMaterialMasterByFactoryAndMaterialNo(string factoryCode, string materialNo, string token);
    }
}
