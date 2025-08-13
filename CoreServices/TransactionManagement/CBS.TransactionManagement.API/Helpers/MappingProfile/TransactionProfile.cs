using AutoMapper;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Dto.TransferLimits;
using CBS.TransactionManagement.Data.Entity.TransferLimits;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.MediatR.TellerP.Commands;
using Microsoft.AspNetCore.Mvc.TagHelpers;

namespace CBS.TransactionManagement.API
{
    public class TransactionProfile : Profile
    {
        public TransactionProfile()
        {
            CreateMap<Transaction, TransactionDto>();
    
    
            CreateMap<ThirdPartyDepositTransactionCommand, Transaction>();
            CreateMap<TransferTransactionCommand, Transaction>();
            CreateMap<DepositTransactionCommand, Transaction>();
            CreateMap<WithdrawalTransactionCommand, Transaction>();
            CreateMap<InitialDepositCommand, Transaction>();
            CreateMap<UpdateTransactionCommand, Transaction>();
            CreateMap<OpenningOfDaySubTellerCommand, Transaction>();
            CreateMap<Transaction, TransactionDto>().ReverseMap();
            CreateMap<TransactionDto, Transaction>().ReverseMap();
            CreateMap<TransactionDto, LoanRepaymentCommand>().ReverseMap();
            CreateMap<Transaction, LoanRepaymentCommand>().ReverseMap();

            CreateMap<TransferDto, Transfer>().ReverseMap();


        }
    }
}
