using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data
{
    public class PrimaryTellerProvisioningHistory : BaseEntity
    {
        [Key]
        public string Id { get; set; }
        public string TellerId { get; set; }
        public string ReferenceId { get; set; }
        public string? CloseOfDayReferenceId { get; set; }
        public string UserIdInChargeOfThisTeller { get; set; }
        public string ProvisionedBy { get; set; }
        public DateTime? OpenedDate { get; set; }
        public string PrimaryTellerId { get; set; }
        public DateTime? ClossedDate { get; set; } = new DateTime(1900, 1, 1);
        public decimal OpenOfDayAmount { get; set; } = 0;
        public decimal CashReplenishmentAmount { get; set; } = 0;
        public string? ReplenishmentReferenceNumber { get; set; }
        public bool IsCashReplenishment { get; set; }
        public decimal CashAtHand { get; set; } = 0;
        public decimal EndOfDayAmount { get; set; } = 0;
        public decimal AccountBalance { get; set; } = 0;
        public string? DailyTellerId { get; set; }
        public decimal PreviouseBalance { get; set; }
        public string LastUserID { get; set; }
        public string BankId { get; set; }
        public string BranchId { get; set; }
        public string ClossedStatus { get; set; }
        public string PrimaryTellerComment { get; set; }
        public virtual Teller Teller { get; set; }
        // Opening Notes and Coins Counts
        public int OpeningNote10000 { get; set; }
        public int OpeningNote5000 { get; set; }
        public int OpeningNote2000 { get; set; }
        public int OpeningNote1000 { get; set; }
        public int OpeningNote500 { get; set; }
        public int OpeningCoin500 { get; set; }
        public int OpeningCoin100 { get; set; }
        public int OpeningCoin50 { get; set; }
        public int OpeningCoin25 { get; set; }
        public int OpeningCoin10 { get; set; }
        public int OpeningCoin5 { get; set; }
        public int OpeningCoin1 { get; set; }

        // Closing Notes and Coins Counts
        public int ClosingNote10000 { get; set; }
        public int ClosingNote5000 { get; set; }
        public int ClosingNote2000 { get; set; }
        public int ClosingNote1000 { get; set; }
        public int ClosingNote500 { get; set; }
        public int ClosingCoin500 { get; set; }
        public int ClosingCoin100 { get; set; }
        public int ClosingCoin50 { get; set; }
        public int ClosingCoin25 { get; set; }
        public int ClosingCoin10 { get; set; }
        public int ClosingCoin5 { get; set; }
        public int ClosingCoin1 { get; set; }

        // Calculated Properties for Opening Denominations
        public decimal OpeningTotal10000 => OpeningNote10000 * 10000;
        public decimal OpeningTotal5000 => OpeningNote5000 * 5000;
        public decimal OpeningTotal2000 => OpeningNote2000 * 2000;
        public decimal OpeningTotal1000 => OpeningNote1000 * 1000;
        public decimal OpeningTotal500 => OpeningNote500 * 500;
        public decimal OpeningTotalCoin500 => OpeningCoin500 * 500;
        public decimal OpeningTotalCoin100 => OpeningCoin100 * 100;
        public decimal OpeningTotalCoin50 => OpeningCoin50 * 50;
        public decimal OpeningTotalCoin25 => OpeningCoin25 * 25;
        public decimal OpeningTotalCoin10 => OpeningCoin10 * 10;
        public decimal OpeningTotalCoin5 => OpeningCoin5 * 5;
        public decimal OpeningTotalCoin1 => OpeningCoin1 * 1;

        // Calculated Properties for Closing Denominations
        public decimal ClosingTotal10000 => ClosingNote10000 * 10000;
        public decimal ClosingTotal5000 => ClosingNote5000 * 5000;
        public decimal ClosingTotal2000 => ClosingNote2000 * 2000;
        public decimal ClosingTotal1000 => ClosingNote1000 * 1000;
        public decimal ClosingTotal500 => ClosingNote500 * 500;
        public decimal ClosingTotalCoin500 => ClosingCoin500 * 500;
        public decimal ClosingTotalCoin100 => ClosingCoin100 * 100;
        public decimal ClosingTotalCoin50 => ClosingCoin50 * 50;
        public decimal ClosingTotalCoin25 => ClosingCoin25 * 25;
        public decimal ClosingTotalCoin10 => ClosingCoin10 * 10;
        public decimal ClosingTotalCoin5 => ClosingCoin5 * 5;
        public decimal ClosingTotalCoin1 => ClosingCoin1 * 1;

        // Total Opening and Closing Amounts
        public decimal TotalOpeningAmount =>
            OpeningTotal10000 + OpeningTotal5000 + OpeningTotal2000 + OpeningTotal1000 + OpeningTotal500 +
            OpeningTotalCoin500 + OpeningTotalCoin100 + OpeningTotalCoin50 + OpeningTotalCoin25 +
            OpeningTotalCoin10 + OpeningTotalCoin5 + OpeningTotalCoin1;

        public decimal TotalClosingAmount =>
            ClosingTotal10000 + ClosingTotal5000 + ClosingTotal2000 + ClosingTotal1000 + ClosingTotal500 +
            ClosingTotalCoin500 + ClosingTotalCoin100 + ClosingTotalCoin50 + ClosingTotalCoin25 +
            ClosingTotalCoin10 + ClosingTotalCoin5 + ClosingTotalCoin1;

        public decimal LastOPerationAmount { get; set; }
        public string LastOperationType { get; set; }
    }
}

