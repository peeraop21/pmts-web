using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PMTs.DataAccess.ComplexModel;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView.Login;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static PMTs.DataAccess.Extentions.JsonExtentions;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace PMTs.WebApplication.Controllers
{
    public class ReCalculateTrimController : Controller
    {
        private readonly IReCalculateTrimService reCalculateTrimService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private int processLimit;

        public ReCalculateTrimController(IReCalculateTrimService reCalculateTrimService, IHttpContextAccessor httpContextAccessor)
        {
            this.reCalculateTrimService = reCalculateTrimService;
            this.httpContextAccessor = httpContextAccessor;
            processLimit = 150;
        }

        [SessionTimeout]
        public IActionResult Index(string status)
        {
            var reCalculateTrimViewModel = new ReCalculateTrimViewModel();
            reCalculateTrimViewModel = reCalculateTrimService.GetFlutesAndMachinesByFactoryCode();
            reCalculateTrimViewModel.Status = status;
            return View(reCalculateTrimViewModel);
        }


        [SessionTimeout]
        [HttpPost]
        public IActionResult ReCalculateTrimChangeRouting(string flute, string action, string numberOfProgress)
        {
            bool isSuccess = false;
            bool hasDataTableRows = false;
            bool hasDataForSave = false;
            bool endProcess = false;
            string exceptionMessage = string.Empty;
            var reCalculateTrimSession = new ChangeReCalculateTrimModel();
            var reCalculateTrims = new List<ReCalculateTrimModel>();
            var routings = new List<Routing>();
            var allProgress = 0;
            var listsInProcess = 0;

            try
            {
                reCalculateTrimSession = SessionExtentions.GetSession<ChangeReCalculateTrimModel>(httpContextAccessor.HttpContext.Session, "ReCalculateTrimData");
                allProgress = (int)Math.Ceiling((decimal)reCalculateTrimSession.ReCalculateTrimModels.Count / (decimal)processLimit);

                reCalculateTrimService.ReCalculateTrim(flute, Convert.ToInt32(numberOfProgress), processLimit, ref reCalculateTrimSession, ref reCalculateTrims, ref routings);
                var condition = Convert.ToInt32(numberOfProgress) == allProgress && reCalculateTrimSession != null && reCalculateTrimSession.DataTable != null;

                listsInProcess = reCalculateTrims.Count;
                if (action == "IsPreview")
                {
                    if (condition)
                    {
                        hasDataTableRows = reCalculateTrimSession.DataTable.Rows.Count > 0 ? true : false;
                        endProcess = true;
                        SessionExtentions.SetSession(httpContextAccessor.HttpContext.Session, "ReCalculateTrimData", reCalculateTrimSession);
                    }
                }
                else
                {
                    //save routings
                    var changeReCalculateTrimSave = new ChangeReCalculateTrimModel();
                    changeReCalculateTrimSave.ReCalculateTrimModels = new List<ReCalculateTrimModel>();
                    changeReCalculateTrimSave.ReCalculateTrimModels.AddRange(reCalculateTrims);
                    changeReCalculateTrimSave.Routings = new List<Routing>();
                    changeReCalculateTrimSave.Routings.AddRange(routings);
                    reCalculateTrimService.SaveReCalculateTrim(changeReCalculateTrimSave);

                    if (condition)
                    {
                        hasDataForSave = reCalculateTrimSession.ReCalculateTrimModels.Count > 0 && reCalculateTrimSession.Routings.Count > 0 ? true : false;
                        endProcess = true;
                    }
                }

                isSuccess = true;
            }
            catch (Exception ex)
            {
                exceptionMessage = ex.Message;
                isSuccess = false;
            }

            return Json(new
            {
                IsSuccess = isSuccess,
                ExceptionMessage = exceptionMessage,
                ListsInProcess = listsInProcess,
                HasDataTableRows = hasDataTableRows,
                HasDataForSave = hasDataForSave,
                EndProcess = endProcess,
                Ref = HttpContext.Session.Id
        });
        }

        [SessionTimeout]
        [HttpPost]
        public IActionResult ReCalculateTrimProgress(string flute, string machine, string boxType, string printMethod, string proType)
        {
            bool isSuccess = false;
            string exceptionMessage = string.Empty;
            var changeReCalculateTrimModel = new ChangeReCalculateTrimModel();
            var allProgress = 0;

            try
            {
                changeReCalculateTrimModel = reCalculateTrimService.GetReCalculateTrim(flute, machine, boxType, printMethod, proType);
                //allProgress = (int)Math.Ceiling((decimal)changeReCalculateTrimModel.ReCalculateTrimModels.Count / (decimal)processLimit);

                isSuccess = true;
            }
            catch (Exception ex)
            {
                exceptionMessage = ex.Message;
                isSuccess = false;
            }

            return Json(new
            {
                IsSuccess = isSuccess,
                ExceptionMessage = exceptionMessage,
                AllProgress = changeReCalculateTrimModel.ReCalculateTrimModels.Count
            });
        }

        [SessionTimeout]
        [HttpPost]
        public IActionResult ClearSessionReCalculateTrim()
        {
            bool isSuccess = false;
            string exceptionMessage = string.Empty;

            try
            {
                var changeReCalculateTrimModel = new ChangeReCalculateTrimModel();
                changeReCalculateTrimModel.ReCalculateTrimModels = new List<ReCalculateTrimModel>(); ;
                changeReCalculateTrimModel.Routings = new List<Routing>();
                SessionExtentions.SetSession(httpContextAccessor.HttpContext.Session, "ReCalculateTrimData", changeReCalculateTrimModel);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                exceptionMessage = ex.Message;
                isSuccess = false;
            }

            return Json(new
            {
                IsSuccess = isSuccess,
                ExceptionMessage = exceptionMessage
            });
        }

        [SessionTimeout]
        public async Task<IActionResult> ReCalculateTrimExportExcel(string flute)
        {
            var stream = new MemoryStream();
            var excelName = $"PreviewRecalculateTrimFlute({flute.ToUpper()})_({DateTime.Now.ToString("dd / MM / yyyy HH.mm.ss")}).xlsx";
            var excelNameErr = $"Error.xlsx";
            var ChangeReCalculateTrimModel = SessionExtentions.GetSession<ChangeReCalculateTrimModel>(httpContextAccessor.HttpContext.Session, "ReCalculateTrimData");

            var dataTable = ChangeReCalculateTrimModel.DataTable;
            if (ChangeReCalculateTrimModel != null && dataTable != null)
            {
                try
                {
                    using (ExcelPackage excelPackage = new ExcelPackage(stream))
                    {
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        ExcelWorksheet workSheet;
                        workSheet = excelPackage.Workbook.Worksheets.Add("PreviewReCalculateTrim_1");
                        if (System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.IsRightToLeft) // Right to Left for Arabic lang
                        {
                            ExcelWorksheetView wv = workSheet.View;
                            wv.RightToLeft = true;
                            workSheet.PrinterSettings.Orientation = eOrientation.Landscape;
                        }
                        else
                        {
                            ExcelWorksheetView wv = workSheet.View;
                            wv.RightToLeft = false;
                            workSheet.PrinterSettings.Orientation = eOrientation.Landscape;
                        }

                        workSheet.Cells["A1:L1"].AutoFitColumns();
                        workSheet.Cells.LoadFromDataTable(dataTable, true, OfficeOpenXml.Table.TableStyles.Light21);
                        var headerCell = workSheet.Cells["A1:L1"];
                        headerCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        headerCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.BurlyWood);
                        headerCell.AutoFilter = false;
                        for (int i = 1; i <= 12; i++)
                        {
                            if (i > 8)
                            {
                                workSheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(135, 206, 235));
                            }
                            else
                            {
                                workSheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(218, 165, 32));
                            }
                            workSheet.Cells[1, i].Style.Font.Bold = true;
                            workSheet.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            workSheet.Cells[1, i].AutoFitColumns();
                        }
                        var headerFont = headerCell.Style.Font;
                        headerFont.Bold = true;
                        excelPackage.Save();
                    }
                    stream.Position = 0;

                }
                catch (Exception ex)
                {

                }
                var changeReCalculateTrimModel = new ChangeReCalculateTrimModel();
                changeReCalculateTrimModel.ReCalculateTrimModels = new List<ReCalculateTrimModel>(); ;
                changeReCalculateTrimModel.Routings = new List<Routing>();
                SessionExtentions.SetSession(httpContextAccessor.HttpContext.Session, "ReCalculateTrimData", changeReCalculateTrimModel);
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName); // this will be the actual export.
            }
            else
            {

                return RedirectToAction("Index", "ReCalculateTrim", new { status = "error" });
                //return File(new MemoryStream(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelNameErr);
            }
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult ImportReCalculateTrimFromFile(IFormFile file)
        {
            var isSuccess = true;
            var exceptionMessage = string.Empty;


            try
            {
                reCalculateTrimService.ImportReCalculateTrimFromFile(file, ref exceptionMessage);
            }
            catch (Exception ex)
            {
                isSuccess = true;
                exceptionMessage = ex.Message;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage });
        }

        [SessionTimeout]
        public async Task<IActionResult> ExportReCalculateTrimTemplate()
        {
            var stream = new MemoryStream();

            string excelName = $"PMTs_ReCalculateTrimTemplate.xlsx";
            try
            {
                //Create a new ExcelPackage
                using (ExcelPackage excelPackage = new ExcelPackage(stream))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    //Set some properties of the Excel document
                    excelPackage.Workbook.Properties.Author = "PMTs";
                    excelPackage.Workbook.Properties.Title = "ReCalculateTrimTemplate";
                    excelPackage.Workbook.Properties.Subject = "PMTs ReCalculateTrimTemplate";
                    excelPackage.Workbook.Properties.Created = DateTime.Now;

                    //Create the WorkSheet
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("ReCalculateTrim_Template");
                    var colFromRGB = System.Drawing.Color.FromArgb(255, 249, 82);

                    worksheet.Cells[1, 1].Value = "MaterialNo";
                    worksheet.Cells[1, 2].Value = "Machine";
                    worksheet.Cells[1, 3].Value = "SheetInWid";
                    worksheet.Cells[1, 4].Value = "Flute";
                    worksheet.Cells[1, 5].Value = "PaperWidthOld";
                    worksheet.Cells[1, 6].Value = "CutNoOld";
                    worksheet.Cells[1, 7].Value = "TrimOld";
                    worksheet.Cells[1, 8].Value = "PercenTrimOld";
                    worksheet.Cells[1, 9].Value = "PaperWidth";
                    worksheet.Cells[1, 10].Value = "CutNo";
                    worksheet.Cells[1, 11].Value = "Trim";
                    worksheet.Cells[1, 12].Value = "PercenTrim";
                    worksheet.Cells[1, 13].Value = "Error Message";

                    for (int i = 1; i <= 13; i++)
                    {
                        worksheet.Cells[1, i].Style.Font.Bold = true;
                        worksheet.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        if (i > 8)
                        {
                            worksheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(135, 206, 235));
                        }
                        else
                        {
                            worksheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(218, 165, 32));
                        }

                        worksheet.Cells[1, i].AutoFitColumns();
                    }

                    worksheet.Cells[1, 13].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                    worksheet.Cells[1, 13].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);
                    worksheet.Cells["A1:M1000"].Style.Numberformat.Format = "@";
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
        public async Task<IActionResult> ExportInvalidReCalculateTrim()
        {
            var stream = new MemoryStream();
            string excelName = $"PMTs-InvalidReCalculateTrimTemplate.xlsx";
            var modelResults = new List<ReCalculateTrimModel>();
            modelResults = SessionExtentions.GetSession<List<ReCalculateTrimModel>>(httpContextAccessor.HttpContext.Session, "ReCalculateTrimFailedResult");
            var invalidRow = 2;

            try
            {
                //Create a new ExcelPackage
                using (ExcelPackage excelPackage = new ExcelPackage(stream))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    //Set some properties of the Excel document
                    excelPackage.Workbook.Properties.Author = "PMTs";
                    excelPackage.Workbook.Properties.Title = "ReCalculateTrim";
                    excelPackage.Workbook.Properties.Subject = "PMTs ReCalculateTrim";
                    excelPackage.Workbook.Properties.Created = DateTime.Now;

                    //Create the WorkSheet
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("ReCalculateTrim_Template");

                    var yellowColFromRGB = System.Drawing.Color.Yellow; //FromArgb(255, 249, 82)
                    var greenColFromRGB = System.Drawing.Color.FromArgb(169, 208, 142);
                    var colFromRGB = System.Drawing.Color.FromArgb(255, 249, 82);

                    worksheet.Cells[1, 1].Value = "MaterialNo";
                    worksheet.Cells[1, 2].Value = "Machine";
                    worksheet.Cells[1, 3].Value = "SheetInWid";
                    worksheet.Cells[1, 4].Value = "Flute";
                    worksheet.Cells[1, 5].Value = "PaperWidthOld";
                    worksheet.Cells[1, 6].Value = "CutNoOld";
                    worksheet.Cells[1, 7].Value = "TrimOld";
                    worksheet.Cells[1, 8].Value = "PercenTrimOld";
                    worksheet.Cells[1, 9].Value = "PaperWidth";
                    worksheet.Cells[1, 10].Value = "CutNo";
                    worksheet.Cells[1, 11].Value = "Trim";
                    worksheet.Cells[1, 12].Value = "PercenTrim";
                    worksheet.Cells[1, 13].Value = "Error Message";

                    for (int i = 1; i <= 13; i++)
                    {
                        worksheet.Cells[1, i].Style.Font.Bold = true;
                        worksheet.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        if (i > 8)
                        {
                            worksheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(135, 206, 235));
                        }
                        else
                        {
                            worksheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(218, 165, 32));
                        }

                        worksheet.Cells[1, i].AutoFitColumns();
                    }

                    worksheet.Cells[1, 13].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                    worksheet.Cells[1, 13].Style.Fill.BackgroundColor.SetColor(yellowColFromRGB);

                    if (modelResults != null)
                    {
                        foreach (var modelResult in modelResults)
                        {
                            invalidRow++;
                            worksheet.Cells[invalidRow, 1].Value = modelResult.MaterialNo;
                            worksheet.Cells[invalidRow, 2].Value = modelResult.Machine;
                            worksheet.Cells[invalidRow, 9].Value = modelResult.PaperWidth != -999 ? modelResult.PercenTrim : null;
                            worksheet.Cells[invalidRow, 10].Value = modelResult.CutNo != -999 ? modelResult.CutNo : null;
                            worksheet.Cells[invalidRow, 11].Value = modelResult.Trim != -999 ? modelResult.Trim : null;
                            worksheet.Cells[invalidRow, 12].Value = modelResult.PercenTrim != -999 ? modelResult.PercenTrim : null;
                            worksheet.Cells[invalidRow, 13].Value = modelResult.ErrorMessase;

                        }
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

    }
}