using CBS.TransactionManagement.Data.Dto.ChargesWaivedP;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Queries.ChargesWaivedP
{
    /// <summary>
    /// Represents a query to retrieve a specific WithdrawalLimits by its unique identifier.
    /// </summary>
    public class GetChargesWaivedQuery : IRequest<ServiceResponse<ChargesWaivedDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the WithdrawalLimits to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
