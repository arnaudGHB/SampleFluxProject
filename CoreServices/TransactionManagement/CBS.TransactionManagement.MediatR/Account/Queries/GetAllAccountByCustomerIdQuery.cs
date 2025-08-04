using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Queries
{
    public class GetAllAccountByCustomerIdQuery : IRequest<ServiceResponse<List<AccountDto>>>
    {
        public string CustomerId { get; set; }
    }
}
