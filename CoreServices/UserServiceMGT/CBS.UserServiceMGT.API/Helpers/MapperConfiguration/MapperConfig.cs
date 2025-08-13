using AutoMapper;
using CBS.UserServiceMGT.API.Helpers;

namespace CBS.UserServiceMGT.API.Helpers
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
