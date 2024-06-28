using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PMTs.DataAccess.ComplexModel;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView.ManageMO;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Services;
using PMTs.WebApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Controllers
{
    public class ManageMOController : Controller
    {
        private readonly IHostingEnvironment environment;
        private readonly IMasterCardService masterCardService;
        private readonly IManageMOService manageMOService;
        private readonly IExtensionService extensionService;
        private readonly IHttpContextAccessor httpContextAccessor;

        public ManageMOController(IHostingEnvironment environment,
            IMasterCardService masterCardService,
            IManageMOService manageMOService,
            IExtensionService extensionService,
            IHttpContextAccessor httpContextAccessor)
        {
            this.environment = environment;
            this.masterCardService = masterCardService;
            this.manageMOService = manageMOService;
            this.extensionService = extensionService;
            this.httpContextAccessor = httpContextAccessor;
        }

        [SessionTimeout]
        public IActionResult Index()
        {
            var manageMOViewModel = new ManageMOViewModel();
            manageMOViewModel.MoDatas = new List<MoDataViewModel>();
            manageMOViewModel.AttachFileMOs = new List<AttachFileMo>();
            manageMOViewModel.SearchTypes = manageMOService.GetSearchType();

            return View(manageMOViewModel);
        }

        [SessionTimeout]
        public JsonResult SearchMOData(string saleOrder, string action)
        {
            var isSuccess = true;
            var exceptionMessage = string.Empty;


            var manageMOViewModel = new ManageMOViewModel();
            manageMOViewModel.MoDatas = new List<MoDataViewModel>();
            manageMOViewModel.AttachFileMOs = new List<AttachFileMo>();
            manageMOViewModel.IsAttachFilePage = !string.IsNullOrEmpty(action) && action == "AttachFileMO" ? true : false;

            try
            {
                manageMOService.GetMODataBySaleOrder(saleOrder, ref manageMOViewModel);
                if (manageMOViewModel.MoDatas == null)
                {
                    isSuccess = false;
                    exceptionMessage = $"S/O {saleOrder} doesn't exist.";
                }
            }
            catch (Exception ex)
            {
                manageMOViewModel.MoDatas = new List<MoDataViewModel>();
                manageMOViewModel.AttachFileMOs = new List<AttachFileMo>();
                exceptionMessage = $"S/O {saleOrder} doesn't exist.";
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_MODataTable", manageMOViewModel) });
        }

        [SessionTimeout]
        public JsonResult SearchMODatas(string saleOrderStart, string saleOrderEnd, string action)
        {
            var isSuccess = true;
            var exceptionMessage = string.Empty;


            var manageMOViewModel = new ManageMOViewModel();
            manageMOViewModel.MoDatas = new List<MoDataViewModel>();
            manageMOViewModel.AttachFileMOs = new List<AttachFileMo>();
            manageMOViewModel.IsAttachFilePage = !string.IsNullOrEmpty(action) && action == "AttachFileMO" ? true : false;

            try
            {
                if (!string.IsNullOrEmpty(saleOrderEnd))
                {
                    manageMOService.GetMODatasBySaleOrder(saleOrderStart, saleOrderEnd, ref manageMOViewModel);
                }
                else
                {
                    manageMOService.GetMODatasBySaleOrder(saleOrderStart, saleOrderEnd, ref manageMOViewModel);
                    //manageMOService.GetMODataBySaleOrder(saleOrderStart, ref manageMOViewModel);
                }

                if (manageMOViewModel.MoDatas == null)
                {
                    isSuccess = false;
                    exceptionMessage = $"Search S/O failed.";
                }
            }
            catch (Exception ex)
            {
                manageMOViewModel.MoDatas = new List<MoDataViewModel>();
                manageMOViewModel.AttachFileMOs = new List<AttachFileMo>();
                exceptionMessage = $"Search S/O failed.";
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_MODataTable", manageMOViewModel) });
        }

        [SessionTimeout]
        public JsonResult SearchMasterDataByMaterialNo(string materialNumber)
        {
            var isSuccess = true;
            var exceptionMessage = string.Empty;

            var moData = new MoDataViewModel();

            try
            {
                manageMOService.SearchAndCreateNewMODataByMaterialNo(materialNumber, ref moData);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                moData = new MoDataViewModel();
                exceptionMessage = ex.Message;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, MOData = JsonConvert.SerializeObject(moData) });
        }

        [SessionTimeout]
        [HttpGet]
        public JsonResult SearchMODatasBySearchType(string searchType, string searchText)
        {
            var isSuccess = true;
            var exceptionMessage = string.Empty;


            var manageMOViewModel = new ManageMOViewModel();
            manageMOViewModel.MoDatas = new List<MoDataViewModel>();
            manageMOViewModel.AttachFileMOs = new List<AttachFileMo>();

            try
            {
                manageMOService.GetMODatasBySearchType(searchType, searchText, ref manageMOViewModel);

                if (manageMOViewModel.MoDatas == null)
                {
                    isSuccess = false;
                    exceptionMessage = $"Search failed.";
                }
            }
            catch (Exception ex)
            {
                manageMOViewModel.MoDatas = new List<MoDataViewModel>();
                manageMOViewModel.AttachFileMOs = new List<AttachFileMo>();
                exceptionMessage = $"Search failed.";
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_MODataTable", manageMOViewModel) });
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult UpdateMOData(string req, string action)
        {
            var isSuccess = true;
            var exceptionMessage = string.Empty;

            var manageMOViewModel = new ManageMOViewModel();
            manageMOViewModel.MoDatas = new List<MoDataViewModel>();
            manageMOViewModel.AttachFileMOs = new List<AttachFileMo>();
            manageMOViewModel.IsAttachFilePage = false;
            var moData = new MoData();

            try
            {
                if (action == "edit")
                {

                    moData = JsonConvert.DeserializeObject<MoData>(req, new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss", Culture = new CultureInfo("en-US") });
                    var culture = new CultureInfo("en-US");
                    if (moData.DueDate != null)
                    {
                        moData.DueText = moData.DueDate.ToString("dd/MM/y", culture);
                    }
                    if (moData.SentKiwi.HasValue && moData.SentKiwi.Value)
                    {
                        moData.MoStatus = "M";
                    }

                    moData.SentKiwi = false;
                }
                else if (action == "delete")
                {
                    moData = JsonConvert.DeserializeObject<MoData>(req);
                    moData.SentKiwi = false;
                }

                manageMOService.UpdateMOData(ref manageMOViewModel, moData, action);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                manageMOViewModel.MoDatas = new List<MoDataViewModel>();
                manageMOViewModel.AttachFileMOs = new List<AttachFileMo>();
                exceptionMessage = ex.Message;
                //exceptionMessage = String.Empty;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_MODataTable", manageMOViewModel) });
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult CreateMOData(string req)
        {
            var isSuccess = true;
            var exceptionMessage = string.Empty;

            var manageMOViewModel = new ManageMOViewModel();
            manageMOViewModel.MoDatas = new List<MoDataViewModel>();
            manageMOViewModel.AttachFileMOs = new List<AttachFileMo>();
            manageMOViewModel.IsAttachFilePage = false;
            var moData = new MoData();

            try
            {
                moData = JsonConvert.DeserializeObject<MoData>(req, new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss", Culture = new CultureInfo("en-US") });
                manageMOService.CreateNewMOData(ref manageMOViewModel, moData);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                manageMOViewModel.MoDatas = new List<MoDataViewModel>();
                manageMOViewModel.AttachFileMOs = new List<AttachFileMo>();
                exceptionMessage = ex.Message;
                //exceptionMessage = String.Empty;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_MODataTable", manageMOViewModel) });
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult AttrachPDFFile(List<IFormFile> files, string orderItem)
        {
            var isSuccess = true;
            var exceptionMessage = string.Empty;

            var manageMOViewModel = new ManageMOViewModel();
            manageMOViewModel.MoDatas = new List<MoDataViewModel>();
            manageMOViewModel.AttachFileMOs = new List<AttachFileMo>();
            manageMOViewModel.IsAttachFilePage = true;

            try
            {
                manageMOService.SaveAttachFileMOData(orderItem, files, ref manageMOViewModel);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                manageMOViewModel.MoDatas = new List<MoDataViewModel>();
                manageMOViewModel.AttachFileMOs = new List<AttachFileMo>();
                exceptionMessage = ex.Message;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_MODataTable", manageMOViewModel) });
        }

        [SessionTimeout]
        public JsonResult DeleteFileMO(string fileName, string orderItem, string seqNo)
        {
            var isSuccess = true;
            var exceptionMessage = string.Empty;

            var manageMOViewModel = new ManageMOViewModel();
            manageMOViewModel.MoDatas = new List<MoDataViewModel>();
            manageMOViewModel.AttachFileMOs = new List<AttachFileMo>();
            manageMOViewModel.IsAttachFilePage = true;

            try
            {
                manageMOService.DeleteAttachFileMOData(orderItem, fileName, seqNo, ref manageMOViewModel);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                manageMOViewModel.MoDatas = new List<MoDataViewModel>();
                manageMOViewModel.AttachFileMOs = new List<AttachFileMo>();
                exceptionMessage = ex.Message;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_MOAttachFileTable", manageMOViewModel) });
        }

        [SessionTimeout]
        public JsonResult ShowAttachFileMO(string orderItem)
        {
            var isSuccess = true;
            var exceptionMessage = string.Empty;

            var manageMOViewModel = new ManageMOViewModel();
            manageMOViewModel.MoDatas = new List<MoDataViewModel>();
            manageMOViewModel.AttachFileMOs = new List<AttachFileMo>();
            manageMOViewModel.IsAttachFilePage = true;

            try
            {
                manageMOService.GetAttachFileMO(orderItem, ref manageMOViewModel);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                manageMOViewModel.MoDatas = new List<MoDataViewModel>();
                manageMOViewModel.AttachFileMOs = new List<AttachFileMo>();
                exceptionMessage = ex.Message;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_MOAttachFileTable", manageMOViewModel) });
        }

        //Tassanai Update 22072020

        [SessionTimeout]
        public JsonResult GetMOSpecByOrder(string orderItem)
        {
            var isSuccess = true;
            var exceptionMessage = string.Empty;

            var moSpec = new MoSpec();


            try
            {
                manageMOService.GetMOSpec(orderItem, ref moSpec);
            }
            catch (Exception ex)
            {
                isSuccess = false;

                exceptionMessage = ex.Message;
            }

            //return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_MOAttachFileTable", moSpec) });

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, MOData = JsonConvert.SerializeObject(moSpec) });
        }
        public JsonResult UpdateMOSpecByOrder(string orderItem, string changeInfo)
        {
            var isSuccess = true;
            var exceptionMessage = string.Empty;

            var moSpec = new MoSpec();


            try
            {
                manageMOService.UpdateMOSpec(orderItem, changeInfo);
            }
            catch (Exception ex)
            {
                isSuccess = false;

                exceptionMessage = ex.Message;
            }

            //return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_MOAttachFileTable", moSpec) });

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage });
        }





        [SessionTimeout]
        [HttpPost]
        public JsonResult ImportFileMOManual(IFormFile file)
        {
            var isSuccess = true;
            var exceptionMessage = string.Empty;

            var manageMOViewModel = new ManageMOViewModel();
            manageMOViewModel.MoDatas = new List<MoDataViewModel>();
            manageMOViewModel.AttachFileMOs = new List<AttachFileMo>();
            manageMOViewModel.IsAttachFilePage = false;

            try
            {
                manageMOService.ImportManualMOFromExcelFile(file, ref manageMOViewModel, ref exceptionMessage);
            }
            catch (Exception ex)
            {
                isSuccess = true;
                //manageMOViewModel.MoDatas = new List<MoDataViewModel>();
                manageMOViewModel.AttachFileMOs = new List<AttachFileMo>();
                exceptionMessage = ex.Message;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_MODataTable", manageMOViewModel) });
        }

        [SessionTimeout]
        public JsonResult CalculateMoTargetQuantity(string materialNO, string orderQuant, string toleranceOver)
        {
            var isSuccess = true;
            var exceptionMessage = string.Empty;
            int targetQuantity = 0;

            try
            {
                targetQuantity = manageMOService.CalculateMoTargetQuantity(materialNO, orderQuant, toleranceOver);
            }
            catch (Exception ex)
            {
                exceptionMessage = $"Can't calculate.";
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, TargetQuantity = targetQuantity });
        }

        [SessionTimeout]
        public async Task<IActionResult> ExportMOManualTemplate()
        {
            var stream = new MemoryStream();

            string excelName = $"PMTs_MOManualTemplate.xlsx";
            try
            {
                //Create a new ExcelPackage
                using (ExcelPackage excelPackage = new ExcelPackage(stream))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    //Set some properties of the Excel document
                    excelPackage.Workbook.Properties.Author = "PMTs";
                    excelPackage.Workbook.Properties.Title = "MOManualTemplate";
                    excelPackage.Workbook.Properties.Subject = "PMTs MOManualTemplate";
                    excelPackage.Workbook.Properties.Created = DateTime.Now;

                    //Create the WorkSheet
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("MOManualTemplate");
                    var colFromRGB = System.Drawing.Color.FromArgb(255, 249, 82);

                    worksheet.Cells[1, 1].Value = "Material_No";
                    worksheet.Cells[1, 1].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                    worksheet.Cells[1, 2].Value = "Order_Quant";
                    worksheet.Cells[1, 2].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                    worksheet.Cells[1, 7].Value = "PO_No";
                    worksheet.Cells[1, 7].Style.Font.Color.SetColor(System.Drawing.Color.Red);

                    worksheet.Cells[1, 3].Value = "Tolerance_Over";
                    worksheet.Cells[1, 4].Value = "Tolerance_Under";
                    worksheet.Cells[1, 5].Value = "Due_Date";
                    worksheet.Cells[1, 6].Value = "Item_Note";
                    worksheet.Cells[1, 8].Value = "Batch";
                    worksheet.Cells[1, 9].Value = "Sold_to";
                    worksheet.Cells[1, 10].Value = "Ship_to";
                    worksheet.Cells[1, 11].Value = "MORNo";

                    for (int i = 1; i <= 11; i++)
                    {
                        worksheet.Cells[1, i].Style.Font.Bold = true;
                        worksheet.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(colFromRGB);
                        worksheet.Cells[1, i].AutoFitColumns();
                    }


                    worksheet.Cells[2, 1].Value = "Ex.MatNO01";
                    worksheet.Cells[2, 2].Value = "9999";
                    worksheet.Cells[2, 3].Value = "0";
                    worksheet.Cells[2, 4].Value = "0";
                    worksheet.Cells[2, 5].Value = "23/02/2020";
                    worksheet.Cells[2, 6].Value = "ทับรอยด้านเดียว";
                    worksheet.Cells[2, 7].Value = "AF-0213M";
                    worksheet.Cells[2, 8].Value = "1991";
                    worksheet.Cells[2, 9].Value = "1005110";
                    worksheet.Cells[2, 10].Value = "1005110";
                    worksheet.Cells[2, 11].Value = "12345";

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

        #region Attach File M/O
        [SessionTimeout]
        public IActionResult AttachFileMO()
        {
            var manageMOViewModel = new ManageMOViewModel();
            manageMOViewModel.MoDatas = new List<MoDataViewModel>();
            manageMOViewModel.AttachFileMOs = new List<AttachFileMo>();
            manageMOViewModel.IsAttachFilePage = true;

            return View(manageMOViewModel);
        }
        #endregion

        #region[Edit Block Platen]
        public IActionResult EditBlockPlaten()
        {
            return View();
        }

        public JsonResult GetBlockPlatenMasterData(string material, string pc)
        {
            EditBlockPlatenModel model = new EditBlockPlatenModel();
            model = manageMOService.GetBlockPlatenMaster(material, pc);
            return Json(new { view = RenderView.RenderRazorViewToString(this, "_EditBlockPlatenTable", model) });
        }

        public JsonResult GetBlockPlatenRouting(string material, string pc)
        {
            EditBlockPlatenModel model = new EditBlockPlatenModel();
            model = manageMOService.GetBlockPlatenRouting(material);
            return Json(new { view = RenderView.RenderRazorViewToString(this, "_EditBlockPlatenRoutingTable", model), data = model.editBlockPlatenRouting });
        }

        public JsonResult EditBlockPlatenRouting(string datas)
        {
            var model = JsonConvert.DeserializeObject<List<EditBlockPlatenRouting>>(datas);
            manageMOService.UpdateBlockPlatenRouting(model);
            return Json("Success");
        }

        #endregion
    }
}