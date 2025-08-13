using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.MemberNoneCashOperationP.Commands
{

    /// <summary>
    /// Represents a command to delete a Member None Cash Operation.
    /// </summary>
    public class DeleteMemberNoneCashOperationCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the Member None Cash Operation to be deleted.
        /// </summary>
        public string OperationId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteMemberNoneCashOperationCommand"/> class.
        /// </summary>
        /// <param name="operationId">The unique identifier of the Member None Cash Operation to be deleted.</param>
        public DeleteMemberNoneCashOperationCommand(string operationId)
        {
            OperationId = operationId;
        }
    }


}
