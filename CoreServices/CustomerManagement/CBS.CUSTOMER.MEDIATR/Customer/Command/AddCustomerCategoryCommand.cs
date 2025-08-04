
using CBS.CUSTOMER.HELPER.Helper;
using MediatR;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.DATA;

namespace CBS.CUSTOMER.MEDIATR.CustomerMediatR
{
    /// <summary>
    /// Represents a command to add a new Customer Category.
    /// </summary>
    public class AddCustomerCategoryCommand : IRequest<ServiceResponse<CreateCustomerCategory>>
    {

       
        public string? Name { get; set; }
        public string? Description { get; set; }



    }

}
