using AutoMapper;
using CBS.UserServiceManagement.API.Helpers;

namespace CBS.UserServiceManagement.API.Helpers
{
    public static class MapperConfig
    {
        public static IMapper GetMapperConfigs()
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new UserMappingProfile());
            });
            return mappingConfig.CreateMapper();
        }
    }
}
