namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IBoardCombineAPIRepository
    {
        string GetBoardCombineList(string factoryCode, string token);

        string GetBoardCombineListSearch(string factoryCode, string token);

        string GetBoardSpecWeightByCode(string factoryCode, string code, string token);

        string GetBoard(string factoryCode, string costField, string lv2, string lv3, string token);

        string GetBoardByCode(string factoryCode, string code, string token);

        string GetBoardByFlute(string factoryCode, string flute, string token);

        void SaveBoardCombine(string jsonString, string token);

        void UpdateBoardCombine(string jsonString, string token);

        void DeleteBoardCombine(string jsonString, string token);

        string GetBoardByBoard(string factoryCode, string board, string flute, string token);

        string GetAllDataMainTain(string factoryCode, string token);
        string GetMaxcode(string factoryCode, string token);
        string AddBoardcombind(string factoryCode, string jsonString, string token);
        void UpdateBoardCombind(string factoryCode, string jsonString, string token);
        string GetAllBoardspectByCode(string factoryCode, string code, string token);
        string GetBoardsByCodes(string factoryCode, string codes, string token);
        string GenerateCode(string factoryCode, string token);
        string GenerateDataForSAP(string factoryCode, string jsonString, string token);
    }
}
