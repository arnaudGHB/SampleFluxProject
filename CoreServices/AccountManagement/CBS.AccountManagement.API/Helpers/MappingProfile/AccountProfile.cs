using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.MediatR.Commands;

namespace CBS.AccountManagement.API
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<Account, AccountDto>().ReverseMap();
            CreateMap<AddAccountCommand, Account>();
            CreateMap<UpdateAccountCommand, Account>();
            CreateMap<AccountCommand, Account>()
      .ForMember(dest => dest.Id, opt => opt.Ignore())
      .ForMember(dest => dest.AccountNumber, opt => opt.MapFrom(src => src.AccountNumber))
      .ForMember(dest => dest.AccountName, opt => opt.MapFrom(src => src.AccountName))
      .ForMember(dest => dest.AccountOwnerId, opt => opt.MapFrom(src => src.AccountOwnerId))
      .ForMember(dest => dest.ChartOfAccountManagementPositionId, opt => opt.MapFrom(src => src.ChartOfAccountManagementPositionId));
        }
    }
}