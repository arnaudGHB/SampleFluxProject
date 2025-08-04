using CBS.NLoan.Data.Dto.CommiteeP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.CommiteeMediaR.LoanCommeteeMemberMediaR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Loan by its unique identifier.
    /// </summary>
    public class GetLoanCommeteeMemberQuery : IRequest<ServiceResponse<LoanCommiteeMemberDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the LoanCommeteeMember to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
