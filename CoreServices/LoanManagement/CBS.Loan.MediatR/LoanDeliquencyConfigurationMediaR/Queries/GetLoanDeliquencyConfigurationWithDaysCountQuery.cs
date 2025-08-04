using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.LoanDeliquencyConfigurationMediaR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Loan by its days count.
    /// </summary>
    public class GetLoanDeliquencyConfigurationWithDaysCountQuery : IRequest<ServiceResponse<LoanDeliquencyConfigurationDto>>
    {
        /// <summary>
        /// Gets or sets the days count of the LoanDeliquencyConfiguration to be retrieved.
        /// </summary>
        public int days { get; set; }
    }
}
