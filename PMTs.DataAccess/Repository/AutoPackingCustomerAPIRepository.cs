using PMTs.DataAccess.Extentions;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Shared;
using System;

namespace PMTs.DataAccess.Repository
{
    public class AutoPackingCustomerAPIRepository : IAutoPackingCustomerAPIRepository
    {
        private readonly string _actionName = "AutoPackingCustomer";

        public string GetAutoPackingCustomers(string factoryCode, string token)
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

        public string GetAutoPackingCustomerByCusId(string factoryCode, string cusId, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetAutoPackingCustomerByCusID" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode + "&CusId=" + cusId, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public void SaveAutoPackingCustomer(string factoryCode, string jsonString, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode, jsonString, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public void UpdateAutoPackingCustomer(string factoryCode, string jsonString, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/UpdateAutoPackingCustomer" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode, jsonString, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public void DeleteAutoPackingCustomer(string factoryCode, string jsonString, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.DELETE.ToString(), Globals.WebAPIUrl + _actionName + "?AppName=" + Globals.AppNameEncrypt, jsonString, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetAutoPackingCustomerDataByKeySearch(string factoryCode, string ddlSearch, string txtSearch, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetAutoPackingCustomerDataByKeySearch" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode + "&TypeSearch=" + ddlSearch + "&KeySearch=" + txtSearch, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetAllAutoPackingCustomerAndCustomer(string FactoryCode, string KeySearch, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetAllAutoPackingCustomerAndCustomer" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + FactoryCode + "&KeySearch=" + KeySearch, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetAutoPackingCustomerAndCustomerByCustName(string FactoryCode, string CustName, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetAutoPackingCustomerAndCustomerByCustName" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + FactoryCode + "&CustName=" + CustName, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetAutoPackingCustomerAndCustomerByCustCode(string FactoryCode, string CustCode, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetAutoPackingCustomerAndCustomerByCustCode" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + FactoryCode + "&CustCode=" + CustCode, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetAutoPackingCustomerAndCustomerByCusId(string FactoryCode, string CusId, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetAutoPackingCustomerAndCustomerByCusId" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + FactoryCode + "&CusId=" + CusId, string.Empty, token);

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
