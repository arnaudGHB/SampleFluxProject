using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Queries
{
    
    public class GetCashMovementTrackerQuery : IRequest<ServiceResponse<CashMovementTrackerDto>>
    {
      
        public string Id { get; set; }
    }
}