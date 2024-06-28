using PMTs.DataAccess.Models;
using System;
using System.Collections.Generic;

namespace PMTs.DataAccess.ModelView.MaintenanceBoard
{
    public class MaintenanceBoardModel
    {
        public List<BoardCombine> ModelList { get; set; }
        public IEnumerable<BoardModel> BoardModelList { get; set; }
        public BoardModel ModelToSave { get; set; }
    }

    public class BoardModel
    {
        public int Id { get; set; }
        public string FactoryCode { get; set; }
        public string Code { get; set; }
        public string Flute { get; set; }
        public string Board { get; set; }
        public string BoardCombine1 { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Thickness { get; set; }
        public decimal? StandardCost { get; set; }
        public string Kiwi { get; set; }
        public string CorrControl { get; set; }
        public decimal? Ectstrength { get; set; }
        public decimal? Fctstrength { get; set; }
        public decimal? Burst { get; set; }
        public bool? StandardBoard { get; set; }
        public bool? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public List<Flute> FluteSelectList { get; set; }
    }
}
