
using CBS.Communication.Common;
using CBS.Communication.Domain.MongoDBContext.Repository.Generic;
using CBS.Communication.Domain.MongoDBContext.Repository.Uow;

namespace CBS.Communication.API.Helpers.DependencyResolver
{
    public static class DependencyInjectionExtension
    {
        public static void AddDependencyInjection(this IServiceCollection services)
        {
            services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
            services.AddScoped<IMongoUnitOfWork, MongoUnitOfWork>();
            services.AddScoped(typeof(IMongoGenericRepository<>), typeof(MongoGenericRepository<>));


        }
    }
}
