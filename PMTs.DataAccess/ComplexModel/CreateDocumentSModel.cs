using PMTs.DataAccess.Models;
using System;
using System.Collections.Generic;

namespace PMTs.DataAccess.ComplexModel
{
    public class CreateDocumentSModel
    {
        public string searchOnMain { get; set; }
        public string searchOnChange { get; set; }
        public DocumentSData documentSData { get; set; }
        public DocumentS documentS { get; set; }
        public List<DocumentS> ldocumentS { get; set; }
        public DocumentSlist documentSlist { get; set; }
        public List<DocumentSlist> ldocumentSlist { get; set; }

        public ManageDocument ManageData { get; set; }
        public List<DocumentsMOData> moDatas { get; set; }

    }

    public class DocumentsMOData : ManageDocument
    {
        public bool isBox { get; set; }
        public bool BoxChange { get; set; }
        public int? ChangeQuantity { get; set; }
        public DateTime DueDate { get; set; }
        public string CustomerName { get; set; }
        public string BoxType { get; set; }
        public int OrderQuantity { get; set; }
        public string MoStatus { get; set; }
        public string ProductType { get; set; }
        public string MaterialType { get; set; }
        public string PoNo { get; set; }
        public string SaleText1 { get; set; }
        public string BlockType { get; set; }
        public int? PieceSet { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }

    public class DocumentSData
    {
        public string OrderItem { get; set; }
        public string CustomerName { get; set; }
        public string PO { get; set; }
        public string MateriailNo { get; set; }
        public string PC { get; set; }
        public string SaleText1 { get; set; }
        public string Flute { get; set; }
        public string OrderQty { get; set; }
        public string DueDate { get; set; }
        public string BoxType { get; set; }
        public string Process { get; set; }

        public string MatType { get; set; }

    }

    public class ManageDocument
    {
        public int Id { get; set; }
        public string FactoryCode { get; set; }
        public string Snumber { get; set; }
        public string OrderItem { get; set; }
        public string Pc { get; set; }
        public string MaterialNo { get; set; }
        public string Flute { get; set; }
        public string DuedateOld { get; set; }
        public string DuedateNew { get; set; }
        public int? OrderQtyOld { get; set; }
        public int? OrderQtyNew { get; set; }
        public bool? Cancel { get; set; }
        public bool? Hold { get; set; }
        public string BoxStatus { get; set; }
        public string PartStatus { get; set; }
        public string Process { get; set; }
        public string Remark { get; set; }
        public string Username { get; set; }
        public string CheckHold { get; set; }
        public string Customer { get; set; }
    }

    public class ReportDocumentS
    {
        public string CompanyTh { get; set; }
        public string CompanyEn { get; set; }
        public string SDocDate { get; set; }
        public string SDocName { get; set; }
        public string Customer { get; set; }
        public string UserCreate { get; set; }
        // public string Docscode        {get;set;}
        public string Docsnumber { get; set; }
        public List<DocumentSlist> reportDocumentSlists { get; set; }

    }
}
