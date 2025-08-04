using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanCollateralMediaR.Commands;
using CBS.NLoan.MediatR.LoanGuarantorMediaR.Commands;
using MediatR;

namespace CBS.NLoan.MediatR.CustomerAccount.Command
{
    /// <summary>
    /// Represents a command to add a new Loan.
    /// </summary>
    public class BlockListOfAccountBalanceCommand : IRequest<ServiceResponse<bool>>
    {
        public string LoanApplicationId { get; set; }
        public List<AccountToBlocked> AccountToBlockeds { get; set; }
    }
    public class AccountToBlocked
    {
        public string AccountNumber { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; }
    }
}
