using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to delete a AccountPolicyName based on DeleteAccountPolicyNameCommand.
    /// </summary>
    public class DeleteCashMovementTrackingConfigurationCommanddHandler : IRequestHandler<DeleteCashMovementTrackingConfigurationCommand, ServiceResponse<bool>>
    {
        private readonly ICashMovementTrackingConfigurationRepository _cashMovementTrackerRepository; // Repository for accessing AccountPolicyName data.
        private readonly ILogger<DeleteCashMovementTrackingConfigurationCommanddHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the DeleteAccountPolicyNameCommandHandler.
        /// </summary>
        /// <param name="AccountPolicyNameRepository">Repository for AccountPolicyName data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteCashMovementTrackingConfigurationCommanddHandler(
UserInfoToken userInfoToken,            ICashMovementTrackingConfigurationRepository cashMovementTrackerRepository, IMapper mapper,
            ILogger<DeleteCashMovementTrackingConfigurationCommanddHandler> logger
, IUnitOfWork<POSContext> uow)
        {
            _userInfoToken = userInfoToken;
            _cashMovementTrackerRepository = cashMovementTrackerRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteAccountPolicyNameCommand to delete a AccountPolicyName.
        /// </summary>
        /// <param name="request">The DeleteAccountPolicyNameCommand containing AccountPolicyName ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteCashMovementTrackingConfigurationCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the AccountPolicyName entity with the specified ID exists
                var existingAccountPolicyName = await _cashMovementTrackerRepository.FindAsync(request.Id);
                if (existingAccountPolicyName == null)
                {
                    errorMessage = $"CashMovementTracker with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                if (_userInfoToken.IsHeadOffice && existingAccountPolicyName.CreatedBy != _userInfoToken.Id)
                {
                    existingAccountPolicyName.IsDeleted = true;
                    _cashMovementTrackerRepository.Update(existingAccountPolicyName);
                    await _uow.SaveAsync();
                    // Prepare the response and return a successful response with 200 status code
                    
                    _logger.LogInformation($"Cash Movement Tracker {request.Id} was successfully deleted.");
                    return ServiceResponse<bool>.ReturnResultWith200(true);
                }
                else
                {
                      errorMessage = $"{_userInfoToken.FullName} is not authourized to perform this operation kindly contact system administrator.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return401(errorMessage);
                }
          
                 
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting Cash Movement Tracker: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }
}