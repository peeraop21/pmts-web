namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IJoinCharacterRepository
    {
        string GetJoinCharacterList(string factoryCode, string token);

    }
}
