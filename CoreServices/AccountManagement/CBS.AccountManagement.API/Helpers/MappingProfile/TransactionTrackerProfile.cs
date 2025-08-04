using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Dto;
using CBS.AccountManagement.Data.Dto.ChartOfAccountDto;
using CBS.AccountManagement.MediatR.Commands;

namespace CBS.AccountManagement.API.Helpers.MappingProfile
{
    public class TransactionTrackerProfile : Profile
    {
        public TransactionTrackerProfile()
        {
            CreateMap<AddTransactionTrackerCommand, TransactionTracker>().ReverseMap();
            CreateMap<TransactionTrackerDto, TransactionTracker>().ReverseMap();
        }
    }
}