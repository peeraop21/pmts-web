using System;
using System.Collections.Generic;

namespace PMTs.DataAccess.ModelView.MaintenancePaperGrade
{
    public class MaintenancePaperGradeViewModel
    {
        public IEnumerable<PaperGradeViewModel> PaperGradeViewModelList { get; set; }
        public PaperGradeViewModel PaperGradeViewModel { get; set; }
    }

    public class PaperGradeViewModel
    {
        public int Id { get; set; }
        public string Paper { get; set; }
        public int BasicWeight { get; set; }
        public bool Liners { get; set; }
        public bool Medium { get; set; }
        public int? MaxPaperWidth { get; set; }
        public int? Cost { get; set; }
        public int Group { get; set; }
        public string Kiwi { get; set; }
        public string Bsh { get; set; }
        public string Grade { get; set; }
        public int PaperId { get; set; }
        public string PaperDes { get; set; }
        public int? Layer { get; set; }
        public double? Stang { get; set; }
        public int? Length { get; set; }
        public bool Active { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string GradeCodeMachine { get; set; }
    }


}
