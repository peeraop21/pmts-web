﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace PMTs.DataAccess.Models;

public partial class PresaleChangeProduct
{
    public int Id { get; set; }

    public string FactoryCode { get; set; }

    public string PsmId { get; set; }

    public string MaterialNo { get; set; }

    public string Description { get; set; }

    public string SaleText1 { get; set; }

    public string SaleText2 { get; set; }

    public string SaleText3 { get; set; }

    public string SaleText4 { get; set; }

    public int? PieceSet { get; set; }

    public string PrintMethod { get; set; }

    public string HighGroup { get; set; }

    public string HighValue { get; set; }

    public int? Bun { get; set; }

    public int? BunLayer { get; set; }

    public int? LayerPalet { get; set; }

    public bool? IsApprove { get; set; }

    public string FileChange { get; set; }
    public string Status { get; set; }
}