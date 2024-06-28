using PMTs.DataAccess.Extentions;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Shared;
using System;

namespace PMTs.DataAccess.Repository
{
    public class ProductCatalogCofigRepository : IProductCatalogCofigRepository
    {
        private readonly string _actionName = "ProductCatalogConfig";


        public string GetProductGroupList(string factoryCode, string Username, string Model, string token)
        {
            // dynamic resultxx = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl+ "Login"  +"/GetUser" +"?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode + "&Username=" + Username, string.Empty);
            //MasterUser mods = new MasterUser();
            //dynamic resulttt = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + "Login" + "/GetAdmin" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode + "&Username=" + Username, JsonConvert.SerializeObject(mods));

            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "?FactoryCode=" + factoryCode + "&Username=" + Username, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string UpdateroductGroupList(string factoryCode, string Username, string Model, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "?FactoryCode=" + factoryCode + "&Username=" + Username, Model, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string UpdateProductCatalogRemark(string factoryCode, string Model, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/SaveProductCatalogRemark" + "?FactoryCode=" + factoryCode, Model, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetProductCatalogRemark(string factoryCode, string PC, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetProductCatalogRemark" + "?FactoryCode=" + factoryCode + "&PC=" + PC, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }


        public string GetHoldMaterialByMaterial(string factoryCode, string Material, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetHoldMaterialByMaterial" + "?FactoryCode=" + factoryCode + "&Material=" + Material, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string SaveHoldMaterial(string factoryCode, string Model, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/SaveHoldMaterial" + "?FactoryCode=" + factoryCode, Model, token);

            if (result.Item1)
            {

                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string UpdateHoldMaterial(string factoryCode, string Model, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/UpdateHoldMaterial" + "?FactoryCode=" + factoryCode, Model, token);

            if (result.Item1)
            {

                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetHoldMaterialHistoryByMaterial(string factoryCode, string Material, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetHoldMaterialHistoryByMaterial" + "?FactoryCode=" + factoryCode + "&Material=" + Material, string.Empty, token);

            if (result.Item1)
            {

                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string SaveHoldMaterialHistory(string factoryCode, string Model, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/SaveHoldMaterialHistory" + "?FactoryCode=" + factoryCode, Model, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetOrderItemByMoData(string factoryCode, string Material, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetOrderItemByMoData" + "?FactoryCode=" + factoryCode + "&Material=" + Material, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string PresaleHoldMaterial(string config, string model)
        {
            // dynamic Res_CallPresale = JsonExtentions.PresaleApiConnect(HTTPAction.POST.ToString(), Globals.PresaleHoldMat, model);
            JsonExtentions.PresaleApiConnect(config, model);
            return "Success";
        }

        public string GetScalePriceMatProduct(string factoryCode, string custId, string custName, string custCode, string pc1, string pc2, string pc3, string materialType, string salePlants, string plantPdts, string materialNo, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetScalePriceMatProduct" + "?FactoryCode=" + factoryCode + "&CustId=" + custId + "&CustName=" + custName + "&CustCode=" + custCode + "&Pc1=" + pc1 + "&Pc2=" + pc2 + "&Pc3=" + pc3 + "&MaterialType=" + materialType + "&salePlants=" + salePlants + "&plantPdts=" + plantPdts + "&MaterialNo=" + materialNo, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetBOMMaterialProduct(string factoryCode, string custId, string custName, string custCode, string pc1, string pc2, string pc3, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetBOMMaterialProduct" + "?FactoryCode=" + factoryCode + "&CustId=" + custId + "&CustName=" + custName + "&CustCode=" + custCode + "&Pc1=" + pc1 + "&Pc2=" + pc2 + "&Pc3=" + pc3, string.Empty, token);

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
