using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.MaintenanceRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services.Interfaces
{
    public interface IMaintenanceRolesService
    {
        void GetRoles(MaintenanceRolesViewModel maintenanceMasterRoleViewModel);
        void SaveRoles(MaintenanceRolesViewModel maintenanceMasterRoleViewModel);
        void UpdateRoles(MasterRoleViewModel ProductGroupViewModel);

        void GetMainMenu(MaintenanceRolesViewModel maintenanceMasterRoleViewModel, int RoleId);
        void GetSubMenu(MaintenanceRolesViewModel maintenanceMasterRoleViewModel, int RoleId, int menuid);

        void AddMenuByRoles(MenuRoleViewModel menuRoleViewModel);

        void DeleteMenuByRoles(int idmenurole);

        void GetRolesById(MenuRoleViewModel menuRoleViewModel);

        //SubMenuRole
        void AddSubMenuByRoles(SubMenuRoleViewModel subMenuRoleViewModel);
        void DeleteSubMenuByRoles(int idSubmenurole);








    }
}
