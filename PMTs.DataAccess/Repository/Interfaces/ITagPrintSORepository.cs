namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface ITagPrintSORepository
    {
        string GetTagPrintSO(string _factoryCode, string token);
    }
}
