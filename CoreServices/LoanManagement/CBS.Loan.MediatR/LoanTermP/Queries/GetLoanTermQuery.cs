using CBS.NLoan.Data.Dto.LoanTermP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.LoanTermP.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Loan by its unique identifier.
    /// </summary>
    public class GetLoanTermQuery : IRequest<ServiceResponse<LoanTermDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the LoanTerm to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
