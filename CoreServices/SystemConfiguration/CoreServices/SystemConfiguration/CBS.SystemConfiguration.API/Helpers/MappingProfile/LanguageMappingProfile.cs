using AutoMapper;
using CBS.SystemConfiguration.Data.Dto.Languages;
using CBS.SystemConfiguration.Data.Entity;
using CBS.SystemConfiguration.MediatR.Language.Commands;

namespace CBS.SystemConfiguration.API.Helpers.MappingProfile
{
    public class LanguageMappingProfile : Profile
    {
        public LanguageMappingProfile()
        {
            CreateMap<Language, LanguageDto>().ReverseMap();
            CreateMap<AddLanguageCommand, Language>();
        }
    }
}
