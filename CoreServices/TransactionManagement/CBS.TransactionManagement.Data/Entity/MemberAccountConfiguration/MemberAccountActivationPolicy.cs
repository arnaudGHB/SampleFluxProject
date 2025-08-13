using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Entity.MemberAccountConfiguration
{
    public class MemberRegistrationFeePolicy:BaseEntity
    {
        public string Id { get; set; }
        public string PolicyName { get; set; }
        public decimal MinimumEntranceFee { get; set; }
        public decimal MaximumEntrancenFee { get; set; }
        public decimal MaximumByeLawsFee { get; set; }
        public decimal MaximumLoanPolicyFee { get; set; }
        public decimal MaximumBuildingContribution { get; set; }
        public bool IsActive { get; set; }
        public decimal MinimumByeLawsFee { get; set; }
       

        public decimal MinimumLoanPolicyFee { get; set; }

        public decimal MinimumBuildingContributionFee { get; set; }
        public decimal YearBuildingContributionFee { get; set; }
        public string LegalForm { get; set; }//Physical_Person Or Moral_Person
        public string AccountTypeForYearyDeductionOfBuildingContribution { get; set; }//Saving, Share, Deposit

        public string? EventCodeEntranceFee { get; set; }
        public string? EventCodeByeLawsFee { get; set; }
        public string? EventCodeLoanPolicyFee { get; set; }
        public string? EventCodeBuildingContributionFee { get; set; }

        public string BankId { get; set; }
        public virtual ICollection<MemberAccountActivation> MemberAccountActivations { get; set; }
    }
}
