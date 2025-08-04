using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.LoanApplicationMediaR.Commands
{
    public class ValidateLoanCommand : IRequest<ServiceResponse<LoanApplicationDto>>
    {
        public string Id { get; set; }
    }
}
