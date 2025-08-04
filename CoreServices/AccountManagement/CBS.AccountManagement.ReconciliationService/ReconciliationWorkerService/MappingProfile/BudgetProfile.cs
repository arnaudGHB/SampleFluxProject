using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.MediatR.Commands;

namespace CBS.AccountManagement.API
{
    public class BudgetProfile : Profile
    {
        public BudgetProfile()
        {
            CreateMap<Budget, BudgetDto>().ReverseMap();
            CreateMap<AddBudgetCommand, Budget>();
            CreateMap<UpdateBudgetCommand, Budget>();
        }
    }
}