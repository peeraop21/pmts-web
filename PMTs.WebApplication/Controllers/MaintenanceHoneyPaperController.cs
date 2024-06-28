using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PMTs.DataAccess.Models;
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
    public class MaintenanceHoneyPaperController : Controller
    {
        private readonly IMaintenanceHoneyPaperService maintenanceHoneyPaperService;

        public MaintenanceHoneyPaperController(IMaintenanceHoneyPaperService maintenanceHoneyPaperService)
        {
            this.maintenanceHoneyPaperService = maintenanceHoneyPaperService;
        }

        [SessionTimeout]
        public IActionResult Index()
        {
            var honeyPapers = new List<HoneyPaper>();

            try
            {
                honeyPapers = maintenanceHoneyPaperService.GetAllHoneypaper();
            }
            catch (Exception ex)
            {

            }

            return View(honeyPapers);
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult CreateUpdateHoneyPaper(string honeyPaperObject, string flag)
        {
            var isSuccess = false;
            var exceptionMessage = "";
            var honeyPaper = new HoneyPaper();

            try
            {
                honeyPaper = JsonConvert.DeserializeObject<HoneyPaper>(honeyPaperObject, new IsoDateTimeConverter { DateTimeFormat = "yyyy-dd-MMTHH:mm:ss" });

                if (honeyPaper != null)
                {
                    if (flag == "Create")
                    {
                        maintenanceHoneyPaperService.CreateHoneypaper(honeyPaper);
                    }
                    else if (flag == "Edit")
                    {
                        maintenanceHoneyPaperService.UpdateHoneypaper(honeyPaper);
                    }

                    isSuccess = true;
                }
                else
                {

                }

            }
            catch (Exception ex)
            {
                exceptionMessage = ex.Message;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage });
        }
    }
}