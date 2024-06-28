using AspectInjector.Broker;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PMTs.DataAccess.ComplexModel;
using PMTs.DataAccess.Extentions;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.ProductCatalog;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Tracing;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PMTs.WebApplication.Services
{
    [TraceAspect]
    public class ProductCatalogService : IProductCatalogService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMasterDataAPIRepository _masterDataService;
        private readonly IRoutingAPIRepository _routingAPIRepository;
        private readonly IProductCatalogCofigRepository _productCatalogCofigRepository;
        private readonly ICompanyProfileAPIRepository _companyProfileAPIRepository;
        private readonly IBomStructAPIRepository _bomStructAPIRepository;
        private readonly IConfiguration configuration;
        private readonly IMaterialTypeAPIRepository _materialTypeAPIRepository;
        private readonly IEOrderingLogAPIRepository eOrderingLogAPIRepository;

        private string _username;
        private string _saleOrg;
        private string _factoryCode;
        private int _roldId;
        private IEnumerable<object> tmp;
        private readonly string _token;

        public ProductCatalogService(IHttpContextAccessor httpContextAccessor,
            IMasterDataAPIRepository masterDataService,
            IRoutingAPIRepository routingAPIRepository,
            IProductCatalogCofigRepository productCatalogCofigRepository,
            ICompanyProfileAPIRepository companyProfileAPIRepository,
            IBomStructAPIRepository bomStructAPIRepository,
            IMaterialTypeAPIRepository materialTypeAPIRepository,
            IConfiguration configuration,
            IEOrderingLogAPIRepository eOrderingLogAPIRepository
           )
        {
            _httpContextAccessor = httpContextAccessor;

            _masterDataService = masterDataService;
            _routingAPIRepository = routingAPIRepository;
            _productCatalogCofigRepository = productCatalogCofigRepository;
            _companyProfileAPIRepository = companyProfileAPIRepository;
            _bomStructAPIRepository = bomStructAPIRepository;
            this.configuration = configuration;
            _materialTypeAPIRepository = materialTypeAPIRepository;
            this.eOrderingLogAPIRepository = eOrderingLogAPIRepository;

            var userSessionModel = SessionExtentions.GetSession<UserSessionModel>(_httpContextAccessor.HttpContext.Session, "UserSessionModel");
            if (userSessionModel != null)
            {
                _username = userSessionModel.UserName;
                _saleOrg = userSessionModel.SaleOrg;
                _factoryCode = userSessionModel.FactoryCode;
                _roldId = userSessionModel.DefaultRoleId;
                _token = userSessionModel.Token;
            }
        }

        public ProductCatalogModel GetProductCatalogFrist()
        {
            ProductCatalogModel model = new ProductCatalogModel();
            model.columnNames = model.ProductCatalog.GetType().GetProperties().Select(k => k.Name).ToList();
            model.columnNamesInModal = model.columnNames.OrderByDescending(x => x).ToList();
            model.productCatalogConfigs = JsonConvert.DeserializeObject<List<ProductCatalogConfig>>(_productCatalogCofigRepository.GetProductGroupList(_factoryCode, _username, string.Empty, _token));
            return model;
        }

        public ProductCatalogModel GetProductCatalog(ProductCatalogsSearch dataSearch)
        {
            ProductCatalogModel model = new ProductCatalogModel();

            var masterProductTemp = JsonConvert.DeserializeObject<List<MasterDataQuery>>(_masterDataService.GetMasterProductCatalog(_factoryCode, JsonConvert.SerializeObject(dataSearch), _token));
            var masterProductTempCount = JsonConvert.DeserializeObject<string>(_masterDataService.GetCountProductCatalogNotop(_factoryCode, JsonConvert.SerializeObject(dataSearch), _token));
            model.RecordCount = string.IsNullOrEmpty(masterProductTempCount) ? "0" : masterProductTempCount;
            string condition = "";
            foreach (var item in masterProductTemp)
            {
                string mat = "'" + item.MaterialNo + "',";
                condition = condition + mat;
            }

            if (!string.IsNullOrEmpty(condition))
            { condition = condition.Substring(0, condition.Length - 1); }
            List<string> conditionlist = new List<string>();
            conditionlist.Add(condition);

            string factorycodeCheckRole = dataSearch.Role == "4" ? dataSearch.FactoryCode : "'" + _factoryCode + "'";
            var temp = JsonConvert.DeserializeObject<List<Routing>>(_routingAPIRepository.GetDapperRoutingByMatList(factorycodeCheckRole, JsonConvert.SerializeObject(conditionlist), _token));

            var ConvertRouting = temp.OrderBy(x => x.SeqNo).GroupBy(cc => cc.MaterialNo)
                     .Select(dd => new
                     {
                         Material = dd.Key,
                         Machine = string.Join("-", dd.Where(x => !string.IsNullOrEmpty(x.Machine)).Select(ee => ee.Machine).ToList()),
                         Alternative1 = string.Join(",", dd.Where(x => !string.IsNullOrEmpty(x.Alternative1)).Select(ee => ee.Alternative1).ToList()),
                         PlateNo = string.Join(",", dd.Where(x => !string.IsNullOrEmpty(x.PlateNo)).Select(ee => ee.PlateNo).ToList()),
                         MylaNo = string.Join(",", dd.Where(x => !string.IsNullOrEmpty(x.MylaNo)).Select(ee => ee.MylaNo).ToList()),

                         Color1 = string.Join(",", dd.Where(x => !string.IsNullOrEmpty(x.Color1)).Select(ee => ee.Color1 + "-" + ee.Shade1).ToList()),
                         Color2 = string.Join(",", dd.Where(x => !string.IsNullOrEmpty(x.Color2)).Select(ee => ee.Color2 + "-" + ee.Shade2).ToList()),
                         Color3 = string.Join(",", dd.Where(x => !string.IsNullOrEmpty(x.Color3)).Select(ee => ee.Color3 + "-" + ee.Shade3).ToList()),
                         Color4 = string.Join(",", dd.Where(x => !string.IsNullOrEmpty(x.Color4)).Select(ee => ee.Color4 + "-" + ee.Shade4).ToList()),
                         Color5 = string.Join(",", dd.Where(x => !string.IsNullOrEmpty(x.Color5)).Select(ee => ee.Color5 + "-" + ee.Shade5).ToList()),
                         Color6 = string.Join(",", dd.Where(x => !string.IsNullOrEmpty(x.Color6)).Select(ee => ee.Color6 + "-" + ee.Shade6).ToList()),
                         Color7 = string.Join(",", dd.Where(x => !string.IsNullOrEmpty(x.Color7)).Select(ee => ee.Color7 + "-" + ee.Shade7).ToList()),

                         TearTape = string.Join(",", dd.Select(ee => ee.TearTape == true ? "True" : "False").ToList()),
                         BlockNo = string.Join(",", dd.Where(x => !string.IsNullOrEmpty(x.BlockNo)).Select(ee => ee.BlockNo).ToList()),
                         RemarkInprocess = string.Join(",", dd.Where(x => !string.IsNullOrEmpty(x.RemarkInprocess)).Select(ee => ee.RemarkInprocess).ToList()),
                         BlockNo2 = string.Join(",", dd.Where(x => !string.IsNullOrEmpty(x.BlockNo2)).Select(ee => ee.BlockNo2 + "/" + ee.MylaNo2).ToList()),
                         BlockNo3 = string.Join(",", dd.Where(x => !string.IsNullOrEmpty(x.BlockNo3)).Select(ee => ee.BlockNo3 + "/" + ee.MylaNo3).ToList()),
                         BlockNo4 = string.Join(",", dd.Where(x => !string.IsNullOrEmpty(x.BlockNo4)).Select(ee => ee.BlockNo4 + "/" + ee.MylaNo4).ToList()),
                         BlockNo5 = string.Join(",", dd.Where(x => !string.IsNullOrEmpty(x.BlockNo5)).Select(ee => ee.BlockNo5 + "/" + ee.MylaNo5).ToList()),
                         No_Open_In = dd.OrderBy(x => x.SeqNo).Select(z => z.NoOpenIn).FirstOrDefault()
                     });

            var tempFinalConvert = ConvertRouting.ToList(); //ConvertRouting.Where(x => x.BlockNo.Contains(dataSearch.blockNo) || x.PlateNo.Contains(dataSearch.plateNo)).ToList();

            var ccc = (from e in masterProductTemp
                       join d in tempFinalConvert
                       on e.MaterialNo equals d.Material
                       into ps
                       from d in ps.DefaultIfEmpty()
                       select new { e, d }).ToList();

            List<ProductCatalogs> tmp = new List<ProductCatalogs>();
            int rec = 0;
            try
            {
                foreach (var item in ccc)
                {
                    rec++;
                    if (rec > 0)
                    {
                        ProductCatalogs product = new ProductCatalogs();
                        product.Remark = item.e.Remark;
                        product.Pc = item.e.Pc;
                        product.MaterialNo = item.e.MaterialNo;
                        product.PartNo = item.e.PartNo;
                        product.Hierarchy = item.e.Hierarchy;
                        product.SaleOrg = item.e.SaleOrg;
                        product.Plant = item.e.Plant;
                        product.CustCode = item.e.CustCode;
                        product.CusId = item.e.CusId;
                        product.CustName = item.e.CustName;
                        product.Description = item.e.Description.Trim();
                        product.SaleText = item.e.SaleText;
                        product.Change = item.e.Change;
                        product.IndGrp = item.e.IndGrp;
                        product.IndDes = item.e.IndDes;
                        product.MaterialType = item.e.MaterialType;
                        product.PrintMethod = item.e.PrintMethod;
                        product.Flute = item.e.Flute;
                        product.Board = item.e.Board;
                        product.Wid = item.e.Wid;
                        product.Leg = item.e.Leg;
                        product.Hig = item.e.Hig;
                        product.BoxType = item.e.BoxType;
                        product.JoinType = item.e.JoinType;
                        product.CutSheetLeng = item.e.CutSheetLeng;
                        product.CutSheetWid = item.e.CutSheetWid;
                        product.Bun = item.e.Bun;
                        product.BunLayer = item.e.BunLayer;
                        product.LayerPalet = item.e.LayerPalet;
                        product.BoxPalet = item.e.BoxPalet;
                        product.WeightBox = item.e.WeightBox;
                        product.SaleUom = item.e.SaleUom;
                        product.BomUom = item.e.BomUom;
                        product.PalletSize = item.e.PalletSize;
                        product.LastUpdate = item.e.LastUpdate;
                        product.PurTxt = item.e.PurTxt;
                        product.HighGroup = item.e.HighGroup;
                        product.HighValue = item.e.HighValue;
                        product.ValidityStartFrom = item.e.ValidityStartFrom;
                        product.ValidityEndDate = item.e.ValidityEndDate;
                        product.ConditionNumber = item.e.ConditionNumber;
                        product.Rate = item.e.Rate;
                        product.CurrencyKey = item.e.CurrencyKey;
                        product.Vendorname = item.e.Vendorname;
                        product.Netprice = item.e.Netprice;
                        product.Unitofnetprice = item.e.Unitofnetprice;
                        product.SourcelistValidfrom = item.e.SourcelistValidfrom;
                        product.SourcelistValidto = item.e.SourcelistValidto;
                        product.Nonmove = item.e.Nonmove;
                        product.NonMoveMonth = item.e.NonMoveMonth;
                        product.StockQA = item.e.StockQA;
                        product.StockFG = item.e.StockFG;
                        product.StockWIP = item.e.StockWIP;
                        product.Machine = item.d == null ? "" : item.d.Machine;
                        product.Alternative1 = item.d == null ? "" : item.d.Alternative1;
                        product.PlateNo = item.d == null ? "" : item.d.PlateNo;
                        product.MylaNo = item.d == null ? "" : item.d.MylaNo;

                        string tmpcolor = item.d == null ? "" : ((string.IsNullOrEmpty(item.d.Color1.Trim()) ? "" : "/" + item.d.Color1) + (string.IsNullOrEmpty(item.d.Color2.Trim()) ? "" : "/" + item.d.Color2) + (string.IsNullOrEmpty(item.d.Color3.Trim()) ? "" : "/" + item.d.Color3) + (string.IsNullOrEmpty(item.d.Color4.Trim()) ? "" : "/" + item.d.Color4) + (string.IsNullOrEmpty(item.d.Color5.Trim()) ? "" : "/" + item.d.Color5) + (string.IsNullOrEmpty(item.d.Color6.Trim()) ? "" : "/" + item.d.Color6) + (string.IsNullOrEmpty(item.d.Color7.Trim()) ? "" : "/" + item.d.Color7));
                        if (tmpcolor != "")
                        {
                            if (tmpcolor.Substring(0, 1) == "/")
                            {
                                tmpcolor = tmpcolor.Substring(1, tmpcolor.Length - 1);
                            }
                        }
                        product.Color = tmpcolor;
                        product.TearTape = item.d == null ? "" : item.d.TearTape;
                        product.BlockNo = item.d == null ? "" : item.d.BlockNo;
                        product.RemarkInprocess = item.d == null ? "" : item.d.RemarkInprocess;
                        product.BlockNo2 = item.d == null ? "" : item.d.BlockNo2;
                        product.BlockNo3 = item.d == null ? "" : item.d.BlockNo3;
                        product.BlockNo4 = item.d == null ? "" : item.d.BlockNo4;
                        product.BlockNo5 = item.d == null ? "" : item.d.BlockNo5;
                        product.NoOpenIn = item.d == null ? null : item.d.No_Open_In;

                        product.Hold = item.e.Hold;
                        product.PieceSet = item.e.PieceSet;
                        product.HoldRemark = item.e.HoldRemark;
                        product.FactoryCode = item.e.FactoryCode;

                        product.CustInvType = item.e.CustInvType;
                        product.CIPInvType = item.e.CIPInvType;
                        product.PdisStatus = item.e.PdisStatus;
                        tmp.Add(product);
                    }
                }
            }
            catch (Exception ex)
            {
                int ddd = rec;
            }

            if (!string.IsNullOrEmpty(dataSearch.blockNo) && !string.IsNullOrEmpty(dataSearch.plateNo))
            {
                model.productCatalogs = tmp.Where(x => x.BlockNo.Contains(dataSearch.blockNo) && x.PlateNo.Contains(dataSearch.plateNo)).ToList();
            }
            else if (!string.IsNullOrEmpty(dataSearch.blockNo))
            {
                model.productCatalogs = tmp.Where(x => x.BlockNo.Contains(dataSearch.blockNo)).ToList();
            }
            else if (!string.IsNullOrEmpty(dataSearch.plateNo))
            {
                model.productCatalogs = tmp.Where(x => x.PlateNo.Contains(dataSearch.plateNo)).ToList();
            }
            else
            {
                model.productCatalogs = tmp.ToList();
            }

            //model.productCatalogs = (from e in masterProductTemp
            //                         join d in tempFinalConvert
            //                         on e.MaterialNo equals d.Material
            //                         into ps from d in ps.DefaultIfEmpty()
            //                         select new ProductCatalogs
            //                         {
            //                             Remark = e.Remark,
            //                             Pc = e.Pc,

            //                             MaterialNo = e.MaterialNo,
            //                             PartNo = e.PartNo,

            //                             Hierarchy = e.Hierarchy,
            //                             SaleOrg = e.SaleOrg,
            //                             Plant = e.Plant,
            //                             CustCode = e.CustCode,
            //                             CusId = e.CusId,
            //                             CustName = e.CustName,
            //                             Description = e.Description,
            //                             SaleText = e.SaleText,
            //                             //SaleText2 =e.SaleText2,
            //                             //SaleText3 =e.SaleText3,
            //                             //SaleText4 = e.SaleText4,
            //                             Change = e.Change,
            //                             IndGrp = e.IndGrp,
            //                             IndDes = e.IndDes,
            //                             MaterialType = e.MaterialType,
            //                             PrintMethod = e.PrintMethod,
            //                             Flute = e.Flute,
            //                             Board = e.Board,
            //                             Wid = e.Wid,
            //                             Leg = e.Leg,
            //                             Hig = e.Hig,
            //                             BoxType = e.BoxType,
            //                             JoinType = e.JoinType,
            //                             CutSheetLeng = e.CutSheetLeng,
            //                             CutSheetWid = e.CutSheetWid,
            //                             Bun = e.Bun,
            //                             BunLayer = e.BunLayer,
            //                             LayerPalet = e.LayerPalet,
            //                             BoxPalet = e.BoxPalet,
            //                             WeightBox = e.WeightBox,
            //                             SaleUom = e.SaleUom,
            //                             BomUom = e.BomUom,
            //                             PalletSize = e.PalletSize,
            //                             LastUpdate = e.LastUpdate,
            //                             PurTxt = e.PurTxt,
            //                             //PurTxt2 = e.PurTxt2,
            //                             //PurTxt3=e.PurTxt3,
            //                             //PurTxt4=e.PurTxt4,
            //                             HighGroup = e.HighGroup,
            //                             HighValue = e.HighValue,

            //                             ValidityStartFrom = e.ValidityStartFrom,
            //                             ValidityEndDate = e.ValidityEndDate,
            //                             ConditionNumber = e.ConditionNumber,
            //                             Rate = e.Rate,
            //                             CurrencyKey = e.CurrencyKey,

            //                             Vendorname = e.Vendorname,
            //                             Netprice = e.Netprice,
            //                             Unitofnetprice = e.Unitofnetprice,
            //                             SourcelistValidfrom = e.SourcelistValidfrom,
            //                             SourcelistValidto = e.SourcelistValidto,

            //                             Nonmove = e.Nonmove,
            //                             NonMoveMonth = e.NonMoveMonth,
            //                             StockQA = e.StockQA,
            //                             StockFG = e.StockFG,
            //                             StockWIP = e.StockWIP,

            //                             Machine = d == null?"": d.Machine,
            //                  Alternative1= d == null ? "" : d.Alternative1,
            //                  PlateNo= d == null ? "" : d.PlateNo,
            //                  MylaNo= d == null ? "" : d.MylaNo,
            //                  //Color = d == null ? "" : ((string.IsNullOrEmpty(d.Color1.Trim()) ? "" : "/" + d.Color1) + (string.IsNullOrEmpty(d.Color2.Trim()) ? "" : "/" + d.Color2) + (string.IsNullOrEmpty(d.Color3.Trim()) ? "" : "/" + d.Color3) + (string.IsNullOrEmpty(d.Color4.Trim()) ? "" : "/" + d.Color4) + (string.IsNullOrEmpty(d.Color5.Trim()) ? "" : "/" + d.Color5) + (string.IsNullOrEmpty(d.Color6.Trim()) ? "" : "/" + d.Color6) + (string.IsNullOrEmpty(d.Color7.Trim()) ? "" : "/" + d.Color7)).Substring(0, 1) == "/" ?
            //                  //((string.IsNullOrEmpty(d.Color1.Trim()) ? "" : "/" + d.Color1) + (string.IsNullOrEmpty(d.Color2.Trim()) ? "" : "/" + d.Color2) + (string.IsNullOrEmpty(d.Color3.Trim()) ? "" : "/" + d.Color3) + (string.IsNullOrEmpty(d.Color4.Trim()) ? "" : "/" + d.Color4) + (string.IsNullOrEmpty(d.Color5.Trim()) ? "" : "/" + d.Color5) + (string.IsNullOrEmpty(d.Color6.Trim()) ? "" : "/" + d.Color6) + (string.IsNullOrEmpty(d.Color7.Trim()) ? "" : "/" + d.Color7)).Substring(1, ((string.IsNullOrEmpty(d.Color1.Trim()) ? "" : "/" + d.Color1) + (string.IsNullOrEmpty(d.Color2.Trim()) ? "" : "/" + d.Color2) + (string.IsNullOrEmpty(d.Color3.Trim()) ? "" : "/" + d.Color3) + (string.IsNullOrEmpty(d.Color4.Trim()) ? "" : "/" + d.Color4) + (string.IsNullOrEmpty(d.Color5.Trim()) ? "" : "/" + d.Color5) + (string.IsNullOrEmpty(d.Color6.Trim()) ? "" : "/" + d.Color6) + (string.IsNullOrEmpty(d.Color7.Trim()) ? "" : "/" + d.Color7)).Length - 1)
            //                  //: ((string.IsNullOrEmpty(d.Color1.Trim()) ? "" : "/" + d.Color1) + (string.IsNullOrEmpty(d.Color2.Trim()) ? "" : "/" + d.Color2) + (string.IsNullOrEmpty(d.Color3.Trim()) ? "" : "/" + d.Color3) + (string.IsNullOrEmpty(d.Color4.Trim()) ? "" : "/" + d.Color4) + (string.IsNullOrEmpty(d.Color5.Trim()) ? "" : "/" + d.Color5) + (string.IsNullOrEmpty(d.Color6.Trim()) ? "" : "/" + d.Color6) + (string.IsNullOrEmpty(d.Color7.Trim()) ? "" : "/" + d.Color7))
            //                  //,
            //                  TearTape = d == null ? "" : d.TearTape,
            //                  BlockNo= d == null ? "" : d.BlockNo,
            //                  RemarkInprocess = d == null ? "" : d.RemarkInprocess,
            //                  BlockNo2= d == null ? "" : d.BlockNo2,
            //                  BlockNo3= d == null ? "" : d.BlockNo3,
            //                  BlockNo4= d == null ? "" : d.BlockNo4,
            //                  BlockNo5= d == null ? "" : d.BlockNo5,
            //                  NoOpenIn = d == null ? null : d.No_Open_In

            //              }).ToList();

            //foreach (object item in model.productCatalogs)
            //{
            //    model.columnNames = item.GetType().GetProperties().OrderBy(k => k.Name).Select(k => k.Name).ToList();
            //    model.columnNames = model.columnNames.OrderByDescending(x => x).ToList();
            //    break;
            //}
            model.columnNames = model.ProductCatalog.GetType().GetProperties().Select(k => k.Name).ToList();
            model.columnNamesInModal = model.columnNames.Where(x => x != "Hold").OrderByDescending(x => x).ToList();
            model.productCatalogConfigs = JsonConvert.DeserializeObject<List<ProductCatalogConfig>>(_productCatalogCofigRepository.GetProductGroupList(_factoryCode, _username, string.Empty, _token));
            return model;
        }

        public void SaveProductCatalogConfig(string[] arrayColumn)
        {
            List<ProductCatalogConfig> proConfig = new List<ProductCatalogConfig>();
            ProductCatalogConfig fristcol = new ProductCatalogConfig();
            fristcol.Id = 0;
            fristcol.UserName = _username;
            fristcol.FactoryCode = _factoryCode;
            fristcol.ColumnName = "Action";
            ProductCatalogConfig secondcol = new ProductCatalogConfig();
            secondcol.Id = 0;
            secondcol.UserName = _username;
            secondcol.FactoryCode = _factoryCode;
            secondcol.ColumnName = "Bom";
            ProductCatalogConfig threecol = new ProductCatalogConfig();
            threecol.Id = 0;
            threecol.UserName = _username;
            threecol.FactoryCode = _factoryCode;
            threecol.ColumnName = "Hold";
            proConfig.Add(fristcol);
            proConfig.Add(secondcol);
            proConfig.Add(threecol);

            if (arrayColumn != null)
            {
                foreach (string s in arrayColumn)
                {
                    ProductCatalogConfig tmp = new ProductCatalogConfig();
                    tmp.Id = 0;
                    tmp.UserName = _username;
                    tmp.FactoryCode = _factoryCode;
                    tmp.ColumnName = s;
                    proConfig.Add(tmp);
                }
            }
            _productCatalogCofigRepository.UpdateroductGroupList(_factoryCode, _username, JsonConvert.SerializeObject(proConfig), _token);
        }

        public void SaveProductCatalogRemark(ProductCatalogRemark Model)
        {
            //List<ProductCatalogConfig> proConfig = new List<ProductCatalogConfig>();
            //foreach (string s in arrayColumn)
            //{
            //    ProductCatalogConfig tmp = new ProductCatalogConfig();
            //    tmp.Id = 0;
            //    tmp.UserName = _username;
            //    tmp.FactoryCode = _factoryCode;
            //    tmp.ColumnName = s;
            //    proConfig.Add(tmp);
            //}
            Model.MaterialNo = _username;

            _productCatalogCofigRepository.UpdateProductCatalogRemark(_factoryCode, JsonConvert.SerializeObject(Model), _token);
        }

        public ProductCatalogRemark GetProductCatalogRemark(string pc)
        {
            return JsonConvert.DeserializeObject<ProductCatalogRemark>(_productCatalogCofigRepository.GetProductCatalogRemark(_factoryCode, pc, _token));
        }

        public DataTable CreateDynamicDataTable(ProductCatalogsSearch dataSearch)
        {
            DataTable dt = new DataTable();
            if (dataSearch.Role == "4")
            {
                if (dataSearch.FactoryCode == "[]")
                {
                    dataSearch.FactoryCode = "";
                }
                else
                {
                    var facList = JsonConvert.DeserializeObject<string[]>(dataSearch.FactoryCode);
                    string facString = "";
                    foreach (var itf in facList)
                    {
                        string tmpfac = "'" + itf + "',";
                        facString = facString + tmpfac;
                        //  facString = facString.Substring(0, facString.Length - 1);
                    }
                    dataSearch.FactoryCode = facString.Substring(0, facString.Length - 1);
                }
            }

            ProductCatalogModel model = new ProductCatalogModel();
            var masterProductTemp = JsonConvert.DeserializeObject<List<MasterDataQuery>>(_masterDataService.GetMasterProductCatalogNotop(_factoryCode, JsonConvert.SerializeObject(dataSearch), _token));

            //string condition = "";
            //foreach (var item in masterProductTemp)
            //{
            //    string mat = "'" + item.MaterialNo + "',";
            //    condition = condition + mat;
            //}

            //if (!string.IsNullOrEmpty(condition))
            //{ condition = condition.Substring(0, condition.Length - 1); }
            //List<string> conditionlist = new List<string>();
            //conditionlist.Add(condition);

            int xx = 0;
            List<string> conditionlist = new List<string>();
            string ss = string.Empty;
            for (int i = 1; i <= masterProductTemp.Count; i++)
            {
                string mat = "'" + masterProductTemp[i - 1].MaterialNo.ToString() + "',";
                ss = ss + mat;
                if ((i % 1000) == 0)
                {
                    string tmpss = ss.Substring(0, ss.Length - 1);
                    conditionlist.Add(tmpss);
                    ss = "";
                }
            }
            if (!string.IsNullOrEmpty(ss))
            {
                string tmpss2 = ss.Substring(0, ss.Length - 1);
                conditionlist.Add(tmpss2);
            }

            List<Routing> RoutingTemp = new List<Routing>();
            for (int it = 0; it < conditionlist.Count; it++)
            {
                List<string> conditionfinal = new List<string>();
                conditionfinal.Add(conditionlist[it]);
                string factorycodeCheckRole = dataSearch.Role == "4" ? dataSearch.FactoryCode == "[]" ? "" : dataSearch.FactoryCode : "'" + _factoryCode + "'";
                var temp = JsonConvert.DeserializeObject<List<Routing>>(_routingAPIRepository.GetDapperRoutingByMatList(factorycodeCheckRole, JsonConvert.SerializeObject(conditionfinal), _token));
                RoutingTemp.AddRange(temp);
            }

            var ConvertRouting = RoutingTemp.OrderBy(x => x.SeqNo).GroupBy(cc => cc.MaterialNo)
                     .Select(dd => new
                     {
                         Material = dd.Key,
                         Machine = string.Join("-", dd.Where(x => !string.IsNullOrEmpty(x.Machine)).Select(ee => ee.Machine).ToList()),
                         Alternative1 = string.Join(",", dd.Where(x => !string.IsNullOrEmpty(x.Alternative1)).Select(ee => ee.Alternative1).ToList()),
                         PlateNo = string.Join(",", dd.Where(x => !string.IsNullOrEmpty(x.PlateNo)).Select(ee => ee.PlateNo).ToList()),
                         MylaNo = string.Join(",", dd.Where(x => !string.IsNullOrEmpty(x.MylaNo)).Select(ee => ee.MylaNo).ToList()),

                         Color1 = string.Join(",", dd.Where(x => !string.IsNullOrEmpty(x.Color1)).Select(ee => ee.Color1 + "/" + ee.Shade1).ToList()),
                         Color2 = string.Join(",", dd.Where(x => !string.IsNullOrEmpty(x.Color2)).Select(ee => ee.Color2 + "/" + ee.Shade2).ToList()),
                         Color3 = string.Join(",", dd.Where(x => !string.IsNullOrEmpty(x.Color3)).Select(ee => ee.Color3 + "/" + ee.Shade3).ToList()),
                         Color4 = string.Join(",", dd.Where(x => !string.IsNullOrEmpty(x.Color4)).Select(ee => ee.Color4 + "/" + ee.Shade4).ToList()),
                         Color5 = string.Join(",", dd.Where(x => !string.IsNullOrEmpty(x.Color5)).Select(ee => ee.Color5 + "/" + ee.Shade5).ToList()),
                         Color6 = string.Join(",", dd.Where(x => !string.IsNullOrEmpty(x.Color6)).Select(ee => ee.Color6 + "/" + ee.Shade6).ToList()),
                         Color7 = string.Join(",", dd.Where(x => !string.IsNullOrEmpty(x.Color7)).Select(ee => ee.Color7 + "/" + ee.Shade7).ToList()),

                         TearTape = string.Join(",", dd.Select(ee => ee.TearTape == true ? "True" : "False").ToList()),
                         BlockNo = string.Join(",", dd.Where(x => !string.IsNullOrEmpty(x.BlockNo)).Select(ee => ee.BlockNo).ToList()),
                         RemarkInprocess = string.Join(",", dd.Where(x => !string.IsNullOrEmpty(x.RemarkInprocess)).Select(ee => ee.RemarkInprocess).ToList()),
                         BlockNo2 = string.Join(",", dd.Where(x => !string.IsNullOrEmpty(x.BlockNo2)).Select(ee => ee.BlockNo2 + "/" + ee.MylaNo2).ToList()),
                         BlockNo3 = string.Join(",", dd.Where(x => !string.IsNullOrEmpty(x.BlockNo3)).Select(ee => ee.BlockNo3 + "/" + ee.MylaNo3).ToList()),
                         BlockNo4 = string.Join(",", dd.Where(x => !string.IsNullOrEmpty(x.BlockNo4)).Select(ee => ee.BlockNo4 + "/" + ee.MylaNo4).ToList()),
                         BlockNo5 = string.Join(",", dd.Where(x => !string.IsNullOrEmpty(x.BlockNo5)).Select(ee => ee.BlockNo5 + "/" + ee.MylaNo5).ToList()),
                         No_Open_In = dd.OrderBy(x => x.SeqNo).Select(z => z.NoOpenIn).FirstOrDefault()
                     });

            var tempFinalConvert = ConvertRouting.ToList();//.Where(x => x.BlockNo.Contains(dataSearch.blockNo) && x.PlateNo.Contains(dataSearch.plateNo)).ToList();

            var ccc = (from e in masterProductTemp
                       join d in tempFinalConvert
                       on e.MaterialNo equals d.Material
                       into ps
                       from d in ps.DefaultIfEmpty()
                       select new { e, d }).ToList();

            List<ProductCatalogs> tmp = new List<ProductCatalogs>();
            int rec = 0;
            try
            {
                foreach (var item in ccc)
                {
                    rec++;
                    if (rec > 0)
                    {
                        ProductCatalogs product = new ProductCatalogs();
                        product.Remark = item.e.Remark;
                        product.Pc = item.e.Pc;
                        product.MaterialNo = item.e.MaterialNo;
                        product.PartNo = item.e.PartNo;
                        product.Hierarchy = item.e.Hierarchy;
                        product.SaleOrg = item.e.SaleOrg;
                        product.Plant = item.e.Plant;
                        product.CustCode = item.e.CustCode;
                        product.CusId = item.e.CusId;
                        product.CustName = item.e.CustName;
                        product.Description = item.e.Description.Trim();
                        product.SaleText = item.e.SaleText;
                        product.Change = item.e.Change;
                        product.IndGrp = item.e.IndGrp;
                        product.IndDes = item.e.IndDes;
                        product.MaterialType = item.e.MaterialType;
                        product.PrintMethod = item.e.PrintMethod;
                        product.Flute = item.e.Flute;
                        product.Board = item.e.Board;
                        product.Wid = item.e.Wid;
                        product.Leg = item.e.Leg;
                        product.Hig = item.e.Hig;
                        product.BoxType = item.e.BoxType;
                        product.JoinType = item.e.JoinType;
                        product.CutSheetLeng = item.e.CutSheetLeng;
                        product.CutSheetWid = item.e.CutSheetWid;
                        product.Bun = item.e.Bun;
                        product.BunLayer = item.e.BunLayer;
                        product.LayerPalet = item.e.LayerPalet;
                        product.BoxPalet = item.e.BoxPalet;
                        product.WeightBox = item.e.WeightBox;
                        product.SaleUom = item.e.SaleUom;
                        product.BomUom = item.e.BomUom;
                        product.PalletSize = item.e.PalletSize;
                        product.LastUpdate = item.e.LastUpdate;
                        product.PurTxt = item.e.PurTxt;
                        product.HighGroup = item.e.HighGroup;
                        product.HighValue = item.e.HighValue;
                        product.ValidityStartFrom = item.e.ValidityStartFrom;
                        product.ValidityEndDate = item.e.ValidityEndDate;
                        product.ConditionNumber = item.e.ConditionNumber;
                        product.Rate = item.e.Rate;
                        product.CurrencyKey = item.e.CurrencyKey;
                        product.Vendorname = item.e.Vendorname;
                        product.Netprice = item.e.Netprice;
                        product.Unitofnetprice = item.e.Unitofnetprice;
                        product.SourcelistValidfrom = item.e.SourcelistValidfrom;
                        product.SourcelistValidto = item.e.SourcelistValidto;
                        product.Nonmove = item.e.Nonmove;
                        product.NonMoveMonth = item.e.NonMoveMonth;
                        product.StockQA = item.e.StockQA;
                        product.StockFG = item.e.StockFG;
                        product.StockWIP = item.e.StockWIP;
                        product.Machine = item.d == null ? "" : item.d.Machine;
                        product.Alternative1 = item.d == null ? "" : item.d.Alternative1;
                        product.PlateNo = item.d == null ? "" : item.d.PlateNo;
                        product.MylaNo = item.d == null ? "" : item.d.MylaNo;

                        string tmpcolor = item.d == null ? "" : ((string.IsNullOrEmpty(item.d.Color1.Trim()) ? "" : "/" + item.d.Color1) + (string.IsNullOrEmpty(item.d.Color2.Trim()) ? "" : "/" + item.d.Color2) + (string.IsNullOrEmpty(item.d.Color3.Trim()) ? "" : "/" + item.d.Color3) + (string.IsNullOrEmpty(item.d.Color4.Trim()) ? "" : "/" + item.d.Color4) + (string.IsNullOrEmpty(item.d.Color5.Trim()) ? "" : "/" + item.d.Color5) + (string.IsNullOrEmpty(item.d.Color6.Trim()) ? "" : "/" + item.d.Color6) + (string.IsNullOrEmpty(item.d.Color7.Trim()) ? "" : "/" + item.d.Color7));
                        if (tmpcolor != "")
                        {
                            if (tmpcolor.Substring(0, 1) == "/")
                            {
                                tmpcolor = tmpcolor.Substring(1, tmpcolor.Length - 1);
                            }
                        }
                        product.Color = tmpcolor;
                        product.TearTape = item.d == null ? "" : item.d.TearTape;
                        product.BlockNo = item.d == null ? "" : item.d.BlockNo;
                        product.RemarkInprocess = item.d == null ? "" : item.d.RemarkInprocess;
                        product.BlockNo2 = item.d == null ? "" : item.d.BlockNo2;
                        product.BlockNo3 = item.d == null ? "" : item.d.BlockNo3;
                        product.BlockNo4 = item.d == null ? "" : item.d.BlockNo4;
                        product.BlockNo5 = item.d == null ? "" : item.d.BlockNo5;
                        product.NoOpenIn = item.d == null ? null : item.d.No_Open_In;

                        product.Hold = item.e.Hold;
                        product.PieceSet = item.e.PieceSet;
                        product.HoldRemark = item.e.HoldRemark;
                        product.FactoryCode = item.e.FactoryCode;

                        product.CustInvType = item.e.CustInvType;
                        product.CIPInvType = item.e.CIPInvType;
                        product.PdisStatus = item.e.PdisStatus;
                        tmp.Add(product);
                    }
                }
            }
            catch (Exception ex)
            {
                int ddd = rec;
            }

            if (!string.IsNullOrEmpty(dataSearch.blockNo) && !string.IsNullOrEmpty(dataSearch.plateNo))
            {
                model.productCatalogs = tmp.Where(x => x.BlockNo.Contains(dataSearch.blockNo) && x.PlateNo.Contains(dataSearch.plateNo)).ToList();
            }
            else if (!string.IsNullOrEmpty(dataSearch.blockNo))
            {
                model.productCatalogs = tmp.Where(x => x.BlockNo.Contains(dataSearch.blockNo)).ToList();
            }
            else if (!string.IsNullOrEmpty(dataSearch.plateNo))
            {
                model.productCatalogs = tmp.Where(x => x.PlateNo.Contains(dataSearch.plateNo)).ToList();
            }
            else
            {
                model.productCatalogs = tmp.ToList();
            }
            // model.productCatalogs = tmp.Where(x => x.BlockNo.Contains(dataSearch.blockNo) && x.PlateNo.Contains(dataSearch.plateNo)).ToList(); ;

            model.columnNames = model.ProductCatalog.GetType().GetProperties().Select(k => k.Name).ToList();
            model.columnNamesInModal = model.columnNames.OrderByDescending(x => x).ToList();
            model.productCatalogConfigs = JsonConvert.DeserializeObject<List<ProductCatalogConfig>>(_productCatalogCofigRepository.GetProductGroupList(_factoryCode, _username, string.Empty, _token));

            DataTable table = new DataTable();
            foreach (var col in model.productCatalogConfigs)
            {
                if (col.ColumnName.ToString() == "Action" || col.ColumnName.ToString() == "Bom")
                {
                }
                else
                {
                    try { table.Columns.Add(col.ColumnName); } catch (Exception ex) { }
                }
            }

            foreach (var item in model.productCatalogs)
            {
                DataRow row1 = table.NewRow();
                foreach (var colselect in model.productCatalogConfigs)
                {
                    if (colselect.ColumnName.Trim().ToLower() != "addremark")
                    {
                        if (colselect.ColumnName == "Remark")
                        {
                            row1["Remark"] = item.Remark;
                        }
                        if (colselect.ColumnName == "Pc")
                        {
                            row1["Pc"] = item.Pc;
                        }
                        if (colselect.ColumnName == "MaterialNo")
                        {
                            row1["MaterialNo"] = item.MaterialNo;
                        }
                        if (colselect.ColumnName == "PartNo")
                        {
                            row1["PartNo"] = item.PartNo;
                        }
                        if (colselect.ColumnName == "Hierarchy")
                        {
                            row1["Hierarchy"] = item.Hierarchy;
                        }
                        if (colselect.ColumnName == "SaleOrg")
                        {
                            row1["SaleOrg"] = item.SaleOrg;
                        }
                        if (colselect.ColumnName == "Plant")
                        {
                            row1["Plant"] = item.Plant;
                        }
                        if (colselect.ColumnName == "CustCode")
                        {
                            row1["CustCode"] = item.CustCode;
                        }
                        if (colselect.ColumnName == "CusId")
                        {
                            row1["CusId"] = item.CusId;
                        }
                        if (colselect.ColumnName == "CustName")
                        {
                            row1["CustName"] = item.CustName;
                        }
                        if (colselect.ColumnName == "Description")
                        {
                            row1["Description"] = item.Description;
                        }
                        if (colselect.ColumnName == "SaleText")
                        {
                            row1["SaleText"] = item.SaleText;
                        }
                        if (colselect.ColumnName == "Change")
                        {
                            row1["Change"] = item.Change;
                        }
                        if (colselect.ColumnName == "IndGrp")
                        {
                            row1["IndGrp"] = item.IndGrp;
                        }
                        if (colselect.ColumnName == "IndDes")
                        {
                            row1["IndDes"] = item.IndDes;
                        }
                        if (colselect.ColumnName == "MaterialType")
                        {
                            row1["MaterialType"] = item.MaterialType;
                        }
                        if (colselect.ColumnName == "PrintMethod")
                        {
                            row1["PrintMethod"] = item.PrintMethod;
                        }
                        if (colselect.ColumnName == "Flute")
                        {
                            row1["Flute"] = item.Flute;
                        }
                        if (colselect.ColumnName == "Board")
                        {
                            row1["Board"] = item.Board;
                        }
                        if (colselect.ColumnName == "Wid")
                        {
                            row1["Wid"] = item.Wid;
                        }
                        if (colselect.ColumnName == "Leg")
                        {
                            row1["Leg"] = item.Leg;
                        }
                        if (colselect.ColumnName == "Hig")
                        {
                            row1["Hig"] = item.Hig;
                        }
                        if (colselect.ColumnName == "BoxType")
                        {
                            row1["BoxType"] = item.BoxType;
                        }
                        if (colselect.ColumnName == "JoinType")
                        {
                            row1["JoinType"] = item.JoinType;
                        }
                        if (colselect.ColumnName == "CutSheetLeng")
                        {
                            row1["CutSheetLeng"] = item.CutSheetLeng;
                        }
                        if (colselect.ColumnName == "CutSheetWid")
                        {
                            row1["CutSheetWid"] = item.CutSheetWid;
                        }
                        if (colselect.ColumnName == "Bun")
                        {
                            row1["Bun"] = item.Bun;
                        }
                        if (colselect.ColumnName == "BunLayer")
                        {
                            row1["BunLayer"] = item.BunLayer;
                        }
                        if (colselect.ColumnName == "LayerPalet")
                        {
                            row1["LayerPalet"] = item.LayerPalet;
                        }
                        if (colselect.ColumnName == "BoxPalet")
                        {
                            row1["BoxPalet"] = item.BoxPalet;
                        }
                        if (colselect.ColumnName == "WeightBox")
                        {
                            row1["WeightBox"] = item.WeightBox;
                        }
                        if (colselect.ColumnName == "SaleUom")
                        {
                            row1["SaleUom"] = item.SaleUom;
                        }
                        if (colselect.ColumnName == "BomUom")
                        {
                            row1["BomUom"] = item.BomUom;
                        }
                        if (colselect.ColumnName == "PalletSize")
                        {
                            row1["PalletSize"] = item.PalletSize;
                        }
                        if (colselect.ColumnName == "LastUpdate")
                        {
                            row1["LastUpdate"] = item.LastUpdate;
                        }
                        if (colselect.ColumnName == "PurTxt")
                        {
                            row1["PurTxt"] = item.PurTxt;
                        }
                        if (colselect.ColumnName == "HighGroup")
                        {
                            row1["HighGroup"] = item.HighGroup;
                        }
                        if (colselect.ColumnName == "HighValue")
                        {
                            row1["HighValue"] = item.HighValue;
                        }
                        if (colselect.ColumnName == "ValidityStartFrom")
                        {
                            row1["ValidityStartFrom"] = item.ValidityStartFrom;
                        }
                        if (colselect.ColumnName == "ValidityEndDate")
                        {
                            row1["ValidityEndDate"] = item.ValidityEndDate;
                        }
                        if (colselect.ColumnName == "ConditionNumber")
                        {
                            row1["ConditionNumber"] = item.ConditionNumber;
                        }
                        if (colselect.ColumnName == "Rate")
                        {
                            row1["Rate"] = item.Rate;
                        }
                        if (colselect.ColumnName == "CurrencyKey")
                        {
                            row1["CurrencyKey"] = item.CurrencyKey;
                        }
                        if (colselect.ColumnName == "Vendorname")
                        {
                            row1["Vendorname"] = item.Vendorname;
                        }
                        if (colselect.ColumnName == "Netprice")
                        {
                            row1["Netprice"] = item.Netprice;
                        }
                        if (colselect.ColumnName == "Unitofnetprice")
                        {
                            row1["Unitofnetprice"] = item.Unitofnetprice;
                        }
                        if (colselect.ColumnName == "SourcelistValidfrom")
                        {
                            row1["SourcelistValidfrom"] = item.SourcelistValidfrom;
                        }
                        if (colselect.ColumnName == "SourcelistValidto")
                        {
                            row1["SourcelistValidto"] = item.SourcelistValidto;
                        }
                        if (colselect.ColumnName == "Nonmove")
                        {
                            row1["Nonmove"] = item.Nonmove;
                        }
                        if (colselect.ColumnName == "NonMoveMonth")
                        {
                            row1["NonMoveMonth"] = item.NonMoveMonth;
                        }
                        if (colselect.ColumnName == "StockWIP")
                        {
                            row1["StockWIP"] = item.StockWIP;
                        }
                        if (colselect.ColumnName == "StockFG")
                        {
                            row1["StockFG"] = item.StockFG;
                        }
                        if (colselect.ColumnName == "StockQA")
                        {
                            row1["StockQA"] = item.StockQA;
                        }
                        if (colselect.ColumnName == "Machine")
                        {
                            row1["Machine"] = item.Machine;
                        }
                        if (colselect.ColumnName == "Alternative1")
                        {
                            row1["Alternative1"] = item.Alternative1;
                        }
                        if (colselect.ColumnName == "PlateNo")
                        {
                            row1["PlateNo"] = item.PlateNo;
                        }
                        if (colselect.ColumnName == "MylaNo")
                        {
                            row1["MylaNo"] = item.MylaNo;
                        }
                        if (colselect.ColumnName == "Color")
                        {
                            row1["Color"] = item.Color;
                        }
                        if (colselect.ColumnName == "TearTape")
                        {
                            row1["TearTape"] = item.TearTape;
                        }
                        if (colselect.ColumnName == "BlockNo")
                        {
                            row1["BlockNo"] = item.BlockNo;
                        }
                        if (colselect.ColumnName == "RemarkInprocess")
                        {
                            row1["RemarkInprocess"] = item.RemarkInprocess;
                        }
                        if (colselect.ColumnName == "BlockNo2")
                        {
                            row1["BlockNo2"] = item.BlockNo2;
                        }
                        if (colselect.ColumnName == "BlockNo3")
                        {
                            row1["BlockNo3"] = item.BlockNo3;
                        }
                        if (colselect.ColumnName == "BlockNo4")
                        {
                            row1["BlockNo4"] = item.BlockNo4;
                        }
                        if (colselect.ColumnName == "BlockNo5")
                        {
                            row1["BlockNo5"] = item.BlockNo5;
                        }
                        if (colselect.ColumnName == "NoOpenIn")
                        {
                            row1["NoOpenIn"] = item.NoOpenIn;
                        }
                        if (colselect.ColumnName == "Hold")
                        {
                            row1["Hold"] = item.Hold;
                        }
                        if (colselect.ColumnName == "PieceSet")
                        {
                            row1["PieceSet"] = item.PieceSet;
                        }
                        if (colselect.ColumnName == "HoldRemark")
                        {
                            row1["HoldRemark"] = item.HoldRemark;
                        }
                        if (colselect.ColumnName == "FactoryCode")
                        {
                            row1["FactoryCode"] = item.FactoryCode;
                        }
                        if (colselect.ColumnName == "CustInvType")
                        {
                            row1["CustInvType"] = item.CustInvType;
                        }
                        if (colselect.ColumnName == "CIPInvType")
                        {
                            row1["CIPInvType"] = item.CIPInvType;
                        }
                    }
                }
                table.Rows.Add(row1);
            }

            return table;
        }

        public ProductCatalogModel GetHoldMaterial(string Material)
        {
            ProductCatalogModel model = new ProductCatalogModel();
            model.holdMaterial = JsonConvert.DeserializeObject<HoldMaterial>(_productCatalogCofigRepository.GetHoldMaterialByMaterial(_factoryCode, Material, _token));
            model.holdMaterialHistories = JsonConvert.DeserializeObject<List<HoldMaterialHistory>>(_productCatalogCofigRepository.GetHoldMaterialHistoryByMaterial(_factoryCode, Material, _token));
            return model;
        }

        public void SaveHoldMaterial(HoldMaterial model)
        {
            model.CreatedBy = _username;
            _productCatalogCofigRepository.SaveHoldMaterial(_factoryCode, JsonConvert.SerializeObject(model), _token);

            if (!model.IsLocked.Value)
            {
                PresaleHoldMaterialModel presale = new PresaleHoldMaterialModel();
                presale.fromSystem = "PMTs";
                presale.materialNo = model.MaterialNo;
                presale.pcCode = model.ChangeProductNo;
                presale.unholdStatus = "Y";
                presale.actionBy = _username;
                presale.remark = model.HoldRemark;
                _productCatalogCofigRepository.PresaleHoldMaterial(configuration.GetConnectionString("PresaleUnHoldMaterial"), JsonConvert.SerializeObject(presale));
            }
        }

        public void UpdateHoldMaterial(HoldMaterial model)
        {
            model.UpdatedBy = _username;
            _productCatalogCofigRepository.UpdateHoldMaterial(_factoryCode, JsonConvert.SerializeObject(model), _token);

            if (!model.IsLocked.Value)
            {
                PresaleHoldMaterialModel presale = new PresaleHoldMaterialModel();
                presale.fromSystem = "PMTs";
                presale.materialNo = model.MaterialNo;
                presale.pcCode = model.ChangeProductNo;
                presale.unholdStatus = "Y";
                presale.actionBy = _username;
                presale.remark = model.HoldRemark;
                _productCatalogCofigRepository.PresaleHoldMaterial(configuration.GetConnectionString("PresaleUnHoldMaterial"), JsonConvert.SerializeObject(presale));
            }
        }

        public void SaveHoldMaterialHistory(HoldMaterialHistory model)
        {
            model.CreatedBy = _username;
            _productCatalogCofigRepository.SaveHoldMaterialHistory(_factoryCode, JsonConvert.SerializeObject(model), _token);
        }

        public ProductCatalogModel GetAllPlant()
        {
            ProductCatalogModel model = new ProductCatalogModel();
            var tmp = JsonConvert.DeserializeObject<List<CompanyProfile>>(_companyProfileAPIRepository.GetCompanyProfileList(_factoryCode, _token));
            foreach (var item in tmp.Where(x => x.StatusPmts == true).ToList())
            {
                Plant tmpplant = new Plant();
                tmpplant.PlantCode = item.SaleOrg;
                tmpplant.Desc = item.SaleOrg + "-" + item.ShortName;
                model.Plant.Add(tmpplant);
            }
            return model;
        }

        public ProductCatalogModel GetAllPlantProduction()
        {
            ProductCatalogModel model = new ProductCatalogModel();
            var tmp = JsonConvert.DeserializeObject<List<CompanyProfile>>(_companyProfileAPIRepository.GetCompanyProfileList(_factoryCode, _token));
            foreach (var item in tmp.Where(x => x.StatusPmts == true).ToList())
            {
                Plant tmpplant = new Plant();
                tmpplant.PlantCode = item.Plant;
                tmpplant.Desc = item.Plant + "-" + item.ShortName;
                model.Plant.Add(tmpplant);
            }
            return model;
        }

        public ProductCatalogModel GetBom(string factorycode, string material)
        {
            ProductCatalogModel model = new ProductCatalogModel();
            model.bomStructView = JsonConvert.DeserializeObject<List<BomStruct>>(_bomStructAPIRepository.SearchBomStructByMaterialNo(factorycode, material, _token));
            return model;
        }

        public string GetOrderItemByMoData(string factoryCode, string Material)
        {
            return JsonConvert.DeserializeObject<string>(_productCatalogCofigRepository.GetOrderItemByMoData(factoryCode, Material, _token));
        }

        public List<CompanyProfile> GetAllCompanyProfile()
        {
            return JsonConvert.DeserializeObject<List<CompanyProfile>>(_companyProfileAPIRepository.GetCompanyProfileList(_factoryCode, _token));
        }

        public CompanyProfile GetAllCompanyProfileByLogin()
        {
            var model = JsonConvert.DeserializeObject<List<CompanyProfile>>(_companyProfileAPIRepository.GetCompanyProfileList(_factoryCode, _token));
            return model.Where(x => x.Plant == _factoryCode).FirstOrDefault();
        }

        public void SearchScalePriceMatProduct(ref ScalePriceMatProductViewModel model, string custId, string custName, string custCode, string pc1, string pc2, string pc3, string materialType, string salePlants, string plantPdts, string materialNo)
        {
            var salePlantJson = !string.IsNullOrEmpty(salePlants) ? salePlants : string.Empty;
            var plantPdtsJson = !string.IsNullOrEmpty(plantPdts) ? plantPdts : string.Empty;
            model.scalePriceMatProductModels = JsonConvert.DeserializeObject<List<ScalePriceMatProductModel>>(_productCatalogCofigRepository.GetScalePriceMatProduct(_factoryCode, custId, custName, custCode, pc1, pc2, pc3, materialType, salePlantJson, plantPdtsJson, materialNo, _token));
            var companyProfiles = JsonConvert.DeserializeObject<List<CompanyProfile>>(_companyProfileAPIRepository.GetCompanyProfileList(_factoryCode, _token));
            model.MaterialTypeList = JsonConvert.DeserializeObject<List<MaterialType>>(Convert.ToString(_materialTypeAPIRepository.GetMaterialTypeList(_token)));
            model.MaterialTypeList.ForEach(m => m.Description = $"{m.MatCode} {m.Description}");
            var activedCompanys = companyProfiles.Where(x => x.StatusPmts == true).ToList();
            foreach (var item in activedCompanys)
            {
                var plant = new PlantModel();
                plant.Plant = item.Plant;
                plant.PlantsDesc = item.Plant + "-" + item.ShortName;
                plant.SaleorgsDesc = item.SaleOrg + "-" + item.ShortName;
                model.Plants.Add(plant);
            }
        }

        public void SearchBOMMaterialProduct(ref List<BOMMaterialProductModel> bomMaterialProductModels, string custId, string custName, string custCode, string pc1, string pc2, string pc3)
        {
            bomMaterialProductModels = JsonConvert.DeserializeObject<List<BOMMaterialProductModel>>(_productCatalogCofigRepository.GetBOMMaterialProduct(_factoryCode, custId, custName, custCode, pc1, pc2, pc3, _token));
        }

        public void GetMaterialTypesAndPlants(ref ScalePriceMatProductViewModel model)
        {
            var companyProfiles = JsonConvert.DeserializeObject<List<CompanyProfile>>(_companyProfileAPIRepository.GetCompanyProfileList(_factoryCode, _token));
            model.MaterialTypeList = JsonConvert.DeserializeObject<List<MaterialType>>(Convert.ToString(_materialTypeAPIRepository.GetMaterialTypeList(_token)));
            model.MaterialTypeList.ForEach(m => m.Description = $"{m.MatCode} {m.Description}");
            foreach (var item in companyProfiles.Where(x => x.StatusPmts == true).ToList())
            {
                var plant = new PlantModel();
                plant.Plant = item.Plant;
                plant.PlantsDesc = item.Plant + "-" + item.ShortName;
                plant.SaleorgsDesc = item.SaleOrg + "-" + item.ShortName;
                model.Plants.Add(plant);
            }
        }

        public void CallApiE_Ordering(HoldMaterial holdMaterial)
        {
            var requestId = "0000000001";
            var requestEOrderingModel = new E_Ordering();
            requestEOrderingModel.materialList = new List<Material>();
            var responseStatus = false;
            var statusMessage = string.Empty;

            //get last id of E-OrderingLog
            var lastEOrderingLog = new EorderingLog();
            lastEOrderingLog = JsonConvert.DeserializeObject<EorderingLog>(eOrderingLogAPIRepository.GetLastEOrderingLog(_factoryCode, _token));
            if (lastEOrderingLog != null)
            {
                requestId = (lastEOrderingLog.Id + 1).ToString().PadLeft(10, '0');
            }

            if (holdMaterial != null)
            {
                requestEOrderingModel.requestId = requestId;
                requestEOrderingModel.materialList.Add(new Material
                {
                    materialNo = holdMaterial.MaterialNo,
                    HoldRemark = holdMaterial.HoldRemark,
                    isHold = holdMaterial.IsLocked
                });

                var jsonObj = JsonConvert.SerializeObject(requestEOrderingModel);

                //sent request to E-Ordering API
                var url = configuration.GetConnectionString("EOrderingAPI") + "master-data/products/catalog";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "PUT";
                request.ContentType = "application/json";
                request.ContentLength = jsonObj.Length;
                var clientId = "7d851948daff44ffa8419b89b1fb1a28";
                var clientSecret = "00c1ac35AE4B40ff8C7072743a6f7F2B";
                string encoded = System.Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1")
                                               .GetBytes(clientId + ":" + clientSecret));
                request.Headers.Add("clientId", clientId);
                request.Headers.Add("clientSecret", clientSecret);
                using (Stream webStream = request.GetRequestStream())
                using (var requestWriter = new StreamWriter(webStream, System.Text.Encoding.ASCII))
                {
                    requestWriter.Write(jsonObj);
                }

                try
                {
                    WebResponse webResponse = request.GetResponse();
                    using var webStream = webResponse.GetResponseStream() ?? Stream.Null;
                    using var responseReader = new StreamReader(webStream);
                    dynamic response = JObject.Parse(responseReader.ReadToEnd());
                    statusMessage = response.message;
                    responseStatus = response.success;
                    Console.Out.WriteLine(response);
                }
                catch (Exception e)
                {
                    Console.Out.WriteLine("-----------------");
                    Console.Out.WriteLine(e.Message);
                    throw new Exception("Can't sent data holdMat to E-Ordering");
                }

                var EorderingLogModel = new EorderingLog
                {
                    Id = 0,
                    MaterialNo = holdMaterial.MaterialNo,
                    JsonRequestBody = jsonObj,
                    RequestId = requestId,
                    SentBy = _username,
                    SentDate = DateTime.Now,
                    StatusMessage = statusMessage,
                    StatusResponse = responseStatus
                };

                eOrderingLogAPIRepository.SaveEOrderingLog(_factoryCode, JsonConvert.SerializeObject(EorderingLogModel), _token);
            }
        }
    }
}