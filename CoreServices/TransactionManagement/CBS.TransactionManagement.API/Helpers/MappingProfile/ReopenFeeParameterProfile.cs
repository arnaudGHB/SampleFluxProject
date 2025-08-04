using AutoMapper;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Dto;

namespace CBS.TransactionManagement.API
{
    public class ReopenFeeParameterProfile : Profile
    {
        public ReopenFeeParameterProfile()
        {
            CreateMap<ReopenFeeParameter, ReopenFeeParameterDto>().ReverseMap();
            CreateMap<AddReopenFeeParameterCommand, ReopenFeeParameter>();
            CreateMap<UpdateReopenFeeParameterCommand, ReopenFeeParameter>();
        }
    }
}
