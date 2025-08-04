using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Queries
{
    public class GetAllDocumentTypeQuery : IRequest<ServiceResponse<List<DocumentTypeDto>>>
    {

    }
    public class GetAllDocumentTypeByDocumentIdQuery : IRequest<ServiceResponse<List<DocumentTypeDto>>>
    {
        public string Id { get;   set; }
    }
}