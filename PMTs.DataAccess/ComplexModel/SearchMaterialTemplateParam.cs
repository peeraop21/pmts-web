using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMTs.DataAccess.ComplexModel
{
    public class SearchMaterialTemplateParam
    {
        public string TypeSearch { get; set; }
        public List<string> Boards { get; set; }
        public List<string> Grades { get; set; }
        public List<string> Customers { get; set; }
    }
}
