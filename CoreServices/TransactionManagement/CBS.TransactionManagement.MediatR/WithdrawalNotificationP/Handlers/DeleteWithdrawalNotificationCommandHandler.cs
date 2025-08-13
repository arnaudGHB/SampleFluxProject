using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Repository.WithdrawalNotificationP;
using CBS.TransactionManagement.Commands.WithdrawalNotificationP;

namespace CBS.TransactionManagement.Handlers.WithdrawalNotificationP
{
    public class DeleteWithdrawalNotificationCommandHandler : IRequestHandler<DeleteWithdrawalNotificationCommand, ServiceResponse<bool>>
    {
        private readonly IWithdrawalNotificationRepository _WithdrawalNotificationRepository; // Repository for accessing WithdrawalNotification data.
        private readonly ILogger<DeleteWithdrawalNotificationCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteWithdrawalNotificationCommandHandler.
        /// </summary>
        /// <param name="WithdrawalNotificationRepository">Repository for WithdrawalNotification data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="uow">Unit of work for managing transactions.</param>
        public DeleteWithdrawalNotificationCommandHandler(
            IWithdrawalNotificationRepository WithdrawalNotificationRepository,
            IMapper mapper,
            ILogger<DeleteWithdrawalNotificationCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow)
        {
            _WithdrawalNotificationRepository = WithdrawalNotificationRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteWithdrawalNotificationCommand to delete a WithdrawalNotification.
        /// </summary>
        /// <param name="request">The DeleteWithdrawalNotificationCommand containing WithdrawalNotification ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteWithdrawalNotificationCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the WithdrawalNotification entity with the specified ID exists
                var existingWithdrawalNotification = await _WithdrawalNotificationRepository.FindAsync(request.Id);
                if (existingWithdrawalNotification == null)
                {
                    errorMessage = $"WithdrawalNotification with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                // Mark the WithdrawalNotification as deleted
                existingWithdrawalNotification.IsDeleted = true;
                _WithdrawalNotificationRepository.Update(existingWithdrawalNotification);

                // Save changes to the database
                await _uow.SaveAsync();

                // Return success response
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                errorMessage = $"Error occurred while deleting WithdrawalNotification: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
