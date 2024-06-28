namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IScoreTypeAPIRepository
    {
        string GetScoreTypeList(string factoryCode, string token);
        string GetScoreTypeById(string factoryCode, int Id, string token);
        string GetScoreTypeByScoreTypeId(string factoryCode, string scoreTypeId, string token);
        string GetScoreTypesByScoreTypeIds(string factoryCode, string scoreTypeIds, string token);
    }
}
