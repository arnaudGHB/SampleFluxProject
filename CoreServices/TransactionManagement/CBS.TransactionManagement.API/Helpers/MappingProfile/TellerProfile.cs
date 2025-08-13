using AutoMapper;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.MediatR.TellerP.Commands;

namespace CBS.TransactionManagement.API
{
    public class TellerProfile : Profile
    {
        public TellerProfile()
        {
            CreateMap<Teller, TellerDto>().ReverseMap();
            CreateMap<Teller, TellerBalanceDto>();
            CreateMap<AddTellerCommand, Teller>();
            CreateMap<UpdateTellerCommand, Teller>();
            CreateMap<MobileMoneyTellerConfigurationCommand, Teller>();
        }
    }
}
