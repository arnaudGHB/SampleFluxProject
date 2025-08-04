using CBS.CUSTOMER.DATA.Dto;

using CBS.CUSTOMER.HELPER.Helper;
using MediatR;

namespace CBS.CUSTOMER.MEDIATR.CustomerMediatR.Commands
{
    /// <summary>
    /// Represents a command to update a customer.
    /// </summary>
    public class UpdateCustomerDateOfBirthCommand : IRequest<ServiceResponse<UpdateCustomerDateBirthDto>>
    {
        public string? CustomerId { get; set; }
        public DateTime DateOfBirth { get; set; }

    }

}
