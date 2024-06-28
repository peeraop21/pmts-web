using PMTs.DataAccess.Extentions;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Shared;
using System;

namespace PMTs.DataAccess.Repository
{
    public class QaItemsAPIRepository : IQaItemsAPIRepository
    {
        private readonly string _actionName = "QaItems";

        public string GetQaItems(string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "?FactoryCode=\"\"", string.Empty, token);
            if (result.Item1)
            {
                return result.Item3;
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }
    }
}
