using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to add a new AddOperationEventNameCommand.
    /// </summary>
    public class AddAccountPolicyCommand : IRequest<ServiceResponse<AccountPolicyDto>>
    {

        public string AccountId { get; set; }
        public decimal MaximumAlert { get; set; }
        public decimal MinimumAlert { get; set; }

        public string Name { get; set; }
        public string MinMessage { get; set; }

        public string MaxMessage { get; set; }

        public string PolicyOwner { get; set; }
    }
}