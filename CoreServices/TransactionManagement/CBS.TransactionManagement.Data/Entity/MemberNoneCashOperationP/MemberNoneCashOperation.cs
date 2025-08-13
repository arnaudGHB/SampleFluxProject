using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Entity.MemberNoneCashOperationP
{
    public class MemberNoneCashOperation:BaseEntity
    {
        public string Id { get; set; }
        public string MemberReference { get; set; }
        public string AccountNUmber { get; set; }
        public string ChartOfAccountId { get; set; }
        public string ChartOfAccountName { get; set; }
        public decimal Amount { get; set; }
        public string Note { get; set; }
        public string InitiatedByUSerId { get; set; }
        public string BookingDirection { get; set; }//Deposit Or Withdrawal
        public string InitiatedUserName { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime AccountingDate { get; set; }
        public string ApprovedByUSerId { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime ApprovalDate { get; set; }
        public string? ApprovalStatus { get; set; }
        public string MemberName { get; set; }
        public string Source { get; set; }
        public string? ApprovalComment { get; set; }
        public string BranchId { get; set; }
        public string BranchName { get; set; }
        public string BranchCode { get; set; }
        public string AccountId { get; set; }
        public virtual Account Account { get; set; }
        public string TransactionReference { get; set; }
    }
}
