using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.MediatR.Commands;

namespace CBS.AccountManagement.API
{
    public class OperationEventProfile : Profile
    {
        public OperationEventProfile()
        {
            CreateMap<OperationEvent, OperationEventDto>().ReverseMap();
            CreateMap<AddOperationEventCommand, OperationEvent>();
            CreateMap<UpdateOperationEventCommand, OperationEvent>();
        }
    }
}