using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using MediatR;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to add a new AccountSubCategory.
    /// </summary>
    public class AddAccountClassCommand : IRequest<ServiceResponse<AccountClassDto>>
    {
       
        public string? AccountNumber { get; set; }
        public string? AccountCategoryId { get; set; }
    }
}