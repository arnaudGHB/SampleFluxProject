using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.LoanProductMediaR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Loan by its unique identifier.
    /// </summary>
    public class GetLoanProductRepaymentCycleQuery : IRequest<ServiceResponse<LoanProductRepaymentCycleDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the Loan to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
