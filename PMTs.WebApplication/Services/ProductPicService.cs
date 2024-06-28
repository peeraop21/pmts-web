using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PMTs.DataAccess;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.NewProduct;
using PMTs.DataAccess.Repository;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Shared;
using PMTs.DataAccess.Tracing;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services
{
    [TraceAspect]
    public class ProductPicService : IProductPicService
    {
        UserSessionModel userSessionModel;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IJoinAPIRepository _joinAPIRepository;
        private readonly IPrintMethodAPIRepository _printMethodAPIRepository;
        private readonly IPalletAPIRepository _palletAPIRepository;
        private readonly IChangeHistoryAPIRepository _changeHistoryAPIRepository;
        private readonly IMasterDataAPIRepository _masterDataAPIRepository;
        private readonly ITransactionsDetailAPIRepository _transactionsDetailAPIRepository;
        private readonly INewProductService _newProductService;
        private readonly IHostingEnvironment _environment;
        private readonly IExtensionService _extensionService;
        private readonly IPMTsConfigAPIRepository _pmtsConfigAPIRepository;


        private readonly string _username;
        private readonly string _saleOrg;
        private readonly string _factoryCode;
        private readonly string _token;

        public ProductPicService(
            IHttpContextAccessor httpContextAccessor,
          IMasterDataAPIRepository masterDataAPIRepository,
            INewProductService newProductService,
            IHostingEnvironment environment,
            IExtensionService extensionService,
            IPMTsConfigAPIRepository pmtsConfigAPIRepository

            )
        {
            _httpContextAccessor = httpContextAccessor;
            _masterDataAPIRepository = masterDataAPIRepository;
            _newProductService = newProductService; ;
            _environment = environment;
            _extensionService = extensionService;
            _pmtsConfigAPIRepository = pmtsConfigAPIRepository;

            userSessionModel = SessionExtentions.GetSession<UserSessionModel>(_httpContextAccessor.HttpContext.Session, "UserSessionModel");
            if (userSessionModel != null)
            {
                _username = userSessionModel.UserName;
                _saleOrg = userSessionModel.SaleOrg;
                _factoryCode = userSessionModel.FactoryCode;
                _token = userSessionModel.Token;
            }
        }


        public void GetPicture(ref TransactionDataModel transactionDataModel, IHostingEnvironment environment, string MaterialNo)
        {
            var transactionDataModelSession = new TransactionDataModel();
            transactionDataModelSession.modelProductPicture = new ProductPictureView();

            string str;
            string[] picture = new string[4];
            var temp = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNo(_factoryCode, MaterialNo, _token));
            picture[0] = temp.DiecutPictPath;
            picture[1] = temp.PrintMasterPath;
            picture[2] = temp.PalletizationPath;
            picture[3] = temp.FgpicPath;
            if (picture[0] != null)
            {
                str = "";
                str = _extensionService.Base64String(environment, picture[0]);
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
            else
            {
                transactionDataModelSession.modelProductPicture.Pic_DrawingName = picture[0];
            }

            if (picture[1] != null)
            {
                str = "";
                str = _extensionService.Base64String(environment, picture[1]);
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
            else
            {
                transactionDataModelSession.modelProductPicture.Pic_PrintName = picture[1];
            }

            if (picture[2] != null)
            {
                str = "";
                str = _extensionService.Base64String(environment, picture[2]);
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
            else
            {
                transactionDataModelSession.modelProductPicture.Pic_PalletName = picture[2];
            }

            if (picture[3] != null)
            {
                str = "";
                str = _extensionService.Base64String(environment, picture[3]);
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
            else
            {
                transactionDataModelSession.modelProductPicture.Pic_FGName = picture[3];
            }


            transactionDataModelSession.MaterialNo = MaterialNo;
            transactionDataModel = transactionDataModelSession;
        }

        public void PicData(ref TransactionDataModel transactionDataModel, IHostingEnvironment environment)
        {
            var transactionDataModelSession = SessionExtentions.GetSession<TransactionDataModel>(_httpContextAccessor.HttpContext.Session, "TransactionDataModel");
            transactionDataModelSession.modelProductPicture = new ProductPictureView();

            _newProductService.SetTransactionStatus(ref transactionDataModelSession, "Picture");

            string str;
            string[] picture = new string[8];
            var temp = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNo(_factoryCode, transactionDataModelSession.MaterialNo, _token));
            picture[0] = temp.DiecutPictPath;
            picture[1] = temp.PrintMasterPath;
            picture[2] = temp.PalletizationPath;
            picture[3] = temp.FgpicPath;

            picture[4] = temp.Semi1Path;
            picture[5] = temp.Semi2Path;
            picture[6] = temp.Semi3Path;
            picture[7] = temp.SemiFilePdfPath;

            var drawingPath = "";
            var printPath = "";
            var palletPath = "";
            var fGPath = "";
            var semi1 = "";
            var semi2 = "";
            var semi3 = "";
            var semi4 = "";

            if (!string.IsNullOrEmpty(picture[0]))
            {
                drawingPath = ConvertPictureToBase64._ConvertPictureToBase64(picture[0]);
                transactionDataModelSession.modelProductPicture.Pic_DrawingPath = picture[0];
                transactionDataModelSession.modelProductPicture.Pic_DrawingName = ConvertPictureToBase64._ConvertPictureToBase64(picture[0]);
            }
            else
            {
                transactionDataModelSession.modelProductPicture.Pic_DrawingName = picture[0];
            }

            if (!string.IsNullOrEmpty(picture[1]))
            {
                printPath = ConvertPictureToBase64._ConvertPictureToBase64(picture[1]);
                transactionDataModelSession.modelProductPicture.Pic_PrintPath = picture[1];
                transactionDataModelSession.modelProductPicture.Pic_PrintName = ConvertPictureToBase64._ConvertPictureToBase64(picture[1]);

            }
            else
            {
                transactionDataModelSession.modelProductPicture.Pic_PrintName = picture[1];
            }

            if (!string.IsNullOrEmpty(picture[2]))
            {
                palletPath = ConvertPictureToBase64._ConvertPictureToBase64(picture[2]);
                transactionDataModelSession.modelProductPicture.Pic_PalletPath = picture[2];
                transactionDataModelSession.modelProductPicture.Pic_PalletName = ConvertPictureToBase64._ConvertPictureToBase64(picture[2]);
                if (transactionDataModelSession.modelProductPicture.Pic_PalletName == null)
                {
                    transactionDataModelSession.modelProductPicture.Pic_PalletName = picture[2];
                    palletPath = picture[2];
                }
            }
            else
            {
                transactionDataModelSession.modelProductPicture.Pic_PalletName = picture[2];
            }

            if (!string.IsNullOrEmpty(picture[3]))
            {
                fGPath = ConvertPictureToBase64._ConvertPictureToBase64(picture[3]);
                transactionDataModelSession.modelProductPicture.Pic_FGPath = picture[3];
                transactionDataModelSession.modelProductPicture.Pic_FGName = ConvertPictureToBase64._ConvertPictureToBase64(picture[3]);

            }
            else
            {
                transactionDataModelSession.modelProductPicture.Pic_FGName = picture[3];
            }



            if (!string.IsNullOrEmpty(picture[4]))
            {
                semi1 = ConvertPictureToBase64._ConvertPictureToBase64(picture[4]);
                transactionDataModelSession.modelProductPicture.Semi1_Path = picture[4];
                transactionDataModelSession.modelProductPicture.Semi1_Name = ConvertPictureToBase64._ConvertPictureToBase64(picture[4]);

            }
            else
            {
                transactionDataModelSession.modelProductPicture.Semi1_Name = picture[4];
            }

            if (!string.IsNullOrEmpty(picture[5]))
            {
                semi2 = ConvertPictureToBase64._ConvertPictureToBase64(picture[5]);
                transactionDataModelSession.modelProductPicture.Semi2_Path = picture[5];
                transactionDataModelSession.modelProductPicture.Semi2_Name = ConvertPictureToBase64._ConvertPictureToBase64(picture[5]);

            }
            else
            {
                transactionDataModelSession.modelProductPicture.Semi2_Name = picture[5];
            }

            if (!string.IsNullOrEmpty(picture[6]))
            {
                semi3 = ConvertPictureToBase64._ConvertPictureToBase64(picture[6]);
                transactionDataModelSession.modelProductPicture.Semi3_Path = picture[6];
                transactionDataModelSession.modelProductPicture.Semi3_Name = ConvertPictureToBase64._ConvertPictureToBase64(picture[6]);

            }
            else
            {
                transactionDataModelSession.modelProductPicture.Semi3_Name = picture[6];
            }


            if (!string.IsNullOrEmpty(picture[7]))
            {
                semi4 = ConvertPictureToBase64._ConvertPictureToBase64(picture[7]);
                transactionDataModelSession.modelProductPicture.SemiFilePdfPath = picture[7];
                transactionDataModelSession.modelProductPicture.SemiFilePdf_Name = picture[7] != null ? Path.GetFileName(picture[7]) : string.Empty;

            }
            else
            {
                transactionDataModelSession.modelProductPicture.SemiFilePdf_Name = picture[7];
            }


            transactionDataModel = transactionDataModelSession;

            //get attach file masterdata
            var masterdata = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNoAndFactory(_factoryCode, transactionDataModelSession.MaterialNo, _token));
            transactionDataModelSession.modelProductPicture.AttachFileMoPath = masterdata != null ? Path.GetFileName(masterdata.AttachFileMoPath) : string.Empty;
            SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "TransactionDataModel", transactionDataModelSession);

            transactionDataModel.modelProductPicture.Pic_DrawingPath = drawingPath;
            transactionDataModel.modelProductPicture.Pic_PrintPath = printPath;
            transactionDataModel.modelProductPicture.Pic_PalletPath = palletPath;
            transactionDataModel.modelProductPicture.Pic_FGPath = fGPath;
            transactionDataModel.modelProductPicture.Semi1_Path = semi1;
            transactionDataModel.modelProductPicture.Semi2_Path = semi2;
            transactionDataModel.modelProductPicture.Semi3_Path = semi3;
            // transactionDataModel.modelProductPicture.SemiFilePdfPath = semi4;
        }

        public void SetPicData(string[,] Base64)
        {
            throw new NotImplementedException();
        }


        public string[] UploadPicture(IFormFile Picture, IHostingEnvironment environment)
        {
            var path = Path.Combine(environment.WebRootPath, "Picture");
            string[] result = new string[3];
            string fileName = null;
            string newFileName = null;
            string base64String = null;
            try
            {

                if (Picture == null)
                {
                    result[0] = newFileName;
                    result[1] = base64String;
                    return result;
                }
                //Create Folder 
                if (!(Directory.Exists(path)))
                {
                    Directory.CreateDirectory(path);
                }

                if (Picture != null)
                {
                    //Getting FileName
                    fileName = ContentDispositionHeaderValue.Parse(Picture.ContentDisposition).FileName.Trim('"');

                    //Assigning Unique Filename (Guid)
                    var myUniqueFileName = Convert.ToString(Guid.NewGuid());

                    //Getting file Extension
                    var FileExtension = Path.GetExtension(fileName);

                    // concating  FileName + FileExtension
                    newFileName = myUniqueFileName + FileExtension;



                    // Combines two strings into a path.
                    fileName = path + $@"\{newFileName}";

                    // if you want to store path of folder in database

                    using (FileStream fs = new FileStream(fileName, FileMode.Create))
                    {
                        //Picture.CopyToAsync(fs);
                        Picture.CopyTo(fs);

                        //   fs.Flush();


                    }

                    Byte[] bytes = File.ReadAllBytes(fileName);
                    string src = "data:" + Picture.ContentType + ";base64,";
                    base64String = src + Convert.ToBase64String(bytes);

                }


                result[0] = newFileName;
                result[1] = base64String;
                return result;
            }
            catch (Exception ex)
            {
                return result = null;
            }

        }
        
        public TransactionDataModel UpdateData(TransactionDataModel model, IHostingEnvironment environment, string Page, IFormFile Pic_Drawing, IFormFile Pic_Print, IFormFile Pic_Pallet, IFormFile Pic_FG, IFormFile Semi1_Print, IFormFile Semi2_Print, IFormFile Semi3_Print, IFormFile Semi4_Print)
        {
            try
            {
                var path = Path.Combine(environment.WebRootPath, "Picture");
                if (model.modelProductPicture == null)
                {
                    model.modelProductPicture = new ProductPictureView();
                }
                var masterData = new MasterData();
                masterData = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNo(_factoryCode, model.MaterialNo, _token));

                if (masterData == null)
                {
                    return model;
                }

                #region [tmp]
                //if (Page == "ProductProp")
                //{
                //    if (model.EventFlag != "Copy")
                //    {
                //        if (model.modelProductPicture.Pic_PalletPath == null)
                //        {
                //            if (model.modelProductPicture.Pic_PalletName != null)
                //            {
                //                _extensionService.DeleteFile(path + "\\" + model.modelProductPicture.Pic_PalletName);

                //            }
                //            masterData.PalletizationPath = null;
                //            model.modelProductPicture.Pic_PalletName = null;
                //            model.modelProductPicture.Pic_PalletPath = null;
                //        }


                //        if (model.modelProductPicture.Pic_FGPath == null)
                //        {
                //            if (model.modelProductPicture.Pic_FGName != null)
                //            {
                //                _extensionService.DeleteFile(path + "\\" + model.modelProductPicture.Pic_FGName);

                //            }
                //            masterData.FgpicPath = null;
                //            model.modelProductPicture.Pic_FGName = null;
                //            model.modelProductPicture.Pic_FGPath = null;
                //        }
                //    }
                //    else
                //    {
                //        if (model.modelProductPicture.Pic_PalletPath == null)
                //        {
                //            masterData.PalletizationPath = null;
                //            model.modelProductPicture.Pic_PalletName = null;
                //            model.modelProductPicture.Pic_PalletPath = null;
                //        }


                //        if (model.modelProductPicture.Pic_FGPath == null)
                //        {
                //            masterData.FgpicPath = null;
                //            model.modelProductPicture.Pic_FGName = null;
                //            model.modelProductPicture.Pic_FGPath = null;
                //        }
                //    }

                //}
                //else
                //{
                //    if (model.EventFlag != "Copy")
                //    {
                //        if (model.modelProductPicture.Pic_DrawingPath == null)
                //        {
                //            if (model.modelProductPicture.Pic_DrawingName != null)
                //            {
                //                _extensionService.DeleteFile(path + "\\" + model.modelProductPicture.Pic_DrawingName);

                //            }
                //            masterData.DiecutPictPath = null;
                //            model.modelProductPicture.Pic_DrawingName = null;
                //            model.modelProductPicture.Pic_DrawingPath = null;
                //        }
                //        if (model.modelProductPicture.Pic_PrintPath == null)
                //        {
                //            if (model.modelProductPicture.Pic_PrintName != null)
                //            {
                //                _extensionService.DeleteFile(path + "\\" + model.modelProductPicture.Pic_PrintName);

                //            }
                //            masterData.PrintMasterPath = null;
                //            model.modelProductPicture.Pic_PrintName = null;
                //            model.modelProductPicture.Pic_PrintPath = null;
                //        }


                //        if (model.modelProductPicture.Pic_PalletPath == null)
                //        {
                //            if (model.modelProductPicture.Pic_PalletName != null)
                //            {
                //                _extensionService.DeleteFile(path + "\\" + model.modelProductPicture.Pic_PalletName);

                //            }
                //            masterData.PalletizationPath = null;
                //            model.modelProductPicture.Pic_PalletName = null;
                //            model.modelProductPicture.Pic_PalletPath = null;
                //        }


                //        if (model.modelProductPicture.Pic_FGPath == null)
                //        {
                //            if (model.modelProductPicture.Pic_FGName != null)
                //            {
                //                _extensionService.DeleteFile(path + "\\" + model.modelProductPicture.Pic_FGName);

                //            }
                //            masterData.FgpicPath = null;
                //            model.modelProductPicture.Pic_FGName = null;
                //            model.modelProductPicture.Pic_FGPath = null;
                //        }
                //    }
                //    else
                //    {
                //        if (model.modelProductPicture.Pic_DrawingPath == null)
                //        {

                //            masterData.DiecutPictPath = null;
                //            model.modelProductPicture.Pic_DrawingName = null;
                //            model.modelProductPicture.Pic_DrawingPath = null;
                //        }
                //        if (model.modelProductPicture.Pic_PrintPath == null)
                //        {

                //            masterData.PrintMasterPath = null;
                //            model.modelProductPicture.Pic_PrintName = null;
                //            model.modelProductPicture.Pic_PrintPath = null;
                //        }


                //        if (model.modelProductPicture.Pic_PalletPath == null)
                //        {

                //            masterData.PalletizationPath = null;
                //            model.modelProductPicture.Pic_PalletName = null;
                //            model.modelProductPicture.Pic_PalletPath = null;
                //        }


                //        if (model.modelProductPicture.Pic_FGPath == null)
                //        {

                //            masterData.FgpicPath = null;
                //            model.modelProductPicture.Pic_FGName = null;
                //            model.modelProductPicture.Pic_FGPath = null;
                //        }
                //    }

                //}
                #endregion

                var fgPath = "";
                var DrawingPath = "";
                var palletPath = "";
                var PrintPath = "";
                if (Pic_Drawing != null)
                {
                    //string[] fileName = _extensionService.UploadPicture(Pic_Drawing);
                    string dataUrl = _extensionService.ResizeImageRatio(Pic_Drawing, 1080, 1080);
                    model.modelProductPicture.Pic_DrawingPath = dataUrl;
                    if (model.modelProductPicture.Pic_DrawingPath != null)
                    {
                        var cursor = model.modelProductPicture.Pic_DrawingPath.IndexOf(',') + 1;
                        var base64 = model.modelProductPicture.Pic_DrawingPath.Substring(cursor);

                        //get path form pmtcontext
                        var mainpath = JsonConvert.DeserializeObject<PmtsConfig>(_pmtsConfigAPIRepository.GetPMTsConfigByFactoryName(_factoryCode, "Drawing_Path", _token)).FucValue;
                        DrawingPath = mainpath + "\\" + Pic_Drawing.FileName;// _factoryCode + "-" + model.MaterialNo + ".png"; ;
                        if (mainpath != null)
                        {
                            //byte[] bytes = Convert.FromBase64String(base64);

                            //using (MemoryStream ms = new MemoryStream(bytes))
                            //{
                            //    using (Bitmap bm2 = new Bitmap(ms))
                            //    {
                            //        bm2.Save(DrawingPath);
                            //    }
                            //}

                            _extensionService.UploadImage(base64, DrawingPath);

                            model.modelProductPicture.Pic_DrawingPath = DrawingPath;
                            masterData.DiecutPictPath = DrawingPath;
                            //_masterDataAPIRepository.UpdateCapImgTransactionDetail(_factoryCode, model.MaterialNo, "0");
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(model.modelProductSpec.PrintMaster))
                    {
                        // delete file in database
                        //var mainpath = JsonConvert.DeserializeObject<PmtsConfig>(_pmtsConfigAPIRepository.GetPMTsConfigByFactoryName(_factoryCode, "Drawing_Path", _token)).FucValue;
                        var deletePath = masterData.DiecutPictPath;
                        if (!string.IsNullOrEmpty(deletePath) && File.Exists(deletePath))
                        {
                            //File.Delete(deletePath);
                            model.modelProductPicture.Pic_DrawingPath = null;
                            masterData.DiecutPictPath = null;
                        }

                    }
                }

                if (Pic_Print != null)
                {
                    //string[] fileName = _extensionService.UploadPicture(Pic_Print);
                    string dataUrl = _extensionService.ResizeImageRatio(Pic_Print, 1080, 1080);
                    model.modelProductPicture.Pic_PrintPath = dataUrl;
                    if (model.modelProductPicture.Pic_PrintPath != null)
                    {
                        var cursor = model.modelProductPicture.Pic_PrintPath.IndexOf(',') + 1;
                        var base64 = model.modelProductPicture.Pic_PrintPath.Substring(cursor);

                        //get path form pmtcontext
                        var mainpath = JsonConvert.DeserializeObject<PmtsConfig>(_pmtsConfigAPIRepository.GetPMTsConfigByFactoryName(_factoryCode, "PrintMaster_Path", _token)).FucValue;
                        PrintPath = mainpath + "\\" + Pic_Print.FileName;// _factoryCode + "-" + model.MaterialNo + ".png";
                        if (mainpath != null)
                        {
                            //byte[] bytes = Convert.FromBase64String(base64);

                            //using (MemoryStream ms = new MemoryStream(bytes))
                            //{
                            //    using (Bitmap bm2 = new Bitmap(ms))
                            //    {
                            //        bm2.Save(PrintPath);
                            //    }
                            //}

                            _extensionService.UploadImage(base64, PrintPath);

                            model.modelProductPicture.Pic_PrintPath = PrintPath;
                            masterData.PrintMasterPath = PrintPath;
                        }
                    }
                }
                else
                {
                    //delete file in database
                    var mainpath = JsonConvert.DeserializeObject<PmtsConfig>(_pmtsConfigAPIRepository.GetPMTsConfigByFactoryName(_factoryCode, "PrintMaster_Path", _token)).FucValue;
                    if (Pic_Print != null)
                    {
                        PrintPath = mainpath + "\\" + Pic_Print.FileName; // _factoryCode + "-" + model.MaterialNo + ".png"; ;
                        if (File.Exists(PrintPath))
                        {
                            //File.Delete(PrintPath);
                            model.modelProductPicture.Pic_PrintPath = null;
                            masterData.PrintMasterPath = null;
                        }
                    }

                }

                if (Pic_Pallet != null)
                {
                    //string[] fileName = _extensionService.UploadPicture(Pic_Pallet);
                    string dataUrl = _extensionService.ResizeImageRatio(Pic_Pallet, 1080, 1080);
                    model.modelProductPicture.Pic_PalletPath = dataUrl;
                    if (model.modelProductPicture.Pic_PalletPath != null)
                    {
                        var cursor = model.modelProductPicture.Pic_PalletPath.IndexOf(',') + 1;
                        var base64 = model.modelProductPicture.Pic_PalletPath.Substring(cursor);

                        //get path form pmtcontext
                        var mainpath = JsonConvert.DeserializeObject<PmtsConfig>(_pmtsConfigAPIRepository.GetPMTsConfigByFactoryName(_factoryCode, "Pallet_Path", _token)).FucValue;
                        palletPath = mainpath + "\\" + Pic_Pallet.FileName; //_factoryCode + "-" + model.MaterialNo + ".png";


                        if (mainpath != null)
                        {
                            //byte[] bytes = Convert.FromBase64String(base64);

                            //using (MemoryStream ms = new MemoryStream(bytes))
                            //{
                            //    using (Bitmap bm2 = new Bitmap(ms))
                            //    {
                            //        bm2.Save(palletPath);
                            //    }
                            //}

                            _extensionService.UploadImage(base64, palletPath);

                            model.modelProductPicture.Pic_PalletPath = palletPath;
                            masterData.PalletizationPath = palletPath;
                        }
                    }
                }
                else
                {
                    //delete file in database
                    var mainpath = JsonConvert.DeserializeObject<PmtsConfig>(_pmtsConfigAPIRepository.GetPMTsConfigByFactoryName(_factoryCode, "Pallet_Path", _token)).FucValue;
                    if (Pic_Pallet != null)
                    {
                        palletPath = mainpath + "\\" + Pic_Pallet.FileName; //_factoryCode + "-" + model.MaterialNo + ".png";
                        if (masterData.PalletizationPath != palletPath)
                        {

                            if (File.Exists(palletPath))
                            {
                                //File.Delete(palletPath);
                                model.modelProductPicture.Pic_PalletPath = null;
                                masterData.PalletizationPath = null;
                            }
                        }
                    }
                }

                if (Pic_FG != null)
                {
                    //string[] fileName = _extensionService.UploadPicture(Pic_FG);
                    string dataUrl = _extensionService.ResizeImageRatio(Pic_FG, 1080, 1080);
                    model.modelProductPicture.Pic_FGPath = dataUrl;
                    if (model.modelProductPicture.Pic_FGPath != null)
                    {
                        var cursor = model.modelProductPicture.Pic_FGPath.IndexOf(',') + 1;
                        var base64 = model.modelProductPicture.Pic_FGPath.Substring(cursor);

                        //get path form pmtcontext
                        var mainpath = JsonConvert.DeserializeObject<PmtsConfig>(_pmtsConfigAPIRepository.GetPMTsConfigByFactoryName(_factoryCode, "FG_Path", _token)).FucValue;
                        fgPath = mainpath + "\\" + Pic_FG.FileName;///_factoryCode + "-" + model.MaterialNo + ".png";
                        if (mainpath != null)
                        {
                            //byte[] bytes = Convert.FromBase64String(base64);

                            //using (MemoryStream ms = new MemoryStream(bytes))
                            //{
                            //    using (Bitmap bm2 = new Bitmap(ms))
                            //    {
                            //        bm2.Save(fgPath);
                            //    }
                            //}

                            _extensionService.UploadImage(base64, fgPath);

                            model.modelProductPicture.Pic_FGPath = fgPath;
                            masterData.FgpicPath = fgPath;
                        }
                    }
                }
                else
                {
                    //delete file in database
                    var mainpath = JsonConvert.DeserializeObject<PmtsConfig>(_pmtsConfigAPIRepository.GetPMTsConfigByFactoryName(_factoryCode, "FG_Path", _token)).FucValue;
                    if (Pic_FG != null)
                    {
                        fgPath = mainpath + "\\" + Pic_FG.FileName;// _factoryCode + "-" + model.MaterialNo + ".png";
                        if (File.Exists(fgPath))
                        {
                            //File.Delete(fgPath);
                            model.modelProductPicture.Pic_FGPath = null;
                            masterData.FgpicPath = null;
                        }
                    }

                }


                //========================================================= Semi

                if (Semi1_Print != null)
                {
                    //string[] fileName = _extensionService.UploadPicture(Semi1_Print);
                    string dataUrl = _extensionService.ResizeImageRatio(Semi1_Print, 1080, 1080);
                    model.modelProductPicture.Semi1_Path = dataUrl;
                    if (model.modelProductPicture.Semi1_Path != null)
                    {
                        var cursor = model.modelProductPicture.Semi1_Path.IndexOf(',') + 1;
                        var base64 = model.modelProductPicture.Semi1_Path.Substring(cursor);

                        //get path form pmtcontext
                        var mainpath = JsonConvert.DeserializeObject<PmtsConfig>(_pmtsConfigAPIRepository.GetPMTsConfigByFactoryName(_factoryCode, "Semi1_Path", _token)).FucValue;
                        fgPath = mainpath + "\\" + Semi1_Print.FileName;///_factoryCode + "-" + model.MaterialNo + ".png";
                        if (mainpath != null)
                        {
                            //byte[] bytes = Convert.FromBase64String(base64);

                            //using (MemoryStream ms = new MemoryStream(bytes))
                            //{
                            //    using (Bitmap bm2 = new Bitmap(ms))
                            //    {
                            //        bm2.Save(fgPath);
                            //    }
                            //}

                            _extensionService.UploadImage(base64, fgPath);

                            model.modelProductPicture.Semi1_Path = fgPath;
                            masterData.Semi1Path = fgPath;
                        }
                    }
                }
                else
                {
                    //delete file in database
                    var mainpath = JsonConvert.DeserializeObject<PmtsConfig>(_pmtsConfigAPIRepository.GetPMTsConfigByFactoryName(_factoryCode, "Semi1_Path", _token)).FucValue;
                    if (Semi1_Print != null)
                    {
                        fgPath = mainpath + "\\" + Semi1_Print.FileName;// _factoryCode + "-" + model.MaterialNo + ".png";
                        if (File.Exists(fgPath))
                        {
                            //File.Delete(fgPath);
                            model.modelProductPicture.Semi1_Path = null;
                            masterData.Semi1Path = null;
                        }
                    }
                    else
                    {
                        model.modelProductPicture.Semi1_Path = null;
                        masterData.Semi1Path = null;
                    }
                }

                if (Semi2_Print != null)
                {
                    //string[] fileName = _extensionService.UploadPicture(Semi2_Print);
                    string dataUrl = _extensionService.ResizeImageRatio(Semi2_Print, 1080, 1080);
                    model.modelProductPicture.Semi2_Path = dataUrl;
                    if (model.modelProductPicture.Semi2_Path != null)
                    {
                        var cursor = model.modelProductPicture.Semi2_Path.IndexOf(',') + 1;
                        var base64 = model.modelProductPicture.Semi2_Path.Substring(cursor);

                        //get path form pmtcontext
                        var mainpath = JsonConvert.DeserializeObject<PmtsConfig>(_pmtsConfigAPIRepository.GetPMTsConfigByFactoryName(_factoryCode, "Semi2_Path", _token)).FucValue;
                        fgPath = mainpath + "\\" + Semi2_Print.FileName;///_factoryCode + "-" + model.MaterialNo + ".png";
                        if (mainpath != null)
                        {
                            //byte[] bytes = Convert.FromBase64String(base64);

                            //using (MemoryStream ms = new MemoryStream(bytes))
                            //{
                            //    using (Bitmap bm2 = new Bitmap(ms))
                            //    {
                            //        bm2.Save(fgPath);
                            //    }
                            //}

                            _extensionService.UploadImage(base64, fgPath);

                            model.modelProductPicture.Semi2_Path = fgPath;
                            masterData.Semi2Path = fgPath;
                        }
                    }
                }
                else
                {
                    //delete file in database
                    var mainpath = JsonConvert.DeserializeObject<PmtsConfig>(_pmtsConfigAPIRepository.GetPMTsConfigByFactoryName(_factoryCode, "Semi2_Path", _token)).FucValue;
                    if (Semi2_Print != null)
                    {
                        fgPath = mainpath + "\\" + Semi2_Print.FileName;// _factoryCode + "-" + model.MaterialNo + ".png";
                        if (File.Exists(fgPath))
                        {
                            //File.Delete(fgPath);
                            model.modelProductPicture.Semi2_Path = null;
                            masterData.Semi2Path = null;
                        }
                    }
                    else
                    {
                        model.modelProductPicture.Semi2_Path = null;
                        masterData.Semi2Path = null;
                    }
                }

                if (Semi3_Print != null)
                {
                    //string[] fileName = _extensionService.UploadPicture(Semi3_Print);
                    string dataUrl = _extensionService.ResizeImageRatio(Semi3_Print, 1080, 1080);
                    model.modelProductPicture.Semi3_Path = dataUrl;
                    if (model.modelProductPicture.Semi3_Path != null)
                    {
                        var cursor = model.modelProductPicture.Semi3_Path.IndexOf(',') + 1;
                        var base64 = model.modelProductPicture.Semi3_Path.Substring(cursor);

                        //get path form pmtcontext
                        var mainpath = JsonConvert.DeserializeObject<PmtsConfig>(_pmtsConfigAPIRepository.GetPMTsConfigByFactoryName(_factoryCode, "Semi3_Path", _token)).FucValue;
                        fgPath = mainpath + "\\" + Semi3_Print.FileName;///_factoryCode + "-" + model.MaterialNo + ".png";
                        if (mainpath != null)
                        {
                            //byte[] bytes = Convert.FromBase64String(base64);

                            //using (MemoryStream ms = new MemoryStream(bytes))
                            //{
                            //    using (Bitmap bm2 = new Bitmap(ms))
                            //    {
                            //        bm2.Save(fgPath);
                            //    }
                            //}

                            _extensionService.UploadImage(base64, fgPath);

                            model.modelProductPicture.Semi3_Path = fgPath;
                            masterData.Semi3Path = fgPath;
                        }
                    }
                }
                else
                {
                    //delete file in database
                    var mainpath = JsonConvert.DeserializeObject<PmtsConfig>(_pmtsConfigAPIRepository.GetPMTsConfigByFactoryName(_factoryCode, "Semi3_Path", _token)).FucValue;
                    if (Semi3_Print != null)
                    {
                        fgPath = mainpath + "\\" + Semi3_Print.FileName;// _factoryCode + "-" + model.MaterialNo + ".png";
                        if (File.Exists(fgPath))
                        {
                            //File.Delete(fgPath);
                            model.modelProductPicture.Semi3_Path = null;
                            masterData.Semi3Path = null;
                        }
                    }
                    else
                    {
                        model.modelProductPicture.Semi3_Path = null;
                        masterData.Semi3Path = null;
                    }

                }

                //if (Semi4_Print != null)
                //{
                //    //string[] fileName = _extensionService.UploadPicture(Semi4_Print);
                //    string dataUrl = _extensionService.ResizeImageRatio(Semi4_Print, 1080, 1080);
                //    model.modelProductPicture.SemiFilePdfPath = dataUrl;
                //    if (model.modelProductPicture.SemiFilePdfPath != null)
                //    {
                //        var cursor = model.modelProductPicture.SemiFilePdfPath.IndexOf(',') + 1;
                //        var base64 = model.modelProductPicture.SemiFilePdfPath.Substring(cursor);

                //        //get path form pmtcontext
                //        var mainpath = JsonConvert.DeserializeObject<PmtsConfig>(_pmtsConfigAPIRepository.GetPMTsConfigByFactoryName(_factoryCode, "Semi4_Path", _token)).FucValue;
                //        fgPath = mainpath + "\\" + Semi4_Print.FileName;///_factoryCode + "-" + model.MaterialNo + ".png";
                //        if (mainpath != null)
                //        {

                //            if (Semi4_Print != null)
                //            {

                //                //Set Key Name
                //                string ImageName = Guid.NewGuid().ToString() + Path.GetExtension(Semi4_Print.FileName);

                //                //Get url To Save
                //                string SavePath = fgPath;

                //                using (var stream = new FileStream(SavePath, FileMode.Create))
                //                {
                //                    Semi4_Print.CopyTo(stream);
                //                }
                //            }

                //            model.modelProductPicture.SemiFilePdfPath = fgPath;
                //            masterData.SemiFilePdfPath = fgPath;
                //        }
                //    }
                //}
                //else
                //{
                //    //delete file in database
                //    var mainpath = JsonConvert.DeserializeObject<PmtsConfig>(_pmtsConfigAPIRepository.GetPMTsConfigByFactoryName(_factoryCode, "Semi4_Path", _token)).FucValue;
                //    if (Semi4_Print != null)
                //    {
                //        fgPath = mainpath + "\\" + Semi4_Print.FileName;// _factoryCode + "-" + model.MaterialNo + ".png";
                //        if (File.Exists(fgPath))
                //        {
                //            File.Delete(fgPath);
                //            model.modelProductPicture.SemiFilePdfPath = null;
                //            masterData.SemiFilePdfPath = null;
                //        }
                //    }
                //    else
                //    {
                //        model.modelProductPicture.SemiFilePdfPath = null;
                //        masterData.SemiFilePdfPath = null;
                //    }

                //}


                //========================================================

                //masterData.DiecutPictPath = model.modelProductPicture.Pic_DrawingPath;
                //masterData.PrintMasterPath = model.modelProductPicture.Pic_DrawingPath;
                //masterData.PalletizationPath = model.modelProductPicture.Pic_DrawingPath;
                //masterData.FgpicPath = fgPath;

                //masterData.DiecutPictPath = SavePictute("Drawing_Path", model.modelProductPicture.Pic_DrawingPath,model.MaterialNo);
                //masterData.PrintMasterPath = SavePictute("PrintMaster_Path", model.modelProductPicture.Pic_PrintPath, model.MaterialNo);
                //masterData.PalletizationPath = SavePictute("Pallet_Path", model.modelProductPicture.Pic_PalletPath, model.MaterialNo);
                //masterData.FgpicPath = SavePictute("FG_Path", model.modelProductPicture.Pic_FGPath, model.MaterialNo);
                masterData.User = _username;
                //pls re-check
                ParentModel masterDataModel = new ParentModel();
                masterDataModel.AppName = Globals.AppNameEncrypt;
                masterDataModel.SaleOrg = _saleOrg;
                masterDataModel.PlantCode = _factoryCode;
                masterDataModel.MasterData = masterData;

                _masterDataAPIRepository.UpdateMasterData(JsonConvert.SerializeObject(masterDataModel), _token);

                var transactionDataModelSession = SessionExtentions.GetSession<TransactionDataModel>(_httpContextAccessor.HttpContext.Session, "TransactionDataModel");

                //_newProductService.SetTransactionStatus(ref transactionDataModelSession, "End");

                //update max step
                _newProductService.UpdateMaxProgress(_factoryCode, ref transactionDataModelSession);

                SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "TransactionDataModel", transactionDataModelSession);


            }
            catch (Exception ex)
            {
            }

            return model;

        }

        //Tassanai Update 06072020 == UploadPicture for artteam
        public ProductPictureView UpdateDataPicture(ProductPictureView model, IHostingEnvironment environment, IFormFile Pic_Drawing, IFormFile Pic_Print, IFormFile Pic_Pallet, IFormFile Pic_FG)
        {
            try
            {
                var path = Path.Combine(environment.WebRootPath, "Picture");
                if (model == null)
                {
                    model = new ProductPictureView();
                }
                var masterData = new MasterData();
                masterData = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNo(_factoryCode, model.MaterialNo, _token));

                if (masterData == null)
                {
                    return model;
                }

                var fgPath = "";
                var DrawingPath = "";
                var palletPath = "";
                var PrintPath = "";
                if (Pic_Drawing != null)
                {
                    //string[] fileName = _extensionService.UploadPicture(Pic_Drawing);
                    string dataUrl = _extensionService.ResizeImageRatio(Pic_Drawing, 1080, 1080);
                    model.Pic_DrawingPath = dataUrl;
                    if (model.Pic_DrawingPath != null)
                    {
                        var cursor = model.Pic_DrawingPath.IndexOf(',') + 1;
                        var base64 = model.Pic_DrawingPath.Substring(cursor);

                        //get path form pmtcontext
                        var mainpath = JsonConvert.DeserializeObject<PmtsConfig>(_pmtsConfigAPIRepository.GetPMTsConfigByFactoryName(_factoryCode, "Drawing_Path", _token)).FucValue;
                        DrawingPath = mainpath + "\\" + Pic_Drawing.FileName;// _factoryCode + "-" + model.MaterialNo + ".png"; ;
                        if (mainpath != null)
                        {
                            //byte[] bytes = Convert.FromBase64String(base64);

                            //using (MemoryStream ms = new MemoryStream(bytes))
                            //{
                            //    using (Bitmap bm2 = new Bitmap(ms))
                            //    {
                            //        bm2.Save(DrawingPath);
                            //    }
                            //}

                            _extensionService.UploadImage(base64, DrawingPath);

                            model.Pic_DrawingPath = DrawingPath;
                            masterData.DiecutPictPath = DrawingPath;
                            //_masterDataAPIRepository.UpdateCapImgTransactionDetail(_factoryCode, model.MaterialNo, "0");
                        }
                    }
                }
                else
                {
                    //if (string.IsNullOrEmpty(model.Pic_PrintName))
                    if (string.IsNullOrEmpty(model.Pic_DrawingPath))
                    {
                        // delete file in database
                        var mainpath = JsonConvert.DeserializeObject<PmtsConfig>(_pmtsConfigAPIRepository.GetPMTsConfigByFactoryName(_factoryCode, "Drawing_Path", _token)).FucValue;
                        if (Pic_Drawing != null)
                        {
                            DrawingPath = mainpath + "\\" + Pic_Drawing.FileName;//_factoryCode + "-" + model.MaterialNo + ".png";
                            if (File.Exists(DrawingPath))
                            {
                                File.Delete(DrawingPath);
                                model.Pic_DrawingPath = null;
                                masterData.DiecutPictPath = null;
                            }
                        }
                        else
                        {
                            //DrawingPath = mainpath + "\\" + Pic_Drawing.FileName;//_factoryCode + "-" + model.MaterialNo + ".png";
                            //if (File.Exists(DrawingPath))
                            //{
                            //File.Delete(DrawingPath);
                            model.Pic_DrawingPath = null;
                            masterData.DiecutPictPath = null;
                            //}
                        }

                    }
                }

                if (Pic_Print != null)
                {
                    //string[] fileName = _extensionService.UploadPicture(Pic_Print);
                    string dataUrl = _extensionService.ResizeImageRatio(Pic_Print, 1080, 1080);
                    model.Pic_PrintPath = dataUrl;
                    if (model.Pic_PrintPath != null)
                    {
                        var cursor = model.Pic_PrintPath.IndexOf(',') + 1;
                        var base64 = model.Pic_PrintPath.Substring(cursor);

                        //get path form pmtcontext
                        var mainpath = JsonConvert.DeserializeObject<PmtsConfig>(_pmtsConfigAPIRepository.GetPMTsConfigByFactoryName(_factoryCode, "PrintMaster_Path", _token)).FucValue;
                        PrintPath = mainpath + "\\" + Pic_Print.FileName;// _factoryCode + "-" + model.MaterialNo + ".png";
                        if (mainpath != null)
                        {
                            //byte[] bytes = Convert.FromBase64String(base64);

                            //using (MemoryStream ms = new MemoryStream(bytes))
                            //{
                            //    using (Bitmap bm2 = new Bitmap(ms))
                            //    {
                            //        bm2.Save(PrintPath);
                            //    }
                            //}

                            _extensionService.UploadImage(base64, PrintPath);

                            model.Pic_PrintPath = PrintPath;
                            masterData.PrintMasterPath = PrintPath;
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(model.Pic_PrintPath))
                    {
                        //delete file in database
                        var mainpath = JsonConvert.DeserializeObject<PmtsConfig>(_pmtsConfigAPIRepository.GetPMTsConfigByFactoryName(_factoryCode, "PrintMaster_Path", _token)).FucValue;
                        if (Pic_Print != null)
                        {
                            PrintPath = mainpath + "\\" + Pic_Print.FileName; // _factoryCode + "-" + model.MaterialNo + ".png"; ;
                            if (File.Exists(PrintPath))
                            {
                                File.Delete(PrintPath);
                                model.Pic_PrintPath = null;
                                masterData.PrintMasterPath = null;
                            }
                        }
                        else
                        {
                            model.Pic_PrintPath = null;
                            masterData.PrintMasterPath = null;
                        }

                    }
                }

                if (Pic_Pallet != null)
                {
                    //string[] fileName = _extensionService.UploadPicture(Pic_Pallet);
                    string dataUrl = _extensionService.ResizeImageRatio(Pic_Pallet, 1080, 1080);
                    model.Pic_PalletPath = dataUrl;
                    if (model.Pic_PalletPath != null)
                    {
                        var cursor = model.Pic_PalletPath.IndexOf(',') + 1;
                        var base64 = model.Pic_PalletPath.Substring(cursor);

                        //get path form pmtcontext
                        var mainpath = JsonConvert.DeserializeObject<PmtsConfig>(_pmtsConfigAPIRepository.GetPMTsConfigByFactoryName(_factoryCode, "Pallet_Path", _token)).FucValue;
                        palletPath = mainpath + "\\" + Pic_Pallet.FileName; //_factoryCode + "-" + model.MaterialNo + ".png";


                        if (mainpath != null)
                        {
                            //byte[] bytes = Convert.FromBase64String(base64);

                            //using (MemoryStream ms = new MemoryStream(bytes))
                            //{
                            //    using (Bitmap bm2 = new Bitmap(ms))
                            //    {
                            //        bm2.Save(palletPath);
                            //    }
                            //}

                            _extensionService.UploadImage(base64, palletPath);

                            model.Pic_PalletPath = palletPath;
                            masterData.PalletizationPath = palletPath;
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(model.Pic_PalletPath))
                    {
                        //delete file in database
                        var mainpath = JsonConvert.DeserializeObject<PmtsConfig>(_pmtsConfigAPIRepository.GetPMTsConfigByFactoryName(_factoryCode, "Pallet_Path", _token)).FucValue;
                        if (Pic_Pallet != null)
                        {
                            palletPath = mainpath + "\\" + Pic_Pallet.FileName; //_factoryCode + "-" + model.MaterialNo + ".png";
                            if (masterData.PalletizationPath != palletPath)
                            {

                                if (File.Exists(palletPath))
                                {
                                    File.Delete(palletPath);
                                    model.Pic_PalletPath = null;
                                    masterData.PalletizationPath = null;
                                }
                            }
                        }
                        else
                        {
                            model.Pic_PalletPath = null;
                            masterData.PalletizationPath = null;
                        }
                    }

                }

                if (Pic_FG != null)
                {
                    //string[] fileName = _extensionService.UploadPicture(Pic_FG);
                    string dataUrl = _extensionService.ResizeImageRatio(Pic_FG, 1080, 1080);
                    model.Pic_FGPath = dataUrl;
                    if (model.Pic_FGPath != null)
                    {
                        var cursor = model.Pic_FGPath.IndexOf(',') + 1;
                        var base64 = model.Pic_FGPath.Substring(cursor);

                        //get path form pmtcontext
                        var mainpath = JsonConvert.DeserializeObject<PmtsConfig>(_pmtsConfigAPIRepository.GetPMTsConfigByFactoryName(_factoryCode, "FG_Path", _token)).FucValue;
                        fgPath = mainpath + "\\" + Pic_FG.FileName;///_factoryCode + "-" + model.MaterialNo + ".png";
                        if (mainpath != null)
                        {
                            //byte[] bytes = Convert.FromBase64String(base64);

                            //using (MemoryStream ms = new MemoryStream(bytes))
                            //{
                            //    using (Bitmap bm2 = new Bitmap(ms))
                            //    {
                            //        bm2.Save(fgPath);
                            //    }
                            //}

                            _extensionService.UploadImage(base64, fgPath);

                            model.Pic_FGPath = fgPath;
                            masterData.FgpicPath = fgPath;
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(model.Pic_FGPath))
                    {
                        //delete file in database
                        var mainpath = JsonConvert.DeserializeObject<PmtsConfig>(_pmtsConfigAPIRepository.GetPMTsConfigByFactoryName(_factoryCode, "FG_Path", _token)).FucValue;
                        if (Pic_FG != null)
                        {
                            fgPath = mainpath + "\\" + Pic_FG.FileName;// _factoryCode + "-" + model.MaterialNo + ".png";
                            if (File.Exists(fgPath))
                            {
                                File.Delete(fgPath);
                                model.Pic_FGPath = null;
                                masterData.FgpicPath = null;
                            }
                        }
                        else
                        {
                            model.Pic_FGPath = null;
                            masterData.FgpicPath = null;
                        }
                    }

                }

                masterData.User = _username;
                //pls re-check
                ParentModel masterDataModel = new ParentModel();
                masterDataModel.AppName = Globals.AppNameEncrypt;
                masterDataModel.SaleOrg = _saleOrg;
                masterDataModel.PlantCode = _factoryCode;
                masterDataModel.MasterData = masterData;

                _masterDataAPIRepository.UpdateMasterData(JsonConvert.SerializeObject(masterDataModel), _token);

                //var transactionDataModelSession = SessionExtentions.GetSession<TransactionDataModel>(_httpContextAccessor.HttpContext.Session, "TransactionDataModel");

                //_newProductService.SetTransactionStatus(ref transactionDataModelSession, "End");

                //update max step
                //_newProductService.UpdateMaxProgress(_factoryCode, ref transactionDataModelSession);

                // SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "TransactionDataModel", transactionDataModelSession);


            }
            catch (Exception ex)
            {
            }

            return model;
        }
        private string SavePictute(string TypeOfPat, string imageBase64, string materialNo)
        {
            var fullPath = "";
            if (imageBase64 != null)
            {
                var cursor = imageBase64.IndexOf(',') + 1;
                var base64 = imageBase64.Substring(cursor);

                //get path form pmtcontext
                var path = JsonConvert.DeserializeObject<PmtsConfig>(_pmtsConfigAPIRepository.GetPMTsConfigByFactoryName(_factoryCode, TypeOfPat, _token)).FucValue;
                fullPath = path + "\\" + _factoryCode + "-" + materialNo + ".png"; ;
                if (path != null)
                {

                    byte[] bytes = Convert.FromBase64String(base64);

                    using (MemoryStream ms = new MemoryStream(bytes))
                    {
                        using (Bitmap bm2 = new Bitmap(ms))
                        {
                            bm2.Save(fullPath);
                        }
                    }
                }
            }

            return fullPath;
        }

        public void ChangePathToBase64Image(ref TransactionDataModel transactionDataModel)
        {
            if (transactionDataModel.modelProductPicture != null)
            {
                var picDrawing = transactionDataModel.modelProductPicture.Pic_DrawingPath;
                var picFG = transactionDataModel.modelProductPicture.Pic_FGPath;
                var picPallet = transactionDataModel.modelProductPicture.Pic_PalletPath;
                var picPrint = transactionDataModel.modelProductPicture.Pic_PrintPath;

                picDrawing = !string.IsNullOrEmpty(picDrawing) && (picDrawing == transactionDataModel.modelProductPicture.Pic_DrawingName) ? ConvertPictureToBase64._ConvertPictureToBase64(picDrawing) : string.Empty;
                picFG = !string.IsNullOrEmpty(picFG) && (picFG == transactionDataModel.modelProductPicture.Pic_FGName) ? ConvertPictureToBase64._ConvertPictureToBase64(picFG) : string.Empty;
                picPallet = !string.IsNullOrEmpty(picPallet) && (picPallet == transactionDataModel.modelProductPicture.Pic_PalletName) ? ConvertPictureToBase64._ConvertPictureToBase64(picPallet) : string.Empty;
                picPrint = !string.IsNullOrEmpty(picPrint) && (picPrint == transactionDataModel.modelProductPicture.Pic_PrintName) ? ConvertPictureToBase64._ConvertPictureToBase64(picPrint) : string.Empty;

                transactionDataModel.modelProductPicture.Pic_DrawingPath = picDrawing;
                transactionDataModel.modelProductPicture.Pic_FGPath = picFG;
                transactionDataModel.modelProductPicture.Pic_PalletPath = picPallet;
                transactionDataModel.modelProductPicture.Pic_PrintPath = picPrint;
            }
        }

        public void CreateBom(TransactionDataModel model)
        {
            model = SessionExtentions.GetSession<TransactionDataModel>(_httpContextAccessor.HttpContext.Session, "TransactionDataModel");
            model.TransactionDetail.IsCreateBOM = true;
            model.modelCategories.Id_MatType = 7;
            model.modelCategories.MatCode = "84";
            model.modelCategories.Id_PU = 3;
            model.modelCategories.Id_SU = 3;
            model.EventFlag = "Copy";
            model.modelProductSpec = new ProductSpecViewModel();

            if (!string.IsNullOrEmpty(model.modelProductInfo.Description))
            {
                model.modelProductInfo.Description = model.modelProductInfo.Description.Length > 37 ?
                    model.modelProductInfo.Description.Substring(0, 37) + "-CC" : model.modelProductInfo.Description + "-CC";
            }

            if (!string.IsNullOrEmpty(model.modelProductInfo.PC))
            {
                var lastPcCharactor = model.modelProductInfo.PC.LastIndexOf("-");
                if (lastPcCharactor != -1)
                {
                    model.modelProductInfo.PC = model.modelProductInfo.PC.Substring(0, lastPcCharactor) + "-CC";
                }
            }

            SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "TransactionDataModel", model);
        }


        public void GetProductCatalogImage(string materialNo, string factoryCode, ref ProductPictureView modelProductPicture)
        {
            var masterdata = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNoAndFactory(factoryCode, materialNo, _token));

            if (masterdata != null)
            {
                var picDrawing = masterdata.DiecutPictPath;
                var picFG = masterdata.FgpicPath;
                var picPallet = masterdata.PalletizationPath;
                var picPrint = masterdata.PrintMasterPath;
                var semi1 = masterdata.Semi1Path;
                var semi2 = masterdata.Semi2Path;
                var semi3 = masterdata.Semi3Path;
                var semiFilePdf = masterdata.SemiFilePdfPath;//masterdata.SemiFilePdfPath;

                modelProductPicture.Pic_DrawingName = !string.IsNullOrEmpty(picDrawing) ? Path.GetFileName(picDrawing) : string.Empty;
                modelProductPicture.Pic_FGName = !string.IsNullOrEmpty(picFG) ? Path.GetFileName(picFG) : string.Empty;
                modelProductPicture.Pic_PalletName = !string.IsNullOrEmpty(picPallet) ? Path.GetFileName(picPallet) : string.Empty;
                modelProductPicture.Pic_PrintName = !string.IsNullOrEmpty(picPrint) ? Path.GetFileName(picPrint) : string.Empty;
                modelProductPicture.Semi1_Name = !string.IsNullOrEmpty(semi1) ? Path.GetFileName(semi1) : string.Empty;
                modelProductPicture.Semi2_Name = !string.IsNullOrEmpty(semi2) ? Path.GetFileName(semi2) : string.Empty;
                modelProductPicture.Semi3_Name = !string.IsNullOrEmpty(semi3) ? Path.GetFileName(semi3) : string.Empty;
                modelProductPicture.SemiFilePdf_Name = !string.IsNullOrEmpty(semiFilePdf) ? Path.GetFileName(semiFilePdf) : string.Empty;

                modelProductPicture.Pic_DrawingPath = !string.IsNullOrEmpty(picDrawing) ? ConvertPictureToBase64._ConvertPictureToBase64(picDrawing) : string.Empty;
                modelProductPicture.Pic_FGPath = !string.IsNullOrEmpty(picFG) ? ConvertPictureToBase64._ConvertPictureToBase64(picFG) : string.Empty;
                modelProductPicture.Pic_PalletPath = !string.IsNullOrEmpty(picPallet) ? ConvertPictureToBase64._ConvertPictureToBase64(picPallet) : string.Empty;
                modelProductPicture.Pic_PrintPath = !string.IsNullOrEmpty(picPrint) ? ConvertPictureToBase64._ConvertPictureToBase64(picPrint) : string.Empty;
                modelProductPicture.Semi1_Path = !string.IsNullOrEmpty(semi1) ? ConvertPictureToBase64._ConvertPictureToBase64(semi1) : string.Empty;
                modelProductPicture.Semi2_Path = !string.IsNullOrEmpty(semi2) ? ConvertPictureToBase64._ConvertPictureToBase64(semi2) : string.Empty;
                modelProductPicture.Semi3_Path = !string.IsNullOrEmpty(semi3) ? ConvertPictureToBase64._ConvertPictureToBase64(semi3) : string.Empty;

                if (!string.IsNullOrEmpty(semiFilePdf))
                {
                    Byte[] bytes = File.ReadAllBytes(semiFilePdf);
                    modelProductPicture.SemiFilePdfPath = Convert.ToBase64String(bytes);
                }
                else
                {
                    modelProductPicture.SemiFilePdfPath = string.Empty;
                }
            }
            else
            {
                modelProductPicture = new ProductPictureView();
            }

        }


        public void GetUploadImage(string materialNo, string factoryCode, ref ProductPictureView modelProductPicture)
        {
            var masterdata = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNoAndFactory(factoryCode, materialNo, _token));

            if (masterdata != null)
            {
                var picDrawing = masterdata.DiecutPictPath;
                var picFG = masterdata.FgpicPath;
                var picPallet = masterdata.PalletizationPath;
                var picPrint = masterdata.PrintMasterPath;

                modelProductPicture.MaterialNo = masterdata.MaterialNo;

                modelProductPicture.Pic_DrawingName = !string.IsNullOrEmpty(picDrawing) ? Path.GetFileName(picDrawing) : string.Empty;
                modelProductPicture.Pic_FGName = !string.IsNullOrEmpty(picFG) ? Path.GetFileName(picFG) : string.Empty;
                modelProductPicture.Pic_PalletName = !string.IsNullOrEmpty(picPallet) ? Path.GetFileName(picPallet) : string.Empty;
                modelProductPicture.Pic_PrintName = !string.IsNullOrEmpty(picPrint) ? Path.GetFileName(picPrint) : string.Empty;

                modelProductPicture.Pic_DrawingPath = !string.IsNullOrEmpty(picDrawing) ? ConvertPictureToBase64._ConvertPictureToBase64(picDrawing) : string.Empty;
                modelProductPicture.Pic_FGPath = !string.IsNullOrEmpty(picFG) ? ConvertPictureToBase64._ConvertPictureToBase64(picFG) : string.Empty;
                modelProductPicture.Pic_PalletPath = !string.IsNullOrEmpty(picPallet) ? ConvertPictureToBase64._ConvertPictureToBase64(picPallet) : string.Empty;
                modelProductPicture.Pic_PrintPath = !string.IsNullOrEmpty(picPrint) ? ConvertPictureToBase64._ConvertPictureToBase64(picPrint) : string.Empty;
            }
            else
            {
                modelProductPicture = new ProductPictureView();
            }

        }


    }
}
