using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Repository.WithdrawalNotificationP;
using CBS.TransactionManagement.Commands.WithdrawalNotificationP;
using CBS.TransactionManagement.Data.Dto.WithdrawalNotificationP;

namespace CBS.TransactionManagement.Handlers.WithdrawalNotificationP
{

    public class UpdateWithdrawalNotificationCommandHandler : IRequestHandler<UpdateWithdrawalNotificationCommand, ServiceResponse<WithdrawalNotificationDto>>
    {
        private readonly IWithdrawalNotificationRepository _WithdrawalNotificationRepository; // Repository for accessing WithdrawalNotification data.
        private readonly ILogger<UpdateWithdrawalNotificationCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateWithdrawalNotificationCommandHandler.
        /// </summary>
        /// <param name="WithdrawalNotificationRepository">Repository for WithdrawalNotification data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="uow">Unit of work for transaction management (optional).</param>
        public UpdateWithdrawalNotificationCommandHandler(
            IWithdrawalNotificationRepository WithdrawalNotificationRepository,
            ILogger<UpdateWithdrawalNotificationCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<TransactionContext> uow = null)
        {
            // Assign provided dependencies to local variables.
            _WithdrawalNotificationRepository = WithdrawalNotificationRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateWithdrawalNotificationCommand to update a WithdrawalNotification.
        /// </summary>
        /// <param name="request">The UpdateWithdrawalNotificationCommand containing updated WithdrawalNotification data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<WithdrawalNotificationDto>> Handle(UpdateWithdrawalNotificationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the existing WithdrawalNotification entity based on the provided ID
                var existingWithdrawalNotification = await _WithdrawalNotificationRepository.FindAsync(request.Id);

                // If the entity with the specified ID does not exist, return a 404 Not Found response
                if (existingWithdrawalNotification == null)
                {
                    return ServiceResponse<WithdrawalNotificationDto>.Return404($"{request.Id} was not found to be updated.");
                }

                // Use AutoMapper to map properties from the request to the existingWithdrawalNotification
                _mapper.Map(request, existingWithdrawalNotification);

                // Set the modified date
                existingWithdrawalNotification.ModifiedDate = DateTime.Now;

                // Use the repository to update the existing WithdrawalNotification entity
                _WithdrawalNotificationRepository.Update(existingWithdrawalNotification);

                // Save changes
                await _uow.SaveAsync();

                // Prepare the response and return a successful response with 200 Status code
                var response = ServiceResponse<WithdrawalNotificationDto>.ReturnResultWith200(_mapper.Map<WithdrawalNotificationDto>(existingWithdrawalNotification));
                _logger.LogInformation($"WithdrawalNotification {request.Id} was successfully updated.");
                return response;
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating WithdrawalNotification: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<WithdrawalNotificationDto>.Return500(e);
            }
        }
    }

}
