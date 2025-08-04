using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to update a Account based on UpdateAccountCommand.
    /// </summary>
    public class UpdateAccountCommandHandler : IRequestHandler<UpdateAccountCommand, ServiceResponse<AccountDto>>
    {
        private readonly IAccountRepository _AccountRepository; // Repository for accessing Account data.
        private readonly ILogger<UpdateAccountCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userTokenInfo;
        /// <summary>
        /// Constructor for initializing the UpdateAccountCommandHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Account data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateAccountCommandHandler(
            IAccountRepository AccountRepository,
            ILogger<UpdateAccountCommandHandler> logger,
            IMapper mapper, UserInfoToken userTokenInfo,
            IUnitOfWork<POSContext> uow = null)
        {
            _AccountRepository = AccountRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow; 
            _userTokenInfo = userTokenInfo;
        }

        /// <summary>
        /// Handles the UpdateAccountCommand to update a Account.
        /// </summary>
        /// <param name="request">The UpdateAccountCommand containing updated Account data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<AccountDto>> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the Account entity to be updated from the repository
                var existingAccount = await _AccountRepository.FindAsync(request.Id);
                // Check if the Account entity exists
                if (existingAccount != null)
                {
                    // Update Account entity properties with values from the request
                    existingAccount = _mapper.Map(request, existingAccount);
                    // Use the repository to update the existing Account entity
                    _AccountRepository.Update(existingAccount);
                    await _uow.SaveAsync();
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<AccountDto>.ReturnResultWith200(_mapper.Map<AccountDto>(existingAccount));
                    _logger.LogInformation($"Account {request.Id} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the Account entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<AccountDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating Account: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<AccountDto>.Return500(e);
            }
        }
    }
}