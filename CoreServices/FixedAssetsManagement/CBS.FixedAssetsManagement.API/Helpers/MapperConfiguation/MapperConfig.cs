using AutoMapper;



namespace CBS.FixedAssetsManagement.API
{
    public static class MapperConfig
    {
        public static IMapper GetMapperConfigs()
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                //
                mc.AddProfile(new AssetProfile());
                mc.AddProfile(new AssetTypeProfile());
                mc.AddProfile(new AssetDisposalProfile());
                mc.AddProfile(new AssetTransferProfile());
                mc.AddProfile(new AssetRevaluationProfile());
                mc.AddProfile(new DepreciationEntryProfile());
                mc.AddProfile(new DepreciationMethodProfile());
                mc.AddProfile(new DepreciationMethodProfile());
                mc.AddProfile(new MaintenanceLogProfile());
       
            });
            return mappingConfig.CreateMapper();
        }
    }
}