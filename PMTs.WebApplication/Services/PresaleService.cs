using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PMTs.DataAccess.ComplexModel;
using PMTs.DataAccess.Extentions;
using PMTs.DataAccess.ModelPresale;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.Login;
using PMTs.DataAccess.ModelView.NewProduct;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Shared;
using PMTs.DataAccess.Tracing;
using PMTs.DataAccess.Utils;
using PMTs.WebApplication.Extentions;
using PMTs.WebApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using static PMTs.DataAccess.ModelView.PresaleViewModel;

namespace PMTs.WebApplication.Services
{
    [TraceAspect]
    public class PresaleService : IPresaleService
    {
        // PresaleContext _presaleContext;
        UserSessionModel userSessionModel;
        IHttpContextAccessor _httpContextAccessor;

        private readonly IBoardCombineAPIRepository _boardCombineAPIRepository;
        private readonly IPresaleChangeProductAPIRepository presaleChangeProductAPIRepository;
        private readonly IPresaleChangeRoutingAPIRepository presaleChangeRoutingAPIRepository;
        private readonly IMasterDataAPIRepository masterDataAPIRepository;
        private readonly IRoutingAPIRepository routingAPIRepository;
        private readonly IProductCatalogCofigRepository _productCatalogCofigRepository;
        private readonly IConfiguration configuration;
        private readonly IMapper mapper;

        private readonly string _username;
        private readonly string _saleOrg;
        private readonly string _factoryCode;
        private readonly string _token;
        private readonly bool businessGroup;
        private string presaleLinkIp;
        private string presaleConnectionStr;

        public PresaleService(IHttpContextAccessor httpContextAccessor
            , IBoardCombineAPIRepository boardCombineAPIRepository,
            IPresaleChangeProductAPIRepository presaleChangeProductAPIRepository,
            IPresaleChangeRoutingAPIRepository presaleChangeRoutingAPIRepository,
            IMasterDataAPIRepository masterDataAPIRepository,
            IRoutingAPIRepository routingAPIRepository,
            IProductCatalogCofigRepository productCatalogCofigRepository,
            IConfiguration configuration,
            IMapper mapper
            )
        {
            _httpContextAccessor = httpContextAccessor;
            _boardCombineAPIRepository = boardCombineAPIRepository;
            this.presaleChangeRoutingAPIRepository = presaleChangeRoutingAPIRepository;
            this.presaleChangeProductAPIRepository = presaleChangeProductAPIRepository;
            this.masterDataAPIRepository = masterDataAPIRepository;
            this.routingAPIRepository = routingAPIRepository;
            _productCatalogCofigRepository = productCatalogCofigRepository;
            this.configuration = configuration;

            userSessionModel = SessionExtentions.GetSession<UserSessionModel>(_httpContextAccessor.HttpContext.Session, "UserSessionModel");
            if (userSessionModel != null)
            {
                _username = userSessionModel.UserName;
                _saleOrg = userSessionModel.SaleOrg;
                _factoryCode = userSessionModel.FactoryCode;
                _token = userSessionModel.Token;
                businessGroup = userSessionModel.BusinessGroup == "Offset";
            }
            this.mapper = mapper;

            presaleLinkIp = "[172.26.64.101].";
            presaleConnectionStr = "155";
        }

        public List<PresaleViewModel> SearchPresale(IConfiguration configuration, PresaleViewModel param)
        {
            //return PresaleRepository.GetPresaleBySearch(configuration, param);
            string plantName = PresaleTools.GetPlantShortName(_factoryCode);

            string condition = "";
            condition += (String.IsNullOrEmpty(param.Param.ddlSearch1)) ? "" : param.Param.ddlSearch1 + " like '%" + param.Param.txtSearch1 + "%' and ";
            condition += (String.IsNullOrEmpty(param.Param.ddlSearch2)) ? "" : param.Param.ddlSearch2 + " like '%" + param.Param.txtSearch2 + "%' and ";
            condition += (String.IsNullOrEmpty(param.Param.ddlSearch3)) ? "" : param.Param.ddlSearch3 + " like '%" + param.Param.txtSearch3 + "%' and ";
            condition += (String.IsNullOrEmpty(param.Param.ddlSearch4)) ? "" : param.Param.ddlSearch4 + " like '%" + param.Param.txtSearch4 + "%' and ";
            //condition = condition.Substring(0, condition.Length - 4); //ตัด & ตัวสุดท้ายออก

            //PresaleViewModel presaleModel = JsonConvert.DeserializeObject <plantName + masterData>(_ + plantName + masterDataAPIRepository.GetPresale(_saleOrg, _plantCode, condition));
            List<PresaleViewModel> presaleViewModels = new List<PresaleViewModel>();
            var masterDatas = new List<PresaleMasterData>();
            string sqlcon = configuration.GetConnectionString("PresaleConnect");

            if (!sqlcon.Contains(presaleConnectionStr))
            {
                presaleLinkIp = string.Empty;
            }

            var sQueryMaster = @"SELECT 
                [PSM_ID] as PsmId
                ,[PSM_Status] as PsmStatus
                ,[Material_No] as MaterialNo
                ,[Part_No] as PartNo
                ,[PC] as Pc
                ,[Hierarchy] as Hierarchy
                ,[Sale_Org] as SaleOrg
                ,[Plant] as Plant
                ,[Cust_Code] as CustCode
                ,[Cus_ID] as CusId
                ,[Cust_Name] as CustName
                ,[Description] as Description
                ,[Sale_Text1] as SaleText1
                ,[Sale_Text2] as SaleText2
                ,[Sale_Text3] as SaleText3
                ,[Sale_Text4] as SaleText4
                ,[Change] as Change
                ,[Language] as Language
                ,[Ind_Grp] as IndGrp
                ,[Ind_Des] as IndDes
                ,[Material_Type] as MaterialType
                ,[Print_Method] as PrintMethod
                ,[TwoPiece] as TwoPiece
                ,[Flute] as Flute
                ,[Code] as Code
                ,[Board] as Board
                ,[GL] as Gl
                ,[GLWeigth]  as  Glweigth
                ,[BM] as  Bm
                ,[BMWeigth] as Bmweigth
                ,[BL] as Bl
                ,[BLWeigth] as Blweigth
                ,[CM] as Cm
                ,[CMWeigth] as Cmweigth
                ,[CL] as Cl
                ,[CLWeigth] as Clweigth
                ,[DM]  as Dm
                ,[DMWeigth] as Dmweigth
                ,[DL] as Dl
                ,[DLWeigth] as Dlweigth
                ,[Wid] as Wid
                ,[Leg] as Leg
                ,[Hig] as Hig
                ,[Box_Type] as BoxType
                ,[RSC_Style] as  RscStyle
                ,[Pro_Type] as  ProType
                ,[JoinType] as JoinType
                ,[Status_Flag] as StatusFlag
                ,[Priority_Flag] as PriorityFlag
                ,[Wire] as Wire
                ,[Outer_Join] as  OuterJoin
                ,[CutSheetLeng] as CutSheetLeng
                ,[CutSheetWid] as CutSheetWid
                ,[Sheet_Area] as SheetArea
                ,[Box_Area] as BoxArea
                ,[ScoreW1] as ScoreW1
                ,[Scorew2] as ScoreW2
                ,[Scorew3] as ScoreW3
                ,[Scorew4] as ScoreW4
                ,[Scorew5] as ScoreW5
                ,[Scorew6] as ScoreW6
                ,[Scorew7] as ScoreW7
                ,[Scorew8] as ScoreW8
                ,[Scorew9] as ScoreW9
                ,[Scorew10]  as ScoreW10
                ,[Scorew11]  as ScoreW11
                ,[Scorew12]  as ScoreW12
                ,[Scorew13]  as ScoreW13
                ,[Scorew14]  as ScoreW14
                ,[Scorew15]  as ScoreW15
                ,[Scorew16]  as ScoreW16
                ,[JointLap] as JointLap 
                ,[ScoreL2] as ScoreL2
                ,[ScoreL3]  as ScoreL3
                ,[ScoreL4]  as ScoreL4
                ,[ScoreL5]  as ScoreL5
                ,[ScoreL6]  as ScoreL6
                ,[ScoreL7]  as ScoreL7
                ,[ScoreL8]  as ScoreL8
                ,[ScoreL9]  as ScoreL9
                ,[Slit] as Slit
                ,[No_Slot] as NoSlot
                ,[Bun] as Bun
                ,[BunLayer] as BunLayer
                ,[LayerPalet] as  LayerPalet
                ,[BoxPalet] as BoxPalet
                ,[Weight_Sh] as WeightSh
                ,[Weight_Box] as WeightBox
                ,[SparePercen] as  SparePercen
                ,[SpareMax] as SpareMax
                ,[SpareMin] as  SpareMin 
                ,[LeadTime] as LeadTime
                ,[Piece_Set] as PieceSet
                ,[Sale_UOM] as  SaleUom
                ,[BOM_UOM] as BomUom
                ,[Hardship]  as Hardship
                ,[PalletSize] as  PalletSize
                ,[Palletization_Path] as  PalletizationPath
                ,[PrintMaster_Path] as PrintMasterPath
                ,[DiecutPict_Path] as DiecutPictPath
                ,[CreateDate] 
                ,[LastUpdate] 
                ,[User]
                ,[Plt_Leg_Double] as PltLegDouble
                ,[Plt_Double_axle] as  PltDoubleAxle
                ,[Plt_Leg_Single] as  PltLegSingle
                ,[Plt_Single_axle] as PltSingleAxle
                ,[Plt_Floor_Above] as PltFloorAbove
                ,[Plt_Floor_Under] as PltFloorUnder
                ,[Plt_Beam] as PltBeam
                ,[Plt_Axle_Height] as  PltAxleHeight
                ,[EanCode] as  EanCode
                ,[PDIS_Status] as  PdisStatus
                ,[Tran_Status] as  TranStatus
                ,[SAP_Status] as  SapStatus
                ,[NewH] as NewH
                ,[Pur_Txt1] as PurTxt1
                ,[Pur_Txt2]  as PurTxt2
                ,[Pur_Txt3]  as PurTxt3
                ,[Pur_Txt4]  as PurTxt4
                ,[UnUpgrad_Board] as  UnUpgradBoard
                ,[High_Group] as  HighGroup
                ,[High_Value] as HighValue
                ,[FLAG_CHANGE] as FlagChange
                ,[PS_AW_file] as PsAwfile
                ,[PS_DW_file] as PsDwFile
                ,[PS_LP_file] as PsLpFile
                ,[PS_PO_file] as PsPoFile
                ,[PS_QT_file] as PsQtFile
                ,[PS_Other_file] as PsOtherFile
                ,[PS_PV_file] as PsPvFile
                ,[PS_All_file] as PsAllFile
                from " + presaleLinkIp + "StagingPresale.dbo." + plantName + "_MasterData WHERE " + condition + "PSM_Status = '1' AND PDIS_Status != 'X' AND ([FLAG_CHANGE] IS NULL OR [FLAG_CHANGE] <> 'Y') " +
                " order by LastUpdate desc";


            using (IDbConnection db = new SqlConnection(configuration.GetConnectionString("PresaleConnect")))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                //Execute sql query
                masterDatas = db.Query<PresaleMasterData>(sQueryMaster).ToList();
            }

