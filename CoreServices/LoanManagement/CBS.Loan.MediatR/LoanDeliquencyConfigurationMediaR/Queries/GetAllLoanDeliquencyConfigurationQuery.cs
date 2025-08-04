using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.LoanDeliquencyConfigurationMediaR.Queries
{
    public class GetAllLoanDeliquencyConfigurationQuery : IRequest<ServiceResponse<List<LoanDeliquencyConfigurationDto>>>
    {
    }
}
