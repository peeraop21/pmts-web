using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OfficeOpenXml;
using PMTs.DataAccess;
using PMTs.DataAccess.ComplexModel;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.AddTagCustomer;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.NewProduct;
using PMTs.DataAccess.Repository;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Shared;
using PMTs.DataAccess.Tracing;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services
{
    [TraceAspect]
    [SessionTimeout]
    public class MasterDataService : IMasterDataService
    {
        private IHttpContextAccessor _httpContextAccessor;
        private IMasterDataAPIRepository _masterDataAPIRepository;
        private readonly ISalesViewAPIRepository salesViewAPIRepository;
        private readonly IPlantViewAPIRepository plantViewAPIRepository;
        private readonly IRoutingAPIRepository routingAPIRepository;
        private readonly ITransactionsDetailAPIRepository transactionsDetailAPIRepository;
        private readonly IPMTsConfigAPIRepository pmtsConfigAPIRepository;
        private readonly ICompanyProfileAPIRepository companyProfileAPIRepository;
        private readonly IStandardPatternNameAPIRepository standardPatternNameAPIRepository;
        private readonly ITagPrintSORepository _tagPrintSORepository;

        private readonly string _username;
        private readonly string _saleOrg;
        private readonly string _factoryCode;
        private readonly string _Token;

        public MasterDataService(IHttpContextAccessor httpContextAccessor,
            IMasterDataAPIRepository masterDataAPIRepository,
            ISalesViewAPIRepository salesViewAPIRepository,
            IPlantViewAPIRepository plantViewAPIRepository,
            IRoutingAPIRepository routingAPIRepository,
            ITransactionsDetailAPIRepository transactionsDetailAPIRepository,
            ICompanyProfileAPIRepository companyProfileAPIRepository,
            IPMTsConfigAPIRepository pmtsConfigAPIRepository,
            ITagPrintSORepository tagPrintSORepository,
            IStandardPatternNameAPIRepository standardPatternNameAPIRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _masterDataAPIRepository = masterDataAPIRepository;
            this.salesViewAPIRepository = salesViewAPIRepository;
            this.plantViewAPIRepository = plantViewAPIRepository;
            this.routingAPIRepository = routingAPIRepository;
            this.transactionsDetailAPIRepository = transactionsDetailAPIRepository;
            this.pmtsConfigAPIRepository = pmtsConfigAPIRepository;
            this.companyProfileAPIRepository = companyProfileAPIRepository;
            this.standardPatternNameAPIRepository = standardPatternNameAPIRepository;
            _tagPrintSORepository = tagPrintSORepository;

            // Initialize SaleOrg and PlantCode from Session
            var userSessionModel = SessionExtentions.GetSession<UserSessionModel>(_httpContextAccessor.HttpContext.Session, "UserSessionModel");
            if (userSessionModel != null)
            {
                _username = userSessionModel.UserName;
                _saleOrg = userSessionModel.SaleOrg;
                _factoryCode = userSessionModel.FactoryCode;
                _Token = userSessionModel.Token;
            }
        }

        public void GetMasterData(ref List<MasterDataRoutingModel> masterDataRoutingModelList, string typeSearch, string keySearch, string flag, bool isMaterialOnly)
        {
            if ((String.IsNullOrEmpty(keySearch) || string.IsNullOrWhiteSpace(keySearch)) && flag == null)
            {
                if (isMaterialOnly)
                {
                    masterDataRoutingModelList = new List<MasterDataRoutingModel>();
                }
                else
                {
                    var result = JsonConvert.DeserializeObject<List<MasterDataRoutingModel>>(_masterDataAPIRepository.GetMasterDataList(_factoryCode, _Token));
                    masterDataRoutingModelList = result.OrderByDescending(x => x.LastUpdate).ToList();
                }
            }
            else
            {
                if (isMaterialOnly)
                {
                    var result = JsonConvert.DeserializeObject<List<MasterDataRoutingModel>>(_masterDataAPIRepository.GetMasterDataAllByKeySearch(keySearch, _Token));
                    masterDataRoutingModelList = result.OrderByDescending(x => x.LastUpdate).ToList();
                }
                else
                {
                    var result = JsonConvert.DeserializeObject<List<MasterDataRoutingModel>>(_masterDataAPIRepository.GetMasterDataByKeySearch(_factoryCode, typeSearch, keySearch, _Token, flag));
                    masterDataRoutingModelList = result.OrderByDescending(x => x.LastUpdate).ToList();
                }
            }
        }

        public void DeleteMasterData(string Material)
        {
            var masterData = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNo(_factoryCode, Material, _Token));

            masterData.PdisStatus = "X";

            ParentModel MasterDataParent = new ParentModel();
            MasterDataParent.AppName = Globals.AppNameEncrypt;
            MasterDataParent.SaleOrg = _saleOrg;
            MasterDataParent.PlantCode = _factoryCode;
            MasterDataParent.MasterData = masterData;
            string jsonString = JsonConvert.SerializeObject(MasterDataParent);
            _masterDataAPIRepository.UpdateMasterData(jsonString, _Token);
        }

        public void GetMasterDataDetail(ref MasterDataRoutingModel masterDataRoutingModelList, string MaterialNo)
        {
            masterDataRoutingModelList = new MasterDataRoutingModel();
            masterDataRoutingModelList = JsonConvert.DeserializeObject<MasterDataRoutingModel>(_masterDataAPIRepository.GetMasterDataByMaterialNo(_factoryCode, MaterialNo, _Token));
        }

        public void UpdateReuseMaterialNos(ref List<MasterData> masterDatas, List<string> materialNOs)
        {
            //current model data
            var existReuseMasterDatas = new List<MasterData>();
            var existReuseRoutings = new List<Routing>();
            var existReusePlantViews = new List<PlantView>();
            var existReuseSalesViews = new List<SalesView>();
            var existReuseTransactionsDetails = new List<TransactionsDetail>();
            //new model data
            var reuseMasterDatas = new List<MasterData>();
            var reuseRoutings = new List<Routing>();
            var reusePlantViews = new List<PlantView>();
            var reuseSalesViews = new List<SalesView>();
            var reuseTransactionsDetails = new List<TransactionsDetail>();

            existReuseMasterDatas = JsonConvert.DeserializeObject<List<MasterData>>(_masterDataAPIRepository.GetReuseMasterDatasByMaterialNos(_factoryCode, JsonConvert.SerializeObject(materialNOs), _Token));
            existReuseRoutings = JsonConvert.DeserializeObject<List<Routing>>(routingAPIRepository.GetRoutingsByMaterialNos(_factoryCode, JsonConvert.SerializeObject(materialNOs), _Token));
            if (existReuseRoutings.Count > 0)
            {
                existReuseRoutings = existReuseRoutings.Where(r => r.PdisStatus.ToUpper() == "X").ToList();
            }

            existReusePlantViews = JsonConvert.DeserializeObject<List<PlantView>>(plantViewAPIRepository.GetReusePlantViewsByMaterialNos(_factoryCode, JsonConvert.SerializeObject(materialNOs), _Token));
            existReuseSalesViews = JsonConvert.DeserializeObject<List<SalesView>>(salesViewAPIRepository.GetReuseSaleViewsByMaterialNos(_factoryCode, JsonConvert.SerializeObject(materialNOs), _Token));
            existReuseTransactionsDetails = JsonConvert.DeserializeObject<List<TransactionsDetail>>(transactionsDetailAPIRepository.GetTransactionsDetailsByMaterialNOs(_factoryCode, JsonConvert.SerializeObject(materialNOs), _Token));
            if (existReuseTransactionsDetails.Count > 0)
            {
                existReuseTransactionsDetails = existReuseTransactionsDetails.Where(r => (r.PdisStatus == "X" || r.PdisStatus == null) && !r.Outsource).ToList();
            }

            foreach (var masterData in existReuseMasterDatas)
            {
                var routings = new List<Routing>();
                var plantViews = new List<PlantView>();
                var saleViews = new List<SalesView>();
                var transactionsDetails = new TransactionsDetail();
                masterData.PdisStatus = "M";
                masterData.User = "ZZ_Import";
                var companyProfile = JsonConvert.DeserializeObject<List<CompanyProfile>>(companyProfileAPIRepository.GetCompanyProfileBySaleOrg(_factoryCode, _saleOrg, _Token));
                if (companyProfile != null && companyProfile[0].Plant == masterData.FactoryCode)
                {
                    masterData.TranStatus = false;
                }
                masterData.LastUpdate = DateTime.Now;
                masterData.UpdatedBy = _username;

                routings = existReuseRoutings.Where(r => r.MaterialNo == masterData.MaterialNo).ToList();

                if (routings.Count > 0)
                {
                    routings.ForEach(r => r.PdisStatus = "M");
                }

                plantViews = existReusePlantViews.Where(r => r.MaterialNo == masterData.MaterialNo).ToList();

                if (plantViews.Count > 0)
                {
                    plantViews.ForEach(r => r.PdisStatus = "M");
                }

                saleViews = existReuseSalesViews.Where(r => r.MaterialNo == masterData.MaterialNo).ToList();

                if (saleViews.Count > 0)
                {
                    saleViews.ForEach(r => r.PdisStatus = "M");
                }

                transactionsDetails = existReuseTransactionsDetails.FirstOrDefault(r => r.MaterialNo == masterData.MaterialNo);

                if (transactionsDetails != null)
                {
                    transactionsDetails.PdisStatus = "M";
                }

                reuseMasterDatas.Add(masterData);
                reuseRoutings.AddRange(routings);
                reusePlantViews.AddRange(plantViews);
                reuseSalesViews.AddRange(saleViews);
                reuseTransactionsDetails.Add(transactionsDetails);
            }

            var parentModel = new ParentModel
            {
                AppName = Globals.AppNameEncrypt,
                SaleOrg = _saleOrg,
                PlantCode = _factoryCode,
                MasterDataList = reuseMasterDatas,
                SalesViewList = reuseSalesViews,
                PlantViewList = reusePlantViews,
                RoutingList = reuseRoutings,
                TransactionsDetailList = reuseTransactionsDetails,
            };

            _masterDataAPIRepository.UpdateReuseMaterialNos(_factoryCode, JsonConvert.SerializeObject(parentModel), _Token);
        }

        public void GetReUseMasterDatas(ref List<MasterDataRoutingModel> masterDatas, string materialNo, List<string> materialNos)
        {
            masterDatas = new List<MasterDataRoutingModel>();
            var materialNoStrs = new List<string>();

            if (!string.IsNullOrEmpty(materialNo) && materialNos == null)
            {
                materialNoStrs.Add(materialNo);
            }
            else
            {
                materialNoStrs.AddRange(materialNos);
                materialNoStrs = materialNoStrs.GroupBy(x => x).Select(g => g.First()).ToList();
            }

            masterDatas = JsonConvert.DeserializeObject<List<MasterDataRoutingModel>>(_masterDataAPIRepository.GetReuseMasterDataRoutingsByMaterialNos(_factoryCode, JsonConvert.SerializeObject(materialNoStrs), _Token));
        }

        public MemoryStream GetAttachFilePDFMOFromMasterData(string materialNo)
        {
            var memoryStream = new MemoryStream();
            var masterdata = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNoAndFactory(_factoryCode, materialNo, _Token));
            var base64 = string.Empty;
            var bytes = File.ReadAllBytes(masterdata.AttachFileMoPath);

            return new MemoryStream(bytes);
        }

        public MemoryStream GetSemiFilePDFMOFromMasterData(string materialNo)
        {
            var memoryStream = new MemoryStream();
            var masterdata = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNoAndFactory(_factoryCode, materialNo, _Token));
            var base64 = string.Empty;
            var bytes = File.ReadAllBytes(masterdata.SemiFilePdfPath);

            return new MemoryStream(bytes);
        }

        public void SaveAttachFileMO(IFormFile file)
        {
            var transactionDataModelSession = SessionExtentions.GetSession<TransactionDataModel>(_httpContextAccessor.HttpContext.Session, "TransactionDataModel");
            var pmtsConfig = JsonConvert.DeserializeObject<PmtsConfig>(pmtsConfigAPIRepository.GetPMTsConfigByFactoryName(_factoryCode, "ManageMO_Path", _Token));

            var masterdata = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNoAndFactory(_factoryCode, transactionDataModelSession.MaterialNo, _Token));
            var filePath = pmtsConfig != null ? pmtsConfig.FucValue : throw new Exception("Can't get file path.");

            var masterdatasOS = new List<MasterData>();
            if (!string.IsNullOrEmpty(transactionDataModelSession.MaterialNo))
            {
                masterdatasOS = JsonConvert.DeserializeObject<List<MasterData>>(_masterDataAPIRepository.GetMasterDatasByMatSaleOrgNonX(_factoryCode, transactionDataModelSession.MaterialNo, _Token));
            }

            if (file.Length > 0)
            {
                var GenarateFileName = transactionDataModelSession.MaterialNo + "_" + _factoryCode + "_File.pdf";
                var fullPath = Path.Combine(filePath, GenarateFileName);

                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }

                using (var localFile = System.IO.File.OpenWrite(fullPath))
                {
                    using (var uploadedFile = file.OpenReadStream())
                    {
                        if (masterdata != null)
                        {
                            masterdata.AttachFileMoPath = fullPath;
                            masterdata.LastUpdate = DateTime.Now;
                            masterdata.UpdatedBy = _username;
                            masterdata.TranStatus = false;
                            masterdata.User = _username;

                            var parentModelMasterData = new ParentModel()
                            {
                                AppName = Globals.AppNameEncrypt,
                                SaleOrg = _saleOrg,
                                PlantCode = _factoryCode,
                                MasterData = masterdata
                            };

                            //upload file to server
                            uploadedFile.CopyTo(localFile);

                            //update attach file mo field in masterdata
                            _masterDataAPIRepository.UpdateMasterData(JsonConvert.SerializeObject(parentModelMasterData), _Token);

                            masterdatasOS.ForEach(m => m.AttachFileMoPath = fullPath);
                            masterdatasOS.ForEach(m => m.LastUpdate = DateTime.Now);
                            masterdatasOS.ForEach(m => m.UpdatedBy = _username);
                            masterdatasOS.ForEach(m => m.TranStatus = false);
                            masterdatasOS.ForEach(m => m.User = _username);

                            _masterDataAPIRepository.UpdateMasterDatas(_factoryCode, JsonConvert.SerializeObject(masterdatasOS), _Token);

                            //set new transesssion
                            transactionDataModelSession.modelProductPicture.AttachFileMoPath = masterdata.AttachFileMoPath;
                        }
                        else
                        {
                            throw new Exception("This master data doesn't exist.");
                        }
                    }
                }
            }

            SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "TransactionDataModel", transactionDataModelSession);
        }

        public void DeleteAttachFileMO()
        {
            var transactionDataModelSession = SessionExtentions.GetSession<TransactionDataModel>(_httpContextAccessor.HttpContext.Session, "TransactionDataModel");
            var pmtsConfig = JsonConvert.DeserializeObject<PmtsConfig>(pmtsConfigAPIRepository.GetPMTsConfigByFactoryName(_factoryCode, "ManageMO_Path", _Token));

            var masterdata = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNoAndFactory(_factoryCode, transactionDataModelSession.MaterialNo, _Token));
            var filePath = pmtsConfig != null ? pmtsConfig.FucValue : throw new Exception("Can't get file path.");

            var masterdatasOS = new List<MasterData>();
            if (!string.IsNullOrEmpty(transactionDataModelSession.MaterialNo))
            {
                masterdatasOS = JsonConvert.DeserializeObject<List<MasterData>>(_masterDataAPIRepository.GetMasterDatasByMatSaleOrgNonX(_factoryCode, transactionDataModelSession.MaterialNo, _Token));
            }
            if (masterdata != null)
            {
                if (File.Exists(masterdata.AttachFileMoPath))
                {
                    //delete file from local
                    File.Delete(masterdata.AttachFileMoPath);
                }

                masterdata.AttachFileMoPath = string.Empty;
                masterdata.LastUpdate = DateTime.Now;
                masterdata.UpdatedBy = _username;
                masterdata.TranStatus = false;
                masterdata.User = _username;

                var parentModelMasterData = new ParentModel()
                {
                    AppName = Globals.AppNameEncrypt,
                    SaleOrg = _saleOrg,
                    PlantCode = _factoryCode,
                    MasterData = masterdata
                };

                //update attach file mo field in masterdata
                _masterDataAPIRepository.UpdateMasterData(JsonConvert.SerializeObject(parentModelMasterData), _Token);

                masterdatasOS.ForEach(m => m.AttachFileMoPath = string.Empty);
                masterdatasOS.ForEach(m => m.LastUpdate = DateTime.Now);
                masterdatasOS.ForEach(m => m.UpdatedBy = _username);
                masterdatasOS.ForEach(m => m.TranStatus = false);
                masterdatasOS.ForEach(m => m.User = _username);

                _masterDataAPIRepository.UpdateMasterDatas(_factoryCode, JsonConvert.SerializeObject(masterdatasOS), _Token);

                //set new transesssion
                transactionDataModelSession.modelProductPicture.AttachFileMoPath = string.Empty;
            }
            else
            {
                throw new Exception("This master data doesn't exist.");
            }

            SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "TransactionDataModel", transactionDataModelSession);
        }

        public void SaveAttachFileSemi(IFormFile file)
        {
            var transactionDataModelSession = SessionExtentions.GetSession<TransactionDataModel>(_httpContextAccessor.HttpContext.Session, "TransactionDataModel");
            var pmtsConfig = JsonConvert.DeserializeObject<PmtsConfig>(pmtsConfigAPIRepository.GetPMTsConfigByFactoryName(_factoryCode, "Semi4_Path", _Token));

            var masterdata = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNoAndFactory(_factoryCode, transactionDataModelSession.MaterialNo, _Token));
            var filePath = pmtsConfig != null ? pmtsConfig.FucValue : throw new Exception("Can't get file path.");

            if (file.Length > 0)
            {
                var GenarateFileName = transactionDataModelSession.MaterialNo + "_" + _factoryCode + "_File.pdf";
                var fullPath = Path.Combine(filePath, GenarateFileName);

                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }

                using (var localFile = System.IO.File.OpenWrite(fullPath))
                {
                    using (var uploadedFile = file.OpenReadStream())
                    {
                        if (masterdata != null)
                        {
                            masterdata.SemiFilePdfPath = fullPath;
                            masterdata.LastUpdate = DateTime.Now;
                            masterdata.UpdatedBy = _username;
                            masterdata.TranStatus = false;
                            masterdata.User = _username;

                            var parentModelMasterData = new ParentModel()
                            {
                                AppName = Globals.AppNameEncrypt,
                                SaleOrg = _saleOrg,
                                PlantCode = _factoryCode,
                                MasterData = masterdata
                            };

                            //upload file to server
                            uploadedFile.CopyTo(localFile);

                            //update attach file mo field in masterdata
                            _masterDataAPIRepository.UpdateMasterData(JsonConvert.SerializeObject(parentModelMasterData), _Token);

                            //set new transesssion
                            transactionDataModelSession.modelProductPicture.SemiFilePdfPath = masterdata.SemiFilePdfPath;
                        }
                        else
                        {
                            throw new Exception("This master data doesn't exist.");
                        }
                    }
                }
            }

            SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "TransactionDataModel", transactionDataModelSession);
        }

        public void DeleteAttachFileSemi()
        {
            var transactionDataModelSession = SessionExtentions.GetSession<TransactionDataModel>(_httpContextAccessor.HttpContext.Session, "TransactionDataModel");
            var pmtsConfig = JsonConvert.DeserializeObject<PmtsConfig>(pmtsConfigAPIRepository.GetPMTsConfigByFactoryName(_factoryCode, "Semi4_Path", _Token));

            var masterdata = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNoAndFactory(_factoryCode, transactionDataModelSession.MaterialNo, _Token));
            var filePath = pmtsConfig != null ? pmtsConfig.FucValue : throw new Exception("Can't get file path.");

            if (masterdata != null)
            {
                if (File.Exists(masterdata.SemiFilePdfPath))
                {
                    //delete file from local
                    File.Delete(masterdata.SemiFilePdfPath);
                }

                masterdata.SemiFilePdfPath = string.Empty;
                masterdata.LastUpdate = DateTime.Now;
                masterdata.UpdatedBy = _username;
                masterdata.TranStatus = false;
                masterdata.User = _username;

                var parentModelMasterData = new ParentModel()
                {
                    AppName = Globals.AppNameEncrypt,
                    SaleOrg = _saleOrg,
                    PlantCode = _factoryCode,
                    MasterData = masterdata
                };

                //update attach file mo field in masterdata
                _masterDataAPIRepository.UpdateMasterData(JsonConvert.SerializeObject(parentModelMasterData), _Token);

                //set new transesssion
                transactionDataModelSession.modelProductPicture.SemiFilePdfPath = string.Empty;
            }
            else
            {
                throw new Exception("This master data doesn't exist.");
            }

            SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "TransactionDataModel", transactionDataModelSession);
        }

        public void ImportUpdateMaterialFromFile(IFormFile file, ref string exceptionMessage)
        {
            if (file != null)
            {
                long size = file.Length;

                // full path to file in temp location
                var filePath = Path.GetTempFileName();
                var column = new List<string>();
                string[] numbericArr = { "Hardship", "Bun", "BunLayer", "LayerPalet", "BoxPalet", "PaperWidth", "CutNo", "Trim", "PercenTrim" };
                using (var ms = new MemoryStream())
                {
                    DataTable table = new DataTable();

                    file.CopyToAsync(ms);
                    var fileBytes = ms.ToArray();
                    string s = Convert.ToBase64String(fileBytes);
                    var invalidData = false;
                    var caseMasterData = true;
                    int maxColumn = 0;
                    int maxRow = 0;

                    using (ExcelPackage excelPackage = new ExcelPackage(ms))
                    {
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        //ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["BoardCombineAcc"];

                        List<ExcelWorksheet> worksheets = excelPackage.Workbook.Worksheets.ToList();
                        foreach (var worksheet in worksheets)
                        {
                            for (var i = worksheet.Dimension.End.Column; i >= 0; i--)
                            {
                                if (worksheet.Cells[1, i].Value != null)
                                {
                                    maxColumn = i;
                                    break;
                                }
                            }

                            //loop all rows
                            for (int i = worksheet.Dimension.Start.Row; i <= worksheet.Dimension.End.Row; i++)
                            {
                                invalidData = false;
                                var rowsObj = new object[maxColumn];
                                //loop all columns in a row
                                for (int j = worksheet.Dimension.Start.Column; j <= maxColumn; j++)
                                {
                                    var columnValue = worksheet.Cells[i, j].Value;
                                    if (i == 1)
                                    {
                                        if (worksheet.Cells[i, j] != null && columnValue != null)
                                        {
                                            column.Add(columnValue.ToString());

                                            if (numbericArr.Contains(columnValue))
                                            {
                                                table.Columns.Add(columnValue.ToString(), typeof(float));
                                            }
                                            else
                                            {
                                                table.Columns.Add(columnValue.ToString(), typeof(string));
                                            }
                                        }

                                        if (columnValue.ToString() == "Machine")
                                        {
                                            caseMasterData = false;
                                        }
                                    }
                                    else if (i >= 2)
                                    {
                                        if (numbericArr.Contains(column[j - 1]))
                                        {
                                            rowsObj[j - 1] = columnValue != null ? columnValue : null;
                                        }
                                        else
                                        {
                                            rowsObj[j - 1] = columnValue == null ? columnValue : columnValue.ToString();
                                        }
                                    }

                                    if (i != 1 && j == maxColumn)
                                    {
                                        if (caseMasterData)
                                        {
                                            if (worksheet.Cells[i, 1].Value != null && worksheet.Cells[i, 2].Value != null)
                                            {
                                                table.Rows.Add(rowsObj);
                                            }
                                        }
                                        else
                                        {
                                            if (worksheet.Cells[i, 1].Value != null && worksheet.Cells[i, 2].Value != null && worksheet.Cells[i, 3].Value != null)
                                            {
                                                table.Rows.Add(rowsObj);
                                            }
                                        }

                                        continue;
                                    }
                                }
                            }
                        }
                    }

                    var masterDatas = new List<MasterData>();
                    var routings = new List<Routing>();
                    var numOfResults = 0;

                    int hardShip, bun, bunLayer, layerPallet, boxPallet, paperWidth, cutNo, trim, seqNO;
                    double percenTrim;

                    if (caseMasterData)
                    {
                        #region Convert Datatable To MasterData

                        masterDatas = (from rw in table.AsEnumerable()
                                       select new MasterData()
                                       {
                                           Id = 0,
                                           MaterialNo = rw.Table.Columns.Contains("Material_No") ? rw["Material_No"].ToString() : null,
                                           PartNo = rw.Table.Columns.Contains("PartNO") ? rw["PartNO"].ToString() : null,
                                           Pc = rw.Table.Columns.Contains("PC") ? rw["PC"].ToString() : null,
                                           Description = rw.Table.Columns.Contains("Description") ? rw["Description"].ToString() : null,
                                           SaleText1 = rw.Table.Columns.Contains("SaleText") ? rw["SaleText"].ToString() : null,
                                           Change = rw.Table.Columns.Contains("Change") ? rw["Change"].ToString() : null,
                                           Hardship = rw.Table.Columns.Contains("Hardship") ? Int32.TryParse(rw["Hardship"].ToString(), out hardShip) ? hardShip : -999 : -999,
                                           Bun = rw.Table.Columns.Contains("Bun") ? Int32.TryParse(rw["Bun"].ToString(), out bun) ? bun : -999 : -999,
                                           BunLayer = rw.Table.Columns.Contains("BunLayer") ? Int32.TryParse(rw["BunLayer"].ToString(), out bunLayer) ? bunLayer : -999 : -999,
                                           LayerPalet = rw.Table.Columns.Contains("LayerPalet") ? Int32.TryParse(rw["LayerPalet"].ToString(), out layerPallet) ? layerPallet : -999 : -999,
                                           BoxPalet = rw.Table.Columns.Contains("BoxPalet") ? Int32.TryParse(rw["BoxPalet"].ToString(), out boxPallet) ? boxPallet : -999 : -999,
                                           FactoryCode = _factoryCode,
                                           LastUpdate = DateTime.Now,
                                           UpdatedBy = _username,
                                           PdisStatus = "M"
                                       }).ToList();

                        #endregion Convert Datatable To MasterData

                        masterDatas.ForEach(x => x.Hardship = x.Hardship.Value != -999 ? x.Hardship : null);
                        masterDatas.ForEach(x => x.Bun = x.Bun.Value != -999 ? x.Bun : null);
                        masterDatas.ForEach(x => x.BunLayer = x.BunLayer.Value != -999 ? x.BunLayer : null);
                        masterDatas.ForEach(x => x.LayerPalet = x.LayerPalet.Value != -999 ? x.LayerPalet : null);
                        masterDatas.ForEach(x => x.BoxPalet = x.BoxPalet.Value != -999 ? x.BoxPalet : null);

                        numOfResults = masterDatas.Count;
                        var masterdataJson = JsonConvert.SerializeObject(masterDatas);
                        var masterdatasResult = JsonConvert.DeserializeObject<List<MasterData>>(_masterDataAPIRepository.UpdateMasterDatasFromExcelFile(_factoryCode, _username, masterdataJson, _Token));
                        exceptionMessage = $"Successfully recorded {masterdatasResult.Count} out of {numOfResults} masterdata.";
                    }
                    else
                    {
                        #region Convert Datatable To Routing

                        routings = (from rw in table.AsEnumerable()
                                    select new Routing()
                                    {
                                        Id = 0,
                                        MaterialNo = rw.Table.Columns.Contains("Material_No") ? rw["Material_No"].ToString() : null,
                                        SeqNo = rw.Table.Columns.Contains("Seq_No") ? Int32.TryParse(rw["Seq_No"].ToString(), out seqNO) ? seqNO : -999 : -999,
                                        Machine = rw.Table.Columns.Contains("Machine") ? rw["Machine"].ToString() : null,
                                        RemarkInprocess = rw.Table.Columns.Contains("RemarkInProcess") ? rw["RemarkInProcess"].ToString() : null,
                                        PaperWidth = rw.Table.Columns.Contains("PaperWidth") ? Int32.TryParse(rw["PaperWidth"].ToString(), out paperWidth) ? paperWidth : -999 : -999,
                                        CutNo = rw.Table.Columns.Contains("CutNo") ? Int32.TryParse(rw["CutNo"].ToString(), out cutNo) ? cutNo : -999 : -999,
                                        Trim = rw.Table.Columns.Contains("Trim") ? Int32.TryParse(rw["Trim"].ToString(), out trim) ? trim : -999 : -999,
                                        PercenTrim = rw.Table.Columns.Contains("PercenTrim") ? double.TryParse(rw["PercenTrim"].ToString(), out percenTrim) ? percenTrim : -999 : -999,
                                        Alternative1 = rw.Table.Columns.Contains("Alternative1") ? rw["Alternative1"].ToString() : null,
                                        Alternative2 = rw.Table.Columns.Contains("Alternative2") ? rw["Alternative2"].ToString() : null,
                                        Alternative3 = rw.Table.Columns.Contains("Alternative3") ? rw["Alternative3"].ToString() : null,
                                        Alternative4 = rw.Table.Columns.Contains("Alternative4") ? rw["Alternative4"].ToString() : null,
                                        Alternative5 = rw.Table.Columns.Contains("Alternative5") ? rw["Alternative5"].ToString() : null,
                                        FactoryCode = _factoryCode,
                                        UpdatedDate = DateTime.Now,
                                        UpdatedBy = _username,
                                        PdisStatus = "M"
                                    }).ToList();

                        #endregion Convert Datatable To Routing

                        routings.ForEach(x => x.SeqNo = x.SeqNo != -999 ? x.SeqNo : 0);
                        routings.ForEach(x => x.PaperWidth = x.PaperWidth.Value != -999 ? x.PaperWidth : null);
                        routings.ForEach(x => x.CutNo = x.CutNo.Value != -999 ? x.CutNo : null);
                        routings.ForEach(x => x.Trim = x.Trim.Value != -999 ? x.Trim : null);
                        routings.ForEach(x => x.PercenTrim = x.PercenTrim.Value != -999 ? x.PercenTrim : null);
                        routings.ForEach(x => x.PercenTrim = x.PercenTrim.Value != -999 ? x.PercenTrim : null);

                        numOfResults = routings.Count;

                        var routingsJson = JsonConvert.SerializeObject(routings);
                        var routingsResult = JsonConvert.DeserializeObject<List<Routing>>(_masterDataAPIRepository.UpdateRoutingsFromExcelFile(_factoryCode, _username, routingsJson, _Token));

                        exceptionMessage = $"Successfully recorded {routingsResult.Count} out of {numOfResults} routing.";
                    }
                }
            }
        }

        //Tassanai Update 10/7/2021
        public void GetMasterDataAddTag(ref MaintainAddTagCustomerModel addTagCustomerModel, string ddlSearch, string inputSerach)
        {
            if (inputSerach != "")
            {
                addTagCustomerModel.TagPrintSO = JsonConvert.DeserializeObject<List<TagPrintSo>>(_tagPrintSORepository.GetTagPrintSO(_factoryCode, _Token)).OrderBy(q => q.Id).Select(q => q.DataText).Distinct().ToList(); ;

                addTagCustomerModel.AddTagCustomerModel = JsonConvert.DeserializeObject<List<AddTagCustomerModel>>(_masterDataAPIRepository.GetMasterDataAllByKeySearchAddTag(ddlSearch, inputSerach, _factoryCode, _Token));
            }
        }

        public void UpdateTagMaterial(ref AddTagCustomerModel addTagCustomerModel)
        {
            //// จัดลำดับ Tag
            string[] TagBundlewords;
            string[] TagPalletwords;
            var tagBundle = "";
            var tagPallet = "";
            if (!String.IsNullOrEmpty(addTagCustomerModel.TagBundle))
            {
                TagBundlewords = addTagCustomerModel.TagBundle.Split(',');

                var TagBundlewordsorted = TagBundlewords.OrderBy(item => item[0]);
                tagBundle = string.Join(",", TagBundlewordsorted);
            }

            if (!String.IsNullOrEmpty(addTagCustomerModel.TagPallet))
            {
                TagPalletwords = addTagCustomerModel.TagPallet.Split(',');

                var TagPalletwordsorted = TagPalletwords.OrderBy(item => item[0]);
                //customer.TagBundle =
                tagPallet = string.Join(",", TagPalletwordsorted);
            }

            addTagCustomerModel.TagBundle = tagBundle;
            addTagCustomerModel.TagPallet = tagPallet;

            var existMasterdata = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNo(_factoryCode, addTagCustomerModel.MaterialNo, _Token));
            existMasterdata.NoTagBundle = addTagCustomerModel.NoTagBundle;
            existMasterdata.TagBundle = tagBundle;
            existMasterdata.TagPallet = tagPallet;
            existMasterdata.HeadTagBundle = addTagCustomerModel.HeadTagBundle;
            existMasterdata.HeadTagPallet = addTagCustomerModel.HeadTagPallet;
            existMasterdata.FootTagBundle = addTagCustomerModel.FootTagBundle;
            existMasterdata.FootTagPallet = addTagCustomerModel.FootTagPallet;
            existMasterdata.User = _username;

            var parentModelMasterData = new ParentModel()
            {
                AppName = Globals.AppNameEncrypt,
                SaleOrg = _saleOrg,
                PlantCode = _factoryCode,
                MasterData = existMasterdata
            };

            var jsonExistMasterdata = JsonConvert.SerializeObject(parentModelMasterData);
            _masterDataAPIRepository.UpdateMasterData(jsonExistMasterdata, _Token);
        }

        public void GetChangePalletSizeData(ref ChangePalletSizeViewModel changePalletSize, string materialNo)
        {
            changePalletSize.MasterDatas = new List<MasterData>();
            changePalletSize.StandardPatternNames = new List<StandardPatternName>();
            if (string.IsNullOrEmpty(materialNo))
            {
                //get masterdata by user "pallet:"
                changePalletSize.MasterDatas = JsonConvert.DeserializeObject<List<MasterData>>(_masterDataAPIRepository.GetMasterDataByUser(_factoryCode, "pallet:", _Token));
            }
            else
            {
                //get masterdatas by materialNo
                changePalletSize.MasterDatas.Add(JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNumberNonX(_factoryCode, materialNo, _Token)));
            }

            changePalletSize.StandardPatternNames = JsonConvert.DeserializeObject<List<StandardPatternName>>(standardPatternNameAPIRepository.GetAll(_factoryCode, _Token));
            changePalletSize.StandardPatternNames.ForEach(s => s.Picturepath = string.Join("", Regex.Split(Path.GetFileName(s.Picturepath), @"(?:\r\n|\n|\r)")));
        }

        public void UpdateMasterDataByChangePalletSize(MasterData masterData)
        {
            _masterDataAPIRepository.UpdateMasterDataByChangePalletSize(_factoryCode, _username, JsonConvert.SerializeObject(masterData), _Token);
        }

        public async Task<List<MasterDataRoutingModel>> GetMasterDataFromFile(List<IFormFile> fileUpload)
        {
            var masterDataRoutings = new List<MasterDataRoutingModel>();
            var materialNos = new List<string>();
            var result = new StringBuilder();

            foreach (var file in fileUpload)
            {
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    while (reader.Peek() >= 0)
                    {
                        var data = await reader.ReadLineAsync();
                        result.Append(data);
                        materialNos.Add(result.ToString().Trim());

                        result.Clear();
                    }
                }
            }

            masterDataRoutings = JsonConvert.DeserializeObject<List<MasterDataRoutingModel>>(_masterDataAPIRepository.GetMasterDataRoutingsByMaterialNos(_factoryCode, JsonConvert.SerializeObject(materialNos), _Token));
            masterDataRoutings = masterDataRoutings.OrderByDescending(x => x.LastUpdate).ToList();

            return masterDataRoutings;
        }

        public List<MasterData> GetMasterDataList(string MaterialNo, string PC)
        {
            return JsonConvert.DeserializeObject<List<MasterData>>(_masterDataAPIRepository.GetMasterDataList(MaterialNo, PC, _factoryCode, _Token));
        }

        public void ReSentMat(string materialno)
        {
            var existTransactionDetail = JsonConvert.DeserializeObject<TransactionsDetail>(transactionsDetailAPIRepository.GetTransactionsDetailByMat(_factoryCode, materialno, _Token));
            var existMasterdata = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNo(_factoryCode, materialno, _Token));

            var masterdata = new MasterData();
            if (existTransactionDetail != null && existTransactionDetail.Outsource && existTransactionDetail.MatSaleOrg != null && existMasterdata.HireFactory != null)
            {
                masterdata = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNo(existMasterdata.HireFactory, existTransactionDetail.MatSaleOrg, _Token));
            }
            else
            {
                masterdata = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNo(_factoryCode, materialno, _Token));
            }

            if (masterdata != null)
            {
                masterdata.TranStatus = false;
                masterdata.SapStatus = false;
                masterdata.PdisStatus = "C";
                masterdata.UpdatedBy = _username;
                masterdata.LastUpdate = DateTime.Now;
                var parentModelMasterData = new ParentModel()
                {
                    AppName = Globals.AppNameEncrypt,
                    SaleOrg = _saleOrg,
                    PlantCode = _factoryCode,
                    MasterData = masterdata
                };
                var jsonExistMasterdata = JsonConvert.SerializeObject(parentModelMasterData);
                _masterDataAPIRepository.UpdateMasterData(jsonExistMasterdata, _Token);
            }
        }
    }
}