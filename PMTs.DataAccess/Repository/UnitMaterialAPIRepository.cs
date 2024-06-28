using PMTs.DataAccess.Extentions;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Shared;
using System;

namespace PMTs.DataAccess.Repository
{
    public class UnitMaterialAPIRepository : IUnitMaterialAPIRepository
    {
        private static readonly string _actionName = "UnitMaterial";

        public string GetUnitMaterialList(string factoryCode, string token)
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

        public string GetUnitMaterialByName(string Name, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetUnitMaterialByName" + "?MaterialName=" + Name, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetUnitMaterialById(string factoryCode, int id, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetUnitMaterialById" + "?FactoryCode=" + factoryCode + "&id=" + id, string.Empty, token);

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
