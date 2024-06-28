namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IBoardAlternativeAPIRepository
    {
        string GetBoardAlternativeList(string factoryCode, string token);

        string GetBoardAlternativeById(string factoryCode, int id, string token);

        string GetBoardAlternativeByMat(string factoryCode, string mat, string token);

        void SaveBoardAlternative(string jsonString, string token);

        void UpdateBoardAlternative(string jsonString, string token);

        void DeleteBoardAlternative(string jsonString, string token);

        string GetBoardAlternativesByMaterialNos(string factoryCode, string materialNOs, string token);
    }
}
