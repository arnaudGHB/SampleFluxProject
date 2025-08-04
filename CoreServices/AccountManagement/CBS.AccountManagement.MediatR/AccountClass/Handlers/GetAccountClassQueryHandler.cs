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
    /// Handles the request to retrieve a specific AccountClass based on its unique identifier.
    /// </summary>
    public class GetAccountClassQueryHandler : IRequestHandler<GetAccountClassQuery, ServiceResponse<AccountClassDto>>
    {
        private readonly IAccountClassRepository _AccountClassRepository; // Repository for accessing AccountClass data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAccountClassQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAccountClassQueryHandler.
        /// </summary>
        /// <param name="AccountClassRepository">Repository for AccountClass data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAccountClassQueryHandler(
            IAccountClassRepository AccountClassRepository,
            IMapper mapper,
            ILogger<GetAccountClassQueryHandler> logger)
        {
            _AccountClassRepository = AccountClassRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAccountClassQuery to retrieve a specific AccountClass.
        /// </summary>
        /// <param name="request">The GetAccountClassQuery containing AccountClass ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<AccountClassDto>> Handle(GetAccountClassQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = "";
            try
            {
                // Retrieve the AccountClass entity with the specified ID from the repository
                var entity = await _AccountClassRepository.FindAsync(request.Id);
                if (entity != null)
                {
                    if (entity.IsDeleted)
                    {
                        string message = "AccountClass has been deleted.";
                        // If the AccountCategory entity was not found, log the error and return a 404 Not Found response
                        _logger.LogError(message);
                        return ServiceResponse<AccountClassDto>.Return404(message);
                    }
                    else
                    {
                        // Map the AccountClass entity to AccountClassDto and return it with a success response
                        var AccountClassDto = _mapper.Map<AccountClassDto>(entity);
                        return ServiceResponse<AccountClassDto>.ReturnResultWith200(AccountClassDto);
                    }
                }
                else
                {
                    // If the AccountClass entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("AccountClass not found.");
                    return ServiceResponse<AccountClassDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting AccountClass: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<AccountClassDto>.Return500(e);
            }
        }
    }
}