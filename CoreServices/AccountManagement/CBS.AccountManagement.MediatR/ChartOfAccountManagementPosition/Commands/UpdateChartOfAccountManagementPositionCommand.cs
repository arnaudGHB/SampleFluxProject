using CBS.AccountManagement.Data;

using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to update a customer.
    /// </summary>
    public class UpdateChartOfAccountManagementPositionCommand : IRequest<ServiceResponse<ChartOfAccountManagementPositionDto>>
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string PositionNumber { get; set; }
        public string RootDescription { get; set; }
        public string ChartOfAccountId { get; set; }
 
    }
}