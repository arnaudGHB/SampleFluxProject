using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Commands
{
    /// <summary>
    /// Represents a command to add a new Account.
    /// </summary>
    public class BlockAmountInAccountCommand : IRequest<ServiceResponse<bool>>
    {
        public string AccountNumber { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public string LoanApplicationId { get; set; }
    }
    
}
