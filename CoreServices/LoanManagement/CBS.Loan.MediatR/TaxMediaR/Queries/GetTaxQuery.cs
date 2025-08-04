using CBS.NLoan.Data.Dto.TaxP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.TaxMediaR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Loan by its unique identifier.
    /// </summary>
    public class GetTaxQuery : IRequest<ServiceResponse<TaxDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the Tax to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
