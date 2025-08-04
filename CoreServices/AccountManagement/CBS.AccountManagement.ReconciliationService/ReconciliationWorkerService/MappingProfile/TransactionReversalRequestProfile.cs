using AutoMapper;
using CBS.AccountManagement.Data;

namespace CBS.AccountManagement.API.Helpers.MappingProfile
{
    public class TransactionReversalRequestProfile : Profile
    {
        public TransactionReversalRequestProfile()
        {
            CreateMap<TransactionReversalRequest, TransactionReversalRequestDto>().ReverseMap();
        }
    }
}