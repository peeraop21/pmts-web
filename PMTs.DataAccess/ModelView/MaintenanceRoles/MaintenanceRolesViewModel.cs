using System.Collections.Generic;

namespace PMTs.DataAccess.ModelView.MaintenanceRoles
{
    public class MaintenanceRolesViewModel
    {
        public IEnumerable<MasterRoleViewModel> MasterRoleViewModelList { get; set; }
        public MasterRoleViewModel MasterRoleViewModel { get; set; }
        public List<MainMenusViewModel> MainMenusList { get; set; }
        public List<SubMainMenusViewModel> SubMainMenusList { get; set; }

    }
    public class MasterRoleViewModel
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string RoleDesc { get; set; }
    }
    public class MainMenusViewModel
    {
        public int Id { get; set; }
        public string MenuNameEn { get; set; }
        public string MenuNameTh { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string IconName { get; set; }
        public int? SortMenu { get; set; }
        public int? RoleId { get; set; }
        public int? IdmenuRole { get; set; }
    }

    public class SubMainMenusViewModel
    {
        //public int Id { get; set; }
        //public string SubMenuName { get; set; }
        //public string Controller { get; set; }
        //public string Action { get; set; }
        //public int MainMenuId { get; set; }
        //public int IdRole { get; set; }
        //public int? IdSubMenuRole { get; set; }
        //public int? IdMenuRole { get; set; }

        public int Id { get; set; }
        public string SubMenuName { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public int MainMenuId { get; set; }
        public int? SubMenuroleID { get; set; } // SubMenurole.Id
        public int? IdSubMenuRole { get; set; }
        public int? Idrole { get; set; }
        public int? Idmenu { get; set; }

    }

    public class MenuRoleViewModel
    {
        public int Id { get; set; }
        public int? IdMenu { get; set; }
        public int? IdRole { get; set; }
        public int? SortMenu { get; set; }

    }

    public class SubMenuRoleViewModel
    {
        public int Id { get; set; }
        public int? IdSubMenuRole { get; set; }
        public int? IdRole { get; set; }
        public int? IdMenu { get; set; }

    }
}
