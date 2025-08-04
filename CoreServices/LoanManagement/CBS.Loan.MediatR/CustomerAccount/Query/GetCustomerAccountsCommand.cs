using CBS.NLoan.Data.Dto.CustomerAccountDto;
using CBS.NLoan.Data.Dto.CustomerP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.CustomerP.Query
{
    public class GetCustomerAccountsCommand : IRequest<ServiceResponse<List<AccountDto>>>
    {
        public string CustomerId { get; set; }
    }
}
