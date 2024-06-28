namespace PMTs.WebApplication.Services.Interfaces
{
    public interface IEmailService
    {
        void SendNotifyWhenCreatedBoard(string boardCode, string boardDesc);
    }
}
