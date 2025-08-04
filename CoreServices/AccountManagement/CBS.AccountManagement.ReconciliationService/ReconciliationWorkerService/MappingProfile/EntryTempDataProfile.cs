using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.MediatR.Commands;

namespace CBS.AccountManagement.API
{
    public class EntryTempDataProfile : Profile
    {
        public EntryTempDataProfile()
        {
            CreateMap<EntryTempData, EntryTempDataDto>().ReverseMap();
            CreateMap<AddEntryTempDataCommand, EntryTempData>();
            CreateMap<UpdateEntryTempDataCommand, EntryTempData>();
        }
    }
}