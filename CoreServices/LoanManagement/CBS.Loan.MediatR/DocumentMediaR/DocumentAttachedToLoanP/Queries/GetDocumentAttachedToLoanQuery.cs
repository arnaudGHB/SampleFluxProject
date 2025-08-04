using CBS.NLoan.Data.Dto.DocumentP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.DocumentMediaR.DocumentAttachedToLoanP.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Loan by its unique identifier.
    /// </summary>
    public class GetDocumentAttachedToLoanQuery : IRequest<ServiceResponse<DocumentAttachedToLoanDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the DocumentAttachedToLoan to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
