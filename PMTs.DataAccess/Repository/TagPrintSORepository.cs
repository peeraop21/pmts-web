using PMTs.DataAccess.Extentions;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Shared;
using System;

namespace PMTs.DataAccess.Repository
{
    public class TagPrintSORepository : ITagPrintSORepository
    {
        private readonly string _actionName = "TagPrintSO";


        public string GetTagPrintSO(string factoryCode, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetTagPrintSO" + "?FactoryCode=" + factoryCode, string.Empty, token);


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
