using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using MySqlX.XDevAPI;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PMTs.DataAccess;
using PMTs.DataAccess.ComplexModel;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.ManageMO;
using PMTs.DataAccess.ModelView.NewProduct;
using PMTs.DataAccess.ModelView.Report;
using PMTs.DataAccess.Repository;
using PMTs.DataAccess.Tracing;
using PMTs.Logs.Logger;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Services;
using PMTs.WebApplication.Services.Interfaces;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Controllers
{
    [SessionTimeout]
    public class ReportController : Controller
    {
        private readonly IHostingEnvironment _environment;
        private readonly IMasterCardService _masterCardService;
        private readonly IExtensionService _extensionService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ISaleOrderService _saleOrderService;
        private readonly IReportService _reportService;
        private readonly IRoutingService routingService;
        private readonly ILoginService loginService;
        private readonly IConfiguration configuration;

        public ReportController(IHostingEnvironment IHostingEnvironment,
            IMasterCardService masterCardService,
            IExtensionService extensionService,
            IHttpContextAccessor httpContextAccessor,
            ISaleOrderService saleOrderService,
            IReportService reportService,
            IConfiguration configuration,
            ILoginService loginService,
            IRoutingService routingService)
        {
            _environment = IHostingEnvironment;
            _masterCardService = masterCardService;
            _extensionService = extensionService;
            _saleOrderService = saleOrderService;
            _reportService = reportService;
            this.httpContextAccessor = httpContextAccessor;
            this.configuration = configuration;
            this.loginService = loginService;
            this.routingService = routingService;
        }

        #region Print Mastercard/ MO

        [SessionTimeout]
        [HttpPost]
        public JsonResult CheckAttachFile([FromBody] PrintMasterCardData model)
        {
            var MoRouting = new List<MoRouting>();
            var isSuccess = false;
            var ShowMsgError = false;
            var errorMessage = string.Empty;
            var errorMaterial = string.Empty;
            var errorMaterials = string.Empty;
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                MoRouting = _masterCardService.CheckMORoutingAttachFile(model.OrderItem);
                if (MoRouting.Count > 0)
                {
                    isSuccess = true;
                    SessionExtentions.SetSession(HttpContext.Session, "MoRoutingNoAttachFile", MoRouting.ToList());
                    return
                        Json(new
                        {
                            IsSuccess = isSuccess,
                            ErrorMessage = errorMessage,
                            SONumberShow = MoRouting
                                .Select((x, i) => new { OrderItem = $"{i + 1}. {x.OrderItem}" })
                                .ToList()
                        });
                }
                else
                {
                    return Json(new { IsSuccess = isSuccess, ErrorMessage = errorMessage, SONumber = new List<string>() });
                }
            }
            catch (Exception)
            {
                return Json(new { IsSuccess = isSuccess, ErrorMessage = errorMessage, SONumber = new List<string>() });
            }
        }

        [SessionTimeout]
        public IActionResult MasterCardMOTextFile()
        {
            var fileName = string.Empty;
            // Initialize SaleOrg and PlantCode from Session
            var userSessionModel = SessionExtentions.GetSession<UserSessionModel>(httpContextAccessor.HttpContext.Session, "UserSessionModel");
            var moRouting = SessionExtentions.GetSession<List<MoRouting>>(HttpContext.Session, "MoRoutingNoAttachFile");
            string txtFileName = $"PrintMastercardMOs_PMTs({DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss")}).txt";

            if (moRouting.Count > 0)
            {
                return new FileContentResult(_masterCardService.SaveTextFileWithOutAttachFile(this, moRouting), "application/pdf") { FileDownloadName = txtFileName };
            }
            else
            {
                return RedirectToAction("MasterCardMOViewList", "Report");
            }
        }

        [SessionTimeout]
        [HttpPost]
        public async Task<JsonResult> SetMasterCardMO([FromBody] PrintMasterCardData model)
        {
            var rootActivity = ActivitySourceProvider.Source!.StartActivity($"{nameof(SetMasterCardMO)} Start");
            var list = new List<MasterCardMO>();
            var Mo = new MasterCardMO();
            var masterCardMoModel = new PrintMasterCardMOModel();
            masterCardMoModel.MasterCardMOs = new List<MasterCardMO>();
            var basePrintMastercard = new BasePrintMastercardData();
            var isSuccess = false;
            var ShowMsgError = false;
            var errorMessage = string.Empty;
            var errorMaterial = string.Empty;
            var errorMaterials = string.Empty;

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

                if (model.OrderItem.Count > 0)
                {
                    var activity = ActivitySourceProvider.Source!.StartActivity($"{nameof(SetMasterCardMO)} :: if (model.OrderItem.Count > 0)");
                    var userTIP = new UserTIP();
                    if (model.IsTip)//get userTIP
                    {
                        userTIP.UserName = "PMTsSystem";
                        userTIP.Password = "PMTs2022";
                        userTIP.Domain = null;
                        userTIP.UrlApi = _reportService.GetTIPApiUrlByFactoryCode();
                        loginService.GetUserTIP(ref userTIP);
                    }

                    basePrintMastercard = await _masterCardService.GetBaseOfPrintMastercardMO(basePrintMastercard, model.OrderItem, userTIP);
                    //set base data of print masterdata mo
                    int loop = 0;
                    Parallel.ForEach(model.OrderItem, i =>
                    {
                        loop++;
                        var activityLoop = ActivitySourceProvider.Source!.StartActivity($"Loop : {loop}");
                        errorMaterial = i;
                        try
                        {
                            var activityLoopGetMasterCardMO = ActivitySourceProvider.Source!.StartActivity($"Loop : {loop} : GetMasterCardMO");
                            Mo = _masterCardService.GetMasterCardMO(i, model.IsPreview, basePrintMastercard);
                            activityLoopGetMasterCardMO.Stop();
                            Mo.IsBundle = !string.IsNullOrEmpty(Mo.TagBundle) ? true : false;
                            //if (!string.IsNullOrEmpty(Mo.TagBundle))
                            //{
                            //    Mo.IsBundle = true;
                            //}
                            //else
                            //{
                            //    Mo.IsBundle = false;
                            //}
                            Mo.IsPallet = !string.IsNullOrEmpty(Mo.TagPallet) ? true : false;
                            //Mo.IsPallet = model.IsPallet;

                            if (!Mo.IsXStatus)
                            {
                                if (string.IsNullOrEmpty(model.ProductType))
                                {
                                    list.Add(Mo);
                                }
                                else
                                {
                                    if (model.ProductType == "Carton")
                                    {
                                        if (Mo.ProductType != "Sheet Board")
                                        {
                                            list.Add(Mo);
                                        }
                                    }
                                    else if (model.ProductType.Equals(Mo.ProductType))
                                    {
                                        list.Add(Mo);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            errorMaterials = errorMaterials + $"{errorMaterial} ,";
                            ShowMsgError = true;

                            //errorMessage = ex.Message;
                            Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                            //continue;
                            return;
                        }
                        activityLoop.Stop();
                    });
                    activity.Stop();
                }
                else if (model.MaterialNo.Count > 0)
                {
                    var activity = ActivitySourceProvider.Source!.StartActivity($"{nameof(SetMasterCardMO)} :: else if (model.MaterialNo.Count > 0)");
                    //set base data of print masterdata
                    _masterCardService.GetBaseOfPrintMastercard(ref basePrintMastercard, model.MaterialNo);

                    var groupmate = model.MaterialNo.Distinct();
                    int loop = 0;
                    Parallel.ForEach(groupmate, i =>
                    {
                        loop++;
                        var activityLoop = ActivitySourceProvider.Source!.StartActivity($"Loop : {loop}");
                        errorMaterial = i;
                        try
                        {
                            errorMaterial = i;
                            Mo = _masterCardService.GetMasterCard(i, basePrintMastercard);

                            if (!Mo.IsXStatus)
                            {
                                if (string.IsNullOrEmpty(model.ProductType))
                                {
                                    list.Add(Mo);
                                }
                                else
                                {
                                    if (model.ProductType == "Carton")
                                    {
                                        if (Mo.ProductType != "Sheet Board")
                                        {
                                            list.Add(Mo);
                                        }
                                    }
                                    else if (model.ProductType.Equals(Mo.ProductType))
                                    {
                                        list.Add(Mo);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            errorMaterials = errorMaterials + $"{errorMaterial} ,";
                            ShowMsgError = true;

                            //errorMessage = ex.Message;
                            Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                            //continue;
                            return;
                        }
                        activityLoop.Stop();
                    });
                    activity.Stop();
                }
                else
                {
                    throw new ArgumentException("error");
                }

                if (list.Count > 0)
                {
                    list = list.OrderBy(m => model.OrderItem.FindIndex(s => s == m.OrderItem)).ToList();

                    if (!model.IsPreview)
                    {
                        //check update printed
                        _masterCardService.UpdateMaterCardPrintedByOrderItems(list.Select(m => m.OrderItem).ToList());
                        list.ForEach(m => m.Printed++);
                    }
                    masterCardMoModel.MasterCardMOs = list;
                    masterCardMoModel.SizeOfPage = model.SizeOfPage;
                    if (!string.IsNullOrEmpty(model.FileName))
                    {
                        masterCardMoModel.FileName = model.FileName;
                        masterCardMoModel.IsPrintedFromFile = true;
                    }

                    SessionExtentions.SetSession(HttpContext.Session, "MasterCardList", masterCardMoModel);

                    isSuccess = true;
                }
                else
                {
                    isSuccess = false;
                    errorMessage = model.OrderItem.Count > 0 ? "Can't find sale order number or sale order status has been deleted" : "Can't find material number";
                }

                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                //list = new List<MasterCardMO>();
                //masterCardMoModel.MasterCardMOs = list;
                //masterCardMoModel.SizeOfPage = model.SizeOfPage;

                //SessionExtentions.SetSession(HttpContext.Session, "MasterCardList", masterCardMoModel);

                //Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                RedirectToAction("MasterCardMOViewList", "Report");
            }

            if (model.OrderItem.Count > 0 && !string.IsNullOrEmpty(errorMaterials))
            {
                errorMessage = $"เกิดข้อผิดพลาดในการพิมพ์ Sale Order {errorMaterials.Substring(0, errorMaterials.Length - 1)} โปรดลองใหม่";
            }
            else if (model.MaterialNo.Count > 0 && !string.IsNullOrEmpty(errorMaterials))
            {
                errorMessage = $"เกิดข้อผิดพลาดในการพิมพ์ Material No. {errorMaterials.Substring(0, errorMaterials.Length - 1)} ข้อมูลบางส่วนไม่สมบูรณ์ กรุณาเข้าไปแก้ไขกด next step จนจบกระบวนการ";
            }
            rootActivity.Stop();
            if (list.Count > 0)
            {
                return Json(new { IsSuccess = isSuccess, ErrorMessage = errorMessage, ShowMsgError = ShowMsgError, MastercardMOs = masterCardMoModel.MasterCardMOs });
            }
            else
            {
                return Json(new { IsSuccess = isSuccess, ErrorMessage = errorMessage, ShowMsgError = ShowMsgError, MastercardMOs = new List<MasterCardMO>() });
            }
        }

        [SessionTimeout]
        [HttpPost]
        public async Task<JsonResult> SetMasterCardMOFromTextFile(List<IFormFile> fileUpload)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;
            var saleOrders = new string[1000];
            var printMastercardViewModel = new PrintMastercardViewModel();
            var saleOrdersStatus = new object();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                printMastercardViewModel = await _masterCardService.SetMasterCardMOFromFile(fileUpload);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                isSuccess = true;

                saleOrders = printMastercardViewModel.MasterDataRoutingModels.Select(m => m.SaleOrder).ToArray();
                saleOrdersStatus = printMastercardViewModel.MasterDataRoutingModels.Select(m => new { m.SaleOrder, m.MoStatus }).ToList();
                if (!string.IsNullOrEmpty(printMastercardViewModel.ErrorSearchOrderItems))
                {
                    exceptionMessage = $"* เกิดข้อผิดพลาดในการค้นหา SaleOrder : {printMastercardViewModel.ErrorSearchOrderItems.Substring(0, printMastercardViewModel.ErrorSearchOrderItems.Length - 2)}";
                }
            }
            catch (Exception ex)
            {
                printMastercardViewModel.MasterDataRoutingModels = new List<MasterDataRoutingModel>();
                ViewData["ErrorMessage"] = "* เกิดข้อผิดพลาด กรุณาเลือกไฟล์ในการค้นหาใหม่";
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                isSuccess = false;

                return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_MasterCardMoViewTable", printMastercardViewModel.MasterDataRoutingModels), saleOrders = saleOrders, saleOrdersStatus = saleOrdersStatus });
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_MasterCardMoViewTable", printMastercardViewModel.MasterDataRoutingModels), saleOrders = saleOrders, saleOrdersStatus = saleOrdersStatus });
        }

        #region Old Code SetMasterCardMOFromTextFile

        //[SessionTimeout]
        //[HttpPost]
        //public async Task<JsonResult> PrintMastercardFromTextFile(List<IFormFile> fileUpload)
        //{
        //    var isSuccess = false;
        //    var message = "";
        //    var typeAction = "";
        //    var errorMessage = $"เกิดข้อผิดพลาดในการพิมพ์ Sale Order";
        //    var errorMaterial = "";
        //    var masterCardMoModel = new PrintMasterCardMOModel();
        //    List<MasterCardMO> list = new List<MasterCardMO>();
        //    MasterCardMO Mo = new MasterCardMO();
        //    var saleOrders = new List<string>();
        //    var result = new StringBuilder();

        //    try
        //    {
        //        Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
        //        foreach (var file in fileUpload)
        //        {
        //            using (var reader = new StreamReader(file.OpenReadStream()))
        //            {
        //                while (reader.Peek() >= 0)
        //                {
        //                    var data = await reader.ReadLineAsync();
        //                    result.Append(data);
        //                    saleOrders.Add(result.ToString().Trim());

        //                    result.Clear();
        //                }
        //            }
        //        }

        //        if (saleOrders.Count > 0)
        //        {
        //            //set number of public print master card mo
        //            foreach (var i in saleOrders)
        //            {
        //                try
        //                {
        //                    _masterCardService.UpdatePublicPrinted(i);

        //                    errorMaterial = i;
        //                    Mo = _masterCardService.GetMasterCardMO(i);
        //                    if (!Mo.IsXStatus)
        //                    {
        //                        list.Add(Mo);
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    errorMaterial = result.ToString().Trim();
        //                    errorMessage = errorMessage + $" {errorMaterial},";
        //                }
        //            }
        //        }

        //        if (list.Count > 0)
        //        {
        //            masterCardMoModel.MasterCardMOs = list;
        //            masterCardMoModel.SizeOfPage = 2;
        //            SessionExtentions.SetSession(HttpContext.Session, "MasterCardList", masterCardMoModel);

        //            isSuccess = true;
        //        }
        //        else
        //        {
        //            isSuccess = false;
        //            message = "Can't find sale order number or sale order status has been deleted";
        //        }

        //        Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
        //    }
        //    catch (Exception ex)
        //    {
        //        message = ex.Message;
        //        Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
        //    }

        //    return Json(new { IsSuccess = isSuccess, ExceptionMessage = message, TypeAction = typeAction });
        //}

        #endregion Old Code SetMasterCardMOFromTextFile

        [SessionTimeout]
        public async Task<IActionResult> MasterCardMO()
        {
            await httpContextAccessor.HttpContext.Session.LoadAsync();
            var fileName = string.Empty;
            // Initialize SaleOrg and PlantCode from Session
            var userSessionModel = SessionExtentions.GetSession<UserSessionModel>(httpContextAccessor.HttpContext.Session, "UserSessionModel");
            var PrintMasterCardMOModel = SessionExtentions.GetSession<PrintMasterCardMOModel>(HttpContext.Session, "MasterCardList");
            PrintMasterCardMOModel.FileName = "";
            string pdfFileName = $"PMTs_{userSessionModel.FactoryCode}_{PrintMasterCardMOModel.FileName}({DateTime.Now.ToString("dd/MM/yyyy HH.mm.ss")}).pdf";

            if (PrintMasterCardMOModel.MasterCardMOs.Count > 0)
            {
                return new FileContentResult(await _masterCardService.SavePDFWithOutAttachFile(this, PrintMasterCardMOModel), "application/pdf") { FileDownloadName = $"PrintMastercardMOs_PMTs({DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss")}).pdf" };

                //if (PrintMasterCardMOModel.IsPrintedFromFile)
                //{
                //    return new Rotativa.AspNetCore.ViewAsPdf("MasterCardMO", PrintMasterCardMOModel)
                //    {
                //        FileName = pdfFileName,
                //        PageMargins = new Margins(0, 2, 0, 2),
                //        PageSize = Rotativa.AspNetCore.Options.Size.A4,
                //        ContentType = "application/pdf",
                //        PageOrientation = Orientation.Portrait,
                //    };

                //}
                //else
                //{
                //    return new Rotativa.AspNetCore.ViewAsPdf("MasterCardMO", PrintMasterCardMOModel)
                //    {
                //        PageMargins = new Margins(0, 2, 0, 2),
                //        PageSize = Rotativa.AspNetCore.Options.Size.A4,
                //        ContentType = "application/pdf",
                //        PageOrientation = Orientation.Portrait,
                //    };
                //}
            }
            else
            {
                return RedirectToAction("MasterCardMOViewList", "Report");
            }
        }

        [SessionTimeout]
        [HttpPost]
        public async Task<JsonResult> SetMasterCardTag([FromBody] PrintMasterCardData model)
        {
            var list = new List<MasterCardMO>();
            var Mo = new MasterCardMO();
            var masterCardMoModel = new PrintMasterCardMOModel();
            var basePrintMastercard = new BasePrintMastercardData();
            var isSuccess = false;
            var ShowMsgError = false;
            var errorMessage = string.Empty;
            var errorMaterial = string.Empty;
            var errorMaterials = string.Empty;

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

                if (model.OrderItem.Count > 0)
                {
                    var userTIP = new UserTIP();
                    if (model.IsTip)//get userTIP
                    {
                        userTIP.UserName = "PMTsSystem";
                        userTIP.Password = "PMTs2022";
                        userTIP.Domain = null;
                        userTIP.UrlApi = _reportService.GetTIPApiUrlByFactoryCode();
                        loginService.GetUserTIP(ref userTIP);
                    }

                    basePrintMastercard = await _masterCardService.GetBaseOfPrintMastercardMO(basePrintMastercard, model.OrderItem, userTIP);
                    //set base data of print masterdata mo

                    foreach (var i in model.OrderItem)
                    {
                        errorMaterial = i;
                        try
                        {
                            Mo = _masterCardService.GetMasterCardMO(i, model.IsPreview, basePrintMastercard);
                            Mo.IsBundle = !string.IsNullOrEmpty(Mo.TagBundle) ? true : false;
                            //if (!string.IsNullOrEmpty(Mo.TagBundle))
                            //{
                            //    Mo.IsBundle = true;
                            //}
                            //else
                            //{
                            //    Mo.IsBundle = false;
                            //}
                            Mo.IsPallet = !string.IsNullOrEmpty(Mo.TagPallet) ? true : false;
                            //Mo.IsPallet = model.IsPallet;

                            if (!model.IsPreview)
                            {
                                //check update printed
                                if (_masterCardService.UpdatePublicPrinted(i))
                                {
                                    Mo.Printed++;
                                }
                            }

                            if (!Mo.IsXStatus)
                            {
                                if (string.IsNullOrEmpty(model.ProductType))
                                {
                                    list.Add(Mo);
                                }
                                else
                                {
                                    if (model.ProductType == "Carton")
                                    {
                                        if (Mo.ProductType != "Sheet Board")
                                        {
                                            list.Add(Mo);
                                        }
                                    }
                                    else if (model.ProductType.Equals(Mo.ProductType))
                                    {
                                        list.Add(Mo);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            errorMaterials = errorMaterials + $"{errorMaterial} ,";
                            ShowMsgError = true;

                            //errorMessage = ex.Message;
                            Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                            continue;
                        }
                    }
                }
                else if (model.MaterialNo.Count > 0)
                {
                    //set base data of print masterdata
                    _masterCardService.GetBaseOfPrintMastercard(ref basePrintMastercard, model.MaterialNo);

                    var groupmate = model.MaterialNo.Distinct();
                    foreach (var i in groupmate)
                    {
                        errorMaterial = i;
                        try
                        {
                            errorMaterial = i;
                            Mo = _masterCardService.GetMasterCard(i, basePrintMastercard);

                            Mo.IsBundle = !string.IsNullOrEmpty(Mo.TagBundle) ? true : false;
                            //if (!string.IsNullOrEmpty(Mo.TagBundle))
                            //{
                            //    Mo.IsBundle = true;
                            //}
                            //else
                            //{
                            //    Mo.IsBundle = false;
                            //}
                            Mo.IsPallet = !string.IsNullOrEmpty(Mo.TagPallet) ? true : false;
                            if (!Mo.IsXStatus)
                            {
                                if (string.IsNullOrEmpty(model.ProductType))
                                {
                                    list.Add(Mo);
                                }
                                else
                                {
                                    if (model.ProductType == "Carton")
                                    {
                                        if (Mo.ProductType != "Sheet Board")
                                        {
                                            list.Add(Mo);
                                        }
                                    }
                                    else if (model.ProductType.Equals(Mo.ProductType))
                                    {
                                        list.Add(Mo);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            errorMaterials = errorMaterials + $"{errorMaterial} ,";
                            ShowMsgError = true;

                            //errorMessage = ex.Message;
                            Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                            continue;
                        }
                    }
                }
                else
                {
                    throw new ArgumentException("error");
                }

                if (list.Count > 0)
                {
                    masterCardMoModel.MasterCardMOs = list;
                    masterCardMoModel.SizeOfPage = model.SizeOfPage;
                    if (!string.IsNullOrEmpty(model.FileName))
                    {
                        masterCardMoModel.FileName = model.FileName;
                        masterCardMoModel.IsPrintedFromFile = true;
                    }

                    SessionExtentions.SetSession(HttpContext.Session, "MasterCardList", masterCardMoModel);

                    //SessionExtentions.SetSession(HttpContext.Session, "MasterCardTag", masterCardMoModel);

                    isSuccess = true;
                }
                else
                {
                    isSuccess = false;
                    errorMessage = model.OrderItem.Count > 0 ? "Can't find sale order number or sale order status has been deleted" : "Can't find material number";
                }

                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                //list = new List<MasterCardMO>();
                //masterCardMoModel.MasterCardMOs = list;
                //masterCardMoModel.SizeOfPage = model.SizeOfPage;

                //SessionExtentions.SetSession(HttpContext.Session, "MasterCardList", masterCardMoModel);

                //Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                RedirectToAction("MasterCardMOViewList", "Report");
            }

            if (model.OrderItem.Count > 0 && !string.IsNullOrEmpty(errorMaterials))
            {
                errorMessage = $"เกิดข้อผิดพลาดในการพิมพ์ Sale Order {errorMaterials.Substring(0, errorMaterials.Length - 1)} โปรดลองใหม่";
            }
            else if (model.MaterialNo.Count > 0 && !string.IsNullOrEmpty(errorMaterials))
            {
                errorMessage = $"เกิดข้อผิดพลาดในการพิมพ์ Material No. {errorMaterials.Substring(0, errorMaterials.Length - 1)} ข้อมูลบางส่วนไม่สมบูรณ์ กรุณาเข้าไปแก้ไขกด next step จนจบกระบวนการ";
            }

            if (list.Count > 0)
            {
                return Json(new { IsSuccess = isSuccess, ErrorMessage = errorMessage, ShowMsgError = ShowMsgError, MastercardMOs = masterCardMoModel.MasterCardMOs });
            }
            else
            {
                return Json(new { IsSuccess = isSuccess, ErrorMessage = errorMessage, ShowMsgError = ShowMsgError, MastercardMOs = new List<MasterCardMO>() });
            }
        }

        public async Task<IActionResult> MasterCardTag()
        {
            var fileName = string.Empty;
            // Initialize SaleOrg and PlantCode from Session
            var userSessionModel = SessionExtentions.GetSession<UserSessionModel>(httpContextAccessor.HttpContext.Session, "UserSessionModel");
            var PrintMasterCardMOModel = SessionExtentions.GetSession<PrintMasterCardMOModel>(HttpContext.Session, "MasterCardList");
            //var  PrintMastercard = SessionExtentions.GetSession<BasePrintMastercardData>(HttpContext.Session, "MasterCardTag");
            //var masterdatatags = PrintMastercard.MasterDatas;
            PrintMasterCardMOModel.FileName = "";
            string pdfFileName = $"PMTs_{userSessionModel.FactoryCode}_{PrintMasterCardMOModel.FileName}({DateTime.Now.ToString("dd/MM/yyyy HH.mm.ss")}).pdf";

            if (PrintMasterCardMOModel.MasterCardMOs.Count > 0)
            {
                return new FileContentResult(await _masterCardService.SavePDFTagFile(this, PrintMasterCardMOModel), "application/pdf") { FileDownloadName = $"PrintMastercardMOs_PMTs({DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss")}).pdf" };
                //return new FileContentResult(_masterCardService.SavePDFTagFile(this, PrintMasterCardMOModel), "application/pdf") { FileDownloadName = $"PrintMastercardMOs_PMTs({DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss")}).pdf" };
            }
            else
            {
                return RedirectToAction("MasterCardMOViewList", "Report");
            }
        }

        [SessionTimeout]
        public JsonResult MasterCardMOViewListSearch(string startSO, string endSO)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;
            var masterDataRoutings = new List<MasterDataRoutingModel>();
            var saleOrders = new string[100];
            var saleOrdersStatus = new object();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                //_masterCardService.SearchMasterCardMOByKeySearch(ref masterDataRoutings, startSO, endSO);
                masterDataRoutings = _masterCardService.SearchMasterCardMOByKeySearch(startSO, endSO);
                //ViewData["StartSO"] = startSO;
                //ViewData["EndSO"] = endSO;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                isSuccess = true;

                saleOrders = masterDataRoutings.Select(m => m.SaleOrder).ToArray();
                saleOrdersStatus = masterDataRoutings.Select(m => new { m.SaleOrder, m.MoStatus, m.Code, m.Board }).ToList();
            }
            catch (Exception ex)
            {
                masterDataRoutings = new List<MasterDataRoutingModel>();
                //ViewData["ErrorMessage"] = "* เกิดข้อผิดพลาด กรุณาระบุ SO ในการค้นหาใหม่";
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                isSuccess = false;
                return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_MasterCardMoViewTable", masterDataRoutings), saleOrders = saleOrders, saleOrdersStatus = saleOrdersStatus });
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_MasterCardMoViewTable", masterDataRoutings), saleOrders = saleOrders, saleOrdersStatus = saleOrdersStatus });
        }

        [SessionTimeout]
        public IActionResult MasterCardMOViewList()
        {
            var masterDataRoutings = new List<MasterDataRoutingModel>();

            return View(masterDataRoutings);
        }

        [SessionTimeout]
        public IActionResult AddRouting(string OrderItem)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var Routing = new List<MoRouting>();

                Routing = _masterCardService.GetMoRouting(OrderItem);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                //return new ViewAsPdf("_AddMORouting", Routing);
                return PartialView("_AddMORouting", Routing);
            }
            catch (Exception ex)
            {
                if (ex.Message == "OrderItem was not in a correct.")
                {
                    ViewBag.Message = string.Format("OrderItem was not in a correct.", "", DateTime.Now.ToString());
                }
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return RedirectToAction("About", "Home"); //mook Edit Later
            }
        }

        [SessionTimeout]
        public async Task<IActionResult> DisplayPDFAttachFile()
        {
            try
            {
                var PrintMasterCardMOModel = SessionExtentions.GetSession<PrintMasterCardMOModel>(HttpContext.Session, "MasterCardList");

                if (PrintMasterCardMOModel == null)
                {
                    return RedirectToAction("Index", "ManageMO");
                }
                var masterCardMOs = PrintMasterCardMOModel.MasterCardMOs.Where(m => !string.IsNullOrEmpty(m.AttchFilesBase64)).ToList();
                if (masterCardMOs.Count > 0)
                {
                    using (var compressedFileStream = new MemoryStream())
                    //Create an archive and store the stream in memory.
                    using (var zipArchive = new ZipArchive(compressedFileStream, ZipArchiveMode.Create, false))
                    {
                        foreach (var masterCardMO in masterCardMOs)
                        {
                            //Create a zip entry for each attachment
                            var zipEntry = zipArchive.CreateEntry($"{masterCardMO.OrderItem}_File.pdf");

                            //Get the stream of the attachment
                            using (var originalFileStream = new MemoryStream(Convert.FromBase64String(masterCardMO.AttchFilesBase64)))
                            using (var zipEntryStream = zipEntry.Open())
                            {
                                //Copy the attachment stream to the zip entry stream
                                originalFileStream.CopyTo(zipEntryStream);
                            }
                        }

                        return new FileContentResult(compressedFileStream.ToArray(), "application/zip") { FileDownloadName = $"AttachFile_PMTs({DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss")}).zip" };
                    }
                }
                else
                {
                    return RedirectToAction("Index", "ManageMO");
                }

                //return File(stream, "application/pdf", $"{orderItem}_File.pdf");
            }
            catch
            {
                return RedirectToAction("Index", "ManageMO");
            }
        }

        #endregion Print Mastercard/ MO

        #region [Sale Order]

        public IActionResult SaleOrder()
        {
            SaleOrderModel model = new SaleOrderModel();
            return View(model);
        }

        //public JsonResult SearchMoSpect(string OrderItem)
        //{
        //    bool isSuccess;
        //    string exceptionMessage = string.Empty;

        //    SaleOrderModel model = new SaleOrderModel();
        //    try
        //    {
        //        Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
        //        model = _saleOrderService.GetMoSpecByOrderItem(OrderItem);
        //        Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
        //        isSuccess = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
        //        isSuccess = false;
        //    }

        //    return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_PartialSaleOrderMoSpec", model), });
        //}

        [SessionTimeout]
        public JsonResult SearchMoSpect(string OrderItem)
        {
            string startSO = OrderItem;
            string endSO = "";
            bool isSuccess;
            string exceptionMessage = string.Empty;
            SaleOrderModel model = new SaleOrderModel();
            var saleOrders = new string[100];
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                List<MasterDataRoutingModel> masterDataRoutings = new List<MasterDataRoutingModel>();
                _masterCardService.SearchMasterCardMOByKeySearch(ref masterDataRoutings, startSO, endSO);
                model.MasterRoutings = masterDataRoutings.Where(w => w.MoStatus != "H").ToList();
                //ViewData["StartSO"] = startSO;
                //ViewData["EndSO"] = endSO;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                isSuccess = true;

                saleOrders = model.MasterRoutings.Where(w => w.MoStatus != "H").Select(m => m.SaleOrder).ToArray();
            }
            catch (Exception ex)
            {
                model = new SaleOrderModel();
                //ViewData["ErrorMessage"] = "* เกิดข้อผิดพลาด กรุณาระบุ SO ในการค้นหาใหม่";
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                isSuccess = false;
                return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_PartialSaleOrderMoSpec", model), saleOrders = saleOrders });
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_PartialSaleOrderMoSpec", model), saleOrders = saleOrders });
        }

        public IActionResult SaleOrderRouting(string saleOrder)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;

            SaleOrderModel model = new SaleOrderModel();
            model = _saleOrderService.BindMoRouting(saleOrder);
            model.OrderSelect = saleOrder;
            return View(model);
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult RenderTblRemark(string Machine)
        {
            SaleOrderModel model = new SaleOrderModel();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var models = _saleOrderService.GetBuildRemark();
                model.modelBuildRemark = models.modelBuildRemark.Where(x => x.Machine == Machine).OrderBy(x => x.List).ToList();
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return Json(RenderView.RenderRazorViewToString(this, "_Routing_TblRemarkList", model));
        }

        [SessionTimeout]
        public JsonResult RenderTblRemarkNosearch()
        {
            SaleOrderModel model = new SaleOrderModel();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var models = _saleOrderService.GetBuildRemark();
                model.modelBuildRemark = models.modelBuildRemark.OrderBy(x => x.List).ToList();
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return Json(RenderView.RenderRazorViewToString(this, "_Routing_TblRemarkList", model));
        }

        [SessionTimeout]
        public ActionResult _Routing_TblRemarkList()
        {
            SaleOrderModel model = new SaleOrderModel();
            return PartialView(model);
        }

        [SessionTimeout]
        public JsonResult GetWeight(string OrderItem)
        {
            List<string> weigth = new List<string>();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                weigth = _saleOrderService.GetWeight(OrderItem);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(weigth);
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json("");
            }
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult GetCalculateCorProp(string machine, string OrderItem)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                RoutingDataModel CalculateRoutingData = new RoutingDataModel();
                string exError = "";
                try
                {
                    exError = "0";
                    CalculateRoutingData = _saleOrderService.CalculateRouting(machine, OrderItem);
                }
                catch
                {
                    exError = "1";
                }
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(new { PaperRollWidth = CalculateRoutingData.PaperRollWidth, Cut = CalculateRoutingData.Cut, Trim = CalculateRoutingData.Trim, PercentTrim = CalculateRoutingData.PercentTrim, exError = exError });
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json("");
            }
        }

        [SessionTimeout]
        public JsonResult GetLastNumberInRouting(string OrderItem)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                string CheckData = "";
                string LastNoOut = "";
                string LastSheetWOut = "";
                string LastSheetLOut = "";
                string LastWidthOut = "";
                var model = _saleOrderService.GetMoRoutingByOrderItem(OrderItem);
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
            SaleOrderModel model = new SaleOrderModel();
            Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            return PartialView(model);
        }

        [SessionTimeout]
        public JsonResult GetListModalSeq(string OrderItem)
        {
            Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
            var model = _saleOrderService.GetMoRoutingByOrderItem(OrderItem);
            Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            return Json(RenderView.RenderRazorViewToString(this, "_Routing_DropdownSeq", model));
        }

        [SessionTimeout]
        public JsonResult CheckRemarkForFlex(string OrderItem)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                string CheckFlexResult = "";
                try
                {
                    var model = _saleOrderService.GetMoRoutingByOrderItem(OrderItem);
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
        public JsonResult CheckDisableButtonSaveByCopy(string OrderItem)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                string checkbtn = "";
                var model = _saleOrderService.GetMoRoutingByOrderItem(OrderItem);
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
        public JsonResult CheckDisableButtonSave(string OrderItem)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                string checkbtn = "";
                var model = _saleOrderService.GetMoRoutingByOrderItem(OrderItem);
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
        [HttpPost]
        public JsonResult GetMachineGroupToMangeSheetlength(string OrderItem)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var model = _saleOrderService.GetMoRoutingByOrderItem(OrderItem);
                string countMatchie = "0";
                foreach (var item in model.modelRouting.RoutingDataList)
                {
                    string MachineGroup = _saleOrderService.GetMachineGroupByMachine(item.Machine);
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
        public JsonResult GetRoutingData(int seqNo, string OrderItem)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var model = _saleOrderService.GetMoRoutingByOrderItem(OrderItem);
                var tmp = model.modelRouting.RoutingDataList.Where(x => x.SeqNo == seqNo).ToList();
                model.modelRouting.RoutingDataList.Clear();
                model.modelRouting.RoutingDataList = tmp;
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
        public PartialViewResult RoutingAddMachine(SaleOrderModel SaleOrderModel)
        {
            var model = _saleOrderService.GetMoRoutingByOrderItem(SaleOrderModel.OrderSelect);
            model.OrderSelect = SaleOrderModel.OrderSelect;
            model.MaterialNo = SaleOrderModel.MaterialNo;
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                RoutingDataModel routingModel = new RoutingDataModel();
                int seqNum = 0;
                _saleOrderService.MapperAddData(model, SaleOrderModel, ref routingModel, ref seqNum);
                model.modelRouting.RoutingDataList.Add(routingModel);
                _saleOrderService.SaveMoRouting(model);
                model.modelRouting.RoutingDataList.Clear();
                var modelselectnew = _saleOrderService.GetMoRoutingByOrderItem(SaleOrderModel.OrderSelect);
                model.modelRouting.RoutingDataList = modelselectnew.modelRouting.RoutingDataList;
                //var remarkAttachFileBase64 = HttpContext.Session.GetString("remarkAttachFile");
                //if (remarkAttachFileBase64 != null)
                //{
                //    model.modelRouting.RoutingDataList.First(i => i.SeqNo == seqNum).RemarkImageFile = remarkAttachFileBase64;
                //    model.modelRouting.RoutingDataList.First(i => i.SeqNo == seqNum).RemarkImageFileName = HttpContext.Session.GetString("remarkAttachFileName");
                //    model.modelRouting.RoutingDataList.First(i => i.SeqNo == seqNum).RemarkAttachFileStatus = 1;
                //    HttpContext.Session.Remove("remarkAttachFile");
                //    HttpContext.Session.Remove("remarkAttachFileName");
                //}
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
        public PartialViewResult RoutingUpdateMachine(SaleOrderModel modelToUpdate)
        {
            var model = _saleOrderService.GetMoRoutingByOrderItem(modelToUpdate.OrderSelect);
            model.OrderSelect = modelToUpdate.OrderSelect;
            model.MaterialNo = modelToUpdate.MaterialNo;
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var routingModel = _saleOrderService.MappingModelRoutingUpdateAndDelete(model, modelToUpdate);
                model = _saleOrderService.UpdateRouting(model, modelToUpdate, routingModel);
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
        public PartialViewResult RoutingInsertMachine(SaleOrderModel SaleOrderModel)
        {
            var model = _saleOrderService.GetMoRoutingByOrderItem(SaleOrderModel.OrderSelect);
            model.OrderSelect = SaleOrderModel.OrderSelect;
            model.MaterialNo = SaleOrderModel.MaterialNo;
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var routingModel = _saleOrderService.MappingModelRoutingUpdateAndDelete(model, SaleOrderModel);
                model = _saleOrderService.InsertRouting(model, SaleOrderModel, routingModel);
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
        public PartialViewResult RoutingCopy(int seqNo, string OrderItem, string Material)
        {
            var model = _saleOrderService.GetMoRoutingByOrderItem(OrderItem);
            model.OrderSelect = OrderItem;
            model.MaterialNo = Material;
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                model = _saleOrderService.CopyRouting(model, seqNo);
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
        public PartialViewResult RoutingSort(string Material, string OrderItem, int seqNo, string action)
        {
            var model = _saleOrderService.GetMoRoutingByOrderItem(OrderItem);
            model.OrderSelect = OrderItem;
            model.MaterialNo = Material;
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

                _saleOrderService.SaveMoRouting(model);
                model.modelRouting.RoutingDataList.Clear();
                var tmps = _saleOrderService.GetMoRoutingByOrderItem(OrderItem);
                model.modelRouting.RoutingDataList = tmps.modelRouting.RoutingDataList;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return PartialView("_RoutingDataList", model);
        }

        [SessionTimeout]
        public JsonResult CheckValidateSubmitRouting(string OrderItem)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                bool result = false;

                var model = _saleOrderService.GetMoRoutingByOrderItem(OrderItem);
                model.OrderSelect = OrderItem;

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
        public JsonResult CheckPresaleRoutingList(string OrderItem)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                bool result = false;
                var model = _saleOrderService.GetMoRoutingByOrderItem(OrderItem);
                model.OrderSelect = OrderItem;
                foreach (var item in model.modelRouting.RoutingDataList)
                {
                    string tmp = _saleOrderService.GetMachineGroupByMachine(item.Machine);
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
        public JsonResult ValidateOutsorcingBeforeSubmit(string OrderItem)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                bool result = false;
                var model = _saleOrderService.GetMoRoutingByOrderItem(OrderItem);
                model.OrderSelect = OrderItem;
                string plantcode = "";
                if (!string.IsNullOrEmpty(model.PlantOs))
                {
                    plantcode = model.PlantOs;
                }
                else if (!string.IsNullOrEmpty(model.moSpec.Plant))
                {
                    plantcode = model.moSpec.Plant;
                }
                var machineTmp = _saleOrderService.GetMachineDataByFactorycode(plantcode);
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
        public PartialViewResult RoutingDelete(string OrderItem, string Material, int seqNo)
        {
            var model = _saleOrderService.GetMoRoutingByOrderItem(OrderItem);
            model.OrderSelect = OrderItem;
            model.MaterialNo = Material;
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                //int seqNumber = 1;

                //model.modelRouting.RoutingDataList.RemoveAll(w => w.SeqNo == seqNo);

                //model.modelRouting.RoutingDataList.ForEach(i => { i.SeqNo = seqNumber; seqNumber++; });
                var tmp_delete = model.modelRouting.RoutingDataList.Where(x => x.SeqNo != seqNo).ToList();
                int seqNumber = 1;
                tmp_delete.ForEach(i => { i.SeqNo = seqNumber; seqNumber++; });
                model.modelRouting.RoutingDataList = tmp_delete;
                _saleOrderService.SaveMoRouting(model);

                model.modelRouting.RoutingDataList.Clear();
                var temp = _saleOrderService.GetMoRoutingByOrderItem(OrderItem);
                model.modelRouting.RoutingDataList = temp.modelRouting.RoutingDataList;

                //int seqNumber = 1;
                //model.modelRouting.RoutingDataList.ForEach(i => { i.SeqNo = seqNumber; seqNumber++; });
                //_saleOrderService.UpdateRouting(model);

                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return PartialView("_RoutingDataList", model);
        }

        //[SessionTimeout]
        //[HttpPost]
        //public JsonResult UpdateAndSavePlantByPlantOS()
        //{
        //    try
        //    {
        //        Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
        //        bool result = true;
        //        var model = SessionExtentions.GetSession<TransactionDataModel>(HttpContext.Session, "TransactionDataModel");
        //        string plantcode = "";
        //        if (!string.IsNullOrEmpty(model.PlantOs))
        //        {
        //            plantcode = model.PlantOs;
        //        }
        //        else if (!string.IsNullOrEmpty(model.modelProductInfo.PLANTCODE))
        //        {
        //            plantcode = model.modelProductInfo.PLANTCODE;
        //        }

        //        if (model.SapStatus)
        //        {
        //            model.modelRouting.RoutingDataList.ToList().ForEach(s => s.Plant = plantcode);
        //            _routingService.SaveRouting(model);
        //            if (model.SapStatus)
        //            {
        //                _routingService.UpdatePDISStatus(plantcode, model.MaterialNo, "M");
        //            }
        //            else
        //            {
        //                _routingService.UpdatePDISStatus(plantcode, model.MaterialNo, "C");
        //            }
        //        }
        //        else
        //        {
        //            // _routingService.SaveRouting(model);
        //            List<RoutingDataModel> tmpsum = new List<RoutingDataModel>();
        //            List<RoutingDataModel> tmpfac = new List<RoutingDataModel>();
        //            List<RoutingDataModel> lsfac = new List<RoutingDataModel>();
        //            tmpfac = model.modelRouting.RoutingDataList;
        //            lsfac = RecurRoutinglistList(tmpfac, model.FactoryCode);//tmprouting1.Select(c => { c.Plant = model.FactoryCode; return c; }).ToList();
        //            model.modelRouting.RoutingDataList.Clear();
        //            model.modelRouting.RoutingDataList = lsfac;
        //            _routingService.SaveRouting(model);

        //            if (model.SapStatus)
        //            {
        //                _routingService.UpdatePDISStatus(model.FactoryCode, model.MaterialNo, "M");
        //            }
        //            else
        //            {
        //                _routingService.UpdatePDISStatus(model.FactoryCode, model.MaterialNo, "C");
        //            }

        //            //tmpsum.InsertRange(0, ccc);
        //            List<RoutingDataModel> tmpplant = new List<RoutingDataModel>();
        //            List<RoutingDataModel> lsplant = new List<RoutingDataModel>();
        //            tmpplant = model.modelRouting.RoutingDataList;
        //            lsplant = RecurRoutinglistList2(tmpplant, plantcode);//tmprouting2.Select(d => { d.Plant = plantcode; return d; }).ToList();
        //            model.modelRouting.RoutingDataList.Clear();
        //            model.modelRouting.RoutingDataList = lsplant;
        //            _routingService.SaveRouting(model);
        //            //tmpsum.InsertRange(ccc.Count - 1 ,xxx);
        //            if (model.SapStatus)
        //            {
        //                _routingService.UpdatePDISStatus(plantcode, model.MaterialNo, "M");
        //            }
        //            else
        //            {
        //                _routingService.UpdatePDISStatus(plantcode, model.MaterialNo, "C");
        //            }

        //            foreach (var ite in lsfac)
        //            {
        //                RoutingDataModel list = new RoutingDataModel();
        //                list = ite;
        //                tmpsum.Add(list);
        //            }

        //            foreach (var ite2 in lsplant)
        //            {
        //                RoutingDataModel list = new RoutingDataModel();
        //                list = ite2;
        //                tmpsum.Add(list);
        //            }

        //            //model.modelRouting.RoutingDataList.ToList().ForEach(s => s.Plant = model.FactoryCode);
        //            //tmpfac = model.modelRouting.RoutingDataList;
        //            //tmpsum.AddRange(tmpfac);
        //            //model.modelRouting.RoutingDataList.ToList().ForEach(s => s.Plant = plantcode);
        //            //tmpplant= model.modelRouting.RoutingDataList;
        //            //tmpsum.AddRange(tmpplant);

        //            //foreach (var itemlst in tmpnew)
        //            //{
        //            //    tmp.Add(itemlst);
        //            //}

        //        }

        //        Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
        //        return Json(new { Result = result });
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
        //        return Json(false);
        //    }

        //}

        //[SessionTimeout]
        //public JsonResult ReCalculateRouting(string machine)
        //{
        //    try
        //    {
        //        Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
        //        var modelTrans = SessionExtentions.GetSession<TransactionDataModel>(HttpContext.Session, "TransactionDataModel");
        //        var CalculateRoutingData = _routingService.CalculateRouting(machine, modelTrans);
        //        //var CalculateRoutingData = _routingService.CalculateRouting(machine);
        //        Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
        //        return Json(new { PaperRollWidth = CalculateRoutingData.PaperRollWidth, Cut = CalculateRoutingData.Cut, Trim = CalculateRoutingData.Trim, PercentTrim = CalculateRoutingData.PercentTrim });
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
        //        return Json("");
        //    }
        //}
        //[SessionTimeout]
        //public JsonResult CheckMathDuplicates(string material)
        //{
        //    try
        //    {
        //        Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
        //        var model = SessionExtentions.GetSession<TransactionDataModel>(HttpContext.Session, "TransactionDataModel");
        //        var counts = model.modelRouting.RoutingDataList.Where(x => x.JoinToMaterialNo.Trim() == material).ToList().Count();
        //        string check = "0";
        //        if (counts > 0)
        //        {
        //            check = "1";
        //        }
        //        else
        //        {
        //            check = "0";
        //        }
        //        Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
        //        return Json(new { CheckData = check });
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
        //        return Json("");
        //    }

        //}

        //[SessionTimeout]
        //public JsonResult CheckHoneyCore(string OrderItem)
        //{
        //    try
        //    {
        //        Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
        //        var model = _saleOrderService.GetMoRoutingByOrderItem(OrderItem);
        //        var counts = model.modelRouting.RoutingDataList.ToList().Count();
        //        string check = "0";
        //        string result = "";
        //        try
        //        {
        //            if (counts > 0)
        //            {
        //                check = "0";
        //            }
        //            else
        //            {
        //                check = "1";
        //                try
        //                {
        //                    double tmp = model.moSpec.widHC * model.moSpec.lenHC;
        //                    result = tmp.ToString();

        //                }
        //                catch
        //                {
        //                    result = "0";
        //                }

        //            }

        //        }
        //        catch
        //        {
        //            check = "0";
        //            result = "0";
        //        }
        //        Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");

        //        return Json(new { CheckData = check, value = result });
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
        //        return Json("");
        //    }
        //}

        //[SessionTimeout]
        //public JsonResult GetQualityspec(string MaterialNo)
        //{
        //    try
        //    {
        //        Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
        //        var listquality = _saleOrderService.GetQualitySpecsByMaterial(MaterialNo).FirstOrDefault();
        //        string result = "";
        //        if (listquality != null)
        //        {
        //            result = listquality.Name + " " + listquality.Value + " " + listquality.Unit;
        //        }
        //        Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
        //        return Json(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
        //        return Json("");
        //    }

        //}

        [SessionTimeout]
        [HttpPost]
        public JsonResult SetMasterCardMOProductCatalogs(string mat, string fac)
        {
            var list = new List<MasterCardMO>();
            var Mo = new MasterCardMO();
            var masterCardMoModel = new PrintMasterCardMOModel();
            var isSuccess = false;
            var errorMessage = "";
            var errorMaterial = "";
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                //if (model.OrderItem.Count > 0)
                //{
                //    //set number of public print master card mo
                //    foreach (var i in model.OrderItem)
                //    {
                //        if (!model.IsPreview)
                //        {
                //            _masterCardService.UpdatePublicPrinted(i);
                //        }

                //        errorMaterial = i;
                //        Mo = _masterCardService.GetMasterCardMO(i, model.IsPreview);
                //        if (!Mo.IsXStatus)
                //        {
                //            list.Add(Mo);
                //        }
                //    }
                //}
                //else if (model.MaterialNo.Count > 0)
                //{
                //    var groupmate = model.MaterialNo.Distinct();

                //    foreach (var i in groupmate)
                //    {
                //        errorMaterial = i;
                //        Mo = _masterCardService.GetMasterCard(i);
                //        list.Add(Mo);
                //    }
                //}
                //else
                //{
                //    throw new ArgumentException("error");
                //}

                Mo = _masterCardService.GetMasterCardProductCatalog(mat, fac);
                list.Add(Mo);

                if (list.Count > 0)
                {
                    masterCardMoModel.MasterCardMOs = list;
                    masterCardMoModel.SizeOfPage = 0;
                    //if (!string.IsNullOrEmpty(""))
                    //{
                    //    masterCardMoModel.FileName = "";
                    //    masterCardMoModel.IsPrintedFromFile = true;
                    //}

                    SessionExtentions.SetSession(HttpContext.Session, "MasterCardList", masterCardMoModel);

                    isSuccess = true;
                }
                else
                {
                    isSuccess = false;
                    errorMessage = 0 > 0 ? "Can't find sale order number or sale order status has been deleted" : "Can't find material number";
                }

                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                list = new List<MasterCardMO>();
                masterCardMoModel.MasterCardMOs = list;
                masterCardMoModel.SizeOfPage = 0;

                SessionExtentions.SetSession(HttpContext.Session, "MasterCardList", masterCardMoModel);
                if (0 > 0)
                {
                    errorMessage = $"เกิดข้อผิดพลาดในการพิมพ์ Sale Order {errorMaterial} โปรดลองใหม่";
                }
                else
                {
                    errorMessage = $"เกิดข้อผิดพลาดในการพิมพ์ Material NO. {errorMaterial} ข้อมูลบางส่วนไม่สมบูรณ์ กรุณาเข้าไปแก้ไขกด next step จนจบกระบวนการ";
                }
                //errorMessage = ex.Message;
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                RedirectToAction("MasterCardMOViewList", "Report");
            }

            return Json(new { IsSuccess = isSuccess, ErrorMessage = errorMessage });
        }

        #endregion [Sale Order]

        #region Report Check Order

        #region Report 11.1

        [SessionTimeout]
        public IActionResult ReportCheckMOAndKIWI()
        {
            var moDatas = new List<MoDataPrintMastercard>();

            return View(moDatas);
        }

        [SessionTimeout]
        public JsonResult SearchReportCheckMOAndKIWI(string startDueDate, string endDueDate)
        {
            bool isSuccess;
            var exceptionMessage = string.Empty;
            var moDatas = new List<MoDataPrintMastercard>();
            var allCheckReportOrder = string.Empty;
            try
            {
                //call get report check MO and KIWI  from api
                _reportService.SearchReportCheckMOAndKIWI(configuration, ref moDatas, startDueDate, endDueDate);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                exceptionMessage = ex.Message;
                isSuccess = false;
                return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_ReportCheckMOAndKIWITable", moDatas) });
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_ReportCheckMOAndKIWITable", moDatas) });
        }

        #endregion Report 11.1

        #region Report 11.2

        [SessionTimeout]
        public IActionResult ReportCheckMOAndTextfileSAP()
        {
            //var moDatas = new List<MoData>();
            var result = new ReportCheckMOAndTextfileSAPViewModel();
            result.moDatas = new List<MoData>();
            result.ConfigWordingReport = _reportService.GetConfigWordingReportByFactoryCode();
            if (result.ConfigWordingReport == null)
            {
                result.ConfigWordingReport = new ConfigWordingReport();
                result.ConfigWordingReport.Wording = "";
            }
            return View(result);
        }

        [SessionTimeout]
        public JsonResult SearchReportCheckMOAndSAP(string startDueDate, string endDueDate, string configWordingString)
        {
            bool isSuccess;
            var exceptionMessage = string.Empty;
            var result = new ReportCheckMOAndTextfileSAPViewModel();
            result.moDatas = new List<MoData>();
            var allCheckReportOrder = string.Empty;
            try
            {
                //call get report check MO and SAP  from api
                _reportService.SearchReportCheckMOAndSAP(configuration, ref result, startDueDate, endDueDate, configWordingString);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                exceptionMessage = ex.Message;
                isSuccess = false;
                return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_ReportCheckMOAndTextfileSAPTable", result) });
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_ReportCheckMOAndTextfileSAPTable", result) });
        }

        [SessionTimeout]
        public JsonResult CreateConfigWordingReport(string configWordingString)
        {
            bool isSuccess;
            var exceptionMessage = string.Empty;

            var allCheckReportOrder = string.Empty;
            try
            {
                var configWordingModel = new ConfigWordingReport
                {
                    Wording = configWordingString
                };
                var configWording = _reportService.CreateConfigWordingReport(JsonConvert.SerializeObject(configWordingModel));
                if (configWording == null)
                {
                    isSuccess = false;
                }

                isSuccess = true;
            }
            catch (Exception ex)
            {
                exceptionMessage = ex.Message;
                isSuccess = false;
                return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage });
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage });
        }

        #endregion Report 11.2

        #region Report 11.3

        [SessionTimeout]
        public IActionResult ReportCheckRepeatOrder()
        {
            var checkRepeatOrders = new List<CheckRepeatOrder>();

            return View(checkRepeatOrders);
        }

        [SessionTimeout]
        public JsonResult SearchReportCheckRepeatOrder(string startDueDate, string endDueDate, string repeatCount)
        {
            bool isSuccess;
            var exceptionMessage = string.Empty;
            var checkRepeatOrders = new List<CheckRepeatOrder>();
            var allCheckReportOrder = string.Empty;
            try
            {
                //call get report check repeat order  from api
                _reportService.SearchReportRepeatOrderItemsByDateAndRepeatCount(ref checkRepeatOrders, startDueDate, endDueDate, repeatCount);

                SessionExtentions.SetSession(httpContextAccessor.HttpContext.Session, "ReportCheckRepeatOrder", checkRepeatOrders);
                if (checkRepeatOrders.Count > 0)
                {
                    checkRepeatOrders.Where(c => !c.MaterialNo.Contains("Z02S/B")).ToList();
                }

                allCheckReportOrder = JsonConvert.SerializeObject(checkRepeatOrders);
                checkRepeatOrders = checkRepeatOrders.GroupBy(x => new { x.MaterialNo, x.DueDate, x.repeatCount, x.PoNo, x.OrderQuant, x.PC, x.FactoryCode }).Select(g => g.First()).ToList();
                isSuccess = true;
            }
            catch (Exception ex)
            {
                exceptionMessage = ex.Message;
                isSuccess = false;
                return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_ReportCheckRepeatOrderTable", checkRepeatOrders), ViewAll = RenderView.RenderRazorViewToString(this, "_RepeatOrderItemsAllTable", new List<CheckRepeatOrder>()) });
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_ReportCheckRepeatOrderTable", checkRepeatOrders), ViewAll = RenderView.RenderRazorViewToString(this, "_RepeatOrderItemsAllTable", JsonConvert.DeserializeObject<List<CheckRepeatOrder>>(allCheckReportOrder)) });
        }

        [SessionTimeout]
        public JsonResult SearchRepeatOrderItems(string materialNO, string poNo, string dueDate, string orderQuant)
        {
            bool isSuccess;
            var exceptionMessage = string.Empty;
            var checkRepeatOrders = new List<CheckRepeatOrder>();
            try
            {
                poNo = string.IsNullOrEmpty(poNo) ? "" : poNo;
                checkRepeatOrders = SessionExtentions.GetSession<List<CheckRepeatOrder>>(HttpContext.Session, "ReportCheckRepeatOrder");

                checkRepeatOrders = checkRepeatOrders.Where(c => c.MaterialNo == materialNO && c.PoNo == poNo && c.DueDate == Convert.ToDateTime(dueDate) && c.OrderQuant == Convert.ToInt32(orderQuant)).ToList();

                isSuccess = true;
            }
            catch (Exception ex)
            {
                exceptionMessage = ex.Message;
                isSuccess = false;
                return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_RepeatOrderItemsTable", checkRepeatOrders) });
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_RepeatOrderItemsTable", checkRepeatOrders) });
        }

        #endregion Report 11.3

        #region Report 11.4

        [SessionTimeout]
        public IActionResult ReportCheckDiffDuedate()
        {
            return View();
        }

        [SessionTimeout]
        public JsonResult SearchReportCheckDiffDueDate(string startDueDate, string endDueDate, int repeatCount)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;
            var checkOrderQtyTooMuch = new List<CheckDiffDueDate>();

            try
            {
                _reportService.SearchReportCheckDiffDueDate(ref checkOrderQtyTooMuch, repeatCount, startDueDate, endDueDate);
                isSuccess = true;
                // checkOrderQtyTooMuch = checkOrderQtyTooMuch.OrderBy(x => x.DueDate).ToList();
            }
            catch (Exception ex)
            {
                exceptionMessage = ex.Message;
                isSuccess = false;
                return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_ReportCheckDiffDuedate", checkOrderQtyTooMuch) });
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_ReportCheckDiffDuedate", checkOrderQtyTooMuch) });
        }

        #endregion Report 11.4

        #region Report 11.5

        [SessionTimeout]
        public IActionResult ReportCheckDuedateToolong()
        {
            return View();
        }

        [SessionTimeout]
        public JsonResult SearchReportCheckDuedateToolong(int repeatCount)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;
            var checkOrderQtyTooMuch = new List<CheckDueDateToolong>();

            try
            {
                _reportService.ReportCheckDueDateToolong(ref checkOrderQtyTooMuch, repeatCount);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                exceptionMessage = ex.Message;
                isSuccess = false;
                return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_ReportCheckDuedateToolong", checkOrderQtyTooMuch) });
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_ReportCheckDuedateToolong", checkOrderQtyTooMuch) });
        }

        #endregion Report 11.5

        #region Report 11.6

        [SessionTimeout]
        public IActionResult ReportCheckOrderQtyTooMuch()
        {
            return View(new List<CheckOrderQtyTooMuch>());
        }

        [SessionTimeout]
        public JsonResult SearchReportCheckOrderQtyTooMuch(string startDueDate, string endDueDate, string repeatCount)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;
            var checkOrderQtyTooMuch = new List<CheckOrderQtyTooMuch>();

            try
            {
                //call get report check orderQty too much from api
                _reportService.SearchReportCheckOrderQtyTooMuchByDateAndRepeatCount(ref checkOrderQtyTooMuch, startDueDate, endDueDate, repeatCount);
                //checkOrderQtyTooMuch.Add(new CheckOrderQtyTooMuch
                //{
                //    MaterialNo = "Z1111111",
                //    PC = "12312-1234-OC",
                //    Name = "บริษัท พานาโซนิค แมนูแฟคเจอริ่ง จำกัด 01",
                //    OrderQuant = 142,
                //    SumQty = 123,
                //    CountTime = 2,
                //    AvgQty = 5,
                //});

                //checkOrderQtyTooMuch.Add(new CheckOrderQtyTooMuch
                //{
                //    MaterialNo = "Z22222222",
                //    PC = "12312-1234-OC",
                //    Name = "บริษัท พานาโซนิค แมนูแฟคเจอริ่ง จำกัด 02",
                //    OrderQuant = 142,
                //    SumQty = 123,
                //    CountTime = 2,
                //    AvgQty = 5,
                //});

                isSuccess = true;
            }
            catch (Exception ex)
            {
                exceptionMessage = ex.Message;
                isSuccess = false;
                return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_ReportCheckOrderQtyTooMuchTable", checkOrderQtyTooMuch) });
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_ReportCheckOrderQtyTooMuchTable", checkOrderQtyTooMuch) });
        }

        #endregion Report 11.6

        #region report 11.7

        [SessionTimeout]
        public IActionResult ReportCheckOrderItem()
        {
            return View(new ReportCheckOrderItem());
        }

        public JsonResult SearchData(string startdate, string enddate)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;
            try
            {
                var model = _reportService.ReportCheckOrederItem(startdate, enddate);
                isSuccess = true;
                return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_ReportCheckOrderItem", model) });
            }
            catch (Exception ex)
            {
                isSuccess = false;
                return Json(new { IsSuccess = isSuccess, ExceptionMessage = ex.Message, View = "" });
            }
        }

        [SessionTimeout]
        public async Task<IActionResult> ReportCheckOrderItemExportExcel(string startdate, string enddate)
        {
            var model = _reportService.ReportCheckOrederItem(startdate, enddate);
            // _productCatalog.CreateDynamicDataTable(data);
            //await _context.BusinessUnits.ToListAsync();
            // above code loads the data using LINQ with EF (query of table), you can substitute this with any data source.
            var streamer = new MemoryStream();

            using (var package = new ExcelPackage(streamer))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                //Set some properties of the Excel document
                package.Workbook.Properties.Author = "PMTs";
                package.Workbook.Properties.Title = "รายงานข้อมูลกำหนดส่ง จำนวนสั่ง ไม่ตรงกันใน ERP-PMTs-Planning";
                package.Workbook.Properties.Subject = "รายงานข้อมูลกำหนดส่ง จำนวนสั่ง ไม่ตรงกันใน ERP-PMTs-Planning";
                package.Workbook.Properties.Created = DateTime.Now;

                ExcelWorksheet workSheet = package.Workbook.Worksheets.Add("ReportCheckOrderItem");
                // workSheet.Cells[1, 1].Value = "Code";
                //GetOrderItemDataReport testkiwi = new GetOrderItemDataReport();
                //testkiwi.OrderItem = "0001";
                //testkiwi.DueDate = "23-23-2020";
                //model.ReportKiwi.Add(testkiwi);
                //model.ReportMo.Add(testkiwi);

                workSheet.Cells["A1"].Value = "OrderItem";
                workSheet.Cells["A1:A2"].Merge = true;
                workSheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                workSheet.Cells["B1"].Value = "SAP";
                workSheet.Cells["B1:C1"].Merge = true;
                workSheet.Cells["B1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Cells["B2"].Value = "OrderQty";
                workSheet.Cells["B2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Cells["C2"].Value = "Due_Date";
                workSheet.Cells["C2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                workSheet.Cells["D1"].Value = "PMTs";
                workSheet.Cells["D1:E1"].Merge = true;
                workSheet.Cells["D1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Cells["D2"].Value = "OrderQty";
                workSheet.Cells["D2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Cells["E2"].Value = "Due_Date";
                workSheet.Cells["E2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                //workSheet.Cells["F1"].Value = "Planning";
                //workSheet.Cells["F1:G1"].Merge = true;
                //workSheet.Cells["F1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                //workSheet.Cells["F2"].Value = "OrderQty";
                //workSheet.Cells["F2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                //workSheet.Cells["G2"].Value = "Due_Date";
                //workSheet.Cells["G2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                workSheet.Cells["A3"].LoadFromCollection(model.ReportFinal);
                //workSheet.Cells["C3"].LoadFromCollection(model.ReportMo);
                //workSheet.Cells["E3"].LoadFromCollection(model.ReportKiwi);

                workSheet.Cells.AutoFitColumns();
                var headerCell = workSheet.Cells["A1:E1"];
                headerCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headerCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.BurlyWood);
                var headerFont = headerCell.Style.Font;
                headerFont.Bold = true;

                var headerCell2 = workSheet.Cells["A2:E2"];
                headerCell2.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headerCell2.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.BurlyWood);
                var headerFont2 = headerCell2.Style.Font;
                headerFont2.Bold = true;

                //workSheet.Cells.LoadFromCollection(myBUs, true);
                //workSheet.Cells.AutoFitColumns();
                //var headerCell = workSheet.Cells["A1:BP1"];
                //headerCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                //headerCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.BurlyWood);
                //var headerFont = headerCell.Style.Font;
                //headerFont.Bold = true;

                package.Save();
            }
            streamer.Position = 0;

            string excelName = $"รายงานข้อมูลกำหนดส่ง จำนวนสั่ง ไม่ตรงกันใน ERP-PMTs-Planning-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";
            // above I define the name of the file using the current datetime.

            return File(streamer, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName); // this will be the actual export.
        }

        #endregion report 11.7

        #region Report 11.8

        [SessionTimeout]
        public IActionResult ReportCheckSOForBilling()
        {
            var moDatas = new List<MoData>();

            return View(moDatas);
        }

        [SessionTimeout]
        public JsonResult SearchReportCheckSOForBilling(string startDueDate, string endDueDate)
        {
            bool isSuccess;
            var exceptionMessage = string.Empty;
            var moDatas = new List<MoDataPrintMastercard>();
            var allCheckReportOrder = string.Empty;
            try
            {
                //call get report check MO is s status and KIWI  from api
                _reportService.SearchReportCheckMOWithSStatusAndKIWI(configuration, ref moDatas, startDueDate, endDueDate);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                exceptionMessage = ex.Message;
                isSuccess = false;
                return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_ReportCheckSOForBillingTable", moDatas) });
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_ReportCheckSOForBillingTable", moDatas) });
        }

        #endregion Report 11.8

        #region Report M/O Manual

        [SessionTimeout]
        public IActionResult ReportMOManual()
        {
            var moDatas = new List<CheckRepeatOrder>();
            return View(moDatas);
        }

        [SessionTimeout]
        public JsonResult SearchReportMOManual(string custName, string materialNo, string pc, string startDueDate, string endDueDate, string startCreateDate, string endCreateDate, string startUpdateDate, string endUpdateDate, string po, string so, string note, string soStatus)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;
            var moDatas = new List<CheckRepeatOrder>();

            try
            {
                _reportService.SearchReportMOManual(ref moDatas, materialNo, custName, pc, startDueDate, endDueDate, startCreateDate, endCreateDate, startUpdateDate, endUpdateDate, po, so, note, soStatus);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                exceptionMessage = ex.Message;
                isSuccess = false;
                moDatas = new List<CheckRepeatOrder>();
                return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_ReportMOManualTable", moDatas) });
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_ReportMOManualTable", moDatas) });
        }

        #endregion Report M/O Manual

        #region Report Check Status Color

        [SessionTimeout]
        public IActionResult ReportCheckStatusColor()
        {
            var checkStatusColorView = new CheckStatusColorViewModel();
            checkStatusColorView.CheckStatusColors = new List<CheckStatusColor>();
            checkStatusColorView.Colors = new List<DataAccess.Models.Color>();
            checkStatusColorView.Colors = routingService.GetInkShadeList();
            return View(checkStatusColorView);
        }

        [SessionTimeout]
        public JsonResult SearchReportCheckStatusColor(int colorId)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;
            var statusColors = new List<CheckStatusColor>();

            try
            {
                _reportService.SearchReportCheckStatusColor(ref statusColors, colorId);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                exceptionMessage = ex.Message;
                isSuccess = false;
                statusColors = new List<CheckStatusColor>();
                return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_ReportCheckStatusColorTable", statusColors) });
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_ReportCheckStatusColorTable", statusColors) });
        }

        #endregion Report Check Status Color

        #endregion Report Check Order
    }
}