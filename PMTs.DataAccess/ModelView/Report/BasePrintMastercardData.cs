using PMTs.DataAccess.ComplexModel;
using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.ModelView.Report
{
    public class BasePrintMastercardData
    {
        public List<MoDataPrintMastercard> MoDatas { get; set; }
        public List<MoSpec> MoSpecs { get; set; }
        public List<MoRoutingPrintMastercard> MoRoutings { get; set; }
        public List<AttachFileMo> AttachFileMOs { get; set; }
        public List<MasterData> MasterDatas { get; set; }
        public List<BoardAlternative> BoardAlternatives { get; set; }
        public List<PmtsConfig> PmtsConfigs { get; set; }
        public List<Machine> Machines { get; set; }
        public List<QualitySpec> QualitySpecs { get; set; }
        public List<BoardCombine> BoardCombines { get; set; }
        public List<BoardUse> BoardUses { get; set; }
        public List<FluteTr> FluteTrs { get; set; }
        public List<BoardSpec> BoardSpecs { get; set; }
        public List<ProductType> ProductTypes { get; set; }

        //mastercard
        public List<TransactionsDetail> TransactionsDetails { get; set; }
        public List<ScoreType> ScoreTypes { get; set; }
        public List<KindOfProductGroup> KindOfProductGroups { get; set; }
        public List<KindOfProduct> KindOfProducts { get; set; }
        public List<ProcessCost> ProcessCosts { get; set; }
        public List<Routing> Routings { get; set; }

        // MO From TIPS
        public List<PlanningMODataAndMoSpec> PlanningMODataAndMoSpecs { get; set; }
        public List<PlanningRouting> PlanningRoutings { get; set; }
    }
}
