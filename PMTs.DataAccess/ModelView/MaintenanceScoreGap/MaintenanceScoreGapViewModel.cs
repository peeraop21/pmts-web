using PMTs.DataAccess.Models;
using System;
using System.Collections.Generic;

namespace PMTs.DataAccess.ModelView.MaintenanceScoreGap
{
    public class MaintenanceScoreGapViewModel
    {
        public IEnumerable<ScoreGapViewModel> ScoreGapViewModelList { get; set; }
        public ScoreGapViewModel ScoreGapViewModel { get; set; }
        public List<ScoreType> ScoreTypeList { get; set; }
    }

    public class ScoreGapViewModel
    {
        public int Id { get; set; }
        public string FactoryCode { get; set; }
        public string Flute { get; set; }
        public string ScoreType { get; set; }
        public double? ScoreGap1 { get; set; }
        public string ControllerCode { get; set; }
        public string PlanProgramCode { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }


}
