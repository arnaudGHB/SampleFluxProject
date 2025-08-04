using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.MediatR.Commands;

namespace CBS.AccountManagement.API
{
    public class AccountTypeProfile : Profile
    {
        public AccountTypeProfile()
        {
            //AccountTypeDto
            CreateMap<AccountType, AccountTypeDto>().ReverseMap();
            CreateMap<AddAccountTypeCommand, AccountType>();
            CreateMap<AddAccountTypeForSystemCommand, AccountType>().ReverseMap();
            CreateMap<AddAccountTypeForSystemCommand, AccountTypeDto>();
            CreateMap<UpdateAccountTypeCommand, AccountType>();
        }
    }
}