using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.MediatR.Commands;

namespace CBS.AccountManagement.API.Helpers.MappingProfile
{
    public class OrganizationalUnitProfile : Profile
    {
        public OrganizationalUnitProfile()
        {
            CreateMap<OrganizationalUnit, OrganizationalUnitDto>().ReverseMap();
            CreateMap<AddOrganizationalUnitCommand, OrganizationalUnit>();
            //CreateMap<UpdateOrganizationalUnitCommand, OrganizationalUnit>();
        }
    }

    public class BudgetPeriodProfile : Profile
    {
        public BudgetPeriodProfile()
        {
            CreateMap<BudgetPeriod, BudgetPeriodDto>().ReverseMap();
            //CreateMap<AddBudgetPeriodCommand, BudgetPeriod>();
            //CreateMap<UpdateOrganizationalUnitCommand, OrganizationalUnit>(); BudgetPeriodProfile
        }
    }
}