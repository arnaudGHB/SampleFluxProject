using AutoMapper;
using CBS.TransactionManagement.Command;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.MediatR.PrimaryTellerProvisioningP.Commands;

namespace CBS.TransactionManagement.API
{
    public class CashReplenishmentPrimaryTellerProfile : Profile
    {
        public CashReplenishmentPrimaryTellerProfile()
        {
            CreateMap<CashReplenishmentPrimaryTeller, CashReplenishmentPrimaryTellerDto>().ReverseMap();
            CreateMap<ValidationCashReplenishmentPrimaryTellerCommand, CashReplenishmentPrimaryTeller>();
            //CreateMap<UpdateCashReplenishmentPrimaryTellerCommand, CashReplenishmentPrimaryTeller>();
        }
    }
}
