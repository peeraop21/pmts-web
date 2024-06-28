using PMTs.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMTs.DataAccess.ModelView.Report
{
    public class CheckStatusColor
    {
        public string MaterialNo { get; set; }
        public string PC { get; set; }
        public string Customer { get; set; }
        public string Description { get; set; }

        public string Color1 { get; set; }

        public string Shade1 { get; set; }

        public string Color2 { get; set; }

        public string Shade2 { get; set; }

        public string Color3 { get; set; }

        public string Shade3 { get; set; }

        public string Color4 { get; set; }

        public string Shade4 { get; set; }

        public string Color5 { get; set; }

        public string Shade5 { get; set; }

        public string Color6 { get; set; }

        public string Shade6 { get; set; }

        public string Color7 { get; set; }

        public string Shade7 { get; set; }

        public string Color8 { get; set; }

        public string Shade8 { get; set; }

        public string Color9 { get; set; }

        public string Shade9 { get; set; }

        public string Color10 { get; set; }

        public string Shade10 { get; set; }
    }

    public class CheckStatusColorViewModel
    {
        public List<CheckStatusColor> CheckStatusColors { get; set; }
        public List<Color> Colors { get; set; }
        public int ColorId { get; set; }
    }
}