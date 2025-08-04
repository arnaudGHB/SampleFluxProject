using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.CashOutThirdPartyP.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific TempAccount by its unique identifier.
    /// </summary>
    public class GetDepositLimitQuery : IRequest<ServiceResponse<CashDepositParameterDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the TempAccount to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
