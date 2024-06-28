using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.ModelView.Login
{
    public class MenuViewModel
    {

        public int Id { get; set; }
        public string MenuNameEn { get; set; }
        public string MenuNameTh { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string IconName { get; set; }


        public int? SortMenu { get; set; }



        //  public dynamic MenuDataList { get; set; }

        public List<SubMenus> SubMenuList { get; set; }
    }

    public partial class SubMenusViewModel
    {
        public int Id { get; set; }
        public string SubMenuName { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public int MainMenuId { get; set; }


    }
}
