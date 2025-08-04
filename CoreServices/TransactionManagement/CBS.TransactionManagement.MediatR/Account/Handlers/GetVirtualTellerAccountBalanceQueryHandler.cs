using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Queries;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Data;
using DocumentFormat.OpenXml.Vml.Office;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Account based on its unique identifier.
    /// </summary>
    public class GetVirtualTellerAccountBalanceQueryHandler : IRequestHandler<GetVirtualTellerAccountBalanceQuery, ServiceResponse<AccountDto>>
    {
        private readonly IAccountRepository _AccountRepository; // Repository for accessing Account data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAccountQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;
        private readonly IDailyTellerRepository _dailyTellerRepository; // Repository for accessing Account data.
        private readonly ITellerOperationRepository _tellerOperationRepository; // Repository for accessing Account data.

        /// <summary>
        /// Constructor for initializing the GetTransactionQueryHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Account data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetVirtualTellerAccountBalanceQueryHandler(
            IAccountRepository AccountRepository,
            UserInfoToken UserInfoToken,
            IMapper mapper,
            ILogger<GetAccountQueryHandler> logger,
            IDailyTellerRepository dailyTellerRepository = null,
            ITellerOperationRepository tellerOperationRepository = null)
        {
            _AccountRepository = AccountRepository;
            _userInfoToken = UserInfoToken;
            _mapper = mapper;
            _logger = logger;
            _dailyTellerRepository = dailyTellerRepository;
            _tellerOperationRepository = tellerOperationRepository;
        }

        /// <summary>
        /// Handles the GetAccountQuery to retrieve a specific Account.
        /// </summary>
        /// <param name="request">The GetAccountQuery containing Account ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<AccountDto>> Handle(GetVirtualTellerAccountBalanceQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;

            try
            {
                var entity = await _AccountRepository.RetrieveMobileMoneyTellerAccount(request.BrnachId, request.QueryParameter);
                var AccountDto = _mapper.Map<AccountDto>(entity);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "Customer account returned successfully", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                return ServiceResponse<AccountDto>.ReturnResultWith200(AccountDto);
            }
            catch (Exception e)
            {
                // Log the error and return a 500 Internal Server Error response with the error message
                errorMessage = $"Error occurred while getting Account: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<AccountDto>.Return500(e);
            }
        }
    }

}
