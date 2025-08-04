using CBS.NLoan.Data.Dto.DocumentP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.DocumentMediaR.DocumentP.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Loan by its unique identifier.
    /// </summary>
    public class GetDocumentQuery : IRequest<ServiceResponse<DocumentDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the Document to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
