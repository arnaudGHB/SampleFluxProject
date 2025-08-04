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
    public class LoanDisbursementPostingCommand : IRequest<ServiceResponse<bool>>
    {
        public string TransactionReferenceId { get; set; }
        public string? MemberReference { get; set; }
        public string AccountHolder { get; set; }
        public string SavingProductId { get; set; }
        public string LoanProductId { get; set; }
        public string SavingProductName { get; set; }

        public string Naration { get; set; }
        public bool IsCommissionFromMember { get; set; } 
        /// <summary>
        /// True if the member is responsible for paying the commission
        /// </summary>
        public string TellerSourceForLoanCommission { get; set; } 
        /// <summary>
        /// MemberCommission
        /// Physical_Teller
        /// </summary>
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }

        public List<DisbursementCollection> DisbursementCollections { get; set; }
        public string GetOperationEventCode()
        {
            return this.SavingProductId + "@Principal_Saving_Account";
        }
        public string GetTransitOperationEventCode()
        {
            return this.LoanProductId + "@Loan_Transit_Account";
  
        }
    }


    public class DisbursementCollection
    {
        public string EventCode { get; set; }
        public decimal Amount { get; set; }
        public string Naration { get; set; }

        public string GetOperationEventCode()
        {
            return this.EventCode;
        }

    }
}
