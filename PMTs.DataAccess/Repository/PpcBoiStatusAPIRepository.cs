using PMTs.DataAccess.Extentions;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Shared;
using System;

namespace PMTs.DataAccess.Repository
{
    public class PpcBoiStatusAPIRepository : IPpcBoiStatusAPIRepository
    {
        private readonly string _actionName = "PpcBoiStatus";
        public void DeletePpcBoiStatus(string jsonString, string token)
        {
            throw new NotImplementedException();
        }

        public string GetPpcBoiStatusList(string factoryCode, string token)
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

        public void SavePpcBoiStatus(string factoryCode, string jsonString, string token)
        {
            throw new NotImplementedException();
        }

        public void UpdatePpcBoiStatus(string factoryCode, string jsonString, string token)
        {
            throw new NotImplementedException();
        }
    }
}
