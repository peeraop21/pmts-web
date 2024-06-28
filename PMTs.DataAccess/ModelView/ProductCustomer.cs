using PMTs.DataAccess.Models;
using System;
using System.Collections.Generic;

namespace PMTs.DataAccess.ModelView
{
    public class ProductCustomer
    {
        public IEnumerable<Customer> CustomerList { get; set; }
        public IEnumerable<CustShipTo> CustShipToList { get; set; }
        public IEnumerable<ProductGroup> ProductGroupList { get; set; }
        public IEnumerable<QaItems> QaItems { get; set; }
        //public IEnumerable<TagPrintSO> TagPrintSO { get; set; }
        public List<string> TagPrintSO { get; set; }

        public int Id { get; set; }
        public string FactoryCode { get; set; }
        public string CustName { get; set; }
        public string CustCode { get; set; }
        public string CusId { get; set; }
        public string Cust { get; set; }
        public string SoldToCode { get; set; }
        public string IndGrp { get; set; }
        public string IndDes { get; set; }
        public string Zone { get; set; }
        public string Route { get; set; }
        public string CustShipTo { get; set; }
        public string CustDeliveryTime { get; set; }
        public string CustClass { get; set; }
        public string CustReq { get; set; }
        public string CustAlert { get; set; }
        public bool? CustStatus { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public int? PriorityFlag { get; set; }

        public string MaterialComment { get; set; }
        public List<QualitySpec> QualitySpecs { get; set; }
        public int PalletOverhang { get; set; }

        // 
        public string TagBundle { get; set; }
        public string TagPallet { get; set; }
        public string NoTagBundle { get; set; }
        public string HeadTagBundle { get; set; }
        public string FootTagBundle { get; set; }
        public string HeadTagPallet { get; set; }
        public string FootTagPallet { get; set; }
        //tassanai 19/1/2022
        public bool COA { get; set; }
        public bool Film { get; set; }

        public string Freetext1TagBundle { get; set; }
        public string Freetext2TagBundle { get; set; }
        public string Freetext3TagBundle { get; set; }

        public string Freetext1TagPallet { get; set; }
        public string Freetext2TagPallet { get; set; }
        public string Freetext3TagPallet { get; set; }




    }
}
