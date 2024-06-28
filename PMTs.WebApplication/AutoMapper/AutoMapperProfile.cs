using AutoMapper;
using PMTs.DataAccess.ComplexModel;
using PMTs.DataAccess.ModelPresale;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.AutoPackingSpec;
using PMTs.DataAccess.ModelView.BomRawMaterial;
using PMTs.DataAccess.ModelView.LogisticAndWarehouse;
using PMTs.DataAccess.ModelView.MaintenanceAccount;
using PMTs.DataAccess.ModelView.MaintenanceAllowance;
using PMTs.DataAccess.ModelView.MaintenanceBuildRemark;
using PMTs.DataAccess.ModelView.MaintenanceCorConfig;
using PMTs.DataAccess.ModelView.MaintenanceCustomer;
using PMTs.DataAccess.ModelView.MaintenanceMachine;
using PMTs.DataAccess.ModelView.MaintenancePaperWidth;
using PMTs.DataAccess.ModelView.MaintenancePMTsConfig;
using PMTs.DataAccess.ModelView.MaintenanceProductType;
using PMTs.DataAccess.ModelView.MaintenanceRoles;
using PMTs.DataAccess.ModelView.MaintenanceScoreGap;
using PMTs.DataAccess.ModelView.ManageMO;
using PMTs.DataAccess.ModelView.Report;
using PMTs.DataAccess.ModelView.UpdateLotsOfMaterial;
using System;
using System.Collections.Generic;

namespace PMTs.WebApplication.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<BoardAlternative, BoardAltViewModel>()
                .ForMember(dest => dest.MaterialNo, opt => opt.MapFrom(src => src.MaterialNo))
                .ForMember(dest => dest.BoardName, opt => opt.MapFrom(src => src.BoardName))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority))
                .ForMember(dest => dest.Active, opt => opt.MapFrom(src => src.Active))
                .ForMember(dest => dest.Flute, opt => opt.MapFrom(src => src.Flute))
                .ForMember(dest => dest.GL, opt => opt.Ignore())
                .ForMember(dest => dest.BM, opt => opt.Ignore())
                .ForMember(dest => dest.BL, opt => opt.Ignore())
                .ForMember(dest => dest.CM, opt => opt.Ignore())
                .ForMember(dest => dest.CL, opt => opt.Ignore())
                .ForMember(dest => dest.DM, opt => opt.Ignore())
                .ForMember(dest => dest.DL, opt => opt.Ignore())
                .ForMember(dest => dest.BoardKiwi, opt => opt.Ignore());

            //CreateMap<BoardAlternative, BoardAltViewModel>();
            CreateMap<Customer, CustomerViewModel>();
            CreateMap<CustomerViewModel, Customer>();
            CreateMap<BoardCombine, BoardViewModel>();
            CreateMap<BoardCombine, BoardUse>();
            CreateMap<MasterUser, AccountViewModel>();
            CreateMap<AccountViewModel, MasterUser>();
            CreateMap<Machine, MachineViewModel>();
            CreateMap<MachineViewModel, Machine>();
            CreateMap<PresaleMasterData, MasterData>();
            CreateMap<PresaleRouting, Routing>();
            CreateMap<Routing, CloneRouting>();
            CreateMap<Joint, JointViewModel>();
            CreateMap<JointViewModel, Joint>();
            CreateMap<PMTs.DataAccess.ModelView.MaintenancePrintMethod.PrintMethodViewModel, PrintMethod>();
            CreateMap<PrintMethod, PMTs.DataAccess.ModelView.MaintenancePrintMethod.PrintMethodViewModel>();
            CreateMap<ProductTypeModel, ProductType>();
            CreateMap<ProductType, ProductTypeModel>();
            CreateMap<MasterData, PresaleChangeProduct>();
            CreateMap<Routing, PresaleChangeRouting>();
            CreateMap<MasterData, MoSpec>();
            CreateMap<Routing, MoRouting>();
            CreateMap<MoData, MoDataViewModel>();
            CreateMap<MoDataViewModel, MoData>();
            CreateMap<ProductSpecViewModel, RSCModel>();
            CreateMap<TruckOptimize, TruckOptimizeViewModel>();
            CreateMap<AutoPackingSpec, AutoPackingSpecViewModel>();
            CreateMap<DocumentsMOData, DocumentSlist>();
            CreateMap<PpcRawMaterialProductionBom, RawMaterialLineFront>();
            CreateMap<PMTs.DataAccess.ModelView.MaintenanceProductGroup.ProductGroupViewModel, ProductGroup>();
            CreateMap<ProductGroup, PMTs.DataAccess.ModelView.MaintenanceProductGroup.ProductGroupViewModel>();
            CreateMap<MasterRole, MasterRoleViewModel>();
            CreateMap<MenuRoleViewModel, MenuRole>();
            CreateMap<AllowanceProcess, AllowanceViewModel>();
            CreateMap<AllowanceViewModel, AllowanceProcess>();
            CreateMap<BuildRemark, BuildRemarkViewModel>();
            CreateMap<BuildRemarkViewModel, BuildRemark>();
            CreateMap<CorConfig, CorConfigViewModel>();
            CreateMap<CorConfigViewModel, CorConfig>();
            CreateMap<PMTs.DataAccess.Models.Joint, PMTs.DataAccess.ModelView.MaintenanceJoint.JointViewModel>();
            CreateMap<PMTs.DataAccess.ModelView.MaintenanceJoint.JointViewModel, PMTs.DataAccess.Models.Joint>();
            CreateMap<PaperWidth, PaperWidthViewModel>();
            CreateMap<PaperWidthViewModel, PaperWidth>();
            CreateMap<PmtsConfig, PMTsConfigViewModel>();
            CreateMap<PMTsConfigViewModel, PmtsConfig>();
            CreateMap<ScoreGap, ScoreGapViewModel>();
            CreateMap<ScoreGapViewModel, ScoreGap>();
            CreateMap<MasterData, MasterDataViewModel>();
            CreateMap<MasterDataViewModel, MasterData>();
        }
    }
}
