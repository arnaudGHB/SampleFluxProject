using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.AccountingEntry.Commands;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Commands
{
    public class LoanRefundCollection
    {

        public string EventAttributeName { get; set; }
        public decimal Amount { get; set; }
        public string Naration { get; set; }
        public string? GetOperationEventCode(string productId)
        {
            return productId + "@"+this.EventAttributeName;
        }
    }
    public class LoanRefundCollectionAlpha
    {
        public string AmountAccountNumber { get; set; }

        public string VatAccountNumber { get; set; }
        public string InterestAccountNumber { get; set; }
        public double AmountCapital { get; set; }
        public double AmountVAT { get; set; }
        public double AmountInterest { get; set; }
        public string VatNaration { get; set; }
        public string InterestNaration { get; set; }

        public static  LoanRefundCollectionAlpha FromLoanRefundCollectionAlpha(Data.BulkTransaction.LoanRefundCollectionAlpha loanRefundCollectionAlpha)
        {
            return new LoanRefundCollectionAlpha
            {
                VatAccountNumber = loanRefundCollectionAlpha.VatAccountNumber,
                InterestAccountNumber = loanRefundCollectionAlpha.InterestAccountNumber,
                AmountVAT= loanRefundCollectionAlpha.AmountVAT,
                AmountInterest = loanRefundCollectionAlpha.AmountInterest,
                VatNaration = loanRefundCollectionAlpha.VatNaration,
                InterestNaration = loanRefundCollectionAlpha.InterestNaration,
                AmountCapital = loanRefundCollectionAlpha.AmountCapital,
                AmountAccountNumber = loanRefundCollectionAlpha.AmountAccountNumber,
              

            };

        }

    }


    public class LoanRefundPostingCommand : IRequest<ServiceResponse<bool>>
    {
        public string TransactionReferenceId { get; set; }
        public string AccountNumber { get; set; }
        public bool IsOldSystemLoan { get; set; }
        public string LoanProductId { get; set; }
        public string BranchId { get; set; }
        public string Naration { get; set; }
        public string TellerSource { get; set; }
        public string? MemberReference { get; set; }
        public List<LoanRefundCollection>? AmountCollection { get; set; }
        public LoanRefundCollectionAlpha? LoanRefundCollectionAlpha { get; set; }

        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }

        public string GetLoanEventCode()
        {
            return this.LoanProductId + "@Loan_Principal_Account";

        }
        public LoanRefundPostingCommand()
        {
            AmountCollection = new List<LoanRefundCollection>();
            LoanRefundCollectionAlpha = new LoanRefundCollectionAlpha();
        }

    }

    public class BulkLoanRefundPostingCommand : IRequest<ServiceResponse<List<Data.AccountingEntry>>>
    {
        public string TransactionReferenceId { get; set; }
        public string AccountNumber { get; set; }
        public bool IsOldSystemLoan { get; set; }
        public string LoanProductId { get; set; }
        public string BranchId { get; set; }
        public string Naration { get; set; }
        public string FromProductId { get; set; }
        public string? MemberReference { get; set; }
        public List<LoanRefundCollection>? AmountCollection { get; set; }
        public LoanRefundCollectionAlpha? LoanRefundCollectionAlpha { get; set; }

        public DateTime TransactionDate { get; set; }
        public double Amount { get;   set; }

        public string GetLoanEventCode()
        {
            return this.LoanProductId + "@Loan_Principal_Account";

        }
        public BulkLoanRefundPostingCommand()
        {
            AmountCollection = new List<LoanRefundCollection>();
            LoanRefundCollectionAlpha = new LoanRefundCollectionAlpha();
        }

    }
}
