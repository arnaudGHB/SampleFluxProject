using CBS.TransactionManagement.Data.Dto.SavingProductFeeP;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.SavingProductFeeP.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Loan by its unique identifier.
    /// </summary>
    public class GetSavingProductFeeQuery : IRequest<ServiceResponse<SavingProductFeeDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the SavingProductFee to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
