using CBS.NLoan.Data.Dto.RefundP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.RefundMediaR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Loan by its unique identifier.
    /// </summary>
    public class GetRefundQuery : IRequest<ServiceResponse<RefundDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the Refund to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
