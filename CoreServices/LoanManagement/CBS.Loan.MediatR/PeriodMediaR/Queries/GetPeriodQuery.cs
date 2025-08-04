using CBS.NLoan.Data.Dto.PeriodP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.PeriodMediaR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Loan by its unique identifier.
    /// </summary>
    public class GetPeriodQuery : IRequest<ServiceResponse<PeriodDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the Period to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
