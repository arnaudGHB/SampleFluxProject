using CBS.AccountManagement.Data;

using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to update a customer.
    /// </summary>
    public class UpdateAccountClassCommand : IRequest<ServiceResponse<AccountClassDto>>
    {
        public string? Id { get; set; }
    
        public string? AccountNumber { get; set; }
 
        public string AccountCategoryId { get; internal set; }
    }
}