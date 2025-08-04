using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using MediatR;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to add a new AddOperationEventNameCommand.
    /// </summary>
    public class AddDocumentTypeCommand : IRequest<ServiceResponse<DocumentTypeDto>>
    {
 
        public string Name { get; set; }
        public string Description { get; set; }
        public string DocumentId { get; set; }

    }
}