using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.LoanApplicationMediaR.Queries
{
    public class GetAllLoanApplicationQuery : IRequest<ServiceResponse<List<LoanApplicationDto>>>
    {
        public string ParamMeter { get; set; }
    }
}
