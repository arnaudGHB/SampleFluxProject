using AutoMapper;
using CBS.BankMGT.Data;

namespace CBS.BankMGT.API
{
    public static class MapperConfig
    {
        public static IMapper GetMapperConfigs()
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                
                 
                mc.AddProfile(new BankZoneBranchProfile());
                mc.AddProfile(new BankingZoneProfile());
                mc.AddProfile(new ThirdPartyInstitutionProfile());
                mc.AddProfile(new ThirdPartyBrancheProfile());
                mc.AddProfile(new CountryProfile());
                mc.AddProfile(new RegionProfile());
                mc.AddProfile(new DivisionProfile());
                mc.AddProfile(new SubdivisionProfile());
                mc.AddProfile(new OrganizationProfile());
                mc.AddProfile(new BranchProfile());
                mc.AddProfile(new BankProfile());
                mc.AddProfile(new TownProfile());
                mc.AddProfile(new CurrencyProfile());
                mc.AddProfile(new EconomicActivityProfile());
                mc.AddProfile(new FundingLineProfile());
                mc.AddProfile(new InstallmentPeriodicityProfile());
                mc.AddProfile(new AuditTrailProfile());
                mc.AddProfile(new DocumentUploadedProfile());
            });
            return mappingConfig.CreateMapper();
        }
    }
}
