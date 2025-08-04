

using MediatR;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.DATA.Dto;

namespace CBS.CustomerSmsConfigurations.MEDIAT
{
    /// <summary>
    /// Represents a command to add a new CustomerSmsConfigurations.
    /// </summary>
    public class AddCustomerSmsConfigurationsCommand : IRequest<ServiceResponse<CreateCustomerSmsConfigurations>>
    {

          public string? Id { get; set; }
        public string? SmsTemplate { get; set; }

    }

}
