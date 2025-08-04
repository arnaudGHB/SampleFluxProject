
using CBS.CUSTOMER.HELPER.Helper;
using MediatR;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.DATA.Dto.PinValidation;

namespace CBS.CUSTOMER.MEDIATR.CustomerMediatR
{
    /// <summary>
    /// Represents a command to verify a new PinValidation.
    /// </summary>
    public class PinValidationCommand : IRequest<ServiceResponse<PinValidationResponse>>
    {

        public string? Telephone { get; set; }
        public string? Pin { get; set; }
        public string? Channel { get; set; }

    }

}
