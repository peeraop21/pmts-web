namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IAutoPackingSpecAPIRepository
    {
        string GetAutoPackingSpecs(string factoryCode, string token);

        string GetAutoPackingSpecByMaterialNo(string factoryCode, string materialNo, string token);

        void SaveAutoPackingSpec(string factoryCode, string jsonString, string token);

        void UpdateAutoPackingSpec(string factoryCode, string jsonString, string token);

        string CreateAutoPackingSpecsFromFile(string factoryCode, string jsonString, string token);

        void SaveAndUpdateAutoPackingSpecFromCusId(string factoryCode, string cusId, string username, string materialNo, string token);
    }
}
