using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PMTs.DataAccess;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView.MaintenanceFlute;
using PMTs.DataAccess.Repository;
using PMTs.Logs.Logger;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Controllers
{
    [SessionTimeout]
    public class MaintenanceFluteController : Controller
    {
        private readonly IExtensionService _extensionService;
        private readonly IMaintenanceFluteService _maintenanceFluteService;
        //private readonly IExtensionService _extensionService;

        public MaintenanceFluteController(IExtensionService extensionService, IMaintenanceFluteService maintenanceFluteService)
        {
            // Initialize Service
            _extensionService = extensionService;
            _maintenanceFluteService = maintenanceFluteService;
        }

        [SessionTimeout]
        public IActionResult Index()
        {
            MaintenanceFluteModel model = new MaintenanceFluteModel();

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                model = _maintenanceFluteService.GetFlute();
                SessionExtentions.SetSession(HttpContext.Session, "MaintainFluteSession", model);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }

            return View(model);
        }

        [SessionTimeout]
        public PartialViewResult _CreateModal()
        {
            MaintenanceFluteModel model = new MaintenanceFluteModel();
            return PartialView(model);
        }

        [SessionTimeout]
        public PartialViewResult _fluteTrView()
        {
            MaintenanceFluteModel model = new MaintenanceFluteModel();
            return PartialView(model);
        }

        [SessionTimeout]
        public JsonResult OpenAddModal()
        {
            MaintenanceFluteModel model = new MaintenanceFluteModel();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }
            return Json(RenderView.RenderRazorViewToString(this, "_CreateModal", model));
        }

        [SessionTimeout]
        public JsonResult OpenViewModal(string flute)
        {
            MaintenanceFluteModel model = new MaintenanceFluteModel();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var ModelSession = SessionExtentions.GetSession<MaintenanceFluteModel>(HttpContext.Session, "MaintainFluteSession");
                model.Flute = ModelSession.Flutes.Where(x => x.Flute1 == flute).FirstOrDefault();
                model.FluteTrs = ModelSession.FluteTrs.Where(x => x.FluteCode == flute).ToList();
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }
            return Json(new { view = RenderView.RenderRazorViewToString(this, "_CreateModal", model), data = model.Flute });
        }

        [SessionTimeout]
        public JsonResult OpenEditModal(string flute)
        {
            MaintenanceFluteModel model = new MaintenanceFluteModel();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var ModelSession = SessionExtentions.GetSession<MaintenanceFluteModel>(HttpContext.Session, "MaintainFluteSession");
                model.Flute = ModelSession.Flutes.Where(x => x.Flute1 == flute).FirstOrDefault();
                model.FluteTrs = ModelSession.FluteTrs.Where(x => x.FluteCode == flute).ToList();
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }
            return Json(new { view = RenderView.RenderRazorViewToString(this, "_CreateModal", model), data = model.Flute });
        }

        //public JsonResult GetFluteModal(string flute)
        //{
        //    MaintenanceFluteModel model = new MaintenanceFluteModel();
        //    try
        //    {
        //        Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
        //        var ModelSession = SessionExtentions.GetSession<MaintenanceFluteModel>(HttpContext.Session, "MaintainFluteSession");
        //        model.Flute = ModelSession.Flutes.Where(x => x.Flute1 == flute).FirstOrDefault();
        //        // model.FluteTrs = ModelSession.FluteTrs.Where(x => x.FluteCode == flute).ToList();
        //        Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
        //        throw ex;
        //    }
        //    return Json(model.Flute);
        //}

        [SessionTimeout]
        [HttpPost]
        public JsonResult ManageData(string req, string arrfluteTr, string flag)
        {
            Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
            MaintenanceFluteModel model = new MaintenanceFluteModel();
            try
            {
                model.Flute = JsonConvert.DeserializeObject<Flute>(req);
                if (arrfluteTr.Length > 2)
                {
                    model.FluteTrs = JsonConvert.DeserializeObject<List<FluteTr>>(arrfluteTr);
                }

                if (flag == "Add")
                {
                    _maintenanceFluteService.AddFlute(model);
                }
                else
                {
                    _maintenanceFluteService.UpdateFlute(model);
                }
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return Json("success");

        }

        [SessionTimeout]
        public JsonResult CheckDuplicateFlute(string Flute)
        {
            var modelsession = SessionExtentions.GetSession<MaintenanceFluteModel>(HttpContext.Session, "MaintainFluteSession");
            string checkDup = "0";
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var ck = modelsession.Flutes.Where(x => x.Flute1 == Flute).FirstOrDefault();
                if (ck != null)
                {
                    checkDup = "1";
                }
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch
            {
                checkDup = "0";
            }
            return Json(new { dup = checkDup });
        }

        [SessionTimeout]
        public JsonResult GetFluteAfterManageData()
        {
            MaintenanceFluteModel model = new MaintenanceFluteModel();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                model = _maintenanceFluteService.GetFlute();
                SessionExtentions.SetSession(HttpContext.Session, "MaintainFluteSession", model);
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return Json(RenderView.RenderRazorViewToString(this, "_FluteTable", model));
        }

    }
}