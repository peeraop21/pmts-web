namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IBoardCombineAccAPIRepository
    {
        string GetCost(string factoryCode, string code, string costField, string token);

        string GetBoardCombineAccList(string factoryCode, string token);

        void AddBoardCombineAccColumn(string factoryCode, string costField, string token);

        void ChangeBoardCombineAccColumn(string factoryCode, string OldCostField, string newCostField, string token);

        void DropBoardCombineAccColumn(string factoryCode, string costField, string token);

        string ImportBoardCombineAcc(string factoryCode, string userName, string jsonString, string token);

        string GetBoardCombineAcc(string factoryCode, string code, string token);
        string UpdateBoardCombineAcc(string factoryCode, string jsonString, string token);
    }
}
