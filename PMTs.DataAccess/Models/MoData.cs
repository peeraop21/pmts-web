﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace PMTs.DataAccess.Models;

public partial class MoData
{
    public int Id { get; set; }

    public string FactoryCode { get; set; }

    public string MoStatus { get; set; }

    public string OrderItem { get; set; }

    public string MaterialNo { get; set; }

    public string Name { get; set; }

    public int OrderQuant { get; set; }

    public double? ToleranceOver { get; set; }

    public double? ToleranceUnder { get; set; }

    public DateTime? OriginalDueDate { get; set; }

    public DateTime DueDate { get; set; }

    public int? TargetQuant { get; set; }

    public string ItemNote { get; set; }

    public string District { get; set; }

    public string PoNo { get; set; }

    public string DateTimeStamp { get; set; }

    public int? Printed { get; set; }

    public string Batch { get; set; }

    public string DueText { get; set; }

    public string SoldTo { get; set; }

    public string ShipTo { get; set; }

    public string PlanStatus { get; set; }

    public int? StockQty { get; set; }

    public bool? IsCreateManual { get; set; }

    public bool? SentKiwi { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string UpdatedBy { get; set; }

    public string Morno { get; set; }

    public string SoKiwi { get; set; }

    public double? SquareInch { get; set; }

    public bool? InterfaceTips { get; set; }

    public int? PrintRoundNo { get; set; }

    public int? AllowancePrintNo { get; set; }

    public int? AfterPrintNo { get; set; }

    public int? DrawAmountNo { get; set; }

    public string MoFrom { get; set; }

    public string SboExternalNumber { get; set; }
}