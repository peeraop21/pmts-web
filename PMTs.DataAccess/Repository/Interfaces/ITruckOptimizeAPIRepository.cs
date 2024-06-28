namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface ITruckOptimizeAPIRepository
    {
        string GetTruckOptimizes(string factoryCode, string token);

        string GetTruckOptimizeByMaterialNo(string factoryCode, string materialNo, string token);

        void SaveTruckOptimize(string factoryCode, string jsonString, string token);

        void UpdateTruckOptimize(string factoryCode, string jsonString, string token);

        string CreateTruckOptimizesFromFile(string factoryCode, string jsonString, string token);
    }
}
