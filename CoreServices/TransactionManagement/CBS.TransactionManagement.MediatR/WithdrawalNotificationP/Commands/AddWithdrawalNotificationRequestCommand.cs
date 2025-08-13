using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Dto.WithdrawalNotificationP;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Commands.WithdrawalNotificationP
{
    /// <summary>
    /// Represents a command to add a new WithdrawalLimits.
    /// </summary>
    public class AddWithdrawalNotificationRequestCommand : IRequest<ServiceResponse<WithdrawalNotificationDto>>
    {
        public string CustomerId { get; set; }
        public string AccountNumber { get; set; }
        public DateTime DateOfIntendedWithdrawal { get; set; }
        public DateTime GracePeriodDate { get; set; }
        public decimal AmountRequired { get; set; }
        public string ReasonForWithdrawal { get; set; }//LoanRepayment Or Consumption, Others
        public decimal AccountBalance { get; set; }
        public decimal LoanBalance { get; set; }
        public string? Purpose { get; set; }
        public decimal FormNotificationCharge { get; set; }
    }

}
