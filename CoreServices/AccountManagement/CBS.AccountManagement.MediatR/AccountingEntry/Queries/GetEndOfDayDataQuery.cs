using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Entity;
using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Queries
{
    public class GetEndOfDayDataQuery : SystemQuery,IRequest<ServiceResponse<CloseOfDayData>>
    {
  

    }
    public class CloseOfDayReconciliation
    {
        public string CashInHand { get; set; }
        public string CashInVault { get; set; }
        public string MobileMoneyMTN { get; set; }
        public string MobileMoneyOrange { get; set; }
        public string Savings { get; set; }
        public string Deposit { get; set; }
        public string OrdinaryShares { get; set; }
        public string PreferenceShare { get; set; }
        public string SalaryAccount { get; set; }
        public string DailyCollection { get; set; }
        public string Income { get; set; }
        public string ExpenseExcludingVAT { get; set; }
        public string LoanDisbursement { get; set; }
        public string LoanApproval { get; set; }
        public string VAT { get; set; }
    }
}