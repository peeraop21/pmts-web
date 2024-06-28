using PMTs.DataAccess.Extentions;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Shared;
using System;

namespace PMTs.DataAccess.Repository
{
    public class RoutingAPIRepository : IRoutingAPIRepository
    {
        private readonly string _actionName = "Routing";
        string route;

        public string GetRoutingList(string factoryCode, string token)
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

        public string GetDapperRoutingByMatList(string factoryCode, string Condition, string token)
        {
            route = _actionName + "/GetDapperRoutingByMat";
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + route + "?FactoryCode=" + factoryCode, Condition, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetRoutingsByMaterialNo(string factoryCode, string materialNo, string token)
        {
            route = _actionName + "/GetRoutingsByMaterialNo";

            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + route + "?FactoryCode=" + factoryCode + "&MaterialNo=" + materialNo, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }
        public string GetRoutingsByMaterialNoContain(string factoryCode, string materialNo, string token)
        {
            route = _actionName + "/GetRoutingsByMaterialNoContain";

            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + route + "?FactoryCode=" + factoryCode + "&MaterialNo=" + materialNo, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }
        public string GetRoutingByMaterialNoAndMachine(string factoryCode, string materialNo, string machine, string token)
        {
            route = _actionName + "/GetRoutingByMaterialNoAndMachine";

            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + route + "?FactoryCode=" + factoryCode + "&MaterialNo=" + materialNo + "&Machine=" + machine, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public void SaveRouting(string factoryCode, string materialNo, string jsonString, string token)//,string MaterialNo)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "?FactoryCode=" + factoryCode + "&Materialno=" + materialNo, jsonString, token);

            //    dynamic result = JsonExtentions.HttpActionToAPI(HTTPAction.POST.ToString(), Globals.WebAPIUrl + route + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode + "&Flute=" + flute, string.Empty);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public void UpdateRouting(string factoryCode, string jsonString, string token)//,string MaterialNo)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.PUT.ToString(), Globals.WebAPIUrl + _actionName + "?FactoryCode=" + factoryCode, jsonString, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public void Update1RowOfRouting(string factoryCode, string jsonString, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.PUT.ToString(), Globals.WebAPIUrl + _actionName + "/Update1RowOfRouting" + "?FactoryCode=" + factoryCode, jsonString, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public void DeleteRouting(string factoryCode, string jsonString, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.DELETE.ToString(), Globals.WebAPIUrl + _actionName + "?FactoryCode=" + factoryCode, jsonString, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public void UpdateRoutingPDISStatus(string factoryCode, string Material, string Status, string token)//,string MaterialNo)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/UpdateRoutingPDISStatus" + "?FactoryCode=" + factoryCode + "&MaterialNo=" + Material + "&Status=" + Status, string.Empty, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetNumberOfRoutingByShipBlk(string factoryCode, string materialNo, bool semiBlk, string token)
        {
            route = _actionName + "/GetNumberOfRoutingByShipBlk";

            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + route + "?FactoryCode=" + factoryCode
                + "&MaterialNo=" + materialNo + "&SemiBlk=" + semiBlk, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public void DeleteRoutingByMaterialNoAndFactory(string factoryCode, string materialNo, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/DeleteRoutingByMatAndFac" + "?FactoryCode=" + factoryCode + "&MaterialNo=" + materialNo, string.Empty, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }

        }

        public void DeleteRoutingByMaterialNoAndFactoryAndSeq(string factoryCode, string materialNo, string Seq, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/DeleteRoutingByMatAndFacAndSeq" + "?FactoryCode=" + factoryCode + "&MaterialNo=" + materialNo + "&Seq=" + Seq, string.Empty, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }

        }

        public string GetRoutingsByMaterialNos(string factoryCode, string materialNos, string token)
        {
            route = _actionName + "/GetRoutingsByMaterialNos";
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + route + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode, materialNos, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public void UpdateRoutings(string factoryCode, string routingsJson, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/UpdateRoutings" + "?FactoryCode=" + factoryCode, routingsJson, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public string UpdateReCalculateTrimFromFile(string factoryCode, string reCalculateTrimJson, string token)
        {

            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/UpdateReCalculateTrimFromFile" + "?FactoryCode=" + factoryCode, reCalculateTrimJson, token);

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
