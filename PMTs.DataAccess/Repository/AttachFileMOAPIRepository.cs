using PMTs.DataAccess.Extentions;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Shared;
using System;

namespace PMTs.DataAccess.Repository
{
    public class AttachFileMOAPIRepository : IAttachFileMOAPIRepository
    {
        private readonly string _actionName = "AttachFileMO";

        public string GetAttachFileMOsByOrderItem(string factoryCode, string orderItem, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetAttachFileMOsByOrderItem" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode + "&OrderItem=" + orderItem, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetAttachFileMOsByOrderItems(string factoryCode, string saleOrders, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/GetAttachFileMOsByOrderItems" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode, saleOrders, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }
        public string GetAttachFileMOsByOrderItemsAndFactoryCode(string factoryCode, string saleOrders, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/GetAttachFileMOsByOrderItemsAndFactoryCode" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode, saleOrders, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public void SaveAttachFileMO(string jsonString, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "?AppName=" + Globals.AppNameEncrypt, jsonString, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public void UpdateAttachFileMO(string jsonString, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.PUT.ToString(), Globals.WebAPIUrl + _actionName + "?AppName=" + Globals.AppNameEncrypt, jsonString, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public void DeleteAttachFileMO(string jsonString, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.DELETE.ToString(), Globals.WebAPIUrl + _actionName + "?AppName=" + Globals.AppNameEncrypt, jsonString, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetAttachFileMOByFileName(string factoryCode, string orderItem, string fileName, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetAttachFileMOByFileName" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode + "&OrderItem=" + orderItem + "&FileName=" + fileName, string.Empty, token);

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
