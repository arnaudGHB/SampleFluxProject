using CBS.AccountManagement.Common;
using CBS.AccountManagement.Common.DBConnection;
using CBS.AccountManagement.MediatR;
using CBS.AccountManagement.Repository;

namespace ReconciliationWorkerService
{
    public static class DependencyInjectionExtension
    {
        public static void AddDependencyInjection(this IServiceCollection services)
        {
            //
          
              services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));// : 
            
            services.AddScoped<IMongoUnitOfWork, MongoUnitOfWork>();
            services.AddScoped(typeof(IMongoGenericRepository<>), typeof(MongoGenericRepository<>));
          
            services.AddScoped<IAccountService, AccountService>();
            //services.AddScoped<IAccountingEntryRecordSequenceRepository, AccountingEntryRecordSequenceRepository>();
            services.AddScoped<ITransactionTrackerRepository, TransactionTrackerRepository>();
            services.AddScoped<IBSAccountRepository, BSAccountRepository>();
            services.AddScoped<IReportDownloadRepository, ReportDownloadRepository>();
            
            services.AddScoped<IConditionalAccountReferenceFinancialReportRepository, ConditionalAccountReferenceFinancialReportRepository>();
            services.AddScoped<IUserNotificationRepository, UserNotificationRepository>();
            services.AddScoped<ITrialBalanceFileRepository, TrialBalanceFileRepository>();
            services.AddScoped<IDepositNotifcationRepository, DepositNotifcationRepository>();
            services.AddScoped<IAccountCategoryRepository, AccountCategoryRepository>();
            services.AddScoped<IBankTransactionRepository, BankTransactionRepository>();
            services.AddScoped<IAccountingEntriesServices, AccountingEntriesServices>();
            services.AddScoped<IAccountingEntryRepository, AccountingEntryRepository>();
            services.AddScoped<ITransactionTrackerRepository, TransactionTrackerRepository>();
            services.AddScoped<IAccountClassRepository, AccountClassRepository>();
            services.AddScoped<ICashReplenishmentRepository, CashReplenishmentRepository>();
            services.AddScoped<IAccountFeatureRepository, AccountFeatureRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IBudgetItemDetailRepository, BudgetItemDetailRepository>();
            services.AddScoped<IBudgetRepository, BudgetRepository>();
            services.AddScoped<IBudgetCategoryRepository, BudgetCategoryRepository>();
            services.AddScoped<IBudgetPeriodRepository, BudgetPeriodRepository>();
            services.AddScoped<IOrganizationalUnitRepository, OrganizationalUnitRepository>();
            services.AddScoped<IProductAccountingBookRepository, ProductAccountingBookRepository>();
            services.AddScoped<IAccountingRuleRepository, AccountingRuleRepository>();
            services.AddScoped<IAccountingRuleEntryRepository, AccountingRuleEntryRepository>();
            services.AddScoped<IAccountTypeRepository, AccountTypeRepository>();
            services.AddScoped<ITransactionDataRepository, TransactionDataRepository>();
            services.AddScoped<IOperationEventRepository, OperationEventRepository>();
            services.AddScoped<ITransactionReversalRequestDataRepository, TransactionReversalRequestDataRepository>();
            services.AddScoped<ITransactionReversalRequestRepository, TransactionReversalRequestRepository>();
            services.AddScoped<IOperationEventAttributeRepository, OperationEventAttributesRepository>();
            services.AddScoped<IChartOfAccountRepository, ChartOfAccountRepository>();
            services.AddScoped<ICashMovementTrackerRepository, CashMovementTrackerRepository>();
            services.AddScoped<ICashMovementTrackingConfigurationRepository, CashMovementTrackingConfigurationRepository>();
            services.AddScoped<IAccountBookKeepingRepository, AccountBookKeepingRepository>();
            services.AddScoped<IEntryTempDataRepository, EntryTempDataRepository>();
            services.AddScoped<ITellerDailyProvisionRepository, TellerDailyProvisionRepository>();
            services.AddScoped<IDocumentReferenceCodeRepository, DocumentReferenceCodeRepository>();
            services.AddScoped<IDocumentRepository, DocumentRepository>();
            services.AddScoped<IDocumentTypeRepository, DocumentTypeRepository>();
            services.AddScoped<ICorrespondingMappingExceptionRepository, CorrespondingMappingExceptionRepository>();
            services.AddScoped<ICorrespondingMappingRepository, CorrespondingMappingRepository>();

            services.AddScoped<ITrialBalanceRepository, TrialBalanceRepository>();
            services.AddScoped<IPostedEntryRepository, PostedEntryRepository>();
            services.AddScoped<IAccountPolicyRepository, AccountPolicyRepository>();
            services.AddScoped<IChartOfAccountManagementPositionRepository, ChartOfAccountManagementPositionRepository>();
            services.AddScoped<ITrailBalanceUploudRepository, TrailBalanceUploudRepository>();
        }
    }
}