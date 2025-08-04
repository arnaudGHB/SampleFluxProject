using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Repository.WithdrawalNotificationP;
using CBS.TransactionManagement.Commands.WithdrawalNotificationP;
using CBS.TransactionManagement.Data.Dto.WithdrawalNotificationP;

namespace CBS.TransactionManagement.Handlers.WithdrawalNotificationValidationP
{

    public class ValidationWithdrawalNotificationCommandHandler : IRequestHandler<ValidationWithdrawalNotificationCommand, ServiceResponse<WithdrawalNotificationDto>>
    {
        private readonly IWithdrawalNotificationRepository _WithdrawalNotificationRepository; // Repository for accessing WithdrawalNotificationValidation data.
        private readonly ILogger<ValidationWithdrawalNotificationCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow; // Unit of work for transaction management.
        private readonly UserInfoToken _userInfoToken; // User info token for accessing user details.

        /// <summary>
        /// Constructor for initializing the ValidationWithdrawalNotificationCommandHandler.
        /// </summary>
        /// <param name="WithdrawalNotificationValidationRepository">Repository for WithdrawalNotificationValidation data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="uow">Unit of work for transaction management (optional).</param>
        /// <param name="userInfoToken">User info token for accessing user details (optional).</param>
        public ValidationWithdrawalNotificationCommandHandler(
            IWithdrawalNotificationRepository WithdrawalNotificationValidationRepository,
            ILogger<ValidationWithdrawalNotificationCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<TransactionContext> uow = null,
            UserInfoToken userInfoToken = null)
        {
            // Assign provided dependencies to local variables.
            _WithdrawalNotificationRepository = WithdrawalNotificationValidationRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the ValidationWithdrawalNotificationCommand to update a WithdrawalNotificationValidation.
        /// </summary>
        /// <param name="request">The ValidationWithdrawalNotificationCommand containing updated WithdrawalNotificationValidation data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<WithdrawalNotificationDto>> Handle(ValidationWithdrawalNotificationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the existing WithdrawalNotificationValidation entity based on the provided ID
                var existingWithdrawalNotificationValidation = await _WithdrawalNotificationRepository.FindAsync(request.Id);

                // If the entity with the specified ID does not exist, return a 404 Not Found response
                if (existingWithdrawalNotificationValidation == null)
                {
                    return ServiceResponse<WithdrawalNotificationDto>.Return404($"{request.Id} was not found to be updated.");
                }

                // Use AutoMapper to map properties from the request to the existingWithdrawalNotificationValidation
                _mapper.Map(request, existingWithdrawalNotificationValidation);

                // If the approval status is "Approved", set additional approval properties
                if (request.ApprovalStatus == Status.Approved.ToString())
                {
                    existingWithdrawalNotificationValidation.ApprovalStatus = request.ApprovalStatus;
                    existingWithdrawalNotificationValidation.ApprovalDate = BaseUtilities.UtcNowToDoualaTime(); // Set approval date to the current date and time
                    existingWithdrawalNotificationValidation.ApprovedByName = _userInfoToken.FullName; // Set the name of the approver
                    existingWithdrawalNotificationValidation.ApprovedComment = request.ApprovalComment; // Set the approval comment
                }

                // Use the repository to update the existing WithdrawalNotificationValidation entity
                _WithdrawalNotificationRepository.Update(existingWithdrawalNotificationValidation);

                // Save changes
                await _uow.SaveAsync();

                // Prepare the response and return a successful response with 200 Status code
                var response = ServiceResponse<WithdrawalNotificationDto>.ReturnResultWith200(_mapper.Map<WithdrawalNotificationDto>(existingWithdrawalNotificationValidation));
                _logger.LogInformation($"WithdrawalNotificationValidation {request.Id} was successfully updated.");
                return response;
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating WithdrawalNotificationValidation: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<WithdrawalNotificationDto>.Return500(e);
            }
        }
    }

}
