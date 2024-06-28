using System;
using System.Collections.Generic;

namespace PMTs.DataAccess.ComplexModel
{
    public class ReportCheckOrderItem
    {
        public List<ReportOrderItemData> ReportOrderItem { get; set; }
        public List<GetOrderItemData> OrderItemSap { get; set; }
        public List<GetOrderItemData> OrderItemKiwi { get; set; }
        public List<GetOrderItemData> OrderItemMoData { get; set; }

        public List<GetOrderItemDataReport> ReportSap { get; set; }
        public List<GetOrderItemDataReport> ReportKiwi { get; set; }
        public List<GetOrderItemDataReport> ReportMo { get; set; }

        public List<ReportOrderItem> ReportFinal { get; set; }
    }

    public class ReportOrderItem
    {
        public string OrderItem { get; set; }
        public string SapOrderQty { get; set; }
        public string SapDueDate { get; set; }
        public string MoOrderQty { get; set; }
        public string MoDueDate { get; set; }
        public string KiwiOrderQty { get; set; }
        public string KiwiDueDate { get; set; }
    }

    public class GetOrderItemDataReport
    {
        public string OrderItem { get; set; }
        public string DueDate { get; set; }
    }


    public class GetOrderItemData
    {
        public string OrderItem { get; set; }
        public DateTime? DueDate { get; set; }
    }

    public class GetOrderItemDataSap
    {
        public string OrderItem { get; set; }
        public string Orderkiwi { get; set; }
        public string SaleQty { get; set; }
        public DateTime? DueDate { get; set; }
    }

    public class ReportOrderItemData
    {
        public string OrderItemSap { get; set; }
        public DateTime? DueDateSap { get; set; }
        public string OrderItemKiwi { get; set; }
        public DateTime? DueDateKiwi { get; set; }
        public string OrderItemMoData { get; set; }
        public DateTime? DueDateMoData { get; set; }
    }
}
