using CBS.NLoan.Data.Dto.CommiteeP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.CommiteeMediaR.LoanCommiteeValidationCriteriaMediaR.Queries
{
    public class GetAllLoanCommiteeGroupQuery : IRequest<ServiceResponse<List<LoanCommiteeGroupDto>>>
    {
    }
}
