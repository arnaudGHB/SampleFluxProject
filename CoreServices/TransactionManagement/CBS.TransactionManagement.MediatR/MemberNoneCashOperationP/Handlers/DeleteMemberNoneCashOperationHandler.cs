using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.MediatR.CashReplenishmentSubTellerP.Commands;
using CBS.TransactionManagement.Repository.MemberNoneCashOperationP;
using CBS.TransactionManagement.MediatR.MemberNoneCashOperationP.Commands;

namespace CBS.TransactionManagement.MediatR.MemberNoneCashOperationP.Handlers
{
    /// <summary>
    /// Handles the command to delete a Member None Cash Operation.
    /// </summary>
    public class DeleteMemberNoneCashOperationHandler : IRequestHandler<DeleteMemberNoneCashOperationCommand, ServiceResponse<bool>>
    {
        private readonly IMemberNoneCashOperationRepository _noneCashOperationRepository; // Repository for None Cash Operations.
        private readonly ILogger<DeleteMemberNoneCashOperationHandler> _logger; // Logger for logging actions and errors.
        private readonly UserInfoToken _userInfoToken; // Token containing user information for auditing.
        private readonly IUnitOfWork<TransactionContext> _uow; // Unit of work for handling transactions.

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteMemberNoneCashOperationHandler"/> class.
        /// </summary>
        public DeleteMemberNoneCashOperationHandler(
            IMemberNoneCashOperationRepository noneCashOperationRepository,
            ILogger<DeleteMemberNoneCashOperationHandler> logger,
            UserInfoToken userInfoToken,
            IUnitOfWork<TransactionContext> uow)
        {
            _noneCashOperationRepository = noneCashOperationRepository;
            _logger = logger;
            _userInfoToken = userInfoToken;
            _uow = uow;
        }

        /// <summary>
        /// Handles the command to delete a Member None Cash Operation.
        /// </summary>
        /// <param name="request">The command containing the operation ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token for the request.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteMemberNoneCashOperationCommand request, CancellationToken cancellationToken)
        {
            string logReference = BaseUtilities.GenerateUniqueNumber(); // Generate a log reference for tracking.

            try
            {
                // Step 1: Validate if the None Cash Operation exists.
                var operation = await _noneCashOperationRepository.FindAsync(request.OperationId);
                if (operation == null)
                {
                    string errorMessage = $"Member None Cash Operation with ID {request.OperationId} not found.";
                    _logger.LogWarning(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.NotFound, LogAction.MemberNoneCashOperationDelete, LogLevelInfo.Warning, logReference);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                // Step 2: Validate if the operation is still in pending status.
                if (operation.ApprovalStatus != Status.Pending.ToString())
                {
                    string errorMessage = "Cannot delete a none-cash operation that has already been approved or processed.";
                    _logger.LogWarning(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Forbidden, LogAction.MemberNoneCashOperationDelete, LogLevelInfo.Warning, logReference);
                    return ServiceResponse<bool>.Return403(errorMessage);
                }

                // Step 3: Perform a soft delete by marking the entity as deleted.
                operation.IsDeleted = true;
                _noneCashOperationRepository.Update(operation);

                // Step 4: Save the changes to the database.
                await _uow.SaveAsync();

                // Step 5: Log and audit the successful deletion.
                string successMessage = $"Member None Cash Operation with ID {request.OperationId} successfully deleted.";
                _logger.LogInformation(successMessage);
                await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.MemberNoneCashOperationDelete, LogLevelInfo.Information, logReference);

                // Return a successful response.
                return ServiceResponse<bool>.ReturnResultWith200(true, successMessage);
            }
            catch (Exception ex)
            {
                // Handle any unexpected exceptions during the process.
                string errorMessage = $"An error occurred while deleting the Member None Cash Operation: {ex.Message}";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.MemberNoneCashOperationDelete, LogLevelInfo.Error, logReference);
                return ServiceResponse<bool>.Return500(errorMessage);
            }
        }
    }
}
