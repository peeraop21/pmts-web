﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace PMTs.DataAccess.Models;

public partial class CompanyProfile
{
    public int Id { get; set; }

    public string CompanyName { get; set; }

    public string SaleOrg { get; set; }

    public string Plant { get; set; }

    public string Code { get; set; }

    public string ShortName { get; set; }

    public int? Group { get; set; }

    public string CustomerGroup { get; set; }

    public string UserApp { get; set; }

    public string PlaningServer { get; set; }

    public string PlaningUser { get; set; }

    public string PlaningPass { get; set; }

    public string PurchasGrp { get; set; }

    public int? Piece2 { get; set; }

    public int? CorMaxCut { get; set; }

    public string Language { get; set; }

    public string Lat { get; set; }

    public string Long { get; set; }

    public bool? StatusPmts { get; set; }

    public string BusinessGroup { get; set; }

    public string PlanningProgram { get; set; }
}