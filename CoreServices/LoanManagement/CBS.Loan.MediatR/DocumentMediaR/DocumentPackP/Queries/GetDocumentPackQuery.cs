using CBS.NLoan.Data.Dto.DocumentP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.DocumentMediaR.DocumentPackP.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Loan by its unique identifier.
    /// </summary>
    public class GetDocumentPackQuery : IRequest<ServiceResponse<DocumentPackDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the DocumentPack to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
