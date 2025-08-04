using CBS.TransactionManagement.Data.Entity.SalaryFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Dto.SalaryManagement.SalaryAnalysisResultP
{
    public class SalaryAnalysisResultDto
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
        public int TotalBranches { get; set; }
        public string FileUploadId { get; set; }
        public FileUpload FileUpload { get; set; }
        public List<SalaryAnalysisResultDetailDto> salaryAnalysisResultDetails { get; set; }
        public object TotalPreferenceShares { get; set; }
    }
}
