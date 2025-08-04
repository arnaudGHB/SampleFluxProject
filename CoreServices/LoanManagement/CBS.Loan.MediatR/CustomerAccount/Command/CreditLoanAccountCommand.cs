using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.CustomerAccount.Command
{
    public class CreditLoanAccountCommand : IRequest<ServiceResponse<CreditLoanAccountCommandDto>>
    {
        public decimal Amount { get; set; }
        public string CustomerId { get; set; }
        public string BankId { get; set; }
        public string BranchId { get; set; }
        public string ReferenceNumber { get; set; }
        public string LoanProductId { get; set; }
        public string LoanProductName { get; set; }
        public string LoanApplicationType { get; set; }

    }
    public class CreditLoanAccountCommandDto
    {
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public DateTime AccountingDate { get; set; }
    }
}
