using AutoMapper;
using CBS.TransactionManagement.Command;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.MediatR.CashReplenishmentSubTellerP.Commands;

namespace CBS.TransactionManagement.API
{
    public class CashReplenishmentProfile : Profile
    {
        public CashReplenishmentProfile()
        {
            CreateMap<CashReplenishmentSubTeller, SubTellerCashReplenishmentDto>().ReverseMap();
            CreateMap<AddCashReplenishmentSubTellerCommand, CashReplenishmentSubTeller>();
            CreateMap<ValidateCashReplenishmentSubTellerCommand, CashReplenishmentSubTeller>();
        }
    }
}
