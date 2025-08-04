using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.MediatR.Commands;

namespace CBS.AccountManagement.API
{
    public class ChartOfAccountManagementPositionProfile : Profile
    {
        public ChartOfAccountManagementPositionProfile()
        {
            CreateMap<ChartOfAccountManagementPosition, ChartOfAccountManagementPositionDto>().ReverseMap();
            CreateMap<AddChartOfAccountManagementPositionCommand, ChartOfAccountManagementPosition>();
            CreateMap<UpdateChartOfAccountManagementPositionCommand, ChartOfAccountManagementPosition>();
        }
    }
}