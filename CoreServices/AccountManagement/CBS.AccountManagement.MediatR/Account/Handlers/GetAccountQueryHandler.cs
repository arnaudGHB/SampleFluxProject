using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Account based on its unique identifier.
    /// </summary>
    public class GetAccountQueryHandler : IRequestHandler<GetAccountQuery, ServiceResponse<AccountDto>>
    {
        private readonly IAccountRepository _AccountRepository; // Repository for accessing Account data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAccountQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAccountQueryHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Account data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAccountQueryHandler(
            IAccountRepository AccountRepository,
            IMapper mapper,
            ILogger<GetAccountQueryHandler> logger)
        {
            _AccountRepository = AccountRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAccountQuery to retrieve a specific Account.
        /// </summary>
        /// <param name="request">The GetAccountQuery containing Account ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<AccountDto>> Handle(GetAccountQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the Account entity with the specified ID from the repository
                var entity = await _AccountRepository.FindAsync(request.Id);
                if (entity != null)
                {
                    if (entity.IsDeleted)
                    {
                        string message = "Account has been deleted.";
                        // If the AccountCategory entity was not found, log the error and return a 404 Not Found response
                        _logger.LogError(message);
                        return ServiceResponse<AccountDto>.Return404(message);
                    }
                    else
                    {
                        // Map the Account entity to AccountDto and return it with a success response
                        var AccountDto = _mapper.Map<AccountDto>(entity);
                        return ServiceResponse<AccountDto>.ReturnResultWith200(AccountDto);

                    }
                }
                else
                {
                    // If the Account entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("Account not found.");
                    return ServiceResponse<AccountDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Account: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<AccountDto>.Return500(e);
            }
        }
    }
}