﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace PMTs.DataAccess.Models;

public partial class MachineFluteTrim
{
    public int Id { get; set; }

    public string FactoryCode { get; set; }

    public string Machine { get; set; }

    public string Flute { get; set; }

    public int Trim { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string UpdatedBy { get; set; }
}