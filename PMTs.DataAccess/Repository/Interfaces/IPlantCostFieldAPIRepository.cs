namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IPlantCostFieldAPIRepository
    {
        string GetPlantCostFields(string factoryCode, string token);

        bool CheckCostFieldinUse(string factoryCode, string costfield, string token);

        void CreatePlantCostField(string jsonString, string token);

        void UpdatePlantCostField(string jsonString, string token);

        void DeletePlantCostField(string jsonString, string token);

        void DeletePlantCostFields(string factoryCode, string token);

        void CreatePlantCostFields(string factoryCode, string jsonString, string token);
    }
}
