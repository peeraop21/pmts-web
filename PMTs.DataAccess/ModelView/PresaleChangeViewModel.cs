using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.ModelView
{
    public class PresaleChangeViewModel
    {
        //change product list
        public IEnumerable<PresaleChangeProduct> PresaleChangeProducts { get; set; }
        public string MaterialNo { get; set; }

        //compare model
        public PresaleChangeProduct MasterDataCompare { get; set; }
        public PresaleChangeProduct PresaleChangeProduct { get; set; }
        public List<PresaleChangeRouting> RoutingsCompare { get; set; }
        public List<PresaleChangeRouting> PresaleChangeRoutings { get; set; }
    }
}