            //var masterDatas = _presaleContext.PresaleMasterData.FromSql(sQueryMaster).AsNoTracking().ToList();
            foreach (var masterData in masterDatas)
            {
                presaleViewModels.Add(new PresaleViewModel
                {
                    presaleMasterData = masterData,
                    PSM_ID = masterData.PsmId,
                    PSM_Status = masterData.PsmStatus,
                    Material_No = masterData.MaterialNo,
                    PC = masterData.Pc,
                    Description = masterData.Description,
                    User = masterData.User,
                    CreateDate = masterData.CreateDate == null ? DateTime.Now : masterData.CreateDate.Value,
                    Cust_Name = masterData.CustName,
                    LastUpdate = masterData.LastUpdate == null ? DateTime.Now : masterData.LastUpdate.Value,
                    Plant = _factoryCode,
                    Board = masterData.Board,
                    flute = masterData.Flute,
                    FlagChange = masterData.FlagChange,
                    PsAwfile = masterData.PsAwfile,
                    PsDwFile = masterData.PsDwFile,
                    PsLpFile = masterData.PsLpFile,
                    PsQtFile = masterData.PsQtFile,
                    PsPoFile = masterData.PsPoFile,
                    PsPvFile = masterData.PsPvFile,
                    PsAllFile = masterData.PsAllFile,
                    PsOtherFile =
                        !string.IsNullOrEmpty(masterData.PsOtherFile)
                        ? JsonConvert.DeserializeObject<OtherFiles>(masterData.PsOtherFile)
                        : new OtherFiles { files = new List<Files>() },
                });
            }

            return presaleViewModels;
        }

