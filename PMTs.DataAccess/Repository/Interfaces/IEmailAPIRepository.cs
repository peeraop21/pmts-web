namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IEmailAPIRepository
    {
        void Send(string factoryCode, string jsonString, string token);
    }
}
