namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IBoardUseAPIRepository
    {
        string GetBoardUseList(string factoryCode, string token);

        string GetBoardUseByMaterialNo(string factoryCode, string MaterialNo, string token);

        void SaveBoardUse(string jsonString, string token);

        void UpdateBoardUse(string jsonString, string token);

        void DeleteBoardUse(string jsonString, string token);

        string GetPaperItemByMaterialNo(string factoryCode, string materialNo, string token);

        string GetBoardUsesByMaterialNos(string factoryCode, string materialNos, string token);
    }
}
