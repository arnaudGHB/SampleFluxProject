using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Entity.ThirtPartyPayment
{
    public class GimacPayment
    {
        public string Id { get; set; }
        public string ItemType { get; set; }
        public string AccountNumber { get; set; }
        public string MemberName { get; set; }
        public string Msisdn { get; set; }
        public string MemberReferenceId { get; set; }
        public string Note { get; set; }
        public string InternalTransactionReference { get; set; }
        public string ExternalTransactionReference { get; set; }
        public string SourceType { get; set; }
        public string ApplicationName { get; set; }
        public string Status { get; set; }
        public decimal Amount { get; set; }
        public string BeneficialAccountNumber { get; set; }
        public DateTime Date { get; set; }
    }
}
