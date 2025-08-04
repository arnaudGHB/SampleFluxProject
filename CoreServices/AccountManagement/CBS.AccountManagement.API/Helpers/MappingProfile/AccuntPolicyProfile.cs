using AutoMapper;
using CBS.AccountManagement.Data;

using CBS.AccountManagement.MediatR.Commands;

namespace CBS.AccountManagement.API
{
    public class  TrialBalanceFileProfile : Profile
    {
        public TrialBalanceFileProfile()
        {
            CreateMap<TrialBalanceFile, TrialBalanceFileDto>().ReverseMap();
            CreateMap<AddCommandTrialBalanceFile, TrialBalanceFile>();
  
        }
    }
    public class AccountPolicyProfile : Profile
    {
        public AccountPolicyProfile()
        {
            CreateMap<AccountPolicy, AccountPolicyDto>().ReverseMap();
            CreateMap<AddAccountPolicyCommand, AccountPolicy>();
            CreateMap<UpdateAccountPolicyCommand, AccountPolicy>();
        }
    }
    public class CashMovementTrackerProfile : Profile
    {
        public CashMovementTrackerProfile()
        {
            CreateMap<CashMovementTracker, CashMovementTrackerDto>().ReverseMap();
            CreateMap<AddCashMovementTrackingConfigurationCommand, CashMovementTracker>();
            CreateMap<UpdateCashMovementTrackerCommand, CashMovementTracker>();
        }
    }
    public class CashMovementTrackingConfigurationProfile : Profile
    {
        public CashMovementTrackingConfigurationProfile()
        {
            CreateMap<CashMovementTrackingConfiguration, CashMovementTrackingConfigurationDto>().ReverseMap();
            CreateMap<AddCashMovementTrackingConfigurationCommand, CashMovementTrackingConfiguration>();
            CreateMap<UpdateCashMovementTrackingConfigurationCommand, CashMovementTrackingConfiguration>();
        }
    }
}