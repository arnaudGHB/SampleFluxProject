using CBS.TransactionManagement.Data.Dto.MemberAccountConfiguration;
using CBS.TransactionManagement.Data.Entity.MemberAccountConfiguration;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MemberAccountConfiguration.Commands
{
    /// <summary>
    /// Represents a command to add a new MemberAccountActivation.
    /// </summary>
    public class AddMemberAccountActivationCommand : IRequest<ServiceResponse<decimal>>
    {
        public string CustomerId { get; set; }
        public List<MemberAccountActivation> MemberAccountActivations { get; set; }
        public string BankId { get; set; }
        public string BranchId { get; set; }
        public bool IsNew { get; set; }
    }

}
