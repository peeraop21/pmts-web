namespace PMTs.DataAccess.ComplexModel
{
    public class BoardSpecStation
    {
        public int Id { get; set; }
        public string BoardId { get; set; }
        public int Item { get; set; }
        public int PaperId { get; set; }
        public string PaperDes { get; set; }
        public int? Weight { get; set; }
        public string Station { get; set; }
        public double? Tr { get; set; }
    }
}
