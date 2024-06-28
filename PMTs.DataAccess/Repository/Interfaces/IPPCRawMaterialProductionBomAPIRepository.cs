namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IPPCRawMaterialProductionBomAPIRepository
    {
        string GetAll(string token);
        string GetPPCRawMaterialProductionBOMsByFgMaterial(string factoryCode, string fgMaterial, string token);
        string GetPPCRawMaterialProductionBOMByFgMaterialAndMaterialNo(string factoryCode, string fgMaterial, string materialNo, string token);
        void SaveRawMaterialProductionBom(string factoryCode, string jsonString, string token);
        void UpdatePPCRawMaterialProductionBom(string factoryCode, string jsonString, string token);
        void DeleteRawMaterial(string factoryCode, string jsonString, string token);
        void DeleteManyRawMaterial(string factoryCode, string jsonString, string token);
        void SaveRawMaterialProductionBoms(string factoryCode, string jsonString, string token);
    }
}
