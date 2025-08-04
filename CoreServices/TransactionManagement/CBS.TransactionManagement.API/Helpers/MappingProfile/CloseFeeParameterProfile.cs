using AutoMapper;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Dto;

namespace CBS.TransactionManagement.API
{
    public class CloseFeeParameterProfile : Profile
    {
        public CloseFeeParameterProfile()
        {
            CreateMap<CloseFeeParameter, CloseFeeParameterDto>().ReverseMap();
            CreateMap<AddCloseFeeParameterCommand, CloseFeeParameter>();
            CreateMap<UpdateCloseFeeParameterCommand, CloseFeeParameter>();
        }
    }
}
