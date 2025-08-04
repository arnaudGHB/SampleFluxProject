using CBS.AccountManagement.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Commands
{
    public class MakeLoanRefinancingCommand : IRequest<ServiceResponse<bool>>
    {
        public string TransactionReferenceId { get; set; }
        public string MemberReference { get; set; }
        public string AccountHolder { get; set; }
        public string SavingProductId { get; set; }
        public string LoanProductId { get; set; }
        public string SavingProductName { get; set; }
        public string Naration { get; set; }
        public decimal AmountToDisbursed { get; set; }
        public decimal AmountToPayBackToCash { get; set; }
        public string DisbursementType { get; set; }
        public DateTime TransactionDate { get; set; }
        public bool IsCommissionFromMember { get; set; }
   
        public List<DisbursementCollection> DisbursementCollections { get; set; }
        public string GetOperationEventCode()
        {
            return this.SavingProductId + "@Principal_Saving_Account";
        }
        public string GetLoanperationEventCode()
        {
            return this.LoanProductId + "@Loan_Principal_Account";
            //   return this.LoanProductId + "@" + this.LoanProductName;
        }
        public string GetTransitOperationEventCode()
        {
            return this.LoanProductId + "@Loan_Transit_Account";

        }
        public string GetPrimaryTeller()
        {
            return "Physical_Teller";

        }
    }
}
