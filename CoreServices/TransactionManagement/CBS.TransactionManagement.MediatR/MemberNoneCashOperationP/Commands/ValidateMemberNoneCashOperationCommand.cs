using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.MemberNoneCashOperationP.Commands
{
    /// <summary>
    /// Represents a command to update a CashReplenishment.
    /// </summary>
    /// <summary>
    /// Command to validate a Member None Cash Operation request.
    /// </summary>
    public class ValidateMemberNoneCashOperationCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// The ID of the Member None Cash Operation to validate.
        /// </summary>
        public string OperationId { get; set; }

        /// <summary>
        /// Indicates if the operation is validated (true) or rejected (false).
        /// </summary>
        public bool IsApproved { get; set; }

        /// <summary>
        /// The comment or note from the validator.
        /// </summary>
        public string ValidationComment { get; set; }

        public ValidateMemberNoneCashOperationCommand(string operationId, bool isApproved, string validationComment)
        {
            OperationId = operationId;
            IsApproved = isApproved;
            ValidationComment = validationComment;
        }
    }


}
