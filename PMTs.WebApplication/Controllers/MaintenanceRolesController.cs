using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PMTs.DataAccess.ModelView.MaintenanceRoles;
using PMTs.Logs.Logger;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Controllers
{
    public class MaintenanceRolesController : Controller
    {

        private readonly IMaintenanceRolesService _maintenanceRolesService;
        private readonly IExtensionService _extensionService;




        public MaintenanceRolesController(IMaintenanceRolesService maintenanceRolesService, IExtensionService extensionService)
        {
            _maintenanceRolesService = maintenanceRolesService;
            _extensionService = extensionService;

        }
        #region MaintenanceRoles

        [SessionTimeout]
        public IActionResult Index()
        {

            MaintenanceRolesViewModel maintenanceRolesViewModel = new MaintenanceRolesViewModel();


            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceRolesService.GetRoles(maintenanceRolesViewModel);
                //_maintenanceRolesService.GetMainMenu(maintenanceRolesViewModel,0);
                //_maintenanceRolesService.GetSubMenu(maintenanceRolesViewModel, 0,0);


                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                throw ex;
            }

            return View(maintenanceRolesViewModel);
        }
        [SessionTimeout]
        [HttpPost]
        public JsonResult SaveRole(MaintenanceRolesViewModel maintenanceRolesViewModel)
        {

            bool isSuccess;
            string exceptionMessage = string.Empty;

            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceRolesService.SaveRoles(maintenanceRolesViewModel);
                _maintenanceRolesService.GetRoles(maintenanceRolesViewModel);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                exceptionMessage = ex.Message;
                isSuccess = false;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_MasterRolesTable", maintenanceRolesViewModel) });

        }
        [SessionTimeout]
        [HttpPost]
        public JsonResult UpdateRole(string req)
        {

            bool isSuccess;
            string exceptionMessage = string.Empty;
            MasterRoleViewModel masterRoleViewModel = new MasterRoleViewModel();
            masterRoleViewModel = JsonConvert.DeserializeObject<MasterRoleViewModel>(req);
            MaintenanceRolesViewModel maintenanceRolesViewModel = new MaintenanceRolesViewModel();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                _maintenanceRolesService.UpdateRoles(masterRoleViewModel);
                _maintenanceRolesService.GetRoles(maintenanceRolesViewModel);
                isSuccess = true;
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                exceptionMessage = ex.Message;
                isSuccess = false;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_MasterRolesTable", maintenanceRolesViewModel) });




        }



        #endregion

        #region Mainmenu
        [HttpGet]
        public JsonResult GetMainMenuData(int RoleId)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                //var model = SessionExtentions.GetSession<TransactionDataModel>(HttpContext.Session, "TransactionDataModel");
                //model.modelRouting.RoutingDataList.Clear();
                //model.modelRouting.RoutingDataList = _routingService.GetRoutingList(model.MaterialNo).Where(x => x.SeqNo == seqNo).ToList();
                MaintenanceRolesViewModel maintenanceRolesViewModel = new MaintenanceRolesViewModel();
                //maintenanceRolesViewModel.MasterRoleViewModel.RoleId = RoleId;
                _maintenanceRolesService.GetMainMenu(maintenanceRolesViewModel, RoleId);

                // var RoutingData = model.modelRouting.RoutingDataList.Where(w => w.SeqNo == seqNo).ToList();
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                //return Json(model.modelRouting.RoutingDataList);
                // return Json(maintenanceRolesViewModel);
                return Json(new { View = RenderView.RenderRazorViewToString(this, "_ConfigMenuByRole", maintenanceRolesViewModel) });

            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json("");
            }
        }
        #endregion
        #region Submenu
        [HttpGet]
        public JsonResult GetSubMenuData(int RoleId, int menuid)
        {
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");
                //var model = SessionExtentions.GetSession<TransactionDataModel>(HttpContext.Session, "TransactionDataModel");
                //model.modelRouting.RoutingDataList.Clear();
                //model.modelRouting.RoutingDataList = _routingService.GetRoutingList(model.MaterialNo).Where(x => x.SeqNo == seqNo).ToList();
                MaintenanceRolesViewModel maintenanceRolesViewModel = new MaintenanceRolesViewModel();
                //maintenanceRolesViewModel.MasterRoleViewModel.RoleId = RoleId;
                _maintenanceRolesService.GetSubMenu(maintenanceRolesViewModel, RoleId, menuid);

                // var RoutingData = model.modelRouting.RoutingDataList.Where(w => w.SeqNo == seqNo).ToList();
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
                //return Json(model.modelRouting.RoutingDataList);
                // return Json(maintenanceRolesViewModel);
                return Json(new { View = RenderView.RenderRazorViewToString(this, "_ConfigSubMenuByRole", maintenanceRolesViewModel) });

            }
            catch (Exception ex)
            {
                Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return Json("");
            }
        }
        #endregion

        //Update and SaveMenuByRoles
        [SessionTimeout]
        [HttpGet]
        public JsonResult SaveMenuByRoles(int roleId, int menuid, bool chkmenu, int idmenurole)
        {

            bool isSuccess;
            string exceptionMessage = string.Empty;
            //MasterRoleViewModel masterRoleViewModel = new MasterRoleViewModel();
            //masterRoleViewModel = JsonConvert.DeserializeObject<MasterRoleViewModel>(req);
            //MaintenanceRolesViewModel maintenanceRolesViewModel = new MaintenanceRolesViewModel();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

                //Get Menu BY Roles
                if (chkmenu == true)
                {
                    MenuRoleViewModel menurole = new MenuRoleViewModel();
                    menurole.IdMenu = menuid;
                    menurole.IdRole = roleId;
                    //Add
                    _maintenanceRolesService.AddMenuByRoles(menurole);
                }
                else
                {
                    //Delete
                    MenuRoleViewModel menurole = new MenuRoleViewModel();
                    menurole.IdMenu = menuid;
                    menurole.IdRole = roleId;
                    menurole.Id = idmenurole;

                    _maintenanceRolesService.DeleteMenuByRoles(idmenurole);

                }
                //_maintenanceRolesService.UpdateRoles(masterRoleViewModel);
                //_maintenanceRolesService.GetRoles(maintenanceRolesViewModel);
                isSuccess = true;
                //Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                //Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                //exceptionMessage = ex.Message;
                isSuccess = false;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_MasterRolesTable", "") });




        }


        public JsonResult SavesubMenuByRoles(int idrole, int menuid, bool chkmenu, int mainMenuId, int subMenuroleID, int idsubmenuRole)
        {
            //idrole = MasterRole.Id
            //menuid = SubMenus.Id
            //mainMenuId = MainMenus.Id
            //subMenuroleID = SubMenurole.id
            bool isSuccess;
            string exceptionMessage = string.Empty;
            //MasterRoleViewModel masterRoleViewModel = new MasterRoleViewModel();
            //masterRoleViewModel = JsonConvert.DeserializeObject<MasterRoleViewModel>(req);
            //MaintenanceRolesViewModel maintenanceRolesViewModel = new MaintenanceRolesViewModel();
            try
            {
                Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "Start");

                //Get Menu BY Roles
                if (chkmenu == true)
                {
                    SubMenuRoleViewModel submenurole = new SubMenuRoleViewModel();
                    submenurole.IdSubMenuRole = menuid;
                    submenurole.IdRole = idrole;
                    submenurole.IdMenu = mainMenuId;

                    //Add
                    _maintenanceRolesService.AddSubMenuByRoles(submenurole);
                }
                else
                {
                    //Delete
                    SubMenuRoleViewModel submenurole = new SubMenuRoleViewModel();
                    submenurole.IdSubMenuRole = menuid;
                    submenurole.IdRole = idrole;
                    submenurole.IdMenu = mainMenuId;

                    _maintenanceRolesService.DeleteSubMenuByRoles(subMenuroleID);

                }
                //_maintenanceRolesService.UpdateRoles(masterRoleViewModel);
                //_maintenanceRolesService.GetRoles(maintenanceRolesViewModel);
                isSuccess = true;
                //Logger.Info("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                //Logger.Error("PMTs", "", this.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                //exceptionMessage = ex.Message;
                isSuccess = false;
            }

            return Json(new { IsSuccess = isSuccess, ExceptionMessage = exceptionMessage, View = RenderView.RenderRazorViewToString(this, "_MasterRolesTable", "") });
        }

    }
}