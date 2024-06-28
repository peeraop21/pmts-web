using PMTs.DataAccess.Extentions;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Shared;
using System;

namespace PMTs.DataAccess.Repository
{
    public class DocumentSRepository : IDocumentSRepository
    {
        private readonly string _actionName = "DocumentS";

        public string GetDocumentS(string factoryCode, string snumber, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetDocumentS?FactoryCode=" + factoryCode + "&SNumber=" + snumber, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string CreateNewDocumentS(string factoryCode, string username, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetDocumentSNew?FactoryCode=" + factoryCode + "&Username=" + username, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }


        public string GetDocumentSList(string factoryCode, string orderitem, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetDocumentSList?FactoryCode=" + factoryCode + "&Snumber=" + orderitem, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetDataMo(string factoryCode, string orderitem, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetDataByMo?FactoryCode=" + factoryCode + "&OrderItem=" + orderitem, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public void SaveDocumentS(string factorycode, string jsonString, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/SaveDocumentS?FactoryCode=" + factorycode, jsonString, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public void UpdateDocumentS(string factorycode, string jsonString, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.PUT.ToString(), Globals.WebAPIUrl + _actionName + "/UpdateDocumentS?FactoryCode=" + factorycode, jsonString, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public void DeleteDocumentS(string factorycode, string id, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.DELETE.ToString(), Globals.WebAPIUrl + _actionName + "/DeleteDocumentS?FactoryCode=" + factorycode + "&Id=" + id, string.Empty, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetDocumentSReport(string factoryCode, string snumber, string usercreate, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/ReportDocumentS?FactoryCode=" + factoryCode + "&SNumber=" + snumber + "&UserCreate=" + usercreate, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string SearchDocumentsAndMODataByOrderItem(string factoryCode, string sNumber, string orderItem, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetDocumentsAndMODataByOrderItem?FactoryCode=" + factoryCode + "&SNumber=" + sNumber + "&OrderItem=" + orderItem, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public void SaveChangeDocuments(string factoryCode, string jsonDocumentSlist, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.PUT.ToString(), Globals.WebAPIUrl + _actionName + "/SaveChangeDocumentS?FactoryCode=" + factoryCode, jsonDocumentSlist, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetDocumentSListForReportDocument(string factoryCode, string materialNo, string customerName, string pc, string so, string token)
        {

            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetDocumentSListForReportDocument?FactoryCode=" + factoryCode + "&MaterialNO=" + materialNo + "&CustName=" + customerName + "&PC=" + pc + "&SO=" + so, string.Empty, token);

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
