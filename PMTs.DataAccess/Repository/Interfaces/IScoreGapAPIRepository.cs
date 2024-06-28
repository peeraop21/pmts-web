namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IScoreGapAPIRepository
    {
        string GetScoreGapList(string factoryCode, string token);

        string GetScoreGapById(int Id, string token);

        void SaveScoreGap(string factoryCode, string jsonString, string token);

        void UpdateScoreGap(string factoryCode, string jsonString, string token);

        void DeleteScoreGap(string jsonString, string token);

        string GetScoreGapsByFactoryCode(string factoryCode, string token);
    }
}