using Microsoft.Extensions.Configuration;
using PMTs.DataAccess.ComplexModel;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services.Interfaces
{
    public interface IReportService
    {
        void SearchReportRepeatOrderItemsByDateAndRepeatCount(ref List<CheckRepeatOrder> checkRepeatOrders, string dateFrom, string dateTo, string repeatCount);

        void SearchReportCheckOrderQtyTooMuchByDateAndRepeatCount(ref List<CheckOrderQtyTooMuch> checkOrderQtyTooMuch, string dateFrom, string dateTo, string repeatCount);

        void SearchReportCheckDiffDueDate(ref List<CheckDiffDueDate> CheckDiffDueDate, int datediff, string dateFrom, string dateTo);

        void ReportCheckDueDateToolong(ref List<CheckDueDateToolong> checkRepeatOrders, int dayCount);

        void SearchReportCheckMOAndKIWI(IConfiguration configuration, ref List<MoDataPrintMastercard> moDatas, string startDueDate, string endDueDate);

        void SearchReportCheckMOAndSAP(IConfiguration configuration, ref ReportCheckMOAndTextfileSAPViewModel reportCheckMOAndTextfileSAPViewModel, string startDueDate, string endDueDate, string configWordingString);

        void SearchReportMOManual(ref List<CheckRepeatOrder> moDatas, string materialNo, string custName, string pc, string startDueDate, string endDueDate, string startCreateDate, string endCreateDate, string startUpdateDate, string endUpdateDate, string po, string so, string note, string soStatus);

        ReportCheckOrderItem ReportCheckOrederItem(string dateFrom, string dateTo);

        void SearchReportCheckMOWithSStatusAndKIWI(IConfiguration configuration, ref List<MoDataPrintMastercard> moDatas, string startDueDate, string endDueDate);

        ConfigWordingReport GetConfigWordingReportByFactoryCode();

        ConfigWordingReport CreateConfigWordingReport(string configWordingString);

        string GetTIPApiUrlByFactoryCode();

        void SearchReportCheckStatusColor(ref List<CheckStatusColor> statusColors, int colorId);
    }
}