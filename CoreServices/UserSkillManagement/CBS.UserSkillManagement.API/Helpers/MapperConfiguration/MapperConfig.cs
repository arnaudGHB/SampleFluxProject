using AutoMapper;
using CBS.UserSkillManagement.API.Helpers.MappingProfile;
using Microsoft.Extensions.DependencyInjection;

namespace CBS.UserSkillManagement.API.Helpers.MapperConfiguration
{
    public static class MapperConfig
    {
        public static IServiceCollection AddMapperConfiguration(this IServiceCollection services)
        {
            var mapperConfig = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new SkillMappingProfile());
                // Add other profiles here as needed
            });

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            return services;
        }
    }
}
