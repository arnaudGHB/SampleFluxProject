using CBS.NLoan.Data.Dto.CommiteeP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.CommiteeMediaR.LoanCommeteeMemberMediaR.Queries
{
    public class GetAllLoanCommeteeMemberQuery : IRequest<ServiceResponse<List<LoanCommiteeMemberDto>>>
    {
    }
}
