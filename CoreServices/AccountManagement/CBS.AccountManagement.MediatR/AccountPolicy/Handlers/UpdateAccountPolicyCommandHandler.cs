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
    /// Handles the command to update a AccountPolicyName based on UpdateAccountPolicyNameCommand.
    /// </summary>
    public class UpdateAccountPolicyCommandHandler : IRequestHandler<UpdateAccountPolicyCommand, ServiceResponse<CashMovementTrackerDto>>
    {
        private readonly IAccountPolicyRepository _AccountPolicyNameRepository; // Repository for accessing AccountPolicyName data.
        private readonly ILogger<UpdateAccountPolicyCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateAccountPolicyNameCommandHandler.
        /// </summary>
        /// <param name="AccountPolicyNameRepository">Repository for AccountPolicyName data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateAccountPolicyCommandHandler(
            IAccountPolicyRepository AccountPolicyNameRepository,
            ILogger<UpdateAccountPolicyCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<POSContext> uow = null)
        {
            _AccountPolicyNameRepository = AccountPolicyNameRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateAccountPolicyNameCommand to update a AccountPolicyName.
        /// </summary>
        /// <param name="request">The UpdateAccountPolicyNameCommand containing updated AccountPolicyName data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CashMovementTrackerDto>> Handle(UpdateAccountPolicyCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the AccountPolicyName entity to be updated from the repository
                var existingAccountPolicyName = await _AccountPolicyNameRepository.FindAsync(request.Id);

                // Check if the AccountPolicyName entity exists
                if (existingAccountPolicyName != null)
                {
                    // Update AccountPolicyName entity properties with values from the request

                    existingAccountPolicyName= _mapper.Map(request, existingAccountPolicyName);
                    // Use the repository to update the existing AccountPolicyName entity
                    _AccountPolicyNameRepository.Update(existingAccountPolicyName);
                    await _uow.SaveAsync();
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<CashMovementTrackerDto>.ReturnResultWith200(_mapper.Map<CashMovementTrackerDto>(existingAccountPolicyName));
                    _logger.LogInformation($"AccountPolicyName {request.Id} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the AccountPolicyName entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<CashMovementTrackerDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating AccountPolicyName: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<CashMovementTrackerDto>.Return500(e);
            }
        }
    }
}