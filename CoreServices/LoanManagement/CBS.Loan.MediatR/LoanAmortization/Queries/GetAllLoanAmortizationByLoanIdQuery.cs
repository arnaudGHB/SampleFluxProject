using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.Queries
{
    public class GetAllLoanAmortizationByLoanIdQuery : IRequest<ServiceResponse<List<LoanAmortizationDto>>>
    {
        public string LoanId { get; set; }
    }
}
