using DataAccess.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using PMTs.DataAccess.ComplexModel;
using PMTs.DataAccess.ModelPresale;
using PMTs.DataAccess.Repository;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Shared;
using PMTs.DataAccess.Tracing;
using PMTs.WebApplication.AutoMapper;
using PMTs.WebApplication.Services;
using PMTs.WebApplication.Services.Interfaces;
using Rotativa.AspNetCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<LdapConfig>(builder.Configuration.GetSection("Ldap"));
builder.Services.AddDbContext<PresaleContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("PresaleConnect")));

//builder.Services.AddTransient<IPresaleRepository, PresaleRepository>();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    //options.IdleTimeout = TimeSpan.FromDays(1);
});

// เช็ค session ให้นับถอยหลังเวลาที่ไม่ได้ทำอะไร
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => false;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});


builder.Services.AddHttpContextAccessor();

builder.Services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
    .AddSessionStateTempDataProvider();

builder.Services.AddSingleton<IFileProvider>(
    new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));

builder.Services.AddMvc();

builder.Services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();
builder.Services.AddSession();

// Adds a default in-memory implementation of IDistributedCache.
builder.Services.AddDistributedMemoryCache();

builder.Services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });

builder.Services.AddMvc()
    .AddViewLocalization(
        LanguageViewLocationExpanderFormat.Suffix,
        opts => { opts.ResourcesPath = "Resources"; })
    .AddDataAnnotationsLocalization();

