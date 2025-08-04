using CBS.TransactionManagement.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Dto.Transaction
{
    public class BulkDeposit
    {
        public string AccountNumber { get; set; }
        public decimal Fee { get; set; }
        public string CustomerId { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public decimal Penalty { get; set; }
        public decimal Interest { get; set; }
        public string? CheckNumber { get; set; }
        public bool IsSWS { get; set; }
        public string? CheckName { get; set; }
        public string? ExternalReference { get; set; }
        public bool IsExternalOperation { get; set; }
        public string? ExternalApplicationName { get; set; }
        public string? SourceType { get; set; }
        public string Currency { get; set; }
        public decimal Total { get; set; }
        public decimal MembershipActivationAmount { get; set; }
        public string? AccountType { get; set; }
        public string? LoanId { get; set; }
        public string? Note { get; set; }
        public string? OperationType { get; set; }
        public bool IsChargesInclussive { get; set; }

        public bool IsDepositDoneByAccountOwner { get; set; }
        public CurrencyNotesRequest? currencyNotesRequest { get; set; } = new CurrencyNotesRequest();
        public Depositer? Depositer { get; set; } = new Depositer();
        public CustomerDto? Customer { get; set; } = new CustomerDto();
        public BranchDto? Branch { get; set; } = new BranchDto();
        public List<CurrencyNotesDto>? currencyNotes { get; set; } = new List<CurrencyNotesDto>();
        public string? PaymentMethod { get; set; }
        public decimal Tax { get; set; }
        public decimal Principal { get; set; }
        public string? PaymentChannel { get; set; }

        public BulkDeposit()
        {
            Fee = 0;
            Amount = 0;
            Balance = 0;
            Penalty = 0;
            Interest = 0;
            Total = 0;
        }

    }

    public class BulkOperation
    {
        public string AccountNumber { get; set; }
        public decimal Fee { get; set; }
        public string CustomerId { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public decimal Penalty { get; set; }
        public decimal VAT { get; set; }
        public decimal Interest { get; set; }
        public decimal Total { get; set; }
        public decimal MembershipActivationAmount { get; set; }
        public string? AccountType { get; set; }
        public string? LoanId { get; set; }
        public string? LoanApplicationId { get; set; }
        public string? Note { get; set; }
        public string? OperationType { get; set; }
        public bool IsSWS { get; set; }
        public string? SourceType { get; set; }
        public string? CheckNumber { get; set; }
        public string? CheckName { get; set; }
        public bool isDepositDoneByAccountOwner { get; set; }
        public bool IsChargesInclussive { get; set; }
        public CurrencyNotesRequest? currencyNotes { get; set; } = new CurrencyNotesRequest();
        public Depositer? Depositer { get; set; } = new Depositer();

        public BulkOperation()
        {
            Fee = 0;
            Amount = 0;
            Balance = 0;
            Penalty = 0;
            Interest = 0;
            Total = 0;
        }
    }

    public class Depositer
    {
        public string? DepositorName { get; set; }
        public string? DepositerNote { get; set; }
        public string? DepositerTelephone { get; set; }
        public string? DepositorIDNumber { get; set; }
        public string? DepositorIDIssueDate { get; set; }
        public string? DepositorIDExpiryDate { get; set; }
        public string? DepositorIDNumberPlaceOfIssue { get; set; }
    }
}
