using Microsoft.AspNetCore.Mvc;
using PMTs.DataAccess.ComplexModel;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView;
using PMTs.Logs.Logger;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Controllers
{
    public class ChangePalletSizeController : Controller
    {
        private readonly IMasterDataService _masterDataService;
        public ChangePalletSizeController(IMasterDataService masterDataService)
        {
            _masterDataService = masterDataService;
        }

        public IActionResult Index()
        {
            var changePalletSize = new ChangePalletSizeViewModel();
            _masterDataService.GetChangePalletSizeData(ref changePalletSize, string.Empty);
            //get masterdata as change pallet size

            return View(changePalletSize);
        }

        [SessionTimeout]
        [HttpPost]
        public IActionResult SearchChangePalletSize(string materialNO)
        {
            bool isSuccess;
            bool existMasterData = false;
            string exceptionMessage = string.Empty;
            var changePalletSize = new ChangePalletSizeViewModel();
            changePalletSize.MasterData = new MasterData();
            changePalletSize.MasterDatas = new List<MasterData>();
            changePalletSize.StandardPatternNames = new List<StandardPatternName>();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _masterDataService.GetChangePalletSizeData(ref changePalletSize, materialNO);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                exceptionMessage = ex.Message;
                isSuccess = false;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_ChangePalletSizeTable", changePalletSize) });
        }


        [SessionTimeout]
        [HttpPost]
        public JsonResult ChangePalletSize(MasterData masterData)
        {
            bool isSuccess;
            bool existMasterData = false;
            string exceptionMessage = string.Empty;
            var changePalletSize = new ChangePalletSizeViewModel();
            changePalletSize.MasterData = new MasterData();
            changePalletSize.MasterDatas = new List<MasterData>();
            changePalletSize.StandardPatternNames = new List<StandardPatternName>();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _masterDataService.UpdateMasterDataByChangePalletSize(masterData);
                _masterDataService.GetChangePalletSizeData(ref changePalletSize, masterData.MaterialNo);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                exceptionMessage = ex.Message;
                isSuccess = false;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_ChangePalletSizeTable", changePalletSize) });
        }

    }
}
