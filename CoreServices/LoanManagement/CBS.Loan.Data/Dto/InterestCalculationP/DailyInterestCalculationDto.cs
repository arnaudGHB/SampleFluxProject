using CBS.NLoan.Data.Entity.LoanApplicationP;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.NLoan.Data.Entity.InterestCalculationP
{
    public class DailyInterestCalculation
    {
        public string Id { get; set; }
        public string LoanId { get; set; }
        public string CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string BranchId { get; set; }
        public decimal InterestCalculated { get; set; }
        public decimal PreviousBalance { get; set; }
        public decimal NewBalance { get; set; }
        public decimal DueAmount { get; set; }
        public decimal InterestRate { get; set; }
        public decimal LoanAmount { get; set; }
        public decimal VatRate { get; set; }
        public decimal CalculatedVat { get; set; }
        public decimal Fines { get; set; }
        public DateTime Date { get; set; }
        public virtual Loan Loan { get; set; }
    }
}
