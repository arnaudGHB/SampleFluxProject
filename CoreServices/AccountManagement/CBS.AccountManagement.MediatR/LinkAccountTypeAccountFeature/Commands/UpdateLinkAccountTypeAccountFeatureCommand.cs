using CBS.AccountManagement.Data;

using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to update a customer.
    /// </summary>
    public class UpdateLinkAccountTypeAccountFeatureCommand : IRequest<ServiceResponse<LinkAccountTypeAccountFeatureDto>>
    {
        public string Id { get; set; }
        public string LinkName { get; set; }
        public string AccountTypeId { get; set; }
        public string AccountFeatureId { get; set; }
    }
}