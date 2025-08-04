using CBS.NLoan.Data;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Data.Entity;
using CBS.NLoan.Data.Entity.FeeP;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Data.Entity.LoanTermP;
using CBS.NLoan.Data.Entity.PenaltyP;
using CBS.NLoan.Data.Entity.TaxP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.LoanProductMediaR.Commands
{
    /// <summary>
    /// Represents a command to add a new Loan.
    /// </summary>
    public class AddLoanProductCommand : IRequest<ServiceResponse<LoanProductDto>>
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public string TargetType { get; set; }
        public string LoanProductCategoryId { get; set; }
        public string LoanTermId { get; set; }
        public bool ActiveStatus { get; set; }
        public bool IsProductWithSavingFacilities { get; set; }

    }

}
