using System;

namespace PMTs.DataAccess.ComplexModel
{
    public class MasterDataRoutingModel
    {
        public string SaleOrder { get; set; }
        public string MaterialNo { get; set; }
        public string PC { get; set; }
        public string SaleOrg { get; set; }
        public string Plant { get; set; }
        public string CustCode { get; set; }
        public string CustID { get; set; }
        public string CustName { get; set; }
        public string Description { get; set; }
        public string Board { get; set; }
        public string BoxType { get; set; }
        public string Machine { get; set; }
        public DateTime? LastUpdate { get; set; }
        public DateTime? Createdate { get; set; }
        public bool TranStatus { get; set; }
        public string PdisStatus { get; set; }
        public int Printed { get; set; }

        public int OrderQuant { get; set; }
        public string DueDate { get; set; }
        public string Batch { get; set; }
        public string MatSaleOrg { get; set; }
        public string PartNo { get; set; }
        public string UpdatedBy { get; set; }

        public MasterDataStatus MasterDataStatus { get; set; }

        //tassanai update 06/08/2021
        public bool TagBunbleStatus { get; set; }
        public bool TagPalletStatus { get; set; }
        public string MoStatus { get; set; }
        public string Code { get; set; }
    }

    public enum MasterDataStatus
    {
        NotAvailable,
        Available,
        AvailableSB
    }
}
