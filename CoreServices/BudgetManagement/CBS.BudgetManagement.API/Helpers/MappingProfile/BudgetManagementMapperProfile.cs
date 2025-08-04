using AutoMapper;
using CBS.BudgetManagement.Data;
using CBS.BudgetManagement.MediatR.Commands;

namespace CBS.BudgetManagement.API
{
    public class BudgetItemProfile : Profile
    {
        public BudgetItemProfile()
        {
            CreateMap<AddBudgetItemCommand, BudgetItemDto>().ReverseMap();
            CreateMap<AddBudgetItemCommand, BudgetItem>();
            CreateMap<UpdateBudgetItemCommand, BudgetItem>();
        }
    }

    public class BudgetAdjustmentProfile : Profile
    {
        public BudgetAdjustmentProfile()
        {
            CreateMap<AddBudgetAdjustmentCommand, BudgetAdjustmentDto>().ReverseMap();
            CreateMap<AddBudgetAdjustmentCommand, BudgetAdjustment>();
            CreateMap<UpdateBudgetAdjustmentCommand, BudgetAdjustment>();
        }
    }
    public class BudgetCategoryProfile : Profile
    {
        public BudgetCategoryProfile()
        {
            CreateMap<AddBudgetCategoryCommand, BudgetCategoryDto>().ReverseMap();
            CreateMap<AddBudgetCategoryCommand, BudgetCategory>();
            CreateMap<UpdateBudgetCategoryCommand, BudgetCategory>();
        }
    }
    public class ProjectBudgetProfile : Profile
    {
        public ProjectBudgetProfile()
        {
            CreateMap<AddProjectBudgetCommand, ProjectBudgetDto>().ReverseMap();
            CreateMap<AddProjectBudgetCommand, ProjectBudget>();
            CreateMap<UpdateProjectBudgetCommand, ProjectBudget>();
        }
    }

    public class ProjectProfile : Profile
    {
        public ProjectProfile()
        {
            CreateMap<AddProjectCommand, ProjectDto>().ReverseMap();
            CreateMap<AddProjectCommand, Project>();
            CreateMap<UpdateProjectCommand, Project>();
        }
    }

    public class ExpenditureProfile : Profile
    {
        public ExpenditureProfile()
        {
            CreateMap<AddExpenditureCommand, ExpenditureDto>().ReverseMap();
            CreateMap<AddExpenditureCommand, Expenditure>();
            CreateMap<UpdateExpenditureCommand, Expenditure>();
        }
    }
    public class FiscalYearProfile : Profile
    {
        public FiscalYearProfile()
        {
            CreateMap<AddFiscalYearCommand, FiscalYearDto>().ReverseMap();
            CreateMap<AddFiscalYearCommand, FiscalYear>();
            CreateMap<UpdateFiscalYearCommand, FiscalYear>();
        }

    }
    public class BudgetPlanProfile : Profile
    {
        public BudgetPlanProfile()
        {
            CreateMap<AddBudgetPlanCommand, BudgetPlanDto>().ReverseMap();
            CreateMap<AddBudgetPlanCommand, BudgetPlan>();
            CreateMap<UpdateBudgetPlanCommand, BudgetPlan>();
        }
    }
    public class SpendingLimitProfile : Profile
    {
        public SpendingLimitProfile()
        {
            CreateMap<AddSpendingLimitCommand, SpendingLimitDto>().ReverseMap();
            CreateMap<AddSpendingLimitCommand, SpendingLimit>();
            CreateMap<UpdateSpendingLimitCommand, SpendingLimit>();
        }
    }

}