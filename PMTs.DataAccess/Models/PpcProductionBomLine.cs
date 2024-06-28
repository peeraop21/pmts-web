using System;

namespace PMTs.DataAccess.Models
{
    public partial class PpcProductionBomLine
    {
        public int Id { get; set; }
        public string ProductionBomNo { get; set; }
        public string VersionCode { get; set; }
        public int LineNo { get; set; }
        public int Type { get; set; }
        public string No { get; set; }
        public string Description { get; set; }
        public string UnitOfMeasureCode { get; set; }
        public decimal Quantity { get; set; }
        public string Position { get; set; }
        public string Position2 { get; set; }
        public string Position3 { get; set; }
        public string LeadTimeOffset { get; set; }
        public string RoutingLinkCode { get; set; }
        public decimal Scrap { get; set; }
        public string VariantCode { get; set; }
        public DateTime StartingDate { get; set; }
        public DateTime EndingDate { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Weight { get; set; }
        public decimal Depth { get; set; }
        public int CalculationFormula { get; set; }
        public decimal QuantityPer { get; set; }
        public int SubType { get; set; }
        public decimal WeightPerPcs { get; set; }
        public decimal NoOfWork { get; set; }
        public decimal SizeOfPaperWidth { get; set; }
        public decimal NoOfCutSize { get; set; }
        public decimal SizeOfPaperHeight { get; set; }
        public string Description2 { get; set; }
        public string SizeUom { get; set; }
        public string ItemCategory { get; set; }
    }
}
