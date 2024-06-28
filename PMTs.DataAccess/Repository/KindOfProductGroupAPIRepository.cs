using PMTs.DataAccess.Extentions;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Shared;
using System;

namespace PMTs.DataAccess.Repository
{
    public class KindOfProductGroupAPIRepository : IKindOfProductGroupAPIRepository
    {
        private static readonly string _actionName = "KindOfProductGroup";

        public string GetKindOfProductGroupList(string factoryCode, string token)
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

        public string GetKindOfProductGroupById(string factoryCode, int Id, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetKindOfProductGroupById" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode + "&Id=" + Id, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public void CreateKindOfProductGroup(string factoryCode, string jsonString, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "?AppName=" + Globals.AppNameEncrypt, jsonString, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public void UpdateKindOfProductGroup(string factoryCode, string jsonString, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.PUT.ToString(), Globals.WebAPIUrl + _actionName + "?AppName=" + Globals.AppNameEncrypt, jsonString, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public void DeleteKindOfProductGroup(string factoryCode, string jsonString, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.DELETE.ToString(), Globals.WebAPIUrl + _actionName + "?AppName=" + Globals.AppNameEncrypt, jsonString, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetKindOfProductGroupsByIds(string factoryCode, string idKindOfProductGroups, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/GetKindOfProductGroupsByIds" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode, idKindOfProductGroups, token);

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
