using AutoMapper.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PMTs.DataAccess;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.Repository;
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
    public class ProductInfoController : Controller
    {
        private readonly IHostingEnvironment _environment;
        private readonly IMasterDataService _masterDataService;
        private readonly IExtensionService _extensionService;


        public ProductInfoController(IMasterDataService masterDataService, IExtensionService extensionService)
        {
            _masterDataService = masterDataService;
            _extensionService = extensionService;
        }


        //public IActionResult Index()
        //{

        //    VMasterDataViewModel model = new VMasterDataViewModel();

        //    try
        //    {
        //        SessionsModel sessions = new SessionsModel
        //        {
        //            UserName = HttpContext.Session.GetString("username"),
        //            SaleOrg = HttpContext.Session.GetString("SALE_ORG"),
        //            PlantCode = HttpContext.Session.GetString("PLANT_CODE")
        //        };

        //        model.ModelList = _masterDataService.GetMasterDataListView().ModelList;

        //    }
        //    catch (Exception ex)
        //    {
        //       // return Json(false);
        //    }

        //    return View(model);

        //}
        //public IActionResult SeachProduct(string TxtSearch, string ddlSearch)
        //{
        //    VMasterDataViewModel model = new VMasterDataViewModel();

        //    try
        //    {
        //        SessionsModel sessions = new SessionsModel
        //        {
        //            UserName = HttpContext.Session.GetString("username"),
        //            SaleOrg = HttpContext.Session.GetString("SALE_ORG"),
        //            PlantCode = HttpContext.Session.GetString("PLANT_CODE")
        //        };

        //        model.ModelList = _masterDataService.GetMasterDataListViewSearch(TxtSearch, ddlSearch).ModelList;

        //    }
        //    catch (Exception ex)
        //    {
        //        // return Json(false);
        //    }

        //    return View("index",model);
        //}




    }
}