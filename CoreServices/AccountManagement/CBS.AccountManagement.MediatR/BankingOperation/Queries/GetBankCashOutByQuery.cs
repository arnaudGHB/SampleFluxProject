using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Entity;
using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific AccountType by its unique identifier.
    /// </summary>
    public class GetBankCashOutTransactionByReferenceIdQuery : IRequest<ServiceResponse<BankTransactionDto>>
    {

        public string Id { get; set; }

    }
}