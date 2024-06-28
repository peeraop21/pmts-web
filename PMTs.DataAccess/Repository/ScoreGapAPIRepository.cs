using PMTs.DataAccess.Extentions;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Shared;
using System;

namespace PMTs.DataAccess.Repository
{
    public class ScoreGapAPIRepository : IScoreGapAPIRepository
    {
        private readonly string _actionName = "ScoreGap";

        public string GetScoreGapList(string factoryCode, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetSCoreGapList" + "?FactoryCode=" + factoryCode, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetScoreGapById(int Id, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetScoreGapById" + "?id=" + Id, string.Empty, token); ;
            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public void SaveScoreGap(string factoryCode, string jsonString, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "?FactoryCode=" + factoryCode, jsonString, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public void UpdateScoreGap(string factoryCode, string jsonString, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.PUT.ToString(), Globals.WebAPIUrl + _actionName + "?FactoryCode=" + factoryCode, jsonString, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public void DeleteScoreGap(string jsonString, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.DELETE.ToString(), Globals.WebAPIUrl + _actionName, jsonString, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetScoreGapsByFactoryCode(string factoryCode, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetScoreGapsByFactoryCode" + "?FactoryCode=" + factoryCode, string.Empty, token); ;
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