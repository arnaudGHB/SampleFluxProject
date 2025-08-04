using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to update a UpdateBudgetCommand.
    /// </summary>
    public class UpdateBudgetCommand : IRequest<ServiceResponse<BudgetDto>>
    {

        public string Id { get; set; }

        public string? Year { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

    }
}