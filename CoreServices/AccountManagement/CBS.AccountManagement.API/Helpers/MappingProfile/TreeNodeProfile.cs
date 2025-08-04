using AutoMapper;
using CBS.AccountManagement.Data.Dto.ChartOfAccountDto;

namespace CBS.AccountManagement.API.Helpers.MappingProfile
{
    public class TreeNodeProfile : Profile
    {
        public TreeNodeProfile()
        {
            CreateMap<JsData, TreeNodeDto>().ReverseMap();
        }
    }
}