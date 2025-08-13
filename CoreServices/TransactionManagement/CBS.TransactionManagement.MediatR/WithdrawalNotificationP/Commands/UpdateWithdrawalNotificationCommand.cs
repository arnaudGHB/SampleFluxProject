using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Dto.WithdrawalNotificationP;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Commands.WithdrawalNotificationP
{
    /// <summary>
    /// Represents a command to update a WithdrawalLimits.
    /// </summary>
    public class UpdateWithdrawalNotificationCommand : IRequest<ServiceResponse<WithdrawalNotificationDto>>
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string AccountNumber { get; set; }
        public DateTime DateOfIntendedWithdrawal { get; set; }
        public DateTime GracePeriodDate { get; set; }
        public decimal AmountRequired { get; set; }
        public string ReasonForWithdrawal { get; set; }//LoanRepayment Or Consumption, Others
        public string? Purpose { get; set; }
        public decimal FormNotificationCharge { get; set; }
    }

}
