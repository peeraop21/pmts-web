namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IBoardAltSpecAPIRepository
    {
        string GetBoardAltSpecList(string factoryCode, string token);

        string GetBoardAltSpecById(string factoryCode, int id, string token);

        void SaveBoardAltSpec(string jsonString, string token);

        void UpdateBoardAltSpec(string jsonString, string token);

        void DeleteBoardAltSpec(string jsonString, string token);
    }
}
