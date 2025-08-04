using CBS.NLoan.Data.Dto.CommiteeP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.CommiteeMediaR.LoanCommiteeValidationMediaR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Loan by its unique identifier.
    /// </summary>
    public class GetLoanCommiteeValidationHistoryQuery : IRequest<ServiceResponse<LoanCommiteeGroupDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the LoanCommiteeValidation to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
