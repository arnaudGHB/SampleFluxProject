using AutoMapper;
using CBS.AccountManagement.Data;

using CBS.AccountManagement.MediatR.Commands;

namespace CBS.AccountManagement.API.Helpers.MappingProfile
{
    public class BudgetCategoryProfile : Profile
    {
        public BudgetCategoryProfile()
        {
            CreateMap<BudgetCategory, BudgetCategoryDto>().ReverseMap();
            CreateMap<AddBudgetCategoryCommand, BudgetCategory>();
            CreateMap<UpdateBudgetCategoryCommand, BudgetCategory>();
        }
    }
}