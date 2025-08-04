using CBS.TransactionManagement.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Dto
{
    public class TellerDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string BankId { get; set; }
        public string BranchId { get; set; }
        public string? TellerType { get; set; }//VirtualTeller,PhysicalTeller,DailyCollectorTeller,NoneCashTeller
        public bool PerformCashIn { get; set; }
        public bool PerformCashOut { get; set; }
        public bool PerformTransfer { get; set; }
        public string? OperationType { get; set; }//Cash, NoneCash
        public string? AccountNumber { get; set; }
        public decimal MinimumAmountToManage { get; set; }
        public decimal MaximumAmountToManage { get; set; }
        public decimal MinimumDepositAmount { get; set; }
        public decimal MaximumDepositAmount { get; set; }
        public decimal MinimumWithdrawalAmount { get; set; }
        public decimal MaximumWithdrawalAmount { get; set; }
        public decimal MinimumTransferAmount { get; set; }
        public decimal MaximumTransferAmount { get; set; }
        public string? MapMobileMoneyToNoneMemberMobileMoneyReference { get; set; }
        public bool IsPrimary { get; set; }
        public bool InUseStatus { get; set; }
        public bool ActiveStatus { get; set; }
        public string? OperationEventCode { get; set; }
        public string? MobileMoneyUserKeepingThePhone { get; set; }
        public string? MobileMoneyFloatNumber { get; set; }
        public decimal MobileMoneyMinimumBalanceAlertLevel { get; set; }
        public decimal MobileMoneyMaximumBalanceAlertLevel { get; set; }

        public string? FromAuxillaryAccountNumber_A { get; set; }
        public string? ToBranchFloatAccountNumberAuxillary_A { get; set; }

        public string? FromHeadOfficeAccountNumber_B { get; set; }
        public string? ToBranchFloatAccountNumberHeadOffice_B { get; set; }

        public string? FromBranchAccountNumber_C { get; set; }
        public string? ToBranchFloatAccountNumberBranch_C { get; set; }

        public string? FromBranchFloatAccountNumber_D { get; set; }
        public string? ToHeadOfficeFloatAccountNumber_D { get; set; }

        public string? PhoneNumberToRecieveAlert { get; set; }
        public string? MobileMoneyAlertMessageInFrench { get; set; }
        public string? MobileMoneyAlertMessageInEnglish { get; set; }
        public List<TransactionDto> Transactions { get; set; }
        public List<PrimaryTellerProvisioningHistory> PrimaryTellerProvisioningHistories { get; set; }
        public List<SubTellerProvioningHistory> SubTellerProvioningHistories { get; set; }
    }
    public class CurrentProvisionPrimaryTellerDto
    {
        public string Id { get; set; }
        public string Text { get; set; }
    }


}
