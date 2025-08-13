using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Entity.SalaryManagement.SalaryAnalysisResultP
{
    public class SalaryAnalysisResultDetail:BaseEntity
    {
        public string Id { get; set; }
        public string Matricule { get; set; }
        public string CustomerId { get; set; }
        public string MemberName { get; set; }
        public decimal NetSalary { get; set; }
        public decimal LoanCapital { get; set; }
        public decimal LoanInterest { get; set; }
        public decimal VAT { get; set; }
        public decimal TotalLoanRepayment { get; set; }
        public decimal Deposit { get; set; }
       
        public decimal Savings { get; set; }
        public decimal Charges { get; set; }
        public decimal Shares { get; set; }
        public decimal PreferenceShares { get; set; }
        public decimal RemainingSalary { get; set; }
        public string? LoanId { get; set; }
        public string? LoanType { get; set; }
        public string? LoanProductId { get; set; }
        public string? LoanProductName { get; set; }
        public bool IsOnldLoan { get; set; }
        public decimal StandingOrderAmount { get; set; }
        public string? StandingOrderStatement { get; set; }
        public string Status { get; set; }
        public string BranchName { get; set; }
        public string BranchCode { get; set; }
        public string BranchId { get; set; }
        public string SalaryAnalysisResultId { get; set; }
        public virtual SalaryAnalysisResult SalaryAnalysisResult { get; set; }

    }

}
