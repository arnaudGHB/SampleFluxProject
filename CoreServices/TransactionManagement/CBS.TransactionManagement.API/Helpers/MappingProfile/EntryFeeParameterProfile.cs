using AutoMapper;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Dto;

namespace CBS.TransactionManagement.API
{
    public class EntryFeeParameterProfile : Profile
    {
        public EntryFeeParameterProfile()
        {
            CreateMap<EntryFeeParameter, EntryFeeParameterDto>().ReverseMap();
            CreateMap<AddEntryFeeParameterCommand, EntryFeeParameter>();
            CreateMap<UpdateEntryFeeParameterCommand, EntryFeeParameter>();
        }
    }
}
