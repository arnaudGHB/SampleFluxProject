using CBS.TransactionManagement.Command;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Command
{
  
    public class LoanRefundCollection
    {
        public string EventAttributeName { get; set; }
        public decimal Amount { get; set; }
        public string Naration { get; set; }
    }
    public class LoanRefundCollectionAlpha
    {
        public string VatAccountNumber { get; set; }
        public string InterestAccountNumber { get; set; }
        public string AmountAccountNumber { get; set; }
        public decimal AmountVAT { get; set; }
        public string VatNaration { get; set; }
        public string InterestNaration { get; set; }
        public decimal AmountInterest { get; set; }
        public decimal AmountCapital { get; set; }
    }

    public class AddLoanRefundPostingCommand : IRequest<ServiceResponse<bool>>
    {
        public string TransactionReferenceId { get; set; }
        public string AccountNumber { get; set; }
        public bool IsOldSystemLoan { get; set; }
        public string LoanProductId { get; set; }
        public string BranchId { get; set; }
        public string Naration { get; set; }
        public string MemberReference { get; set; }
        public string TellerSource { get; set; }
        public List<LoanRefundCollection>? AmountCollection { get; set; }
        public LoanRefundCollectionAlpha LoanRefundCollectionAlpha { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public AddLoanRefundPostingCommand()
        {
            AmountCollection= new List<LoanRefundCollection>();
            LoanRefundCollectionAlpha = new LoanRefundCollectionAlpha();
        }
    }

}


//public class DailyCollectionConfirmationPostingEventCommand
//{
//    public string DailyCollectorSource { get; set; }//DailyCollector
//    public string? TransactionReferenceId { get; set; }
//    public string? PhysicalTellerDestination { get; set; } // PhysicalTeller
//    public DateTime TransactionDate { get; set; }
//    public string? Amount { get; set; }


//}
//public class DailyCollectionMonthlyCommisionEventCommand
//{
//    public string ProductId { get; set; }//DailyCollector
//    public string? TransactionReferenceId { get; set; }
//    //  public string? RevenueAccount { get;  set; } // Commission_Account
//    public DateTime TransactionDate { get; set; }
//    public string? Amount { get; set; }


//}
//public class DailyCollectionMonthlyPayableEventCommand
//{
//    public string ProductId { get; set; }//DailyCollector

//    public string ExpenseEventCode { get; set; }//DailyCollector
//    public string? TransactionReferenceId { get; set; }
//    //  public string? RevenueAccount { get;  set; } // Commission_Account
//    public DateTime TransactionDate { get; set; }
//    public string? Amount { get; set; }


//}


//public class DailyCollectionPostingEventCommand : IRequest<ServiceResponse<bool>>
//{
//    public string TellerSource { get; set; }//DailyCollector
//    public string? TransactionReferenceId { get; set; } // Loan contract reference 
//    public string? ProductId { get; set; } // Daily saving productId
//    public DateTime TransactionDate { get; set; }

//    public List<DailyAmountCollection>? DailyAmountCollection { get; set; }
//}





