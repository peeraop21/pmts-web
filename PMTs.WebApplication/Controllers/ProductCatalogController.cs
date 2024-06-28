using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.ProductCatalog;
using PMTs.Logs.Logger;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Controllers
{
    [SessionTimeout]
    public class ProductCatalogController : Controller
    {
        private readonly IHostingEnvironment _environment;
        private readonly IProductCatalogService _productCatalog;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBomService _bomService;
        private readonly IProductPicService productPicService;

        private string RoleID = "";

        public ProductCatalogController(IHostingEnvironment IHostingEnvironment, IProductCatalogService productCatalog, IHttpContextAccessor httpContextAccessor, IBomService bomService, IProductPicService productPicService)
        {
            _environment = IHostingEnvironment;
            _productCatalog = productCatalog;
            _httpContextAccessor = httpContextAccessor;
            _bomService = bomService;
            this.productPicService = productPicService;
        }

        #region Product Catalog

        [SessionTimeout]
        public IActionResult Index()
        {
            ProductCatalogModel model = new ProductCatalogModel();
            model = _productCatalog.GetProductCatalogFrist();
            string[] array = null;
            if (model.productCatalogConfigs.Count == 0)
            {
                _productCatalog.SaveProductCatalogConfig(array);
                model = _productCatalog.GetProductCatalogFrist();
            }
            var userSessionModel = SessionExtentions.GetSession<UserSessionModel>(_httpContextAccessor.HttpContext.Session, "UserSessionModel");
            RoleID = userSessionModel.DefaultRoleId.ToString();
            //RoleID = "4";
            model.Role = RoleID;
            var plant = _productCatalog.GetAllPlant();
            model.Plant = plant.Plant;

            // model = _productCatalog.GetProductCatalog();
            return View(model);
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult SettingConfig(string[] array)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;
            Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
            try
            {
                _productCatalog.SaveProductCatalogConfig(array);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage });
        }

        //[SessionTimeout]
        //[HttpPost]
        //public JsonResult SearchProCatalog(string req)
        //{
        //    bool isSuccess;
        //    string exceptionMessage = string.Empty;
        //    string Message = string.Empty;
        //    Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
        //    ProductCatalogModel model = new ProductCatalogModel();
        //    try
        //    {
        //        ProductCatalogsSearch data = new ProductCatalogsSearch();
        //        data = JsonConvert.DeserializeObject<ProductCatalogsSearch>(req);
        //        model = _productCatalog.GetProductCatalog(data);
        //        isSuccess = true;
        //        if (model.productCatalogs.Count <= 0)
        //        {
        //            Message = "0";
        //        }
        //        Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
        //    }
        //  catch (Exception ex)
        //    {
        //        isSuccess = false;
        //        Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
        //    }
        //    return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage ,View = RenderRazorViewToString(this, "_RenderTbl", model) , Message = Message });

        //}

        [SessionTimeout]
        [HttpPost]
        public JsonResult SearchProCatalogDynamicColumn(string req)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;
            string Message = string.Empty;
            Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
            ProductCatalogModel model = new ProductCatalogModel();
            try
            {
                ProductCatalogsSearch data = new ProductCatalogsSearch();
                data = JsonConvert.DeserializeObject<ProductCatalogsSearch>(req);
                if (data.Role == "4")
                {
                    string[] array = JsonConvert.DeserializeObject<string[]>(data.FactoryCode);
                    string condition = "";
                    foreach (var item in array)
                    {
                        string mat = "'" + item + "',";
                        condition = condition + mat;
                    }

                    if (!string.IsNullOrEmpty(condition))
                    { condition = condition.Substring(0, condition.Length - 1); }
                    data.FactoryCode = condition;

                    string[] array2 = JsonConvert.DeserializeObject<string[]>(data.FactoryCodeProduction);
                    string condition2 = "";
                    foreach (var item2 in array2)
                    {
                        string mat = "'" + item2 + "',";
                        condition2 = condition2 + mat;
                    }

                    if (!string.IsNullOrEmpty(condition2))
                    { condition2 = condition2.Substring(0, condition2.Length - 1); }
                    data.FactoryCodeProduction = condition2;
                }

                model = _productCatalog.GetProductCatalog(data);
                isSuccess = true;
                if (model.productCatalogs.Count <= 0)
                {
                    Message = "0";
                }
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = "", Message = Message, jsonresult = model.productCatalogs, RecordCount = model.RecordCount });
        }

        [SessionTimeout]
        public static string RenderRazorViewToString(Controller controller, string viewName, object model)
        {
            controller.ViewData.ModelState.Clear();
            controller.ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                ViewEngineResult viewResult = null;
                var engine = controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;

                viewResult = engine.FindView(controller.ControllerContext, viewName, false);

                var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw, new HtmlHelperOptions());
                viewResult.View.RenderAsync(viewContext);

                return sw.GetStringBuilder().ToString();
            }
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult SaveRemark(string req)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;
            Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
            try
            {
                var data = JsonConvert.DeserializeObject<ProductCatalogRemark>(req);
                _productCatalog.SaveProductCatalogRemark(data);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage });
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult GetRemark(string pc)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;
            Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            ProductCatalogRemark model = new ProductCatalogRemark();
            try
            {
                //  var data = JsonConvert.DeserializeObject<ProductCatalogRemark>(req);
                model = _productCatalog.GetProductCatalogRemark(pc);
                if (model == null)
                {
                    model = new ProductCatalogRemark();
                    model.PC = pc;
                }
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return Json(model);
        }

        [SessionTimeout]
        public async Task<IActionResult> ExportProductCatalogExcel(string req)
        {
            ProductCatalogsSearch data = new ProductCatalogsSearch();
            //string normal = ConvertHexToString(req, System.Text.Encoding.Unicode);
            //data = JsonConvert.DeserializeObject<ProductCatalogsSearch>(normal);
            //var myBUs = _productCatalog.CreateDynamicDataTable(data); ;//await _context.BusinessUnits.ToListAsync();
            //// above code loads the data using LINQ with EF (query of table), you can substitute this with any data source.
            data = JsonConvert.DeserializeObject<ProductCatalogsSearch>(req);
            var myBUs = _productCatalog.CreateDynamicDataTable(data);
            var streamer = new MemoryStream();

            using (var package = new ExcelPackage(streamer))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                //Set some properties of the Excel document
                package.Workbook.Properties.Author = "PMTs";
                package.Workbook.Properties.Title = "ProductCatalog";
                package.Workbook.Properties.Subject = "PMTs ProductCatalog";
                package.Workbook.Properties.Created = DateTime.Now;

                ExcelWorksheet workSheet = package.Workbook.Worksheets.Add("ProductCatalog");
                // workSheet.Cells[1, 1].Value = "Code";
                workSheet.Cells.LoadFromDataTable(myBUs, true);
                workSheet.Cells.AutoFitColumns();
                var headerCell = workSheet.Cells["A1:CA1"];
                headerCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headerCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.BurlyWood);
                var headerFont = headerCell.Style.Font;
                headerFont.Bold = true;

                package.Save();
            }
            streamer.Position = 0;

            string excelName = $"ProductCatalog-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";
            //string excelName = $"ProductCatalog-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.pdf";
            // above I define the name of the file using the current datetime.

            return File(streamer, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName); // this will be the actual export.
            //return File(streamer, "application/pdf", excelName); // this will be the actual export.
        }

        [SessionTimeout]
        public JsonResult EncryptJsonStringBeforePostByURL(string req)
        {
            string hex = ConvertStringToHex(req, System.Text.Encoding.Unicode);
            return Json(hex);
        }

        #region [Encrypt Decrypt]

        private static string Encrypt(string clearText)
        {
            string EncryptionKey = "abc123";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        private static string Decrypt(string cipherText)
        {
            string EncryptionKey = "abc123";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        public static string ConvertStringToHex(String input, System.Text.Encoding encoding)
        {
            Byte[] stringBytes = encoding.GetBytes(input);
            StringBuilder sbBytes = new StringBuilder(stringBytes.Length * 2);
            foreach (byte b in stringBytes)
            {
                sbBytes.AppendFormat("{0:X2}", b);
            }
            return sbBytes.ToString();
        }

        public static string ConvertHexToString(String hexInput, System.Text.Encoding encoding)
        {
            int numberChars = hexInput.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hexInput.Substring(i, 2), 16);
            }
            return encoding.GetString(bytes);
        }

        #endregion [Encrypt Decrypt]

        [SessionTimeout]
        [HttpPost]
        public JsonResult GetHoldMaterial(string Material)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;
            Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            ProductCatalogModel model = new ProductCatalogModel();
            try
            {
                model = _productCatalog.GetHoldMaterial(Material);
                if (model.holdMaterial == null)
                {
                    model.holdMaterial = new HoldMaterial();
                }
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return Json(new { DataHoldMaterial = model.holdMaterial, ViewHoldMaterialHistory = RenderRazorViewToString(this, "_RenderTblHoldHistory", model) });
        }

        public PartialViewResult _RenderTblHoldHistory()
        {
            return PartialView();
        }

        public PartialViewResult _RenderTbl()
        {
            return PartialView();
        }

        public PartialViewResult _RenderDropDownPlant()
        {
            return PartialView();
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult RenderBom(string material, string fac)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;
            Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            ProductCatalogModel model = new ProductCatalogModel();
            try
            {
                if (string.IsNullOrEmpty(fac))
                {
                    var userSessionModel = SessionExtentions.GetSession<UserSessionModel>(_httpContextAccessor.HttpContext.Session, "UserSessionModel");
                    BOMViewModel bom = new BOMViewModel();
                    // _bomService.GetMatParentToProductCatalog(ref bom, material, "MaterialNo", userSessionModel.FactoryCode);
                    var tmp = _productCatalog.GetBom(userSessionModel.FactoryCode, material);
                    model.bomStructView = tmp.bomStructView;
                    //  prod
                    //  model.bomView = bom;
                }
                else
                {
                    if (fac == "[]")
                    {
                        var plant = _productCatalog.GetAllPlantProduction();
                        model.Plant = plant.Plant;
                        foreach (var item in model.Plant)
                        {
                            var tmp = _productCatalog.GetBom(item.PlantCode, material);
                            //BOMViewModel bom = new BOMViewModel();
                            //_bomService.GetMatParentToProductCatalog(ref bom, material, "MaterialNo", item);
                            // model.bomView.lstMasterData = new List<MasterData>();
                            //model.bomviews.AddRange(bom.lstMasterData);
                            model.bomStructView.AddRange(tmp.bomStructView);
                        }
                    }
                    else
                    {
                        string[] array = JsonConvert.DeserializeObject<string[]>(fac);
                        string condition = "";
                        foreach (var item in array)
                        {
                            var tmp = _productCatalog.GetBom(item, material);
                            //BOMViewModel bom = new BOMViewModel();
                            //_bomService.GetMatParentToProductCatalog(ref bom, material, "MaterialNo", item);
                            // model.bomView.lstMasterData = new List<MasterData>();
                            //model.bomviews.AddRange(bom.lstMasterData);
                            model.bomStructView.AddRange(tmp.bomStructView);

                            //BOMViewModel bom = new BOMViewModel();
                            //_bomService.GetMatParentToProductCatalog(ref bom, material, "MaterialNo", item);
                            //model.bomView.lstMasterData = new List<MasterData>();
                            //model.bomView.lstMasterData.AddRange(bom.lstMasterData);
                        }
                    }
                }

                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return Json(new { View = RenderRazorViewToString(this, "_DataTableBomParent", model) });
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult RefreshDropdown()
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;
            Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            ProductCatalogModel model = new ProductCatalogModel();
            try
            {
                var plant = _productCatalog.GetAllPlant();
                model.Plant = plant.Plant;
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return Json(new { View = RenderRazorViewToString(this, "_RenderDropDownPlant", model) });
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult RefreshDropdownProduction()
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;
            Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            ProductCatalogModel model = new ProductCatalogModel();
            try
            {
                var plant = _productCatalog.GetAllPlantProduction();
                model.Plant = plant.Plant;
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return Json(new { View = RenderRazorViewToString(this, "_RenderDropDownPlantProduction", model) });
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult SaveHoldMaterial(string req)
        {
            bool isSuccess = true;
            string exceptionMessage = string.Empty;
            Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            ProductCatalogModel model = new ProductCatalogModel();
            try
            {
                var data = JsonConvert.DeserializeObject<HoldMaterial>(req);
                HoldMaterialHistory dataHis = new HoldMaterialHistory();
                dataHis.MaterialNo = data.MaterialNo;
                dataHis.HoldRemark = data.HoldRemark;
                dataHis.ChangeProductNo = data.ChangeProductNo;
                dataHis.IsLocked = data.IsLocked;

                model = _productCatalog.GetHoldMaterial(data.MaterialNo);
                if (model.holdMaterial == null)
                {
                    model.holdMaterial = new HoldMaterial();
                }
                if (string.IsNullOrEmpty(model.holdMaterial.MaterialNo))
                {
                    _productCatalog.SaveHoldMaterial(data);
                    _productCatalog.SaveHoldMaterialHistory(dataHis);
                }
                else
                {
                    _productCatalog.UpdateHoldMaterial(data);
                    _productCatalog.SaveHoldMaterialHistory(dataHis);
                }

                #region Call api E-ordering

                _productCatalog.CallApiE_Ordering(data);

                #endregion Call api E-ordering

                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                isSuccess = false;
                exceptionMessage = ex.Message;
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            //return Json(model);
            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, result = model });
        }

        public JsonResult GetOrderItemInMoData(string Fac, string Mat)
        {
            var str = _productCatalog.GetOrderItemByMoData(Fac, Mat);
            return Json(str);
        }

        public JsonResult GetAllCompanyProfile()
        {
            var model = _productCatalog.GetAllCompanyProfile();
            return Json(model);
        }

        public JsonResult GetAllCompanyProfileByLogin()
        {
            var model = _productCatalog.GetAllCompanyProfileByLogin();
            //model.Group = 5;
            return Json(model);
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult GetImageOfProductCatalog(string factoryCode, string materialNo)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;
            var productPicModel = new ProductPictureView();
            Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

            try
            {
                productPicService.GetProductCatalogImage(materialNo, factoryCode, ref productPicModel);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                isSuccess = false;
                exceptionMessage = ex.Message;
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return Json(new
            {
                IsSuccess = isSuccess,
                ExceptionMessage = exceptionMessage,
                palletizationFileName = productPicModel.Pic_PalletName,
                palletizationBase64 = productPicModel.Pic_PalletPath,
                diecutFileName = productPicModel.Pic_DrawingName,
                diecutBase64 = productPicModel.Pic_DrawingPath,
                printmasterFileName = productPicModel.Pic_PrintName,
                printmasterBase64 = productPicModel.Pic_PrintPath,
                fgFileName = productPicModel.Pic_FGName,
                fgBase64 = productPicModel.Pic_FGPath,
                semi1FileName = productPicModel.Semi1_Name,
                semi1Base64 = productPicModel.Semi1_Path,
                semi2FileName = productPicModel.Semi2_Name,
                semi2Base64 = productPicModel.Semi2_Path,
                semi3FileName = productPicModel.Semi3_Name,
                semi3Base64 = productPicModel.Semi3_Path,
                semiPDFFileName = productPicModel.SemiFilePdf_Name,
                semiPDFFileBase64 = productPicModel.SemiFilePdfPath,
            });
        }

        #endregion Product Catalog

        #region BOMMaterialProduct

        [SessionTimeout]
        public IActionResult BOMMaterialProduct()
        {
            var bomMaterialProductModels = new List<BOMMaterialProductModel>();
            return View(bomMaterialProductModels);
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult SearchBOMMaterialProduct(string custId, string custName, string custCode, string pc1, string pc2, string pc3)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;
            string Message = string.Empty;
            var bomMaterialProductModels = new List<BOMMaterialProductModel>();
            Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
            try
            {
                _productCatalog.SearchBOMMaterialProduct(ref bomMaterialProductModels, custId, custName, custCode, pc1, pc2, pc3);
                //bomMaterialProductModels.Add(new BOMMaterialProductModel
                //{
                //    MaterialNo = "Z00112233",
                //    CusId = "10015912",
                //    CustCode = "SLUBRSS",
                //    Pc = "4911-031-01-123123",
                //    Board = "OOOO/1234/OOOO/1234/OOOO/1234/OOOO/1234",
                //    CustName = "บริษัทไพโอเนียร์ แมนูแฟคเจอริ่ง(ประเทศไทย) จำกัด  12345667",
                //    PartNo = "PG-WC0293 for C-125317",
                //    Flute = "BCCC",
                //    SaleText1 = "A452XXXX ลัง TEST G3 CARTON HONEY LEMON",
                //    Hig = 44444,
                //    Leg = 55555,
                //    Wid = 66666,
                //    Price = 49000,
                //    Quantity = 5,
                //    TotalPrice = 240

                //});
                //bomMaterialProductModels.Add(new BOMMaterialProductModel {
                //    MaterialNo = "Z22222222",
                //    Pc = "4911-031-01-CC",
                //    TotalPrice = 590
                //});
                isSuccess = true;

                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_BOMMaterialProductTable", bomMaterialProductModels) });
        }

        #endregion BOMMaterialProduct

        #region ScalePriceMatProduct

        [SessionTimeout]
        public IActionResult ScalePriceMatProduct()
        {
            var model = new ScalePriceMatProductViewModel();
            model.scalePriceMatProductModels = new List<ScalePriceMatProductModel>();
            model.MaterialTypeList = new List<MaterialType>();
            model.Plants = new List<PlantModel>();
            _productCatalog.GetMaterialTypesAndPlants(ref model);
            return View(model);
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult SearchScalePriceMatProduct(string custId, string custName, string custCode, string pc1, string pc2, string pc3, string materialType, string salePlants, string plantPdts, string materialNo)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;
            string Message = string.Empty;

            var model = new ScalePriceMatProductViewModel();
            model.scalePriceMatProductModels = new List<ScalePriceMatProductModel>();
            model.MaterialTypeList = new List<MaterialType>();
            model.Plants = new List<PlantModel>();
            Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
            try
            {
                _productCatalog.SearchScalePriceMatProduct(ref model, custId, custName, custCode, pc1, pc2, pc3, materialType, salePlants, plantPdts, materialNo);

                isSuccess = true;

                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_ScalePriceMatProductTable", model.scalePriceMatProductModels) });
        }

        #endregion ScalePriceMatProduct
    }
}