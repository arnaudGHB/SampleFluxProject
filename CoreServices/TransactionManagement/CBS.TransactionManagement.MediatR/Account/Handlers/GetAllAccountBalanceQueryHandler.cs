using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Queries;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Data;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Accounts based on the GetAllAccountBalanceQuery.
    /// </summary>
    public class GetAllAccountBalanceQueryHandler : IRequestHandler<GetAllAccountsBalanceQuery, ServiceResponse<AllAccountsBalanceDto>>
    {
        private readonly IAccountRepository _AccountRepository; // Repository for accessing Accounts data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllAccountBalanceQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetAllAccountBalanceQueryHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Accounts data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllAccountBalanceQueryHandler(
            IAccountRepository AccountRepository,
            UserInfoToken userInfoToken,
            IMapper mapper, ILogger<GetAllAccountBalanceQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _AccountRepository = AccountRepository;
            _mapper = mapper;
            _userInfoToken = userInfoToken;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllAccountBalanceQuery to retrieve all Accounts.
        /// </summary>
        /// <param name="request">The GetAllAccountBalanceQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<AllAccountsBalanceDto>> Handle(GetAllAccountsBalanceQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Build the query using IQueryable and apply AsNoTracking for read-only optimization
                var query = _AccountRepository
                    .FindBy(a => a.CustomerId == request.CustomerId && !a.IsDeleted)
                    .AsNoTracking() // No tracking for read-only operations
                    .Include(p => p.Product);

                // Execute the query and calculate total balance
                var entities = await query.ToListAsync(cancellationToken);
                var accountDtos = _mapper.Map<List<AccountDto>>(entities);
                // Create DTO with total balance and account details
                var allAccountsBalanceDto = new AllAccountsBalanceDto
                {
                    TotalBalance = BaseUtilities.FormatCurrency(entities.Sum(x => x.Balance)),
                    accounts = accountDtos
                };

                // Log success and return the result
                await LogAudit(request, allAccountsBalanceDto.TotalBalance, "All Accounts balance returned successfully.", LogLevelInfo.Information, 200);

                return ServiceResponse<AllAccountsBalanceDto>.ReturnResultWith200(allAccountsBalanceDto);
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError(e, "Failed to get all accounts");
                await LogAudit(request, e.Message, "Failed to get all Accounts", LogLevelInfo.Error, 500);

                return ServiceResponse<AllAccountsBalanceDto>.Return500(e, "Failed to get all Accounts");
            }
        }

        // Simplified logging method to reduce duplication
        private async Task LogAudit(GetAllAccountsBalanceQuery request, string messageDetail, string message, LogLevelInfo logLevel, int statusCode)
        {
            await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, $"{message} {messageDetail}", logLevel.ToString(), statusCode, _userInfoToken.Token);
        }
    }
}
