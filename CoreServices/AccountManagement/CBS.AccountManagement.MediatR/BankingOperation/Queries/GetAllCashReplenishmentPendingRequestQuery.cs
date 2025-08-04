using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Entity;
using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific AccountType by its unique identifier.GetAllCashReplenishmentPendingRequestQuery
    /// </summary>
    public class GetAllCashReplenishmentPendingRequestQuery : IRequest<ServiceResponse<List<CashReplenishmentDto>>>
    {
        public string BranchId { get; set; }
    }
}