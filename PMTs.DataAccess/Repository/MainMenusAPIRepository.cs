using PMTs.DataAccess.Extentions;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Shared;
using System;

namespace PMTs.DataAccess.Repository
{
    public class MainMenusAPIRepository : IMainMenusAPIRepository
    {
        private readonly string _actionName = "MainMenus";

        public string GetMainMenusList(string factoryCode)
        {
            dynamic result = JsonExtentions.HttpActionToAPI(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "?FactoryCode=" + factoryCode, string.Empty);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetMainMenuByRoleId(string factoryCode, int roleId)
        {
            //ห้ามเเก้ ไม่เกี่ยกับ jwt
            dynamic result = JsonExtentions.HttpActionToAPI(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetMainMenuByRoleId" + "?FactoryCode=" + factoryCode + "&roleid=" + roleId, string.Empty);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public void SaveMainMenus(string jsonString)
        {
            dynamic result = JsonExtentions.HttpActionToAPI(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName, jsonString);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public void UpdateMainMenus(string jsonString)
        {
            dynamic result = JsonExtentions.HttpActionToAPI(HTTPAction.PUT.ToString(), Globals.WebAPIUrl + _actionName, jsonString);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public void DeleteMainMenus(string jsonString)
        {
            dynamic result = JsonExtentions.HttpActionToAPI(HTTPAction.DELETE.ToString(), Globals.WebAPIUrl + _actionName, jsonString);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }


        //Tassanai update 03/04/2020
        public string GetMainMenuAllByRoleId(string factoryCode, int roleId)
        {
            dynamic result = JsonExtentions.HttpActionToAPI(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetMainMenuAllByRoleId" + "?FactoryCode=" + factoryCode + "&roleid=" + roleId, string.Empty);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }
    }
}