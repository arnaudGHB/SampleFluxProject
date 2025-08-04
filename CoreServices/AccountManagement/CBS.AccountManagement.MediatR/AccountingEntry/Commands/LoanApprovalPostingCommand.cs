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
    public class LoanApprovalPostingCommand : IRequest<ServiceResponse<bool>>
    {
        public string TransactionReferenceId { get; set; }
        public string AccountNumber { get; set; }
        public string? MemberReference { get; set; }
        public string LoanProductId { get; set; }
        public string LoanProductName { get; set; }
        public string Naration { get; set; }
        public string BranchId { get; set; }
        public decimal Amount  { get; set; }
        public DateTime TransactionDate { get; set; }
        public string GetOperationEventCode()
        {
            return this.LoanProductId + "@Loan_Principal_Account";
         //   return this.LoanProductId + "@" + this.LoanProductName;
        }
        public string GetTransitOperationEventCode()
        {
            return this.LoanProductId + "@Loan_Transit_Account";
            //   return this.LoanProductId + "@" + this.865847643450339@;
        }
    }
   
}
