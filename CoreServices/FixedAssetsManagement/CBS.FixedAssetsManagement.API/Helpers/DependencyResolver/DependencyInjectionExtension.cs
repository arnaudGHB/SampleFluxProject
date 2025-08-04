using CBS.FixedAssetsManagement.Common;
using CBS.FixedAssetsManagement.MediatR;
using CBS.FixedAssetsManagement.Repository;

namespace CBS.FixedAssetsManagement.API.Helpers
{
    public static class DependencyInjectionExtension
    {
        public static void AddDependencyInjection(this IServiceCollection services)
        {
            //
            services.AddScoped<IAssetRepository, AssetRepository>();
            services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
            services.AddScoped<IAssetTypeRepository, AssetTypeRepository>();
            services.AddScoped<IAssetTransferRepository, AssetTransferRepository>();
            services.AddScoped<IAssetDisposalRepository, AssetDisposalRepository>();
            services.AddScoped<IAssetRevaluationRepository, AssetRevaluationRepository>();
            services.AddScoped<IDepreciationEntryRepository, DepreciationEntryRepository>();
            services.AddScoped<ILocationRepository, LocationRepository>();
            services.AddScoped<IDepreciationMethodRepository, DepreciationMethodRepository>();
            services.AddScoped<IMaintenanceLogRepository, MaintenanceLogRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();

        }
    }
}