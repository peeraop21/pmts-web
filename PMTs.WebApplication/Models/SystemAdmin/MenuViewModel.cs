using PMTs.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Models.SystemAdmin
{
    public class MenuViewModel
    {
        public int Id { get; set; }
        public string MenuName { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string IconName { get; set; }
        public int SortMenu { get; set; }
        public int? RoleId { get; set; }
        public string RoleName { get; set; }

        public List<SubMenus> SubMenuList { get; set; }



    }
}
