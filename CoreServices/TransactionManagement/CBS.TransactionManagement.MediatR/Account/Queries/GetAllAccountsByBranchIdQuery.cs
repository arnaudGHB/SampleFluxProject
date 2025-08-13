using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Queries
{
    public class GetAllAccountsByBranchIdQuery : IRequest<ServiceResponse<List<AccountDto>>>
    {
        public string BranchId { get; set; }
    }
}
