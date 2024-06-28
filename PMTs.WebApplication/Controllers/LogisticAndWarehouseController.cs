using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView.LogisticAndWarehouse;
using PMTs.Logs.Logger;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Controllers
{
    public class LogisticAndWarehouseController : Controller
    {
        private readonly ILogisticAndWarehouseService logisticAndWarehourseService;

        public LogisticAndWarehouseController(ILogisticAndWarehouseService logisticAndWarehourseService)
        {
            this.logisticAndWarehourseService = logisticAndWarehourseService;
        }

        [SessionTimeout]
        public IActionResult Index()
        {
            var result = new TruckOptimizeMainModel();
            result.TruckOptimizeViewModels = new List<TruckOptimizeViewModel>();
            result.TruckOptimize = new TruckOptimize();
            return View(result);
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult SearchTruckOptimize(string materialNo)
        {
            bool isSuccess;
            bool existMasterData = false;
            string exceptionMessage = string.Empty;
            var result = new TruckOptimizeMainModel();
            result.TruckOptimizeViewModels = new List<TruckOptimizeViewModel>();
            result.TruckOptimize = new TruckOptimize();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                result.TruckOptimizeViewModels = logisticAndWarehourseService.SearchTruckOptimize(materialNo, ref existMasterData).ToList();
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                exceptionMessage = ex.Message;
                isSuccess = false;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_TruckOptimizeTable", result), ExistMasterData = existMasterData });
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult SaveAndEditTruckOptimize(TruckOptimize truckOptimize)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;
            var result = new TruckOptimizeMainModel();
            result.TruckOptimizeViewModels = new List<TruckOptimizeViewModel>();
            result.TruckOptimize = new TruckOptimize();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                result.TruckOptimizeViewModels.Add(logisticAndWarehourseService.SaveAndUpdateTruckOptimize(truckOptimize));
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                exceptionMessage = ex.Message;
                isSuccess = false;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_TruckOptimizeTable", result) });
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult ImportTruckOptimizeFile(IFormFile file)
        {
            var isSuccess = true;
            var exceptionMessage = string.Empty;

            var result = new TruckOptimizeMainModel();
            result.TruckOptimizeViewModels = new List<TruckOptimizeViewModel>();
            result.TruckOptimize = new TruckOptimize();


            try
            {
                logisticAndWarehourseService.ImportTruckOptimizeFromFile(file, ref result, ref exceptionMessage);
            }
            catch (Exception ex)
            {
                isSuccess = true;
                //manageMOViewModel.MoDatas = new List<MoDataViewModel>();
                exceptionMessage = ex.Message;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_TruckOptimizeTable", result) });
        }


        [SessionTimeout]
        public async Task<IActionResult> TruckOptimizeExportTemplate()
        {
            var stream = new MemoryStream();

            string excelName = $"PMTs_TruckOptimizeTemplate.xlsx";
            try
            {
                //Create a new ExcelPackage
                using (ExcelPackage excelPackage = new ExcelPackage(stream))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    //Set some properties of the Excel document
                    excelPackage.Workbook.Properties.Author = "PMTs";
                    excelPackage.Workbook.Properties.Title = "TruckOptimizeTemplate";
                    excelPackage.Workbook.Properties.Subject = "PMTs TruckOptimizeTemplate";
                    excelPackage.Workbook.Properties.Created = DateTime.Now;

                    //Create the WorkSheet
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("TruckOptimizeTemplate");
                    var colFromRGB = System.Drawing.Color.FromArgb(255, 249, 82);

                    worksheet.Cells[1, 1].Value = "Material_No";
                    worksheet.Cells[1, 1].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                    worksheet.Cells[1, 2].Value = "FGPallet_W";
                    worksheet.Cells[1, 3].Value = "FGPallet_L";
                    worksheet.Cells[1, 4].Value = "FGPallet_H";
                    worksheet.Cells[1, 5].Value = "FGBundle_W";
                    worksheet.Cells[1, 6].Value = "FGBundle_L";
                    worksheet.Cells[1, 7].Value = "FGBundle_H";
                    worksheet.Cells[1, 8].Value = "PalletSize_W";
                    worksheet.Cells[1, 9].Value = "PalletSize_L";
                    worksheet.Cells[1, 10].Value = "PalletSize_H";

                    for (int i = 1; i <= 10; i++)
                    {
                        worksheet.Cells[1, i].Style.Font.Bold = true;
                        worksheet.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(colFromRGB);
                        worksheet.Cells[1, i].AutoFitColumns();
                    }


                    worksheet.Cells[2, 1].Value = "Ex.MaterialNo.01";
                    worksheet.Cells[2, 2].Value = "1";
                    worksheet.Cells[2, 3].Value = "2";
                    worksheet.Cells[2, 4].Value = "3";
                    worksheet.Cells[2, 5].Value = "4";
                    worksheet.Cells[2, 6].Value = "5";
                    worksheet.Cells[2, 7].Value = "6";
                    worksheet.Cells[2, 8].Value = "7";
                    worksheet.Cells[2, 9].Value = "8";
                    worksheet.Cells[2, 10].Value = "9";

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