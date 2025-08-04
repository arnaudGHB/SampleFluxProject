using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Queries
{
    
    /// <summary>
    /// Represents a query to retrieve a specific AccountCategory by its unique identifier.
    /// </summary>
    public class GetDocumentRefQuery : IRequest<ServiceResponse<DocumentRef>>
    {
        
    }
}
