using CBS.SystemConfiguration.Data;

using CBS.SystemConfiguration.Helper;

using MediatR;

namespace CBS.SystemConfiguration.MediatR.Commands
{
    /// <summary>
    /// Represents a command to update a UpdateCountryCommand.
    /// </summary>
    public class UpdateCountryCommand : IRequest<ServiceResponse<CountryDto>>
    {

        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

    }
}