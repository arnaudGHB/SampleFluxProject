using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Entity;
using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific AccountType by its unique identifier.
    /// </summary>
    public class GetCashReplenishmentByReferenceIdQuery : IRequest<ServiceResponse<CashReplenishmentDto>>
    {

        public string ReferenceId { get; set; }

    }
}