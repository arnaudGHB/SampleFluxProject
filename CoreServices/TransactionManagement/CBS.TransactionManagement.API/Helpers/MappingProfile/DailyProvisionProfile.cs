using AutoMapper;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using Microsoft.AspNetCore.Mvc.TagHelpers;

namespace CBS.TransactionManagement.API
{
    public class DailyProvisionProfile : Profile
    {
        public DailyProvisionProfile()
        {
            CreateMap<DailyProvision, DailyProvisionDto>()
    .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => BaseUtilities.FormatDateTime(src.CreatedDate))).ReverseMap();

            //CreateMap<AddTransactionCommand, Transaction>();
            //CreateMap<ThirdPartyDepositTransactionCommand, Transaction>();
            //CreateMap<TransferTransactionCommand, Transaction>();
            //CreateMap<DepositTransactionCommand, Transaction>();
            //CreateMap<WithdrawalTransactionCommand, Transaction>();
            //CreateMap<InitialDepositCommand, Transaction>();
            //CreateMap<UpdateTransactionCommand, Transaction>();
        }
    }
}
