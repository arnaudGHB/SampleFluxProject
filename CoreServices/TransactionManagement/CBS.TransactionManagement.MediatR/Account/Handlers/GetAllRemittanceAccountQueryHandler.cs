using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Queries;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using System.Linq;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the retrieval of all remittance accounts based on the specified query.
    /// </summary>
    public class GetAllRemittanceAccountQueryHandler : IRequestHandler<GetAllRemittanceAccountQuery, ServiceResponse<List<AccountDto>>>
    {
        private readonly IAccountRepository _accountRepository; // Repository for accessing accounts data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllRemittanceAccountQueryHandler> _logger; // Logger for logging actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetAllRemittanceAccountQueryHandler.
        /// </summary>
        /// <param name="accountRepository">Repository for accounts data access.</param>
        /// <param name="userInfoToken">User information for logging and auditing.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging actions and errors.</param>
        public GetAllRemittanceAccountQueryHandler(
            IAccountRepository accountRepository,
            UserInfoToken userInfoToken,
            IMapper mapper,
            ILogger<GetAllRemittanceAccountQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _accountRepository = accountRepository;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllRemittanceAccountQuery to retrieve remittance accounts based on the provided query parameters.
        /// </summary>
        /// <param name="request">The GetAllRemittanceAccountQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<AccountDto>>> Handle(GetAllRemittanceAccountQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Get valid RemittanceTypes as a list of string values
                var validRemittanceTypes = Enum.GetNames(typeof(RemittanceTypes)).ToList();

                // Fetch and filter accounts directly from the database
                var accounts = await _accountRepository.All.AsNoTracking()
                    .Where(account => !account.IsDeleted && account.BranchId==request.BranchId && validRemittanceTypes.Contains(account.AccountType))
                    .ToListAsync();

                //// Log successful retrieval
                //await APICallHelper.AuditLogger(
                //    _userInfoToken.Email,
                //    LogAction.Read.ToString(),
                //    request,
                //    "All remittance accounts returned successfully",
                //    LogLevelInfo.Information.ToString(),
                //    200,
                //    _userInfoToken.Token);

                // Map the filtered accounts to DTOs and return
                return ServiceResponse<List<AccountDto>>.ReturnResultWith200(_mapper.Map<List<AccountDto>>(accounts));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all remittance accounts: {e.Message}");
                await APICallHelper.AuditLogger(
                    _userInfoToken.Email,
                    LogAction.Read.ToString(),
                    request,
                    $"Failed to get all remittance accounts: {e.Message}",
                    LogLevelInfo.Error.ToString(),
                    500,
                    _userInfoToken.Token);

                return ServiceResponse<List<AccountDto>>.Return500(e, "Failed to get all remittance accounts");
            }
        }
    }
}
