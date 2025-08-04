using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Commands
{
    /// <summary>
    /// Represents a command to add a new Account.
    /// </summary>
    public class AddDefaultAccountCommand : IRequest<ServiceResponse<bool>>
    {
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
    }

}
