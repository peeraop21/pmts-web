using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace PMTs.DataAccess.Extentions
{
    internal enum HTTPAction
    {
        GET,
        POST,
        PUT,
        DELETE
    }

    public static class JsonExtentions
    {
        public static (bool IsSuccess, string ExceptionMessage, string Data) HttpActionToAPI(string method, string url, string jsonString)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = method;
            if (!method.Equals("GET"))
            {
                using var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
                streamWriter.Write(jsonString);
                streamWriter.Flush();
                streamWriter.Close();
            }

            //var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            //using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            //{
            //    dynamic response = JsonConvert.DeserializeObject(streamReader.ReadToEnd());
            //    isSuccess = Convert.ToBoolean(response.isSuccess);
            //    exceptionMessage = response.exceptionMessage == null ? "" : response.exceptionMessage.ToString();
            //    data = response.data.ToString();
            //}

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            var receiveStream = httpResponse.GetResponseStream();
            var readStream = new StreamReader(receiveStream, Encoding.UTF8);
            dynamic data = readStream.ReadToEnd();
            var response = new CustomResponse<string>();
            try
            {
                response = JsonConvert.DeserializeObject<CustomResponse<string>>(data);
                if (response.StatusCode == 200)
                {
                    isSuccess = true;
                }
                else
                {
                    isSuccess = false;
                    exceptionMessage = response.Message + " - " + response.Result;
                }
            }
            catch
            {
                var err = JsonConvert.DeserializeObject<CustomResponse<Error>>(data);
                {
                    isSuccess = false;
                    exceptionMessage = err.Result.ErrorMessage;
                }
            }

            return (isSuccess, exceptionMessage, response.Result);
        }

        public static (bool IsSuccess, string ExceptionMessage, string Data) HttpActionToAPISCG(string method, string url, string jsonString)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = method;
            httpWebRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(jsonString));
            if (!method.Equals("GET"))
            {
                using var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());

                streamWriter.Write(jsonString);
                streamWriter.Flush();
                streamWriter.Close();
            }
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            Stream receiveStream = httpResponse.GetResponseStream();
            var readStream = new StreamReader(receiveStream, Encoding.UTF8);
            dynamic data = readStream.ReadToEnd();
            isSuccess = true;
            return (isSuccess, exceptionMessage, data);
        }

        //==========jwt authen call
        public static (bool IsSuccess, string ExceptionMessage, string Data) HttpActionToJwtPMTsApi(string method, string url, string jsonString, string token)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;

            //  var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIiLCJqdGkiOiIyYTYwMDYwMC1mMGU5LTRiYTgtOGQwNC05YTA1MDNmMTdiZDMiLCJBcHBOYW1lIjoiQWRtaW4iLCJGYWN0b3J5IjoiMjU5QiIsIlVzZXJOYW1lIjoiQm9vIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiUE1UcyIsImV4cCI6MTU4NjQwNzMzMCwiaXNzIjoiSnd0Um9sZUJhc2VkQXV0aCIsImF1ZCI6Ikp3dFJvbGVCYXNlZEF1dGgifQ.v22LzWbfwJpYEFXIE4_87SO9eJvAXKdefk-JsITCdg4";

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = method;
            httpWebRequest.Headers["Authorization"] = "Bearer " + token;
            if (!method.Equals("GET"))
            {
                using var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
                streamWriter.Write(jsonString);
                streamWriter.Flush();
                streamWriter.Close();
            }
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            var receiveStream = httpResponse.GetResponseStream();
            var readStream = new StreamReader(receiveStream, Encoding.UTF8);
            dynamic data = readStream.ReadToEnd();
            var response = new CustomResponse<string>();
            try
            {
                response = JsonConvert.DeserializeObject<CustomResponse<string>>(data);
                if (response.StatusCode == 200)
                {
                    isSuccess = true;
                }
                else
                {
                    isSuccess = false;
                    exceptionMessage = response.Message + " - " + response.Result;
                }
            }
            catch
            {
                var err = JsonConvert.DeserializeObject<CustomResponse<Error>>(data);
                {
                    isSuccess = false;
                    exceptionMessage = err.Result.ErrorMessage;
                }
            }

            return (isSuccess, exceptionMessage, response.Result);
        }

        public class CustomResponse<T>
        {
            public string Message { get; set; }
            public int StatusCode { get; set; }
            public T Result { get; set; }
        }

        public class Error
        {
            public string ErrorMessage { get; set; }

            //public static int LogError(Exception ex)
            //{
            //    try
            //    {
            //        using (StreamWriter sw = File.AppendText(AppDomain.CurrentDomain.BaseDirectory + "/ErrorLog.txt"))
            //        {
            //            sw.WriteLine("DateTime : " + DateTime.Now + Environment.NewLine);
            //            if (ex.Message != null)
            //            {
            //                sw.WriteLine(Environment.NewLine + "Message" + ex.Message);
            //                sw.WriteLine(Environment.NewLine + "StackTrace" + ex.StackTrace);
            //            }
            //        again: if (ex.InnerException != null)
            //            {
            //                sw.WriteLine(Environment.NewLine + "Inner Exception : " + ex.InnerException.Message);
            //            }
            //            if (ex.InnerException.InnerException != null)
            //            {
            //                ex = ex.InnerException;
            //                goto again;
            //            }

            //            sw.WriteLine("------******------");
            //        }
            //        return StatusCodes.Status500InternalServerError;
            //    }
            //    catch (Exception)
            //    {
            //        return StatusCodes.Status500InternalServerError;
            //    }
            //}
        }

        //public static void getrequest()
        //{
        //    HttpClient client;
        //    // web api Url
        //    string url = string.Format("http://localhost:60143/api/Values");
        //    string bearerToken = string.Format("bearer token from web api");

        //        client = new HttpClient();
        //        client.BaseAddress = new Uri(url);
        //        client.DefaultRequestHeaders.Accept.Clear();
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

        //        HttpResponseMessage responseMessage = client.GetAsync(url);
        //        if (responseMessage.IsSuccessStatusCode)
        //        {
        //            var responseData = responseMessage.Content.ReadAsStringAsync().Result;
        //            var jsonResponse = JsonConvert.DeserializeObject<List<string>>(responseData);
        //        }
        //}

        public static void PresaleApiConnect(string URL, string DATA)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = DATA.Length;
            using (Stream webStream = request.GetRequestStream())
            using (var requestWriter = new StreamWriter(webStream, System.Text.Encoding.ASCII))
            {
                requestWriter.Write(DATA);
            }

            try
            {
                WebResponse webResponse = request.GetResponse();
                using var webStream = webResponse.GetResponseStream() ?? Stream.Null;
                using var responseReader = new StreamReader(webStream);
                string response = responseReader.ReadToEnd();
                Console.Out.WriteLine(response);
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(e.Message);
            }
        }
    }
}