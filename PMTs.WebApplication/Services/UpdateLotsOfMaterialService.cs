using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using OfficeOpenXml;
using PMTs.DataAccess.ComplexModel;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.UpdateLotsOfMaterial;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Tracing;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services
{
    [TraceAspect]
    public class UpdateLotsOfMaterialService : IUpdateLotsOfMaterialService
    {
        private readonly IMapper mapper;
        private readonly ICompanyProfileAPIRepository _companyProfileAPIRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IMasterDataAPIRepository _masterDataAPIRepository;

        private readonly string _username;
        private readonly string _saleOrg;
        private readonly string _factoryCode;
        private readonly string _token;

        public UpdateLotsOfMaterialService(
            IHttpContextAccessor httpContextAccessor,
            IHostingEnvironment hostingEnvironment,
            ICompanyProfileAPIRepository companyProfileAPIRepository,
            IMasterDataAPIRepository masterDataAPIRepository,
            IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _hostingEnvironment = hostingEnvironment;
            _companyProfileAPIRepository = companyProfileAPIRepository;
            _masterDataAPIRepository = masterDataAPIRepository;

            // Initialize User Data From Session
            var userSessionModel = SessionExtentions.GetSession<UserSessionModel>(_httpContextAccessor.HttpContext.Session, "UserSessionModel");
            if (userSessionModel != null)
            {
                _username = userSessionModel.UserName;
                _saleOrg = userSessionModel.SaleOrg;
                _factoryCode = userSessionModel.FactoryCode;
                _token = userSessionModel.Token;
            }

            this.mapper = mapper;
        }

        public void GetMasterDataToExcelFile(string planCodeSelect)
        {
            throw new NotImplementedException();
        }

        public void GetCompanyProfileSelectList(ref UpdateLotsOfMaterialViewModel UpdateLotsOfMaterialViewModel)
        {
            var companyProfiles = new List<CompanyProfile>();
            companyProfiles = JsonConvert.DeserializeObject<List<CompanyProfile>>(_companyProfileAPIRepository.GetCompanyProfileList(_factoryCode, _token));
            UpdateLotsOfMaterialViewModel.CompanyProfiles = new List<SelectListItem>();
            foreach (var companyProfile in companyProfiles)
            {
                UpdateLotsOfMaterialViewModel.CompanyProfiles.Add(new SelectListItem { Text = companyProfile.Plant, Value = companyProfile.SaleOrg + " " + companyProfile.ShortName });
            }
            UpdateLotsOfMaterialViewModel.factoryCode = _factoryCode;
        }

        public void ReadExcelFileToUpdateMasterData(ref string message, string planCodeSelect, List<IFormFile> fileUpload, ref UpdateLotsOfMaterialViewModel updateLotsOfMaterialViewModel)
        {
            if (fileUpload.Count > 0)
            {
                var fileExcel = fileUpload[0];

                long size = fileExcel.Length;

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
                        ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["Sheet1"];

                        if (worksheet == null)
                        {
                            message = "Sheet Name Wrong ! Sheet Name should be 'Sheet1'";
                        }
                        else
                        {
                            //loop all rows
                            for (int i = worksheet.Dimension.Start.Row; i <= worksheet.Dimension.End.Row; i++)
                            {
                                var rowsObj = new object[worksheet.Dimension.End.Column];

                                if (worksheet.Dimension.End.Column < 2)
                                {
                                    message = "You have column not enough to update PC and Description";
                                    break;
                                }
                                //loop all columns in a row
                                for (int j = worksheet.Dimension.Start.Column; j <= worksheet.Dimension.End.Column; j++)
                                {
                                    if (i == 1)
                                    {
                                        if (worksheet.Cells[i, j].Value.ToString() == "MaterialNo")
                                        {
                                            table.Columns.Add(worksheet.Cells[i, j].Value.ToString(), typeof(string));
                                        }
                                        else if (worksheet.Cells[i, j].Value.ToString() == "PC")
                                        {
                                            table.Columns.Add(worksheet.Cells[i, j].Value.ToString(), typeof(string));
                                        }
                                        else if (worksheet.Cells[i, j].Value.ToString() == "Description")
                                        {
                                            table.Columns.Add(worksheet.Cells[i, j].Value.ToString(), typeof(string));
                                        }

                                    }
                                    else
                                    {
                                        if (worksheet.Cells[i, j].Value != null)
                                        {
                                            if (j == 1 && worksheet.Cells[i, 1].Value.ToString().Length > 18)
                                            {
                                                message = "Length of Material is too long.";
                                                break;
                                            }

                                            if (j == 2 && worksheet.Cells[i, 2].Value.ToString().Length > 20)
                                            {
                                                message = "Length of PC is too long.";
                                                break;
                                            }

                                            if (j == 3 && worksheet.Cells[i, 3].Value.ToString().Length > 40)
                                            {
                                                worksheet.Cells[i, j].Value = worksheet.Cells[i, j].Value.ToString().Substring(0, 40);
                                                //message = "Length of Description is too long.";
                                                //break;
                                            }
                                            rowsObj[j - 1] = worksheet.Cells[i, j].Value.ToString().Replace("'", "''");
                                        }
                                        else
                                        {
                                            rowsObj[j - 1] = "";
                                        }
                                    }

                                    if (j == worksheet.Dimension.End.Column && i != 1 && rowsObj[0] != null)
                                    {
                                        table.Rows.Add(rowsObj);
                                    }
                                }
                                if (message != "")
                                {
                                    break;
                                }
                            }
                        }
                        worksheet.Dispose();
                        excelPackage.Dispose();
                    }

                    if (table.Columns.Count != 0 && table.Rows.Count != 0 && message == "" && table.Rows.Count <= 200)
                    {
                        UpdateMatModel updateMat = new UpdateMatModel();
                        updateMat.MatMasters = new List<MatMaster>();
                        var matMaster = new MatMaster();

                        updateMat.MatMasters = (from rw in table.AsEnumerable()
                                                select new MatMaster()
                                                {
                                                    MaterialNo = rw["MaterialNo"].ToString(),
                                                    PC = rw["PC"].ToString(),
                                                    Description = rw["Description"].ToString()
                                                }).ToList();

                        var jsonDataTable = JsonConvert.SerializeObject(updateMat);
                        message = _masterDataAPIRepository.UpdateLotsMasterData(_factoryCode, _username, "upData", jsonDataTable, _token);
                        updateLotsOfMaterialViewModel.MasterDatas = SetMasterData(table);
                    }
                    else
                    {
                        if (table.Rows.Count > 200)
                        {
                            message = "Please import data less than 300 transactions.";
                        }
                        else
                        {
                            message = "No Data ! or Data is not correct. Please recheck or contact administrator.";
                        }
                    }
                    ms.Dispose();
                }
            }
        }

        public void ReadExcelFileToUpdateStatusX(ref string message, string planCodeSelect, List<IFormFile> fileUpload, ref UpdateLotsOfMaterialViewModel updateLotsOfMaterialViewModel)
        {
            if (fileUpload.Count > 0)
            {
                var fileExcel = fileUpload[0];

                long size = fileExcel.Length;

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
                        ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["Sheet1"];

                        if (worksheet == null)
                        {
                            message = "Sheet Name Wrong ! Sheet Name should be 'Sheet1'";
                        }
                        else
                        {
                            var a = 0;
                            //loop all rows
                            for (int i = worksheet.Dimension.Start.Row; i <= worksheet.Dimension.End.Row; i++)
                            {
                                var rowsObj = new object[1];

                                if (worksheet.Dimension.End.Column < 1)
                                {
                                    message = "You have column not enough to delete Material.";
                                    break;
                                }
                                //loop all columns in a row
                                for (int j = worksheet.Dimension.Start.Column; j <= worksheet.Dimension.End.Column; j++)
                                {
                                    if (i == 1)
                                    {
                                        if (worksheet.Cells[i, j].Value.ToString() == "MaterialNo")
                                        {
                                            table.Columns.Add(worksheet.Cells[i, j].Value.ToString(), typeof(string));
                                            a = j;
                                        }
                                    }
                                    else
                                    {
                                        if (j == a && worksheet.Cells[i, j].Value != null)
                                        {
                                            if (worksheet.Cells[i, 1].Value.ToString().Length > 18)
                                            {
                                                message = "Length of Material is too long.";
                                                break;
                                            }
                                            else
                                            {
                                                rowsObj[j - 1] = worksheet.Cells[i, j].Value;
                                            }
                                        }
                                    }

                                    if (j == worksheet.Dimension.End.Column && i != 1 && rowsObj[0] != null)
                                    {
                                        table.Rows.Add(rowsObj);
                                    }
                                }
                                if (message != "")
                                {
                                    break;
                                }
                            }
                        }
                        worksheet.Dispose();
                        excelPackage.Dispose();
                    }


                    if (table.Columns.Count != 0 && table.Rows.Count != 0 && message == "" && table.Rows.Count <= 200)
                    {
                        UpdateMatModel updateMat = new UpdateMatModel();
                        updateMat.MatMasters = new List<MatMaster>();
                        var matMaster = new MatMaster();

                        updateMat.MatMasters = (from rw in table.AsEnumerable()
                                                select new MatMaster()
                                                {
                                                    MaterialNo = rw["MaterialNo"].ToString()
                                                }).ToList();

                        var jsonDataTable = JsonConvert.SerializeObject(updateMat);
                        message = _masterDataAPIRepository.UpdateLotsMasterData(_factoryCode, _username, "upStatus", jsonDataTable, _token);
                        updateLotsOfMaterialViewModel.MasterDatas = SetMasterData(table);
                    }
                    else
                    {
                        if (table.Rows.Count > 200)
                        {
                            message = "Please import data less than 300 transactions.";
                        }
                        else
                        {
                            message = "No Data ! or Data is not correct. Please recheck or contact administrator.";
                        }
                    }
                    ms.Dispose();
                }
            }
        }

        public List<MasterDataViewModel> SetMasterData(DataTable table)
        {
            var existMasterdata = new MasterData();
            var masterdatas = new List<MasterDataViewModel>();
            var masterdata = new MasterDataViewModel();
            foreach (var tb in table.AsEnumerable())
            {
                existMasterdata = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNoAndFactory(_factoryCode, tb["MaterialNo"].ToString(), _token));

                masterdata = mapper.Map<MasterDataViewModel>(existMasterdata);
                masterdatas.Add(masterdata);
            }

            return masterdatas;
        }
    }
}