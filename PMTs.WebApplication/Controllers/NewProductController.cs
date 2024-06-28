using AutoMapper;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Mvc.Rendering;

using Microsoft.AspNetCore.Mvc.Rendering;

using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using MySqlX.XDevAPI.Common;
using Newtonsoft.Json;
using PMTs.DataAccess;
using PMTs.DataAccess.ComplexModel;
using PMTs.DataAccess.ModelPresale;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.NewProduct;
using PMTs.DataAccess.ModelView.Report;
using PMTs.DataAccess.Repository;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Shared;
using PMTs.Logs.Logger;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Models;
using PMTs.WebApplication.Services;
using PMTs.WebApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

using System.IO;

using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PMTs.WebApplication.Controllers
{
    [SessionTimeout]
    //  [SessionTransactionTimeout]
    public class NewProductController : Controller
    {
        private readonly IMaintenanceCustomerService _maintenanceCustomerService;
        private readonly IExtensionService _extensionService;
        private readonly IProductCustomerService _productCustomerService;
        private readonly ICategoryService _categoryService;
        private readonly IProductSpecService _productSpecService;
        private readonly IProductPropService _productPropService;
        private readonly IProductInfoService _productInfoService;
        private readonly IRoutingService _routingService;
        private readonly IProductERPService _productERPService;
        private readonly IProductPicService _productPicService;
        private readonly IPresaleService _presaleService;
        private readonly INewProductService _newProductService;
        private readonly IHostingEnvironment _environment;
        private readonly IMasterDataService _masterDataService;
        private readonly IMasterCardService _masterCardService;

        public NewProductController(
            IMaintenanceCustomerService maintenanceCustomerService,
            IExtensionService extensionService,
            IProductCustomerService productCustomerService,
            ICategoryService categoryService,
            IProductSpecService productSpecService,
            IProductPropService productPropService,
            IProductInfoService productInfoService,
            IRoutingService routingService,
            IProductERPService productERPService,
           IProductPicService productPicService,
            IPresaleService presaleService,
            INewProductService newProductService,
            IHostingEnvironment IHostingEnvironment,
            IMasterDataService masterDataService,
            IMasterCardService masterCardService

            )
        {
            _maintenanceCustomerService = maintenanceCustomerService;
            _extensionService = extensionService;
            _productCustomerService = productCustomerService;
            _categoryService = categoryService;
            _productSpecService = productSpecService;
            _productPropService = productPropService;
            _productInfoService = productInfoService;
            _routingService = routingService;
            _productERPService = productERPService;
            _productPicService = productPicService;
            _presaleService = presaleService;
            _newProductService = newProductService;
            _environment = IHostingEnvironment;
            _masterDataService = masterDataService;
            _masterCardService = masterCardService;
        }

        #region Outsourcing

        [SessionTimeout]
        public IActionResult Outsourcing(string factoryCode, string materialNo)
        {
            //var masterDataList = new List<MasterDataRoutingModel>();
            var outsourcingViewModel = new OutsourcingViewModel();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _newProductService.GetCompanyProfiles(ref outsourcingViewModel);
                //_masterDataService.GetMasterData(ref masterDataList, "SaleOrg", factoryCode + "," + materialNo, isMaterialOnly: factoryCode == null ? true : false);
                //outsourcingViewModel.MasterDataRoutingModels = masterDataList;
                _newProductService.GetHireOrders(ref outsourcingViewModel);
                _newProductService.GetHireMappings(ref outsourcingViewModel);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }

            return View(outsourcingViewModel);
        }

        //[SessionTimeout]
        //public IActionResult SearchOutsourcing(string factoryCode, string materialNo)
        //{
        //    var masterDataList = new List<MasterDataRoutingModel>();
        //    var outsourcingViewModel = new OutsourcingViewModel();

        //    try
        //    {
        //        Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
        //        _newProductService.GetCompanyProfiles(ref outsourcingViewModel);
        //        _masterDataService.GetMasterData(ref masterDataList, "SaleOrg", factoryCode + "," + materialNo, isMaterialOnly: factoryCode == null ? true : false);
        //        //outsourcingViewModel.MasterDataRoutingModels = masterDataList;
        //        Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
        //        throw ex;
        //    }

        //    return PartialView("_OutsourcingTable", outsourcingViewModel);
        //}

        [SessionTimeout]
        [HttpPost]
        public IActionResult NextOutsourcing(OutsourcingViewModel outsourcing)
        {
            var actionTran = outsourcing.Action.Equals("Create") ? "CreateOs" : "CopyOs";
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _newProductService.SaveFactoryCodeToSession(outsourcing);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return RedirectToAction("Categories", "NewProduct", new { actionTran = actionTran, materialNo = outsourcing.MaterialNo });
            }
            catch (Exception ex)
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Method");
                _newProductService.GetCompanyProfiles(ref outsourcing);
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }

            return RedirectToAction("Outsourcing", "NewProduct");
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult CopyOSCheckMaterial(string materialNo, string action, string plantOs)
        {
            var isSuccess = false;
            var exceptionMessage = string.Empty;
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                //do something
                _newProductService.CheckDuplicateMaterial(plantOs, materialNo, action);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                isSuccess = true;
            }
            catch (Exception ex)
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Method");
                //do something
                exceptionMessage = ex.Message;
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage });
        }

        #endregion Outsourcing

        #region Categories

        [SessionTimeout]
        public IActionResult Categories(string actionTran, string materialNo, string psmId, string currentPage)
        {
            //Logger.Info("TestLogs");
            TransactionDataModel transactionDataModel = new TransactionDataModel();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _categoryService.GetCategory(this, ref transactionDataModel, actionTran, materialNo, psmId);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                //if (ex.Message.Contains("404")) {
                //}
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }

            return View(transactionDataModel);
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult SaveCategories(TransactionDataModel transactionDataModel)
        {
            bool isSuccess;
            string exeptionMessage = string.Empty;

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _categoryService.SaveCategories(transactionDataModel);

                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                exeptionMessage = ex.Message;
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                isSuccess = false;
            }

            return Json(new { isSuccess = isSuccess, exeptionMessage = exeptionMessage });
        }

        #endregion Categories

        #region Customer

        [SessionTimeout]
        public IActionResult Customer()
        {
            TransactionDataModel transactionDataModel = new TransactionDataModel();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _productCustomerService.GetCustomer(this, ref transactionDataModel);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return View(transactionDataModel);
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult SaveCustomer(TransactionDataModel transactionDataModel, string QaSpecArr)
        {
            bool isSuccess;
            string exeptionMessage = string.Empty;
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _productCustomerService.SaveCustomer(ref transactionDataModel, QaSpecArr);

                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                exeptionMessage = ex.Message;
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                isSuccess = false;
            }

            return Json(new { isSuccess = isSuccess, exceptionMessage = exeptionMessage });
        }

        #endregion Customer

        #region ProductInfo

        [SessionTimeout]
        public IActionResult ProductInfo()
        {
            TransactionDataModel transactionDataModel = new TransactionDataModel();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _productInfoService.GetProductInfo(this, ref transactionDataModel);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }

            return View(transactionDataModel);
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult SaveProductInfo(TransactionDataModel transactionDataModel)
        {
            bool isSuccess;
            string exeptionMessage = string.Empty;
            string materialCode = string.Empty;
            bool isOverBackward = true;
            bool isExistCost = true;

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _productInfoService.SaveProductInfo(ref transactionDataModel, ref isOverBackward, ref isExistCost);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                isSuccess = true;
                materialCode = transactionDataModel.modelCategories != null ? transactionDataModel.modelCategories.MatCode : null;
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                exeptionMessage = ex.Message;

                isSuccess = false;
            }

            return Json(new { isSuccess = isSuccess, exeptionMessage = exeptionMessage, materialNo = transactionDataModel.MaterialNo, IsShowSaveModel = isOverBackward, materialType = materialCode, IsExistCost = isExistCost });
        }

        #endregion ProductInfo

        #region ProductSpec

        [SessionTimeout]
        public JsonResult addCoating(string station)
        {
            var model = SessionExtentions.GetSession<TransactionDataModel>(HttpContext.Session, "TransactionDataModel");
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                model.modelProductSpec.Coating = new List<Coating>();
                model.modelProductSpec.ListCoatingType = model.modelProductSpec.Additive.Select(x => x.Type).Distinct().Select(sli => new SelectListItem { Value = sli, Text = sli });
                model.modelProductSpec.ListCoatingName = model.modelProductSpec.Additive.Select(x => x.Description).Distinct().Select(sli => new SelectListItem { Value = sli, Text = sli });
                string[] arrList = { "Top 3", "Top 2", "Top 1", "Botton 1", "Botton 2", "Botton 3" };
                for (int i = 0; i < 6; i++)
                {
                    Coating tmp = new Coating();
                    tmp.Station = station;
                    tmp.Slide = arrList[i].ToString();
                    tmp.Type = "";
                    tmp.Name = "";
                    model.modelProductSpec.Coating.Add(tmp);
                }
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return Json(RenderView.RenderRazorViewToString(this, "_CoatingTBLRender", model));
        }

        [SessionTimeout]
        public JsonResult GetGroupCoating()
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var model = SessionExtentions.GetSession<TransactionDataModel>(HttpContext.Session, "TransactionDataModel");
                var coating = _productSpecService.Coatinglst(model);
                var strGroup = coating.Select(x => x.Station).Distinct();
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(strGroup);
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json("");
            }
        }

        [SessionTimeout]
        public JsonResult editCoating(string station)
        {
            var model = SessionExtentions.GetSession<TransactionDataModel>(HttpContext.Session, "TransactionDataModel");
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var coating = _productSpecService.Coatinglst(model);
                var temp = coating.Where(x => x.Station == station).ToList();
                model.modelProductSpec.ListCoatingType = model.modelProductSpec.Additive.Select(x => x.Type).Distinct().Select(sli => new SelectListItem { Value = sli, Text = sli });
                model.modelProductSpec.ListCoatingName = model.modelProductSpec.Additive.Select(x => x.Description).Distinct().Select(sli => new SelectListItem { Value = sli, Text = sli });
                model.modelProductSpec.Coating.Clear();
                foreach (var item in temp)
                {
                    Coating tmp = new Coating();
                    tmp.Station = item.Station;
                    tmp.Slide = item.Slide == "T" ? "Top" + " " + item.Layer.ToString() : "Botton" + " " + item.Layer.ToString();
                    tmp.Type = item.Type;
                    tmp.Name = item.Name;
                    model.modelProductSpec.Coating.Add(tmp);
                }
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return Json(RenderView.RenderRazorViewToString(this, "_CoatingTBLRender", model));
        }

        [SessionTimeout]
        public IActionResult ProductSpec(string ActionTran)
        {
            TransactionDataModel transactionDataModel = new TransactionDataModel();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _productSpecService.GetProductSpec(ref transactionDataModel, ActionTran);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return View(transactionDataModel);
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult BoardAlt(string Prefix, string flute)
        {
            var BoardList = new List<BoardViewModel>();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                BoardList = _productSpecService.GetBoardAlt(Prefix, flute);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return Json(BoardList);
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult SearchBoardAltList(string flute)
        {
            var BoardList = new List<SearchBoardAlt>();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                BoardList = _productSpecService.SearchBoardAlt(flute);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return Json(BoardList);
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult BoardSpec(string Code, string flute, string plant)
        {
            var BoardSpec = new List<BoardSpecWeight>();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                BoardSpec = _productSpecService.GetBoardSpec(Code, flute, plant);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return Json(BoardSpec);
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult ChkPriority(ProductSpecViewModel model)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var dup = _productSpecService.chkPriority(model);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(new { FlagDup = dup });
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json(new { FlagDup = "" });
            }
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult Coating(string CoatingArray)
        {
            try
            {
                TransactionDataModel modelx = new TransactionDataModel();
                modelx = SessionExtentions.GetSession<TransactionDataModel>(HttpContext.Session, "TransactionDataModel");
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _productSpecService.Coating(CoatingArray, modelx);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json("success");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json("");
            }
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult GetBoard(ProductSpecViewModel model)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var exe = _productSpecService.GetBoard(model);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(exe);
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json("");
            }
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult Compute(ProductSpecViewModel model)
        {
            bool isSuccess;
            //var exe = new ProductSpecViewModel();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var exe = _productSpecService.ComputeRSC(model);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                isSuccess = true;
                return Json(exe);
                //return Json(exe, new { isSuccess });
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                isSuccess = false;
                return Json("");
            }
            //return Json(new { isSuccess = isSuccess, data = exe });
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult ComputeOneP(ProductSpecViewModel model)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var exe = _productSpecService.ComputeOneP(model);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(exe);
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json("");
            }
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult ComputeTwoP(ProductSpecViewModel model)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var exe = _productSpecService.ComputeTwoP(model);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(exe);
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json("");
            }
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult ComputeDC(ProductSpecViewModel model)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var exe = _productSpecService.ComputeDC(model);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(exe);
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json("");
            }
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult ComputeSF(ProductSpecViewModel model)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var exe = _productSpecService.ComputeSF(model);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(exe);
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json("");
            }
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult ComputeHC(ProductSpecViewModel model)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var exe = _productSpecService.ComputeHC(model);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(exe);
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json("");
            }
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult ComputeHB(ProductSpecViewModel model)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var exe = _productSpecService.ComputeHB(model);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(exe);
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json("");
            }
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult ComputeCG(ProductSpecViewModel model)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var exe = _productSpecService.ComputeCG(model);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(exe);
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json("");
            }
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult CriteriaBoardFromThickness(ProductSpecViewModel model)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var exe = _productSpecService.CriteriaBoardFromThickness(model);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(exe);
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json("");
            }
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult CriteriaBoardHoney(ProductSpecViewModel model)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var exe = _productSpecService.CriteriaBoardHoney(model);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(exe);
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json("");
            }
        }

        //[HttpPost]
        //public JsonResult ComputeOther(ProductSpecViewModel model)
        //{
        //    try
        //    {
        //        Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
        //        var exe = _productSpecService.ComputeOther(model);
        //        Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
        //        return Json(exe);
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
        //        return Json("");
        //    }

        //}
        [SessionTimeout]
        [HttpPost]
        public JsonResult ComputeTeeth(ProductSpecViewModel model)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var exe = _productSpecService.ComputeTeeth(model);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(exe);
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json("");
            }
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult ComputeWeightAndArea(ProductSpecViewModel model)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var exe = _productSpecService.ComputeWeightAndArea(model);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(exe);
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json("");
            }
        }

        [SessionTimeout]
        public IActionResult AddBoardAlt(ProductSpecViewModel model)
        {
            TransactionDataModel modelx = new TransactionDataModel();

            modelx = SessionExtentions.GetSession<TransactionDataModel>(HttpContext.Session, "TransactionDataModel");
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                modelx.modelProductSpec = _productSpecService.AddBoardAlt(model);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return PartialView("_AddBoard", modelx.modelProductSpec);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Input string was not in a correct format.")
                {
                    ViewBag.Message = string.Format("Input string was not in a correct format.", "", DateTime.Now.ToString());
                }
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return RedirectToAction("ProductSpec", "NewProduct");
            }
        }

        [SessionTimeout]
        public IActionResult RemoveBoardAlt(int prior)
        {
            TransactionDataModel model = new TransactionDataModel();

            model = SessionExtentions.GetSession<TransactionDataModel>(HttpContext.Session, "TransactionDataModel");
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                model.modelProductSpec = _productSpecService.RemoveBoardAlt(prior);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return PartialView("_AddBoard", model.modelProductSpec);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Input string was not in a correct format.")
                {
                    ViewBag.Message = string.Format("Input string was not in a correct format.", "", DateTime.Now.ToString());
                }
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return RedirectToAction("ProductSpec", "NewProduct");
            }
        }

        [SessionTimeout]
        [HttpPost]
        public PartialViewResult SortBoardAlt(int seqNo, string action)
        {
            TransactionDataModel model = new TransactionDataModel();

            model = SessionExtentions.GetSession<TransactionDataModel>(HttpContext.Session, "TransactionDataModel");
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                model.modelProductSpec = _productSpecService.SortBoardAlt(model, seqNo, action);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return PartialView("_AddBoard", model.modelProductSpec);
        }

        [SessionTimeout]
        public IActionResult ShowBoardAlt()
        {
            ProductSpecViewModel modelToReturn = new ProductSpecViewModel();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                modelToReturn = _productSpecService.ShowBoardAlt();
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return PartialView("_AddBoard", modelToReturn);
        }

        [SessionTimeout]
        [HttpPost]
        public void SaveImageFileSpec(IFormFile PrintMaster)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                string base64StringImage = _extensionService.ConvertImageToBase64(PrintMaster);
                HttpContext.Session.SetString("PrintMaster", base64StringImage);
                HttpContext.Session.SetString("PrintMasterName", PrintMaster.FileName);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        [SessionTimeout]
        public void RemoveFileSpec()
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                HttpContext.Session.Remove("PrintMaster");
                HttpContext.Session.Remove("PrintMasterName");
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult SaveProductSpec(TransactionDataModel temp, IFormFile PrintMaster)
        {
            bool isSuccess;
            TransactionDataModel model = new TransactionDataModel();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                //string[] fileName = _newProductService.UploadPicture(PrintMaster, _environment, HttpContext);
                _productSpecService.SaveDataToModel_ProductSpec(temp, PrintMaster);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                isSuccess = true;
                //return RedirectToAction("ProductProp", "NewProduct");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                //throw ex;
                isSuccess = false;
                //if (ex.Message == "Input string was not in a correct format.")
                //{
                //    ViewBag.Message = string.Format("Plase fill the box", "", DateTime.Now.ToString());
                //}
                //return RedirectToAction("ProductSpec", "NewProduct");
            }
            return Json(new { isSuccess = isSuccess });
        }

        #endregion ProductSpec

        #region ProductProp

        [SessionTimeout]
        public IActionResult ProductProp()
        {
            TransactionDataModel transactionDataModel = new TransactionDataModel();
            try
            {
                //System.Reflection.AssemblyName assembly = System.Reflection.Assembly.GetExecutingAssembly().GetName();

                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _productPropService.ProductPropData(ref transactionDataModel);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                // Redirect To Error Page
            }
            return View(transactionDataModel);
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult SaveProductProp(TransactionDataModel transactionDataModel)
        {
            bool isSuccess;
            string exeptionMessage = string.Empty;

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _productPropService.SaveProductProp(ref transactionDataModel);
                //update changeInfo
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                isSuccess = true;
            }
            catch (Exception ex)
            {
                exeptionMessage = ex.Message;
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                isSuccess = false;
            }

            return Json(new { isSuccess = isSuccess, exeptionMessage = exeptionMessage, materialNo = transactionDataModel.MaterialNo });
        }

        [SessionTimeout]
        public PartialViewResult _ProductProp_SetProperty(TransactionDataModel model)
        {
            return PartialView(model);
        }

        [SessionTimeout]
        public PartialViewResult _ProductProp_SpareProperty(TransactionDataModel model)
        {
            return PartialView(model);
        }

        [SessionTimeout]
        public PartialViewResult _ProductProp_ProcessProperty_Digital(TransactionDataModel model)
        {
            return PartialView(model);
        }

        [SessionTimeout]
        public PartialViewResult _ProductProp_PackingProperty_Digital(TransactionDataModel model)
        {
            return PartialView(model);
        }

        [SessionTimeout]
        public PartialViewResult _ProductProp_ProcessProperty_NewCG_HoneyCore_HoneyComb(TransactionDataModel model)
        {
            return PartialView(model);
        }

        [SessionTimeout]
        public PartialViewResult _ProductProp_PackingProperty_NewCG(TransactionDataModel model)
        {
            return PartialView(model);
        }

        [SessionTimeout]
        public PartialViewResult _ProductProp_PackingProperty_HoneyComb(TransactionDataModel model)
        {
            return PartialView(model);
        }

        [SessionTimeout]
        public PartialViewResult _ProductProp_PackingProperty_HoneyCore(TransactionDataModel model)
        {
            return PartialView(model);
        }

        [SessionTimeout]
        public PartialViewResult _ProductProp_Standard(TransactionDataModel model)
        {
            return PartialView(model);
            return PartialView(model);
        }

        [SessionTimeout]
        public JsonResult GetPallet(TransactionDataModel transactionDataModel, string JoinId, string palletSize, int WidDC, int LegDC, int Overhang)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var JoinTypeFilter = JoinId;
                var palletSizeFilter = palletSize;
                _productPropService.GetPallet(ref transactionDataModel, JoinTypeFilter, palletSizeFilter, WidDC, LegDC, Overhang);
                var modelSession = SessionExtentions.GetSession<TransactionDataModel>(HttpContext.Session, "TransactionDataModel");
                var picpallet = modelSession.modelProductProp.PicPallet;
                string bunlayer = modelSession.modelProductProp.BunLayer.ToString();
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                //var aaa = "";
                return Json(new { resultPallet = picpallet, bunlayer = bunlayer });
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json("");
            }
        }

        //[HttpPost]
        //public JsonResult Compute(ProductSpecViewModel model)
        //{
        //    var exe = _productSpecService.ComputeRSC(model);
        //    return Json(exe);
        //}

        #region [Temp]

        //[HttpPost]
        //public IActionResult SaveProductProp(TransactionDataModel transactionDataModel)
        //{
        //    try
        //    {
        //       // _productPicService.PicData(ref transactionDataModel, _environment);
        //        _productPropService.SaveProductProp(transactionDataModel);

        //        return RedirectToAction("Routing");
        //    }
        //    catch (Exception ex)
        //    {
        //        return RedirectToAction("ProductProp");

        //    }
        //    //    TransactionDataModel model = new TransactionDataModel();

        //    //    if (ModelState.IsValid)
        //    //    {
        //    //        try
        //    //        {
        //    //           // model = _productPropService.ProductProp(ProductProp);
        //    //            _productPropService.SaveProductProp(transactionDataModel);

        //    //         //   model = _productPropService.SavePropData(model);
        //    ////            //model = ProductPicService.UpdateData(context, sessions, ProductProp, model.MaterialNo, model, _environment, "ProductProp");

        //    ////            SessionExtentions.SetSession(HttpContext.Session, "TransactionDataModel", model);
        //    //        }
        //    //        catch (Exception ex)
        //    //        {
        //    //       }

        //    //       return RedirectToAction("Routing");
        //    //  }

        //    //    model = SessionExtentions.GetSession<TransactionDataModel>(HttpContext.Session, "TransactionDataModel");
        //    //    //model = ProductPicService.PicData(HttpContext, _environment, context);
        //    //    model = _productPropService.ProductPropData(model);

        //    //    SessionExtentions.SetSession(HttpContext.Session, "TransactionDataModel", model);

        //    //    _newProductService.SetTransactionStatus(ref model, "ProductProperties");
        //    //   return View(model);
        //    //   return View();
        //    //    //return View(ProductProp);
        //}

        //[HttpPost]
        //public PartialViewResult UpdatePropChangeInfo(TransactionDataModel ProductProp)
        //{
        //    TransactionDataModel model = new TransactionDataModel();
        //    try
        //    {
        //        model = _productPropService.ProductProp(ProductProp);
        //        SessionsModel sessions = new SessionsModel
        //        {
        //            UserName = HttpContext.Session.GetString("username"),
        //            SaleOrg = HttpContext.Session.GetString("SALE_ORG"),
        //            PlantCode = HttpContext.Session.GetString("PLANT_CODE")
        //        };

        //        model = _productPropService.SavePropData(model);

        //        SessionExtentions.SetSession(HttpContext.Session, "TransactionDataModel", model);
        //    }
        //    catch (Exception ex)
        //    {
        //    }

        //    // return PartialView("_ERPPlantView", model);
        //    return PartialView("_ChangeHistory", model);
        //}

        //[HttpPost]
        //public PartialViewResult UpdateHistory(TransactionDataModel ProductProp)
        //{
        //    TransactionDataModel model = new TransactionDataModel();

        //    // model = ProductPropService.UpdateChangeHisData(context, HttpContext, model);
        //    return PartialView("_ChangeHistory", model);
        //}

        #endregion [Temp]

        #endregion ProductProp

        #region Routing

        [SessionTimeout]
        public IActionResult Routing()
        {
            var model = SessionExtentions.GetSession<TransactionDataModel>(HttpContext.Session, "TransactionDataModel");
            try
            {
                //_routingService.AutoRoutingList(model);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                model.amountColor = model.modelProductProp.AmountColor == null ? 0 : model.modelProductProp.AmountColor;
                if (model.EventFlag == "Create")
                {
                    _routingService.BindDataToModel(model);
                    ////if(model.modelRouting.Machine == null)
                    ////{
                    ////    _routingService.AutoRoutingList(model);
                    ////}
                    /////=== clear routing กรณีที่เป็น digital
                    ////if(model.RealEventFlag == "Copy" && (model.modelCategories.FormGroup == "DIGITAL" || model.modelCategories.FormGroup == "BOM" || model.modelCategories.FormGroup == "TCG" )
                    ////|| (model.modelCategories.MatCode == "14" || model.modelCategories.MatCode == "24" || model.modelCategories.MatCode == "84" || model.modelCategories.MatCode == "82"))
                    //if (model.RealEventFlag != "Copy")
                    //{
                    //    // clear routing
                    //    model.modelRouting.RoutingDataList.Clear();

                    //}
                }
                else if (model.EventFlag == "Presale")
                {
                    _routingService.InitialPresaleRouting();
                }

                if (model.EventFlag == "Create" && model.modelRouting.RoutingDataList.Count() > 0)
                {
                    string plantcode = "";
                    if (!string.IsNullOrEmpty(model.PlantOs))
                    {
                        plantcode = model.PlantOs;
                    }
                    else if (!string.IsNullOrEmpty(model.modelProductInfo.PLANTCODE))
                    {
                        plantcode = model.modelProductInfo.PLANTCODE;
                    }

                    if (plantcode == "") //check ว่าเป็นงานจ้างหรือไม่ ถ้ามี Plancode = ใช่
                    {
                        if (model.modelCategories.MatCode == "84" || model.modelCategories.MatCode == "24" || model.modelCategories.MatCode == "14")
                        {
                            var tmp = model.modelRouting.RoutingDataList.OrderByDescending(x => x.SeqNo).Take(1).ToList();
                            model.modelRouting.RoutingDataList.Clear();
                            model.modelRouting.RoutingDataList = tmp;
                            model.modelRouting.RoutingDataList.ForEach(f => f.SeqNo = 1);
                        }
                        _routingService.SaveRouting(model);
                    }
                    else
                    {
                    }
                }
                _newProductService.SetTransactionStatus(ref model, "ProductRouting");

                //if (model.modelRouting.RoutingDataList.Count() == 0)
                //{
                //    _routingService.AutoRoutingList(model);
                //}
                if (model.modelRouting != null)
                {
                    if (model.modelRouting.RoutingDataList != null)
                    {
                        model.modelRouting.RoutingDataList = model.modelRouting.RoutingDataList.OrderBy(x => x.SeqNo).ToList();
                    }
                }
                SessionExtentions.SetSession(HttpContext.Session, "TransactionDataModel", model);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return View(model);
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return View("");
            }
        }

        [SessionTimeout]
        [HttpPost]
        public PartialViewResult RoutingAddMachine(TransactionDataModel transactionDataModel)
        {
            var model = SessionExtentions.GetSession<TransactionDataModel>(HttpContext.Session, "TransactionDataModel");
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                RoutingDataModel routingModel = new RoutingDataModel();
                int seqNum = 0;
                _routingService.MappingModelRouting(model, transactionDataModel, ref routingModel, ref seqNum);
                model.modelRouting.RoutingDataList.Add(routingModel);
                _routingService.SaveRouting(model);
                model.modelRouting.RoutingDataList.Clear();
                model.modelRouting.RoutingDataList = _routingService.GetRoutingList(model.MaterialNo).OrderBy(x => x.SeqNo).ToList();
                var remarkAttachFileBase64 = HttpContext.Session.GetString("remarkAttachFile");
                if (remarkAttachFileBase64 != null)
                {
                    model.modelRouting.RoutingDataList.First(i => i.SeqNo == seqNum).RemarkImageFile = remarkAttachFileBase64;
                    model.modelRouting.RoutingDataList.First(i => i.SeqNo == seqNum).RemarkImageFileName = HttpContext.Session.GetString("remarkAttachFileName");
                    model.modelRouting.RoutingDataList.First(i => i.SeqNo == seqNum).RemarkAttachFileStatus = 1;
                    HttpContext.Session.Remove("remarkAttachFile");
                    HttpContext.Session.Remove("remarkAttachFileName");
                }
                SessionExtentions.SetSession(HttpContext.Session, "TransactionDataModel", model);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return PartialView("_RoutingDataList", model);
        }

        [SessionTimeout]
        [HttpPost]
        public PartialViewResult RoutingUpdateMachine(TransactionDataModel modelToUpdate)
        {
            var model = SessionExtentions.GetSession<TransactionDataModel>(HttpContext.Session, "TransactionDataModel");
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var routingModel = _routingService.MappingModelRoutingUpdateAndDelete(model, modelToUpdate);

                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Data Main => " + JsonConvert.SerializeObject(model));
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Data Trans => " + JsonConvert.SerializeObject(modelToUpdate));
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Data Update => " + JsonConvert.SerializeObject(routingModel));

                model = _routingService.UpdateRouting(model, modelToUpdate, routingModel);
                SessionExtentions.SetSession(HttpContext.Session, "TransactionDataModel", model);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return PartialView("_RoutingDataList", model);
        }

        [SessionTimeout]
        [HttpPost]
        public PartialViewResult RoutingInsertMachine(TransactionDataModel transactionDataModel)
        {
            var model = SessionExtentions.GetSession<TransactionDataModel>(HttpContext.Session, "TransactionDataModel");
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var routingModel = _routingService.MappingModelRoutingUpdateAndDelete(model, transactionDataModel);
                model = _routingService.InsertRouting(model, transactionDataModel, routingModel);
                SessionExtentions.SetSession(HttpContext.Session, "TransactionDataModel", model);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return PartialView("_RoutingDataList", model);
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult GetMachine(string keywordMachine, string machineGroup)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var MachineList = _routingService.GetMachineList(keywordMachine, machineGroup);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(MachineList);
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json("");
            }
        }

        [SessionTimeout]
        public JsonResult GetWeight()
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var modelSession = SessionExtentions.GetSession<TransactionDataModel>(HttpContext.Session, "TransactionDataModel");
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(_routingService.GetWeight(modelSession));
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json("");
            }
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult GetCalculateCorProp(string machine, int wid)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var modelTrans = SessionExtentions.GetSession<TransactionDataModel>(HttpContext.Session, "TransactionDataModel");
                RoutingDataModel CalculateRoutingData = new RoutingDataModel();
                string exError = "";
                try
                {
                    exError = "0";
                    CalculateRoutingData = _routingService.CalculateRouting(machine, modelTrans, wid);
                }
                catch
                {
                    exError = "1";
                }
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                //var CalculateRoutingData = _routingService.CalculateRouting(machine);
                return Json(new { PaperRollWidth = CalculateRoutingData.PaperRollWidth, Cut = CalculateRoutingData.Cut, Trim = CalculateRoutingData.Trim, PercentTrim = CalculateRoutingData.PercentTrim, exError = exError });
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json("");
            }
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult GetRemark(string keywordRemark)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var RemarkList = _routingService.GetRemarkList(keywordRemark);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(RemarkList);
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json("");
            }
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult GetMachineGroup(string machine)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                string MachineGroup = _routingService.GetMachineGroupByMachine(machine);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(new { MachineGroup = MachineGroup });
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json("");
            }
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult GetMachineAfterSelect(string Machine)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var model = _routingService.GetMachine(Machine);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(model);
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json("");
            }
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult GetMachineGroupToMangeSheetlength()
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var model = SessionExtentions.GetSession<TransactionDataModel>(HttpContext.Session, "TransactionDataModel");
                string countMatchie = "0";
                foreach (var item in model.modelRouting.RoutingDataList)
                {
                    string MachineGroup = _routingService.GetMachineGroupByMachine(item.Machine);
                    if (MachineGroup == "2")
                    {
                        countMatchie = "1";
                    }
                }
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(new { CheckMachineCount = countMatchie });
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json("");
            }
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult GetDataToCondition(string machine)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var dataToCondition = _routingService.GetPlatenRotary(machine);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(new { isPropCor = dataToCondition[0], isPropPrint = dataToCondition[1], isPropDieCut = dataToCondition[2], isCalPaperwidth = dataToCondition[3], IsRepeatLength = dataToCondition[4], IsMCMove = dataToCondition[5] });
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json("");
            }
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult GetRoutingData(int seqNo)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var model = SessionExtentions.GetSession<TransactionDataModel>(HttpContext.Session, "TransactionDataModel");
                model.modelRouting.RoutingDataList.Clear();
                model.modelRouting.RoutingDataList = _routingService.GetRoutingList(model.MaterialNo).Where(x => x.SeqNo == seqNo).ToList();

                // var RoutingData = model.modelRouting.RoutingDataList.Where(w => w.SeqNo == seqNo).ToList();
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(model.modelRouting.RoutingDataList);
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json("");
            }
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult GetScoreGab(string flut, string scoretypeid)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var shadeList = _routingService.GetScoreGapList(flut, scoretypeid);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(shadeList);
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json("");
            }
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult GetShadeByInk(string inkName)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var shadeList = _routingService.GetShadeByInkList(inkName);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(shadeList);
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json("");
            }
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult GetInkByShade(string shadeName)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var inkList = _routingService.GetInkByShadeList(shadeName);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(inkList);
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json("");
            }
        }

        [SessionTimeout]
        [HttpPost]
        public void SaveImageFileToTemp(IFormFile remarkAttachFile)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                string base64StringImage = _extensionService.ConvertImageToBase64(remarkAttachFile);
                HttpContext.Session.SetString("remarkAttachFile", base64StringImage);
                HttpContext.Session.SetString("remarkAttachFileName", remarkAttachFile.FileName);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        [SessionTimeout]
        public void RemoveAttachFile()
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                HttpContext.Session.Remove("remarkAttachFile");
                HttpContext.Session.Remove("remarkAttachFileName");
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        [SessionTimeout]
        [HttpPost]
        public PartialViewResult RoutingCopy(int seqNo)
        {
            var model = SessionExtentions.GetSession<TransactionDataModel>(HttpContext.Session, "TransactionDataModel");
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                model = _routingService.CopyRouting(model, seqNo);
                SessionExtentions.SetSession(HttpContext.Session, "TransactionDataModel", model);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return PartialView("_RoutingDataList", model);
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return PartialView("_RoutingDataList", model);
        }

        [SessionTimeout]
        [HttpPost]
        public PartialViewResult RoutingDelete(string Mat, int seqNo)
        {
            var model = SessionExtentions.GetSession<TransactionDataModel>(HttpContext.Session, "TransactionDataModel");
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                //int seqNumber = 1;

                //model.modelRouting.RoutingDataList.RemoveAll(w => w.SeqNo == seqNo);

                //model.modelRouting.RoutingDataList.ForEach(i => { i.SeqNo = seqNumber; seqNumber++; });
                _routingService.DeleteRoutingByMaterialNoAndFactoryAndSeq(Mat, seqNo.ToString());

                model.modelRouting.RoutingDataList.Clear();
                model.modelRouting.RoutingDataList = _routingService.GetRoutingList(model.MaterialNo).OrderBy(x => x.SeqNo).ToList();
                int seqNumber = 1;
                // model.modelRouting.RoutingDataList.RemoveAll(w => w.SeqNo == seqNo);
                model.modelRouting.RoutingDataList.ForEach(i => { i.SeqNo = seqNumber; seqNumber++; });
                _routingService.UpdateRouting(model);

                SessionExtentions.SetSession(HttpContext.Session, "TransactionDataModel", model);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return PartialView("_RoutingDataList", model);
        }

        [SessionTimeout]
        [HttpPost]
        public PartialViewResult RoutingSort(int seqNo, string action)
        {
            var model = SessionExtentions.GetSession<TransactionDataModel>(HttpContext.Session, "TransactionDataModel");
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                int seqNumber = 1;
                if (model.modelRouting.RoutingDataList.Count > 0)
                {
                    var itemToMove = model.modelRouting.RoutingDataList[seqNo - 1];
                    if (action == "Up" && (seqNo - 1) != 0)
                    {
                        model.modelRouting.RoutingDataList.RemoveAt(seqNo - 1);
                        model.modelRouting.RoutingDataList.Insert(seqNo - 2, itemToMove);
                    }
                    else if (action == "Down" && (seqNo - 1) != model.modelRouting.RoutingDataList.Count - 1)
                    {
                        model.modelRouting.RoutingDataList.RemoveAt(seqNo - 1);
                        model.modelRouting.RoutingDataList.Insert(seqNo, itemToMove);
                    }

                    model.modelRouting.RoutingDataList.ForEach(i => { i.SeqNo = seqNumber; seqNumber++; });
                }

                _routingService.UpdateRouting(model);
                model.modelRouting.RoutingDataList.Clear();
                model.modelRouting.RoutingDataList = _routingService.GetRoutingList(model.MaterialNo).OrderBy(x => x.SeqNo).ToList();
                SessionExtentions.SetSession(HttpContext.Session, "TransactionDataModel", model);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return PartialView("_RoutingDataList", model);
        }

        [SessionTimeout]
        public JsonResult CheckValidateSubmitRouting()
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                bool result = false;

                var model = SessionExtentions.GetSession<TransactionDataModel>(HttpContext.Session, "TransactionDataModel");

                if (model.modelRouting.RoutingDataList.Count > 0 && model.modelRouting.RoutingDataList[model.modelRouting.RoutingDataList.Count - 1].Machine.Contains("คลัง"))
                {
                    result = true;
                }
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(new { Result = result });
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json("");
            }
        }

        [SessionTimeout]
        public JsonResult CheckPresaleRoutingList()
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                bool result = false;
                var model = SessionExtentions.GetSession<TransactionDataModel>(HttpContext.Session, "TransactionDataModel");
                foreach (var item in model.modelRouting.RoutingDataList)
                {
                    string tmp = _routingService.GetMachineGroupByMachine(item.Machine);
                    if (tmp == "")
                    { result = true; }
                }

                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(new { Result = result });
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json(true);
            }
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult ValidateOutsorcingBeforeSubmit()
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                bool result = false;
                var model = SessionExtentions.GetSession<TransactionDataModel>(HttpContext.Session, "TransactionDataModel");
                string plantcode = "";
                if (!string.IsNullOrEmpty(model.PlantOs))
                {
                    plantcode = model.PlantOs;
                }
                else if (!string.IsNullOrEmpty(model.modelProductInfo.PLANTCODE))
                {
                    plantcode = model.modelProductInfo.PLANTCODE;
                }
                var machineTmp = _routingService.GetMachineDataByFactorycode(plantcode);
                foreach (var item in model.modelRouting.RoutingDataList)
                {
                    var checkismachine = machineTmp.Where(x => x.Machine1 == item.Machine).FirstOrDefault();
                    if (checkismachine == null)
                    {
                        result = true;
                    }
                }

                //if (model.EventFlag == "Create")
                //{ result = false ; }

                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(new { Result = result });
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json(true);
            }
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult UpdatePlantByPlantOS()
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                bool result = true;
                var model = SessionExtentions.GetSession<TransactionDataModel>(HttpContext.Session, "TransactionDataModel");
                string plantcode = "";
                if (!string.IsNullOrEmpty(model.PlantOs))
                {
                    plantcode = model.PlantOs;
                }
                else if (!string.IsNullOrEmpty(model.modelProductInfo.PLANTCODE))
                {
                    plantcode = model.modelProductInfo.PLANTCODE;
                }
                model.modelRouting.RoutingDataList.ToList().ForEach(s => s.Plant = plantcode);
                SessionExtentions.SetSession(HttpContext.Session, "TransactionDataModel", model);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(new { Result = result });
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json(false);
            }
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult CheckProcessCoreIs0()
        {
            string status0 = "0";
            try
            {
                var model = SessionExtentions.GetSession<TransactionDataModel>(HttpContext.Session, "TransactionDataModel");

                foreach (var item in model.modelRouting.RoutingDataList)
                {
                    var group = _routingService.GetMachineGroupByMachine(item.Machine);
                    var dataToCondition = _routingService.GetPlatenRotary(item.Machine);
                    if (group == "1" && dataToCondition[0] == 1)
                    {
                        if (string.IsNullOrEmpty(item.PaperRollWidth) || item.PaperRollWidth == "0")
                        {
                            status0 = "1";
                        }
                    }
                }
            }
            catch
            {
            }
            return Json(status0);
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult UpdateAndSavePlantByPlantOS()
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                bool result = true;
                var model = SessionExtentions.GetSession<TransactionDataModel>(HttpContext.Session, "TransactionDataModel");
                string plantcode = "";
                if (!string.IsNullOrEmpty(model.PlantOs))
                {
                    plantcode = model.PlantOs;
                }
                else if (!string.IsNullOrEmpty(model.modelProductInfo.PLANTCODE))
                {
                    plantcode = model.modelProductInfo.PLANTCODE;
                }

                if (model.SapStatus)
                {
                    model.modelRouting.RoutingDataList.ToList().ForEach(s => s.Plant = plantcode);
                    _routingService.SaveRouting(model);
                    if (model.SapStatus)
                    {
                        _routingService.UpdatePDISStatus(plantcode, model.MaterialNo, "M");
                    }
                    else
                    {
                        _routingService.UpdatePDISStatus(plantcode, model.MaterialNo, "C");
                    }
                }
                else
                {
                    // _routingService.SaveRouting(model);
                    List<RoutingDataModel> tmpsum = new List<RoutingDataModel>();
                    List<RoutingDataModel> tmpfac = new List<RoutingDataModel>();
                    List<RoutingDataModel> lsfac = new List<RoutingDataModel>();
                    tmpfac = model.modelRouting.RoutingDataList;
                    lsfac = RecurRoutinglistList(tmpfac, model.FactoryCode);//tmprouting1.Select(c => { c.Plant = model.FactoryCode; return c; }).ToList();
                    model.modelRouting.RoutingDataList.Clear();
                    model.modelRouting.RoutingDataList = lsfac;
                    _routingService.SaveRouting(model);

                    if (model.SapStatus)
                    {
                        _routingService.UpdatePDISStatus(model.FactoryCode, model.MaterialNo, "M");
                    }
                    else
                    {
                        _routingService.UpdatePDISStatus(model.FactoryCode, model.MaterialNo, "C");
                    }

                    //tmpsum.InsertRange(0, ccc);
                    List<RoutingDataModel> tmpplant = new List<RoutingDataModel>();
                    List<RoutingDataModel> lsplant = new List<RoutingDataModel>();
                    if (model.RealEventFlag != "CopyOs")
                    {
                        if (model.RealEventFlag != "CreateOs" && model.RealEventFlag != "Presale")
                        {
                            tmpplant = model.modelRouting.RoutingDataList;
                            lsplant = RecurRoutinglistList2(tmpplant, plantcode);//tmprouting2.Select(d => { d.Plant = plantcode; return d; }).ToList();
                            model.modelRouting.RoutingDataList.Clear();
                            model.modelRouting.RoutingDataList = lsplant;
                            model.MaterialNo = string.IsNullOrEmpty(model.modelProductInfo.MatOursource) ? model.MaterialNo : model.modelProductInfo.MatOursource;
                            if (!string.IsNullOrEmpty(model.modelProductInfo.MatOursource))
                            {
                                model.modelRouting.RoutingDataList.ToList().ForEach(x => x.Plant = plantcode);
                            }
                            _routingService.SaveRouting(model);
                        }
                    }

                    //tmpsum.InsertRange(ccc.Count - 1 ,xxx);
                    if (model.SapStatus)
                    {
                        _routingService.UpdatePDISStatus(plantcode, model.MaterialNo, "M");
                    }
                    else
                    {
                        _routingService.UpdatePDISStatus(plantcode, model.MaterialNo, "C");
                    }

                    //foreach (var ite in lsfac)
                    //{
                    //    RoutingDataModel list = new RoutingDataModel();
                    //    list = ite;
                    //    tmpsum.Add(list);
                    //}

                    //foreach (var ite2 in lsplant)
                    //{
                    //    RoutingDataModel list = new RoutingDataModel();
                    //    list = ite2;
                    //    tmpsum.Add(list);
                    //}

                    //model.modelRouting.RoutingDataList.ToList().ForEach(s => s.Plant = model.FactoryCode);
                    //tmpfac = model.modelRouting.RoutingDataList;
                    //tmpsum.AddRange(tmpfac);
                    //model.modelRouting.RoutingDataList.ToList().ForEach(s => s.Plant = plantcode);
                    //tmpplant= model.modelRouting.RoutingDataList;
                    //tmpsum.AddRange(tmpplant);

                    //foreach (var itemlst in tmpnew)
                    //{
                    //    tmp.Add(itemlst);
                    //}
                }

                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(new { Result = result });
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json(false);
            }
        }

        public List<RoutingDataModel> RecurRoutinglistList(List<RoutingDataModel> List, string Expression)
        {
            List<RoutingDataModel> result = new List<RoutingDataModel>();
            result = List.Select(d => { d.Plant = Expression; return d; }).ToList();
            return result;
        }

        public List<RoutingDataModel> RecurRoutinglistList2(List<RoutingDataModel> List2, string Expression)
        {
            List<RoutingDataModel> result = new List<RoutingDataModel>();
            result = List2.Select(dd => { dd.Plant = Expression; return dd; }).ToList();
            return result;
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult SaveRouting(string arraydiecut, string arrayprint)// ,   TransactionDataModel modelToSave)
        {
            bool result;

            //Routing modelToSave = new Routing();
            //modelToSave = JsonConvert.DeserializeObject<Routing>(transaction);

            var model = SessionExtentions.GetSession<TransactionDataModel>(HttpContext.Session, "TransactionDataModel");
            Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
            try
            {
                if (model.EventFlag == "Create" || model.EventFlag == "Copy" || model.EventFlag == "Presale")
                {
                    _routingService.SaveRouting(model);
                    model = SessionExtentions.GetSession<TransactionDataModel>(HttpContext.Session, "TransactionDataModel");
                    model.modelProductSpec.FlagRouting = 1;
                    if (model.SapStatus)
                    {
                        _routingService.UpdatePDISStatus(model.FactoryCode, model.MaterialNo, "M");
                    }
                    else
                    {
                        _routingService.UpdatePDISStatus(model.FactoryCode, model.MaterialNo, "C");
                    }

                    foreach (var item in model.modelRouting.RoutingDataList)
                    {
                        if (item.PrintingPlateType == "Semi" || item.PrintingPlateType == "Shipping Mark")
                        {
                            model.modelProductERP = new ProductERPPlantViewModel();
                            _routingService.UpdatePlantViewShipBlk(model.MaterialNo, "X");
                            model.modelProductERP.ShipBlk = "X";
                            break;
                        }
                    }
                }
                else if (model.EventFlag == "Edit")
                {
                    if (model.RealEventFlag != "Copy")
                    {
                        _routingService.UpdateRouting(model);
                        model = SessionExtentions.GetSession<TransactionDataModel>(HttpContext.Session, "TransactionDataModel");
                        model.modelProductSpec.FlagRouting = 1;
                        if (model.SapStatus)
                        {
                            _routingService.UpdatePDISStatus(model.FactoryCode, model.MaterialNo, "M");
                        }
                        else
                        {
                            _routingService.UpdatePDISStatus(model.FactoryCode, model.MaterialNo, "C");
                        }
                        foreach (var item in model.modelRouting.RoutingDataList)
                        {
                            if (item.PrintingPlateType == "Semi" || item.PrintingPlateType == "Shipping Mark")
                            {
                                model.modelProductERP = new ProductERPPlantViewModel();
                                _routingService.UpdatePlantViewShipBlk(model.MaterialNo, "X");
                                model.modelProductERP.ShipBlk = "X";
                                break;
                            }
                        }
                    }

                    //   var tmp = model.modelRouting.RoutingDataList.OrderByDescending(x => x.SeqNo).FirstOrDefault();
                }
                _routingService.UpdateRoutingPDISStatusEmployment(model.MaterialNo, "", model.SaleOrg);

                //check sent unhold to presale

                //check mat is PresaleChangeProduct
                var checkPresaleChangeProduct = _presaleService.GetPresaleChangeProductByMaterialNo(model.MaterialNo);
                if (checkPresaleChangeProduct != null)
                {
                    //Verify pre-approval of sales holds
                    _presaleService.UpdateUnHoldToPresale(model.MaterialNo);
                }

                SessionExtentions.SetSession(HttpContext.Session, "TransactionDataModel", model);
                result = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                result = false;
            }

            return Json(new { Result = result });
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult RenderTblRemark(string Machine)
        {
            var model = SessionExtentions.GetSession<TransactionDataModel>(HttpContext.Session, "TransactionDataModel");
            TransactionDataModel trans = new TransactionDataModel();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                trans.modelBuildRemark = model.modelBuildRemark.Where(x => x.Machine == Machine).OrderBy(x => x.List).ToList();
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return Json(RenderView.RenderRazorViewToString(this, "_Routing_TblRemarkList", trans));
        }

        [SessionTimeout]
        public JsonResult RenderTblRemarkNosearch()
        {
            var model = SessionExtentions.GetSession<TransactionDataModel>(HttpContext.Session, "TransactionDataModel");
            TransactionDataModel trans = new TransactionDataModel();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                trans.modelBuildRemark = model.modelBuildRemark.OrderBy(x => x.List).ToList();
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return Json(RenderView.RenderRazorViewToString(this, "_Routing_TblRemarkList", trans));
        }

        [SessionTimeout]
        public ActionResult _Routing_TblRemarkList()
        {
            TransactionDataModel ms = new TransactionDataModel();
            //var ms = SessionExtentions.GetSession<TransactionDataModel>(HttpContext.Session, "TransactionDataModel");
            return PartialView(ms);
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult CheckPlattenAndRotary(string machine)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                string checkplaten = "";
                var data = _routingService.GetMachineDataPlatenAndRotalyByMachine(machine);
                if (data) { checkplaten = "1"; } else { checkplaten = "0"; }
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(new { CheckPlatenRotary = checkplaten });
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json("");
            }
        }

        [SessionTimeout]
        public JsonResult CheckDisableButtonSave(string MaterialNo)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                string checkbtn = "";
                var routingList = _routingService.GetRoutingList(MaterialNo).OrderByDescending(x => x.SeqNo).FirstOrDefault();
                if (routingList != null)
                {
                    if (routingList.Machine.Contains("คลัง"))
                    { checkbtn = "1"; }
                    else { checkbtn = "0"; }
                }
                else { checkbtn = "0"; }

                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(new { CheckButtonEnable = checkbtn });
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json("");
            }
        }

        [SessionTimeout]
        public JsonResult CheckDisableButtonSaveByCopy(string MaterialNo)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                string checkbtn = "";
                var model = SessionExtentions.GetSession<TransactionDataModel>(HttpContext.Session, "TransactionDataModel");
                var routingList = model.modelRouting.RoutingDataList.OrderByDescending(x => x.SeqNo).FirstOrDefault();
                if (routingList != null)
                {
                    if (routingList.Machine.Contains("คลัง"))
                    { checkbtn = "1"; }
                    else { checkbtn = "0"; }
                }
                else { checkbtn = "0"; }

                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(new { CheckButtonEnable = checkbtn });
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json("");
            }
        }

        [SessionTimeout]
        public JsonResult CheckCoreConfixTearTapeMax(string machine, int cut, int line)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                string checkteartape = "";
                string TearTapeNewCut = "";
                string LineMax = "";
                string LineMaxVal = "";
                var CorConfigLst = _routingService.GetCoreConfig().Where(x => x.Name == machine).FirstOrDefault();
                if (CorConfigLst != null)
                {
                    if (line > CorConfigLst.TearTapeMax)
                    {
                        LineMaxVal = CorConfigLst.TearTapeMax.ToString();
                        LineMax = "1";
                    }
                    else
                    {
                        LineMaxVal = "0";
                        double sum = cut * line;
                        if (sum > CorConfigLst.TearTapeMax)
                        {
                            string cutlast = Math.Round(Convert.ToDecimal(CorConfigLst.TearTapeMax / line)).ToString();
                            TearTapeNewCut = cutlast;
                            checkteartape = "1";
                        }
                        else
                        {
                            checkteartape = "0";
                        }
                    }
                }
                else
                {
                    checkteartape = "0";
                }
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(new { CheckTearTape = checkteartape, TearTapeNewCut = TearTapeNewCut, LineMax = LineMax, LineMaxVal = LineMaxVal });
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json("");
            }
        }

        [SessionTimeout]
        public JsonResult CheckRemarkForFlex()
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                string CheckFlexResult = "";
                try
                {
                    var model = SessionExtentions.GetSession<TransactionDataModel>(HttpContext.Session, "TransactionDataModel");
                    var checkflex = model.modelRouting.RoutingDataList.Where(x => x.Remark.Contains("ลิ้นกาวมีหาง")).ToList();
                    if (checkflex.Count > 0)
                    {
                        CheckFlexResult = "1";
                    }
                    else { CheckFlexResult = "0"; }
                }
                catch
                {
                    CheckFlexResult = "0";
                }
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(new { CheckFlexResult = CheckFlexResult });
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json("");
            }
        }

        [SessionTimeout]
        public JsonResult CalculateNewCut(string Cut, string WidthIn, string Flut, string Machine)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var modelTrans = SessionExtentions.GetSession<TransactionDataModel>(HttpContext.Session, "TransactionDataModel");
                RoutingDataModel model = new RoutingDataModel();
                string exError = "";
                try
                {
                    model = _routingService.CalculateNewCut(Cut, WidthIn, Flut, modelTrans.MaterialNo, Machine);
                    exError = "1";
                }
                catch
                {
                    exError = "0";
                }
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(new { model = model, exError = exError, cut = Cut });
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json("");
            }
        }

        [SessionTimeout]
        public JsonResult CalculatePaperRollWidth(string PaperwWidth, string WidthIn, string Flut, string Cut)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                RoutingDataModel model = new RoutingDataModel();
                string exError = "";
                try
                {
                    model = _routingService.CalculateNewPaperRoll(PaperwWidth, WidthIn, Flut, Cut);
                    exError = "1";
                }
                catch
                {
                    exError = "0";
                }
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(new { model = model, exError = exError });
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json("");
            }
        }

        [SessionTimeout]
        public JsonResult ReCalculateRouting(string machine, int wid)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var modelTrans = SessionExtentions.GetSession<TransactionDataModel>(HttpContext.Session, "TransactionDataModel");
                var CalculateRoutingData = _routingService.CalculateRouting(machine, modelTrans, wid);
                //var CalculateRoutingData = _routingService.CalculateRouting(machine);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(new { PaperRollWidth = CalculateRoutingData.PaperRollWidth, Cut = CalculateRoutingData.Cut, Trim = CalculateRoutingData.Trim, PercentTrim = CalculateRoutingData.PercentTrim });
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json("");
            }
        }

        [SessionTimeout]
        public JsonResult GetLastNumberInRouting()
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                string CheckData = "";
                string LastNoOut = "";
                string LastSheetWOut = "";
                string LastSheetLOut = "";
                string LastWidthOut = "";
                var model = SessionExtentions.GetSession<TransactionDataModel>(HttpContext.Session, "TransactionDataModel");
                var checkflex = model.modelRouting.RoutingDataList.OrderByDescending(x => x.SeqNo).FirstOrDefault();
                if (checkflex != null)
                {
                    LastNoOut = checkflex.NoOpenOut;
                    LastWidthOut = checkflex.WeightOut;
                    LastSheetLOut = checkflex.SheetLengthOut;
                    LastSheetWOut = checkflex.SheetWidthOut;
                    CheckData = "1";
                }
                else { CheckData = "0"; }
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(new { CheckData = CheckData, LastNoOut = LastNoOut, LastWidthOut = LastWidthOut, LastSheetLOut = LastSheetLOut, LastSheetWOut = LastSheetWOut });
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json("");
            }
        }

        [SessionTimeout]
        public PartialViewResult _Routing_DropdownSeq()
        {
            Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
            var model = SessionExtentions.GetSession<TransactionDataModel>(HttpContext.Session, "TransactionDataModel");
            Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            return PartialView(model);
        }

        [SessionTimeout]
        public JsonResult GetListModalSeq()
        {
            Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
            var model = SessionExtentions.GetSession<TransactionDataModel>(HttpContext.Session, "TransactionDataModel");
            Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            return Json(RenderView.RenderRazorViewToString(this, "_Routing_DropdownSeq", model));
        }

        [SessionTimeout]
        public JsonResult CheckMathDuplicates(string material)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var model = SessionExtentions.GetSession<TransactionDataModel>(HttpContext.Session, "TransactionDataModel");
                var counts = model.modelRouting.RoutingDataList.Where(x => x.JoinToMaterialNo.Trim() == material).ToList().Count();
                string check = "0";
                if (counts > 0)
                {
                    check = "1";
                }
                else
                {
                    check = "0";
                }
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(new { CheckData = check });
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json("");
            }
        }

        [SessionTimeout]
        public JsonResult CheckHoneyCore()
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var model = SessionExtentions.GetSession<TransactionDataModel>(HttpContext.Session, "TransactionDataModel");
                var counts = model.modelRouting.RoutingDataList.ToList().Count();
                string check = "0";
                string result = "";
                try
                {
                    if (counts > 0)
                    {
                        check = "0";
                    }
                    else
                    {
                        check = "1";
                        try
                        {
                            string tmp = model.modelProductSpec.widHC + " x " + model.modelProductSpec.lenHC;
                            result = tmp;
                        }
                        catch
                        {
                            result = "0";
                        }
                    }
                }
                catch
                {
                    check = "0";
                    result = "0";
                }
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");

                return Json(new { CheckData = check, value = result });
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json("");
            }
        }

        [SessionTimeout]
        public JsonResult GetQualityspec()
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var model = SessionExtentions.GetSession<TransactionDataModel>(HttpContext.Session, "TransactionDataModel");
                // List<QualitySpec> listquality = new List<QualitySpec>();
                var listquality = _routingService.GetQualitySpecsByMaterial(model.MaterialNo).FirstOrDefault();
                string result = "";
                if (listquality != null)
                {
                    result = listquality.Name + " " + listquality.Value + " " + listquality.Unit;
                }
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(result);
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json("");
            }
        }

        #endregion Routing

        #region ProductERP

        [SessionTimeout]
        public IActionResult ProductERPMain()
        {
            TransactionDataModel transactionDataModel = new TransactionDataModel();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _productERPService.GetProductERP(this, ref transactionDataModel);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }

            return View(transactionDataModel);
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult GetPurchaseGrp(ProductERPPlantViewModel productERPPlantViewModel)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                if (productERPPlantViewModel.Plant != "")//else
                {
                    var PurchCode = _productERPService.GenPurch(productERPPlantViewModel.Plant);
                    Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                    return Json(new { PurchCodeGrp = PurchCode });
                }
                else
                {
                    return Json(new { PurchCodeGrp = "" });
                }
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json("");
            }
        }

        [SessionTimeout]
        [HttpPost]
        public PartialViewResult SavePlantViewERP(TransactionDataModel transactionDataModel)
        {
            // if (ModelState.IsValid)
            //  {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _productERPService.SavePlantViewERP(this, ref transactionDataModel);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }
            //    }

            return PartialView("_ERPPlantView", transactionDataModel);
        }

        [SessionTimeout]
        [HttpPost]
        //public PartialViewResult UpdatePlantViewERP(TransactionDataModel transactionDataModel, int selectedTab, string btnUpdateERP)
        public PartialViewResult UpdatePlantViewERP(TransactionDataModel transactionDataModel)
        {
            //if (ModelState.IsValid)
            //{
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                //_productERPService.UpdatePlantViewERP(this, ref transactionDataModel, btnUpdateERP);
                _productERPService.UpdatePlantViewERP(this, ref transactionDataModel);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }
            //}

            return PartialView("_ERPPlantView", transactionDataModel);
        }

        [SessionTimeout]
        [HttpPost]
        public IActionResult DeletePlantViewERP(TransactionDataModel transactionDataModel, int selectedTab, string btnDeleteERP)
        {
            //if (ModelState.IsValid)
            //{
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _productERPService.DeletePlantViewERP(this, ref transactionDataModel, selectedTab, btnDeleteERP);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }
            //}

            return PartialView("_ERPPlantView", transactionDataModel);
        }

        [SessionTimeout]
        [HttpPost]
        public PartialViewResult SaveSaleViewERP(TransactionDataModel transactionDataModel)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _productERPService.SaveSaleViewERP(this, ref transactionDataModel);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }

            return PartialView("_ERPSaleView", transactionDataModel);
        }

        [SessionTimeout]
        [HttpPost]
        public PartialViewResult UpdateSaleViewERP(TransactionDataModel transactionDataModel)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _productERPService.UpdateSaleViewERP(this, ref transactionDataModel);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }

            return PartialView("_ERPSaleView", transactionDataModel);
        }

        [SessionTimeout]
        [HttpPost]
        public IActionResult DeleteSaleViewERP(TransactionDataModel transactionDataModel, int selectedTab)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _productERPService.DeleteSaleViewERP(this, ref transactionDataModel, selectedTab);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }

            return PartialView("_ERPSaleView", transactionDataModel);
        }

        [SessionTimeout]
        [HttpPost]
        public PartialViewResult SavePurchaseERP(TransactionDataModel transactionDataModel)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _productERPService.SavePurchaseViewERP(this, ref transactionDataModel);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }

            return PartialView("_PurchaseView", transactionDataModel);
        }

        [SessionTimeout]
        public IActionResult ERPNext()
        {
            return RedirectToAction("ProductPic");
        }

        #endregion ProductERP

        #region ProductPic

        [SessionTimeout]
        public IActionResult ProductPic()
        {
            TransactionDataModel transactionDataModel = new TransactionDataModel();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _productPicService.PicData(ref transactionDataModel, _environment);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                // Redirect To Error Page
            }

            return View(transactionDataModel);
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult AttrachFileMO(List<IFormFile> files)
        {
            var isSuccess = true;
            var exceptionMessage = string.Empty;

            try
            {
                _masterDataService.SaveAttachFileMO(files[0]);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                exceptionMessage = ex.Message;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage });
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult DeleteAttrachFileMO()
        {
            var isSuccess = true;
            var exceptionMessage = string.Empty;

            try
            {
                _masterDataService.DeleteAttachFileMO();
            }
            catch (Exception ex)
            {
                isSuccess = false;
                exceptionMessage = ex.Message;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage });
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult AttrachFileSemi(List<IFormFile> files)
        {
            var isSuccess = true;
            var exceptionMessage = string.Empty;

            try
            {
                _masterDataService.SaveAttachFileSemi(files[0]);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                exceptionMessage = ex.Message;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage });
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult DeleteAttrachFileSemi()
        {
            var isSuccess = true;
            var exceptionMessage = string.Empty;

            try
            {
                _masterDataService.DeleteAttachFileSemi();
            }
            catch (Exception ex)
            {
                isSuccess = false;
                exceptionMessage = ex.Message;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage });
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult ProdPicSaveImage(TransactionDataModel ProductPic, IFormFile Pic_Drawing, IFormFile Pic_Print, IFormFile Pic_Pallet, IFormFile Pic_FG, IFormFile Semi1_Print, IFormFile Semi2_Print, IFormFile Semi3_Print, IFormFile Semi4_Print)
        {
            bool isSuccess;
            string exeptionMessage = string.Empty;
            TransactionDataModel model = new TransactionDataModel();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                model = SessionExtentions.GetSession<TransactionDataModel>(HttpContext.Session, "TransactionDataModel");
                //update Image
                //   SessionExtentions.SetSession(HttpContext.Session, "TransactionDataModel", model);
                _productPicService.UpdateData(model, _environment, "ProductPic", Pic_Drawing, Pic_Print, Pic_Pallet, Pic_FG, Semi1_Print, Semi2_Print, Semi3_Print, Semi4_Print);
                //  SessionExtentions.SetSession(HttpContext.Session, "TransactionDataModel", null);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                exeptionMessage = ex.Message;

                isSuccess = false;
            }

            return Json(new { isSuccess = isSuccess, exeptionMessage = exeptionMessage });
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult RemovePrintMasterProductPic()
        {
            bool isSuccess;
            string exeptionMessage = string.Empty;
            TransactionDataModel model = new TransactionDataModel();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                model = SessionExtentions.GetSession<TransactionDataModel>(HttpContext.Session, "TransactionDataModel");
                model.modelProductSpec.PrintMaster = null;
                SessionExtentions.SetSession(HttpContext.Session, "TransactionDataModel", model);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                exeptionMessage = ex.Message;

                isSuccess = false;
            }

            return Json(new { isSuccess = isSuccess, exeptionMessage = exeptionMessage });
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult CreateBOM()
        {
            bool isSuccess;
            string exeptionMessage = string.Empty;
            TransactionDataModel model = new TransactionDataModel();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _productPicService.CreateBom(model);

                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                exeptionMessage = ex.Message;

                isSuccess = false;
            }

            return Json(new { isSuccess = isSuccess, exeptionMessage = exeptionMessage });
        }

        [SessionTimeout]
        public async Task<IActionResult> DownloadAttachFilePDF(string materialNo)
        {
            var stream = new MemoryStream();
            var pdfName = $"AttachFile({materialNo.ToUpper()})_({DateTime.Now.ToString("dd / MM / yyyy HH.mm.ss")}).pdf";
            if (!string.IsNullOrEmpty(materialNo))
            {
                stream = _masterDataService.GetAttachFilePDFMOFromMasterData(materialNo);
                return File(stream, "application/pdf", pdfName);
            }
            else
            {
                return RedirectToAction("ProductPic", "NewProduct");
            }
        }

        [SessionTimeout]
        public async Task<IActionResult> DownloadAttachFilePDFSemi(string Path)
        {
            var stream = new MemoryStream();

            //string dePath = FromHexString(Path);

            var pdfName = $"A SemiFile({Path.ToUpper()})_({DateTime.Now.ToString("dd / MM / yyyy HH.mm.ss")}).pdf";
            //if (!string.IsNullOrEmpty(Path))
            //{
            stream = _masterDataService.GetSemiFilePDFMOFromMasterData(Path);
            return File(stream, "application/pdf", pdfName);
            //}
            //else
            //{
            //    return RedirectToAction("ProductPic", "NewProduct");
            //}
        }

        #region [ Helper ProductPic ]

        public class ModelConvert
        {
            public string Path { get; set; }
        }

        public JsonResult ConvertPath(ModelConvert model)
        {
            return Json(ToHexString(model.Path));
        }

        private string ToHexString(string str)
        {
            var sb = new StringBuilder();

            var bytes = Encoding.Unicode.GetBytes(str);
            foreach (var t in bytes)
            {
                sb.Append(t.ToString("X2"));
            }

            return sb.ToString(); // returns: "48656C6C6F20776F726C64" for "Hello world"
        }

        private string FromHexString(string hexString)
        {
            var bytes = new byte[hexString.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            return Encoding.Unicode.GetString(bytes); // returns: "Hello world" for "48656C6C6F20776F726C64"
        }

        #endregion [ Helper ProductPic ]

        //[HttpPost]
        //public void ProdPicSaveImage(IFormFile Pic_Drawing, IFormFile Pic_Print, IFormFile Pic_Pallet, IFormFile Pic_FG)//1111111
        //{
        //    var fileName = new string[4, 2];
        //    //* fileName & newFileName
        //    //*  [0] = Drawing
        //    //*  [1] = Print Master
        //    //*  [2] = Pallet
        //    //*  [3] = F/G Picture
        //    try
        //    {
        //        SessionsModel Sessions = new SessionsModel
        //        {
        //            UserName = HttpContext.Session.GetString("username"),
        //            SaleOrg = HttpContext.Session.GetString("SALE_ORG"),
        //            PlantCode = HttpContext.Session.GetString("PLANT_CODE"),
        //        };

        //        string[] temp;
        //        TransactionDataModel model = SessionExtentions.GetSession<TransactionDataModel>(HttpContext.Session, "TransactionDataModel");
        //        if (Pic_Drawing != null)
        //        {
        //            temp = _newProductService.UploadPicture(Pic_Drawing, _environment);
        //            fileName[0, 0] = temp[0];
        //            fileName[0, 1] = temp[1];
        //        }
        //        if (Pic_Print != null)
        //        {
        //            temp = _newProductService.UploadPicture(Pic_Print, _environment);
        //            fileName[1, 0] = temp[0];
        //            fileName[1, 1] = temp[1];
        //        }

        //        if (Pic_Pallet != null)
        //        {
        //            temp = _newProductService.UploadPicture(Pic_Pallet, _environment);
        //            fileName[2, 0] = temp[0];
        //            fileName[2, 1] = temp[1];
        //        }

        //        if (Pic_FG != null)
        //        {
        //            temp = _newProductService.UploadPicture(Pic_FG, _environment);
        //            fileName[3, 0] = temp[0];
        //            fileName[3, 1] = temp[1];
        //        }

        //        if (model.MaterialNo != null)
        //        {
        //            bool StatusSave = ProductPicRepository.SaveMasterData(fileName, model);
        //            if (StatusSave == true)
        //            {
        //                ViewBag.StatusSave = "Your picture has been saved successfully";
        //            }
        //            else
        //            {
        //                ViewBag.StatusSave = "A problem has been occurred while save your picture.";
        //                throw new ArgumentException("A problem has been occurred while save your picture.");

        //            }
        //        }
        //        else
        //        {
        //            ViewBag.StatusSave = "A problem has been occurred while save your picture.";
        //            throw new ArgumentException("A problem has been occurred while save your picture.");
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}

        #endregion ProductPic

        #region DetailMatmaster

        [SessionTimeout]
        public IActionResult DetailProduct(string actionTran, string materialNo, string psmId, string currentPage)
        {
            TransactionDataModel transactionDataModel = new TransactionDataModel();
            var basePrintMastercard = new BasePrintMastercardData();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _categoryService.GetCategory(this, ref transactionDataModel, actionTran, materialNo, psmId);
                if (actionTran == "Detail")
                {
                    _productPropService.ProductPropData(ref transactionDataModel);
                    _productERPService.GetProductERP(this, ref transactionDataModel);
                }

                MasterCardMO Mo = new MasterCardMO();
                _masterCardService.GetBaseOfPrintMastercard(ref basePrintMastercard, new List<string> { materialNo });

                Mo = _masterCardService.GetMasterCard(materialNo, basePrintMastercard);
                transactionDataModel.Rout = Mo.Rout.ToList();

                _productPicService.ChangePathToBase64Image(ref transactionDataModel);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                //if (ex.Message.Contains("404")) {
                //}
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return View(transactionDataModel);
        }

        #endregion DetailMatmaster

        //tassanai autorouting
        [SessionTimeout]
        [HttpPost]
        public PartialViewResult Autorouting()
        {
            var model = SessionExtentions.GetSession<TransactionDataModel>(HttpContext.Session, "TransactionDataModel");
            model.modelRouting.RoutingDataList.Clear();

            try
            {
                if (model.modelRouting.RoutingDataList.Count() == 0)
                {
                    _routingService.AutoRoutingList(model);
                }

                //add to session
                SessionExtentions.SetSession(HttpContext.Session, "TransactionDataModel", model);

                return PartialView("_RoutingDataList", model);
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return PartialView("");
            }
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult GetRouting(string material)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

                var booldataexist = false;
                var RoutingDataList = _routingService.GetRoutingList(material).ToList();
                if (RoutingDataList.Count() > 0)
                {
                    booldataexist = true;
                }
                var resultdata = "";
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                //var aaa = "";

                return Json(new { Result = booldataexist });
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json("");
            }
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult ReSentMat(string materialNo)
        {
            bool isSuccess = false;
            string exceptionMessage = string.Empty;

            //call repository
            try
            {
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _masterDataService.ReSentMat(materialNo);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage });
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                isSuccess = false;
                return Json(new { IsSuccess = isSuccess, ExceptionMessage = ex.Message });
                throw ex;
            }
        }
    }
}