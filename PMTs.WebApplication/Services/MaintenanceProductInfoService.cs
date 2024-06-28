using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using OfficeOpenXml;
using PMTs.DataAccess.ComplexModel;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView;
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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services
{
    [TraceAspect]
    public class MaintenanceProductInfoService : IMaintenanceProductInfoService
    {
        private readonly IMasterDataAPIRepository masterDataAPIRepository;
        private readonly IRoutingAPIRepository routingAPIRepository;
        private readonly ISalesViewAPIRepository salesViewAPIRepository;
        private readonly IPlantViewAPIRepository plantViewAPIRepository;
        private readonly ITransactionsDetailAPIRepository transactionsDetailAPIRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPPCRawMaterialProductionBomAPIRepository _ppcRawMaterialProductionBomAPIRepository;
        private readonly IPaperGradeAPIRepository _papergradeAPIRepository;

        private readonly string _username;
        private readonly string _saleOrg;
        private readonly string _factoryCode;
        private readonly string _token;

        public MaintenanceProductInfoService(
            IMasterDataAPIRepository masterDataAPIRepository,
            IRoutingAPIRepository routingAPIRepository,
            ISalesViewAPIRepository salesViewAPIRepository,
            IPlantViewAPIRepository plantViewAPIRepository,
            ITransactionsDetailAPIRepository transactionsDetailAPIRepository,
            IHttpContextAccessor httpContextAccessor,
            IPPCRawMaterialProductionBomAPIRepository ppcRawMaterialProductionBomAPIRepository,
            IPaperGradeAPIRepository papergradeAPIRepository
            )
        {
            this.masterDataAPIRepository = masterDataAPIRepository;
            this.routingAPIRepository = routingAPIRepository;
            this.salesViewAPIRepository = salesViewAPIRepository;
            this.plantViewAPIRepository = plantViewAPIRepository;
            this.transactionsDetailAPIRepository = transactionsDetailAPIRepository;
            this._ppcRawMaterialProductionBomAPIRepository = ppcRawMaterialProductionBomAPIRepository;
            _httpContextAccessor = httpContextAccessor;

            // Initialize User Data From Session
            var userSessionModel = SessionExtentions.GetSession<UserSessionModel>(_httpContextAccessor.HttpContext.Session, "UserSessionModel");
            if (userSessionModel != null)
            {
                _username = userSessionModel.UserName;
                _saleOrg = userSessionModel.SaleOrg;
                _factoryCode = userSessionModel.FactoryCode;
                _token = userSessionModel.Token;
            }
            _papergradeAPIRepository = papergradeAPIRepository;
        }

        public void DeleteProductViewList(string materialNo)
        {
            #region Set Model Update Master Data
            var existMasterData = JsonConvert.DeserializeObject<MasterData>(masterDataAPIRepository.GetMasterDataByMaterialNumberNonX(_factoryCode, materialNo, _token));
            existMasterData.PdisStatus = "X";
            existMasterData.TranStatus = false;
            existMasterData.LastUpdate = DateTime.Now;
            existMasterData.UpdatedBy = _username;
            existMasterData.User = _username;

            ParentModel MasterDataParent = new ParentModel();
            MasterDataParent.AppName = Globals.AppNameEncrypt;
            MasterDataParent.SaleOrg = _saleOrg;
            MasterDataParent.PlantCode = _factoryCode;
            MasterDataParent.MasterData = existMasterData;
            masterDataAPIRepository.UpdateMasterData(JsonConvert.SerializeObject(MasterDataParent), _token);
            #endregion

            #region Set Model Update SaleView
            var existSaleViews = JsonConvert.DeserializeObject<List<SalesView>>(salesViewAPIRepository.GetSaleViewsByMaterialNoAndFactoryCode(_factoryCode, materialNo, _token));
            var saleViewModel = new List<SalesView>();
            foreach (var existSaleView in existSaleViews)
            {
                existSaleView.PdisStatus = "X";
                existSaleView.TranStatus = false;
                ParentModel saleViewParentModel = new ParentModel();
                saleViewParentModel.AppName = Globals.AppNameEncrypt;
                saleViewParentModel.SaleOrg = _saleOrg;
                saleViewParentModel.PlantCode = _factoryCode;
                saleViewParentModel.SalesView = existSaleView;
                salesViewAPIRepository.UpdateSaleView(JsonConvert.SerializeObject(saleViewParentModel), _token);
            }
            #endregion

            #region Set Model Update PlantView
            var existPlantView = JsonConvert.DeserializeObject<PlantView>(plantViewAPIRepository.GetPlantViewByMaterialNo(_factoryCode, materialNo, _token));

            existPlantView.PdisStatus = "X";
            ParentModel plantViewParentModel = new ParentModel();
            plantViewParentModel.AppName = Globals.AppNameEncrypt;
            plantViewParentModel.SaleOrg = _saleOrg;
            plantViewParentModel.PlantCode = _factoryCode;
            plantViewParentModel.PlantView = existPlantView;
            plantViewAPIRepository.UpdatePlantView(JsonConvert.SerializeObject(plantViewParentModel), _token);
            #endregion

            #region Set Model Update Routing
            var existRoutings = JsonConvert.DeserializeObject<List<Routing>>(routingAPIRepository.GetRoutingsByMaterialNo(_factoryCode, materialNo, _token));
            var routings = new List<Routing>();

            foreach (var existRouting in existRoutings)
            {
                existRouting.PdisStatus = "X";
                routings.Add(existRouting);
            }
            routingAPIRepository.UpdateRoutingPDISStatus(_factoryCode, materialNo, "X", _token);
            // routingAPIRepository.UpdateRouting(_factoryCode,JsonConvert.SerializeObject(routings));

            #endregion

            #region Set Model Update TransactionDetail
            var existTransactionDetail = JsonConvert.DeserializeObject<TransactionsDetail>(transactionsDetailAPIRepository.GetTransactionsDetailByMat(_factoryCode, materialNo, _token));
            existTransactionDetail.PdisStatus = "X";

            ParentModel transactionDetailParentModel = new ParentModel();
            transactionDetailParentModel.AppName = Globals.AppNameEncrypt;
            transactionDetailParentModel.SaleOrg = _saleOrg;
            transactionDetailParentModel.PlantCode = _factoryCode;
            transactionDetailParentModel.TransactionsDetail = existTransactionDetail;
            transactionsDetailAPIRepository.UpdateTransactionsDetail(JsonConvert.SerializeObject(transactionDetailParentModel), _token);
            #endregion
        }

        public void ReadExcelFileToChangeBoardNewMaterial(ref List<ChangeBoardNewMaterial> changeBoardNewMaterials, List<IFormFile> fileUpload, bool checkImport)
        {
            var isInvalidData = false;
            var result = new List<ChangeBoardNewMaterial>();

            if (checkImport)
            {

                if (fileUpload.Count > 0)
                {
                    var fileExcel = fileUpload[0];

                    long size = fileExcel.Length;
                    if (size > 102400)
                    {
                        throw new Exception("The import file size limitation is 100KB.");
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
                            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["ChangeBoardNewMaterial_Template"];

                            //loop all rows
                            for (int i = worksheet.Dimension.Start.Row; i <= worksheet.Dimension.End.Row; i++)
                            {
                                var rowsObj = new object[worksheet.Dimension.End.Column];
                                isInvalidData = false;
                                var changeBoardNewProduct = new ChangeBoardNewMaterial();

                                //loop all columns in a row
                                for (int j = worksheet.Dimension.Start.Column; j <= worksheet.Dimension.End.Column && !isInvalidData; j++)
                                {
                                    if ((j != 2 && j != 5 && j != 6 && j != 7 && j != 8 && j != 9) && (worksheet.Cells[i, j].Value == null || worksheet.Cells[i, j].Value == ""))
                                    {
                                        isInvalidData = true;
                                    }


                                    if (i != 1 && i != 2)
                                    {
                                        var currentValue = isInvalidData || worksheet.Cells[i, j].Value == null || worksheet.Cells[i, j].Value == "" ? null : worksheet.Cells[i, j].Value.ToString();
                                        switch (j)
                                        {
                                            case 1:
                                                changeBoardNewProduct = new ChangeBoardNewMaterial();
                                                changeBoardNewProduct.CopyMaterialNo = currentValue;
                                                break;
                                            case 2:
                                                changeBoardNewProduct.PC = currentValue;
                                                break;
                                            case 3:
                                                changeBoardNewProduct.Flute = currentValue;
                                                break;
                                            case 4:
                                                changeBoardNewProduct.NewBoard = currentValue;
                                                break;
                                            case 5:
                                                changeBoardNewProduct.Price = currentValue;
                                                break;
                                            case 6:
                                                changeBoardNewProduct.HighValue = currentValue;
                                                break;
                                            case 7:
                                                changeBoardNewProduct.BoardAlternative = currentValue;
                                                break;
                                            case 8:
                                                changeBoardNewProduct.Change = currentValue;
                                                break;
                                            default:
                                                break;
                                        };

                                        if (j == worksheet.Dimension.End.Column && i != 1 && i != 2)
                                        {
                                            if (isInvalidData)
                                            {
                                                changeBoardNewProduct.IsCreatedSuccess = false;
                                            }
                                            else
                                            {
                                                changeBoardNewProduct.IsCreatedSuccess = true;
                                            }

                                            changeBoardNewMaterials.Add(changeBoardNewProduct);
                                        }
                                    }
                                }
                            }
                        }

                        var jsonData = JsonConvert.SerializeObject(changeBoardNewMaterials);

                        //send json data of board combine acc to update database with api.
                        result = JsonConvert.DeserializeObject<List<ChangeBoardNewMaterial>>(masterDataAPIRepository.CreateChangeBoardNewMaterial(_factoryCode, _username, checkImport, jsonData, _token));

                        if (result != null && result.Count > 0)
                        {
                            changeBoardNewMaterials = new List<ChangeBoardNewMaterial>();
                            changeBoardNewMaterials.AddRange(result);
                        }

                    }
                }
            }
            else
            {
                var jsonData = JsonConvert.SerializeObject(changeBoardNewMaterials);

                //send json data of board combine acc to update database with api.
                result = JsonConvert.DeserializeObject<List<ChangeBoardNewMaterial>>(masterDataAPIRepository.CreateChangeBoardNewMaterial(_factoryCode, _username, false, jsonData, _token));

                if (result != null && result.Count > 0)
                {
                    changeBoardNewMaterials = new List<ChangeBoardNewMaterial>();
                    changeBoardNewMaterials.AddRange(result);
                }
            }
        }

        public void ReadExcelFileToChangeFactoryNewMaterial(ref List<ChangeBoardNewMaterial> changeBoardNewMaterials, List<IFormFile> fileUpload, bool checkImport)
        {
            var isInvalidData = false;
            var result = new List<ChangeBoardNewMaterial>();

            if (checkImport)
            {

                if (fileUpload.Count > 0)
                {
                    var fileExcel = fileUpload[0];

                    long size = fileExcel.Length;
                    if (size > 102400)
                    {
                        throw new Exception("The import file size limitation is 100KB.");
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
                            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["ChangeFactoryNewMat_Template"];

                            //loop all rows
                            for (int i = worksheet.Dimension.Start.Row; i <= worksheet.Dimension.End.Row; i++)
                            {
                                var rowsObj = new object[worksheet.Dimension.End.Column];
                                isInvalidData = false;
                                var changeBoardNewProduct = new ChangeBoardNewMaterial();

                                //loop all columns in a row
                                for (int j = worksheet.Dimension.Start.Column; j <= worksheet.Dimension.End.Column && !isInvalidData; j++)
                                {
                                    if (j != 3 && j != 4 && (worksheet.Cells[i, j].Value == null || worksheet.Cells[i, j].Value == ""))
                                    {
                                        isInvalidData = true;
                                    }


                                    if (i != 1 && i != 2)
                                    {
                                        var currentValue = isInvalidData || worksheet.Cells[i, j].Value == null || worksheet.Cells[i, j].Value == "" ? null : worksheet.Cells[i, j].Value.ToString();
                                        switch (j)
                                        {
                                            case 1:
                                                changeBoardNewProduct = new ChangeBoardNewMaterial();
                                                changeBoardNewProduct.CopyFactoryCode = currentValue;
                                                break;
                                            case 2:
                                                changeBoardNewProduct.CopyMaterialNo = currentValue;
                                                break;
                                            case 3:
                                                changeBoardNewProduct.MaterialType = currentValue;
                                                break;
                                            default:
                                                break;
                                        };

                                        if (j == 4 && i != 1 && i != 2)
                                        {
                                            if (isInvalidData)
                                            {
                                                changeBoardNewProduct.IsCreatedSuccess = false;
                                            }
                                            else
                                            {
                                                changeBoardNewProduct.IsCreatedSuccess = true;
                                            }

                                            changeBoardNewMaterials.Add(changeBoardNewProduct);
                                        }
                                    }
                                }
                            }
                        }

                        var jsonData = JsonConvert.SerializeObject(changeBoardNewMaterials);

                        //send json data of board combine acc to update database with api.
                        result = JsonConvert.DeserializeObject<List<ChangeBoardNewMaterial>>(masterDataAPIRepository.CreateChangeFactoryNewMaterial(_factoryCode, _username, checkImport, jsonData, _token));

                        if (result != null && result.Count > 0)
                        {
                            changeBoardNewMaterials = new List<ChangeBoardNewMaterial>();
                            changeBoardNewMaterials.AddRange(result);
                        }

                    }
                }
            }
            else
            {
                var jsonData = JsonConvert.SerializeObject(changeBoardNewMaterials);

                //send json data of board combine acc to update database with api.
                result = JsonConvert.DeserializeObject<List<ChangeBoardNewMaterial>>(masterDataAPIRepository.CreateChangeFactoryNewMaterial(_factoryCode, _username, false, jsonData, _token));

                if (result != null && result.Count > 0)
                {
                    changeBoardNewMaterials = new List<ChangeBoardNewMaterial>();
                    changeBoardNewMaterials.AddRange(result);
                }
            }
        }
        public List<Routing> GetRoutingDataList(string MaterialNo)
        {
            return JsonConvert.DeserializeObject<List<Routing>>(routingAPIRepository.GetRoutingsByMaterialNoContain(_factoryCode, MaterialNo, _token)).ToList();
        }
        public List<PpcRawMaterialProductionBom> GetBomRawMatDataList(string MaterialNo)
        {
            return JsonConvert.DeserializeObject<List<PpcRawMaterialProductionBom>>(_ppcRawMaterialProductionBomAPIRepository.GetPPCRawMaterialProductionBOMsByFgMaterial(_factoryCode, MaterialNo, _token)).ToList();
        }
        public void ReplaceRoutingDataList(string MaterialFrom, string MaterialTo)
        {
            var routingFrom = JsonConvert.DeserializeObject<List<Routing>>(routingAPIRepository.GetRoutingsByMaterialNo(_factoryCode, MaterialFrom, _token));
            var bomRawMatFrom = JsonConvert.DeserializeObject<List<PpcRawMaterialProductionBom>>(_ppcRawMaterialProductionBomAPIRepository.GetPPCRawMaterialProductionBOMsByFgMaterial(_factoryCode, MaterialFrom, _token));
            var bomRawMatTo = JsonConvert.DeserializeObject<List<PpcRawMaterialProductionBom>>(_ppcRawMaterialProductionBomAPIRepository.GetPPCRawMaterialProductionBOMsByFgMaterial(_factoryCode, MaterialTo, _token));
            #region PreData

            var dateNow = DateTime.Now;
            if (routingFrom.Count > 0)
            {
                foreach (var item in routingFrom)
                {
                    item.MaterialNo = MaterialTo;
                    item.Id = 0;
                    item.CreatedDate = dateNow;
                    item.CreatedBy = _username;
                }
            }

            if (bomRawMatFrom.Count > 0)
            {
                foreach (var item in bomRawMatFrom)
                {
                    item.FgMaterial = MaterialTo;
                    item.Id = 0;
                    item.Plant = _factoryCode;
                    item.CreateDate = dateNow;
                    item.CreateBy = _username;
                    item.UpdateDate = dateNow;
                    item.UpdateBy = _username;
                }
            }
            #endregion
            try
            {
                routingAPIRepository.SaveRouting(_factoryCode, MaterialTo, JsonConvert.SerializeObject(routingFrom), _token);

                if (bomRawMatTo.Count > 0)
                {
                    _ppcRawMaterialProductionBomAPIRepository.DeleteManyRawMaterial(_factoryCode, JsonConvert.SerializeObject(bomRawMatTo), _token);
                }

                _ppcRawMaterialProductionBomAPIRepository.SaveRawMaterialProductionBoms(_factoryCode, JsonConvert.SerializeObject(bomRawMatFrom), _token);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void InitialDataChangeBoardNewMaterial(ref List<string> boards, ref List<Customer> customers, ref List<string> grades)
        {
            boards = JsonConvert.DeserializeObject<List<string>>(masterDataAPIRepository.GetBoardDistinctFromMasterData(_factoryCode, _token));
            customers = JsonConvert.DeserializeObject<List<Customer>>(masterDataAPIRepository.GetCustomerDistinctFromMasterData(_factoryCode, _token));
            grades = JsonConvert.DeserializeObject<List<string>>(_papergradeAPIRepository.GetGradeList(_factoryCode, _token));
        }

        public List<ChangeBoardNewMaterial> GetForTemplateChangeBoardNewMaterials(SearchMaterialTemplateParam param)
        {
            List<ChangeBoardNewMaterial> changeBoardNewMaterials = new List<ChangeBoardNewMaterial>();
            var jsonData = JsonConvert.SerializeObject(param);
            changeBoardNewMaterials = JsonConvert.DeserializeObject<List<ChangeBoardNewMaterial>>(masterDataAPIRepository.GetForTemplateChangeBoardNewMaterials(_factoryCode, jsonData, _token));
            return changeBoardNewMaterials;
        }
    }
}
