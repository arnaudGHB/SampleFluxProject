using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Queries
{
    public class GetCustomerCommandQuery : IRequest<ServiceResponse<CustomerDto>>
    {
        public string CustomerID { get; set; }
    }
}
