using CBS.TransactionManagement.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Dto.ReversalRequestP
{
    public class ReversalRequestDto
    {
        public string Id { get; set; }
        public string TransactionReference { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; } // "Pending", "Approved", "Rejected","Validated"
        public string InitiatedBy { get; set; }
        public string ApprovedBy { get; set; }
        public string ValidatedBy { get; set; }
        public string ApprovedComment { get; set; }
        public string ValidationComment { get; set; }
        public bool IsValidated { get; set; }
        public string IncidentNote { get; set; }
        public bool IsAppoved { get; set; }
        public string DebitDirection { get; set; }
        public bool RequestStatus { get; set; }
        public DateTime ValidationDate { get; set; }
        public DateTime ApprovedDate { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime DateTreated { get; set; }
        public string? TreatedTellerName { get; set; }
        public string? TreatedTellerCode { get; set; }
        public string? TreatedUserName { get; set; }
        public string BranchId { get; set; }
        public string CustomerId { get; set; }
        public string AccountId { get; set; }
        public string AccountNumber { get; set; }
        public string TellerId { get; set; }
        public AccountDto Account { get; set; }
        public TellerDto Teller { get; set; }
        public List<TransactionDto> Transactions { get; set; }
    }
}
