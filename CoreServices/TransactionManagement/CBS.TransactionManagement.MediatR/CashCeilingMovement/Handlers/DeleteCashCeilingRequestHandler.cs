using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.CashCeilingMovement.Commands;
using CBS.TransactionManagement.Repository.CashCeilingMovement;

namespace CBS.TransactionManagement.CashCeilingMovement.Handlers
{

    /// <summary>
    /// Handles the deletion of a cash ceiling request by marking it as deleted in the repository.
    /// </summary>
    public class DeleteCashCeilingRequestHandler : IRequestHandler<DeleteCashCeilingRequestCommand, ServiceResponse<bool>>
    {
        private readonly ICashCeilingRequestRepository _cashCeilingRequestRepository;
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly ILogger<DeleteCashCeilingRequestHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteCashCeilingRequestHandler"/> class.
        /// </summary>
        /// <param name="cashCeilingRequestRepository">Repository for cash ceiling request data access.</param>
        /// <param name="uow">Unit of work for managing database transactions.</param>
        /// <param name="logger">Logger for logging and debugging purposes.</param>
        public DeleteCashCeilingRequestHandler(
            ICashCeilingRequestRepository cashCeilingRequestRepository,
            IUnitOfWork<TransactionContext> uow,
            ILogger<DeleteCashCeilingRequestHandler> logger)
        {
            _cashCeilingRequestRepository = cashCeilingRequestRepository;
            _uow = uow;
            _logger = logger;
        }

        /// <summary>
        /// Handles the deletion of a cash ceiling request.
        /// </summary>
        /// <param name="request">Command containing the ID of the cash ceiling request to delete.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>
        /// A ServiceResponse indicating the success or failure of the deletion.
        /// </returns>
        public async Task<ServiceResponse<bool>> Handle(DeleteCashCeilingRequestCommand request, CancellationToken cancellationToken)
        {
            try
            {

                // Retrieve the cash ceiling request by ID
                var cashCeilingRequest = await _cashCeilingRequestRepository.FindAsync(request.Id);

                // Check if the request exists
                if (cashCeilingRequest == null)
                {
                    string errorMessage = $"Cash ceiling request with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.NotFound, LogAction.CashCeilingRequestDeletion, LogLevelInfo.Warning);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                // Mark the request as deleted
                cashCeilingRequest.IsDeleted = true;

                // Update the repository
                _cashCeilingRequestRepository.Update(cashCeilingRequest);

                // Save changes to the database
                await _uow.SaveAsync();

                // Log and audit successful deletion
                string successMessage = $"Cash ceiling request with ID {request.Id} was successfully deleted";
                _logger.LogInformation(successMessage);
                await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.CashCeilingRequestDeletion, LogLevelInfo.Information);

                // Return success response
                return ServiceResponse<bool>.ReturnResultWith200(true, successMessage);
            }
            catch (Exception ex)
            {
                // Handle unexpected exceptions
                string errorMessage = $"An error occurred while deleting the cash ceiling request with ID {request.Id}.";
                _logger.LogError(ex, errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.CashCeilingRequestDeletion, LogLevelInfo.Error);
                return ServiceResponse<bool>.Return500(ex);
            }
        }
    }
}
