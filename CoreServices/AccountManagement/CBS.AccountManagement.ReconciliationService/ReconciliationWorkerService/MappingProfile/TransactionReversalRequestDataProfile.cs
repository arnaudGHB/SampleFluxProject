using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.MediatR.Commands;

namespace CBS.AccountManagement.API.Helpers.MappingProfile
{
    public class TransactionReversalRequestDataProfile : Profile
    {
        public TransactionReversalRequestDataProfile()
        {
            CreateMap<TransactionReversalRequestData, TransactionReversalRequestDataDto>().ReverseMap();
        }
    }
}