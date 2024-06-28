namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IPlantViewAPIRepository
    {
        string GetPlantViewList(string factoryCode, string token);

        string GetPlantViewByMaterialNo(string factoryCode, string materialNo, string token);

        string GetPlantViewByMaterialNoAndPlant(string factoryCode, string materialNo, string plant, string token);

        string GetPlantViewsByMaterialNo(string factoryCode, string materialNo, string token);

        void SavePlantView(string jsonString, string token);

        void UpdatePlantView(string jsonString, string token);

        void DeletePlantView(string jsonString, string token);

        string GetPlantViewByPlant(string factoryCode, string materialNo, string plant, string token);

        void UpdatePlantViewShipBlk(string FactoryCode, string MaterialNo, string Status, string token);

        string GetReusePlantViewsByMaterialNos(string factoryCode, string materialNos, string token);
    }
}
