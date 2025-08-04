using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Entity.MobileMoney
{
    public class MobileMoneyCashTopup : BaseEntity
    {
        public string Id { get; set; }
        public decimal Amount { get; set; }
        public string? OperatorType { get; set; }//MTN Or Orange
        public string? SourceType { get; set; }//GAV, OtherTransfer, HeadOffice, Wirering, M2Float
        public DateTime RequestDate { get; set; }
        public string BranchId { get; set; }
        public string BankId { get; set; }
        public string? RequestNote { get; set; }
        public string? RequestInitiatedBy { get; set; }
        public DateTime RequestApprovalDate { get; set; }
        public string? RequestApprovedBy { get; set; }
        public string? RequestApprovalStatus { get; set; }
        public string? RequestApprovalNote { get; set; }
        public string? RequestReference { get; set; }
        public string? MobileMoneyTransactionId { get; set; }
        public string? MobileMoneyMemberReference { get; set; }
        public string? AccountNumber { get; set; }
        public string? PhoneNumber { get; set; }
        public string? TellerId { get; set; }

    }
}
