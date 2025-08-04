using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.MediatR.SalaryManagement.StandingOrderP.Commands;
using CBS.TransactionManagement.Repository.SalaryManagement.StandingOrderP;

namespace CBS.TransactionManagement.MediatR.SalaryManagement.StandingOrderP.Handlers
{
    /// <summary>
    /// Handles the command to add a new Transfer Transaction.
    /// </summary>
    /// 

    public class DeleteStandingOrderCommandHandler : IRequestHandler<DeleteStandingOrderCommand, ServiceResponse<bool>>
    {
        private readonly IStandingOrderRepository _standingOrderRepository;
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly ILogger<DeleteStandingOrderCommandHandler> _logger;

        /// <summary>
        /// Constructor for the DeleteStandingOrderCommandHandler.
        /// </summary>
        /// <param name="standingOrderRepository">Repository for managing standing orders.</param>
        /// <param name="uow">Unit of work for managing transactions.</param>
        /// <param name="logger">Logger instance for logging operations.</param>
        public DeleteStandingOrderCommandHandler(
            IStandingOrderRepository standingOrderRepository,
            IUnitOfWork<TransactionContext> uow,
            ILogger<DeleteStandingOrderCommandHandler> logger)
        {
            _standingOrderRepository = standingOrderRepository;
            _uow = uow;
            _logger = logger;
        }

        /// <summary>
        /// Handles the DeleteStandingOrderCommand to delete a standing order.
        /// </summary>
        /// <param name="request">Command containing the StandingOrderId to delete.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>ServiceResponse indicating success or failure of the deletion.</returns>
        public async Task<ServiceResponse<bool>> Handle(DeleteStandingOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate input
                if (string.IsNullOrEmpty(request.Id))
                {
                    string errorMessage = "StandingOrderId is required to delete a standing order.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.StandingOrder, LogAction.StandingOrder, LogLevelInfo.Error, null);
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return422(errorMessage);
                }

                // Find the standing order by ID
                var standingOrder = await _standingOrderRepository.FindAsync(request.Id);

                if (standingOrder == null)
                {
                    string errorMessage = $"Standing order with ID {request.Id} not found.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.StandingOrder, LogAction.StandingOrder, LogLevelInfo.Warning, null);
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                // Mark the standing order as deleted
                standingOrder.IsDeleted = true;

                // Commit the transaction
                await _uow.SaveAsync();

                // Log success
                string successMessage = $"Standing order with ID {request.Id} successfully deleted.";
                await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.StandingOrder, LogLevelInfo.Information);
                _logger.LogInformation(successMessage);

                return ServiceResponse<bool>.ReturnResultWith200(true, successMessage);
            }
            catch (Exception ex)
            {
                // Log the error
                string errorMessage = $"Error occurred while deleting standing order with ID {request.Id}.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.StandingOrder, LogLevelInfo.Error);
                _logger.LogError(ex, errorMessage);

                return ServiceResponse<bool>.Return500(ex, errorMessage);
            }
        }
    }

}