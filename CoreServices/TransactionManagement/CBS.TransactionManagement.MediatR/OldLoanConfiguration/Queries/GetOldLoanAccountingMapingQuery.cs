using CBS.TransactionManagement.Data.Dto.OldLoanConfiguration;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.OldLoanConfiguration.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific TempAccount by its unique identifier.
    /// </summary>
    public class GetOldLoanAccountingMapingQuery : IRequest<ServiceResponse<OldLoanAccountingMapingDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the TempAccount to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
