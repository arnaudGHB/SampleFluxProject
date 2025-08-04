using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Dto.SalaryManagement.StandingOrderP
{
    public class StandingOrderDto
    {
        public string Id { get; set; }
        public string MemberId { get; set; }
        public string MemberName { get; set; }

        public decimal Amount { get; set; }
        public string SourceAccountType { get; set; }// Salary,Deposit, MemberShare, Saving,PreferenceShare
        public string DestinationAccountType { get; set; } // Loan, Savings, Deposit, Shares
        public string Purpose { get; set; }// Loan Repayment, Savings, Increase Share, Transfer to Deposit, Increase Preference shares
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsAutomatic { get; set; }
        public string Frequency { get; set; } // Monthly, Weekly, etc.
        public string Priority { get; set; } // High, Medium, Low
        public string BranchId { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string UserName { get; set; }

        public bool? ExternalAccount { get; set; }

        public string? ExternalAccountNumber { get; set; }

        public string? ExternalAccountHolderName { get; set; }
        public string PersonalNote { get; set; }

    }

}
