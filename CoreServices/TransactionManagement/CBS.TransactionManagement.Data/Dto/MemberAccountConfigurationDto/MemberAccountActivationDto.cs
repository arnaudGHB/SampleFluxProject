using CBS.TransactionManagement.Data.Entity.MemberAccountConfiguration;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Dto.MemberAccountConfiguration
{
    public class MemberAccountActivationDto
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public decimal EntranceFee { get; set; }
        public decimal ByeLawFee { get; set; }
        public decimal LoanPolicyFee { get; set; }
        public decimal BuildingContribution { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal Balance { get; set; }
        public decimal TotalFee { get; set; }
        public string BankId { get; set; }
        public string BranchId { get; set; }
        public string MemberAccountActivationPolicyId { get; set; }
        public virtual MemberRegistrationFeePolicy MemberAccountActivationPolicy { get; set; }

    }
}
