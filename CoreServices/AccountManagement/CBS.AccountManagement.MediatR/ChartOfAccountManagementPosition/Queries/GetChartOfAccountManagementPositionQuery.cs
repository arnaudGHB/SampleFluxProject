using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Queries
{
    public class GetChartOfAccountManagementPositionQuery : IRequest<ServiceResponse<ChartOfAccountManagementPositionDto>>
    {
        public string? Id { get; set; }
    }
}