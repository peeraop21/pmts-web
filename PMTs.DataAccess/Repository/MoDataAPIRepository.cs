using Newtonsoft.Json;
using PMTs.DataAccess.ComplexModel;
using PMTs.DataAccess.Extentions;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Shared;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PMTs.DataAccess.Repository
{
    public class MoDataAPIRepository : IMoDataAPIRepository
    {
        private readonly string _actionName = "MoData";

        public string GetMoDataList(string factoryCode, string token)
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

        public void SaveMoData(string jsonString, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName, jsonString, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public void UpdateMoData(string jsonString, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.PUT.ToString(), Globals.WebAPIUrl + _actionName, jsonString, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public void DeleteMoData(string jsonString, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.DELETE.ToString(), Globals.WebAPIUrl + _actionName, jsonString, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetMoDataListBySaleOrder(string factoryCode, string stratSO, string endSO, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetMoDataListBySaleOrder" + "?FactoryCode=" + factoryCode + "&StratSO=" + stratSO + "&EndSO=" + endSO, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetMoDataListBySaleOrderNonX(string factoryCode, string stratSO, string endSO, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetMODataListBySONonX" + "?FactoryCode=" + factoryCode + "&StratSO=" + stratSO + "&EndSO=" + endSO, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetMoDataListBySearchTypeNonX(string factoryCode, string searchType, string searchText, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetMoDataListBySearchTypeNonX" + "?FactoryCode=" + factoryCode + "&SearchType=" + searchType + "&SearchText=" + searchText, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public static string ConvertStringToHex(String input, System.Text.Encoding encoding)
        {
            Byte[] stringBytes = encoding.GetBytes(input);
            var sbBytes = new StringBuilder(stringBytes.Length * 2);
            foreach (byte b in stringBytes)
            {
                sbBytes.AppendFormat("{0:X2}", b);
            }
            return sbBytes.ToString();
        }

        public string GetMoDataListBySaleOrders(string factoryCode, string saleOrders, string token)
        {
            // dynamic resulst = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/Test" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode + "&SaleOrders=", saleOrders);
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/GetMoDataListBySaleOrders" + "?FactoryCode=" + factoryCode + "&SaleOrders=", saleOrders, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetMoDataListBySaleOrdersByDapper(string factoryCode, string saleOrders, string token)
        {
            // dynamic resulst = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/Test" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode + "&SaleOrders=", saleOrders);
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/GetMoDataListBySaleOrdersByDapper" + "?FactoryCode=" + factoryCode + "&SaleOrders=", saleOrders, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetMoDataBySaleOrderNonX(string factoryCode, string saleOrder, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetMoDataBySaleOrderNonX" + "?FactoryCode=" + factoryCode + "&SaleOrder=" + saleOrder, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetMoDatasBySaleOrderNonX(string factoryCode, string saleOrder, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetMoDatasBySaleOrderNonX" + "?FactoryCode=" + factoryCode + "&SaleOrder=" + saleOrder, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string UpdateMoDataSentKIWI(string factoryCode, string saleOrder, string User, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/UpdateMoDataSentKIWI" + "?FactoryCode=" + factoryCode + "&SaleOrder=" + saleOrder + "&UserBy=" + User, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string CreateMODataFromExcelFile(string factoryCode, string username, string jsonModel, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/CreateMODataFromExcelFile" + "?FactoryCode=" + factoryCode + "&Username=" + username, jsonModel, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        private static (bool IsSuccess, string ExceptionMessage, string Data) CallApi(string URL, string method)
        {
            bool isSuccess;

            string fullPath = URL;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(fullPath);
            request.Method = method;
            request.KeepAlive = true;
            request.ContentType = "appication/json";
            request.Headers.Add("Content-Type", "appication/json");

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using var sr = new StreamReader(response.GetResponseStream());

            dynamic myResponse = sr.ReadToEnd();
            isSuccess = Convert.ToBoolean(myResponse.isSuccess);
            string exceptionMessage = myResponse.exceptionMessage.ToString();
            dynamic data = myResponse.data.ToString();
            return (isSuccess, exceptionMessage, data);
        }

        public string CreateMOManual(string moDatas, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/CreateMOManual", moDatas, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string ReportCheckRepeatOrder(string factoryCode, string dateFrom, string dateTo, string repeatCount, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/ReportCheckRepeatOrder" + "?FactoryCode=" + factoryCode + "&dateFrom=" + dateFrom + "&dateTo=" + dateTo + "&repeatCount=" + repeatCount, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string ReportCheckOrderQtyTooMuch(string factoryCode, string dateFrom, string dateTo, string repeatCount, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/ReportCheckOrderQtyTooMuch" + "?FactoryCode=" + factoryCode + "&dateFrom=" + dateFrom + "&dateTo=" + dateTo + "&repeatCount=" + repeatCount, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string ReportCheckDiffDueDate(string FactoryCode, int datediff, string dateFrom, string dateTo, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/CheckDiffDueDate" + "?FactoryCode=" + FactoryCode + "&datediff=" + datediff + "&dateFrom=" + dateFrom + "&dateTo=" + dateTo, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string ReportCheckDueDateToolong(string FactoryCode, int dayCount, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/CheckDueDateToolong" + "?FactoryCode=" + FactoryCode + "&dayCount=" + dayCount, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetMoDatasByDueDateRange(string factoryCode, string startDueDate, string endDueDate, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetMoDatasByDueDateRange" + "?FactoryCode=" + factoryCode + "&StartDueDate=" + startDueDate + "&EndDueDate=" + endDueDate, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetReportCheckOrderItem(string Username, string factoryCode, string startDueDate, string endDueDate, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/GetReportCheckData" + "?Username=" + Username + "&FactoryCode=" + factoryCode + "&StartDate=" + startDueDate + "&EndDate=" + endDueDate, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetMoDatasByDueDateRangeAndStatus(string factoryCode, string status, string startDueDate, string endDueDate, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetMoDatasByDueDateRangeAndStatus" + "?FactoryCode=" + factoryCode + "&Status=" + status + "&StartDueDate=" + startDueDate + "&EndDueDate=" + endDueDate, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        //boo editblock platen
        public string GetBlockPlatenMaster(string factorycode, string material, string pc, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetBlockPlatenMaster" + "?factorycode=" + factorycode + "&material=" + material + "&pc=" + pc, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetBlockPlatenRouting(string factorycode, string material, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetBlockPlatenRouting" + "?factorycode=" + factorycode + "&material=" + material, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string UpdateBlockPlatenRouting(string factorycode, string username, string model, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/UpdateBlockPlatenRouting" + "?factorycode=" + factorycode + "&username=" + username, model, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string ReportMOManual(string factoryCode, string materialNo, string custname, string pc, string startDueDate, string endDueDate, string startCreateDate, string endCreateDate, string startUpdateDate, string endUpdateDate, string po, string so, string note, string soStatus, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/ReportMOManual" + "?FactoryCode=" + factoryCode + "&MaterialNo=" + materialNo + "&CustName=" + custname + "&PC=" + pc + "&StartDueDate=" + startDueDate + "&EndDueDate=" + endDueDate + "&StartCreateDate=" + startCreateDate + "&EndCreateDate=" + endCreateDate + "&StartUpdateDate=" + startUpdateDate + "&EndUpdateDate=" + endUpdateDate + "&PO=" + po + "&SO=" + so + "&Note=" + note + "&SoStatus=" + soStatus, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string SearchMoDataListBySaleOrderNonXAndH(string factoryCode, string startSO, string endSO, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/SearchMODataListBySONonXAndH" + "?FactoryCode=" + factoryCode + "&StratSO=" + startSO + "&EndSO=" + endSO, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetMasterCardMOsBySaleOrders(string factoryCode, string jsonString, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/GetMasterCardMOsBySaleOrders" + "?FactoryCode=" + factoryCode, jsonString, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetBaseOfMasterCardMOsBySaleOrders(string factoryCode, bool isUserTIPs, string jsonString, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/GetBaseOfMasterCardMOsBySaleOrders" + "?FactoryCode=" + factoryCode + "&isUserTIPs=" + isUserTIPs, jsonString, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetMoDataListBySaleOrderNonXAndH(string factoryCode, string startSO, string endSO, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetMODataListBySONonXAndH" + "?FactoryCode=" + factoryCode + "&StratSO=" + startSO + "&EndSO=" + endSO, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string CheckMaterialNo(string factoryCode, string materialNo, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/CheckMaterialNo" + "?FactoryCode=" + factoryCode + "&MaterialNo=" + materialNo, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public async Task<PlaningMOModel> GetDataOfMOFromTIPs(PlaningMOModel planingMOModel, UserTIP userTIP, string orderItems)
        {
            #region OldVersion

            ////For TEST
            //var testOrderItems = new List<string>();
            //testOrderItems.Add("4380638302");
            //testOrderItems.Add("4380638303");
            //orderItems = JsonConvert.SerializeObject(testOrderItems);
            ////
            ///
            //var httpWebRequest = (HttpWebRequest)WebRequest.Create(userTIP.UrlApi + "/tips-service/Interface/GetPlanningDataForPMTs");
            //httpWebRequest.ContentType = "application/json";
            //httpWebRequest.Method = HTTPAction.POST.ToString();

            //using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            //{
            //    streamWriter.Write(orderItems);
            //    streamWriter.Flush();
            //    streamWriter.Close();
            //    httpWebRequest.Headers["Authorization"] = "Bearer " + userTIP.Token;
            //}

            //var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            //Stream receiveStream = httpResponse.GetResponseStream();
            //StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
            //data = readStream.ReadToEnd();

            #endregion OldVersion

            #region new Edit httpClient

            var newUrl = userTIP.UrlApi + "/tips-service/Interface/GetPlanningDataForPMTs";
            var client = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true });
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userTIP.Token);
            var buffer = Encoding.UTF8.GetBytes(orderItems);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(newUrl, byteContent);
            var streamResponse = await response.Content.ReadAsStreamAsync();
            var readStream = new StreamReader(streamResponse, Encoding.UTF8);
            dynamic data = readStream.ReadToEnd();
            planingMOModel = JsonConvert.DeserializeObject<PlaningMOModel>(data);
            return planingMOModel;

            #endregion new Edit httpClient
        }

        public void UpdateLogPrintMO(string logPrintMO, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/CreateLogPrintMO", logPrintMO, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public void UpdatePrintedMODataByOrderItems(string factoryCode, string username, string jsonString, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/UpdatePrintedMODataByOrderItems" + "?FactoryCode=" + factoryCode + "&Username=" + username, jsonString, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }
    }
}