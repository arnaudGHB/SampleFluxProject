using CBS.SystemConfiguration.Data;
using CBS.SystemConfiguration.Helper;
using MediatR;

namespace CBS.SystemConfiguration.MediatR.Commands
{
    /// <summary>
    /// Represents a command to add a new AddOperationEventNameCommand.
    /// </summary>
    public class AddCountryCommand : IRequest<ServiceResponse<CountryDto>>
    {

        public string Code { get; set; }
        public string Name { get; set; }



    }
}