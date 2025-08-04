using CBS.NLoan.Data.Dto.WriteOffLoanP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.WriteOffLoanMediaR.Commands
{
    /// <summary>
    /// Represents a command to add a new Loan.
    /// </summary>
    public class AddWriteOffLoanCommand : IRequest<ServiceResponse<WriteOffLoanDto>>
    {
        public string LoanId { get; set; }
        public double OutstandingLoanBalance { get; set; }
        public double AccruedInterests { get; set; }
        public double AccruedPenalties { get; set; }
        public int PastDueDays { get; set; }
        public double OverduePrincipal { get; set; }
        public string Comment { get; set; }
        public string WriteOffMethod { get; set; }//Enum
        public string AccountingRuleId { get; set; }
        public string OrganizationId { get; set; }
        public string BankId { get; set; }
        public string BranchId { get; set; }
    }

}
