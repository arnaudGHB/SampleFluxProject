using CBS.NLoan.Data.Dto.CollateraP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.LoanProductCollateralMediaR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Loan by its unique identifier.
    /// </summary>
    public class GetLoanProductCollateralQuery : IRequest<ServiceResponse<LoanProductCollateralDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the LoanProductCollateral to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
