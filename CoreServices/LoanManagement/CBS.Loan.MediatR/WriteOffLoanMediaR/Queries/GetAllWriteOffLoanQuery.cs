using CBS.NLoan.Data.Dto.WriteOffLoanP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.WriteOffLoanMediaR.Queries
{
    public class GetAllWriteOffLoanQuery : IRequest<ServiceResponse<List<WriteOffLoanDto>>>
    {
    }
}
