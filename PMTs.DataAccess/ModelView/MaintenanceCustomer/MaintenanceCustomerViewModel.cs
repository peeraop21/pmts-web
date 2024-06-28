using PMTs.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PMTs.DataAccess.ModelView.MaintenanceCustomer
{
    public class MaintenanceCustomerViewModel
    {
        public IEnumerable<CustomerViewModel> CustomerViewModelList { get; set; }
        public IEnumerable<CustShipTo> CustShipToList { get; set; }
        public IEnumerable<ProductGroup> ProductGroupList { get; set; }
        public List<string> QaItems { get; set; }
        public CustomerViewModel CustomerViewModel { get; set; }
        public string CustShipToArrayList { get; set; }

        //Tassanai Update
        public List<string> TagPrintSO { get; set; }
    }

    public class CustomerViewModel
    {
        public int Id { get; set; }
        public string SaleOrg { get; set; }
        public string PlantCode { get; set; }
        [Required]
        public string CustName { get; set; }
        public string SoldToCode { get; set; }
        [Required]
        public string CustCode { get; set; }
        [Required]
        public string CusId { get; set; }
        public string IndGrp { get; set; }
        public string Zone { get; set; }
        public string Route { get; set; }
        public string CustShipTo { get; set; }
        public string CustDeliveryTime { get; set; }
        public string CustClass { get; set; }
        public string CustReq { get; set; }
        public string CustAlert { get; set; }
        public bool? CustStatus { get; set; }
        public int? PalletOverhang { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public List<ProductGroup> ProductGroupList { get; set; }
        public List<CustShipTo> CustShipToList { get; set; }
        public string CustShipToJsonList { get; set; }
        public string QASpec { get; set; }
        public string Accgroup { get; set; }
        public string Cust { get; set; }
        public int? PriorityFlag { get; set; }
        public string TagBundle { get; set; }
        public string TagPallet { get; set; }
        public string NoTagBundle { get; set; }
        public string HeadTagBundle { get; set; }
        public string FootTagBundle { get; set; }
        public string HeadTagPallet { get; set; }
        public string FootTagPallet { get; set; }
        public string Freetext1TagBundle { get; set; }
        public string Freetext2TagBundle { get; set; }
        public string Freetext3TagBundle { get; set; }

        public string Freetext1TagPallet { get; set; }
        public string Freetext2TagPallet { get; set; }
        public string Freetext3TagPallet { get; set; }
        public bool COA { get; set; }
        public bool Film { get; set; }


    }

}
