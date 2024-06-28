using AutoMapper;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.LogisticAndWarehouse;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Tracing;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services
{
    [TraceAspect]
    public class LogisticAndWarehouseService : ILogisticAndWarehouseService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IMasterDataAPIRepository masterDataAPIRepository;
        private readonly ITruckOptimizeAPIRepository truckOptimizeAPIRepository;
        private readonly IMapper mapper;

        private readonly string _username;
        private readonly string _saleOrg;
        private readonly string _factoryCode;
        private readonly string _token;

        public LogisticAndWarehouseService(IHttpContextAccessor httpContextAccessor,
            IMasterDataAPIRepository masterDataAPIRepository,
            ITruckOptimizeAPIRepository truckOptimizeAPIRepository,
            IMapper mapper)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.masterDataAPIRepository = masterDataAPIRepository;
            this.truckOptimizeAPIRepository = truckOptimizeAPIRepository;

            // Initialize UserData From Session
            var userSessionModel = SessionExtentions.GetSession<UserSessionModel>(httpContextAccessor.HttpContext.Session, "UserSessionModel");
            if (userSessionModel != null)
            {
                _username = userSessionModel.UserName;
                _saleOrg = userSessionModel.SaleOrg;
                _factoryCode = userSessionModel.FactoryCode;
                _token = userSessionModel.Token;
            }

            this.mapper = mapper;
        }

        public void ImportTruckOptimizeFromFile(IFormFile file, ref TruckOptimizeMainModel truckOptimizeMainModel, ref string exceptionMessage)
        {
            if (file != null)
            {
                long size = file.Length;

                // full path to file in temp location
                var filePath = Path.GetTempFileName();

                using (var ms = new MemoryStream())
                {
                    DataTable table = new DataTable();

                    file.CopyToAsync(ms);
                    var fileBytes = ms.ToArray();
                    string s = Convert.ToBase64String(fileBytes);
                    var firstSheet = true;
                    var invalidData = false;

                    using (ExcelPackage excelPackage = new ExcelPackage(ms))
                    {
                        //ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["BoardCombineAcc"];
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        List<ExcelWorksheet> worksheets = excelPackage.Workbook.Worksheets.ToList();

                        foreach (var worksheet in worksheets)
                        {
                            //loop all rows
                            for (int i = worksheet.Dimension.Start.Row; i <= worksheet.Dimension.End.Row; i++)
                            {
                                invalidData = false;
                                var rowsObj = new object[worksheet.Dimension.End.Column];
                                //loop all columns in a row
                                for (int j = worksheet.Dimension.Start.Column; j <= worksheet.Dimension.End.Column; j++)
                                {
                                    if (i == 1 && firstSheet)
                                    {
                                        table.Columns.Add(worksheet.Cells[i, j].Value.ToString(), typeof(string));
                                    }
                                    else
                                    {
                                        var columnValue = worksheet.Cells[i, j].Value;
                                        if (j == 1)
                                        {
                                            invalidData = columnValue == null;
                                            rowsObj[j - 1] = columnValue == null ? worksheet.Cells[i, j].Value : worksheet.Cells[i, j].Value.ToString();
                                        }
                                        else
                                        {
                                            rowsObj[j - 1] = !invalidData ? Convert.ToInt32(worksheet.Cells[i, j].Value) : 0;
                                        }
                                    }

                                    if (j == worksheet.Dimension.End.Column && i != 1 && !invalidData)
                                    {
                                        table.Rows.Add(rowsObj);
                                    }
                                }
                            }

                            firstSheet = false;
                        }
                    }

                    var truckOptimizes = new List<TruckOptimize>();
                    var result = new List<TruckOptimize>();

                    #region Convert Datatable To TruckOptimize Model
                    truckOptimizes = (from rw in table.AsEnumerable()
                                      select new TruckOptimize()
                                      {
                                          Id = 0,
                                          MaterialNo = rw["Material_No"].ToString(),
                                          FgpalletW = Convert.ToInt32(rw["FGPallet_W"]),
                                          FgpalletL = Convert.ToInt32(rw["FGPallet_L"]),
                                          FgpalletH = Convert.ToInt32(rw["FGPallet_H"]),
                                          FgbundleH = Convert.ToInt32(rw["FGBundle_H"]),
                                          FgbundleL = Convert.ToInt32(rw["FGBundle_L"]),
                                          FgbundleW = Convert.ToInt32(rw["FGBundle_W"]),
                                          PalletSizeH = Convert.ToInt32(rw["PalletSize_H"]),
                                          PalletSizeL = Convert.ToInt32(rw["PalletSize_L"]),
                                          PalletSizeW = Convert.ToInt32(rw["PalletSize_W"]),
                                          FactoryCode = _factoryCode,
                                          CreatedDate = DateTime.Now,
                                          CreatedBy = _username,
                                          UpdatedDate = DateTime.Now,
                                          UpdatedBy = _username
                                      }).ToList();

                    #endregion

                    if (truckOptimizes.Count > 200)
                    {
                        throw new Exception("Upload Excel file (Over 200 Material).");
                    }

                    result = JsonConvert.DeserializeObject<List<TruckOptimize>>(truckOptimizeAPIRepository.CreateTruckOptimizesFromFile(_factoryCode, JsonConvert.SerializeObject(truckOptimizes), _token));
                    var textExceptionMessage = "";

                    foreach (var truckOptimize in result)
                    {
                        if (truckOptimize.Id == 0 || truckOptimize.Id == null)
                        {
                            textExceptionMessage = textExceptionMessage + truckOptimize.MaterialNo + ", ";
                        }
                        else
                        {

                            var truckOptimizeViewModel = mapper.Map<TruckOptimizeViewModel>(truckOptimize);
                            truckOptimizeViewModel.IsAvailable = true;
                            truckOptimizeMainModel.TruckOptimizeViewModels.Add(truckOptimizeViewModel);
                        }
                    }

                    exceptionMessage = !string.IsNullOrEmpty(textExceptionMessage) ? $"Material No. {textExceptionMessage.Substring(0, textExceptionMessage.Length - 2)} doesn't exist." : string.Empty;
                }
            }
        }

        public TruckOptimizeViewModel SaveAndUpdateTruckOptimize(TruckOptimize truckOptimize)
        {
            var isavailable = false;
            if (truckOptimize.Id == null || truckOptimize.Id == 0)
            {
                truckOptimize.Id = 0;
                truckOptimize.FactoryCode = _factoryCode;
                truckOptimize.CreatedBy = _username;
                truckOptimize.CreatedDate = DateTime.Now;

                truckOptimizeAPIRepository.SaveTruckOptimize(_factoryCode, JsonConvert.SerializeObject(truckOptimize), _token);
                isavailable = true;
            }
            else
            {
                truckOptimize.UpdatedBy = _username;
                truckOptimize.UpdatedDate = DateTime.Now;

                truckOptimizeAPIRepository.UpdateTruckOptimize(_factoryCode, JsonConvert.SerializeObject(truckOptimize), _token);
                isavailable = true;
            }

            var truckOptimizes = JsonConvert.DeserializeObject<List<TruckOptimize>>(truckOptimizeAPIRepository.GetTruckOptimizeByMaterialNo(_factoryCode, truckOptimize.MaterialNo, _token));
            var result = mapper.Map<TruckOptimize, TruckOptimizeViewModel>(truckOptimizes[0]);
            result.IsAvailable = isavailable;
            return result;
        }

        public IEnumerable<TruckOptimizeViewModel> SearchTruckOptimize(string materialNo, ref bool existMasterData)
        {
            var result = new List<TruckOptimizeViewModel>();

            var masterData = JsonConvert.DeserializeObject<MasterData>(masterDataAPIRepository.GetMasterDataByMaterialNoAndFactory(_factoryCode, materialNo, _token));

            if (masterData != null)
            {
                existMasterData = true;
                var truckOptimizes = JsonConvert.DeserializeObject<List<TruckOptimize>>(truckOptimizeAPIRepository.GetTruckOptimizeByMaterialNo(_factoryCode, materialNo, _token));
                result = mapper.Map<List<TruckOptimize>, List<TruckOptimizeViewModel>>(truckOptimizes);
                result.ForEach(x => x.IsAvailable = (x.Id != 0) ? true : false);
                if (result.Count == 0)
                {
                    result.Clear();
                    result.Add(new TruckOptimizeViewModel
                    {
                        Id = 0,
                        MaterialNo = materialNo
                    });
                }
            }

            return result;
        }
    }
}
