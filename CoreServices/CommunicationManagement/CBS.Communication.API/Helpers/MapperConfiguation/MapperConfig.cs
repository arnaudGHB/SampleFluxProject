using AutoMapper;
using CBS.Communication.API.Helpers.MappingProfile;

namespace CBS.Communication.API.Helpers.MapperConfiguation
{
    public static class MapperConfig
    {
        public static IMapper GetMapperConfigs()
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {

                mc.AddProfile(new CommunicationProfiles());
            
            });
            return mappingConfig.CreateMapper();
        }
    }
}
