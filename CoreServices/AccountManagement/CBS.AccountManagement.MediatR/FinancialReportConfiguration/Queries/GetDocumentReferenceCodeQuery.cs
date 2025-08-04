using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Queries
{
 
    public class GetDocumentReferenceCodeQuery : IRequest<ServiceResponse<DocumentReferenceCodeDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the AccountCategory to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
    public class GetCorrespondingMappingByDocumentReferenceCodeIdQuery : IRequest<ServiceResponse<List<CorrespondingMappingDto>>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the AccountCategory to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
    public class GetCorrespondingMappingExceptionsByDocumentReferenceCodeIdQuery : IRequest<ServiceResponse<List<CorrespondingMappingExceptionDto>>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the AccountCategory to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}