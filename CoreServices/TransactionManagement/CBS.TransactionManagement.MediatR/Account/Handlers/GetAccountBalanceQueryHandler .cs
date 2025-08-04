using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Queries;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using Microsoft.EntityFrameworkCore;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Account based on its unique identifier.
    /// </summary>
    public class GetAccountBalanceQueryHandler : IRequestHandler<GetAccountBalanceQuery, ServiceResponse<AccountBalanceDto>>
    {
        private readonly IAccountRepository _AccountRepository; // Repository for accessing Account data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly UserInfoToken _userInfoToken;
        private readonly ILogger<GetAccountBalanceQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetTransactionQueryHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Account data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAccountBalanceQueryHandler(
            IAccountRepository AccountRepository,
            UserInfoToken UserInfoToken,
            IMapper mapper,
            ILogger<GetAccountBalanceQueryHandler> logger)
        {
            _AccountRepository = AccountRepository;
            _userInfoToken = UserInfoToken;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAccountBalanceQuery to retrieve a specific Account.
        /// </summary>
        /// <param name="request">The GetAccountBalanceQuery containing Account ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<AccountBalanceDto>> Handle(GetAccountBalanceQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the Account entity with the specified ID from the repository
                var entity = await _AccountRepository.FindBy(a=> a.Id == request.Id).Include(p=>p.Product).FirstAsync();
                if (entity != null)
                {
                    // Map the Account entity to AccountBalanceDto and return it with a success response
                    var AccountBalanceDto = _mapper.Map<AccountBalanceDto>(entity);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "Customer account balance returned successfully", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                    return ServiceResponse<AccountBalanceDto>.ReturnResultWith200(AccountBalanceDto);
                }
                else
                {
                    // If the Account entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("Account not found.");
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "Account not found.", LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<AccountBalanceDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Account: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<AccountBalanceDto>.Return500(e);
            }
        }
    }

}
