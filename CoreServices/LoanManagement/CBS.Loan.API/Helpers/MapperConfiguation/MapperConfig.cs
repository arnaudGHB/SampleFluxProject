using AutoMapper;
using CBS.NLoan.API.Helpers.MappingProfile;

namespace CBS.NLoan.API.Helpers.MapperConfiguation
{
    public static class MapperConfig
    {
        public static IMapper GetMapperConfigs()
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new LoanProfile());
            });
            return mappingConfig.CreateMapper();
        }
    }
}
