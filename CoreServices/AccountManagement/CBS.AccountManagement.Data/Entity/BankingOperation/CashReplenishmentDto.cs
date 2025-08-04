using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data.Entity 
{
    public class CashReplenishmentDto
    {
        public string  Id { get; set; }
        public string ReferenceId { get; set; }
        public decimal Amount { get; set; }
        public string RequestMessage { get; set; }
        public decimal AmountApproved { get; set; }
        public decimal AmountRequested { get; set; }
        public string IssuedBy { get; set; }
        public string IssuedDate { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime ApprovedDate { get; set; }
        public bool IsApproved { get; set; }
        public string CurrencyCode { get; set; }
        public string ApprovedMessage { get; set; }
        public string BranchId { get; set; }
        public string Status { get; set; }
        public string CashRequisitionType { get; set; } = "REQUEST";

        public string ParentCashReplenishId { get; set; }
        public string CorrespondingBranchId { get; set; }
        public string TempData { get; set; }
    }
}
