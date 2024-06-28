using PMTs.DataAccess.ComplexModel;
using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.ModelView
{
    public class ParentModel
    {
        public string AppName { get; set; }
        public string SaleOrg { get; set; }
        public string PlantCode { get; set; }
        public string FactoryCode { get; set; }

        public AllowanceHard AllowanceHard { get; set; }
        public AllowanceProcess AllowanceProcess { get; set; }
        public BoardAlternative BoardAlternative { get; set; }
        public BoardCombine BoardCombine { get; set; }
        public BoardSpec BoardSpec { get; set; }
        public BoardUse BoardUse { get; set; }
        public BomStruct BomStruct { get; set; }
        public BuildRemark BuildRemark { get; set; }
        public ChangeHistory ChangeHistory { get; set; }
        public Color Color { get; set; }
        public Coating Coating { get; set; }
        public CompanyProfile CompanyProfile { get; set; }
        public CorConfig CorConfig { get; set; }
        public CustShipTo CustShipTo { get; set; }
        public Customer Customer { get; set; }
        public Flute Flute { get; set; }
        public FluteLayer FluteLayer { get; set; }
        public FluteTr FluteTr { get; set; }
        public HierarchyLv2Matrix HierarchyLv2Matrix { get; set; }
        public Joint Joint { get; set; }
        public KindOfProduct KindOfProduct { get; set; }
        public KindOfProductGroup KindOfProductGroup { get; set; }
        public Machine Machine { get; set; }
        public MainMenus MainMenus { get; set; }
        public MapCost MapCost { get; set; }
        public MasterData MasterData { get; set; }
        public MasterRole MasterRole { get; set; }
        public MasterUser MasterUser { get; set; }
        public MaterialType MaterialType { get; set; }
        public MenuRole MenuRole { get; set; }
        public MoBoardAlternative MoBoardAlternative { get; set; }
        public MoBoardUse MoBoardUse { get; set; }
        public MoData MoData { get; set; }
        public MoDatalog MoDatalog { get; set; }
        public MoRouting MoRouting { get; set; }
        public MoSpec MoSpec { get; set; }
        public MoTemp MoTemp { get; set; }
        public Pallet Pallet { get; set; }
        public ProductGroup ProductGroup { get; set; }
        public PaperGrade PaperGrade { get; set; }
        public PaperWidth PaperWidth { get; set; }
        public PlantView PlantView { get; set; }
        public PmtsConfig PmtsConfig { get; set; }
        public PrintMethod PrintMethod { get; set; }
        public ProcessCost ProcessCost { get; set; }
        public ProductType ProductType { get; set; }
        public ProductionType ProductionType { get; set; }
        public Routing Routing { get; set; }
        public RunningNo RunningNo { get; set; }
        public SalesView SalesView { get; set; }
        public SubMenus SubMenus { get; set; }
        public ScoreGap ScoreGap { get; set; }
        public TransactionsDetail TransactionsDetail { get; set; }
        public UnitMaterial UnitMaterial { get; set; }
        public SubMenurole SubMenurole { get; set; }



        // Dynamic
        public List<AllowanceHard> AllowanceHardList { get; set; }
        public List<AllowanceProcess> AllowanceProcessList { get; set; }
        public List<BoardAlternative> BoardAlternativeList { get; set; }
        public List<BoardCombine> BoardCombineList { get; set; }
        public List<BoardSpec> BoardSpecList { get; set; }
        public List<BoardUse> BoardUseList { get; set; }
        public List<BomStruct> BomStructList { get; set; }
        public List<BuildRemark> BuildRemarkList { get; set; }
        public List<ChangeHistory> ChangeHistoryList { get; set; }
        public List<Color> ColorList { get; set; }
        public List<CompanyProfile> CompanyProfileList { get; set; }
        public List<CorConfig> CorConfigList { get; set; }
        public List<CustShipTo> CustShipToList { get; set; }
        public List<Customer> CustomerList { get; set; }
        public List<Flute> FluteList { get; set; }
        public List<FluteLayer> FluteLayerList { get; set; }
        //public List<FluteMaster> FluteMasterList { get; set; }
        public List<FluteTr> FluteTrList { get; set; }
        public List<HierarchyLv2Matrix> HierarchyLv2MatrixList { get; set; }
        public List<Joint> JointList { get; set; }
        public List<KindOfProduct> KindOfProductList { get; set; }
        public List<KindOfProductGroup> KindOfProductGroupList { get; set; }
        public List<Machine> MachineList { get; set; }
        public List<MainMenus> MainMenusList { get; set; }
        public List<MapCost> MapCostList { get; set; }
        public List<MasterData> MasterDataList { get; set; }
        public List<MasterRole> MasterRoleList { get; set; }
        public List<MasterUser> MasterUserList { get; set; }
        public List<MaterialType> MaterialTypeList { get; set; }
        public List<MenuRole> MenuList { get; set; }
        public List<MoBoardAlternative> MoBoardAlternativeList { get; set; }
        public List<MoBoardUse> MoBoardUseList { get; set; }
        public List<MoData> MoDataList { get; set; }
        public List<MoDatalog> MoDatalogList { get; set; }
        public List<MoRouting> MoRoutingList { get; set; }
        public List<MoSpec> MoSpecList { get; set; }
        public List<MoTemp> MoTempList { get; set; }
        public List<Pallet> PalletList { get; set; }
        public List<PaperGrade> PaperGradeList { get; set; }
        public List<PaperWidth> PaperWidthList { get; set; }
        public List<PlantView> PlantViewList { get; set; }
        public List<PmtsConfig> PmtsConfigList { get; set; }
        public List<PrintMethod> PrintMethodList { get; set; }
        public List<ProcessCost> ProcessCostList { get; set; }
        public List<ProductType> ProductTypeList { get; set; }
        public List<ProductionType> ProductionTypeList { get; set; }
        public List<Routing> RoutingList { get; set; }
        public List<RunningNo> RunningNoList { get; set; }
        public List<SalesView> SalesViewList { get; set; }
        public List<SubMenus> SubMenusList { get; set; }
        public List<TransactionsDetail> TransactionsDetailList { get; set; }
        public List<UnitMaterial> UnitMaterialList { get; set; }

        public List<Coating> CoatingList { get; set; }

        // Complex
        public MasterDataRoutingModel MasterDataRoutingModel { get; set; }

        // Dynamic Complex
        public List<MasterDataRoutingModel> MasterDataRoutingModelList { get; set; }
    }
}
