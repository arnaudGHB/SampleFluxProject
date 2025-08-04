using CBS.SystemConfiguration.Data;

using CBS.SystemConfiguration.Helper;

using MediatR;

namespace CBS.SystemConfiguration.MediatR.Commands
{
    /// <summary>
    /// Represents a command to update a UpdateSubdivisionCommand.
    /// </summary>
    public class UpdateSubdivisionCommand : IRequest<ServiceResponse<SubdivisionDto>>
    {

        public string Id { get; set; }
        public string Name { get; set; }
        public string DivisionId { get; set; }


    }
}