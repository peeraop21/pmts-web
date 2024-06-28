using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.NewProduct;
using PMTs.DataAccess.Repository;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Tracing;
using PMTs.WebApplication.Extentions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services
{
    [TraceAspect]
    public class ViewPictureService
    {
        IHttpContextAccessor httpContextAccessor;
        private readonly IMasterDataAPIRepository _masterDataAPIRepository;
        private string token;

        public ViewPictureService(IMasterDataAPIRepository masterDataAPIRepository)
        {
            _masterDataAPIRepository = masterDataAPIRepository;
        }

        public TransactionDataModel GetPicture(string materialNO, string FactoryCode)
        {
            var transactionDataModelSession = new TransactionDataModel();
            transactionDataModelSession.modelProductPicture = new ProductPictureView();

            //string URL = URLByQRCode;
            //string URL = "https://localhost:44360/api/MasterData/GetMasterDataByMaterialNo?AppName=W2pJlQL8hpY=&FactoryCode=252B&MaterialNo=Z038035721";
            //res respose = callApi(URLByQRCode);

            //var userSessionModel = SessionExtentions.GetSession<UserSessionModel>(httpContextAccessor.HttpContext.Session, "UserSessionModel");
            //if (userSessionModel != null)
            //{
            //    token = userSessionModel.Token;
            //}
            string str;
            string[] picture = new string[4];
            var masterData = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNo(FactoryCode, materialNO, null));
            picture[0] = masterData.DiecutPictPath;
            picture[1] = masterData.PrintMasterPath;
            picture[2] = masterData.PalletizationPath;
            picture[3] = masterData.FgpicPath;

            //transactionDataModelSession.modelProductPicture.Pic_DrawingPath = temp.DiecutPictPath;

            try
            {
                if (picture[0] != null)
                {
                    str = "";
                    str = ConvertPictureToBase64._ConvertPictureToBase64(picture[0]);
                    if (str != null)
                    {
                        transactionDataModelSession.modelProductPicture.Pic_DrawingPath = str;
                    }
                    else
                    {
                        transactionDataModelSession.modelProductPicture.Pic_DrawingPath = picture[0];
                    }

                    transactionDataModelSession.modelProductPicture.Pic_DrawingName = picture[0];

                }
            }
            catch { }

            try
            {
                if (picture[1] != null)
                {
                    str = "";
                    str = ConvertPictureToBase64._ConvertPictureToBase64(picture[1]);
                    if (str != null)
                    {
                        transactionDataModelSession.modelProductPicture.Pic_PrintPath = str;
                    }
                    else
                    {
                        transactionDataModelSession.modelProductPicture.Pic_PrintPath = picture[1];
                    }
                    transactionDataModelSession.modelProductPicture.Pic_PrintName = picture[1];

                }
            }
            catch { }

            try
            {
                if (picture[2] != null)
                {
                    str = "";
                    str = ConvertPictureToBase64._ConvertPictureToBase64(picture[2]);
                    if (str != null)
                    {
                        transactionDataModelSession.modelProductPicture.Pic_PalletPath = str;
                    }
                    else
                    {
                        transactionDataModelSession.modelProductPicture.Pic_PalletPath = picture[2];
                    }
                    transactionDataModelSession.modelProductPicture.Pic_PalletName = picture[2];

                }
            }
            catch { }


            try
            {
                if (picture[3] != null)
                {
                    str = "";
                    str = ConvertPictureToBase64._ConvertPictureToBase64(picture[3]);
                    if (str != null)
                    {
                        transactionDataModelSession.modelProductPicture.Pic_FGPath = str;
                    }
                    else
                    {
                        transactionDataModelSession.modelProductPicture.Pic_FGPath = picture[3];
                    }
                    transactionDataModelSession.modelProductPicture.Pic_FGName = picture[3];

                }

            }
            catch { }

            transactionDataModelSession.MaterialNo = masterData.MaterialNo;
            //transactionDataModel = transactionDataModelSession;

            return transactionDataModelSession;
        }

        public class res
        {
            public bool isSuccess { get; set; }
            public string exceptionMessage { get; set; }
            public string data { get; set; }
        }


        private res callApi(string URL)
        {
            string myResponse = "";
            try
            {
                string fullPath = URL;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(fullPath);
                request.Method = "Get";
                request.KeepAlive = true;
                request.ContentType = "appication/json";
                request.Headers.Add("Content-Type", "appication/json");
                //request.ContentType = "application/x-www-form-urlencoded";

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using (System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    myResponse = sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            var json = JsonConvert.DeserializeObject<res>(myResponse);
            return json;
        }

    }
}
