using CBS.NLoan.Data.Dto.TaxP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.TaxMediaR.Commands
{
    /// <summary>
    /// Represents a command to add a new Loan.
    /// </summary>
    public class AddTaxCommand : IRequest<ServiceResponse<TaxDto>>
    {
        public string Name { get; set; }
        public decimal TaxRate { get; set; }
        public bool AppliedWhenLoanRequestIsGreaterThanSaving { get; set; }
        public bool AppliedOnInterest { get; set; }
        public decimal SavingControlAmount { get; set; }
        public bool IsVat { get; set; }

        public string? Description { get; set; }

    }

}
