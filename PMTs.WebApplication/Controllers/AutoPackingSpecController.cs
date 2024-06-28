using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView.AutoPackingSpec;
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
    public class AutoPackingSpecController : Controller
    {
        private readonly IAutoPackingSpecService autoPackingSpecService;

        public AutoPackingSpecController(IAutoPackingSpecService autoPackingSpecService)
        {
            this.autoPackingSpecService = autoPackingSpecService;
        }


        [SessionTimeout]
        public IActionResult Index()
        {
            var result = new AutoPackingSpecMainModel();
            result.AutoPackingSpecs = new List<AutoPackingSpecViewModel>();
            result.AutoPackingSpec = new AutoPackingSpec();
            result.AutoPackingConfigs = new List<AutoPackingConfig>();
            autoPackingSpecService.GetAutoPackingConfigs(ref result);

            return View(result);
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult SearchAutoPackingSpec(string materialNo)
        {
            bool isSuccess;
            bool existMasterData = false;
            string exceptionMessage = string.Empty;
            var result = new AutoPackingSpecMainModel();
            result.AutoPackingSpecs = new List<AutoPackingSpecViewModel>();
            result.AutoPackingSpec = new AutoPackingSpec();
            result.AutoPackingConfigs = new List<AutoPackingConfig>();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                result.AutoPackingSpecs = autoPackingSpecService.SearchAutoPackingSpec(materialNo, ref existMasterData).ToList();
                autoPackingSpecService.GetAutoPackingConfigs(ref result);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                exceptionMessage = ex.Message;
                isSuccess = false;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_AutoPackingSpecTable", result), ExistMasterData = existMasterData });
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult SaveAndEditAutoPackingSpec(AutoPackingSpec autoPackingSpec)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;
            var result = new AutoPackingSpecMainModel();
            result.AutoPackingSpecs = new List<AutoPackingSpecViewModel>();
            result.AutoPackingSpec = new AutoPackingSpec();
            result.AutoPackingConfigs = new List<AutoPackingConfig>();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                result.AutoPackingSpecs.Add(autoPackingSpecService.SaveAndUpdateAutoPackingSpec(autoPackingSpec));
                autoPackingSpecService.GetAutoPackingConfigs(ref result);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                exceptionMessage = ex.Message;
                isSuccess = false;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_AutoPackingSpecTable", result) });
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult ImportAutoPackingSpecFile(IFormFile file)
        {
            var isSuccess = true;
            var exceptionMessage = string.Empty;

            var result = new AutoPackingSpecMainModel();
            result.AutoPackingSpecs = new List<AutoPackingSpecViewModel>();
            result.AutoPackingSpec = new AutoPackingSpec();
            result.AutoPackingConfigs = new List<AutoPackingConfig>();

            try
            {
                autoPackingSpecService.ImportAutoPackingSpecFromFile(file, ref result, ref exceptionMessage);
                autoPackingSpecService.GetAutoPackingConfigs(ref result);
            }
            catch (Exception ex)
            {
                isSuccess = true;
                result = new AutoPackingSpecMainModel();
                result.AutoPackingSpecs = new List<AutoPackingSpecViewModel>();
                result.AutoPackingSpec = new AutoPackingSpec();
                exceptionMessage = ex.Message;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_AutoPackingSpecTable", result) });
        }

        [SessionTimeout]
        public async Task<IActionResult> ExportAutoPackingSpecTemplate()
        {
            var stream = new MemoryStream();

            string excelName = $"PMTs_AutoPackingSpecTemplate.xlsx";
            try
            {
                //Create a new ExcelPackage
                using (ExcelPackage excelPackage = new ExcelPackage(stream))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    //Set some properties of the Excel document
                    excelPackage.Workbook.Properties.Author = "PMTs";
                    excelPackage.Workbook.Properties.Title = "AutoPackingSpecTemplate";
                    excelPackage.Workbook.Properties.Subject = "PMTs AutoPackingSpecTemplate";
                    excelPackage.Workbook.Properties.Created = DateTime.Now;

                    //Create the WorkSheet
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("AutoPackingSpecTemplate");
                    var colFromRGB = System.Drawing.Color.FromArgb(255, 249, 82);

                    worksheet.Cells[1, 1].Value = "Material_No";
                    worksheet.Cells[1, 1].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                    worksheet.Cells[1, 2].Value = "nPalletType";
                    worksheet.Cells[1, 3].Value = "cPalletArrange";
                    worksheet.Cells[1, 4].Value = "cPalletStackPos";
                    worksheet.Cells[1, 5].Value = "nStrapCompression";
                    worksheet.Cells[1, 6].Value = "cStrapType";
                    worksheet.Cells[1, 7].Value = "nWrapType";
                    worksheet.Cells[1, 8].Value = "nTopBoardType";
                    worksheet.Cells[1, 9].Value = "nBottomBoardType";
                    worksheet.Cells[1, 10].Value = "cStrapperBottomProtection";
                    worksheet.Cells[1, 11].Value = "cStrapperTopProtection";
                    worksheet.Cells[1, 12].Value = "cornerGuard";

                    for (int i = 1; i <= 12; i++)
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
                    worksheet.Cells[2, 11].Value = "10";
                    worksheet.Cells[2, 12].Value = "11";

                    worksheet.Cells["A1:L1000"].Style.Numberformat.Format = "@";
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