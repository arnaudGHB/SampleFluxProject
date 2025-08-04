using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.MediatR.Commands;

namespace CBS.AccountManagement.API
{
    public class OperationEventAttributesProfile : Profile
    {
        public OperationEventAttributesProfile()
        {
            CreateMap<OperationEventAttributes, OperationEventAttributesDto>().ReverseMap();
            CreateMap<AddOperationEventAttributesCommand, OperationEventAttributes>();
            CreateMap<UpdateOperationEventAttributesCommand, OperationEventAttributes>();
        }
    }
}