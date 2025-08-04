using CBS.CUSTOMER.DATA.Dto;

using CBS.CUSTOMER.HELPER.Helper;
using MediatR;

namespace CBS.CUSTOMER.MEDIATR.CustomerMediatR.Commands
{
    /// <summary>
    /// Represents a command to update a customer matricle.
    /// </summary>
    public class UpdateCustomerMatriculeCommand : IRequest<ServiceResponse<UpdateCustomer>>
    {


        public string? CustomerId { get; set; }

        public string? Matricule { get; set; }
    }

}
