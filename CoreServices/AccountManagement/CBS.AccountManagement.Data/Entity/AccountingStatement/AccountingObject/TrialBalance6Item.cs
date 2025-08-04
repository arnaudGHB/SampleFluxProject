using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data
{
    // Class representing each line item in the Trial Balance
    public class TrialBalance: BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? AccountNumber { get; set; }
        public string? AccountName { get; set; }
        public  double BeginningDebitBalance { get; set; }
        public double BeginningCreditBalance { get; set; }
        public double DebitBalance { get; set; }
        public double CreditBalance { get; set; }
        public double EndingDebitBalance { get; set; }
        public double EndingCreditBalance { get; set; }
        public string? Account1 { get; set; }
        public string? Account2 { get; set; }
        public string? Account3 { get; set; }
        public string? Account4 { get; set; }
        public string? Account5 { get; set; }

    }

    public class TrialBalance4column
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? AccountNumber { get; set; }
        public string? AccountName { get; set; }
        public double? BeginningBalance { get; set; }
        public double? DebitBalance { get; set; }
        public double? CreditBalance { get; set; }
        public double? EndingBalance { get; set; }


    }
}
