using PMTs.DataAccess.Models;

namespace PMTs.DataAccess.ModelView.ManageMO
{
    public class MoDataViewModel : MoData
    {
        public string PC { get; set; }
        public string SaleText { get; set; }

        //Tassanai 06/08/2021
        public string TagBundle { get; set; }
        public string TagPallet { get; set; }

    }
}
