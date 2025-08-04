using CBS.NLoan.Data.Dto.FeeP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.LoanApplicationFeeMediaR.LoanApplicationFeeP.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Loan by its unique identifier.
    /// </summary>
    public class GetLoanApplicationFeeQuery : IRequest<ServiceResponse<LoanApplicationFeeDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the LoanApplicationFee to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
