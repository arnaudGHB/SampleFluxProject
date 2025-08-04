using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to delete a ChartOfAccount.
    /// </summary>
    public class DeleteChartOfAccountCommandWithAccountNumber : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the ChartOfAccount to be deleted.
        /// </summary>
        public string Id { get; set; }
    }
}