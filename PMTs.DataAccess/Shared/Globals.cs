using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace PMTs.DataAccess.Shared
{
    public static class Globals
    {
        public static string AppNameEncrypt = "W2pJlQL8hpY=";
        public static string WebAPIUrl { get; set; }
        public static string WebADSCG { get; set; }
        public static string PresaleHoldMat { get; set; }
        public static List<SelectListItem> Domain()
        {
            List<SelectListItem> myList = new List<SelectListItem>();
            var data = new[]{
                 new SelectListItem{ Value="",Text=""},
                 new SelectListItem{ Value="CEMENTHAI",Text="CEMENTHAI"}, };
            myList = data.ToList();


            return myList;
        }
    }
}
