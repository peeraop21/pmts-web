﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace PMTs.DataAccess.Models;

public partial class PpcRawMaterialMaster
{
    public int Id { get; set; }

    public int? MaterialType { get; set; }

    public string MaterialNumber { get; set; }

    public string Plant { get; set; }

    public string MaterialDescription { get; set; }

    public decimal? NetWeight { get; set; }

    public string MaterialGroup { get; set; }

    public string Uom { get; set; }

    public DateTime? DateOfCreation { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string UpdateBy { get; set; }

    public string OldMaterialNumber { get; set; }
}