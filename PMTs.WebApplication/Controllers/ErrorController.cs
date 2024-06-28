using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using PMTs.DataAccess.ModelView.NewProduct;
using PMTs.Logs.Logger;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Models;
using PMTs.WebApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PMTs.WebApplication.Controllers
{
    [SessionTimeout]
    public class ErrorController : Controller
    {

        private readonly IProductPicService _productPicService;
        private readonly IHostingEnvironment _environment;

        public ErrorController(IProductPicService productPicService,
            IHostingEnvironment IHostingEnvironment)
        {
            _environment = IHostingEnvironment;
            _productPicService = productPicService;
        }
        [SessionTimeout]
        // GET: /<controller>/
        public IActionResult Index(ErrorViewModel errorViewModel)
        {
            return View(errorViewModel);
        }
        [SessionTimeout]
        public IActionResult ViewPicture(string materialNo)
        {
            TransactionDataModel transactionDataModel = new TransactionDataModel();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _productPicService.GetPicture(ref transactionDataModel, _environment, materialNo);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                // Redirect To Error Page
            }

            return View(transactionDataModel);
        }
    }
}
