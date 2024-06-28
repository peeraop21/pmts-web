namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IDocumentSRepository
    {
        string GetDocumentS(string factoryCode, string snumber, string token);
        string CreateNewDocumentS(string factoryCode, string username, string token);
        string GetDocumentSList(string factoryCode, string orderitem, string token);
        string GetDataMo(string factoryCode, string orderitem, string token);
        void SaveDocumentS(string factorycode, string jsonString, string token);
        void UpdateDocumentS(string factorycode, string jsonString, string token);
        void DeleteDocumentS(string factorycode, string id, string token);
        string GetDocumentSReport(string factoryCode, string snumber, string usercreate, string token);
        string SearchDocumentsAndMODataByOrderItem(string factoryCode, string sNumber, string orderItem, string token);
        void SaveChangeDocuments(string factoryCode, string jsonDocumentSlist, string token);
        string GetDocumentSListForReportDocument(string factoryCode, string materialNo, string customerName, string pc, string so, string token);
    }
}
