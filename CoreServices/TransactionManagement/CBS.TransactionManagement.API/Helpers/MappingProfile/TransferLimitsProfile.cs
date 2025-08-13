using AutoMapper;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Dto;

namespace CBS.TransactionManagement.API
{
    public class TransferLimitsProfile : Profile
    {
        public TransferLimitsProfile()
        {
            CreateMap<TransferParameter, TransferParameterDto>().ReverseMap();
            CreateMap<AddTransferLimitsCommand, TransferParameter>();
            CreateMap<UpdateTransferLimitsCommand, TransferParameter>();
        }
    }
}
