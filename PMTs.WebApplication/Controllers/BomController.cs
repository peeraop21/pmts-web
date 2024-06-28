using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using PMTs.DataAccess;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.Repository;
using PMTs.Logs.Logger;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Services;
using PMTs.WebApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Controllers
{
    [SessionTimeout]
    public class BomController : Controller
    {
        private readonly IBomService _bomService;
        private readonly IExtensionService _extensionService;

        public BomController(IBomService bomService, IExtensionService extensionService)
        {
            _extensionService = extensionService;
            _bomService = bomService;
        }
        [SessionTimeout]
        public IActionResult Index()
        {
            BOMViewModel model = new BOMViewModel();
            model.lstMasterData = new List<MasterData>();
            model.lstBomStructs = new List<BomStruct>();
            model.plants = _bomService.GetAllPlant();
            //model.GroupCompany = _bomService.GetAllGroupCompany();
            return View(model);
        }
        [SessionTimeout]
        [HttpPost]
        public JsonResult SearchMatBom(string mat, string app, string prod)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                BOMViewModel model = new BOMViewModel();
                if (!string.IsNullOrEmpty(mat))
                {
                    model.MaterialNo = mat;
                }
                if (!string.IsNullOrEmpty(app))
                {
                    model.AbbservName = app;
                }
                if (!string.IsNullOrEmpty(prod))
                {
                    model.ProductCode = prod;
                }
                var result = _bomService.GetMatChild(model);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(RenderView.RenderRazorViewToString(this, "_DataTableBomParentSearch", result));
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json(RenderView.RenderRazorViewToString(this, "_DataTableBomParentSearch", null));
            }

        }
        [SessionTimeout]
        [HttpPost]
        public JsonResult SearchBomStruct(string txtSearch, string ddlSearch)
        {
            BOMViewModel bomViewModel = new BOMViewModel();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _bomService.GetMatParent(ref bomViewModel, txtSearch, ddlSearch);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(RenderView.RenderRazorViewToString(this, "_DataTableBomParent", bomViewModel));///PartialView("_DataTableBom", bomViewModel);
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json(false);
            }

        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult SearchBomStructChild(string Mat, string txtSearch, string ddlSearch)
        {
            BOMViewModel bomViewModelChild = new BOMViewModel();
            BOMViewModel bomViewModelParent = new BOMViewModel();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _bomService.GetMatParent(ref bomViewModelParent, txtSearch, ddlSearch);
                bomViewModelParent.masterData = bomViewModelParent.lstMasterData.Where(x => x.MaterialNo == Mat).FirstOrDefault();
                _bomService.GetParentChildByMat(ref bomViewModelChild, Mat);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(new { parentmaterialno = bomViewModelParent.masterData.MaterialNo, viewchild = RenderView.RenderRazorViewToString(this, "_DataTableBom", bomViewModelChild), viewparent = RenderView.RenderRazorViewToString(this, "_DataTableSelectParent", bomViewModelParent) });///PartialView("_DataTableBom", bomViewModel);
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json(false);
            }

        }

        //[HttpPost]
        //public PartialViewResult GetBomStructList(string txtSearch, string ddlSearch)
        //{
        //    BOMViewModel model = new BOMViewModel();

        //    model.lstBomStructs = BomService.GetBomStruct(context, txtSearch, ddlSearch).lstBomStructs;

        //    return PartialView("_DataTableBom", model);
        //}
        [SessionTimeout]
        [HttpPost]
        public JsonResult CheckDupBomStruct(string Mat, string array)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                BOMViewModel tmp = new BOMViewModel();

                BOMViewModel bomViewModel = new BOMViewModel();
                //  _bomService.GetMatParent(ref bomViewModel, txtSearch, ddlSearch);
                _bomService.GetParentChildByMat(ref bomViewModel, Mat);

                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(true);
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json(false);
            }
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult CheckBeforeAddBomStruct(BOMViewModel model)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var modelToReturn = _bomService.GetBomStruct(model.ParentMaterialNo);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(modelToReturn); //PartialView("_DataTableBom", modelToReturn);
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json(null);//PartialView("_DataTableBom", null);
            }
        }


        [SessionTimeout]
        [HttpPost]
        public JsonResult AddBomStruct(BOMViewModel model)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _bomService.SaveBomStruct(model);
                var modelToReturn = _bomService.GetBomStruct(model.ParentMaterialNo);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(RenderView.RenderRazorViewToString(this, "_DataTableBom", modelToReturn)); //PartialView("_DataTableBom", modelToReturn);
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json(RenderView.RenderRazorViewToString(this, "_DataTableBom", null));//PartialView("_DataTableBom", null);
            }
        }
        [SessionTimeout]
        [HttpPut]
        public JsonResult DeleteBomStruct(int Id)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var Mat = _bomService.DeleteBomStruct(Id);
                var modelToReturn = _bomService.GetBomStruct(Mat);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(RenderView.RenderRazorViewToString(this, "_DataTableBom", modelToReturn));  // PartialView("_DataTableBom", modelToReturn);             
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json(RenderView.RenderRazorViewToString(this, "_DataTableBom", null));//PartialView("_DataTableBom", null);
            }

        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult EditBomStruct(string bomStructStr)
        {
            var isSuccess = false;
            var exceptionMessage = string.Empty;
            try
            {
                var bomStructModel = JsonConvert.DeserializeObject<BomStruct>(bomStructStr);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _bomService.EditBOMStruct(bomStructModel);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                isSuccess = true;
                return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage });
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage });
            }

        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult AddBomStructNew(string parentmat, string array, string mat, string app, string prod)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

                string listDup = "";
                BOMViewModel modelGetMatChild = new BOMViewModel();
                if (!string.IsNullOrEmpty(mat))
                {
                    modelGetMatChild.MaterialNo = mat;
                }
                if (!string.IsNullOrEmpty(app))
                {
                    modelGetMatChild.AbbservName = app;
                }
                if (!string.IsNullOrEmpty(prod))
                {
                    modelGetMatChild.ProductCode = prod;
                }
                var result = _bomService.GetMatChild(modelGetMatChild);
                BOMViewModel model = new BOMViewModel();
                string[] res = JsonConvert.DeserializeObject<string[]>(array);
                BOMViewModel modeltemp = new BOMViewModel();
                _bomService.GetParentChildByMat(ref modeltemp, parentmat);
                foreach (string item in res)
                {
                    var checkdup = modeltemp.lstBomStructs.Where(x => x.Follower == item).FirstOrDefault();
                    if (checkdup == null)
                    {
                        var tmpgetdata = result.lstMasterData.Where(z => z.MaterialNo == item).FirstOrDefault();
                        model.ParentMaterialNo = parentmat;
                        model.Weight = tmpgetdata.WeightBox == null ? "0" : tmpgetdata.WeightBox.ToString();
                        model.MaterialNo = tmpgetdata.MaterialNo;
                        model.Pieces = tmpgetdata.PieceSet.ToString();
                        //save
                        _bomService.SaveBomStruct(model);
                    }
                    else
                    {
                        listDup = listDup + "," + item;
                    }
                }



                string lastlistDup = listDup == "" ? "" : listDup.Substring(1, listDup.Length - 1);
                var modelToReturn = _bomService.GetBomStruct(parentmat);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(new { view = RenderView.RenderRazorViewToString(this, "_DataTableBom", modelToReturn), datadup = lastlistDup }); //PartialView("_DataTableBom", modelToReturn);
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json(RenderView.RenderRazorViewToString(this, "_DataTableBom", null));//PartialView("_DataTableBom", null);
            }
        }

        //copy bom to new plant
        [SessionTimeout]
        [HttpPost]
        public JsonResult CopyBomToNewPlant(string parentmat, string plant)
        {
            try
            {
                var data = _bomService.GetMasterdataByMaterial(parentmat, plant);
                if (data != null)
                {
                    _bomService.CopyMatNewPlant(parentmat, plant);
                }
                else
                {
                    return Json("NotMatch");
                }


                return Json("Success");
            }
            catch (Exception ex)
            {
                return Json("Fail");
            }
        }


        [SessionTimeout]
        [HttpPost]
        public JsonResult ResentBOM(string materialNo)
        {
            bool isSuccess = false;
            string exceptionMessage = string.Empty;

            //call repository
            try
            {
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _bomService.ResentBOM(materialNo);
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
