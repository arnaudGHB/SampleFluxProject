using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Commands
{
    /// <summary>
    /// Represents a command to update a Transaction.
    /// </summary>
    public class UpdateLoanAccountBalanceCommand : IRequest<ServiceResponse<bool>>
    {
        public string CustomerId { get; set; }
        public decimal Balance { get; set; }
        public decimal Interest { get; set; }
        public string LoanId { get; set; }
        public string ExternalReference { get; set; }
    }

}
