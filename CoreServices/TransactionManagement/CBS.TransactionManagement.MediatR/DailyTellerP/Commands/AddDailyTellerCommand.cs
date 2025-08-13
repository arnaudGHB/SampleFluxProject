using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.DailyTellerP.Commands
{
    /// <summary>
    /// Represents a command to add a new DepositLimit.
    /// </summary>
    public class AddDailyTellerCommand : IRequest<ServiceResponse<DailyTellerDto>>
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string ProvisionedBy { get; set; }
        public string TellerId { get; set; }
        public bool Status { get; set; }
        public bool IsPrimary { get; set; }
        public string UserBranchId { get; set; }
        public string BranchId { get; set; }
        public decimal MaximumWithdrawalAmount { get; set; }
        public decimal MaximumCeilin { get; set; }

    }

}
