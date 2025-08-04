

using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.HELPER.Helper;
using MediatR;


namespace CBS.Customer.MEDIATR.CustomerMediatR
{
    /// <summary>
    /// Represents a command to add a new OrganisationCustomer.
    /// </summary>
    public class AddOrganizationCustomerCommand : IRequest<ServiceResponse<CreateOrganizationCustomer>>
    {
        public string? CustomerId { get; set; }
        public string? OrganizationId { get; set; }
        public string? Position { get; set; }



    }

}
