using CBS.BankMGT.Common.Repository.Generic;
using CBS.BankMGT.Common.Repository.Uow;
using CBS.BankMGT.Common.UnitOfWork;
using CBS.BankMGT.Repository;

namespace CBS.BankMGT.API.Helpers
{
    public static class DependencyInjectionExtension
    {
        public static void AddDependencyInjection(this IServiceCollection services)
        {
 
            services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
            services.AddScoped<IMongoUnitOfWork, MongoUnitOfWork>();
            services.AddScoped(typeof(IMongoGenericRepository<>), typeof(MongoGenericRepository<>));
            services.AddScoped<IBankingZoneRepository, BankingZoneRepository>();
            services.AddScoped<IBankZoneBranchRepository, BankZoneBranchRepository>();
            services.AddScoped<ICountryRepository, CountryRepository>();
            services.AddScoped<IRegionRepository, RegionRepository>();
            services.AddScoped<IThirdPartyBrancheRepository, ThirdPartyBrancheRepository>();
            services.AddScoped<IThirdPartyInstitutionRepository, ThirdPartyInstitutionRepository>();
            services.AddScoped<IDivisionRepository, DivisionRepository>();
            services.AddScoped<ISubdivisionRepository, SubdivisionRepository>();
            services.AddScoped<IBankRepository, BankRepository>();
            services.AddScoped<IBranchRepository, BranchRepository>();
            services.AddScoped<IOrganizationRepository, OrganizationRepository>();
            services.AddScoped<ITownRepository, TownRepository>();
            services.AddScoped<ICurrencyRepository, CurrencyRepository>();
            services.AddScoped<IEconomicActivityRepository, EconomicActivityRepository>();
            services.AddScoped<IFundingLineRepository, FundingLineRepository>();
            services.AddScoped<IInstallmentPeriodicityRepository, InstallmentPeriodicityRepository>();
            services.AddScoped<IAuditTrailRepository, AuditTrailRepository>();
            services.AddScoped<IDocumentUploadedRepository, DocumentUploadedRepository>();
        }
    }
}
