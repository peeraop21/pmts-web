using PMTs.DataAccess.ComplexModel;
using PMTs.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services.Interfaces
{
    public interface IDocumentSService
    {
        CreateDocumentSModel GetDocumentS(string snumber);
        CreateDocumentSModel CreateDocumentS();
        DocumentSData GetOrderData(string orderitem);
        void AddDocList(ManageDocument model);
        void EditDocList(ManageDocument model);
        void DeleteDocList(string id);
        List<DocumentSlist> GetDocumentSList(string Snumber);
        ReportDocumentS GetReportDocumentS(string Snumber);
        void SearchDocumentsAndMODataByOrderItem(ref List<DocumentsMOData> moDatas, string orderItem, string sNumber);
        void ChangeOrderQuantity(ref List<DocumentsMOData> moDatas, string orderItem, string changeOrderQuantity);
        void BindSelectMoDatasToDocuments(ref DocumentsMOData moData, ref bool orderItemsForSave, List<string> orderItems);
        void SaveChangeDocuments(List<ManageDocument> model, string orderItem);
        void SearchReportDocumentS(ref List<DocumentSlist> documentSlists, string customerName, string so, string materialNo, string pc);
        string GetShortNameFacOfOutsourceByOrderItem(string orderItem);
    }
}
