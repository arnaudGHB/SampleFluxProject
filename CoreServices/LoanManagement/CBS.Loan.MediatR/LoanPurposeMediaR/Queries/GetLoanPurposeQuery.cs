using CBS.NLoan.Data.Dto.LoanPurposeP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.LoanPurposeMediaR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Loan by its unique identifier.
    /// </summary>
    public class GetLoanPurposeQuery : IRequest<ServiceResponse<LoanPurposeDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the LoanPurpose to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
