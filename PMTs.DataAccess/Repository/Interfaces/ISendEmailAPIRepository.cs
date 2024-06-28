namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface ISendEmailAPIRepository
    {
        string GetEmailForSendNotifyByFactoryCode(string factoryCode, string token);
    }
}
