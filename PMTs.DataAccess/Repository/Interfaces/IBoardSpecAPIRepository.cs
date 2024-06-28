namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IBoardSpecAPIRepository
    {
        string GetBoardSpecList(string factoryCode, string token);

        string GetBoardSpecByBoardId(string factoryCode, string boardId, string token);

        string GetBoardSpecStationByBoardId(string factoryCode, string boardId, string token);

        string GetBoardSpecWeightByBoardId(string factoryCode, string boardId, string token);

        string GetBoardSpecByCode(string factoryCode, string code, string token);

        void SaveBoardSpec(string jsonString, string token);

        void UpdateBoardSpec(string jsonString, string token);

        void DeleteBoardSpec(string jsonString, string token);

        string GetBoardSpecsByCodes(string factoryCode, string codes, string token);
    }
}
