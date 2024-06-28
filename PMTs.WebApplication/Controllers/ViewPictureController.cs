using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PMTs.DataAccess.ModelView.NewProduct;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.Logs.Logger;
using PMTs.WebApplication.Services;
using PMTs.WebApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Controllers
{
    public class ViewPictureController : Controller
    {
        IMasterDataAPIRepository masterDataAPIRepository;
        public ViewPictureController(IMasterDataAPIRepository masterDataAPIRepository)
        {
            this.masterDataAPIRepository = masterDataAPIRepository;
        }

        //private readonly IProductPicService _productPicService;
        //private readonly IHostingEnvironment _environment;

        public IActionResult Index(string param)
        {
            // param
            string[] parameter = param.Split(',').ToArray();
            string factoryCode = parameter[0];
            string materialNo = parameter[1];
            //string URL = "http://10.28.58.90/pmtsapi/api/MasterData/GetMasterDataByMaterialNo?AppName=W2pJlQL8hpY=&FactoryCode=" + FactoryCode + "&MaterialNo=" + MaterialNo + "";
            string URL = "https://localhost:44360/api/MasterData/GetMasterDataByMaterialNo?AppName=W2pJlQL8hpY=&FactoryCode=" + factoryCode + "&MaterialNo=" + materialNo + "";

            ViewPictureService _productPicService = new ViewPictureService(masterDataAPIRepository);
            TransactionDataModel transactionDataModel = new TransactionDataModel();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                transactionDataModel = _productPicService.GetPicture(materialNo, factoryCode);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", factoryCode, this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return RedirectToAction("ErrorPage");
                // Redirect To Error Page
            }

            return View(transactionDataModel);
            //  return View();
        }

        public IActionResult ErrorPage()
        {
            return View();
        }
    }
}