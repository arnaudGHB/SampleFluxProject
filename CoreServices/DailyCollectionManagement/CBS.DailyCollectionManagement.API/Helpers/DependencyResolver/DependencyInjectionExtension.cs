using CBS.DailyCollectionManagement.Common;
using CBS.DailyCollectionManagement.Common.DBConnection;
using CBS.DailyCollectionManagement.MediatR;
using CBS.DailyCollectionManagement.Repository;

namespace CBS.DailyCollectionManagement.API.Helpers
{
    public static class DependencyInjectionExtension
    {
        public static void AddDependencyInjection(this IServiceCollection services)
        {
            //
          
              services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));// : 
            
            services.AddScoped<IMongoUnitOfWork, MongoUnitOfWork>();
            services.AddScoped(typeof(IMongoGenericRepository<>), typeof(MongoGenericRepository<>));
         
        }
    }
}