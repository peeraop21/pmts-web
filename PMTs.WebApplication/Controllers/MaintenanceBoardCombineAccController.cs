using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.MaintenanceBoardCombineAcc;
using PMTs.DataAccess.ModelView.NewProduct;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.Logs.Logger;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace PMTs.WebApplication.Controllers
{
    [SessionTimeout]
    public class MaintenanceBoardCombineAccController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IMaintenanceBoardCombineAccService _maintenanceBoardCombineAccService;
        private readonly INewProductService _newProductService;
        private readonly IPlantCostFieldAPIRepository _plantCostFieldAPIRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MaintenanceBoardCombineAccController(IHostingEnvironment hostingEnvironment,
            IMaintenanceBoardCombineAccService maintenanceBoardCombineAccService,
            INewProductService newProductService,
            IPlantCostFieldAPIRepository plantCostFieldAPIRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _hostingEnvironment = hostingEnvironment;
            _maintenanceBoardCombineAccService = maintenanceBoardCombineAccService;
            _newProductService = newProductService; ;
            _plantCostFieldAPIRepository = plantCostFieldAPIRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        [SessionTimeout]
        public IActionResult Index()
        {
            MaintenanceBoardCombineAccViewModel maintenanceBoardCombineAccViewModel = new MaintenanceBoardCombineAccViewModel();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceBoardCombineAccService.GetCompanyProfileSelectList(ref maintenanceBoardCombineAccViewModel);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }

            return View(maintenanceBoardCombineAccViewModel);
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult ExcelBoardCombineAcc(string fileDownload, List<IFormFile> fileUpload)
        {
            var isSuccess = false;
            var message = "";
            var typeAction = "";
            var saleOrg = "";
            //test
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

                if (fileUpload.Count > 0)
                {
                    typeAction = "Import Data";
                    _maintenanceBoardCombineAccService.ReadExcelFileToBoardCombineAcc(ref message, ref saleOrg, fileUpload);
                }

                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                if (ex.Message.ToString().Contains("Plant cost field's data of sale org \"" + saleOrg + "\" is null."))
                {
                    message = ex.Message;
                }
                else if (ex.Message.ToString().Contains("The import file size limitation is 100KB"))
                {
                    message = ex.Message;
                }
                else
                {
                    message = typeAction == "Export Data" ? "File BoardCombineAcc.xlxs must not be open. Please try again..." : "Data is incorrect. Please try again...";
                }
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = message, TypeAction = typeAction });
        }

        [SessionTimeout]
        public async Task<IActionResult> MaintenanceBoardCombineAccExportExcel(string exportSelect)
        {
            var stream = new MemoryStream();

            string excelName = $"PMTs-{exportSelect}({DateTime.Now.ToString("dd/MM/yyyy HH.mm.ss")}).xlsx";
            var userSessionModel = SessionExtentions.GetSession<UserSessionModel>(_httpContextAccessor.HttpContext.Session, "UserSessionModel");

            var _token = userSessionModel.Token;
            try
            {
                var costFields = JsonConvert.DeserializeObject<List<PlantCostField>>(_plantCostFieldAPIRepository.GetPlantCostFields(exportSelect, _token)).Where(p => p.FactoryCode == exportSelect).Select(p => p.CostField).Distinct().ToArray();

                // above code loads the data using LINQ with EF (query of table), you can substitute this with any data source.

                if (costFields.Length == 0)
                {
                    throw new Exception("Plant cost field's data of sale org \"" + exportSelect + "\" is null.");
                }
                //Create a new ExcelPackage
                using (ExcelPackage excelPackage = new ExcelPackage(stream))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    //Set some properties of the Excel document
                    excelPackage.Workbook.Properties.Author = "PMTs";
                    excelPackage.Workbook.Properties.Title = exportSelect + "_BoardCombineAcc";
                    excelPackage.Workbook.Properties.Subject = "PMTs BoardCombineAcc";
                    excelPackage.Workbook.Properties.Created = DateTime.Now;

                    //Create the WorkSheet
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("BoardCombineAcc");

                    worksheet.Cells[1, 1].Value = "Code";

                    for (var i = 0; i < costFields.Length; i++)
                    {
                        worksheet.Cells[1, (2 + i)].Value = costFields[i];
                    }

                    ////create folder if doesn't exist
                    //var directory = @"C:\Users\" + Environment.UserName + @"\Downloads\PMTsExcel";
                    //if (!Directory.Exists(@"C:\Users\" + Environment.UserName + @"\Downloads\PMTsExcel"))
                    //{
                    //    Directory.CreateDirectory(directory);
                    //}

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
    }
}