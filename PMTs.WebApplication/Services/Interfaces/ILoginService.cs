using PMTs.DataAccess.ModelView.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services.Interfaces
{
    public interface ILoginService
    {
        void GetLogin(ref LoginViewModel LoginViewModel);
        void GetUser(ref LoginViewModel LoginViewModel);
        void GetUserTIP(ref UserTIP userTIP);
    }
}
