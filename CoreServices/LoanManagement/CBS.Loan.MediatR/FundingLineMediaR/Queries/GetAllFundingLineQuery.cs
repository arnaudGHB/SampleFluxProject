using CBS.NLoan.Data.Dto.FundingLineP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.FundingLineMediaR.Queries
{
    public class GetAllFundingLineQuery : IRequest<ServiceResponse<List<FundingLineDto>>>
    {
    }
}
