using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Dto.Transaction
{
    public class TransactionThirdPartyDto
    {
        public string Id { get; set; }
        public decimal Amount { get; set; }
        public string AccountNumber { get; set; }
        public string Status { get; set; }
        public string TransactionReference { get; set; }
        public string TransactionType { get; set; }
        public string Note { get; set; }
        public string TelephoneNumber { get; set; }
        public decimal Fee { get; set; }
        public decimal TotalCharge { get; set; }
        public DateTime TransactionDate { get; set; }
        public string AmountInWord { get; set; }
        public string BranchCode { get; set; }
        public string TellerCode { get; set; }
        public string ExternalReference { get; set; }
        public string ExternalApplicationName { get; set; }
    }

    public class TransferThirdPartyDto
    {
        public string Id { get; set; }
        public decimal Amount { get; set; }
        public string CustomerId { get; set; }
        public string SenderAccountNumber { get; set; }
        public string ReciverAccountNumber { get; set; }
        public string Status { get; set; }
        public string TransactionReference { get; set; }
        public string Note { get; set; }
        public decimal Fee { get; set; }
        public decimal TotalCharge { get; set; }
        public DateTime TransactionDate { get; set; }
        public string AmountInWord { get; set; }
        public string BranchCode { get; set; }
        public string TellerCode { get; set; }
        public string UserName { get; set; }
        public string ExternalReference { get; set; }
        public string ExternalApplicationName { get; set; }
    }
    public class TransferChargesDto
    {
        public decimal ServiceCharge { get; set; }
        public decimal TotalCharges { get; set; }
        public decimal FormChargeCharges { get; set; }
    }
}
