using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using PMTs.DataAccess;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView.MaintenanceProductType;
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
    [SessionTimeout]
    public class MaintenanceCategoriesController : Controller
    {
        private readonly IMaintenanceCategoriesService _maintenanceProductTypeService;
        private readonly INewProductService _newProductService;
        private readonly IExtensionService _extensionService;

        public MaintenanceCategoriesController(IMaintenanceCategoriesService maintenanceProductTypeService, INewProductService newProductService, IExtensionService extensionService)
        {
            _maintenanceProductTypeService = maintenanceProductTypeService;
            _newProductService = newProductService;
            _extensionService = extensionService;
        }

        #region Matching Categories
        [SessionTimeout]
        public IActionResult Index()
        {
            MaintenanceKindOfProductGroupCreateModel model = new MaintenanceKindOfProductGroupCreateModel();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceProductTypeService.GetMaintenanceCategoriesData(this, ref model);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                //info log
            }
            catch (Exception ex)
            {
                //Error log
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }

            return View(model);
        }
        [SessionTimeout]
        [HttpPost]
        public JsonResult CreateHierarchyLv2Matrix(string KindOfProductID, string ProcessCostID, string KindOfProductGroupID, string ProductTypeArr)
        {
            bool isSuccess = false;
            string message = "";

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceProductTypeService.SaveKindOfProductGroup(KindOfProductID, ProcessCostID, KindOfProductGroupID, ProductTypeArr);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                //info log
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                //log error
                message = "Can't match product type! Please tyy again...";
                isSuccess = false;
            }

            return Json(new { IsSuccess = isSuccess, ExeptionMessage = message });
        }

        //[HttpPost]
        //public PartialViewResult CreateHierarchyLv2Matrix(string KindOfProductID, string ProcessCostID, string KindOfProductGroupID, string ProductTypeArr)
        //{
        //    MaintenanceKindOfProductGroupCreateModel maintenanceKindOfProductGroupCreateModel = new MaintenanceKindOfProductGroupCreateModel();
        //    maintenanceKindOfProductGroupCreateModel.ProductTypes = new List<ProductTypeModel>();

        //    try
        //    {
        //        _maintenanceProductTypeService.SaveKindOfProductGroup(KindOfProductID, ProcessCostID, KindOfProductGroupID, ProductTypeArr);

        //        //info log
        //    }
        //    catch (Exception ex)
        //    {
        //        //log error
        //        throw ex;
        //    }

        //    return PartialView("_ProductTypeTable", maintenanceKindOfProductGroupCreateModel);
        //}

        #endregion

        #region KindOfProduct
        [SessionTimeout]
        public IActionResult MaintenanceKindOfProduct()
        {
            List<KindOfProduct> KindOfProducts = new List<KindOfProduct>();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceProductTypeService.GetKindOfProducts(ref KindOfProducts);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return View(KindOfProducts);
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult CreateKindOfProduct(MaintenanceProductTypeCreateModel productTypeCreateModel)
        {
            bool isSuccess;
            string exeptionMessage = string.Empty;

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                //do something
                _maintenanceProductTypeService.CreateKindOfProduct(productTypeCreateModel);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                exeptionMessage = ex.Message;

                isSuccess = false;
            }

            return Json(new { IsSuccess = isSuccess, ExeptionMessage = exeptionMessage, ActionType = "create" });
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult EditKindOfProduct(string req)
        {
            bool isSuccess;
            string exeptionMessage = string.Empty;
            var productTypeCreateModel = new MaintenanceProductTypeCreateModel();

            try
            {
                productTypeCreateModel = JsonConvert.DeserializeObject<MaintenanceProductTypeCreateModel>(req);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                //do something
                _maintenanceProductTypeService.EditKindOfProduct(productTypeCreateModel);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                exeptionMessage = "Can't update Kind of Product! Please try again...";

                isSuccess = false;
            }

            return Json(new { IsSuccess = isSuccess, ExeptionMessage = exeptionMessage, ActionType = "edit" });
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult DeleteKindOfProduct(int id)
        {
            bool isSuccess;
            string exeptionMessage = string.Empty;

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                //do something
                _maintenanceProductTypeService.DeleteKindOfProduct(id);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                exeptionMessage = $"Can't delete Kind of Product UID : {id}! Please try again...";

                isSuccess = false;
            }

            return Json(new { IsSuccess = isSuccess, ExeptionMessage = exeptionMessage, ActionType = "delete" });
        }

        #endregion

        #region ProcessCost
        [SessionTimeout]
        public IActionResult MaintenanceProcessCost()
        {
            List<ProcessCost> ProcessCosts = new List<ProcessCost>();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceProductTypeService.GetProcessCosts(ref ProcessCosts);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return View(ProcessCosts);
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult CreateProcessCost(MaintenanceProductTypeCreateModel productTypeCreateModel)
        {
            bool isSuccess;
            string exeptionMessage = string.Empty;

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                //do something
                _maintenanceProductTypeService.CreateProcessCost(productTypeCreateModel);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                exeptionMessage = ex.Message;

                isSuccess = false;
            }

            return Json(new { IsSuccess = isSuccess, ExeptionMessage = exeptionMessage, ActionType = "create" });
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult EditProcessCost(string req)
        {
            bool isSuccess;
            string exeptionMessage = string.Empty;
            var productTypeCreateModel = new MaintenanceProductTypeCreateModel();

            try
            {
                productTypeCreateModel = JsonConvert.DeserializeObject<MaintenanceProductTypeCreateModel>(req);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                //do something
                _maintenanceProductTypeService.EditProcessCost(productTypeCreateModel);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                exeptionMessage = "Can't update Process Cost! Please try again...";

                isSuccess = false;
            }

            return Json(new { IsSuccess = isSuccess, ExeptionMessage = exeptionMessage, ActionType = "edit" });
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult DeleteProcessCost(int id)
        {
            bool isSuccess;
            string exeptionMessage = string.Empty;

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                //do something
                _maintenanceProductTypeService.DeleteProcessCost(id);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                exeptionMessage = $"Can't delete Process Cost UID : {id}! Please try again...";

                isSuccess = false;
            }

            return Json(new { IsSuccess = isSuccess, ExeptionMessage = exeptionMessage, ActionType = "delete" });
        }

        #endregion

        #region ProductType
        [SessionTimeout]
        public IActionResult MaintenanceProductType()
        {
            MaintenanceKindOfProductGroupCreateModel kindOfProductGroupCreateModel = new MaintenanceKindOfProductGroupCreateModel();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceProductTypeService.GetProductTypes(ref kindOfProductGroupCreateModel);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return View(kindOfProductGroupCreateModel);
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult CreateProductType(MaintenanceProductTypeCreateModel productTypeCreateModel)
        {
            bool isSuccess;
            string exeptionMessage = string.Empty;

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                //do something
                _maintenanceProductTypeService.CreateProductType(productTypeCreateModel);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                exeptionMessage = ex.Message;

                isSuccess = false;
            }

            return Json(new { IsSuccess = isSuccess, ExeptionMessage = exeptionMessage, ActionType = "create" });
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult EditProductType(string req)
        {
            bool isSuccess;
            string exeptionMessage = string.Empty;
            MaintenanceProductTypeCreateModel productTypeCreateModel = new MaintenanceProductTypeCreateModel();

            try
            {
                productTypeCreateModel = JsonConvert.DeserializeObject<MaintenanceProductTypeCreateModel>(req);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                //do something
                _maintenanceProductTypeService.EditProductType(productTypeCreateModel);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                exeptionMessage = "Can't update Product Type! Please try again...";

                isSuccess = false;
            }

            return Json(new { IsSuccess = isSuccess, ExeptionMessage = exeptionMessage, ActionType = "edit" });
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult DeleteProductType(int id)
        {
            bool isSuccess;
            string exeptionMessage = string.Empty;

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                //do something
                _maintenanceProductTypeService.DeleteProductType(id);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                exeptionMessage = $"Can't delete Product Type UID : {id}! Please try again...";

                isSuccess = false;
            }

            return Json(new { IsSuccess = isSuccess, ExeptionMessage = exeptionMessage, ActionType = "delete" });
        }

        #endregion

        #region KindOfProductGroup
        [SessionTimeout]
        public IActionResult MaintenanceKindOfProductGroup()
        {
            List<KindOfProductGroup> KindOfProductGroups = new List<KindOfProductGroup>();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceProductTypeService.GetKindOfProductGroups(ref KindOfProductGroups);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return View(KindOfProductGroups);
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult CreateKindOfProductGroup(MaintenanceProductTypeCreateModel productTypeCreateModel)
        {
            bool isSuccess;
            string exeptionMessage = string.Empty;

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                //do something
                _maintenanceProductTypeService.CreateKindOfProductGroup(productTypeCreateModel);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                if (ex.ToString().Contains("The duplicate key value"))
                {
                    exeptionMessage = $"The UID : {productTypeCreateModel.UID} is a duplicate value";
                }
                else
                {
                    exeptionMessage = "Can't create new Kind of Product Group! Please try again...";
                }
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                isSuccess = false;
            }

            return Json(new { IsSuccess = isSuccess, ExeptionMessage = exeptionMessage, ActionType = "create" });
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult EditKindOfProductGroup(string req)
        {
            bool isSuccess;
            string exeptionMessage = string.Empty;
            MaintenanceProductTypeCreateModel productTypeCreateModel = new MaintenanceProductTypeCreateModel();

            try
            {
                productTypeCreateModel = JsonConvert.DeserializeObject<MaintenanceProductTypeCreateModel>(req);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                //do something
                _maintenanceProductTypeService.EditKindOfProductGroup(productTypeCreateModel);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                //exeptionMessage = ex.Message;
                exeptionMessage = "Can't update Kind of Product Group! Please try again...";

                isSuccess = false;
            }

            return Json(new { IsSuccess = isSuccess, ExeptionMessage = exeptionMessage, ActionType = "edit" });
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult DeleteKindOfProductGroup(int id)
        {
            bool isSuccess;
            string exeptionMessage = string.Empty;

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                //do something
                _maintenanceProductTypeService.DeleteKindOfProductGroup(id);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                exeptionMessage = $"Can't delete Kind of Product Group UID : {id}! Please try again...";

                isSuccess = false;
            }
            return Json(new { IsSuccess = isSuccess, ExeptionMessage = exeptionMessage, ActionType = "delete" });
        }

        #endregion

        #region Function

        #endregion

    }
}