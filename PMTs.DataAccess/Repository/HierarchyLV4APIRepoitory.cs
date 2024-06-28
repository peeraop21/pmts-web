using PMTs.DataAccess.Extentions;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Shared;
using System;

namespace PMTs.DataAccess.Repository
{
    public class HierarchyLv4APIRepository : IHierarchyLv4APIRepository
    {
        private readonly string _actionName = "HierarchyLV4";

        public string GetAllHierarchyLv4s(string factoryCode, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public void SaveHierarchy4(string factoryCode, string hierarchyLv4Json, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "?AppName=" + Globals.AppNameEncrypt, hierarchyLv4Json, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public void UpdateHierarchy4(string factoryCode, string hierarchyLv4Json, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.PUT.ToString(), Globals.WebAPIUrl + _actionName + "?AppName=" + Globals.AppNameEncrypt, hierarchyLv4Json, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }
    }
}
