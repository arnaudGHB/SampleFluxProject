using CBS.TransactionManagement.Data.Entity.SalaryFiles;
using CBS.TransactionManagement.Data.Entity.SalaryManagement.SalaryAnalysisResultP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Entity.SalaryFilesDto
{
    public class SalaryExtractDto
    {
        public string Id { get; set; }
        public decimal NetSalary { get; set; }
        public decimal Saving { get; set; }
        public decimal Deposit { get; set; }
        public decimal Shares { get; set; }
        public decimal Charges { get; set; }
        public decimal LoanCapital { get; set; }
        public decimal VAT { get; set; }
        public decimal TotalLoanRepayment { get; set; }
        public decimal LoanInterest { get; set; }
        public bool Status { get; set; }
        public decimal PreferenceShares { get; set; }
        public string? LoanProductId { get; set; }
        public string? LoanProductName { get; set; }
        public bool IsOnldLoan { get; set; }

        public decimal Salary { get; set; }
        public decimal RemainingSalary { get; set; }
        public string? LoanId { get; set; }
        public string? LoanType { get; set; }
        public decimal StandingOrderAmount { get; set; }
        public string? StandingOrderStatement { get; set; }
        public string BranchId { get; set; }
        public string? BranchCode { get; set; }
        public string? BranchName { get; set; }
        public string FileUploadId { get; set; }
        public string? FileUploadIdReferenceId { get; set; }
        public string Matricule { get; set; }
        public string MemberReference { get; set; }
        public string MemberName { get; set; }
        public string? UploadedBy { get; set; }
        public string? ExecutedBy { get; set; }
        public string SalaryAnalysisResultId { get; set; }
        public DateTime ExtrationDate { get; set; }
        public DateTime ExecutionDate { get; set; }
        public DateTime AccountingDate { get; set; }
        public virtual FileUpload FileUpload { get; set; }
        public virtual SalaryAnalysisResult SalaryAnalysisResult { get; set; }
    }
    public class SalaryProcessingDto
    {
        public decimal TotalDeposit { get; set; }
        public decimal TotalSaving { get; set; }
        public decimal TotalLoanCapital { get; set; }
        public decimal TotalInterest { get; set; }
        public decimal TotalVat { get; set; }
        public decimal TotalShares { get; set; }
        public int NumberOfDeposit { get; set; }
        public int NumberOfSaving { get; set; }
        public int NumberOfShare { get; set; }
        public int NumberOfLoanRepayment { get; set; }
        public decimal NetSalryUploaded { get; set; }
    }

}
