namespace PMTs.DataAccess.ComplexModel
{
    public partial class Cost
    {
        public string FactoryCode { get; set; }
        public string BoardCode { get; set; }
        public string CostField { get; set; }
        public double? CostPerTon { get; set; }
    }
}
