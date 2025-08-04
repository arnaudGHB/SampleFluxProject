using CBS.NLoan.Data.Dto.CollateraP;
using CBS.NLoan.Data.Dto.Notifications;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.Notifications.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Loan by its unique identifier.
    /// </summary>
    public class GetOTPNotificationByCodeQuery : IRequest<ServiceResponse<OTPNotificationDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the Collateral to be retrieved.
        /// </summary>
        public string OTPCode { get; set; }
    }
}
