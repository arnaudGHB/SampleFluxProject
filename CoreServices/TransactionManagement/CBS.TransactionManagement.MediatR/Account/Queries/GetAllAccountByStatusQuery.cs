using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Queries
{
    public class GetAllAccountByStatusQuery : IRequest<ServiceResponse<List<AccountDto>>>
    {
        public string Status { get; set; }
    }
}
