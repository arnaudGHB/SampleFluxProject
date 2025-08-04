using CBS.NLoan.Data;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Data.Entity.FeeP;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Data.Entity.PenaltyP;
using CBS.NLoan.Data.Entity.TaxP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.LoanProductMediaR.Commands
{
    /// <summary>
    /// Represents a command to add a new Loan.
    /// </summary>
    public class AddLoanProductRepaymentOrderCommand : IRequest<ServiceResponse<bool>>
    {
        public int InterestOrder { get; set; }
        public int CapitalOrder { get; set; }
        public int FineOrder { get; set; }
        public decimal InterestRate { get; set; }
        public decimal CapitalRate { get; set; }
        public decimal FineRate { get; set; }
        public string LoanDeliquencyPeriod { get; set; }
        public string? LoanProductRepaymentOrderType { get; set; }
        public string RepaymentTypeName { get; set; }

    }

}
