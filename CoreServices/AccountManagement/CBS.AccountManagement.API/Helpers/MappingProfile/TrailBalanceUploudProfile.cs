using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.MediatR.Commands;

namespace CBS.AccountManagement.API
{
    public class TrailBalanceUploudProfile : Profile
    {
        public TrailBalanceUploudProfile()
        {
            CreateMap<TrailBalanceUploud, TrailBalanceUploudDto>().ReverseMap();
            CreateMap<AddTrailBalanceUploudCommand, TrailBalanceUploudDto>();
            CreateMap<UpdateTrailBalanceUploudCommand, TrailBalanceUploudDto>();
        }
    }

    public class BankTransactionProfile : Profile
    {
        public BankTransactionProfile()
        {
            CreateMap<BankTransaction, BankTransactionDto>().ReverseMap();
            //CreateMap<AddTrailBalanceUploudCommand, TrailBalanceUploudDto>();
            //CreateMap<UpdateTrailBalanceUploudCommand, TrailBalanceUploudDto>();
        }
    }
}