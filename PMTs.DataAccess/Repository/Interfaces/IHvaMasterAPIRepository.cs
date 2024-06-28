namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IHvaMasterAPIRepository
    {
        string GetHvaMasters(string factoryCode, string token);

        string GetHvaMasterByHighValue(string factoryCode, string highValue, string token);
    }
}
