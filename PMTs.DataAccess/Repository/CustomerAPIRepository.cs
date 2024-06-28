using PMTs.DataAccess.Extentions;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Shared;
using System;

namespace PMTs.DataAccess.Repository
{
    public class CustomerAPIRepository : ICustomerAPIRepository
    {
        private readonly string _actionName = "Customer";

        public string GetCustomerList(string factoryCode, string token)
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

        public string GetCustomersByCustomerGroup(string factoryCode, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetCustomersByCustomerGroup" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetCustomerShipTo(string factoryCode, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetCustomerShipTo" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }
        public string GetMasterDataByKeySearch(string factoryCode, string typeSearch, string keySearch, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetMasterDataByKeySearch" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode + "&typeSearch=" + typeSearch + "&keySearch=" + keySearch, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetCustomerByCusID(string factoryCode, string cusID, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetCustomerByCusID" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode + "&cusID=" + cusID, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetCustomerByCusIDAndCustName(string factoryCode, string cusID, string custName, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetCustomerByCusIDAndCustName" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode + "&cusID=" + cusID + "&custName=" + custName, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetCustomerById(string factoryCode, int Id, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetCustomerById" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode + "&Id=" + Id, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }


        public void SaveCustomer(string jsonString, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "?AppName=" + Globals.AppNameEncrypt, jsonString, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public void UpdateCustomer(string jsonString, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.PUT.ToString(), Globals.WebAPIUrl + _actionName + "?AppName=" + Globals.AppNameEncrypt, jsonString, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public void DeleteCustomer(string jsonString, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.DELETE.ToString(), Globals.WebAPIUrl + _actionName + "?AppName=" + Globals.AppNameEncrypt, jsonString, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }
        public string GetCustomerShipToByCustname(string factoryCode, string CustName, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetCustomerByCusName" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode + "&CustName=" + CustName, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }
        public string GetCustomerShipToByCustCode(string factoryCode, string cusCode, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetCustomerByCusCode" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode + "&CustCode=" + cusCode, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }
        public string GetCustomerShipToByCustId(string factoryCode, string cusId, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetCustomerByCustID" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode + "&CusID=" + cusId, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public void DeleteCustomerByID(string Factorycode, int ID, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/DeleteCustomerByID" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + Factorycode + "&ID=" + ID, string.Empty, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetCustomersByCusID(string factoryCode, string cusID, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetCustomersByCusId" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode + "&CusId=" + cusID, string.Empty, token);

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
