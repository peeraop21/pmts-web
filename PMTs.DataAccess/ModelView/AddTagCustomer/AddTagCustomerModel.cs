using System.Collections.Generic;

namespace PMTs.DataAccess.ModelView.AddTagCustomer
{
    public class MaintainAddTagCustomerModel
    {
        public List<string> TagPrintSO { get; set; }
        public IEnumerable<AddTagCustomerModel> AddTagCustomerModel { get; set; }
    }
    //Tassanai Update

    public class AddTagCustomerModel
    {
        public string FactoryCode { get; set; }
        public string MaterialNo { get; set; }
        public string PC { get; set; }
        public string SaleOrg { get; set; }
        public string Plant { get; set; }
        public string CustCode { get; set; }
        public string CustID { get; set; }
        public string CustName { get; set; }
        public string Description { get; set; }
        public string SaleText1 { get; set; }
        public string SaleText2 { get; set; }
        public string SaleText3 { get; set; }
        public string SaleText4 { get; set; }
        public string Flute { get; set; }
        public string Board { get; set; }
        public string TagBundle { get; set; }
        public string TagPallet { get; set; }
        public string NoTagBundle { get; set; }
        public string HeadTagBundle { get; set; }
        public string FootTagBundle { get; set; }
        public string HeadTagPallet { get; set; }
        public string FootTagPallet { get; set; }





    }
}
