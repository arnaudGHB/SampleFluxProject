using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Dto
{
    public class SubTellerCashReplenishmentDto
    {
        public string Id { get; set; }
        public decimal RequestedAmount { get; set; }
        public decimal ConfirmedAmount { get; set; }
        public string RequesterUserId { get; set; }
        public string? ApprovedBy { get; set; }
        public string? ApprovedByUserId { get; set; }
        public DateTime ApprovedDate { get; set; }
        public DateTime InitializeDate { get; set; }
        public string? ApprovedComment { get; set; }
        public string? Requetcomment { get; set; }
        public string? ApprovedStatus { get; set; }//Pending,Approved, Rejected
        public string TellerId { get; set; }
        public string BranchId { get; set; }
        public string TransactionReference { get; set; }
        public string RequesterName { get; set; }

    }
}
