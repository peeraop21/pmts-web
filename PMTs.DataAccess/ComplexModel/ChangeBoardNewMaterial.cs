namespace PMTs.DataAccess.ComplexModel
{
    public class ChangeBoardNewMaterial : MasterDataRoutingModel
    {
        public bool IsCreatedSuccess { get; set; }
        public string CopyMaterialNo { get; set; }
        //public string HierarchyLV3 { get; set; }
        //public string HierarchyLV4 { get; set; }
        public string Hierarchy { get; set; }
        public string CodeNewBoard { get; set; }
        public string ErrorMessage { get; set; }
        public double Cost { get; set; }
        public string Price { get; set; }
        public string Change { get; set; }
        public string HighValue { get; set; }
        public string BoardAlternative { get; set; }
        public string Flute { get; set; }
        public string NewBoard { get; set; }
        public string CopyFactoryCode { get; set; }
        public string MaterialType { get; set; }
    }
}
