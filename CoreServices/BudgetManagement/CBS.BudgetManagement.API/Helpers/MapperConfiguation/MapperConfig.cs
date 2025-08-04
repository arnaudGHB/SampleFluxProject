using AutoMapper;
 

namespace CBS.BudgetManagement.API
{
    public static class MapperConfig
    {
        public static IMapper GetMapperConfigs()
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                //
                mc.AddProfile(new BudgetItemProfile());
                mc.AddProfile(new BudgetAdjustmentProfile());
                mc.AddProfile(new BudgetCategoryProfile());
                mc.AddProfile(new ProjectBudgetProfile());
                mc.AddProfile(new ProjectProfile());
                mc.AddProfile(new SpendingLimitProfile());
                mc.AddProfile(new FiscalYearProfile());
                mc.AddProfile(new ExpenditureProfile());
                mc.AddProfile(new BudgetPlanProfile());
                
            });
            return mappingConfig.CreateMapper();
        }
    }
}