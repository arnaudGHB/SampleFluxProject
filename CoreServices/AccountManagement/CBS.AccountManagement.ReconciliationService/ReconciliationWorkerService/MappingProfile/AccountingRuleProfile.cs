using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.MediatR.Commands;

namespace CBS.AccountManagement.API
{
    public class AccountingRuleProfile : Profile
    {
        public AccountingRuleProfile()
        {
            CreateMap<AccountingRule, AccountingRuleDto>().ReverseMap();
            CreateMap<AddAccountingRuleCommand, AccountingRule>();
            CreateMap<UpdateAccountingRuleCommand, AccountingRule>();
        }
    }
}