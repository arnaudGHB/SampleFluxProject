using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Commands
{
    public class DepositPostingEventCommand : IRequest<ServiceResponse<List<AccountingEntryDto>>>
    {
        /// <summary>
        /// Reference of Transaction
        /// </summary>
        public string? TransactionReferenceId { get; set; } // Loan contract reference
        /// <summary>
        /// EventCode  
        /// </summary>
        public string? EventCode { get; set; }  // Generated from rule
        /// <summary>
        /// Primary Amount
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// OwnerAccountId  
        /// </summary>
        public string? OwnerAccountId { get; set; }
        public string ProductId { get;   set; }
        public string ProductName { get;   set; }
        public DateTime TransactionDate { get; set; }
    }
}
