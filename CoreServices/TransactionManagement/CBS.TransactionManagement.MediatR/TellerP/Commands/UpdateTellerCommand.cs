using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.TellerP.Commands
{
    /// <summary>
    /// Represents a command to update a Teller.
    /// </summary>
    public class UpdateTellerCommand : IRequest<ServiceResponse<TellerDto>>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string BankId { get; set; }
        public string BranchId { get; set; }
        public string TellerType { get; set; }//VirtualTeller,PhysicalTeller,DailyCollectorTeller,NoneCashTeller
        public bool PerformCashIn { get; set; }
        public bool PerformCashOut { get; set; }
        public bool PerformTransfer { get; set; }
        public string OperationType { get; set; }//Cash, NoneCash
        public string? MapMobileMoneyToNoneMemberMobileMoneyReference { get; set; }

        public decimal MinimumAmountToManage { get; set; }
        public decimal MaximumAmountToManage { get; set; }
        public decimal MinimumDepositAmount { get; set; }
        public decimal MaximumDepositAmount { get; set; }
        public decimal MinimumWithdrawalAmount { get; set; }
        public decimal MaximumWithdrawalAmount { get; set; }
        public decimal MinimumTransferAmount { get; set; }
        public decimal MaximumTransferAmount { get; set; }
        public bool IsPrimary { get; set; }
        public bool ActiveStatus { get; set; }
    }

}
