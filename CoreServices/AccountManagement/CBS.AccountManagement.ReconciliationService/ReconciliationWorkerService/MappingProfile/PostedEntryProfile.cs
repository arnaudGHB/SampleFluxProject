using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.MediatR.Commands;

namespace CBS.AccountManagement.API
{
    public class PostedEntryProfile : Profile
    {
        public PostedEntryProfile()
        {
            CreateMap<PostedEntry, PostedEntryDto>().ReverseMap();
            CreateMap<AddEntryTempDataCommand, EntryTempData>();
            CreateMap<UpdateEntryTempDataCommand, EntryTempData>();
        }
    }
}