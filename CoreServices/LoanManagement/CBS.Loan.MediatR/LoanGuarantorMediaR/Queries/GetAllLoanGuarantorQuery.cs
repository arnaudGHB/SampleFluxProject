using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.LoanGuarantorMediaR.Queries
{
    public class GetAllLoanGuarantorQuery : IRequest<ServiceResponse<List<LoanGuarantorDto>>>
    {
    }
}
