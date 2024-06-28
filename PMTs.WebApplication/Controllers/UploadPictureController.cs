using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.NewProduct;
using PMTs.Logs.Logger;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Controllers
{
    public class UploadPictureController : Controller
    {
        private readonly IProductPicService _productPicService;
        private readonly IHostingEnvironment _environment;

        public UploadPictureController(IProductPicService productPicService, IHostingEnvironment IHostingEnvironment)
        {
            _productPicService = productPicService;
            _environment = IHostingEnvironment;

        }



        public IActionResult Index()
        {

            var productPicModel = new ProductPictureView();
            return View(productPicModel);
        }


        [SessionTimeout]
        public PartialViewResult ViewListProduct(string TxtSearch)
        {
            var userSessionModel = SessionExtentions.GetSession<UserSessionModel>(HttpContext.Session, "UserSessionModel");

            var materialNo = TxtSearch;
            var factoryCode = userSessionModel.FactoryCode; //"252B";
            var productPicModel = new ProductPictureView();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _productPicService.GetUploadImage(TxtSearch, factoryCode, ref productPicModel);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                // Redirect To Error Page
            }

            return PartialView("_UploadPictureForm", productPicModel);
        }

        [HttpPost]
        public JsonResult SearchMaterial(string txtSearch)
        {
            var userSessionModel = SessionExtentions.GetSession<UserSessionModel>(HttpContext.Session, "UserSessionModel");
            var materialNo = txtSearch;
            var factoryCode = userSessionModel.FactoryCode;//"252B";
            var productPicModel = new ProductPictureView();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                //_bomService.GetMatParent(ref bomViewModel, txtSearch, ddlSearch);
                _productPicService.GetUploadImage(materialNo, factoryCode, ref productPicModel);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                return Json(RenderView.RenderRazorViewToString(this, "_UploadPictureForm", productPicModel));///PartialView("_DataTableBom", bomViewModel);
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json(false);
            }

        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult ProdPicSaveImage(ProductPictureView model, IFormFile Pic_Drawing, IFormFile Pic_Print, IFormFile Pic_Pallet, IFormFile Pic_FG)
        {
            bool isSuccess;
            string exeptionMessage = string.Empty;
            //TransactionDataModel model = new TransactionDataModel();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                //model = SessionExtentions.GetSession<TransactionDataModel>(HttpContext.Session, "TransactionDataModel");
                //update Image
                //   SessionExtentions.SetSession(HttpContext.Session, "TransactionDataModel", model);
                //_productPicService.UpdateData(model, _environment, "ProductPic", Pic_Drawing, Pic_Print, Pic_Pallet, Pic_FG);
                _productPicService.UpdateDataPicture(model, _environment, Pic_Drawing, Pic_Print, Pic_Pallet, Pic_FG);


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


    }
}