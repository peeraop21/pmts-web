using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView.BomRawMaterial;
using PMTs.Logs.Logger;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PMTs.WebApplication.Controllers
{
    public class BomRawController : Controller
    {
        private readonly IBomRawMaterialService _bomRawMaterialService;

        public BomRawController(IBomRawMaterialService bomRawMaterialService)
        {
            _bomRawMaterialService = bomRawMaterialService;
        }
        public IActionResult Index(string materialNo)
        {
            BomRawMaterialViewModel model = new BomRawMaterialViewModel();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                model.Lst_UnitOfMeasureCode = _bomRawMaterialService.GetListUnitOfMeasureCode();
                model.MaterialNoSearch = !string.IsNullOrEmpty(materialNo) ? materialNo : string.Empty;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return View(model);
        }

        [SessionTimeout]
        [HttpGet]
        public JsonResult SearchFgMaterial(string materialNo)
        {
            BomRawMaterialViewModel model = new BomRawMaterialViewModel();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                model.MasterDataList = _bomRawMaterialService.SearchMasterDataByMaterialNo(materialNo);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(new
                {
                    IsSuccess = true,
                    ExceptionMessage = "",
                    View = RenderView.RenderRazorViewToString(this, "_SearchFgModal", model),
                    RowCount = model.MasterDataList.Count,
                    MasterData = model.MasterDataList != null && model.MasterDataList.Count == 1 ?
                    JsonConvert.SerializeObject(model.MasterDataList.FirstOrDefault()) : ""
                });
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json(new { IsSuccess = false, ExceptionMessage = ex.Message });
            }

        }

        [SessionTimeout]
        [HttpGet]
        public JsonResult SearchRawMaterial(string materialNo, string materialDesc)
        {
            BomRawMaterialViewModel model = new BomRawMaterialViewModel();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                model.PPCRawMaterialMastersList = _bomRawMaterialService.SearchPPCRawMaterialMasterByMaterialNo(materialNo, materialDesc);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(new { IsSuccess = true, ExceptionMessage = "", View = RenderView.RenderRazorViewToString(this, "_RawMaterialTable", model) });
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json(RenderView.RenderRazorViewToString(this, "_SearchRawMaterial", null));
            }

        }

        [SessionTimeout]
        [HttpGet]
        public JsonResult GetRawMaterialProductionBomByFGMaterial(string fgMaterial)
        {
            List<RawMaterialLineFront> model = new List<RawMaterialLineFront>();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                model = _bomRawMaterialService.GetRawMaterialProductionBomByFGMaterial(fgMaterial);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(new { IsSuccess = true, ExceptionMessage = "", Data = model });
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json(RenderView.RenderRazorViewToString(this, "_SearchRawMaterial", null));
            }

        }

        [SessionTimeout]
        [HttpGet]
        public JsonResult CheckMaterialNo(string materialNo)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var result = _bomRawMaterialService.CheckMaterialNo(materialNo);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(result);
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json(ex.Message);
            }
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult InsertOrUpdateRawMat([FromBody] RawMaterialLineRequest request)
        {
            BomRawMaterialViewModel model = new BomRawMaterialViewModel();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _bomRawMaterialService.SaveRawMaterialProductionBom(request);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(new { IsSuccess = true, ExceptionMessage = "", View = RenderView.RenderRazorViewToString(this, "_SearchRawMaterial", model) });
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json(RenderView.RenderRazorViewToString(this, "_SearchRawMaterial", null));
            }
        }

        [SessionTimeout]
        [HttpGet]
        public JsonResult GetUnitOfMeasureCode()
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var result = _bomRawMaterialService.GetListUnitOfMeasureCode();
                result.Insert(0, new SelectListItem { Text = "", Value = "" });
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(result);
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json(ex.Message);
            }
        }
        [SessionTimeout]
        [HttpPut]
        public JsonResult DeleteRawMaterial(int Id)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _bomRawMaterialService.DeleteRawMaterial(Id);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(new { IsSuccess = true, ExceptionMessage = "" });
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json(new { IsSuccess = false, ExceptionMessage = ex.Message });
            }
        }

        #region Raw Material Master  

        [SessionTimeout]
        public IActionResult RawMaterialMaster()
        {
            RawMaterialMasterViewModel model = new RawMaterialMasterViewModel();
            model.PpcRawMaterialMasters = new List<DataAccess.Models.PpcRawMaterialMaster>();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                model.PpcRawMaterialMasters = _bomRawMaterialService.SearchPPCRawMaterialMastersByFactoryCode();
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return View(model);
        }

        [SessionTimeout]
        public JsonResult SearchRawMaterialMaster(string materialNo, string description)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;
            var model = new RawMaterialMasterViewModel();
            model.PpcRawMaterialMasters = new List<DataAccess.Models.PpcRawMaterialMaster>();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                if (string.IsNullOrEmpty(materialNo) && string.IsNullOrEmpty(description))
                {
                    model.PpcRawMaterialMasters = _bomRawMaterialService.SearchPPCRawMaterialMastersByFactoryCode();
                }
                else
                {
                    model.PpcRawMaterialMasters = _bomRawMaterialService.SearchPPCRawMaterialMastersByFactoryAndMaterialNoAndDescription(materialNo, description);
                }

                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                exceptionMessage = ex.Message;
                isSuccess = false;
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_RawMaterialMasterTable", model) });
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult UpdateRawMaterialMaster(PpcRawMaterialMaster ppcRawMaterialMaster)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;
            var model = new RawMaterialMasterViewModel();
            model.PpcRawMaterialMasters = new List<DataAccess.Models.PpcRawMaterialMaster>();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _bomRawMaterialService.UpdateRawMaterialMaster(ppcRawMaterialMaster);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                exceptionMessage = ex.Message;
                isSuccess = false;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_RawMaterialMasterTable", model) });
        }

        [SessionTimeout]
        [HttpPut]
        public JsonResult DeleteRawMaterialMaster(int Id)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _bomRawMaterialService.DeleteRawMaterialMaster(Id);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                isSuccess = true;
            }
            catch (Exception ex)
            {
                isSuccess = false;
                exceptionMessage = ex.Message;
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage });
        }
        [SessionTimeout]
        [HttpPost]
        public JsonResult AddRawMaterialMaster(PpcRawMaterialMaster ppcRawMaterialMaster)
        {
            bool isSuccess;
            string exceptionMessage = string.Empty;
            var model = new RawMaterialMasterViewModel();
            model.PpcRawMaterialMasters = new List<DataAccess.Models.PpcRawMaterialMaster>();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _bomRawMaterialService.SaveRawMaterialMaster(ppcRawMaterialMaster);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                exceptionMessage = ex.Message;
                isSuccess = false;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_RawMaterialMasterTable", model) });
        }
        #endregion
    }
}
