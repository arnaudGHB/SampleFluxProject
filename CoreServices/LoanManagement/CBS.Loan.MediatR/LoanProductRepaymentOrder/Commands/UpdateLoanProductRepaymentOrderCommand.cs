using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.LoanProductMediaR.Commands
{
    /// <summary>
    /// Represents a command to update a Loan.
    /// </summary>
    public class UpdateLoanProductRepaymentOrderCommand : IRequest<ServiceResponse<LoanProductRepaymentOrderDto>>
    {
        public string Id { get; set; }
        public int InterestOrder { get; set; }
        public int CapitalOrder { get; set; }
        public int FineOrder { get; set; }
        public decimal InterestRate { get; set; }
        public decimal CapitalRate { get; set; }
        public decimal FineRate { get; set; }
        public string LoanDeliquencyPeriod { get; set; }
        public string RepaymentTypeName { get; set; }
        public string LoanProductId { get; set; }
    }

}
