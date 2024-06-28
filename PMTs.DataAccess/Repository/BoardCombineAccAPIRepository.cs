using PMTs.DataAccess.Extentions;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Shared;
using System;

namespace PMTs.DataAccess.Repository
{
    public class BoardCombineAccAPIRepository : IBoardCombineAccAPIRepository
    {
        private readonly string _actionName = "BoardCombineAcc";

        public string GetBoardCombineAccList(string factoryCode, string token)
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

        public string GetCost(string factoryCode, string code, string costField, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetCost" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode + "&Code=" + code + "&CostField=" + costField, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                //throw new Exception(result.Item2);
                return Convert.ToString(result.Item3);
            }
        }

        public void AddBoardCombineAccColumn(string factoryCode, string costField, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/AddBoardCombineAccColumn" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode + "&CostField=" + costField, string.Empty, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public void ChangeBoardCombineAccColumn(string factoryCode, string OldCostField, string newCostField, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/ChangeBoardCombineAccColumn" + "?AppName=" + Globals.AppNameEncrypt + "&OldCostField=" + OldCostField + "&NewCostField=" + newCostField, string.Empty, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public string ImportBoardCombineAcc(string factoryCode, string userName, string jsonString, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/ImportBoardCombineAcc" + "?FactoryCode=" + factoryCode + "&UserName=" + userName, jsonString, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }

            return result.Item3;
        }

        public void DropBoardCombineAccColumn(string factoryCode, string costField, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/DropBoardCombineAccColumn" + "?AppName=" + Globals.AppNameEncrypt + "&CostField=" + costField, string.Empty, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }



        public string GetBoardCombineAcc(string factoryCode, string code, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetBoardcombindAcc" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode + "&Code=" + code, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string UpdateBoardCombineAcc(string factoryCode, string jsonString, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/UpdateBoardcombindAcc" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode, jsonString, token);

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
