using AutoMapper;
using CBS.SystemConfiguration.API.Helpers;
using CBS.SystemConfiguration.Data;

namespace CBS.SystemConfiguration.API
{
    public static class MapperConfig
    {
        public static IMapper GetMapperConfigs()
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                //AddCommand
 
                mc.AddProfile(new TownProfile());
                mc.AddProfile(new SubdivisionProfile());
                mc.AddProfile(new DivisionProfile());
                mc.AddProfile(new RegionProfile());
                mc.AddProfile(new CountryProfile());
 
            });
            return mappingConfig.CreateMapper();
        }
    }
}