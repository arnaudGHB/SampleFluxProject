using CBS.NLoan.Data.Dto.FundingLineP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.FundingLineMediaR.Commands
{
    /// <summary>
    /// Represents a command to add a new Loan.
    /// </summary>
    public class AddFundingLineCommand : IRequest<ServiceResponse<FundingLineDto>>
    {
        public string Name { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; }
        public string CurrencyId { get; set; }
        public string AccountingRuleId { get; set; }
        public string OrganizationId { get; set; }
        public string BankId { get; set; }
        public string BranchId { get; set; }
    }


}
