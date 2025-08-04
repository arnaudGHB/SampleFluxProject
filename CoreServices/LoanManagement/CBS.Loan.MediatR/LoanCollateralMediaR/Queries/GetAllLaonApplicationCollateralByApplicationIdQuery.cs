using CBS.NLoan.Data.Dto.CollateraP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.LoanCollateralMediaR.Queries
{
    public class GetAllLaonApplicationCollateralByApplicationIdQuery : IRequest<ServiceResponse<List<LoanApplicationCollateralDto>>>
    {
        public string loanApplicationId { get; set; }
    }
}
