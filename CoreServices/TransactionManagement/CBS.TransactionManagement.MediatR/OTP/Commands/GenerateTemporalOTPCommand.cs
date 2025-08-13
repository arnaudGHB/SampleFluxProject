using CBS.TransactionManagement.Data.Dto.OTP;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.OTP.Commands
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
