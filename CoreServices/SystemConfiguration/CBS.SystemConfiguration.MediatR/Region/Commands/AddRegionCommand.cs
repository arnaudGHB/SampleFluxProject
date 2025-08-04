using CBS.SystemConfiguration.Data;
using CBS.SystemConfiguration.Helper;
using MediatR;

namespace CBS.SystemConfiguration.MediatR.Commands
{
    /// <summary>
    /// Represents a command to add a new AddOperationEventNameCommand.
    /// </summary>
    public class AddRegionCommand : IRequest<ServiceResponse<RegionDto>>
    {
 
        public string Name { get; set; }

        public string CountryId { get; set; }


    }
}