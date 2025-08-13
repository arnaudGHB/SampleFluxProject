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
    /// Handles the retrieval of all Accounts based on the GetAllAccountStatisticsQuery.
    /// </summary>
    public class GetAllAccountStatisticsQueryHandler : IRequestHandler<GetAllAccountStatisticsQuery, ServiceResponse<AccountStatisticsDto>>
    {
        private readonly IAccountRepository _AccountRepository; // Repository for accessing Accounts data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllAccountStatisticsQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;
        /// <summary>
        /// Constructor for initializing the GetAllAccountStatisticsQueryHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Accounts data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllAccountStatisticsQueryHandler(
            IAccountRepository AccountRepository,
            UserInfoToken userInfoToken,
            IMapper mapper, ILogger<GetAllAccountStatisticsQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _AccountRepository = AccountRepository;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _logger = logger;
        }
        /// <summary>
        /// Handles the GetAllAccountStatisticsQuery to retrieve account statistics by branch or all accounts, excluding teller accounts and deleted accounts.
        /// </summary>
        /// <param name="request">The GetAllAccountStatisticsQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>

        public async Task<ServiceResponse<AccountStatisticsDto>> Handle(GetAllAccountStatisticsQuery request, CancellationToken cancellationToken)
        {
            string message;
            try
            {
                // Base query with branch filter if needed
                var query = _AccountRepository.All
                    .Where(a => !a.IsTellerAccount && !a.IsDeleted);

                if (!string.IsNullOrEmpty(request.QueryParameter) && request.QueryParameter.ToLower() == "bybranch" && !string.IsNullOrEmpty(request.BranchId))
                {
                    query = query.Where(a => a.BranchId == request.BranchId);
                    message = $"Filtering accounts by branch ID: {request.BranchId} by [{_userInfoToken.FullName} {_userInfoToken.BranchName}]";
                }
                else
                {
                    message = $"Retrieving statistics for all accounts by [{_userInfoToken.FullName} {_userInfoToken.BranchName}].";
                }
                _logger.LogInformation(message);

                // Aggregate main statistics in a single query
                var overallStatistics = await query
                    .GroupBy(a => 1)
                    .Select(group => new
                    {
                        TotalNumberOfAccounts = group.Count(),
                        TotalBalance = group.Sum(a => a.Balance),
                        TotalBlockedAmount = group.Sum(a => a.BlockedAmount),
                        TotalActiveAccounts = group.Count(a => a.Status == AccountStatus.Active.ToString()),
                        TotalInactiveAccounts = group.Count(a => a.Status == AccountStatus.Inactive.ToString()),
                        TotalBranches = group.Select(a => a.BranchId).Distinct().Count(),
                        TotalMembers = group.Select(a => a.CustomerId).Distinct().Count()
                    })
                    .FirstOrDefaultAsync(cancellationToken);

                // Group by AccountType for per-account statistics
                var statisticPerAccounts = await query
                    .GroupBy(a => a.AccountType)
                    .Select(group => new StatisticPerAccountDto
                    {
                        AccountType = group.Key,
                        NumberOfAccounts = group.Count(),
                        NumberOfActiveAccounts = group.Count(a => a.Status == AccountStatus.Active.ToString()),
                        NumberOfInActiveAccounts = group.Count(a => a.Status == AccountStatus.Inactive.ToString()),
                        Balance = group.Sum(a => a.Balance),
                        BlockedAmount = group.Sum(a => a.BlockedAmount),
                        BalanceWithoutBlocked = group.Sum(a => a.Balance - a.BlockedAmount)
                    })
                    .ToListAsync(cancellationToken);

                // Map the statistics to AccountStatisticsDto
                var accountStatisticsDto = new AccountStatisticsDto
                {
                    TotalNumberOfAccounts = overallStatistics.TotalNumberOfAccounts,
                    TotalBranches = overallStatistics.TotalBranches,
                    TotalMembers = overallStatistics.TotalMembers,
                    TotalNumberOfActiveAccounts = overallStatistics.TotalActiveAccounts,
                    TotalNumberOfInActiveAccounts = overallStatistics.TotalInactiveAccounts,
                    TotalBalance = overallStatistics.TotalBalance,
                    TotalBalanceWithoutBlocked = overallStatistics.TotalBalance - overallStatistics.TotalBlockedAmount,
                    TotalBlockedAmount = overallStatistics.TotalBlockedAmount,
                    StatisticPerAccounts = statisticPerAccounts
                };

                message = $"Account statistics generated successfully.";
                _logger.LogInformation(message);
                await BaseUtilities.LogAndAuditAsync(message, accountStatisticsDto, HttpStatusCodeEnum.OK, LogAction.AccountStatistics, LogLevelInfo.Information);

                return ServiceResponse<AccountStatisticsDto>.ReturnResultWith200(accountStatisticsDto);
            }
            catch (Exception e)
            {
                message = $"Failed to retrieve account statistics due to an error: {e.Message}";
                _logger.LogError(message);
                await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.InternalServerError, LogAction.AccountStatistics, LogLevelInfo.Error);
                return ServiceResponse<AccountStatisticsDto>.Return500(message);
            }
        }



        public async Task<ServiceResponse<AccountStatisticsDto>> Handlexxx(GetAllAccountStatisticsQuery request, CancellationToken cancellationToken)
        {
            string message;
            try
            {
                // Define the base query for non-teller, active, non-deleted accounts
                var query = _AccountRepository.All
                    .Where(a => !a.IsTellerAccount && !a.IsDeleted); // Exclude teller and deleted accounts

                // Apply branch filter if QueryParameter is "bybranch"
                if (!string.IsNullOrEmpty(request.QueryParameter) && request.QueryParameter.ToLower() == "bybranch" && !string.IsNullOrEmpty(request.BranchId))
                {
                    query = query.Where(a => a.BranchId == request.BranchId);
                    message = $"Filtering accounts by branch ID: {request.BranchId} by [{_userInfoToken.FullName} {_userInfoToken.BranchName}]";
                }
                else
                {
                    message = $"Retrieving statistics for all accounts by [{_userInfoToken.FullName} {_userInfoToken.BranchName}].";
                }
                _logger.LogInformation(message);

                // Retrieve the accounts based on the query
                var accounts = await query.ToListAsync(cancellationToken);
                message = $"Retrieved {accounts.Count} accounts after filtering by [{_userInfoToken.FullName} {_userInfoToken.BranchName}].";
                _logger.LogInformation(message);

                // Calculate the main statistics for the filtered accounts
                var totalNumberOfAccounts = accounts.Count;
                var totalBranches = accounts.Select(a => a.BranchId).Distinct().Count();
                var totalMembers = accounts.Select(a => a.CustomerId).Distinct().Count();
                var totalNumberOfActiveAccounts = accounts.Count(a => a.Status == AccountStatus.Active.ToString());
                var totalNumberOfInactiveAccounts = accounts.Count(a => a.Status == AccountStatus.Inactive.ToString());
                var totalBalance = accounts.Sum(a => a.Balance);
                var totalBlockedAmount = accounts.Sum(a => a.BlockedAmount);  // Summing blocked amounts
                var totalBalanceWithoutBlocked = accounts.Sum(a => a.Balance - a.BlockedAmount);  // Total balance excluding blocked amount

                // Prepare the message for the main statistics
                message = $"Total accounts: {totalNumberOfAccounts}, Total branches: {totalBranches}, Total members: {totalMembers}, " +
                          $"Active accounts: {totalNumberOfActiveAccounts}, Inactive accounts: {totalNumberOfInactiveAccounts}, " +
                          $"Total balance: {totalBalance}, Total blocked amount: {totalBlockedAmount}, " +
                          $"Total balance without blocked: {totalBalanceWithoutBlocked} by [{_userInfoToken.FullName} {_userInfoToken.BranchName}].";

                // Log the main statistics
                _logger.LogInformation(message);

                // Group by AccountType to calculate statistics per account type
                var statisticPerAccounts = accounts
                    .GroupBy(a => a.AccountType)
                    .Select(group => new StatisticPerAccountDto
                    {
                        AccountType = group.Key,
                        NumberOfAccounts = group.Count(),
                        NumberOfActiveAccounts = group.Count(a => a.Status == AccountStatus.Active.ToString()),
                        NumberOfInActiveAccounts = group.Count(a => a.Status == AccountStatus.Inactive.ToString()),
                        Balance = group.Sum(a => a.Balance),
                        BlockedAmount = group.Sum(a => a.BlockedAmount),
                        BalanceWithoutBlocked = group.Sum(a => a.Balance - a.BlockedAmount)  // Balance excluding blocked amounts
                    })
                    .ToList();

                // Create the AccountStatisticsDto instance with calculated statistics
                var accountStatisticsDto = new AccountStatisticsDto
                {
                    TotalNumberOfAccounts = totalNumberOfAccounts,
                    TotalBranches = totalBranches,
                    TotalMembers = totalMembers,
                    TotalNumberOfActiveAccounts = totalNumberOfActiveAccounts,
                    TotalNumberOfInActiveAccounts = totalNumberOfInactiveAccounts,
                    TotalBalance = totalBalance,
                    TotalBalanceWithoutBlocked = totalBalanceWithoutBlocked,  // Set the balance excluding blocked amounts
                    TotalBlockedAmount = totalBlockedAmount,  // Set the total blocked amount
                    StatisticPerAccounts = statisticPerAccounts
                };

                // Log the successful generation of account statistics
                message = $"Account statistics generated successfully: {message}";
                _logger.LogInformation(message);
                await BaseUtilities.LogAndAuditAsync(message, accountStatisticsDto, HttpStatusCodeEnum.OK, LogAction.AccountStatistics, LogLevelInfo.Information);

                return ServiceResponse<AccountStatisticsDto>.ReturnResultWith200(accountStatisticsDto);
            }
            catch (Exception e)
            {
                message = $"Failed to retrieve account statistics due to an error: {e.Message}";
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError(message);
                await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.InternalServerError, LogAction.AccountStatistics, LogLevelInfo.Error);
                return ServiceResponse<AccountStatisticsDto>.Return500(message);
            }
        }
    }
}
