using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PMTs.DataAccess.ModelView.AddTagCustomer;
using PMTs.Logs.Logger;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Controllers
{
    public class AddTagCustomerController : Controller
    {
        private readonly IMasterDataService _masterDataService;
        public AddTagCustomerController(IMasterDataService masterDataService)
        {
            _masterDataService = masterDataService;
        }
        public IActionResult Index(string ddlSearch, string inputSerach)
        {
            MaintainAddTagCustomerModel addTagCustomerModel = new MaintainAddTagCustomerModel();
            //List<AddTagCustomerModel> addTagCustomerModel = new List<AddTagCustomerModel>();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _masterDataService.GetMasterDataAddTag(ref addTagCustomerModel, ddlSearch, inputSerach);

                SessionExtentions.SetSession(HttpContext.Session, "TransactionDataModel", null);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }

            return View(addTagCustomerModel);
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult SaveAddTag(string req)
        {
            bool isSuccess;
            string exeptionMessage = string.Empty;
            AddTagCustomerModel addTagCustomerModel = new AddTagCustomerModel();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                //_productCustomerService.SaveCustomer(ref transactionDataModel, QaSpecArr);

                addTagCustomerModel = JsonConvert.DeserializeObject<AddTagCustomerModel>(req);

                _masterDataService.UpdateTagMaterial(ref addTagCustomerModel);
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
        //[SessionTimeout]
        //public IActionResult Index2(string inputMaterialNo)
        //{
        //    List<AddTagCustomerModel> addTagCustomerModel = new List<AddTagCustomerModel>();

        //    try
        //    {
        //        Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
        //        _masterDataService.GetMasterDataAddTag(ref addTagCustomerModel, inputMaterialNo);
        //        SessionExtentions.SetSession(HttpContext.Session, "TransactionDataModel", null);
        //        Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
        //        throw ex;
        //    }

        //    return View(addTagCustomerModel);
        //}

    }
}
