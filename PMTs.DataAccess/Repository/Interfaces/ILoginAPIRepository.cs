namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface ILoginAPIRepository
    {
        void GetLogin(string jsonstring, string token);
    }
}
