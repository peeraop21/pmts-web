using PMTs.DataAccess.ComplexModel;
using PMTs.DataAccess.Extentions;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Shared;
using System;

namespace PMTs.DataAccess.Repository
{
    public class PresaleChangeProductAPIRepository : IPresaleChangeProductAPIRepository
    {
        private readonly string _actionName = "PresaleChangeProduct";

        public string GetPresaleChangeProductByMaterialNo(string factoryCode, string materialNo, string psmId, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetPresaleChangeProductByMaterialNo" + "?FactoryCode=" + factoryCode + "&MaterialNo=" + materialNo + "&PSMId=" + psmId, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetAllPresaleChangeProducts(string factoryCode, string token)
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

        public string GetPresaleChangeProductsByActiveStatus(string factoryCode, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetPresaleChangeProductsByActiveStatus" + "?FactoryCode=" + factoryCode , string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }
        public string GetPresaleChangeProductsByMaterialNo(string factoryCode, string materialNo, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetPresaleChangeProductsByMaterialNo" + "?FactoryCode=" + factoryCode + "&MaterialNo=" + materialNo, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public void UpdatePresaleChangeProductStatusById(string factoryCode, int id, string status, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/UpdatePresaleChangeProductStatusById" + "?FactoryCode=" + factoryCode + "&Id=" + id + "&Status=" + status, string.Empty, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public void UpdatePresaleChangeProduct(string factoryCode, string jsonString, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.PUT.ToString(), Globals.WebAPIUrl + _actionName + "?FactoryCode=" + factoryCode, jsonString, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetPresaleChangeProductsByKeySearch(string factoryCode, string typeSearch, string keySearch, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetPresaleChangeProductsByKeySearch" + "?FactoryCode=" + factoryCode + "&TypeSearch=" + typeSearch + "&KeySearch=" + keySearch, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        //public void DeletePresaleChangeProduct(string factoryCode, string jsonString)
        //{
        //    dynamic result = JsonExtentions.HttpActionToAPI(HTTPAction.DELETE.ToString(), Globals.WebAPIUrl + _actionName + "?AppName=" + Globals.AppNameEncrypt, jsonString);

        //    if (!result.Item1)
        //    {
        //        throw new Exception(result.Item2);
        //    }
        //}

    }
}
