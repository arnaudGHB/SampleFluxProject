using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Handlers;
using CBS.TransactionManagement.MediatR.GAV.Query;

namespace CBS.TransactionManagement.MediatR.GAV.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Account based on its unique identifier. 
    /// </summary>
    public class GetMemberAccountBalanceTTPQueryHandler : IRequestHandler<GetMemberAccountBalanceTTPQuery, ServiceResponse<AccountBalanceThirdPartyDto>>
    {
        private readonly IAccountRepository _AccountRepository; // Repository for accessing Account data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAccountQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;
        /// <summary>
        /// Constructor for initializing the GetTransactionQueryHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Account data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetMemberAccountBalanceTTPQueryHandler(
            IAccountRepository AccountRepository,
            UserInfoToken UserInfoToken,
            IMapper mapper,
            ILogger<GetAccountQueryHandler> logger)
        {
            _AccountRepository = AccountRepository;
            _userInfoToken = UserInfoToken;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAccountQuery to retrieve a specific Account.
        /// </summary>
        /// <param name="request">The GetAccountQuery containing Account ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<AccountBalanceThirdPartyDto>> Handle(GetMemberAccountBalanceTTPQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the Account entity with the specified ID from the repository
                var entity = await _AccountRepository.FindBy(x => x.AccountNumber == request.AccountNumber).FirstOrDefaultAsync();
                //var entity = await _AccountRepository.FindAsync(request.Id);
                if (entity != null)
                {

                    // Map the Account entity to AccountBalanceThirdPartyDto and return it with a success response
                    var AccountBalanceThirdPartyDto = _mapper.Map<AccountBalanceThirdPartyDto>(entity);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "Customer account returned successfully", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                    return ServiceResponse<AccountBalanceThirdPartyDto>.ReturnResultWith200(AccountBalanceThirdPartyDto);
                }
                else
                {
                    // If the Account entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("Account not found.");
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "Account not found.", LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<AccountBalanceThirdPartyDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Account: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<AccountBalanceThirdPartyDto>.Return500(e);
            }
        }
    }

}
