using PMTs.DataAccess.Extentions;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Shared;
using System;

namespace PMTs.DataAccess.Repository
{
    public class PresaleChangeRoutingAPIRepository : IPresaleChangeRoutingAPIRepository
    {
        private readonly string _actionName = "PresaleChangeRouting";

        public string GetAllPresaleChangeRoutings(string factoryCode, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "?FactoryCode=" + factoryCode, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetPresaleChangeRoutingsByMaterialNo(string factoryCode, string materialNo, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetPresaleChangeRoutingsByMaterialNo" + "?FactoryCode=" + factoryCode + "&MaterialNo=" + materialNo, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public void UpdatePresaleRoutings(string factoryCode, string jsonString, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/UpdatePresaleRoutings" + "?FactoryCode=" + factoryCode, jsonString, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        //public void CreatePresaleChangeRouting(string factoryCode, string jsonString)
        //{
        //    dynamic result = JsonExtentions.HttpActionToAPI(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "?AppName=" + Globals.AppNameEncrypt, jsonString);

        //    if (!result.Item1)
        //    {
        //        throw new Exception(result.Item2);
        //    }
        //}

        //public void UpdatePresaleChangeRouting(string factoryCode, string jsonString)
        //{
        //    dynamic result = JsonExtentions.HttpActionToAPI(HTTPAction.PUT.ToString(), Globals.WebAPIUrl + _actionName + "?AppName=" + Globals.AppNameEncrypt, jsonString);

        //    if (!result.Item1)
        //    {
        //        throw new Exception(result.Item2);
        //    }
        //}

        //public void DeletePresaleChangeRouting(string factoryCode, string jsonString)
        //{
        //    dynamic result = JsonExtentions.HttpActionToAPI(HTTPAction.DELETE.ToString(), Globals.WebAPIUrl + _actionName + "?AppName=" + Globals.AppNameEncrypt, jsonString);

        //    if (!result.Item1)
        //    {
        //        throw new Exception(result.Item2);
        //    }
        //}
    }
}
