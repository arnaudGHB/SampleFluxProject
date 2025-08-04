using CBS.NLoan.Data.Dto.FundingLineP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.FundingLineMediaR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Loan by its unique identifier.
    /// </summary>
    public class GetFundingLineQuery : IRequest<ServiceResponse<FundingLineDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the FundingLine to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
