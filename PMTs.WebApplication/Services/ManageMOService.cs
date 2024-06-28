using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using OfficeOpenXml;
using PMTs.DataAccess.ComplexModel;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.ManageMO;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Shared;
using PMTs.DataAccess.Tracing;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services
{
    [TraceAspect]
    public class ManageMOService : IManageMOService
    {
        IHttpContextAccessor _httpContextAccessor;
        IHostingEnvironment _hostingEnvironment;
        private readonly IMapper mapper;
        private readonly IMoDataAPIRepository _moDataAPIRepository;
        private readonly IMoSpecAPIRepository _moSpecAPIRepository;
        private readonly IMoRoutingAPIRepository _moRoutingAPIRepository;
        private readonly IMasterDataAPIRepository _masterDataAPIRepository;
        private readonly IRoutingAPIRepository _routingAPIRepository;
        private readonly IRunningNoAPIRepository _runningNoAPIRepository;
        private readonly ICompanyProfileAPIRepository _companyProfileAPIRepository;
        private readonly IPMTsConfigAPIRepository _pMTsConfigAPIRepository;
        private readonly IAttachFileMOAPIRepository _attachFileMOAPIRepository;
        private readonly IFormulaAPIRepository _formulaAPIRepository;
        private readonly ICustomerAPIRepository _customerAPIRepository;
        private readonly IVMIServiceAPIRepository _vMIServiceAPIRepository;
        private readonly IMoBomRawMatAPIRepository _moBomRawMatAPIRepository;

        private readonly string _username;
        private readonly string _saleOrg;
        private readonly string _factoryCode;
        private readonly string _token;
        private readonly string _businessGroup;
        private readonly string bussGroupPPC = "Offset";

        public ManageMOService(IHttpContextAccessor httpContextAccessor,
            IHostingEnvironment hostingEnvironment,
            IMoDataAPIRepository moDataAPIRepository,
            IMoSpecAPIRepository moSpecAPIRepository,
            IMoRoutingAPIRepository moRoutingAPIRepository,
            IMasterDataAPIRepository masterDataAPIRepository,
            IRoutingAPIRepository routingAPIRepository,
            IRunningNoAPIRepository runningNoAPIRepository,
            ICompanyProfileAPIRepository companyProfileAPIRepository,
            IPMTsConfigAPIRepository pMTsConfigAPIRepository,
            IAttachFileMOAPIRepository attachFileMOAPIRepository,
            IFormulaAPIRepository formulaAPIRepository,
            ICustomerAPIRepository customerAPIRepository,
            IVMIServiceAPIRepository vMIServiceAPIRepository,
            IMoBomRawMatAPIRepository moBomRawMatAPIRepository,
            IMapper mapper
            )
        {
            _httpContextAccessor = httpContextAccessor;
            _hostingEnvironment = hostingEnvironment;
            _moDataAPIRepository = moDataAPIRepository;
            _moSpecAPIRepository = moSpecAPIRepository;
            _moRoutingAPIRepository = moRoutingAPIRepository;
            _masterDataAPIRepository = masterDataAPIRepository;
            _routingAPIRepository = routingAPIRepository;
            _runningNoAPIRepository = runningNoAPIRepository;
            _companyProfileAPIRepository = companyProfileAPIRepository;
            _pMTsConfigAPIRepository = pMTsConfigAPIRepository;
            _attachFileMOAPIRepository = attachFileMOAPIRepository;
            _formulaAPIRepository = formulaAPIRepository;
            _customerAPIRepository = customerAPIRepository;
            _vMIServiceAPIRepository = vMIServiceAPIRepository;
            _moBomRawMatAPIRepository = moBomRawMatAPIRepository;

            // Initialize UserData From Session
            var userSessionModel = SessionExtentions.GetSession<UserSessionModel>(_httpContextAccessor.HttpContext.Session, "UserSessionModel");
            if (userSessionModel != null)
            {
                _username = userSessionModel.UserName;
                _saleOrg = userSessionModel.SaleOrg;
                _factoryCode = userSessionModel.FactoryCode;
                _token = userSessionModel.Token;
                _businessGroup = userSessionModel.BusinessGroup;
            }

            this.mapper = mapper;
        }

        #region Function
        private static string ConvertStringToHex(String input, System.Text.Encoding encoding)
        {
            Byte[] stringBytes = encoding.GetBytes(input);
            StringBuilder sbBytes = new StringBuilder(stringBytes.Length * 2);
            foreach (byte b in stringBytes)
            {
                sbBytes.AppendFormat("{0:X2}", b);
            }
            return sbBytes.ToString();
        }

        private string ConvertHexToString(String hexInput, System.Text.Encoding encoding)
        {
            int numberChars = hexInput.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hexInput.Substring(i, 2), 16);
            }
            return encoding.GetString(bytes);
        }

        public string GenSaleOrderNumber()
        {
            var Running = JsonConvert.DeserializeObject<RunningNo>(_runningNoAPIRepository.GetRunningNoByGroupId(_factoryCode, "SO", _token));

            try
            {
                if (Running == null)
                {
                    var ex = new ArgumentNullException($"Running No does not exist.");
                    throw ex;
                }

                if (Running.Running >= Running.EndNo)
                {
                    var ex = new ArgumentOutOfRangeException(nameof(Running), $"Limited Running No. ,Please contact admin to correct.");
                    throw ex;
                }

                int so_no;
                string so_str, saleOrderNO;

                so_no = Running.Running + 1;
                so_str = Convert.ToString(so_no);
                so_str = so_str.PadLeft(Running.Length, '0');
                if (Running.UpdatedDate.HasValue && (Running.UpdatedDate.Value.Month != DateTime.Now.Month))
                {
                    so_str = "1".PadLeft(Running.Length, '0');
                }

                var fixStr = string.Empty;
                fixStr = Running.Fix + DateTime.Now.Year.ToString().Substring(2, 2) + DateTime.Now.Month.ToString().PadLeft(2, '0');
                saleOrderNO = fixStr + so_str;

                return saleOrderNO;
            }
            catch (Exception)
            {
                return "";
            }
        }

        private void UpdateRunningNo()
        {
            var Running = 0;

            var companyProfiles = new List<CompanyProfile>();
            companyProfiles = JsonConvert.DeserializeObject<List<CompanyProfile>>(_companyProfileAPIRepository.GetCompanyProfileBySaleOrg(_factoryCode, _saleOrg, _token));
            var runningNoOriginal = JsonConvert.DeserializeObject<RunningNo>(_runningNoAPIRepository.GetRunningNoByGroupId(_factoryCode, "SO", _token));

            if (runningNoOriginal != null)
            {
                Running = runningNoOriginal.Running + 1;
                if (runningNoOriginal.UpdatedDate.HasValue && (runningNoOriginal.UpdatedDate.Value.Month != DateTime.Now.Month))
                {
                    Running = 0;
                }
            }

            foreach (var companyProfile in companyProfiles)
            {
                // Get RunningNo
                var runningNoObject = JsonConvert.DeserializeObject<RunningNo>(_runningNoAPIRepository.GetRunningNoByGroupId(companyProfile.Plant, "SO", _token));

                if (runningNoObject != null)
                {
                    // Running Number
                    runningNoObject.Running = Running;
                    runningNoObject.UpdatedBy = _username;
                    runningNoObject.UpdatedDate = DateTime.Now;

                    //if (runningNoObject.UpdatedDate.HasValue && (runningNoObject.UpdatedDate.Value.Month != new DateTime().Month))
                    //{
                    //    runningNoObject.Running = 0;
                    //}

                    // Update RunningNo
                    ParentModel parentModelRunningNo = new ParentModel()
                    {
                        AppName = Globals.AppNameEncrypt,
                        SaleOrg = _saleOrg,
                        PlantCode = companyProfile.Plant,
                        RunningNo = runningNoObject
                    };

                    // Update RunningNo
                    _runningNoAPIRepository.UpdateRunningNo(JsonConvert.SerializeObject(parentModelRunningNo), _token);
                }
            }

        }

        private string CheckedCurrentSO(string orderItem)
        {
            var runningNO = new RunningNo();
            runningNO = JsonConvert.DeserializeObject<RunningNo>(_runningNoAPIRepository.GetRunningNoByGroupId(_factoryCode, "SO", _token));
            if (runningNO != null && !string.IsNullOrEmpty(orderItem))
            {
                if (Convert.ToInt32(orderItem.Substring(6, 4)) <= Convert.ToInt32(runningNO.Running))
                {
                    return GenSaleOrderNumber();
                }
            }

            return orderItem;
        }

        public List<SearchTypeModel> GetSearchType()
        {
            var ddlSearch = new List<SearchTypeModel>();

            ddlSearch.Add(new SearchTypeModel { SearchValue = "OrderItem", SearchDesc = "S/O Number" });
            ddlSearch.Add(new SearchTypeModel { SearchValue = "Material_No", SearchDesc = "Material No" });
            ddlSearch.Add(new SearchTypeModel { SearchValue = "PC", SearchDesc = "PC" });
            ddlSearch.Add(new SearchTypeModel { SearchValue = "Cust_Name", SearchDesc = "Customer Name" });
            ddlSearch.Add(new SearchTypeModel { SearchValue = "Sold_to", SearchDesc = "Sold To" });
            return ddlSearch;
        }

        #endregion

        #region Service
        public void GetMODataBySaleOrder(string saleOrder, ref ManageMOViewModel manageMOViewModel)
        {
            SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "MOManual_SaleOrderSearch", saleOrder);
            var modatas = JsonConvert.DeserializeObject<List<MoData>>(_moDataAPIRepository.GetMoDatasBySaleOrderNonX(_factoryCode, saleOrder, _token));

            manageMOViewModel.MoDatas = mapper.Map<List<MoDataViewModel>>(modatas);
        }
        public void GetMODatasBySearchType(string searchType, string searchText, ref ManageMOViewModel manageMOViewModel)
        {
            var modatas = JsonConvert.DeserializeObject<List<MoData>>(_moDataAPIRepository.GetMoDataListBySearchTypeNonX(_factoryCode, searchType, searchText, _token));
            manageMOViewModel.MoDatas = mapper.Map<List<MoDataViewModel>>(modatas);
        }
        public void GetMODatasBySaleOrder(string saleOrderStart, string saleOrderEnd, ref ManageMOViewModel manageMOViewModel)
        {
            var modatas = JsonConvert.DeserializeObject<List<MoDataViewModel>>(_moDataAPIRepository.GetMoDataListBySaleOrderNonX(_factoryCode, saleOrderStart, saleOrderEnd, _token));

            //manageMOViewModel.MoDatas = mapper.Map<List<MoDataViewModel>>(modatas);
            manageMOViewModel.MoDatas = modatas;
        }

        public void SearchAndCreateNewMODataByMaterialNo(string materialNumber, ref MoDataViewModel moData)
        {
            var existMasterdata = new MasterData();
            existMasterdata = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNumberNonNotX(_factoryCode, materialNumber, _token));

            if (existMasterdata == null)
            {
                throw new Exception($"Material No. {materialNumber} doesn't exist.");
            }
            else
            {
                Random r = new Random();
                int n = r.Next();

                moData.Name = existMasterdata.CustName;
                //moData.OrderItem = n.ToString();
                moData.OrderItem = GenSaleOrderNumber();
                moData.MaterialNo = existMasterdata.MaterialNo;
                var tempSoldToShipTo = existMasterdata.CusId.Length == 7 ? "000" + existMasterdata.CusId : existMasterdata.CusId;
                moData.SoldTo = tempSoldToShipTo;
                moData.ShipTo = tempSoldToShipTo;
                moData.MoStatus = "C";
                moData.PC = existMasterdata.Pc;
                if (string.IsNullOrEmpty(existMasterdata.CusId))
                {
                    throw new Exception("Error : Invalid CusId from masterdata.");
                }
                var customer = JsonConvert.DeserializeObject<Customer>(_customerAPIRepository.GetCustomerByCusIDAndCustName(_factoryCode, existMasterdata.CusId, existMasterdata.CustName, _token));
                moData.District = customer != null ? customer.District : string.Empty;
                moData.SaleText = existMasterdata.SaleText1 + existMasterdata.SaleText2 + existMasterdata.SaleText3 + existMasterdata.SaleText4;
            }
        }

        public void UpdateMOData(ref ManageMOViewModel manageMOViewModel, MoData moData, string action)
        {
            if (action != "delete")
            {
                moData.SentKiwi = false;
                moData.MoStatus = "M";
            }
            else
            {
                moData.MoStatus = "X";
            }

            //if (!moData.InterfaceTIPs.HasValue)
            //{
            //    moData.InterfaceTIPs = false;
            //}

            moData.InterfaceTips = false;
            moData.UpdatedDate = DateTime.Now;
            moData.UpdatedBy = _username;
            _moDataAPIRepository.UpdateMoData(JsonConvert.SerializeObject(moData), _token);

            var saleOrderSearch = SessionExtentions.GetSession<string>(_httpContextAccessor.HttpContext.Session, "MOManual_SaleOrderSearch");
            //var moDatas = JsonConvert.DeserializeObject<List<MoData>>(_moDataAPIRepository.GetMoDatasBySaleOrderNonX(_factoryCode, saleOrderSearch, _token));
            var moDatas = JsonConvert.DeserializeObject<List<MoData>>(_moDataAPIRepository.GetMoDataListBySaleOrderNonX(_factoryCode, moData.OrderItem, null, _token));
            manageMOViewModel.MoDatas = mapper.Map<List<MoDataViewModel>>(moDatas);
        }

        public void CreateNewMOData(ref ManageMOViewModel manageMOViewModel, MoData moData)
        {
            var moDatas = new List<MoData>();
            moData.FactoryCode = _factoryCode;
            moData.UpdatedBy = _username;
            moData.InterfaceTips = false;

            moDatas.Add(moData);
            moData = JsonConvert.DeserializeObject<List<MoData>>(_moDataAPIRepository.CreateMOManual(JsonConvert.SerializeObject(moDatas), _token)).FirstOrDefault();
            if (string.IsNullOrEmpty(moData.OrderItem))
            {
                throw new Exception($"Material No. {moData.MaterialNo} doesn't exist.");
            }

            if (bussGroupPPC.Equals(_businessGroup))
            {
                var calMoBomRawMat = JsonConvert.DeserializeObject<CalculateOffsetModel>(_formulaAPIRepository.GetCalculateMoTargetQuantityOffset(_factoryCode, moData.OrderQuant.ToString(), moData.MaterialNo, moData.OrderItem, _username, _token));
                if (calMoBomRawMat != null)
                {
                    var calMoData = calMoBomRawMat.MoData;
                    var newMoData = moData;
                    if (newMoData != null)
                    {
                        newMoData.TargetQuant = calMoData.TargetQuant;
                        newMoData.PrintRoundNo = calMoData.PrintRoundNo;
                        newMoData.AllowancePrintNo = calMoData.AllowancePrintNo;
                        newMoData.AfterPrintNo = calMoData.AfterPrintNo;
                        newMoData.DrawAmountNo = calMoData.DrawAmountNo;
                        newMoData.UpdatedDate = DateTime.Now;
                        newMoData.UpdatedBy = _username;
                        _moDataAPIRepository.UpdateMoData(JsonConvert.SerializeObject(newMoData), _token);
                    }

                    var moBomRawMat = calMoBomRawMat.moBomRawmats;
                    //moBomRawMat.ForEach(a => a.CreateBy = _username);
                    if (moBomRawMat.Count > 0)
                    {
                        _moBomRawMatAPIRepository.SaveMoBomRawMatsList(_factoryCode, JsonConvert.SerializeObject(moBomRawMat), _token);
                    }
                }
            }

            manageMOViewModel.MoDatas.Add(mapper.Map<MoDataViewModel>(moData));

            #region OLD code
            //var existMasterdata = new MasterData();
            //var routings = new List<Routing>();
            //var moSpec = new MoSpec();

            //existMasterdata = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNumberNonX(_factoryCode, moData.MaterialNo, _token));

            //if (existMasterdata == null)
            //{
            //    throw new Exception($"Material No. {moData.MaterialNo} doesn't exist.");
            //}
            //else
            //{
            //    moData.OrderItem = CheckedCurrentSO(moData.OrderItem);
            //    moSpec = mapper.Map<MoSpec>(existMasterdata);

            //    #region Save MO Data
            //    moData.Id = 0;
            //    moData.FactoryCode = _factoryCode;
            //    moData.Printed = 0;

            //    var culture = new CultureInfo("en-US");
            //    moData.DateTimeStamp = DateTime.Now.ToString("yMMddHHmmss", culture);
            //    moData.DueText = moData.DueDate.ToString("dd/MM/y", culture);
            //    moData.MoStatus = "C";
            //    //moData.SoldTo = existMasterdata.CusId;
            //    //moData.ShipTo = existMasterdata.CusId;
            //    moData.PlanStatus = null;
            //    moData.StockQty = null;
            //    moData.SentKIWI = false;
            //    moData.IsCreateManual = true;
            //    moData.UpdatedDate = DateTime.Now;
            //    moData.UpdatedBy = _username;

            //    ParentModel moDataparentModel = new ParentModel
            //    {
            //        AppName = Globals.AppNameEncrypt,
            //        SaleOrg = _saleOrg,
            //        PlantCode = _factoryCode,
            //        FactoryCode = _factoryCode,
            //        MoData = moData
            //    };

            //    //create new Mo data,
            //    _moDataAPIRepository.SaveMoData(JsonConvert.SerializeObject(moDataparentModel), _token);

            //    UpdateRunningNo();

            //    #endregion

            //    #region Save MO Spec
            //    moSpec.Id = 0;
            //    moSpec.User = _username;
            //    moSpec.OrderItem = moData.OrderItem;
            //    moSpec.LastUpdate = DateTime.Now;
            //    moSpec.CreateDate = DateTime.Now;

            //    ParentModel parentModel = new ParentModel
            //    {
            //        AppName = Globals.AppNameEncrypt,
            //        SaleOrg = _saleOrg,
            //        PlantCode = _factoryCode,
            //        FactoryCode = _factoryCode,
            //        MoSpec = moSpec
            //    };

            //    _moSpecAPIRepository.SaveMoSpec(JsonConvert.SerializeObject(parentModel), _token);

            //    #endregion

            //    #region Save MO Routing

            //    routings = JsonConvert.DeserializeObject<List<Routing>>(_routingAPIRepository.GetRoutingsByMaterialNo(_factoryCode, moData.MaterialNo, _token));
            //    var moRoutings = new List<MoRouting>();

            //    foreach (var routing in routings)
            //    {
            //        var moRouting = new MoRouting();
            //        moRouting = mapper.Map<MoRouting>(routing);
            //        moRouting.Id = 0;
            //        moRouting.Plant = _factoryCode;
            //        moRouting.FactoryCode = _factoryCode;
            //        moRouting.OrderItem = moData.OrderItem;
            //        moRoutings.Add(moRouting);
            //    }

            //    //save morouting
            //    if (moRoutings.Count > 0)
            //    {
            //        _moRoutingAPIRepository.SaveMORoutingsBySaleOrder(_factoryCode, moData.OrderItem, JsonConvert.SerializeObject(moRoutings), _token);
            //    }
            //    #endregion
            //manageMOViewModel.MoDatas.Add(mapper.Map<MoDataViewModel>(moData));
            //}
            #endregion
        }

        public void SaveAttachFileMOData(string orderItem, List<IFormFile> files, ref ManageMOViewModel manageMOViewModel)
        {
            var updateSuccess = false;
            var pmtsConfig = JsonConvert.DeserializeObject<PmtsConfig>(_pMTsConfigAPIRepository.GetPMTsConfigByFactoryName(_factoryCode, "ManageMO_Path", _token));
            var filePath = pmtsConfig != null ? pmtsConfig.FucValue : throw new Exception("Can't get file path.");
            var attachFiles = JsonConvert.DeserializeObject<List<AttachFileMo>>(_attachFileMOAPIRepository.GetAttachFileMOsByOrderItem(_factoryCode, orderItem, _token));
            var seqNo = 1;

            if (attachFiles.Count > 0)
            {
                seqNo = (attachFiles.OrderByDescending(a => a.SeqNo.Value).FirstOrDefault().SeqNo.Value) + 1;
            }

            //create folder if doesn't exist
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            #region Upload PDF to Server
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var GenarateFileName = orderItem + "_File_" + seqNo + ".pdf";
                    var fullPath = Path.Combine(filePath, GenarateFileName);

                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }

                    using (var localFile = System.IO.File.OpenWrite(fullPath))
                    {
                        using (var uploadedFile = file.OpenReadStream())
                        {
                            uploadedFile.CopyTo(localFile);

                            var attachFileObj = new AttachFileMo
                            {
                                Id = 0,
                                FactoryCode = _factoryCode,
                                OrderItem = orderItem,
                                PathInit = file.FileName,
                                PathNew = fullPath,
                                SeqNo = seqNo,
                                Status = true,
                                UpdatedBy = _username,
                                UpdatedDate = DateTime.Now,
                                CreatedBy = _username,
                                CreatedDate = DateTime.Now,
                            };

                            _attachFileMOAPIRepository.SaveAttachFileMO(JsonConvert.SerializeObject(attachFileObj), _token);
                            updateSuccess = true;
                        }
                    }

                    seqNo++;
                }
            }
            #endregion

            if (updateSuccess)
            {
                SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "MOManual_SaleOrderSearch", orderItem);
                //var modatas = JsonConvert.DeserializeObject<List<MoData>>(_moDataAPIRepository.GetMoDatasBySaleOrderNonX(_factoryCode, orderItem, _token));
                var modatas = JsonConvert.DeserializeObject<List<MoData>>(_moDataAPIRepository.GetMoDataListBySaleOrderNonX(_factoryCode, orderItem, null, _token));
                manageMOViewModel.MoDatas = mapper.Map<List<MoDataViewModel>>(modatas);
            }
        }

        public void DeleteAttachFileMOData(string orderItem, string fileName, string seqNO, ref ManageMOViewModel manageMOViewModel)
        {
            var updateSuccess = false;

            var attachFileObj = JsonConvert.DeserializeObject<AttachFileMo>(_attachFileMOAPIRepository.GetAttachFileMOByFileName(_factoryCode, orderItem, fileName, _token));
            if (attachFileObj != null)
            {
                attachFileObj.Status = false;
                if (attachFileObj.SeqNo.Equals(Convert.ToInt32(seqNO)))
                {
                    _attachFileMOAPIRepository.UpdateAttachFileMO(JsonConvert.SerializeObject(attachFileObj), _token);

                    if (File.Exists(attachFileObj.PathNew))
                    {
                        //delete file from local 
                        File.Delete(attachFileObj.PathNew);
                    }

                    updateSuccess = true;
                }
            }

            if (updateSuccess)
            {
                SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "MOManual_SaleOrderSearch", orderItem);
                //var modatas = JsonConvert.DeserializeObject<List<MoData>>(_moDataAPIRepository.GetMoDatasBySaleOrderNonX(_factoryCode, orderItem, _token));
                var modatas = JsonConvert.DeserializeObject<List<MoData>>(_moDataAPIRepository.GetMoDataListBySaleOrderNonX(_factoryCode, orderItem, null, _token));
                manageMOViewModel.MoDatas = mapper.Map<List<MoDataViewModel>>(modatas);
                var attachFileMOs = JsonConvert.DeserializeObject<List<AttachFileMo>>(_attachFileMOAPIRepository.GetAttachFileMOsByOrderItem(_factoryCode, orderItem, _token)).Where(a => a.Status == true).ToList();
                manageMOViewModel.AttachFileMOs = attachFileMOs.Count == 0 ? new List<AttachFileMo>() : attachFileMOs;
            }
        }

        public void GetAttachFileMO(string orderItem, ref ManageMOViewModel manageMOViewModel)
        {
            SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "MOManual_SaleOrderSearch", orderItem);
            //var modatas = JsonConvert.DeserializeObject<List<MoData>>(_moDataAPIRepository.GetMoDatasBySaleOrderNonX(_factoryCode, orderItem, _token));
            var modatas = JsonConvert.DeserializeObject<List<MoData>>(_moDataAPIRepository.GetMoDataListBySaleOrderNonX(_factoryCode, orderItem, null, _token));
            manageMOViewModel.MoDatas = mapper.Map<List<MoDataViewModel>>(modatas);
            var attachFileMOs = JsonConvert.DeserializeObject<List<AttachFileMo>>(_attachFileMOAPIRepository.GetAttachFileMOsByOrderItem(_factoryCode, orderItem, _token)).Where(a => a.Status == true).ToList();
            manageMOViewModel.AttachFileMOs = attachFileMOs.Count == 0 ? new List<AttachFileMo>() : attachFileMOs;
        }

        //tassanai update 22/7/2020
        public void GetMOSpec(string orderItem, ref MoSpec moSpec)
        {

            var existMasterData = JsonConvert.DeserializeObject<MoSpec>(_moSpecAPIRepository.GetMoSpecBySaleOrder(_factoryCode, orderItem, _token));
            moSpec = existMasterData;
        }

        public void UpdateMOSpec(string orderItem, string changeInfo)
        {
            var MOSpecData = JsonConvert.DeserializeObject<MoSpec>(_moSpecAPIRepository.GetMoSpecBySaleOrder(_factoryCode, orderItem, _token));



            ParentModel MOSpectParent = new ParentModel();
            MOSpectParent.AppName = Globals.AppNameEncrypt;
            MOSpectParent.SaleOrg = _saleOrg;
            MOSpectParent.PlantCode = _factoryCode;
            MOSpectParent.MoSpec = MOSpecData;
            string jsonString = JsonConvert.SerializeObject(MOSpectParent);

            _moSpecAPIRepository.UpdateMoSpecChangestring(_factoryCode, orderItem, changeInfo, _token);
        }

        public void ImportManualMOFromExcelFile(IFormFile file, ref ManageMOViewModel manageMOViewModel, ref string exceptionMessage)
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
                                        if (j == 2 && i != 1)
                                        {
                                            rowsObj[j - 1] = Convert.ToInt32(worksheet.Cells[i, j].Value);
                                        }
                                        else if ((j == 3 && i != 1) || (j == 4 && i != 1))
                                        {
                                            double? value = null;
                                            if (worksheet.Cells[i, j].Value != null)
                                            {
                                                value = Convert.ToDouble(worksheet.Cells[i, j].Value);
                                            }
                                            else
                                            {
                                                value = 0;
                                            }

                                            rowsObj[j - 1] = value;
                                        }
                                        else if (j == 5 && i != 1)
                                        {
                                            var strDate = worksheet.Cells[i, j].Value;
                                            if (strDate != null)
                                            {
                                                strDate = strDate.ToString().Split(" ")[0];
                                                var splitDate = strDate.ToString().Split("/");
                                                var day = Convert.ToInt32(splitDate[0]);
                                                var month = Convert.ToInt32(splitDate[1]);
                                                var year = Convert.ToInt32(splitDate[2]);
                                                rowsObj[j - 1] = new DateTime(year, month, day);
                                            }
                                        }
                                        else if (i != 1)
                                        {
                                            if (j == 1)
                                            {
                                                rowsObj[j - 1] = worksheet.Cells[i, j].Value != null ? worksheet.Cells[i, j].Value.ToString().ToUpper().Trim() : worksheet.Cells[i, j].Value;
                                            }
                                            else
                                            {
                                                rowsObj[j - 1] = worksheet.Cells[i, j].Value;
                                            }
                                        }
                                    }

                                    if (j == worksheet.Dimension.End.Column && i != 1)
                                    {
                                        table.Rows.Add(rowsObj);
                                    }
                                }
                            }

                            firstSheet = false;
                        }
                    }

                    var existMasterdata = new MasterData();
                    var routings = new List<Routing>();
                    var moSpec = new MoSpec();
                    var moDatas = new List<MoData>();
                    var result = new List<MoData>();

                    #region Convert Datatable To MO Data Model
                    moDatas = (from rw in table.AsEnumerable()
                               where !string.IsNullOrEmpty(rw["Material_No"].ToString())
                               select new MoData()
                               {
                                   MaterialNo = rw["Material_No"].ToString(),
                                   OrderQuant = Convert.ToInt32(rw["Order_Quant"]),
                                   ToleranceOver = Convert.ToDouble(rw["Tolerance_Over"]),
                                   ToleranceUnder = Convert.ToDouble(rw["Tolerance_Under"]),
                                   DueDate = Convert.ToDateTime(rw["Due_Date"]),
                                   ItemNote = Convert.ToString(rw["Item_Note"]),
                                   PoNo = Convert.ToString(rw["PO_No"]),
                                   Batch = Convert.ToString(rw["Batch"]),
                                   SoldTo = Convert.ToString(rw["Sold_to"]),
                                   ShipTo = Convert.ToString(rw["Ship_to"]),
                                   Morno = Convert.ToString(rw["MORNo"]),
                               }).ToList();

                    #endregion

                    moDatas.ForEach(m => m.UpdatedBy = _username);
                    moDatas.ForEach(m => m.FactoryCode = _factoryCode);
                    moDatas.ForEach(m => m.InterfaceTips = false);

                    result = JsonConvert.DeserializeObject<List<MoData>>(_moDataAPIRepository.CreateMOManual(JsonConvert.SerializeObject(moDatas), _token));

                    foreach (var moData in result)
                    {
                        if (string.IsNullOrEmpty(moData.OrderItem))
                        {
                            exceptionMessage = exceptionMessage + $"Material No. {moData.MaterialNo} doesn't exist.";
                        }

                        manageMOViewModel.MoDatas.Add(mapper.Map<MoDataViewModel>(moData));
                    }

                    #region Get And Set Manual MO Data Old
                    //foreach (var moData in modatas)
                    //{
                    //    existMasterdata = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNumberNonX(_factoryCode, moData.MaterialNo, _token));

                    //    if (existMasterdata == null)
                    //    {
                    //        exceptionMessage = exceptionMessage + $"Material No. {moData.MaterialNo} doesn't exist.";
                    //        manageMOViewModel.MoDatas.Add(mapper.Map<MoDataViewModel>(moData));
                    //    }
                    //    else
                    //    {
                    //        moSpec = mapper.Map<MoSpec>(existMasterdata);

                    //        #region Set MO Data
                    //        moData.Id = 0;
                    //        moData.FactoryCode = _factoryCode;
                    //        moData.Printed = 0;

                    //        var culture = new CultureInfo("en-US");
                    //        moData.DateTimeStamp = DateTime.Now.ToString("yMMddHHmmss", culture);
                    //        moData.DueText = moData.DueDate.ToString("dd/MM/y", culture);
                    //        moData.MoStatus = "C";
                    //        moData.PlanStatus = null;
                    //        moData.StockQty = null;
                    //        moData.SentKIWI = false;
                    //        moData.IsCreateManual = true;
                    //        moData.UpdatedDate = DateTime.Now;
                    //        moData.UpdatedBy = _username;
                    //        moData.OrderItem = GenSaleOrderNumber();
                    //        moData.Name = existMasterdata.CustName;

                    //        #endregion

                    //        #region Set MO Spec
                    //        moSpec.Id = 0;
                    //        moSpec.User = _username;
                    //        moSpec.OrderItem = moData.OrderItem;
                    //        moSpec.LastUpdate = DateTime.Now;
                    //        moSpec.CreateDate = DateTime.Now;

                    //        #endregion

                    //        #region Set MO Routing

                    //        routings = JsonConvert.DeserializeObject<List<Routing>>(_routingAPIRepository.GetRoutingsByMaterialNo(_factoryCode, moData.MaterialNo, _token));
                    //        var moRoutings = new List<MoRouting>();

                    //        foreach (var routing in routings)
                    //        {
                    //            var moRouting = new MoRouting();
                    //            moRouting = mapper.Map<MoRouting>(routing);
                    //            moRouting.Id = 0;
                    //            moRouting.Plant = _factoryCode;
                    //            moRouting.FactoryCode = _factoryCode;
                    //            moRouting.OrderItem = moData.OrderItem;
                    //            moRoutings.Add(moRouting);
                    //        }
                    //        #endregion

                    //        var moManual = new MOManualModel();
                    //        moManual.MoData = moData;
                    //        moManual.MoSpec = moSpec;
                    //        moManual.MoRoutings = moRoutings;

                    //        var moDataResult = JsonConvert.DeserializeObject<MoData>(_moDataAPIRepository.CreateMODataFromExcelFile(_factoryCode, _username, JsonConvert.SerializeObject(moManual), _token));
                    //        if (!String.IsNullOrEmpty(moDataResult.OrderItem))
                    //        {
                    //            UpdateRunningNo();
                    //        }
                    //        else
                    //        {
                    //            exceptionMessage = exceptionMessage + $"Material No. {moData.MaterialNo} can't create new MO manual.";
                    //        }

                    //        manageMOViewModel.MoDatas.Add(mapper.Map<MoDataViewModel>(moData));
                    //    }

                    //}

                    #endregion
                }
            }
        }

        public int CalculateMoTargetQuantity(string materialNO, string orderQuant, string toleranceOver)
        {
            var flute = string.Empty;
            string cut = "0";
            var existMasterData = JsonConvert.DeserializeObject<MasterData>(_masterDataAPIRepository.GetMasterDataByMaterialNumberNonX(_factoryCode, materialNO, _token));
            if (existMasterData != null)
            {
                flute = existMasterData.Flute;
            }

            var existRoutingCorr = JsonConvert.DeserializeObject<Routing>(_routingAPIRepository.GetRoutingByMaterialNoAndMachine(_factoryCode, materialNO, "COR", _token));
            if (existRoutingCorr != null)
            {
                cut = existRoutingCorr.CutNo.HasValue ? existRoutingCorr.CutNo.Value.ToString() : "0";
            }
            return Convert.ToInt32(_formulaAPIRepository.CalculateMoTargetQuantity(_factoryCode, orderQuant, toleranceOver, flute, materialNO, cut, _token));
        }

        #endregion


        #region [Edit block platen]
        public EditBlockPlatenModel GetBlockPlatenMaster(string material, string pc)
        {
            return JsonConvert.DeserializeObject<EditBlockPlatenModel>(_moDataAPIRepository.GetBlockPlatenMaster(_factoryCode, material, pc, _token));
        }
        public EditBlockPlatenModel GetBlockPlatenRouting(string material)
        {
            return JsonConvert.DeserializeObject<EditBlockPlatenModel>(_moDataAPIRepository.GetBlockPlatenRouting(_factoryCode, material, _token));
        }
        public string UpdateBlockPlatenRouting(List<EditBlockPlatenRouting> model)
        {
            return _moDataAPIRepository.UpdateBlockPlatenRouting(_factoryCode, _username, JsonConvert.SerializeObject(model), _token);
        }
        #endregion
    }
}
