
using CBS.CUSTOMER.HELPER.Helper;
using MediatR;
using CBS.CUSTOMER.DATA.Dto;

namespace CBS.CUSTOMER.MEDIATR.CustomerMediatR
{
    /// <summary>
    /// Represents a command to add a new customer.
    /// </summary>
    public class IDVerificationCommand : IRequest<ServiceResponse<String>>
    {

        public string? ValidationText { get; set; }
        public string? ImageSrc { get; set; }
   

    }

}
