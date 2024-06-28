using PMTs.DataAccess.Extentions;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Shared;
using System;

namespace PMTs.DataAccess.Repository
{
    public class MenuAPIRepository : IMenuAPIRepository
    {
        private readonly string _actionName = "MainMenus/GetMenuByRoleid";
        public dynamic GetMenuList(int roleid)
        {
            // dynamic result = JsonExtentions.HttpActionToAPI(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "?AppName=" + Globals.AppNameEncrypt + "&roleid=" + roleid, string.Empty);
            dynamic result = JsonExtentions.HttpActionToAPI(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "?roleid=" + roleid, string.Empty);

            // var url = Globals.WebAPIUrl + _actionName +;
            // dynamic result = JsonExtentions.HttpActionToAPI(HTTPAction.GET.ToString(), url, string.Empty);

            if (result.Item1)
            {
                return result.Item3;
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }
        private readonly string _actionNameSub = "SubMenus";
        public dynamic GetSubMenuList()
        {
            // dynamic result = JsonExtentions.HttpActionToAPI(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "?AppName=" + Globals.AppNameEncrypt + "&roleid=" + roleid, string.Empty);
            dynamic result = JsonExtentions.HttpActionToAPI(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionNameSub, string.Empty);

            // var url = Globals.WebAPIUrl + _actionName +;
            // dynamic result = JsonExtentions.HttpActionToAPI(HTTPAction.GET.ToString(), url, string.Empty);

            if (result.Item1)
            {
                return result.Item3;
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }
    }
}
