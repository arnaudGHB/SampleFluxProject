using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.MediatR.Commands;

namespace CBS.AccountManagement.API
{
    public class AccountingEntryProfile : Profile
    {
        public AccountingEntryProfile()
        {
            CreateMap<AccountingEntry, AccountingEntryDto>().ReverseMap();
            CreateMap<MakeAccountPostingCommand, AccountingEntryDto>();
            CreateMap<UpdateAccountingEntryCommand, AccountingEntryDto>();
        }
    }
}