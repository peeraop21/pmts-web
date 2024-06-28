using PMTs.DataAccess.Extentions;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Shared;
using System;

namespace PMTs.DataAccess.Repository
{
    public class PlantViewAPIRepository : IPlantViewAPIRepository
    {
        private readonly string _actionName = "PlantView";

        public string GetPlantViewList(string factoryCode, string token)
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

        public string GetPlantViewByMaterialNo(string factoryCode, string materialNo, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetPlantViewByMaterialNo" + "?FactoryCode=" + factoryCode + "&MaterialNo=" + materialNo, string.Empty, token);

            if (result.Item1)
            {
                return result.Item3;
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetPlantViewByMaterialNoAndPlant(string factoryCode, string materialNo, string plant, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetPlantViewByMaterialNoAndPlant" + "?FactoryCode=" + factoryCode + "&MaterialNo=" + materialNo + "&Plant=" + plant, string.Empty, token);

            if (result.Item1)
            {
                return result.Item3;
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetPlantViewsByMaterialNo(string factoryCode, string materialNo, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetPlantViewsByMaterialNo" + "?FactoryCode=" + factoryCode + "&MaterialNo=" + materialNo, string.Empty, token);
            //dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetPlantViewsByMaterialNo" + "?AppName=" + Globals.AppNameEncrypt  + "&MaterialNo=" + materialNo, string.Empty);


            if (result.Item1)
            {
                return result.Item3;
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public void SavePlantView(string jsonString, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName, jsonString, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public void UpdatePlantView(string jsonString, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.PUT.ToString(), Globals.WebAPIUrl + _actionName, jsonString, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public void DeletePlantView(string jsonString, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.DELETE.ToString(), Globals.WebAPIUrl + _actionName, jsonString, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetPlantViewByPlant(string factoryCode, string materialNo, string plant, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetPlantViewByPlant" + "?FactoryCode=" + factoryCode + "&MaterialNo=" + materialNo + "&Plant=" + plant, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public void UpdatePlantViewShipBlk(string FactoryCode, string MaterialNo, string Status, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/UpdatePlantViewShipBlk" + "?FactoryCode=" + FactoryCode + "&MaterialNo=" + MaterialNo + "&Status=" + Status, string.Empty, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetReusePlantViewsByMaterialNos(string factoryCode, string materialNos, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/GetReusePlantViewsByMaterialNos" + "?FactoryCode=" + factoryCode, materialNos, token);

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
