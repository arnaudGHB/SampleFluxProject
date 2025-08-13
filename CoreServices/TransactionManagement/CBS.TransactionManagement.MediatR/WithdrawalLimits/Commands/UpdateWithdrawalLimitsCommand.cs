using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Commands
{
    /// <summary>
    /// Represents a command to update a WithdrawalLimits.
    /// </summary>
    public class UpdateWithdrawalLimitsCommand : IRequest<ServiceResponse<WithdrawalParameterDto>>
    {
        public string Id { get; set; }
        public string? ProductId { get; set; }
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public decimal PhysicalPersonWithdrawalFormFee { get; set; } = 0;
        public decimal MoralPersonWithdrawalFormFee { get; set; } = 0;
        public int NotificationPeriodInMonths { get; set; }
        public string WithdrawalType { get; set; }
        public decimal CamCCULShare { get; set; }
        public decimal FluxAndPTMShare { get; set; }
        public decimal HeadOfficeShare { get; set; }
        public decimal PartnerShare { get; set; }
        public decimal SourceBrachOfficeShare { get; set; }
        public decimal DestinationBranchOfficeShare { get; set; }
        public bool MustNotifyOnWithdrawal { get; set; }
        public string BankId { get; set; }
        public string? Path { get; set; }

    }

}
