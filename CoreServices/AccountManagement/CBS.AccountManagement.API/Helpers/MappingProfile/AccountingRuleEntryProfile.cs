using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.MediatR.Commands;

namespace CBS.AccountManagement.API
{
    public class AccountingRuleEntryProfile : Profile
    {
        public AccountingRuleEntryProfile()
        {
            CreateMap<AccountingRuleEntry, AccountingRuleEntryDto>().ReverseMap();
            CreateMap<AddAccountingRuleEntryCommand, AccountingRuleEntry>();
            CreateMap<UpdateAccountingRuleEntryCommand, AccountingRuleEntry>();
        }
    }
}