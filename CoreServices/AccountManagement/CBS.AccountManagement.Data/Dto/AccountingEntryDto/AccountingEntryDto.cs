using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data
{
    public class AccountingEntryDto
    {

        // Unique ID number for the entry=
        public string Id { get; set; }

        // Date the accounting entry was created
        public DateTime EntryDate { get; set; }

        public string EntryDatetime { get; set; }

        // Effective date for posting the accounting impact
        public DateTime ValueDate { get; set; }

        // Type of entry (Debit or Credit)
        public string EntryType { get; set; }

      
        // Currency denomination 
        public string Currency { get; set; }

        // Text description explaining the purpose of the transaction
        public string Description { get; set; }

        // ID linking to source documents related to transaction
        public string ReferenceID { get; set; }
 
        public string Status { get; set; }
 
        public string ReviewedBy { get; set; }
        public string? DrAccountId { get; set; }
        public string? CrAccountId { get; set; }
        public string? DrAccountNumber { get; set; }
        public string? AccountNumber { get; set; }
        public string? AccountName { get; set; }
        public string? CrAccountNumber { get; set; }
        public decimal DrAmount { get; set; }
        public decimal CrAmount { get; set; }
        public decimal CurrentBalance { get; set; } = 0;
        public bool IsAuxilaryEntry { get; set; } = false;
        public string AccountNumberReference { get; set; }
        public string InitiatorId { get; set; } = "";
        public string Source { get; set; }
        public string? EventCode { get; set; }  // Generated from rule
        public string? BankId { get; set; } // Related Bank 
        public string? BranchId { get; set; } // Related branch 
        public string? CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public string ExternalBranchId { get; set; }

        public string OperationType { get; set; }
        public decimal DrBalanceBroughtForward { get; set; } = 0;
        public decimal CrBalanceBroughtForward { get; set; } = 0;
        public decimal Amount { get; set; }
        public decimal CrCurrentBalance { get; set; }
        public decimal DrCurrentBalance { get; set; }
        public string AccountId { get; set; }
        public string AccountCartegory { get; set; }
        public string EntryDateTime { get; set; }
        public string Representative { get; set; } 
        public string Naration { get; set; }
    }
    public class AccountingGeneralLedger
    {
        public string EntityId { get; set; }
        public string EntityType { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string? BranchName { get; set; }
        public string? BranchLocation { get; set; }
        public string? BranchAddress { get; set; }
        public string Capital { get; set; }
        public string ImmatriculationNumber { get; set; }
        public string WebSite { get; set; }
        public string BranchTelephone { get; set; }
        public string HeadOfficeTelePhone { get; set; }
        public string BranchCode { get; set; }
        public string? Name { get; set; }
        public string? Location { get; set; }
        public string? Address { get; set; }
        public string MainAccountNumber { get; set; }
        public List<AccountingEntryDto>? AccountingEntries { get; set; }
    }
    public class ReportHeader
    {
        public string EntityId { get; set; }
        public string EntityType { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string? BranchName { get; set; }
        public string? BranchLocation { get; set; }
        public string? BranchAddress { get; set; }
        public string Capital { get; set; }
        public string ImmatriculationNumber { get; set; }
        public string WebSite { get; set; }
        public string BranchTelephone { get; set; }
        public string HeadOfficeTelePhone { get; set; }
        public string BranchCode { get; set; }
        public string? Name { get; set; }
        public string? Location { get; set; }
        public string? Address { get; set; }
        public string MainAccountNumber { get; set; }
    }
        public class AccountingEntriesReport: ReportHeader
    {
    
        public List<AccountingEntryDto>? AccountingEntries { get; set; }
    }
    public class AccountLedgerDetails : ReportHeader
    {

        public List<LedgerDetails>? LedgerDetails { get; set; } = new List<LedgerDetails>();
    }
    public class LedgerDetails 
    {
       
        public string AccountNumber { get; set; }
        public string? AccountName { get; set; }
        public string? BeginningBalance { get; set; }
        public List<AccountingEntryDto>? AccountingEntries { get; set; }
         
    }
  
    public class AccountDetails: ReportHeader
    {
        public List<AccountData>? LedgerAccountDetails { get; set; }
      
    }
    public class AccountData
    {
        public string Cartegory { get; set; }
        public string AccountNumber { get; set; }
        public string? AccountName { get; set; }
        public string? CurrentBalance { get; set; }
    }
}
