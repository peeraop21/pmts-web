using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.MaintenancePlantCostField;
using PMTs.Logs.Logger;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Services;
using PMTs.WebApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Controllers
{
    [SessionTimeout]
    public class MaintenancePlantCostFieldController : Controller
    {
        private readonly IMaintenancePlantCostFieldService _maintenancePlantCostFieldService;

        public MaintenancePlantCostFieldController(IMaintenancePlantCostFieldService maintenancePlantCostFieldService)
        {
            this._maintenancePlantCostFieldService = maintenancePlantCostFieldService;
        }
        [SessionTimeout]
        public IActionResult Index()
        {
            var plantCostFields = new List<PlantCostFieldViewModel>();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                //do something
                _maintenancePlantCostFieldService.GetPlantCostField(ref plantCostFields);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }

            return View(plantCostFields);
        }
        [SessionTimeout]
        [HttpPost]
        public JsonResult UpdatePlantCostField(string plantCostFieldArr)
        {
            bool isSuccess = false;
            string message = "";

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                //do something
                _maintenancePlantCostFieldService.UpdatePlantCostField(plantCostFieldArr);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");

                isSuccess = true;
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }

            return Json(new { IsSuccess = isSuccess, ExeptionMessage = message });
        }
    }
}