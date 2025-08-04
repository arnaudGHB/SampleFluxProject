using CBS.NLoan.Data.Dto.CollateraP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.CollateralMediaR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Loan by its unique identifier.
    /// </summary>
    public class GetCollateralQuery : IRequest<ServiceResponse<CollateralDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the Collateral to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
