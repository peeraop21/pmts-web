using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Org.BouncyCastle.Utilities.Collections;
using PMTs.DataAccess;
using PMTs.DataAccess.ComplexModel;
using PMTs.DataAccess.ComplexModel;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.NewProduct;
using PMTs.DataAccess.ModelView.Report;
using PMTs.DataAccess.Repository;
using PMTs.Logs.Logger;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Services;
using PMTs.WebApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Controllers
{
    [SessionTimeout]
    public class MaintenanceProductInfoController : Controller
    {
        private readonly IMasterDataService _masterDataService;
        private readonly INewProductService _newProductService;
        private readonly IExtensionService _extensionService;
        private readonly ICategoryService _categoryService;
        private readonly IMasterCardService _masterCardService;
        private readonly IProductInfoService _productInfoService;
        private readonly IMaintenanceProductInfoService _maintenanceProductInfoService;
        private readonly IHttpContextAccessor httpContextAccessor;

        public MaintenanceProductInfoController(
            IMasterDataService masterDataService,
            INewProductService newProductService,
            IExtensionService extensionService,
            ICategoryService categoryService,
            IMasterCardService masterCardService,
            IProductInfoService productInfoService,
            IMaintenanceProductInfoService maintenanceProductInfoService,
            IHttpContextAccessor httpContextAccessor
            )
        {
            _masterDataService = masterDataService;
            _newProductService = newProductService;
            _extensionService = extensionService;
            _categoryService = categoryService;
            _masterCardService = masterCardService;
            _productInfoService = productInfoService;
            _maintenanceProductInfoService = maintenanceProductInfoService;
            this.httpContextAccessor = httpContextAccessor;
        }

        #region Product View List
        [SessionTimeout]
        public IActionResult Index()
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                return View();
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch
            {
                throw new Exception();
                return RedirectToAction("index", "Login");

            }

        }

        [SessionTimeout]
        public IActionResult ViewListProduct(string TxtSearch, string ddlSearch, string flag)
        {
            List<MasterDataRoutingModel> masterDataRoutingModel = new List<MasterDataRoutingModel>();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _masterDataService.GetMasterData(ref masterDataRoutingModel, ddlSearch, TxtSearch, flag, isMaterialOnly: false);
                SessionExtentions.SetSession(HttpContext.Session, "TransactionDataModel", null);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }

            return View(masterDataRoutingModel);
        }


        [SessionTimeout]
        [HttpPost]
        public async Task<IActionResult> SearchProductFromTextFile(List<IFormFile> fileUpload)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;
            var materialNos = new string[1000];
            var masterDataRoutings = new List<MasterDataRoutingModel>();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                masterDataRoutings = await _masterDataService.GetMasterDataFromFile(fileUpload);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                isSuccess = true;
                materialNos = masterDataRoutings.Select(m => m.MaterialNo + "-" + m.PdisStatus).ToArray();
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                isSuccess = false;
                return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "ViewListProduct", masterDataRoutings), materialNos = materialNos });
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "ViewListProduct", masterDataRoutings), materialNos = materialNos });

        }


        [SessionTimeout]
        [HttpGet]
        public IActionResult SearchProduct(string TxtSearch, string ddlSearch, string flag)
        {
            List<MasterDataRoutingModel> masterDataRoutingModel = new List<MasterDataRoutingModel>();

            try
            {
                _masterDataService.GetMasterData(ref masterDataRoutingModel, ddlSearch, TxtSearch, flag, isMaterialOnly: false);
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }

            return View(masterDataRoutingModel);
            // return View();
        }

        [SessionTimeout]
        [HttpPost]
        public IActionResult SubmitCheckbox(string DataTableMatconsole)
        {
            if (DataTableMatconsole != null)
            {
                var MatArray = DataTableMatconsole.Split(',');  // now you have an array of 3 strings
                                                                //   DataTableMatconsole2 = String.Join(",", fooArray);
                                                                // Loop over strings.
                foreach (string matNo in MatArray)
                {
                    Console.WriteLine(matNo);
                }
            }

            return View();
        }

        [SessionTimeout]
        [HttpPut]
        public JsonResult DeleteViewListProduct(string Material)
        {

            bool isSuccess;
            string exceptionMessage = string.Empty;

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceProductInfoService.DeleteProductViewList(Material);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                exceptionMessage = ex.Message;
                isSuccess = false;
            }
            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage });
        }
        [SessionTimeout]
        [HttpGet]
        public JsonResult GetRoutingDataList(string MaterialNo)
        {
            List<Routing> model = new List<Routing>();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                model = _maintenanceProductInfoService.GetRoutingDataList(MaterialNo);
                model.ForEach(o => o.Machine = $"{o.SeqNo.ToString()} | {o.Machine}");
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(new { IsSuccess = true, ExceptionMessage = "", Data = model });
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json(RenderView.RenderRazorViewToString(this, "_SearchRawMaterial", null));
            }

        }
        [SessionTimeout]
        [HttpGet]
        public JsonResult GetBomRawMatDataList(string MaterialNo)
        {
            List<PpcRawMaterialProductionBom> model = new List<PpcRawMaterialProductionBom>();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                model = _maintenanceProductInfoService.GetBomRawMatDataList(MaterialNo);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(new { IsSuccess = true, ExceptionMessage = "", Data = model });
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json(RenderView.RenderRazorViewToString(this, "_SearchRawMaterial", null));
            }

        }
        [SessionTimeout]
        [HttpGet]
        public JsonResult GetMasterDataList(string MaterialNo, string PC)
        {
            List<MasterData> model = new List<MasterData>();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                model = _masterDataService.GetMasterDataList(MaterialNo, PC);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(new { IsSuccess = true, ExceptionMessage = "", Data = model });
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json(new { IsSuccess = false, ExceptionMessage = ex.Message, Data = model });
            }

        }
        [SessionTimeout]
        [HttpPut]
        public JsonResult ReplaceRoutingDataList(string MaterialFrom, string MaterialTo)
        {

            bool isSuccess;
            string exceptionMessage = string.Empty;

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceProductInfoService.ReplaceRoutingDataList(MaterialFrom, MaterialTo);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                exceptionMessage = ex.Message;
                isSuccess = false;
            }
            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage });
        }
        #endregion

        #region DetailOfProduct
        [SessionTimeout]
        public IActionResult DetailProduct(string MaterialNo)
        {

            MasterCardMO Mo = new MasterCardMO();
            var basePrintMastercard = new BasePrintMastercardData();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _masterCardService.GetBaseOfPrintMastercard(ref basePrintMastercard, new List<string> { MaterialNo });

                Mo = _masterCardService.GetMasterCard(MaterialNo, basePrintMastercard);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return View(Mo);
        }

        #endregion

        #region Reuse Material NO

        [SessionTimeout]
        public IActionResult ReuseMaterialNo()
        {
            List<MasterDataRoutingModel> masterDataRoutingModel = new List<MasterDataRoutingModel>();
            return View(masterDataRoutingModel);
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult UpdateReuseMaterialNO([FromBody] List<string> materialNOs)
        {
            bool isSuccess = false;
            string exceptionMessage = string.Empty;
            var masterDatas = new List<MasterData>();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _masterDataService.UpdateReuseMaterialNos(ref masterDatas, materialNOs);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                masterDatas = new List<MasterData>();
                exceptionMessage = ex.Message;
                return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage });
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage });
        }

        [SessionTimeout]
        [HttpPost]
        public async Task<JsonResult> SearchReuseMaterialNo(string materialNo)
        {
            bool isSuccess = false;
            string exceptionMessage = string.Empty;
            //var masterDatas = new List<MasterData>();
            var masterDatas = new List<MasterDataRoutingModel>();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _masterDataService.GetReUseMasterDatas(ref masterDatas, materialNo, null);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                //masterDatas = new List<MasterData>();
                masterDatas = new List<MasterDataRoutingModel>();
                exceptionMessage = ex.Message;
                return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_ReuseMaterialNoTable", masterDatas) });
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_ReuseMaterialNoTable", masterDatas) });
        }

        [SessionTimeout]
        [HttpPost]
        public async Task<JsonResult> SearchReuseMaterialNoFromFile(IFormFile fileUpload)
        {
            bool isSuccess = false;
            string exceptionMessage = string.Empty;
            var masterDatas = new List<MasterDataRoutingModel>();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var materialNos = new List<string>();
                var result = new StringBuilder();

                using (var reader = new StreamReader(fileUpload.OpenReadStream()))
                {
                    while (reader.Peek() >= 0)
                    {
                        var data = await reader.ReadLineAsync();
                        result.Append(data);
                        materialNos.Add(result.ToString().Trim());

                        result.Clear();
                    }
                }

                _masterDataService.GetReUseMasterDatas(ref masterDatas, null, materialNos);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                masterDatas = new List<MasterDataRoutingModel>();
                exceptionMessage = ex.Message;
                return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_ReuseMaterialNoTable", masterDatas) });
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_ReuseMaterialNoTable", masterDatas) });
        }

        #endregion

        #region Change Board New Material

        public IActionResult ChangeBoardNewMaterial()
        {
            List<ChangeBoardNewMaterial> masterDatas = new List<ChangeBoardNewMaterial>();
            List<string> boards = new List<string>();
            List<string> grades = new List<string>();
            List<Customer> customers = new List<Customer>();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                //
                _maintenanceProductInfoService.InitialDataChangeBoardNewMaterial(ref boards, ref customers, ref grades);
                ViewBag.Boards = boards;
                ViewBag.Grades = grades;
                ViewBag.Customers = customers;
                SessionExtentions.SetSession(HttpContext.Session, "TransactionDataModel", null);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }

            return View(masterDatas);
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult ImportChangeBoardNewMaterialFromExcel(List<IFormFile> fileUpload, bool checkImport)
        {
            var isSuccess = false;
            var message = "";
            var saleOrg = "";
            var failCount = 0;
            var alreadyCount = 0;
            var modelResult = new List<ChangeBoardNewMaterial>();
            var failedResult = new List<ChangeBoardNewMaterial>();
            var alreadyResult = new List<ChangeBoardNewMaterial>();

            try
            {
                modelResult = SessionExtentions.GetSession<List<ChangeBoardNewMaterial>>(httpContextAccessor.HttpContext.Session, "ChangeBoardNewMaterials");
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                if (modelResult == null || checkImport)
                {
                    modelResult = new List<ChangeBoardNewMaterial>();
                }

                if (fileUpload.Count > 0 || (fileUpload.Count == 0 && !checkImport))
                {
                    if (fileUpload.Count > 0)
                    {
                        var fileExcel = fileUpload[0];

                        long size = fileExcel.Length;
                        if (size > 150000)
                        {
                            throw new Exception("The import file size limitation is 150KB.");
                        }
                    }

                    if (!checkImport)
                    {
                        modelResult = modelResult.Where(m => m.IsCreatedSuccess).ToList();
                    }
                    _maintenanceProductInfoService.ReadExcelFileToChangeBoardNewMaterial(ref modelResult, fileUpload, checkImport);
                    failedResult = modelResult.Where(m => !m.IsCreatedSuccess).ToList();
                    failCount = failedResult != null && failedResult.Count > 0 ? failedResult.Count : 0;
                    alreadyResult = modelResult.Where(m => m.IsCreatedSuccess).ToList();
                    alreadyCount = alreadyResult != null && alreadyResult.Count > 0 ? alreadyResult.Count : 0;
                }

                SessionExtentions.SetSession(httpContextAccessor.HttpContext.Session, "ChangeBoardNewMaterials", null);

                if (checkImport)
                {
                    SessionExtentions.SetSession(httpContextAccessor.HttpContext.Session, "ChangeBoardNewMaterials", modelResult);
                }

                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                if (ex.Message.ToString().Contains("The import file size limitation is 100KB"))
                {
                    message = ex.Message;
                }
                else
                {
                    message = "File excel must not be open or data is incorrect. Please try again...";
                }
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return Json(new
            {
                IsSuccess = isSuccess,
                ExceptionMessage = message,
                View = RenderView.RenderRazorViewToString(this, "_ChangeBoardNewMaterialTable", modelResult),
                ViewAlreadys = RenderView.RenderRazorViewToString(this, "_AlreadyChangeBoardNewMaterialTable", alreadyResult),
                ViewFaileds = RenderView.RenderRazorViewToString(this, "_FailedChangeBoardNewMaterialTable", failedResult),
                FailedCount = failCount,
                AlreadyCount = alreadyCount
            });
        }

        [SessionTimeout]
        public async Task<IActionResult> ExportChangeBoardNewMaterialTemplate()
        {
            var stream = new MemoryStream();

            string excelName = $"PMTs-ChangeBoardNewMaterialTemplate.xlsx";
            var userSessionModel = SessionExtentions.GetSession<UserSessionModel>(httpContextAccessor.HttpContext.Session, "UserSessionModel");

            var _token = userSessionModel.Token;

            try
            {
                //Create a new ExcelPackage
                using (ExcelPackage excelPackage = new ExcelPackage(stream))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    //Set some properties of the Excel document
                    excelPackage.Workbook.Properties.Author = "PMTs";
                    excelPackage.Workbook.Properties.Title = "ChangeBoardNewMaterial";
                    excelPackage.Workbook.Properties.Subject = "PMTs ChangeBoardNewMaterial";
                    excelPackage.Workbook.Properties.Created = DateTime.Now;

                    //Create the WorkSheet
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("ChangeBoardNewMaterial_Template");

                    var yellowColFromRGB = System.Drawing.Color.Yellow; //FromArgb(255, 249, 82)
                    var greenColFromRGB = System.Drawing.Color.FromArgb(169, 208, 142);
                    worksheet.Cells[1, 1].Value = "File Upload Template";
                    worksheet.Cells[1, 1, 1, 9].Merge = true; //Merge columns start and end range
                    worksheet.Cells[1, 1, 1, 9].Style.Font.Bold = true; //Font should be bold
                    worksheet.Cells[1, 1, 1, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Alignment is center
                    worksheet.Cells[1, 1, 1, 9].Style.Font.Size = 16;
                    worksheet.Cells[1, 1, 1, 9].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, 1, 1, 9].AutoFitColumns();
                    worksheet.Cells[1, 1, 1, 9].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(192, 0, 0));
                    worksheet.Cells[1, 1, 1, 9].Style.Font.Color.SetColor(System.Drawing.Color.White);

                    worksheet.Cells[2, 1].Value = "Material_No";
                    worksheet.Cells[2, 2].Value = "PC";
                    worksheet.Cells[2, 3].Value = "Flute";
                    worksheet.Cells[2, 4].Value = "NewBoard";
                    worksheet.Cells[2, 5].Value = "Price";
                    worksheet.Cells[2, 6].Value = "HVA";
                    worksheet.Cells[2, 7].Value = "BOARD ALTERNATIVE";
                    worksheet.Cells[2, 8].Value = "Change";
                    worksheet.Cells[2, 9].Value = "Error Message";

                    for (int i = 1; i <= 9; i++)
                    {
                        worksheet.Cells[2, i].Style.Font.Bold = true;
                        worksheet.Cells[2, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[2, i].AutoFitColumns();
                        worksheet.Cells[2, i].Style.Fill.BackgroundColor.SetColor(yellowColFromRGB);
                        worksheet.Cells[2, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        if (i <= 5)
                        {
                            worksheet.Cells[2, i].Style.Fill.BackgroundColor.SetColor(greenColFromRGB);
                        }

                        if (i == 9)
                        {
                            worksheet.Cells[2, i].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                        }

                    }

                    worksheet.Cells["A1:G100"].Style.Numberformat.Format = "@";

                    //Save your file
                    excelPackage.Save();
                }

                stream.Position = 0;
                // above I define the name of the file using the current datetime.
            }
            catch (Exception ex)
            {

            }

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName); // this will be the actual export.
        }

        [SessionTimeout]
        public async Task<IActionResult> ExportChangeBoardNewMaterialTemplateWithData(string typeSearch, List<string> boards, List<string> grades, List<string> customers)
        {
            var stream = new MemoryStream();

            string excelName = $"PMTs-ChangeBoardNewMaterialTemplate.xlsx";
            var userSessionModel = SessionExtentions.GetSession<UserSessionModel>(httpContextAccessor.HttpContext.Session, "UserSessionModel");

            var _token = userSessionModel.Token;
            SearchMaterialTemplateParam searchMaterialTemplateParam = new SearchMaterialTemplateParam()
            {
                TypeSearch = typeSearch,
                Boards = boards,
                Grades = grades,
                Customers = customers
            };
            try
            {
                //Create a new ExcelPackage
                using (ExcelPackage excelPackage = new ExcelPackage(stream))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    //Set some properties of the Excel document
                    excelPackage.Workbook.Properties.Author = "PMTs";
                    excelPackage.Workbook.Properties.Title = "ChangeBoardNewMaterial";
                    excelPackage.Workbook.Properties.Subject = "PMTs ChangeBoardNewMaterial";
                    excelPackage.Workbook.Properties.Created = DateTime.Now;

                    //Create the WorkSheet
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("ChangeBoardNewMaterial_Template");

                    var yellowColFromRGB = System.Drawing.Color.Yellow; //FromArgb(255, 249, 82)
                    var greenColFromRGB = System.Drawing.Color.FromArgb(169, 208, 142);
                    worksheet.Cells[1, 1].Value = "File Upload Template";
                    worksheet.Cells[1, 1, 1, 9].Merge = true; //Merge columns start and end range
                    worksheet.Cells[1, 1, 1, 9].Style.Font.Bold = true; //Font should be bold
                    worksheet.Cells[1, 1, 1, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Alignment is center
                    worksheet.Cells[1, 1, 1, 9].Style.Font.Size = 16;
                    worksheet.Cells[1, 1, 1, 9].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, 1, 1, 9].AutoFitColumns();
                    worksheet.Cells[1, 1, 1, 9].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(192, 0, 0));
                    worksheet.Cells[1, 1, 1, 9].Style.Font.Color.SetColor(System.Drawing.Color.White);

                    worksheet.Cells[2, 1].Value = "Material_No";
                    worksheet.Cells[2, 2].Value = "PC";
                    worksheet.Cells[2, 3].Value = "Flute";
                    worksheet.Cells[2, 4].Value = "NewBoard";
                    worksheet.Cells[2, 5].Value = "Price";
                    worksheet.Cells[2, 6].Value = "HVA";
                    worksheet.Cells[2, 7].Value = "BOARD ALTERNATIVE";
                    worksheet.Cells[2, 8].Value = "Change";
                    worksheet.Cells[2, 9].Value = "Error Message";

                    for (int i = 1; i <= 9; i++)
                    {
                        worksheet.Cells[2, i].Style.Font.Bold = true;
                        worksheet.Cells[2, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[2, i].AutoFitColumns();
                        worksheet.Cells[2, i].Style.Fill.BackgroundColor.SetColor(yellowColFromRGB);
                        worksheet.Cells[2, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        if (i <= 5)
                        {
                            worksheet.Cells[2, i].Style.Fill.BackgroundColor.SetColor(greenColFromRGB);
                        }

                        if (i == 9)
                        {
                            worksheet.Cells[2, i].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                        }

                    }


                    var data = _maintenanceProductInfoService.GetForTemplateChangeBoardNewMaterials(searchMaterialTemplateParam);
             
                    for (int i = 0; i < data.Count(); i++)
                    {
                        worksheet.Cells[3 + i, 1].Value = data[i].CopyMaterialNo;
                        worksheet.Cells[3 + i, 2].Value = data[i].PC;
                        worksheet.Cells[3 + i, 3].Value = data[i].Flute;
                        worksheet.Cells[3 + i, 4].Value = data[i].NewBoard;
                        worksheet.Cells[3 + i, 5].Value = data[i].Price;
                        worksheet.Cells[3 + i, 6].Value = data[i].HighValue;
                        worksheet.Cells[3 + i, 7].Value = data[i].BoardAlternative;
                        worksheet.Cells[3 + i, 8].Value = data[i].Change;
                        worksheet.Cells[3 + i, 9].Value = "";
                    }
                    worksheet.Cells["A1:G100"].Style.Numberformat.Format = "@";

                    //Save your file
                    excelPackage.Save();
                }

                stream.Position = 0;
                // above I define the name of the file using the current datetime.
            }
            catch (Exception ex)
            {

            }

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName); // this will be the actual export.
        }


        [SessionTimeout]
        public async Task<IActionResult> ExportInvalidChangeBoardNewMaterialDataTemplate()
        {
            var stream = new MemoryStream();

            string excelName = $"PMTs-InvalidChangeBoardNewMaterialDataTemplate.xlsx";
            var userSessionModel = SessionExtentions.GetSession<UserSessionModel>(httpContextAccessor.HttpContext.Session, "UserSessionModel");
            var modelResults = new List<ChangeBoardNewMaterial>();
            var FailedResults = new List<ChangeBoardNewMaterial>();
            var invalidRow = 2;

            modelResults = SessionExtentions.GetSession<List<ChangeBoardNewMaterial>>(httpContextAccessor.HttpContext.Session, "ChangeBoardNewMaterials");
            if (modelResults != null && modelResults.Count > 0)
            {
                FailedResults = modelResults.Where(m => !m.IsCreatedSuccess).ToList();
            }

            try
            {
                //Create a new ExcelPackage
                using (ExcelPackage excelPackage = new ExcelPackage(stream))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    //Set some properties of the Excel document
                    excelPackage.Workbook.Properties.Author = "PMTs";
                    excelPackage.Workbook.Properties.Title = "ChangeBoardNewMaterial";
                    excelPackage.Workbook.Properties.Subject = "PMTs ChangeBoardNewMaterial";
                    excelPackage.Workbook.Properties.Created = DateTime.Now;

                    //Create the WorkSheet
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("ChangeBoardNewMaterial_Template");

                    var yellowColFromRGB = System.Drawing.Color.Yellow; //FromArgb(255, 249, 82)
                    var greenColFromRGB = System.Drawing.Color.FromArgb(169, 208, 142);

                    worksheet.Cells[1, 1].Value = "File Upload Template";
                    worksheet.Cells[1, 1, 1, 9].Merge = true; //Merge columns start and end range
                    worksheet.Cells[1, 1, 1, 9].Style.Font.Bold = true; //Font should be bold
                    worksheet.Cells[1, 1, 1, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Alignment is center
                    worksheet.Cells[1, 1, 1, 9].Style.Font.Size = 16;
                    worksheet.Cells[1, 1, 1, 9].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, 1, 1, 9].AutoFitColumns();
                    worksheet.Cells[1, 1, 1, 9].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(192, 0, 0));
                    worksheet.Cells[1, 1, 1, 9].Style.Font.Color.SetColor(System.Drawing.Color.White);

                    worksheet.Cells[2, 1].Value = "Material_No";
                    worksheet.Cells[2, 2].Value = "PC";
                    worksheet.Cells[2, 3].Value = "Flute";
                    worksheet.Cells[2, 4].Value = "NewBoard";
                    worksheet.Cells[2, 5].Value = "Price";
                    worksheet.Cells[2, 6].Value = "HVA";
                    worksheet.Cells[2, 7].Value = "BOARD ALTERNATIVE";
                    worksheet.Cells[2, 8].Value = "Change";
                    worksheet.Cells[2, 9].Value = "Error Message";

                    for (int i = 1; i <= 9; i++)
                    {
                        worksheet.Cells[2, i].Style.Font.Bold = true;
                        worksheet.Cells[2, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[2, i].AutoFitColumns();
                        worksheet.Cells[2, i].Style.Fill.BackgroundColor.SetColor(yellowColFromRGB);
                        worksheet.Cells[2, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        if (i <= 5)
                        {
                            worksheet.Cells[2, i].Style.Fill.BackgroundColor.SetColor(greenColFromRGB);
                        }

                        if (i == 9)
                        {
                            worksheet.Cells[2, i].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                        }

                    }

                    foreach (var FailedResult in FailedResults)
                    {
                        invalidRow++;
                        worksheet.Cells[invalidRow, 1].Value = FailedResult.CopyMaterialNo;
                        worksheet.Cells[invalidRow, 2].Value = FailedResult.PC;
                        worksheet.Cells[invalidRow, 3].Value = FailedResult.Flute;
                        worksheet.Cells[invalidRow, 4].Value = FailedResult.NewBoard;
                        worksheet.Cells[invalidRow, 5].Value = FailedResult.Price;
                        worksheet.Cells[invalidRow, 6].Value = FailedResult.HighValue;
                        worksheet.Cells[invalidRow, 7].Value = FailedResult.BoardAlternative;
                        worksheet.Cells[invalidRow, 8].Value = FailedResult.Change;
                        worksheet.Cells[invalidRow, 9].Value = FailedResult.ErrorMessage;

                    }

                    ////worksheet.Cells[2, 1].Value = "252B";
                    //worksheet.Cells[2, 1].Value = "Ex. Z031081555";
                    //worksheet.Cells[2, 2].Value = "999";
                    //worksheet.Cells[2, 3].Value = "999";
                    //worksheet.Cells[2, 4].Value = "4054";

                    //Save your file
                    excelPackage.Save();
                }

                stream.Position = 0;
                // above I define the name of the file using the current datetime.
            }
            catch (Exception ex)
            {

            }

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName); // this will be the actual export.
        }

        #endregion

        #region Change Factory New Material

        public IActionResult ChangeFactoryNewMaterial()
        {
            List<ChangeBoardNewMaterial> masterDatas = new List<ChangeBoardNewMaterial>();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                //
                SessionExtentions.SetSession(HttpContext.Session, "TransactionDataModel", null);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }

            return View(masterDatas);
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult ImportChangeFactoryNewMaterialFromExcel(List<IFormFile> fileUpload, bool checkImport)
        {
            var isSuccess = false;
            var message = "";
            var saleOrg = "";
            var failCount = 0;
            var alreadyCount = 0;
            var modelResult = new List<ChangeBoardNewMaterial>();
            var failedResult = new List<ChangeBoardNewMaterial>();
            var alreadyResult = new List<ChangeBoardNewMaterial>();

            try
            {
                modelResult = SessionExtentions.GetSession<List<ChangeBoardNewMaterial>>(httpContextAccessor.HttpContext.Session, "ChangeFactoryNewMaterials");
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                if (modelResult == null || checkImport)
                {
                    modelResult = new List<ChangeBoardNewMaterial>();
                }

                if (fileUpload.Count > 0 || (fileUpload.Count == 0 && !checkImport))
                {
                    if (!checkImport)
                    {
                        modelResult = modelResult.Where(m => m.IsCreatedSuccess).ToList();
                    }
                    _maintenanceProductInfoService.ReadExcelFileToChangeFactoryNewMaterial(ref modelResult, fileUpload, checkImport);
                    failedResult = modelResult.Where(m => !m.IsCreatedSuccess).ToList();
                    failCount = failedResult != null && failedResult.Count > 0 ? failedResult.Count : 0;
                    alreadyResult = modelResult.Where(m => m.IsCreatedSuccess).ToList();
                    alreadyCount = alreadyResult != null && alreadyResult.Count > 0 ? alreadyResult.Count : 0;
                }

                SessionExtentions.SetSession(httpContextAccessor.HttpContext.Session, "ChangeFactoryNewMaterials", null);

                if (checkImport)
                {
                    SessionExtentions.SetSession(httpContextAccessor.HttpContext.Session, "ChangeFactoryNewMaterials", modelResult);
                }

                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                if (ex.Message.ToString().Contains("The import file size limitation is 100KB"))
                {
                    message = ex.Message;
                }
                else
                {
                    message = "File excel must not be open or data is incorrect. Please try again...";
                }
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return Json(new
            {
                IsSuccess = isSuccess,
                ExceptionMessage = message,
                View = RenderView.RenderRazorViewToString(this, "_ChangeFactoryNewMaterialTable", modelResult),
                ViewAlreadys = RenderView.RenderRazorViewToString(this, "_AlreadyChangeFactoryNewMaterialTable", alreadyResult),
                ViewFaileds = RenderView.RenderRazorViewToString(this, "_FailedChangeFactoryNewMaterialTable", failedResult),
                FailedCount = failCount,
                AlreadyCount = alreadyCount
            });
        }

        [SessionTimeout]
        public async Task<IActionResult> ExportChangeFactoryNewMaterialTemplate()
        {
            var stream = new MemoryStream();

            string excelName = $"PMTs-ChangeFactoryNewMaterialTemplate.xlsx";
            var userSessionModel = SessionExtentions.GetSession<UserSessionModel>(httpContextAccessor.HttpContext.Session, "UserSessionModel");

            var _token = userSessionModel.Token;

            try
            {
                //Create a new ExcelPackage
                using (ExcelPackage excelPackage = new ExcelPackage(stream))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    //Set some properties of the Excel document
                    excelPackage.Workbook.Properties.Author = "PMTs";
                    excelPackage.Workbook.Properties.Title = "ChangeFactoryNewMaterial";
                    excelPackage.Workbook.Properties.Subject = "PMTs ChangeFactoryNewMaterial";
                    excelPackage.Workbook.Properties.Created = DateTime.Now;

                    //Create the WorkSheet
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("ChangeFactoryNewMat_Template");

                    var yellowColFromRGB = System.Drawing.Color.Yellow; //FromArgb(255, 249, 82)
                    var greenColFromRGB = System.Drawing.Color.FromArgb(169, 208, 142);
                    worksheet.Cells[1, 1].Value = "File Upload Template";
                    worksheet.Cells[1, 1, 1, 4].Merge = true; //Merge columns start and end range
                    worksheet.Cells[1, 1, 1, 4].Style.Font.Bold = true; //Font should be bold
                    worksheet.Cells[1, 1, 1, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Alignment is center
                    worksheet.Cells[1, 1, 1, 4].Style.Font.Size = 16;
                    worksheet.Cells[1, 1, 1, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, 1, 1, 4].AutoFitColumns();
                    worksheet.Cells[1, 1, 1, 4].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(192, 0, 0));
                    worksheet.Cells[1, 1, 1, 4].Style.Font.Color.SetColor(System.Drawing.Color.White);

                    worksheet.Cells[2, 1].Value = "FactoryCode";
                    worksheet.Cells[2, 2].Value = "Material_No";
                    worksheet.Cells[2, 3].Value = "Material_Type";
                    worksheet.Cells[2, 4].Value = "Error Message";

                    for (int i = 1; i <= 4; i++)
                    {
                        worksheet.Cells[2, i].Style.Font.Bold = true;
                        worksheet.Cells[2, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[2, i].AutoFitColumns();
                        worksheet.Cells[2, i].Style.Fill.BackgroundColor.SetColor(yellowColFromRGB);
                        worksheet.Cells[2, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        if (i <= 3)
                        {
                            worksheet.Cells[2, i].Style.Fill.BackgroundColor.SetColor(greenColFromRGB);
                        }

                        if (i == 4)
                        {
                            worksheet.Cells[2, i].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                        }

                    }

                    worksheet.Cells["A1:G100"].Style.Numberformat.Format = "@";

                    //Save your file
                    excelPackage.Save();
                }

                stream.Position = 0;
                // above I define the name of the file using the current datetime.
            }
            catch (Exception ex)
            {

            }

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName); // this will be the actual export.
        }

        [SessionTimeout]
        public async Task<IActionResult> ExportInvalidChangeFactoryNewMaterialDataTemplate()
        {
            var stream = new MemoryStream();

            string excelName = $"PMTs-InvalidChangeFactoryNewMaterialDataTemplate.xlsx";
            var userSessionModel = SessionExtentions.GetSession<UserSessionModel>(httpContextAccessor.HttpContext.Session, "UserSessionModel");
            var modelResults = new List<ChangeBoardNewMaterial>();
            var FailedResults = new List<ChangeBoardNewMaterial>();
            var invalidRow = 2;

            modelResults = SessionExtentions.GetSession<List<ChangeBoardNewMaterial>>(httpContextAccessor.HttpContext.Session, "ChangeFactoryNewMaterials");
            if (modelResults != null && modelResults.Count > 0)
            {
                FailedResults = modelResults.Where(m => !m.IsCreatedSuccess).ToList();
            }

            try
            {
                //Create a new ExcelPackage
                using (ExcelPackage excelPackage = new ExcelPackage(stream))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    //Set some properties of the Excel document
                    excelPackage.Workbook.Properties.Author = "PMTs";
                    excelPackage.Workbook.Properties.Title = "ChangeFactoryNewMaterial";
                    excelPackage.Workbook.Properties.Subject = "PMTs ChangeFactoryNewMaterial";
                    excelPackage.Workbook.Properties.Created = DateTime.Now;

                    //Create the WorkSheet
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("ChangeFactoryNewMat_Template");

                    var yellowColFromRGB = System.Drawing.Color.Yellow; //FromArgb(255, 249, 82)
                    var greenColFromRGB = System.Drawing.Color.FromArgb(169, 208, 142);

                    worksheet.Cells[1, 1].Value = "File Upload Template";
                    worksheet.Cells[1, 1, 1, 4].Merge = true; //Merge columns start and end range
                    worksheet.Cells[1, 1, 1, 4].Style.Font.Bold = true; //Font should be bold
                    worksheet.Cells[1, 1, 1, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Alignment is center
                    worksheet.Cells[1, 1, 1, 4].Style.Font.Size = 16;
                    worksheet.Cells[1, 1, 1, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, 1, 1, 4].AutoFitColumns();
                    worksheet.Cells[1, 1, 1, 4].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(192, 0, 0));
                    worksheet.Cells[1, 1, 1, 4].Style.Font.Color.SetColor(System.Drawing.Color.White);

                    worksheet.Cells[2, 1].Value = "FactoryCode";
                    worksheet.Cells[2, 2].Value = "Material_No";
                    worksheet.Cells[2, 3].Value = "Material_Type";
                    worksheet.Cells[2, 4].Value = "Error Message";

                    for (int i = 1; i <= 4; i++)
                    {
                        worksheet.Cells[2, i].Style.Font.Bold = true;
                        worksheet.Cells[2, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[2, i].AutoFitColumns();
                        worksheet.Cells[2, i].Style.Fill.BackgroundColor.SetColor(yellowColFromRGB);
                        worksheet.Cells[2, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        if (i <= 3)
                        {
                            worksheet.Cells[2, i].Style.Fill.BackgroundColor.SetColor(greenColFromRGB);
                        }

                        if (i == 4)
                        {
                            worksheet.Cells[2, i].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                        }

                    }

                    foreach (var FailedResult in FailedResults)
                    {
                        invalidRow++;
                        worksheet.Cells[invalidRow, 1].Value = FailedResult.CopyFactoryCode;
                        worksheet.Cells[invalidRow, 2].Value = FailedResult.CopyMaterialNo;
                        worksheet.Cells[invalidRow, 3].Value = FailedResult.MaterialType;
                        worksheet.Cells[invalidRow, 9].Value = FailedResult.ErrorMessage;

                    }

                    //Save your file
                    excelPackage.Save();
                }

                stream.Position = 0;
                // above I define the name of the file using the current datetime.
            }
            catch (Exception ex)
            {

            }

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName); // this will be the actual export.
        }

        #endregion


        #region Update Material By Import Excel

        [SessionTimeout]
        public IActionResult UpdateMaterialByImportExcel()
        {
            var updateMaterialModel = new UpdateMaterialViewModel();
            return View(updateMaterialModel);
        }

        [SessionTimeout]
        public async Task<IActionResult> ExportUpdateMaterialTemplate(string optionSelected)
        {
            var stream = new MemoryStream();
            var routingOptionArr = new string[] { "Machine", "RemarkInProcess", "PaperWidth", "CutNo", "Trim", "PercentTrim", "Alternative1", "Alternative2", "Alternative3", "Alternative4", "Alternative5" };
            var optionArr = string.IsNullOrEmpty(optionSelected) ? null : optionSelected.Split(",").Distinct().ToArray();
            var isRouting = optionArr != null && optionArr.Intersect(routingOptionArr).Count() > 0;
            var numOfColumn = !isRouting ? 3 : 4;

            string excelName = $"PMTs_UpdateMaterialTemplate.xlsx";
            try
            {
                //Create a new ExcelPackage
                using (ExcelPackage excelPackage = new ExcelPackage(stream))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    //Set some properties of the Excel document
                    excelPackage.Workbook.Properties.Author = "PMTs";
                    excelPackage.Workbook.Properties.Title = "UpdateMaterialTemplate";
                    excelPackage.Workbook.Properties.Subject = "PMTs UpdateMaterialTemplate";
                    excelPackage.Workbook.Properties.Created = DateTime.Now;

                    //Create the WorkSheet
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("UpdateMaterial_Template");
                    var colFromRGB = System.Drawing.Color.FromArgb(255, 249, 82);

                    worksheet.Cells[1, 1].Value = "FactoryCode";
                    worksheet.Cells[1, 2].Value = "Material_No";

                    if (isRouting)
                    {
                        worksheet.Cells[1, 3].Value = "Seq_No";
                    }

                    var pointor = 0;
                    for (var i = numOfColumn; i < numOfColumn + optionArr.Length; i++)
                    {

                        worksheet.Cells[1, i].Value = optionArr[pointor];
                        pointor++;
                    }


                    for (int i = 1; i <= optionArr.Length + numOfColumn - 1; i++)
                    {
                        worksheet.Cells[1, i].Style.Font.Bold = true;
                        worksheet.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(colFromRGB);
                        worksheet.Cells[1, i].AutoFitColumns();
                    }

                    worksheet.Cells["A1:L1000"].Style.Numberformat.Format = "@";
                    //Save your file
                    excelPackage.Save();
                }

                stream.Position = 0;
            }
            catch (Exception ex)
            {

            }
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName); // this will be the actual export.
        }


        [SessionTimeout]
        [HttpPost]
        public JsonResult ImportUpdateMaterialFromFile(IFormFile file)
        {
            var isSuccess = true;
            var exceptionMessage = string.Empty;

            try
            {
                _masterDataService.ImportUpdateMaterialFromFile(file, ref exceptionMessage);
            }
            catch (Exception ex)
            {
                isSuccess = true;
                exceptionMessage = ex.Message;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage });
        }

        #endregion
    }
}