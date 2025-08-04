using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.Helper;
using MediatR;

namespace CBS.BankMGT.MediatR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Division by its unique identifier.
    /// </summary>
    public class GetDivisionQuery : IRequest<ServiceResponse<DivisionDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the Division to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
