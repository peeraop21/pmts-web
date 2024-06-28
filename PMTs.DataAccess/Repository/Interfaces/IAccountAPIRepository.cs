using PMTs.DataAccess.ModelView.Login;

namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IAccountAPIRepository
    {
        string GetaccountList(string factoryCode, string token);

        string GetLogin(string jsonString, string token);

        dynamic GetUser(string jsonString, string token);

        void SaveAccount(string jsonString, string token);

        void UpdateAccount(string jsonString, string token);

        string GetLoginAD(string jsonString, string token);

        string CheckGetLogin(string jsonString, string token);

        string GetAllAccountList(string token);
        UserTIP GetLoginTip(string apiTIPUrl, string jsonString);

        //string GetAccountById(string factoryCode, int Id, string token);
    }
}
