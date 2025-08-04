using AutoMapper;
using CBS.DailyCollectionManagement.API.Helpers;
 

namespace CBS.DailyCollectionManagement.API
{
    public static class MapperConfig
    {
        public static IMapper GetMapperConfigs()
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                //AddCommand 
                //mc.AddProfile(new TransactionTrackerProfile());
             
            });
            return mappingConfig.CreateMapper();
        }
    }
}