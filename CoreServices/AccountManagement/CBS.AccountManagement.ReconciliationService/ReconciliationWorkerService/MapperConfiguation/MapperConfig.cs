using AutoMapper;
using CBS.AccountManagement.API;
using CBS.AccountManagement.API.Helpers;
using CBS.AccountManagement.API.Helpers.MappingProfile;

namespace ReconciliationWorkerService
{
    public static class MapperConfig
    {
        public static IMapper GetMapperConfigs()
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                //AddCommand 
                mc.AddProfile(new TransactionTrackerProfile());
                mc.AddProfile(new ReportProfile());
                mc.AddProfile(new TrialBalanceFileProfile());
                mc.AddProfile(new CashMovementTrackingConfigurationProfile());
                mc.AddProfile(new CashMovementTrackerProfile());
                mc.AddProfile(new UsersNotificationProfiles());
                mc.AddProfile(new DepositNotificationProfile());
                mc.AddProfile(new BankTransactionProfile());
                mc.AddProfile(new TrailBalanceUploudProfile());
                mc.AddProfile(new PostedEntryProfile());
                mc.AddProfile(new AccountPolicyProfile());
                mc.AddProfile(new ChartOfAccountManagementPositionProfile());
                mc.AddProfile(new CashReplenishMentProfile());
                mc.AddProfile(new AccountCategoryProfile());
                mc.AddProfile(new SubAccountClassProfile());
                mc.AddProfile(new AccountProfile());
                mc.AddProfile(new AccountRubriqueProfile());
                mc.AddProfile(new DocumentProfile());
                mc.AddProfile(new DocumentTypeProfile());
                mc.AddProfile(new CorrespondingMappingExceptionProfile());
                mc.AddProfile(new CorrespondingMappingProfile());
                mc.AddProfile(new DocumentReferenceCodeProfile());
                mc.AddProfile(new AccountTypeProfile());
                mc.AddProfile(new AccountTypeDetailProfile());
                mc.AddProfile(new AccountingEntryProfile());
                mc.AddProfile(new AccountingRuleProfile());
                mc.AddProfile(new AccountingRuleEntryProfile());
                mc.AddProfile(new ChartOfAccountProfile());
                mc.AddProfile(new OperationEventProfile());
                mc.AddProfile(new BudgetProfile());
                mc.AddProfile(new BudgetCategoryProfile());
                mc.AddProfile(new OrganizationalUnitProfile());
                mc.AddProfile(new BudgetPeriodProfile());
                mc.AddProfile(new ProductAccountingBookProfile());
                mc.AddProfile(new OperationEventAttributesProfile());
                mc.AddProfile(new LinkAccountTypeAccountFeatureProfile());
                mc.AddProfile(new TreeNodeProfile());
                mc.AddProfile(new TransactionReversalRequestProfile());
                mc.AddProfile(new TransactionReversalRequestDataProfile());
                mc.AddProfile(new EntryTempDataProfile());
            });
            return mappingConfig.CreateMapper();
        }
    }
}