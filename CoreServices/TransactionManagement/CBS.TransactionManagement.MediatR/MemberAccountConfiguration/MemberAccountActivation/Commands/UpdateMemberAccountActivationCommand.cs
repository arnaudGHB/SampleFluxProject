using CBS.TransactionManagement.Data.Dto.MemberAccountConfiguration;
using CBS.TransactionManagement.Data.Entity.MemberAccountConfiguration;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MemberAccountConfiguration.Commands
{
    /// <summary>
    /// Represents a command to update a TransferLimits.
    /// </summary>
    public class UpdateMemberAccountActivationCommand : IRequest<ServiceResponse<MemberAccountActivationDto>>
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public decimal CustomeAmount { get; set; }
        public string BankId { get; set; }
        public string BranchId { get; set; }

    }

}