        public PresaleMasterData SearchPresaleByPsmId(IConfiguration configuration, string psmId)
        {
            //return PresaleRepository.GetPresaleBySearch(configuration, param);
            string plantName = PresaleTools.GetPlantShortName(_factoryCode);

            var presaleMasterData = new PresaleMasterData();
            string sqlcon = configuration.GetConnectionString("PresaleConnect");

            if (!sqlcon.Contains(presaleConnectionStr))
            {
                presaleLinkIp = string.Empty;
            }

            var sQueryMaster = @"SELECT 
                [PSM_ID] as PsmId
                ,[PSM_Status] as PsmStatus
                ,[Material_No] as MaterialNo
                ,[Part_No] as PartNo
                ,[PC] as Pc
                ,[Hierarchy] as Hierarchy
                ,[Sale_Org] as SaleOrg
                ,[Plant] as Plant
                ,[Cust_Code] as CustCode
                ,[Cus_ID] as CusId
                ,[Cust_Name] as CustName
                ,[Description] as Description
                ,[Sale_Text1] as SaleText1
                ,[Sale_Text2] as SaleText2
                ,[Sale_Text3] as SaleText3
                ,[Sale_Text4] as SaleText4
                ,[Change] as Change
                ,[Language] as Language
                ,[Ind_Grp] as IndGrp
                ,[Ind_Des] as IndDes
                ,[Material_Type] as MaterialType
                ,[Print_Method] as PrintMethod
                ,[TwoPiece] as TwoPiece
                ,[Flute] as Flute
                ,[Code] as Code
                ,[Board] as Board
                ,[GL] as Gl
                ,[GLWeigth]  as  Glweigth
                ,[BM] as  Bm
                ,[BMWeigth] as Bmweigth
                ,[BL] as Bl
                ,[BLWeigth] as Blweigth
                ,[CM] as Cm
                ,[CMWeigth] as Cmweigth
                ,[CL] as Cl
                ,[CLWeigth] as Clweigth
                ,[DM]  as Dm
                ,[DMWeigth] as Dmweigth
                ,[DL] as Dl
                ,[DLWeigth] as Dlweigth
                ,[Wid] as Wid
                ,[Leg] as Leg
                ,[Hig] as Hig
                ,[Box_Type] as BoxType
                ,[RSC_Style] as  RscStyle
                ,[Pro_Type] as  ProType
                ,[JoinType] as JoinType
                ,[Status_Flag] as StatusFlag
                ,[Priority_Flag] as PriorityFlag
                ,[Wire] as Wire
                ,[Outer_Join] as  OuterJoin
                ,[CutSheetLeng] as CutSheetLeng
                ,[CutSheetWid] as CutSheetWid
                ,[Sheet_Area] as SheetArea
                ,[Box_Area] as BoxArea
                ,[ScoreW1] as ScoreW1
                ,[Scorew2] as ScoreW2
                ,[Scorew3] as ScoreW3
                ,[Scorew4] as ScoreW4
                ,[Scorew5] as ScoreW5
                ,[Scorew6] as ScoreW6
                ,[Scorew7] as ScoreW7
                ,[Scorew8] as ScoreW8
                ,[Scorew9] as ScoreW9
                ,[Scorew10]  as ScoreW10
                ,[Scorew11]  as ScoreW11
                ,[Scorew12]  as ScoreW12
                ,[Scorew13]  as ScoreW13
                ,[Scorew14]  as ScoreW14
                ,[Scorew15]  as ScoreW15
                ,[Scorew16]  as ScoreW16
                ,[JointLap] as JointLap 
                ,[ScoreL2] as ScoreL2
                ,[ScoreL3]  as ScoreL3
                ,[ScoreL4]  as ScoreL4
                ,[ScoreL5]  as ScoreL5
                ,[ScoreL6]  as ScoreL6
                ,[ScoreL7]  as ScoreL7
                ,[ScoreL8]  as ScoreL8
                ,[ScoreL9]  as ScoreL9
                ,[Slit] as Slit
                ,[No_Slot] as NoSlot
                ,[Bun] as Bun
                ,[BunLayer] as BunLayer
                ,[LayerPalet] as  LayerPalet
                ,[BoxPalet] as BoxPalet
                ,[Weight_Sh] as WeightSh
                ,[Weight_Box] as WeightBox
                ,[SparePercen] as  SparePercen
                ,[SpareMax] as SpareMax
                ,[SpareMin] as  SpareMin 
                ,[LeadTime] as LeadTime
                ,[Piece_Set] as PieceSet
                ,[Sale_UOM] as  SaleUom
                ,[BOM_UOM] as BomUom
                ,[Hardship]  as Hardship
                ,[PalletSize] as  PalletSize
                ,[Palletization_Path] as  PalletizationPath
                ,[PrintMaster_Path] as PrintMasterPath
                ,[DiecutPict_Path] as DiecutPictPath
                ,[CreateDate] 
                ,[LastUpdate] 
                ,[User]
                ,[Plt_Leg_Double] as PltLegDouble
                ,[Plt_Double_axle] as  PltDoubleAxle
                ,[Plt_Leg_Single] as  PltLegSingle
                ,[Plt_Single_axle] as PltSingleAxle
                ,[Plt_Floor_Above] as PltFloorAbove
                ,[Plt_Floor_Under] as PltFloorUnder
                ,[Plt_Beam] as PltBeam
                ,[Plt_Axle_Height] as  PltAxleHeight
                ,[EanCode] as  EanCode
                ,[PDIS_Status] as  PdisStatus
                ,[Tran_Status] as  TranStatus
                ,[SAP_Status] as  SapStatus
                ,[NewH] as NewH
                ,[Pur_Txt1] as PurTxt1
                ,[Pur_Txt2]  as PurTxt2
                ,[Pur_Txt3]  as PurTxt3
                ,[Pur_Txt4]  as PurTxt4
                ,[UnUpgrad_Board] as  UnUpgradBoard
                ,[High_Group] as  HighGroup
                ,[High_Value] as HighValue
                ,[FLAG_CHANGE] as FlagChange
                from " + presaleLinkIp + "StagingPresale.dbo." + plantName + $"_MasterData WHERE PSM_ID = '{psmId}' AND PSM_Status = '1' AND PDIS_Status != 'X' ";// AND ([FLAG_CHANGE] IS NULL OR [FLAG_CHANGE] <> 'Y')";


            using (IDbConnection db = new SqlConnection(configuration.GetConnectionString("PresaleConnect")))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                //Execute sql query
                presaleMasterData = db.Query<PresaleMasterData>(sQueryMaster).FirstOrDefault();
            }

