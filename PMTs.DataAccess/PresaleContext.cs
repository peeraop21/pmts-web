using Microsoft.EntityFrameworkCore;

namespace PMTs.DataAccess.ModelPresale
{
    public partial class PresaleContext : DbContext
    {
        public PresaleContext()
        {
        }

        public PresaleContext(DbContextOptions<PresaleContext> options)
            : base(options)
        {
        }

        //public virtual DbSet<NaiMasterData> NaiMasterData { get; set; }
        //public virtual DbSet<NaiRouting> NaiRouting { get; set; }      
        //public virtual DbSet<TckkMasterData> TckkMasterData { get; set; }
        //public virtual DbSet<TckkRouting> TckkRouting { get; set; }
        //public virtual DbSet<TckpMasterData> TckpMasterData { get; set; }
        //public virtual DbSet<TckpRouting> TckpRouting { get; set; }
        //public virtual DbSet<TcnkMasterData> TcnkMasterData { get; set; }
        //public virtual DbSet<TcnkRouting> TcnkRouting { get; set; }
        //public virtual DbSet<TcpbMasterData> TcpbMasterData { get; set; }
        //public virtual DbSet<TcpbRouting> TcpbRouting { get; set; }
        //public virtual DbSet<TcptMasterData> TcptMasterData { get; set; }
        //public virtual DbSet<TcptRouting> TcptRouting { get; set; }
        //public virtual DbSet<TcrbMasterData> TcrbMasterData { get; set; }
        //public virtual DbSet<TcrbRouting> TcrbRouting { get; set; }
        //public virtual DbSet<TcryMasterData> TcryMasterData { get; set; }
        //public virtual DbSet<TcryRouting> TcryRouting { get; set; }
        //public virtual DbSet<TcsbMasterData> TcsbMasterData { get; set; }
        //public virtual DbSet<TcsbRouting> TcsbRouting { get; set; }
        //public virtual DbSet<TcsMasterData> TcsMasterData { get; set; }
        //public virtual DbSet<TcspMasterData> TcspMasterData { get; set; }
        //public virtual DbSet<TcspRouting> TcspRouting { get; set; }
        //public virtual DbSet<TcsRouting> TcsRouting { get; set; }

        //public virtual DbSet<PresaleMasterData> PresaleMasterData { get; set; }


        public virtual DbSet<PresaleMasterData> PresaleMasterData { get; set; }
        public virtual DbSet<PresaleRouting> PresaleRouting { get; set; }




        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                // optionsBuilder.UseSqlServer("Server=10.28.58.90;Database=StagingPresale;User Id=sa;Password=!qaz2wsx;MultipleActiveResultSets=True");
                optionsBuilder.UseSqlServer("Server=172.18.8.155;Database=StagingPresale;User Id=psm;Password=psmadmin;MultipleActiveResultSets=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<PresaleMasterData>(entity =>
            {
                entity.HasKey(e => e.PsmId);

                //entity.ToTable("TCCB_MasterData");

                entity.Property(e => e.PsmId)
                    .HasColumnName("PSM_ID")
                    .HasMaxLength(50)
                    .ValueGeneratedNever();

                entity.Property(e => e.Bl)
                    .HasColumnName("BL")
                    .HasMaxLength(3);

                entity.Property(e => e.Blweigth).HasColumnName("BLWeigth");

                entity.Property(e => e.Bm)
                    .HasColumnName("BM")
                    .HasMaxLength(3);

                entity.Property(e => e.Bmweigth).HasColumnName("BMWeigth");

                entity.Property(e => e.Board).HasMaxLength(50);

                entity.Property(e => e.BomUom)
                    .HasColumnName("BOM_UOM")
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.BoxArea).HasColumnName("Box_Area");

                entity.Property(e => e.BoxType)
                    .HasColumnName("Box_Type")
                    .HasMaxLength(60);

                entity.Property(e => e.Change).HasMaxLength(100);

                entity.Property(e => e.Cl)
                    .HasColumnName("CL")
                    .HasMaxLength(3);

                entity.Property(e => e.Clweigth).HasColumnName("CLWeigth");

                entity.Property(e => e.Cm)
                    .HasColumnName("CM")
                    .HasMaxLength(3);

                entity.Property(e => e.Cmweigth).HasColumnName("CMWeigth");

                entity.Property(e => e.Code).HasMaxLength(20);

                entity.Property(e => e.CreateDate).HasColumnType("smalldatetime");

                entity.Property(e => e.CusId)
                    .HasColumnName("Cus_ID")
                    .HasMaxLength(10);

                entity.Property(e => e.CustCode)
                    .HasColumnName("Cust_Code")
                    .HasMaxLength(10);

                entity.Property(e => e.CustName)
                    .HasColumnName("Cust_Name")
                    .HasMaxLength(50);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(40);

                entity.Property(e => e.DiecutPictPath)
                    .HasColumnName("DiecutPict_Path")
                    .HasMaxLength(50);

                entity.Property(e => e.Dl)
                    .HasColumnName("DL")
                    .HasMaxLength(3);

                entity.Property(e => e.Dlweigth).HasColumnName("DLWeigth");

                entity.Property(e => e.Dm)
                    .HasColumnName("DM")
                    .HasMaxLength(3);

                entity.Property(e => e.Dmweigth).HasColumnName("DMWeigth");

                entity.Property(e => e.EanCode).HasMaxLength(13);

                entity.Property(e => e.Flute).HasMaxLength(3);

                entity.Property(e => e.Gl)
                    .HasColumnName("GL")
                    .HasMaxLength(3);

                entity.Property(e => e.Glweigth).HasColumnName("GLWeigth");

                entity.Property(e => e.Hierarchy)
                    .IsRequired()
                    .HasMaxLength(18);

                entity.Property(e => e.HighGroup)
                    .HasColumnName("High_Group")
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.HighValue)
                    .HasColumnName("High_Value")
                    .HasMaxLength(4)
                    .IsUnicode(false);

                entity.Property(e => e.IndDes)
                    .HasColumnName("Ind_Des")
                    .HasMaxLength(50);

                entity.Property(e => e.IndGrp)
                    .HasColumnName("Ind_Grp")
                    .HasMaxLength(3);

                entity.Property(e => e.JoinType).HasMaxLength(60);

                entity.Property(e => e.Language)
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.LastUpdate).HasColumnType("smalldatetime");

                entity.Property(e => e.MaterialNo)
                    .IsRequired()
                    .HasColumnName("Material_No")
                    .HasMaxLength(18);

                entity.Property(e => e.MaterialType)
                    .HasColumnName("Material_Type")
                    .HasMaxLength(2);

                entity.Property(e => e.NoSlot).HasColumnName("No_Slot");

                entity.Property(e => e.OuterJoin).HasColumnName("Outer_Join");

                entity.Property(e => e.PalletSize).HasMaxLength(10);

                entity.Property(e => e.PalletizationPath)
                    .HasColumnName("Palletization_Path")
                    .HasMaxLength(50);

                entity.Property(e => e.PartNo)
                    .HasColumnName("Part_No")
                    .HasMaxLength(22);

                entity.Property(e => e.Pc)
                    .IsRequired()
                    .HasColumnName("PC")
                    .HasMaxLength(15);

