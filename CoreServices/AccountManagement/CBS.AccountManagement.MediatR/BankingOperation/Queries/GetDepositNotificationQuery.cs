using CBS.AccountManagement.Data;

using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific AccountType by its unique identifier.
    /// </summary>
    public class GetDepositNotificationQuery : IRequest<ServiceResponse<DepositNotificationDto>>
    {

        public string Id { get; set; }

    }
}