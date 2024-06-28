﻿using PMTs.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PMTs.DataAccess.ModelView.MaintenanceMachine
{
    public class MaintenanceMachineViewModel
    {
        public IEnumerable<MachineViewModel> MachineViewModelList { get; set; }
        public List<MachineGroup> MachineGroupList { get; set; }
        public MachineViewModel MachineViewModel { get; set; }
        public List<JointViewModel> JoinList { get; set; }
    }

    public class MachineViewModel
    {
        public int Id { get; set; }
        public string FactoryCode { get; set; }
        public string Plant { get; set; }
        [Required]
        public string Machine1 { get; set; }
        [Required]
        public string Code { get; set; }
        public string PlanCode { get; set; }
        public int? Colour { get; set; }
        public bool Glue { get; set; }
        public int? Speed { get; set; }
        public double? SpePercen { get; set; }
        public int? SpeMin { get; set; }
        public int? SpeMax { get; set; }
        public int? SpeTranf { get; set; }
        public int? SetupTime { get; set; }
        public int? SetupWaste { get; set; }
        public int? RunWaste { get; set; }
        public DateTime? PrepareTimeWait { get; set; }
        public DateTime? PostTimeWait { get; set; }
        public string Remark { get; set; }
        public bool Platen { get; set; }
        public bool Rotary { get; set; }
        public int? HumanResoure { get; set; }
        public bool? McMove { get; set; }
        public string TimeUnit { get; set; }
        public int? Hardship { get; set; }
        public bool StdProcess { get; set; }
        public int? MinQuantity { get; set; }
        public int? MinSpeed { get; set; }
        public bool? CapChk { get; set; }
        public int? StdDowntime { get; set; }
        public string MachineType { get; set; }
        public string MachineSubType { get; set; }
        public int? Cap { get; set; }
        public string CapUnit { get; set; }
        public int? SeqNo { get; set; }
        public int? StdSpeed { get; set; }
        public int? StdSetupTime { get; set; }
        public string MachineGroup { get; set; }
        public bool IsCalPaperwidth { get; set; }
        public bool IsPropCor { get; set; }
        public bool IsPropPrint { get; set; }
        public bool IsPropDieCut { get; set; }
        public bool? MachineStatus { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public bool ShowProcess { get; set; }
        public int? Leadtime { get; set; }
        public int? DownTime { get; set; }
        public decimal? MaxTonsFG { get; set; }


        //Tassanai 15122021
        public int Maxa { get; set; }
        public int Maxb { get; set; }
        public int Maxc { get; set; }
        public int Maxl { get; set; }
        public int Maxw { get; set; }
        public int Mina { get; set; }
        public int Minb { get; set; }
        public int Minc { get; set; }
        public int Minl { get; set; }
        public int Minw { get; set; }
        public int PaperRollWidth { get; set; }
        public int Cutternum { get; set; }
        public int Maxd { get; set; }
        public int Maxe { get; set; }
        public int Mind { get; set; }
        public int Mine { get; set; }
        public int Priority { get; set; }
        public string CodeMachineType { get; set; }
        public string GlueType { get; set; }



    }

}