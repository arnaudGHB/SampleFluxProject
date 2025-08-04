using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using MediatR;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to add a new AddOperationEventNameCommand.
    /// </summary>
    public class AddDocumentCommand : IRequest<ServiceResponse<DocumentDto>>
    {
 
        public string Name { get; set; }
        public string Description { get; set; }
 

    }
}