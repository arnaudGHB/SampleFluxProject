using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.RescheduleLoanMediaR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Loan by its unique identifier.
    /// </summary>
    public class GetRescheduleLoanQuery : IRequest<ServiceResponse<RescheduleLoanDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the RescheduleLoan to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
