namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IBoardCostAPIRepository
    {
        string GetBoardCostList(string factoryCode, string token);

        string GetBoardCostByCode(string factoryCode, string code, string costField, string token);

        void SaveBoardCost(string jsonString, string token);

        void UpdateBoardCost(string jsonString, string token);

        void DeleteBoardCost(string jsonString, string token);
    }
}
