using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using PMTs.DataAccess;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.NewProduct;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Controllers
{
    [SessionTimeout]
    public class DetailController : Controller
    {

        private readonly IHostingEnvironment _hostEnvironment;

        public DetailController(IHostingEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }


    }
}