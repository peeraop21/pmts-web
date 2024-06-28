using System.Collections.Generic;

namespace PMTs.DataAccess.ComplexModel
{
    public class EditBlockPlatenModel
    {
        public EditBlockPlatenModel()
        {
            editBlockPlatenRouting = new List<EditBlockPlatenRouting>();
            editBlockPlatenMasters = new List<EditBlockPlatenMaster>();
        }
        public List<EditBlockPlatenRouting> editBlockPlatenRouting { get; set; }
        public List<EditBlockPlatenMaster> editBlockPlatenMasters { get; set; }
    }

    public class EditBlockPlatenRouting
    {
        public string factorycode { get; set; }
        public string materialno { get; set; }
        public string machine { get; set; }
        public string printingplate { get; set; }
        public string cuttingdieno { get; set; }
        public string mylano { get; set; }
        public string seq { get; set; }
    }

    public class EditBlockPlatenMaster
    {
        public string factorycode { get; set; }
        public string materialno { get; set; }
        public string pc { get; set; }
        public string custname { get; set; }
        public string saletext { get; set; }
    }


}
