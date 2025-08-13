using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Entity
{
    public class BlockedAccount:BaseEntity
    {
        public string Id { get; set; }
        public string AccountId { get; set; }
        public string AccountNumber{ get; set; }
        public string MemberReference { get; set; }
        public string MemberName { get; set; }
        public string Reason { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
        public string LoanApplicationId { get; set; }
        public decimal Amount { get; set; }
        public decimal AccountBalance { get; set; }
        public virtual Account Account { get; set; }
    }
}
