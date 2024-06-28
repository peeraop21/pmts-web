using System;
using System.Collections.Generic;

namespace PMTs.DataAccess.ComplexModel
{

    public class BoardCombindMainTainModel
    {
        public BoardCombindMainTainModel()
        {
            BoardCombind = new List<BoardCombind>();
            FluteTR = new List<FluteTR>();
            BoardSpect = new List<BoardSpect>();
            Option = new List<Option>();
            _BoardCombind = new BoardCombind();
            _BoardSpect = new List<BoardSpect>();
            PaperGrade = new List<PaperGrades>();
            Preview = new List<Preview>();
            PreviewTblHeader = new PreviewTblHeader();
            editCostFieldsModel = new EditCostFieldsModel();
            ProductTypeOptions = new List<ProductTypeOptionModel>();

        }
        public string MaxID { get; set; }
        public List<BoardCombind> BoardCombind { get; set; }
        public List<FluteTR> FluteTR { get; set; }
        public List<BoardSpect> BoardSpect { get; set; }
        public List<Option> Option { get; set; }
        public List<PaperGrades> PaperGrade { get; set; }

        public List<BoardSpect> _BoardSpect { get; set; }
        public BoardCombind _BoardCombind { get; set; }

        public List<Preview> Preview { get; set; }

        public PreviewTblHeader PreviewTblHeader { get; set; }

        public EditCostFieldsModel editCostFieldsModel { get; set; }

        public string GroupCompany { get; set; }

        public List<ProductTypeOptionModel> ProductTypeOptions { get; set; }


    }



    public class BoardCombind
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Flute { get; set; }
        public string Board { get; set; }
        public string Board_Combine { get; set; }
        public double? Weight { get; set; }
        public double? Thickness { get; set; }
        public double? StandardCost { get; set; }
        public string Kiwi { get; set; }
        public string CorrControl { get; set; }
        public int? Strength { get; set; }
        public double? ECTStrength { get; set; }
        public double? FCTStrength { get; set; }
        public double? Burst { get; set; }
        public bool? StandardBoard { get; set; }
        public bool? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }

    public class FluteTR
    {
        public int Id { get; set; }
        public string FactoryCode { get; set; }
        public string FluteCode { get; set; }
        public string Station { get; set; }
        public decimal? Tr { get; set; }
        public int Item { get; set; }
        public bool? HasCoating { get; set; }
        public bool? Status { get; set; }
    }

    public class BoardSpect
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Grade { get; set; }
        public string Station { get; set; }
        public int Item { get; set; }
    }

    public class Option
    {
        public string value { get; set; }
        public string text { get; set; }
    }

    public class PaperGrades
    {
        public int Id { get; set; }
        public string Paper { get; set; }
        public int BasicWeight { get; set; }
        public bool? Liners { get; set; }
        public bool? Medium { get; set; }
        public int? MaxPaperWidth { get; set; }
        public int? Cost { get; set; }
        public int Group { get; set; }
        public string Kiwi { get; set; }
        public string Bsh { get; set; }
        public string Grade { get; set; }
        public int PaperId { get; set; }
        public string PaperDes { get; set; }
        public int? Layer { get; set; }
        public double? Stang { get; set; }
        public int? Length { get; set; }
        public bool? Active { get; set; }
    }

    public class Preview
    {
        public string tbl0 { get; set; }
        public string tbl1 { get; set; }
        public string tbl2 { get; set; }
        public string tbl3 { get; set; }
        public string tbl4 { get; set; }
        public string tbl5 { get; set; }
    }

    public class PreviewTblHeader
    {
        public string lon { get; set; }
        public string cover { get; set; }
        public string cover_row { get; set; }
    }

    public class ProductTypeOptionModel
    {
        public int IdKindOfProduct { get; set; }
        public string KindOfProductName { get; set; }
        public int? GroupPriority { get; set; }
        public List<ProductTypeOption> ProductTypeOptions { get; set; }
    }

    public class ProductTypeOption
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
    public class ExportDataForSAPRequest
    {
        public string BoardCode { get; set; }
        public string Board { get; set; }
        public List<string> HirerarchyLv2Codes { get; set; }
    }

    public class ExportDataForSAPResponse
    {
        public List<ExportDataForSAPItem> Items { get; set; }

    }
    public class ExportDataForSAPItem
    {
        public string Code { get; set; }
        public string Board { get; set; }
    }
}
