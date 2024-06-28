using PMTs.DataAccess.Models;
using System.Collections.Generic;
using System.Data;
using System.Reflection.PortableExecutable;
using Machine = PMTs.DataAccess.Models.Machine;

namespace PMTs.DataAccess.ComplexModel
{
    public class ChangeReCalculateTrimModel
    {
        public string Flute { get; set; }
        public List<Routing> Routings { get; set; }
        public List<ReCalculateTrimModel> ReCalculateTrimModels { get; set; }
        public DataTable DataTable { get; set; }
    }

    public class ReCalculateTrimModel : Routing
    {
        public string Flute { get; set; }
        public int? TrimOfFlute { get; set; }
        //public string Board { get; set; }
        //public int GroupPaperWidth { get; set; }
        //public int? CutSheetWid { get; set; }
        //public int CutOff { get; set; }
        //public bool MinTrim { get; set; }
        //public int PageMin { get; set; }//MinOut
        //public int PageMinTrim { get; set; }
        //public int PageMax { get; set; }
        public bool UpdateStatus { get; set; }
        public string ErrorMessase { get; set; }
    }

    public class ReCalculateTrimViewModel
    {
        public List<FluteAndMachineModel> FluteAndMachineModels { get; set; }
        public List<Machine> Machines { get; set; }
        public string Status { get; set; }
    }

    public partial class ParamCalPaperWidth
    {
        public string MachineName { get; set; }
        public string FactoryCode { get; set; }
        public string Flute { get; set; }
        public int SheetInWid { get; set; }
        public string MaterialNo { get; set; }
        IEnumerable<string> PaperItem { get; set; }
    }

    public partial class ReturnCalPaperWidth
    {
        public string MaterialNo { get; set; }
        public string MachineName { get; set; }
        public int SheetInWid { get; set; }
        public string Flute { get; set; }

        public string PaperWidthOld { get; set; }
        public string CutOld { get; set; }
        public string TrimOld { get; set; }
        public string PercentTrimOld { get; set; }

        public string PaperWidth { get; set; }
        public string Cut { get; set; }
        public string Trim { get; set; }
        public string PercentTrim { get; set; }
    }
}
