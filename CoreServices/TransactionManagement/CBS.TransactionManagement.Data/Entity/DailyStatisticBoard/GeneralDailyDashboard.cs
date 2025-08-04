using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Entity.DailyStatisticBoard
{
    public class GeneralDailyDashboard
    {
        public string Id { get; set; }
        public string BranchId { get; set; }
        public string? BranchName { get; set; }
        public string? BranchCode { get; set; }
        public int NumberOfCashIn { get; set; }
        public int NumberOfCashOut { get; set; }
        public decimal TotalCashInAmount { get; set; }
        public decimal TotalCashOutAmount { get; set; }
        public int NewMembers { get; set; }
        public int ClosedAccounts { get; set; }
        public int ActiveAccounts { get; set; }
        public int DormantAccounts { get; set; }
        public int NumberOfInterBranchCashIn { get; set; }
        public int NumberOfRemiitanceInitiated { get; set; }
        public int NumberOfRemiitanceReception { get; set; }
        public decimal VolumeOfRemiitanceInitiated { get; set; }
        public decimal VolumeOfRemiitanceReception { get; set; }
        public int NumberOfInterBranchCashOut { get; set; }
        public decimal VolumeOfInterBranchCashIn { get; set; }
        public decimal VolumeOfInterBranchCashOut { get; set; }
        public decimal LoanDisbursements { get; set; }
        public decimal LoanRepayments { get; set; }
        public decimal ServiceFeesCollected { get; set; }
        public decimal InterestPaid { get; set; }
        public decimal Vat { get; set; }
        public decimal Penalties { get; set; }

        public decimal DailyExpenses { get; set; }
        public decimal OrdinaryShares { get; set; }
        public decimal PreferenceShares { get; set; }
        public decimal Savings { get; set; }
        public decimal Deposits { get; set; }
        public decimal CashInHand57 { get; set; }
        public decimal CashInHand56 { get; set; }
        public decimal MTNMobileMoney { get; set; }
        public decimal MobileMoneyCashOut { get; set; }
        public int NumberOfCashOutMTN { get; set; }

        public int NumberOfCashOutOrange { get; set; }
        public int NumberOfCashInMTN { get; set; }
        public int NumberOfLoanFee { get; set; }
        public int NumberOfLoanDisbursementFee { get; set; }
        public int NumberOfCashInOrange { get; set; }
        public decimal OrangeMoneyCashOut { get; set; }

        public decimal OrangeMoney { get; set; }
        public decimal DailyCollectionCashOut { get; set; }
        public decimal DailyCollectionCashIn { get; set; }
        public int NumberOfDailyCollectionCashOut { get; set; }
        public int NumberOfDailyCollectionCashIn { get; set; }

        public decimal MomocashCollection { get; set; }
        public decimal Transfer { get; set; }
        public int NumberOfTransfer { get; set; }
        public decimal PrimaryTillOpenOfDayBalance { get; set; }

        public decimal SubTillTillOpenOfDayBalance { get; set; }
        public DateTime Date { get; set; }
        public DateTime AccountingDate { get; set; }
        public decimal SubTillBalance { get; set; }
        public decimal PrimaryTillBalance { get; set; }
        public decimal CashReplenishmentSubTill { get; set; }
        public decimal CashReplenishmentPrimaryTill { get; set; }

    }

}
