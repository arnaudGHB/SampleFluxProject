using CBS.SystemConfiguration.Common;
using CBS.SystemConfiguration.MediatR;
using CBS.SystemConfiguration.Repository;

namespace CBS.SystemConfiguration.API.Helpers
{
    public static class DependencyInjectionExtension
    {
        public static void AddDependencyInjection(this IServiceCollection services)
        {
            //
            //services.AddScoped< IWebHostEnvironment,IWebHostEnvironment > ();
            services.AddScoped<ITownRepository, TownRepository>();
            services.AddScoped<IDivisionRepository, DivisionRepository>();
            services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
   
            services.AddScoped<ISubdivisionRepository, SubdivisionRepository>();
            services.AddScoped<IRegionRepository, RegionRepository>();
            services.AddScoped<ICountryRepository, CountryRepository>();
           
  
        }
    }
}