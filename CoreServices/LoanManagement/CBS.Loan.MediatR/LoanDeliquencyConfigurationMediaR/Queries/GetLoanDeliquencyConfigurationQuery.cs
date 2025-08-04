using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.LoanDeliquencyConfigurationMediaR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Loan by its unique identifier.
    /// </summary>
    public class GetLoanDeliquencyConfigurationQuery : IRequest<ServiceResponse<LoanDeliquencyConfigurationDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the LoanDeliquencyConfiguration to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
