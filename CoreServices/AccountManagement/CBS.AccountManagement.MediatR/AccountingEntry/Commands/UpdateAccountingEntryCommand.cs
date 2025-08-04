using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to update a Account.
    /// </summary>
    public class UpdateAccountingEntryCommand : IRequest<ServiceResponse<AccountingEntryDto>>
    {
        public string? Id { get; set; } // Primary key, identity

        public string? Reference { get; set; } // Loan contract reference

        public string? DebitAccountNumber { get; set; }
        public string? CreditAccountNumber { get; set; }
        public decimal CrAmount { get; set; }
        public decimal DrAmount { get; set; }
        public DateTime TransactionDate { get; set; } // Date of transaction

        public DateTime? ExportDate { get; set; } // Date when exported

        public bool IsExported { get; set; } // Export status flag

        public string? CurrencyId { get; set; } // Currency reference

        public float ExchangeRate { get; set; } // Conversion rate 

        public string? AccountingRuleId { get; set; }  // Generated from rule

        public string? OrganizationId { get; set; } // Related branch 
        public string? BankId { get; set; } // Related branch 

        public string? BranchId { get; set; } // Related branch 

        public string? ClosureId { get; set; } // Link to branch closure

        public string? FiscalYearId { get; set; } // Fiscal year cont
    }
}