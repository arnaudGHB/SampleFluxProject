using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Queries
{
    public class GetAllAccountStatisticsQuery : IRequest<ServiceResponse<AccountStatisticsDto>>
    {
        public string BranchId { get; set; }
        public string QueryParameter { get; set; }
    }

   
}
