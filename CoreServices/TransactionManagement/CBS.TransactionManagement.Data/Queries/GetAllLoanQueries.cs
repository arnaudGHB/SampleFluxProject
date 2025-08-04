using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Dto.LoanRepayment;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Data.LoanQueries
{
    /// <summary>
    /// Represents a query to retrieve a specific Transaction by its unique identifier.
    /// </summary>
    public class GetAllLoanQueries : IRequest<ServiceResponse<List<Loan>>>
    {
        public string QueryParam { get; set; }
        public bool IsByBranch { get; set; }
        public string BranchId { get; set; }

    }
    public class GetCustomerLoan : IRequest<ServiceResponse<List<Loan>>>
    {
        public string CustomerId { get; set; }

    }

}