                entity.Property(e => e.PdisStatus)
                    .IsRequired()
                    .HasColumnName("PDIS_Status")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.PieceSet).HasColumnName("Piece_Set");

                entity.Property(e => e.Plant).HasMaxLength(5);

                entity.Property(e => e.PltAxleHeight).HasColumnName("Plt_Axle_Height");

                entity.Property(e => e.PltBeam).HasColumnName("Plt_Beam");

                entity.Property(e => e.PltDoubleAxle).HasColumnName("Plt_Double_axle");

                entity.Property(e => e.PltFloorAbove).HasColumnName("Plt_Floor_Above");

                entity.Property(e => e.PltFloorUnder).HasColumnName("Plt_Floor_Under");

                entity.Property(e => e.PltLegDouble).HasColumnName("Plt_Leg_Double");

                entity.Property(e => e.PltLegSingle).HasColumnName("Plt_Leg_Single");

                entity.Property(e => e.PltSingleAxle).HasColumnName("Plt_Single_axle");

                entity.Property(e => e.PrintMasterPath)
                    .HasColumnName("PrintMaster_Path")
                    .HasMaxLength(50);

                entity.Property(e => e.PrintMethod)
                    .HasColumnName("Print_Method")
                    .HasMaxLength(50);

                entity.Property(e => e.PriorityFlag)
                    .HasColumnName("Priority_Flag")
                    .HasMaxLength(2);

                entity.Property(e => e.ProType)
                    .HasColumnName("Pro_Type")
                    .HasMaxLength(60);

                entity.Property(e => e.PsmStatus)
                    .HasColumnName("PSM_Status")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.PurTxt1)
                    .HasColumnName("Pur_Txt1")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.PurTxt2)
                    .HasColumnName("Pur_Txt2")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.PurTxt3)
                    .HasColumnName("Pur_Txt3")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.PurTxt4)
                    .HasColumnName("Pur_Txt4")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.RscStyle)
                    .HasColumnName("RSC_Style")
                    .HasMaxLength(50);

                entity.Property(e => e.SaleOrg)
                    .IsRequired()
                    .HasColumnName("Sale_Org")
                    .HasMaxLength(5);

                entity.Property(e => e.SaleText1)
                    .HasColumnName("Sale_Text1")
                    .HasMaxLength(40);

                entity.Property(e => e.SaleText2)
                    .HasColumnName("Sale_Text2")
                    .HasMaxLength(40);

                entity.Property(e => e.SaleText3)
                    .HasColumnName("Sale_Text3")
                    .HasMaxLength(40);

                entity.Property(e => e.SaleText4)
                    .HasColumnName("Sale_Text4")
                    .HasMaxLength(40);

                entity.Property(e => e.SaleUom)
                    .HasColumnName("Sale_UOM")
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.SapStatus).HasColumnName("SAP_Status");

                entity.Property(e => e.SheetArea).HasColumnName("Sheet_Area");

                entity.Property(e => e.StatusFlag)
                    .HasColumnName("Status_Flag")
                    .HasMaxLength(30);

                entity.Property(e => e.TranStatus).HasColumnName("Tran_Status");

                entity.Property(e => e.UnUpgradBoard).HasColumnName("UnUpgrad_Board");

                entity.Property(e => e.User)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.WeightBox).HasColumnName("Weight_Box");

                entity.Property(e => e.WeightSh).HasColumnName("Weight_Sh");
            });

            modelBuilder.Entity<PresaleRouting>(entity =>
            {
                entity.HasKey(e => new { e.PsmId, e.SeqNo });

                //entity.ToTable("TCCB_Routing");

                entity.Property(e => e.PsmId)
                    .HasColumnName("PSM_ID")
                    .HasMaxLength(50);

                entity.Property(e => e.SeqNo).HasColumnName("Seq_No");

                entity.Property(e => e.Alternative1)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Alternative2)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Alternative3).HasMaxLength(50);

                entity.Property(e => e.Alternative4).HasMaxLength(50);

                entity.Property(e => e.Alternative5).HasMaxLength(50);

                entity.Property(e => e.Alternative6).HasMaxLength(50);

                entity.Property(e => e.Alternative7).HasMaxLength(50);

                entity.Property(e => e.Alternative8).HasMaxLength(50);

                entity.Property(e => e.BlockNo)
                    .HasColumnName("Block_No")
                    .HasMaxLength(50);

                entity.Property(e => e.Color1).HasMaxLength(20);

                entity.Property(e => e.Color2).HasMaxLength(20);

                entity.Property(e => e.Color3).HasMaxLength(20);

                entity.Property(e => e.Color4).HasMaxLength(20);

                entity.Property(e => e.Color5).HasMaxLength(20);

                entity.Property(e => e.Color6).HasMaxLength(20);

                entity.Property(e => e.Color7).HasMaxLength(20);

                entity.Property(e => e.ColorArea1).HasColumnName("Color_Area1");

                entity.Property(e => e.ColorArea2).HasColumnName("Color_Area2");

                entity.Property(e => e.ColorArea3).HasColumnName("Color_Area3");

                entity.Property(e => e.ColorArea4).HasColumnName("Color_Area4");

                entity.Property(e => e.ColorArea5).HasColumnName("Color_Area5");

                entity.Property(e => e.ColorArea6).HasColumnName("Color_Area6");

                entity.Property(e => e.ColorArea7).HasColumnName("Color_Area7");

                entity.Property(e => e.ColorCount).HasColumnName("Color_count");

                entity.Property(e => e.ColourCount).HasColumnName("Colour_Count");

                entity.Property(e => e.CutNo).HasColumnName("Cut_No");

                entity.Property(e => e.JoinMatNo)
                    .HasColumnName("Join_Mat_no")
                    .HasMaxLength(18);

                entity.Property(e => e.Machine).HasMaxLength(50);

                entity.Property(e => e.MatCode)
                    .HasColumnName("Mat_Code")
                    .HasMaxLength(20);

                entity.Property(e => e.MaterialNo)
                    .IsRequired()
                    .HasColumnName("Material_No")
                    .HasMaxLength(18);

                entity.Property(e => e.McMove).HasColumnName("MC_Move");

                entity.Property(e => e.MylaNo)
                    .HasColumnName("Myla_No")
                    .HasMaxLength(50);

                entity.Property(e => e.NoOpenIn).HasColumnName("No_Open_in");

                entity.Property(e => e.NoOpenOut).HasColumnName("No_Open_out");

                entity.Property(e => e.NoneBlk).HasColumnName("None_Blk");

                entity.Property(e => e.PaperWidth).HasColumnName("Paper_Width");

                entity.Property(e => e.PdisStatus)
                    .HasColumnName("PDIS_Status")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.PlanCode)
                    .HasColumnName("Plan_Code")
                    .HasMaxLength(10);

                entity.Property(e => e.Plant).HasMaxLength(5);

                entity.Property(e => e.PlateNo)
                    .HasColumnName("Plate_No")
                    .HasMaxLength(50);

                entity.Property(e => e.PostTm).HasColumnName("Post_tm");

                entity.Property(e => e.PrepareTm).HasColumnName("Prepare_tm");

                entity.Property(e => e.PsmStatus)
                    .HasColumnName("PSM_Status")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.RemarkInprocess)
                    .HasColumnName("Remark_Inprocess")
                    .HasMaxLength(255);

                entity.Property(e => e.RotateIn).HasColumnName("Rotate_In");

                entity.Property(e => e.RotateOut).HasColumnName("Rotate_Out");

                entity.Property(e => e.RunWaste).HasColumnName("Run_waste");

                entity.Property(e => e.SapStatus).HasColumnName("SAP_Status");

                entity.Property(e => e.SemiBlk).HasColumnName("Semi_Blk");

                entity.Property(e => e.SeparatMatNo)
                    .HasColumnName("Separat_Mat_no")
                    .HasMaxLength(18);

                entity.Property(e => e.SetupTm).HasColumnName("Setup_tm");

                entity.Property(e => e.SetupWaste).HasColumnName("Setup_waste");

                entity.Property(e => e.Shade1).HasMaxLength(10);

                entity.Property(e => e.Shade2).HasMaxLength(10);

                entity.Property(e => e.Shade3).HasMaxLength(10);

                entity.Property(e => e.Shade4).HasMaxLength(10);

                entity.Property(e => e.Shade5).HasMaxLength(10);

                entity.Property(e => e.Shade6).HasMaxLength(10);

                entity.Property(e => e.Shade7).HasMaxLength(10);

                entity.Property(e => e.SheetInLeg).HasColumnName("Sheet_in_Leg");

                entity.Property(e => e.SheetInWid).HasColumnName("Sheet_in_Wid");

                entity.Property(e => e.SheetOutLeg).HasColumnName("Sheet_out_Leg");

                entity.Property(e => e.SheetOutWid).HasColumnName("Sheet_out_Wid");

                entity.Property(e => e.ShipBlk).HasColumnName("Ship_Blk");

                entity.Property(e => e.StackHeight).HasColumnName("Stack_Height");

                entity.Property(e => e.StanBlk).HasColumnName("Stan_Blk");

                entity.Property(e => e.StdProcess).HasColumnName("Std_Process");

                entity.Property(e => e.TranStatus).HasColumnName("Tran_Status");

                entity.Property(e => e.UnUpgradBoard)
                    .HasColumnName("UnUpgrad_Board")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.WasteLeg).HasColumnName("Waste_Leg");

                entity.Property(e => e.WasteWid).HasColumnName("Waste_Wid");

                entity.Property(e => e.WeightIn).HasColumnName("Weight_in");

                entity.Property(e => e.WeightOut).HasColumnName("Weight_out");
            });

            /*
            modelBuilder.Entity<NaiMasterData>(entity =>
            {
                entity.HasKey(e => e.PsmId);

                entity.ToTable("NAI_MasterData");

                entity.Property(e => e.PsmId)
                    .HasColumnName("PSM_ID")
                    .HasMaxLength(50)
                    .ValueGeneratedNever();

                entity.Property(e => e.Bl)
                    .HasColumnName("BL")
                    .HasMaxLength(3);

                entity.Property(e => e.Blweigth).HasColumnName("BLWeigth");

                entity.Property(e => e.Bm)
                    .HasColumnName("BM")
                    .HasMaxLength(3);

                entity.Property(e => e.Bmweigth).HasColumnName("BMWeigth");

                entity.Property(e => e.Board).HasMaxLength(50);

                entity.Property(e => e.BomUom)
                    .HasColumnName("BOM_UOM")
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.BoxArea).HasColumnName("Box_Area");

                entity.Property(e => e.BoxType)
                    .HasColumnName("Box_Type")
                    .HasMaxLength(60);

                entity.Property(e => e.Change).HasMaxLength(100);

                entity.Property(e => e.Cl)
                    .HasColumnName("CL")
                    .HasMaxLength(3);

                entity.Property(e => e.Clweigth).HasColumnName("CLWeigth");

                entity.Property(e => e.Cm)
                    .HasColumnName("CM")
                    .HasMaxLength(3);

                entity.Property(e => e.Cmweigth).HasColumnName("CMWeigth");

                entity.Property(e => e.Code).HasMaxLength(20);

                entity.Property(e => e.CreateDate).HasColumnType("smalldatetime");

                entity.Property(e => e.CusId)
                    .HasColumnName("Cus_ID")
                    .HasMaxLength(10);

                entity.Property(e => e.CustCode)
                    .HasColumnName("Cust_Code")
                    .HasMaxLength(10);

                entity.Property(e => e.CustName)
                    .HasColumnName("Cust_Name")
                    .HasMaxLength(50);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(40);

                entity.Property(e => e.DiecutPictPath)
                    .HasColumnName("DiecutPict_Path")
                    .HasMaxLength(50);

                entity.Property(e => e.Dl)
                    .HasColumnName("DL")
                    .HasMaxLength(3);

                entity.Property(e => e.Dlweigth).HasColumnName("DLWeigth");

                entity.Property(e => e.Dm)
                    .HasColumnName("DM")
                    .HasMaxLength(3);

                entity.Property(e => e.Dmweigth).HasColumnName("DMWeigth");

                entity.Property(e => e.EanCode).HasMaxLength(13);

                entity.Property(e => e.Flute).HasMaxLength(3);

                entity.Property(e => e.Gl)
                    .HasColumnName("GL")
                    .HasMaxLength(3);

                entity.Property(e => e.Glweigth).HasColumnName("GLWeigth");

                entity.Property(e => e.Hierarchy)
                    .IsRequired()
                    .HasMaxLength(18);

                entity.Property(e => e.HighGroup)
                    .HasColumnName("High_Group")
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.HighValue)
                    .HasColumnName("High_Value")
                    .HasMaxLength(4)
                    .IsUnicode(false);

                entity.Property(e => e.IndDes)
                    .HasColumnName("Ind_Des")
                    .HasMaxLength(50);

                entity.Property(e => e.IndGrp)
                    .HasColumnName("Ind_Grp")
                    .HasMaxLength(3);

                entity.Property(e => e.JoinType).HasMaxLength(60);

                entity.Property(e => e.Language)
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.LastUpdate).HasColumnType("smalldatetime");

                entity.Property(e => e.MaterialNo)
                    .IsRequired()
                    .HasColumnName("Material_No")
                    .HasMaxLength(18);

                entity.Property(e => e.MaterialType)
                    .HasColumnName("Material_Type")
                    .HasMaxLength(2);

                entity.Property(e => e.NoSlot).HasColumnName("No_Slot");

                entity.Property(e => e.OuterJoin).HasColumnName("Outer_Join");

                entity.Property(e => e.PalletSize).HasMaxLength(10);

                entity.Property(e => e.PalletizationPath)
                    .HasColumnName("Palletization_Path")
                    .HasMaxLength(50);

                entity.Property(e => e.PartNo)
                    .HasColumnName("Part_No")
                    .HasMaxLength(22);

                entity.Property(e => e.Pc)
                    .IsRequired()
                    .HasColumnName("PC")
                    .HasMaxLength(15);

                entity.Property(e => e.PdisStatus)
                    .IsRequired()
                    .HasColumnName("PDIS_Status")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.PieceSet).HasColumnName("Piece_Set");

                entity.Property(e => e.Plant).HasMaxLength(5);

                entity.Property(e => e.PltAxleHeight).HasColumnName("Plt_Axle_Height");

                entity.Property(e => e.PltBeam).HasColumnName("Plt_Beam");

                entity.Property(e => e.PltDoubleAxle).HasColumnName("Plt_Double_axle");

                entity.Property(e => e.PltFloorAbove).HasColumnName("Plt_Floor_Above");

                entity.Property(e => e.PltFloorUnder).HasColumnName("Plt_Floor_Under");

                entity.Property(e => e.PltLegDouble).HasColumnName("Plt_Leg_Double");

                entity.Property(e => e.PltLegSingle).HasColumnName("Plt_Leg_Single");

                entity.Property(e => e.PltSingleAxle).HasColumnName("Plt_Single_axle");

                entity.Property(e => e.PrintMasterPath)
                    .HasColumnName("PrintMaster_Path")
                    .HasMaxLength(50);

                entity.Property(e => e.PrintMethod)
                    .HasColumnName("Print_Method")
                    .HasMaxLength(50);

                entity.Property(e => e.PriorityFlag)
                    .HasColumnName("Priority_Flag")
                    .HasMaxLength(2);

                entity.Property(e => e.ProType)
                    .HasColumnName("Pro_Type")
                    .HasMaxLength(60);

                entity.Property(e => e.PsmStatus)
                    .HasColumnName("PSM_Status")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.PurTxt1)
                    .HasColumnName("Pur_Txt1")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.PurTxt2)
                    .HasColumnName("Pur_Txt2")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.PurTxt3)
                    .HasColumnName("Pur_Txt3")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.PurTxt4)
                    .HasColumnName("Pur_Txt4")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.RscStyle)
                    .HasColumnName("RSC_Style")
                    .HasMaxLength(50);

                entity.Property(e => e.SaleOrg)
                    .IsRequired()
                    .HasColumnName("Sale_Org")
                    .HasMaxLength(5);

                entity.Property(e => e.SaleText1)
                    .HasColumnName("Sale_Text1")
                    .HasMaxLength(40);

                entity.Property(e => e.SaleText2)
                    .HasColumnName("Sale_Text2")
                    .HasMaxLength(40);

                entity.Property(e => e.SaleText3)
                    .HasColumnName("Sale_Text3")
                    .HasMaxLength(40);

                entity.Property(e => e.SaleText4)
                    .HasColumnName("Sale_Text4")
                    .HasMaxLength(40);

                entity.Property(e => e.SaleUom)
                    .HasColumnName("Sale_UOM")
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.SapStatus).HasColumnName("SAP_Status");

                entity.Property(e => e.SheetArea).HasColumnName("Sheet_Area");

                entity.Property(e => e.StatusFlag)
                    .HasColumnName("Status_Flag")
                    .HasMaxLength(30);

                entity.Property(e => e.TranStatus).HasColumnName("Tran_Status");

                entity.Property(e => e.UnUpgradBoard).HasColumnName("UnUpgrad_Board");

                entity.Property(e => e.User)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.WeightBox).HasColumnName("Weight_Box");

                entity.Property(e => e.WeightSh).HasColumnName("Weight_Sh");
            });

            modelBuilder.Entity<NaiRouting>(entity =>
            {
                entity.HasKey(e => new { e.PsmId, e.SeqNo });

                entity.ToTable("NAI_Routing");

                entity.Property(e => e.PsmId)
                    .HasColumnName("PSM_ID")
                    .HasMaxLength(50);

                entity.Property(e => e.SeqNo).HasColumnName("Seq_No");

                entity.Property(e => e.Alternative1)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Alternative2)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Alternative3).HasMaxLength(50);

                entity.Property(e => e.Alternative4).HasMaxLength(50);

                entity.Property(e => e.Alternative5).HasMaxLength(50);

                entity.Property(e => e.Alternative6).HasMaxLength(50);

                entity.Property(e => e.Alternative7).HasMaxLength(50);

                entity.Property(e => e.Alternative8).HasMaxLength(50);

                entity.Property(e => e.BlockNo)
                    .HasColumnName("Block_No")
                    .HasMaxLength(50);

                entity.Property(e => e.Color1).HasMaxLength(20);

                entity.Property(e => e.Color2).HasMaxLength(20);

                entity.Property(e => e.Color3).HasMaxLength(20);

                entity.Property(e => e.Color4).HasMaxLength(20);

                entity.Property(e => e.Color5).HasMaxLength(20);

                entity.Property(e => e.Color6).HasMaxLength(20);

                entity.Property(e => e.Color7).HasMaxLength(20);

                entity.Property(e => e.ColorArea1).HasColumnName("Color_Area1");

                entity.Property(e => e.ColorArea2).HasColumnName("Color_Area2");

                entity.Property(e => e.ColorArea3).HasColumnName("Color_Area3");

                entity.Property(e => e.ColorArea4).HasColumnName("Color_Area4");

                entity.Property(e => e.ColorArea5).HasColumnName("Color_Area5");

                entity.Property(e => e.ColorArea6).HasColumnName("Color_Area6");

                entity.Property(e => e.ColorArea7).HasColumnName("Color_Area7");

                entity.Property(e => e.ColorCount).HasColumnName("Color_count");

                entity.Property(e => e.ColourCount).HasColumnName("Colour_Count");

                entity.Property(e => e.CutNo).HasColumnName("Cut_No");

                entity.Property(e => e.JoinMatNo)
                    .HasColumnName("Join_Mat_no")
                    .HasMaxLength(18);

                entity.Property(e => e.Machine).HasMaxLength(50);

                entity.Property(e => e.MatCode)
                    .HasColumnName("Mat_Code")
                    .HasMaxLength(20);

                entity.Property(e => e.MaterialNo)
                    .IsRequired()
                    .HasColumnName("Material_No")
                    .HasMaxLength(18);

                entity.Property(e => e.McMove).HasColumnName("MC_Move");

                entity.Property(e => e.MylaNo)
                    .HasColumnName("Myla_No")
                    .HasMaxLength(50);

                entity.Property(e => e.NoOpenIn).HasColumnName("No_Open_in");

                entity.Property(e => e.NoOpenOut).HasColumnName("No_Open_out");

                entity.Property(e => e.NoneBlk).HasColumnName("None_Blk");

                entity.Property(e => e.PaperWidth).HasColumnName("Paper_Width");

                entity.Property(e => e.PdisStatus)
                    .HasColumnName("PDIS_Status")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.PlanCode)
                    .HasColumnName("Plan_Code")
                    .HasMaxLength(10);

                entity.Property(e => e.Plant).HasMaxLength(5);

                entity.Property(e => e.PlateNo)
                    .HasColumnName("Plate_No")
                    .HasMaxLength(50);

                entity.Property(e => e.PostTm).HasColumnName("Post_tm");

                entity.Property(e => e.PrepareTm).HasColumnName("Prepare_tm");

                entity.Property(e => e.PsmStatus)
                    .HasColumnName("PSM_Status")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.RemarkInprocess)
                    .HasColumnName("Remark_Inprocess")
                    .HasMaxLength(255);

                entity.Property(e => e.RotateIn).HasColumnName("Rotate_In");

                entity.Property(e => e.RotateOut).HasColumnName("Rotate_Out");

                entity.Property(e => e.RunWaste).HasColumnName("Run_waste");

                entity.Property(e => e.SapStatus).HasColumnName("SAP_Status");

                entity.Property(e => e.SemiBlk).HasColumnName("Semi_Blk");

                entity.Property(e => e.SeparatMatNo)
                    .HasColumnName("Separat_Mat_no")
                    .HasMaxLength(18);

                entity.Property(e => e.SetupTm).HasColumnName("Setup_tm");

                entity.Property(e => e.SetupWaste).HasColumnName("Setup_waste");

                entity.Property(e => e.Shade1).HasMaxLength(10);

                entity.Property(e => e.Shade2).HasMaxLength(10);

                entity.Property(e => e.Shade3).HasMaxLength(10);

                entity.Property(e => e.Shade4).HasMaxLength(10);

                entity.Property(e => e.Shade5).HasMaxLength(10);

                entity.Property(e => e.Shade6).HasMaxLength(10);

                entity.Property(e => e.Shade7).HasMaxLength(10);

                entity.Property(e => e.SheetInLeg).HasColumnName("Sheet_in_Leg");

                entity.Property(e => e.SheetInWid).HasColumnName("Sheet_in_Wid");

                entity.Property(e => e.SheetOutLeg).HasColumnName("Sheet_out_Leg");

                entity.Property(e => e.SheetOutWid).HasColumnName("Sheet_out_Wid");

                entity.Property(e => e.ShipBlk).HasColumnName("Ship_Blk");

                entity.Property(e => e.StackHeight).HasColumnName("Stack_Height");

                entity.Property(e => e.StanBlk).HasColumnName("Stan_Blk");

                entity.Property(e => e.StdProcess).HasColumnName("Std_Process");

                entity.Property(e => e.TranStatus).HasColumnName("Tran_Status");

                entity.Property(e => e.UnUpgradBoard)
                    .HasColumnName("UnUpgrad_Board")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.WasteLeg).HasColumnName("Waste_Leg");

                entity.Property(e => e.WasteWid).HasColumnName("Waste_Wid");

                entity.Property(e => e.WeightIn).HasColumnName("Weight_in");

                entity.Property(e => e.WeightOut).HasColumnName("Weight_out");
            });

            modelBuilder.Entity<TccbMasterData>(entity =>
            {
                entity.HasKey(e => e.PsmId);

                entity.ToTable("TCCB_MasterData");

                entity.Property(e => e.PsmId)
                    .HasColumnName("PSM_ID")
                    .HasMaxLength(50)
                    .ValueGeneratedNever();

                entity.Property(e => e.Bl)
                    .HasColumnName("BL")
                    .HasMaxLength(3);

                entity.Property(e => e.Blweigth).HasColumnName("BLWeigth");

                entity.Property(e => e.Bm)
                    .HasColumnName("BM")
                    .HasMaxLength(3);

                entity.Property(e => e.Bmweigth).HasColumnName("BMWeigth");

                entity.Property(e => e.Board).HasMaxLength(50);

                entity.Property(e => e.BomUom)
                    .HasColumnName("BOM_UOM")
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.BoxArea).HasColumnName("Box_Area");

                entity.Property(e => e.BoxType)
                    .HasColumnName("Box_Type")
                    .HasMaxLength(60);

                entity.Property(e => e.Change).HasMaxLength(100);

                entity.Property(e => e.Cl)
                    .HasColumnName("CL")
                    .HasMaxLength(3);

                entity.Property(e => e.Clweigth).HasColumnName("CLWeigth");

                entity.Property(e => e.Cm)
                    .HasColumnName("CM")
                    .HasMaxLength(3);

                entity.Property(e => e.Cmweigth).HasColumnName("CMWeigth");

                entity.Property(e => e.Code).HasMaxLength(20);

                entity.Property(e => e.CreateDate).HasColumnType("smalldatetime");

                entity.Property(e => e.CusId)
                    .HasColumnName("Cus_ID")
                    .HasMaxLength(10);

                entity.Property(e => e.CustCode)
                    .HasColumnName("Cust_Code")
                    .HasMaxLength(10);

                entity.Property(e => e.CustName)
                    .HasColumnName("Cust_Name")
                    .HasMaxLength(50);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(40);

                entity.Property(e => e.DiecutPictPath)
                    .HasColumnName("DiecutPict_Path")
                    .HasMaxLength(50);

                entity.Property(e => e.Dl)
                    .HasColumnName("DL")
                    .HasMaxLength(3);

                entity.Property(e => e.Dlweigth).HasColumnName("DLWeigth");

                entity.Property(e => e.Dm)
                    .HasColumnName("DM")
                    .HasMaxLength(3);

                entity.Property(e => e.Dmweigth).HasColumnName("DMWeigth");

                entity.Property(e => e.EanCode).HasMaxLength(13);

                entity.Property(e => e.Flute).HasMaxLength(3);

                entity.Property(e => e.Gl)
                    .HasColumnName("GL")
                    .HasMaxLength(3);

                entity.Property(e => e.Glweigth).HasColumnName("GLWeigth");

                entity.Property(e => e.Hierarchy)
                    .IsRequired()
                    .HasMaxLength(18);

                entity.Property(e => e.HighGroup)
                    .HasColumnName("High_Group")
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.HighValue)
                    .HasColumnName("High_Value")
                    .HasMaxLength(4)
                    .IsUnicode(false);

                entity.Property(e => e.IndDes)
                    .HasColumnName("Ind_Des")
                    .HasMaxLength(50);

                entity.Property(e => e.IndGrp)
                    .HasColumnName("Ind_Grp")
                    .HasMaxLength(3);

                entity.Property(e => e.JoinType).HasMaxLength(60);

                entity.Property(e => e.Language)
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.LastUpdate).HasColumnType("smalldatetime");

                entity.Property(e => e.MaterialNo)
                    .IsRequired()
                    .HasColumnName("Material_No")
                    .HasMaxLength(18);

                entity.Property(e => e.MaterialType)
                    .HasColumnName("Material_Type")
                    .HasMaxLength(2);

                entity.Property(e => e.NoSlot).HasColumnName("No_Slot");

                entity.Property(e => e.OuterJoin).HasColumnName("Outer_Join");

                entity.Property(e => e.PalletSize).HasMaxLength(10);

                entity.Property(e => e.PalletizationPath)
                    .HasColumnName("Palletization_Path")
                    .HasMaxLength(50);

                entity.Property(e => e.PartNo)
                    .HasColumnName("Part_No")
                    .HasMaxLength(22);

                entity.Property(e => e.Pc)
                    .IsRequired()
                    .HasColumnName("PC")
                    .HasMaxLength(15);

                entity.Property(e => e.PdisStatus)
                    .IsRequired()
                    .HasColumnName("PDIS_Status")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.PieceSet).HasColumnName("Piece_Set");

                entity.Property(e => e.Plant).HasMaxLength(5);

                entity.Property(e => e.PltAxleHeight).HasColumnName("Plt_Axle_Height");

                entity.Property(e => e.PltBeam).HasColumnName("Plt_Beam");

                entity.Property(e => e.PltDoubleAxle).HasColumnName("Plt_Double_axle");

                entity.Property(e => e.PltFloorAbove).HasColumnName("Plt_Floor_Above");

                entity.Property(e => e.PltFloorUnder).HasColumnName("Plt_Floor_Under");

                entity.Property(e => e.PltLegDouble).HasColumnName("Plt_Leg_Double");

                entity.Property(e => e.PltLegSingle).HasColumnName("Plt_Leg_Single");

                entity.Property(e => e.PltSingleAxle).HasColumnName("Plt_Single_axle");

                entity.Property(e => e.PrintMasterPath)
                    .HasColumnName("PrintMaster_Path")
                    .HasMaxLength(50);

                entity.Property(e => e.PrintMethod)
                    .HasColumnName("Print_Method")
                    .HasMaxLength(50);

                entity.Property(e => e.PriorityFlag)
                    .HasColumnName("Priority_Flag")
                    .HasMaxLength(2);

                entity.Property(e => e.ProType)
                    .HasColumnName("Pro_Type")
                    .HasMaxLength(60);

                entity.Property(e => e.PsmStatus)
                    .HasColumnName("PSM_Status")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.PurTxt1)
                    .HasColumnName("Pur_Txt1")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.PurTxt2)
                    .HasColumnName("Pur_Txt2")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.PurTxt3)
                    .HasColumnName("Pur_Txt3")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.PurTxt4)
                    .HasColumnName("Pur_Txt4")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.RscStyle)
                    .HasColumnName("RSC_Style")
                    .HasMaxLength(50);

                entity.Property(e => e.SaleOrg)
                    .IsRequired()
                    .HasColumnName("Sale_Org")
                    .HasMaxLength(5);

                entity.Property(e => e.SaleText1)
                    .HasColumnName("Sale_Text1")
                    .HasMaxLength(40);

                entity.Property(e => e.SaleText2)
                    .HasColumnName("Sale_Text2")
                    .HasMaxLength(40);

                entity.Property(e => e.SaleText3)
                    .HasColumnName("Sale_Text3")
                    .HasMaxLength(40);

                entity.Property(e => e.SaleText4)
                    .HasColumnName("Sale_Text4")
                    .HasMaxLength(40);

                entity.Property(e => e.SaleUom)
                    .HasColumnName("Sale_UOM")
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.SapStatus).HasColumnName("SAP_Status");

                entity.Property(e => e.SheetArea).HasColumnName("Sheet_Area");

                entity.Property(e => e.StatusFlag)
                    .HasColumnName("Status_Flag")
                    .HasMaxLength(30);

                entity.Property(e => e.TranStatus).HasColumnName("Tran_Status");

                entity.Property(e => e.UnUpgradBoard).HasColumnName("UnUpgrad_Board");

                entity.Property(e => e.User)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.WeightBox).HasColumnName("Weight_Box");

                entity.Property(e => e.WeightSh).HasColumnName("Weight_Sh");
            });

            modelBuilder.Entity<TccbRouting>(entity =>
            {
                entity.HasKey(e => new { e.PsmId, e.SeqNo });

                entity.ToTable("TCCB_Routing");

                entity.Property(e => e.PsmId)
                    .HasColumnName("PSM_ID")
                    .HasMaxLength(50);

                entity.Property(e => e.SeqNo).HasColumnName("Seq_No");

                entity.Property(e => e.Alternative1)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Alternative2)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Alternative3).HasMaxLength(50);

                entity.Property(e => e.Alternative4).HasMaxLength(50);

                entity.Property(e => e.Alternative5).HasMaxLength(50);

                entity.Property(e => e.Alternative6).HasMaxLength(50);

                entity.Property(e => e.Alternative7).HasMaxLength(50);

                entity.Property(e => e.Alternative8).HasMaxLength(50);

                entity.Property(e => e.BlockNo)
                    .HasColumnName("Block_No")
                    .HasMaxLength(50);

                entity.Property(e => e.Color1).HasMaxLength(20);

                entity.Property(e => e.Color2).HasMaxLength(20);

                entity.Property(e => e.Color3).HasMaxLength(20);

                entity.Property(e => e.Color4).HasMaxLength(20);

                entity.Property(e => e.Color5).HasMaxLength(20);

                entity.Property(e => e.Color6).HasMaxLength(20);

                entity.Property(e => e.Color7).HasMaxLength(20);

                entity.Property(e => e.ColorArea1).HasColumnName("Color_Area1");

                entity.Property(e => e.ColorArea2).HasColumnName("Color_Area2");

                entity.Property(e => e.ColorArea3).HasColumnName("Color_Area3");

                entity.Property(e => e.ColorArea4).HasColumnName("Color_Area4");

                entity.Property(e => e.ColorArea5).HasColumnName("Color_Area5");

                entity.Property(e => e.ColorArea6).HasColumnName("Color_Area6");

                entity.Property(e => e.ColorArea7).HasColumnName("Color_Area7");

                entity.Property(e => e.ColorCount).HasColumnName("Color_count");

                entity.Property(e => e.ColourCount).HasColumnName("Colour_Count");

                entity.Property(e => e.CutNo).HasColumnName("Cut_No");

                entity.Property(e => e.JoinMatNo)
                    .HasColumnName("Join_Mat_no")
                    .HasMaxLength(18);

                entity.Property(e => e.Machine).HasMaxLength(50);

                entity.Property(e => e.MatCode)
                    .HasColumnName("Mat_Code")
                    .HasMaxLength(20);

                entity.Property(e => e.MaterialNo)
                    .IsRequired()
                    .HasColumnName("Material_No")
                    .HasMaxLength(18);

                entity.Property(e => e.McMove).HasColumnName("MC_Move");

                entity.Property(e => e.MylaNo)
                    .HasColumnName("Myla_No")
                    .HasMaxLength(50);

                entity.Property(e => e.NoOpenIn).HasColumnName("No_Open_in");

                entity.Property(e => e.NoOpenOut).HasColumnName("No_Open_out");

                entity.Property(e => e.NoneBlk).HasColumnName("None_Blk");

                entity.Property(e => e.PaperWidth).HasColumnName("Paper_Width");

                entity.Property(e => e.PdisStatus)
                    .HasColumnName("PDIS_Status")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.PlanCode)
                    .HasColumnName("Plan_Code")
                    .HasMaxLength(10);

                entity.Property(e => e.Plant).HasMaxLength(5);

                entity.Property(e => e.PlateNo)
                    .HasColumnName("Plate_No")
                    .HasMaxLength(50);

                entity.Property(e => e.PostTm).HasColumnName("Post_tm");

                entity.Property(e => e.PrepareTm).HasColumnName("Prepare_tm");

                entity.Property(e => e.PsmStatus)
                    .HasColumnName("PSM_Status")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.RemarkInprocess)
                    .HasColumnName("Remark_Inprocess")
                    .HasMaxLength(255);

                entity.Property(e => e.RotateIn).HasColumnName("Rotate_In");

                entity.Property(e => e.RotateOut).HasColumnName("Rotate_Out");

                entity.Property(e => e.RunWaste).HasColumnName("Run_waste");

                entity.Property(e => e.SapStatus).HasColumnName("SAP_Status");

                entity.Property(e => e.SemiBlk).HasColumnName("Semi_Blk");

                entity.Property(e => e.SeparatMatNo)
                    .HasColumnName("Separat_Mat_no")
                    .HasMaxLength(18);

                entity.Property(e => e.SetupTm).HasColumnName("Setup_tm");

                entity.Property(e => e.SetupWaste).HasColumnName("Setup_waste");

                entity.Property(e => e.Shade1).HasMaxLength(10);

                entity.Property(e => e.Shade2).HasMaxLength(10);

                entity.Property(e => e.Shade3).HasMaxLength(10);

                entity.Property(e => e.Shade4).HasMaxLength(10);

                entity.Property(e => e.Shade5).HasMaxLength(10);

                entity.Property(e => e.Shade6).HasMaxLength(10);

                entity.Property(e => e.Shade7).HasMaxLength(10);

                entity.Property(e => e.SheetInLeg).HasColumnName("Sheet_in_Leg");

                entity.Property(e => e.SheetInWid).HasColumnName("Sheet_in_Wid");

                entity.Property(e => e.SheetOutLeg).HasColumnName("Sheet_out_Leg");

                entity.Property(e => e.SheetOutWid).HasColumnName("Sheet_out_Wid");

                entity.Property(e => e.ShipBlk).HasColumnName("Ship_Blk");

                entity.Property(e => e.StackHeight).HasColumnName("Stack_Height");

                entity.Property(e => e.StanBlk).HasColumnName("Stan_Blk");

                entity.Property(e => e.StdProcess).HasColumnName("Std_Process");

                entity.Property(e => e.TranStatus).HasColumnName("Tran_Status");

                entity.Property(e => e.UnUpgradBoard)
                    .HasColumnName("UnUpgrad_Board")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.WasteLeg).HasColumnName("Waste_Leg");

                entity.Property(e => e.WasteWid).HasColumnName("Waste_Wid");

                entity.Property(e => e.WeightIn).HasColumnName("Weight_in");

                entity.Property(e => e.WeightOut).HasColumnName("Weight_out");
            });

            modelBuilder.Entity<TckkMasterData>(entity =>
            {
                entity.HasKey(e => e.PsmId);

                entity.ToTable("TCKK_MasterData");

                entity.Property(e => e.PsmId)
                    .HasColumnName("PSM_ID")
                    .HasMaxLength(50)
                    .ValueGeneratedNever();

                entity.Property(e => e.Bl)
                    .HasColumnName("BL")
                    .HasMaxLength(3);

                entity.Property(e => e.Blweigth).HasColumnName("BLWeigth");

                entity.Property(e => e.Bm)
                    .HasColumnName("BM")
                    .HasMaxLength(3);

                entity.Property(e => e.Bmweigth).HasColumnName("BMWeigth");

                entity.Property(e => e.Board).HasMaxLength(50);

                entity.Property(e => e.BomUom)
                    .HasColumnName("BOM_UOM")
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.BoxArea).HasColumnName("Box_Area");

                entity.Property(e => e.BoxType)
                    .HasColumnName("Box_Type")
                    .HasMaxLength(60);

                entity.Property(e => e.Change).HasMaxLength(100);

                entity.Property(e => e.Cl)
                    .HasColumnName("CL")
                    .HasMaxLength(3);

                entity.Property(e => e.Clweigth).HasColumnName("CLWeigth");

                entity.Property(e => e.Cm)
                    .HasColumnName("CM")
                    .HasMaxLength(3);

                entity.Property(e => e.Cmweigth).HasColumnName("CMWeigth");

                entity.Property(e => e.Code).HasMaxLength(20);

                entity.Property(e => e.CreateDate).HasColumnType("smalldatetime");

                entity.Property(e => e.CusId)
                    .HasColumnName("Cus_ID")
                    .HasMaxLength(10);

                entity.Property(e => e.CustCode)
                    .HasColumnName("Cust_Code")
                    .HasMaxLength(10);

                entity.Property(e => e.CustName)
                    .HasColumnName("Cust_Name")
                    .HasMaxLength(50);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(40);

                entity.Property(e => e.DiecutPictPath)
                    .HasColumnName("DiecutPict_Path")
                    .HasMaxLength(50);

                entity.Property(e => e.Dl)
                    .HasColumnName("DL")
                    .HasMaxLength(3);

                entity.Property(e => e.Dlweigth).HasColumnName("DLWeigth");

                entity.Property(e => e.Dm)
                    .HasColumnName("DM")
                    .HasMaxLength(3);

                entity.Property(e => e.Dmweigth).HasColumnName("DMWeigth");

                entity.Property(e => e.EanCode).HasMaxLength(13);

                entity.Property(e => e.Flute).HasMaxLength(3);

                entity.Property(e => e.Gl)
                    .HasColumnName("GL")
                    .HasMaxLength(3);

                entity.Property(e => e.Glweigth).HasColumnName("GLWeigth");

                entity.Property(e => e.Hierarchy)
                    .IsRequired()
                    .HasMaxLength(18);

                entity.Property(e => e.HighGroup)
                    .HasColumnName("High_Group")
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.HighValue)
                    .HasColumnName("High_Value")
                    .HasMaxLength(4)
                    .IsUnicode(false);

                entity.Property(e => e.IndDes)
                    .HasColumnName("Ind_Des")
                    .HasMaxLength(50);

                entity.Property(e => e.IndGrp)
                    .HasColumnName("Ind_Grp")
                    .HasMaxLength(3);

                entity.Property(e => e.JoinType).HasMaxLength(60);

                entity.Property(e => e.Language)
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.LastUpdate).HasColumnType("smalldatetime");

                entity.Property(e => e.MaterialNo)
                    .IsRequired()
                    .HasColumnName("Material_No")
                    .HasMaxLength(18);

                entity.Property(e => e.MaterialType)
                    .HasColumnName("Material_Type")
                    .HasMaxLength(2);

                entity.Property(e => e.NoSlot).HasColumnName("No_Slot");

                entity.Property(e => e.OuterJoin).HasColumnName("Outer_Join");

                entity.Property(e => e.PalletSize).HasMaxLength(10);

                entity.Property(e => e.PalletizationPath)
                    .HasColumnName("Palletization_Path")
                    .HasMaxLength(50);

                entity.Property(e => e.PartNo)
                    .HasColumnName("Part_No")
                    .HasMaxLength(22);

                entity.Property(e => e.Pc)
                    .IsRequired()
                    .HasColumnName("PC")
                    .HasMaxLength(15);

                entity.Property(e => e.PdisStatus)
                    .IsRequired()
                    .HasColumnName("PDIS_Status")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.PieceSet).HasColumnName("Piece_Set");

                entity.Property(e => e.Plant).HasMaxLength(5);

                entity.Property(e => e.PltAxleHeight).HasColumnName("Plt_Axle_Height");

                entity.Property(e => e.PltBeam).HasColumnName("Plt_Beam");

                entity.Property(e => e.PltDoubleAxle).HasColumnName("Plt_Double_axle");

                entity.Property(e => e.PltFloorAbove).HasColumnName("Plt_Floor_Above");

                entity.Property(e => e.PltFloorUnder).HasColumnName("Plt_Floor_Under");

                entity.Property(e => e.PltLegDouble).HasColumnName("Plt_Leg_Double");

                entity.Property(e => e.PltLegSingle).HasColumnName("Plt_Leg_Single");

                entity.Property(e => e.PltSingleAxle).HasColumnName("Plt_Single_axle");

                entity.Property(e => e.PrintMasterPath)
                    .HasColumnName("PrintMaster_Path")
                    .HasMaxLength(50);

                entity.Property(e => e.PrintMethod)
                    .HasColumnName("Print_Method")
                    .HasMaxLength(50);

                entity.Property(e => e.PriorityFlag)
                    .HasColumnName("Priority_Flag")
                    .HasMaxLength(2);

                entity.Property(e => e.ProType)
                    .HasColumnName("Pro_Type")
                    .HasMaxLength(60);

                entity.Property(e => e.PsmStatus)
                    .HasColumnName("PSM_Status")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.PurTxt1)
                    .HasColumnName("Pur_Txt1")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.PurTxt2)
                    .HasColumnName("Pur_Txt2")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.PurTxt3)
                    .HasColumnName("Pur_Txt3")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.PurTxt4)
                    .HasColumnName("Pur_Txt4")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.RscStyle)
                    .HasColumnName("RSC_Style")
                    .HasMaxLength(50);

                entity.Property(e => e.SaleOrg)
                    .IsRequired()
                    .HasColumnName("Sale_Org")
                    .HasMaxLength(5);

                entity.Property(e => e.SaleText1)
                    .HasColumnName("Sale_Text1")
                    .HasMaxLength(40);

                entity.Property(e => e.SaleText2)
                    .HasColumnName("Sale_Text2")
                    .HasMaxLength(40);

                entity.Property(e => e.SaleText3)
                    .HasColumnName("Sale_Text3")
                    .HasMaxLength(40);

                entity.Property(e => e.SaleText4)
                    .HasColumnName("Sale_Text4")
                    .HasMaxLength(40);

                entity.Property(e => e.SaleUom)
                    .HasColumnName("Sale_UOM")
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.SapStatus).HasColumnName("SAP_Status");

                entity.Property(e => e.SheetArea).HasColumnName("Sheet_Area");

                entity.Property(e => e.StatusFlag)
                    .HasColumnName("Status_Flag")
                    .HasMaxLength(30);

                entity.Property(e => e.TranStatus).HasColumnName("Tran_Status");

                entity.Property(e => e.UnUpgradBoard).HasColumnName("UnUpgrad_Board");

                entity.Property(e => e.User)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.WeightBox).HasColumnName("Weight_Box");

                entity.Property(e => e.WeightSh).HasColumnName("Weight_Sh");
            });

            modelBuilder.Entity<TckkRouting>(entity =>
            {
                entity.HasKey(e => new { e.PsmId, e.SeqNo });

                entity.ToTable("TCKK_Routing");

                entity.Property(e => e.PsmId)
                    .HasColumnName("PSM_ID")
                    .HasMaxLength(50);

                entity.Property(e => e.SeqNo).HasColumnName("Seq_No");

                entity.Property(e => e.Alternative1)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Alternative2)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Alternative3).HasMaxLength(50);

                entity.Property(e => e.Alternative4).HasMaxLength(50);

                entity.Property(e => e.Alternative5).HasMaxLength(50);

                entity.Property(e => e.Alternative6).HasMaxLength(50);

                entity.Property(e => e.Alternative7).HasMaxLength(50);

                entity.Property(e => e.Alternative8).HasMaxLength(50);

                entity.Property(e => e.BlockNo)
                    .HasColumnName("Block_No")
                    .HasMaxLength(50);

                entity.Property(e => e.Color1).HasMaxLength(20);

                entity.Property(e => e.Color2).HasMaxLength(20);

                entity.Property(e => e.Color3).HasMaxLength(20);

                entity.Property(e => e.Color4).HasMaxLength(20);

                entity.Property(e => e.Color5).HasMaxLength(20);

                entity.Property(e => e.Color6).HasMaxLength(20);

                entity.Property(e => e.Color7).HasMaxLength(20);

                entity.Property(e => e.ColorArea1).HasColumnName("Color_Area1");

                entity.Property(e => e.ColorArea2).HasColumnName("Color_Area2");

                entity.Property(e => e.ColorArea3).HasColumnName("Color_Area3");

                entity.Property(e => e.ColorArea4).HasColumnName("Color_Area4");

                entity.Property(e => e.ColorArea5).HasColumnName("Color_Area5");

                entity.Property(e => e.ColorArea6).HasColumnName("Color_Area6");

                entity.Property(e => e.ColorArea7).HasColumnName("Color_Area7");

                entity.Property(e => e.ColorCount).HasColumnName("Color_count");

                entity.Property(e => e.ColourCount).HasColumnName("Colour_Count");

                entity.Property(e => e.CutNo).HasColumnName("Cut_No");

                entity.Property(e => e.JoinMatNo)
                    .HasColumnName("Join_Mat_no")
                    .HasMaxLength(18);

                entity.Property(e => e.Machine).HasMaxLength(50);

                entity.Property(e => e.MatCode)
                    .HasColumnName("Mat_Code")
                    .HasMaxLength(20);

                entity.Property(e => e.MaterialNo)
                    .IsRequired()
                    .HasColumnName("Material_No")
                    .HasMaxLength(18);

                entity.Property(e => e.McMove).HasColumnName("MC_Move");

                entity.Property(e => e.MylaNo)
                    .HasColumnName("Myla_No")
                    .HasMaxLength(50);

                entity.Property(e => e.NoOpenIn).HasColumnName("No_Open_in");

                entity.Property(e => e.NoOpenOut).HasColumnName("No_Open_out");

                entity.Property(e => e.NoneBlk).HasColumnName("None_Blk");

                entity.Property(e => e.PaperWidth).HasColumnName("Paper_Width");

                entity.Property(e => e.PdisStatus)
                    .HasColumnName("PDIS_Status")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.PlanCode)
                    .HasColumnName("Plan_Code")
                    .HasMaxLength(10);

                entity.Property(e => e.Plant).HasMaxLength(5);

                entity.Property(e => e.PlateNo)
                    .HasColumnName("Plate_No")
                    .HasMaxLength(50);

                entity.Property(e => e.PostTm).HasColumnName("Post_tm");

                entity.Property(e => e.PrepareTm).HasColumnName("Prepare_tm");

                entity.Property(e => e.PsmStatus)
                    .HasColumnName("PSM_Status")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.RemarkInprocess)
                    .HasColumnName("Remark_Inprocess")
                    .HasMaxLength(255);

                entity.Property(e => e.RotateIn).HasColumnName("Rotate_In");

                entity.Property(e => e.RotateOut).HasColumnName("Rotate_Out");

                entity.Property(e => e.RunWaste).HasColumnName("Run_waste");

                entity.Property(e => e.SapStatus).HasColumnName("SAP_Status");

                entity.Property(e => e.SemiBlk).HasColumnName("Semi_Blk");

                entity.Property(e => e.SeparatMatNo)
                    .HasColumnName("Separat_Mat_no")
                    .HasMaxLength(18);

                entity.Property(e => e.SetupTm).HasColumnName("Setup_tm");

                entity.Property(e => e.SetupWaste).HasColumnName("Setup_waste");

                entity.Property(e => e.Shade1).HasMaxLength(10);

                entity.Property(e => e.Shade2).HasMaxLength(10);

                entity.Property(e => e.Shade3).HasMaxLength(10);

                entity.Property(e => e.Shade4).HasMaxLength(10);

                entity.Property(e => e.Shade5).HasMaxLength(10);

                entity.Property(e => e.Shade6).HasMaxLength(10);

                entity.Property(e => e.Shade7).HasMaxLength(10);

                entity.Property(e => e.SheetInLeg).HasColumnName("Sheet_in_Leg");

                entity.Property(e => e.SheetInWid).HasColumnName("Sheet_in_Wid");

                entity.Property(e => e.SheetOutLeg).HasColumnName("Sheet_out_Leg");

                entity.Property(e => e.SheetOutWid).HasColumnName("Sheet_out_Wid");

                entity.Property(e => e.ShipBlk).HasColumnName("Ship_Blk");

                entity.Property(e => e.StackHeight).HasColumnName("Stack_Height");

                entity.Property(e => e.StanBlk).HasColumnName("Stan_Blk");

                entity.Property(e => e.StdProcess).HasColumnName("Std_Process");

                entity.Property(e => e.TranStatus).HasColumnName("Tran_Status");

                entity.Property(e => e.UnUpgradBoard)
                    .HasColumnName("UnUpgrad_Board")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.WasteLeg).HasColumnName("Waste_Leg");

                entity.Property(e => e.WasteWid).HasColumnName("Waste_Wid");

                entity.Property(e => e.WeightIn).HasColumnName("Weight_in");

                entity.Property(e => e.WeightOut).HasColumnName("Weight_out");
            });

            modelBuilder.Entity<TckpMasterData>(entity =>
            {
                entity.HasKey(e => e.PsmId);

                entity.ToTable("TCKP_MasterData");

                entity.Property(e => e.PsmId)
                    .HasColumnName("PSM_ID")
                    .HasMaxLength(50)
                    .ValueGeneratedNever();

                entity.Property(e => e.Bl)
                    .HasColumnName("BL")
                    .HasMaxLength(3);

                entity.Property(e => e.Blweigth).HasColumnName("BLWeigth");

                entity.Property(e => e.Bm)
                    .HasColumnName("BM")
                    .HasMaxLength(3);

                entity.Property(e => e.Bmweigth).HasColumnName("BMWeigth");

                entity.Property(e => e.Board).HasMaxLength(50);

                entity.Property(e => e.BomUom)
                    .HasColumnName("BOM_UOM")
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.BoxArea).HasColumnName("Box_Area");

                entity.Property(e => e.BoxType)
                    .HasColumnName("Box_Type")
                    .HasMaxLength(60);

                entity.Property(e => e.Change).HasMaxLength(100);

                entity.Property(e => e.Cl)
                    .HasColumnName("CL")
                    .HasMaxLength(3);

                entity.Property(e => e.Clweigth).HasColumnName("CLWeigth");

                entity.Property(e => e.Cm)
                    .HasColumnName("CM")
                    .HasMaxLength(3);

                entity.Property(e => e.Cmweigth).HasColumnName("CMWeigth");

                entity.Property(e => e.Code).HasMaxLength(20);

                entity.Property(e => e.CreateDate).HasColumnType("smalldatetime");

                entity.Property(e => e.CusId)
                    .HasColumnName("Cus_ID")
                    .HasMaxLength(10);

                entity.Property(e => e.CustCode)
                    .HasColumnName("Cust_Code")
                    .HasMaxLength(10);

                entity.Property(e => e.CustName)
                    .HasColumnName("Cust_Name")
                    .HasMaxLength(50);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(40);

                entity.Property(e => e.DiecutPictPath)
                    .HasColumnName("DiecutPict_Path")
                    .HasMaxLength(50);

                entity.Property(e => e.Dl)
                    .HasColumnName("DL")
                    .HasMaxLength(3);

                entity.Property(e => e.Dlweigth).HasColumnName("DLWeigth");

                entity.Property(e => e.Dm)
                    .HasColumnName("DM")
                    .HasMaxLength(3);

                entity.Property(e => e.Dmweigth).HasColumnName("DMWeigth");

                entity.Property(e => e.EanCode).HasMaxLength(13);

                entity.Property(e => e.Flute).HasMaxLength(3);

                entity.Property(e => e.Gl)
                    .HasColumnName("GL")
                    .HasMaxLength(3);

                entity.Property(e => e.Glweigth).HasColumnName("GLWeigth");

                entity.Property(e => e.Hierarchy)
                    .IsRequired()
                    .HasMaxLength(18);

                entity.Property(e => e.HighGroup)
                    .HasColumnName("High_Group")
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.HighValue)
                    .HasColumnName("High_Value")
                    .HasMaxLength(4)
                    .IsUnicode(false);

                entity.Property(e => e.IndDes)
                    .HasColumnName("Ind_Des")
                    .HasMaxLength(50);

                entity.Property(e => e.IndGrp)
                    .HasColumnName("Ind_Grp")
                    .HasMaxLength(3);

                entity.Property(e => e.JoinType).HasMaxLength(60);

                entity.Property(e => e.Language)
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.LastUpdate).HasColumnType("smalldatetime");

                entity.Property(e => e.MaterialNo)
                    .IsRequired()
                    .HasColumnName("Material_No")
                    .HasMaxLength(18);

                entity.Property(e => e.MaterialType)
                    .HasColumnName("Material_Type")
                    .HasMaxLength(2);

                entity.Property(e => e.NoSlot).HasColumnName("No_Slot");

                entity.Property(e => e.OuterJoin).HasColumnName("Outer_Join");

                entity.Property(e => e.PalletSize).HasMaxLength(10);

                entity.Property(e => e.PalletizationPath)
                    .HasColumnName("Palletization_Path")
                    .HasMaxLength(50);

                entity.Property(e => e.PartNo)
                    .HasColumnName("Part_No")
                    .HasMaxLength(22);

                entity.Property(e => e.Pc)
                    .IsRequired()
                    .HasColumnName("PC")
                    .HasMaxLength(15);

                entity.Property(e => e.PdisStatus)
                    .IsRequired()
                    .HasColumnName("PDIS_Status")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.PieceSet).HasColumnName("Piece_Set");

                entity.Property(e => e.Plant).HasMaxLength(5);

                entity.Property(e => e.PltAxleHeight).HasColumnName("Plt_Axle_Height");

                entity.Property(e => e.PltBeam).HasColumnName("Plt_Beam");

                entity.Property(e => e.PltDoubleAxle).HasColumnName("Plt_Double_axle");

                entity.Property(e => e.PltFloorAbove).HasColumnName("Plt_Floor_Above");

                entity.Property(e => e.PltFloorUnder).HasColumnName("Plt_Floor_Under");

                entity.Property(e => e.PltLegDouble).HasColumnName("Plt_Leg_Double");

                entity.Property(e => e.PltLegSingle).HasColumnName("Plt_Leg_Single");

                entity.Property(e => e.PltSingleAxle).HasColumnName("Plt_Single_axle");

                entity.Property(e => e.PrintMasterPath)
                    .HasColumnName("PrintMaster_Path")
                    .HasMaxLength(50);

                entity.Property(e => e.PrintMethod)
                    .HasColumnName("Print_Method")
                    .HasMaxLength(50);

                entity.Property(e => e.PriorityFlag)
                    .HasColumnName("Priority_Flag")
                    .HasMaxLength(2);

                entity.Property(e => e.ProType)
                    .HasColumnName("Pro_Type")
                    .HasMaxLength(60);

                entity.Property(e => e.PsmStatus)
                    .HasColumnName("PSM_Status")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.PurTxt1)
                    .HasColumnName("Pur_Txt1")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.PurTxt2)
                    .HasColumnName("Pur_Txt2")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.PurTxt3)
                    .HasColumnName("Pur_Txt3")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.PurTxt4)
                    .HasColumnName("Pur_Txt4")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.RscStyle)
                    .HasColumnName("RSC_Style")
                    .HasMaxLength(50);

                entity.Property(e => e.SaleOrg)
                    .IsRequired()
                    .HasColumnName("Sale_Org")
                    .HasMaxLength(5);

                entity.Property(e => e.SaleText1)
                    .HasColumnName("Sale_Text1")
                    .HasMaxLength(40);

                entity.Property(e => e.SaleText2)
                    .HasColumnName("Sale_Text2")
                    .HasMaxLength(40);

                entity.Property(e => e.SaleText3)
                    .HasColumnName("Sale_Text3")
                    .HasMaxLength(40);

                entity.Property(e => e.SaleText4)
                    .HasColumnName("Sale_Text4")
                    .HasMaxLength(40);

                entity.Property(e => e.SaleUom)
                    .HasColumnName("Sale_UOM")
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.SapStatus).HasColumnName("SAP_Status");

                entity.Property(e => e.SheetArea).HasColumnName("Sheet_Area");

                entity.Property(e => e.StatusFlag)
                    .HasColumnName("Status_Flag")
                    .HasMaxLength(30);

                entity.Property(e => e.TranStatus).HasColumnName("Tran_Status");

                entity.Property(e => e.UnUpgradBoard).HasColumnName("UnUpgrad_Board");

                entity.Property(e => e.User)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.WeightBox).HasColumnName("Weight_Box");

                entity.Property(e => e.WeightSh).HasColumnName("Weight_Sh");
            });

            modelBuilder.Entity<TckpRouting>(entity =>
            {
                entity.HasKey(e => new { e.PsmId, e.SeqNo });

                entity.ToTable("TCKP_Routing");

                entity.Property(e => e.PsmId)
                    .HasColumnName("PSM_ID")
                    .HasMaxLength(50);

                entity.Property(e => e.SeqNo).HasColumnName("Seq_No");

                entity.Property(e => e.Alternative1)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Alternative2)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Alternative3).HasMaxLength(50);

                entity.Property(e => e.Alternative4).HasMaxLength(50);

                entity.Property(e => e.Alternative5).HasMaxLength(50);

                entity.Property(e => e.Alternative6).HasMaxLength(50);

                entity.Property(e => e.Alternative7).HasMaxLength(50);

                entity.Property(e => e.Alternative8).HasMaxLength(50);

                entity.Property(e => e.BlockNo)
                    .HasColumnName("Block_No")
                    .HasMaxLength(50);

                entity.Property(e => e.Color1).HasMaxLength(20);

                entity.Property(e => e.Color2).HasMaxLength(20);

                entity.Property(e => e.Color3).HasMaxLength(20);

                entity.Property(e => e.Color4).HasMaxLength(20);

                entity.Property(e => e.Color5).HasMaxLength(20);

                entity.Property(e => e.Color6).HasMaxLength(20);

                entity.Property(e => e.Color7).HasMaxLength(20);

                entity.Property(e => e.ColorArea1).HasColumnName("Color_Area1");

                entity.Property(e => e.ColorArea2).HasColumnName("Color_Area2");

                entity.Property(e => e.ColorArea3).HasColumnName("Color_Area3");

                entity.Property(e => e.ColorArea4).HasColumnName("Color_Area4");

                entity.Property(e => e.ColorArea5).HasColumnName("Color_Area5");

                entity.Property(e => e.ColorArea6).HasColumnName("Color_Area6");

                entity.Property(e => e.ColorArea7).HasColumnName("Color_Area7");

                entity.Property(e => e.ColorCount).HasColumnName("Color_count");

                entity.Property(e => e.ColourCount).HasColumnName("Colour_Count");

                entity.Property(e => e.CutNo).HasColumnName("Cut_No");

                entity.Property(e => e.JoinMatNo)
                    .HasColumnName("Join_Mat_no")
                    .HasMaxLength(18);

                entity.Property(e => e.Machine).HasMaxLength(50);

                entity.Property(e => e.MatCode)
                    .HasColumnName("Mat_Code")
                    .HasMaxLength(20);

                entity.Property(e => e.MaterialNo)
                    .IsRequired()
                    .HasColumnName("Material_No")
                    .HasMaxLength(18);

                entity.Property(e => e.McMove).HasColumnName("MC_Move");

                entity.Property(e => e.MylaNo)
                    .HasColumnName("Myla_No")
                    .HasMaxLength(50);

                entity.Property(e => e.NoOpenIn).HasColumnName("No_Open_in");

                entity.Property(e => e.NoOpenOut).HasColumnName("No_Open_out");

                entity.Property(e => e.NoneBlk).HasColumnName("None_Blk");

                entity.Property(e => e.PaperWidth).HasColumnName("Paper_Width");

                entity.Property(e => e.PdisStatus)
                    .HasColumnName("PDIS_Status")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.PlanCode)
                    .HasColumnName("Plan_Code")
                    .HasMaxLength(10);

                entity.Property(e => e.Plant).HasMaxLength(5);

                entity.Property(e => e.PlateNo)
                    .HasColumnName("Plate_No")
                    .HasMaxLength(50);

                entity.Property(e => e.PostTm).HasColumnName("Post_tm");

                entity.Property(e => e.PrepareTm).HasColumnName("Prepare_tm");

                entity.Property(e => e.PsmStatus)
                    .HasColumnName("PSM_Status")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.RemarkInprocess)
                    .HasColumnName("Remark_Inprocess")
                    .HasMaxLength(255);

                entity.Property(e => e.RotateIn).HasColumnName("Rotate_In");

                entity.Property(e => e.RotateOut).HasColumnName("Rotate_Out");

                entity.Property(e => e.RunWaste).HasColumnName("Run_waste");

                entity.Property(e => e.SapStatus).HasColumnName("SAP_Status");

                entity.Property(e => e.SemiBlk).HasColumnName("Semi_Blk");

                entity.Property(e => e.SeparatMatNo)
                    .HasColumnName("Separat_Mat_no")
                    .HasMaxLength(18);

                entity.Property(e => e.SetupTm).HasColumnName("Setup_tm");

                entity.Property(e => e.SetupWaste).HasColumnName("Setup_waste");

                entity.Property(e => e.Shade1).HasMaxLength(10);

                entity.Property(e => e.Shade2).HasMaxLength(10);

                entity.Property(e => e.Shade3).HasMaxLength(10);

                entity.Property(e => e.Shade4).HasMaxLength(10);

                entity.Property(e => e.Shade5).HasMaxLength(10);

                entity.Property(e => e.Shade6).HasMaxLength(10);

                entity.Property(e => e.Shade7).HasMaxLength(10);

                entity.Property(e => e.SheetInLeg).HasColumnName("Sheet_in_Leg");

                entity.Property(e => e.SheetInWid).HasColumnName("Sheet_in_Wid");

                entity.Property(e => e.SheetOutLeg).HasColumnName("Sheet_out_Leg");

                entity.Property(e => e.SheetOutWid).HasColumnName("Sheet_out_Wid");

                entity.Property(e => e.ShipBlk).HasColumnName("Ship_Blk");

                entity.Property(e => e.StackHeight).HasColumnName("Stack_Height");

                entity.Property(e => e.StanBlk).HasColumnName("Stan_Blk");

                entity.Property(e => e.StdProcess).HasColumnName("Std_Process");

                entity.Property(e => e.TranStatus).HasColumnName("Tran_Status");

                entity.Property(e => e.UnUpgradBoard)
                    .HasColumnName("UnUpgrad_Board")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.WasteLeg).HasColumnName("Waste_Leg");

                entity.Property(e => e.WasteWid).HasColumnName("Waste_Wid");

                entity.Property(e => e.WeightIn).HasColumnName("Weight_in");

                entity.Property(e => e.WeightOut).HasColumnName("Weight_out");
            });

            modelBuilder.Entity<TcnkMasterData>(entity =>
            {
                entity.HasKey(e => e.PsmId);

                entity.ToTable("TCNK_MasterData");

                entity.Property(e => e.PsmId)
                    .HasColumnName("PSM_ID")
                    .HasMaxLength(50)
                    .ValueGeneratedNever();

                entity.Property(e => e.Bl)
                    .HasColumnName("BL")
                    .HasMaxLength(3);

                entity.Property(e => e.Blweigth).HasColumnName("BLWeigth");

                entity.Property(e => e.Bm)
                    .HasColumnName("BM")
                    .HasMaxLength(3);

                entity.Property(e => e.Bmweigth).HasColumnName("BMWeigth");

                entity.Property(e => e.Board).HasMaxLength(50);

                entity.Property(e => e.BomUom)
                    .HasColumnName("BOM_UOM")
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.BoxArea).HasColumnName("Box_Area");

                entity.Property(e => e.BoxType)
                    .HasColumnName("Box_Type")
                    .HasMaxLength(60);

                entity.Property(e => e.Change).HasMaxLength(100);

                entity.Property(e => e.Cl)
                    .HasColumnName("CL")
                    .HasMaxLength(3);

                entity.Property(e => e.Clweigth).HasColumnName("CLWeigth");

                entity.Property(e => e.Cm)
                    .HasColumnName("CM")
                    .HasMaxLength(3);

                entity.Property(e => e.Cmweigth).HasColumnName("CMWeigth");

                entity.Property(e => e.Code).HasMaxLength(20);

                entity.Property(e => e.CreateDate).HasColumnType("smalldatetime");

                entity.Property(e => e.CusId)
                    .HasColumnName("Cus_ID")
                    .HasMaxLength(10);

                entity.Property(e => e.CustCode)
                    .HasColumnName("Cust_Code")
                    .HasMaxLength(10);

                entity.Property(e => e.CustName)
                    .HasColumnName("Cust_Name")
                    .HasMaxLength(50);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(40);

                entity.Property(e => e.DiecutPictPath)
                    .HasColumnName("DiecutPict_Path")
                    .HasMaxLength(50);

                entity.Property(e => e.Dl)
                    .HasColumnName("DL")
                    .HasMaxLength(3);

                entity.Property(e => e.Dlweigth).HasColumnName("DLWeigth");

                entity.Property(e => e.Dm)
                    .HasColumnName("DM")
                    .HasMaxLength(3);

                entity.Property(e => e.Dmweigth).HasColumnName("DMWeigth");

                entity.Property(e => e.EanCode).HasMaxLength(13);

                entity.Property(e => e.Flute).HasMaxLength(3);

                entity.Property(e => e.Gl)
                    .HasColumnName("GL")
                    .HasMaxLength(3);

                entity.Property(e => e.Glweigth).HasColumnName("GLWeigth");

                entity.Property(e => e.Hierarchy)
                    .IsRequired()
                    .HasMaxLength(18);

                entity.Property(e => e.HighGroup)
                    .HasColumnName("High_Group")
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.HighValue)
                    .HasColumnName("High_Value")
                    .HasMaxLength(4)
                    .IsUnicode(false);

                entity.Property(e => e.IndDes)
                    .HasColumnName("Ind_Des")
                    .HasMaxLength(50);

                entity.Property(e => e.IndGrp)
                    .HasColumnName("Ind_Grp")
                    .HasMaxLength(3);

                entity.Property(e => e.JoinType).HasMaxLength(60);

                entity.Property(e => e.Language)
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.LastUpdate).HasColumnType("smalldatetime");

                entity.Property(e => e.MaterialNo)
                    .IsRequired()
                    .HasColumnName("Material_No")
                    .HasMaxLength(18);

                entity.Property(e => e.MaterialType)
                    .HasColumnName("Material_Type")
                    .HasMaxLength(2);

                entity.Property(e => e.NoSlot).HasColumnName("No_Slot");

                entity.Property(e => e.OuterJoin).HasColumnName("Outer_Join");

                entity.Property(e => e.PalletSize).HasMaxLength(10);

                entity.Property(e => e.PalletizationPath)
                    .HasColumnName("Palletization_Path")
                    .HasMaxLength(50);

                entity.Property(e => e.PartNo)
                    .HasColumnName("Part_No")
                    .HasMaxLength(22);

                entity.Property(e => e.Pc)
                    .IsRequired()
                    .HasColumnName("PC")
                    .HasMaxLength(15);

                entity.Property(e => e.PdisStatus)
                    .IsRequired()
                    .HasColumnName("PDIS_Status")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.PieceSet).HasColumnName("Piece_Set");

                entity.Property(e => e.Plant).HasMaxLength(5);

                entity.Property(e => e.PltAxleHeight).HasColumnName("Plt_Axle_Height");

                entity.Property(e => e.PltBeam).HasColumnName("Plt_Beam");

                entity.Property(e => e.PltDoubleAxle).HasColumnName("Plt_Double_axle");

                entity.Property(e => e.PltFloorAbove).HasColumnName("Plt_Floor_Above");

                entity.Property(e => e.PltFloorUnder).HasColumnName("Plt_Floor_Under");

                entity.Property(e => e.PltLegDouble).HasColumnName("Plt_Leg_Double");

                entity.Property(e => e.PltLegSingle).HasColumnName("Plt_Leg_Single");

                entity.Property(e => e.PltSingleAxle).HasColumnName("Plt_Single_axle");

                entity.Property(e => e.PrintMasterPath)
                    .HasColumnName("PrintMaster_Path")
                    .HasMaxLength(50);

                entity.Property(e => e.PrintMethod)
                    .HasColumnName("Print_Method")
                    .HasMaxLength(50);

                entity.Property(e => e.PriorityFlag)
                    .HasColumnName("Priority_Flag")
                    .HasMaxLength(2);

                entity.Property(e => e.ProType)
                    .HasColumnName("Pro_Type")
                    .HasMaxLength(60);

                entity.Property(e => e.PsmStatus)
                    .HasColumnName("PSM_Status")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.PurTxt1)
                    .HasColumnName("Pur_Txt1")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.PurTxt2)
                    .HasColumnName("Pur_Txt2")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.PurTxt3)
                    .HasColumnName("Pur_Txt3")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.PurTxt4)
                    .HasColumnName("Pur_Txt4")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.RscStyle)
                    .HasColumnName("RSC_Style")
                    .HasMaxLength(50);

                entity.Property(e => e.SaleOrg)
                    .IsRequired()
                    .HasColumnName("Sale_Org")
                    .HasMaxLength(5);

                entity.Property(e => e.SaleText1)
                    .HasColumnName("Sale_Text1")
                    .HasMaxLength(40);

                entity.Property(e => e.SaleText2)
                    .HasColumnName("Sale_Text2")
                    .HasMaxLength(40);

                entity.Property(e => e.SaleText3)
                    .HasColumnName("Sale_Text3")
                    .HasMaxLength(40);

                entity.Property(e => e.SaleText4)
                    .HasColumnName("Sale_Text4")
                    .HasMaxLength(40);

                entity.Property(e => e.SaleUom)
                    .HasColumnName("Sale_UOM")
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.SapStatus).HasColumnName("SAP_Status");

                entity.Property(e => e.SheetArea).HasColumnName("Sheet_Area");

                entity.Property(e => e.StatusFlag)
                    .HasColumnName("Status_Flag")
                    .HasMaxLength(30);

                entity.Property(e => e.TranStatus).HasColumnName("Tran_Status");

                entity.Property(e => e.UnUpgradBoard).HasColumnName("UnUpgrad_Board");

                entity.Property(e => e.User)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.WeightBox).HasColumnName("Weight_Box");

                entity.Property(e => e.WeightSh).HasColumnName("Weight_Sh");
            });

            modelBuilder.Entity<TcnkRouting>(entity =>
            {
                entity.HasKey(e => new { e.PsmId, e.SeqNo });

                entity.ToTable("TCNK_Routing");

                entity.Property(e => e.PsmId)
                    .HasColumnName("PSM_ID")
                    .HasMaxLength(50);

                entity.Property(e => e.SeqNo).HasColumnName("Seq_No");

                entity.Property(e => e.Alternative1)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Alternative2)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Alternative3).HasMaxLength(50);

                entity.Property(e => e.Alternative4).HasMaxLength(50);

                entity.Property(e => e.Alternative5).HasMaxLength(50);

                entity.Property(e => e.Alternative6).HasMaxLength(50);

                entity.Property(e => e.Alternative7).HasMaxLength(50);

                entity.Property(e => e.Alternative8).HasMaxLength(50);

                entity.Property(e => e.BlockNo)
                    .HasColumnName("Block_No")
                    .HasMaxLength(50);

                entity.Property(e => e.Color1).HasMaxLength(20);

                entity.Property(e => e.Color2).HasMaxLength(20);

                entity.Property(e => e.Color3).HasMaxLength(20);

                entity.Property(e => e.Color4).HasMaxLength(20);

                entity.Property(e => e.Color5).HasMaxLength(20);

                entity.Property(e => e.Color6).HasMaxLength(20);

                entity.Property(e => e.Color7).HasMaxLength(20);

                entity.Property(e => e.ColorArea1).HasColumnName("Color_Area1");

                entity.Property(e => e.ColorArea2).HasColumnName("Color_Area2");

                entity.Property(e => e.ColorArea3).HasColumnName("Color_Area3");

                entity.Property(e => e.ColorArea4).HasColumnName("Color_Area4");

                entity.Property(e => e.ColorArea5).HasColumnName("Color_Area5");

                entity.Property(e => e.ColorArea6).HasColumnName("Color_Area6");

                entity.Property(e => e.ColorArea7).HasColumnName("Color_Area7");

                entity.Property(e => e.ColorCount).HasColumnName("Color_count");

                entity.Property(e => e.ColourCount).HasColumnName("Colour_Count");

                entity.Property(e => e.CutNo).HasColumnName("Cut_No");

                entity.Property(e => e.JoinMatNo)
                    .HasColumnName("Join_Mat_no")
                    .HasMaxLength(18);

                entity.Property(e => e.Machine).HasMaxLength(50);

                entity.Property(e => e.MatCode)
                    .HasColumnName("Mat_Code")
                    .HasMaxLength(20);

                entity.Property(e => e.MaterialNo)
                    .IsRequired()
                    .HasColumnName("Material_No")
                    .HasMaxLength(18);

                entity.Property(e => e.McMove).HasColumnName("MC_Move");

                entity.Property(e => e.MylaNo)
                    .HasColumnName("Myla_No")
                    .HasMaxLength(50);

                entity.Property(e => e.NoOpenIn).HasColumnName("No_Open_in");

                entity.Property(e => e.NoOpenOut).HasColumnName("No_Open_out");

                entity.Property(e => e.NoneBlk).HasColumnName("None_Blk");

                entity.Property(e => e.PaperWidth).HasColumnName("Paper_Width");

                entity.Property(e => e.PdisStatus)
                    .HasColumnName("PDIS_Status")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.PlanCode)
                    .HasColumnName("Plan_Code")
                    .HasMaxLength(10);

                entity.Property(e => e.Plant).HasMaxLength(5);

                entity.Property(e => e.PlateNo)
                    .HasColumnName("Plate_No")
                    .HasMaxLength(50);

                entity.Property(e => e.PostTm).HasColumnName("Post_tm");

                entity.Property(e => e.PrepareTm).HasColumnName("Prepare_tm");

                entity.Property(e => e.PsmStatus)
                    .HasColumnName("PSM_Status")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.RemarkInprocess)
                    .HasColumnName("Remark_Inprocess")
                    .HasMaxLength(255);

                entity.Property(e => e.RotateIn).HasColumnName("Rotate_In");

                entity.Property(e => e.RotateOut).HasColumnName("Rotate_Out");

                entity.Property(e => e.RunWaste).HasColumnName("Run_waste");

                entity.Property(e => e.SapStatus).HasColumnName("SAP_Status");

                entity.Property(e => e.SemiBlk).HasColumnName("Semi_Blk");

                entity.Property(e => e.SeparatMatNo)
                    .HasColumnName("Separat_Mat_no")
                    .HasMaxLength(18);

                entity.Property(e => e.SetupTm).HasColumnName("Setup_tm");

                entity.Property(e => e.SetupWaste).HasColumnName("Setup_waste");

                entity.Property(e => e.Shade1).HasMaxLength(10);

                entity.Property(e => e.Shade2).HasMaxLength(10);

                entity.Property(e => e.Shade3).HasMaxLength(10);

                entity.Property(e => e.Shade4).HasMaxLength(10);

                entity.Property(e => e.Shade5).HasMaxLength(10);

                entity.Property(e => e.Shade6).HasMaxLength(10);

                entity.Property(e => e.Shade7).HasMaxLength(10);

                entity.Property(e => e.SheetInLeg).HasColumnName("Sheet_in_Leg");

                entity.Property(e => e.SheetInWid).HasColumnName("Sheet_in_Wid");

                entity.Property(e => e.SheetOutLeg).HasColumnName("Sheet_out_Leg");

                entity.Property(e => e.SheetOutWid).HasColumnName("Sheet_out_Wid");

                entity.Property(e => e.ShipBlk).HasColumnName("Ship_Blk");

                entity.Property(e => e.StackHeight).HasColumnName("Stack_Height");

                entity.Property(e => e.StanBlk).HasColumnName("Stan_Blk");

                entity.Property(e => e.StdProcess).HasColumnName("Std_Process");

                entity.Property(e => e.TranStatus).HasColumnName("Tran_Status");

                entity.Property(e => e.UnUpgradBoard)
                    .HasColumnName("UnUpgrad_Board")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.WasteLeg).HasColumnName("Waste_Leg");

                entity.Property(e => e.WasteWid).HasColumnName("Waste_Wid");

                entity.Property(e => e.WeightIn).HasColumnName("Weight_in");

                entity.Property(e => e.WeightOut).HasColumnName("Weight_out");
            });

            modelBuilder.Entity<TcpbMasterData>(entity =>
            {
                entity.HasKey(e => e.PsmId);

                entity.ToTable("TCPB_MasterData");

                entity.Property(e => e.PsmId)
                    .HasColumnName("PSM_ID")
                    .HasMaxLength(50)
                    .ValueGeneratedNever();

                entity.Property(e => e.Bl)
                    .HasColumnName("BL")
                    .HasMaxLength(3);

                entity.Property(e => e.Blweigth).HasColumnName("BLWeigth");

                entity.Property(e => e.Bm)
                    .HasColumnName("BM")
                    .HasMaxLength(3);

                entity.Property(e => e.Bmweigth).HasColumnName("BMWeigth");

                entity.Property(e => e.Board).HasMaxLength(50);

                entity.Property(e => e.BomUom)
                    .HasColumnName("BOM_UOM")
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.BoxArea).HasColumnName("Box_Area");

                entity.Property(e => e.BoxType)
                    .HasColumnName("Box_Type")
                    .HasMaxLength(60);

                entity.Property(e => e.Change).HasMaxLength(100);

                entity.Property(e => e.Cl)
                    .HasColumnName("CL")
                    .HasMaxLength(3);

                entity.Property(e => e.Clweigth).HasColumnName("CLWeigth");

                entity.Property(e => e.Cm)
                    .HasColumnName("CM")
                    .HasMaxLength(3);

                entity.Property(e => e.Cmweigth).HasColumnName("CMWeigth");

                entity.Property(e => e.Code).HasMaxLength(20);

                entity.Property(e => e.CreateDate).HasColumnType("smalldatetime");

                entity.Property(e => e.CusId)
                    .HasColumnName("Cus_ID")
                    .HasMaxLength(10);

                entity.Property(e => e.CustCode)
                    .HasColumnName("Cust_Code")
                    .HasMaxLength(10);

                entity.Property(e => e.CustName)
                    .HasColumnName("Cust_Name")
                    .HasMaxLength(50);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(40);

                entity.Property(e => e.DiecutPictPath)
                    .HasColumnName("DiecutPict_Path")
                    .HasMaxLength(50);

                entity.Property(e => e.Dl)
                    .HasColumnName("DL")
                    .HasMaxLength(3);

                entity.Property(e => e.Dlweigth).HasColumnName("DLWeigth");

                entity.Property(e => e.Dm)
                    .HasColumnName("DM")
                    .HasMaxLength(3);

                entity.Property(e => e.Dmweigth).HasColumnName("DMWeigth");

                entity.Property(e => e.EanCode).HasMaxLength(13);

                entity.Property(e => e.Flute).HasMaxLength(3);

                entity.Property(e => e.Gl)
                    .HasColumnName("GL")
                    .HasMaxLength(3);

                entity.Property(e => e.Glweigth).HasColumnName("GLWeigth");

                entity.Property(e => e.Hierarchy)
                    .IsRequired()
                    .HasMaxLength(18);

                entity.Property(e => e.HighGroup)
                    .HasColumnName("High_Group")
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.HighValue)
                    .HasColumnName("High_Value")
                    .HasMaxLength(4)
                    .IsUnicode(false);

                entity.Property(e => e.IndDes)
                    .HasColumnName("Ind_Des")
                    .HasMaxLength(50);

                entity.Property(e => e.IndGrp)
                    .HasColumnName("Ind_Grp")
                    .HasMaxLength(3);

                entity.Property(e => e.JoinType).HasMaxLength(60);

                entity.Property(e => e.Language)
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.LastUpdate).HasColumnType("smalldatetime");

                entity.Property(e => e.MaterialNo)
                    .IsRequired()
                    .HasColumnName("Material_No")
                    .HasMaxLength(18);

                entity.Property(e => e.MaterialType)
                    .HasColumnName("Material_Type")
                    .HasMaxLength(2);

                entity.Property(e => e.NoSlot).HasColumnName("No_Slot");

                entity.Property(e => e.OuterJoin).HasColumnName("Outer_Join");

                entity.Property(e => e.PalletSize).HasMaxLength(10);

                entity.Property(e => e.PalletizationPath)
                    .HasColumnName("Palletization_Path")
                    .HasMaxLength(50);

                entity.Property(e => e.PartNo)
                    .HasColumnName("Part_No")
                    .HasMaxLength(22);

                entity.Property(e => e.Pc)
                    .IsRequired()
                    .HasColumnName("PC")
                    .HasMaxLength(15);

                entity.Property(e => e.PdisStatus)
                    .IsRequired()
                    .HasColumnName("PDIS_Status")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.PieceSet).HasColumnName("Piece_Set");

                entity.Property(e => e.Plant).HasMaxLength(5);

                entity.Property(e => e.PltAxleHeight).HasColumnName("Plt_Axle_Height");

                entity.Property(e => e.PltBeam).HasColumnName("Plt_Beam");

                entity.Property(e => e.PltDoubleAxle).HasColumnName("Plt_Double_axle");

                entity.Property(e => e.PltFloorAbove).HasColumnName("Plt_Floor_Above");

                entity.Property(e => e.PltFloorUnder).HasColumnName("Plt_Floor_Under");

                entity.Property(e => e.PltLegDouble).HasColumnName("Plt_Leg_Double");

                entity.Property(e => e.PltLegSingle).HasColumnName("Plt_Leg_Single");

                entity.Property(e => e.PltSingleAxle).HasColumnName("Plt_Single_axle");

                entity.Property(e => e.PrintMasterPath)
                    .HasColumnName("PrintMaster_Path")
                    .HasMaxLength(50);

                entity.Property(e => e.PrintMethod)
                    .HasColumnName("Print_Method")
                    .HasMaxLength(50);

                entity.Property(e => e.PriorityFlag)
                    .HasColumnName("Priority_Flag")
                    .HasMaxLength(2);

                entity.Property(e => e.ProType)
                    .HasColumnName("Pro_Type")
                    .HasMaxLength(60);

                entity.Property(e => e.PsmStatus)
                    .HasColumnName("PSM_Status")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.PurTxt1)
                    .HasColumnName("Pur_Txt1")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.PurTxt2)
                    .HasColumnName("Pur_Txt2")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.PurTxt3)
                    .HasColumnName("Pur_Txt3")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.PurTxt4)
                    .HasColumnName("Pur_Txt4")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.RscStyle)
                    .HasColumnName("RSC_Style")
                    .HasMaxLength(50);

                entity.Property(e => e.SaleOrg)
                    .IsRequired()
                    .HasColumnName("Sale_Org")
                    .HasMaxLength(5);

                entity.Property(e => e.SaleText1)
                    .HasColumnName("Sale_Text1")
                    .HasMaxLength(40);

                entity.Property(e => e.SaleText2)
                    .HasColumnName("Sale_Text2")
                    .HasMaxLength(40);

                entity.Property(e => e.SaleText3)
                    .HasColumnName("Sale_Text3")
                    .HasMaxLength(40);

                entity.Property(e => e.SaleText4)
                    .HasColumnName("Sale_Text4")
                    .HasMaxLength(40);

                entity.Property(e => e.SaleUom)
                    .HasColumnName("Sale_UOM")
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.SapStatus).HasColumnName("SAP_Status");

                entity.Property(e => e.SheetArea).HasColumnName("Sheet_Area");

                entity.Property(e => e.StatusFlag)
                    .HasColumnName("Status_Flag")
                    .HasMaxLength(30);

                entity.Property(e => e.TranStatus).HasColumnName("Tran_Status");

                entity.Property(e => e.UnUpgradBoard).HasColumnName("UnUpgrad_Board");

                entity.Property(e => e.User)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.WeightBox).HasColumnName("Weight_Box");

                entity.Property(e => e.WeightSh).HasColumnName("Weight_Sh");
            });

            modelBuilder.Entity<TcpbRouting>(entity =>
            {
                entity.HasKey(e => new { e.PsmId, e.SeqNo });

                entity.ToTable("TCPB_Routing");

                entity.Property(e => e.PsmId)
                    .HasColumnName("PSM_ID")
                    .HasMaxLength(50);

                entity.Property(e => e.SeqNo).HasColumnName("Seq_No");

                entity.Property(e => e.Alternative1)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Alternative2)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Alternative3).HasMaxLength(50);

                entity.Property(e => e.Alternative4).HasMaxLength(50);

                entity.Property(e => e.Alternative5).HasMaxLength(50);

                entity.Property(e => e.Alternative6).HasMaxLength(50);

                entity.Property(e => e.Alternative7).HasMaxLength(50);

                entity.Property(e => e.Alternative8).HasMaxLength(50);

                entity.Property(e => e.BlockNo)
                    .HasColumnName("Block_No")
                    .HasMaxLength(50);

                entity.Property(e => e.Color1).HasMaxLength(20);

                entity.Property(e => e.Color2).HasMaxLength(20);

                entity.Property(e => e.Color3).HasMaxLength(20);

                entity.Property(e => e.Color4).HasMaxLength(20);

                entity.Property(e => e.Color5).HasMaxLength(20);

                entity.Property(e => e.Color6).HasMaxLength(20);

                entity.Property(e => e.Color7).HasMaxLength(20);

                entity.Property(e => e.ColorArea1).HasColumnName("Color_Area1");

                entity.Property(e => e.ColorArea2).HasColumnName("Color_Area2");

                entity.Property(e => e.ColorArea3).HasColumnName("Color_Area3");

                entity.Property(e => e.ColorArea4).HasColumnName("Color_Area4");

                entity.Property(e => e.ColorArea5).HasColumnName("Color_Area5");

                entity.Property(e => e.ColorArea6).HasColumnName("Color_Area6");

                entity.Property(e => e.ColorArea7).HasColumnName("Color_Area7");

                entity.Property(e => e.ColorCount).HasColumnName("Color_count");

                entity.Property(e => e.ColourCount).HasColumnName("Colour_Count");

                entity.Property(e => e.CutNo).HasColumnName("Cut_No");

                entity.Property(e => e.JoinMatNo)
                    .HasColumnName("Join_Mat_no")
                    .HasMaxLength(18);

                entity.Property(e => e.Machine).HasMaxLength(50);

                entity.Property(e => e.MatCode)
                    .HasColumnName("Mat_Code")
                    .HasMaxLength(20);

                entity.Property(e => e.MaterialNo)
                    .IsRequired()
                    .HasColumnName("Material_No")
                    .HasMaxLength(18);

                entity.Property(e => e.McMove).HasColumnName("MC_Move");

                entity.Property(e => e.MylaNo)
                    .HasColumnName("Myla_No")
                    .HasMaxLength(50);

                entity.Property(e => e.NoOpenIn).HasColumnName("No_Open_in");

                entity.Property(e => e.NoOpenOut).HasColumnName("No_Open_out");

                entity.Property(e => e.NoneBlk).HasColumnName("None_Blk");

                entity.Property(e => e.PaperWidth).HasColumnName("Paper_Width");

                entity.Property(e => e.PdisStatus)
                    .HasColumnName("PDIS_Status")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.PlanCode)
                    .HasColumnName("Plan_Code")
                    .HasMaxLength(10);

                entity.Property(e => e.Plant).HasMaxLength(5);

                entity.Property(e => e.PlateNo)
                    .HasColumnName("Plate_No")
                    .HasMaxLength(50);

                entity.Property(e => e.PostTm).HasColumnName("Post_tm");

                entity.Property(e => e.PrepareTm).HasColumnName("Prepare_tm");

                entity.Property(e => e.PsmStatus)
                    .HasColumnName("PSM_Status")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.RemarkInprocess)
                    .HasColumnName("Remark_Inprocess")
                    .HasMaxLength(255);

                entity.Property(e => e.RotateIn).HasColumnName("Rotate_In");

                entity.Property(e => e.RotateOut).HasColumnName("Rotate_Out");

                entity.Property(e => e.RunWaste).HasColumnName("Run_waste");

                entity.Property(e => e.SapStatus).HasColumnName("SAP_Status");

                entity.Property(e => e.SemiBlk).HasColumnName("Semi_Blk");

                entity.Property(e => e.SeparatMatNo)
                    .HasColumnName("Separat_Mat_no")
                    .HasMaxLength(18);

                entity.Property(e => e.SetupTm).HasColumnName("Setup_tm");

                entity.Property(e => e.SetupWaste).HasColumnName("Setup_waste");

                entity.Property(e => e.Shade1).HasMaxLength(10);

                entity.Property(e => e.Shade2).HasMaxLength(10);

                entity.Property(e => e.Shade3).HasMaxLength(10);

                entity.Property(e => e.Shade4).HasMaxLength(10);

                entity.Property(e => e.Shade5).HasMaxLength(10);

                entity.Property(e => e.Shade6).HasMaxLength(10);

                entity.Property(e => e.Shade7).HasMaxLength(10);

                entity.Property(e => e.SheetInLeg).HasColumnName("Sheet_in_Leg");

                entity.Property(e => e.SheetInWid).HasColumnName("Sheet_in_Wid");

                entity.Property(e => e.SheetOutLeg).HasColumnName("Sheet_out_Leg");

                entity.Property(e => e.SheetOutWid).HasColumnName("Sheet_out_Wid");

                entity.Property(e => e.ShipBlk).HasColumnName("Ship_Blk");

                entity.Property(e => e.StackHeight).HasColumnName("Stack_Height");

                entity.Property(e => e.StanBlk).HasColumnName("Stan_Blk");

                entity.Property(e => e.StdProcess).HasColumnName("Std_Process");

                entity.Property(e => e.TranStatus).HasColumnName("Tran_Status");

                entity.Property(e => e.UnUpgradBoard)
                    .HasColumnName("UnUpgrad_Board")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.WasteLeg).HasColumnName("Waste_Leg");

                entity.Property(e => e.WasteWid).HasColumnName("Waste_Wid");

                entity.Property(e => e.WeightIn).HasColumnName("Weight_in");

                entity.Property(e => e.WeightOut).HasColumnName("Weight_out");
            });

            modelBuilder.Entity<TcptMasterData>(entity =>
            {
                entity.HasKey(e => e.PsmId);

                entity.ToTable("TCPT_MasterData");

                entity.Property(e => e.PsmId)
                    .HasColumnName("PSM_ID")
                    .HasMaxLength(50)
                    .ValueGeneratedNever();

                entity.Property(e => e.Bl)
                    .HasColumnName("BL")
                    .HasMaxLength(3);

                entity.Property(e => e.Blweigth).HasColumnName("BLWeigth");

                entity.Property(e => e.Bm)
                    .HasColumnName("BM")
                    .HasMaxLength(3);

                entity.Property(e => e.Bmweigth).HasColumnName("BMWeigth");

                entity.Property(e => e.Board).HasMaxLength(50);

                entity.Property(e => e.BomUom)
                    .HasColumnName("BOM_UOM")
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.BoxArea).HasColumnName("Box_Area");

                entity.Property(e => e.BoxType)
                    .HasColumnName("Box_Type")
                    .HasMaxLength(60);

                entity.Property(e => e.Change).HasMaxLength(100);

                entity.Property(e => e.Cl)
                    .HasColumnName("CL")
                    .HasMaxLength(3);

                entity.Property(e => e.Clweigth).HasColumnName("CLWeigth");

                entity.Property(e => e.Cm)
                    .HasColumnName("CM")
                    .HasMaxLength(3);

                entity.Property(e => e.Cmweigth).HasColumnName("CMWeigth");

                entity.Property(e => e.Code).HasMaxLength(20);

                entity.Property(e => e.CreateDate).HasColumnType("smalldatetime");

                entity.Property(e => e.CusId)
                    .HasColumnName("Cus_ID")
                    .HasMaxLength(10);

                entity.Property(e => e.CustCode)
                    .HasColumnName("Cust_Code")
                    .HasMaxLength(10);

                entity.Property(e => e.CustName)
                    .HasColumnName("Cust_Name")
                    .HasMaxLength(50);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(40);

                entity.Property(e => e.DiecutPictPath)
                    .HasColumnName("DiecutPict_Path")
                    .HasMaxLength(50);

                entity.Property(e => e.Dl)
                    .HasColumnName("DL")
                    .HasMaxLength(3);

                entity.Property(e => e.Dlweigth).HasColumnName("DLWeigth");

                entity.Property(e => e.Dm)
                    .HasColumnName("DM")
                    .HasMaxLength(3);

                entity.Property(e => e.Dmweigth).HasColumnName("DMWeigth");

                entity.Property(e => e.EanCode).HasMaxLength(13);

                entity.Property(e => e.Flute).HasMaxLength(3);

                entity.Property(e => e.Gl)
                    .HasColumnName("GL")
                    .HasMaxLength(3);

                entity.Property(e => e.Glweigth).HasColumnName("GLWeigth");

                entity.Property(e => e.Hierarchy)
                    .IsRequired()
                    .HasMaxLength(18);

                entity.Property(e => e.HighGroup)
                    .HasColumnName("High_Group")
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.HighValue)
                    .HasColumnName("High_Value")
                    .HasMaxLength(4)
                    .IsUnicode(false);

                entity.Property(e => e.IndDes)
                    .HasColumnName("Ind_Des")
                    .HasMaxLength(50);

                entity.Property(e => e.IndGrp)
                    .HasColumnName("Ind_Grp")
                    .HasMaxLength(3);

                entity.Property(e => e.JoinType).HasMaxLength(60);

                entity.Property(e => e.Language)
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.LastUpdate).HasColumnType("smalldatetime");

                entity.Property(e => e.MaterialNo)
                    .IsRequired()
                    .HasColumnName("Material_No")
                    .HasMaxLength(18);

                entity.Property(e => e.MaterialType)
                    .HasColumnName("Material_Type")
                    .HasMaxLength(2);

                entity.Property(e => e.NoSlot).HasColumnName("No_Slot");

                entity.Property(e => e.OuterJoin).HasColumnName("Outer_Join");

                entity.Property(e => e.PalletSize).HasMaxLength(10);

                entity.Property(e => e.PalletizationPath)
                    .HasColumnName("Palletization_Path")
                    .HasMaxLength(50);

                entity.Property(e => e.PartNo)
                    .HasColumnName("Part_No")
                    .HasMaxLength(22);

                entity.Property(e => e.Pc)
                    .IsRequired()
                    .HasColumnName("PC")
                    .HasMaxLength(15);

                entity.Property(e => e.PdisStatus)
                    .IsRequired()
                    .HasColumnName("PDIS_Status")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.PieceSet).HasColumnName("Piece_Set");

                entity.Property(e => e.Plant).HasMaxLength(5);

                entity.Property(e => e.PltAxleHeight).HasColumnName("Plt_Axle_Height");

                entity.Property(e => e.PltBeam).HasColumnName("Plt_Beam");

                entity.Property(e => e.PltDoubleAxle).HasColumnName("Plt_Double_axle");

                entity.Property(e => e.PltFloorAbove).HasColumnName("Plt_Floor_Above");

                entity.Property(e => e.PltFloorUnder).HasColumnName("Plt_Floor_Under");

                entity.Property(e => e.PltLegDouble).HasColumnName("Plt_Leg_Double");

                entity.Property(e => e.PltLegSingle).HasColumnName("Plt_Leg_Single");

                entity.Property(e => e.PltSingleAxle).HasColumnName("Plt_Single_axle");

                entity.Property(e => e.PrintMasterPath)
                    .HasColumnName("PrintMaster_Path")
                    .HasMaxLength(50);

                entity.Property(e => e.PrintMethod)
                    .HasColumnName("Print_Method")
                    .HasMaxLength(50);

                entity.Property(e => e.PriorityFlag)
                    .HasColumnName("Priority_Flag")
                    .HasMaxLength(2);

                entity.Property(e => e.ProType)
                    .HasColumnName("Pro_Type")
                    .HasMaxLength(60);

                entity.Property(e => e.PsmStatus)
                    .HasColumnName("PSM_Status")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.PurTxt1)
                    .HasColumnName("Pur_Txt1")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.PurTxt2)
                    .HasColumnName("Pur_Txt2")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.PurTxt3)
                    .HasColumnName("Pur_Txt3")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.PurTxt4)
                    .HasColumnName("Pur_Txt4")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.RscStyle)
                    .HasColumnName("RSC_Style")
                    .HasMaxLength(50);

                entity.Property(e => e.SaleOrg)
                    .IsRequired()
                    .HasColumnName("Sale_Org")
                    .HasMaxLength(5);

                entity.Property(e => e.SaleText1)
                    .HasColumnName("Sale_Text1")
                    .HasMaxLength(40);

                entity.Property(e => e.SaleText2)
                    .HasColumnName("Sale_Text2")
                    .HasMaxLength(40);

                entity.Property(e => e.SaleText3)
                    .HasColumnName("Sale_Text3")
                    .HasMaxLength(40);

                entity.Property(e => e.SaleText4)
                    .HasColumnName("Sale_Text4")
                    .HasMaxLength(40);

                entity.Property(e => e.SaleUom)
                    .HasColumnName("Sale_UOM")
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.SapStatus).HasColumnName("SAP_Status");

                entity.Property(e => e.SheetArea).HasColumnName("Sheet_Area");

                entity.Property(e => e.StatusFlag)
                    .HasColumnName("Status_Flag")
                    .HasMaxLength(30);

                entity.Property(e => e.TranStatus).HasColumnName("Tran_Status");

                entity.Property(e => e.UnUpgradBoard).HasColumnName("UnUpgrad_Board");

                entity.Property(e => e.User)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.WeightBox).HasColumnName("Weight_Box");

                entity.Property(e => e.WeightSh).HasColumnName("Weight_Sh");
            });

            modelBuilder.Entity<TcptRouting>(entity =>
            {
                entity.HasKey(e => new { e.PsmId, e.SeqNo });

                entity.ToTable("TCPT_Routing");

                entity.Property(e => e.PsmId)
                    .HasColumnName("PSM_ID")
                    .HasMaxLength(50);

                entity.Property(e => e.SeqNo).HasColumnName("Seq_No");

                entity.Property(e => e.Alternative1)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Alternative2)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Alternative3).HasMaxLength(50);

                entity.Property(e => e.Alternative4).HasMaxLength(50);

                entity.Property(e => e.Alternative5).HasMaxLength(50);

                entity.Property(e => e.Alternative6).HasMaxLength(50);

                entity.Property(e => e.Alternative7).HasMaxLength(50);

                entity.Property(e => e.Alternative8).HasMaxLength(50);

                entity.Property(e => e.BlockNo)
                    .HasColumnName("Block_No")
                    .HasMaxLength(50);

                entity.Property(e => e.Color1).HasMaxLength(20);

                entity.Property(e => e.Color2).HasMaxLength(20);

                entity.Property(e => e.Color3).HasMaxLength(20);

                entity.Property(e => e.Color4).HasMaxLength(20);

                entity.Property(e => e.Color5).HasMaxLength(20);

                entity.Property(e => e.Color6).HasMaxLength(20);

                entity.Property(e => e.Color7).HasMaxLength(20);

                entity.Property(e => e.ColorArea1).HasColumnName("Color_Area1");

                entity.Property(e => e.ColorArea2).HasColumnName("Color_Area2");

                entity.Property(e => e.ColorArea3).HasColumnName("Color_Area3");

                entity.Property(e => e.ColorArea4).HasColumnName("Color_Area4");

                entity.Property(e => e.ColorArea5).HasColumnName("Color_Area5");

                entity.Property(e => e.ColorArea6).HasColumnName("Color_Area6");

                entity.Property(e => e.ColorArea7).HasColumnName("Color_Area7");

                entity.Property(e => e.ColorCount).HasColumnName("Color_count");

                entity.Property(e => e.ColourCount).HasColumnName("Colour_Count");

                entity.Property(e => e.CutNo).HasColumnName("Cut_No");

                entity.Property(e => e.JoinMatNo)
                    .HasColumnName("Join_Mat_no")
                    .HasMaxLength(18);

                entity.Property(e => e.Machine).HasMaxLength(50);

                entity.Property(e => e.MatCode)
                    .HasColumnName("Mat_Code")
                    .HasMaxLength(20);

                entity.Property(e => e.MaterialNo)
                    .IsRequired()
                    .HasColumnName("Material_No")
                    .HasMaxLength(18);

                entity.Property(e => e.McMove).HasColumnName("MC_Move");

                entity.Property(e => e.MylaNo)
                    .HasColumnName("Myla_No")
                    .HasMaxLength(50);

                entity.Property(e => e.NoOpenIn).HasColumnName("No_Open_in");

                entity.Property(e => e.NoOpenOut).HasColumnName("No_Open_out");

                entity.Property(e => e.NoneBlk).HasColumnName("None_Blk");

                entity.Property(e => e.PaperWidth).HasColumnName("Paper_Width");

                entity.Property(e => e.PdisStatus)
                    .HasColumnName("PDIS_Status")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.PlanCode)
                    .HasColumnName("Plan_Code")
                    .HasMaxLength(10);

                entity.Property(e => e.Plant).HasMaxLength(5);

                entity.Property(e => e.PlateNo)
                    .HasColumnName("Plate_No")
                    .HasMaxLength(50);

                entity.Property(e => e.PostTm).HasColumnName("Post_tm");

                entity.Property(e => e.PrepareTm).HasColumnName("Prepare_tm");

                entity.Property(e => e.PsmStatus)
                    .HasColumnName("PSM_Status")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.RemarkInprocess)
                    .HasColumnName("Remark_Inprocess")
                    .HasMaxLength(255);

                entity.Property(e => e.RotateIn).HasColumnName("Rotate_In");

                entity.Property(e => e.RotateOut).HasColumnName("Rotate_Out");

                entity.Property(e => e.RunWaste).HasColumnName("Run_waste");

                entity.Property(e => e.SapStatus).HasColumnName("SAP_Status");

                entity.Property(e => e.SemiBlk).HasColumnName("Semi_Blk");

                entity.Property(e => e.SeparatMatNo)
                    .HasColumnName("Separat_Mat_no")
                    .HasMaxLength(18);

                entity.Property(e => e.SetupTm).HasColumnName("Setup_tm");

                entity.Property(e => e.SetupWaste).HasColumnName("Setup_waste");

                entity.Property(e => e.Shade1).HasMaxLength(10);

                entity.Property(e => e.Shade2).HasMaxLength(10);

                entity.Property(e => e.Shade3).HasMaxLength(10);

                entity.Property(e => e.Shade4).HasMaxLength(10);

                entity.Property(e => e.Shade5).HasMaxLength(10);

                entity.Property(e => e.Shade6).HasMaxLength(10);

                entity.Property(e => e.Shade7).HasMaxLength(10);

                entity.Property(e => e.SheetInLeg).HasColumnName("Sheet_in_Leg");

                entity.Property(e => e.SheetInWid).HasColumnName("Sheet_in_Wid");

                entity.Property(e => e.SheetOutLeg).HasColumnName("Sheet_out_Leg");

                entity.Property(e => e.SheetOutWid).HasColumnName("Sheet_out_Wid");

                entity.Property(e => e.ShipBlk).HasColumnName("Ship_Blk");

                entity.Property(e => e.StackHeight).HasColumnName("Stack_Height");

                entity.Property(e => e.StanBlk).HasColumnName("Stan_Blk");

                entity.Property(e => e.StdProcess).HasColumnName("Std_Process");

                entity.Property(e => e.TranStatus).HasColumnName("Tran_Status");

                entity.Property(e => e.UnUpgradBoard)
                    .HasColumnName("UnUpgrad_Board")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.WasteLeg).HasColumnName("Waste_Leg");

                entity.Property(e => e.WasteWid).HasColumnName("Waste_Wid");

                entity.Property(e => e.WeightIn).HasColumnName("Weight_in");

                entity.Property(e => e.WeightOut).HasColumnName("Weight_out");
            });

            modelBuilder.Entity<TcrbMasterData>(entity =>
            {
                entity.HasKey(e => e.PsmId);

                entity.ToTable("TCRB_MasterData");

                entity.Property(e => e.PsmId)
                    .HasColumnName("PSM_ID")
                    .HasMaxLength(50)
                    .ValueGeneratedNever();

                entity.Property(e => e.Bl)
                    .HasColumnName("BL")
                    .HasMaxLength(3);

                entity.Property(e => e.Blweigth).HasColumnName("BLWeigth");

                entity.Property(e => e.Bm)
                    .HasColumnName("BM")
                    .HasMaxLength(3);

                entity.Property(e => e.Bmweigth).HasColumnName("BMWeigth");

                entity.Property(e => e.Board).HasMaxLength(50);

                entity.Property(e => e.BomUom)
                    .HasColumnName("BOM_UOM")
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.BoxArea).HasColumnName("Box_Area");

                entity.Property(e => e.BoxType)
                    .HasColumnName("Box_Type")
                    .HasMaxLength(60);

                entity.Property(e => e.Change).HasMaxLength(100);

                entity.Property(e => e.Cl)
                    .HasColumnName("CL")
                    .HasMaxLength(3);

                entity.Property(e => e.Clweigth).HasColumnName("CLWeigth");

                entity.Property(e => e.Cm)
                    .HasColumnName("CM")
                    .HasMaxLength(3);

                entity.Property(e => e.Cmweigth).HasColumnName("CMWeigth");

                entity.Property(e => e.Code).HasMaxLength(20);

                entity.Property(e => e.CreateDate).HasColumnType("smalldatetime");

                entity.Property(e => e.CusId)
                    .HasColumnName("Cus_ID")
                    .HasMaxLength(10);

                entity.Property(e => e.CustCode)
                    .HasColumnName("Cust_Code")
                    .HasMaxLength(10);

                entity.Property(e => e.CustName)
                    .HasColumnName("Cust_Name")
                    .HasMaxLength(50);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(40);

                entity.Property(e => e.DiecutPictPath)
                    .HasColumnName("DiecutPict_Path")
                    .HasMaxLength(50);

                entity.Property(e => e.Dl)
                    .HasColumnName("DL")
                    .HasMaxLength(3);

                entity.Property(e => e.Dlweigth).HasColumnName("DLWeigth");

                entity.Property(e => e.Dm)
                    .HasColumnName("DM")
                    .HasMaxLength(3);

                entity.Property(e => e.Dmweigth).HasColumnName("DMWeigth");

                entity.Property(e => e.EanCode).HasMaxLength(13);

                entity.Property(e => e.Flute).HasMaxLength(3);

                entity.Property(e => e.Gl)
                    .HasColumnName("GL")
                    .HasMaxLength(3);

                entity.Property(e => e.Glweigth).HasColumnName("GLWeigth");

                entity.Property(e => e.Hierarchy)
                    .IsRequired()
                    .HasMaxLength(18);

                entity.Property(e => e.HighGroup)
                    .HasColumnName("High_Group")
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.HighValue)
                    .HasColumnName("High_Value")
                    .HasMaxLength(4)
                    .IsUnicode(false);

                entity.Property(e => e.IndDes)
                    .HasColumnName("Ind_Des")
                    .HasMaxLength(50);

                entity.Property(e => e.IndGrp)
                    .HasColumnName("Ind_Grp")
                    .HasMaxLength(3);

                entity.Property(e => e.JoinType).HasMaxLength(60);

                entity.Property(e => e.Language)
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.LastUpdate).HasColumnType("smalldatetime");

                entity.Property(e => e.MaterialNo)
                    .IsRequired()
                    .HasColumnName("Material_No")
                    .HasMaxLength(18);

                entity.Property(e => e.MaterialType)
                    .HasColumnName("Material_Type")
                    .HasMaxLength(2);

                entity.Property(e => e.NoSlot).HasColumnName("No_Slot");

                entity.Property(e => e.OuterJoin).HasColumnName("Outer_Join");

                entity.Property(e => e.PalletSize).HasMaxLength(10);

                entity.Property(e => e.PalletizationPath)
                    .HasColumnName("Palletization_Path")
                    .HasMaxLength(50);

                entity.Property(e => e.PartNo)
                    .HasColumnName("Part_No")
                    .HasMaxLength(22);

                entity.Property(e => e.Pc)
                    .IsRequired()
                    .HasColumnName("PC")
                    .HasMaxLength(15);

                entity.Property(e => e.PdisStatus)
                    .IsRequired()
                    .HasColumnName("PDIS_Status")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.PieceSet).HasColumnName("Piece_Set");

                entity.Property(e => e.Plant).HasMaxLength(5);

                entity.Property(e => e.PltAxleHeight).HasColumnName("Plt_Axle_Height");

                entity.Property(e => e.PltBeam).HasColumnName("Plt_Beam");

                entity.Property(e => e.PltDoubleAxle).HasColumnName("Plt_Double_axle");

                entity.Property(e => e.PltFloorAbove).HasColumnName("Plt_Floor_Above");

                entity.Property(e => e.PltFloorUnder).HasColumnName("Plt_Floor_Under");

                entity.Property(e => e.PltLegDouble).HasColumnName("Plt_Leg_Double");

                entity.Property(e => e.PltLegSingle).HasColumnName("Plt_Leg_Single");

                entity.Property(e => e.PltSingleAxle).HasColumnName("Plt_Single_axle");

                entity.Property(e => e.PrintMasterPath)
                    .HasColumnName("PrintMaster_Path")
                    .HasMaxLength(50);

                entity.Property(e => e.PrintMethod)
                    .HasColumnName("Print_Method")
                    .HasMaxLength(50);

                entity.Property(e => e.PriorityFlag)
                    .HasColumnName("Priority_Flag")
                    .HasMaxLength(2);

                entity.Property(e => e.ProType)
                    .HasColumnName("Pro_Type")
                    .HasMaxLength(60);

                entity.Property(e => e.PsmStatus)
                    .HasColumnName("PSM_Status")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.PurTxt1)
                    .HasColumnName("Pur_Txt1")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.PurTxt2)
                    .HasColumnName("Pur_Txt2")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.PurTxt3)
                    .HasColumnName("Pur_Txt3")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.PurTxt4)
                    .HasColumnName("Pur_Txt4")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.RscStyle)
                    .HasColumnName("RSC_Style")
                    .HasMaxLength(50);

                entity.Property(e => e.SaleOrg)
                    .IsRequired()
                    .HasColumnName("Sale_Org")
                    .HasMaxLength(5);

                entity.Property(e => e.SaleText1)
                    .HasColumnName("Sale_Text1")
                    .HasMaxLength(40);

                entity.Property(e => e.SaleText2)
                    .HasColumnName("Sale_Text2")
                    .HasMaxLength(40);

                entity.Property(e => e.SaleText3)
                    .HasColumnName("Sale_Text3")
                    .HasMaxLength(40);

                entity.Property(e => e.SaleText4)
                    .HasColumnName("Sale_Text4")
                    .HasMaxLength(40);

                entity.Property(e => e.SaleUom)
                    .HasColumnName("Sale_UOM")
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.SapStatus).HasColumnName("SAP_Status");

                entity.Property(e => e.SheetArea).HasColumnName("Sheet_Area");

                entity.Property(e => e.StatusFlag)
                    .HasColumnName("Status_Flag")
                    .HasMaxLength(30);

                entity.Property(e => e.TranStatus).HasColumnName("Tran_Status");

                entity.Property(e => e.UnUpgradBoard).HasColumnName("UnUpgrad_Board");

                entity.Property(e => e.User)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.WeightBox).HasColumnName("Weight_Box");

                entity.Property(e => e.WeightSh).HasColumnName("Weight_Sh");
            });

            modelBuilder.Entity<TcrbRouting>(entity =>
            {
                entity.HasKey(e => new { e.PsmId, e.SeqNo });

                entity.ToTable("TCRB_Routing");

                entity.Property(e => e.PsmId)
                    .HasColumnName("PSM_ID")
                    .HasMaxLength(50);

                entity.Property(e => e.SeqNo).HasColumnName("Seq_No");

                entity.Property(e => e.Alternative1)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Alternative2)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Alternative3).HasMaxLength(50);

                entity.Property(e => e.Alternative4).HasMaxLength(50);

                entity.Property(e => e.Alternative5).HasMaxLength(50);

                entity.Property(e => e.Alternative6).HasMaxLength(50);

                entity.Property(e => e.Alternative7).HasMaxLength(50);

                entity.Property(e => e.Alternative8).HasMaxLength(50);

                entity.Property(e => e.BlockNo)
                    .HasColumnName("Block_No")
                    .HasMaxLength(50);

                entity.Property(e => e.Color1).HasMaxLength(20);

                entity.Property(e => e.Color2).HasMaxLength(20);

                entity.Property(e => e.Color3).HasMaxLength(20);

                entity.Property(e => e.Color4).HasMaxLength(20);

                entity.Property(e => e.Color5).HasMaxLength(20);

                entity.Property(e => e.Color6).HasMaxLength(20);

                entity.Property(e => e.Color7).HasMaxLength(20);

                entity.Property(e => e.ColorArea1).HasColumnName("Color_Area1");

                entity.Property(e => e.ColorArea2).HasColumnName("Color_Area2");

                entity.Property(e => e.ColorArea3).HasColumnName("Color_Area3");

                entity.Property(e => e.ColorArea4).HasColumnName("Color_Area4");

                entity.Property(e => e.ColorArea5).HasColumnName("Color_Area5");

                entity.Property(e => e.ColorArea6).HasColumnName("Color_Area6");

                entity.Property(e => e.ColorArea7).HasColumnName("Color_Area7");

                entity.Property(e => e.ColorCount).HasColumnName("Color_count");

                entity.Property(e => e.ColourCount).HasColumnName("Colour_Count");

                entity.Property(e => e.CutNo).HasColumnName("Cut_No");

                entity.Property(e => e.JoinMatNo)
                    .HasColumnName("Join_Mat_no")
                    .HasMaxLength(18);

                entity.Property(e => e.Machine).HasMaxLength(50);

                entity.Property(e => e.MatCode)
                    .HasColumnName("Mat_Code")
                    .HasMaxLength(20);

                entity.Property(e => e.MaterialNo)
                    .IsRequired()
                    .HasColumnName("Material_No")
                    .HasMaxLength(18);

                entity.Property(e => e.McMove).HasColumnName("MC_Move");

                entity.Property(e => e.MylaNo)
                    .HasColumnName("Myla_No")
                    .HasMaxLength(50);

                entity.Property(e => e.NoOpenIn).HasColumnName("No_Open_in");

                entity.Property(e => e.NoOpenOut).HasColumnName("No_Open_out");

                entity.Property(e => e.NoneBlk).HasColumnName("None_Blk");

                entity.Property(e => e.PaperWidth).HasColumnName("Paper_Width");

                entity.Property(e => e.PdisStatus)
                    .HasColumnName("PDIS_Status")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.PlanCode)
                    .HasColumnName("Plan_Code")
                    .HasMaxLength(10);

                entity.Property(e => e.Plant).HasMaxLength(5);

                entity.Property(e => e.PlateNo)
                    .HasColumnName("Plate_No")
                    .HasMaxLength(50);

                entity.Property(e => e.PostTm).HasColumnName("Post_tm");

                entity.Property(e => e.PrepareTm).HasColumnName("Prepare_tm");

                entity.Property(e => e.PsmStatus)
                    .HasColumnName("PSM_Status")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.RemarkInprocess)
                    .HasColumnName("Remark_Inprocess")
                    .HasMaxLength(255);

                entity.Property(e => e.RotateIn).HasColumnName("Rotate_In");

                entity.Property(e => e.RotateOut).HasColumnName("Rotate_Out");

                entity.Property(e => e.RunWaste).HasColumnName("Run_waste");

                entity.Property(e => e.SapStatus).HasColumnName("SAP_Status");

                entity.Property(e => e.SemiBlk).HasColumnName("Semi_Blk");

                entity.Property(e => e.SeparatMatNo)
                    .HasColumnName("Separat_Mat_no")
                    .HasMaxLength(18);

                entity.Property(e => e.SetupTm).HasColumnName("Setup_tm");

                entity.Property(e => e.SetupWaste).HasColumnName("Setup_waste");

                entity.Property(e => e.Shade1).HasMaxLength(10);

                entity.Property(e => e.Shade2).HasMaxLength(10);

                entity.Property(e => e.Shade3).HasMaxLength(10);

                entity.Property(e => e.Shade4).HasMaxLength(10);

                entity.Property(e => e.Shade5).HasMaxLength(10);

                entity.Property(e => e.Shade6).HasMaxLength(10);

                entity.Property(e => e.Shade7).HasMaxLength(10);

                entity.Property(e => e.SheetInLeg).HasColumnName("Sheet_in_Leg");

                entity.Property(e => e.SheetInWid).HasColumnName("Sheet_in_Wid");

                entity.Property(e => e.SheetOutLeg).HasColumnName("Sheet_out_Leg");

                entity.Property(e => e.SheetOutWid).HasColumnName("Sheet_out_Wid");

                entity.Property(e => e.ShipBlk).HasColumnName("Ship_Blk");

                entity.Property(e => e.StackHeight).HasColumnName("Stack_Height");

                entity.Property(e => e.StanBlk).HasColumnName("Stan_Blk");

                entity.Property(e => e.StdProcess).HasColumnName("Std_Process");

                entity.Property(e => e.TranStatus).HasColumnName("Tran_Status");

                entity.Property(e => e.UnUpgradBoard)
                    .HasColumnName("UnUpgrad_Board")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.WasteLeg).HasColumnName("Waste_Leg");

                entity.Property(e => e.WasteWid).HasColumnName("Waste_Wid");

                entity.Property(e => e.WeightIn).HasColumnName("Weight_in");

                entity.Property(e => e.WeightOut).HasColumnName("Weight_out");
            });

            modelBuilder.Entity<TcryMasterData>(entity =>
            {
                entity.HasKey(e => e.PsmId);

                entity.ToTable("TCRY_MasterData");

                entity.Property(e => e.PsmId)
                    .HasColumnName("PSM_ID")
                    .HasMaxLength(50)
                    .ValueGeneratedNever();

                entity.Property(e => e.Bl)
                    .HasColumnName("BL")
                    .HasMaxLength(3);

                entity.Property(e => e.Blweigth).HasColumnName("BLWeigth");

                entity.Property(e => e.Bm)
                    .HasColumnName("BM")
                    .HasMaxLength(3);

                entity.Property(e => e.Bmweigth).HasColumnName("BMWeigth");

                entity.Property(e => e.Board).HasMaxLength(50);

                entity.Property(e => e.BomUom)
                    .HasColumnName("BOM_UOM")
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.BoxArea).HasColumnName("Box_Area");

                entity.Property(e => e.BoxType)
                    .HasColumnName("Box_Type")
                    .HasMaxLength(60);

                entity.Property(e => e.Change).HasMaxLength(100);

                entity.Property(e => e.Cl)
                    .HasColumnName("CL")
                    .HasMaxLength(3);

                entity.Property(e => e.Clweigth).HasColumnName("CLWeigth");

                entity.Property(e => e.Cm)
                    .HasColumnName("CM")
                    .HasMaxLength(3);

                entity.Property(e => e.Cmweigth).HasColumnName("CMWeigth");

                entity.Property(e => e.Code).HasMaxLength(20);

                entity.Property(e => e.CreateDate).HasColumnType("smalldatetime");

                entity.Property(e => e.CusId)
                    .HasColumnName("Cus_ID")
                    .HasMaxLength(10);

                entity.Property(e => e.CustCode)
                    .HasColumnName("Cust_Code")
                    .HasMaxLength(10);

                entity.Property(e => e.CustName)
                    .HasColumnName("Cust_Name")
                    .HasMaxLength(50);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(40);

                entity.Property(e => e.DiecutPictPath)
                    .HasColumnName("DiecutPict_Path")
                    .HasMaxLength(50);

                entity.Property(e => e.Dl)
                    .HasColumnName("DL")
                    .HasMaxLength(3);

                entity.Property(e => e.Dlweigth).HasColumnName("DLWeigth");

                entity.Property(e => e.Dm)
                    .HasColumnName("DM")
                    .HasMaxLength(3);

                entity.Property(e => e.Dmweigth).HasColumnName("DMWeigth");

                entity.Property(e => e.EanCode).HasMaxLength(13);

                entity.Property(e => e.Flute).HasMaxLength(3);

                entity.Property(e => e.Gl)
                    .HasColumnName("GL")
                    .HasMaxLength(3);

                entity.Property(e => e.Glweigth).HasColumnName("GLWeigth");

                entity.Property(e => e.Hierarchy)
                    .IsRequired()
                    .HasMaxLength(18);

                entity.Property(e => e.HighGroup)
                    .HasColumnName("High_Group")
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.HighValue)
                    .HasColumnName("High_Value")
                    .HasMaxLength(4)
                    .IsUnicode(false);

                entity.Property(e => e.IndDes)
                    .HasColumnName("Ind_Des")
                    .HasMaxLength(50);

                entity.Property(e => e.IndGrp)
                    .HasColumnName("Ind_Grp")
                    .HasMaxLength(3);

                entity.Property(e => e.JoinType).HasMaxLength(60);

                entity.Property(e => e.Language)
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.LastUpdate).HasColumnType("smalldatetime");

                entity.Property(e => e.MaterialNo)
                    .IsRequired()
                    .HasColumnName("Material_No")
                    .HasMaxLength(18);

                entity.Property(e => e.MaterialType)
                    .HasColumnName("Material_Type")
                    .HasMaxLength(2);

                entity.Property(e => e.NoSlot).HasColumnName("No_Slot");

                entity.Property(e => e.OuterJoin).HasColumnName("Outer_Join");

                entity.Property(e => e.PalletSize).HasMaxLength(10);

                entity.Property(e => e.PalletizationPath)
                    .HasColumnName("Palletization_Path")
                    .HasMaxLength(50);

                entity.Property(e => e.PartNo)
                    .HasColumnName("Part_No")
                    .HasMaxLength(22);

                entity.Property(e => e.Pc)
                    .IsRequired()
                    .HasColumnName("PC")
                    .HasMaxLength(15);

                entity.Property(e => e.PdisStatus)
                    .IsRequired()
                    .HasColumnName("PDIS_Status")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.PieceSet).HasColumnName("Piece_Set");

                entity.Property(e => e.Plant).HasMaxLength(5);

                entity.Property(e => e.PltAxleHeight).HasColumnName("Plt_Axle_Height");

                entity.Property(e => e.PltBeam).HasColumnName("Plt_Beam");

                entity.Property(e => e.PltDoubleAxle).HasColumnName("Plt_Double_axle");

                entity.Property(e => e.PltFloorAbove).HasColumnName("Plt_Floor_Above");

                entity.Property(e => e.PltFloorUnder).HasColumnName("Plt_Floor_Under");

                entity.Property(e => e.PltLegDouble).HasColumnName("Plt_Leg_Double");

                entity.Property(e => e.PltLegSingle).HasColumnName("Plt_Leg_Single");

                entity.Property(e => e.PltSingleAxle).HasColumnName("Plt_Single_axle");

                entity.Property(e => e.PrintMasterPath)
                    .HasColumnName("PrintMaster_Path")
                    .HasMaxLength(50);

                entity.Property(e => e.PrintMethod)
                    .HasColumnName("Print_Method")
                    .HasMaxLength(50);

                entity.Property(e => e.PriorityFlag)
                    .HasColumnName("Priority_Flag")
                    .HasMaxLength(2);

                entity.Property(e => e.ProType)
                    .HasColumnName("Pro_Type")
                    .HasMaxLength(60);

                entity.Property(e => e.PsmStatus)
                    .HasColumnName("PSM_Status")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.PurTxt1)
                    .HasColumnName("Pur_Txt1")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.PurTxt2)
                    .HasColumnName("Pur_Txt2")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.PurTxt3)
                    .HasColumnName("Pur_Txt3")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.PurTxt4)
                    .HasColumnName("Pur_Txt4")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.RscStyle)
                    .HasColumnName("RSC_Style")
                    .HasMaxLength(50);

                entity.Property(e => e.SaleOrg)
                    .IsRequired()
                    .HasColumnName("Sale_Org")
                    .HasMaxLength(5);

                entity.Property(e => e.SaleText1)
                    .HasColumnName("Sale_Text1")
                    .HasMaxLength(40);

                entity.Property(e => e.SaleText2)
                    .HasColumnName("Sale_Text2")
                    .HasMaxLength(40);

                entity.Property(e => e.SaleText3)
                    .HasColumnName("Sale_Text3")
                    .HasMaxLength(40);

                entity.Property(e => e.SaleText4)
                    .HasColumnName("Sale_Text4")
                    .HasMaxLength(40);

                entity.Property(e => e.SaleUom)
                    .HasColumnName("Sale_UOM")
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.SapStatus).HasColumnName("SAP_Status");

                entity.Property(e => e.SheetArea).HasColumnName("Sheet_Area");

                entity.Property(e => e.StatusFlag)
                    .HasColumnName("Status_Flag")
                    .HasMaxLength(30);

                entity.Property(e => e.TranStatus).HasColumnName("Tran_Status");

                entity.Property(e => e.UnUpgradBoard).HasColumnName("UnUpgrad_Board");

                entity.Property(e => e.User)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.WeightBox).HasColumnName("Weight_Box");

                entity.Property(e => e.WeightSh).HasColumnName("Weight_Sh");
            });

            modelBuilder.Entity<TcryRouting>(entity =>
            {
                entity.HasKey(e => new { e.PsmId, e.SeqNo });

                entity.ToTable("TCRY_Routing");

                entity.Property(e => e.PsmId)
                    .HasColumnName("PSM_ID")
                    .HasMaxLength(50);

                entity.Property(e => e.SeqNo).HasColumnName("Seq_No");

                entity.Property(e => e.Alternative1)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Alternative2)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Alternative3).HasMaxLength(50);

                entity.Property(e => e.Alternative4).HasMaxLength(50);

                entity.Property(e => e.Alternative5).HasMaxLength(50);

                entity.Property(e => e.Alternative6).HasMaxLength(50);

                entity.Property(e => e.Alternative7).HasMaxLength(50);

                entity.Property(e => e.Alternative8).HasMaxLength(50);

                entity.Property(e => e.BlockNo)
                    .HasColumnName("Block_No")
                    .HasMaxLength(50);

                entity.Property(e => e.Color1).HasMaxLength(20);

                entity.Property(e => e.Color2).HasMaxLength(20);

                entity.Property(e => e.Color3).HasMaxLength(20);

                entity.Property(e => e.Color4).HasMaxLength(20);

                entity.Property(e => e.Color5).HasMaxLength(20);

                entity.Property(e => e.Color6).HasMaxLength(20);

                entity.Property(e => e.Color7).HasMaxLength(20);

                entity.Property(e => e.ColorArea1).HasColumnName("Color_Area1");

                entity.Property(e => e.ColorArea2).HasColumnName("Color_Area2");

                entity.Property(e => e.ColorArea3).HasColumnName("Color_Area3");

                entity.Property(e => e.ColorArea4).HasColumnName("Color_Area4");

                entity.Property(e => e.ColorArea5).HasColumnName("Color_Area5");

                entity.Property(e => e.ColorArea6).HasColumnName("Color_Area6");

                entity.Property(e => e.ColorArea7).HasColumnName("Color_Area7");

                entity.Property(e => e.ColorCount).HasColumnName("Color_count");

                entity.Property(e => e.ColourCount).HasColumnName("Colour_Count");

                entity.Property(e => e.CutNo).HasColumnName("Cut_No");

                entity.Property(e => e.JoinMatNo)
                    .HasColumnName("Join_Mat_no")
                    .HasMaxLength(18);

                entity.Property(e => e.Machine).HasMaxLength(50);

                entity.Property(e => e.MatCode)
                    .HasColumnName("Mat_Code")
                    .HasMaxLength(20);

                entity.Property(e => e.MaterialNo)
                    .IsRequired()
                    .HasColumnName("Material_No")
                    .HasMaxLength(18);

                entity.Property(e => e.McMove).HasColumnName("MC_Move");

                entity.Property(e => e.MylaNo)
                    .HasColumnName("Myla_No")
                    .HasMaxLength(50);

                entity.Property(e => e.NoOpenIn).HasColumnName("No_Open_in");

                entity.Property(e => e.NoOpenOut).HasColumnName("No_Open_out");

                entity.Property(e => e.NoneBlk).HasColumnName("None_Blk");

                entity.Property(e => e.PaperWidth).HasColumnName("Paper_Width");

                entity.Property(e => e.PdisStatus)
                    .HasColumnName("PDIS_Status")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.PlanCode)
                    .HasColumnName("Plan_Code")
                    .HasMaxLength(10);

                entity.Property(e => e.Plant).HasMaxLength(5);

                entity.Property(e => e.PlateNo)
                    .HasColumnName("Plate_No")
                    .HasMaxLength(50);

                entity.Property(e => e.PostTm).HasColumnName("Post_tm");

                entity.Property(e => e.PrepareTm).HasColumnName("Prepare_tm");

                entity.Property(e => e.PsmStatus)
                    .HasColumnName("PSM_Status")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.RemarkInprocess)
                    .HasColumnName("Remark_Inprocess")
                    .HasMaxLength(255);

                entity.Property(e => e.RotateIn).HasColumnName("Rotate_In");

                entity.Property(e => e.RotateOut).HasColumnName("Rotate_Out");

                entity.Property(e => e.RunWaste).HasColumnName("Run_waste");

                entity.Property(e => e.SapStatus).HasColumnName("SAP_Status");

                entity.Property(e => e.SemiBlk).HasColumnName("Semi_Blk");

                entity.Property(e => e.SeparatMatNo)
                    .HasColumnName("Separat_Mat_no")
                    .HasMaxLength(18);

                entity.Property(e => e.SetupTm).HasColumnName("Setup_tm");

                entity.Property(e => e.SetupWaste).HasColumnName("Setup_waste");

                entity.Property(e => e.Shade1).HasMaxLength(10);

                entity.Property(e => e.Shade2).HasMaxLength(10);

                entity.Property(e => e.Shade3).HasMaxLength(10);

                entity.Property(e => e.Shade4).HasMaxLength(10);

                entity.Property(e => e.Shade5).HasMaxLength(10);

                entity.Property(e => e.Shade6).HasMaxLength(10);

                entity.Property(e => e.Shade7).HasMaxLength(10);

                entity.Property(e => e.SheetInLeg).HasColumnName("Sheet_in_Leg");

                entity.Property(e => e.SheetInWid).HasColumnName("Sheet_in_Wid");

                entity.Property(e => e.SheetOutLeg).HasColumnName("Sheet_out_Leg");

                entity.Property(e => e.SheetOutWid).HasColumnName("Sheet_out_Wid");

                entity.Property(e => e.ShipBlk).HasColumnName("Ship_Blk");

                entity.Property(e => e.StackHeight).HasColumnName("Stack_Height");

                entity.Property(e => e.StanBlk).HasColumnName("Stan_Blk");

                entity.Property(e => e.StdProcess).HasColumnName("Std_Process");

                entity.Property(e => e.TranStatus).HasColumnName("Tran_Status");

                entity.Property(e => e.UnUpgradBoard)
                    .HasColumnName("UnUpgrad_Board")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.WasteLeg).HasColumnName("Waste_Leg");

                entity.Property(e => e.WasteWid).HasColumnName("Waste_Wid");

                entity.Property(e => e.WeightIn).HasColumnName("Weight_in");

                entity.Property(e => e.WeightOut).HasColumnName("Weight_out");
            });

            modelBuilder.Entity<TcsbMasterData>(entity =>
            {
                entity.HasKey(e => e.PsmId);

                entity.ToTable("TCSB_MasterData");

                entity.Property(e => e.PsmId)
                    .HasColumnName("PSM_ID")
                    .HasMaxLength(50)
                    .ValueGeneratedNever();

                entity.Property(e => e.Bl)
                    .HasColumnName("BL")
                    .HasMaxLength(3);

                entity.Property(e => e.Blweigth).HasColumnName("BLWeigth");

                entity.Property(e => e.Bm)
                    .HasColumnName("BM")
                    .HasMaxLength(3);

                entity.Property(e => e.Bmweigth).HasColumnName("BMWeigth");

                entity.Property(e => e.Board).HasMaxLength(50);

                entity.Property(e => e.BomUom)
                    .HasColumnName("BOM_UOM")
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.BoxArea).HasColumnName("Box_Area");

                entity.Property(e => e.BoxType)
                    .HasColumnName("Box_Type")
                    .HasMaxLength(60);

                entity.Property(e => e.Change).HasMaxLength(100);

                entity.Property(e => e.Cl)
                    .HasColumnName("CL")
                    .HasMaxLength(3);

                entity.Property(e => e.Clweigth).HasColumnName("CLWeigth");

                entity.Property(e => e.Cm)
                    .HasColumnName("CM")
                    .HasMaxLength(3);

                entity.Property(e => e.Cmweigth).HasColumnName("CMWeigth");

                entity.Property(e => e.Code).HasMaxLength(20);

                entity.Property(e => e.CreateDate).HasColumnType("smalldatetime");

                entity.Property(e => e.CusId)
                    .HasColumnName("Cus_ID")
                    .HasMaxLength(10);

                entity.Property(e => e.CustCode)
                    .HasColumnName("Cust_Code")
                    .HasMaxLength(10);

                entity.Property(e => e.CustName)
                    .HasColumnName("Cust_Name")
                    .HasMaxLength(50);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(40);

                entity.Property(e => e.DiecutPictPath)
                    .HasColumnName("DiecutPict_Path")
                    .HasMaxLength(50);

                entity.Property(e => e.Dl)
                    .HasColumnName("DL")
                    .HasMaxLength(3);

                entity.Property(e => e.Dlweigth).HasColumnName("DLWeigth");

                entity.Property(e => e.Dm)
                    .HasColumnName("DM")
                    .HasMaxLength(3);

                entity.Property(e => e.Dmweigth).HasColumnName("DMWeigth");

                entity.Property(e => e.EanCode).HasMaxLength(13);

                entity.Property(e => e.Flute).HasMaxLength(3);

                entity.Property(e => e.Gl)
                    .HasColumnName("GL")
                    .HasMaxLength(3);

                entity.Property(e => e.Glweigth).HasColumnName("GLWeigth");

                entity.Property(e => e.Hierarchy)
                    .IsRequired()
                    .HasMaxLength(18);

                entity.Property(e => e.HighGroup)
                    .HasColumnName("High_Group")
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.HighValue)
                    .HasColumnName("High_Value")
                    .HasMaxLength(4)
                    .IsUnicode(false);

                entity.Property(e => e.IndDes)
                    .HasColumnName("Ind_Des")
                    .HasMaxLength(50);

                entity.Property(e => e.IndGrp)
                    .HasColumnName("Ind_Grp")
                    .HasMaxLength(3);

                entity.Property(e => e.JoinType).HasMaxLength(60);

                entity.Property(e => e.Language)
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.LastUpdate).HasColumnType("smalldatetime");

                entity.Property(e => e.MaterialNo)
                    .IsRequired()
                    .HasColumnName("Material_No")
                    .HasMaxLength(18);

                entity.Property(e => e.MaterialType)
                    .HasColumnName("Material_Type")
                    .HasMaxLength(2);

                entity.Property(e => e.NoSlot).HasColumnName("No_Slot");

                entity.Property(e => e.OuterJoin).HasColumnName("Outer_Join");

                entity.Property(e => e.PalletSize).HasMaxLength(10);

                entity.Property(e => e.PalletizationPath)
                    .HasColumnName("Palletization_Path")
                    .HasMaxLength(50);

                entity.Property(e => e.PartNo)
                    .HasColumnName("Part_No")
                    .HasMaxLength(22);

                entity.Property(e => e.Pc)
                    .IsRequired()
                    .HasColumnName("PC")
                    .HasMaxLength(15);

                entity.Property(e => e.PdisStatus)
                    .IsRequired()
                    .HasColumnName("PDIS_Status")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.PieceSet).HasColumnName("Piece_Set");

                entity.Property(e => e.Plant).HasMaxLength(5);

                entity.Property(e => e.PltAxleHeight).HasColumnName("Plt_Axle_Height");

                entity.Property(e => e.PltBeam).HasColumnName("Plt_Beam");

                entity.Property(e => e.PltDoubleAxle).HasColumnName("Plt_Double_axle");

                entity.Property(e => e.PltFloorAbove).HasColumnName("Plt_Floor_Above");

                entity.Property(e => e.PltFloorUnder).HasColumnName("Plt_Floor_Under");

                entity.Property(e => e.PltLegDouble).HasColumnName("Plt_Leg_Double");

                entity.Property(e => e.PltLegSingle).HasColumnName("Plt_Leg_Single");

                entity.Property(e => e.PltSingleAxle).HasColumnName("Plt_Single_axle");

                entity.Property(e => e.PrintMasterPath)
                    .HasColumnName("PrintMaster_Path")
                    .HasMaxLength(50);

                entity.Property(e => e.PrintMethod)
                    .HasColumnName("Print_Method")
                    .HasMaxLength(50);

                entity.Property(e => e.PriorityFlag)
                    .HasColumnName("Priority_Flag")
                    .HasMaxLength(2);

                entity.Property(e => e.ProType)
                    .HasColumnName("Pro_Type")
                    .HasMaxLength(60);

                entity.Property(e => e.PsmStatus)
                    .HasColumnName("PSM_Status")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.PurTxt1)
                    .HasColumnName("Pur_Txt1")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.PurTxt2)
                    .HasColumnName("Pur_Txt2")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.PurTxt3)
                    .HasColumnName("Pur_Txt3")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.PurTxt4)
                    .HasColumnName("Pur_Txt4")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.RscStyle)
                    .HasColumnName("RSC_Style")
                    .HasMaxLength(50);

                entity.Property(e => e.SaleOrg)
                    .IsRequired()
                    .HasColumnName("Sale_Org")
                    .HasMaxLength(5);

                entity.Property(e => e.SaleText1)
                    .HasColumnName("Sale_Text1")
                    .HasMaxLength(40);

                entity.Property(e => e.SaleText2)
                    .HasColumnName("Sale_Text2")
                    .HasMaxLength(40);

                entity.Property(e => e.SaleText3)
                    .HasColumnName("Sale_Text3")
                    .HasMaxLength(40);

                entity.Property(e => e.SaleText4)
                    .HasColumnName("Sale_Text4")
                    .HasMaxLength(40);

                entity.Property(e => e.SaleUom)
                    .HasColumnName("Sale_UOM")
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.SapStatus).HasColumnName("SAP_Status");

                entity.Property(e => e.SheetArea).HasColumnName("Sheet_Area");

                entity.Property(e => e.StatusFlag)
                    .HasColumnName("Status_Flag")
                    .HasMaxLength(30);

                entity.Property(e => e.TranStatus).HasColumnName("Tran_Status");

                entity.Property(e => e.UnUpgradBoard).HasColumnName("UnUpgrad_Board");

                entity.Property(e => e.User)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.WeightBox).HasColumnName("Weight_Box");

                entity.Property(e => e.WeightSh).HasColumnName("Weight_Sh");
            });

            modelBuilder.Entity<TcsbRouting>(entity =>
            {
                entity.HasKey(e => new { e.PsmId, e.SeqNo });

                entity.ToTable("TCSB_Routing");

                entity.Property(e => e.PsmId)
                    .HasColumnName("PSM_ID")
                    .HasMaxLength(50);

                entity.Property(e => e.SeqNo).HasColumnName("Seq_No");

                entity.Property(e => e.Alternative1)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Alternative2)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Alternative3).HasMaxLength(50);

                entity.Property(e => e.Alternative4).HasMaxLength(50);

                entity.Property(e => e.Alternative5).HasMaxLength(50);

                entity.Property(e => e.Alternative6).HasMaxLength(50);

                entity.Property(e => e.Alternative7).HasMaxLength(50);

                entity.Property(e => e.Alternative8).HasMaxLength(50);

                entity.Property(e => e.BlockNo)
                    .HasColumnName("Block_No")
                    .HasMaxLength(50);

                entity.Property(e => e.Color1).HasMaxLength(20);

                entity.Property(e => e.Color2).HasMaxLength(20);

                entity.Property(e => e.Color3).HasMaxLength(20);

                entity.Property(e => e.Color4).HasMaxLength(20);

                entity.Property(e => e.Color5).HasMaxLength(20);

                entity.Property(e => e.Color6).HasMaxLength(20);

                entity.Property(e => e.Color7).HasMaxLength(20);

                entity.Property(e => e.ColorArea1).HasColumnName("Color_Area1");

                entity.Property(e => e.ColorArea2).HasColumnName("Color_Area2");

                entity.Property(e => e.ColorArea3).HasColumnName("Color_Area3");

                entity.Property(e => e.ColorArea4).HasColumnName("Color_Area4");

                entity.Property(e => e.ColorArea5).HasColumnName("Color_Area5");

                entity.Property(e => e.ColorArea6).HasColumnName("Color_Area6");

                entity.Property(e => e.ColorArea7).HasColumnName("Color_Area7");

                entity.Property(e => e.ColorCount).HasColumnName("Color_count");

                entity.Property(e => e.ColourCount).HasColumnName("Colour_Count");

                entity.Property(e => e.CutNo).HasColumnName("Cut_No");

                entity.Property(e => e.JoinMatNo)
                    .HasColumnName("Join_Mat_no")
                    .HasMaxLength(18);

                entity.Property(e => e.Machine).HasMaxLength(50);

                entity.Property(e => e.MatCode)
                    .HasColumnName("Mat_Code")
                    .HasMaxLength(20);

                entity.Property(e => e.MaterialNo)
                    .IsRequired()
                    .HasColumnName("Material_No")
                    .HasMaxLength(18);

                entity.Property(e => e.McMove).HasColumnName("MC_Move");

                entity.Property(e => e.MylaNo)
                    .HasColumnName("Myla_No")
                    .HasMaxLength(50);

                entity.Property(e => e.NoOpenIn).HasColumnName("No_Open_in");

                entity.Property(e => e.NoOpenOut).HasColumnName("No_Open_out");

                entity.Property(e => e.NoneBlk).HasColumnName("None_Blk");

                entity.Property(e => e.PaperWidth).HasColumnName("Paper_Width");

                entity.Property(e => e.PdisStatus)
                    .HasColumnName("PDIS_Status")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.PlanCode)
                    .HasColumnName("Plan_Code")
                    .HasMaxLength(10);

                entity.Property(e => e.Plant).HasMaxLength(5);

                entity.Property(e => e.PlateNo)
                    .HasColumnName("Plate_No")
                    .HasMaxLength(50);

                entity.Property(e => e.PostTm).HasColumnName("Post_tm");

                entity.Property(e => e.PrepareTm).HasColumnName("Prepare_tm");

                entity.Property(e => e.PsmStatus)
                    .HasColumnName("PSM_Status")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.RemarkInprocess)
                    .HasColumnName("Remark_Inprocess")
                    .HasMaxLength(255);

                entity.Property(e => e.RotateIn).HasColumnName("Rotate_In");

                entity.Property(e => e.RotateOut).HasColumnName("Rotate_Out");

                entity.Property(e => e.RunWaste).HasColumnName("Run_waste");

                entity.Property(e => e.SapStatus).HasColumnName("SAP_Status");

                entity.Property(e => e.SemiBlk).HasColumnName("Semi_Blk");

                entity.Property(e => e.SeparatMatNo)
                    .HasColumnName("Separat_Mat_no")
                    .HasMaxLength(18);

                entity.Property(e => e.SetupTm).HasColumnName("Setup_tm");

                entity.Property(e => e.SetupWaste).HasColumnName("Setup_waste");

                entity.Property(e => e.Shade1).HasMaxLength(10);

                entity.Property(e => e.Shade2).HasMaxLength(10);

                entity.Property(e => e.Shade3).HasMaxLength(10);

                entity.Property(e => e.Shade4).HasMaxLength(10);

                entity.Property(e => e.Shade5).HasMaxLength(10);

                entity.Property(e => e.Shade6).HasMaxLength(10);

                entity.Property(e => e.Shade7).HasMaxLength(10);

                entity.Property(e => e.SheetInLeg).HasColumnName("Sheet_in_Leg");

                entity.Property(e => e.SheetInWid).HasColumnName("Sheet_in_Wid");

                entity.Property(e => e.SheetOutLeg).HasColumnName("Sheet_out_Leg");

                entity.Property(e => e.SheetOutWid).HasColumnName("Sheet_out_Wid");

                entity.Property(e => e.ShipBlk).HasColumnName("Ship_Blk");

                entity.Property(e => e.StackHeight).HasColumnName("Stack_Height");

                entity.Property(e => e.StanBlk).HasColumnName("Stan_Blk");

                entity.Property(e => e.StdProcess).HasColumnName("Std_Process");

                entity.Property(e => e.TranStatus).HasColumnName("Tran_Status");

                entity.Property(e => e.UnUpgradBoard)
                    .HasColumnName("UnUpgrad_Board")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.WasteLeg).HasColumnName("Waste_Leg");

                entity.Property(e => e.WasteWid).HasColumnName("Waste_Wid");

                entity.Property(e => e.WeightIn).HasColumnName("Weight_in");

                entity.Property(e => e.WeightOut).HasColumnName("Weight_out");
            });

            modelBuilder.Entity<TcsMasterData>(entity =>
            {
                entity.HasKey(e => e.PsmId);

                entity.ToTable("TCS_MasterData");

                entity.Property(e => e.PsmId)
                    .HasColumnName("PSM_ID")
                    .HasMaxLength(50)
                    .ValueGeneratedNever();

                entity.Property(e => e.Bl)
                    .HasColumnName("BL")
                    .HasMaxLength(3);

                entity.Property(e => e.Blweigth).HasColumnName("BLWeigth");

                entity.Property(e => e.Bm)
                    .HasColumnName("BM")
                    .HasMaxLength(3);

                entity.Property(e => e.Bmweigth).HasColumnName("BMWeigth");

                entity.Property(e => e.Board).HasMaxLength(50);

                entity.Property(e => e.BomUom)
                    .HasColumnName("BOM_UOM")
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.BoxArea).HasColumnName("Box_Area");

                entity.Property(e => e.BoxType)
                    .HasColumnName("Box_Type")
                    .HasMaxLength(60);

                entity.Property(e => e.Change).HasMaxLength(100);

                entity.Property(e => e.Cl)
                    .HasColumnName("CL")
                    .HasMaxLength(3);

                entity.Property(e => e.Clweigth).HasColumnName("CLWeigth");

                entity.Property(e => e.Cm)
                    .HasColumnName("CM")
                    .HasMaxLength(3);

                entity.Property(e => e.Cmweigth).HasColumnName("CMWeigth");

                entity.Property(e => e.Code).HasMaxLength(20);

                entity.Property(e => e.CreateDate).HasColumnType("smalldatetime");

                entity.Property(e => e.CusId)
                    .HasColumnName("Cus_ID")
                    .HasMaxLength(10);

                entity.Property(e => e.CustCode)
                    .HasColumnName("Cust_Code")
                    .HasMaxLength(10);

                entity.Property(e => e.CustName)
                    .HasColumnName("Cust_Name")
                    .HasMaxLength(50);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(40);

                entity.Property(e => e.DiecutPictPath)
                    .HasColumnName("DiecutPict_Path")
                    .HasMaxLength(50);

                entity.Property(e => e.Dl)
                    .HasColumnName("DL")
                    .HasMaxLength(3);

                entity.Property(e => e.Dlweigth).HasColumnName("DLWeigth");

                entity.Property(e => e.Dm)
                    .HasColumnName("DM")
                    .HasMaxLength(3);

                entity.Property(e => e.Dmweigth).HasColumnName("DMWeigth");

                entity.Property(e => e.EanCode).HasMaxLength(13);

                entity.Property(e => e.Flute).HasMaxLength(3);

                entity.Property(e => e.Gl)
                    .HasColumnName("GL")
                    .HasMaxLength(3);

                entity.Property(e => e.Glweigth).HasColumnName("GLWeigth");

                entity.Property(e => e.Hierarchy)
                    .IsRequired()
                    .HasMaxLength(18);

                entity.Property(e => e.HighGroup)
                    .HasColumnName("High_Group")
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.HighValue)
                    .HasColumnName("High_Value")
                    .HasMaxLength(4)
                    .IsUnicode(false);

                entity.Property(e => e.IndDes)
                    .HasColumnName("Ind_Des")
                    .HasMaxLength(50);

                entity.Property(e => e.IndGrp)
                    .HasColumnName("Ind_Grp")
                    .HasMaxLength(3);

                entity.Property(e => e.JoinType).HasMaxLength(60);

                entity.Property(e => e.Language)
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.LastUpdate).HasColumnType("smalldatetime");

                entity.Property(e => e.MaterialNo)
                    .IsRequired()
                    .HasColumnName("Material_No")
                    .HasMaxLength(18);

                entity.Property(e => e.MaterialType)
                    .HasColumnName("Material_Type")
                    .HasMaxLength(2);

                entity.Property(e => e.NoSlot).HasColumnName("No_Slot");

                entity.Property(e => e.OuterJoin).HasColumnName("Outer_Join");

                entity.Property(e => e.PalletSize).HasMaxLength(10);

                entity.Property(e => e.PalletizationPath)
                    .HasColumnName("Palletization_Path")
                    .HasMaxLength(50);

                entity.Property(e => e.PartNo)
                    .HasColumnName("Part_No")
                    .HasMaxLength(22);

                entity.Property(e => e.Pc)
                    .IsRequired()
                    .HasColumnName("PC")
                    .HasMaxLength(15);

                entity.Property(e => e.PdisStatus)
                    .IsRequired()
                    .HasColumnName("PDIS_Status")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.PieceSet).HasColumnName("Piece_Set");

                entity.Property(e => e.Plant).HasMaxLength(5);

                entity.Property(e => e.PltAxleHeight).HasColumnName("Plt_Axle_Height");

                entity.Property(e => e.PltBeam).HasColumnName("Plt_Beam");

                entity.Property(e => e.PltDoubleAxle).HasColumnName("Plt_Double_axle");

                entity.Property(e => e.PltFloorAbove).HasColumnName("Plt_Floor_Above");

                entity.Property(e => e.PltFloorUnder).HasColumnName("Plt_Floor_Under");

                entity.Property(e => e.PltLegDouble).HasColumnName("Plt_Leg_Double");

                entity.Property(e => e.PltLegSingle).HasColumnName("Plt_Leg_Single");

                entity.Property(e => e.PltSingleAxle).HasColumnName("Plt_Single_axle");

                entity.Property(e => e.PrintMasterPath)
                    .HasColumnName("PrintMaster_Path")
                    .HasMaxLength(50);

                entity.Property(e => e.PrintMethod)
                    .HasColumnName("Print_Method")
                    .HasMaxLength(50);

                entity.Property(e => e.PriorityFlag)
                    .HasColumnName("Priority_Flag")
                    .HasMaxLength(2);

                entity.Property(e => e.ProType)
                    .HasColumnName("Pro_Type")
                    .HasMaxLength(60);

                entity.Property(e => e.PsmStatus)
                    .HasColumnName("PSM_Status")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.PurTxt1)
                    .HasColumnName("Pur_Txt1")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.PurTxt2)
                    .HasColumnName("Pur_Txt2")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.PurTxt3)
                    .HasColumnName("Pur_Txt3")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.PurTxt4)
                    .HasColumnName("Pur_Txt4")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.RscStyle)
                    .HasColumnName("RSC_Style")
                    .HasMaxLength(50);

                entity.Property(e => e.SaleOrg)
                    .IsRequired()
                    .HasColumnName("Sale_Org")
                    .HasMaxLength(5);

                entity.Property(e => e.SaleText1)
                    .HasColumnName("Sale_Text1")
                    .HasMaxLength(40);

                entity.Property(e => e.SaleText2)
                    .HasColumnName("Sale_Text2")
                    .HasMaxLength(40);

                entity.Property(e => e.SaleText3)
                    .HasColumnName("Sale_Text3")
                    .HasMaxLength(40);

                entity.Property(e => e.SaleText4)
                    .HasColumnName("Sale_Text4")
                    .HasMaxLength(40);

                entity.Property(e => e.SaleUom)
                    .HasColumnName("Sale_UOM")
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.SapStatus).HasColumnName("SAP_Status");

                entity.Property(e => e.SheetArea).HasColumnName("Sheet_Area");

                entity.Property(e => e.StatusFlag)
                    .HasColumnName("Status_Flag")
                    .HasMaxLength(30);

                entity.Property(e => e.TranStatus).HasColumnName("Tran_Status");

                entity.Property(e => e.UnUpgradBoard).HasColumnName("UnUpgrad_Board");

                entity.Property(e => e.User)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.WeightBox).HasColumnName("Weight_Box");

                entity.Property(e => e.WeightSh).HasColumnName("Weight_Sh");
            });

            modelBuilder.Entity<TcspMasterData>(entity =>
            {
                entity.HasKey(e => e.PsmId);

                entity.ToTable("TCSP_MasterData");

                entity.Property(e => e.PsmId)
                    .HasColumnName("PSM_ID")
                    .HasMaxLength(50)
                    .ValueGeneratedNever();

                entity.Property(e => e.Bl)
                    .HasColumnName("BL")
                    .HasMaxLength(3);

                entity.Property(e => e.Blweigth).HasColumnName("BLWeigth");

                entity.Property(e => e.Bm)
                    .HasColumnName("BM")
                    .HasMaxLength(3);

                entity.Property(e => e.Bmweigth).HasColumnName("BMWeigth");

                entity.Property(e => e.Board).HasMaxLength(50);

                entity.Property(e => e.BomUom)
                    .HasColumnName("BOM_UOM")
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.BoxArea).HasColumnName("Box_Area");

                entity.Property(e => e.BoxType)
                    .HasColumnName("Box_Type")
                    .HasMaxLength(60);

                entity.Property(e => e.Change).HasMaxLength(100);

                entity.Property(e => e.Cl)
                    .HasColumnName("CL")
                    .HasMaxLength(3);

                entity.Property(e => e.Clweigth).HasColumnName("CLWeigth");

                entity.Property(e => e.Cm)
                    .HasColumnName("CM")
                    .HasMaxLength(3);

                entity.Property(e => e.Cmweigth).HasColumnName("CMWeigth");

                entity.Property(e => e.Code).HasMaxLength(20);

                entity.Property(e => e.CreateDate).HasColumnType("smalldatetime");

                entity.Property(e => e.CusId)
                    .HasColumnName("Cus_ID")
                    .HasMaxLength(10);

                entity.Property(e => e.CustCode)
                    .HasColumnName("Cust_Code")
                    .HasMaxLength(10);

                entity.Property(e => e.CustName)
                    .HasColumnName("Cust_Name")
                    .HasMaxLength(50);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(40);

                entity.Property(e => e.DiecutPictPath)
                    .HasColumnName("DiecutPict_Path")
                    .HasMaxLength(50);

                entity.Property(e => e.Dl)
                    .HasColumnName("DL")
                    .HasMaxLength(3);

                entity.Property(e => e.Dlweigth).HasColumnName("DLWeigth");

                entity.Property(e => e.Dm)
                    .HasColumnName("DM")
                    .HasMaxLength(3);

                entity.Property(e => e.Dmweigth).HasColumnName("DMWeigth");

                entity.Property(e => e.EanCode).HasMaxLength(13);

                entity.Property(e => e.Flute).HasMaxLength(3);

                entity.Property(e => e.Gl)
                    .HasColumnName("GL")
                    .HasMaxLength(3);

                entity.Property(e => e.Glweigth).HasColumnName("GLWeigth");

                entity.Property(e => e.Hierarchy)
                    .IsRequired()
                    .HasMaxLength(18);

                entity.Property(e => e.HighGroup)
                    .HasColumnName("High_Group")
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.HighValue)
                    .HasColumnName("High_Value")
                    .HasMaxLength(4)
                    .IsUnicode(false);

                entity.Property(e => e.IndDes)
                    .HasColumnName("Ind_Des")
                    .HasMaxLength(50);

                entity.Property(e => e.IndGrp)
                    .HasColumnName("Ind_Grp")
                    .HasMaxLength(3);

                entity.Property(e => e.JoinType).HasMaxLength(60);

                entity.Property(e => e.Language)
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.LastUpdate).HasColumnType("smalldatetime");

                entity.Property(e => e.MaterialNo)
                    .IsRequired()
                    .HasColumnName("Material_No")
                    .HasMaxLength(18);

                entity.Property(e => e.MaterialType)
                    .HasColumnName("Material_Type")
                    .HasMaxLength(2);

                entity.Property(e => e.NoSlot).HasColumnName("No_Slot");

                entity.Property(e => e.OuterJoin).HasColumnName("Outer_Join");

                entity.Property(e => e.PalletSize).HasMaxLength(10);

                entity.Property(e => e.PalletizationPath)
                    .HasColumnName("Palletization_Path")
                    .HasMaxLength(50);

                entity.Property(e => e.PartNo)
                    .HasColumnName("Part_No")
                    .HasMaxLength(22);

                entity.Property(e => e.Pc)
                    .IsRequired()
                    .HasColumnName("PC")
                    .HasMaxLength(15);

                entity.Property(e => e.PdisStatus)
                    .IsRequired()
                    .HasColumnName("PDIS_Status")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.PieceSet).HasColumnName("Piece_Set");

                entity.Property(e => e.Plant).HasMaxLength(5);

                entity.Property(e => e.PltAxleHeight).HasColumnName("Plt_Axle_Height");

                entity.Property(e => e.PltBeam).HasColumnName("Plt_Beam");

                entity.Property(e => e.PltDoubleAxle).HasColumnName("Plt_Double_axle");

                entity.Property(e => e.PltFloorAbove).HasColumnName("Plt_Floor_Above");

                entity.Property(e => e.PltFloorUnder).HasColumnName("Plt_Floor_Under");

                entity.Property(e => e.PltLegDouble).HasColumnName("Plt_Leg_Double");

                entity.Property(e => e.PltLegSingle).HasColumnName("Plt_Leg_Single");

                entity.Property(e => e.PltSingleAxle).HasColumnName("Plt_Single_axle");

                entity.Property(e => e.PrintMasterPath)
                    .HasColumnName("PrintMaster_Path")
                    .HasMaxLength(50);

                entity.Property(e => e.PrintMethod)
                    .HasColumnName("Print_Method")
                    .HasMaxLength(50);

                entity.Property(e => e.PriorityFlag)
                    .HasColumnName("Priority_Flag")
                    .HasMaxLength(2);

                entity.Property(e => e.ProType)
                    .HasColumnName("Pro_Type")
                    .HasMaxLength(60);

                entity.Property(e => e.PsmStatus)
                    .HasColumnName("PSM_Status")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.PurTxt1)
                    .HasColumnName("Pur_Txt1")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.PurTxt2)
                    .HasColumnName("Pur_Txt2")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.PurTxt3)
                    .HasColumnName("Pur_Txt3")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.PurTxt4)
                    .HasColumnName("Pur_Txt4")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.RscStyle)
                    .HasColumnName("RSC_Style")
                    .HasMaxLength(50);

                entity.Property(e => e.SaleOrg)
                    .IsRequired()
                    .HasColumnName("Sale_Org")
                    .HasMaxLength(5);

                entity.Property(e => e.SaleText1)
                    .HasColumnName("Sale_Text1")
                    .HasMaxLength(40);

                entity.Property(e => e.SaleText2)
                    .HasColumnName("Sale_Text2")
                    .HasMaxLength(40);

                entity.Property(e => e.SaleText3)
                    .HasColumnName("Sale_Text3")
                    .HasMaxLength(40);

                entity.Property(e => e.SaleText4)
                    .HasColumnName("Sale_Text4")
                    .HasMaxLength(40);

                entity.Property(e => e.SaleUom)
                    .HasColumnName("Sale_UOM")
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.SapStatus).HasColumnName("SAP_Status");

                entity.Property(e => e.SheetArea).HasColumnName("Sheet_Area");

                entity.Property(e => e.StatusFlag)
                    .HasColumnName("Status_Flag")
                    .HasMaxLength(30);

                entity.Property(e => e.TranStatus).HasColumnName("Tran_Status");

                entity.Property(e => e.UnUpgradBoard).HasColumnName("UnUpgrad_Board");

                entity.Property(e => e.User)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.WeightBox).HasColumnName("Weight_Box");

                entity.Property(e => e.WeightSh).HasColumnName("Weight_Sh");
            });

            modelBuilder.Entity<TcspRouting>(entity =>
            {
                entity.HasKey(e => new { e.PsmId, e.SeqNo });

                entity.ToTable("TCSP_Routing");

                entity.Property(e => e.PsmId)
                    .HasColumnName("PSM_ID")
                    .HasMaxLength(50);

                entity.Property(e => e.SeqNo).HasColumnName("Seq_No");

                entity.Property(e => e.Alternative1)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Alternative2)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Alternative3).HasMaxLength(50);

                entity.Property(e => e.Alternative4).HasMaxLength(50);

                entity.Property(e => e.Alternative5).HasMaxLength(50);

                entity.Property(e => e.Alternative6).HasMaxLength(50);

                entity.Property(e => e.Alternative7).HasMaxLength(50);

                entity.Property(e => e.Alternative8).HasMaxLength(50);

                entity.Property(e => e.BlockNo)
                    .HasColumnName("Block_No")
                    .HasMaxLength(50);

                entity.Property(e => e.Color1).HasMaxLength(20);

                entity.Property(e => e.Color2).HasMaxLength(20);

                entity.Property(e => e.Color3).HasMaxLength(20);

                entity.Property(e => e.Color4).HasMaxLength(20);

                entity.Property(e => e.Color5).HasMaxLength(20);

                entity.Property(e => e.Color6).HasMaxLength(20);

                entity.Property(e => e.Color7).HasMaxLength(20);

                entity.Property(e => e.ColorArea1).HasColumnName("Color_Area1");

                entity.Property(e => e.ColorArea2).HasColumnName("Color_Area2");

                entity.Property(e => e.ColorArea3).HasColumnName("Color_Area3");

                entity.Property(e => e.ColorArea4).HasColumnName("Color_Area4");

                entity.Property(e => e.ColorArea5).HasColumnName("Color_Area5");

                entity.Property(e => e.ColorArea6).HasColumnName("Color_Area6");

                entity.Property(e => e.ColorArea7).HasColumnName("Color_Area7");

                entity.Property(e => e.ColorCount).HasColumnName("Color_count");

                entity.Property(e => e.ColourCount).HasColumnName("Colour_Count");

                entity.Property(e => e.CutNo).HasColumnName("Cut_No");

                entity.Property(e => e.JoinMatNo)
                    .HasColumnName("Join_Mat_no")
                    .HasMaxLength(18);

                entity.Property(e => e.Machine).HasMaxLength(50);

                entity.Property(e => e.MatCode)
                    .HasColumnName("Mat_Code")
                    .HasMaxLength(20);

                entity.Property(e => e.MaterialNo)
                    .IsRequired()
                    .HasColumnName("Material_No")
                    .HasMaxLength(18);

                entity.Property(e => e.McMove).HasColumnName("MC_Move");

                entity.Property(e => e.MylaNo)
                    .HasColumnName("Myla_No")
                    .HasMaxLength(50);

                entity.Property(e => e.NoOpenIn).HasColumnName("No_Open_in");

                entity.Property(e => e.NoOpenOut).HasColumnName("No_Open_out");

                entity.Property(e => e.NoneBlk).HasColumnName("None_Blk");

                entity.Property(e => e.PaperWidth).HasColumnName("Paper_Width");

                entity.Property(e => e.PdisStatus)
                    .HasColumnName("PDIS_Status")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.PlanCode)
                    .HasColumnName("Plan_Code")
                    .HasMaxLength(10);

                entity.Property(e => e.Plant).HasMaxLength(5);

                entity.Property(e => e.PlateNo)
                    .HasColumnName("Plate_No")
                    .HasMaxLength(50);

                entity.Property(e => e.PostTm).HasColumnName("Post_tm");

                entity.Property(e => e.PrepareTm).HasColumnName("Prepare_tm");

                entity.Property(e => e.PsmStatus)
                    .HasColumnName("PSM_Status")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.RemarkInprocess)
                    .HasColumnName("Remark_Inprocess")
                    .HasMaxLength(255);

                entity.Property(e => e.RotateIn).HasColumnName("Rotate_In");

                entity.Property(e => e.RotateOut).HasColumnName("Rotate_Out");

                entity.Property(e => e.RunWaste).HasColumnName("Run_waste");

                entity.Property(e => e.SapStatus).HasColumnName("SAP_Status");

                entity.Property(e => e.SemiBlk).HasColumnName("Semi_Blk");

                entity.Property(e => e.SeparatMatNo)
                    .HasColumnName("Separat_Mat_no")
                    .HasMaxLength(18);

                entity.Property(e => e.SetupTm).HasColumnName("Setup_tm");

                entity.Property(e => e.SetupWaste).HasColumnName("Setup_waste");

                entity.Property(e => e.Shade1).HasMaxLength(10);

                entity.Property(e => e.Shade2).HasMaxLength(10);

                entity.Property(e => e.Shade3).HasMaxLength(10);

                entity.Property(e => e.Shade4).HasMaxLength(10);

                entity.Property(e => e.Shade5).HasMaxLength(10);

                entity.Property(e => e.Shade6).HasMaxLength(10);

                entity.Property(e => e.Shade7).HasMaxLength(10);

                entity.Property(e => e.SheetInLeg).HasColumnName("Sheet_in_Leg");

                entity.Property(e => e.SheetInWid).HasColumnName("Sheet_in_Wid");

                entity.Property(e => e.SheetOutLeg).HasColumnName("Sheet_out_Leg");

                entity.Property(e => e.SheetOutWid).HasColumnName("Sheet_out_Wid");

                entity.Property(e => e.ShipBlk).HasColumnName("Ship_Blk");

                entity.Property(e => e.StackHeight).HasColumnName("Stack_Height");

                entity.Property(e => e.StanBlk).HasColumnName("Stan_Blk");

                entity.Property(e => e.StdProcess).HasColumnName("Std_Process");

                entity.Property(e => e.TranStatus).HasColumnName("Tran_Status");

                entity.Property(e => e.UnUpgradBoard)
                    .HasColumnName("UnUpgrad_Board")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.WasteLeg).HasColumnName("Waste_Leg");

                entity.Property(e => e.WasteWid).HasColumnName("Waste_Wid");

                entity.Property(e => e.WeightIn).HasColumnName("Weight_in");

                entity.Property(e => e.WeightOut).HasColumnName("Weight_out");
            });

            modelBuilder.Entity<TcsRouting>(entity =>
            {
                entity.HasKey(e => new { e.PsmId, e.SeqNo });

                entity.ToTable("TCS_Routing");

                entity.Property(e => e.PsmId)
                    .HasColumnName("PSM_ID")
                    .HasMaxLength(50);

                entity.Property(e => e.SeqNo).HasColumnName("Seq_No");

                entity.Property(e => e.Alternative1)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Alternative2)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Alternative3).HasMaxLength(50);

                entity.Property(e => e.Alternative4).HasMaxLength(50);

                entity.Property(e => e.Alternative5).HasMaxLength(50);

                entity.Property(e => e.Alternative6).HasMaxLength(50);

                entity.Property(e => e.Alternative7).HasMaxLength(50);

                entity.Property(e => e.Alternative8).HasMaxLength(50);

                entity.Property(e => e.BlockNo)
                    .HasColumnName("Block_No")
                    .HasMaxLength(50);

                entity.Property(e => e.Color1).HasMaxLength(20);

                entity.Property(e => e.Color2).HasMaxLength(20);

                entity.Property(e => e.Color3).HasMaxLength(20);

                entity.Property(e => e.Color4).HasMaxLength(20);

                entity.Property(e => e.Color5).HasMaxLength(20);

                entity.Property(e => e.Color6).HasMaxLength(20);

                entity.Property(e => e.Color7).HasMaxLength(20);

                entity.Property(e => e.ColorArea1).HasColumnName("Color_Area1");

                entity.Property(e => e.ColorArea2).HasColumnName("Color_Area2");

                entity.Property(e => e.ColorArea3).HasColumnName("Color_Area3");

                entity.Property(e => e.ColorArea4).HasColumnName("Color_Area4");

                entity.Property(e => e.ColorArea5).HasColumnName("Color_Area5");

                entity.Property(e => e.ColorArea6).HasColumnName("Color_Area6");

                entity.Property(e => e.ColorArea7).HasColumnName("Color_Area7");

                entity.Property(e => e.ColorCount).HasColumnName("Color_count");

                entity.Property(e => e.ColourCount).HasColumnName("Colour_Count");

                entity.Property(e => e.CutNo).HasColumnName("Cut_No");

                entity.Property(e => e.JoinMatNo)
                    .HasColumnName("Join_Mat_no")
                    .HasMaxLength(18);

                entity.Property(e => e.Machine).HasMaxLength(50);

                entity.Property(e => e.MatCode)
                    .HasColumnName("Mat_Code")
                    .HasMaxLength(20);

                entity.Property(e => e.MaterialNo)
                    .IsRequired()
                    .HasColumnName("Material_No")
                    .HasMaxLength(18);

                entity.Property(e => e.McMove).HasColumnName("MC_Move");

                entity.Property(e => e.MylaNo)
                    .HasColumnName("Myla_No")
                    .HasMaxLength(50);

                entity.Property(e => e.NoOpenIn).HasColumnName("No_Open_in");

                entity.Property(e => e.NoOpenOut).HasColumnName("No_Open_out");

                entity.Property(e => e.NoneBlk).HasColumnName("None_Blk");

                entity.Property(e => e.PaperWidth).HasColumnName("Paper_Width");

                entity.Property(e => e.PdisStatus)
                    .HasColumnName("PDIS_Status")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.PlanCode)
                    .HasColumnName("Plan_Code")
                    .HasMaxLength(10);

                entity.Property(e => e.Plant).HasMaxLength(5);

                entity.Property(e => e.PlateNo)
                    .HasColumnName("Plate_No")
                    .HasMaxLength(50);

                entity.Property(e => e.PostTm).HasColumnName("Post_tm");

                entity.Property(e => e.PrepareTm).HasColumnName("Prepare_tm");

                entity.Property(e => e.PsmStatus)
                    .HasColumnName("PSM_Status")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.RemarkInprocess)
                    .HasColumnName("Remark_Inprocess")
                    .HasMaxLength(255);

                entity.Property(e => e.RotateIn).HasColumnName("Rotate_In");

                entity.Property(e => e.RotateOut).HasColumnName("Rotate_Out");

                entity.Property(e => e.RunWaste).HasColumnName("Run_waste");

                entity.Property(e => e.SapStatus).HasColumnName("SAP_Status");

                entity.Property(e => e.SemiBlk).HasColumnName("Semi_Blk");

                entity.Property(e => e.SeparatMatNo)
                    .HasColumnName("Separat_Mat_no")
                    .HasMaxLength(18);

                entity.Property(e => e.SetupTm).HasColumnName("Setup_tm");

                entity.Property(e => e.SetupWaste).HasColumnName("Setup_waste");

                entity.Property(e => e.Shade1).HasMaxLength(10);

                entity.Property(e => e.Shade2).HasMaxLength(10);

                entity.Property(e => e.Shade3).HasMaxLength(10);

                entity.Property(e => e.Shade4).HasMaxLength(10);

                entity.Property(e => e.Shade5).HasMaxLength(10);

                entity.Property(e => e.Shade6).HasMaxLength(10);

                entity.Property(e => e.Shade7).HasMaxLength(10);

                entity.Property(e => e.SheetInLeg).HasColumnName("Sheet_in_Leg");

                entity.Property(e => e.SheetInWid).HasColumnName("Sheet_in_Wid");

                entity.Property(e => e.SheetOutLeg).HasColumnName("Sheet_out_Leg");

                entity.Property(e => e.SheetOutWid).HasColumnName("Sheet_out_Wid");

                entity.Property(e => e.ShipBlk).HasColumnName("Ship_Blk");

                entity.Property(e => e.StackHeight).HasColumnName("Stack_Height");

                entity.Property(e => e.StanBlk).HasColumnName("Stan_Blk");

                entity.Property(e => e.StdProcess).HasColumnName("Std_Process");

                entity.Property(e => e.TranStatus).HasColumnName("Tran_Status");

                entity.Property(e => e.UnUpgradBoard)
                    .HasColumnName("UnUpgrad_Board")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.WasteLeg).HasColumnName("Waste_Leg");

                entity.Property(e => e.WasteWid).HasColumnName("Waste_Wid");

                entity.Property(e => e.WeightIn).HasColumnName("Weight_in");

                entity.Property(e => e.WeightOut).HasColumnName("Weight_out");
            });

            */

        }
    }
}
