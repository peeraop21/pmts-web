using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.MaintenanceBoardCombineAcc;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Tracing;
using PMTs.WebApplication.Controllers;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services
{
    [TraceAspect]
    public class MaintenanceBoardCombineAccService : IMaintenanceBoardCombineAccService
    {
        private readonly IBoardCombineAccAPIRepository _boardCombineAccAPIRepository;
        private readonly IPlantCostFieldAPIRepository _plantCostFieldAPIRepository;
        private readonly ICompanyProfileAPIRepository _companyProfileAPIRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHostingEnvironment _hostingEnvironment;

        private readonly string _username;
        private readonly string _saleOrg;
        private readonly string _factoryCode;
        private string _token;


        public MaintenanceBoardCombineAccService(IBoardCombineAccAPIRepository boardCombineAccAPIRepository,
            IHttpContextAccessor httpContextAccessor,
            IPlantCostFieldAPIRepository plantCostFieldAPIRepository,
            IHostingEnvironment hostingEnvironment,
            ICompanyProfileAPIRepository companyProfileAPIRepository)
        {
            _boardCombineAccAPIRepository = boardCombineAccAPIRepository;
            _httpContextAccessor = httpContextAccessor;
            _plantCostFieldAPIRepository = plantCostFieldAPIRepository;
            _hostingEnvironment = hostingEnvironment;
            _companyProfileAPIRepository = companyProfileAPIRepository;

            // Initialize User Data From Session
            var userSessionModel = SessionExtentions.GetSession<UserSessionModel>(_httpContextAccessor.HttpContext.Session, "UserSessionModel");
            if (userSessionModel != null)
            {
                _username = userSessionModel.UserName;
                _saleOrg = userSessionModel.SaleOrg;
                _factoryCode = userSessionModel.FactoryCode;
                _token = userSessionModel.Token;

            }
        }

        public void GetBoardCombineAccDataToExcelFile(string planCodeSelect)
        {
            throw new NotImplementedException();
        }

        public void GetCompanyProfileSelectList(ref MaintenanceBoardCombineAccViewModel maintenanceBoardCombineAccViewModel)
        {
            var companyProfiles = new List<CompanyProfile>();
            companyProfiles = JsonConvert.DeserializeObject<List<CompanyProfile>>(_companyProfileAPIRepository.GetCompanyProfileList(_factoryCode, _token));
            maintenanceBoardCombineAccViewModel.CompanyProfiles = new List<SelectListItem>();
            foreach (var companyProfile in companyProfiles)
            {
                maintenanceBoardCombineAccViewModel.CompanyProfiles.Add(new SelectListItem { Text = companyProfile.Plant, Value = companyProfile.SaleOrg + " " + companyProfile.ShortName });
            }
        }

        //public void GetBoardCombineAccDataToExcelFile(string plantCodeSelect)
        //{
        //    var pmtsExcelPath = @"C:\Users\" + Environment.UserName + @"\Downloads\PMTsExcel\" + plantCodeSelect + "_BoardCombineAcc.xlsx";

        //    var costFields = JsonConvert.DeserializeObject<List<PlantCostField>>(_plantCostFieldAPIRepository.GetPlantCostFields(plantCodeSelect)).Where(p => p.FactoryCode == plantCodeSelect).Select(p => p.CostField).Distinct().ToArray();

        //    if (costFields.Length == 0)
        //    {
        //        throw new Exception("Plant cost field's data of sale org \"" + plantCodeSelect + "\" is null.");
        //    }
        //    //Create a new ExcelPackage
        //    using (ExcelPackage excelPackage = new ExcelPackage())
        //    {
        //        //Set some properties of the Excel document
        //        excelPackage.Workbook.Properties.Author = "PMTs";
        //        excelPackage.Workbook.Properties.Title = plantCodeSelect + "_BoardCombineAcc";
        //        excelPackage.Workbook.Properties.Subject = "PMTs BoardCombineAcc";
        //        excelPackage.Workbook.Properties.Created = DateTime.Now;

        //        //Create the WorkSheet
        //        ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("BoardCombineAcc");

        //        worksheet.Cells[1, 1].Value = "Code";

        //        for (var i = 0; i < costFields.Length; i++)
        //        {
        //            worksheet.Cells[1, (2 + i)].Value = costFields[i];
        //        }

        //        //create folder if doesn't exist
        //        var directory = @"C:\Users\" + Environment.UserName + @"\Downloads\PMTsExcel";
        //        if (!Directory.Exists(@"C:\Users\" + Environment.UserName + @"\Downloads\PMTsExcel"))
        //        {
        //            Directory.CreateDirectory(directory);
        //        }

        //        //Save your file
        //        excelPackage.SaveAs(new FileInfo(pmtsExcelPath));
        //    }

        //    ////Opening an existing Excel file
        //    //FileInfo fi = new FileInfo(pmtsExcelPath);
        //    //using (ExcelPackage excelPackage = new ExcelPackage(fi))
        //    //{
        //    //    //Get a WorkSheet by index. Note that EPPlus indexes are base 1, not base 0!
        //    //    ExcelWorksheet firstWorksheet = excelPackage.Workbook.Worksheets["BoardCombineAcc"];

        //    ////Get a WorkSheet by name. If the worksheet doesn't exist, throw an exeption
        //    //ExcelWorksheet namedWorksheet = excelPackage.Workbook.Worksheets["Sheet 1"];

        //    ////If you don't know if a worksheet exists, you could use LINQ,
        //    ////So it doesn't throw an exception, but return null in case it doesn't find it
        //    //ExcelWorksheet anotherWorksheet = excelPackage.Workbook.Worksheets.FirstOrDefault(x => x.Name == "SomeWorksheet");

        //    ////Get the content from cells A1 and B1 as string, in two different notations
        //    //string valA1 = firstWorksheet.Cells["A1"].Value.ToString();
        //    //string valB1 = firstWorksheet.Cells[1, 2].Value.ToString();
        //    //    firstWorksheet.Cells[1, 3].Value = "123123";

        //    //    //Save your file
        //    //    excelPackage.Save();

        //    //}
        //}

        public void ReadExcelFileToBoardCombineAcc(ref string message, ref string planCodeSelect, List<IFormFile> fileUpload)
        {
            var isInvalidCode = false;
            try
            {
                if (fileUpload.Count > 0)
                {
                    var fileExcel = fileUpload[0];

                    long size = fileExcel.Length;
                    if (size > 150000)
                    {
                        throw new Exception("The import file size limitation is 150KB.");
                    }

                    // full path to file in temp location
                    var filePath = Path.GetTempFileName();

                    using (var ms = new MemoryStream())
                    {
                        DataTable table = new DataTable();

                        fileExcel.CopyToAsync(ms);
                        var fileBytes = ms.ToArray();
                        string s = Convert.ToBase64String(fileBytes);

                        using (ExcelPackage excelPackage = new ExcelPackage(ms))
                        {
                            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["BoardCombineAcc"];

                            //loop all rows
                            for (int i = worksheet.Dimension.Start.Row; i <= worksheet.Dimension.End.Row; i++)
                            {
                                var rowsObj = new object[worksheet.Dimension.End.Column];
                                isInvalidCode = false;
                                //loop all columns in a row
                                for (int j = worksheet.Dimension.Start.Column; j <= worksheet.Dimension.End.Column && !isInvalidCode; j++)
                                {
                                    if (i == 1)
                                    {
                                        if (worksheet.Cells[i, j].Value == null || worksheet.Cells[i, j].Value == "" || worksheet.Cells[i, 1].Value.ToString() != "Code")
                                        {
                                            throw new Exception("invalid column name (Code).");
                                        }
                                        else if (worksheet.Cells[i, j].Value != null && worksheet.Cells[i, j].Value != "")
                                        {
                                            if (worksheet.Cells[i, j].Value.ToString() == "Code")
                                            {
                                                table.Columns.Add(worksheet.Cells[i, j].Value.ToString(), typeof(string));
                                            }
                                            else
                                            {
                                                table.Columns.Add(worksheet.Cells[i, j].Value.ToString(), typeof(double));
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (j == 1)
                                        {
                                            if (worksheet.Cells[i, j].Value == null || worksheet.Cells[i, j].Value == "")
                                            {
                                                isInvalidCode = true;
                                            }
                                            else
                                            {
                                                rowsObj[j - 1] = worksheet.Cells[i, j].Value;
                                            }
                                        }
                                        else
                                        {
                                            if (worksheet.Cells[1, j].Value != null && worksheet.Cells[1, j].Value != "")
                                            {
                                                double? value = null;
                                                if (worksheet.Cells[i, j].Value != null)
                                                {
                                                    value = Convert.ToDouble(worksheet.Cells[i, j].Value);
                                                }

                                                rowsObj[j - 1] = value;
                                            }
                                        }
                                    }

                                    if (j == worksheet.Dimension.End.Column && i != 1)
                                    {
                                        table.Rows.Add(rowsObj);
                                    }
                                }
                            }
                        }

                        var jsonDataTable = JsonConvert.SerializeObject(table);

                        //send json data of board combine acc to update database with api.

                        message = _boardCombineAccAPIRepository.ImportBoardCombineAcc(_factoryCode, _username, jsonDataTable, _token);
                        planCodeSelect = _saleOrg;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
