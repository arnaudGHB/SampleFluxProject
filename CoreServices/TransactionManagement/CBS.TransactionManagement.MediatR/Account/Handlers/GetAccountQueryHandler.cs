using AutoMapper;
using MediatR;

using CBS.TransactionManagement.Queries;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Account based on its unique identifier.
    /// </summary>
    public class GetAccountQueryHandler : IRequestHandler<GetAccountQuery, ServiceResponse<AccountDto>>
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
        public GetAccountQueryHandler(
            IAccountRepository AccountRepository,
            IMapper mapper,
            UserInfoToken userInfoToken,
            ILogger<GetAccountQueryHandler> logger)
        {
            _AccountRepository = AccountRepository;
            _userInfoToken = userInfoToken;
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
                var entity = await _AccountRepository.AllIncluding(a => a.Product.CashDepositParameters, b => b.Product.WithdrawalParameters, t => t.Product.TransferParameters).FirstOrDefaultAsync(x => x.Id == request.Id);
                //var entity = await _AccountRepository.FindAsync(request.Id);
                if (entity != null)
                {
                    // Map the Account entity to AccountDto and return it with a success response
                    var AccountDto = _mapper.Map<AccountDto>(entity);
                    //var serialisedAccountDto = Newtonsoft.Json.JsonConvert.SerializeObject(entity);
                    //_logger.LogError($"Account found: {serialisedAccountDto} ");
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "Customer account returned successfully", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                    return ServiceResponse<AccountDto>.ReturnResultWith200(AccountDto);
                }
                else
                {
                    // If the Account entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("Account not found.");
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "Account not found.", LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<AccountDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Account: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.Log(LogLevel.Error, e, errorMessage, null);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<AccountDto>.Return500(e, errorMessage);
            }
        }
    }

}
