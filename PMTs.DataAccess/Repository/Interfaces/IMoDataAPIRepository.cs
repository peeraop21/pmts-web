using PMTs.DataAccess.ComplexModel;
using PMTs.DataAccess.ModelView.Login;
using System.Threading.Tasks;

namespace PMTs.DataAccess.Repository.Interfaces
{
    public interface IMoDataAPIRepository
    {
        string GetMoDataList(string factoryCode, string token);

        void SaveMoData(string jsonString, string token);

        void UpdateMoData(string jsonString, string token);

        void DeleteMoData(string jsonString, string token);

        string GetMoDataListBySaleOrder(string factoryCode, string stratSO, string endSO, string token);

        string GetMoDataListBySaleOrderNonX(string factoryCode, string stratSO, string endSO, string token);

        string GetMoDataListBySearchTypeNonX(string factoryCode, string searchType, string searchText, string token);

        string GetMoDataListBySaleOrders(string factoryCode, string saleOrders, string token);

        string GetMoDataBySaleOrderNonX(string factoryCode, string saleOrder, string token);

        string GetMoDatasBySaleOrderNonX(string factoryCode, string saleOrder, string token);

        string UpdateMoDataSentKIWI(string factoryCode, string saleOrder, string User, string token);

        string CreateMODataFromExcelFile(string factoryCode, string username, string jsonMORouingModel, string token);

        string CreateMOManual(string moDatas, string token);

        string ReportCheckRepeatOrder(string factoryCode, string dateFrom, string dateTo, string repeatCount, string token);

        string ReportCheckOrderQtyTooMuch(string factoryCode, string dateFrom, string dateTo, string repeatCount, string token);

        string ReportCheckDiffDueDate(string FactoryCode, int datediff, string dateFrom, string dateTo, string token);

        string ReportCheckDueDateToolong(string FactoryCode, int dayCount, string token);

        string ReportMOManual(string factoryCode, string materialNo, string custname, string pc, string startDueDate, string endDueDate, string startCreateDate, string endCreateDate, string startUpdateDate, string endUpdateDate, string po, string so, string note, string soStatus, string token);

        string GetMoDatasByDueDateRange(string factoryCode, string startDueDate, string endDueDate, string token);

        string GetReportCheckOrderItem(string Username, string factoryCode, string startDueDate, string endDueDate, string token);

        string GetMoDataListBySaleOrdersByDapper(string factoryCode, string saleOrders, string token);

        string GetMoDatasByDueDateRangeAndStatus(string factoryCode, string status, string startDueDate, string endDueDate, string token);

        //boo edit block platen
        string GetBlockPlatenMaster(string factorycode, string material, string pc, string token);

        string GetBlockPlatenRouting(string factorycode, string material, string token);

        string UpdateBlockPlatenRouting(string factorycode, string username, string model, string token);

        string GetMoDataListBySaleOrderNonXAndH(string factoryCode, string startSO, string endSO, string token);

        string CheckMaterialNo(string factoryCode, string materialNo, string token);

        Task<PlaningMOModel> GetDataOfMOFromTIPs(PlaningMOModel planingMOModel, UserTIP userTIP, string orderItems);

        void UpdateLogPrintMO(string logPrintMO, string token);

        string SearchMoDataListBySaleOrderNonXAndH(string factoryCode, string startSO, string endSO, string token);

        string GetMasterCardMOsBySaleOrders(string factoryCode, string jsonString, string token);

        string GetBaseOfMasterCardMOsBySaleOrders(string factoryCode, bool isUserTIPs, string jsonString, string token);

        void UpdatePrintedMODataByOrderItems(string factoryCode, string username, string jsonString, string token);
    }
}