builder.Services.Configure<RequestLocalizationOptions>(
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

builder.Services.AddMvc()
.AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

// Auto Mapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

Globals.WebAPIUrl = builder.Configuration["WebAPIUrl"].ToString();
Globals.WebADSCG = builder.Configuration["WebADSCG"].ToString();
//Globals.PresaleHoldMat = Configuration["PresaleUnHoldMaterial"].ToString();
builder.Services.AddScoped<IAuthenticationService, LdapAuthenticationService>();

//Tassanai Update 19/11/2562
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IBomService, BomService>();
builder.Services.AddScoped<IMaintenanceAccountService, MaintenanceAccountService>();
builder.Services.AddScoped<IMaintenanceCustomerService, MaintenanceCustomerService>();
builder.Services.AddScoped<IMaintenanceMachineService, MaintenanceMachineService>();
builder.Services.AddScoped<IMaintenanceAllowanceService, MaintenanceAllowanceService>();
builder.Services.AddScoped<IMaintenanceBuildRemarkService, MaintenanceBuildRemarkService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductPicService, ProductPicService>();
builder.Services.AddScoped<IProductPropService, ProductPropService>();
builder.Services.AddScoped<IProductSpecService, ProductSpecService>();
builder.Services.AddScoped<IMasterDataService, MasterDataService>();
builder.Services.AddScoped<INewProductService, NewProductService>();
builder.Services.AddScoped<IPresaleService, PresaleService>();
builder.Services.AddScoped<IProductCustomerService, ProductCustomerService>();
builder.Services.AddScoped<IProductERPService, ProductERPService>();
builder.Services.AddScoped<IProductInfoService, ProductInfoService>();
builder.Services.AddScoped<IRoutingService, RoutingService>();
builder.Services.AddScoped<IExtensionService, ExtensionService>();
builder.Services.AddScoped<IMasterCardService, MasterCardService>();
builder.Services.AddScoped<IMaintenanceCategoriesService, MaintenanceCategoriesService>();
builder.Services.AddScoped<IMaintenanceBoardService, MaintenanceBoardService>();
builder.Services.AddScoped<IMaintenanceColorService, MaintenanceColorService>();
builder.Services.AddScoped<IMaintenanceCorConfigService, MaintenanceCorConfigService>();
builder.Services.AddScoped<IMaintenanceJointService, MaintenanceJointService>();
builder.Services.AddScoped<IMaintenancePaperWidthService, MaintenancePaperWidthService>();
builder.Services.AddScoped<IMaintenancePaperGradeService, MaintenancePaperGradeService>();
builder.Services.AddScoped<IMaintenancePMTsConfigService, MaintenancePMTsConfigService>();
builder.Services.AddScoped<IMaintenancePrintMethodService, MaintenancePrintMethodService>();
builder.Services.AddScoped<IMaintenanceProductGroupService, MaintenanceProductGroupService>();
builder.Services.AddScoped<IMaintenanceScoreGapService, MaintenanceScoreGapService>();
builder.Services.AddScoped<IMaintenanceMapCostService, MaintenanceMapCostService>();
builder.Services.AddScoped<IMaintenancePlantCostFieldService, MaintenancePlantCostFieldService>();
builder.Services.AddScoped<IMaintenanceBoardCombineAccService, MaintenanceBoardCombineAccService>();
builder.Services.AddScoped<IMaintenanceFluteService, MaintenanceFluteService>();
builder.Services.AddScoped<IMaintenanceHoneyPaperService, MaintenanceHoneyPaperService>();
builder.Services.AddScoped<IMaintenanceProductInfoService, MaintenanceProductInfoService>();
builder.Services.AddScoped<IMaintenanceAdditiveService, MaintenanceAdditiveService>();
builder.Services.AddScoped<IMaintenanceAllowanceHardService, MaintenanceAllowanceHardService>();
builder.Services.AddScoped<IProductCatalogService, ProductCatalogService>();
builder.Services.AddScoped<ISaleOrderService, SaleOrderService>();
builder.Services.AddScoped<IManageMOService, ManageMOService>();
builder.Services.AddScoped<IUpdateLotsOfMaterialService, UpdateLotsOfMaterialService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<ILogisticAndWarehouseService, LogisticAndWarehouseService>();
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddScoped<IMaintenanceRolesService, MaintenanceRolesService>();
builder.Services.AddScoped<IDocumentSService, DocumentSService>();
builder.Services.AddScoped<IAutoPackingSpecService, AutoPackingSpecService>();
builder.Services.AddScoped<IReCalculateTrimService, ReCalculateTrimService>();
builder.Services.AddScoped<IAutoPackingCustomerService, AutoPackingCustomerService>();
builder.Services.AddScoped<IBomRawMaterialService, BomRawMaterialService>();
builder.Services.AddScoped<IMaintenanceFluteTrimService, MaintenanceFluteTrimService>();

// Api singleton
builder.Services.AddSingleton<IAdditiveAPIRepository, AdditiveAPIRepository>();
builder.Services.AddSingleton<IAccountAPIRepository, AccountAPIRepository>();
builder.Services.AddSingleton<IAllowanceHardAPIRepository, AllowanceHardAPIRepository>();
builder.Services.AddSingleton<IAllowanceProcessAPIRepository, AllowanceProcessAPIRepository>();
builder.Services.AddSingleton<IBoardAlternativeAPIRepository, BoardAlternativeAPIRepository>();
builder.Services.AddSingleton<IBoardAltSpecAPIRepository, BoardAltSpecAPIRepository>();
builder.Services.AddSingleton<IBoardAltSpecAPIRepository, BoardAltSpecAPIRepository>();
builder.Services.AddSingleton<IBoardCombineAPIRepository, BoardCombineAPIRepository>();
builder.Services.AddSingleton<IBoardCombineAccAPIRepository, BoardCombineAccAPIRepository>();
builder.Services.AddSingleton<IBoardCostAPIRepository, BoardCostAPIRepository>();
builder.Services.AddSingleton<IBoardSpecAPIRepository, BoardSpecAPIRepository>();
builder.Services.AddSingleton<IBoardUseAPIRepository, BoardUseAPIRepository>();
builder.Services.AddSingleton<IBomStructAPIRepository, BomStructAPIRepository>();
builder.Services.AddSingleton<IBuildRemarkAPIRepository, BuildRemarkAPIRepository>();
builder.Services.AddSingleton<IChangeHistoryAPIRepository, ChangeHistoryAPIRepository>();
builder.Services.AddSingleton<IColorAPIRepository, ColorAPIRepository>();
builder.Services.AddSingleton<ICompanyProfileAPIRepository, CompanyProfileAPIRepository>();
builder.Services.AddSingleton<ICorConfigAPIRepository, CorConfigAPIRepository>();
builder.Services.AddSingleton<ICustomerAPIRepository, CustomerAPIRepository>();
builder.Services.AddSingleton<ICustShipToAPIRepository, CustShipToAPIRepository>();
builder.Services.AddSingleton<IFluteAPIRepository, FluteAPIRepository>();
builder.Services.AddSingleton<IFluteLayerAPIRepository, FluteLayerAPIRepository>();
builder.Services.AddSingleton<IFluteTrAPIRepository, FluteTrAPIRepository>();
builder.Services.AddSingleton<IHierarchyLv2APIRepository, HierarchyLv2APIRepository>();
builder.Services.AddSingleton<IHierarchyLv3APIRepository, HierarchyLv3APIRepository>();
builder.Services.AddSingleton<IHierarchyLv4APIRepository, HierarchyLv4APIRepository>();
builder.Services.AddSingleton<IHoneyPaperAPIRepository, HoneyPaperAPIRepository>();
builder.Services.AddSingleton<IJoinAPIRepository, JoinAPIRepository>();
builder.Services.AddSingleton<IKindOfProductGroupAPIRepository, KindOfProductGroupAPIRepository>();
builder.Services.AddSingleton<IKindOfProductAPIRepository, KindOfProductAPIRepository>();
builder.Services.AddSingleton<ILoginAPIRepository, LoginAPIRepository>();
builder.Services.AddSingleton<IMachineAPIRepository, MachineAPIRepository>();
builder.Services.AddSingleton<IMachineGroupAPIRepository, MachineGroupAPIRepository>();
builder.Services.AddSingleton<IMainMenusAPIRepository, MainMenusAPIRepository>();
builder.Services.AddSingleton<IMapCostAPIRepository, MapCostAPIRepository>();
builder.Services.AddSingleton<IMasterDataAPIRepository, MasterDataAPIRepository>();
builder.Services.AddSingleton<IMasterRoleAPIRepository, MasterRoleAPIRepository>();
builder.Services.AddSingleton<IMasterUserAPIRepository, MasterUserAPIRepository>();
builder.Services.AddSingleton<IMaterialTypeAPIRepository, MaterialTypeAPIRepository>();
builder.Services.AddSingleton<IMenuAPIRepository, MenuAPIRepository>();
builder.Services.AddSingleton<IMoBoardAlternativeAPIRepository, MoBoardAlternativeAPIRepository>();
builder.Services.AddSingleton<IMoBoardUseAPIRepository, MoBoardUseAPIRepository>();
builder.Services.AddSingleton<IMoDataAPIRepository, MoDataAPIRepository>();
builder.Services.AddSingleton<IMoDatalogAPIRepository, MoDatalogAPIRepository>();
builder.Services.AddSingleton<IMoRoutingAPIRepository, MoRoutingAPIRepository>();
builder.Services.AddSingleton<IMoSpecAPIRepository, MoSpecAPIRepository>();
builder.Services.AddSingleton<IMoTempAPIRepository, MoTempAPIRepository>();
builder.Services.AddSingleton<IPalletAPIRepository, PalletAPIRepository>();
builder.Services.AddSingleton<IPaperGradeAPIRepository, PaperGradeAPIRepository>();
builder.Services.AddSingleton<IPaperWidthAPIRepository, PaperWidthAPIRepository>();
builder.Services.AddSingleton<IPlantViewAPIRepository, PlantViewAPIRepository>();
builder.Services.AddSingleton<IPMTsConfigAPIRepository, PMTsConfigAPIRepository>();
builder.Services.AddSingleton<IPMTsNewDbContextAPIRepository, PMTsNewDbContextAPIRepository>();
builder.Services.AddSingleton<IPrintMethodAPIRepository, PrintMethodAPIRepository>();
builder.Services.AddSingleton<IProcessCostAPIRepository, ProcessCostAPIRepository>();
builder.Services.AddSingleton<IProductionTypeAPIRepository, ProductionTypeAPIRepository>();
builder.Services.AddSingleton<IProductTypeAPIRepository, ProductTypeAPIRepository>();
builder.Services.AddSingleton<IProductGroupAPIRepository, ProductGroupAPIRepository>();
builder.Services.AddSingleton<IRoutingAPIRepository, RoutingAPIRepository>();
builder.Services.AddSingleton<IRunningNoAPIRepository, RunningNoAPIRepository>();
builder.Services.AddSingleton<ISalesViewAPIRepository, SalesViewAPIRepository>();
builder.Services.AddSingleton<ISubMenusAPIRepository, SubMenusAPIRepository>();
builder.Services.AddSingleton<ITransactionsDetailAPIRepository, TransactionsDetailAPIRepository>();
builder.Services.AddSingleton<IUnitMaterialAPIRepository, UnitMaterialAPIRepository>();
builder.Services.AddSingleton<ICoatingAPIRepository, CoatingAPIRepository>();
builder.Services.AddSingleton<IScoreGapAPIRepository, ScoreGapAPIRepository>();
builder.Services.AddSingleton<IScoreTypeAPIRepository, ScoreTypeAPIRepository>();
builder.Services.AddSingleton<IFormulaAPIRepository, FormulaAPIRepository>();
builder.Services.AddSingleton<IPlantCostFieldAPIRepository, PlantCostFieldAPIRepository>();
builder.Services.AddSingleton<IQaItemsAPIRepository, QaItemsAPIRepository>();
builder.Services.AddSingleton<IQualitySpecAPIRepository, QualitySpecAPIRepository>();
builder.Services.AddSingleton<IHvaMasterAPIRepository, HvaMasterAPIRepository>();
builder.Services.AddSingleton<IHireMappingAPIRepository, HireMappingAPIRepository>();
builder.Services.AddSingleton<IHireOrderAPIRepository, HireOrderAPIRepository>();
builder.Services.AddSingleton<IPresaleChangeProductAPIRepository, PresaleChangeProductAPIRepository>();
builder.Services.AddSingleton<IPresaleChangeRoutingAPIRepository, PresaleChangeRoutingAPIRepository>();
builder.Services.AddSingleton<IStandardPatternNameAPIRepository, StandardPatternNameAPIRepository>();
builder.Services.AddSingleton<IProductCatalogCofigRepository, ProductCatalogCofigRepository>();
builder.Services.AddSingleton<IAttachFileMOAPIRepository, AttachFileMOAPIRepository>();
builder.Services.AddSingleton<ISetCategoriesOldMatAPIRepository, SetCategoriesOldMatAPIRepository>();

builder.Services.AddSingleton<IMasterRoleAPIRepository, MasterRoleAPIRepository>();
builder.Services.AddSingleton<IVMIServiceAPIRepository, VMIServiceAPIRepository>();
builder.Services.AddSingleton<ITruckOptimizeAPIRepository, TruckOptimizeAPIRepository>();

builder.Services.AddSingleton<IDocumentSRepository, DocumentSRepository>();
builder.Services.AddSingleton<IConfigWordingReportAPIRepository, ConfigWordingReportAPIRepository>();
builder.Services.AddSingleton<IAutoPackingSpecAPIRepository, AutoPackingSpecAPIRepository>();

builder.Services.AddSingleton<IJoinCharacterRepository, JoinCharacterAPIRepository>();

builder.Services.AddSingleton<ITagPrintSORepository, TagPrintSORepository>();
builder.Services.AddSingleton<IAutoPackingCustomerAPIRepository, AutoPackingCustomerAPIRepository>();
builder.Services.AddSingleton<IAutoPackingConfigAPIRepository, AutoPackingConfigAPIRepository>();
builder.Services.AddSingleton<IPpcBoiStatusAPIRepository, PpcBoiStatusAPIRepository>();
builder.Services.AddSingleton<IPpcWorkTypeAPIRepository, PpcWorkTypeAPIRepository>();
builder.Services.AddSingleton<IFSCCodeAPIRepository, FSCCodeAPIRepository>();
builder.Services.AddSingleton<IFSCFGCodeAPIRepository, FSCFGCodeAPIRepository>();

builder.Services.AddSingleton<IPPCRawMaterialMasterAPIRepository, PPCRawMaterialMasterAPIRepository>();
builder.Services.AddSingleton<IPPCRawMaterialProductionBomAPIRepository, PPCRawMaterialProductionBomAPIRepository>();
builder.Services.AddSingleton<IMoBomRawMatAPIRepository, MoBomRawMatAPIRepository>();
builder.Services.AddSingleton<IPPCMasterRpacAPIRepository, PPCMasterRpacAPIRepository>();
builder.Services.AddSingleton<IInterfaceSystemAPIAPIRepository, InterfaceSystemAPIAPIRepository>();
builder.Services.AddSingleton<IEOrderingLogAPIRepository, EOrderingLogAPIRepository>();
builder.Services.AddSingleton<IEmailAPIRepository, EmailAPIRepository>();
builder.Services.AddSingleton<ISendEmailAPIRepository, SendEmailAPIRepository>();
builder.Services.AddSingleton<IMachineFluteTrimAPIRepository, MachineFluteTrimAPIRepository>();

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddOpenTelemetryTracing(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
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
var options = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(options.Value);
app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(name: "default", pattern: "{controller=Login}/{action=Index}/{id?}");
});

RotativaConfiguration.Setup(app.Environment.WebRootPath, "Rotativa");

app.Run();
