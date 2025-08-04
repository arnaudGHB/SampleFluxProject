using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using MediatR;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to add a new customer.
    /// </summary>
    public class AddAccountFeatureCommand : IRequest<ServiceResponse<AccountFeatureDto>>
    {

        public string Name { get; set; }
        public string Description { get; set; }
        public AddAccountFeatureCommand()
        {
            
        }
    }
}