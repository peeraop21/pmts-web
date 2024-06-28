namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IMapCostAPIRepository
    {
        string GetMapCostList(string factoryCode, string token);

        string GetCostField(string factoryCode, string lv2, string lv3, string lv4, string token);

        void CreateMapCost(string jsonString, string token);

        void UpdateMapCost(string jsonString, string token);

        void DeleteMapCost(string jsonString, string token);
    }
}
