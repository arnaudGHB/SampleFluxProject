using Microsoft.AspNetCore.Authentication;
using ReconciliationWorkerService;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using CBS.AccountManagement.Data;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using ApiProcessingService;
using CBS.AccountManagement.Helper;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.Web.Administration;
using CBS.AccountManagement.Common.DBConnection;
using Microsoft.AspNetCore.Hosting;
using CBS.AccountManagement.Repository;
using Microsoft.EntityFrameworkCore;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.MediatR;
using MediatR;
//using Microsoft.Extensions.Hosting.WindowsServices;
var builder = Host.CreateApplicationBuilder(args);

 
// Add services to the container
builder.Services.AddHttpClient();
builder.Services.AddScoped<UserInfoToken>();
builder.Services.AddSingleton<JWTMiddleware>();
var mongoConnectionString = builder.Configuration.GetSection("MongoDB").GetSection("DatabaseConnectionString").Value;
var mongoDatabaseName = builder.Configuration.GetSection("MongoDB").GetSection("DatabaseName").Value;
builder.Services.AddScoped<IMongoDbConnection>(sp => new MongoDbConnection(mongoConnectionString, mongoDatabaseName));

builder.Services.AddDbContext<POSContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add other services
builder.Services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
builder.Services.AddScoped<ITransactionTrackerRepository, TransactionTrackerRepository>();

builder.Services.AddSingleton(MapperConfig.GetMapperConfigs());
builder.Services.AddSingleton<IWebHostEnvironment>(new MockWebHostEnvironment());
builder.Services.AddDependencyInjection();
// Add services to the container
builder.Services.AddRouting(); // Ensure routing services are added
builder.Services.AddScoped<PathHelper>();
// Add authorization services if needed
builder.Services.AddAuthorization();
// Configure JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
builder.Services.AddJwtAuthenticationConfiguration(jwtSettings!);
builder.Configuration.GetConnectionString("ConnectionStrings");
// Add hosted service
builder.Services.AddHostedService<ReconciliationWorker>();
builder.Services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));// : 

builder.Services.AddScoped<IMongoUnitOfWork, MongoUnitOfWork>();
builder.Services.AddScoped(typeof(IMongoGenericRepository<>), typeof(MongoGenericRepository<>));

builder.Services.AddScoped<IAccountService, AccountService>();
//  builder.Services.AddScoped<IAccountingEntryRecordSequenceRepository, AccountingEntryRecordSequenceRepository>();
builder.Services.AddScoped<ITransactionTrackerRepository, TransactionTrackerRepository>();
builder.Services.AddScoped<IBSAccountRepository, BSAccountRepository>();
builder.Services.AddScoped<IReportDownloadRepository, ReportDownloadRepository>();

builder.Services.AddScoped<IConditionalAccountReferenceFinancialReportRepository, ConditionalAccountReferenceFinancialReportRepository>();
builder.Services.AddScoped<IUserNotificationRepository, UserNotificationRepository>();
builder.Services.AddScoped<ITrialBalanceFileRepository, TrialBalanceFileRepository>();
builder.Services.AddScoped<IDepositNotifcationRepository, DepositNotifcationRepository>();
builder.Services.AddScoped<IAccountCategoryRepository, AccountCategoryRepository>();
builder.Services.AddScoped<IBankTransactionRepository, BankTransactionRepository>();
builder.Services.AddScoped<IAccountingEntriesServices, AccountingEntriesServices>();
builder.Services.AddScoped<IAccountingEntryRepository, AccountingEntryRepository>();
builder.Services.AddScoped<ITransactionTrackerRepository, TransactionTrackerRepository>();
builder.Services.AddScoped<IAccountClassRepository, AccountClassRepository>();
builder.Services.AddScoped<ICashReplenishmentRepository, CashReplenishmentRepository>();
builder.Services.AddScoped<IAccountFeatureRepository, AccountFeatureRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IBudgetItemDetailRepository, BudgetItemDetailRepository>();
builder.Services.AddScoped<IBudgetRepository, BudgetRepository>();
builder.Services.AddScoped<IBudgetCategoryRepository, BudgetCategoryRepository>();
builder.Services.AddScoped<IBudgetPeriodRepository, BudgetPeriodRepository>();
builder.Services.AddScoped<IOrganizationalUnitRepository, OrganizationalUnitRepository>();
builder.Services.AddScoped<IProductAccountingBookRepository, ProductAccountingBookRepository>();
builder.Services.AddScoped<IAccountingRuleRepository, AccountingRuleRepository>();
builder.Services.AddScoped<IAccountingRuleEntryRepository, AccountingRuleEntryRepository>();
builder.Services.AddScoped<IAccountTypeRepository, AccountTypeRepository>();
builder.Services.AddScoped<ITransactionDataRepository, TransactionDataRepository>();
builder.Services.AddScoped<IOperationEventRepository, OperationEventRepository>();
builder.Services.AddScoped<ITransactionReversalRequestDataRepository, TransactionReversalRequestDataRepository>();
builder.Services.AddScoped<ITransactionReversalRequestRepository, TransactionReversalRequestRepository>();
builder.Services.AddScoped<IOperationEventAttributeRepository, OperationEventAttributesRepository>();
builder.Services.AddScoped<IChartOfAccountRepository, ChartOfAccountRepository>();
builder.Services.AddScoped<ICashMovementTrackerRepository, CashMovementTrackerRepository>();
builder.Services.AddScoped<ICashMovementTrackingConfigurationRepository, CashMovementTrackingConfigurationRepository>();
builder.Services.AddScoped<IAccountBookKeepingRepository, AccountBookKeepingRepository>();
builder.Services.AddScoped<IEntryTempDataRepository, EntryTempDataRepository>();
builder.Services.AddScoped<ITellerDailyProvisionRepository, TellerDailyProvisionRepository>();
builder.Services.AddScoped<IDocumentReferenceCodeRepository, DocumentReferenceCodeRepository>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<IDocumentTypeRepository, DocumentTypeRepository>();
builder.Services.AddScoped<ICorrespondingMappingExceptionRepository, CorrespondingMappingExceptionRepository>();
builder.Services.AddScoped<ICorrespondingMappingRepository, CorrespondingMappingRepository>();

builder.Services.AddScoped<ITrialBalanceRepository, TrialBalanceRepository>();
builder.Services.AddScoped<IPostedEntryRepository, PostedEntryRepository>();
builder.Services.AddScoped<IAccountPolicyRepository, AccountPolicyRepository>();
builder.Services.AddScoped<IChartOfAccountManagementPositionRepository, ChartOfAccountManagementPositionRepository>();
builder.Services.AddScoped<ITrailBalanceUploudRepository, TrailBalanceUploudRepository>();


// Configure Windows Service

// Register MediatR
var assembly = AppDomain.CurrentDomain.Load("CBS.AccountManagement.MediatR");
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
// Register other services
builder.Services.AddScoped<IAccountingEntriesServices, AccountingEntriesServices>();
builder.Services.AddHostedService<ReconciliationWorker>();
var host = builder.Build();
await host.RunAsync();
