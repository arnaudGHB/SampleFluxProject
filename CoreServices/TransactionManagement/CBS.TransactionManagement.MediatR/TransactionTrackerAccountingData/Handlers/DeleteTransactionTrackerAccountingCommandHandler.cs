using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.MediatR.TransactionTrackerAccountingData.Commands;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Common.Repository.Uow;
using CBS.TransactionManagement.Data.Entity.MongoDBObjects;

namespace CBS.TransactionManagement.MediatR.TransactionTrackerAccountingData.Handlers
{
    /// <summary>
    /// Handles the command to delete a TransactionTrackerAccounting based on DeleteTransactionTrackerAccountingCommand.
    /// </summary>
    public class DeleteTransactionTrackerAccountingCommandHandler : IRequestHandler<DeleteTransactionTrackerAccountingCommand, ServiceResponse<bool>>
    {
        private readonly ILogger<DeleteTransactionTrackerAccountingCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMongoUnitOfWork _mongoUnitOfWork; // MongoDB Unit of Work.

        /// <summary>
        /// Constructor for initializing the DeleteTransactionTrackerAccountingCommandHandler.
        /// </summary>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mongoUnitOfWork">MongoDB Unit of Work.</param>
        public DeleteTransactionTrackerAccountingCommandHandler(
            ILogger<DeleteTransactionTrackerAccountingCommandHandler> logger,
            IMongoUnitOfWork mongoUnitOfWork)
        {
            _logger = logger;
            _mongoUnitOfWork = mongoUnitOfWork;
        }

        /// <summary>
        /// Handles the DeleteTransactionTrackerAccountingCommand to delete a TransactionTrackerAccounting entity.
        /// </summary>
        /// <param name="request">The DeleteTransactionTrackerAccountingCommand containing the ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteTransactionTrackerAccountingCommand request, CancellationToken cancellationToken)
        {
            var logAction = LogAction.DeleteTransactionTrackerAccounting;
            try
            {
                // Log the incoming request
                _logger.LogInformation("Attempting to delete TransactionTrackerAccounting with ID: {Id}", request.Id);

                // Get the MongoDB repository for TransactionTrackerAccounting
                var transactionTrackerRepository = _mongoUnitOfWork.GetRepository<TransactionTrackerAccounting>();

                // Check if the TransactionTrackerAccounting entity exists
                var existingEntity = await transactionTrackerRepository.GetByIdAsync(request.Id);
                if (existingEntity == null)
                {
                    var notFoundMessage = $"TransactionTrackerAccounting with ID {request.Id} not found.";
                    _logger.LogWarning(notFoundMessage);

                    // Log and audit the failed operation
                    await BaseUtilities.LogAndAuditAsync(notFoundMessage, request, HttpStatusCodeEnum.NotFound, logAction, LogLevelInfo.Warning, request.Id);

                    // Return a 404 response
                    return ServiceResponse<bool>.Return404(notFoundMessage);
                }

                // Delete the entity
                await transactionTrackerRepository.DeleteAsync(existingEntity.Id);

                // Log and audit the successful operation
                await BaseUtilities.LogAndAuditAsync(
                    $"Successfully deleted TransactionTrackerAccounting with ID: {request.Id}",
                    request,
                    HttpStatusCodeEnum.OK,
                    logAction,
                    LogLevelInfo.Information,
                    request.Id);

                // Return success response
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                var errorMessage = $"Error occurred while deleting TransactionTrackerAccounting with ID: {request.Id}. Error: {e.Message}";
                _logger.LogError(e, errorMessage);

                // Log and audit the failed operation
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, logAction, LogLevelInfo.Error, request.Id);

                // Return a 500 Internal Server Error response with error details
                return ServiceResponse<bool>.Return500(e, errorMessage);
            }
        }
    }

}
