using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.UpdateLotsOfMaterial;
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
    public class UpdateLotsOfMaterialController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IUpdateLotsOfMaterialService _updateLotsOfMaterialService;

        public UpdateLotsOfMaterialController(IHostingEnvironment hostingEnvironment,
            IUpdateLotsOfMaterialService updateLotsOfMaterialService)
        {
            _hostingEnvironment = hostingEnvironment;
            _updateLotsOfMaterialService = updateLotsOfMaterialService;
        }
        // GET: UpdateLotsOfMaterial
        public ActionResult Index()
        {
            UpdateLotsOfMaterialViewModel updateLotsOfMaterialViewModel = new UpdateLotsOfMaterialViewModel();
            updateLotsOfMaterialViewModel.MasterDatas = new List<MasterDataViewModel>();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _updateLotsOfMaterialService.GetCompanyProfileSelectList(ref updateLotsOfMaterialViewModel);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }
            return View(updateLotsOfMaterialViewModel);
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult ExcelUpdateMat(string fileDownload, List<IFormFile> fileUpload, string importSelect)
        {
            var isSuccess = false;
            var message = "";
            var typeAction = "";
            UpdateLotsOfMaterialViewModel updateLotsOfMaterialViewModel = new UpdateLotsOfMaterialViewModel();
            updateLotsOfMaterialViewModel.MasterDatas = new List<MasterDataViewModel>();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                if (!string.IsNullOrEmpty(fileDownload))
                {
                    typeAction = "Export Data";
                    //OnPostExportExcelAsync(importSelect);
                }
                else if (fileUpload.Count > 0)
                {
                    typeAction = "Import Data";
                    _updateLotsOfMaterialService.ReadExcelFileToUpdateMasterData(ref message, importSelect, fileUpload, ref updateLotsOfMaterialViewModel);
                }

                if (message == "")
                {
                    isSuccess = true;
                }
                else
                {
                    isSuccess = false;
                }

                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                if (ex.Message.ToString().Contains("Factory Code \"" + importSelect + "\" is null."))
                {
                    message = ex.Message;
                }
                else
                {
                    message = typeAction == "Export Data" ? "File .xlxs must not be open. Please try again..." : "Data is incorrect. Please try again...";
                }
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = message, TypeAction = typeAction, View = RenderView.RenderRazorViewToString(this, "_MasterDataTable", updateLotsOfMaterialViewModel) });
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult ExcelUpdateStatus(string fileDownload, List<IFormFile> fileUploadx, string importSelect)
        {
            var isSuccess = false;
            var message = "";
            var typeAction = "";
            UpdateLotsOfMaterialViewModel updateLotsOfMaterialViewModel = new UpdateLotsOfMaterialViewModel();
            updateLotsOfMaterialViewModel.MasterDatas = new List<MasterDataViewModel>();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                if (!string.IsNullOrEmpty(fileDownload))
                {
                    typeAction = "Export Data";
                }
                else if (fileUploadx.Count > 0)
                {
                    typeAction = "Import Data";
                    _updateLotsOfMaterialService.ReadExcelFileToUpdateStatusX(ref message, importSelect, fileUploadx, ref updateLotsOfMaterialViewModel);
                }

                if (message == "")
                {
                    isSuccess = true;
                }
                else
                {
                    isSuccess = false;
                }

                //isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                if (ex.Message.ToString().Contains("Factory Code \"" + importSelect + "\" is null."))
                {
                    message = ex.Message;
                }
                else
                {
                    message = typeAction == "Export Data" ? "File .xlxs must not be open. Please try again..." : "Data is incorrect. Please try again...";
                }
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = message, TypeAction = typeAction, View = RenderView.RenderRazorViewToString(this, "_MasterDataTable", updateLotsOfMaterialViewModel) });
        }

        [SessionTimeout]
        public async Task<IActionResult> UpdateMatExportExcel(string excelTemplate)
        {
            var stream = new MemoryStream();

            string excelName = "";

            try
            {
                if (excelTemplate == "updateMat")
                {
                    excelName = $"PMTs_TemplateUpdatePCandDescription.xlsx";
                    //Create a new ExcelPackage
                    using (ExcelPackage excelPackage = new ExcelPackage(stream))
                    {
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        //Set some properties of the Excel document
                        excelPackage.Workbook.Properties.Author = "PMTs";
                        excelPackage.Workbook.Properties.Title = "TemplateUpdatePCandDescription";
                        excelPackage.Workbook.Properties.Subject = "PMTs TemplateUpdatePCandDescription";
                        excelPackage.Workbook.Properties.Created = DateTime.Now;

                        //Create the WorkSheet
                        ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet1");

                        worksheet.Cells[1, 1].Value = "MaterialNo";
                        worksheet.Cells[1, 2].Value = "PC";
                        worksheet.Cells[1, 3].Value = "Description";

                        //Save your file
                        excelPackage.Save();
                    }
                }
                else if (excelTemplate == "deleteMat")
                {
                    excelName = $"PMTs_TemplateDeleteMaterial.xlsx";
                    //Create a new ExcelPackage
                    using (ExcelPackage excelPackage = new ExcelPackage(stream))
                    {
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        //Set some properties of the Excel document
                        excelPackage.Workbook.Properties.Author = "PMTs";
                        excelPackage.Workbook.Properties.Title = "TemplateDeleteMaterial";
                        excelPackage.Workbook.Properties.Subject = "PMTs TemplateDeleteMaterial";
                        excelPackage.Workbook.Properties.Created = DateTime.Now;

                        //Create the WorkSheet
                        ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet1");

                        worksheet.Cells[1, 1].Value = "MaterialNo";

                        //Save your file
                        excelPackage.Save();
                    }
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