using CBS.UserServiceManagement.Common;
using CBS.UserServiceManagement.Data;
using CBS.UserServiceManagement.Domain;
using CBS.UserServiceManagement.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace CBS.UserServiceManagement.API.Helpers.DependencyResolver
{
    public static class DependencyInjectionExtension
    {
        public static void AddInfrastructureServices(this IServiceCollection services)
        {
            // Enregistrement du UnitOfWork (Scoped: une instance par requête HTTP)
            services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));

            // Enregistrement de notre repository spécifique (Scoped)
            services.AddScoped<IUserRepository, UserRepository>();
            
            // Enregistrement de UserInfoToken (Scoped)
            services.AddScoped<UserInfoToken>();
        }
    }
}
