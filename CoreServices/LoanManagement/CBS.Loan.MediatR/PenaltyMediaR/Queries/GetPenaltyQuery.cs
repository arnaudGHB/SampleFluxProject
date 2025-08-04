using CBS.NLoan.Data.Dto.PenaltyP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.PenaltyMediaR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Loan by its unique identifier.
    /// </summary>
    public class GetPenaltyQuery : IRequest<ServiceResponse<PenaltyDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the Penalty to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
