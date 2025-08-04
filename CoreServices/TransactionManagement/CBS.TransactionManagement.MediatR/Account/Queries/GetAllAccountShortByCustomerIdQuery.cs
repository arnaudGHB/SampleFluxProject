using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Queries
{
    public class GetAllAccountShortByCustomerIdQuery : IRequest<ServiceResponse<List<AccountShortDto>>>
    {
        public string CustomerId { get; set; }
    }
}
