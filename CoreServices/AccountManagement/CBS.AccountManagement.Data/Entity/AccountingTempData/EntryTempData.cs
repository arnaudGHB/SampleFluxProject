using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data
{
    public class EntryTempData : BaseEntity
    {

        public string?   AccountName { get; set; }
        public string? AccountNumber { get; set; }

        public string BookingDirection { get; set; }

        public string? Status { get; set; } = "Pending";
        public decimal Amount { get; set; }
        public string? AccountBalance { get; set; }
        public string? Description { get; set; }

        public string? Reference { get; set; }
        [Key]
        public string Id { get; set; }
        public string? AccountId { get; set; }
        public string AccountingEventId { get; set; } = "MANUAL USER";
        public string PositingSource { get; set; }
        public bool? IsdoubbleValidationRequired { get; set; }
        public string BranchId { get; set; }
        public string ExternalBranchId { get; set; }
        public bool IsInterBranchTransaction { get; set; }

        public static bool  EvaluateEntry(List<EntryTempData> entries)
        {
            decimal totalDebit = 0;
            decimal totalCredit = 0;

            foreach (var entry in entries)
            {
                if (entry.BookingDirection.ToLower() == "debit")
                {
                    totalDebit += entry.Amount;
                }
                else if (entry.BookingDirection.ToLower() == "credit")
                {
                    totalCredit += entry.Amount;
                }
            }

            return totalDebit == totalCredit;
        }
    }
}
