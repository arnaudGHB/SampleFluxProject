using CBS.NLoan.Data.Dto.CustomerLoanAccountP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.CustomerLoanAccountMediaR.Queries
{
    public class GetAllCustomerLoanAccountQuery : IRequest<ServiceResponse<List<CustomerLoanAccountDto>>>
    {
    }
}
