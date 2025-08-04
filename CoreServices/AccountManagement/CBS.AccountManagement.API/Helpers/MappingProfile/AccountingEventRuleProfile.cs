using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.MediatR.Commands;

namespace CBS.AccountManagement.API
{
    public class AccountingEventRuleProfile : Profile
    {
        public AccountingEventRuleProfile()
        {
            //CreateMap<AccountingRule, AccountingRuleDto>().ReverseMap();
             
            CreateMap<UpdateAccountingEventRuleCommand, AccountingEventRule>();
            CreateMap<AddAccountingRuleCommand, AccountingEventRule>().ReverseMap();

        }
    }
}