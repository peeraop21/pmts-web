using AutoMapper;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.MaintenanceRoles;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Shared;
using PMTs.DataAccess.Tracing;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services
{
    [TraceAspect]
    public class MaintenanceRolesService : IMaintenanceRolesService
    {
        IHttpContextAccessor _httpContextAccessor;


        private readonly IMasterRoleAPIRepository _masterRoleAPIRepository;

        private readonly IMainMenusAPIRepository _mainMenusAPIRepository;
        private readonly ISubMenusAPIRepository _subMenusAPIRepository;
        private readonly IMapper mapper;
        private readonly string _username;
        private readonly string _saleOrg;
        private readonly string _factoryCode;
        private readonly string _token;

        public MaintenanceRolesService(IHttpContextAccessor httpContextAccessor, IMasterRoleAPIRepository masterRoleAPIRepository,
             IMainMenusAPIRepository mainMenusAPIRepository,
            ISubMenusAPIRepository subMenusAPIRepository, IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _masterRoleAPIRepository = masterRoleAPIRepository;
            _mainMenusAPIRepository = mainMenusAPIRepository;
            _subMenusAPIRepository = subMenusAPIRepository;

            var userSessionModel = SessionExtentions.GetSession<UserSessionModel>(_httpContextAccessor.HttpContext.Session, "UserSessionModel");
            if (userSessionModel != null)
            {
                _username = userSessionModel.UserName;
                _saleOrg = userSessionModel.SaleOrg;
                _factoryCode = userSessionModel.FactoryCode;
                _token = userSessionModel.Token;
            }

            this.mapper = mapper;
        }

        #region Roles

        public void GetRoles(MaintenanceRolesViewModel maintenanceMasterRoleViewModel)
        {
            var MasterRolesList = JsonConvert.DeserializeObject<List<MasterRole>>(_masterRoleAPIRepository.GetMasterRoleList(_factoryCode, _token));
            var MasterRolesModelViewList = mapper.Map<List<MasterRole>, List<MasterRoleViewModel>>(MasterRolesList);
            maintenanceMasterRoleViewModel.MasterRoleViewModelList = MasterRolesModelViewList;

        }
        public void SaveRoles(MaintenanceRolesViewModel model)
        {
            ParentModel RolesModel = new ParentModel();
            RolesModel.AppName = Globals.AppNameEncrypt;
            RolesModel.FactoryCode = _factoryCode;
            RolesModel.PlantCode = _factoryCode;

            //model.ColorViewModel.ColorStatus = true;
            //model.MasterRoleViewModel.FactoryCode = _factoryCode;

            RolesModel.MasterRole = mapper.Map<MasterRoleViewModel, MasterRole>(model.MasterRoleViewModel);

            string RolesListJsonString = JsonConvert.SerializeObject(RolesModel);

            _masterRoleAPIRepository.SaveMasterRole(RolesListJsonString, _token);
        }
        public void UpdateRoles(MasterRoleViewModel masterRoleViewModel)
        {
            ParentModel RolesModel = new ParentModel();
            RolesModel.AppName = Globals.AppNameEncrypt;
            RolesModel.FactoryCode = _factoryCode;
            RolesModel.PlantCode = _factoryCode;

            //ColorViewModel.ColorStatus = true;
            RolesModel.FactoryCode = _factoryCode;
            RolesModel.MasterRole = mapper.Map<MasterRoleViewModel, MasterRole>(masterRoleViewModel);

            //RolesModel.Color.UpdatedDate = DateTime.Now;
            //RolesModel.Color.UpdatedBy = _username;

            string jsonString = JsonConvert.SerializeObject(RolesModel);

            _masterRoleAPIRepository.UpdateMasterRole(jsonString, _token);
        }
        #endregion

        public void GetMainMenu(MaintenanceRolesViewModel maintenanceMasterRoleViewModel, int RoleId)
        {
            List<SubMenus> subMenuList;
            List<MainMenusViewModel> menuList;
            menuList = JsonConvert.DeserializeObject<List<MainMenusViewModel>>(_mainMenusAPIRepository.GetMainMenuAllByRoleId(_factoryCode, RoleId));
            subMenuList = JsonConvert.DeserializeObject<List<SubMenus>>(_subMenusAPIRepository.GetSubMenusList(_factoryCode, _token));



            List<MainMenusViewModel> menuViewModelList = new List<MainMenusViewModel>();
            menuList.ForEach(i =>
            {
                menuViewModelList.Add(new MainMenusViewModel
                {
                    Id = i.Id,
                    MenuNameEn = i.MenuNameEn,
                    MenuNameTh = i.MenuNameTh,
                    Controller = i.Controller,
                    Action = i.Action,
                    IconName = i.IconName,
                    RoleId = i.RoleId,
                    IdmenuRole = i.IdmenuRole

                    // SubMenuList = subMenuListByRole.Where(w => w.MainMenuId == i.Id).ToList()


                });
            });
            maintenanceMasterRoleViewModel.MainMenusList = menuViewModelList;


        }

        public void GetSubMenu(MaintenanceRolesViewModel maintenanceMasterRoleViewModel, int RoleId, int menuid)
        {
            List<SubMainMenusViewModel> subMenuList;
            List<MainMenusViewModel> menuList;
            menuList = JsonConvert.DeserializeObject<List<MainMenusViewModel>>(_mainMenusAPIRepository.GetMainMenuAllByRoleId(_factoryCode, RoleId));
            subMenuList = JsonConvert.DeserializeObject<List<SubMainMenusViewModel>>(_subMenusAPIRepository.GetSubMenusAllListBYRole(_factoryCode, RoleId, _token));

            List<SubMainMenusViewModel> submenuViewModelList = new List<SubMainMenusViewModel>();

            foreach (SubMainMenusViewModel i in subMenuList.Where(x => x.MainMenuId == menuid).OrderBy(x => x.Id))
            {

                submenuViewModelList.Add(new SubMainMenusViewModel
                {
                    Id = i.Id,
                    SubMenuName = i.SubMenuName,
                    MainMenuId = i.MainMenuId,
                    SubMenuroleID = i.SubMenuroleID,
                    IdSubMenuRole = i.IdSubMenuRole,
                    Idrole = i.Idrole,
                    Idmenu = i.Idmenu

                });


            }


            //subMenuList.Where(x=>x.MainMenuId == 1).ForEach(i =>
            //{
            //    submenuViewModelList.Add(new SubMainMenusViewModel
            //    {
            //        Id = i.Id,
            //        SubMenuName = i.SubMenuName,
            //        MainMenuId = i.MainMenuId,
            //        IdRole = i.IdRole
            //        // SubMenuList = subMenuListByRole.Where(w => w.MainMenuId == i.Id).ToList()


            //    });
            //});
            maintenanceMasterRoleViewModel.SubMainMenusList = submenuViewModelList;


        }


        public void AddMenuByRoles(MenuRoleViewModel menurole)
        {

            ParentModel menuRole = new ParentModel();
            menuRole.AppName = Globals.AppNameEncrypt;
            menuRole.FactoryCode = _factoryCode;
            menuRole.PlantCode = _factoryCode;
            menuRole.MenuRole = mapper.Map<MenuRoleViewModel, MenuRole>(menurole);
            string menuRoleJsonString = JsonConvert.SerializeObject(menuRole);
            _masterRoleAPIRepository.SaveMenuByRoles(menuRoleJsonString, _token);

        }
        public void GetRolesById(MenuRoleViewModel menurole)
        {

        }
        public void DeleteMenuByRoles(int idmenurole)
        {


            _masterRoleAPIRepository.DeleteMenuByRoles(idmenurole, _token);

        }

        // Submenurole
        public void AddSubMenuByRoles(SubMenuRoleViewModel submenurole)
        {

            ParentModel model = new ParentModel();
            model.AppName = Globals.AppNameEncrypt;
            model.FactoryCode = _factoryCode;
            model.PlantCode = _factoryCode;
            model.SubMenurole = mapper.Map<SubMenuRoleViewModel, SubMenurole>(submenurole);
            string submenuRoleJsonString = JsonConvert.SerializeObject(model);
            _masterRoleAPIRepository.SaveSubMenuByRoles(submenuRoleJsonString, _token);

        }

        public void DeleteSubMenuByRoles(int subMenuroleID)
        {


            _masterRoleAPIRepository.DeleteSubMenuByRoles(subMenuroleID, _token);

        }


    }
}
