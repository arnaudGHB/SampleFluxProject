using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Entity.CashCeilingMovement
{
    public class CashCeilingRequest:BaseEntity
    {
        public string Id { get; set; }
        public string TellerId { get; set; }
        public bool Status { get; set; }
        public string BranchId { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public decimal CashoutRequestAmount { get; set; }
        public string RequestType { get; set; }//Cash_To_Vault Or Subteller_Cash_To_PrimaryTeller
        public string? Requetcomment { get; set; }
        public string RequestedByUserId { get; set; }
        public string RequestedBy { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime ApprovedDate { get; set; }
        public DateTime InitializeDate { get; set; }
        public string? ApprovedComment { get; set; }
        public string? ApprovedStatus { get; set; }//Pending,Approved, Rejected
        public string TransactionReference { get; set; }
        public virtual Teller Teller { get; set; }
    }
}
