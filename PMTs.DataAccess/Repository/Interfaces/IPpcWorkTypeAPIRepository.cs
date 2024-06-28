namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IPpcWorkTypeAPIRepository
    {
        string GetPpcWorkTypeList(string factoryCode, string token);

    }
}
