using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using MediatR;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to add a new LinkAccountTypeAccountFeature.
    /// </summary>
    public class AddLinkAccountTypeAccountFeatureCommand : IRequest<ServiceResponse<LinkAccountTypeAccountFeatureDto>>
    {
        public string LinkName { get; set; }
        public string AccountTypeId { get; set; }
        public string AccountFeatureId { get; set; }
         
    }
}