using CBS.TransactionManagement.Data.Entity.SalaryFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Entity.SalaryManagement.SalaryAnalysisResultP
{
    public class SalaryAnalysisResult:BaseEntity
    {
        public string Id { get; set; }
        public string TotalMembers { get; set; }
        public decimal TotalNetSalary { get; set; }
        public decimal TotalLoanCapital { get; set; }
        public decimal TotalLoanInterest { get; set; }
        public decimal TotalVAT { get; set; }
        public decimal TotalLoanRepayment { get; set; }
        public decimal TotalDeposit { get; set; }
        public decimal TotalSavings { get; set; }
        public decimal TotalCharges { get; set; }
        public decimal TotalShares { get; set; }
        public decimal TotalRemainingSalary { get; set; }
        public decimal TotalPreferenceShares { get; set; }

        public int TotalBranches { get; set; }
        public string BranchId { get; set; }
        public string BranchName { get; set; }
        public string BranchCode { get; set; }
        public string FileUploadId { get; set; }
        public string ExecutedBy { get; set; }
        public DateTime Date { get; set; }
        public virtual FileUpload FileUpload { get; set; }
        public ICollection<SalaryAnalysisResultDetail> salaryAnalysisResultDetails { get; set; }
    }

}
