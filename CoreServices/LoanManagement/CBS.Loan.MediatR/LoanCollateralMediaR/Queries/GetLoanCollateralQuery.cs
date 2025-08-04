using CBS.NLoan.Data.Dto.CollateraP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.LoanCollateralMediaR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Loan by its unique identifier.
    /// </summary>
    public class GetLoanCollateralQuery : IRequest<ServiceResponse<LoanApplicationCollateralDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the LoanCollateral to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
