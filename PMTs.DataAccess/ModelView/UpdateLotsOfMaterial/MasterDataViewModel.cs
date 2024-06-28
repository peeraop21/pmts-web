namespace PMTs.DataAccess.ModelView.UpdateLotsOfMaterial
{
    public class MasterDataViewModel
    {
        public int Id { get; set; }
        public string FactoryCode { get; set; }
        public string SaleOrg { get; set; }
        public string MaterialNo { get; set; }
        public string Pc { get; set; }
        public string Description { get; set; }
        public string CustName { get; set; }
        public string PdisStatus { get; set; }
        public bool TranStatus { get; set; }
        public bool SapStatus { get; set; }
    }
}
