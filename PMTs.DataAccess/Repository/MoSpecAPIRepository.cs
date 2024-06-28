using PMTs.DataAccess.Extentions;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Shared;
using System;

namespace PMTs.DataAccess.Repository
{
    public class MoSpecAPIRepository : IMoSpecAPIRepository
    {
        private readonly string _actionName = "MoSpec";

        public string GetMoSpecList(string factoryCode, string token)
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

        public void SaveMoSpec(string jsonString, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName, jsonString, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public void UpdateMoSpec(string jsonString, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.PUT.ToString(), Globals.WebAPIUrl + _actionName, jsonString, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }
        //tassanai update 

        public void UpdateMoSpecChangestring(string factoryCode, string orderItem, string changeInfo, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/UpdateMospecChangeInfo" + "?FactoryCode=" + factoryCode + "&OrderItem=" + orderItem + "&ChangeInfo=" + changeInfo, string.Empty, token);


            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }
        public void DeleteMoSpec(string jsonString, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.DELETE.ToString(), Globals.WebAPIUrl + _actionName, jsonString, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetMoSpecBySaleOrder(string factoryCode, string orderItem, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetMoSpecBySaleOrder" + "?FactoryCode=" + factoryCode + "&OrderItem=" + orderItem, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetMoSpecByOrderItem(string factoryCode, string orderItem, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetMoSpecByOrderItem" + "?FactoryCode=" + factoryCode + "&OrderItem=" + orderItem, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetMOSpecsBySaleOrders(string factoryCode, string saleOrders, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/GetMOSpecsBySaleOrders" + "?FactoryCode=" + factoryCode, saleOrders, token);

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
