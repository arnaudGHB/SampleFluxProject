using CBS.AccountManagement.Data;

using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to update a UpdateDocumentCommand.
    /// </summary>
    public class UpdateDocumentCommand : IRequest<ServiceResponse<DocumentDto>>
    {

        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DocumentId { get; set; }
    }
}