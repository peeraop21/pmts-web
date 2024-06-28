using AutoMapper;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using OfficeOpenXml;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView.AutoPackingSpec;
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
    public class AutoPackingSpecService : IAutoPackingSpecService
    {
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IMasterDataAPIRepository masterDataAPIRepository;
        private readonly IAutoPackingSpecAPIRepository autoPackingSpecAPIRepository;
        private readonly IAutoPackingConfigAPIRepository autoPackingConfigAPIRepository;

        private readonly string _username;
        private readonly string _saleOrg;
        private readonly string _factoryCode;
        private readonly string _token;

        public AutoPackingSpecService(IHttpContextAccessor httpContextAccessor,
            IMasterDataAPIRepository masterDataAPIRepository,
            IAutoPackingSpecAPIRepository autoPackingSpecAPIRepository,
            IAutoPackingConfigAPIRepository autoPackingConfigAPIRepository,
            IMapper mapper
            )
        {
            this.httpContextAccessor = httpContextAccessor;
            this.masterDataAPIRepository = masterDataAPIRepository;
            this.autoPackingSpecAPIRepository = autoPackingSpecAPIRepository;
            this.autoPackingConfigAPIRepository = autoPackingConfigAPIRepository;
            this.mapper = mapper;

            // Initialize UserData From Session
            var userSessionModel = SessionExtentions.GetSession<UserSessionModel>(httpContextAccessor.HttpContext.Session, "UserSessionModel");
            if (userSessionModel != null)
            {
                _username = userSessionModel.UserName;
                _saleOrg = userSessionModel.SaleOrg;
                _factoryCode = userSessionModel.FactoryCode;
                _token = userSessionModel.Token;
            }
        }

        public void GetAutoPackingConfigs(ref AutoPackingSpecMainModel result)
        {
            result.AutoPackingConfigs = new List<AutoPackingConfig>();
            result.AutoPackingConfigs = JsonConvert.DeserializeObject<List<AutoPackingConfig>>(autoPackingConfigAPIRepository.GetAutoPackingConfigs(_factoryCode, _token));
        }

        public void ImportAutoPackingSpecFromFile(IFormFile file, ref AutoPackingSpecMainModel autoPackingSpecMainModel, ref string exceptionMessage)
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
                                            rowsObj[j - 1] = !invalidData && columnValue != null && columnValue.ToString().Length < 4 ? worksheet.Cells[i, j].Value : null;
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

                    var autoPackingSpecs = new List<AutoPackingSpec>();
                    var result = new List<AutoPackingSpec>();

                    #region Convert Datatable To AutoPackingSpec Model
                    autoPackingSpecs = (from rw in table.AsEnumerable()
                                        select new AutoPackingSpec()
                                        {
                                            Id = 0,
                                            MaterialNo = rw["Material_No"].ToString(),
                                            NPalletType = rw["nPalletType"].ToString(),
                                            CPalletArrange = rw["cPalletArrange"].ToString(),
                                            CPalletStackPos = rw["cPalletStackPos"].ToString(),
                                            NStrapCompression = rw["nStrapCompression"].ToString(),
                                            CStrapType = rw["cStrapType"].ToString(),
                                            NWrapType = rw["nWrapType"].ToString(),
                                            NTopBoardType = rw["nTopBoardType"].ToString(),
                                            NBottomBoardType = rw["nBottomBoardType"].ToString(),
                                            CStrapperBottomProtection = rw["cStrapperBottomProtection"].ToString(),
                                            CStrapperTopProtection = rw["cStrapperTopProtection"].ToString(),
                                            CornerGuard = rw["cornerGuard"].ToString(),
                                            FactoryCode = _factoryCode,
                                            CreatedDate = DateTime.Now,
                                            CreatedBy = _username,
                                            UpdatedBy = _username,
                                            UpdatedDate = DateTime.Now
                                        }).ToList();

                    #endregion

                    if (autoPackingSpecs.Count > 200)
                    {
                        throw new Exception("Upload Excel file (Over 200 Material).");
                    }

                    result = JsonConvert.DeserializeObject<List<AutoPackingSpec>>(autoPackingSpecAPIRepository.CreateAutoPackingSpecsFromFile(_factoryCode, JsonConvert.SerializeObject(autoPackingSpecs), _token));
                    var textExceptionMessage = "";

                    foreach (var AutoPackingSpec in result)
                    {
                        if (AutoPackingSpec.Id == 0 || AutoPackingSpec.Id == null)
                        {
                            textExceptionMessage = textExceptionMessage + AutoPackingSpec.MaterialNo + ", ";
                        }
                        else
                        {
                            var AutoPackingSpecViewModel = mapper.Map<AutoPackingSpecViewModel>(AutoPackingSpec);
                            AutoPackingSpecViewModel.IsAvailable = true;
                            autoPackingSpecMainModel.AutoPackingSpecs.Add(AutoPackingSpecViewModel);
                        }
                    }

                    exceptionMessage = !string.IsNullOrEmpty(textExceptionMessage) ? $"Material No. {textExceptionMessage.Substring(0, textExceptionMessage.Length - 2)} doesn't exist." : string.Empty;
                }
            }
        }

        public AutoPackingSpecViewModel SaveAndUpdateAutoPackingSpec(AutoPackingSpec autoPackingSpec)
        {
            var isavailable = false;
            if (autoPackingSpec.Id == null || autoPackingSpec.Id == 0)
            {
                autoPackingSpec.Id = 0;
                autoPackingSpec.FactoryCode = _factoryCode;
                autoPackingSpec.CreatedBy = _username;
                autoPackingSpec.CreatedDate = DateTime.Now;

                autoPackingSpecAPIRepository.SaveAutoPackingSpec(_factoryCode, JsonConvert.SerializeObject(autoPackingSpec), _token);
                isavailable = true;
            }
            else
            {
                autoPackingSpec.UpdatedBy = _username;
                autoPackingSpec.UpdatedDate = DateTime.Now;

                autoPackingSpecAPIRepository.UpdateAutoPackingSpec(_factoryCode, JsonConvert.SerializeObject(autoPackingSpec), _token);
                isavailable = true;
            }

            var autoPackingSpecs = JsonConvert.DeserializeObject<List<AutoPackingSpec>>(autoPackingSpecAPIRepository.GetAutoPackingSpecByMaterialNo(_factoryCode, autoPackingSpec.MaterialNo, _token));
            var result = mapper.Map<AutoPackingSpec, AutoPackingSpecViewModel>(autoPackingSpecs[0]);
            result.IsAvailable = isavailable;
            return result;
        }

        public IEnumerable<AutoPackingSpecViewModel> SearchAutoPackingSpec(string materialNo, ref bool existMasterData)
        {
            var result = new List<AutoPackingSpecViewModel>();

            var masterData = JsonConvert.DeserializeObject<MasterData>(masterDataAPIRepository.GetMasterDataByMaterialNoAndFactory(_factoryCode, materialNo, _token));

            if (masterData != null)
            {
                existMasterData = true;
                var autoPackingSpecs = JsonConvert.DeserializeObject<List<AutoPackingSpec>>(autoPackingSpecAPIRepository.GetAutoPackingSpecByMaterialNo(_factoryCode, materialNo, _token));
                result = mapper.Map<List<AutoPackingSpec>, List<AutoPackingSpecViewModel>>(autoPackingSpecs);
                result.ForEach(x => x.IsAvailable = (x.Id != 0) ? true : false);
                if (result.Count == 0)
                {
                    result.Clear();
                    result.Add(new AutoPackingSpecViewModel
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
