using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Entity;
using CBS.AccountManagement.MediatR.BankingOperation.Commands;
using CBS.AccountManagement.MediatR.Commands;

namespace CBS.AccountManagement.API
{
    public class CashReplenishMentProfile : Profile
    {
        public CashReplenishMentProfile()
        {
            CreateMap<CashReplenishment, CashReplenishmentDto>().ReverseMap();
            CreateMap<AddCashInfusionCommand, CashReplenishment>();
            CreateMap<AddCashReplenishmentApprovalCommand, CashReplenishment>();
        }
    }
}