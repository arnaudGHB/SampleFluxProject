using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.LoanGuarantorMediaR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Loan by its unique identifier.
    /// </summary>
    public class GetLoanGuarantorQuery : IRequest<ServiceResponse<LoanGuarantorDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the LoanGuarantor to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
