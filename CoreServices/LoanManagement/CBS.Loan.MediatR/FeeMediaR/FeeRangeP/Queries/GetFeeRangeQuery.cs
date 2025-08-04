using CBS.NLoan.Data.Dto.FeeP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.FeeMediaR.FeeRangeP.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Loan by its unique identifier.
    /// </summary>
    public class GetFeeRangeQuery : IRequest<ServiceResponse<FeeRangeDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the Fee to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
