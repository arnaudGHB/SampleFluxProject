using CBS.AccountManagement.Data;

using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to update a customer.
    /// </summary>
    public class UpdateAccountCartegoryCommand : IRequest<ServiceResponse<AccountCartegoryDto>>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

    }
}