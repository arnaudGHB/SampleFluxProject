using CBS.SystemConfiguration.Data;

using CBS.SystemConfiguration.Helper;

using MediatR;

namespace CBS.SystemConfiguration.MediatR.Commands
{
    /// <summary>
    /// Represents a command to update a UpdateRegionCommand.
    /// </summary>
    public class UpdateRegionCommand : IRequest<ServiceResponse<RegionDto>>
    {

        public string Id { get; set; }
        public string Name { get; set; }
        public string CountryId { get; set; }

    }
}