using CBS.SystemConfiguration.Data;
using CBS.SystemConfiguration.Helper;
using MediatR;

namespace CBS.SystemConfiguration.MediatR.Commands
{
    /// <summary>
    /// Represents a command to add a new AddOperationEventNameCommand.
    /// </summary>
    public class AddSubdivisionCommand : IRequest<ServiceResponse<SubdivisionDto>>
    {

        public string Id { get; set; }
        public string Name { get; set; }
        public string DivisionId { get; set; }


    }
}