using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.CycleNameMediaR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Loan by its unique identifier.
    /// </summary>
    public class GetLoanProductCategoryQuery : IRequest<ServiceResponse<LoanProductCategoryDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the LoanProductCategory to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
