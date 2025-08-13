using CBS.TransactionManagement.Data.Entity.FeeP;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Entity.MemberAccountConfiguration
{
    public class MemberAccountActivation : BaseEntity
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string FeeId { get; set; }
        public string? FeeName { get; set; }
        public decimal Amount { get; set; }
        public decimal CustomeAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public bool IsPaid { get; set; }
        public decimal Balance { get; set; }

        public DateTime DatePaid { get; set; } = DateTime.MaxValue;
        public string BankId { get; set; }
        public string BranchId { get; set; }
        public bool Status { get; set; }
        public virtual Fee Fee { get; set; }
    
    }
}
