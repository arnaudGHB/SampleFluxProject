using CBS.NLoan.Data.Dto.WriteOffLoanP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.WriteOffLoanMediaR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Loan by its unique identifier.
    /// </summary>
    public class GetWriteOffLoanQuery : IRequest<ServiceResponse<WriteOffLoanDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the WriteOffLoan to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
