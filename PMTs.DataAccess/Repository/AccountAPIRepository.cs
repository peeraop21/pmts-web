using Newtonsoft.Json;
using PMTs.DataAccess.Extentions;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Shared;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace PMTs.DataAccess.Repository
{
    public class AccountAPIRepository : IAccountAPIRepository
    {
        private static readonly string _actionName = "MasterUser";

        //public static string GetAccountById(string factoryCode,  int Id, string token)
        public static string GetAccountById(string factoryCode, int Id, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetAccountById" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode + "&id=" + Id, string.Empty, token);
            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetLogin(string jsonString, string token)
        {
            //ห้ามเเก้ ไม่เกี่ยกับ jwt
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/Login?", jsonString, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string CheckGetLogin(string jsonString, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/CheckLogin" + "?AppName=" + Globals.AppNameEncrypt, jsonString, token);

            return Convert.ToString(result.Item3);
        }

        public string GetLoginAD(string jsonString, string token)
        {
            //ห้ามเเก้ ไม่เกี่ยกับ jwt
            //dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebADSCG, jsonString,token);

            dynamic result = JsonExtentions.HttpActionToAPISCG(HTTPAction.GET.ToString(), Globals.WebADSCG, jsonString);


            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public dynamic GetUser(string jsonString, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "?AppName=" + Globals.AppNameEncrypt, string.Empty, token);
            if (result.Item1)
            {
                return result;
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public static dynamic LoginUser(string jsonString, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "?AppName=" + Globals.AppNameEncrypt, jsonString, token);
            if (result.Item1)
            {
                return result;
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetaccountList(string factoryCode, string token)
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

        public void SaveAccount(string jsonString, string token)

        {
            try
            {
                dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "?AppName=" + Globals.AppNameEncrypt, jsonString, token);

                if (!result.Item1)
                {
                    throw new Exception(result.Item2);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message); ;
            }
        }

        public void UpdateAccount(string jsonString, string token)
        {
            try
            {
                dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.PUT.ToString(), Globals.WebAPIUrl + _actionName + "?AppName=" + Globals.AppNameEncrypt, jsonString, token);

                if (!result.Item1)
                {
                    throw new Exception(result.Item2);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public string GetAllAccountList(string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetAll" + "?AppName=" + Globals.AppNameEncrypt, string.Empty, token);
            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public UserTIP GetLoginTip(string apiTIPUrl, string jsonString)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiTIPUrl + "/authen-service/api/Authen/login");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = HTTPAction.POST.ToString();

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(jsonString);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            Stream receiveStream = httpResponse.GetResponseStream();
            var readStream = new StreamReader(receiveStream, Encoding.UTF8);
            dynamic data = readStream.ReadToEnd();
            return JsonConvert.DeserializeObject<UserTIP>(data);
        }
    }
}