            return presaleMasterData;
        }

        public MasterDataTransactionModel ImportPresale(IConfiguration configuration, PresaleViewModel presale)
        {
            MasterDataTransactionModel master = new MasterDataTransactionModel();
            master.Routings = new List<Routing>();
            //PresaleViewModel oPresale = PresaleRepository.GetMasterDataRoutingPresale(configuration, _presaleContext, presale);
            master.TransactionAction = "Presale";

            PresaleViewModel result = new PresaleViewModel();
            result.presaleMasterData = new PresaleMasterData();
            result.presaleRoutingModels = new List<PresaleRouting>();
            master.TransactionsDetail = new TransactionsDetail();
            master.PresaleMasterData = new PresaleMasterData();
            try
            {
                string plantName = PresaleTools.GetPlantShortName(_factoryCode);
                string sqlcon = configuration.GetConnectionString("PresaleConnect");

                if (!sqlcon.Contains(presaleConnectionStr))
                {
                    presaleLinkIp = string.Empty;
                }

                #region query master
                var sQueryMaster = @"SELECT 
                    [PSM_ID] as PsmId
                    ,[PSM_Status] as PsmStatus
                    ,[Material_No] as MaterialNo
                    ,[Part_No] as PartNo
                    ,[PC] as Pc
                    ,[Hierarchy] as Hierarchy
                    ,[Sale_Org] as SaleOrg
                    ,[Plant] as Plant
                    ,[Cust_Code] as CustCode
                    ,[Cus_ID] as CusId
                    ,[Cust_Name] as CustName
                    ,[Description] as Description
                    ,[Sale_Text1] as SaleText1
                    ,[Sale_Text2] as SaleText2
                    ,[Sale_Text3] as SaleText3
                    ,[Sale_Text4] as SaleText4
                    ,[Change] as Change
                    ,[Language] as Language
                    ,[Ind_Grp] as IndGrp
                    ,[Ind_Des] as IndDes
                    ,[Material_Type] as MaterialType
                    ,[Print_Method] as PrintMethod
                    ,[TwoPiece] as TwoPiece
                    ,[Flute] as Flute
                    ,[Code] as Code
                    ,[Board] as Board
                    ,[GL] as Gl
                    ,[GLWeigth]  as  Glweigth
                    ,[BM] as  Bm
                    ,[BMWeigth] as Bmweigth
                    ,[BL] as Bl
                    ,[BLWeigth] as Blweigth
                    ,[CM] as Cm
                    ,[CMWeigth] as Cmweigth
                    ,[CL] as Cl
                    ,[CLWeigth] as Clweigth
                    ,[DM]  as Dm
                    ,[DMWeigth] as Dmweigth
                    ,[DL] as Dl
                    ,[DLWeigth] as Dlweigth
                    ,[Wid] as Wid
                    ,[Leg] as Leg
                    ,[Hig] as Hig
                    ,[Box_Type] as BoxType
                    ,[RSC_Style] as  RscStyle
                    ,[Pro_Type] as  ProType
                    ,[JoinType] as JoinType
                    ,[Status_Flag] as StatusFlag
                    ,[Priority_Flag] as PriorityFlag
                    ,[Wire] as Wire
                    ,[Outer_Join] as  OuterJoin
                    ,[CutSheetLeng] as CutSheetLeng
                    ,[CutSheetWid] as CutSheetWid
                    ,[Sheet_Area] as SheetArea
                    ,[Box_Area] as BoxArea
                    ,[ScoreW1] as ScoreW1
                    ,[Scorew2] as ScoreW2
                    ,[Scorew3] as ScoreW3
                    ,[Scorew4] as ScoreW4
                    ,[Scorew5] as ScoreW5
                    ,[Scorew6] as ScoreW6
                    ,[Scorew7] as ScoreW7
                    ,[Scorew8] as ScoreW8
                    ,[Scorew9] as ScoreW9
                    ,[Scorew10]  as ScoreW10
                    ,[Scorew11]  as ScoreW11
                    ,[Scorew12]  as ScoreW12
                    ,[Scorew13]  as ScoreW13
                    ,[Scorew14]  as ScoreW14
                    ,[Scorew15]  as ScoreW15
                    ,[Scorew16]  as ScoreW16
                    ,[JointLap] as JointLap 
                    ,[ScoreL2] as ScoreL2
                    ,[ScoreL3]  as ScoreL3
                    ,[ScoreL4]  as ScoreL4
                    ,[ScoreL5]  as ScoreL5
                    ,[ScoreL6]  as ScoreL6
                    ,[ScoreL7]  as ScoreL7
                    ,[ScoreL8]  as ScoreL8
                    ,[ScoreL9]  as ScoreL9
                    ,[Slit] as Slit
                    ,[No_Slot] as NoSlot
                    ,[Bun] as Bun
                    ,[BunLayer] as BunLayer
                    ,[LayerPalet] as  LayerPalet
                    ,[BoxPalet] as BoxPalet
                    ,[Weight_Sh] as WeightSh
                    ,[Weight_Box] as WeightBox
                    ,[SparePercen] as  SparePercen
                    ,[SpareMax] as SpareMax
                    ,[SpareMin] as  SpareMin 
                    ,[LeadTime] as LeadTime
                    ,[Piece_Set] as PieceSet
                    ,[Sale_UOM] as  SaleUom
                    ,[BOM_UOM] as BomUom
                    ,[Hardship]  as Hardship
                    ,[PalletSize] as  PalletSize
                    ,[Palletization_Path] as  PalletizationPath
                    ,[PrintMaster_Path] as PrintMasterPath
                    ,[DiecutPict_Path] as DiecutPictPath
                    ,[CreateDate] 
                    ,[LastUpdate] 
                    ,[User]
                    ,[Plt_Leg_Double] as PltLegDouble
                    ,[Plt_Double_axle] as  PltDoubleAxle
                    ,[Plt_Leg_Single] as  PltLegSingle
                    ,[Plt_Single_axle] as PltSingleAxle
                    ,[Plt_Floor_Above] as PltFloorAbove
                    ,[Plt_Floor_Under] as PltFloorUnder
                    ,[Plt_Beam] as PltBeam
                    ,[Plt_Axle_Height] as  PltAxleHeight
                    ,[EanCode] as  EanCode
                    ,[PDIS_Status] as  PdisStatus
                    ,[Tran_Status] as  TranStatus
                    ,[SAP_Status] as  SapStatus
                    ,[NewH] as NewH
                    ,[Pur_Txt1] as PurTxt1
                    ,[Pur_Txt2]  as PurTxt2
                    ,[Pur_Txt3]  as PurTxt3
                    ,[Pur_Txt4]  as PurTxt4
                    ,[UnUpgrad_Board] as  UnUpgradBoard
                    ,[High_Group] as  HighGroup
                    ,[High_Value] as HighValue
                    ,[customer_req] as CustComment
                    ,[material_comment] as MaterialComment
                    ,[join_character] as JoinCharacter
                    ,[process_cost] as ProcessCost
                    from " + presaleLinkIp + "StagingPresale.dbo." + plantName + "_MasterData WHERE PSM_ID = '" + presale.PSM_ID + "' and PSM_Status = '1' AND PDIS_Status != 'X' ";
                #endregion
                var sQueryTransactionDetail = string.Empty;
                #region query transaction detail
                if (businessGroup)
                {
                    sQueryTransactionDetail = @"SELECT
                    [IdKindProcessCost] as IdProcessCost
                    ,[IdKindOfProductType] as IdKindOfProduct	
                    ,[IdMaterialType] as IdMaterialType		
                    ,[IsWrap] as IsWrap			
                    ,[IsNotch] as IsNotch		
                    ,[NotchDegree] as NotchDegree	
                    ,[NotchArea] as NotchArea		
                    ,[NotchSide] as NotchSide		
                    ,[Side_A] as SideA			
                    ,[Side_B] as SideB			
                    ,[Side_C] as SideC			
                    ,[Side_D] as SideD			
                    ,[CGType] as Cgtype
                    ,[HvaGroup1] as HvaGroup1
                    ,[HvaGroup2] as HvaGroup2
                    ,[HvaGroup3] as HvaGroup3
                    ,[HvaGroup4] as HvaGroup4
                    ,[HvaGroup5] as HvaGroup5
                    ,[HvaGroup6] as HvaGroup6
                    ,[HvaGroup7] as HvaGroup7
                    ,[NewPrintPlate] as NewPrintPlate 
                    ,[OldPrintPlate] as OldPrintPlate 
                    ,[NewBlockDieCut] as NewBlockDieCut
                    ,[OldBlockDieCut] as OldBlockDieCut
                    ,[ExampleColor] as ExampleColor 
                    ,[CoatingType] as CoatingType 
                    ,[CoatingTypeDesc] as CoatingTypeDesc
                    ,[PaperHorizontal] as PaperHorizontal
                    ,[PaperVertical] as PaperVertical 
                    ,[FluteHorizontal] as FluteHorizontal
                    ,[FluteVertical] as FluteVertical
                    from " + presaleLinkIp + "StagingPresale.dbo." + plantName + "_MasterData WHERE PSM_ID = '" + presale.PSM_ID + "' and PSM_Status = '1' AND PDIS_Status != 'X' ";
                }
                else
                {
                    sQueryTransactionDetail = @"SELECT
                    [IdKindProcessCost] as IdProcessCost
                    ,[IdKindOfProductType] as IdKindOfProduct	
                    ,[IdMaterialType] as IdMaterialType		
                    ,[IsWrap] as IsWrap			
                    ,[IsNotch] as IsNotch		
                    ,[NotchDegree] as NotchDegree	
                    ,[NotchArea] as NotchArea		
                    ,[NotchSide] as NotchSide		
                    ,[Side_A] as SideA			
                    ,[Side_B] as SideB			
                    ,[Side_C] as SideC			
                    ,[Side_D] as SideD			
                    ,[CGType] as Cgtype
                    ,[HvaGroup1] as HvaGroup1
                    ,[HvaGroup2] as HvaGroup2
                    ,[HvaGroup3] as HvaGroup3
                    ,[HvaGroup4] as HvaGroup4
                    ,[HvaGroup5] as HvaGroup5
                    ,[HvaGroup6] as HvaGroup6
                    ,[HvaGroup7] as HvaGroup7
                    from " + presaleLinkIp + "StagingPresale.dbo." + plantName + "_MasterData WHERE PSM_ID = '" + presale.PSM_ID + "' and PSM_Status = '1' AND PDIS_Status != 'X' ";

                }
                #endregion

                #region query routing
                var sQueryRouting = @"SELECT 
                    [PSM_ID]  as  PsmId
                    ,[PSM_Status] as  PsmStatus
                    ,[Seq_No] as  SeqNo 
                    ,[Plant] as  Plant
                    ,[Material_No]  as  MaterialNo
                    ,[Mat_Code] as  MatCode
                    ,[Plan_Code] as  PlanCode
                    ,[Machine] as  Machine
                    ,[Alternative1] as Alternative1
                    ,[Alternative2]  as Alternative2
                    ,[Std_Process] as  StdProcess
                    ,[Speed] as  Speed
                    ,[Colour_Count] as  ColourCount
                    ,[MC_Move] as  McMove
                    ,[HandHold] as  HandHold
                    ,[Plate_No] as  PlateNo
                    ,[Myla_No] as  MylaNo
                    ,[Paper_Width] as  PaperWidth
                    ,[Cut_No] as  CutNo 
                    ,[Trim] as  Trim
                    ,[PercenTrim] as  PercenTrim
                    ,[Waste_Leg] as  WasteLeg
                    ,[Waste_Wid] as  WasteWid
                    ,[Sheet_in_Leg] as  SheetInLeg
                    ,[Sheet_in_Wid] as  SheetInWid
                    ,[Sheet_out_Leg] as  SheetOutLeg
                    ,[Sheet_out_Wid] as  SheetOutWid
                    ,[Weight_in] as WeightIn
                    ,[Weight_out] as  WeightOut
                    ,[No_Open_in] as NoOpenIn
                    ,[No_Open_out] as  NoOpenOut
                    ,[Color1] as Color1
                    ,[Shade1] as Shade1
                    ,[Color2] as  Color2
                    ,[Shade2] as Shade2
                    ,[Color3] as  Color3
                    ,[Shade3] as Shade3
                    ,[Color4] 
                    ,[Shade4]
                    ,[Color5]
                    ,[Shade5]
                    ,[Color6]
                    ,[Shade6]
                    ,[Color7]
                    ,[Shade7]
                    ,[Color_Area1] as  ColorArea1
                    ,[Color_Area2] as ColorArea2
                    ,[Color_Area3] as ColorArea3
                    ,[Color_Area4] as ColorArea4
                    ,[Color_Area5] as ColorArea5
                    ,[Color_Area6] as ColorArea6
                    ,[Color_Area7] as ColorArea7
                    ,[Platen]
                    ,[Rotary]
                    ,[TearTape]
                    ,[None_Blk] as  NoneBlk
                    ,[Stan_Blk] as  StanBlk
                    ,[Semi_Blk]  as SemiBlk
                    ,[Ship_Blk] as  ShipBlk
                    ,[Block_No] as  BlockNo
                    ,[Join_Mat_no] as  JoinMatNo
                    ,[Separat_Mat_no] as  SeparatMatNo
                    ,[Remark_Inprocess] as  RemarkInprocess
                    ,[Hardship]  as Hardship
                    ,[PDIS_Status]  as  PdisStatus
                    ,[Tran_Status]  as  TranStatus
                    ,[SAP_Status]  as  SapStatus
                    ,[Alternative3]
                    ,[Alternative4]
                    ,[Alternative5]
                    ,[Alternative6]
                    ,[Alternative7]
                    ,[Alternative8]
                    ,[Rotate_In] as RotateIn
                    ,[Rotate_Out] as  RotateOut
                    ,[Stack_Height] as StackHeight
                    ,[Setup_tm] as  SetupTm
                    ,[Setup_waste] as  SetupWaste
                    ,[Prepare_tm] as PrepareTm
                    ,[Post_tm] as  PostTm
                    ,[Run_waste] as  RunWaste
                    ,[Human]
                    ,[Color_count] as ColorCount
                    ,[UnUpgrad_Board] as UnUpgradBoard
                    ,[rout_barcode] as CustBarcodeNo
                    from " + presaleLinkIp + "StagingPresale.dbo." + plantName + "_Routing WHERE PSM_ID = '" + presale.PSM_ID + "' and PSM_Status = '1' AND PDIS_Status != 'X' ";
                #endregion

                using (IDbConnection db = new SqlConnection(configuration.GetConnectionString("PresaleConnect")))
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    //Execute sql query                       
                    result.presaleMasterData = db.Query<PresaleMasterData>(sQueryMaster).FirstOrDefault();// _presaleContext.PresaleMasterData.FromSql(sQueryMaster).FirstOrDefault();
                    master.TransactionsDetail = db.Query<TransactionsDetail>(sQueryTransactionDetail).FirstOrDefault();
                    if (result != null)
                    {
                        result.presaleRoutingModels = db.Query<PresaleRouting>(sQueryRouting).ToList();//_presaleContext.PresaleRouting.FromSql(sQueryRouting).AsNoTracking().ToList();

                        //Nut 20Sep21
                        //if (result.presaleMasterData != null)
                        //{
                        //    if (!string.IsNullOrEmpty(result.presaleMasterData.Code))
                        //    {
                        //        var boardCombine = JsonConvert.DeserializeObject<BoardCombine>(_boardCombineAPIRepository.GetBoardByCode(_factoryCode, result.presaleMasterData.Code, _token));
                        //        if (boardCombine == null)
                        //        {
                        //            var boardx = JsonConvert.DeserializeObject<BoardCombine>(_boardCombineAPIRepository.GetBoardByBoard(_factoryCode, result.presaleMasterData.Board, result.presaleMasterData.Flute, _token));
                        //            if (boardx == null)
                        //            {

                        //            }
                        //            else
                        //            {

                        //            }
                        //            result.presaleMasterData.Code = JsonConvert.DeserializeObject<BoardCombine>(_boardCombineAPIRepository.GetBoardByBoard(_factoryCode, result.presaleMasterData.Board, result.presaleMasterData.Flute, _token)).Code;
                        //        }
                        //    }
                        //}
                    }
                }

                foreach (var presaleRoutingModel in result.presaleRoutingModels)
                {
                    master.Routings.Add(mapper.Map<PresaleRouting, Routing>(presaleRoutingModel));
                }
                //waiting for code
                master.MasterData = mapper.Map<PresaleMasterData, MasterData>(result.presaleMasterData);
                master.PresaleMasterData = result.presaleMasterData;
            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }
            return master;
        }

        public bool CheckImportPresale(string board, string flute)
        {
            if (string.IsNullOrEmpty(board))
            {
                return true;
            }

            var boardCombine = JsonConvert.DeserializeObject<BoardCombine>(_boardCombineAPIRepository.GetBoardByBoard(_factoryCode, board, flute, _token));
            if (boardCombine == null)
            {
                return false;
            }
            return true;
        }

        public async void UpdatePresale(string pSM_Id, string materialNo)
        {
            string plantName = PresaleTools.GetPlantShortName(_factoryCode);
            string sqlcon = configuration.GetConnectionString("PresaleConnect");

            if (!sqlcon.Contains(presaleConnectionStr))
            {
                presaleLinkIp = string.Empty;
            }

            var sQueryUpdateMaster = $"Update {presaleLinkIp}StagingPresale.dbo.{plantName}_MasterData SET PSM_Status = '3', Material_No = '{materialNo}' WHERE PSM_ID = '{pSM_Id}'";
            var sQueryUpdateRouting = $"Update {presaleLinkIp}StagingPresale.dbo.{plantName}_Routing SET PSM_Status = '3', Material_No = '{materialNo}' WHERE PSM_ID = '{pSM_Id}'";


            using (IDbConnection db = new SqlConnection(configuration.GetConnectionString("PresaleConnect")))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                //Execute sql query             
                await db.ExecuteAsync(sQueryUpdateMaster);
                await db.ExecuteAsync(sQueryUpdateRouting);
            }


        }
        public async void RejectPresale(string pSM_Id)
        {
            string plantName = PresaleTools.GetPlantShortName(_factoryCode);
            string sqlcon = configuration.GetConnectionString("PresaleConnect");

            if (!sqlcon.Contains(presaleConnectionStr))
            {
                presaleLinkIp = string.Empty;
            }

            var sQueryUpdateMaster = $"Update {presaleLinkIp}StagingPresale.dbo.{plantName}_MasterData SET PSM_Status = '4' WHERE PSM_ID = '{pSM_Id}'";
            var sQueryUpdateRouting = $"Update {presaleLinkIp}StagingPresale.dbo.{plantName}_Routing SET PSM_Status = '4' WHERE PSM_ID = '{pSM_Id}'";


            this.SentToMasterCardPresale("4", pSM_Id);
            using (IDbConnection db = new SqlConnection(configuration.GetConnectionString("PresaleConnect")))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                //Execute sql query             
                await db.ExecuteAsync(sQueryUpdateMaster);
                await db.ExecuteAsync(sQueryUpdateRouting);
            }
        }
        public async void ApprovePresale(string pSM_Id)
        {
            string plantName = PresaleTools.GetPlantShortName(_factoryCode);
            string sqlcon = configuration.GetConnectionString("PresaleConnect");

            if (!sqlcon.Contains(presaleConnectionStr))
            {
                presaleLinkIp = string.Empty;
            }

            var sQueryUpdateMaster = $"Update {presaleLinkIp}StagingPresale.dbo.{plantName}_MasterData SET PSM_Status = '2' WHERE PSM_ID = '{pSM_Id}'";
            var sQueryUpdateRouting = $"Update {presaleLinkIp}StagingPresale.dbo.{plantName}_Routing SET PSM_Status = '2' WHERE PSM_ID = '{pSM_Id}'";


            this.SentToMasterCardPresale("2", pSM_Id);
            using (IDbConnection db = new SqlConnection(configuration.GetConnectionString("PresaleConnect")))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                //Execute sql query             
                await db.ExecuteAsync(sQueryUpdateMaster);
                await db.ExecuteAsync(sQueryUpdateRouting);
            }
        }
        public async void DeletePresale(string pSM_Id)
        {
            string plantName = PresaleTools.GetPlantShortName(_factoryCode);
            string sqlcon = configuration.GetConnectionString("PresaleConnect");

            if (!sqlcon.Contains(presaleConnectionStr))
            {
                presaleLinkIp = string.Empty;
            }

            var sQueryUpdateMaster = $"Update {presaleLinkIp}StagingPresale.dbo.{plantName}_MasterData SET PDIS_Status = 'X' WHERE PSM_ID = '{pSM_Id}'";
            var sQueryUpdateRouting = $"Update {presaleLinkIp}StagingPresale.dbo.{plantName}_Routing SET PDIS_Status = 'X' WHERE PSM_ID = '{pSM_Id}'";


            using (IDbConnection db = new SqlConnection(configuration.GetConnectionString("PresaleConnect")))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                //Execute sql query             
                await db.ExecuteAsync(sQueryUpdateMaster);
                await db.ExecuteAsync(sQueryUpdateRouting);
            }
        }

        public async void ApprovePresaleChangeSameMat(string pSM_Id, int id)
        {
            //update presale change product status
            presaleChangeProductAPIRepository.UpdatePresaleChangeProductStatusById(_factoryCode, id, "2", _token);

            this.SentToMasterCardPresale("2", pSM_Id);
        }
        public async void RejectPresaleChangeSameMat(string pSM_Id, int id)
        {
            //update presale change product status
            presaleChangeProductAPIRepository.UpdatePresaleChangeProductStatusById(_factoryCode, id, "4", _token);
            this.SentToMasterCardPresale("4", pSM_Id);
        }
        public void GetPresaleChangeProduct(ref PresaleChangeViewModel presaleChangeViewModel, string materialNo)
        {
            presaleChangeViewModel.PresaleChangeProducts = new List<PresaleChangeProduct>();
            //get presale change product 
            if (string.IsNullOrEmpty(materialNo))
            {
                presaleChangeViewModel.PresaleChangeProducts = JsonConvert.DeserializeObject<List<PresaleChangeProduct>>(presaleChangeProductAPIRepository.GetPresaleChangeProductsByActiveStatus(_factoryCode, _token));
            }
            else
            {
                presaleChangeViewModel.PresaleChangeProducts = JsonConvert.DeserializeObject<List<PresaleChangeProduct>>(presaleChangeProductAPIRepository.GetPresaleChangeProductsByMaterialNo(_factoryCode, materialNo, _token));
            }
        }

        public bool CheckExistMasterData(string facrotyCode, string materialNo)
        {
            MasterData masterData = new MasterData();
            masterData = JsonConvert.DeserializeObject<MasterData>(masterDataAPIRepository.GetMasterDataByMaterialNo(facrotyCode, materialNo, _token));
            return masterData != null ? true : false;
        }

        public void GetCompareProduct(ref PresaleChangeViewModel presaleChangeViewModel, string materialNo, string psmId)
        {
            presaleChangeViewModel.PresaleChangeProduct = new PresaleChangeProduct();
            presaleChangeViewModel.MasterDataCompare = new PresaleChangeProduct();
            presaleChangeViewModel.RoutingsCompare = new List<PresaleChangeRouting>();
            presaleChangeViewModel.PresaleChangeRoutings = new List<PresaleChangeRouting>();
            presaleChangeViewModel.MaterialNo = materialNo;

            #region Get Compare Master Data
            var existMasterData = JsonConvert.DeserializeObject<MasterData>(masterDataAPIRepository.GetMasterDataByMaterialNo(_factoryCode, materialNo, _token));

            presaleChangeViewModel.MasterDataCompare = mapper.Map<MasterData, PresaleChangeProduct>(existMasterData);
            presaleChangeViewModel.PresaleChangeProduct = JsonConvert.DeserializeObject<PresaleChangeProduct>(presaleChangeProductAPIRepository.GetPresaleChangeProductByMaterialNo(_factoryCode, materialNo, psmId, _token));
            #endregion

            #region Compare Routing
            var existRoutings = JsonConvert.DeserializeObject<List<Routing>>(routingAPIRepository.GetRoutingsByMaterialNo(_factoryCode, materialNo, _token));

            if (existRoutings.Count > 0)
            {
                existRoutings = existRoutings.OrderBy(r => r.SeqNo).ToList();
            }

            foreach (var existRouting in existRoutings)
            {
                var routingPresale = mapper.Map<Routing, PresaleChangeRouting>(existRouting);
                presaleChangeViewModel.RoutingsCompare.Add(routingPresale);
            }

            var changeRoutings = JsonConvert.DeserializeObject<List<PresaleChangeRouting>>(presaleChangeRoutingAPIRepository.GetPresaleChangeRoutingsByMaterialNo(_factoryCode, materialNo, _token));
            if (changeRoutings.Count > 0)
            {
                presaleChangeViewModel.PresaleChangeRoutings = changeRoutings.Where(r => r.PsmId == psmId).OrderBy(r => r.SeqNo).ToList();
            }
            #endregion

            SessionExtentions.SetSession(_httpContextAccessor.HttpContext.Session, "CompareProduct", presaleChangeViewModel);
        }

        public void ChangeProductPresaleToMaster(string materialNo, string psmId)
        {
            var newRoutings = new List<Routing>();

            var compareProduct = SessionExtentions.GetSession<PresaleChangeViewModel>(_httpContextAccessor.HttpContext.Session, "CompareProduct");

            if (compareProduct != null && !string.IsNullOrEmpty(compareProduct.MaterialNo))
            {
                if (compareProduct.MaterialNo == materialNo && compareProduct.MasterDataCompare.MaterialNo.Equals(materialNo) && compareProduct.MasterDataCompare.FactoryCode.Equals(_factoryCode)
                    && compareProduct.PresaleChangeProduct.MaterialNo.Equals(materialNo) && compareProduct.PresaleChangeProduct.FactoryCode.Equals(_factoryCode))
                {
                    #region Change Masterdata From Master Presale
                    var masterdata = JsonConvert.DeserializeObject<MasterData>(masterDataAPIRepository.GetMasterDataByMaterialNo(_factoryCode, materialNo, _token));
                    if (masterdata != null)
                    {
                        masterdata.PsmId = compareProduct.PresaleChangeProduct.PsmId;
                        masterdata.MaterialNo = compareProduct.PresaleChangeProduct.MaterialNo;
                        masterdata.Description = compareProduct.PresaleChangeProduct.Description;
                        masterdata.SaleText1 = compareProduct.PresaleChangeProduct.SaleText1;
                        masterdata.SaleText2 = compareProduct.PresaleChangeProduct.SaleText2;
                        masterdata.SaleText3 = compareProduct.PresaleChangeProduct.SaleText3;
                        masterdata.SaleText4 = compareProduct.PresaleChangeProduct.SaleText4;
                        masterdata.PieceSet = compareProduct.PresaleChangeProduct.PieceSet;
                        masterdata.PrintMethod = compareProduct.PresaleChangeProduct.PrintMethod;
                        masterdata.HighGroup = compareProduct.PresaleChangeProduct.HighGroup;
                        masterdata.HighValue = compareProduct.PresaleChangeProduct.HighValue;
                        masterdata.Bun = compareProduct.PresaleChangeProduct.Bun;
                        masterdata.BunLayer = compareProduct.PresaleChangeProduct.BunLayer;
                        masterdata.LayerPalet = compareProduct.PresaleChangeProduct.LayerPalet;
                        masterdata.TranStatus = false;
                        masterdata.UpdatedBy = _username;
                        masterdata.LastUpdate = DateTime.Now;
                        masterdata.User = _username;

                        var parentModelMasterData = new ParentModel()
                        {
                            AppName = Globals.AppNameEncrypt,
                            SaleOrg = _saleOrg,
                            PlantCode = _factoryCode,
                            MasterData = masterdata
                        };

                        masterDataAPIRepository.UpdateMasterData(JsonConvert.SerializeObject(parentModelMasterData), _token);
                    }

                    #endregion

                    #region Change Routing data From Routing Presale
                    newRoutings.Clear();
                    var routings = JsonConvert.DeserializeObject<List<Routing>>(routingAPIRepository.GetRoutingsByMaterialNo(_factoryCode, materialNo, _token));

                    if (routings.Count > 0)
                    {
                        routings = routings.OrderBy(r => r.SeqNo).ToList();
                    }

                    foreach (var routingsPresale in compareProduct.PresaleChangeRoutings)
                    {
                        var seqNo = routingsPresale.SeqNo;
                        var tempRouting = routings.FirstOrDefault(r => r.SeqNo == seqNo);

                        if (tempRouting != null)
                        {
                            tempRouting.MatCode = routingsPresale.MatCode;
                            tempRouting.PlanCode = routingsPresale.PlanCode;
                            tempRouting.Machine = routingsPresale.Machine;
                            tempRouting.Color1 = routingsPresale.Color1;
                            tempRouting.Shade1 = routingsPresale.Shade1;
                            tempRouting.Color2 = routingsPresale.Color2;
                            tempRouting.Shade2 = routingsPresale.Shade2;
                            tempRouting.Color3 = routingsPresale.Color3;
                            tempRouting.Shade3 = routingsPresale.Shade3;
                            tempRouting.Color4 = routingsPresale.Color4;
                            tempRouting.Shade4 = routingsPresale.Shade4;
                            tempRouting.Color5 = routingsPresale.Color5;
                            tempRouting.Shade5 = routingsPresale.Shade5;
                            tempRouting.Color6 = routingsPresale.Color6;
                            tempRouting.Shade6 = routingsPresale.Shade6;
                            tempRouting.Color7 = routingsPresale.Color7;
                            tempRouting.Shade7 = routingsPresale.Shade7;
                            newRoutings.Add(tempRouting);
                        }
                    }

                    if (newRoutings.Count > 0)
                    {
                        //update routings
                        routingAPIRepository.UpdateRoutings(_factoryCode, JsonConvert.SerializeObject(newRoutings), _token);
                    }

                    #endregion

                    #region Update Approve Change Presale
                    var changeMasterPresale = JsonConvert.DeserializeObject<PresaleChangeProduct>(presaleChangeProductAPIRepository.GetPresaleChangeProductByMaterialNo(_factoryCode, materialNo, psmId, _token));
                    if (changeMasterPresale != null)
                    {
                        changeMasterPresale.IsApprove = true;
                        presaleChangeProductAPIRepository.UpdatePresaleChangeProduct(_factoryCode, JsonConvert.SerializeObject(changeMasterPresale), _token);

                        var changeRoutingPresale = JsonConvert.DeserializeObject<List<PresaleChangeRouting>>(presaleChangeRoutingAPIRepository.GetPresaleChangeRoutingsByMaterialNo(_factoryCode, materialNo, _token));
                        if (changeRoutingPresale.Count > 0)
                        {
                            changeRoutingPresale.ForEach(c => c.IsApprove = true);
                            presaleChangeRoutingAPIRepository.UpdatePresaleRoutings(_factoryCode, JsonConvert.SerializeObject(changeRoutingPresale), _token);
                        }
                    }

                    #endregion

                    //sent status to presale
                    UpdateStatusSameMatFromPmt(masterdata.PdisStatus, changeMasterPresale.MaterialNo);
                }
            }
        }

        public void GetPresaleChangeProductByKeySearch(ref PresaleChangeViewModel presaleChangeViewModel, string typeSearch, string keySearch)
        {
            presaleChangeViewModel.PresaleChangeProducts = new List<PresaleChangeProduct>();
            presaleChangeViewModel.PresaleChangeProducts = JsonConvert.DeserializeObject<List<PresaleChangeProduct>>(presaleChangeProductAPIRepository.GetPresaleChangeProductsByKeySearch(_factoryCode, typeSearch, keySearch, _token));
        }

        public List<PresaleChangeProduct> GetPresaleChangeProductByMaterialNo(string keySearch)
        {
            var presaleChangeProducts = new List<PresaleChangeProduct>();
            presaleChangeProducts = JsonConvert.DeserializeObject<List<PresaleChangeProduct>>(presaleChangeProductAPIRepository.GetPresaleChangeProductsByKeySearch(_factoryCode, "Material No", keySearch, _token));
            return presaleChangeProducts;
        }

        public List<PresaleViewModel> SearchChangeProductNewMat(IConfiguration configuration, PresaleViewModel presale)
        {
            //return PresaleRepository.GetPresaleBySearch(configuration, param);
            string plantName = PresaleTools.GetPlantShortName(_factoryCode);

            string condition = "";
            condition += (String.IsNullOrEmpty(presale.Param.ddlSearch1)) ? "" : presale.Param.ddlSearch1 + " like '%" + presale.Param.txtSearch1 + "%' and ";
            condition += (String.IsNullOrEmpty(presale.Param.ddlSearch2)) ? "" : presale.Param.ddlSearch2 + " like '%" + presale.Param.txtSearch2 + "%' and ";
            condition += (String.IsNullOrEmpty(presale.Param.ddlSearch3)) ? "" : presale.Param.ddlSearch3 + " like '%" + presale.Param.txtSearch3 + "%' and ";
            condition += (String.IsNullOrEmpty(presale.Param.ddlSearch4)) ? "" : presale.Param.ddlSearch4 + " like '%" + presale.Param.txtSearch4 + "%' and ";

            List<PresaleViewModel> presaleViewModels = new List<PresaleViewModel>();
            var masterDatas = new List<PresaleMasterData>();
            string sqlcon = configuration.GetConnectionString("PresaleConnect");
            if (!sqlcon.Contains(presaleConnectionStr))
                presaleLinkIp = string.Empty;

            var sQueryMaster = @"SELECT 
                [PSM_ID] as PsmId
                ,[PSM_Status] as PsmStatus
                ,[Material_No] as MaterialNo
                ,[Part_No] as PartNo
                ,[PC] as Pc
                ,[Hierarchy] as Hierarchy
                ,[Sale_Org] as SaleOrg
                ,[Plant] as Plant
                ,[Cust_Code] as CustCode
                ,[Cus_ID] as CusId
                ,[Cust_Name] as CustName
                ,[Description] as Description
                ,[Sale_Text1] as SaleText1
                ,[Sale_Text2] as SaleText2
                ,[Sale_Text3] as SaleText3
                ,[Sale_Text4] as SaleText4
                ,[Change] as Change
                ,[Language] as Language
                ,[Ind_Grp] as IndGrp
                ,[Ind_Des] as IndDes
                ,[Material_Type] as MaterialType
                ,[Print_Method] as PrintMethod
                ,[TwoPiece] as TwoPiece
                ,[Flute] as Flute
                ,[Code] as Code
                ,[Board] as Board
                ,[GL] as Gl
                ,[GLWeigth]  as  Glweigth
                ,[BM] as  Bm
                ,[BMWeigth] as Bmweigth
                ,[BL] as Bl
                ,[BLWeigth] as Blweigth
                ,[CM] as Cm
                ,[CMWeigth] as Cmweigth
                ,[CL] as Cl
                ,[CLWeigth] as Clweigth
                ,[DM]  as Dm
                ,[DMWeigth] as Dmweigth
                ,[DL] as Dl
                ,[DLWeigth] as Dlweigth
                ,[Wid] as Wid
                ,[Leg] as Leg
                ,[Hig] as Hig
                ,[Box_Type] as BoxType
                ,[RSC_Style] as  RscStyle
                ,[Pro_Type] as  ProType
                ,[JoinType] as JoinType
                ,[Status_Flag] as StatusFlag
                ,[Priority_Flag] as PriorityFlag
                ,[Wire] as Wire
                ,[Outer_Join] as  OuterJoin
                ,[CutSheetLeng] as CutSheetLeng
                ,[CutSheetWid] as CutSheetWid
                ,[Sheet_Area] as SheetArea
                ,[Box_Area] as BoxArea
                ,[ScoreW1] as ScoreW1
                ,[Scorew2] as ScoreW2
                ,[Scorew3] as ScoreW3
                ,[Scorew4] as ScoreW4
                ,[Scorew5] as ScoreW5
                ,[Scorew6] as ScoreW6
                ,[Scorew7] as ScoreW7
                ,[Scorew8] as ScoreW8
                ,[Scorew9] as ScoreW9
                ,[Scorew10]  as ScoreW10
                ,[Scorew11]  as ScoreW11
                ,[Scorew12]  as ScoreW12
                ,[Scorew13]  as ScoreW13
                ,[Scorew14]  as ScoreW14
                ,[Scorew15]  as ScoreW15
                ,[Scorew16]  as ScoreW16
                ,[JointLap] as JointLap 
                ,[ScoreL2] as ScoreL2
                ,[ScoreL3]  as ScoreL3
                ,[ScoreL4]  as ScoreL4
                ,[ScoreL5]  as ScoreL5
                ,[ScoreL6]  as ScoreL6
                ,[ScoreL7]  as ScoreL7
                ,[ScoreL8]  as ScoreL8
                ,[ScoreL9]  as ScoreL9
                ,[Slit] as Slit
                ,[No_Slot] as NoSlot
                ,[Bun] as Bun
                ,[BunLayer] as BunLayer
                ,[LayerPalet] as  LayerPalet
                ,[BoxPalet] as BoxPalet
                ,[Weight_Sh] as WeightSh
                ,[Weight_Box] as WeightBox
                ,[SparePercen] as  SparePercen
                ,[SpareMax] as SpareMax
                ,[SpareMin] as  SpareMin 
                ,[LeadTime] as LeadTime
                ,[Piece_Set] as PieceSet
                ,[Sale_UOM] as  SaleUom
                ,[BOM_UOM] as BomUom
                ,[Hardship]  as Hardship
                ,[PalletSize] as  PalletSize
                ,[Palletization_Path] as  PalletizationPath
                ,[PrintMaster_Path] as PrintMasterPath
                ,[DiecutPict_Path] as DiecutPictPath
                ,[CreateDate] 
                ,[LastUpdate] 
                ,[User]
                ,[Plt_Leg_Double] as PltLegDouble
                ,[Plt_Double_axle] as  PltDoubleAxle
                ,[Plt_Leg_Single] as  PltLegSingle
                ,[Plt_Single_axle] as PltSingleAxle
                ,[Plt_Floor_Above] as PltFloorAbove
                ,[Plt_Floor_Under] as PltFloorUnder
                ,[Plt_Beam] as PltBeam
                ,[Plt_Axle_Height] as  PltAxleHeight
                ,[EanCode] as  EanCode
                ,[PDIS_Status] as  PdisStatus
                ,[Tran_Status] as  TranStatus
                ,[SAP_Status] as  SapStatus
                ,[NewH] as NewH
                ,[Pur_Txt1] as PurTxt1
                ,[Pur_Txt2]  as PurTxt2
                ,[Pur_Txt3]  as PurTxt3
                ,[Pur_Txt4]  as PurTxt4
                ,[UnUpgrad_Board] as  UnUpgradBoard
                ,[High_Group] as  HighGroup
                ,[High_Value] as HighValue
                ,[PS_AW_file] as PsAwfile
                ,[PS_DW_file] as PsDwFile
                ,[PS_LP_file] as PsLpFile
                ,[PS_PO_file] as PsPoFile
                ,[PS_QT_file] as PsQtFile
                ,[PS_Other_file] as PsOtherFile
                from " + presaleLinkIp + "StagingPresale.dbo." + plantName + "_MasterData WHERE " + condition + "PSM_Status = '1' AND PDIS_Status != 'X' AND FLAG_CHANGE = 'Y'";


            using (IDbConnection db = new SqlConnection(configuration.GetConnectionString("PresaleConnect")))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                //Execute sql query
                masterDatas = db.Query<PresaleMasterData>(sQueryMaster).ToList();
            }

            //var masterDatas = _presaleContext.PresaleMasterData.FromSql(sQueryMaster).AsNoTracking().ToList();
            foreach (var masterData in masterDatas)
            {
                presaleViewModels.Add(new PresaleViewModel
                {
                    presaleMasterData = masterData,
                    PSM_ID = masterData.PsmId,
                    PSM_Status = masterData.PsmStatus,
                    Material_No = masterData.MaterialNo,
                    PC = masterData.Pc,
                    Description = masterData.Description,
                    User = masterData.User,
                    CreateDate = masterData.CreateDate.Value,
                    Cust_Name = masterData.CustName,
                    LastUpdate = masterData.LastUpdate.Value,
                    flute = masterData.Flute,
                    Plant = _factoryCode,
                    Board = masterData.Board,
                    PsAwfile = masterData.PsAwfile,
                    PsDwFile = masterData.PsDwFile,
                    PsLpFile = masterData.PsLpFile,
                    PsQtFile = masterData.PsQtFile,
                    PsPoFile = masterData.PsPoFile,
                    PsOtherFile =
                        !string.IsNullOrEmpty(masterData.PsOtherFile)
                        ? JsonConvert.DeserializeObject<OtherFiles>(masterData.PsOtherFile)
                        : new OtherFiles { files = new List<Files>() },
                });
            }

            return presaleViewModels;
        }
        public void SentToMasterCardPresale(string status, string psmId)
        {
            var url = configuration.GetConnectionString("PresaleAPI") + "SentToMasterCard/UpdateFromPmt";
            var data = new PresaleRejectMaterialModel
            {
                itemCode = psmId,
                psmStatus = status,
                userName = _username
            };
            JsonExtentions.PresaleApiConnect(url, JsonConvert.SerializeObject(data));
        }
        public void UpdateStatusSameMatFromPmt(string pmtStatus, string matNO)
        {
            var url = configuration.GetConnectionString("PresaleAPI") + "ChangeProduct/UpdateStatusSameMatFromPmt";
            var data = new PresaleStatusSameMatViewModel
            {
                matNo = matNO,
                pmtStatus = pmtStatus,
                userName = _username
            };
            JsonExtentions.PresaleApiConnect(url, JsonConvert.SerializeObject(data));
        }
        public void UpdateUnHoldToPresale(string materialNo)
        {
            var url = configuration.GetConnectionString("PresaleAPI") + "ChangeProduct/UpdateUnHoldFromPmts";
            var data = new PresaleUpdateUnHoldViewModel
            {
                materialNo = materialNo,
                userName = _username
            };
            JsonExtentions.PresaleApiConnect(url, JsonConvert.SerializeObject(data));

        }
    }
}
