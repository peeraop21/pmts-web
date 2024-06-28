using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PMTs.DataAccess;
using PMTs.DataAccess.ModelPresale;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.NewProduct;
using PMTs.Logs.Logger;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Services;
using PMTs.WebApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Controllers
{
    [SessionTimeout]
    public class PresaleController : Controller
    {
        private static PresaleContext _presaleContext;
        private static IConfiguration _configuration;
        private readonly IHostingEnvironment _environment;
        private readonly IPresaleService _presaleService;
        private readonly IExtensionService _extensionService;
        private readonly IHttpContextAccessor httpContextAccessor;

        public PresaleController(PresaleContext presaleContext
            , IConfiguration configuration
            , IHostingEnvironment IHostingEnvironment
            , IPresaleService presaleService
            , IExtensionService extensionService
            , IHttpContextAccessor httpContextAccessor)
        {
            _presaleContext = presaleContext;
            _configuration = configuration;
            _environment = IHostingEnvironment;
            _presaleService = presaleService;
            _extensionService = extensionService;
            this.httpContextAccessor = httpContextAccessor;
        }

        #region View
        #region New Product
        [SessionTimeout]
        public IActionResult Index()
        {
            SessionExtentions.SetSession(httpContextAccessor.HttpContext.Session, "ImportPresaleChangeNewMat", new PresaleMasterData());

            PresaleViewModel p = new PresaleViewModel();
            p.PresaleList = new List<PresaleViewModel>();
            return View(p);
        }

        [SessionTimeout]
        public IActionResult SearchPresale(PresaleViewModel presale)
        {
            PresaleViewModel model = new PresaleViewModel();
            model.PresaleList = new List<PresaleViewModel>();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                model.PresaleList = _presaleService.SearchPresale(_configuration, presale);
                model.PresaleList.ForEach(p => p.Board = string.Join("", Regex.Split(p.Board, @"(?:\r\n|\n|\r)")));
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                // return view error
            }

            return View("Index", model);
        }

        #endregion

        #region Change Product
        [SessionTimeout]
        public IActionResult ChangeProduct()
        {
            PresaleChangeViewModel presaleChangeViewModel = new PresaleChangeViewModel();

            try
            {
                _presaleService.GetPresaleChangeProduct(ref presaleChangeViewModel, string.Empty);
            }
            catch (Exception ex)
            {

            }

            return View(presaleChangeViewModel);
        }

        [SessionTimeout]
        public JsonResult SearchChangeProduct(string typeSearch, string keySearch)
        {
            var isSuccess = false;
            var isExistPresale = false;
            var message = "";
            var presaleChangeViewModel = new PresaleChangeViewModel();
            presaleChangeViewModel.PresaleChangeProducts = new List<PresaleChangeProduct>();

            try
            {
                //call get presale change product from api
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

                if (!string.IsNullOrEmpty(keySearch))
                {
                    _presaleService.GetPresaleChangeProductByKeySearch(ref presaleChangeViewModel, typeSearch, keySearch);
                }
                else
                {
                    _presaleService.GetPresaleChangeProduct(ref presaleChangeViewModel, string.Empty);
                }

                if (presaleChangeViewModel.PresaleChangeProducts != null && presaleChangeViewModel.PresaleChangeProducts.ToList().Count > 0)
                {
                    isExistPresale = true;
                }

                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");

            }
            catch (Exception ex)
            {
                message = ex.Message;
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = message, View = RenderView.RenderRazorViewToString(this, "_TableChangeProduct", presaleChangeViewModel), isExistPresale = isExistPresale });
        }
        #endregion

        #region Change Product New Mat
        [SessionTimeout]
        public IActionResult ChangeProductNewMat()
        {
            PresaleViewModel p = new PresaleViewModel();
            p.PresaleList = new List<PresaleViewModel>();
            return View(p);
        }

        [SessionTimeout]
        public IActionResult SearchChangeProductNewMat(PresaleViewModel presale)
        {
            PresaleViewModel p = new PresaleViewModel();
            p.PresaleList = new List<PresaleViewModel>();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                p.PresaleList = _presaleService.SearchChangeProductNewMat(_configuration, presale);
                p.PresaleList = p.PresaleList != null && p.PresaleList.Count > 0 ? p.PresaleList.Where(p => !string.IsNullOrEmpty(p.Material_No)).ToList() : p.PresaleList;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                // return view error
            }

            return View("ChangeProductNewMat", p);
        }

        #endregion

        #region CompareProduct

        [SessionTimeout]
        public IActionResult CompareProduct(string materialNo, string psmId)
        {
            PresaleChangeViewModel presaleChangeViewModel = new PresaleChangeViewModel();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _presaleService.GetCompareProduct(ref presaleChangeViewModel, materialNo, psmId);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                // return view error
            }

            return View(presaleChangeViewModel);
        }


        #endregion

        #endregion

        #region Function
        [SessionTimeout]
        [HttpPost]
        public JsonResult CheckImportPresale(string board, string flute, string psmId)
        {
            var hasBoard = true;
            var massage = "";

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                //board = !string.IsNullOrEmpty(board) ? board.Trim() : board;
                //hasBoard = _presaleService.CheckImportPresale(board, flute);

                if (!string.IsNullOrEmpty(psmId))
                {
                    var presaleSelected = _presaleService.SearchPresaleByPsmId(_configuration, psmId);
                    SessionExtentions.SetSession(httpContextAccessor.HttpContext.Session, "ImportPresaleChangeNewMat", presaleSelected);

                }

                //if (!hasBoard)
                //{
                //    massage = "ไม่มี board \"" + board + "\" ในระบบ PMTs กรุณาเพิ่ม board ดังกล่าวเข้าระบบก่อน";
                //}

                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                hasBoard = false;
                massage = ex.Message;
            }

            return Json(new { HasBoard = hasBoard, Message = massage });
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult ConfirmChangeProduct(string materialNo, string psmId)
        {
            var isSuccess = true;
            var message = "";

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _presaleService.ChangeProductPresaleToMaster(materialNo, psmId);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                isSuccess = false;
                message = ex.Message;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = message });
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult CheckCompareMasterData(string facrotyCode, string materialNo)
        {
            var hasMasterData = false;
            var massage = "";

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var isExistMasterData = _presaleService.CheckExistMasterData(facrotyCode, materialNo);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                hasMasterData = isExistMasterData;

            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                hasMasterData = false;
                massage = ex.Message;
            }

            return Json(new { HasMasterData = hasMasterData });
        }
        [SessionTimeout]
        [HttpPost]
        public JsonResult RejectPresale(string psmId)
        {
            var isSuccess = true;
            var message = "";

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _presaleService.RejectPresale(psmId);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                isSuccess = false;
                message = ex.Message;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = message });
        }
        [SessionTimeout]
        [HttpPost]
        public JsonResult DeletePresale(string psmId)
        {
            var isSuccess = true;
            var message = "";

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _presaleService.DeletePresale(psmId);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                isSuccess = false;
                message = ex.Message;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = message });
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult SentPresaleNewMatStatus(string psmId, string status)
        {
            var isSuccess = false;

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                if (status == "2")
                {
                    _presaleService.ApprovePresale(psmId);
                }
                else if (status == "4")
                {
                    _presaleService.RejectPresale(psmId);
                }
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                isSuccess = false;
            }

            return Json(new { IsSuccess = isSuccess });
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult SentPresaleSameMatStatus(string psmId, int id, string status)
        {
            var isSuccess = false;

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                if (status == "2")
                {
                    _presaleService.ApprovePresaleChangeSameMat(psmId, id);
                }
                else if (status == "4")
                {
                    _presaleService.RejectPresaleChangeSameMat(psmId, id);
                }
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                isSuccess = false;
            }

            return Json(new { IsSuccess = isSuccess });
        }

        #endregion
    }
}