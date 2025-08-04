using CBS.BudgetManagement.Common;
using CBS.BudgetManagement.MediatR;
using CBS.BudgetManagement.Repository;

namespace CBS.BudgetManagement.API.Helpers
{
    public static class DependencyInjectionExtension
    {
        public static void AddDependencyInjection(this IServiceCollection services)
        {
            //
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
            services.AddScoped<IProjectBudgetRepository, ProjectBudgetRepository>();
            services.AddScoped<IBudgetItemRepository, BudgetItemRepository>();
            services.AddScoped<IBudgetCategoryRepository, BudgetCategoryRepository>();
            services.AddScoped<IBudgetAdjustmentRepository, BudgetAdjustmentRepository>();
            services.AddScoped<IFiscalYearRepository, FiscalYearRepository>();
            services.AddScoped<IExpenditureRepository, ExpenditureRepository>();
            services.AddScoped<ISpendingLimitRepository, SpendingLimitRepository>();
            services.AddScoped<IBudgetPlanRepository, BudgetPlanRepository>();


        }
    }
}