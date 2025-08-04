using CBS.AccountManagement.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Commands
{
    public class CashRequisitionCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// Reference of Transaction
        /// </summary>
        public string? TransactionReferenceId { get; set; } // Loan contract reference
        /// <summary>
        /// EventCode "Cash_Requisition"
        /// </summary>
        public string? EventCode { get; set; }  // Generated from rule

        public string? Naration { get; set; }
        public decimal Amount { get; set; }
        public CurrencyNotesRequest CurrencyNotes { get; set; }

        public DateTime TransactionDate { get; set; }
 
    }
 
}