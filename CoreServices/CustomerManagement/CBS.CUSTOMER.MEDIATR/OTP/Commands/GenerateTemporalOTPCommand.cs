using CBS.CUSTOMER.DATA.Dto.OTP;
using CBS.CUSTOMER.HELPER.Helper;
using MediatR;

namespace CBS.CUSTOMER.MEDIATR.OTP.Commands
{
    /// <summary>
    /// Represents a command to add a new Loan.
    /// </summary>
    public class GenerateTemporalOTPCommand : IRequest<ServiceResponse<TempOTPDto>>
    {
        public string Id { get; set; }
        public bool IsVerify { get; set; } = true;

    }
    //  {
    //  "userId": "650535634",
    //  "otpCode": "4500"
    //}
}
