using AutoMapper;
using CBS.CUSTOMER.API.Helpers.MappingProfile;

namespace CBS.CUSTOMER.API.Helpers.MapperConfiguation
{
    public static class MapperConfig
    {
        public static IMapper GetMapperConfigs()
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new CustomerProfiles());
                mc.AddProfile(new EmployeeProfile());
            });
            return mappingConfig.CreateMapper();
        }
    }
}
