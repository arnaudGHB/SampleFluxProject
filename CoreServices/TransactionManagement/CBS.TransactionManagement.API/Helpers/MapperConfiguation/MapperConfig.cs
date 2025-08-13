using AutoMapper;
using CBS.TransactionManagement.API;
using CBS.TransactionManagement.API.Helpers.MappingProfile;
using CBS.TransactionManagement.Data;

namespace CBS.TransactionManagement.API
{
    public static class MapperConfig
    {
        public static IMapper GetMapperConfigs()
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new TransactionProfile());
                mc.AddProfile(new AccountProfile());
                mc.AddProfile(new DepositLimitProfile());
                mc.AddProfile(new WithdrawalLimitsProfile());
                mc.AddProfile(new SavingProductProfile());
                mc.AddProfile(new TransferLimitsProfile());
                mc.AddProfile(new ConfigsProfile());
                mc.AddProfile(new TellerHistoryProfile());
                mc.AddProfile(new TellerProfile());
                mc.AddProfile(new CurrencyNotesProfile());
                mc.AddProfile(new AccountingEventProfile());
                mc.AddProfile(new EntryFeeParameterProfile());
                mc.AddProfile(new CloseFeeParameterProfile());
                mc.AddProfile(new ReopenFeeParameterProfile());
                mc.AddProfile(new ManagementFeeParameterProfile());
                mc.AddProfile(new CashReplenishmentProfile());
                mc.AddProfile(new CashReplenishmentPrimaryTellerProfile());
            });
            return mappingConfig.CreateMapper();
        }
    }
}
