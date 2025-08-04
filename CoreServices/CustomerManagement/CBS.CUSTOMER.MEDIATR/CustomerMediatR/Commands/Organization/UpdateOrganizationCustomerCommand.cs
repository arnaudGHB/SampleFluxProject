using CBS.CUSTOMER.DATA.Dto;

using CBS.CUSTOMER.HELPER.Helper;
using MediatR;

namespace CBS.CUSTOMER.MEDIATR.CustomerMediatR.Commands
{
    /// <summary>
    /// Represents a command to update a OrganisationCustomer.
    /// </summary>
    public class UpdateOrganizationCustomerCommand : IRequest<ServiceResponse<UpdateOrganizationCustomer>>
    {
        public string? OrganizationCustomerId { get; set; }
        public string? Position { get; set; }



    }

}
