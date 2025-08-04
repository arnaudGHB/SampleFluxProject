using CBS.NLoan.Data.Dto.AlertProfileP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.AlertProfileMediaR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Loan by its unique identifier.
    /// </summary>
    public class GetAlertProfileQuery : IRequest<ServiceResponse<AlertProfileDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the AlertProfile to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
