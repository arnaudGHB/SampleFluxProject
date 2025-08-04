using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using MediatR;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to add a new AccountSubCategory.
    /// </summary>
    public class AddChartOfAccountManagementPositionCommand : IRequest<ServiceResponse<ChartOfAccountManagementPositionDto>>
    {
        public string PositionNumber { get; set; }
        public string Description { get; set; }
        public string RootDescription { get; set; }
        public string ChartOfAccountId { get; set; }
        public bool IsHeadOfficeAccount { get; set; }
    }

    /// <summary>
    /// Represents a command to add a new AccountSubCategory.
    /// </summary>
 
}