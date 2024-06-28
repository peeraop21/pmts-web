using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI.Common;
using Newtonsoft.Json;
using PMTs.DataAccess.ComplexModel;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView.MaintenanceFlute;
using PMTs.DataAccess.ModelView.MaintenanceFluteTrim;
using PMTs.Logs.Logger;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Services;
using PMTs.WebApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PMTs.WebApplication.Controllers
{
    public class MaintenanceFluteTrimController : Controller
    {
        private readonly IMaintenanceFluteTrimService _maintenanceFluteTrimService;
        public MaintenanceFluteTrimController(IMaintenanceFluteTrimService maintenanceFluteTrimService) 
        {
            _maintenanceFluteTrimService = maintenanceFluteTrimService;
        }


        [SessionTimeout]
        public IActionResult Index()
        {
            MaintenanceFluteTrimModel model = new MaintenanceFluteTrimModel();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                model = _maintenanceFluteTrimService.InitalPage();
                SessionExtentions.SetSession(HttpContext.Session, "MaintenanceFluteTrimSession", model);

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
            MaintenanceFluteTrimModel model = new MaintenanceFluteTrimModel();
            return PartialView(model);
        }

        [SessionTimeout]
        public JsonResult OpenAddModal()
        {
            MaintenanceFluteTrimModel model = new MaintenanceFluteTrimModel();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                model = SessionExtentions.GetSession<MaintenanceFluteTrimModel>(HttpContext.Session, "MaintenanceFluteTrimSession");
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
        public JsonResult OpenViewModal(int id)
        {
            MaintenanceFluteTrimModel model = new MaintenanceFluteTrimModel();
            MachineFluteTrim data = new MachineFluteTrim();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                model = SessionExtentions.GetSession<MaintenanceFluteTrimModel>(HttpContext.Session, "MaintenanceFluteTrimSession");
                data = model.MachineFluteTrims.Where(x => x.Id == id).FirstOrDefault();
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }
            return Json(new { view = RenderView.RenderRazorViewToString(this, "_CreateModal", model), data = data });
        }

        [SessionTimeout]
        public JsonResult OpenEditModal(int id)
        {
            MaintenanceFluteTrimModel model = new MaintenanceFluteTrimModel();
            MachineFluteTrim data = new MachineFluteTrim();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                model = SessionExtentions.GetSession<MaintenanceFluteTrimModel>(HttpContext.Session, "MaintenanceFluteTrimSession");
                data = model.MachineFluteTrims.Where(x => x.Id == id).FirstOrDefault();
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }
            return Json(new { view = RenderView.RenderRazorViewToString(this, "_CreateModal", model), data = data });
        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult ManageData(string req, string flag)
        {
            Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
            bool result = false;

            try
            {
                var model = JsonConvert.DeserializeObject<MachineFluteTrim>(req);
                if (flag == "Add")
                {
                    result = _maintenanceFluteTrimService.SaveMachineFluteTrim(model);
                }
                else
                {
                    var modelSession = SessionExtentions.GetSession<MaintenanceFluteTrimModel>(HttpContext.Session, "MaintenanceFluteTrimSession");
                    var oldData = modelSession.MachineFluteTrims.Where(w => w.Id == model.Id).FirstOrDefault();
                    if(oldData != null)
                    {
                        oldData.Machine = model.Machine;
                        oldData.Flute = model.Flute;
                        oldData.Trim = model.Trim;
                        result = _maintenanceFluteTrimService.UpdateMachineFluteTrim(oldData);
                    }
                    else
                    {
                        result = false;
                    }
                }
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return Json(result);

        }

        [SessionTimeout]
        [HttpPost]
        public JsonResult CheckDuplicateMachineFluteTrim(string machine, string flute)
        {
            var modelsession = SessionExtentions.GetSession<MaintenanceFluteTrimModel>(HttpContext.Session, "MaintenanceFluteTrimSession");
            string checkDup = "0";
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                var ck = modelsession.MachineFluteTrims.Where(w => w.Machine == machine && w.Flute == flute).Any();
                if (ck == true)
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



    }
}
