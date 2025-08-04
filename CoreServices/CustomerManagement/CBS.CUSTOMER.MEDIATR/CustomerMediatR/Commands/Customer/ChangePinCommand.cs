
using CBS.CUSTOMER.DATA.Dto.PinValidation;
using CBS.CUSTOMER.HELPER.Helper;
using MediatR;


namespace CBS.CUSTOMER.MEDIATR.CustomerMediatR
{
    /// <summary>
    /// Represents a command to verify a new ChangeValidation.
    /// </summary>
    public class ChangePinCommand : IRequest<ServiceResponse<ChangePinResponse>>
    {
        public string? Telephone { get; set; }
        public string? Pin { get; set; }
        public string? NewPin { get; set; }

    }

}
