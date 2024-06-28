using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using PMTs.DataAccess;
using PMTs.DataAccess.ComplexModel;
using PMTs.DataAccess.ModelPresale;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.AutoPackingSpec;
using PMTs.DataAccess.ModelView.BomRawMaterial;
using PMTs.DataAccess.ModelView.LogisticAndWarehouse;
using PMTs.DataAccess.ModelView.MaintenanceAccount;
using PMTs.DataAccess.ModelView.MaintenanceCustomer;
using PMTs.DataAccess.ModelView.MaintenanceMachine;
using PMTs.DataAccess.ModelView.MaintenanceProductType;
using PMTs.DataAccess.ModelView.ManageMO;
using PMTs.DataAccess.ModelView.NewProduct;
using PMTs.DataAccess.ModelView.Report;
using PMTs.DataAccess.Repository;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Shared;
using PMTs.WebApplication.AutoMapper;
using PMTs.WebApplication.Services;
using PMTs.WebApplication.Services.Interfaces;
using Rotativa.AspNetCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.Configure<LdapConfig>(Configuration.GetSection("Ldap"));
            services.AddDbContext<PresaleContext>(options => options.UseSqlServer(Configuration.GetConnectionString("PresaleConnect")));

            //services.AddTransient<IPresaleRepository, PresaleRepository>();

            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                //options.IdleTimeout = TimeSpan.FromDays(1);
            });

            // เช็ค session ให้นับถอยหลังเวลาที่ไม่ได้ทำอะไร
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            //services.AddMvc()
            //   .AddJsonOptions(options =>
            //   {
            //       options.SerializerSettings.ContractResolver
            //           = new Newtonsoft.Json.Serialization.DefaultContractResolver();
            //   });

            services.AddHttpContextAccessor();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddSessionStateTempDataProvider();

            services.AddSingleton<IFileProvider>(
                new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));

            services.AddMvc();

            services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();
            services.AddSession();

            // Adds a default in-memory implementation of IDistributedCache.
            services.AddDistributedMemoryCache();

            services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });

            services.AddMvc()
                .AddViewLocalization(
                    LanguageViewLocationExpanderFormat.Suffix,
                    opts => { opts.ResourcesPath = "Resources"; })
                .AddDataAnnotationsLocalization();

            services.Configure<RequestLocalizationOptions>(
            opts =>
            {
                var supportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("en-GB"),
                    new CultureInfo("en-US"),
                    new CultureInfo("en"),
                    new CultureInfo("th"),
                    new CultureInfo("vi")
                };

                opts.DefaultRequestCulture = new RequestCulture("en-GB");
                // opts.DefaultRequestCulture = new RequestCulture("th");
                // Formatting numbers, dates, etc.
                opts.SupportedCultures = supportedCultures;
                // UI strings that we have localized.
                opts.SupportedUICultures = supportedCultures;
            });

            services.AddMvc()
            .AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            // Auto Mapper
            services.AddAutoMapper(typeof(AutoMapperProfile));

            //var configuration = new MapperConfiguration(cfg =>
            //{
            //    cfg.CreateMap<Customer, CustomerViewModel>();
            //    cfg.CreateMap<CustomerViewModel, Customer>();
            //    cfg.CreateMap<BoardAlternative, BoardAltViewModel>();
            //    cfg.CreateMap<BoardCombine, BoardViewModel>();
            //    cfg.CreateMap<BoardCombine, BoardUse>();
            //    cfg.CreateMap<MasterUser, AccountViewModel>();
            //    cfg.CreateMap<AccountViewModel, MasterUser>();
            //    cfg.CreateMap<Machine, MachineViewModel>();
            //    cfg.CreateMap<MachineViewModel, Machine>();
            //    cfg.CreateMap<PresaleMasterData, MasterData>();
            //    cfg.CreateMap<PresaleRouting, Routing>();
            //    cfg.CreateMap<Routing, CloneRouting>();
            //    cfg.CreateMap<Joint, JointViewModel>();
            //    cfg.CreateMap<JointViewModel, Joint>();
            //    cfg.CreateMap<PrintMethodViewModel, PrintMethod>();
            //    cfg.CreateMap<PrintMethod, PrintMethodViewModel>();
            //    cfg.CreateMap<ProductTypeModel, ProductType>();
            //    cfg.CreateMap<ProductType, ProductTypeModel>();
            //    cfg.CreateMap<MasterData, PresaleChangeProduct>();
            //    cfg.CreateMap<Routing, PresaleChangeRouting>();
            //    cfg.CreateMap<MasterData, MoSpec>();
            //    cfg.CreateMap<Routing, MoRouting>();
            //    cfg.CreateMap<MoData, MoDataViewModel>();
            //    cfg.CreateMap<ProductSpecViewModel, RSCModel>();
            //    cfg.CreateMap<TruckOptimize, TruckOptimizeViewModel>();
            //    cfg.CreateMap<AutoPackingSpec, AutoPackingSpecViewModel>();
            //    cfg.CreateMap<DocumentsMOData, DocumentSlist>();
            //    cfg.CreateMap<PpcRawMaterialProductionBom, RawMaterialLineFront>();
            //});
            //var mapper = configuration.CreateMapper();

            Globals.WebAPIUrl = Configuration["WebAPIUrl"].ToString();
            Globals.WebADSCG = Configuration["WebADSCG"].ToString();
            //Globals.PresaleHoldMat = Configuration["PresaleUnHoldMaterial"].ToString();
            services.AddScoped<IAuthenticationService, LdapAuthenticationService>();

            //Tassanai Update 19/11/2562
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IBomService, BomService>();
            services.AddScoped<IMaintenanceAccountService, MaintenanceAccountService>();
            services.AddScoped<IMaintenanceCustomerService, MaintenanceCustomerService>();
            services.AddScoped<IMaintenanceMachineService, MaintenanceMachineService>();
            services.AddScoped<IMaintenanceAllowanceService, MaintenanceAllowanceService>();
            services.AddScoped<IMaintenanceBuildRemarkService, MaintenanceBuildRemarkService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductPicService, ProductPicService>();
            services.AddScoped<IProductPropService, ProductPropService>();
            services.AddScoped<IProductSpecService, ProductSpecService>();
            services.AddScoped<IMasterDataService, MasterDataService>();
            services.AddScoped<INewProductService, NewProductService>();
            services.AddScoped<IPresaleService, PresaleService>();
            services.AddScoped<IProductCustomerService, ProductCustomerService>();
            services.AddScoped<IProductERPService, ProductERPService>();
            services.AddScoped<IProductInfoService, ProductInfoService>();
            services.AddScoped<IRoutingService, RoutingService>();
            services.AddScoped<IExtensionService, ExtensionService>();
            services.AddScoped<IMasterCardService, MasterCardService>();
            services.AddScoped<IMaintenanceCategoriesService, MaintenanceCategoriesService>();
            services.AddScoped<IMaintenanceBoardService, MaintenanceBoardService>();
            services.AddScoped<IMaintenanceColorService, MaintenanceColorService>();
            services.AddScoped<IMaintenanceCorConfigService, MaintenanceCorConfigService>();
            services.AddScoped<IMaintenanceJointService, MaintenanceJointService>();
            services.AddScoped<IMaintenancePaperWidthService, MaintenancePaperWidthService>();
            services.AddScoped<IMaintenancePaperGradeService, MaintenancePaperGradeService>();
            services.AddScoped<IMaintenancePMTsConfigService, MaintenancePMTsConfigService>();
            services.AddScoped<IMaintenancePrintMethodService, MaintenancePrintMethodService>();
            services.AddScoped<IMaintenanceProductGroupService, MaintenanceProductGroupService>();
            services.AddScoped<IMaintenanceScoreGapService, MaintenanceScoreGapService>();
            services.AddScoped<IMaintenanceMapCostService, MaintenanceMapCostService>();
            services.AddScoped<IMaintenancePlantCostFieldService, MaintenancePlantCostFieldService>();
            services.AddScoped<IMaintenanceBoardCombineAccService, MaintenanceBoardCombineAccService>();
            services.AddScoped<IMaintenanceFluteService, MaintenanceFluteService>();
            services.AddScoped<IMaintenanceHoneyPaperService, MaintenanceHoneyPaperService>();
            services.AddScoped<IMaintenanceProductInfoService, MaintenanceProductInfoService>();
            services.AddScoped<IMaintenanceAdditiveService, MaintenanceAdditiveService>();
            services.AddScoped<IMaintenanceAllowanceHardService, MaintenanceAllowanceHardService>();
            services.AddScoped<IProductCatalogService, ProductCatalogService>();
            services.AddScoped<ISaleOrderService, SaleOrderService>();
            services.AddScoped<IManageMOService, ManageMOService>();
            services.AddScoped<IUpdateLotsOfMaterialService, UpdateLotsOfMaterialService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<ILogisticAndWarehouseService, LogisticAndWarehouseService>();
            services.AddScoped<IEmailService, EmailService>();

            services.AddScoped<IMaintenanceRolesService, MaintenanceRolesService>();
            services.AddScoped<IDocumentSService, DocumentSService>();
            services.AddScoped<IAutoPackingSpecService, AutoPackingSpecService>();
            services.AddScoped<IReCalculateTrimService, ReCalculateTrimService>();
            services.AddScoped<IAutoPackingCustomerService, AutoPackingCustomerService>();
            services.AddScoped<IBomRawMaterialService, BomRawMaterialService>();

            // Api singleton
            services.AddSingleton<IAdditiveAPIRepository, AdditiveAPIRepository>();
            services.AddSingleton<IAccountAPIRepository, AccountAPIRepository>();
            services.AddSingleton<IAllowanceHardAPIRepository, AllowanceHardAPIRepository>();
            services.AddSingleton<IAllowanceProcessAPIRepository, AllowanceProcessAPIRepository>();
            services.AddSingleton<IBoardAlternativeAPIRepository, BoardAlternativeAPIRepository>();
            services.AddSingleton<IBoardAltSpecAPIRepository, BoardAltSpecAPIRepository>();
            services.AddSingleton<IBoardAltSpecAPIRepository, BoardAltSpecAPIRepository>();
            services.AddSingleton<IBoardCombineAPIRepository, BoardCombineAPIRepository>();
            services.AddSingleton<IBoardCombineAccAPIRepository, BoardCombineAccAPIRepository>();
            services.AddSingleton<IBoardCostAPIRepository, BoardCostAPIRepository>();
            services.AddSingleton<IBoardSpecAPIRepository, BoardSpecAPIRepository>();
            services.AddSingleton<IBoardUseAPIRepository, BoardUseAPIRepository>();
            services.AddSingleton<IBomStructAPIRepository, BomStructAPIRepository>();
            services.AddSingleton<IBuildRemarkAPIRepository, BuildRemarkAPIRepository>();
            services.AddSingleton<IChangeHistoryAPIRepository, ChangeHistoryAPIRepository>();
            services.AddSingleton<IColorAPIRepository, ColorAPIRepository>();
            services.AddSingleton<ICompanyProfileAPIRepository, CompanyProfileAPIRepository>();
            services.AddSingleton<ICorConfigAPIRepository, CorConfigAPIRepository>();
            services.AddSingleton<ICustomerAPIRepository, CustomerAPIRepository>();
            services.AddSingleton<ICustShipToAPIRepository, CustShipToAPIRepository>();
            services.AddSingleton<IFluteAPIRepository, FluteAPIRepository>();
            services.AddSingleton<IFluteLayerAPIRepository, FluteLayerAPIRepository>();
            services.AddSingleton<IFluteTrAPIRepository, FluteTrAPIRepository>();
            services.AddSingleton<IHierarchyLv2APIRepository, HierarchyLv2APIRepository>();
            services.AddSingleton<IHierarchyLv3APIRepository, HierarchyLv3APIRepository>();
            services.AddSingleton<IHierarchyLv4APIRepository, HierarchyLv4APIRepository>();
            services.AddSingleton<IHoneyPaperAPIRepository, HoneyPaperAPIRepository>();
            services.AddSingleton<IJoinAPIRepository, JoinAPIRepository>();
            services.AddSingleton<IKindOfProductGroupAPIRepository, KindOfProductGroupAPIRepository>();
            services.AddSingleton<IKindOfProductAPIRepository, KindOfProductAPIRepository>();
            services.AddSingleton<ILoginAPIRepository, LoginAPIRepository>();
            services.AddSingleton<IMachineAPIRepository, MachineAPIRepository>();
            services.AddSingleton<IMachineGroupAPIRepository, MachineGroupAPIRepository>();
            services.AddSingleton<IMainMenusAPIRepository, MainMenusAPIRepository>();
            services.AddSingleton<IMapCostAPIRepository, MapCostAPIRepository>();
            services.AddSingleton<IMasterDataAPIRepository, MasterDataAPIRepository>();
            services.AddSingleton<IMasterRoleAPIRepository, MasterRoleAPIRepository>();
            services.AddSingleton<IMasterUserAPIRepository, MasterUserAPIRepository>();
            services.AddSingleton<IMaterialTypeAPIRepository, MaterialTypeAPIRepository>();
            services.AddSingleton<IMenuAPIRepository, MenuAPIRepository>();
            services.AddSingleton<IMoBoardAlternativeAPIRepository, MoBoardAlternativeAPIRepository>();
            services.AddSingleton<IMoBoardUseAPIRepository, MoBoardUseAPIRepository>();
            services.AddSingleton<IMoDataAPIRepository, MoDataAPIRepository>();
            services.AddSingleton<IMoDatalogAPIRepository, MoDatalogAPIRepository>();
            services.AddSingleton<IMoRoutingAPIRepository, MoRoutingAPIRepository>();
            services.AddSingleton<IMoSpecAPIRepository, MoSpecAPIRepository>();
            services.AddSingleton<IMoTempAPIRepository, MoTempAPIRepository>();
            services.AddSingleton<IPalletAPIRepository, PalletAPIRepository>();
            services.AddSingleton<IPaperGradeAPIRepository, PaperGradeAPIRepository>();
            services.AddSingleton<IPaperWidthAPIRepository, PaperWidthAPIRepository>();
            services.AddSingleton<IPlantViewAPIRepository, PlantViewAPIRepository>();
            services.AddSingleton<IPMTsConfigAPIRepository, PMTsConfigAPIRepository>();
            services.AddSingleton<IPMTsNewDbContextAPIRepository, PMTsNewDbContextAPIRepository>();
            services.AddSingleton<IPrintMethodAPIRepository, PrintMethodAPIRepository>();
            services.AddSingleton<IProcessCostAPIRepository, ProcessCostAPIRepository>();
            services.AddSingleton<IProductionTypeAPIRepository, ProductionTypeAPIRepository>();
            services.AddSingleton<IProductTypeAPIRepository, ProductTypeAPIRepository>();
            services.AddSingleton<IProductGroupAPIRepository, ProductGroupAPIRepository>();
            services.AddSingleton<IRoutingAPIRepository, RoutingAPIRepository>();
            services.AddSingleton<IRunningNoAPIRepository, RunningNoAPIRepository>();
            services.AddSingleton<ISalesViewAPIRepository, SalesViewAPIRepository>();
            services.AddSingleton<ISubMenusAPIRepository, SubMenusAPIRepository>();
            services.AddSingleton<ITransactionsDetailAPIRepository, TransactionsDetailAPIRepository>();
            services.AddSingleton<IUnitMaterialAPIRepository, UnitMaterialAPIRepository>();
            services.AddSingleton<ICoatingAPIRepository, CoatingAPIRepository>();
            services.AddSingleton<IScoreGapAPIRepository, ScoreGapAPIRepository>();
            services.AddSingleton<IScoreTypeAPIRepository, ScoreTypeAPIRepository>();
            services.AddSingleton<IFormulaAPIRepository, FormulaAPIRepository>();
            services.AddSingleton<IPlantCostFieldAPIRepository, PlantCostFieldAPIRepository>();
            services.AddSingleton<IQaItemsAPIRepository, QaItemsAPIRepository>();
            services.AddSingleton<IQualitySpecAPIRepository, QualitySpecAPIRepository>();
            services.AddSingleton<IHvaMasterAPIRepository, HvaMasterAPIRepository>();
            services.AddSingleton<IHireMappingAPIRepository, HireMappingAPIRepository>();
            services.AddSingleton<IHireOrderAPIRepository, HireOrderAPIRepository>();
            services.AddSingleton<IPresaleChangeProductAPIRepository, PresaleChangeProductAPIRepository>();
            services.AddSingleton<IPresaleChangeRoutingAPIRepository, PresaleChangeRoutingAPIRepository>();
            services.AddSingleton<IStandardPatternNameAPIRepository, StandardPatternNameAPIRepository>();
            services.AddSingleton<IProductCatalogCofigRepository, ProductCatalogCofigRepository>();
            services.AddSingleton<IAttachFileMOAPIRepository, AttachFileMOAPIRepository>();
            services.AddSingleton<ISetCategoriesOldMatAPIRepository, SetCategoriesOldMatAPIRepository>();

            services.AddSingleton<IMasterRoleAPIRepository, MasterRoleAPIRepository>();
            services.AddSingleton<IVMIServiceAPIRepository, VMIServiceAPIRepository>();
            services.AddSingleton<ITruckOptimizeAPIRepository, TruckOptimizeAPIRepository>();

            services.AddSingleton<IDocumentSRepository, DocumentSRepository>();
            services.AddSingleton<IConfigWordingReportAPIRepository, ConfigWordingReportAPIRepository>();
            services.AddSingleton<IAutoPackingSpecAPIRepository, AutoPackingSpecAPIRepository>();

            services.AddSingleton<IJoinCharacterRepository, JoinCharacterAPIRepository>();

            services.AddSingleton<ITagPrintSORepository, TagPrintSORepository>();
            services.AddSingleton<IAutoPackingCustomerAPIRepository, AutoPackingCustomerAPIRepository>();
            services.AddSingleton<IAutoPackingConfigAPIRepository, AutoPackingConfigAPIRepository>();
            services.AddSingleton<IPpcBoiStatusAPIRepository, PpcBoiStatusAPIRepository>();
            services.AddSingleton<IPpcWorkTypeAPIRepository, PpcWorkTypeAPIRepository>();
            services.AddSingleton<IFSCCodeAPIRepository, FSCCodeAPIRepository>();
            services.AddSingleton<IFSCFGCodeAPIRepository, FSCFGCodeAPIRepository>();

            services.AddSingleton<IPPCRawMaterialMasterAPIRepository, PPCRawMaterialMasterAPIRepository>();
            services.AddSingleton<IPPCRawMaterialProductionBomAPIRepository, PPCRawMaterialProductionBomAPIRepository>();
            services.AddSingleton<IMoBomRawMatAPIRepository, MoBomRawMatAPIRepository>();
            services.AddSingleton<IPPCMasterRpacAPIRepository, PPCMasterRpacAPIRepository>();
            services.AddSingleton<IInterfaceSystemAPIAPIRepository, InterfaceSystemAPIAPIRepository>();
            services.AddSingleton<IEOrderingLogAPIRepository, EOrderingLogAPIRepository>();
            services.AddSingleton<IEmailAPIRepository, EmailAPIRepository>();
            services.AddSingleton<ISendEmailAPIRepository, SendEmailAPIRepository>();


        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                //app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseSession();
            var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(options.Value);

            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //        name: "default",
            //         //template: "{controller=Account}/{action=Login}/{id?}");
            //         template: "{controller=Login}/{action=Index}/{id?}");
            //});

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(name: "default", pattern: "{controller=Login}/{action=Index}/{id?}");
            });

            RotativaConfiguration.Setup(env.WebRootPath, "Rotativa");

        }
    }
}

