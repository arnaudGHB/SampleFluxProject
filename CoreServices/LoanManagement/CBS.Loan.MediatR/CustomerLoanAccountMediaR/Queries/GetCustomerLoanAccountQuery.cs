using CBS.NLoan.Data.Dto.CustomerLoanAccountP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.CustomerLoanAccountMediaR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Loan by its unique identifier.
    /// </summary>
    public class GetCustomerLoanAccountQuery : IRequest<ServiceResponse<CustomerLoanAccountDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the CustomerLoanAccount to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
