using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Queries;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Accounts based on the GetAllAccountShortQuery.
    /// </summary>
    public class GetAllAccountShortQueryHandler : IRequestHandler<GetAllAccountShortQuery, ServiceResponse<List<AccountShortDto>>>
    {
        private readonly IAccountRepository _AccountRepository; // Repository for accessing Accounts data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllAccountShortQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;
        /// <summary>
        /// Constructor for initializing the GetAllAccountShortQueryHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Accounts data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllAccountShortQueryHandler(
            IAccountRepository AccountRepository,
            UserInfoToken UserInfoToken,
            IMapper mapper, ILogger<GetAllAccountShortQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _AccountRepository = AccountRepository;
            _userInfoToken = UserInfoToken;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllAccountShortQuery to retrieve all Accounts.
        /// </summary>
        /// <param name="request">The GetAllAccountShortQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<AccountShortDto>>> Handle(GetAllAccountShortQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all Accounts entities from the repository
                var entities = await _AccountRepository.All.ToListAsync();

                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "All accounts returned successfully", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                return ServiceResponse<List<AccountShortDto>>.ReturnResultWith200(_mapper.Map<List<AccountShortDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all Accounts: {e.Message}");
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, $"Failed to get all Accounts: {e.Message}", LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<List<AccountShortDto>>.Return500(e, "Failed to get all Accounts");
            }
        }
    }
}
