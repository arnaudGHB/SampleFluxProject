using CBS.NLoan.Data.Dto.CommiteeP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.CommiteeMediaR.LoanCommiteeValidationMediaR.Queries
{
    public class GetAllLoanCommiteeValidationHistoryQuery : IRequest<ServiceResponse<List<LoanCommiteeGroupDto>>>
    {
    }
}
